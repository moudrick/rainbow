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
    /// The portal provider.
    /// </summary>
    public abstract class PortalProvider : ProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortalProvider"/> class.
        /// </summary>
        protected PortalProvider()
        {
        }

        #region Constants and Fields

        /// <summary>
        ///     Camel case. Must match web.config section name
        /// </summary>
        private const string ProviderType = "portalDataSource";

        /// <summary>
        ///     private instance
        /// </summary>
        private static PortalProvider instance;

        #endregion

        #region Public Methods

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        public static PortalProvider Instance()
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
                var cacheKey = "Rainbow::Data::PortalDataSource::" + config.DefaultProvider;

                if (cache[cacheKey] == null)
                {
                    // The assembly should be in \bin or GAC, so we simply need
                    // to get an instance of the type
                    try
                    {
                        cache.Insert(
                            cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof(PortalProvider)));
                    }
                    catch (Exception e)
                    {
                        throw new ProviderException("Unable to load provider", e);
                    }
                }

                instance = (PortalProvider)cache[cacheKey];
            }

            return instance;
        }

        /// <summary>
        /// Adds the specified new Portal.
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        public abstract void Add(IPortal portal);

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public abstract void CommitChanges();

        /// <summary>
        /// Creates a new blank IPortal object.
        /// </summary>
        /// <returns>
        /// </returns>
        public abstract IPortal CreateNew();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>
        /// </returns>
        public abstract IEnumerable<IPortal> GetAll();

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public abstract IPortal GetById(Guid id);

        /// <summary>
        /// Removes the specified Portal.
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        public abstract void Remove(IPortal portal);

        /// <summary>
        /// Updates the specified Portal.
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        public abstract void Update(IPortal portal);

        #endregion
    }
}