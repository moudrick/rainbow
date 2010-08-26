using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using Rainbow.Framework.Configuration;

namespace Rainbow.Framework.Configuration.Cache
{
    /// <summary>
    /// Class used by Rainbow for manage current cache
    /// </summary>
    sealed public class CurrentCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentCache"/> class.
        /// </summary>
        private CurrentCache() { }
        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     was static, changed to const per fxcop suggestion
        /// </remarks>
        public const int CacheTime = 120; // Time in seconds used by cache methods

        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A bool value...</returns>
        public static bool Exists(string key)
        {
            System.Diagnostics.Trace.TraceInformation("CacheItem Query [key: {0}]", key);
            return HttpContext.Current.Cache[key] != null;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A object value...</returns>
        public static object Get(string key)
        {
            System.Diagnostics.Trace.TraceInformation("CacheItem Get [key: {0}]", key);
            return HttpContext.Current.Cache[key];
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="dependency">The dependency.</param>
        public static void Insert(string key, object value, CacheDependency dependency)
        {
            onRemove = new CacheItemRemovedCallback(RemovedCallback);

            if (Portal.UniqueId != null)
            {
                // Jes1111
                if (!key.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
                    key = String.Concat(Portal.UniqueId, key);

                //HttpContext.Current.Cache.Insert(key, obj, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, onRemove);
                HttpContext.Current.Cache.Insert(key, value, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.Zero, CacheItemPriority.Default, onRemove);
                System.Diagnostics.Trace.TraceInformation("CacheItem Insert (with dependency) [key: {0}]", key);
            }
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Insert(string key, object value)
        {
            onRemove = new CacheItemRemovedCallback(RemovedCallback);

            // Jes1111
            if (!key.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
                key = String.Concat(Portal.UniqueId, key);

            HttpContext.Current.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, onRemove);

            System.Diagnostics.Trace.TraceInformation("CacheItem Insert (no dependency) [key: {0}]", key);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Remove(string key)
        {
            // Jes1111
            if (!key.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
                key = String.Concat(Portal.UniqueId, key);

            HttpContext.Current.Cache.Remove(key);

            System.Diagnostics.Trace.TraceInformation("CacheItem Remove [key: {0}]", key);
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public static void RemoveAll(string prefix)
        {
            System.Diagnostics.Trace.TraceInformation("CacheItem RemoveAll called");

            // Jes1111
            if (!prefix.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
                prefix = String.Concat(Portal.UniqueId, prefix);

            //string auxkey = Portal.UniqueId + prefix;

            foreach (DictionaryEntry cacheItem in HttpContext.Current.Cache)
            {
                //if (cacheItem.Key.ToString().StartsWith(auxkey))
                if (cacheItem.Key.ToString().StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    Remove(cacheItem.Key.ToString());
            }
        }

        // added: Jes1111 - 27-02-2005
        private static CacheItemRemovedCallback onRemove;

        /// <summary>
        /// help!
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="reason">The reason.</param>
        public static void RemovedCallback(String key, Object value, CacheItemRemovedReason reason)
        {
            //TODO: Fix me!
            System.Diagnostics.Trace.TraceInformation("CacheItem {0} {1}", reason, key);
        }

    }
}