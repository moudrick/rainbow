using System;
using System.Configuration;
using System.Configuration.Provider;

namespace Rainbow.Framework.Provider
{
    /// <summary>
    /// Summary description for ProviderHelper.
    /// </summary>
    public sealed class ProviderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private ProviderHelper()
        {
        }

        /// <summary>
        /// Instantiates the provider.
        /// </summary>
        /// <param name="providerSettings">The provider settings.</param>
        /// <param name="provType">Type of the prov.</param>
        /// <returns></returns>
        public static ProviderBase InstantiateProvider(ProviderSettings providerSettings, Type providerTypeToInstantiate)
        {
            if ((providerSettings.Type == null) || (providerSettings.Type.Length < 1))
                throw new ConfigurationErrorsException(
                    "Provider could not be instantiated. The Type parameter cannot be null.");

            Type providerType = Type.GetType(providerSettings.Type);
            if (providerType == null)
                throw new ConfigurationErrorsException(
                    "Provider could not be instantiated. The Type could not be found.");

            if (!providerTypeToInstantiate.IsAssignableFrom(providerType))
                throw new ConfigurationErrorsException("Provider must implement type \'" + providerTypeToInstantiate.ToString() + "\'.");

            object providerObj = Activator.CreateInstance(providerType);
            if (providerObj == null)
                throw new ConfigurationErrorsException("Provider could not be instantiated.");

            ProviderBase providerBase = ((ProviderBase) providerObj);

            try
            {
                providerBase.Initialize(providerSettings.Name, providerSettings.Parameters);
                return providerBase;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Instantiates the providers.
        /// </summary>
        /// <param name="configProviders">The config providers.</param>
        /// <param name="providers">The providers.</param>
        /// <param name="typeOfProvider">The type of provider.</param>
        public static void InstantiateProviders(ProviderCollection configProviders, ref ProviderCollection providers,
                                                Type typeOfProvider)
        {
            foreach (ProviderSettings providerSettings in configProviders)
            {
                providers.Add(InstantiateProvider(providerSettings, typeOfProvider));
            }
        }
    }
}