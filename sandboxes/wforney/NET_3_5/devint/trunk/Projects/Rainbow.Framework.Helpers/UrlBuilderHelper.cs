// Created by John Mandia (john.mandia@whitelightsolutions.com)
namespace Rainbow.Framework.Helpers
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;

    using Rainbow.Framework.BusinessLogic;
    using Rainbow.Framework.Configuration;

    /// <summary>
    /// Summary description for Helper.
    /// </summary>
    internal static class UrlBuilderHelper
    {
        #region Constants and Fields

        /// <summary>
        /// The is place holder id.
        /// </summary>
        public const string IsPlaceHolderId = "TabPlaceholder";

        /// <summary>
        /// The page name id.
        /// </summary>
        public const string PageNameId = "UrlPageName";

        /// <summary>
        /// The tab link id.
        /// </summary>
        public const string TabLinkId = "TabLink";

        /// <summary>
        /// The url keywords id.
        /// </summary>
        public const string UrlKeywordsId = "TabUrlKeyword";

        #endregion

        #region Properties

        /// <summary>
        ///     ApplicationPath, Application dependent relative Application Path.
        ///     Base dir for all portal code
        ///     Since it is common for all portals is declared as static
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                return Path.ApplicationRoot;
            }
        }

        /// <summary>
        /// Gets the current site's database connection string
        /// </summary>
        /// <value>The site connection string.</value>
        private static string SiteConnectionString
        {
            get
            {
                return Settings.ConnectionString;
            }
        }

        /// <summary>
        ///     This static string fetches the site's alias either via querystring, cookie or domain and returns it
        /// </summary>
        private static string SiteUniqueId
        {
            get
            {
                return Portal.UniqueId;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all cached url elements for a given page
        /// </summary>
        /// <param name="pageId">
        /// </param>
        public static void ClearUrlElements(int pageId)
        {
            var applicationCache = HttpContext.Current.Cache;

            var placeHolderCacheKey = UrlElementCacheKey(pageId, IsPlaceHolderId);
            var tabLinkCacheKey = UrlElementCacheKey(pageId, TabLinkId);
            var pageNameCacheKey = UrlElementCacheKey(pageId, PageNameId);
            var urlKeywordsCacheKey = UrlElementCacheKey(pageId, UrlKeywordsId);

            if (applicationCache[placeHolderCacheKey] != null)
            {
                applicationCache.Remove(placeHolderCacheKey);
            }

            if (applicationCache[tabLinkCacheKey] != null)
            {
                applicationCache.Remove(tabLinkCacheKey);
            }

            if (applicationCache[pageNameCacheKey] != null)
            {
                applicationCache.Remove(pageNameCacheKey);
            }

            if (applicationCache[urlKeywordsCacheKey] != null)
            {
                applicationCache.Remove(urlKeywordsCacheKey);
            }
        }

        /// <summary>
        /// This method is used to get all Url Elements in one go
        /// </summary>
        /// <param name="pageId">
        /// The ID of the page you are interested in
        /// </param>
        /// <param name="cacheDuration">
        /// The length of time these values should be cached once retrieved
        /// </param>
        /// <param name="placeHolder">
        /// Is this url a place holder (Not a real url)
        /// </param>
        /// <param name="tabLink">
        /// Is this Url a link to an external site/resource
        /// </param>
        /// <param name="urlKeywords">
        /// Are there any keywords that should be added to this url
        /// </param>
        /// <param name="pageName">
        /// Does this url have a friendly page name other than the default
        /// </param>
        public static void GetUrlElements(
            int pageId, 
            double cacheDuration, 
            ref bool placeHolder, 
            ref string tabLink, 
            ref string urlKeywords, 
            ref string pageName)
        {
            // pageID 0 is a default page shared across portals with no real settings
            if (pageId == 0)
            {
                return;
            }

            var placeHolderKey = UrlElementCacheKey(pageId, IsPlaceHolderId);
            var tabLinkKey = UrlElementCacheKey(pageId, TabLinkId);
            var pageNameKey = UrlElementCacheKey(pageId, PageNameId);
            var urlKeywordsKey = UrlElementCacheKey(pageId, UrlKeywordsId);

            // calling HttpContext.Current.Cache all the time incurs a small performance hit so get a reference to it once and reuse that for greater performance
            var applicationCache = HttpContext.Current.Cache;

            // if any values are null refetch
            if (applicationCache[placeHolderKey] == null || applicationCache[tabLinkKey] == null ||
                applicationCache[pageNameKey] == null || applicationCache[urlKeywordsKey] == null)
            {
                using (var conn = new SqlConnection(SiteConnectionString))
                {
                    try
                    {
                        // Open the connection
                        conn.Open();

                        using (
                            var cmd =
                                new SqlCommand(
                                    "SELECT ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageId +
                                    " AND SettingName = '" + PageNameId +
                                    "'),'') as PageName,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" +
                                    pageId + " AND SettingName = '" + UrlKeywordsId +
                                    "'),'') as Keywords,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" +
                                    pageId + " AND SettingName = '" + TabLinkId +
                                    "'),'') as ExternalLink,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" +
                                    pageId + " AND SettingName = '" + IsPlaceHolderId + "'),'') as IsPlaceHolder", 
                                    conn))
                        {
                            // 1. Instantiate a new command above
                            // 2. populate values
                            var pageElements = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            if (pageElements.HasRows)
                            {
                                pageElements.Read();

                                // NOTE: Below you will see an implementation that has been commented out as it didn't seem to work well with the tabsetting cache dependency and always retrieved it again and again.
                                // If someone can figure out why it cant see the cached value please apply the fix and switch the implementation back as it is more ideal (would allow users to see their changes straight away)

                                // If this changes it means that the tabsettings have changed which means the urlkeyword, tablink or placeholder status has changed
                                // String[] dependencyKey = new String[1];
                                // dependencyKey[0] = Rainbow.Framework.Settings.Cache.Key.TabSettings(pageID);
                                if (pageElements["PageName"].ToString() != String.Empty)
                                {
                                    pageName = Convert.ToString(pageElements["PageName"]);
                                    pageName = Regex.Replace(pageName, @"[^A-Za-z0-9]", "-");
                                    pageName += ".aspx";

                                    // insert value in cache so it doesn't always try to retrieve it
                                    // NOTE: This is the tabsettings Cache Dependency approach see note above
                                    // applicationCache.Insert(pageNameKey, pageName, new CacheDependency(null, dependencyKey));
                                    if (cacheDuration == 0)
                                    {
                                        applicationCache.Insert(pageNameKey, pageName);
                                    }
                                    else
                                    {
                                        applicationCache.Insert(
                                            pageNameKey, 
                                            pageName, 
                                            null, 
                                            DateTime.Now.AddMinutes(cacheDuration), 
                                            Cache.NoSlidingExpiration);
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
                                        applicationCache.Insert(
                                            pageNameKey, 
                                            string.Empty, 
                                            null, 
                                            DateTime.Now.AddMinutes(cacheDuration), 
                                            Cache.NoSlidingExpiration);
                                    }
                                }

                                if (pageElements["Keywords"].ToString() != String.Empty)
                                {
                                    urlKeywords = Convert.ToString(pageElements["Keywords"]);
                                    urlKeywords = Regex.Replace(urlKeywords, @"[^A-Za-z0-9]", "-");
                                }

                                // insert value in cache so it doesn't always try to retrieve it

                                // NOTE: This is the tabsettings Cache Dependency approach see note above
                                // applicationCache.Insert(urlKeywordsKey, urlKeywords, new CacheDependency(null, dependencyKey));								
                                if (cacheDuration == 0)
                                {
                                    applicationCache.Insert(urlKeywordsKey, urlKeywords);
                                }
                                else
                                {
                                    applicationCache.Insert(
                                        urlKeywordsKey, 
                                        urlKeywords, 
                                        null, 
                                        DateTime.Now.AddMinutes(cacheDuration), 
                                        Cache.NoSlidingExpiration);
                                }

                                if (pageElements["ExternalLink"].ToString() != String.Empty)
                                {
                                    tabLink = Convert.ToString(pageElements["ExternalLink"]);
                                }

                                // insert value in cache so it doesn't always try to retrieve it

                                // NOTE: This is the tabsettings Cache Dependency approach see note above
                                // applicationCache.Insert(tabLinkKey, tabLink, new CacheDependency(null, dependencyKey));
                                if (cacheDuration == 0)
                                {
                                    applicationCache.Insert(tabLinkKey, tabLink);
                                }
                                else
                                {
                                    applicationCache.Insert(
                                        tabLinkKey, 
                                        tabLink, 
                                        null, 
                                        DateTime.Now.AddMinutes(cacheDuration), 
                                        Cache.NoSlidingExpiration);
                                }

                                if (pageElements["IsPlaceHolder"].ToString() != String.Empty)
                                {
                                    placeHolder = bool.Parse(pageElements["IsPlaceHolder"].ToString());
                                }

                                // insert value in cache so it doesn't always try to retrieve it

                                // NOTE: This is the tabsettings Cache Dependency approach see note above
                                // applicationCache.Insert(isPlaceHolderKey, placeHolder.ToString(), new CacheDependency(null, dependencyKey));
                                if (cacheDuration == 0)
                                {
                                    applicationCache.Insert(placeHolderKey, placeHolder.ToString());
                                }
                                else
                                {
                                    applicationCache.Insert(
                                        placeHolderKey, 
                                        placeHolder.ToString(), 
                                        null, 
                                        DateTime.Now.AddMinutes(cacheDuration), 
                                        Cache.NoSlidingExpiration);
                                }
                            }

                            // close the reader
                            pageElements.Close();
                        }
                    }
                    catch
                    {
                        // TODO: Decide whether or not this should be logged. If it is a large site upgrading then it would quickly fill up a log file.
                        // If there is no value in the database then it thows an error as it is expecting something.
                        // This can happen with the initial setup or if no entries for a tab have been made
                    }
                    finally
                    {
                        // Close the connection
                        conn.Close();
                    }
                }
            }
            else
            {
                // if cached value is empty string then leave it as default
                if (applicationCache[pageNameKey].ToString() != String.Empty)
                {
                    pageName = applicationCache[pageNameKey].ToString();
                }

                urlKeywords = applicationCache[urlKeywordsKey].ToString();
                tabLink = applicationCache[tabLinkKey].ToString();
                placeHolder = bool.Parse(applicationCache[placeHolderKey].ToString());
            }
        }

        /// <summary>
        /// This method is used to retrieve a specific Url Property
        /// </summary>
        /// <param name="pageId">
        /// The ID of the page the Url belongs to
        /// </param>
        /// <param name="propertyId">
        /// The ID of the property you are interested in
        /// </param>
        /// <param name="cacheDuration">
        /// The number of minutes you want to cache this returned value for
        /// </param>
        /// <returns>
        /// A string value representing the property you are interested in
        /// </returns>
        public static string PageSpecificProperty(int pageId, string propertyId, double cacheDuration)
        {
            // Page 0 is shared across portals as a default setting (It doesn't have any real data associated with it so return defaults);
            if (pageId == 0)
            {
                return propertyId == IsPlaceHolderId ? "False" : string.Empty;
            }

            // get the unique cache key for the property requested
            var uniquePropertyCacheKey = UrlElementCacheKey(pageId, propertyId);

            // calling HttpContext.Current.Cache all the time incurs a small performance hit so get a reference to it once and reuse that for greater performance
            var applicationCache = HttpContext.Current.Cache;

            if (applicationCache[uniquePropertyCacheKey] == null)
            {
                var property = string.Empty;

                using (var conn = new SqlConnection(SiteConnectionString))
                {
                    try
                    {
                        // Open the connection
                        conn.Open();

                        using (
                            var cmd =
                                new SqlCommand(
                                    "SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageId +
                                    " AND SettingName = '" + propertyId + "'", 
                                    conn))
                        {
                            // 1. Instantiate a new command above
                            // 2. Call ExecuteNonQuery to send command
                            property = (string)cmd.ExecuteScalar();
                        }
                    }
                    catch
                    {
                        // TODO: Decide whether or not this should be logged. If it is a large site upgrading then it would quickly fill up a log file.
                        // If there is no value in the database then it thows an error as it is expecting something.
                        // This can happen with the initial setup or if no entries for a tab have been made
                    }
                    finally
                    {
                        // Close the connection
                        conn.Close();
                    }
                }

                // if null is returned always ensure that either a bool (if it is a TabPlaceholder) or an empty string is returned.
                if (string.IsNullOrEmpty(property))
                {
                    // Check to make sure it is not a placeholder...if it is change it to false otherwise ensure that it's value is ""
                    if (propertyId == IsPlaceHolderId)
                    {
                        property = "False";
                    }
                    else
                    {
                        property = string.Empty;
                    }
                }
                else
                {
                    // Just check to see that it is cleaned before caching it (i.e. removing illegal characters)
                    // If this section grows too much I will clean it up into methods instead of using if else checks.
                    if ((propertyId == PageNameId) || (propertyId == UrlKeywordsId))
                    {
                        // Replace any illegal characters such as space and special characters and replace it with "-"
                        property = Regex.Replace(property, @"[^A-Za-z0-9]", "-");
                        if (propertyId == PageNameId)
                        {
                            property += ".aspx";
                        }
                    }
                }

                // NOTE: Below you will see an implementation that has been commented out as it didn't seem to work well with the tabsetting cache dependency and always retrieved it again and again.
                // If someone can figure out why it cant see the cached value please apply the fix and switch the implementation back as it is more ideal (would allow users to see their changes straight away)

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
                    applicationCache.Insert(
                        uniquePropertyCacheKey, 
                        property, 
                        null, 
                        DateTime.Now.AddMinutes(cacheDuration), 
                        Cache.NoSlidingExpiration);
                }

                return property;
            }
            
            return applicationCache[uniquePropertyCacheKey].ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds up a cache key for Url Elements/Properties
        /// </summary>
        /// <param name="pageId">
        /// The ID of the page for which you want to generate a url element cache key for
        /// </param>
        /// <param name="urlElement">
        /// The Url element you are after (IsPlaceHolderID/TabLinkID/PageNameID/UrlKeywordsID) constants
        /// </param>
        /// <returns>
        /// A unique key
        /// </returns>
        private static string UrlElementCacheKey(int pageId, string urlElement)
        {
            return string.Concat(SiteUniqueId, pageId, urlElement);
        }

        #endregion
    }
}