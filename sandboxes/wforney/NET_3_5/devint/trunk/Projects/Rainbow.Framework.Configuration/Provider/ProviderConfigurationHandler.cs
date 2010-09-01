namespace Rainbow.Framework.Provider
{
    using System.Configuration;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Summary description for ProviderConfigurationHandler.
    /// </summary>
    public class ProviderConfigurationHandler : IConfigurationSectionHandler
    {
        #region Public Methods

        /// <summary>
        /// Creates the specified parent.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <returns>
        /// The create.
        /// </returns>
        public virtual object Create(object parent, object configContext, IXPathNavigable section)
        {
            var config = new ProviderConfiguration();
            config.LoadValuesFromConfigurationXml(section);
            return config;
        }

        #endregion

        #region Implemented Interfaces

        #region IConfigurationSectionHandler

        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">
        /// Parent object.
        /// </param>
        /// <param name="configContext">
        /// Configuration context object.
        /// </param>
        /// <param name="section">
        /// Section XML node.
        /// </param>
        /// <returns>
        /// The created section handler object.
        /// </returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var nav = section.CreateNavigator();
            return Create(parent, configContext, nav);
        }

        #endregion

        #endregion
    }
}