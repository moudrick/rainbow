using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using Rainbow.Framework.Configuration.Cache;
using System.Diagnostics;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Data.MsSql.Debugger;

namespace Rainbow.Framework.Data.MsSql
{
    partial class Module : IComparable<Module>
    {
        const string strAdmin = "Admin;";
        const string strNoModule = "No Module";

        /// <summary>
        /// Called when [created].
        /// </summary>
        partial void OnCreated()
        {
            ModuleId = 0;
            PaneName = "no pane";
            Title = string.Empty;
            AuthorizedRolesEdit = strAdmin;
            AuthorizedRolesView = "All Users;";
            AuthorizedRolesAdd = strAdmin;
            AuthorizedRolesDelete = strAdmin;
            AuthorizedRolesProperties = strAdmin;
            AuthorizedRolesModuleMove = strAdmin;
            AuthorizedRolesModuleDelete = strAdmin;
            CacheTime = 0;
            Order = 0;
            IsShowMobile = false;
            IsCollapsable = false;
            IsSupportWorkflow = false;
        }

        /// <summary>
        /// The GetModuleSettings Method returns a List&lt;ModuleSetting&gt; of
        /// custom module specific settings from the database.  This method is
        /// used by some user control modules to access misc settings.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="_baseSettings">The _base settings.</param>
        /// <returns></returns>
        public static List<ModuleSetting> GetModuleSettings(int moduleId, List<ModuleSetting> _baseSettings)
        {

            if (!CurrentCache.Exists(Key.ModuleSettings(moduleId)))
            {
                // Get Settings for this module from the database
                try
                {
                    DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
                    db.Log = new DebuggerWriter();

                    var q = from bs in _baseSettings
                            join qms in db.ModuleSettings.Where(ms => ms.ModuleId == moduleId) on bs.SettingName equals qms.SettingName
                            select bs;


                    foreach (var s in q)
                    {
                        if (string.IsNullOrEmpty(s.SettingValue))
                        {
                            var q2 = q.Where(ms => ms.SettingName == s.SettingName).Single();
                            s.SettingValue = q2.SettingValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log error
                    Debug.WriteLine("GetModuleSettings: " + ex.Message);
                    throw;
                }

                CurrentCache.Insert(Key.ModuleSettings(moduleId), _baseSettings);
            }
            else
            {
                _baseSettings = (List<ModuleSetting>)CurrentCache.Get(Key.ModuleSettings(moduleId));
            }
            return _baseSettings;
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
        public static string GetModuleDesktopSource(int moduleId)
        {
            string ControlPath = string.Empty;
            try
            {
                DataClassesDataContext db = new DataClassesDataContext();
                db.Log = new DebuggerWriter();

                var q = db.Modules.Single(m => m.ModuleId == moduleId).ModuleDefinition.GeneralModuleDefinition.DesktopSource;

                ControlPath = Path.WebPathCombine(Path.ApplicationRoot, q);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                //log error
                Debug.WriteLine("GetModuleDesktopSrc: " + ex.Message);
                throw;
            }
            return ControlPath;
        }

        /// <summary>
        /// The DeleteModule method deletes a specified Module from the Modules database table.<br/>
        /// DeleteModule Stored Procedure
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        [History("JB - john@bowenweb.com", "2005/05/12", "Added support for Recycler module")]
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public static void DeleteModule(int moduleID)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new Rainbow.Framework.Data.MsSql.Debugger.DebuggerWriter();

            //BOWEN 11 June 2005 - BEGIN
            //Rainbow.Framework.Data.MsSql.Portal portal = (Rainbow.Framework.Data.MsSql.Portal)HttpContext.Current.Items["PortalSettings"];
            //wforney: removed above to enable deleting a module from any portal by referencing module id
            Rainbow.Framework.Data.MsSql.Portal portal = db.Modules.Single(m => m.ModuleId == moduleID).Page.Portal;

            bool useRecycler =
                bool.Parse(portal.PortalSettings.SingleOrDefault(p => p.SettingName == "SITESETTINGS_USE_RECYCLER").SettingValue);

            // TODO: THIS LINE DISABLES THE RECYCLER DUE SOME TROUBLES WITH IT !!!!!! Fix those troubles and then uncomment.
            useRecycler = false;

            if (useRecycler)
            {
                //TODO: Remove dependency on mail helper or put mail helper in a project that is not dependent on this one.

                //db.rb_DeleteModuleToRecycler((int?)moduleID, MailHelper.GetCurrentUserEmailAddress(), (DateTime?)DateTime.Now);
            }
            else
            {
                db.Modules.DeleteOnSubmit(db.Modules.Single(m => m.ModuleId == moduleID));
                db.SubmitChanges();
            }
        }

        #region IComparable<Module> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Module other)
        {
            int compareOrder = other.Order;

            if (Order == compareOrder) return 0;
            if (Order < compareOrder) return -1;
            if (Order > compareOrder) return 1;
            return 0;
        }

        #endregion

        /// <summary>
        /// The GetModuleDefinitionByGUID method returns the id of the Module
        /// that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public static int GetModuleDefinitionByGuid(int portalID, Guid guid)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new Rainbow.Framework.Data.MsSql.Debugger.DebuggerWriter();

            //SELECT
            //    @ModuleID =
            //(
            //    SELECT     rb_ModuleDefinitions.ModuleDefID
            //    FROM         rb_GeneralModuleDefinitions LEFT OUTER JOIN
            //                          rb_ModuleDefinitions ON rb_GeneralModuleDefinitions.GeneralModDefID = rb_ModuleDefinitions.GeneralModDefID
            //    WHERE     (rb_ModuleDefinitions.PortalID = @PortalID) AND (rb_ModuleDefinitions.GeneralModDefID = @Guid)
            //)
            var q = from md in db.ModuleDefinitions
                    where md.PortalId == portalID && md.GeneralModDefId == guid
                    join gmd in db.GeneralModuleDefinitions on md.GeneralModDefId equals gmd.GeneralModDefId into gmdt
                    from x in gmdt.DefaultIfEmpty()
                    select md.ModuleDefId;

            return q.Single();

            //int? moduleId = null;
            //db.rb_GetModuleDefinitionByGuid((int?)portalID, (Guid?)guid, ref moduleId);

            //return moduleId.Value;
        }

        /// <summary>
        /// The GetModuleDefinitionByName method returns the id of the Module
        /// that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns></returns>
        public static int GetModuleDefinitionByName(int portalID, string moduleName)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new Rainbow.Framework.Data.MsSql.Debugger.DebuggerWriter();

            return db.ModuleDefinitions.Single(md => md.PortalId == portalID &&
                md.GeneralModuleDefinition.FriendlyName == moduleName).ModuleDefId;
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
        /// <param name="PropertiesRoles">The properties roles.</param>
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
        [History("bill@improvtech.com", "2007/12/16", "Updated for LINQ")]
        [Obsolete("Just make a new Module object and add it to the db with linq")]
        public static int AddModule(int pageID, int moduleOrder, string paneName, string title, int moduleDefID, int cacheTime,
                             string editRoles, string viewRoles, string addRoles, string deleteRoles,
                             string PropertiesRoles,
                             string moveModuleRoles, string deleteModuleRoles, bool showMobile, string publishingRoles,
                             bool supportWorkflow, bool showEveryWhere, bool supportCollapsable)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new Rainbow.Framework.Data.MsSql.Debugger.DebuggerWriter();

            int? moduleID = null;

            db.rb_AddModule((int?)pageID, (int?)moduleOrder, title, paneName, (int?)moduleDefID, (int?)cacheTime, editRoles, addRoles, viewRoles, deleteRoles,
                PropertiesRoles, moveModuleRoles, deleteModuleRoles, (bool?)showMobile, publishingRoles, (bool?)supportWorkflow, (bool?)showEveryWhere,
                (bool?)supportCollapsable, ref moduleID);

            return (int)moduleID;
        }

        /// <summary>
        /// The GetModuleInUse method returns a list of modules in use with this portal<br/>
        /// GetModuleInUse Stored Procedure
        /// </summary>
        /// <param name="defID">The def ID.</param>
        /// <returns></returns>
        public static IEnumerable<GetModuleInUseResult> GetModuleInUse(Guid defID)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new Rainbow.Framework.Data.MsSql.Debugger.DebuggerWriter();

            var inUse = from md in db.ModuleDefinitions
                        where md.GeneralModDefId == defID
                        select new GetModuleInUseResult
                        {
                            PortalID = md.PortalId,
                            PortalAlias = md.Portal.PortalAlias,
                            PortalName = md.Portal.PortalName,
                            Checked = '1'
                        };
            var notInUse = from p in db.Portals
                           where !(from md in db.ModuleDefinitions
                                   where md.GeneralModDefId == defID
                                   select md.Portal).Contains(p)
                           select new GetModuleInUseResult
                           {
                               PortalID = p.PortalId,
                               PortalAlias = p.PortalAlias,
                               PortalName = p.PortalName,
                               Checked = '0'
                           };

            var miu = inUse.Union(notInUse);

            return miu;
        }

        /// <summary>
        /// Get Modules in All Portals
        /// </summary>
        /// <returns>List&lt;ModuleItem&gt;</returns>
        public static List<Module> GetModulesAllPortals()
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            List<Module> modules = new List<Module>();

            modules.Add(new Module() { ModuleId = 0, Title = General.GetString(strNoModule), Order = -1 });

            var q = (from m in modules select m).Union(
                from m in db.Modules.Where(m => m.ModuleId > 0 &&
                    !m.ModuleDefinition.GeneralModuleDefinition.IsAdmin.Value &&
                    m.ModuleDefinition.GeneralModuleDefinition.GeneralModDefId != new Guid("F9F9C3A4-6E16-43b4-B540-984DDB5F1CD2") &&
                    m.ModuleDefinition.GeneralModuleDefinition.GeneralModDefId != new Guid("F9F9C3A4-6E16-43b4-B540-984DDB5F1CD0"))
                orderby m.Page.Portal.PortalAlias, m.Title
                select new Module()
                {
                    ModuleId = m.ModuleId,
                    Title = m.Page.Portal.PortalAlias + "/" + m.Page.PageName + "/" + m.Title + " (" +
                        m.ModuleDefinition.GeneralModuleDefinition.FriendlyName + ")",
                    //PortalAlias = portal.PortalAlias,
                    Order = m.Page.PageOrder,
                    PaneName = m.PaneName,
                    ModuleDefId = m.ModuleDefId
                });

            return q.ToList();
        }

        /// <summary>
        /// The GetModuleByName method returns a list of all module with
        /// the specified Name (Type) within the Portal.
        /// It is used to get all instances of a specified module used in a Portal.
        /// e.g. All Image Gallery
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetModuleByName Stored Procedure</remarks>
        public static IEnumerable<Module> GetModulesByName(string moduleName, int portalID)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            //TODO: Replace this type of thing with a single binding list function.
            List<Module> modules = new List<Module>();

            modules.Add(new Module() { ModuleId = 0, Title = General.GetString(strNoModule), Order = -1 });

            var mods = (from m in modules select m).Union(
                db.Modules.Where(m => m.ModuleDefinition.PortalId == portalID &&
                m.ModuleDefinition.GeneralModuleDefinition.FriendlyName == moduleName).OrderBy(m => m.Title));

            return mods;
        }

        /// <summary>
        /// GetModulesSinglePortal
        /// </summary>
        /// <param name="PortalID">The portal ID.</param>
        /// <returns></returns>
        public static IEnumerable<Module> GetModulesSinglePortal(int PortalID)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            List<Module> modules = new List<Module>();

            modules.Add(new Module() { ModuleId = 0, Title = General.GetString(strNoModule), Order = -1 });

            //TODO: Why are we changing the title here? If possible get rid of that.

            var mods = (from m in modules select m).Union(
                from mod in db.Modules.Where(m => m.ModuleDefinition.GeneralModDefId != new Guid("F9F9C3A4-6E16-43b4-B540-984DDB5F1CD2") &&
                    m.ModuleDefinition.GeneralModDefId != new Guid("F9F9C3A4-6E16-43b4-B540-984DDB5F1CD0") &&
                    m.Page.PortalId == PortalID)
                orderby mod.Page.PageOrder, mod.Title
                select new Module()
                {
                    ModuleId = mod.ModuleId,
                    Title = mod.Page.PageName + "/" + mod.Title + " (" +
                        mod.ModuleDefinition.GeneralModuleDefinition.FriendlyName + ")",
                    //PortalAlias = portal.PortalAlias,
                    Order = mod.Page.PageOrder,
                    PaneName = mod.PaneName,
                    ModuleDefId = mod.ModuleDefId
                });

            return mods;
        }

        /// <summary>
        /// The UpdateModuleDefinitions method updates
        /// all module definitions in every portal
        /// </summary>
        /// <param name="GeneralModDefID">The general mod def ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="ischecked">if set to <c>true</c> [ischecked].</param>
        public static void UpdateModuleDefinitions(Guid GeneralModDefID, int portalID, bool ischecked)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            try
            {
                db.rb_UpdateModuleDefinitions((Guid?)GeneralModDefID, (int?)portalID, (bool?)ischecked);
            }
            catch (Exception ex)
            {
                //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                //ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                Debug.WriteLine("An Error Occurred in UpdateModuleDefinitions: " + ex.Message);
            }
        }

        /// <summary>
        /// The UpdateModuleOrder method update Modules Order.<br/>
        /// UpdateModuleOrder Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="ModuleOrder">The module order.</param>
        /// <param name="pane">The pane.</param>
        [Obsolete("Just grab Module object and update instead")]
        public static void UpdateModuleOrder(int ModuleID, int ModuleOrder, string pane)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            var q = db.Modules.Single(m => m.ModuleId == ModuleID);

            q.Order = ModuleOrder;
            q.PaneName = pane;

            db.SubmitChanges();
        }
    }
}
