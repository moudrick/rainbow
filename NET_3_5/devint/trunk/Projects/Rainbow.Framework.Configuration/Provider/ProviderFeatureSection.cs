namespace Rainbow.Framework.Provider
{
    using System.Configuration;
    /// <summary>
    /// Provider Feature Section
    /// </summary>
    public abstract class ProviderFeatureSection : System.Configuration.ConfigurationSection
    {
        private readonly ConfigurationProperty defaultProvider = new ConfigurationProperty("defaultProvider", typeof(string), null);

        private readonly ConfigurationProperty providers = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null);

        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFeatureSection"/> class.
        /// </summary>
        public ProviderFeatureSection()
        {
            properties.Add(providers);
            properties.Add(defaultProvider);
        }

        /// <summary>
        /// Gets or sets the default provider.
        /// </summary>
        /// <value>The default provider.</value>
        [ConfigurationProperty("defaultProvider")]
        public string DefaultProvider
        {
            get { return (string)base[defaultProvider]; }
            set { base[defaultProvider] = value; }
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[providers]; }
        }

        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> of properties for the element.</returns>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return properties; }
        }
    }
}