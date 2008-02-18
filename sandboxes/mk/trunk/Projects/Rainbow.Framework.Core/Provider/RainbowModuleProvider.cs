using System;
using System.Collections;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Data;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers.Configuration;
using Rainbow.Framework.Security;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Framework.Providers
{
    ///<summary>
    /// This is interface class for get module settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public class RainbowModuleProvider : ProviderBase
    {
        const string strDesktopSrc = "DesktopSrc";
        const string strATAddRoles = "@AddRoles";
        const string strATAdmin = "@Admin";
        const string strATApprovalRoles = "@ApprovalRoles";
        const string strATAssemblyName = "@AssemblyName";
        const string strATCacheTime = "@CacheTime";
        const string strATClassName = "@ClassName";
        const string strATDeleteModuleRoles = "@DeleteModuleRoles";
        const string strATDeleteRoles = "@DeleteRoles";
        const string strATDesktopSrc = "@DesktopSrc";
        const string strATEditRoles = "@EditRoles";
        const string strATFriendlyName = "@FriendlyName";
        const string strATGeneralModDefID = "@GeneralModDefID";
        const string strATGuid = "@Guid";
        const string strATMobileSrc = "@MobileSrc";
        const string strATModuleDefID = "@ModuleDefID";
        const string strATModuleID = "@ModuleID";
        const string strATModuleOrder = "@ModuleOrder";
        const string strATModuleTitle = "@ModuleTitle";
        const string strATMoveModuleRoles = "@MoveModuleRoles";
        const string strATPaneName = "@PaneName";
        const string strATPortalID = "@PortalID";
        const string strATPropertiesRoles = "@PropertiesRoles";
        const string strATPublishingRoles = "@PublishingRoles";
        const string strATSearchable = "@Searchable";
        const string strATShowEveryWhere = "@ShowEveryWhere";
        const string strATShowMobile = "@ShowMobile";
        const string strATSupportCollapsable = "@SupportCollapsable";
        const string strATSupportWorkflow = "@SupportWorkflow";
        const string strATPageID = "@TabID";
        const string strATViewRoles = "@ViewRoles";
        const string strGUID = "GUID";
        const string strNoModule = "NO_MODULE";
        const string strrb_GetModulesInPage = "rb_GetModulesInTab";

        const string providerType = "module";

        /// <summary>
        /// Gets default configured Module provider.
        /// Singleton pattern standard member.
        /// </summary>
        /// <returns>Default instance of Portal Provider class</returns>
        public static RainbowModuleProvider Instance
        {
            get
            {
                return ProviderConfiguration.GetDefaultProviderFromCache<RainbowModuleProvider>(
                    providerType, RainbowContext.Current.HttpContext.Cache);
            }
        }
        
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
        public string GetModuleDesktopSrc(int moduleID)
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
        public Hashtable GetModuleSettings(int moduleID, Page page)
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
        string GetControlPath(int moduleID)
        {
            string controlPath = Path.ApplicationRoot + "/";
            using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
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
        public RainbowModule InstantiateNewModuleSettings(int moduleID, RainbowModule moduleConfiguration)
        {
            string controlPath = string.Empty;
            SqlDataReader dr = GetModuleDefinitionByID(moduleID);
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
            RainbowModule moduleSettings = CreateModuleSettings();
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
        Hashtable GetModuleSettingsHashtable(int moduleID)
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
        Hashtable GetModuleSettingsHashtable(int moduleID, Guid userID)
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
        public void UpdateCustomModuleSetting(int moduleID, Guid userID, string key, string value)
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
        public Hashtable GetModuleUserSettings(int moduleID, Guid userID, Page page)
        {
            string controlPath = GetControlPath(moduleID);
            try
            {
                PortalModuleControlCustom portalModule = (PortalModuleControlCustom)page.LoadControl(controlPath);
                Hashtable setting = GetModuleUserSettings(moduleID, RainbowPrincipal.CurrentUser.Identity.ProviderUserKey, portalModule.CustomizedUserSettings);
                return setting;
            }
            catch (Exception ex)
            {
                // Rainbow.Framework.Configuration.ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
                // throw;
                throw new Framework.Exceptions.RainbowException(LogLevel.Fatal, "There was a problem loading: '" + controlPath + "'", ex);
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
        public Hashtable GetModuleUserSettings(int moduleID, Guid userID, Hashtable customSettings)
        {
            Hashtable settings = GetModuleSettingsHashtable(moduleID, userID);

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

        SqlDataReader GetModuleDefinitionByID(int moduleID)
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
        public Hashtable GetModuleSettings(int moduleID, Hashtable baseSettings)
        {
            if (!CurrentCache.Exists(Key.ModuleSettings(moduleID)))
            {
                Hashtable settings = GetModuleSettingsHashtable(moduleID);

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
        public Hashtable GetModuleSettings(int moduleID)
        {
            return (GetModuleSettings(moduleID, new Web.UI.Page()));
        }

        /// <summary>
        /// The UpdateModuleSetting Method updates a single module setting
        /// in the ModuleSettings database table.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void UpdateModuleSetting(int moduleID, string key, string value)
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

        ///<summary>
        /// Instantiates RainbowModule.
        ///</summary>
        ///<returns></returns>
        public RainbowModule CreateModuleSettings()
        {
            return new RainbowModule();
        }

        /// <summary>
        /// AddGeneralModuleDefinitions
        /// </summary>
        /// <param name="generalModDefID">GeneralModDefID</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="desktopSrc">The desktop SRC.</param>
        /// <param name="mobileSrc">The mobile SRC.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="admin">if set to <c>true</c> [admin].</param>
        /// <param name="searchable">if set to <c>true</c> [searchable].</param>
        /// <returns>The newly created ID</returns>
        public Guid AddGeneralModuleDefinitions(Guid generalModDefID,
                                                string friendlyName,
                                                string desktopSrc,
                                                string mobileSrc,
                                                string assemblyName,
                                                string className,
                                                bool admin,
                                                bool searchable)
        {
            using (SqlConnection connection = DBHelper.SqlConnection)
            {
                using (
                    SqlCommand myCommand =
                        new SqlCommand("rb_AddGeneralModuleDefinitions", connection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterGeneralModDefID =
                        new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
                    parameterGeneralModDefID.Value = generalModDefID;
                    myCommand.Parameters.Add(parameterGeneralModDefID);
                    SqlParameter parameterFriendlyName =
                        new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
                    parameterFriendlyName.Value = friendlyName;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    SqlParameter parameterDesktopSrc =
                        new SqlParameter(strATDesktopSrc, SqlDbType.NVarChar, 256);
                    parameterDesktopSrc.Value = desktopSrc;
                    myCommand.Parameters.Add(parameterDesktopSrc);
                    SqlParameter parameterMobileSrc =
                        new SqlParameter(strATMobileSrc, SqlDbType.NVarChar, 256);
                    parameterMobileSrc.Value = mobileSrc;
                    myCommand.Parameters.Add(parameterMobileSrc);
                    SqlParameter parameterAssemblyName =
                        new SqlParameter(strATAssemblyName, SqlDbType.VarChar, 50);
                    parameterAssemblyName.Value = assemblyName;
                    myCommand.Parameters.Add(parameterAssemblyName);
                    SqlParameter parameterClassName =
                        new SqlParameter(strATClassName, SqlDbType.NVarChar, 128);
                    parameterClassName.Value = className;
                    myCommand.Parameters.Add(parameterClassName);
                    SqlParameter parameterAdmin = new SqlParameter(strATAdmin, SqlDbType.Bit);
                    parameterAdmin.Value = admin;
                    myCommand.Parameters.Add(parameterAdmin);
                    SqlParameter parameterSearchable =
                        new SqlParameter(strATSearchable, SqlDbType.Bit);
                    parameterSearchable.Value = searchable;
                    myCommand.Parameters.Add(parameterSearchable);
                    // Open the database connection and execute the command
                    connection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Warn,
                                             "An Error Occurred in AddGeneralModuleDefinitions. ",
                                             ex);
                    }

                    // Return the newly created ID
                    return new Guid(parameterGeneralModDefID.Value.ToString());
                }
            }
        }

        /// <summary>
        /// The AddModule method updates a specified Module within the Modules database table.
        /// If the module does not yet exist,the stored procedure adds it.<br/>
        /// AddModule Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="moduleOrder">The module order.</param>
        /// <param name="paneName">Name of the pane.</param>
        /// <param name="title">The title.</param>
        /// <param name="moduleDefID">The module def ID.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="editRoles">The edit roles.</param>
        /// <param name="viewRoles">The view roles.</param>
        /// <param name="addRoles">The add roles.</param>
        /// <param name="deleteRoles">The delete roles.</param>
        /// <param name="propertiesRoles">The properties roles.</param>
        /// <param name="moveModuleRoles">The move module roles.</param>
        /// <param name="deleteModuleRoles">The delete module roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="publishingRoles">The publishing roles.</param>
        /// <param name="supportWorkflow">if set to <c>true</c> [support workflow].</param>
        /// <param name="showEveryWhere">if set to <c>true</c> [show every where].</param>
        /// <param name="supportCollapsable">if set to <c>true</c> [support collapsable].</param>
        /// <returns></returns>
        [History("jviladiu@portalServices.net", "2004/08/19", "Added support for move & delete modules roles")]
        [History("john.mandia@whitelightsolutions.com", "2003/05/24", "Added support for showEveryWhere")]
        [History("bja@reedtek.com", "2003/05/16", "Added support for win. mgmt min/max/close -- supportCollapsable")]
        public int AddModule(int pageID,
                             int moduleOrder,
                             string paneName,
                             string title,
                             int moduleDefID,
                             int cacheTime,
                             string editRoles,
                             string viewRoles,
                             string addRoles,
                             string deleteRoles,
                             string propertiesRoles,
                             string moveModuleRoles,
                             string deleteModuleRoles,
                             bool showMobile,
                             string publishingRoles,
                             bool supportWorkflow,
                             bool showEveryWhere,
                             bool supportCollapsable)
        {
            // Changes by Geert.Audenaert@Syntegra.Com Date: 6/2/2003
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_AddModule", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterModuleID =
                        new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterModuleID);
                    SqlParameter parameterModuleDefinitionID =
                        new SqlParameter(strATModuleDefID, SqlDbType.Int, 4);
                    parameterModuleDefinitionID.Value = moduleDefID;
                    myCommand.Parameters.Add(parameterModuleDefinitionID);
                    SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterModuleOrder =
                        new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
                    parameterModuleOrder.Value = moduleOrder;
                    myCommand.Parameters.Add(parameterModuleOrder);
                    SqlParameter parameterTitle =
                        new SqlParameter(strATModuleTitle, SqlDbType.NVarChar, 256);
                    parameterTitle.Value = title;
                    myCommand.Parameters.Add(parameterTitle);
                    SqlParameter parameterPaneName =
                        new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
                    parameterPaneName.Value = paneName;
                    myCommand.Parameters.Add(parameterPaneName);
                    SqlParameter parameterCacheTime =
                        new SqlParameter(strATCacheTime, SqlDbType.Int, 4);
                    parameterCacheTime.Value = cacheTime;
                    myCommand.Parameters.Add(parameterCacheTime);
                    SqlParameter parameterEditRoles =
                        new SqlParameter(strATEditRoles, SqlDbType.NVarChar, 256);
                    parameterEditRoles.Value = editRoles;
                    myCommand.Parameters.Add(parameterEditRoles);
                    SqlParameter parameterViewRoles =
                        new SqlParameter(strATViewRoles, SqlDbType.NVarChar, 256);
                    parameterViewRoles.Value = viewRoles;
                    myCommand.Parameters.Add(parameterViewRoles);
                    SqlParameter parameterAddRoles =
                        new SqlParameter(strATAddRoles, SqlDbType.NVarChar, 256);
                    parameterAddRoles.Value = addRoles;
                    myCommand.Parameters.Add(parameterAddRoles);
                    SqlParameter parameterDeleteRoles =
                        new SqlParameter(strATDeleteRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteRoles.Value = deleteRoles;
                    myCommand.Parameters.Add(parameterDeleteRoles);
                    SqlParameter parameterPropertiesRoles =
                        new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
                    parameterPropertiesRoles.Value = propertiesRoles;
                    myCommand.Parameters.Add(parameterPropertiesRoles);
                    // Added by jviladiu@portalservices.net (19/08/2004)
                    SqlParameter parameterMoveModuleRoles =
                        new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
                    parameterMoveModuleRoles.Value = moveModuleRoles;
                    myCommand.Parameters.Add(parameterMoveModuleRoles);
                    // Added by jviladiu@portalservices.net (19/08/2004)
                    SqlParameter parameterDeleteModuleRoles =
                        new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteModuleRoles.Value = deleteModuleRoles;
                    myCommand.Parameters.Add(parameterDeleteModuleRoles);
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 6/2/2003
                    SqlParameter parameterPublishingRoles =
                        new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
                    parameterPublishingRoles.Value = publishingRoles;
                    myCommand.Parameters.Add(parameterPublishingRoles);
                    SqlParameter parameterSupportWorkflow =
                        new SqlParameter(strATSupportWorkflow, SqlDbType.Bit, 1);
                    parameterSupportWorkflow.Value = supportWorkflow;
                    myCommand.Parameters.Add(parameterSupportWorkflow);
                    // End Change Geert.Audenaert@Syntegra.Com
                    SqlParameter parameterShowMobile =
                        new SqlParameter(strATShowMobile, SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);
                    // Start Change john.mandia@whitelightsolutions.com
                    SqlParameter parameterShowEveryWhere =
                        new SqlParameter(strATShowEveryWhere, SqlDbType.Bit, 1);
                    parameterShowEveryWhere.Value = showEveryWhere;
                    myCommand.Parameters.Add(parameterShowEveryWhere);
                    // End Change  john.mandia@whitelightsolutions.com
                    // Start Change bja@reedtek.com
                    SqlParameter parameterSupportCollapsable =
                        new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
                    parameterSupportCollapsable.Value = supportCollapsable;
                    myCommand.Parameters.Add(parameterSupportCollapsable);
                    // End Change  bja@reedtek.com
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in AddModule. ", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in AddModule. ", ex);
                    }
                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The DeleteModule method deletes a specified Module from the Modules database table.<br/>
        /// DeleteModule Stored Procedure
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        [History("JB - john@bowenweb.com", "2005/05/12", "Added support for Recycler module")]
        public void DeleteModule(int moduleID)
        {
            //BOWEN 11 June 2005 - BEGIN
            Portal portalSettings = PortalProvider.Instance.CurrentPortal;
            bool useRecycler = bool.Parse(
                    PortalProvider.Instance.GetPortalCustomSettings(portalSettings.PortalID,
                    PortalProvider.Instance.GetPortalBaseSettings(
                    portalSettings.PortalPath))["SITESETTINGS_USE_RECYCLER"].ToString());

            // TODO: THIS LINE DISABLES THE RECYCLER DUE SOME TROUBLES WITH IT !!!!!! Fix those troubles and then discomment.
            useRecycler = false;

            using (SqlConnection connection = DBHelper.SqlConnection)
            {
                using (SqlCommand command =
                    new SqlCommand((useRecycler ? "rb_DeleteModuleToRecycler" : "rb_DeleteModule"), connection))
                {
                    // Mark the Command as a SPROC
                    command.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC

                    SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Value = moduleID;
                    command.Parameters.Add(parameterModuleID);

                    if (useRecycler) //Recycler needs some extra params for entry
                    {
                        // Add Recycler-specific Parameters to SPROC
                        SqlParameter paramDeletedBy = new SqlParameter("@DeletedBy", SqlDbType.NVarChar, 250);
                        paramDeletedBy.Value = MailHelper.GetCurrentUserEmailAddress();
                        command.Parameters.Add(paramDeletedBy);

                        SqlParameter paramDeletedDate = new SqlParameter("@DateDeleted", SqlDbType.DateTime, 8);
                        paramDeletedDate.Value = DateTime.Now;
                        command.Parameters.Add(paramDeletedDate);
                    }
                    //BOWEN 11 June 2005 - END
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Warn,
                            "An Error Occurred in DeleteModule. Parameter : " + moduleID, ex);
                    }
                }
            }
        }

        /// <summary>
        /// The DeleteModuleDefinition method deletes the specified
        /// module type definition from the portal.
        /// </summary>
        /// <param name="defID">The def ID.</param>
        /// <remarks>Other relevant sources: DeleteModuleDefinition Stored Procedure</remarks>
        public void DeleteModuleDefinition(Guid defID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_DeleteModuleDefinition", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterModuleDefID = new SqlParameter(strATModuleDefID, SqlDbType.UniqueIdentifier);
                    parameterModuleDefID.Value = defID;
                    myCommand.Parameters.Add(parameterModuleDefID);
                    // Open the database connection and execute the command
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in DeleteModuleDefinition. Parameter : " + defID.ToString(), ex);
                        ErrorHandler.Publish(LogLevel.Warn,
                            "An Error Occurred in DeleteModuleDefinition. Parameter : " + defID,
                            ex);
                    }
                }
            }
        }

        /// <summary>
        /// Exists the module products in page.
        /// </summary>
        /// <param name="tabID">The tab ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>A bool value...</returns>
        public bool ExistModuleProductsInPage(int tabID, int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand(strrb_GetModulesInPage, myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = tabID;
                    myCommand.Parameters.Add(parameterPageID);
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);

                    myConnection.Open();
                    Guid moduleGuid = new Guid("{EC24FABD-FB16-4978-8C81-1ADD39792377}");
                    bool retorno = false;

                    using (SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (result.Read())
                        {
                            if (moduleGuid.Equals(result.GetGuid(1))) retorno = true;
                        }
                    }
                    return retorno;
                }
            }
        }

        /// <summary>
        /// Find module id defined by the guid in a tab in the portal
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public ArrayList FindModuleItemsByGuid(int portalID, Guid guid)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_FindModulesByGuid", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
                    parameterFriendlyName.Value = guid;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    // Open the database connection and execute the command
                    myConnection.Open();
                    ArrayList modList = new ArrayList();

                    using (SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (result.Read())
                        {
                            ModuleItem m = new ModuleItem();
                            m.ID = (int)result["ModuleId"];
                            modList.Add(m);
                        }
                    }
                    return modList;
                }
            }
        }

        /// <summary>
        /// The GetModuleDefinitionByGUID method returns the id of the Module
        /// that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public int GetModuleDefinitionByGuid(int portalID, Guid guid)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetModuleDefinitionByGuid", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
                    parameterFriendlyName.Value = guid;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterModuleID);

                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Warn,
                            string.Format("An Error Occurred in GetModuleDefinitionByGuid. Parameter : {0}", guid),
                            ex);
                    }
                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The UpdateModuleOrder method update Modules Order.<br/>
        /// UpdateModuleOrder Stored Procedure
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="moduleOrder">The module order.</param>
        /// <param name="pane">The pane.</param>
        public void UpdateModuleOrder(int moduleID, int moduleOrder, string pane)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleOrder", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Value = moduleID;
                    myCommand.Parameters.Add(parameterModuleID);
                    SqlParameter parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
                    parameterModuleOrder.Value = moduleOrder;
                    myCommand.Parameters.Add(parameterModuleOrder);
                    SqlParameter parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
                    parameterPaneName.Value = pane;
                    myCommand.Parameters.Add(parameterPaneName);
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
                    }
                }
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="solutionID"></param>
        ///<param name="portalID"></param>
        public void UpdateSolutionModuleDefinition(int solutionID, int portalID)
        {
            // get module definitions
            SqlDataReader myReader = GetSolutionModuleDefinitions(solutionID);

            // Always call Read before accessing data.
            try
            {
                while (myReader.Read())
                {
                    UpdateModuleDefinitions(new Guid(myReader["GeneralModDefID"].ToString()), portalID, true);
                }
            }
            finally
            {
                myReader.Close(); //by Manu, fixed bug 807858
            }
        }

        /// <summary>
        /// The GetSolutionModuleDefinitions method returns a list of all module type definitions.<br></br>
        /// GetSolutionModuleDefinitions Stored Procedure
        /// </summary>
        /// <param name="solutionID">The solution ID.</param>
        /// <returns></returns>
        static SqlDataReader GetSolutionModuleDefinitions(int solutionID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
            SqlCommand myCommand = new SqlCommand("rb_GetSolutionModuleDefinitions", myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameterSolutionID = new SqlParameter("@SolutionID", SqlDbType.Int, 4);
            parameterSolutionID.Value = solutionID;
            myCommand.Parameters.Add(parameterSolutionID);
            // Open the database connection and execute the command
            myConnection.Open();
            SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            // Return the datareader
            return dr;
        }

        /// <summary>
        /// The UpdateModule method updates a specified Module within the Modules database table.
        /// If the module does not yet exist, the stored procedure adds it.<br/>
        /// UpdateModule Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="moduleOrder">The module order.</param>
        /// <param name="paneName">Name of the pane.</param>
        /// <param name="title">The title.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="editRoles">The edit roles.</param>
        /// <param name="viewRoles">The view roles.</param>
        /// <param name="addRoles">The add roles.</param>
        /// <param name="deleteRoles">The delete roles.</param>
        /// <param name="propertiesRoles">The properties roles.</param>
        /// <param name="moveModuleRoles">The move module roles.</param>
        /// <param name="deleteModuleRoles">The delete module roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="publishingRoles">The publishing roles.</param>
        /// <param name="supportWorkflow">if set to <c>true</c> [support workflow].</param>
        /// <param name="approvalRoles">The approval roles.</param>
        /// <param name="showEveryWhere">if set to <c>true</c> [show every where].</param>
        /// <param name="supportCollapsable">if set to <c>true</c> [support collapsable].</param>
        /// <returns></returns>
        public int UpdateModule(int pageID,
                                int moduleID,
                                int moduleOrder,
                                string paneName,
                                string title,
                                int cacheTime,
                                string editRoles,
                                string viewRoles,
                                string addRoles,
                                string deleteRoles,
                                string propertiesRoles,
                                string moveModuleRoles,
                                string deleteModuleRoles,
                                bool showMobile,
                                string publishingRoles,
                                bool supportWorkflow,
                                string approvalRoles,
                                bool showEveryWhere,
                                bool supportCollapsable)
        {
            // Changes by Geert.Audenaert@Syntegra.Com Date: 6/2/2003
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateModule", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Value = moduleID;
                    myCommand.Parameters.Add(parameterModuleID);
                    SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
                    parameterModuleOrder.Value = moduleOrder;
                    myCommand.Parameters.Add(parameterModuleOrder);
                    SqlParameter parameterTitle = new SqlParameter(strATModuleTitle, SqlDbType.NVarChar, 256);
                    parameterTitle.Value = title;
                    myCommand.Parameters.Add(parameterTitle);
                    SqlParameter parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
                    parameterPaneName.Value = paneName;
                    myCommand.Parameters.Add(parameterPaneName);
                    SqlParameter parameterCacheTime = new SqlParameter(strATCacheTime, SqlDbType.Int, 4);
                    parameterCacheTime.Value = cacheTime;
                    myCommand.Parameters.Add(parameterCacheTime);
                    SqlParameter parameterEditRoles = new SqlParameter(strATEditRoles, SqlDbType.NVarChar, 256);
                    parameterEditRoles.Value = editRoles;
                    myCommand.Parameters.Add(parameterEditRoles);
                    SqlParameter parameterViewRoles = new SqlParameter(strATViewRoles, SqlDbType.NVarChar, 256);
                    parameterViewRoles.Value = viewRoles;
                    myCommand.Parameters.Add(parameterViewRoles);
                    SqlParameter parameterAddRoles = new SqlParameter(strATAddRoles, SqlDbType.NVarChar, 256);
                    parameterAddRoles.Value = addRoles;
                    myCommand.Parameters.Add(parameterAddRoles);
                    SqlParameter parameterDeleteRoles = new SqlParameter(strATDeleteRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteRoles.Value = deleteRoles;
                    myCommand.Parameters.Add(parameterDeleteRoles);
                    SqlParameter parameterPropertiesRoles =
                        new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
                    parameterPropertiesRoles.Value = propertiesRoles;
                    myCommand.Parameters.Add(parameterPropertiesRoles);
                    // Added by jviladiu@portalservices.net (19/08/2004)
                    SqlParameter parameterMoveModuleRoles =
                        new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
                    parameterMoveModuleRoles.Value = moveModuleRoles;
                    myCommand.Parameters.Add(parameterMoveModuleRoles);
                    // Added by jviladiu@portalservices.net (19/08/2004)
                    SqlParameter parameterDeleteModuleRoles =
                        new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteModuleRoles.Value = deleteModuleRoles;
                    myCommand.Parameters.Add(parameterDeleteModuleRoles);
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 6/2/2003
                    SqlParameter parameterPublishingRoles =
                        new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
                    parameterPublishingRoles.Value = publishingRoles;
                    myCommand.Parameters.Add(parameterPublishingRoles);
                    SqlParameter parameterSupportWorkflow = new SqlParameter(strATSupportWorkflow, SqlDbType.Bit, 1);
                    parameterSupportWorkflow.Value = supportWorkflow;
                    myCommand.Parameters.Add(parameterSupportWorkflow);
                    // End Change Geert.Audenaert@Syntegra.Com
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 27/2/2003
                    SqlParameter parameterApprovalRoles = new SqlParameter(strATApprovalRoles, SqlDbType.NVarChar, 256);
                    parameterApprovalRoles.Value = approvalRoles;
                    myCommand.Parameters.Add(parameterApprovalRoles);
                    // End Change Geert.Audenaert@Syntegra.Com
                    SqlParameter parameterShowMobile = new SqlParameter(strATShowMobile, SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);
                    // Addition by john.mandia@whitelightsolutions.com to add show on every page functionality
                    SqlParameter parameterShowEveryWhere = new SqlParameter(strATShowEveryWhere, SqlDbType.Bit, 1);
                    parameterShowEveryWhere.Value = showEveryWhere;
                    myCommand.Parameters.Add(parameterShowEveryWhere);
                    // Change by baj@reedtek.com
                    // Date: 16/5/2003
                    SqlParameter parameterSupportCollapsable =
                        new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
                    parameterSupportCollapsable.Value = supportCollapsable;
                    myCommand.Parameters.Add(parameterSupportCollapsable);
                    // End Change baj@reedtek.com
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
                    }
                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// UpdateGeneralModuleDefinitions
        /// </summary>
        /// <param name="generalModDefID">GeneralModDefID</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="desktopSrc">The desktop SRC.</param>
        /// <param name="mobileSrc">The mobile SRC.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="admin">if set to <c>true</c> [admin].</param>
        /// <param name="searchable">if set to <c>true</c> [searchable].</param>
        public void UpdateGeneralModuleDefinitions(Guid generalModDefID,
                                                   string friendlyName,
                                                   string desktopSrc,
                                                   string mobileSrc,
                                                   string assemblyName,
                                                   string className,
                                                   bool admin,
                                                   bool searchable)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateGeneralModuleDefinitions", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Update Parameters to SPROC
                    SqlParameter parameterGeneralModDefID =
                        new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
                    parameterGeneralModDefID.Value = generalModDefID;
                    myCommand.Parameters.Add(parameterGeneralModDefID);
                    SqlParameter parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
                    parameterFriendlyName.Value = friendlyName;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    SqlParameter parameterDesktopSrc = new SqlParameter(strATDesktopSrc, SqlDbType.NVarChar, 256);
                    parameterDesktopSrc.Value = desktopSrc;
                    myCommand.Parameters.Add(parameterDesktopSrc);
                    SqlParameter parameterMobileSrc = new SqlParameter(strATMobileSrc, SqlDbType.NVarChar, 256);
                    parameterMobileSrc.Value = mobileSrc;
                    myCommand.Parameters.Add(parameterMobileSrc);
                    SqlParameter parameterAssemblyName = new SqlParameter(strATAssemblyName, SqlDbType.VarChar, 50);
                    parameterAssemblyName.Value = assemblyName;
                    myCommand.Parameters.Add(parameterAssemblyName);
                    SqlParameter parameterClassName = new SqlParameter(strATClassName, SqlDbType.NVarChar, 128);
                    parameterClassName.Value = className;
                    myCommand.Parameters.Add(parameterClassName);
                    SqlParameter parameterAdmin = new SqlParameter(strATAdmin, SqlDbType.Bit);
                    parameterAdmin.Value = admin;
                    myCommand.Parameters.Add(parameterAdmin);
                    SqlParameter parameterSearchable = new SqlParameter(strATSearchable, SqlDbType.Bit);
                    parameterSearchable.Value = searchable;
                    myCommand.Parameters.Add(parameterSearchable);
                    // Execute the command
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex));
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the module GUID.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns>A System.Guid value...</returns>
        public Guid GetModuleGuid(int moduleID)
        {
            Guid moduleGuid = Guid.Empty;
            string cacheGuid = Key.ModuleSettings(moduleID) + "GUID";
            if (CurrentCache.Get(cacheGuid) == null)
            {
                using (SqlConnection connection = DBHelper.SqlConnection)
                {
                    using (SqlCommand command = new SqlCommand("rb_GetGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                        parameterModuleID.Value = moduleID;
                        command.Parameters.Add(parameterModuleID);
                        connection.Open();
                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                moduleGuid = dr.GetGuid(0);
                            }
                        }
                    }
                }
                CurrentCache.Insert(cacheGuid, moduleGuid);
            }
            else
            {
                moduleGuid = (Guid)CurrentCache.Get(cacheGuid);
            }
            return moduleGuid;
        }

        /// <summary>
        /// The UpdateModuleDefinitions method updates
        /// all module definitions in every portal
        /// </summary>
        /// <param name="generalModDefID">The general mod def ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="ischecked">if set to <c>true</c> [ischecked].</param>
        public void UpdateModuleDefinitions(Guid generalModDefID, int portalID, bool ischecked)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection connection = DBHelper.SqlConnection)
            {
                using (SqlCommand command = new SqlCommand("rb_UpdateModuleDefinitions", connection))
                {
                    // Mark the Command as a SPROC
                    command.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterGeneralModDefID =
                        new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
                    parameterGeneralModDefID.Value = generalModDefID;
                    command.Parameters.Add(parameterGeneralModDefID);
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    command.Parameters.Add(parameterPortalID);
                    SqlParameter parameterischecked = new SqlParameter("@ischecked", SqlDbType.Bit);
                    parameterischecked.Value = ischecked;
                    command.Parameters.Add(parameterischecked);

                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                    }
                }
            }
        }
    }
}
