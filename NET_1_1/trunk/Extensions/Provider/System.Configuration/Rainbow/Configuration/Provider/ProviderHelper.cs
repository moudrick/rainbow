using System;
using System.Configuration;
using System.Configuration.Provider;

namespace Rainbow.Configuration.Provider
{
	/// <summary>
	/// Summary description for ProviderHelper.
	/// </summary>
	public sealed class ProviderHelper
	{
		/// <summary>
		/// 
		/// </summary>
		private ProviderHelper()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="providerSettings"></param>
		/// <param name="provType"></param>
		/// <returns></returns>
		public static ProviderBase InstantiateProvider(ProviderSettings providerSettings, Type provType)
		{
			if ((providerSettings.Type == null) || (providerSettings.Type.Length < 1))
				throw new ConfigurationException("Provider could not be instantiated. The Type parameter cannot be null.");

			Type providerType = Type.GetType(providerSettings.Type);
			if (providerType == null)
				throw new ConfigurationException("Provider could not be instantiated. The Type could not be found.");

			if (!provType.IsAssignableFrom(providerType))
				throw new ConfigurationException("Provider must implement type \'" + provType.ToString() + "\'.");

			object providerObj = Activator.CreateInstance(providerType);
			if (providerObj == null)
				throw new ConfigurationException("Provider could not be instantiated.");

			ProviderBase providerBase = ((ProviderBase) providerObj);

			try
			{
				providerBase.Initialize(providerSettings.Name, providerSettings.Parameters);
				return providerBase;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configProviders"></param>
		/// <param name="providers"></param>
		/// <param name="provType"></param>
		public static void InstantiateProviders(ProviderCollection configProviders, ref ProviderCollection providers, Type provType)
		{
			foreach (ProviderSettings providerSettings in configProviders)
			{
				providers.Add(ProviderHelper.InstantiateProvider(providerSettings, provType));
			}
		}
	}
}