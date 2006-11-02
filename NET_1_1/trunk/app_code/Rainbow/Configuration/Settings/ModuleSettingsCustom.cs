using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Rainbow.Settings;
using Rainbow.UI.WebControls;

namespace Rainbow.Configuration
{
	/// <summary>
	/// ModuleSettingsCustom extends the ModuleSettings class to allow authenticated users
	/// to 'customize' a module to their own preference.
	/// </summary>
	public class ModuleSettingsCustom : ModuleSettings
	{
		const string strDesktopSrc = "DesktopSrc";
		
		#region Data Access Methods used only by PortalModuleControlCustom
		
		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database. This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static Hashtable GetModuleUserSettings(int moduleID, int userID, Page page)
		{
			string ControlPath = Path.ApplicationRoot + "/";

			using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
			{
				if(dr.Read())
				{
					ControlPath += dr[strDesktopSrc].ToString();
				}
			}

			PortalModuleControlCustom portalModule;
			Hashtable setting;
			try
			{
				portalModule = (PortalModuleControlCustom) page.LoadControl(ControlPath);
				setting = GetModuleUserSettings(moduleID, int.Parse(PortalSettings.CurrentUser.Identity.ID), portalModule.CustomizedUserSettings);
			}
			catch(Exception ex)
			{
				// Rainbow.Configuration.ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
				// throw;
				throw new RainbowException(LogLevel.Fatal, "There was a problem loading: '" + ControlPath + "'", ex);
			}
			return setting;
		}

		/// <summary>
		/// Retrieves the custom user settings for the current user for this module
		/// from the database.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		public static Hashtable GetModuleUserSettings(int moduleID, int userID, Hashtable _customSettings) 
		{			
			// Get Settings for this module from the database
			Hashtable _settings = new Hashtable();
				
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetModuleUserSettings", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);

					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);

					// Execute the command
					myConnection.Open();
					using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection)) 
					{
						while (dr.Read()) 
						{
							_settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
						}
					}
				}
			}

			
			//foreach (string key in _customSettings.Keys)
			foreach (string key in _customSettings.Keys)
			{
				if(_settings[key] != null)
				{
					SettingItem s = ((SettingItem) _customSettings[key]);
					if (_settings[key].ToString().Length != 0)
						s.Value = _settings[key].ToString();
					//_customSettings[key] = s;
				}
			}

			return _customSettings;
		}

		/// <summary>
		/// The UpdateCustomModuleSetting Method updates a single module setting 
		/// for the current user in the rb_ModuleUserSettings database table.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void UpdateCustomModuleSetting(int moduleID, int userID, string key, string value) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleUserSetting", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);

					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);

					SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
					parameterKey.Value = key;
					myCommand.Parameters.Add(parameterKey);
            
					SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
					parameterValue.Value = value;
					myCommand.Parameters.Add(parameterValue);
            
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}
		#endregion

	}
}