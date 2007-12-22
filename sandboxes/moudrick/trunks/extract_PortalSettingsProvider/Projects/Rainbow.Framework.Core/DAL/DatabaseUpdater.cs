using System;
using System.Collections;
using System.Web;
using System.Xml;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Settings;

namespace Rainbow.Framework.Core.DAL
{
    ///<summary>
    /// Updates database to the latest version
    ///</summary>
    public class DatabaseUpdater
    {
        readonly string webApplicationRootPhysicalPath;

        ///<summary>
        /// List of actions to perform update
        ///</summary>
        public readonly ArrayList UpdateList = new ArrayList();


        ///<summary>
        /// List of update error messages
        ///</summary>
        public readonly ArrayList Errors = new ArrayList();

        ///<summary>
        /// List of update non-error messages
        ///</summary>
        public readonly ArrayList Messages = new ArrayList();

        DatabaseUpdater.UpdateEntry[] scriptsList;
        string initialStatusReport;

        ///<summary>
        /// Database staatus beofre any operation
        ///</summary>
        public string InitialStatusReport
        {
            get
            {
                return initialStatusReport;
            }
        }


        ///<summary>
        /// Creates an updater
        ///</summary>
        ///<param name="webApplicationRootPhysicalPath">Web Applcation Root Physical Path</param>
        public DatabaseUpdater(string webApplicationRootPhysicalPath)
        {
            this.webApplicationRootPhysicalPath = webApplicationRootPhysicalPath;
        }

        [Serializable]
        class UpdateEntry : IComparable {
            /// <summary>
            /// IComparable.CompareTo implementation.
            /// </summary>
            /// <param name="obj">An object to compare with this instance.</param>
            /// <returns>
            /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
            /// </returns>
            /// <exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception>
            public int CompareTo( object obj ) {
                if ( obj is UpdateEntry ) {
                    UpdateEntry upd = ( UpdateEntry )obj;
                    if ( VersionNumber.CompareTo( upd.VersionNumber ) == 0 ) //Version numbers are equal
                        return Version.CompareTo( upd.Version );
                    else
                        return VersionNumber.CompareTo( upd.VersionNumber );
                }
                throw new ArgumentException( "object is not a UpdateEntry" );
            }

            public int VersionNumber = 0;
            public string Version = string.Empty;
            public readonly ArrayList scriptNames = new ArrayList();
            public DateTime Date;
            public readonly ArrayList Modules = new ArrayList();
            public bool Apply = false;
        }

        /// <summary>
        /// This property returns db version.
        /// It does not rely on cached value and always gets the actual value.
        /// </summary>
        /// <value>The database version.</value>
        public static int DatabaseVersion 
        {
            get 
            {
                //Clear version cache so we are sure we update correctly
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[Database.dbKey] = null;
                HttpContext.Current.Application.UnLock();
                return Database.DatabaseVersion;
            }
        }

        ///<summary>
        /// Prepares preview list of updates to perform
        ///</summary>
        ///<exception cref="Exception"></exception>
        public void PreviewUpdate()
        {
            int dbVersion = DatabaseUpdater.DatabaseVersion;

            if (dbVersion > 1114 && dbVersion < 1519)
            {
                ErrorHandler.Publish(LogLevel.Error, "Unsupported version " + dbVersion + " detected.");
                throw new Exception("Version before 1519 are not supported anymore. Please upgrade to a newer version or upgrade manually.");
            }

            if (dbVersion < 1114)
            {
                initialStatusReport = string.Format("Empty/New database - CodeVersion: {0}", Portal.CodeVersion);
            }
            else
            {
                initialStatusReport = string.Format("dbVersion: {0} - CodeVersion: {1}",
                    dbVersion, Portal.CodeVersion);
            }

            // ******************************
            // New code starts here - Jes1111
            // ******************************
            // this is not a performance-sensitive routine, so XmlDocument is sufficient
            XmlDocument xmlDocument = new XmlDocument();
            ArrayList tempScriptsList = new ArrayList();

            if (dbVersion < Portal.CodeVersion)
            {
                ErrorHandler.Publish(LogLevel.Debug, "db:" + dbVersion + " Code:" + Portal.CodeVersion);

                // load the history file
                string filename = System.IO.Path.Combine(webApplicationRootPhysicalPath, @"Setup\Scripts\History.xml");
                xmlDocument.Load(filename);

                // get a list of <Release> nodes
                XmlNodeList releases = xmlDocument.DocumentElement.SelectNodes("Release");

                // iterate over the <Release> nodes
                // (we can do this because XmlNodeList implements IEnumerable)
                foreach (XmlNode release in releases)
                {
                    DatabaseUpdater.UpdateEntry updateEntry = new DatabaseUpdater.UpdateEntry();

                    // get the header information
                    // we check for null to avoid exception if any of these nodes are not present
                    if (release.SelectSingleNode("ID") != null)
                    {
                        updateEntry.VersionNumber = Int32.Parse(release.SelectSingleNode("ID/text()").Value);
                    }

                    if (release.SelectSingleNode("Version") != null)
                    {
                        updateEntry.Version = release.SelectSingleNode("Version/text()").Value;
                    }

                    if (release.SelectSingleNode("Script") != null)
                    {
                        updateEntry.scriptNames.Add(release.SelectSingleNode("Script/text()").Value);
                    }

                    if (release.SelectSingleNode("Date") != null)
                    {
                        updateEntry.Date = DateTime.Parse(release.SelectSingleNode("Date/text()").Value);
                    }

                    //We should apply this patch
                    if (dbVersion < updateEntry.VersionNumber)
                    {
                        //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected version to apply: " + myUpdate.Version);

                        updateEntry.Apply = true;

                        // get a list of <Installer> nodes
                        XmlNodeList installers = release.SelectNodes("Modules/Installer/text()");

                        // iterate over the <Installer> Nodes (in original document order)
                        // (we can do this because XmlNodeList implements IEnumerable)
                        foreach (XmlNode installer in installers)
                        {
                            //and build an ArrayList of the scripts... 
                            updateEntry.Modules.Add(installer.Value);
                            //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected module to install: " + installer.Value);
                        }

                        // get a <Script> node, if any
                        XmlNodeList sqlScripts = release.SelectNodes("Scripts/Script/text()");

                        // iterate over the <Installer> Nodes (in original document order)
                        // (we can do this because XmlNodeList implements IEnumerable)
                        foreach (XmlNode sqlScript in sqlScripts)
                        {
                            //and build an ArrayList of the scripts... 
                            updateEntry.scriptNames.Add(sqlScript.Value);
                            //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected script to run: " + sqlScript.Value);
                        }

                        tempScriptsList.Add(updateEntry);
                    }
                }

                //If we have some version to apply...
                if (tempScriptsList.Count > 0)
                {
                    scriptsList = (DatabaseUpdater.UpdateEntry[])tempScriptsList.ToArray(typeof(DatabaseUpdater.UpdateEntry));

                    //by Manu. Versions are sorted by version number
                    Array.Sort(scriptsList);

                    //Create a flat version for binding
                    int currentVersion = 0;
                    foreach (DatabaseUpdater.UpdateEntry updateEntry in scriptsList)
                    {
                        if (updateEntry.Apply)
                        {
                            if (currentVersion != updateEntry.VersionNumber)
                            {
                                UpdateList.Add("Version: " + updateEntry.VersionNumber);
                                currentVersion = updateEntry.VersionNumber;
                            }

                            foreach (string scriptName in updateEntry.scriptNames)
                            {
                                if (scriptName.Length > 0)
                                {
                                    UpdateList.Add("-- Script: " + scriptName);
                                }
                            }

                            foreach (string moduleInstaller in updateEntry.Modules)
                            {
                                if (moduleInstaller.Length > 0)
                                {
                                    UpdateList.Add("-- Module: " + moduleInstaller);
                                }
                            }
                        }
                    }
                }
            }
        }

        ///<summary>
        /// Performs update
        ///</summary>
        public void PerformUpdate()
        {
            foreach (DatabaseUpdater.UpdateEntry updateEntry in scriptsList)
            {
                //Version check (a script may update more than one version at once)
                if (updateEntry.Apply && DatabaseUpdater.DatabaseVersion < updateEntry.VersionNumber && DatabaseVersion < Portal.CodeVersion)
                {
                    foreach (string scriptName in updateEntry.scriptNames)
                    {
                        //It may be a module update only
                        if (scriptName.Length > 0)
                        {
                            string currentScriptName =
                                System.IO.Path.Combine(webApplicationRootPhysicalPath + "/Setup/Scripts/", scriptName);
                            ErrorHandler.Publish(LogLevel.Info,
                                string.Format("CODE: {0} - DB: {1} - CURR: {2} - Applying: {3}",
                                    Portal.CodeVersion,
                                    DatabaseUpdater.DatabaseVersion,
                                    updateEntry.VersionNumber,
                                    currentScriptName));
                            ArrayList currentErrors = Rainbow.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
                            Errors.AddRange(currentErrors);                    //Display errors if any

                            if (currentErrors.Count > 0)
                            {
                                Errors.Insert(0, "<P>" + scriptName + "</P>");
                                ErrorHandler.Publish(LogLevel.Error,
                                    "Version " + updateEntry.Version + " completed with errors.  - " +
                                    scriptName);
                                Console.WriteLine("Version " + updateEntry.Version + " completed with errors.  - " +
                                    scriptName);
                                break;
                            }
                        }
                    }

                    //Installing modules
                    foreach (string moduleInstaller in updateEntry.Modules)
                    {
                        string currentModuleInstaller =
                            System.IO.Path.Combine(webApplicationRootPhysicalPath + "/", moduleInstaller);

                        try
                        {
                            ModuleInstall.InstallGroup(currentModuleInstaller, true);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Publish(LogLevel.Fatal,
                                                 "Exception in UpdateDatabaseCommand installing module: " +
                                                 currentModuleInstaller, ex);
                            if (ex.InnerException != null)
                            {
                                // Display more meaningful error message if InnerException is defined
                                ErrorHandler.Publish(LogLevel.Warn,
                                                     "Exception in UpdateDatabaseCommand installing module: " +
                                                     currentModuleInstaller, ex.InnerException);
                                Errors.Add("Exception in UpdateDatabaseCommand installing module: " +
                                           currentModuleInstaller + "<br/>" + ex.InnerException.Message + "<br/>" +
                                           ex.InnerException.StackTrace);
                            }
                            else
                            {
                                ErrorHandler.Publish(LogLevel.Warn,
                                                     "Exception in UpdateDatabaseCommand installing module: " +
                                                     currentModuleInstaller, ex);
                                Errors.Add(ex.Message);
                            }
                            Console.WriteLine("Version " + updateEntry.Version + " completed with errors.  - " +
                                currentModuleInstaller);
                        }
                    }

                    if (Equals(Errors.Count, 0))
                    {
                        //Update db with version
                        string versionUpdater;
                        versionUpdater = "INSERT INTO [rb_Versions] ([Release],[Version],[ReleaseDate]) VALUES('" +
                                         updateEntry.VersionNumber + "','" + updateEntry.Version + "', CONVERT(datetime, '" +
                                         updateEntry.Date.Month + "/" + updateEntry.Date.Day + "/" + updateEntry.Date.Year + "', 101))";
                        Rainbow.Framework.Data.DBHelper.ExeSQL(versionUpdater);
                        ErrorHandler.Publish(LogLevel.Info,
                                             "Version number: " + updateEntry.Version + " applied successfully.");

                        //Mark this update as done
                        ErrorHandler.Publish(LogLevel.Info, "Sucessfully applied version: " + updateEntry.Version);
                    }
                }
                else
                {
                    //Skipped
                    //string skippedMessage = "Skipping: " + myUpdate.Version
                    //	+ " DbVersion (" + DatabaseVersion
                    //	+ ") "
                    //	+ " Codeversion (" + Portal.CodeVersion
                    //	+ ")";
                    // messages.Add(skippedMessage);
                    ErrorHandler.Publish(LogLevel.Info,
                                         "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseUpdater.DatabaseVersion + " - CURR: " +
                                         updateEntry.VersionNumber + " - Skipping: " + updateEntry.Version);
                }
            }
        }
    }
}
