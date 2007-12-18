using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Rainbow.Core;
using Rainbow.Settings;
using Page = System.Web.UI.Page;

namespace Rainbow.Configuration
{
	/// <summary>
	/// ModuleSettings Class encapsulates the detailed settings 
	/// for a specific Module in the Portal.
	/// </summary>
	[History("gman3001", "2004/10/06", "Added GetModuleDesktopSrc method to return the source path of a Module specified by its ID")]
	[History("Jes1111", "2003/04/24", "Added Cacheable property")]
	public class ModuleSettings
	{
		private const string strAdmin = "Admin;";
		private const string strDesktopSrc = "DesktopSrc";
		private const string strATModuleID = "@ModuleID";

		private int moduleID;

		/// <summary>
		/// ModuleID
		/// </summary>
		public int ModuleID
		{
			get { return moduleID; }
			set { moduleID = value; }
		}


		private int moduleDefID;

		/// <summary>
		/// ModuleDefID
		/// </summary>
		public int ModuleDefID
		{
			get { return moduleDefID; }
			set { moduleDefID = value; }
		}


		private int tabID;

		/// <summary>
		/// TabID
		/// </summary>
		public int TabID
		{
			get { return tabID; }
			set { tabID = value; }
		}


		private int cacheTime;

		/// <summary>
		/// CacheTime
		/// </summary>
		public int CacheTime
		{
			get { return cacheTime; }
			set { cacheTime = value; }
		}


		private int moduleOrder;

		/// <summary>
		/// ModuleOrder
		/// </summary>
		public int ModuleOrder
		{
			get { return moduleOrder; }
			set { moduleOrder = value; }
		}


		private string paneName;

		/// <summary>
		/// PaneName
		/// </summary>
		public string PaneName
		{
			get { return paneName; }
			set { paneName = value; }
		}


		private string moduleTitle;

		/// <summary>
		/// ModuleTitle
		/// </summary>
		public string ModuleTitle
		{
			get { return moduleTitle; }
			set { moduleTitle = value; }
		}


		private string authorizedEditRoles;

		/// <summary>
		/// AuthorizedEditRoles
		/// </summary>
		public string AuthorizedEditRoles
		{
			get { return authorizedEditRoles; }
			set { authorizedEditRoles = value; }
		}


		private string authorizedViewRoles;

		/// <summary>
		/// AuthorizedViewRoles
		/// </summary>
		public string AuthorizedViewRoles
		{
			get { return authorizedViewRoles; }
			set { authorizedViewRoles = value; }
		}


		private string authorizedAddRoles;

		/// <summary>
		/// AuthorizedAddRoles
		/// </summary>
		public string AuthorizedAddRoles
		{
			get { return authorizedAddRoles; }
			set { authorizedAddRoles = value; }
		}


		private string authorizedDeleteRoles;

		/// <summary>
		/// AuthorizedDeleteRoles
		/// </summary>
		public string AuthorizedDeleteRoles
		{
			get { return authorizedDeleteRoles; }
			set { authorizedDeleteRoles = value; }
		}


		private string authorizedPropertiesRoles;

		/// <summary>
		/// AuthorizedPropertiesRoles
		/// </summary>
		public string AuthorizedPropertiesRoles
		{
			get { return authorizedPropertiesRoles; }
			set { authorizedPropertiesRoles = value; }
		}


		private string authorizedDeleteModuleRoles;

		/// <summary>
		/// AuthorizedDeleteModuleRoles 
		/// </summary>
		public string AuthorizedDeleteModuleRoles
		{
			get { return authorizedDeleteModuleRoles; }
			set { authorizedDeleteModuleRoles = value; }
		}

		private string authorizedMoveModuleRoles;
		// Added by jviladiu@portalServices.net 19/8/2004
		/// <summary>
		/// AuthorizedMoveModuleRoles
		/// </summary>
		public string AuthorizedMoveModuleRoles
		{
			get { return authorizedMoveModuleRoles; }
			set { authorizedMoveModuleRoles = value; }
		}

		private string authorizedPublishingRoles;
		// Added by jviladiu@portalServices.net 19/8/2004
		/// <summary>
		/// AuthorizedPublishingRoles
		/// </summary>
		public string AuthorizedPublishingRoles
		{
			get { return authorizedPublishingRoles; }
			set { authorizedPublishingRoles = value; }
		}

		private string authorizedApproveRoles;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 6/2/2003
		/// <summary>
		/// AuthorizedApproveRoles
		/// </summary>
		public string AuthorizedApproveRoles
		{
			get { return authorizedApproveRoles; }
			set { authorizedApproveRoles = value; }
		}

		private WorkflowState workflowStatus;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 27/2/2003
		/// <summary>
		/// WorkflowStatus
		/// </summary>
		public WorkflowState WorkflowStatus
		{
			get { return workflowStatus; }
			set { workflowStatus = value; }
		}

		private bool supportWorkflow;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 27/2/2003
		/// <summary>
		/// SupportWorkflow
		/// </summary>
		public bool SupportWorkflow
		{
			get { return supportWorkflow; }
			set { supportWorkflow = value; }
		}

		private bool showMobile;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 6/2/2003
		/// <summary>
		/// ShowMobile
		/// </summary>
		public bool ShowMobile
		{
			get { return showMobile; }
			set { showMobile = value; }
		}


		private bool showEveryWhere;

		/// <summary>
		/// ShowEveryWhere
		/// </summary>
		public bool ShowEveryWhere
		{
			get { return showEveryWhere; }
			set { showEveryWhere = value; }
		}

		private bool supportCollapsable;
		// Change by john.mandia@whitelightsolutions.com - Date: 5/24/2003
		/// <summary>
		/// SupportCollapsable
		/// </summary>
		public bool SupportCollapsable
		{
			get { return supportCollapsable; }
			set { supportCollapsable = value; }
		}

		private string desktopSrc;
		// Change by bja@reedtek.com - Date: 5/12/2003
		/// <summary>
		/// DesktopSrc
		/// </summary>
		public string DesktopSrc
		{
			get { return desktopSrc; }
			set { desktopSrc = value; }
		}


		private string mobileSrc;

		/// <summary>
		/// MobileSrc
		/// </summary>
		public string MobileSrc
		{
			get { return mobileSrc; }
			set { mobileSrc = value; }
		}


		private Guid guidID;

		/// <summary>
		/// GuidID
		/// </summary>
		public Guid GuidID
		{
			get { return guidID; }
			set { guidID = value; }
		}


		private bool admin;

		/// <summary>
		/// Is Admin?
		/// </summary>
		public bool Admin
		{
			get { return admin; }
			set { admin = value; }
		}


		private ArrayList cacheDependency = new ArrayList();
		// Change 28/Feb/2003 - Jeremy Esland - Cache
		// used to store list of files for cache dependency -
		// optionally filled by module code
		// read by code in CachedPortalModuleControl
		/// <summary>
		/// String array of cache dependency files
		/// </summary>
		public ArrayList CacheDependency
		{
			get { return cacheDependency; }
			set { cacheDependency = value; }
		}


		private bool cacheable;
		// Jes1111
		/// <summary>
		/// Is Cacheable?
		/// </summary>
		public bool Cacheable
		{
			get { return cacheable; }
			set { cacheable = value; }
		}


		/// <summary>
		/// ModuleSettings
		/// </summary>
		public ModuleSettings()
		{
			ModuleID = 0;
			PaneName = "no pane";
			ModuleTitle = string.Empty;
			AuthorizedEditRoles = strAdmin;
			AuthorizedViewRoles = "All Users;";
			AuthorizedAddRoles = strAdmin;
			AuthorizedDeleteRoles = strAdmin;
			AuthorizedPropertiesRoles = strAdmin;
			AuthorizedMoveModuleRoles = strAdmin;
			AuthorizedDeleteModuleRoles = strAdmin;
			CacheTime = 0;
			ModuleOrder = 0;
			ShowMobile = false;
			DesktopSrc = string.Empty;
			MobileSrc = string.Empty;
			SupportCollapsable = false;
			SupportWorkflow = false;
		}

		// Added by gman3001 10/06/2004, to allow for the retrieval of a Module's Desktop src path
		// in order to dynamically load a module control by its Module ID
		/// <summary>
		/// The GetModuleDesktopSrc Method returns a string of
		/// the specified Module's Desktop Src, which can be used to 
		/// Load this modules control into a page.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns>The desktop source path string</returns>
		public static string GetModuleDesktopSrc(int moduleID)
		{
			string ControlPath = string.Empty;

			using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
			{
				if (dr.Read())
					ControlPath = Path.WebPathCombine(Path.ApplicationRoot, dr[strDesktopSrc].ToString());
				dr.Close(); //by Manu, fixed bug 807858
			}

			return ControlPath;
		}

		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database.  This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="_baseSettings"></param>
		/// <returns></returns>
		public static Hashtable GetModuleSettings(int moduleID, Hashtable _baseSettings)
		{
			if (!CurrentCache.Exists(Key.ModuleSettings(moduleID)))
			{
				// Get Settings for this module from the database
				Hashtable _settings = new Hashtable();

				// Create Instance of Connection and Command Object
				using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
				{
					using (SqlCommand myCommand = new SqlCommand("rb_GetModuleSettings", myConnection))
					{
						// Mark the Command as a SPROC
						myCommand.CommandType = CommandType.StoredProcedure;

						// Add Parameters to SPROC
						SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
						parameterModuleID.Value = moduleID;
						myCommand.Parameters.Add(parameterModuleID);

						// Execute the command
						myConnection.Open();
						using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
						{
							try
							{
								while (dr.Read())
									_settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
							}
							finally
							{
								dr.Close(); //by Manu, fixed bug 807858
							}
						}
					}
				}

				foreach (string key in _baseSettings.Keys)
				{
					if (_settings[key] != null)
					{
						SettingItem s = ((SettingItem) _baseSettings[key]);
						if (_settings[key].ToString() != string.Empty)
							s.Value = _settings[key].ToString();
					}
				}

				CurrentCache.Insert(Key.ModuleSettings(moduleID), _baseSettings);
			}
			else
				_baseSettings = (Hashtable) CurrentCache.Get(Key.ModuleSettings(moduleID));
			return _baseSettings;
		}

		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database. This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static Hashtable GetModuleSettings(int moduleID, Page page)
		{
			string ControlPath = Path.ApplicationRoot + "/";

			using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
			{
				try
				{
					if (dr.Read())
						ControlPath += dr[strDesktopSrc].ToString();
				}
				finally
				{
					dr.Close(); //by Manu, fixed bug 807858
				}
			}

			PortalModuleControl portalModule;
			Hashtable setting;
			try
			{
				portalModule = (PortalModuleControl) page.LoadControl(ControlPath);
				setting = GetModuleSettings(moduleID, portalModule.BaseSettings);
				return setting;
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
				throw;
			}
		}

		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database. This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		public static Hashtable GetModuleSettings(int moduleID)
		{
			return (GetModuleSettings(moduleID, new UI.Page()));
		}

		/// <summary>
		/// The UpdateModuleSetting Method updates a single module setting 
		/// in the ModuleSettings database table.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void UpdateModuleSetting(int moduleID, String key, String value)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleSetting", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);

					SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
					parameterKey.Value = key;
					myCommand.Parameters.Add(parameterKey);

					SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
					parameterValue.Value = value;
					myCommand.Parameters.Add(parameterValue);

					// Execute the command
					myConnection.Open();
					try
					{
						myCommand.ExecuteNonQuery();
					}
					finally
					{
						myConnection.Close();
					}
				}
			}

			//Invalidate cache
			CurrentCache.Remove(Key.ModuleSettings(moduleID));
		}

		/// <summary>
		/// GetModuleDefinitionByID
		/// </summary>
		/// <param name="ModuleID">ModuleID</param>
		/// <returns>A SqlDataReader</returns>
		public static SqlDataReader GetModuleDefinitionByID(int moduleID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = PortalSettings.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetModuleDefinitionByID", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			// Return the datareader
			return result;
		}
	}
}