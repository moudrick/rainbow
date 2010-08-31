// Created by Manu
namespace Rainbow.Framework.Logging
{
    using System;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.IO;
    using System.Web;
    using System.Xml;

    using Rainbow.Framework.Provider;

    /// <summary>
    /// Summary description for LogProvider.
    /// </summary>
    public abstract class LogProvider : ProviderBase
    {
        #region Constants and Fields

        /// <summary>
        ///     Camel case. Must match web.config section name
        /// </summary>
        private const string ProviderType = "log";

        #endregion

        #region Public Methods

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns>A LogProvider.</returns>
        public static LogProvider Instance()
        {
            // Use the cache because the reflection used later is expensive
            var cache = HttpRuntime.Cache;

            // Get the names of providers
            ProviderConfiguration config;
            config = ProviderConfiguration.GetProviderConfiguration(ProviderType);

            // If config not found (missing web.config)
            if (config == null)
            {
                // Try to provide a default anyway
                var defaultNode = new XmlDocument();
                defaultNode.LoadXml(
                    "<log defaultProvider=\"Log4NetLog\"><providers><clear /><add name=\"Log4NetLog\" type=\"Rainbow.Framework.Logging.Log4NetLogProvider, Rainbow.Provider.Implementation\" /></providers></log>");

                // Get the names of providers
                config = new ProviderConfiguration();
                config.LoadValuesFromConfigurationXml(defaultNode.DocumentElement);
            }

            // Read specific configuration information for this provider
            var providerSettings = (ProviderSettings)config.Providers[config.DefaultProvider];

            // In the cache?
            string cacheKey = "Rainbow::Configuration::Log::" + config.DefaultProvider;
            if (cache[cacheKey] == null)
            {
                // The assembly should be in \bin or GAC, so we simply need
                // to get an instance of the type
                try
                {
                    cache.Insert(cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof(LogProvider)));
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to load provider", e);
                }
            }

            return (LogProvider)cache[cacheKey];
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public abstract void Log(LogLevels level, object message);

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        public abstract void Log(LogLevels level, object message, Exception t);

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sw">
        /// The sw.
        /// </param>
        public abstract void Log(LogLevels level, object message, StringWriter sw);

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="sw">
        /// The sw.
        /// </param>
        public abstract void Log(LogLevels level, object message, Exception t, StringWriter sw);

        #endregion
    }
}