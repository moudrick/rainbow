using System;

namespace Rainbow.Data.Configuration
{
	/// <summary>
	/// Summary description for ConfigEsperantus.
	/// </summary>
	public class ConfigEsperantus
	{
		public ConfigEsperantus(){}

		private string keysStore;
		public string KeysStore
		{
			get
			{
				return keysStore;
			}
			set
			{
				keysStore = value;
                
			}
		}

		private string keysStoreParameters;
		public string KeysStoreParameters
		{
			get
			{
				return keysStoreParameters;
			}
			set
			{
				keysStoreParameters = value;
                
			}
		}

		private string countryRegionsStore;
		public string CountryRegionsStore
		{
			get
			{
				return countryRegionsStore;
			}
			set
			{
				countryRegionsStore = value;
                
			}
		}

		private string countryRegionsStoreParameters;
		public string CountryRegionsStoreParameters
		{
			get
			{
				return countryRegionsStoreParameters;
			}
			set
			{
				countryRegionsStoreParameters = value;
                
			}
		}

	}
}
