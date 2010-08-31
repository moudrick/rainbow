namespace Rainbow.Tests
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Xml;

    using Rainbow.Framework.Data;
    using Rainbow.Framework.Helpers;

    /// <summary>
    /// The update entry.
    /// </summary>
    [Serializable]
    internal class UpdateEntry : IComparable
    {
        #region Constants and Fields

        /// <summary>
        /// The apply.
        /// </summary>
        public bool Apply;

        /// <summary>
        /// The date.
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// The modules.
        /// </summary>
        public ArrayList Modules = new ArrayList();

        /// <summary>
        /// The version.
        /// </summary>
        public string Version = string.Empty;

        /// <summary>
        /// The version number.
        /// </summary>
        public int VersionNumber;

        /// <summary>
        /// The script names.
        /// </summary>
        public ArrayList scriptNames = new ArrayList();

        #endregion

        #region Implemented Interfaces

        #region IComparable

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// obj is not the same type as this instance. 
        /// </exception>
        public int CompareTo(object obj)
        {
            if (obj is UpdateEntry)
            {
                var upd = (UpdateEntry)obj;
                return this.VersionNumber.CompareTo(upd.VersionNumber) == 0 ? this.Version.CompareTo(upd.Version) : this.VersionNumber.CompareTo(upd.VersionNumber);
            }

            throw new ArgumentException("object is not a UpdateEntry");
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// The test helper.
    /// </summary>
    public class TestHelper
    {
        #region Constants and Fields

        /// <summary>
        /// The scripts list.
        /// </summary>
        private static UpdateEntry[] scriptsList;

        #endregion

        #region Public Methods

        /// <summary>
        /// The recreate db schema.
        /// </summary>
        public static void RecreateDbSchema()
        {
            InitializeScriptList();
            RunScriptList();
        }

        /// <summary>
        /// The run data script.
        /// </summary>
        /// <param name="strRelativeScriptPath">
        /// The str relative script path.
        /// </param>
        public static void RunDataScript(string strRelativeScriptPath)
        {
            // 1 - prepend full root
            /*NOTE: we can't simply get the Executing Path of the calling assembly
				* because that will vary if the assembly is shadow-copied by NUnit or 
				* other testing tools. Therefore, for simplicity in this demo, we just
				* get it as a App key.
				*/
            var strUtRoot = ConfigurationManager.AppSettings["DataScriptsPath"];
            var strFullScriptPath = strUtRoot + strRelativeScriptPath;
            RunScript(strFullScriptPath);
        }

        /// <summary>
        /// The run rainbow script.
        /// </summary>
        /// <param name="strRelativeScriptPath">
        /// The str relative script path.
        /// </param>
        public static void RunRainbowScript(string strRelativeScriptPath)
        {
            // 1 - prepend full root
            /*NOTE: we can't simply get the Executing Path of the calling assembly
				* because that will vary if the assembly is shadow-copied by NUnit or 
				* other testing tools. Therefore, for simplicity in this demo, we just
				* get it as a App key.
				*/
            var strUtRoot = ConfigurationManager.AppSettings["RainbowSetupScriptsPath"];
            var strFullScriptPath = strUtRoot + strRelativeScriptPath;
            RunScript(strFullScriptPath);
        }

        /// <summary>
        /// The tear down db.
        /// </summary>
        public static void TearDownDb()
        {
            RunDataScript("TearDown.sql");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize script list.
        /// </summary>
        private static void InitializeScriptList()
        {
            const int DbVersion = 0;

            var xmlDocument = new XmlDocument();
            var tempScriptsList = new ArrayList();

            // load the history file
            var docPath = ConfigurationManager.AppSettings["RainbowSetupScriptsPath"] + "History.xml";
            xmlDocument.Load(docPath);

            // get a list of <Release> nodes
            if (xmlDocument.DocumentElement != null)
            {
                var releases = xmlDocument.DocumentElement.SelectNodes("Release");

                if (releases != null)
                {
                    foreach (XmlNode release in releases)
                    {
                        var updateEntry = new UpdateEntry();

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

                        // We should apply this patch
                        if (DbVersion < updateEntry.VersionNumber)
                        {
                            // Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected version to apply: " + myUpdate.Version);
                            updateEntry.Apply = true;

                            // get a list of <Installer> nodes
                            var installers = release.SelectNodes("Modules/Installer/text()");

                            // iterate over the <Installer> Nodes (in original document order)
                            // (we can do this because XmlNodeList implements IEnumerable)
                            if (installers != null)
                            {
                                foreach (XmlNode installer in installers)
                                {
                                    // and build an ArrayList of the scripts... 
                                    updateEntry.Modules.Add(installer.Value);

                                    // Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected module to install: " + installer.Value);
                                }
                            }

                            // get a <Script> node, if any
                            var sqlScripts = release.SelectNodes("Scripts/Script/text()");

                            // iterate over the <Installer> Nodes (in original document order)
                            // (we can do this because XmlNodeList implements IEnumerable)
                            if (sqlScripts != null)
                            {
                                foreach (XmlNode sqlScript in sqlScripts)
                                {
                                    // and build an ArrayList of the scripts... 
                                    updateEntry.scriptNames.Add(sqlScript.Value);

                                    // Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected script to run: " + sqlScript.Value);
                                }
                            }

                            tempScriptsList.Add(updateEntry);
                        }
                    }
                }
            }

            // If we have some version to apply...
            if (tempScriptsList.Count > 0)
            {
                scriptsList = (UpdateEntry[])tempScriptsList.ToArray(typeof(UpdateEntry));

                // by Manu. Versions are sorted by version number
                Array.Sort(scriptsList);

                // Create a flat version for binding
                var currentVersion = 0;
                foreach (var update in scriptsList.Where(updateEntry => updateEntry.Apply))
                {
                    if (currentVersion != update.VersionNumber)
                    {
                        LogHelper.Logger.Log(LogLevel.Debug, string.Format("Version: {0}", update.VersionNumber));
                        currentVersion = update.VersionNumber;
                    }

                    foreach (string scriptName in
                        update.scriptNames.Cast<string>().Where(scriptName => scriptName.Length > 0))
                    {
                        LogHelper.Logger.Log(LogLevel.Debug, string.Format("-- Script: {0}", scriptName));
                    }

                    foreach (string moduleInstaller in update.Modules)
                    {
                        if (moduleInstaller.Length > 0)
                        {
                            LogHelper.Logger.Log(
                                LogLevel.Debug, string.Format("-- Module: {0} (ignored recreating test DB)", moduleInstaller));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The run script list.
        /// </summary>
        private static void RunScriptList()
        {
            var dataScriptsDoc = new XmlDocument();
            dataScriptsDoc.Load(ConfigurationManager.AppSettings["DataScriptsDefinitionFile"]);

            var databaseVersion = 0;
            var errors = new ArrayList();

            foreach (var updateEntry in scriptsList)
            {
                // Version check (a script may update more than one version at once)
                if (updateEntry.Apply && databaseVersion < updateEntry.VersionNumber)
                {
                    // It may be a module update only
                    foreach (var currentScriptName in
                        updateEntry.scriptNames.Cast<string>().Where(scriptName => scriptName.Length > 0))
                    {
                        Console.WriteLine("DB: {0} - CURR: {1} - Applying: {2}", databaseVersion, updateEntry.VersionNumber, currentScriptName);

                        RunRainbowScript(currentScriptName);
                    }

                    ////Installing modules
                    // foreach ( string moduleInstaller in myUpdate.Modules ) {
                    // string currentModuleInstaller =
                    // Server.MapPath( System.IO.Path.Combine( Path.ApplicationRoot + "/", moduleInstaller ) );

                    // try {
                    // ModuleInstall.InstallGroup( currentModuleInstaller, true );
                    // }
                    // catch ( Exception ex ) {
                    // Console.WriteLine( "Exception in UpdateDatabaseCommand installing module: " +
                    // currentModuleInstaller, ex );
                    // if ( ex.InnerException != null ) {
                    // // Display more meaningful error message if InnerException is defined
                    // Console.WriteLine( "Exception in UpdateDatabaseCommand installing module: " +
                    // currentModuleInstaller, ex.InnerException );
                    // errors.Add( "Exception in UpdateDatabaseCommand installing module: " +
                    // currentModuleInstaller + "<br/>" + ex.InnerException.Message + "<br/>" +
                    // ex.InnerException.StackTrace );
                    // }
                    // else {
                    // Console.WriteLine( "Exception in UpdateDatabaseCommand installing module: " +
                    // currentModuleInstaller, ex );
                    // errors.Add( ex.Message );
                    // }
                    // }
                    // }
                    if (Equals(errors.Count, 0))
                    {
                        // Update db with version
                        databaseVersion = updateEntry.VersionNumber;
                        Console.WriteLine("Version number: {0} applied successfully.", updateEntry.Version);

                        // apply any additional data scripts after applying version
                        var dataScriptsNodeList =
                            dataScriptsDoc.SelectNodes(
                                string.Format("/SqlDataScripts/SqlDataScript[@runAfterVersion={0}]", updateEntry.VersionNumber));
                        if (dataScriptsNodeList != null)
                        {
                            foreach (var node in
                                dataScriptsNodeList.Cast<XmlNode>().Where(node => node.Attributes != null))
                            {
                                Console.WriteLine("Running data script {0} after version {1}", node.Attributes["fileName"].Value, updateEntry.VersionNumber);
                                RunDataScript(node.Attributes["fileName"].Value);
                            }
                        }

                        // Mark this update as done
                        Console.WriteLine("Sucessfully applied version: {0}", updateEntry.Version);
                    }
                }
                else
                {
                    Console.WriteLine("DB: {0} - CURR: {1} - Skipping: {2}", databaseVersion, updateEntry.VersionNumber, updateEntry.Version);
                }
            }
        }

        /// <summary>
        /// Runs the script.
        /// </summary>
        /// <param name="fullScriptPath">The full script path.</param>
        private static void RunScript(string fullScriptPath)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            DBHelper.ExecuteScript(fullScriptPath, conn);
            Console.WriteLine("Ran script {0}", fullScriptPath);
        }

        #endregion
    }

    /// <summary>
    /// The sql admin.
    /// </summary>
    internal struct SqlAdmin
    {
        #region Constants and Fields

        /// <summary>
        /// The database.
        /// </summary>
        private readonly string database;

        /// <summary>
        /// The password.
        /// </summary>
        private readonly string password;

        /// <summary>
        /// The server.
        /// </summary>
        private readonly string server;

        /// <summary>
        /// The user id.
        /// </summary>
        private readonly string userId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAdmin"/> struct.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="password">The password.</param>
        /// <param name="server">The server.</param>
        /// <param name="database">The database.</param>
        public SqlAdmin(string userId, string password, string server, string database)
        {
            this.userId = userId;
            this.password = password;
            this.server = server;
            this.database = database;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Database.
        /// </summary>
        public string Database
        {
            get
            {
                return this.database;
            }
        }

        /// <summary>
        /// Gets Password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }
        }

        /// <summary>
        /// Gets Server.
        /// </summary>
        public string Server
        {
            get
            {
                return this.server;
            }
        }

        /// <summary>
        /// Gets UserId.
        /// </summary>
        public string UserId
        {
            get
            {
                return this.userId;
            }
        }

        #endregion
    }
}