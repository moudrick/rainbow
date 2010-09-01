namespace Rainbow.Framework.Provider
{
    using System;
    using System.Configuration;
    using System.Configuration.Provider;

    /// <summary>
    /// Provider Helper
    /// </summary>
    public static class ProviderHelper
    {
        #region Public Methods

        /// <summary>
        /// Instantiates the provider.
        /// </summary>
        /// <param name="providerSettings">
        /// The provider settings.
        /// </param>
        /// <param name="providerTypeToInstantiate">
        /// The provider Type To Instantiate.
        /// </param>
        /// <returns>
        /// </returns>
        public static ProviderBase InstantiateProvider(
            ProviderSettings providerSettings, Type providerTypeToInstantiate)
        {
            if (string.IsNullOrEmpty(providerSettings.Type))
            {
                throw new ConfigurationErrorsException(
                    "Provider could not be instantiated. The Type parameter cannot be null.");
            }

            var providerType = Type.GetType(providerSettings.Type);
            if (providerType == null)
            {
                throw new ConfigurationErrorsException(
                    "Provider could not be instantiated. The Type could not be found.");
            }

            if (!providerTypeToInstantiate.IsAssignableFrom(providerType))
            {
                throw new ConfigurationErrorsException(
                    "Provider must implement type \'" + providerTypeToInstantiate + "\'.");
            }

            var providerObj = Activator.CreateInstance(providerType);
            if (providerObj == null)
            {
                throw new ConfigurationErrorsException("Provider could not be instantiated.");
            }

            var providerBase = (ProviderBase)providerObj;
            providerBase.Initialize(providerSettings.Name, providerSettings.Parameters);
            return providerBase;
        }

        /// <summary>
        /// Instantiates the providers.
        /// </summary>
        /// <param name="configProviders">
        /// The config providers.
        /// </param>
        /// <param name="providers">
        /// The providers.
        /// </param>
        /// <param name="typeOfProvider">
        /// The type of provider.
        /// </param>
        public static void InstantiateProviders(
            ProviderCollection configProviders, ref ProviderCollection providers, Type typeOfProvider)
        {
            foreach (ProviderSettings providerSettings in configProviders)
            {
                providers.Add(InstantiateProvider(providerSettings, typeOfProvider));
            }
        }

        #endregion
    }
}