using System.Configuration;
using Rainbow.Framework.Context;

namespace Rainbow.Framework.Context
{
    /// <summary>
    /// Concrete IConfigStrategy - reads from ConfigurationSettings.AppSettings
    /// </summary>
    public class ConfigurationManagerStrategy : IConfigStrategy
    {
        /// <summary>
        /// Fetches value for key from ConfigurationSettings.AppSettings
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>string value</returns>
        public string GetAppSetting(string key)
        {
            if (key != null && key.Length != 0)
            {
                return ConfigurationManager.AppSettings[key];
                //return ConfigurationSettings.AppSettings[key];
            }
            return null;
        }
    }
}
