// Created by John Mandia (john.mandia@whitelightsolutions.com)
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Web;
using Rainbow.Framework.Providers;

using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Caching;
using Rainbow.Framework.Context;

namespace Rainbow.Framework.Providers.MsSql
{
    /// <summary>
    /// Rainbow standard implementation.
    /// This code has been developed and extended by John Mandia (www.whitelightsolutions.com), 
    /// Manu (www.duemetri.com), Jes (www.marinateq.com) and Cory.
    /// </summary>
    public class SqlUrlBuilderProvider : UrlBuilderProvider
    {
        sealed class UrlBuilderHelper
        {
            public const string IsPlaceHolderID = "TabPlaceholder";
            public const string TabLinkID = "TabLink";
            public const string PageNameID = "UrlPageName";
            public const string UrlKeywordsID = "TabUrlKeyword";

            /// <summary>
            /// ApplicationPath, Application dependent relative Application Path.
            /// Base dir for all portal code
            /// Since it is common for all portals is declared as static
            /// </summary>
            public static string ApplicationPath
            {
                get { return Path.ApplicationRoot; }
            }

            /// <summary>
            ///     Returns the current site's database connection string
            /// </summary>
            static string SiteConnectionString
            {
                get { return Config.ConnectionString; }
            }

            UrlBuilderHelper()
            {
            }

            /// <summary>
            /// Clears all cached url elements for a given page
            /// </summary>
            /// <param name="pageID"></param>
            public static void ClearUrlElements(int pageID)
            {
                Cache applicationCache = HttpContext.Current.Cache;

                string placeHolderCacheKey = UrlElementCacheKey(pageID, IsPlaceHolderID);
                string tabLinkCacheKey = UrlElementCacheKey(pageID, TabLinkID);
                string pageNameCacheKey = UrlElementCacheKey(pageID, PageNameID);
                string urlKeywordsCacheKey = UrlElementCacheKey(pageID, UrlKeywordsID);

                if (applicationCache[placeHolderCacheKey] != null)
                    applicationCache.Remove(placeHolderCacheKey);

                if (applicationCache[tabLinkCacheKey] != null)
                    applicationCache.Remove(tabLinkCacheKey);

                if (applicationCache[pageNameCacheKey] != null)
                    applicationCache.Remove(pageNameCacheKey);

                if (applicationCache[urlKeywordsCacheKey] != null)
                    applicationCache.Remove(urlKeywordsCacheKey);
            }

            /// <summary>
            /// This method is used to retrieve a specific Url Property
            /// </summary>
            /// <param name="pageID">The ID of the page the Url belongs to</param>
            /// <param name="propertyID">The ID of the property you are interested in</param>
            /// <param name="cacheDuration">The number of minutes you want to cache this returned value for</param>
            /// <returns>A string value representing the property you are interested in</returns>
            public static string PageSpecificProperty(int pageID, string propertyID, double cacheDuration)
            {
                // Page 0 is shared across portals as a default setting (It doesn't have any real data associated with it so return defaults);
                if (pageID == 0)
                {
                    if (propertyID == IsPlaceHolderID)
                    {
                        return "False";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

                // get the unique cache key for the property requested
                string uniquePropertyCacheKey = UrlElementCacheKey(pageID, propertyID);

                // calling HttpContext.Current.Cache all the time incurs a small performance hit so get a reference to it once and reuse that for greater performance
                Cache applicationCache = HttpContext.Current.Cache;

                if (applicationCache[uniquePropertyCacheKey] == null)
                {
                    string property = string.Empty;

                    using (SqlConnection conn = new SqlConnection(SiteConnectionString))
                    {
                        try
                        {
                            // Open the connection
                            conn.Open();

                            using (SqlCommand cmd = new SqlCommand("SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID + " AND SettingName = '" + propertyID + "'", conn))
                            {
                                // 1. Instantiate a new command above
                                // 2. Call ExecuteNonQuery to send command
                                property = (string)cmd.ExecuteScalar();
                            }
                        }

                        catch
                        {
                            ;// TODO: Decide whether or not this should be logged. If it is a large site upgrading then it would quickly fill up a log file.
                            // If there is no value in the database then it thows an error as it is expecting something.
                            // This can happen with the initial setup or if no entries for a tab have been made
                        }

                        finally
                        {
                            conn.Close();
                        }
                    }

                    // if null is returned always ensure that either a bool (if it is a TabPlaceholder) or an empty string is returned.
                    if ((property == null) || (property.Length == 0))
                    {
                        // Check to make sure it is not a placeholder...if it is change it to false otherwise ensure that it's value is ""
                        if (propertyID == IsPlaceHolderID)
                            property = "False";

                        else
                            property = string.Empty;
                    }

                    else
                    {
                        // Just check to see that it is cleaned before caching it (i.e. removing illegal characters)
                        // If this section grows too much I will clean it up into methods instead of using if else checks.
                        if ((propertyID == PageNameID) || (propertyID == UrlKeywordsID))
                        {
                            // Replace any illegal characters such as space and special characters and replace it with "-"
                            property = Regex.Replace(property, @"[^A-Za-z0-9]", "-");
                            if (propertyID == PageNameID)
                                property += ".aspx";
                        }
                    }

                    // NOTE: Below you will see an implementation that has been commented out as it didn't seem to work well with the tabsetting cache dependency and always retrieved it again and again.
                    //       If someone can figure out why it cant see the cached value please apply the fix and switch the implementation back as it is more ideal (would allow users to see their changes straight away)

                    // If this changes it means that the tabsettings have changed which means the urlkeyword, tablink or placeholder status has changed

                    // String[] dependencyKey = new String[1];
                    // dependencyKey[0] = Rainbow.Framework.Settings.Cache.Key.TabSettings(pageID);
                    // applicationCache.Insert(uniquePropertyCacheKey, property, new CacheDependency(null, dependencyKey));

                    if (cacheDuration == 0)
                    {
                        applicationCache.Insert(uniquePropertyCacheKey, property);
                    }
                    else
                    {
                        applicationCache.Insert(uniquePropertyCacheKey, property, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
                    }
                    return property;
                }

                else
                    return applicationCache[uniquePropertyCacheKey].ToString();
            }

            /// <summary>
            /// This method is used to get all Url Elements in one go
            /// </summary>
            /// <param name="pageID">The ID of the page you are interested in</param>
            /// <param name="cacheDuration">The length of time these values should be cached once retrieved</param>
            /// <param name="_isPlaceHolder">Is this url a place holder (Not a real url)</param>
            /// <param name="_tabLink">Is this Url a link to an external site/resource</param>
            /// <param name="_urlKeywords">Are there any keywords that should be added to this url</param>
            /// <param name="_pageName">Does this url have a friendly page name other than the default</param>
            public static void GetUrlElements(int pageID, double cacheDuration, ref bool _isPlaceHolder, ref string _tabLink, ref string _urlKeywords, ref string _pageName)
            {
                // pageID 0 is a default page shared across portals with no real settings
                if (pageID == 0)
                    return;

                string isPlaceHolderKey = UrlElementCacheKey(pageID, IsPlaceHolderID);
                string tabLinkKey = UrlElementCacheKey(pageID, TabLinkID);
                string pageNameKey = UrlElementCacheKey(pageID, PageNameID);
                string urlKeywordsKey = UrlElementCacheKey(pageID, UrlKeywordsID);

                // calling HttpContext.Current.Cache all the time incurs a small performance hit so get a reference to it once and reuse that for greater performance
                Cache applicationCache = HttpContext.Current.Cache;

                // if any values are null refetch
                if (applicationCache[isPlaceHolderKey] == null || applicationCache[tabLinkKey] == null || applicationCache[pageNameKey] == null || applicationCache[urlKeywordsKey] == null)
                {
                    using (SqlConnection conn = new SqlConnection(SiteConnectionString))
                    {
                        try
                        {
                            // Open the connection
                            conn.Open();

                            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID + " AND SettingName = '" + PageNameID + "'),'') as PageName,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID + " AND SettingName = '" + UrlKeywordsID + "'),'') as Keywords,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID + " AND SettingName = '" + TabLinkID + "'),'') as ExternalLink,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID + " AND SettingName = '" + IsPlaceHolderID + "'),'') as IsPlaceHolder", conn))
                            {
                                // 1. Instantiate a new command above
                                // 2. populate values
                                SqlDataReader pageElements = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                                if (pageElements.HasRows)
                                {
                                    pageElements.Read();

                                    // NOTE: Below you will see an implementation that has been commented out as it didn't seem to work well with the tabsetting cache dependency and always retrieved it again and again.
                                    //       If someone can figure out why it cant see the cached value please apply the fix and switch the implementation back as it is more ideal (would allow users to see their changes straight away)

                                    // If this changes it means that the tabsettings have changed which means the urlkeyword, tablink or placeholder status has changed
                                    // String[] dependencyKey = new String[1];
                                    // dependencyKey[0] = Rainbow.Framework.Settings.Cache.Key.TabSettings(pageID);

                                    if (pageElements["PageName"].ToString() != String.Empty)
                                    {
                                        _pageName = Convert.ToString(pageElements["PageName"]);
                                        _pageName = Regex.Replace(_pageName, @"[^A-Za-z0-9]", "-");
                                        _pageName += ".aspx";

                                        // insert value in cache so it doesn't always try to retrieve it

                                        // NOTE: This is the tabsettings Cache Dependency approach see note above
                                        // applicationCache.Insert(pageNameKey, _pageName, new CacheDependency(null, dependencyKey));
                                        if (cacheDuration == 0)
                                        {
                                            applicationCache.Insert(pageNameKey, _pageName);
                                        }
                                        else
                                        {
                                            applicationCache.Insert(pageNameKey, _pageName, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
                                        }
                                    }
                                    else
                                    {
                                        // insert value in cache so it doesn't always try to retrieve it add empty string so as not to use up too much resources

                                        // NOTE: This is the tabsettings Cache Dependency approach see note above
                                        // applicationCache.Insert(pageNameKey, string.Empty, new CacheDependency(null, dependencyKey));
                                        if (cacheDuration == 0)
                                        {
                                            applicationCache.Insert(pageNameKey, string.Empty);
                                        }
                                        else
                                        {
                                            applicationCache.Insert(pageNameKey, string.Empty, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
                                        }
                                    }

                                    if (pageElements["Keywords"].ToString() != String.Empty)
                                    {
                                        _urlKeywords = Convert.ToString(pageElements["Keywords"]);
                                        _urlKeywords = Regex.Replace(_urlKeywords, @"[^A-Za-z0-9]", "-");
                                    }
                                    // insert value in cache so it doesn't always try to retrieve it

                                    // NOTE: This is the tabsettings Cache Dependency approach see note above
                                    // applicationCache.Insert(urlKeywordsKey, _urlKeywords, new CacheDependency(null, dependencyKey));								

                                    if (cacheDuration == 0)
                                    {
                                        applicationCache.Insert(urlKeywordsKey, _urlKeywords);
                                    }
                                    else
                                    {
                                        applicationCache.Insert(urlKeywordsKey, _urlKeywords, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
                                    }

                                    if (pageElements["ExternalLink"].ToString() != String.Empty)
                                    {
                                        _tabLink = Convert.ToString(pageElements["ExternalLink"]);
                                    }
                                    // insert value in cache so it doesn't always try to retrieve it

                                    // NOTE: This is the tabsettings Cache Dependency approach see note above
                                    // applicationCache.Insert(tabLinkKey, _tabLink, new CacheDependency(null, dependencyKey));
                                    if (cacheDuration == 0)
                                    {
                                        applicationCache.Insert(tabLinkKey, _tabLink);
                                    }
                                    else
                                    {
                                        applicationCache.Insert(tabLinkKey, _tabLink, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
                                    }

                                    if (pageElements["IsPlaceHolder"].ToString() != String.Empty)
                                    {
                                        _isPlaceHolder = bool.Parse(pageElements["IsPlaceHolder"].ToString());
                                    }
                                    // insert value in cache so it doesn't always try to retrieve it

                                    // NOTE: This is the tabsettings Cache Dependency approach see note above
                                    // applicationCache.Insert(isPlaceHolderKey, _isPlaceHolder.ToString(), new CacheDependency(null, dependencyKey));
                                    if (cacheDuration == 0)
                                    {
                                        applicationCache.Insert(isPlaceHolderKey, _isPlaceHolder.ToString());
                                    }
                                    else
                                    {
                                        applicationCache.Insert(isPlaceHolderKey, _isPlaceHolder.ToString(), null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
                                    }
                                }
                                // close the reader
                                pageElements.Close();
                            }
                        }
                        catch
                        {
                            ;// TODO: Decide whether or not this should be logged. If it is a large site upgrading then it would quickly fill up a log file.
                            // If there is no value in the database then it thows an error as it is expecting something.
                            // This can happen with the initial setup or if no entries for a tab have been made
                        }

                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    // if cached value is empty string then leave it as default
                    if (applicationCache[pageNameKey].ToString() != String.Empty)
                        _pageName = applicationCache[pageNameKey].ToString();

                    _urlKeywords = applicationCache[urlKeywordsKey].ToString();
                    _tabLink = applicationCache[tabLinkKey].ToString();
                    _isPlaceHolder = bool.Parse(applicationCache[isPlaceHolderKey].ToString());
                }
            }

            /// <summary>
            /// Builds up a cache key for Url Elements/Properties
            /// </summary>
            /// <param name="pageID">The ID of the page for which you want to generate a url element cache key for</param>
            /// <param name="UrlElement">The Url element you are after (IsPlaceHolderID/TabLinkID/PageNameID/UrlKeywordsID) constants</param>
            /// <returns>A unique key</returns>
            static string UrlElementCacheKey(int pageID, string UrlElement)
            {
                return string.Concat(RainbowContext.Current.UniqueID, pageID, UrlElement);
            }
        }

        string _defaultSplitter = "__";
        string _handlerFlag = string.Empty;
        bool _aliasInUrl = false;
        bool _langInUrl = false;
        string _ignoreTargetPage = "tablayout.aspx";
        double _cacheMinutes = 5;
        bool _pageidNoSplitter = false;
        string _friendlyPageName = "default.aspx";

        /// <summary> 
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language. 
        /// </summary> 
        /// <param name="targetPage">Linked page</param> 
        /// <param name="pageID">ID of the page</param> 
        /// <param name="modID">ID of the module</param> 
        /// <param name="culture">Client culture</param> 
        /// <param name="customAttributes">Any custom attribute that can be needed. Use the following format...single attribute: paramname--paramvalue . Multiple attributes: paramname--paramvalue/paramname2--paramvalue2/paramname3--paramvalue3 </param> 
        /// <param name="currentAlias">Current Alias</param> 
        /// <param name="urlKeywords">Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.</param> 
        public override string BuildUrl(string targetPage, int pageID, int modID, CultureInfo culture,
                                        string customAttributes, string currentAlias, string urlKeywords)
        {
            bool _isPlaceHolder = false;
            string _tabLink = string.Empty;
            string _urlKeywords = string.Empty;
            string _pageName = _friendlyPageName;

            // Get Url Elements this helper method (Will either retrieve from cache or database)
            UrlBuilderHelper.GetUrlElements(pageID, _cacheMinutes, ref _isPlaceHolder, ref _tabLink, ref _urlKeywords,
                                            ref _pageName);

            //2_aug_2004 Cory Isakson
            //Begin Navigation Enhancements
            if (!(targetPage.ToLower().EndsWith(_ignoreTargetPage.ToLower())))
                // Do not modify URLs when working with TabLayout Administration Page
            {
                // if it is a placeholder it is not supposed to have any url
                if (_isPlaceHolder) return string.Empty;

                // if it is a tab link it means it is a link to an external resource
                if (_tabLink.Length != 0) return _tabLink;
            }
            //End Navigation Enhancements
            StringBuilder sb = new StringBuilder();

            // Obtain ApplicationPath
            if (targetPage.StartsWith("~/"))
            {
                sb.Append(UrlBuilderHelper.ApplicationPath);
                targetPage = targetPage.Substring(2);
            }
            sb.Append("/");

            if (!targetPage.EndsWith(".aspx")) //Images
            {
                sb.Append(targetPage);
                return sb.ToString();
            }

            HttpContext.Current.Trace.Warn("Target Page = " + targetPage);

            // Separate path
            // If page contains path, or it is not an aspx 
            // or handlerFlag is not set: do not use handler
            if (targetPage.LastIndexOf('/') > 0 || !targetPage.EndsWith(".aspx") || _handlerFlag.Length == 0)
            {
                sb.Append(targetPage);
                sb.Append("?");
                // Add pageID to URL
                sb.Append("pageID=");
                sb.Append(pageID.ToString());

                // Add Alias to URL
                if (_aliasInUrl)
                {
                    sb.Append("&alias="); // changed for compatibility with handler
                    sb.Append(currentAlias);
                }

                // Add ModID to URL
                if (modID > 0)
                {
                    sb.Append("&mid=");
                    sb.Append(modID.ToString());
                }

                // Add Language to URL
                if (_langInUrl)
                {
                    sb.Append("&lang="); // changed for compatibility with handler
                    sb.Append(culture.Name); // manu fix: culture.Name
                }

                // Add custom attributes
                if (customAttributes != null && customAttributes != string.Empty)
                {
                    sb.Append("&");
                    customAttributes = customAttributes.Replace("/", "&");
                    customAttributes = customAttributes.Replace(_defaultSplitter, "=");
                    sb.Append(customAttributes);
                }
                return sb.ToString().Replace("&&", "&");
            }
            else // use handler
            {
                // Add smarturl tag
                sb.Append(_handlerFlag);
                sb.Append("/");

                // Add custom Keywords to the Url
                if (urlKeywords != null && urlKeywords != string.Empty)
                {
                    sb.Append(urlKeywords);
                    sb.Append("/");
                }
                else
                {
                    urlKeywords = _urlKeywords;

                    // Add custom Keywords to the Url
                    if (urlKeywords != null && urlKeywords.Length != 0)
                    {
                        sb.Append(urlKeywords);
                        sb.Append("/");
                    }
                }

                // Add Alias to URL
                if (_aliasInUrl)
                {
                    sb.Append("alias");
                    sb.Append(_defaultSplitter + currentAlias);
                    sb.Append("/");
                }

                // Add Language to URL
                if (_langInUrl)
                {
                    sb.Append("lang");
                    sb.Append(_defaultSplitter + culture.Name);
                    sb.Append("/");
                }
                // Add ModID to URL
                if (modID > 0)
                {
                    sb.Append("mid");
                    sb.Append(_defaultSplitter + modID);
                    sb.Append("/");
                }

                // Add custom attributes
                if (customAttributes != null && customAttributes != string.Empty)
                {
                    customAttributes = customAttributes.Replace("&", "/");
                    customAttributes = customAttributes.Replace("=", _defaultSplitter);
                    sb.Append(customAttributes);
                    sb.Append("/");
                }

                if (_pageidNoSplitter)
                {
                    // Add pageID to URL
                    sb.Append( "pageid" );
                    sb.Append( _defaultSplitter + pageID );
                    sb.Append( "/" );
                }
                else
                {
                    sb.Append( pageID );
                    sb.Append( "/" );
                }

                // TODO : Need to fix page names rewrites
                // if (targetPage == DefaultPage)
                //		sb.Append(_pageName);
                //	else
                //		sb.Append(targetPage);
                sb.Append( _friendlyPageName );

                //Return page
                return sb.ToString().Replace("//", "/");
            }
        }

        /// <summary>
        /// The initialize method lets you retrieve provider specific settings from web.config
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configValue"></param>
        public override void Initialize(string name, NameValueCollection configValue)
        {

            base.Initialize( name, configValue );

            // For legacy support first check provider settings then web.config/rainbow.config legacy settings
            if (configValue["handlersplitter"] != null)
            {
                _defaultSplitter = configValue["handlersplitter"];
            }
            else
            {
                if (ConfigurationManager.AppSettings["HandlerDefaultSplitter"] != null)
                    _defaultSplitter = ConfigurationManager.AppSettings["HandlerDefaultSplitter"];
            }

            // For legacy support first check provider settings then web.config/rainbow.config legacy settings
            if (configValue["handlerflag"] != null)
            {
                _handlerFlag = configValue["handlerflag"];
            }
            else
            {
                if (ConfigurationManager.AppSettings["HandlerFlag"] != null)
                    _handlerFlag = ConfigurationManager.AppSettings["HandlerFlag"];
            }

            // For legacy support first check provider settings then web.config/rainbow.config legacy settings
            if (configValue["aliasinurl"] != null)
            {
                _aliasInUrl = bool.Parse(configValue["aliasinurl"]);
            }
            else
            {
                if (ConfigurationManager.AppSettings["UseAlias"] != null)
                    _aliasInUrl = bool.Parse(ConfigurationManager.AppSettings["UseAlias"]);
            }

            // For legacy support first check provider settings then web.config/rainbow.config legacy settings
            if (configValue["langinurl"] != null)
            {
                _langInUrl = bool.Parse(configValue["langinurl"]);
            }
            else
            {
                if (ConfigurationManager.AppSettings["LangInURL"] != null)
                    _langInUrl = bool.Parse(ConfigurationManager.AppSettings["LangInURL"]);
            }

            if (configValue["ignoretargetpage"] != null)
            {
                _ignoreTargetPage = configValue["ignoretargetpage"];
            }

            if (configValue["cacheminutes"] != null)
            {
                _cacheMinutes = Convert.ToDouble(configValue["cacheminutes"]);
            }

            if (configValue["pageidnosplitter"] != null)
            {
                _pageidNoSplitter = bool.Parse(configValue["pageidnosplitter"]);
            }
            else {
                if ( ConfigurationManager.AppSettings[ "PageIdNoSplitter" ] != null )
                    _pageidNoSplitter = bool.Parse( ConfigurationManager.AppSettings[ "PageIdNoSplitter" ] );
            }

            // For legacy support first check provider settings then web.config/rainbow.config legacy settings
            if ( configValue[ "friendlypagename" ] != null ) {
                // TODO: Friendly url's need to be fixed
                _friendlyPageName = configValue[ "friendlypagename" ];
            }
            else {
                if ( ConfigurationManager.AppSettings[ "FriendlyPageName" ] != null )
                    _friendlyPageName = ConfigurationManager.AppSettings[ "FriendlyPageName" ];
            }
        }

        /// <summary> 
        /// Determines if a tab is simply a placeholder in the navigation
        /// </summary> 
        public override bool IsPlaceholder(int pageID)
        {
            return
                bool.Parse(
                    UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.IsPlaceHolderID, _cacheMinutes));
        }

        /// <summary> 
        /// Returns the URL for a tab that is a link only.
        /// </summary> 
        public override string TabLink(int pageID)
        {
            return UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.TabLinkID, _cacheMinutes);
        }

        /// <summary> 
        /// Returns any keywords which are meant to be placed in the url
        /// </summary> 
        public override string UrlKeyword(int pageID)
        {
            return
                UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.UrlKeywordsID, _cacheMinutes);
        }

        /// <summary> 
        /// Returns the page name that has been specified. 
        /// </summary> 
        public override string UrlPageName(int pageID)
        {
            string _urlPageName =
                UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.PageNameID, _cacheMinutes);
            // TODO: URL Firendly names need to be fixed
            if (_urlPageName.Length == 0)
                _urlPageName = _friendlyPageName;

            return _urlPageName;
        }

        /// <summary>
        /// Gets the default page from web.config/rainbow.config
        /// </summary>
        public override string DefaultPage
        {
            get
            {
                // TODO: Jes1111 - check this with John
                //string strTemp = ConfigurationSettings.AppSettings["HandlerTargetUrl"];

                // TODO : JONATHAN - PROBLEM WITH DEFAULT PAGE LIKE THIS
                string strTemp = _friendlyPageName;
                // TODO : JONATHAN - PROBLEM WITH DEFAULT PAGE LIKE THIS
                if (strTemp.Length == 0 || strTemp == null)
                {
                    strTemp = "Default.aspx";
                }
                return strTemp;
            }
        }

        /// <summary>
        /// Returns the default paramater splitter from provider settings (or web.config/rainbow.config if not specified in provider) 
        /// </summary>
        public override string DefaultSplitter
        {
            get { return _defaultSplitter; }
        }

        /// <summary> 
        /// Clears the cached url element settings
        /// </summary> 
        public override void Clear(int pageID)
        {
            UrlBuilderHelper.ClearUrlElements(pageID);
        }
    }
}