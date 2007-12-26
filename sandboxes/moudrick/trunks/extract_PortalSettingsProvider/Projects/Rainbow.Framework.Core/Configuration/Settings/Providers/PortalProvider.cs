using System;
using System.Collections;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Rainbow.Framework.Providers.RainbowRoleProvider;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Site.Data;
using Rainbow.Framework.Users.Data;
using Rainbow.Framework.Providers.RainbowSiteMapProvider;

namespace Rainbow.Framework.Core.Configuration.Settings.Providers
{
    ///<summary>
    /// This is interface class for get portal settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public class PortalProvider //: ProviderBase
    {
        struct moduleTemplate
        {
            public int id;
            public Guid GuidID;
        }

        struct tabTemplate
        {
            public int oldID;
            public int newID;
        }

        const string strATPortalID = "@PortalID";
        const string strATPageID = "@PageID";
        const string strATAlwaysShowEditButton = "@AlwaysShowEditButton";
        const string strATPortalName = "@PortalName";
        const string strATPortalPath = "@PortalPath";

        const string strrb_GetPortals = "rb_GetPortals";

        const string strAdmins = "Admins;";
        const string strAllUsers = "All Users";

        const string strRightPane = "RightPane";
        const string strContentPane = "ContentPane";
        const string strLeftPane = "LeftPane";

        const string strGUIDHTMLDocument = "{0B113F51-FEA3-499A-98E7-7B83C192FDBB}";
        const string strGUIDLanguageSwitcher = "{25E3290E-3B9A-4302-9384-9CA01243C00F}";
        const string strGUIDLogin = "{A0F1F62B-FDC7-4DE5-BBAD-A5DAF31D960A}";
        const string strGUIDManageUsers = "{B6A48596-9047-4564-8555-61E3B31D7272}";
        const string strGUIDModules = "{5E0DB0C7-FD54-4F55-ACF5-6ECF0EFA59C0}";
        const string strGUIDSecurityRoles = "{A406A674-76EB-4BC1-BB35-50CD2C251F9C}";
        const string strGUIDSiteSettings = "{EBBB01B1-FBB5-4E79-8FC4-59BCA1D0554E}";
        const string strGUIDPages = "{1C575D94-70FC-4A83-80C3-2087F726CBB3}";

        public static readonly PortalProvider Instance = new PortalProvider();

        ///<summary>
        /// Gets currently loaded portal object
        ///</summary>
        public PortalSettings CurrentPortal
        {
            get
            {
                return (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }
        }

        /// <summary>
        /// Gets portal custom settings from persistence
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public Hashtable GetPortalCustomSettings(int portalID)
        {
            Hashtable settings = new Hashtable();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalCustomSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterportalID.Value = portalID;
                    myCommand.Parameters.Add(parameterportalID);
                    // Execute the command
                    myConnection.Open();
                    SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    try
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                    finally
                    {
                        dr.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// Fills brief portal settings for edit
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="portalID"></param>
        public void FillPortalSettingsBrief(PortalSettings settings, int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsPortalID", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    // Open the database connection and execute the command
                    myConnection.Open();
                    SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection); //by Manu CloseConnection

                    try
                    {
                        if (result.Read())
                        {
                            settings.PortalID = Int32.Parse(result["PortalID"].ToString());
                            settings.PortalName = result["PortalName"].ToString();
                            settings.PortalAlias = result["PortalAlias"].ToString();
                            //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + result["PortalName"].ToString();
                            settings.PortalTitle = string.Concat(Config.PortalTitlePrefix, result["PortalName"].ToString());
                            settings.PortalPath = result["PortalPath"].ToString();
                            settings.ActivePage.PageID = 0;
                            // added Thierry (tiptopweb) used for dropdown for layout and theme
                            settings.ActivePage.PortalPath = settings.PortalPath;
                            settings.ActiveModule = 0;
                        }
                        else
                        {
                            throw new ProviderException();
                        }
                    }
                    finally
                    {
                        result.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings persistence.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void UpdatePortalSetting(int portalID, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdatePortalSetting", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC

                    SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterportalID.Value = portalID;
                    myCommand.Parameters.Add(parameterportalID);

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
        }

        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns></returns>
        public string GetLanguageList(string portalAlias)
        {
            string langlist = string.Empty;
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsLangList", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                    myCommand.Parameters.Add(parameterPortalAlias);
                    // Open the database connection and execute the command
                    myConnection.Open();

                    try
                    {
                        //Better null check here by Manu
                        object tmp = myCommand.ExecuteScalar();

                        if (tmp != null)
                        {
                            langlist = tmp.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Configuration.LogLevel.Warn, "Get languages from db", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "Failed to get languages from database.", ex); // Jes1111
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }
            return langlist;
        }

        /// <summary>
        /// The PortalSettings Factory Method encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to render
        /// a Portal Page view for a given request.<br/>
        /// These Portal Settings are stored within a SQL database, and are
        /// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        /// This stored procedure returns values as SPROC output parameters,
        /// and using three result sets.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        public PortalSettings InstantiateNewPortalSettings(int pageID, string portalAlias)
        {
            return new PortalSettings(pageID, portalAlias);
        }

        /// <summary>
        /// The PortalSettings Factory Method encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to get
        /// custom setting for a different portal than current (EditPortal.aspx.cs)<br/>
        /// These Portal Settings are stored within a SQL database, and are
        /// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        /// This overload it is used
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        public PortalSettings InstantiateNewPortalSettings(int portalID)
        {
            return new PortalSettings(portalID);
        }

        /// <summary>
        /// The CreatePortal method create a new basic portal based on solutions table.
        /// </summary>
        /// <param name="solutionID">The solution ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        [History("john.mandia@whitelightsolutions.com", "2003/05/26", "Added extra info so that sign in is added to home tab of new portal and lang switcher is added to module list")]
        [History("bja@reedtek.com", "2003/05/16", "Added extra parameter for collpasable window")]
        public int CreatePortal(int solutionID, string portalAlias, string portalName, string portalPath)
        {
            // Create a new portal
            int portalID = AddPortal(portalAlias, portalName, portalPath);

            ModulesDB modules = new ModulesDB();
            modules.UpdateSolutionModuleDefinition(solutionID, portalID);

            if (!Config.UseSingleUserBase)
            {
                // Create the stradmin User for the new portal
                UsersDB iRainbowMembershipProvider = new UsersDB();
                // Create the "Admins" role for the new portal
                //Guid roleID = User.AddRole("Admins");
                Guid roleID = RainbowRoleProvider.Instance.CreateRole(portalAlias, "Admins");
                Guid userID = iRainbowMembershipProvider.AddUser(portalAlias,
                                                                 "admin@rainbowportal.net",
                                                                 "admin",
                                                                 "admin");

                // Create a new row in a many to many table (userroles)
                // giving the "admins" role to the stradmin user
                iRainbowMembershipProvider.AddUserRole(roleID, userID);
            }

            PagesDB tabs = new PagesDB();
            // Create a new Page "home"
            int homePageID = tabs.AddPage(portalID, "Home", 1);
            // Create a new Page "admin"
            string localizedString = General.GetString("ADMIN_TAB_NAME");
            int adminPageID = tabs.AddPage(portalID, localizedString, strAdmins, 9999);
            // Add Modules for portal use
            // Html Document
            modules.UpdateModuleDefinitions(new Guid(strGUIDHTMLDocument), portalID, true);
            // Add Modules for portal administration
            // Site Settings (Admin)
            localizedString = General.GetString("MODULE_SITE_SETTINGS");
            modules.UpdateModuleDefinitions(new Guid(strGUIDSiteSettings), portalID, true);
            modules.AddModule(adminPageID, 1, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDSiteSettings)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Pages (Admin)
            localizedString = General.GetString("MODULE_TABS");
            modules.UpdateModuleDefinitions(new Guid(strGUIDPages), portalID, true);
            modules.AddModule(adminPageID, 2, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDPages)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Roles (Admin)
            localizedString = General.GetString("MODULE_SECURITY_ROLES");
            modules.UpdateModuleDefinitions(new Guid(strGUIDSecurityRoles), portalID, true);
            modules.AddModule(adminPageID, 3, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDSecurityRoles)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Manage Users (Admin)
            localizedString = General.GetString("MODULE_MANAGE_USERS");
            modules.UpdateModuleDefinitions(new Guid(strGUIDManageUsers), portalID, true);
            modules.AddModule(adminPageID, 4, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDManageUsers)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Module Definitions (Admin)
            localizedString = General.GetString("MODULE_MODULES");
            modules.UpdateModuleDefinitions(new Guid(strGUIDModules), portalID, true);
            modules.AddModule(adminPageID, 1, strRightPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDModules)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // End Change Geert.Audenaert@Syntegra.Com
            // Change by john.mandia@whitelightsolutions.com
            // Add Signin Module and put it on the hometab
            // Signin
            localizedString = General.GetString("MODULE_LOGIN", "Login");
            modules.UpdateModuleDefinitions(new Guid(strGUIDLogin), portalID, true);
            modules.AddModule(homePageID, -1, strLeftPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDLogin)), 0, strAdmins,
                              "Unauthenticated Users;Admins;", strAdmins, strAdmins, strAdmins, strAdmins, strAdmins,
                              false, string.Empty, false, false, false);
            // Add language switcher to available modules
            // Language Switcher
            modules.UpdateModuleDefinitions(new Guid(strGUIDLanguageSwitcher), portalID, true);
            // End of change by john.mandia@whitelightsolutions.com
            // Create paths
            CreatePortalPath(portalPath);
            return portalID;
        }

        /// <summary>
        /// Creates the portal.
        /// </summary>
        /// <param name="templateID">The template ID.</param>
        /// <param name="templateAlias">The template alias.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        public int CreatePortal(int templateID, string templateAlias, string portalAlias, string portalName,
                                 string portalPath)
        {
            int newPortalID;

            PagesDB tabs = new PagesDB();
            ModulesDB modules = new ModulesDB();
            //UsersDB users = new UsersDB();

            // create an Array to stores modules ID and GUID for finding them later
            ArrayList templateModules = new ArrayList();
            moduleTemplate module;
            // create an Array to stores tabs ID for finding them later
            ArrayList templateTabs = new ArrayList();
            tabTemplate tab;

            // Create a new portal
            newPortalID = AddPortal(portalAlias, portalName, portalPath);

            // Open the connection to the PortalTemplates Database
            SqlConnection myConnection = GetConnection();
            SqlConnection my2ndConnection = GetConnection();
            SqlConnection my3rdConnection = GetConnection();
            myConnection.Open();
            my2ndConnection.Open();
            my3rdConnection.Open();

            // get module definitions and save them in the new portal
            SqlDataReader myReader = GetTemplateModuleDefinitions(templateID, myConnection);

            // Always call Read before accessing data.
            while (myReader.Read())
            {
                module.id = (int)myReader["ModuleDefID"];
                module.GuidID = GetGeneralModuleDefinitionByName(myReader["FriendlyName"].ToString(), my2ndConnection);
                try
                {
                    // save module definitions in the new portal
                    modules.UpdateModuleDefinitions(module.GuidID, newPortalID, true);
                    // Save the modules into a list for finding them later
                    templateModules.Add(module);
                }
                catch
                {
                    // tried to add a Module thas doesn´t exists in this implementation of the portal
                }
            }

            myReader.Close();

            // TODO: Is this still valid? Admin user will be created the first time the portal is accessed
            //if (!Config.UseSingleUserBase)
            //{
            //    // TODO: multiple portals still not supported
            //    Guid userID;

            //    // Create the "admin" User for the new portal
            //    string AdminEmail = "admin@rainbowportal.net";
            //    userID = users.AddUser("admin", AdminEmail, "admin", newPortalID);

            //    // Create a new row in a many to many table (userroles)
            //    // giving the "admins" role to the "admin" user
            //    users.AddUserRole("admin", userID);
            //}

            // Get all the Tabs in the Template Portal, store IDs in a list for finding them later
            // and create the Tabs in the new Portal
            myReader = GetTabsByPortal(templateID, myConnection);

            // Always call Read before accessing data.
            while (myReader.Read())
            {
                // Save the tabs into a list for finding them later
                tab.oldID = (int)myReader["PageID"];
                tab.newID =
                    tabs.AddPage(newPortalID, myReader["PageName"].ToString(),
                                 Int32.Parse(myReader["PageOrder"].ToString()));
                templateTabs.Add(tab);
            }
            myReader.Close();

            //Clear SiteMaps Cache
            RainbowSiteMapProvider.ClearAllRainbowSiteMapCaches();

            // now I have to get them again to set up the ParentID for each Tab
            myReader = GetTabsByPortal(templateID, myConnection);

            // Always call Read before accessing data.
            while (myReader.Read())
            {
                // Find the news TabID and ParentTabID
                IEnumerator myEnumerator = templateTabs.GetEnumerator();
                int newTabID = -1;
                int newParentTabID = -1;

                while (myEnumerator.MoveNext() && (newTabID == -1 || newParentTabID == -1))
                {
                    tab = (tabTemplate)myEnumerator.Current;
                    if (tab.oldID == (int)myReader["PageID"])
                    {
                        newTabID = tab.newID;
                    }
                    if (tab.oldID == Int32.Parse("0" + myReader["ParentPageID"]))
                    {
                        newParentTabID = tab.newID;
                    }
                }

                if (newParentTabID == -1)
                {
                    newParentTabID = 0;
                }

                // Update the Tab in the new portal
                tabs.UpdatePage(newPortalID, newTabID, newParentTabID, myReader["PageName"].ToString(),
                                Int32.Parse(myReader["PageOrder"].ToString()), myReader["AuthorizedRoles"].ToString(),
                                myReader["MobilePageName"].ToString(), (bool)myReader["ShowMobile"]);

                // Finally use GetPortalSettings to access each Tab and its Modules in the Template Portal
                // and create them in the new Portal
                SqlDataReader result;

                try
                {
                    result = GetPageModules(Int32.Parse(myReader["PageID"].ToString()), my2ndConnection);

                    while (result.Read())
                    {
                        ModuleSettings m = new ModuleSettings();
                        m.ModuleID = (int)result["ModuleID"];
                        m.ModuleDefID = (int)result["ModuleDefID"];
                        m.PageID = newTabID;
                        m.PaneName = (string)result["PaneName"];
                        m.ModuleTitle = (string)result["ModuleTitle"];

                        object myValue = result["AuthorizedEditRoles"];
                        m.AuthorizedEditRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedViewRoles"];
                        m.AuthorizedViewRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedAddRoles"];
                        m.AuthorizedAddRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedDeleteRoles"];
                        m.AuthorizedDeleteRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedPropertiesRoles"];
                        m.AuthorizedPropertiesRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedMoveModuleRoles"];
                        m.AuthorizedMoveModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedDeleteModuleRoles"];
                        m.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["AuthorizedPublishingRoles"];
                        m.AuthorizedPublishingRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["SupportWorkflow"];
                        m.SupportWorkflow = !Convert.IsDBNull(myValue) ? (bool)myValue : false;

                        myValue = result["AuthorizedApproveRoles"];
                        m.AuthorizedApproveRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;

                        myValue = result["WorkflowState"];
                        m.WorkflowStatus = !Convert.IsDBNull(myValue)
                                               ? (WorkflowState)(0 + (byte)myValue)
                                               : WorkflowState.Original;

                        try
                        {
                            myValue = result["SupportCollapsable"];
                        }
                        catch
                        {
                            myValue = DBNull.Value;
                        }
                        m.SupportCollapsable = DBNull.Value != myValue ? (bool)myValue : false;

                        try
                        {
                            myValue = result["ShowEveryWhere"];
                        }
                        catch
                        {
                            myValue = DBNull.Value;
                        }
                        m.ShowEveryWhere = DBNull.Value != myValue ? (bool)myValue : false;

                        m.CacheTime = int.Parse(result["CacheTime"].ToString());
                        m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());

                        myValue = result["ShowMobile"];
                        m.ShowMobile = !Convert.IsDBNull(myValue) ? (bool)myValue : false;

                        // Find the new ModuleDefID assigned to the module in the new portal
                        myEnumerator = templateModules.GetEnumerator();
                        int newModuleDefID = 0;

                        while (myEnumerator.MoveNext() && newModuleDefID == 0)
                        {
                            module = (moduleTemplate)myEnumerator.Current;
                            if (module.id == m.ModuleDefID)
                                newModuleDefID = modules.GetModuleDefinitionByGuid(newPortalID, module.GuidID);
                        }

                        if (newModuleDefID > 0)
                        {
                            // add the module to the new tab
                            int newModuleID = modules.AddModule(newTabID, m.ModuleOrder, m.PaneName, m.ModuleTitle,
                                                                newModuleDefID, m.CacheTime, m.AuthorizedEditRoles,
                                                                m.AuthorizedViewRoles,
                                                                m.AuthorizedAddRoles, m.AuthorizedDeleteRoles,
                                                                m.AuthorizedPropertiesRoles,
                                                                m.AuthorizedMoveModuleRoles,
                                                                m.AuthorizedDeleteModuleRoles,
                                                                m.ShowMobile, m.AuthorizedPublishingRoles,
                                                                m.SupportWorkflow,
                                                                m.ShowEveryWhere, m.SupportCollapsable);
                            // At the end, get all ModuleSettings and save them in the new module
                            SqlDataReader dr = GetModuleSettings(m.ModuleID, my3rdConnection);

                            while (dr.Read())
                            {
                                ModuleSettingsProvider.UpdateModuleSetting(newModuleID, dr["SettingName"].ToString(),
                                                                   dr["SettingValue"].ToString());
                            }
                            dr.Close();
                        }
                    }

                    result.Close();
                }
                catch
                {
                    // Error? ignore Tab ...
                }
            }
            myReader.Close();

            // Set the CustomSettings of the New Portal based in the Template Portal
            myReader = GetPortalCustomSettings(templateID, myConnection);

            // Always call Read before accessing data.
            while (myReader.Read())
            {
                PortalSettings.UpdatePortalSetting(newPortalID, myReader["SettingName"].ToString(),
                                                   myReader["SettingValue"].ToString());
            }

            myReader.Close();

            // close the conections
            myConnection.Close();
            myConnection.Dispose();
            my2ndConnection.Close();
            my2ndConnection.Dispose();
            my3rdConnection.Close();
            my3rdConnection.Dispose();

            // Create paths
            PortalProvider.Instance.CreatePortalPath(portalPath);

            return newPortalID;
        }

        /// <summary>
        /// Gets the portals.
        /// </summary>
        /// <returns></returns>
        public DataSet GetPortalsDataSet()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = GetConnection();
            string selectSQL = "SELECT PortalID, PortalAlias from rb_Portals WHERE PortalID >= 0";
            SqlDataAdapter myCommand = new SqlDataAdapter(selectSQL, myConnection);

            // Create and Fill the DataSet
            DataSet myDataSet = new DataSet();
            try
            {
                myCommand.Fill(myDataSet);
            }
            finally
            {
                myCommand.Dispose();
                myConnection.Close();
                myConnection.Dispose();
            }
            // Return the dataset
            return myDataSet;
        }

        /// <summary>
        /// Removes portal from database. All tabs, modules and data wil be removed.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        public void DeletePortal(int portalID)
        {
            try
            {
                // Create Instance of Connection and Command Object
                using (SqlConnection myConnection = Config.SqlConnectionString)
                {
                    using (SqlCommand myCommand = new SqlCommand("rb_DeletePortal", myConnection))
                    {
                        // Mark the Command as a SPROC
                        myCommand.CommandType = CommandType.StoredProcedure;
                        // Add Parameters to SPROC
                        SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                        parameterPortalID.Value = portalID;
                        myCommand.Parameters.Add(parameterPortalID);
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
            }
            catch (SqlException sqlex)
            {
                string aux =
                    General.GetString("DELETE_PORTAL_ERROR", "There was an error on deleting the portal", this);
                Rainbow.Framework.ErrorHandler.Publish(Rainbow.Framework.LogLevel.Error, aux, sqlex);
                throw new ProviderException(aux, sqlex);
            }
        }

        /// <summary>
        /// The UpdatePortalInfo method updates the name and access settings for the portal.<br/>
        /// Uses UpdatePortalInfo Stored Procedure.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <param name="alwaysShow">if set to <c>true</c> [always show].</param>
        public void UpdatePortalInfo(int portalID, string portalName, string portalPath, bool alwaysShow)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdatePortalInfo", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPortalName = new SqlParameter(strATPortalName, SqlDbType.NVarChar, 128);
                    parameterPortalName.Value = portalName;
                    myCommand.Parameters.Add(parameterPortalName);
                    // jes1111
                    //					string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
                    //
                    //					if(pd!=null)
                    //					{
                    //						if (portalPath.IndexOf (pd) > -1)
                    //							portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
                    //					}
                    string pd = Config.PortalsDirectory;
                    if (portalPath.IndexOf(pd) > -1)
                        portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

                    SqlParameter parameterPortalPath = new SqlParameter(strATPortalPath, SqlDbType.NVarChar, 128);
                    parameterPortalPath.Value = portalPath;
                    myCommand.Parameters.Add(parameterPortalPath);
                    SqlParameter parameterAlwaysShow = new SqlParameter(strATAlwaysShowEditButton, SqlDbType.Bit, 1);
                    parameterAlwaysShow.Value = alwaysShow;
                    myCommand.Parameters.Add(parameterAlwaysShow);
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
        }

        /// <summary>
        /// The GetPortals method returns an ArrayList containing all of the
        /// Portals registered in this database.<br/>
        /// </summary>
        /// <returns></returns>
        public ArrayList GetPortals()
        {
            ArrayList list = new ArrayList();

            SqlDataReader dr = GetPortalsSqlDataReader();
            try
            {
                while (dr.Read())
                {
                    PortalItem p = new PortalItem();
                    p.Name = dr["PortalName"].ToString();
                    p.Path = dr["PortalPath"].ToString();
                    p.ID = Convert.ToInt32(dr["PortalID"].ToString());
                    list.Add(p);
                }
            }
            finally
            {
                dr.Close(); //by Manu, fixed bug 807858
            }
            return list;
        }

        /// <summary>
        /// The GetPortals method returns an ArrayList containing all of the
        /// Portals registered in this database.<br/>
        /// GetPortals Stored Procedure
        /// </summary>
        /// <returns>portals</returns>
        public ArrayList GetPortalsArrayList()
        {
            ArrayList portals = new ArrayList();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand(strrb_GetPortals, myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Execute the command
                    myConnection.Open();

                    using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        try
                        {
                            while (dr.Read())
                            {
                                PortalItem p = new PortalItem();
                                p.Name = dr["PortalName"].ToString();
                                p.Path = dr["PortalPath"].ToString();
                                p.ID = Convert.ToInt32(dr["PortalID"].ToString());
                                portals.Add(p);
                            }
                        }

                        finally
                        {
                            dr.Close(); //by Manu, fixed bug 807858
                        }
                    }
                    // Return the portals
                    return portals;
                }
            }
        }

        internal void FillPortalSettingsFull(PortalSettings settings, int pageID, string portalAlias)
        {
            settings.CurrentLayout = "Default";
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                    myCommand.Parameters.Add(parameterPortalAlias);

                    SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);

                    SqlParameter parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12);
                    parameterPortalLanguage.Value = settings.PortalContentLanguage.Name;
                    myCommand.Parameters.Add(parameterPortalLanguage);

                    // Add out parameters to Sproc
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalID);

                    SqlParameter parameterPortalName = new SqlParameter("@PortalName", SqlDbType.NVarChar, 128);
                    parameterPortalName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalName);

                    SqlParameter parameterPortalPath = new SqlParameter("@PortalPath", SqlDbType.NVarChar, 128);
                    parameterPortalPath.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalPath);

                    SqlParameter parameterEditButton = new SqlParameter("@AlwaysShowEditButton", SqlDbType.Bit, 1);
                    parameterEditButton.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterEditButton);

                    SqlParameter parameterPageName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50);
                    parameterPageName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageName);

                    SqlParameter parameterPageOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterPageOrder.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageOrder);

                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterParentPageID);

                    SqlParameter parameterMobilePageName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobilePageName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterMobilePageName);

                    SqlParameter parameterAuthRoles = new SqlParameter("@AuthRoles", SqlDbType.NVarChar, 256);
                    parameterAuthRoles.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterAuthRoles);

                    SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterShowMobile);

                    SqlDataReader result;
                    try
                    {
                        // Open the database connection and execute the command
                        //						try // jes1111
                        //						{
                        myConnection.Open();
                        result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                        // Read the first resultset -- Desktop Page Information
                        while (result.Read())
                        {
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int)result["PageID"];
                            tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                            tabDetails.PageName = (string)result["PageName"];
                            tabDetails.PageOrder = (int)result["PageOrder"];
                            tabDetails.PageLayout = settings.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            settings.PortalAlias = portalAlias;
                            // Update the AuthorizedRoles Variable
                            settings.DesktopPages.Add(tabDetails);
                        }

                        if (settings.DesktopPages.Count == 0)
                        {
                            return; //Abort load
                            //throw new Exception("The portal you requested has no Pages. PortalAlias: '" + portalAlias + "'", new HttpException(404, "Portal not found"));
                        }
                        // Read the second result --  Mobile Page Information
                        result.NextResult();

                        while (result.Read())
                        {
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int)result["PageID"];
                            tabDetails.PageName = (string)result["MobilePageName"];
                            tabDetails.PageLayout = settings.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            settings.MobilePages.Add(tabDetails);
                        }
                        // Read the third result --  Module Page Information
                        result.NextResult();

                        while (result.Read())
                        {
                            ModuleSettings m = new ModuleSettings();
                            m.ModuleID = (int)result["ModuleID"];
                            m.ModuleDefID = (int)result["ModuleDefID"];
                            m.GuidID = (Guid)result["GeneralModDefID"];
                            m.PageID = (int)result["TabID"];
                            m.PaneName = (string)result["PaneName"];
                            m.ModuleTitle = (string)result["ModuleTitle"];
                            object myValue = result["AuthorizedEditRoles"];
                            m.AuthorizedEditRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedViewRoles"];
                            m.AuthorizedViewRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedAddRoles"];
                            m.AuthorizedAddRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedDeleteRoles"];
                            m.AuthorizedDeleteRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedPropertiesRoles"];
                            m.AuthorizedPropertiesRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            // jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
                            myValue = result["AuthorizedMoveModuleRoles"];
                            m.AuthorizedMoveModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedDeleteModuleRoles"];
                            m.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            // Change by Geert.Audenaert@Syntegra.Com
                            // Date: 6/2/2003
                            myValue = result["AuthorizedPublishingRoles"];
                            m.AuthorizedPublishingRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["SupportWorkflow"];
                            m.SupportWorkflow = !Convert.IsDBNull(myValue) ? (bool)myValue : false;
                            // Date: 27/2/2003
                            myValue = result["AuthorizedApproveRoles"];
                            m.AuthorizedApproveRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["WorkflowState"];
                            m.WorkflowStatus = !Convert.IsDBNull(myValue) ? (WorkflowState)(0 + (byte)myValue) : WorkflowState.Original;

                            // End Change Geert.Audenaert@Syntegra.Com
                            // Start Change bja@reedtek.com
                            try
                            {
                                myValue = result["SupportCollapsable"];
                            }
                            catch
                            {
                                myValue = DBNull.Value;
                            }
                            m.SupportCollapsable = DBNull.Value != myValue ? (bool)myValue : false;

                            // End Change  bja@reedtek.com
                            // Start Change john.mandia@whitelightsolutions.com
                            try
                            {
                                myValue = result["ShowEveryWhere"];
                            }
                            catch
                            {
                                myValue = DBNull.Value;
                            }
                            m.ShowEveryWhere = DBNull.Value != myValue ? (bool)myValue : false;
                            // End Change  john.mandia@whitelightsolutions.com
                            m.CacheTime = int.Parse(result["CacheTime"].ToString());
                            m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());
                            myValue = result["ShowMobile"];
                            m.ShowMobile = !Convert.IsDBNull(myValue) ? (bool)myValue : false;
                            m.DesktopSrc = result["DesktopSrc"].ToString();
                            m.MobileSrc = result["MobileSrc"].ToString();
                            m.Admin = bool.Parse(result["Admin"].ToString());
                            settings.ActivePage.Modules.Add(m);
                        }
                        // Now read Portal out params 
                        result.NextResult();
                        result.Close(); //by Manu, fixed bug 807858

                        settings.PortalID = (int)parameterPortalID.Value;
                        settings.PortalName = (string)parameterPortalName.Value;
                        //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
                        settings.PortalTitle = String.Concat(Config.PortalTitlePrefix, settings.PortalName);
                        //jes1111 - this.PortalPath = Settings.Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
                        settings.PortalPath = Path.WebPathCombine(Config.PortalsDirectory, (string)parameterPortalPath.Value);
                        //jes1111 - this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003
                        settings.PortalSecurePath = Config.PortalSecureDirectory;

                        //ActivePage initialization
                        settings.ActivePage.PageID = pageID;
                        settings.ActivePage.PageLayout = settings.CurrentLayout;
                        settings.ActivePage.ParentPageID = Int32.Parse("0" + parameterParentPageID.Value);
                        settings.ActivePage.PageOrder = (int)parameterPageOrder.Value;
                        settings.ActivePage.MobilePageName = (string)parameterMobilePageName.Value;
                        settings.ActivePage.AuthorizedRoles = (string)parameterAuthRoles.Value;
                        settings.ActivePage.PageName = (string)parameterPageName.Value;
                        settings.ActivePage.ShowMobile = (bool)parameterShowMobile.Value;
                        settings.ActivePage.PortalPath = settings.PortalPath; // thierry@tiptopweb.com.au for page custom layout
                    }
                    catch (SqlException sqlException)
                    {
                        throw new ProviderException("Error load portal", sqlException);
                    }
                    finally
                    {
                        //by Manu fix close bug #2
                        if (myConnection.State == ConnectionState.Open)
                        {
                            myConnection.Close();
                        }
                    }
                }
            }
        }

        /// AddPortal Stored Procedure
        int AddPortal(string portalAlias, string portalName, string portalPath)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_AddPortal", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias;
                    myCommand.Parameters.Add(parameterPortalAlias);
                    SqlParameter parameterPortalName = new SqlParameter(strATPortalName, SqlDbType.NVarChar, 128);
                    parameterPortalName.Value = portalName;
                    myCommand.Parameters.Add(parameterPortalName);
                    //jes1111
                    //					string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
                    //
                    //					if(pd!=null)
                    //					{
                    //					if (portalPath.IndexOf (pd) > -1)
                    //						portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
                    //					}
                    string pd = Config.PortalsDirectory;
                    if (portalPath.IndexOf(pd) > -1)
                        portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

                    SqlParameter parameterPortalPath = new SqlParameter(strATPortalPath, SqlDbType.NVarChar, 128);
                    parameterPortalPath.Value = portalPath;
                    myCommand.Parameters.Add(parameterPortalPath);
                    SqlParameter parameterAlwaysShow = new SqlParameter(strATAlwaysShowEditButton, SqlDbType.Bit, 1);
                    parameterAlwaysShow.Value = false;
                    myCommand.Parameters.Add(parameterAlwaysShow);
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalID);
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }

                    finally
                    {
                        myConnection.Close();
                    }
                    return (int)parameterPortalID.Value;
                }
            }
        }

        /// <summary>
        /// Gets the template module definitions.
        /// </summary>
        /// <param name="templateID">The template ID.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        SqlDataReader GetTemplateModuleDefinitions(int templateID, SqlConnection myConnection)
        {
            SqlCommand myCommand = new SqlCommand("rb_GetCurrentModuleDefinitions", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
            parameterPortalID.Value = templateID;
            myCommand.Parameters.Add(parameterPortalID);

            // execute the command
            SqlDataReader dr = myCommand.ExecuteReader();

            // Return the datareader
            return dr;
        }

//        /// <summary>
//        /// Gets the portal roles.
//        /// </summary>
//        /// <param name="templateID">The template ID.</param>
//        /// <param name="myConnection">My connection.</param>
//        /// <returns></returns>
//        SqlDataReader GetPortalRoles(int templateID, SqlConnection myConnection)
//        {
//
//            // Create Instance of Command Object
//            SqlCommand myCommand = new SqlCommand("rb_GetPortalRoles", myConnection);
//
//            // Mark the Command as a SPROC
//            myCommand.CommandType = CommandType.StoredProcedure;
//
//            // Add Parameters to SPROC
//            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
//            parameterPortalID.Value = templateID;
//            myCommand.Parameters.Add(parameterPortalID);
//
//            // execute the command
//            SqlDataReader dr = myCommand.ExecuteReader();
//
//            // Return the datareader
//            return dr;
//        }

        /// <summary>
        /// Gets the portal custom settings.
        /// </summary>
        /// <param name="templateID">The template ID.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        SqlDataReader GetPortalCustomSettings(int templateID, SqlConnection myConnection)
        {
            // Create Instance of Command Object
            SqlCommand myCommand = new SqlCommand("rb_GetPortalCustomSettings", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
            parameterPortalID.Value = templateID;
            myCommand.Parameters.Add(parameterPortalID);

            // execute the command
            SqlDataReader dr = myCommand.ExecuteReader();

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// Gets the name of the general module definition by.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        Guid GetGeneralModuleDefinitionByName(string moduleName, SqlConnection myConnection)
        {
            // Instance of Command Object
            SqlCommand myCommand = new SqlCommand("rb_GetGeneralModuleDefinitionByName", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterFriendlyName = new SqlParameter("@FriendlyName", SqlDbType.NVarChar, 128);
            parameterFriendlyName.Value = moduleName;
            myCommand.Parameters.Add(parameterFriendlyName);

            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.UniqueIdentifier);
            parameterModuleID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterModuleID);

            // Execute the command
            myCommand.ExecuteNonQuery();

            if (parameterModuleID.Value != null && parameterModuleID.Value.ToString().Length != 0)
            {
                try
                {
                    return new Guid(parameterModuleID.Value.ToString());
                }
                catch (Exception ex)
                {
                    string message = "'" + parameterModuleID.Value + "' seems not a valid GUID.";
                    ErrorHandler.Publish(LogLevel.Error, message, ex);
                    throw new ProviderException(message, ex);
                }
            }
            else
            {
                string message = "Invalid GUID: Null GUID!.";
                ErrorHandler.Publish(LogLevel.Error, message);
                throw new ProviderException(message);
            }
        }

        /// <summary>
        /// Gets the page modules.
        /// </summary>
        /// <param name="tabID">The tab ID.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        SqlDataReader GetPageModules(int tabID, SqlConnection myConnection)
        {
            string selectSQL = "select ModuleID, ModuleDefID, ModuleOrder, PaneName, ModuleTitle, " +
                               "AuthorizedEditRoles, AuthorizedViewRoles, AuthorizedAddRoles, " +
                               "AuthorizedDeleteRoles, AuthorizedPropertiesRoles, CacheTime, " +
                               "ShowMobile, AuthorizedPublishingRoles, SupportWorkflow, " +
                               "AuthorizedApproveRoles, WorkflowState, SupportCollapsable, " +
                               "ShowEveryWhere, AuthorizedMoveModuleRoles, AuthorizedDeleteModuleRoles " +
                               "from rb_Modules where TabID=" + tabID;
            SqlCommand myCommand = new SqlCommand(selectSQL, myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.Text;

            // execute the command
            SqlDataReader dr = myCommand.ExecuteReader();

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// Gets the module settings.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        SqlDataReader GetModuleSettings(int moduleID, SqlConnection myConnection)
        {
            SqlCommand myCommand = new SqlCommand("rb_GetModuleSettings", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            // Execute the command
            SqlDataReader dr = myCommand.ExecuteReader();

            return dr;
        }

        /// <summary>
        /// Gets the tabs by portal.
        /// </summary>
        /// <param name="templateID">The template ID.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        SqlDataReader GetTabsByPortal(int templateID, SqlConnection myConnection)
        {
            SqlCommand myCommand = new SqlCommand("rb_GetTabsByPortal", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
            parameterPortalID.Value = templateID;
            myCommand.Parameters.Add(parameterPortalID);

            // Execute the command
            SqlDataReader dr = myCommand.ExecuteReader();

            return dr;
        }

        /// <summary>
        /// Creates the portal path.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        void CreatePortalPath(string portalPath)
        {
            portalPath = portalPath.Replace("/", string.Empty);
            portalPath = portalPath.Replace("\\", string.Empty);
            portalPath = portalPath.Replace(".", string.Empty);

            if (!portalPath.StartsWith("_"))
                portalPath = "_" + portalPath;
            // jes1111			
            //			string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
            //
            //			if(pd!=null)
            //			{
            //			if (portalPath.IndexOf (pd) > -1)
            //				portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
            //			}
            string pd = Config.PortalsDirectory;
            if (portalPath.IndexOf(pd) > -1)
                portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

            // jes1111 - string portalPhisicalDir = HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + ConfigurationSettings.AppSettings[strPortalsDirectory] + "/" + portalPath);
            string portalPhisicalDir =
                HttpContext.Current.Server.MapPath(
                    Path.WebPathCombine(Path.ApplicationRoot, Config.PortalsDirectory, portalPath));
            if (!System.IO.Directory.Exists(portalPhisicalDir))
            {
                System.IO.Directory.CreateDirectory(portalPhisicalDir);
            }
            // Subdirs
            string[] subdirs = { "images", "polls", "documents", "xml" };
            for (int i = 0; i <= subdirs.GetUpperBound(0); i++)
            {
                if (!System.IO.Directory.Exists(portalPhisicalDir + "\\" + subdirs[i]))
                {
                    System.IO.Directory.CreateDirectory(portalPhisicalDir + "\\" + subdirs[i]);
                }
            }
        }

        /// GetPortals Stored Procedure
        SqlDataReader GetPortalsSqlDataReader()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetPortals", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader 
            return result;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        SqlConnection GetConnection()
        {
            // Watch if there's a Template's database in the config.sys
            // else, use the same database as the portal
            //jes1111- string portalSqlConnectionID = "PortalTemplatesConnectionString";
            string strSqlConnection;

            //jes1111 - if(ConfigurationSettings.AppSettings[portalSqlConnectionID] != null)
            if (Config.PortalTemplatesConnectionString.Length != 0)
                //jes1111 - strSqlConnection = ConfigurationSettings.AppSettings[portalSqlConnectionID];
                strSqlConnection = Config.PortalTemplatesConnectionString;
            else
                //jes1111 - strSqlConnection = ConfigurationSettings.AppSettings["ConnectionString"];
                strSqlConnection = Config.ConnectionString;

            return (new SqlConnection(strSqlConnection));
        }
    }
}
