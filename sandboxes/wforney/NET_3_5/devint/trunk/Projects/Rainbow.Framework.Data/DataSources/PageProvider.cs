namespace Rainbow.Framework.Data.DataSources
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Web;

    using Rainbow.Framework.Interfaces;
    using Rainbow.Framework.Provider;

    /// <summary>
    /// Page Provider
    /// </summary>
    public abstract class PageProvider : ProviderBase
    {
        #region Constants and Fields

        /// <summary>
        ///     Camel case. Must match web.config section name
        /// </summary>
        private const string ProviderType = "pageDataSource";

        /// <summary>
        ///     private instance
        /// </summary>
        private static PageProvider instance;

        #endregion

        #region Public Methods

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        public static PageProvider Instance()
        {
            // Use 'Lazy initialization'
            if (instance == null)
            {
                // Use the cache because the reflection used later is expensive
                var cache = HttpRuntime.Cache;

                // Get the names of providers
                var config = ProviderConfiguration.GetProviderConfiguration(ProviderType);

                // Read specific configuration information for this provider
                var providerSettings = (ProviderSettings)config.Providers[config.DefaultProvider];

                // In the cache?
                var cacheKey = "Rainbow::Data::PageDataSource::" + config.DefaultProvider;

                if (cache[cacheKey] == null)
                {
                    // The assembly should be in \bin or GAC, so we simply need
                    // to get an instance of the type
                    try
                    {
                        cache.Insert(
                            cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof(PageProvider)));
                    }
                    catch (Exception e)
                    {
                        throw new ProviderException("Unable to load provider", e);
                    }
                }

                instance = (PageProvider)cache[cacheKey];
            }

            return instance;
        }

        /// <summary>
        /// Adds the specified new Page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        public abstract void Add(IPage page);

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public abstract void CommitChanges();

        /// <summary>
        /// Creates a new Page.
        /// </summary>
        /// <returns>
        /// The new blank IPage object.
        /// </returns>
        public abstract IPage CreateNew();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>
        /// </returns>
        public abstract IEnumerable<IPage> GetAll();

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public abstract IPage GetById(Guid id);

        /// <summary>
        /// Removes the specified Page.
        /// </summary>
        /// <param name="page">
        /// The Page.
        /// </param>
        public abstract void Remove(IPage page);

        /// <summary>
        /// Updates the specified Page.
        /// </summary>
        /// <param name="page">
        /// The Page.
        /// </param>
        public abstract void Update(IPage page);

        #endregion
    }
}