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
			//System.Diagnostics.Debug.WriteLine ("CacheItem Query [key: " + key + "]");
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
			//System.Diagnostics.Debug.WriteLine ("CacheItem Get [key: " + key + "]");
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
			onRemove = new CacheItemRemovedCallback(RemovedCallback);

			// Jes1111
			if ( !key.StartsWith(Portal.UniqueID) )
				key = String.Concat(Portal.UniqueID,key);

			//HttpContext.Current.Cache.Insert(key, obj, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, onRemove);
			HttpContext.Current.Cache.Insert(key, obj, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.Zero, CacheItemPriority.Default, onRemove);
			System.Diagnostics.Debug.WriteLine ("CacheItem Insert (with dependency) [key: " + key + "]");
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
			onRemove = new CacheItemRemovedCallback(RemovedCallback);

			// Jes1111
			if ( !key.StartsWith(Portal.UniqueID) )
				key = String.Concat(Portal.UniqueID,key);

			HttpContext.Current.Cache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), CacheItemPriority.Default, onRemove);
			System.Diagnostics.Debug.WriteLine ("CacheItem Insert (no dependency) [key: " + key + "]");
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
			// Jes1111
			if ( !key.StartsWith(Portal.UniqueID) )
				key = String.Concat(Portal.UniqueID,key);
			
			HttpContext.Current.Cache.Remove(key);
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
			System.Diagnostics.Debug.WriteLine ("CacheItem RemoveAll called");

			// Jes1111
			if ( !prefix.StartsWith(Portal.UniqueID) )
				prefix = String.Concat(Portal.UniqueID,prefix);

			//string auxkey = Portal.UniqueID + prefix;

			foreach (DictionaryEntry cacheItem in HttpContext.Current.Cache)
			{
				//if (cacheItem.Key.ToString().StartsWith(auxkey))
				if (cacheItem.Key.ToString().StartsWith(prefix))
					Remove(cacheItem.Key.ToString());
			}
		}
		
		// added: Jes1111 - 27-02-2005
		private static CacheItemRemovedCallback onRemove = null;
		
		/// <summary>
		/// help!
		/// </summary>
		/// <param name="k"></param>
		/// <param name="v"></param>
		/// <param name="r"></param>
		public static void RemovedCallback(String k, Object v, CacheItemRemovedReason r)
		{
			System.Diagnostics.Debug.WriteLine("CacheItem " + r + " " + k );
		}

	}
}