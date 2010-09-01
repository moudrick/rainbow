namespace Rainbow.Framework.BusinessLogic
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;
    using System.Web;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Configuration.Properties;
    using Rainbow.Framework.Configuration.Web;

    /// <summary>
    /// This class contains useful information for Extension, Module and Core Developers.
    /// </summary>
    public static class Portal
    {
        #region Constants and Fields

        /// <summary>
        /// The context.
        /// </summary>
        private static WebContextReader context = new WebContextReader();

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the code version.
        /// </summary>
        /// <value>The code version.</value>
        public static int CodeVersion
        {
            get
            {
                return context.Current != null && context.Current.Application["CodeVersion"] != null
                           ? (int)context.Current.Application["CodeVersion"]
                           : 0;
            }
        }

        /// <summary>
        ///     Gets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        public static int PageId
        {
            get
            {
                string strPageId = null;

                return FindPageIdFromQueryString(context.Current.Request.QueryString, ref strPageId)
                           ? Config.GetIntegerFromString(false, strPageId, 0)
                           : 0;
            }
        }

        /// <summary>
        ///     Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public static string ProductVersion
        {
            get
            {
                if (HttpContext.Current.Application["ProductVersion"] == null)
                {
                    var f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
                    HttpContext.Current.Application.UnLock();
                }

                return (string)HttpContext.Current.Application["ProductVersion"];
            }
        }

        /// <summary>
        ///    Gets the site's alias either via querystring, cookie or domain and returns it
        /// </summary>
        /// <value>The unique ID.</value>
        public static string UniqueId
        {
            // new version - Jes1111 - 07/07/2005
            get
            {
                if (context.Current.Items["PortalAlias"] == null)
                {
                    // not already in context
                    string uniquePortalId = Settings.DefaultPortal; // set default value

                    FindAlias(context.Current.Request, ref uniquePortalId); // will change uniquePortalID if it can

                    context.Current.Items.Add("PortalAlias", uniquePortalId); // add to context

                    return uniquePortalId; // return current value
                }
                
                // already in context
                return (string)context.Current.Items["PortalAlias"]; // return from context
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the alias from cookies.
        /// </summary>
        /// <param name="cookies">
        /// The cookies.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The find alias from cookies.
        /// </returns>
        public static bool FindAliasFromCookies(HttpCookieCollection cookies, ref string alias)
        {
            var c = cookies["PortalAlias"];
            if (c == null)
            {
                return false;
            }
            
            var cookieValue = c.Value.Trim().ToLowerInvariant();
            if (cookieValue.Length == 0)
            {
                // ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "FindAliasFromCookies failed - PortalAlias found but value was empty.");
                return false;
            }
            
            alias = cookieValue;
            return true;
        }

        /// <summary>
        /// Finds the alias from query string.
        /// </summary>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The find alias from query string.
        /// </returns>
        public static bool FindAliasFromQueryString(NameValueCollection queryString, ref string alias)
        {
            if (queryString == null)
            {
                return false;
            }
            
            if (queryString["Alias"] == null)
            {
                return false;
            }

            var queryStringValues = queryString.GetValues("Alias");
            var queryStringValue = string.Empty;

            if (queryStringValues != null && queryStringValues.Length > 0)
            {
                queryStringValue = queryStringValues[0].Trim().ToLowerInvariant();
            }

            if (queryStringValue.Length == 0)
            {
                // ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "FindAliasFromQueryString failed - Alias param found but value was empty.");
                return false;
            }
            
            alias = queryStringValue;
            return true;
        }

        /// <summary>
        /// Finds the alias from URI.
        /// </summary>
        /// <param name="requestUri">
        /// The request URI.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="defaultPortal">
        /// The default portal.
        /// </param>
        /// <param name="removeWww">
        /// if set to <c>true</c> [remove WWW].
        /// </param>
        /// <param name="removeTld">
        /// if set to <c>true</c> [remove TLD].
        /// </param>
        /// <param name="secondLevelDomains">
        /// The second level domains.
        /// </param>
        /// <returns>
        /// The find alias from uri.
        /// </returns>
        public static bool FindAliasFromUri(
            Uri requestUri, 
            ref string alias, 
            string defaultPortal, 
            bool removeWww, 
            bool removeTld, 
            string secondLevelDomains)
        {
            // if request is to localhost, return default portal 
            if (requestUri.IsLoopback)
            {
                alias = defaultPortal;
                return true;
            }

            if (requestUri.HostNameType == UriHostNameType.Dns)
            {
                // get it from hostname
                var hostDelim = new[] { '.' };

                // step 1: split hostname into parts
                var hostPartsList = new ArrayList(requestUri.Host.Split(hostDelim));

                // step 2: do we need to remove "www"?
                if (removeWww && hostPartsList[0].ToString() == "www")
                {
                    hostPartsList.RemoveAt(0);
                }

                // step 3: do we need to remove TLD?
                if (removeTld)
                {
                    hostPartsList.Reverse();
                    if (hostPartsList.Count > 2 && hostPartsList[0].ToString().Length == 2)
                    {
                        // this is a ccTLD, so need to check if next segment is a pseudo-gTLD
                        var globalTlds = new ArrayList(secondLevelDomains.Split(new[] { ';' }));
                        if (globalTlds.Contains(hostPartsList[1].ToString()))
                        {
                            hostPartsList.RemoveRange(0, 2);
                        }
                        else
                        {
                            hostPartsList.RemoveAt(0);
                        }
                    }
                    else
                    {
                        hostPartsList.RemoveAt(0);
                    }

                    hostPartsList.Reverse();
                }

                // step 4: re-assemble the remaining parts
                alias = string.Join(".", (string[])hostPartsList.ToArray(typeof(string)));
                return true;
            }
            
            alias = defaultPortal;
            return true;
        }

        /// <summary>
        /// Finds the page id from query string.
        /// </summary>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// The find page id from query string.
        /// </returns>
        public static bool FindPageIdFromQueryString(NameValueCollection queryString, ref string pageId)
        {
            string[] queryStringValues;

            // tabID = 240
            if (queryString == null)
            {
                return false;
            }
            
            if (queryString["pageId"] != null)
            {
                // Properties.Resources.str_PageID
                queryStringValues = queryString.GetValues(Resources.str_PageID);
            }
            else if (queryString["tabId"] != null)
            {
                // Properties.Resources.str_TabID
                queryStringValues = queryString.GetValues(Resources.str_TabID);
            }
            else
            {
                return false;
            }

            var queryStringValue = string.Empty;

            if (queryStringValues != null && queryStringValues.Length > 0)
            {
                queryStringValue = queryStringValues[0].Trim().ToLowerInvariant();
            }

            if (queryStringValue.Length == 0)
            {
                // ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "FindPageIDFromQueryString failed - Alias param found but value was empty.");
                return false;
            }
            
            pageId = queryStringValue;
            return true;
        }

        /// <summary>
        /// Gets the proxy parameters as configured in web.config by Phillo 22/01/2003
        /// </summary>
        /// <returns>
        /// A WebProxy.
        /// </returns>
        public static WebProxy GetProxy()
        {
            // jes1111 - if(ConfigurationSettings.AppSettings["ProxyServer"].Length > 0) 
            // if(Config.ProxyServer.Length > 0) 
            // { 
            var webProxy = new WebProxy();
            var networkCredential = new NetworkCredential
                {
                    Domain = Settings.ProxyDomain,
                    UserName = Settings.ProxyUserId,
                    Password = Settings.ProxyPassword 
                };

            // myCredential.Domain = ConfigurationSettings.AppSettings["ProxyDomain"]; 
            // myCredential.UserName = ConfigurationSettings.AppSettings["ProxyUserID"]; 
            // myCredential.Password = ConfigurationSettings.AppSettings["ProxyPassword"]; 
            webProxy.Credentials = networkCredential;

            // myProxy.Address = new Uri(ConfigurationSettings.AppSettings["ProxyServer"]); 
            webProxy.Address = new Uri(Settings.ProxyServer);
            return webProxy;

            // } 

            // else 
            // { 
            //     return(null); 
            // } 
        }

        /// <summary>
        /// Sets reader for context in this class
        /// </summary>
        /// <param name="reader">
        /// an instance of a Concrete Strategy Reader
        /// </param>
        public static void SetReader(WebContextReader reader)
        {
            context = reader;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the alias.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        private static void FindAlias(HttpRequest request, ref string alias)
        {
            if (FindAliasFromQueryString(request.QueryString, ref alias))
            {
                return;
            }
            
            if (FindAliasFromCookies(request.Cookies, ref alias))
            {
                return;
            }
            
            FindAliasFromUri(
                request.Url, 
                ref alias, 
                Settings.DefaultPortal,
                Settings.RemoveWWW,
                Settings.RemoveTLD,
                Settings.SecondLevelDomains);
            return;
        }

        #endregion
    }
}