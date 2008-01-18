using System;
using System.Collections;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Caching;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Data;
using Rainbow.Framework.DataTypes;
using Rainbow.Framework.Design;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Scheduler;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Site.Data;
using Rainbow.Framework.Users.Data;
using Path=Rainbow.Framework.Path;

namespace Rainbow.Framework.Providers.MsSql
{
    ///<summary>
    /// MsSql implementation for PortalProvider
    ///</summary>
    public class MsSqlPortalProvider : PortalProvider
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
        const string strName = "Name";

        IScheduler scheduler; // Federico (ifof@libero.it) 18 jun 2003
        CultureInfo[] rainbowCultures = null;

        /// <summary>
        /// Gets or sets the scheduler.
        /// </summary>
        /// <value>The scheduler.</value>
        IScheduler Scheduler
        {
            get { return scheduler; }
            set { scheduler = value; }
        }

        /// <summary>
        /// Gets the rainbow cultures.
        /// </summary>
        /// <value>The rainbow cultures.</value>
        CultureInfo[] RainbowCultures
        {
            get
            {
                if (rainbowCultures == null)
                {
                    string baseDir = Path.ApplicationPhysicalPath + "bin";
                    string[] dirs = Directory.GetDirectories(baseDir);
                    char[] separators = { '\\', '/' };

                    ArrayList rainbowCulturesArray = new ArrayList();
                    foreach (string str in dirs)
                    {
                        if ((Directory.GetFiles(str, "Rainbow.resources.dll")).Length == 1)
                        {
                            string lang = str.Substring(str.LastIndexOfAny(separators) + 1);
                            rainbowCulturesArray.Add(new CultureInfo(lang));
                        }
                    }
                    rainbowCultures = new CultureInfo[rainbowCulturesArray.Count];
                    rainbowCulturesArray.CopyTo(rainbowCultures);
                }
                return rainbowCultures;
            }
        }

        /// <summary>
        /// Gets portal custom settings from persistence
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public override Hashtable GetPortalCustomSettings(int portalID)
        {
            Hashtable settings = new Hashtable();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        public override void FillPortalSettingsBrief(Portal settings, int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
                            SetPageID(settings.ActivePage, 0);
                            SetPortalPath(settings.ActivePage, settings.PortalPath);
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
        protected override void UpdatePortalSettingSpecific(int portalID, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        static string GetLanguageList(string portalAlias)
        {
            string langlist;
            if (CurrentCache.Exists(Key.LanguageList()))
            {
                langlist = (string)CurrentCache.Get(Key.LanguageList());
            }
            else
            {
                langlist = GetLanguageListInternal(portalAlias);
                if (langlist.Length == 0)
                {
                    //jes1111 - langlist = ConfigurationSettings.AppSettings["DefaultLanguage"]; //default
                    langlist = Config.DefaultLanguage; //default
                }
                CurrentCache.Insert(Key.LanguageList(), langlist);
            }
            return langlist;
        }

        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns></returns>
        static string GetLanguageListInternal(string portalAlias)
        {
            string langlist = string.Empty;
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        public override Portal InstantiateNewPortal(int pageID, string portalAlias)
        {
            // Changes culture/language according to settings
            try
            {
                //Moved here for support db call
                Rainbow.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(GetLanguageList(portalAlias), portalAlias);
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Warn, "Failed to load languages, loading defaults.", ex); // Jes1111
                Rainbow.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(Localization.LanguageSwitcher.LANGUAGE_DEFAULT, portalAlias);
            }

            Portal portal = GetNew();

            try
            {
                FillPortalSettingsFull(portal, pageID, portalAlias);
            }
            catch (ProviderException ex)
            {
                Uri requestUri = HttpContext.Current.Request.Url;
                string databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
                if (databaseUpdateRedirect.StartsWith("~/"))
                {
                    databaseUpdateRedirect = databaseUpdateRedirect.TrimStart(new char[] { '~' });
                }
                if (!requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture).EndsWith(databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                {
                    throw new DatabaseUnreachableException("This may be a new db", ex.InnerException);
                }
                else
                {
                    ErrorHandler.Publish(LogLevel.Warn, "This may be a new db"); // Jes1111
                }
                return portal;
            }

            //Provide a valid tab id if it is missing
            if (portal.ActivePage.PageID == 0)
            {
                SetPageID(portal.ActivePage, ((PageStripDetails)portal.DesktopPages[0]).PageID);
            }
            //Go to get custom settings
            SetCustomSettings(portal, 
                              GetPortalCustomSettings(portal.PortalID, GetPortalBaseSettings(portal.PortalPath)));
             
            
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(portal.PortalPath);
            //Default
            themeManager.Load(portal.CustomSettings["SITESETTINGS_THEME"].ToString());
            portal.CurrentThemeDefault = themeManager.CurrentTheme;

            //Alternate
            if (portal.CustomSettings["SITESETTINGS_ALT_THEME"].ToString() == portal.CurrentThemeDefault.Name)
            {
                portal.CurrentThemeAlt = portal.CurrentThemeDefault;
            }
            else
            {
                themeManager.Load(portal.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
                portal.CurrentThemeAlt = themeManager.CurrentTheme;
            }
            //themeManager.Save(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            //Set layout
            portal.CurrentLayout = portal.CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();

            // Jes1111
            // Generate DesktopPagesXml
            //jes1111 - if (bool.Parse(ConfigurationSettings.AppSettings["PortalSettingDesktopPagesXml"]))
            //if (Config.PortalSettingDesktopPagesXml)
            //	this.DesktopPagesXml = GetDesktopPagesXml();

            return portal;
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
        public override Portal InstantiateNewPortal(int portalID)
        {
            Portal portal = GetNew();
            try
            {
                FillPortalSettingsBrief(portal, portalID);
            }
            catch (ProviderException)
            {
                throw new Exception("The portal you requested cannot be found. PortalID: " + portalID,
                                    new HttpException(404, "Portal not found"));
            }

            //Go to get custom settings
            SetCustomSettings(portal, 
                              GetPortalCustomSettings(portalID, GetPortalBaseSettings(portal.PortalPath)));
            portal.CurrentLayout = portal.CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(portal.PortalPath);
            //Default
            themeManager.Load(portal.CustomSettings["SITESETTINGS_THEME"].ToString());
            portal.CurrentThemeDefault = themeManager.CurrentTheme;
            //Alternate
            themeManager.Load(portal.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
            portal.CurrentThemeAlt = themeManager.CurrentTheme;

            return portal;
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
        public override int CreatePortal(int solutionID, string portalAlias, string portalName, string portalPath)
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

            // Create a new Page "home"
            int homePageID = PortalPageProvider.Instance.AddPage(portalID, "Home", 1);
            // Create a new Page "admin"
            string localizedString = General.GetString("ADMIN_TAB_NAME");
            int adminPageID = PortalPageProvider.Instance.AddPage(portalID, localizedString, strAdmins, 9999);
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
        public override int CreatePortal(int templateID,
                                         string templateAlias,
                                         string portalAlias,
                                         string portalName,
                                         string portalPath)
        {
            int newPortalID;
            ModulesDB modules = new ModulesDB();

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
                module.id = (int) myReader["ModuleDefID"];
                module.GuidID = GetGeneralModuleDefinitionByName(myReader["FriendlyName"].ToString(), my2ndConnection);
                try
                {
                    // save module definitions in the new portal
                    modules.UpdateModuleDefinitions(module.GuidID, newPortalID, true);
                    // Save the modules into a list for finding them later
                    templateModules.Add(module);
                }
                catch {;} // tried to add a Module thas doesn´t exists in this implementation of the portal
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
                tab.oldID = (int) myReader["PageID"];
                tab.newID =
                    PortalPageProvider.Instance.AddPage(newPortalID,
                                                        myReader["PageName"].ToString(),
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
                    tab = (tabTemplate) myEnumerator.Current;
                    if (tab.oldID == (int) myReader["PageID"])
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
                PortalPageProvider.Instance.UpdatePage(newPortalID,
                                                       newTabID,
                                                       newParentTabID,
                                                       myReader["PageName"].ToString(),
                                                       Int32.Parse(myReader["PageOrder"].ToString()),
                                                       myReader["AuthorizedRoles"].ToString(),
                                                       myReader["MobilePageName"].ToString(),
                                                       (bool) myReader["ShowMobile"]);

                // Finally use GetPortalSettings to access each Tab and its Modules in the Template Portal
                // and create them in the new Portal
                SqlDataReader result;

                try
                {
                    result = GetPageModules(Int32.Parse(myReader["PageID"].ToString()), my2ndConnection);

                    while (result.Read())
                    {
                        ModuleSettings m = new ModuleSettings();
                        m.ModuleID = (int) result["ModuleID"];
                        m.ModuleDefID = (int) result["ModuleDefID"];
                        m.PageID = newTabID;
                        m.PaneName = (string) result["PaneName"];
                        m.ModuleTitle = (string) result["ModuleTitle"];

                        object myValue = result["AuthorizedEditRoles"];
                        m.AuthorizedEditRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedViewRoles"];
                        m.AuthorizedViewRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedAddRoles"];
                        m.AuthorizedAddRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedDeleteRoles"];
                        m.AuthorizedDeleteRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedPropertiesRoles"];
                        m.AuthorizedPropertiesRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedMoveModuleRoles"];
                        m.AuthorizedMoveModuleRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedDeleteModuleRoles"];
                        m.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["AuthorizedPublishingRoles"];
                        m.AuthorizedPublishingRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["SupportWorkflow"];
                        m.SupportWorkflow = !Convert.IsDBNull(myValue) ? (bool) myValue : false;

                        myValue = result["AuthorizedApproveRoles"];
                        m.AuthorizedApproveRoles = !Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;

                        myValue = result["WorkflowState"];
                        m.WorkflowStatus = !Convert.IsDBNull(myValue)
                                               ? (WorkflowState) (0 + (byte) myValue)
                                               : WorkflowState.Original;

                        try
                        {
                            myValue = result["SupportCollapsable"];
                        }
                        catch
                        {
                            myValue = DBNull.Value;
                        }
                        m.SupportCollapsable = DBNull.Value != myValue ? (bool) myValue : false;

                        try
                        {
                            myValue = result["ShowEveryWhere"];
                        }
                        catch
                        {
                            myValue = DBNull.Value;
                        }
                        m.ShowEveryWhere = DBNull.Value != myValue ? (bool) myValue : false;

                        m.CacheTime = int.Parse(result["CacheTime"].ToString());
                        m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());

                        myValue = result["ShowMobile"];
                        m.ShowMobile = !Convert.IsDBNull(myValue) ? (bool) myValue : false;

                        // Find the new ModuleDefID assigned to the module in the new portal
                        myEnumerator = templateModules.GetEnumerator();
                        int newModuleDefID = 0;

                        while (myEnumerator.MoveNext() && newModuleDefID == 0)
                        {
                            module = (moduleTemplate) myEnumerator.Current;
                            if (module.id == m.ModuleDefID)
                            {
                                newModuleDefID = modules.GetModuleDefinitionByGuid(newPortalID, module.GuidID);
                            }
                        }

                        if (newModuleDefID > 0)
                        {
                            // add the module to the new tab
                            int newModuleID = modules.AddModule(newTabID,
                                                                m.ModuleOrder,
                                                                m.PaneName,
                                                                m.ModuleTitle,
                                                                newModuleDefID,
                                                                m.CacheTime,
                                                                m.AuthorizedEditRoles,
                                                                m.AuthorizedViewRoles,
                                                                m.AuthorizedAddRoles,
                                                                m.AuthorizedDeleteRoles,
                                                                m.AuthorizedPropertiesRoles,
                                                                m.AuthorizedMoveModuleRoles,
                                                                m.AuthorizedDeleteModuleRoles,
                                                                m.ShowMobile,
                                                                m.AuthorizedPublishingRoles,
                                                                m.SupportWorkflow,
                                                                m.ShowEveryWhere,
                                                                m.SupportCollapsable);
                            // At the end, get all ModuleSettings and save them in the new module
                            SqlDataReader dr = GetModuleSettings(m.ModuleID, my3rdConnection);

                            while (dr.Read())
                            {
                                ModuleSettingsProvider.UpdateModuleSetting(newModuleID,
                                                                           dr["SettingName"].ToString(),
                                                                           dr["SettingValue"].ToString());
                            }
                            dr.Close();
                        }
                    }

                    result.Close();
                }
                catch {;} // Error? ignore Tab ...
            }
            myReader.Close();

            // Set the CustomSettings of the New Portal based in the Template Portal
            myReader = GetPortalCustomSettings(templateID, myConnection);

            // Always call Read before accessing data.
            while (myReader.Read())
            {
                UpdatePortalSetting(newPortalID,
                                    myReader["SettingName"].ToString(),
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
            CreatePortalPath(portalPath);

            return newPortalID;
        }

        /// <summary>
        /// Gets the portals.
        /// </summary>
        /// <returns></returns>
        public override DataSet GetPortalsDataSet()
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
        public override void DeletePortal(int portalID)
        {
            try
            {
                // Create Instance of Connection and Command Object
                using (SqlConnection myConnection = DBHelper.SqlConnection)
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
                ErrorHandler.Publish(LogLevel.Error, aux, sqlex);
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
        public override void UpdatePortalInfo(int portalID, string portalName, string portalPath, bool alwaysShow)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        public override ArrayList GetPortals()
        {
            ArrayList list = new ArrayList();

            SqlDataReader dr = GetPortalsSqlDataReader();
            try
            {
                while (dr.Read())
                {
                    PortalItem portalItem = new PortalItem();
                    portalItem.Name = dr["PortalName"].ToString();
                    portalItem.Path = dr["PortalPath"].ToString();
                    portalItem.ID = Convert.ToInt32(dr["PortalID"].ToString());
                    list.Add(portalItem);
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
        public override ArrayList GetPortalsArrayList()
        {
            ArrayList portals = new ArrayList();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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

        /// <summary>
        /// Gets the portal base settings.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        public override Hashtable GetPortalBaseSettings(string portalPath)
        {
            Hashtable baseSettings;

            if (!CurrentCache.Exists(Key.PortalBaseSettings()))
            {
                // fix: Jes1111 - 27-02-2005 - for proper operation of caching
                LayoutManager layoutManager = new LayoutManager(portalPath);
                ArrayList layoutList = layoutManager.GetLayouts();
                ThemeManager themeManager = new ThemeManager(portalPath);
                ArrayList themeList = themeManager.GetThemes();

                //Define base settings
                baseSettings = new Hashtable();
                int groupOrderBase;
                SettingItemGroup group;

                #region Theme Management

                group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
                groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;

                SettingItem image = new SettingItem(new UploadedFileDataType(Path.WebPathCombine(Path.ApplicationRoot, portalPath))); //StringDataType
                image.Order = groupOrderBase + 5;
                image.Group = group;
                image.EnglishName = "Logo";
                image.Description = "Enter the name of logo file here. The logo will be searched in your portal dir. For the default portal is (~/_Rainbow).";
                baseSettings.Add("SITESETTINGS_LOGO", image);

                //ArrayList layoutList = new LayoutManager(portalPath).GetLayouts();
                SettingItem tabLayoutSetting = new SettingItem(new CustomListDataType(layoutList, strName, strName));
                tabLayoutSetting.Value = "Default";
                tabLayoutSetting.Order = groupOrderBase + 10;
                tabLayoutSetting.Group = group;
                tabLayoutSetting.EnglishName = "Page layout";
                tabLayoutSetting.Description = "Specify the site level page layout here.";
                baseSettings.Add("SITESETTINGS_PAGE_LAYOUT", tabLayoutSetting);

                //ArrayList themeList = new ThemeManager(portalPath).GetThemes();
                SettingItem theme = new SettingItem(new CustomListDataType(themeList, strName, strName));
                theme.Required = true;
                theme.Order = groupOrderBase + 15;
                theme.Group = group;
                theme.EnglishName = "Theme";
                theme.Description = "Specify the site level theme here.";
                baseSettings.Add("SITESETTINGS_THEME", theme);

                //SettingItem ThemeAlt = new SettingItem(new CustomListDataType(new ThemeManager(portalPath).GetThemes(), strName, strName));
                SettingItem themeAlt = new SettingItem(new CustomListDataType(themeList, strName, strName));
                themeAlt.Required = true;
                themeAlt.Order = groupOrderBase + 20;
                themeAlt.Group = group;
                themeAlt.EnglishName = "Alternate theme";
                themeAlt.Description = "Specify the site level alternate theme here.";
                baseSettings.Add("SITESETTINGS_ALT_THEME", themeAlt);

                // Jes1111 - 2004-08-06 - Zen support
                SettingItem allowModuleCustomThemes = new SettingItem(new BooleanDataType());
                allowModuleCustomThemes.Order = groupOrderBase + 25;
                allowModuleCustomThemes.Group = group;
                allowModuleCustomThemes.Value = "False";
                allowModuleCustomThemes.EnglishName = "Allow Module Custom Themes?";
                allowModuleCustomThemes.Description = "Select to allow Custom Theme to be set on Modules.";
                baseSettings.Add("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES", allowModuleCustomThemes);

                #endregion

                #region Security/User Management

                groupOrderBase = (int)SettingItemGroup.SECURITY_USER_SETTINGS;
                group = SettingItemGroup.SECURITY_USER_SETTINGS;
                // Show input for Portal Admins when using Windows Authenication and Multiportal
                // cisakson@yahoo.com 28.April.2003
                // This setting is removed in Global.asa for non-Windows authenticaton sites.
                SettingItem portalAdmins = new SettingItem(new StringDataType());
                portalAdmins.Order = groupOrderBase + 5;
                portalAdmins.Group = group;
                //jes1111 - PortalAdmins.Value = ConfigurationSettings.AppSettings["ADAdministratorGroup"];
                portalAdmins.Value = Config.ADAdministratorGroup;
                portalAdmins.Required = false;
                portalAdmins.Description = "Show input for Portal Admins when using Windows Authenication and Multiportal";
                baseSettings.Add("WindowsAdmins", portalAdmins);
                // Allow new registrations?
                SettingItem allowNewRegistrations = new SettingItem(new BooleanDataType());
                allowNewRegistrations.Order = groupOrderBase + 10;
                allowNewRegistrations.Group = group;
                allowNewRegistrations.Value = "True";
                allowNewRegistrations.EnglishName = "Allow New Registrations?";
                allowNewRegistrations.Description = "Check this to allow users register themselves. Leave blank for register through User Manager only.";
                baseSettings.Add("SITESETTINGS_ALLOW_NEW_REGISTRATION", allowNewRegistrations);
                //MH: added dynamic load of registertypes depending on the  content in the DesktopModules/Register/ folder
                // Register
                Hashtable regPages = new Hashtable();

                foreach (string registerPage in Directory.GetFiles(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/DesktopModules/CoreModules/Register/"), "register*.ascx", SearchOption.AllDirectories))
                {
                    string registerPageDisplayName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1, registerPage.LastIndexOf(".") - registerPage.LastIndexOf("\\") - 1);
                    //string registerPageName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1);
                    string registerPageName = registerPage.Replace(Path.ApplicationPhysicalPath, "~/").Replace("\\", "/");
                    regPages.Add(registerPageDisplayName, registerPageName.ToLower());
                }
                // Register Layout Setting
                SettingItem regType = new SettingItem(new CustomListDataType(regPages, "Key", "Value"));
                regType.Required = true;
                regType.Value = "Register.ascx";
                regType.EnglishName = "Register Type";
                regType.Description = "Choose here how Register Page should look like.";
                regType.Order = groupOrderBase + 15;
                regType.Group = group;
                baseSettings.Add("SITESETTINGS_REGISTER_TYPE", regType);
                //MH:end
                // Register Layout Setting module id reference by manu
                SettingItem regModuleID = new SettingItem(new IntegerDataType());
                regModuleID.Value = "0";
                regModuleID.Required = true;
                regModuleID.Order = groupOrderBase + 16;
                regModuleID.Group = group;
                regModuleID.EnglishName = "Register Module ID";
                regModuleID.Description = "Some custom registration may require additional settings, type here the ID of the module from where we should load settings (0= not used). Usually this module is added in an hidden area.";
                baseSettings.Add("SITESETTINGS_REGISTER_MODULEID", regModuleID);
                // Send mail on new registration to
                SettingItem onRegisterSendTo = new SettingItem(new StringDataType());
                onRegisterSendTo.Value = string.Empty;
                onRegisterSendTo.Required = false;
                onRegisterSendTo.Order = groupOrderBase + 17;
                onRegisterSendTo.Group = group;
                onRegisterSendTo.EnglishName = "Send Mail To";
                onRegisterSendTo.Description = "On new registration a mail will be send to the email address you provide here.";
                baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_TO", onRegisterSendTo);

                // Send mail on new registration to User from
                SettingItem onRegisterSendFrom = new SettingItem(new StringDataType());
                onRegisterSendFrom.Value = string.Empty;
                onRegisterSendFrom.Required = false;
                onRegisterSendFrom.Order = groupOrderBase + 18;
                onRegisterSendFrom.Group = group;
                onRegisterSendFrom.EnglishName = "Send Mail From";
                onRegisterSendFrom.Description = "On new registration a mail will be send to the new user from the email address you provide here.";
                baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_FROM", onRegisterSendFrom);

                //Terms of service
                Portal portal = Instance.CurrentPortal;
                PortalUrl portalUrl = portal != null ? portal.PortalUrl : new PortalUrl(string.Empty);
                SettingItem termsOfService = new SettingItem(portalUrl);
                termsOfService.Order = groupOrderBase + 20;
                termsOfService.Group = group;
                termsOfService.EnglishName = "Terms file name";
                termsOfService.Description = "Type here a file name used for showing terms and condition in each register page. Provide localized version adding _<culturename>. E.g. Terms.txt, will search for Terms.txt and for Terms_en-US.txt";
                baseSettings.Add("SITESETTINGS_TERMS_OF_SERVICE", termsOfService);

                // TODO: We need to bring back a country store of some sort? it should be in resources....
                /*
                 * 
				try
				{
					//Country filter limits country list, leave blank for all
					ArrayList countryList = new ArrayList(CountryInfo.GetCountries(CountryTypes.AllCountries, CountryFields.DisplayName));
					countryList.Insert(0, new CountryInfo());
					SettingItem CountriesFilter = new SettingItem(new MultiSelectListDataType(countryList, "DisplayName", strName));
					CountriesFilter.Order = _groupOrderBase + 25;
					CountriesFilter.Group = _Group;
					CountriesFilter.EnglishName = "Allowed countries";
					CountriesFilter.Description = "Allowed countries limits country list in RegisterFull page, select 'World' for no filter.";
					baseSettings.Add("SITESETTINGS_COUNTRY_FILTER", CountriesFilter);
				}

				catch (NullReferenceException ex)
				{
					//ErrorHandler.HandleException(ex);
					ErrorHandler.Publish(Rainbow.Framework.LogLevel.Error, "Failed to create 'CountriesFilter' in PortalSettings.", ex); // Jes1111
				}
                */
                #endregion

                #region HTML Header Management

                groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
                group = SettingItemGroup.META_SETTINGS;
                // added: Jes1111 - page DOCTYPE setting
                SettingItem DocType = new SettingItem(new StringDataType());

                DocType.Order = groupOrderBase + 5;

                DocType.Group = group;

                DocType.EnglishName = "DOCTYPE string";

                DocType.Description = "Allows you to enter a DOCTYPE string which will be inserted as the first line of the HTML output page (i.e. above the <html> element). Use this to force Quirks or Standards mode, particularly in IE. See <a href=\"http://gutfeldt.ch/matthias/articles/doctypeswitch/table.html\" target=\"_blank\">here</a> for details. NOTE: Rainbow.Zen requires a setting that guarantees Standards mode on all browsers.";

                DocType.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_DOCTYPE", DocType);
                //by John Mandia <john.mandia@whitelightsolutions.com>
                SettingItem TabTitle = new SettingItem(new StringDataType());
                TabTitle.Order = groupOrderBase + 10;
                TabTitle.Group = group;
                TabTitle.EnglishName = "Page title";
                TabTitle.Description = "Allows you to enter a default tab / page title (Shows at the top of your browser).";
                baseSettings.Add("SITESETTINGS_PAGE_TITLE", TabTitle);
                /*
				 * John Mandia: Removed This Setting. Now You can define specific Url Keywords via Tab Settings only. This is to speed up url building.
				 * 
				SettingItem TabUrlKeyword = new SettingItem(new StringDataType());
				TabUrlKeyword.Order = _groupOrderBase + 15;
				TabUrlKeyword.Group = _Group;
				TabUrlKeyword.Value = "Portal";
				TabUrlKeyword.EnglishName = "Keyword to Identify all pages";
				TabUrlKeyword.Description = "This setting is not fully implemented yet. It was to help with search engine optimisation by allowing you to specify a default keyword that would appear in your url."; 
				baseSettings.Add("SITESETTINGS_PAGE_URL_KEYWORD", TabUrlKeyword);
				*/
                SettingItem TabMetaKeyWords = new SettingItem(new StringDataType());
                TabMetaKeyWords.Order = groupOrderBase + 15;
                TabMetaKeyWords.Group = group;
                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want Meta Keywords; http://sourceforge.net/tracker/index.php?func=detail&aid=915614&group_id=66837&atid=515929
                TabMetaKeyWords.EnglishName = "Page keywords";
                TabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what your site is about.";
                baseSettings.Add("SITESETTINGS_PAGE_META_KEYWORDS", TabMetaKeyWords);
                SettingItem TabMetaDescription = new SettingItem(new StringDataType());
                TabMetaDescription.Order = groupOrderBase + 20;
                TabMetaDescription.Group = group;
                TabMetaDescription.EnglishName = "Page description";
                TabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a default description (Not too long though. 1 paragraph is enough) that describes your portal.";
                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want a defautl descripton
                baseSettings.Add("SITESETTINGS_PAGE_META_DESCRIPTION", TabMetaDescription);
                SettingItem TabMetaEncoding = new SettingItem(new StringDataType());
                TabMetaEncoding.Order = groupOrderBase + 25;
                TabMetaEncoding.Group = group;
                TabMetaEncoding.EnglishName = "Page encoding";
                TabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the default content type.";
                TabMetaEncoding.Value = "<META http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\" />";
                baseSettings.Add("SITESETTINGS_PAGE_META_ENCODING", TabMetaEncoding);
                SettingItem TabMetaOther = new SettingItem(new StringDataType());
                TabMetaOther.Order = groupOrderBase + 30;
                TabMetaOther.Group = group;
                TabMetaOther.EnglishName = "Default Additional Meta Tag Entries";
                TabMetaOther.Description = "This setting allows you to enter new tags into the Tab / Page's HEAD Tag. As an example we have added a portal tag to identify the version, but you could have a meta refresh tag or something else like a css reference instead.";
                TabMetaOther.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_PAGE_META_OTHERS", TabMetaOther);
                SettingItem TabKeyPhrase = new SettingItem(new StringDataType());
                TabKeyPhrase.Order = groupOrderBase + 35;
                TabKeyPhrase.Group = group;
                TabKeyPhrase.EnglishName = "Default Page Keyphrase";
                TabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a common message for the entire portal e.g. Welcome to x portal! This can be used for search engine optimisation. It allows you to define a keyword rich phrase to be used throughout your portal.";
                TabKeyPhrase.Value = "Enter your default keyword rich Tab / Page phrase here. ";
                baseSettings.Add("SITESETTINGS_PAGE_KEY_PHRASE", TabKeyPhrase);
                // added: Jes1111 - <body> element attributes setting
                SettingItem BodyAttributes = new SettingItem(new StringDataType());
                BodyAttributes.Order = groupOrderBase + 45;
                BodyAttributes.Group = group;
                BodyAttributes.EnglishName = "&lt;body&gt; attributes";
                BodyAttributes.Description = "Allows you to enter a string which will be inserted within the <body> element, e.g. leftmargin=\"0\" bottommargin=\"0\", etc. NOTE: not advisable to use this to inject onload() function calls as there is a programmatic function for that. NOTE also that is your CSS is well sorted you should not need anything here.";
                BodyAttributes.Required = false;
                baseSettings.Add("SITESETTINGS_BODYATTS", BodyAttributes);

                //end by John Mandia <john.mandia@whitelightsolutions.com>

                #endregion

                # region Language/Culture Management

                //groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
                group = SettingItemGroup.CULTURE_SETTINGS;

                SettingItem langList = new SettingItem(
                    new MultiSelectListDataType(RainbowCultures, "DisplayName", "Name"));
                langList.Group = group;
                langList.EnglishName = "Language list";
                //jes1111 - LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; 
                langList.Value = Config.DefaultLanguage;
                langList.Required = false;
                langList.Description = "This is a list of the languages that the site will support. You can select multiples languages by pressing shift in your keyboard";
                baseSettings.Add("SITESETTINGS_LANGLIST", langList);

                # endregion

                #region Miscellaneous Settings

                groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                group = SettingItemGroup.MISC_SETTINGS;
                // Show modified by summary on/off
                SettingItem ShowModifiedBy = new SettingItem(new BooleanDataType());
                ShowModifiedBy.Order = groupOrderBase + 10;
                ShowModifiedBy.Group = group;
                ShowModifiedBy.Value = "False";
                ShowModifiedBy.EnglishName = "Show modified by";
                ShowModifiedBy.Description = "Check to show by whom the module is last modified.";
                baseSettings.Add("SITESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy);
                // Default Editor Configuration used for new modules and workflow modules. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultEditor = new SettingItem(new HtmlEditorDataType());
                DefaultEditor.Order = groupOrderBase + 20;
                DefaultEditor.Group = group;
                DefaultEditor.Value = "FreeTextBox";
                DefaultEditor.EnglishName = "Default Editor";
                DefaultEditor.Description = "This Editor is used by workflow and is the default for new modules.";
                baseSettings.Add("SITESETTINGS_DEFAULT_EDITOR", DefaultEditor);
                // Default Editor Width. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultWidth = new SettingItem(new IntegerDataType());
                DefaultWidth.Order = groupOrderBase + 25;
                DefaultWidth.Group = group;
                DefaultWidth.Value = "700";
                DefaultWidth.EnglishName = "Editor Width";
                DefaultWidth.Description = "Default Editor Width";
                baseSettings.Add("SITESETTINGS_EDITOR_WIDTH", DefaultWidth);
                // Default Editor Height. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultHeight = new SettingItem(new IntegerDataType());
                DefaultHeight.Order = groupOrderBase + 30;
                DefaultHeight.Group = group;
                DefaultHeight.Value = "400";
                DefaultHeight.EnglishName = "Editor Height";
                DefaultHeight.Description = "Default Editor Height";
                baseSettings.Add("SITESETTINGS_EDITOR_HEIGHT", DefaultHeight);
                //Show Upload (Active up editor only). jviladiu@portalServices.net 13/07/2004
                SettingItem ShowUpload = new SettingItem(new BooleanDataType());
                ShowUpload.Value = "true";
                ShowUpload.Order = groupOrderBase + 35;
                ShowUpload.Group = group;
                ShowUpload.EnglishName = "Upload?";
                ShowUpload.Description = "Only used if Editor is ActiveUp HtmlTextBox";
                baseSettings.Add("SITESETTINGS_SHOWUPLOAD", ShowUpload);
                // Default Image Folder. jviladiu@portalServices.net 29/07/2004
                SettingItem DefaultImageFolder = new SettingItem(new FolderDataType(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + portalPath + "/images"), "default"));
                DefaultImageFolder.Order = groupOrderBase + 40;
                DefaultImageFolder.Group = group;
                DefaultImageFolder.Value = "default";
                DefaultImageFolder.EnglishName = "Default Image Folder";
                DefaultImageFolder.Description = "Set the default image folder used by Current Editor";
                baseSettings.Add("SITESETTINGS_DEFAULT_IMAGE_FOLDER", DefaultImageFolder);
                groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                group = SettingItemGroup.MISC_SETTINGS;
                // Show module arrows to an administrator
                SettingItem ShowModuleArrows = new SettingItem(new BooleanDataType());
                ShowModuleArrows.Order = groupOrderBase + 50;
                ShowModuleArrows.Group = group;
                ShowModuleArrows.Value = "True";
                ShowModuleArrows.EnglishName = "Show module arrows";
                ShowModuleArrows.Description = "Check to show the arrows in the module title to move modules.";
                baseSettings.Add("SITESETTINGS_SHOW_MODULE_ARROWS", ShowModuleArrows);

                //BOWEN 11 June 2005
                // Use Recycler Module for deleted modules
                SettingItem UseRecycler = new SettingItem(new BooleanDataType());
                UseRecycler.Order = groupOrderBase + 55;
                UseRecycler.Group = group;
                UseRecycler.Value = "True";
                UseRecycler.EnglishName = "Use Recycle Bin for Deleted Modules";
                UseRecycler.Description = "Check to make deleted modules go to the recycler instead of permanently deleting them.";
                baseSettings.Add("SITESETTINGS_USE_RECYCLER", UseRecycler);
                //BOWEN 11 June 2005

                #endregion

                // Fix: Jes1111 - 27-02-2005 - incorrect setting for cache dependency
                //CacheDependency settingDependancies = new CacheDependency(null, new string[]{Rainbow.Framework.Settings.Cache.Key.ThemeList(ThemeManager.Path)});
                // set up a cache dependency object which monitors the four folders we are interested in
                CacheDependency settingDependencies =
                    new CacheDependency(
                        new string[]
                            {
                                LayoutManager.Path,
                                layoutManager.PortalLayoutPath,
                                ThemeManager.Path,
                                themeManager.PortalThemePath
                            });

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalBaseSettings(), baseSettings, settingDependencies);
                }
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.PortalBaseSettings());
            }
            return baseSettings;
        }

        //        /// <summary>
        //        /// Get the ParentPageID of a certain Page 06/11/2004 Rob Siera
        //        /// </summary>
        //        /// <param name="pageID">The page ID.</param>
        //        /// <param name="tabList">The tab list.</param>
        //        /// <returns></returns>
        //        public int GetParentPageID(int pageID, ArrayList tabList)
        //        {
        //            PageStripDetails tmpPage;
        //
        //            for (int i = 0; i < tabList.Count; i++)
        //            {
        //                tmpPage = (PageStripDetails)tabList[i];
        //
        //                if (tmpPage.PageID == pageID)
        //                {
        //                    return tmpPage.ParentPageID;
        //                }
        //            }
        //            throw new ArgumentOutOfRangeException("pageID", "Root not found");
        //        }

        /// <summary>
        /// Get the proxy parameters as configured in web.config by Phillo 22/01/2003
        /// </summary>
        /// <returns></returns>
        public override WebProxy GetProxy()
        {
            //jes1111 - if(ConfigurationSettings.AppSettings["ProxyServer"].Length > 0) 
            //if(Config.ProxyServer.Length > 0) 
            //{ 
            WebProxy myProxy = new WebProxy();
            NetworkCredential myCredential = new NetworkCredential();
            //myCredential.Domain = ConfigurationSettings.AppSettings["ProxyDomain"]; 
            //myCredential.UserName = ConfigurationSettings.AppSettings["ProxyUserID"]; 
            //myCredential.Password = ConfigurationSettings.AppSettings["ProxyPassword"]; 
            myCredential.Domain = Config.ProxyDomain;
            myCredential.UserName = Config.ProxyUserID;
            myCredential.Password = Config.ProxyPassword;
            myProxy.Credentials = myCredential;
            //myProxy.Address = new Uri(ConfigurationSettings.AppSettings["ProxyServer"]); 
            myProxy.Address = new Uri(Config.ProxyServer);
            return (myProxy);
            //} 

            //else 
            //{ 
            //	return(null); 
            //} 
        }

        /// <summary>
        /// The PortalSettings.GetPortalSettings Method returns a hashtable of
        /// custom Portal specific settings from the database. This method is
        /// used by Portals to access misc settings.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="baseSettings">The _base settings.</param>
        /// <returns></returns>
        public override Hashtable GetPortalCustomSettings(int portalID, Hashtable baseSettings)
        {
            if (!CurrentCache.Exists(Key.PortalSettings()))
            {
                // Get Settings for this Portal from the database
                Hashtable settings = GetPortalCustomSettings(portalID);
                foreach (string key in baseSettings.Keys)
                {
                    if (settings[key] != null)
                    {
                        SettingItem s = ((SettingItem)baseSettings[key]);

                        if (settings[key].ToString().Length != 0)
                        {
                            s.Value = settings[key].ToString();
                        }
                    }
                }
                // Fix: Jes1111 - 27-02-2005 - change to make PortalSettings cache item dependent on PortalBaseSettings
                //Rainbow.Framework.Settings.Cache.CurrentCache.Insert(Rainbow.Framework.Settings.Cache.Key.PortalSettings(), baseSettings);
                CacheDependency settingDependencies =
                    new CacheDependency(
                        null,
                        new string[]
                            {
                                Key.PortalBaseSettings()
                            });

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalSettings(), baseSettings, settingDependencies);
                }
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.PortalSettings());
            }
            return baseSettings;
        }

        /// <summary>
        /// Flushes the base settings cache.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        public override void FlushBaseSettingsCache(string portalPath)
        {
            CurrentCache.Remove(Key.PortalBaseSettings());
        }

        /// <summary>
        /// The get tab root should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public override PageStripDetails GetRootPage(int parentPageID, ArrayList tabList)
        {
            //Changes Indah Fuldner 25.04.2003 (With assumtion that the rootlevel tab has ParentPageID = 0)
            //Search for the root tab in current array
            PageStripDetails rootPage;

            for (int i = 0; i < tabList.Count; i++)
            {
                rootPage = (PageStripDetails)tabList[i];

                // return rootPage;
                if (rootPage.PageID == parentPageID)
                {
                    parentPageID = rootPage.ParentPageID;
                    //string parentName=rootPage.PageName;

                    if (parentPageID != 0)
                        i = -1;

                    else
                        return rootPage;
                }
            }
            //End Indah Fuldner
            throw new ArgumentOutOfRangeException("parentPageID", "Root not found");
        }

        /// <summary>
        /// The GetRootPage should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public override PageStripDetails GetRootPage(PortalPage tab, ArrayList tabList)
        {
            return GetRootPage(tab.PageID, tabList);
        }

        public override void StartScheduler()
        {
            Scheduler = CachedScheduler.GetScheduler(
                RainbowContext.Current.HttpContext.Server.MapPath(Path.ApplicationRoot),
                DBHelper.SqlConnection,
                Config.SchedulerPeriod,
                Config.SchedulerCacheSize);
            Scheduler.Start();
        }

        static void FillPortalSettingsFull(Portal portal, int pageID, string portalAlias)
        {
            portal.CurrentLayout = "Default";
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
                    parameterPortalLanguage.Value = portal.PortalContentLanguage.Name;
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
                            tabDetails.PageLayout = portal.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            portal.PortalAlias = portalAlias;
                            // Update the AuthorizedRoles Variable
                            portal.DesktopPages.Add(tabDetails);
                        }

                        if (portal.DesktopPages.Count == 0)
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
                            tabDetails.PageLayout = portal.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            portal.MobilePages.Add(tabDetails);
                        }
                        // Read the third result --  Module Page Information
                        result.NextResult();

                        PortalPage activePortalPage = portal.ActivePage;

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
                            activePortalPage.Modules.Add(m);
                        }
                        // Now read Portal out params 
                        result.NextResult();
                        result.Close(); //by Manu, fixed bug 807858

                        portal.PortalID = (int)parameterPortalID.Value;
                        portal.PortalName = (string)parameterPortalName.Value;
                        //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
                        portal.PortalTitle = String.Concat(Config.PortalTitlePrefix, portal.PortalName);
                        //jes1111 - this.PortalPath = Settings.Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
                        portal.PortalPath = Path.WebPathCombine(Config.PortalsDirectory, (string)parameterPortalPath.Value);
                        //jes1111 - this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003
                        portal.PortalSecurePath = Config.PortalSecureDirectory;

                        //ActivePage initialization
                        PortalPageProvider.FillPortalPage(activePortalPage,
                                                          pageID,
                                                          portal.CurrentLayout,
                                                          Int32.Parse("0" + parameterParentPageID.Value),
                                                          (int)parameterPageOrder.Value,
                                                          (string)parameterMobilePageName.Value,
                                                          (string)parameterAuthRoles.Value,
                                                          (string)parameterPageName.Value,
                                                          (bool)parameterShowMobile.Value,
                                                          portal.PortalPath);
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
        static int AddPortal(string portalAlias, string portalName, string portalPath)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        static SqlDataReader GetTemplateModuleDefinitions(int templateID, SqlConnection myConnection)
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

        /// <summary>
        /// Gets the portal custom settings.
        /// </summary>
        /// <param name="templateID">The template ID.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        static SqlDataReader GetPortalCustomSettings(int templateID, SqlConnection myConnection)
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
        static Guid GetGeneralModuleDefinitionByName(string moduleName, SqlConnection myConnection)
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
        static SqlDataReader GetPageModules(int tabID, SqlConnection myConnection)
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
        static SqlDataReader GetModuleSettings(int moduleID, SqlConnection myConnection)
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
        static SqlDataReader GetTabsByPortal(int templateID, SqlConnection myConnection)
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
        static void CreatePortalPath(string portalPath)
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
            if (!Directory.Exists(portalPhisicalDir))
            {
                Directory.CreateDirectory(portalPhisicalDir);
            }
            // Subdirs
            string[] subdirs = { "images", "polls", "documents", "xml" };
            for (int i = 0; i <= subdirs.GetUpperBound(0); i++)
            {
                if (!Directory.Exists(portalPhisicalDir + "\\" + subdirs[i]))
                {
                    Directory.CreateDirectory(portalPhisicalDir + "\\" + subdirs[i]);
                }
            }
        }

        /// GetPortals Stored Procedure
        static SqlDataReader GetPortalsSqlDataReader()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
            //TODO: [moudrick] use isolated per-provider configured connection string
            if (Config.PortalTemplatesConnectionString.Length != 0)
            {
                //jes1111 - strSqlConnection = ConfigurationSettings.AppSettings[portalSqlConnectionID];
                strSqlConnection = Config.PortalTemplatesConnectionString;
            }
            else
            {
                //jes1111 - strSqlConnection = ConfigurationSettings.AppSettings["ConnectionString"];
                strSqlConnection = Config.ConnectionString;
            }

            return (new SqlConnection(strSqlConnection));
        }
    }
}
