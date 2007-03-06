using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Caching;
using Microsoft.Practices.EnterpriseLibrary.Configuration;
using Rainbow.Configuration.Provider;
using Rainbow.Data.Configuration;

namespace Rainbow.Data
{
	/// <summary>
	/// Summary description for DataProvider.
	/// </summary>
	public class DataProvider : ProviderBase
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected DataProvider()
		{
		}

		#region ConfigEsperantus
		/// <summary>
		///     Using the static method, read the cached configuration settings
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		protected ConfigEsperantus configEsperantus = ConfigurationManager.GetConfiguration("EsperantusSettings") as ConfigEsperantus;

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public ConfigEsperantus ConfigEsperantus
		{
			get { return configEsperantus; }
			set { configEsperantus = value; }
		}
		#endregion

		#region ConfigScheduler
		/// <summary>
		///     Using the static method, read the cached configuration settings
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		protected ConfigScheduler configScheduler = ConfigurationManager.GetConfiguration("SchedulerSettings") as ConfigScheduler;

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public ConfigScheduler ConfigScheduler
		{
			get { return configScheduler; }
			set { configScheduler = value; }
		}
		#endregion
		
		#region ConfigData
		/// <summary>
		///     Using the static method, read the cached configuration settings
		/// </summary>
		/// <remarks>
		///     Isn't used for provider configuration, but maybe we should as
		///     it has cache built in...
		/// </remarks>
		protected ConfigData configData = ConfigurationManager.GetConfiguration("DataSettings") as ConfigData;

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public ConfigData ConfigData
		{
			get { return configData; }
			set { configData = value; }
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public void UpdateConfigData()
		{
			ConfigurationManager.WriteConfiguration("DataSettings", ConfigData);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="newConfigData" type="Rainbow.Data.Config">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void UpdateConfigData(ConfigData newConfigData)
		{
			ConfigurationManager.WriteConfiguration("DataSettings", newConfigData);
		}
		#endregion

		#region Provider

		/// <summary>
		/// Camel case. Must match web.config section name
		/// </summary>
		private const string providerType = "dataLayer";

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static DataProvider Instance()
		{
			// Use the cache because the reflection used later is expensive
			Cache cache = HttpRuntime.Cache;
			string cacheKey = null;
			// Get the names of providers
			ProviderConfiguration config = ProviderConfiguration.GetProviderConfiguration(providerType);
			// Read specific configuration information for this provider
			ProviderSettings providerSettings = (ProviderSettings) config.Providers[config.DefaultProvider];
			// In the cache?
			cacheKey = "Rainbow::Web::DataLayer::" + config.DefaultProvider;

			if (cache[cacheKey] == null)
			{ // The assembly should be in \bin or GAC, so we simply need

				// to get an instance of the type
				try
				{
					cache.Insert(cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof (DataProvider)));
				}

				catch (Exception e)
				{
					throw new ApplicationException("Unable to load provider", e);
				}
				catch
				{
					throw;
				}
			}
			return (DataProvider) cache[cacheKey];
		}

		#endregion
	}
}