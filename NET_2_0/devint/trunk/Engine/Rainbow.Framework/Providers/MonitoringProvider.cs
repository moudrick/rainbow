using System;
using System.Configuration.Provider;
using Rainbow.Framework.Context;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Providers
{
    /// <summary>
    /// Monitoring class is called by the rainbow components to write an entry
    /// into the monitoring database table.  It is used to maintain and show
    /// site statistics such as who has logged on and at what time.
    /// Written by Paul Yarrow, paul@paulyarrow.com
    /// </summary>
    public abstract class MonitoringProvider : ProviderBase
    {
        const string providerType = "monitoring";

        /// <summary>
        /// Gets default configured Portal provider.
        /// Singleton pattern standard member.
        /// </summary>
        /// <returns>Default instance of Portal Provider class</returns>
        public static MonitoringProvider Instance
        {
            get
            {
                return ProviderConfiguration.GetDefaultProviderFromCache<MonitoringProvider>(
                    providerType, RainbowContext.Current.HttpContext.Cache);
            }
        }

        /// <summary>
        /// Logs the entry.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="userField">The user field.</param>
        public abstract void LogEntry(Guid userID,
                                      int portalID,
                                      long pageID,
                                      string actionType,
                                      string userField);
    }
}