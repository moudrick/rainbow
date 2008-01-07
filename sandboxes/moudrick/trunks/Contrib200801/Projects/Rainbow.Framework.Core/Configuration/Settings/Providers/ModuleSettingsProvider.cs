using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Rainbow.Framework.Data;
using Rainbow.Framework.Security;
using Rainbow.Framework.Settings.Cache;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Framework.Core.Configuration.Settings.Providers
{
    ///<summary>
    /// This is interface class for get module settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public class ModuleSettingsProvider
    {
        const string strDesktopSrc = "DesktopSrc";
        const string strATModuleID = "@ModuleID";
        
        /// <summary>
        /// The GetModuleDesktopSrc Method returns a string of
        /// the specified Module's Desktop Src, which can be used to
        /// Load this modules control into a page.
        /// </summary>
        /// <remarks>
        /// Added by gman3001 10/06/2004, to allow for the retrieval of a Module's Desktop src path
        /// in order to dynamically load a module control by its Module ID
        /// </remarks>
        /// <param name="moduleID">The module ID.</param>
        /// <returns>The desktop source path string</returns>
        public static string GetModuleDesktopSrc(int moduleID)
        {
            string controlPath = string.Empty;
            using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
            {
                if (dr.Read())
                {
                    controlPath = Path.WebPathCombine(Path.ApplicationRoot, dr[strDesktopSrc].ToString());
                }
            }
            return controlPath;
        }

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the database. This method is
        /// used by some user control modules to access misc settings.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public static Hashtable GetModuleSettings(int moduleID, Page page)
        {
            string controlPath = Path.ApplicationRoot + "/";
            using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
            {
                if (dr.Read())
                {
                    controlPath += dr[strDesktopSrc].ToString();
                }
            }

            try
            {
                PortalModuleControl portalModule = (PortalModuleControl)page.LoadControl(controlPath);
                Hashtable setting = GetModuleSettings(moduleID, portalModule.BaseSettings);
                return setting;
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem loading: '" + controlPath + "'", ex); // Jes1111
                //Rainbow.Framework.Configuration.ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
                //throw;
            }
        }

        /// <summary>
        /// Gets control path from current persistence for Module Settings
        /// </summary>
        /// <param name="moduleID">Module ID</param>
        /// <returns></returns>
        public static string GetControlPath(int moduleID)
        {
            string controlPath = Path.ApplicationRoot + "/";
            using (SqlDataReader dr = ModuleSettingsProvider.GetModuleDefinitionByID(moduleID))
            {
                if (dr.Read())
                {
                    controlPath += dr[strDesktopSrc].ToString();
                }
            }
            return controlPath;
        }

        /// <summary>
        /// Instantiates new module settings from parameters and appropriate persistence.
        /// </summary>
        /// <param name="moduleID">Module ID</param>
        /// <param name="moduleConfiguration">Current module configuration</param>
        /// <returns>New module instance</returns>
        public static ModuleSettings InstantiateNewModuleSettings(int moduleID, ModuleSettings moduleConfiguration)
        {
            string controlPath = string.Empty;
            SqlDataReader dr = ModuleSettingsProvider.GetModuleDefinitionByID(moduleID);
            try
            {
                if (dr.Read())
                {
                    controlPath = Path.ApplicationRoot + "/" + dr["DesktopSrc"];
                }
            }
            finally
            {
                dr.Close();
            }
            //Update settings
            ModuleSettings moduleSettings = new ModuleSettings();
            moduleSettings.ModuleID = moduleID;
            moduleSettings.PageID = moduleConfiguration.PageID;
            moduleSettings.PaneName = moduleConfiguration.PaneName;
            moduleSettings.ModuleTitle = moduleConfiguration.ModuleTitle;
            moduleSettings.AuthorizedEditRoles = string.Empty; //Readonly
            moduleSettings.AuthorizedViewRoles = string.Empty; //Readonly
            moduleSettings.AuthorizedAddRoles = string.Empty; //Readonly
            moduleSettings.AuthorizedDeleteRoles = string.Empty; //Readonly
            moduleSettings.AuthorizedPropertiesRoles = moduleConfiguration.AuthorizedPropertiesRoles;
            moduleSettings.CacheTime = moduleConfiguration.CacheTime;
            moduleSettings.ModuleOrder = moduleConfiguration.ModuleOrder;
            moduleSettings.ShowMobile = moduleConfiguration.ShowMobile;
            moduleSettings.DesktopSrc = controlPath;
            moduleSettings.MobileSrc = string.Empty; //Not supported yet
            // added bja@reedtek.com
            moduleSettings.SupportCollapsable = moduleConfiguration.SupportCollapsable;
            return moduleSettings;
        }

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the appropriate persistence.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns></returns>
        public static Hashtable GetModuleSettingsHashtable(int moduleID)
        {
            Hashtable settings = new Hashtable();
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetModuleSettings", myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Value = moduleID;
                    myCommand.Parameters.Add(parameterModuleID);

                    myConnection.Open();
                    using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the appropriate persistence.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// /// <param name="userID">The User ID.</param>
        /// <returns></returns>
        public static Hashtable GetModuleSettingsHashtable(int moduleID, Guid userID)
        {
            Hashtable settings = new Hashtable();
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetModuleUserSettings", myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
                    parameterModuleID.Value = moduleID;
                    myCommand.Parameters.Add(parameterModuleID);

                    SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
                    parameterUserID.Value = userID;
                    myCommand.Parameters.Add(parameterUserID);

                    myConnection.Open();
                    using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// The UpdateCustomModuleSetting Method updates a single module setting
        /// for the current user in the rb_ModuleUserSettings database table.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdateCustomModuleSetting(int moduleID, Guid userID, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the database. This method is
        /// used by some user control modules to access misc settings.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public static Hashtable GetModuleUserSettings(int moduleID, Guid userID, Page page)
        {
            string controlPath = ModuleSettingsProvider.GetControlPath(moduleID);
            try
            {
                PortalModuleControlCustom portalModule = (PortalModuleControlCustom)page.LoadControl(controlPath);
                Hashtable setting = GetModuleUserSettings(moduleID, (Guid)RainbowPrincipal.CurrentUser.Identity.ProviderUserKey, portalModule.CustomizedUserSettings);
                return setting;
            }
            catch (Exception ex)
            {
                // Rainbow.Framework.Configuration.ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
                // throw;
                throw new Rainbow.Framework.Exceptions.RainbowException(Rainbow.Framework.LogLevel.Fatal, "There was a problem loading: '" + controlPath + "'", ex);
            }
        }

        /// <summary>
        /// Retrieves the custom user settings for the current user for this module
        /// from the database.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="customSettings">The _custom settings.</param>
        /// <returns></returns>
        public static Hashtable GetModuleUserSettings(int moduleID, Guid userID, Hashtable customSettings)
        {
            Hashtable settings = ModuleSettingsProvider.GetModuleSettingsHashtable(moduleID, userID);

            foreach (string key in customSettings.Keys)
            {
                if (settings[key] != null)
                {
                    SettingItem s = ((SettingItem)customSettings[key]);
                    if (settings[key].ToString().Length != 0)
                    {
                        s.Value = settings[key].ToString();
                        //_customSettings[key] = s;
                    }
                }
            }
            return customSettings;
        }

        static SqlDataReader GetModuleDefinitionByID(int moduleID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection connection = DBHelper.SqlConnection;
            SqlCommand command = new SqlCommand("rb_GetModuleDefinitionByID", connection);

            // Mark the Command as a SPROC
            command.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int);
            parameterModuleID.Value = moduleID;
            command.Parameters.Add(parameterModuleID);

            // Execute the command
            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the database.  This method is
        /// used by some user control modules to access misc settings.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="baseSettings">The _base settings.</param>
        /// <returns></returns>
        public static Hashtable GetModuleSettings(int moduleID, Hashtable baseSettings)
        {
            if (!CurrentCache.Exists(Key.ModuleSettings(moduleID)))
            {
                Hashtable settings = ModuleSettingsProvider.GetModuleSettingsHashtable(moduleID);

                foreach (string key in baseSettings.Keys)
                {
                    if (settings[key] != null)
                    {
                        SettingItem s = ((SettingItem)baseSettings[key]);
                        if (settings[key].ToString().Length != 0)
                            s.Value = settings[key].ToString();
                    }
                }
                CurrentCache.Insert(Key.ModuleSettings(moduleID), baseSettings);
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.ModuleSettings(moduleID));
            }
            return baseSettings;
        }

        /// <summary>
        /// The GetModuleSettings Method returns a hashtable of
        /// custom module specific settings from the database. This method is
        /// used by some user control modules to access misc settings.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns></returns>
        public static Hashtable GetModuleSettings(int moduleID)
        {
            return (ModuleSettingsProvider.GetModuleSettings(moduleID, new Rainbow.Framework.Web.UI.Page()));
        }

        /// <summary>
        /// The UpdateModuleSetting Method updates a single module setting
        /// in the ModuleSettings database table.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdateModuleSetting(int moduleID, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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

                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                }
            }
            //Invalidate cache
            CurrentCache.Remove(Key.ModuleSettings(moduleID));
        }
    }
}
