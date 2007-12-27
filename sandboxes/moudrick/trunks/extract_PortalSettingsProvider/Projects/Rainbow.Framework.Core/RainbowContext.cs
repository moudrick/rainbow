using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Rainbow.Context;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Core.DAL;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Security;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Settings.Cache;
using Path=System.IO.Path;

namespace Rainbow.Framework.Core
{
    ///<summary>
    /// Facade for providers.
    ///</summary>
    public class RainbowContext
    {
        static readonly RainbowContext current = new RainbowContext(HttpContext.Current);

        readonly HttpContext httpContext;
        readonly Context.Reader rainbowContextReader;

        ///<summary>
        /// Gets current Rainbow Context
        ///</summary>
        public static RainbowContext Current
        {
            get { return current; }
        }

        /// <summary>
        /// CurrentUser
        /// </summary>
        /// <value>The current user.</value>
        //TODO: [moudrick] make it non-static
        public static RainbowPrincipal CurrentUser
        {
            get
            {
                RainbowPrincipal rainbowPrincipal;

                if (HttpContext.Current.User is RainbowPrincipal)
                {
                    rainbowPrincipal = (RainbowPrincipal)HttpContext.Current.User;
                }
                else
                {
                    rainbowPrincipal = new RainbowPrincipal(HttpContext.Current.User.Identity, null);
                }
                return rainbowPrincipal;
            }
        }

        /// <summary>
        /// Gets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        public int PageID
        {
            get
            {
                string strPageID = null;

                if (FindPageIdFromQueryString(rainbowContextReader.Current.Request.QueryString, ref strPageID))
                {
                    return Config.GetIntegerFromString(false, strPageID, 0);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// This static string fetches the site's alias either via querystring, cookie or domain and returns it
        /// </summary>
        /// <value>The unique ID.</value>
        public string UniqueID
        {
            // new version - Jes1111 - 07/07/2005
            get
            {
                if (rainbowContextReader.Current.Items["PortalAlias"] == null) // not already in context
                {
                    string uniquePortalID = Config.DefaultPortal; // set default value
                    FindAlias(rainbowContextReader.Current.Request, ref uniquePortalID); // will change uniquePortalID if it can
                    rainbowContextReader.Current.Items.Add("PortalAlias", uniquePortalID); // add to context
                    return uniquePortalID; // return current value
                }
                else // already in context
                {
                    return (string)rainbowContextReader.Current.Items["PortalAlias"]; // return from context
                }
            }
        }

        RainbowContext(HttpContext httpContext)
        {
            this.httpContext = httpContext;
            this.rainbowContextReader = new Context.Reader(new WebContextReader());
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings persistence.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void UpdatePortalSetting(int portalID, string key, string value)
        {
            PortalProvider.Instance.UpdatePortalSetting(portalID, key, value);
            CurrentCache.Remove(Key.PortalSettings());
        }

        ///<summary>
        /// Rewrites path
        /// 1st Check: is it a dangerously malformed request?
        /// Important patch http://support.microsoft.com/?kbid=887459
        ///</summary>
        public void RewritePath()
        {
            string currentURL = httpContext.Request.Path.ToLower();

            httpContext.Trace.Warn("Application_BeginRequest :: " + currentURL);
            if (PageID > 0)
            {
                //Creates the physical path on the server 
                string physicalPath = httpContext.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/") + 1));

                // TODO: Can we enhance performance here by checking to see if it is a friedly url page
                // name instead of doing an IO check for exists?
                // checks to see if the file does not exsists.
                if (!File.Exists(physicalPath)) // Rewrites the path
                {
                    httpContext.RewritePath("~/default.aspx?" + httpContext.Request.ServerVariables["QUERY_STRING"]);
                }
            }
            else
            {
                string pname = currentURL.Substring(currentURL.LastIndexOf("/") + 1);
                pname = pname.Substring(0, (pname.Length - 5));
                if (Regex.IsMatch(pname, @"^\d+$"))
                {
                    httpContext.RewritePath("~/default.aspx?pageid=" + pname +
                                        httpContext.Request.ServerVariables["QUERY_STRING"]);
                }
            }

            // 1st Check: is it a dangerously malformed request?
            //Important patch http://support.microsoft.com/?kbid=887459
            if (httpContext.Request.Path.IndexOf('\\') >= 0 ||
                Path.GetFullPath(httpContext.Request.PhysicalPath) != httpContext.Request.PhysicalPath)
            {
                throw new RainbowRedirect(LogLevel.Warn, HttpStatusCode.NotFound, "Malformed request", null);
            }
        }

        ///<summary>
        /// ************ 'calculate' response to this request ************
        ///
        /// Test 1 - try requested Alias and requested PageID
        /// Test 2 - try requested Alias and PageID 0
        /// Test 3 - try default Alias and requested PageID
        /// Test 4 - try default Alias and PageID 0
        ///
        /// The UrlToleranceLevel determines how many times the test is allowed to fail before the request is considered
        /// to be "an error" and is therefore redirected:
        ///
        /// UrlToleranceLevel 1 
        ///		- requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, InvalidPageIdRedirect page is shown
        /// 
        /// UrlToleranceLevel 2 
        ///		- requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, PageID 0 (Home page) is shown
        ///
        /// UrlToleranceLevel 3 - !!!!!!!! not working?
        ///		- if requested Alias is invalid, default Alias will be used
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, InvalidPageIdRedirect page is shown
        /// 
        /// UrlToleranceLevel 4 - 
        ///		- if requested Alias is invalid, default Alias will be used
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, PageID 0 (Home page) is shown
        ///
        ///</summary>
        ///<exception cref="RainbowRedirect"></exception>
        public void CalculatePortalResponse()
        {
            PortalSettings portalSettings = null;
            int pageID = PageID; // Get PageID from QueryString
            string portalAlias = UniqueID; // Get requested alias from querystring, cookies or hostname
            string defaultAlias = Config.DefaultPortal; // get default portal from config

            // load arrays with values to test
            string[] testAlias = new string[4] { portalAlias, portalAlias, defaultAlias, defaultAlias };
            int[] testPageID = new int[4] { pageID, 0, pageID, 0 };

            int testsAllowed = Config.UrlToleranceLevel;
            int testsToRun = testsAllowed > 2 ? 4 : 2;
            // if requested alias is default alias, limit UrlToleranceLevel to max value of 2 and limit tests to 2
            if (portalAlias == defaultAlias)
            {
                testsAllowed = testsAllowed % 2;
                testsToRun = 2;
            }

            int testsCounter = 1;
            while (testsCounter <= testsToRun)
            {
                //try with current values from arrays
                portalSettings = PortalProvider.Instance.InstantiateNewPortalSettings(testPageID[testsCounter - 1], testAlias[testsCounter - 1]);

                // test returned result
                if (portalSettings.PortalAlias != null)
                {
                    break; // successful hit
                }
                else
                {
                    testsCounter++; // increment the test counter and continue
                }
            }

            if (portalSettings == null || portalSettings.PortalAlias == null)
            {
                // critical error - neither requested alias nor default alias could be found in DB
                throw new RainbowRedirect(
                    Config.NoPortalErrorRedirect,
                    LogLevel.Fatal,
                    Config.NoPortalErrorResponse,
                    "Unable to load any portal - redirecting request to ErrorNoPortal page.",
                    null);
            }

            if (testsCounter <= testsAllowed) // success
            {
                // Portal Settings has passed the test so add it to Context
                httpContext.Items.Add("PortalSettings", portalSettings);
                httpContext.Items.Add("PortalID", portalSettings.PortalID); // jes1111
            }
            else // need to redirect
            {
                if (portalSettings.PortalAlias != portalAlias) // we didn't get the portal we asked for
                {
                    throw new RainbowRedirect(
                        Config.InvalidAliasRedirect,
                        LogLevel.Info,
                        HttpStatusCode.NotFound,
                        "Invalid Alias specified in request URL - redirecting (404) to InvalidAliasRedirect page.",
                        null);
                }

                if (portalSettings.ActivePage.PageID != pageID) // we didn't get the page we asked for
                {
                    throw new RainbowRedirect(
                        Config.InvalidPageIdRedirect,
                        LogLevel.Info,
                        HttpStatusCode.NotFound,
                        "Invalid PageID specified in request URL - redirecting (404) to InvalidPageIdRedirect page.",
                        null);
                }
            }

            // Save cookies
            //saveCookie = true; // Jes1111 - why is this always set to true? is it needed?
            //ExtendCookie(settings); 
            //if (saveCookie) // Jes1111 - why is this always set to true? is it needed?
            //{
            httpContext.Response.Cookies["PortalAlias"].Path = "/";
            httpContext.Response.Cookies["PortalAlias"].Value = portalSettings.PortalAlias;
            //}

            //Try to get alias from cookie to determine if alias has been changed
            bool refreshSite = false;
            if (httpContext.Request.Cookies["PortalAlias"] != null &&
                httpContext.Request.Cookies["PortalAlias"].Value.ToLower() != UniqueID)
            {
                refreshSite = true; //Portal has changed since last page request
            }
            // if switching portals then clean parameters [TipTopWeb]
            // Must be the last instruction in this method 

            // 5/7/2006 Ed Daniel
            // Added hack for Http 302 by extending condition below to check for more than 3 cookies
            if (refreshSite && httpContext.Request.Cookies.Keys.Count > 3)
            {
                // Signout and force the browser to refresh only once to avoid any dead-lock
                if (httpContext.Request.Cookies["refreshed"] == null
                    || (httpContext.Request.Cookies["refreshed"] != null
                        && httpContext.Response.Cookies["refreshed"].Value == "false"))
                {
                    string rawUrl = httpContext.Request.RawUrl;

                    // jes1111 - not needed now
                    //					//by Manu avoid endless loop when portal does not exists
                    //					if (rawUrl.EndsWith("init")) // jes1111: is this still valid/needed?
                    //						context.Response.Redirect("~/app_support/ErrorNoPortal.html", true);
                    //
                    //					// add parameter at the end of the command line to detect the dead-lock 
                    //					if (rawUrl.LastIndexOf(@"?") > 0)
                    //						rawUrl += "&init";
                    //					else rawUrl += "?init";

                    httpContext.Response.Cookies["refreshed"].Value = "true";
                    httpContext.Response.Cookies["refreshed"].Path = "/";
                    httpContext.Response.Cookies["refreshed"].Expires = DateTime.Now.AddMinutes(1);

                    // sign-out, if refreshed param on the command line we will not call it again
                    PortalSecurity.SignOut(rawUrl, false);
                }
            }

            // invalidate cookie, so the page can be refreshed when needed
            if (httpContext.Request.Cookies["refreshed"] != null && httpContext.Request.Cookies.Keys.Count > 3)
            {
                httpContext.Response.Cookies["refreshed"].Path = "/";
                httpContext.Response.Cookies["refreshed"].Value = "false";
                httpContext.Response.Cookies["refreshed"].Expires = DateTime.Now.AddMinutes(1);
            }
        }

        /// <summary>
        /// Finds the page id from query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        static bool FindPageIdFromQueryString(NameValueCollection queryString, ref string pageID)
        {
            string[] queryStringValues;
            // tabID = 240
            if (queryString != null)
            {
                if (queryString[GlobalInternalStrings.str_PageID] != null)
                {
                    queryStringValues = queryString.GetValues(GlobalInternalStrings.str_PageID);
                }
                else if (queryString[GlobalInternalStrings.str_TabID] != null)
                {
                    queryStringValues = queryString.GetValues(GlobalInternalStrings.str_TabID);
                }
                else
                {
                    return false;
                }

                string queryStringValue = string.Empty;

                if (queryStringValues != null && queryStringValues.Length > 0)
                {
                    queryStringValue = queryStringValues[0].Trim().ToLower(CultureInfo.InvariantCulture);
                }

                if (queryStringValue.Length != 0)
                {
                    pageID = queryStringValue;
                    return true;
                }
                else
                {
                    //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "FindPageIDFromQueryString failed - Alias param found but value was empty.");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        ///<summary>
        /// 2nd Check: is the AllPortals Lock switched on?
        /// let the user through if client IP address is in LockExceptions list, otherwise throw...
        ///</summary>
        ///<exception cref="PortalsLockedException"></exception>
        public void CheckAllPortalsLock()
        {
            if (Config.LockAllPortals)
            {
                string rawUrl = httpContext.Request.RawUrl.ToLower(CultureInfo.InvariantCulture);
                string lockRedirect = Config.LockRedirect;
                if (!rawUrl.EndsWith(lockRedirect))
                {
                    // construct IPList
                    string[] lockKeyHolders = Config.LockKeyHolders.Split(new char[] { ';' });
                    IPList ipList = new IPList();
                    foreach (string lockKeyHolder in lockKeyHolders)
                    {
                        if (lockKeyHolder.IndexOf("-") > -1)
                        {
                            ipList.AddRange(lockKeyHolder.Substring(0, lockKeyHolder.IndexOf("-")),
                                            lockKeyHolder.Substring(lockKeyHolder.IndexOf("-") + 1));
                        }
                        else
                        {
                            ipList.Add(lockKeyHolder);
                        }
                    }
                    // check if requestor's IP address is in allowed list
                    if (!ipList.CheckNumber(httpContext.Request.UserHostAddress))
                    {
                        throw new PortalsLockedException();
                    }
                }
            }
        }

        ///<summary>
        /// 3rd Check: is database/code version correct?
        ///</summary>
        public void CheckDatabaseVersion()
        {
            int versionDelta = DatabaseUpdater.DatabaseVersion.CompareTo(DatabaseUpdater.CodeVersion);
            // if DB and code versions do not match
            if (versionDelta != 0)
            {
                Uri requestUri = httpContext.Request.Url;
                string databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
                if (databaseUpdateRedirect.StartsWith("~/"))
                {
                    databaseUpdateRedirect = databaseUpdateRedirect.TrimStart(new char[] { '~' });
                }

                if (
                    !
                    requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture).EndsWith(
                        databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                {
                    // ...and this is not DB Update page
                    string errorMessage = "Database version: " + DatabaseUpdater.DatabaseVersion + " Code version: " +
                                          DatabaseUpdater.CodeVersion;
                    if (versionDelta < 0) // DB Version is behind Code Version
                    {
                        // Jonathan : WHy wouldnt we redirect to update page?
                        // TODO : Check with people why this was like this....
                        httpContext.Response.Redirect(Framework.Settings.Path.ApplicationRoot + databaseUpdateRedirect, true);
                        // so update?
                        ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                        // throw new DatabaseVersionException(errorMessage);
                    }
                    else // DB version is ahead of Code Version
                    {
                        ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                        // Jonathan : WHy wouldnt we redirect to update page?
                        // TODO : Check with people why this was like this....
                        // Who cares ?
                        // throw new CodeVersionException(errorMessage);
                    }
                }
                else // this is already DB Update page... 
                {
                    return; // so skip creation of PortalSettings
                }
            }
        }

        /// <summary>
        /// Finds the alias.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="alias">The alias.</param>
        void FindAlias(HttpRequest request, ref string alias)
        {
            if (FindAliasFromQueryString(request.QueryString, ref alias))
            {
                return;
            }
            else if (FindAliasFromCookies(request.Cookies, ref alias))
            {
                return;
            }
            else
            {
                FindAliasFromUri(request.Url,
                                 ref alias,
                                 Config.DefaultPortal,
                                 Config.RemoveWWW,
                                 Config.RemoveTLD,
                                 Config.SecondLevelDomains);
                return;
            }
        }

        /// <summary>
        /// Finds the alias from cookies.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        bool FindAliasFromCookies(HttpCookieCollection cookies, ref string alias)
        {
            if (cookies["PortalAlias"] != null)
            {
                string cookieValue = cookies["PortalAlias"].Value.Trim().ToLower(CultureInfo.InvariantCulture);
                if (cookieValue.Length != 0)
                {
                    alias = cookieValue;
                    return true;
                }
                else
                {
                    //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "FindAliasFromCookies failed - PortalAlias found but value was empty.");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Finds the alias from query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        bool FindAliasFromQueryString(NameValueCollection queryString, ref string alias)
        {
            if (queryString != null)
            {
                if (queryString["Alias"] != null)
                {
                    string[] queryStringValues = queryString.GetValues("Alias");
                    string queryStringValue = string.Empty;

                    if (queryStringValues.Length > 0)
                    {
                        queryStringValue = queryStringValues[0].Trim().ToLower(CultureInfo.InvariantCulture);
                    }

                    if (queryStringValue.Length != 0)
                    {
                        alias = queryStringValue;
                        return true;
                    }
                    else
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "FindAliasFromQueryString failed - Alias param found but value was empty.");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the alias from URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="defaultPortal">The default portal.</param>
        /// <param name="removeWWW">if set to <c>true</c> [remove WWW].</param>
        /// <param name="removeTLD">if set to <c>true</c> [remove TLD].</param>
        /// <param name="secondLevelDomains">The second level domains.</param>
        /// <returns></returns>
        void FindAliasFromUri(Uri requestUri,
                 ref string alias,
                 string defaultPortal,
                 bool removeWWW,
                 bool removeTLD,
                 string secondLevelDomains)
        {
            // if request is to localhost, return default portal 
            if (requestUri.IsLoopback)
            {
                alias = defaultPortal;
                //return true;
            }
            else if (requestUri.HostNameType == UriHostNameType.Dns) // get it from hostname
            {
                char[] hostDelim = new char[] {'.'};

                // step 1: split hostname into parts
                ArrayList hostPartsList = new ArrayList(requestUri.Host.Split(hostDelim));

                // step 2: do we need to remove "www"?
                if (removeWWW && hostPartsList[0].ToString() == "www")
                {
                    hostPartsList.RemoveAt(0);
                }

                // step 3: do we need to remove TLD?
                if (removeTLD)
                {
                    hostPartsList.Reverse();
                    if (hostPartsList.Count > 2 && hostPartsList[0].ToString().Length == 2)
                    {
                        // this is a ccTLD, so need to check if next segment is a pseudo-gTLD
                        ArrayList gTLDs = new ArrayList(secondLevelDomains.Split(new char[] {';'}));
                        if (gTLDs.Contains(hostPartsList[1].ToString()))
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
                alias = String.Join(".", (string[]) hostPartsList.ToArray(typeof (String)));
                //return true;
            }
            else
            {
                alias = defaultPortal;
                //return true;
            }
        }
    }
}
