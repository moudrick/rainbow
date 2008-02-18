using System;
using System.Collections;
using System.Xml;
using Rainbow.Framework.Data;
using Rainbow.Framework.Helpers;

namespace Rainbow.Framework.Core.DAL
{
    ///<summary>
    /// Updates database to the latest version
    ///</summary>
    public class DatabaseUpdater
    {
        readonly string updateScriptPath;
        readonly string moduleScriptBasePath;

        UpdateEntry[] scriptsList;
        string initialStatusReport;

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
        ///<param name="updateScriptPath">Update Script Path</param>
        ///<param name="moduleScriptBasePath"></param>
        public DatabaseUpdater(string updateScriptPath, string moduleScriptBasePath)
        {
            this.updateScriptPath = updateScriptPath;
            this.moduleScriptBasePath = moduleScriptBasePath;
        }

        ///<summary>
        /// Prepares preview list of updates to perform
        ///</summary>
        ///<exception cref="Exception"></exception>
        public void PreviewUpdate(VersionController versionController)
        {
            int dbVersion = versionController.DatabaseVersionWithCacheReset;

            if (dbVersion > 1114 && dbVersion < 1519)
            {
                ErrorHandler.Publish(LogLevel.Error, "Unsupported version " + dbVersion + " detected.");
                throw new Exception("Version before 1519 are not supported anymore. Please upgrade to a newer version or upgrade manually.");
            }

            if (dbVersion < 1114)
            {
                initialStatusReport = string.Format("Empty/New database - CodeVersion: {0}", versionController.CodeVersion);
            }
            else
            {
                initialStatusReport = string.Format("dbVersion: {0} - CodeVersion: {1}",
                    dbVersion, versionController.CodeVersion);
            }

            // ******************************
            // New code starts here - Jes1111
            // ******************************
            // this is not a performance-sensitive routine, so XmlDocument is sufficient
            XmlDocument xmlDocument = new XmlDocument();
            ArrayList tempScriptsList = new ArrayList();

            if (dbVersion < versionController.CodeVersion)
            {
                ErrorHandler.Publish(LogLevel.Debug, "db:" + dbVersion + " Code:" + versionController.CodeVersion);

                // load the history file
                string filename = System.IO.Path.Combine(updateScriptPath, "History.xml");
                xmlDocument.Load(filename);

                // get a list of <Release> nodes
                XmlNodeList releases = xmlDocument.DocumentElement.SelectNodes("Release");

                // iterate over the <Release> nodes
                // (we can do this because XmlNodeList implements IEnumerable)
                foreach (XmlNode release in releases)
                {
                    UpdateEntry updateEntry = new UpdateEntry();

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
                    scriptsList = (UpdateEntry[])tempScriptsList.ToArray(typeof(UpdateEntry));

                    //by Manu. Versions are sorted by version number
                    Array.Sort(scriptsList);

                    //Create a flat version for binding
                    int currentVersion = 0;
                    foreach (UpdateEntry updateEntry in scriptsList)
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
        public void PerformUpdate(VersionController versionController)
        {
            foreach (UpdateEntry updateEntry in scriptsList)
            {
                //Version check (a script may update more than one version at once)
                if (updateEntry.Apply 
                    && versionController.DatabaseVersionWithCacheReset < updateEntry.VersionNumber 
                    && versionController.DatabaseVersionWithCacheReset < versionController.CodeVersion)
                {
                    foreach (string scriptName in updateEntry.scriptNames)
                    {
                        //It may be a module update only
                        if (scriptName.Length > 0)
                        {
                            string currentScriptName = System.IO.Path.Combine(updateScriptPath, scriptName);
                            ErrorHandler.Publish(LogLevel.Info,
                                string.Format("CODE: {0} - DB: {1} - CURR: {2} - Applying: {3}",
                                    versionController.CodeVersion,
                                    versionController.DatabaseVersionWithCacheReset,
                                    updateEntry.VersionNumber,
                                    currentScriptName));
                            ArrayList currentErrors = DBHelper.ExecuteScript(currentScriptName, true);
                            Errors.AddRange(currentErrors);                    //Display errors if any

                            if (currentErrors.Count > 0)
                            {
                                Errors.Insert(0, "<P>" + scriptName + "</P>");
                                string message = string.Format("Version {0} completed with errors.  - {1}", 
                                    updateEntry.Version, scriptName);
                                ErrorHandler.Publish(LogLevel.Error, message);
                                break;
                            }
                        }
                    }

                    //Installing modules
                    foreach (string moduleInstaller in updateEntry.Modules)
                    {
                        string currentModuleInstaller =
                            System.IO.Path.Combine(moduleScriptBasePath, moduleInstaller);

                        try
                        {
                            ModuleInstall.InstallGroup(currentModuleInstaller, true);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Publish(LogLevel.Fatal,
                                string.Format("Exception in UpdateDatabaseCommand installing module: {0}", 
                                    currentModuleInstaller), 
                                ex);
                            if (ex.InnerException != null)
                            {
                                // Display more meaningful error message if InnerException is defined
                                ErrorHandler.Publish(LogLevel.Warn,
                                    string.Format("Exception in UpdateDatabaseCommand installing module: {0}", 
                                        currentModuleInstaller), 
                                    ex.InnerException);
                                Errors.Add(
                                    string.Format(
                                        "Exception in UpdateDatabaseCommand installing module: {0}<br/>{1}<br/>{2}",
                                        currentModuleInstaller,
                                        ex.InnerException.Message,
                                        ex.InnerException.StackTrace));
                            }
                            else
                            {
                                ErrorHandler.Publish(LogLevel.Warn,
                                    string.Format("Exception in UpdateDatabaseCommand installing module: {0}", 
                                        currentModuleInstaller), 
                                    ex);
                                Errors.Add(ex.Message);
                            }
                        }
                    }

                    if (Errors.Count == 0)
                    {
                        string versionUpdater = string.Format(
                            "INSERT INTO [rb_Versions] ([Release],[Version],[ReleaseDate]) VALUES('{0}','{1}', CONVERT(datetime, '{2}/{3}/{4}', 101))",
                            updateEntry.VersionNumber,
                            updateEntry.Version,
                            updateEntry.Date.Month,
                            updateEntry.Date.Day,
                            updateEntry.Date.Year);
                        DBHelper.ExeSQL(versionUpdater);
                        ErrorHandler.Publish(LogLevel.Info,
                            string.Format("Version number: {0} applied successfully.", 
                            updateEntry.Version));
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
                        string.Format("CODE: {0} - DB: {1} - CURR: {2} - Skipping: {3}",
                            versionController.CodeVersion,
                            versionController.DatabaseVersionWithCacheReset,
                            updateEntry.VersionNumber,
                            updateEntry.Version));
//                                         "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseVersion + " - CURR: " +
//                                         updateEntry.VersionNumber + " - Skipping: " + updateEntry.Version);
                }
            }
        }
    }
}
