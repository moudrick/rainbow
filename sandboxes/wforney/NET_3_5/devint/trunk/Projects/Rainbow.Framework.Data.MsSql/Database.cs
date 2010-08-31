namespace Rainbow.Framework.Data.MsSql
{
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Data.MsSql.Debugger;
    using Rainbow.Framework.Exceptions;

    /// <summary>
    /// The Database
    /// </summary>
    public partial class Database
    {
        /// <summary>
        /// Gets the db key.
        /// </summary>
        /// <value>The db key.</value>
        public static string dbKey
        {
            get
            {
                var databaseKey = "CurrentDatabase";
                using (var db = new DataClassesDataContext(Config.ConnectionString))
                {
                    db.Log = new DebuggerWriter();
                    if (Config.EnableMultiDBSupport)
                    {
                        // For multidb support
                        databaseKey = string.Format(
                            "DatabaseVersion_{0}_{1}", db.Connection.DataSource, db.Connection.Database);
                    }
                }

                return databaseKey;
            }
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        public static int DatabaseVersion
        {
            // by Manu 16/10/2003
            // Added 2 mods:
            // 1) Rbversion is created if it is missed.
            //    This is expecially good for empty databases.
            //    Be aware that this can break compatibility with 1613 version
            // 2) Connection problems are thown immediately as errors.
            get
            {
                using (var db = new DataClassesDataContext(Config.ConnectionString))
                {
                    db.Log = new DebuggerWriter();
                    if (HttpContext.Current.Application[dbKey] == null)
                    {
                        try
                        {
                            // Create rbversion if it is missing
                            const string CreateRbVersions = "IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rb_Versions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
                                                            "CREATE TABLE [rb_Versions] (" +
                                                            "[Release] [int] NOT NULL , " +
                                                            "[Version] [nvarchar] (50) NULL , " +
                                                            "[ReleaseDate] [datetime] NULL " +
                                                            ") ON [PRIMARY]";
                            db.ExecuteCommand(CreateRbVersions);
                        }
                        catch (SqlException ex)
                        {
                            throw new DatabaseUnreachableException(
                                "Failed to get Database Version - most likely cannot connect to db or no permission.", ex);
                            
                            // Jes1111
                            // Rainbow.Framework.Configuration.ErrorHandler.HandleException("If this fails most likely cannot connect to db or no permission", ex);
                            // If this fails most likely cannot connect to db or no permission
                            // throw;
                        }


                        var versions = from v in db.Versions orderby v.Release descending select v;

                        // TODO: This should be the best place
                        // where run the code for empty db

                        // Caches dbversion
                        int curVersion = versions.Count() > 0 ? versions.First().Release : 1110;

                        HttpContext.Current.Application.Lock();
                        HttpContext.Current.Application[dbKey] = curVersion;
                        HttpContext.Current.Application.UnLock();
                    }

                    return (int)HttpContext.Current.Application[dbKey];
                }
            }
        }
    }
}