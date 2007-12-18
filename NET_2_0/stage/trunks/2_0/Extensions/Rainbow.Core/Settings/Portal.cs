using System.Configuration;
using System.Globalization;
using System.Web;
using Rainbow.Core;
using Rainbow.Settings.Cache;

namespace Rainbow.Settings
{
	/// <summary>
	/// This class contains useful information for Extension, Module and Core Developers.
	/// </summary>
	public sealed class Portal
	{
		private static Config config = new Config("RainbowConfig");
		
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private Portal()
		{
		}

		/// <summary>
		/// This static string fetches the site's alias either via querystring, cookie or domain and returns it
		/// </summary>
		public static string UniqueID
		{
			get
			{
				//Check for Alias first
				//Try to get portal alias from querystring
				if (HttpContext.Current.Request.Params["Alias"] != null)
				{
					//Alias is in the QueryString
					return HttpContext.Current.Request.Params["Alias"];
				}
				else if (HttpContext.Current.Request.Cookies["PortalAlias"] != null)
				{
					//If we have a Cookie, but no Param, then use cookie value
					return HttpContext.Current.Request.Cookies["PortalAlias"].Value;
				}
				else //Is the Domain a valid alias?
				{
					//parse the domain name from the URL (ie. www.domainname.com ) 
					string domainName = HttpContext.Current.Request.Url.Host.ToLower(CultureInfo.InvariantCulture);

//					if (bool.Parse(ConfigurationSettings.AppSettings["RemoveWWW"]) && domainName.StartsWith("www."))
					if (bool.Parse(config.ConfigData["RemoveWWW"].ToString()) && domainName.StartsWith("www."))
						domainName = domainName.Substring(4, (domainName.Length - 4));
					//domainName = "mydomain.com";  

					//Remove trailing domain name .xx
//					if (bool.Parse(ConfigurationSettings.AppSettings["IgnoreFirstDomain"]) && (domainName.LastIndexOf(@".") > 0))
					if (bool.Parse(config.ConfigData["IgnoreFirstDomain"].ToString()) && domainName.LastIndexOf(@".") > 0)
						domainName = domainName.Substring(0, domainName.LastIndexOf(@"."));
					//domainName = "www.mydomain" or domainName = "mydomain"

					return domainName;
				}
			}
		}


		private static readonly bool enableMultiDbSupport =
//			(ConfigurationSettings.AppSettings["EnableMultiDbSupport"] == null
			(config.ConfigData["EnableMultiDbSupport"] == null
				? false
				: bool.Parse(config.ConfigData["EnableMultiDbSupport"].ToString()));
//				: bool.Parse(ConfigurationSettings.AppSettings["EnableMultiDbSupport"]));

		private static readonly string connectionString =
//			(ConfigurationSettings.AppSettings["ConnectionString"] == null
			(config.ConfigData["ConnectionString"] == null
			? "server=localhost;Trusted_Connection=true;database=Rainbow;Application Name=Rainbow"
			: config.ConfigData["ConnectionString"].ToString());
//				: ConfigurationSettings.AppSettings["ConnectionString"]);

		/// <summary>
		/// Database connection
		/// </summary>
		public static string ConnectionString
		{
			get
			{
				//Manu, improved performance on an often called routine
				if (enableMultiDbSupport)
				{
					// JohnMandia has changed this. First it searches for the domain name in 
					// web.config (MultiDbMode) if it's not there it defaults to the single connection string
					// José Viladiu. Add Cache support
					string keyConnection = UniqueID.ToString() + "_ConnectionString";
					string siteConnectionString;
					if (CurrentCache.Exists(keyConnection))
						siteConnectionString = (string) CurrentCache.Get(keyConnection);
					else
					{
						if (ConfigurationSettings.AppSettings[keyConnection] != null)
							siteConnectionString = ConfigurationSettings.AppSettings[keyConnection];
						else
							siteConnectionString = connectionString;
						CurrentCache.Insert(keyConnection, siteConnectionString);
					}
					return (siteConnectionString);
				}
				else
				{
					//If there is no multidb support we just return the static connection
					return (connectionString);
				}
			}
		}
		
//		private static readonly string smtpServer =
//			(ConfigurationSettings.AppSettings["SmtpServer"] == null
//				? string.Empty // Cluster server support by marcb
//				: ConfigurationSettings.AppSettings["SmtpServer"]);


		/// <summary>
		/// SmtpServer
		/// </summary>
		public static string SmtpServer
		{
			get
			{
				return config.ConfigData["SmtpServer"].ToString();
			}
		}

	}
}