using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using Rainbow.Framework.Context;

namespace Rainbow.Framework.Context
{
    /// <summary>
    /// Class used by Rainbow for manage current cache
    /// </summary>
    sealed public class CurrentCache
    {		
        // added: Jes1111 - 27-02-2005
        static CacheItemRemovedCallback onRemove = null;

        CurrentCache() { }
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
            //System.Diagnostics.Debug.WriteLine ("CacheItem Query [key: " + key + "]");
            return HttpContext.Current.Cache[key] != null;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A object value...</returns>
        public static object Get(string key)
        {
            //System.Diagnostics.Debug.WriteLine ("CacheItem Get [key: " + key + "]");
            return HttpContext.Current.Cache[key];
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="dependency">The dependency.</param>
        public static void Insert(string key, object obj, CacheDependency dependency)
        {
            onRemove = RemovedCallback;
            if (RainbowContext.Current.UniqueID != null)
            {
                // Jes1111
                if (!key.StartsWith(RainbowContext.Current.UniqueID))
                {
                    key = String.Concat(RainbowContext.Current.UniqueID, key);
                }

                //HttpContext.Current.Cache.Insert(key, obj, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, onRemove);
                HttpContext.Current.Cache.Insert(key, obj, dependency, Cache.NoAbsoluteExpiration, TimeSpan.Zero, CacheItemPriority.Default, onRemove);
                System.Diagnostics.Debug.WriteLine("CacheItem Insert (with dependency) [key: " + key + "]");
            }
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        public static void Insert(string key, object obj)
        {
            onRemove = RemovedCallback;
            // Jes1111
            if (!key.StartsWith(RainbowContext.Current.UniqueID))
            {
                key = String.Concat(RainbowContext.Current.UniqueID, key);
            }

            HttpContext.Current.Cache.Insert(key, obj, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, onRemove);
            System.Diagnostics.Debug.WriteLine("CacheItem Insert (no dependency) [key: " + key + "]");
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Remove(string key)
        {
            // Jes1111
            if (!key.StartsWith(RainbowContext.Current.UniqueID))
            {
                key = string.Concat(RainbowContext.Current.UniqueID, key);
            }
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public static void RemoveAll(string prefix)
        {
            System.Diagnostics.Debug.WriteLine("CacheItem RemoveAll called");

            // Jes1111
            if (!prefix.StartsWith(RainbowContext.Current.UniqueID))
            {
                prefix = String.Concat(RainbowContext.Current.UniqueID, prefix);
            }

            //string auxkey = Portal.UniqueID + prefix;

            foreach (DictionaryEntry cacheItem in HttpContext.Current.Cache)
            {
                //if (cacheItem.Key.ToString().StartsWith(auxkey))
                if (cacheItem.Key.ToString().StartsWith(prefix))
                {
                    Remove(cacheItem.Key.ToString());
                }
            }
        }

        /// <summary>
        /// help!
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="reason">The r.</param>
        public static void RemovedCallback(string key, object value, 
                                           CacheItemRemovedReason reason)
        {
            System.Diagnostics.Debug.WriteLine("CacheItem " + reason + " " + key);
        }
    }
}