using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Rainbow.Framework.Data.MsSql;
using Rainbow.Framework.Exceptions;

namespace Rainbow.Framework.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Gets the db key.
        /// </summary>
        /// <value>The db key.</value>
        public static string dbKey
        {
            get
            {
                string dbKey = "CurrentDatabase";
                DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);

                if (Config.EnableMultiDbSupport)
                    dbKey = "DatabaseVersion_" + db.Connection.DataSource + "_" +
                            db.Connection.Database; // For multidb support
                
                return dbKey;
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
                DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
                //Caches dbversion
                int curVersion = 0;

                if (HttpContext.Current.Application[dbKey] == null)
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
                        db.ExecuteCommand(createRbVersions);
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

                    var versions = from v in db.rb_Versions orderby v.Release descending select v;
                    curVersion = versions.First().Release;

                    if (curVersion == 0)
                    {
                        curVersion = 1110;
                        // TODO: This should be the best place
                        // where run the code for empty db
                    }
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[dbKey] = curVersion;
                    HttpContext.Current.Application.UnLock();
                }
                return (int)HttpContext.Current.Application[dbKey];
            }
        }
    }
}