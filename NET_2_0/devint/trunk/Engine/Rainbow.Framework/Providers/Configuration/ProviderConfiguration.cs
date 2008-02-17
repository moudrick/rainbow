using System;
using System.Collections;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Caching;
using System.Xml;

namespace Rainbow.Framework.Providers.Configuration
{
    /// <summary>
    /// </summary>
    public class ProviderConfiguration
    {
        string defaultProvider;
        readonly Hashtable providers = new Hashtable();

        /// <summary>
        /// Gets the default provider
        /// </summary>
        /// <value>The default provider.</value>
        public string DefaultProvider
        {
            get { return defaultProvider; }
        }

        /// <summary>
        /// Gets the loaded providers
        /// </summary>
        /// <value>The providers.</value>
        public Hashtable Providers
        {
            get { return providers; }
        }

        /// <summary>
        /// Gets the configuration object for the specified provider
        /// </summary>
        /// <param name="provider">Name of the provider object to retrieve</param>
        /// <returns></returns>
        public static ProviderConfiguration GetProviderConfiguration(string provider)
        {
            return (ProviderConfiguration) ConfigurationManager.GetSection("providers/" + provider);
        }

        public static ProviderType GetDefaultProviderFromCache<ProviderType>(string providerType, Cache cache)
            where ProviderType : ProviderBase
        {
            ProviderConfiguration providerConfiguration = GetProviderConfiguration(providerType);
            ProviderSettings providerSettings = (ProviderSettings)providerConfiguration
                                                                      .Providers[providerConfiguration.DefaultProvider];
            string cacheKey = string.Format("Rainbow::Web::Provider::{0}::{1}",
                                            providerType, providerConfiguration.DefaultProvider);

            if (cache[cacheKey] == null)
            {
                // The assembly should be in \bin or GAC, so we simply need
                // to get an instance of the type
                try
                {
                    cache.Insert(cacheKey,
                                 InstantiateProvider(providerSettings, typeof(ProviderType)));
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to load provider", e);
                }
            }
            return (ProviderType)cache[cacheKey];
        }

        /// <summary>
        /// Instantiates the provider.
        /// </summary>
        /// <param name="providerSettings">The provider settings.</param>
        /// <param name="provType">Type of the prov.</param>
        /// <returns></returns>
        public static ProviderBase InstantiateProvider(ProviderSettings providerSettings, Type provType)
        {
            if ((providerSettings.Type == null) || (providerSettings.Type.Length < 1))
                throw new ConfigurationErrorsException(
                    "Provider could not be instantiated. The Type parameter cannot be null.");

            Type providerType = Type.GetType(providerSettings.Type);
            if (providerType == null)
                throw new ConfigurationErrorsException(
                    "Provider could not be instantiated. The Type could not be found.");

            if (!provType.IsAssignableFrom(providerType))
                throw new ConfigurationErrorsException("Provider must implement type \'" + provType + "\'.");

            object providerObj = Activator.CreateInstance(providerType);
            if (providerObj == null)
                throw new ConfigurationErrorsException("Provider could not be instantiated.");

            ProviderBase providerBase = ((ProviderBase) providerObj);

            providerBase.Initialize(providerSettings.Name, providerSettings.Parameters);
            return providerBase;
        }

        /// <summary>
        /// Loads provider information from the configuration node
        /// </summary>
        /// <param name="node">Node representing configuration information</param>
        public void LoadValuesFromConfigurationXml(XmlNode node)
        {
            XmlAttributeCollection attributeCollection = node.Attributes;

            // Get the default provider
            defaultProvider = attributeCollection["defaultProvider"].Value;

            // Read child nodes
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "providers")
                {
                    GetProviders(child);
                }
            }
        }

        /// <summary>
        /// Configures Provider(s) based on the configuration node
        /// </summary>
        /// <param name="node"></param>
        void GetProviders(XmlNode node)
        {
            foreach (XmlNode provider in node.ChildNodes)
            {
                switch (provider.Name)
                {
                    case "add":
                        providers.Add(provider.Attributes["name"].Value,
                                      new ProviderSettings(provider.Attributes["name"].Value,
                                                           provider.Attributes["type"].Value));
                        break;

                    case "remove":
                        providers.Remove(provider.Attributes["name"].Value);
                        break;

                    case "clear":
                        providers.Clear();
                        break;
                }
            }
        }
    }
}