using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.DataSources;
using Rainbow.Framework.Data.Entities;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Data.MsSql.Debugger;

namespace Rainbow.Framework.Data.MsSql.DataSources
{
    public class PortalDataSource : IPortalDataSource
    {
        DataClassesDataContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalDataSource"/> class.
        /// </summary>
        public PortalDataSource()
        {
            db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();
        }

        #region IPortalDataSource Members

        /// <summary>
        /// Adds the specified new portal.
        /// </summary>
        /// <param name="newPortal">The new portal.</param>
        /// <returns>The newly created IPortal object.</returns>
        public void Add(ref IPortal newPortal)
        {
            int i = Add(newPortal.PortalAlias, newPortal.PortalTitle, newPortal.PortalAlias);
            newPortal = db.Portals.Single(p => p.PortalId == i) as IPortal;
        }

        /// <summary>
        /// Adds the portal based on specified solution id.
        /// </summary>
        /// <param name="solutionId">The solution id.</param>
        /// <param name="newPortal">The new portal.</param>
        public void Add(int solutionId, ref IPortal newPortal)
        {
            int i = AddFromSolution(solutionId, newPortal.PortalAlias, newPortal.PortalTitle, newPortal.PortalAlias);
            newPortal = db.Portals.Single(p => p.PortalId == i) as IPortal;
        }

        /// <summary>
        /// Creates the new portal.
        /// </summary>
        /// <returns>IPortal</returns>
        public IPortal CreateNew()
        {
            return new Portal() as IPortal;
        }

        /// <summary>
        /// Gets the portal by id.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <returns>IPortal</returns>
        public IPortal GetById(int portalId)
        {
            var p = db.Portals.Single(pid => pid.PortalId == portalId);

            //grab layout
            p.CurrentLayout = p.PortalSettings.Single(cs => cs.SettingName == "SITESETTINGS_PAGE_LAYOUT").SettingValue;
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(p.PortalPathRelative);
            //Default
            themeManager.Load(p.PortalSettings.Single(cs => cs.SettingName == "SITESETTINGS_THEME").SettingValue);
            p.CurrentThemeDefault = themeManager.CurrentTheme;
            //Alternate
            themeManager.Load(p.PortalSettings.Single(cs => cs.SettingName == "SITESETTINGS_ALT_THEME").SettingValue);
            p.CurrentThemeAlternate = themeManager.CurrentTheme;

            return p as IPortal;
        }

        /// <summary>
        /// Gets all portals.
        /// </summary>
        /// <returns>IEnumerable&lt;IPortal&gt;</returns>
        public IEnumerable<IPortal> GetAll()
        {
            var q = from p in db.Portals select p;

            return q as IEnumerable<IPortal>;
        }

        /// <summary>
        /// Updates the specified portal.
        /// </summary>
        /// <param name="portal">IPortal</param>
        public void Update(IPortal portal)
        {
            Portal p = db.Portals.Single(pt => pt.PortalId == portal.PortalId);

            p.PortalTitle = portal.PortalTitle;

            //string pd = Config.PortalsDirectory;
            //if (portalPath.IndexOf(pd) > -1)
            //    portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);
            //p.PortalPath = portalPath;

            p.IsAlwaysShowEditButton = portal.IsAlwaysShowEditButton;
        }

        /// <summary>
        /// Removes the specified portal id.
        /// </summary>
        /// <param name="portal">IPortal</param>
        public void Remove(IPortal portal)
        {
            Remove(portal.PortalId);
        }

        public void CommitChanges()
        {
            db.SubmitChanges();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Adds the specified portal alias.
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns>The new portal ID.</returns>
        private int Add(string portalAlias, string portalName, string portalPath)
        {
            string pd = Config.PortalsDirectory;
            if (portalPath.IndexOf(pd) > -1)
                portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

            Portal p = new Portal();
            p.PortalAlias = portalAlias;
            p.PortalName = portalName;
            p.PortalPath = portalPath;
            p.IsAlwaysShowEditButton = false;

            db.Portals.InsertOnSubmit(p);

            return p.PortalId;
        }

        /// <summary>
        /// The CreatePortal method create a new basic portal based on solutions table.
        /// </summary>
        /// <param name="solutionID">The solution ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        [History("john.mandia@whitelightsolutions.com", "2003/05/26",
                 "Added extra info so that sign in is added to home tab of new portal and lang switcher is added to module list")]
        [History("bja@reedtek.com", "2003/05/16", "Added extra parameter for collpasable window")]
        private int AddFromSolution(int solutionID, string portalAlias, string portalName, string portalPath)
        {
            int portalID;
            // Create a new portal
            portalID = Add(portalAlias, portalName, portalPath);
            Portal p;

            p = db.Portals.Single(pt => pt.PortalId == portalID);

            var qmd = from s in db.Solutions
                      join sd in db.SolutionModuleDefinitions on s.SolutionsID equals sd.SolutionsID
                      join gmd in db.GeneralModuleDefinitions on sd.GeneralModDefID equals gmd.GeneralModDefId
                      where s.SolutionsID == solutionID
                      select gmd;

            foreach (var md in qmd)
                Module.UpdateModuleDefinitions(md.GeneralModDefId, portalID, true);

            if (!Config.UseSingleUserBase)
            {
                // Create the stradmin User for the new portal
                MembershipUser user = Membership.CreateUser(stradmin, stradmin, AdminEmail);
                // Create the "Admins" role for the new portal
                Roles.CreateRole("Admins");
                Roles.AddUserToRole(stradmin, "Admins");
            }

            // Create a new Page "home"
            //int homePageID = 
            Page homePage = new Page() { PortalId = p.PortalId, PageName = "Home", PageOrder = 1 };
            p.Pages.Add(homePage);
            // Create a new Page "admin"
            string localizedString = General.GetString("ADMIN_TAB_NAME");
            Page adminPage = new Page() { PortalId = p.PortalId, PageName = localizedString, AuthorizedRoles = strAdmins, PageOrder = 9999 };
            //int adminPageID = 
            p.Pages.Add(adminPage);
            // Add Modules for portal use
            // Html Document
            Module.UpdateModuleDefinitions(new Guid(strGUIDHTMLDocument), p.PortalId, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDSiteSettings), p.PortalId, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDPages), portalID, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDSecurityRoles), portalID, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDManageUsers), portalID, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDModules), portalID, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDLogin), portalID, true);
            Module.UpdateModuleDefinitions(new Guid(strGUIDLanguageSwitcher), portalID, true);

            // Add Modules for portal administration
            // Site Settings (Admin)
            localizedString = General.GetString("MODULE_SITE_SETTINGS");
            Module.AddModule(adminPage.PageId, 1, strContentPane, localizedString,
                              Module.GetModuleDefinitionByGuid(p.PortalId, new Guid(strGUIDSiteSettings)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Pages (Admin)
            localizedString = General.GetString("MODULE_TABS");
            Module.AddModule(adminPage.PageId, 2, strContentPane, localizedString,
                              Module.GetModuleDefinitionByGuid(p.PortalId, new Guid(strGUIDPages)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Roles (Admin)
            localizedString = General.GetString("MODULE_SECURITY_ROLES");
            Module.AddModule(adminPage.PageId, 3, strContentPane, localizedString,
                              Module.GetModuleDefinitionByGuid(p.PortalId, new Guid(strGUIDSecurityRoles)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Manage Users (Admin)
            localizedString = General.GetString("MODULE_MANAGE_USERS");
            Module.AddModule(adminPage.PageId, 4, strContentPane, localizedString,
                              Module.GetModuleDefinitionByGuid(p.PortalId, new Guid(strGUIDManageUsers)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Module Definitions (Admin)
            localizedString = General.GetString("MODULE_MODULES");
            Module.AddModule(adminPage.PageId, 1, strRightPane, localizedString,
                              Module.GetModuleDefinitionByGuid(p.PortalId, new Guid(strGUIDModules)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // End Change Geert.Audenaert@Syntegra.Com
            // Change by john.mandia@whitelightsolutions.com
            // Add Signin Module and put it on the hometab
            // Signin
            localizedString = General.GetString("MODULE_LOGIN", "Login");
            Module.AddModule(homePage.PageId, -1, strLeftPane, localizedString,
                              Module.GetModuleDefinitionByGuid(p.PortalId, new Guid(strGUIDLogin)), 0, strAdmins,
                              "Unauthenticated Users;Admins;", strAdmins, strAdmins, strAdmins, strAdmins, strAdmins,
                              false, string.Empty, false, false, false);
            // Add language switcher to available modules
            // Language Switcher
            // End of change by john.mandia@whitelightsolutions.com
            // Create paths
            Portal.CreatePortalPath(p.PortalPath);
            return p.PortalId;
        }

        /// <summary>
        /// Removes portal from database. All tabs, modules and data will be removed.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        private void Remove(int portalId)
        {
            var q = db.Portals.Single(p => p.PortalId == portalId);

            db.Pages.DeleteAllOnSubmit(q.Pages);
            db.Portals.DeleteOnSubmit(q);
        }

        #endregion
    }
}
