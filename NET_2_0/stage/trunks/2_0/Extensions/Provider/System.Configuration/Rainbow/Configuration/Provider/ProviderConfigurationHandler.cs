using System;
using System.Configuration;
using System.Xml;

namespace Rainbow.Configuration.Provider
{
	/// <summary>
	/// Summary description for ProviderConfigurationHandler.
	/// </summary>
	public class ProviderConfigurationHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="context"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public virtual object Create(Object parent, Object context, XmlNode node)
		{
			ProviderConfiguration config = new ProviderConfiguration();
			config.LoadValuesFromConfigurationXml(node);
			return config;
		}
	}
}