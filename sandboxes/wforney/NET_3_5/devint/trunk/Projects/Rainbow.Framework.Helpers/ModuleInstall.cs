namespace Rainbow.Framework.Helpers
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    using Rainbow.Framework.Configuration;

    using Path = Rainbow.Framework.Configuration.Path;

    /// <summary>
    /// ModuleInstall incapsulates all the logic for install, 
    /// uninstall modules on portal
    /// </summary>
    [History("jminond", "2006/02/22", "corrected case where install group is null exception")]
    public class ModuleInstall
    {
        /// <summary>
        /// Installs the group.
        /// </summary>
        /// <param name="groupFileName">Name of the group file.</param>
        /// <param name="install">if set to <c>true</c> [install].</param>
        public static void InstallGroup(string groupFileName, bool install)
        {
            DataTable modules = GetInstallGroup(groupFileName);

            // In case Modules are null
            if (modules != null && (modules.Rows.Count > 0))
            {
                foreach (DataRow r in modules.Rows)
                {
                    string friendlyName = r["FriendlyName"].ToString();
                    string desktopSource = r["DesktopSource"].ToString();
                    string mobileSource = r["MobileSource"].ToString();

                    Install(friendlyName, desktopSource, mobileSource, install);
                }
            }
            else
            {
                Exception ex = new Exception("Tried to install 0 modules in groupFileName:" + groupFileName);
                //ErrorHandler.Publish(LogLevel.Warn, ex);
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Gets the install group.
        /// </summary>
        /// <param name="groupFileName">Name of the group file.</param>
        /// <returns></returns>
        private static DataTable GetInstallGroup(string groupFileName)
        {
            //Load the XML as dataset
            using (DataSet ds = new DataSet())
            {
                string installer = groupFileName;

                try
                {
                    ds.ReadXml(installer);
                }
                catch (Exception ex)
                {
                    //ErrorHandler.Publish(LogLevel.Error, "Exception installing module: " + installer, ex);
                    Debug.WriteLine(ex.Message);
                    return null;
                }

                return ds.Tables[0];
            }
        }

        /// <summary>
        /// Uninstalls the group.
        /// </summary>
        /// <param name="groupFileName">Name of the group file.</param>
        public static void UninstallGroup(string groupFileName)
        {
            DataTable modules = GetInstallGroup(groupFileName);

            foreach (DataRow r in modules.Rows)
            {
                // string friendlyName = r["FriendlyName"].ToString();
                string desktopSource = r["DesktopSource"].ToString();
                string mobileSource = r["MobileSource"].ToString();

                Uninstall(desktopSource, mobileSource);
            }
        }

        /// <summary>
        /// Installs the specified friendly name.
        /// </summary>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="desktopSource">The desktop source.</param>
        /// <param name="mobileSource">The mobile source.</param>
        public static void Install(string friendlyName, string desktopSource, string mobileSource)
        {
            Install(friendlyName, desktopSource, mobileSource, true);
        }

        /// <summary>
        /// Installs module
        /// </summary>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="desktopSource">The desktop source.</param>
        /// <param name="mobileSource">The mobile source.</param>
        /// <param name="install">if set to <c>true</c> [install].</param>
        public static void Install(string friendlyName, string desktopSource, string mobileSource, bool install)
        {
            Rainbow.Framework.Data.MsSql.DataClassesDataContext db = new Rainbow.Framework.Data.MsSql.DataClassesDataContext(Config.ConnectionString);

            ErrorHandler.Publish(LogLevels.Info,
                                 "Installing DesktopModule '" + friendlyName + "' from '" + desktopSource + "'");
            if (!string.IsNullOrEmpty(mobileSource))
                ErrorHandler.Publish(LogLevels.Info,
                                     "Installing MobileModule '" + friendlyName + "' from '" + mobileSource + "'");

            string controlFullPath = Path.ApplicationRoot + "/" + desktopSource;

            // Instantiate the module
            Page page = new Page();
            //http://sourceforge.net/tracker/index.php?func=detail&aid=738670&group_id=66837&atid=515929
            //Rainbow.Framework.Web.UI.Page page = new Rainbow.Framework.Web.UI.Page();

            Control myControl = page.LoadControl(controlFullPath);
            if (!(myControl is PortalModuleControl))
                throw new Exception("Module '" + myControl.GetType().FullName + "' is not a PortalModule Control");

            var portalModule = (PortalModuleControl)myControl;

            // Check mobile module
            if (mobileSource != null && mobileSource.Length != 0 && mobileSource.ToLower().EndsWith(".ascx"))
            {
                //TODO: Check mobile module
                //TODO: MobilePortalModuleControl mobileModule = (MobilePortalModuleControl) page.LoadControl(Rainbow.Framework.Settings.Path.ApplicationRoot + "/" + mobileSource);
                if (!File.Exists(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + mobileSource)))
                    throw new FileNotFoundException("Mobile Control not found");
            }

            // Get Module ID
            Guid defID = portalModule.GuidID;

            //Get Assembly name
            string assemblyName = portalModule.GetType().BaseType.Assembly.CodeBase;
            assemblyName = assemblyName.Substring(assemblyName.LastIndexOf('/') + 1); //Get name only

            // Get Module Class name
            string className = portalModule.GetType().BaseType.FullName;

            // Now we add the definition to module list 
            var modules = new ModulesDB();

            if (install)
            {
                //Install as new module

                //Call Install
                try
                {
                    ErrorHandler.Publish(LogLevels.Debug, "Installing '" + friendlyName + "' as new module.");
                    portalModule.Install(null);
                }
                catch (Exception ex)
                {
                    //Error occurred
                    portalModule.Rollback(null);
                    //Rethrow exception
                    throw new Exception("Exception occurred installing '" + portalModule.GuidID.ToString() + "'!", ex);
                }

                var rows = db.GeneralModuleDefinitions.Where(d => d.GeneralModDefId == defID);
                if (rows.Count() > 0)
                    ErrorHandler.Publish(LogLevels.Warn,
                        string.Format("AddGeneralModuleDefinitions: The definition you tried to add already exists. {0} updating...",
                            rows.Count()));

                Rainbow.Framework.Data.MsSql.GeneralModuleDefinition gmd = new Rainbow.Framework.Data.MsSql.GeneralModuleDefinition()
                {
                    IsAdmin = portalModule.AdminModule,
                    AssemblyName = assemblyName,
                    ClassName = className,
                    DesktopSource = desktopSource,
                    FriendlyName = friendlyName,
                    GeneralModDefID = defID,
                    MobileSource = mobileSource,
                    IsSearchable = portalModule.Searchable
                };
                db.GeneralModuleDefinitions.InsertOnSubmit(gmd);
                db.SubmitChanges();

                // All is fine: we can call Commit
                portalModule.Commit(null);
            }
            else
            {
                // Update the general module definition
                try
                {
                    ErrorHandler.Publish(LogLevels.Debug, "Updating '" + friendlyName + "' as new module.");

                    var q = db.rb_GeneralModuleDefinitions.Where(gmd => gmd.GeneralModDefID == defID).Single();

                    q.GeneralModDefID = defID;
                    q.FriendlyName = friendlyName;
                    q.DesktopSrc = desktopSource;
                    q.MobileSrc = mobileSource;
                    q.AssemblyName = assemblyName;
                    q.ClassName = className;
                    q.Admin = portalModule.AdminModule;
                    q.Searchable = portalModule.Searchable;

                    db.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (Exception ex)
                {
                    //Rethrow exception
                    throw new Exception(
                        "UpdateGeneralModuleDefinitions Exception '" + portalModule.GuidID.ToString() + "'!", ex);
                }
            }

            // Update the module definition - install for portal 0
            modules.UpdateModuleDefinitions(defID, 0, true);
        }

        /// <summary>
        /// Uninstalls the specified desktop source.
        /// </summary>
        /// <param name="desktopSource">The desktop source.</param>
        /// <param name="mobileSource">The mobile source.</param>
        public static void Uninstall(string desktopSource, string mobileSource)
        {
            Page page = new Page();

            // Istantiate the module
            PortalModuleControl portalModule =
                (PortalModuleControl)page.LoadControl(Path.ApplicationRoot + "/" + desktopSource);

            //Call Uninstall
            try
            {
                portalModule.Uninstall(null);
            }
            catch (Exception ex)
            {
                //Rethrow exception
                throw new Exception("Exception during uninstall!", ex);
            }

            // Delete definition
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.rb_GeneralModuleDefinitions.DeleteAllOnSubmit(db.rb_GeneralModuleDefinitions.Where(g => g.GeneralModDefID == portalModule.GuidID));
            db.SubmitChanges(ConflictMode.ContinueOnConflict);
        }
    }
}