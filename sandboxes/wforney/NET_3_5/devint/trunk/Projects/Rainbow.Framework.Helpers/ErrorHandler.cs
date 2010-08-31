namespace Rainbow.Framework
{
    using System;
    using System.Collections;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Exceptions;
    using Rainbow.Framework.Helpers;

    /// <summary>
    /// This class in combination with the Web.Config file handles all the Errors that are not caught programatically
    ///     99% of the time Errors will be caught by Rainbow's HttpUrlModule, this class will be called, errors will be 
    ///     logged depending on what was specified by the Web.Config file, after that the error cascades up and is caught
    ///     by the customErrors settings in Web.Config. Here you can specify errors and which pages to redirect to.
    ///     Visitors will be directed to dynamic aspx pages for General Errors and 404 Errors (Specified aspx page does not exist)
    ///     These pages are dynamic and will keep the theme you selected for your portal. It also makes use of Rainbow's
    ///     multi-language support. If these dynamic pages themselves have an error (e.g the Database has crashed 
    ///     so it can't retrieve the theme or translations, then there is code in these pages to catch errors at the
    ///     Page Level and redirect to a static html page (one for general errors and one for 404 errors). 
    ///     These pages will have no theme at all, just text (So that they will work across multiple themes) and the 
    ///     text will be in English (No Translation - Although multiple versions of the html pages could be created to
    ///     handle this. Please specify if it is urgent.
    /// 
    ///     Thanks go to  Joan M for the Original Code.
    ///     Modified and extended by John Mandia.
    ///     Major re-write by Jes1111 - 17/June/2005 - see http://support.rainbowportal.net/confluence/display/DOX/New+Exception+Handling+and+Logging+features
    /// </summary>
    [History("JohnMandia", "john.mandia@whitelightsolutions.com", "1.2", "2003/04/09", 
        "Updated LogToFile code to allow users to specify logfile location and specify frequency of the log files daily monthly yearly or all. Also created the LogHelper file with useful functions."
        )]
    [History("Manu", "manu-dea@hotmail dot it", "1.3", "2004/05/16", 
        "Commented out obsolete code or marked as obsolete. Will be removed in future versions.")]
    public class ErrorHandler
    {
        // 		const string strTOE = "Time of Error: ";
        // 		const string strSrvName = "SERVER_NAME";
        // 		const string strSrc = "Source: ";
        // 		const string strErrMsg = "Error Message: ";
        // 		const string strTgtSite = "Target Site: ";
        // 		const string strStkTrace = "Stack Trace: ";
        #region Public Methods

        /// <summary>
        /// Handles the exception.
        /// </summary>
        [Obsolete("use one of the Publish() overloads")]
        public static void HandleException()
        {
            var e = HttpContext.Current.Server.GetLastError();

            if (e == null)
            {
                return;
            }

            e = e.GetBaseException();

            HandleException(e);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        [Obsolete("use one of the Publish() overloads")]
        public static void HandleException(Exception e)
        {
            // InnerHandleException(FormatExceptionDescription(e), e);
            Publish(LogLevels.Error, e);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [Obsolete("use one of the Publish() overloads")]
        public static void HandleException(string message, Exception e)
        {
            // InnerHandleException(message + Environment.NewLine + FormatExceptionDescription(e), e);
            Publish(LogLevels.Error, message, e);
        }

        /// <summary>
        /// Called only by Application_Error in global.asax.cs to deal with unhandled exceptions.
        /// </summary>
        public static void ProcessUnhandledException()
        {
            try
            {
                var e = HttpContext.Current.Server.GetLastError();

                string redirectUrl = Config.SmartErrorRedirect; // default value
                var httpStatusCode = HttpStatusCode.InternalServerError; // default value
                var myCacheKey = string.Empty;
                StringBuilder sb;

                if (HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Config.SmartErrorRedirect.Substring(2)))
                {
                    HttpContext.Current.Response.Write("Sorry - a critical error has occurred - unable to continue");
                    HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    HttpContext.Current.Response.End();
                }

                try
                {
                    var logLevel = LogLevels.Fatal; // default value
                    if (e is DatabaseUnreachableException || e is SqlException)
                    {
                        logLevel = LogLevels.Fatal;
                        redirectUrl = Config.DatabaseErrorRedirect;
                        httpStatusCode = Config.DatabaseErrorResponse;
                    }
                    else if (e is DatabaseVersionException)
                    {
                        // db version is behind code version
                        logLevel = LogLevels.Fatal;
                        httpStatusCode = Config.DatabaseUpdateResponse;
                        redirectUrl = Config.DatabaseUpdateRedirect;
                    }
                    else if (e is CodeVersionException)
                    {
                        // code version is behind db version
                        logLevel = LogLevels.Fatal;
                        httpStatusCode = Config.CodeUpdateResponse;
                        redirectUrl = Config.CodeUpdateRedirect;
                    }
                    else if (e is PortalsLockedException)
                    {
                        // AllPortals lock is "on"
                        logLevel = ((PortalsLockedException)e).Level;
                        
                        // _auxMessage = "Attempt to access locked portal by non-keyholder.";
                        httpStatusCode = ((PortalsLockedException)e).StatusCode;
                        redirectUrl = Config.LockRedirect;
                        e = null;
                    }
                    else if (e is RainbowRedirect)
                    {
                        logLevel = ((RainbowRedirect)e).Level;
                        httpStatusCode = ((RainbowRedirect)e).StatusCode;
                        redirectUrl = ((RainbowRedirect)e).RedirectUrl;
                    }
                    else if (e is RainbowException)
                    {
                        logLevel = ((RainbowException)e).Level;
                        httpStatusCode = ((RainbowException)e).StatusCode;
                    }
                    else if (e is HttpException)
                    {
                        logLevel = LogLevels.Fatal;
                        httpStatusCode = (HttpStatusCode)((HttpException)e).GetHttpCode();
                    }
                    else
                    {
                        logLevel = LogLevels.Fatal; // default value
                        httpStatusCode = HttpStatusCode.InternalServerError; // default value
                    }

                    // create unique id
                    string myGuid = Guid.NewGuid().ToString("N");
                    string auxMessage = string.Format("errorGUID: {0}", myGuid);

                    // log it
                    var sw = new StringWriter();
                    PublishToLog(logLevel, auxMessage, e, sw);

                    // bundle the info
                    var storedError = new ArrayList(3) { logLevel, myGuid, sw };

                    // cache it
                    sb = new StringBuilder(Portal.UniqueID);
                    sb.Append("_rb_error_");
                    sb.Append(myGuid);
                    myCacheKey = sb.ToString();
                    CurrentCache.Insert(myCacheKey, storedError);
                }
                catch
                {
                    try
                    {
                        HttpContext.Current.Response.WriteFile(Config.CriticalErrorRedirect);
                        HttpContext.Current.Response.StatusCode = (int)Config.CriticalErrorResponse;
                    }
                    catch
                    {
                        HttpContext.Current.Response.Write("Sorry - a critical error has occurred - unable to continue");
                        HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    }
                }
                finally
                {
                    if (redirectUrl.StartsWith("http://"))
                    {
                        HttpContext.Current.Response.Redirect(redirectUrl, true);
                    }
                    else if (redirectUrl.StartsWith("~/") && redirectUrl.IndexOf(".aspx") > 0)
                    {
                        // append params to redirect url
                        if (!redirectUrl.StartsWith(@"http://"))
                        {
                            sb = new StringBuilder();
                            if (redirectUrl.IndexOf("?") != -1)
                            {
                                sb.Append(redirectUrl.Substring(0, redirectUrl.IndexOf("?") + 1));
                                sb.Append(((int)httpStatusCode).ToString());
                                sb.Append("&eid=");
                                sb.Append(myCacheKey);
                                sb.Append("&");
                                sb.Append(redirectUrl.Substring(redirectUrl.IndexOf("?") + 1));
                                redirectUrl = sb.ToString();
                            }
                            else
                            {
                                sb.Append(redirectUrl);
                                sb.Append("?");
                                sb.Append(((int)httpStatusCode).ToString());
                                sb.Append("&eid=");
                                sb.Append(myCacheKey);
                                redirectUrl = sb.ToString();
                            }
                        }

                        HttpContext.Current.Response.Redirect(redirectUrl, true);
                    }
                    else if (redirectUrl.StartsWith("~/") && redirectUrl.IndexOf(".htm") > 0)
                    {
                        HttpContext.Current.Response.WriteFile(redirectUrl);
                        HttpContext.Current.Response.StatusCode = (int)httpStatusCode;
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        HttpContext.Current.Response.End();
                    }
                    else
                    {
                        HttpContext.Current.Response.Write("Sorry - a critical error has occurred - unable to continue");
                        HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        HttpContext.Current.Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Publish(LogLevels.Fatal, "Unexpected error in ErrorHandler", ex);
            }
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">
        /// LogLevel enum
        /// </param>
        /// <param name="auxMessage">
        /// Text message to be shown in log entry
        /// </param>
        public static void Publish(LogLevels logLevel, string auxMessage)
        {
            PublishToLog(logLevel, auxMessage, null);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">
        /// LogLevel enum
        /// </param>
        /// <param name="e">
        /// Exception object (can be null)
        /// </param>
        public static void Publish(LogLevels logLevel, Exception e)
        {
            PublishToLog(logLevel, string.Empty, e);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">
        /// LogLevel enum
        /// </param>
        /// <param name="auxMessage">
        /// Text message to be shown in log entry
        /// </param>
        /// <param name="e">
        /// Exception object (can be null)
        /// </param>
        public static void Publish(LogLevels logLevel, string auxMessage, Exception e)
        {
            PublishToLog(logLevel, auxMessage, e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Publishes the exception.
        /// </summary>
        /// <param name="logLevel">
        /// Rainbow.Framework.Configuration.LogLevel enumerator
        /// </param>
        /// <param name="auxMessage">
        /// Text message to be shown in log entry
        /// </param>
        /// <param name="e">
        /// Exception object (can be null)
        /// </param>
        private static void PublishToLog(LogLevels logLevel, string auxMessage, Exception e)
        {
            // log it
            LogHelper.Logger.Log(logLevel, auxMessage, e);
        }

        /// <summary>
        /// Publishes the exception.
        /// </summary>
        /// <param name="logLevel">
        /// Rainbow.Framework.Configuration.LogLevel enumerator
        /// </param>
        /// <param name="auxMessage">
        /// Text message to be shown in log entry
        /// </param>
        /// <param name="e">
        /// Exception object (can be null)
        /// </param>
        /// <param name="sw">
        /// A StringWriter object which will be filled with a formatted verion of the log entry
        /// </param>
        private static void PublishToLog(LogLevels logLevel, string auxMessage, Exception e, StringWriter sw)
        {
            // log it
            LogHelper.Logger.Log(logLevel, auxMessage, e, sw);
        }

        #endregion
    }
}