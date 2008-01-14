using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Rainbow.Framework.Exceptions; //RainbowRedirectException, PortalsLockedException
using Rainbow.Framework.Helpers; //IPList

namespace Rainbow.Framework.Context
{
    ///<summary>
    ///</summary>
    public interface IRainbowContext : IHttpContextStrategy, IConfigStrategy
    { }

    ///<summary>
    /// Facade for providers.
    ///</summary>
    public class RainbowContext : IRainbowContext
    {
        const string QueryStringKey_PageID = "PageID"; // Standardize text PageID
        const string QueryStringKey_TabID = "TabID"; // Support for old TabID

        static readonly RainbowContext current =
            new RainbowContext(new WebHttpContextStrategy(), new ConfigurationManagerStrategy());

        readonly IHttpContextStrategy rainbowContextReader;
        readonly IConfigStrategy rainbowConfigReader;

        ///<summary>
        /// Gets current Rainbow Context
        ///</summary>
        public static RainbowContext Current
        {
            get { return current; }
        }

        #region IHttpContextStrategy members

        ///<summary>
        /// HttpContext that is gotten by current strategy
        ///</summary>
        public HttpContext HttpContext
        {
            get
            {
                return rainbowContextReader.HttpContext;
            }
        }

        #endregion

        #region IConfigStrategy members

        public string GetAppSetting(string key)
        {
            return rainbowConfigReader.GetAppSetting(key);
        }

        #endregion

        /// <summary>
        /// Gets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        public int PageID
        {
            get
            {
                string strPageID = null;

                if (FindPageIdFromQueryString(rainbowContextReader.HttpContext.Request.QueryString, ref strPageID))
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
                if (rainbowContextReader.HttpContext.Items["PortalAlias"] == null) // not already in context
                {
                    string uniquePortalID = Config.DefaultPortal; // set default value
                    FindAlias(rainbowContextReader.HttpContext.Request, ref uniquePortalID); // will change uniquePortalID if it can
                    rainbowContextReader.HttpContext.Items.Add("PortalAlias", uniquePortalID); // add to context
                    return uniquePortalID; // return current value
                }
                else // already in context
                {
                    return (string)rainbowContextReader.HttpContext.Items["PortalAlias"]; // return from context
                }
            }
        }

        RainbowContext(IHttpContextStrategy contextReader, IConfigStrategy configReader)
        {
            rainbowContextReader = contextReader;
            rainbowConfigReader = configReader;
        }

        ///<summary>
        /// Rewrites path
        /// 1st Check: is it a dangerously malformed request?
        /// Important patch http://support.microsoft.com/?kbid=887459
        ///</summary>
        /// <exception cref="RainbowRedirectException"></exception>
        public void RewritePath()
        {
            string currentURL = HttpContext.Request.Path.ToLower();

            HttpContext.Trace.Warn("Application_BeginRequest :: " + currentURL);
            if (PageID > 0)
            {
                //Creates the physical path on the server 
                string physicalPath = HttpContext.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/") + 1));

                // TODO: Can we enhance performance here by checking to see if it is a friedly url page
                // name instead of doing an IO check for exists?
                // checks to see if the file does not exsists.
                if (!File.Exists(physicalPath)) // Rewrites the path
                {
                    HttpContext.RewritePath("~/default.aspx?" + HttpContext.Request.ServerVariables["QUERY_STRING"]);
                }
            }
            else
            {
                string pname = currentURL.Substring(currentURL.LastIndexOf("/") + 1);
                pname = pname.Substring(0, (pname.Length - 5));
                if (Regex.IsMatch(pname, @"^\d+$"))
                {
                    HttpContext.RewritePath("~/default.aspx?pageid=" + pname +
                                            HttpContext.Request.ServerVariables["QUERY_STRING"]);
                }
            }

            // 1st Check: is it a dangerously malformed request?
            //Important patch http://support.microsoft.com/?kbid=887459
            if (HttpContext.Request.Path.IndexOf('\\') >= 0 ||
                System.IO.Path.GetFullPath(HttpContext.Request.PhysicalPath) != HttpContext.Request.PhysicalPath)
            {
                throw new RainbowRedirectException(LogLevel.Warn, HttpStatusCode.NotFound, 
                                                   "Malformed request", null);
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
                if (queryString[QueryStringKey_PageID] != null)
                {
                    queryStringValues = queryString.GetValues(QueryStringKey_PageID);
                }
                else if (queryString[QueryStringKey_TabID] != null)
                {
                    queryStringValues = queryString.GetValues(QueryStringKey_TabID);
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
                string rawUrl = HttpContext.Request.RawUrl.ToLower(CultureInfo.InvariantCulture);
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
                    if (!ipList.CheckNumber(HttpContext.Request.UserHostAddress))
                    {
                        throw new PortalsLockedException();
                    }
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
