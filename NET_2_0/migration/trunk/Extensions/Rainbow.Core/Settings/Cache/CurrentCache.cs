using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Rainbow.Settings.Cache
{
	/// <summary>
	/// Class used by Rainbow for manage current cache
	/// </summary>
	sealed public class CurrentCache
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private CurrentCache() {}
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     was static, changed to const per fxcop suggestion
		/// </remarks>
		public const int CacheTime = 120; // Time in seconds used by cache methods

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		public static bool Exists(string key)
		{
			return HttpContext.Current.Cache[key] != null;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A object value...
		/// </returns>
		public static object Get(string key)
		{
			// System.Diagnostics.Debug.WriteLine (" --> Get from Cache : " + key);
			return HttpContext.Current.Cache[key];
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="obj" type="object">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dependency" type="System.Web.Caching.CacheDependency">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void Insert(string key, object obj, CacheDependency dependency)
		{
			HttpContext.Current.Cache.Insert(key, obj, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime));
			// System.Diagnostics.Debug.WriteLine (" --> Insert in Cache with Dependencys: " + key);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="obj" type="object">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void Insert(string key, object obj)
		{
			HttpContext.Current.Cache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime));
			// System.Diagnostics.Debug.WriteLine (" --> Insert in Cache : " + key);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void Remove(string key)
		{
			HttpContext.Current.Cache.Remove(key);
			// System.Diagnostics.Debug.WriteLine (" --> Remove from Cache : " + key);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="prefix" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void RemoveAll(string prefix)
		{
			string auxkey = Portal.UniqueID + prefix;

			foreach (DictionaryEntry cacheItem in HttpContext.Current.Cache)
			{
				if (cacheItem.Key.ToString().StartsWith(auxkey))
					Remove(cacheItem.Key.ToString());
			}
		}
	}
}