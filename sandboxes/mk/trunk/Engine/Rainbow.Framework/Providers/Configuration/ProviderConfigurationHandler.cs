using System.Configuration;
using System.Xml;
using Rainbow.Framework.Providers.Configuration;

namespace Rainbow.Framework.Providers.Configuration
{
    /// <summary>
    /// </summary>
    public class ProviderConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public virtual object Create(object parent, object context, XmlNode node)
        {
            ProviderConfiguration config = new ProviderConfiguration();
            config.LoadValuesFromConfigurationXml(node);
            return config;
        }
    }
}
