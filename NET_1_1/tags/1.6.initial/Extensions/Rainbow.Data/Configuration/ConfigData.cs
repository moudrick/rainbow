using System.Collections;

namespace Rainbow.Data.Configuration
{
	/// <summary>
	/// Summary description for Config.
	/// </summary>
	public class ConfigData
	{
		public ConfigData()
		{
		}

		private bool useProxyServerForServerWebRequests;
		public bool UseProxyServerForServerWebRequests
		{
			get
			{
				return useProxyServerForServerWebRequests;
			}
			set
			{
				useProxyServerForServerWebRequests = value;
                
			}
		}


		private string connectionString;

		/// <summary>
		///     Default connection string
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		private Hashtable connectionStrings;

		/// <summary>
		///     Portal specific connection strings
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public Hashtable ConnectionStrings
		{
			get { return connectionStrings; }
			set { connectionStrings = value; }
		}


		private bool enableMonitoring = true;

		/// <summary>
		///     EnableMonitoring turns statistics tracking on/off
		/// </summary>
		/// <remarks>
		///     TODO:  Implement this in the specific data provider(s) and move this setting elsewhere or remove the abstract keyword
		/// </remarks>
		public bool EnableMonitoring
		{
			get { return enableMonitoring; }
			set { enableMonitoring = value; }
		}

		private string aDUserName;

		/// <summary>
		///     Active Directory UserName
		/// </summary>
		/// <value>
		///     <para>
		///         DOMAIN\USERNAME
		///     </para>
		/// </value>
		public string ADUserName
		{
			get
			{
				return aDUserName == null ?
					string.Empty : aDUserName;
			}
			set { aDUserName = value; }
		}

		private string aDUserPassword;

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
		public string ADUserPassword
		{
			get
			{
				return aDUserPassword == null ?
					string.Empty : aDUserPassword;
			}
			set { aDUserPassword = value; }
		}

		private bool useSingleUserBase;

		/// <summary>
		/// If true all users will be loaded from portal 0 instance
		/// </summary>
		public bool UseSingleUserBase
		{
			get { return useSingleUserBase; }
			set { useSingleUserBase = value; }
		}

		private bool removeWWW = true;

		public bool RemoveWWW
		{
			get { return removeWWW; }
			set { removeWWW = value; }
		}

		private bool ignoreFirstDomain = true;

		public bool IgnoreFirstDomain
		{
			get { return ignoreFirstDomain; }
			set { ignoreFirstDomain = value; }
		}

		private bool enableMultiDbSupport;

		public bool EnableMultiDbSupport
		{
			get { return enableMultiDbSupport; }
			set { enableMultiDbSupport = value; }
		}

		private bool enableADUser;

		public bool EnableADUser
		{
			get { return enableADUser; }
			set { enableADUser = value; }
		}


		private string defaultLanguage;

		public string DefaultLanguage
		{
			get { return defaultLanguage; }
			set { defaultLanguage = value; }
		}


		private string portalSecureDirectory;

		public string PortalSecureDirectory
		{
			get { return portalSecureDirectory; }
			set { portalSecureDirectory = value; }
		}


		private string smtpServer = "localhost";

		public string SmtpServer
		{
			get { return smtpServer; }
			set { smtpServer = value; }
		}


		private string portalTitlePrefix = "Rainbow - ";

		public string PortalTitlePrefix
		{
			get { return portalTitlePrefix; }
			set { portalTitlePrefix = value; }
		}


		private string proxyServer;

		public string ProxyServer
		{
			get { return proxyServer; }
			set { proxyServer = value; }
		}

		private string proxyDomain;

		public string ProxyDomain
		{
			get { return proxyDomain; }
			set { proxyDomain = value; }
		}

		private string proxyUserID;

		public string ProxyUserID
		{
			get { return proxyUserID; }
			set { proxyUserID = value; }
		}

		private string proxyPassword;

		public string ProxyPassword
		{
			get { return proxyPassword; }
			set { proxyPassword = value; }
		}

		private string defaultPortal;
		public string DefaultPortal
		{
			get
			{
				return defaultPortal;
			}
			set
			{
				defaultPortal = value;
                
			}
		}

		private bool checkForFilePermissions = true;
		public bool CheckForFilePermissions
		{
			get
			{
				return checkForFilePermissions;
			}
			set
			{
				checkForFilePermissions = value;
                
			}
		}

	}
}