using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;
using Rainbow.Framework.Provider;
using Rainbow.Framework.Data.Entities;
using System.Configuration.Provider;
using System.Configuration;

namespace Rainbow.Framework.Data.DataSources
{
    /// <summary>
    /// Page Provider
    /// </summary>
    public abstract class PageProvider : ProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageProvider"/> class.
        /// </summary>
        /// <remarks>
        /// Note: Constructor is 'protected' (Singleton pattern)
        /// </remarks>
        protected PageProvider() { }

        #region Provider

        /// <summary>
        /// Camel case. Must match web.config section name
        /// </summary>
        private const string providerType = "pageDataSource";

        /// <summary>
        /// private instance
        /// </summary>
        private static PageProvider _instance;

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static PageProvider Instance()
        {
            // Use 'Lazy initialization'
            if (_instance == null)
            {
                // Use the cache because the reflection used later is expensive
                Cache cache = HttpRuntime.Cache;
                string cacheKey;
                // Get the names of providers
                ProviderConfiguration config = ProviderConfiguration.GetProviderConfiguration(providerType);
                // Read specific configuration information for this provider
                ProviderSettings providerSettings = (ProviderSettings)config.Providers[config.DefaultProvider];
                // In the cache?
                cacheKey = "Rainbow::Data::PageDataSource::" + config.DefaultProvider;

                if (cache[cacheKey] == null)
                {
                    // The assembly should be in \bin or GAC, so we simply need
                    // to get an instance of the type
                    try
                    {
                        cache.Insert(cacheKey,
                                     ProviderHelper.InstantiateProvider(providerSettings, typeof(PageProvider)));
                    }
                    catch (Exception e)
                    {
                        throw new ProviderException("Unable to load provider", e);
                    }
                }

                _instance = (PageProvider)cache[cacheKey];
            }

            return _instance;
        }

        #endregion

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IPage> GetAll();
        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public abstract IPage GetById(Guid id);
        /// <summary>
        /// Adds the specified new Page.
        /// </summary>
        /// <param name="page">The page.</param>
        public abstract void Add(IPage page);
        /// <summary>
        /// Creates a new Page.
        /// </summary>
        /// <returns>The new blank IPage object.</returns>
        public abstract IPage CreateNew();
        /// <summary>
        /// Updates the specified Page.
        /// </summary>
        /// <param name="page">The Page.</param>
        public abstract void Update(IPage page);
        /// <summary>
        /// Removes the specified Page.
        /// </summary>
        /// <param name="page">The Page.</param>
        public abstract void Remove(IPage page);
        /// <summary>
        /// Commits the changes.
        /// </summary>
        public abstract void CommitChanges();
    }
}
