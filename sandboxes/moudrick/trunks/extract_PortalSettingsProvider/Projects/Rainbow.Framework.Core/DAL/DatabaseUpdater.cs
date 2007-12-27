using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Xml;
using Rainbow.Framework.Data;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Settings;

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
 
        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        public static int DatabaseVersion
        {
            //by Manu 16/10/2003
            //Added 2 mods:
            //1) Rbversion is created if it is missed.
            //   This is expecially good for empty databases.
            //   Be aware that this can break compatibility with 1613 version
            //2) Connection problems are thown immediately as errors.
            get
            {
                //Caches dbversion

                if (HttpContext.Current.Application[DbKey] == null)
                {
                    try
                    {
                        //Create rbversion if it is missing
                        string createRbVersions =
                            "IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rb_Versions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
                            "CREATE TABLE [rb_Versions] (" +
                            "[Release] [int] NOT NULL , " +
                            "[Version] [nvarchar] (50) NULL , " +
                            "[ReleaseDate] [datetime] NULL " +
                            ") ON [PRIMARY]"
                            ;
                        DBHelper.ExeSQL(createRbVersions);
                    }

                    catch (SqlException ex)
                    {
                        throw new DatabaseUnreachableException(
                            "Failed to get Database Version - most likely cannot connect to db or no permission.", ex);
                        // Jes1111
                        //Rainbow.Framework.Configuration.ErrorHandler.HandleException("If this fails most likely cannot connect to db or no permission", ex);
                        //If this fails most likely cannot connect to db or no permission
                        //throw;
                    }
                    object version = DBHelper.ExecuteSQLScalar(
                        "SELECT TOP 1 Release FROM rb_Versions ORDER BY Release DESC");

                    int curVersion;
                    if (version != null)
                    {
                        curVersion = Int32.Parse(version.ToString());
                    }
                    else
                    {
                        curVersion = 1110;
                        // TODO: This should be the best place
                        // where run the codefor empty db
                    }
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[DbKey] = curVersion;
                    HttpContext.Current.Application.UnLock();
                }
                return (int)HttpContext.Current.Application[DbKey];
            }
        }

        /// <summary>
        /// Gets the code version.
        /// </summary>
        /// <value>The code version.</value>
        public static int CodeVersion
        {
            get
            {
                HttpContext httpContext = HttpContext.Current;
                const string codeVersionParameterName = "CodeVersion";
                if (httpContext != null)
                {
                    if (httpContext.Application[codeVersionParameterName] == null)
                    {
                        FileVersionInfo f =
                            FileVersionInfo.GetVersionInfo(
                                Assembly.GetAssembly(typeof (RainbowContext)).Location);
                        HttpContext.Current.Application.Lock();
                        HttpContext.Current.Application[codeVersionParameterName] = f.FilePrivatePart;
                        HttpContext.Current.Application.UnLock();
                    }
                    return (int)httpContext.Application[codeVersionParameterName];
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public static string ProductVersion
        {
            get
            {
                if (HttpContext.Current.Application["ProductVersion"] == null)
                {
                    FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
                    HttpContext.Current.Application.UnLock();
                }
                return (string)HttpContext.Current.Application["ProductVersion"];
            }
        }

        /// <summary>
        /// This property returns db version.
        /// It does not rely on cached value and always gets the actual value.
        /// </summary>
        /// <value>The database version.</value>
        static int DatabaseVersionWithCacheReset
        {
            get
            {
                //Clear version cache so we are sure we update correctly
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[DbKey] = null;
                HttpContext.Current.Application.UnLock();
                int version = DatabaseVersion;
                Debug.WriteLine("DatabaseVersion: " + version);
                return version;
            }
        }
 
        static string DbKey
        {
            get
            {
                string dbKey = "CurrentDatabase";
                if (Config.EnableMultiDbSupport)
                {
                    dbKey = "DatabaseVersion_" + Config.SqlConnectionString.DataSource + "_" +
                            Config.SqlConnectionString.Database; // For multidb support
                }
                return dbKey;
            }
        }
  
        ///<summary>
        /// Creates an updater
        ///</summary>
        ///<param name="updateScriptPath">Update Script Path</param>
        ///<param name="moduleScriptBasePath"></param>
        public DatabaseUpdater(string updateScriptPath, 
            string moduleScriptBasePath)
        {
            this.updateScriptPath = updateScriptPath;
            this.moduleScriptBasePath = moduleScriptBasePath;
        }

        ///<summary>
        /// Prepares preview list of updates to perform
        ///</summary>
        ///<exception cref="Exception"></exception>
        public void PreviewUpdate()
        {
            int dbVersion = DatabaseVersionWithCacheReset;

            if (dbVersion > 1114 && dbVersion < 1519)
            {
                ErrorHandler.Publish(LogLevel.Error, "Unsupported version " + dbVersion + " detected.");
                throw new Exception("Version before 1519 are not supported anymore. Please upgrade to a newer version or upgrade manually.");
            }

            if (dbVersion < 1114)
            {
                initialStatusReport = string.Format("Empty/New database - CodeVersion: {0}", CodeVersion);
            }
            else
            {
                initialStatusReport = string.Format("dbVersion: {0} - CodeVersion: {1}",
                    dbVersion, CodeVersion);
            }

            // ******************************
            // New code starts here - Jes1111
            // ******************************
            // this is not a performance-sensitive routine, so XmlDocument is sufficient
            XmlDocument xmlDocument = new XmlDocument();
            ArrayList tempScriptsList = new ArrayList();

            if (dbVersion < CodeVersion)
            {
                ErrorHandler.Publish(LogLevel.Debug, "db:" + dbVersion + " Code:" + CodeVersion);

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
        public void PerformUpdate()
        {
            foreach (UpdateEntry updateEntry in scriptsList)
            {
                //Version check (a script may update more than one version at once)
                if (updateEntry.Apply && DatabaseVersionWithCacheReset < updateEntry.VersionNumber && DatabaseVersionWithCacheReset < CodeVersion)
                {
                    foreach (string scriptName in updateEntry.scriptNames)
                    {
                        //It may be a module update only
                        if (scriptName.Length > 0)
                        {
                            string currentScriptName = System.IO.Path.Combine(updateScriptPath, scriptName);
                            ErrorHandler.Publish(LogLevel.Info,
                                string.Format("CODE: {0} - DB: {1} - CURR: {2} - Applying: {3}",
                                    CodeVersion,
                                    DatabaseVersionWithCacheReset,
                                    updateEntry.VersionNumber,
                                    currentScriptName));
                            ArrayList currentErrors = Rainbow.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
                            Errors.AddRange(currentErrors);                    //Display errors if any

                            if (currentErrors.Count > 0)
                            {
                                Errors.Insert(0, "<P>" + scriptName + "</P>");
                                ErrorHandler.Publish(LogLevel.Error,
                                    "Version " + updateEntry.Version + " completed with errors.  - " + scriptName);
                                Debug.WriteLine(
                                    "Version " + updateEntry.Version + " completed with errors.  - " + scriptName);
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
                            Debug.WriteLine(
                                "Version " + updateEntry.Version + " completed with errors.  - " + currentModuleInstaller);
                        }
                    }

                    if (Errors.Count == 0)
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
                        string.Format("CODE: {0} - DB: {1} - CURR: {2} - Skipping: {3}",
                            CodeVersion,
                            DatabaseVersionWithCacheReset,
                            updateEntry.VersionNumber,
                            updateEntry.Version));
//                                         "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseVersion + " - CURR: " +
//                                         updateEntry.VersionNumber + " - Skipping: " + updateEntry.Version);
                }
            }
        }
    }
}
