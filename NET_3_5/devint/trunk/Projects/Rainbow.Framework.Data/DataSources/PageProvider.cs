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
    public abstract class PageProvider : ProviderBase
    {
        #region Provider

        /// <summary>
        /// Camel case. Must match web.config section name
        /// </summary>
        private const string providerType = "pageDataSource";

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static PageProvider Instance()
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
                    throw new Exception("Unable to load provider", e);
                }
            }
            return (PageProvider)cache[cacheKey];
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
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public abstract IPage GetById(Guid Id);
        /// <summary>
        /// Adds the specified new Page.
        /// </summary>
        /// <param name="newPage">The new Page.</param>
        public abstract void Add(ref IPage newPage);
        public abstract IPage CreateNew();
        /// <summary>
        /// Updates the specified Page.
        /// </summary>
        /// <param name="Page">The Page.</param>
        public abstract void Update(IPage Page);
        /// <summary>
        /// Removes the specified Page.
        /// </summary>
        /// <param name="Page">The Page.</param>
        public abstract void Remove(IPage Page);
        /// <summary>
        /// Commits the changes.
        /// </summary>
        public abstract void CommitChanges();
    }
}
