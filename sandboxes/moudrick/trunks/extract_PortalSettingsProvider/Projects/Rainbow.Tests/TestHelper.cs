using System;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Xml;
using NUnit.Framework;
using Rainbow.Framework.Core.DAL;
using Rainbow.Framework.Data;
using Rainbow.Framework.Helpers;
using Rainbow.Framework;
using Rainbow.Framework.Settings;
using Subtext.TestLibrary;

namespace Rainbow.Tests {
    
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
                    UpdateEntry upd = ( UpdateEntry ) obj;
                    if ( VersionNumber.CompareTo( upd.VersionNumber ) == 0 ) //Version numbers are equal
                    {
                        return Version.CompareTo( upd.Version );
                    }
                    else {
                        return VersionNumber.CompareTo( upd.VersionNumber );
                    }
                }
                throw new ArgumentException( "object is not a UpdateEntry" );
            }


            public int VersionNumber = 0;
            public string Version = string.Empty;
            public ArrayList scriptNames = new ArrayList();
            public DateTime Date;
            public ArrayList Modules = new ArrayList();
            public bool Apply = false;
        }
   

    public class TestHelper 
    {
        public static void TearDownDB() 
        {
            RunDataScript("TearDown.sql");
        }
        static UpdateEntry[] scriptsList;

        public static void RecreateDBSchema() 
        {
            InitializeScriptList();
            RunScriptList();
/*
            XmlDocument dataScriptsDoc = new XmlDocument();
            dataScriptsDoc.Load(ConfigurationManager.AppSettings["DataScriptsDefinitionFile"]);

            OnSuccessfulUpdateEntry onSuccessfulUpdateEntry = delegate(UpdateEntry updateEntry)
            {
                int databaseVersion = updateEntry.VersionNumber;
                Console.WriteLine("Version number: " + updateEntry.Version + " applied successfully.");

                // apply any additional data scripts after applying version
                XmlNodeList dataScriptsNodeList =
                    dataScriptsDoc.SelectNodes("/SqlDataScripts/SqlDataScript[@runAfterVersion=" +
                                               updateEntry.VersionNumber + "]");
                foreach (XmlNode node in dataScriptsNodeList)
                {
                    Console.WriteLine("Running data script " + node.Attributes["fileName"].Value +
                                      " after version " + updateEntry.VersionNumber);
                    RunDataScript(node.Attributes["fileName"].Value);
                }

                //Mark this update as done
                Console.WriteLine("Sucessfully applied version: " + updateEntry.Version);
                Console.WriteLine(
                    string.Format("DatabaseVersion: {0} / {1}", databaseVersion, Database.DatabaseVersion));

            };
            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SimulateRequest();

                string mapPath = ConfigurationManager.AppSettings["RainbowWebApplicationRoot"];
                DatabaseUpdater updater = new DatabaseUpdater(mapPath + @"Setup\Scripts\", mapPath);
                updater.OnSuccessfulUpdateEntry = onSuccessfulUpdateEntry;
                updater.PreviewUpdate();
                updater.PerformUpdate();
                //Assert.AreEqual(0, updater.Errors.Count);
            }
  */
        }

//        static int DatabaseVersion
//        {
//            get
//            {
//                //Clear version cache so we are sure we update correctly
//                HttpContext.Current.Application.Lock();
//                HttpContext.Current.Application[Database.dbKey] = null;
//                HttpContext.Current.Application.UnLock();
//                return Database.DatabaseVersion;
//            }
//        }

        static void InitializeScriptList() 
        {
            XmlDocument myDoc = new XmlDocument();
            ArrayList tempScriptsList = new ArrayList();

            // load the history file
            string myDocPath = ConfigurationManager.AppSettings["RainbowWebApplicationRoot"] + @"\Setup\Scripts\History.xml"; 
            myDoc.Load( myDocPath );

            // get a list of <Release> nodes
            XmlNodeList releases = myDoc.DocumentElement.SelectNodes( "Release" );

            foreach ( XmlNode release in releases ) {
                UpdateEntry myUpdate = new UpdateEntry();

                // get the header information
                // we check for null to avoid exception if any of these nodes are not present
                if ( release.SelectSingleNode( "ID" ) != null ) {
                    myUpdate.VersionNumber = Int32.Parse( release.SelectSingleNode( "ID/text()" ).Value );
                }

                if ( release.SelectSingleNode( "Version" ) != null ) {
                    myUpdate.Version = release.SelectSingleNode( "Version/text()" ).Value;
                }

                if ( release.SelectSingleNode( "Script" ) != null ) {
                    myUpdate.scriptNames.Add( release.SelectSingleNode( "Script/text()" ).Value );
                }

                if ( release.SelectSingleNode( "Date" ) != null ) {
                    myUpdate.Date = DateTime.Parse( release.SelectSingleNode( "Date/text()" ).Value );
                }
                int dbVersion = 0;//DatabaseVersion;
                //We should apply this patch
                if ( dbVersion < myUpdate.VersionNumber ) {
                    //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected version to apply: " + myUpdate.Version);

                    if (dbVersion > 1114 && dbVersion < 1519)
                    {
                        continue;
//                        ErrorHandler.Publish(LogLevel.Error, "Unsupported version " + dbVersion + " detected.");
//                        throw new Exception("Version before 1519 are not supported anymore. Please upgrade to a newer version or upgrade manually.");
                    }

                    if (dbVersion < 1114)
                        Console.WriteLine("Empty/New database - CodeVersion: " + Portal.CodeVersion);
                    else
                        Console.WriteLine("dbVersion: " + dbVersion + " - CodeVersion: " +
                                          Portal.CodeVersion); ;

                    myUpdate.Apply = true;

                    // get a list of <Installer> nodes
                    XmlNodeList installers = release.SelectNodes( "Modules/Installer/text()" );

                    // iterate over the <Installer> Nodes (in original document order)
                    // (we can do this because XmlNodeList implements IEnumerable)
                    foreach ( XmlNode installer in installers ) {
                        //and build an ArrayList of the scripts... 
                        myUpdate.Modules.Add( installer.Value );
                        //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected module to install: " + installer.Value);
                    }

                    // get a <Script> node, if any
                    XmlNodeList sqlScripts = release.SelectNodes( "Scripts/Script/text()" );

                    // iterate over the <Installer> Nodes (in original document order)
                    // (we can do this because XmlNodeList implements IEnumerable)
                    foreach ( XmlNode sqlScript in sqlScripts ) {
                        //and build an ArrayList of the scripts... 
                        myUpdate.scriptNames.Add( sqlScript.Value );
                        //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Debug, "Detected script to run: " + sqlScript.Value);
                    }
                    tempScriptsList.Add( myUpdate );

                }
            }

            //If we have some version to apply...
            if ( tempScriptsList.Count > 0 ) {
                scriptsList = ( UpdateEntry[] )tempScriptsList.ToArray( typeof( UpdateEntry ) );

                //by Manu. Versions are sorted by version number
                Array.Sort( scriptsList );

                //Create a flat version for binding
                int currentVersion = 0;
                foreach ( UpdateEntry myUpdate in scriptsList ) {
                    if ( myUpdate.Apply ) {
                        if ( currentVersion != myUpdate.VersionNumber ) {
                            LogHelper.Logger.Log( LogLevel.Debug, "Version: " + myUpdate.VersionNumber );
                            currentVersion = myUpdate.VersionNumber;
                        }

                        foreach ( string scriptName in myUpdate.scriptNames ) {
                            if ( scriptName.Length > 0 ) {
                                LogHelper.Logger.Log( LogLevel.Debug, "-- Script: " + scriptName );
                            }
                        }

                        foreach ( string moduleInstaller in myUpdate.Modules ) {
                            if ( moduleInstaller.Length > 0 )
                                LogHelper.Logger.Log( LogLevel.Debug, "-- Module: " + moduleInstaller + " (ignored recreating test DB)" );
                        }
                    }
                }
            }
        }

        private static void RunScriptList() {

            XmlDocument dataScriptsDoc = new XmlDocument();
            dataScriptsDoc.Load( ConfigurationManager.AppSettings["DataScriptsDefinitionFile"] );

            int databaseVersion = 0;
            ArrayList errors = new ArrayList();

            foreach (UpdateEntry myUpdate in scriptsList)
            {
                using (HttpSimulator simulator = new HttpSimulator())
                {
                    simulator.SimulateRequest(new Uri("http://localhost/Setup/Update.aspx"));

                    //Version check (a script may update more than one version at once)
                    if (myUpdate.Apply && databaseVersion < myUpdate.VersionNumber)
                    {

                        foreach (string scriptName in myUpdate.scriptNames)
                        {
                            //It may be a module update only
                            if (scriptName.Length > 0)
                            {
                                string currentScriptName = System.IO.Path.Combine(
                                    ConfigurationManager.AppSettings["RainbowWebApplicationRoot"] + @"\Setup\Scripts\", scriptName);
                                Console.WriteLine("DB: " + databaseVersion + " - CURR: " +
                                                  myUpdate.VersionNumber + " - Applying: " + currentScriptName);

                                RunRainbowScript(currentScriptName);
                            }
                        }

                        if (Equals(errors.Count, 0))
                        {
                            //Update db with version
//                            onSuccessfulUpdateEntry(myUpdate);
                            databaseVersion = myUpdate.VersionNumber;
                            Console.WriteLine("Version number: " + myUpdate.Version + " applied successfully.");

                            // apply any additional data scripts after applying version
                            XmlNodeList dataScriptsNodeList =
                                dataScriptsDoc.SelectNodes("/SqlDataScripts/SqlDataScript[@runAfterVersion=" +
                                                           myUpdate.VersionNumber + "]");
                            foreach (XmlNode node in dataScriptsNodeList)
                            {
                                Console.WriteLine("Running data script " + node.Attributes["fileName"].Value +
                                                  " after version " + myUpdate.VersionNumber);
                                RunDataScript(node.Attributes["fileName"].Value);
                            }

                            //Mark this update as done
                            Console.WriteLine("Sucessfully applied version: " + myUpdate.Version);
                            Console.WriteLine(
                                string.Format("DatabaseVersion: {0} / {1}", databaseVersion, Database.DatabaseVersion));
                        }
                    }
                    else
                    {
                        Console.WriteLine("DB: " + databaseVersion + " - CURR: " +
                                          myUpdate.VersionNumber + " - Skipping: " + myUpdate.Version);
                    }
                }
            }
        }
  
        public static void RunRainbowScript( string strRelativeScriptPath ) {
            //1 - prepend full root
            /*NOTE: we can't simply get the Executing Path of the calling assembly
				* because that will vary if the assembly is shadow-copied by NUnit or 
				* other testing tools. Therefore, for simplicity in this demo, we just
				* get it as a App key.
				*/

            string strUTRoot = ConfigurationManager.AppSettings[ "RainbowSetupScriptsPath" ];
            string strFullScriptPath = strUTRoot + strRelativeScriptPath;
            _RunScript( strFullScriptPath );
        }

        public static void RunDataScript( string strRelativeScriptPath ) {
            //1 - prepend full root
            /*NOTE: we can't simply get the Executing Path of the calling assembly
				* because that will vary if the assembly is shadow-copied by NUnit or 
				* other testing tools. Therefore, for simplicity in this demo, we just
				* get it as a App key.
				*/

            string strUTRoot = ConfigurationManager.AppSettings["DataScriptsPath"];
            string strFullScriptPath = strUTRoot + strRelativeScriptPath;
            _RunScript( strFullScriptPath );
        }

        static void _RunScript( string strFullScriptPath ) {
            SqlConnection conn = new SqlConnection( ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString );
            DBHelper.ExecuteScript( strFullScriptPath, conn );
            Console.WriteLine(  "Ran script " + strFullScriptPath );
        }
    }

    internal struct SqlAdmin 
    {
        public SqlAdmin(string userId, string password, string server, string database)
        {
            this.userId = userId;
            this.password = password;
            this.server = server;
            this.database = database;
        }

        readonly string userId;
        readonly string password;
        readonly string server;
        readonly string database;

        public string UserId
        {
            get { return userId; }
        }

        public string Password
        {
            get { return password; }
        }

        public string Server
        {
            get { return server; }
        }

        public string Database
        {
            get { return database; }
        }
    }
}
