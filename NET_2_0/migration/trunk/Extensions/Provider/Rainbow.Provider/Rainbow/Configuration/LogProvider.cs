// Created by Manu
using System;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Web;
using System.Web.Caching;
using Rainbow.Configuration.Provider;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Summary description for LogProvider.
	/// </summary>
	public abstract class LogProvider : ProviderBase
	{
		/// <summary>
		/// Camel case. Must match web.config section name
		/// </summary>
		private const string providerType = "log";

		/// <summary>
		/// 
		/// </summary>
		protected LogProvider()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static LogProvider Instance()
		{
			// Use the cache because the reflection used later is expensive
			Cache cache = HttpRuntime.Cache;
			string cacheKey = null;

			// Get the names of providers
			ProviderConfiguration config;
			config = ProviderConfiguration.GetProviderConfiguration(providerType);

			//If config not found (missing web.config)
			if(config == null)
			{
				//Try to provide a default anyway
				System.Xml.XmlDocument defaultNode = new System.Xml.XmlDocument();
				defaultNode.LoadXml("<log defaultProvider=\"Log4NetLog\"><providers><clear /><add name=\"Log4NetLog\" type=\"Rainbow.Configuration.Log4NetLogProvider, Rainbow.Provider.Implementation\" /></providers></log>");

				// Get the names of providers
				config = new ProviderConfiguration();
				config.LoadValuesFromConfigurationXml(defaultNode.DocumentElement);
			}

			// Read specific configuration information for this provider
			ProviderSettings providerSettings = (ProviderSettings) config.Providers[config.DefaultProvider];

			// In the cache?
			cacheKey = "Rainbow::Configuration::Log::" + config.DefaultProvider;
			if (cache[cacheKey] == null)
			{
				// The assembly should be in \bin or GAC, so we simply need
				// to get an instance of the type
				try
				{
					cache.Insert(cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof (LogProvider)));
				}
				catch (Exception e)
				{
					throw new Exception("Unable to load provider", e);
				}
				catch
				{
					throw;
				}
			}

			return (LogProvider) cache[cacheKey];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public abstract void Log(LogLevel level, object message);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="t"></param>
		public abstract void Log(LogLevel level, object message, Exception t);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="sw"></param>
		public abstract void Log(LogLevel level, object message, StringWriter sw);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="t"></param>
		/// <param name="sw"></param>
		public abstract void Log(LogLevel level, object message, Exception t, StringWriter sw);

	}
}