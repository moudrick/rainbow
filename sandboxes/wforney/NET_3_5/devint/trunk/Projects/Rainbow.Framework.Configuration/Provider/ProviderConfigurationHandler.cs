using System;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;

namespace Rainbow.Framework.Provider
{
    /// <summary>
    /// Summary description for ProviderConfigurationHandler.
    /// </summary>
    public class ProviderConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configContext">The config context.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public virtual object Create(Object parent, Object configContext, IXPathNavigable section)
        {
            ProviderConfiguration config = new ProviderConfiguration();
            config.LoadValuesFromConfigurationXml(section);
            return config;
        }

        #region IConfigurationSectionHandler Members

        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            XPathNavigator nav = section.CreateNavigator();
            return Create(parent, configContext, nav);
        }

        #endregion
    }
}