using System;
using System.Collections;
//using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Rainbow.Framework.Core; //RainbowContext
using Rainbow.Framework.Exceptions; //all
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Settings; //Config
using Rainbow.Framework.Settings.Cache; //CurrentCache

namespace Rainbow.Framework
{
    /// <summary>
    /// This class in combination with the Web.Config file handles all the Errors that are not caught programatically
    /// 99% of the time Errors will be caught by Rainbow's HttpUrlModule, this class will be called, errors will be 
    /// logged depending on what was specified by the Web.Config file, after that the error cascades up and is caught
    /// by the customErrors settings in Web.Config. Here you can specify errors and which pages to redirect to.
    /// Visitors will be directed to dynamic aspx pages for General Errors and 404 Errors (Specified aspx page does not exist)
    /// These pages are dynamic and will keep the theme you selected for your portal. It also makes use of Rainbow's
    /// multi-language support. If these dynamic pages themselves have an error (e.g the Database has crashed 
    /// so it can't retrieve the theme or translations, then there is code in these pages to catch errors at the
    /// Page Level and redirect to a static html page (one for general errors and one for 404 errors). 
    /// These pages will have no theme at all, just text (So that they will work across multiple themes) and the 
    /// text will be in English (No Translation - Although multiple versions of the html pages could be created to
    /// handle this. Please specify if it is urgent.
    /// 
    /// Thanks go to  Joan M for the Original Code.
    /// Modified and extended by John Mandia.
    /// Major re-write by Jes1111 - 17/June/2005 - see http://support.rainbowportal.net/confluence/display/DOX/New+Exception+Handling+and+Logging+features
    /// </summary>
    [
        History("JohnMandia", "john.mandia@whitelightsolutions.com", "1.2", "2003/04/09",
            "Updated LogToFile code to allow users to specify logfile location and specify frequency of the log files daily monthly yearly or all. Also created the LogHelper file with useful functions."
            )]
    [
        History("Manu", "manu-dea@hotmail dot it", "1.3", "2004/05/16",
            "Commented out obsolete code or marked as obsolete. Will be removed in future versions.")]
    public class ErrorHandler
    {
        /// <summary>
        /// Called only by Application_Error in global.asax.cs to deal with unhandled exceptions.
        /// </summary>
        public static void ProcessUnhandledException()
        {
            try
            {
                HttpContext httpContext = RainbowContext.Current.HttpContext;
                if (httpContext.Request != null &&
                    httpContext.Request.Url.AbsolutePath.EndsWith(
                        Config.SmartErrorRedirect.Substring(2)))
                {
                    httpContext.Response.Write("Sorry - a critical error has occurred - unable to continue");
                    httpContext.Response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;
                    httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    httpContext.Response.End();
                }

                Exception lastError = httpContext.Server.GetLastError();
                string redirectUrl = Config.SmartErrorRedirect; // default value
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError; // default value
                string cacheKey = string.Empty;
                StringBuilder stringBuilder;

                try
                {
                    LogLevel logLevel;
                    if (lastError is DatabaseUnreachableException
                        || lastError.GetType().FullName.Contains("System.Data.SqlClient.SqlException") )
                    {
                        logLevel = LogLevel.Fatal;
                        redirectUrl = Config.DatabaseErrorRedirect;
                        httpStatusCode = Config.DatabaseErrorResponse;
                    }
                    else if (lastError is DatabaseVersionException) // db version is behind code version
                    {
                        logLevel = LogLevel.Fatal;
                        httpStatusCode = Config.DatabaseUpdateResponse;
                        redirectUrl = Config.DatabaseUpdateRedirect;
                    }
                    else if (lastError is CodeVersionException) // code version is behind db version
                    {
                        logLevel = LogLevel.Fatal;
                        httpStatusCode = Config.CodeUpdateResponse;
                        redirectUrl = Config.CodeUpdateRedirect;
                    }
                    else if (lastError is PortalsLockedException) // AllPortals lock is "on"
                    {
                        logLevel = ((PortalsLockedException) lastError).Level;
                        //_auxMessage = "Attempt to access locked portal by non-keyholder.";
                        httpStatusCode = ((PortalsLockedException) lastError).StatusCode;
                        redirectUrl = Config.LockRedirect;
                        lastError = null;
                    }
                    else if (lastError is RainbowRedirectException)
                    {
                        logLevel = ((RainbowRedirectException) lastError).Level;
                        httpStatusCode = ((RainbowRedirectException) lastError).StatusCode;
                        redirectUrl = ((RainbowRedirectException) lastError).RedirectUrl;
                    }
                    else if (lastError is RainbowException)
                    {
                        logLevel = ((RainbowException) lastError).Level;
                        httpStatusCode = ((RainbowException) lastError).StatusCode;
                    }
                    else if (lastError is HttpException)
                    {
                        logLevel = LogLevel.Fatal;
                        httpStatusCode = (HttpStatusCode) ((HttpException) lastError).GetHttpCode();
                    }
                    else
                    {
                        logLevel = LogLevel.Fatal; // default value
                        httpStatusCode = HttpStatusCode.InternalServerError; // default value
                    }

                    // create unique id
                    string guid = Guid.NewGuid().ToString("N");
                    string auxMessage = string.Format("errorGUID: {0}", guid);

                    // log it
                    StringWriter stringWriter = new StringWriter();
                    LogHelper.Logger.Log(logLevel, auxMessage, lastError, stringWriter);

                    // bundle the info
                    ArrayList storedError = new ArrayList(3);
                    storedError.Add(logLevel);
                    storedError.Add(guid);
                    storedError.Add(stringWriter);
                    // cache it
                    stringBuilder = new StringBuilder(RainbowContext.Current.UniqueID);
                    stringBuilder.Append("_rb_error_");
                    stringBuilder.Append(guid);
                    cacheKey = stringBuilder.ToString();
                    CurrentCache.Insert(cacheKey, storedError);
                }
                catch
                {
                    try
                    {
                        httpContext.Response.WriteFile(Config.CriticalErrorRedirect);
                        httpContext.Response.StatusCode = (int) Config.CriticalErrorResponse;
                    }
                    catch
                    {
                        httpContext.Response.Write("Sorry - a critical error has occurred - unable to continue");
                        httpContext.Response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;
                    }
                }
                finally
                {
                    if (redirectUrl.StartsWith("http://"))
                    {
                        httpContext.Response.Redirect(redirectUrl, true);
                    }
                    else if (redirectUrl.StartsWith("~/") && redirectUrl.IndexOf(".aspx") > 0)
                    {
                        // append params to redirect url
                        if (!redirectUrl.StartsWith(@"http://"))
                        {
                            stringBuilder = new StringBuilder();
                            if (redirectUrl.IndexOf("?") != -1)
                            {
                                stringBuilder.Append(redirectUrl.Substring(0, redirectUrl.IndexOf("?") + 1));
                                stringBuilder.Append(((int) httpStatusCode).ToString());
                                stringBuilder.Append("&eid=");
                                stringBuilder.Append(cacheKey);
                                stringBuilder.Append("&");
                                stringBuilder.Append(redirectUrl.Substring(redirectUrl.IndexOf("?") + 1));
                                redirectUrl = stringBuilder.ToString();
                            }
                            else
                            {
                                stringBuilder.Append(redirectUrl);
                                stringBuilder.Append("?");
                                stringBuilder.Append(((int) httpStatusCode).ToString());
                                stringBuilder.Append("&eid=");
                                stringBuilder.Append(cacheKey);
                                redirectUrl = stringBuilder.ToString();
                            }
                        }
                        httpContext.Response.Redirect(redirectUrl, true);
                    }
                    else if (redirectUrl.StartsWith("~/") && redirectUrl.IndexOf(".htm") > 0)
                    {
                        httpContext.Response.WriteFile(redirectUrl);
                        httpContext.Response.StatusCode = (int) httpStatusCode;
                        httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        httpContext.Response.End();
                    }
                    else
                    {
                        httpContext.Response.Write("Sorry - a critical error has occurred - unable to continue");
                        httpContext.Response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;
                        httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        httpContext.Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Publish(LogLevel.Fatal, "Unexpected error in ErrorHandler", ex);
            }
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        /// <param name="auxMessage">Text message to be shown in log entry</param>
        public static void Publish(LogLevel logLevel, string auxMessage)
        {
            PublishToLog(logLevel, auxMessage, null);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        /// <param name="e">Exception object (can be null)</param>
        public static void Publish(LogLevel logLevel, Exception e)
        {
            PublishToLog(logLevel, string.Empty, e);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        /// <param name="auxMessage">Text message to be shown in log entry</param>
        /// <param name="e">Exception object (can be null)</param>
        public static void Publish(LogLevel logLevel, string auxMessage, Exception e)
        {
            PublishToLog(logLevel, auxMessage, e);
        }

        /// <summary>
        /// Publishes the exception.
        /// </summary>
        /// <param name="_logLevel">Rainbow.Framework.Configuration.LogLevel enumerator</param>
        /// <param name="_auxMessage">Text message to be shown in log entry</param>
        /// <param name="e">Exception object (can be null)</param>
        static void PublishToLog(LogLevel _logLevel, string _auxMessage, Exception e)
        {
            LogHelper.Logger.Log(_logLevel, _auxMessage, e);
        }
    }
}
