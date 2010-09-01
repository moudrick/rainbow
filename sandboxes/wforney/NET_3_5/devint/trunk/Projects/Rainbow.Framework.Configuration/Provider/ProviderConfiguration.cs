namespace Rainbow.Framework.Provider
{
    using System.Collections;
    using System.Configuration;
    using System.Xml.XPath;

    /// <summary>
    /// Provider Configuration
    /// </summary>
    public class ProviderConfiguration
    {
        #region Constants and Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderConfiguration"/> class.
        /// </summary>
        public ProviderConfiguration()
        {
            this.Providers = new Hashtable();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the default provider
        /// </summary>
        /// <value>The default provider.</value>
        public string DefaultProvider { get; private set; }

        /// <summary>
        ///     Gets the loaded providers
        /// </summary>
        /// <value>The providers.</value>
        public Hashtable Providers { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the configuration object for the specified provider
        /// </summary>
        /// <param name="provider">
        /// Name of the provider object to retrieve
        /// </param>
        /// <returns>
        /// Provider configuration.
        /// </returns>
        public static ProviderConfiguration GetProviderConfiguration(string provider)
        {
            return (ProviderConfiguration)ConfigurationManager.GetSection("providers/" + provider);
        }

        /// <summary>
        /// Loads provider information from the configuration node
        /// </summary>
        /// <param name="node">
        /// Node representing configuration information
        /// </param>
        public void LoadValuesFromConfigurationXml(IXPathNavigable node)
        {
            var nav = node.CreateNavigator();
            if (nav.HasAttributes && !string.IsNullOrEmpty(nav.GetAttribute("defaultProvider", string.Empty)))
            {
                this.DefaultProvider = nav.GetAttribute("defaultProvider", string.Empty);
            }

            // XmlAttributeCollection attributeCollection = node.Attributes;

            // Get the default provider
            // defaultProvider = attributeCollection["defaultProvider"].Value;

            // Read child nodes
            if (nav.HasChildren)
            {
                foreach (XPathNavigator child in nav.SelectChildren("providers", string.Empty))
                {
                    this.GetProviders(child);
                }
            }

            // foreach (XmlNode child in node.ChildNodes)
            // {
            // if (child.Name == "providers")
            // GetProviders(child);
            // }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures Provider(s) based on the configuration node
        /// </summary>
        /// <param name="node">
        /// </param>
        private void GetProviders(XPathNavigator node)
        {
            if (!node.HasChildren)
            {
                return;
            }

            foreach (XPathNavigator provider in node.SelectChildren(XPathNodeType.All))
            {
                // foreach (XmlNode provider in node.ChildNodes)
                switch (provider.Name)
                {
                    case "add":
                        this.Providers.Add(
                            provider.GetAttribute("name", string.Empty),
                            new ProviderSettings(provider.GetAttribute("name", string.Empty), provider.GetAttribute("type", string.Empty)));

                        // providers.Add(provider.Attributes["name"].Value,
                        // new ProviderSettings(provider.Attributes["name"].Value,
                        // provider.Attributes["type"].Value));
                        break;

                    case "remove":
                        this.Providers.Remove(provider.GetAttribute("name", string.Empty));

                        // providers.Remove(provider.Attributes["name"].Value);
                        break;

                    case "clear":
                        this.Providers.Clear();
                        break;
                }
            }
        }

        #endregion
    }
}