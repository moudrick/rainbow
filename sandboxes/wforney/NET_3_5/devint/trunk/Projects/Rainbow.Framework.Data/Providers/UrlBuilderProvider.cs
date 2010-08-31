// Created by John Mandia (john.mandia@whitelightsolutions.com)
namespace Rainbow.Framework.Data.Providers
{
    using System;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Globalization;
    using System.Web;

    using Rainbow.Framework.Provider;

    /// <summary>
    /// Summary description for UrlBuilderProvider.
    /// </summary>
    public abstract class UrlBuilderProvider : ProviderBase
    {
        #region Constants and Fields

        /// <summary>
        ///     Camel case. Must match web.config section name
        /// </summary>
        private const string ProviderType = "urlBuilder";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the default page.
        /// </summary>
        /// <value>The default page.</value>
        public abstract string DefaultPage { get; }

        /// <summary>
        ///     Gets the default splitter.
        /// </summary>
        /// <value>The default splitter.</value>
        public abstract string DefaultSplitter { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        public static UrlBuilderProvider Instance()
        {
            // Use the cache because the reflection used later is expensive
            var cache = HttpRuntime.Cache;

            // Get the names of providers
            var config = ProviderConfiguration.GetProviderConfiguration(ProviderType);

            // Read specific configuration information for this provider
            var providerSettings = (ProviderSettings)config.Providers[config.DefaultProvider];

            // In the cache?
            string cacheKey = "Rainbow::Web::UrlBuilder::" + config.DefaultProvider;

            if (cache[cacheKey] == null)
            {
                // The assembly should be in \bin or GAC, so we simply need

                // to get an instance of the type
                try
                {
                    cache.Insert(
                        cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof(UrlBuilderProvider)));
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to load provider", e);
                }
            }

            return (UrlBuilderProvider)cache[cacheKey];
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="tabID">
        /// ID of the tab
        /// </param>
        /// <param name="modID">
        /// ID of the module
        /// </param>
        /// <param name="culture">
        /// Client culture
        /// </param>
        /// <param name="customAttributes">
        /// Any custom attribute that can be needed. Use the following format...single attribute: paramname--paramvalue . Multiple attributes: paramname--paramvalue/paramname2--paramvalue2/paramname3--paramvalue3 
        /// </param>
        /// <param name="currentAlias">
        /// Current Alias
        /// </param>
        /// <param name="urlKeywords">
        /// Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public abstract string BuildUrl(
            string targetPage, 
            int tabID, 
            int modID, 
            CultureInfo culture, 
            string customAttributes, 
            string currentAlias, 
            string urlKeywords);

        /// <summary>
        /// Clears any stored settings for the current page
        /// </summary>
        /// <param name="tabID">
        /// The tab ID.
        /// </param>
        public abstract void Clear(int tabID);

        /// <summary>
        /// Determines if a tab is simply a placeholder in the navigation
        /// </summary>
        /// <param name="tabID">
        /// The tab ID.
        /// </param>
        /// <returns>
        /// The is placeholder.
        /// </returns>
        public abstract bool IsPlaceholder(int tabID);

        /// <summary>
        /// Returns the URL for a tab that is a link only.
        /// </summary>
        /// <param name="tabID">
        /// The tab ID.
        /// </param>
        /// <returns>
        /// The tab link.
        /// </returns>
        public abstract string TabLink(int tabID);

        /// <summary>
        /// Returns any keywords which are meant to be placed in the url
        /// </summary>
        /// <param name="tabID">
        /// The tab ID.
        /// </param>
        /// <returns>
        /// The url keyword.
        /// </returns>
        public abstract string UrlKeyword(int tabID);

        /// <summary>
        /// Returns the page name that has been specified.
        /// </summary>
        /// <param name="tabID">
        /// The tab ID.
        /// </param>
        /// <returns>
        /// The url page name.
        /// </returns>
        public abstract string UrlPageName(int tabID);

        #endregion
    }
}