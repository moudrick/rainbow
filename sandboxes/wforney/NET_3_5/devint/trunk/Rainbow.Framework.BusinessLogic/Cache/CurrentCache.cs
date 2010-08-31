namespace Rainbow.Framework.Configuration.Cache
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Caching;

    /// <summary>
    /// Class used by Rainbow for manage current cache
    /// </summary>
    public sealed class CurrentCache
    {
        #region Constants and Fields

        /// <summary>
        /// The cache time.
        /// </summary>
        /// <remarks>
        /// was static, changed to const per fxcop suggestion
        /// </remarks>
        public const int CacheTime = 120; // Time in seconds used by cache methods

        /// <summary>
        /// The on remove.
        /// </summary>
        private static CacheItemRemovedCallback onRemove;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="CurrentCache"/> class from being created. 
        ///     Initializes a new instance of the <see cref="CurrentCache"/> class.
        /// </summary>
        private CurrentCache()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A bool value...
        /// </returns>
        public static bool Exists(string key)
        {
            Trace.TraceInformation("CacheItem Query [key: {0}]", key);
            return HttpContext.Current.Cache[key] != null;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A object value...
        /// </returns>
        public static object Get(string key)
        {
            Trace.TraceInformation("CacheItem Get [key: {0}]", key);
            return HttpContext.Current.Cache[key];
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dependency">
        /// The dependency.
        /// </param>
        public static void Insert(string key, object value, CacheDependency dependency)
        {
            onRemove = new CacheItemRemovedCallback(RemovedCallback);

            if (Portal.UniqueId == null)
            {
                return;
            }

            // Jes1111
            if (!key.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
            {
                key = String.Concat(Portal.UniqueId, key);
            }

            // HttpContext.Current.Cache.Insert(key, obj, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, OnRemove);
            HttpContext.Current.Cache.Insert(
                key, 
                value, 
                dependency, 
                Cache.NoAbsoluteExpiration, 
                TimeSpan.Zero, 
                CacheItemPriority.Default, 
                onRemove);
            Trace.TraceInformation("CacheItem Insert (with dependency) [key: {0}]", key);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void Insert(string key, object value)
        {
            onRemove = new CacheItemRemovedCallback(RemovedCallback);

            // Jes1111
            if (!key.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
            {
                key = String.Concat(Portal.UniqueId, key);
            }

            HttpContext.Current.Cache.Insert(
                key, 
                value, 
                null, 
                Cache.NoAbsoluteExpiration, 
                TimeSpan.FromSeconds(CacheTime), 
                CacheItemPriority.Default, 
                onRemove);

            Trace.TraceInformation("CacheItem Insert (no dependency) [key: {0}]", key);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void Remove(string key)
        {
            // Jes1111
            if (!key.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
            {
                key = String.Concat(Portal.UniqueId, key);
            }

            HttpContext.Current.Cache.Remove(key);

            Trace.TraceInformation("CacheItem Remove [key: {0}]", key);
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        public static void RemoveAll(string prefix)
        {
            Trace.TraceInformation("CacheItem RemoveAll called");

            // Jes1111
            if (!prefix.StartsWith(Portal.UniqueId, StringComparison.OrdinalIgnoreCase))
            {
                prefix = String.Concat(Portal.UniqueId, prefix);
            }

            // string auxkey = Portal.UniqueId + prefix;
            // if (cacheItem.Key.ToString().StartsWith(auxkey))
            foreach (var cacheItem in
                HttpContext.Current.Cache.Cast<DictionaryEntry>().Where(cacheItem => cacheItem.Key.ToString().StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                Remove(cacheItem.Key.ToString());
            }
        }

        // added: Jes1111 - 27-02-2005

        /// <summary>
        /// help!
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public static void RemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            // TODO: Fix me!
            Trace.TraceInformation("CacheItem {0} {1}", reason, key);
        }

        #endregion
    }
}