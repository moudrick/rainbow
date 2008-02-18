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
    public abstract class PortalProvider : ProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortalProvider"/> class.
        /// </summary>
        /// <remarks>
        /// Note: Constructor is 'protected' (Singleton pattern)
        /// </remarks>
        protected PortalProvider() { }

        #region Provider

        /// <summary>
        /// Camel case. Must match web.config section name
        /// </summary>
        private const string providerType = "portalDataSource";

        /// <summary>
        /// private instance
        /// </summary>
        private static PortalProvider _instance;

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static PortalProvider Instance()
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
                cacheKey = "Rainbow::Data::PortalDataSource::" + config.DefaultProvider;

                if (cache[cacheKey] == null)
                {
                    // The assembly should be in \bin or GAC, so we simply need
                    // to get an instance of the type
                    try
                    {
                        cache.Insert(cacheKey,
                                     ProviderHelper.InstantiateProvider(providerSettings, typeof(PortalProvider)));
                    }

                    catch (Exception e)
                    {
                        throw new ProviderException("Unable to load provider", e);
                    }
                }

                _instance = (PortalProvider)cache[cacheKey];
            }

            return _instance;
        }

        #endregion

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IPortal> GetAll();
        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public abstract IPortal GetById(Guid id);
        /// <summary>
        /// Adds the specified new Portal.
        /// </summary>
        /// <param name="portal">The portal.</param>
        public abstract void Add(IPortal portal);
        /// <summary>
        /// Creates a new blank IPortal object.
        /// </summary>
        /// <returns></returns>
        public abstract IPortal CreateNew();
        /// <summary>
        /// Updates the specified Portal.
        /// </summary>
        /// <param name="portal">The portal.</param>
        public abstract void Update(IPortal portal);
        /// <summary>
        /// Removes the specified Portal.
        /// </summary>
        /// <param name="portal">The portal.</param>
        public abstract void Remove(IPortal portal);
        /// <summary>
        /// Commits the changes.
        /// </summary>
        public abstract void CommitChanges();
    }
}
