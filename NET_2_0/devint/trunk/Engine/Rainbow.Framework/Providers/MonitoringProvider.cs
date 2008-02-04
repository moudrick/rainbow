using System;
using System.Configuration.Provider;
using Rainbow.Framework.Context;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Providers.Configuration;

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

        /// <summary>
        /// returns the total hit count for a portal
        /// </summary>
        /// <param name="portalID">portal id to get stats for</param>
        /// <returns>
        /// total number of hits to the portal of all types
        /// </returns>
        public abstract int GetTotalPortalHits(int portalID);

        /// <summary>
        /// Get Users Online
        /// Add to the Cache
        /// HttpContext.Current.Cache.Insert("WhoIsOnlineAnonUserCount", anonUserCount, null, DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
        /// HttpContext.Current.Cache.Insert("WhoIsOnlineRegUserCount", regUsersOnlineCount, null, DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
        /// HttpContext.Current.Cache.Insert("WhoIsOnlineRegUsersString", regUsersString, null, DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="minutesToCheckForUsers">The minutes to check for users.</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="anonUserCount">The anon user count.</param>
        /// <param name="regUsersOnlineCount">The reg users online count.</param>
        /// <param name="regUsersString">The reg users string.</param>
        public abstract void FillUsersOnlineCache(int portalID,
                                                int minutesToCheckForUsers,
                                                int cacheTimeout,
                                                out int anonUserCount,
                                                out int regUsersOnlineCount,
                                                out string regUsersString);
    }
}
