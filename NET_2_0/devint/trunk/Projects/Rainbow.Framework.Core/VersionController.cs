using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Web;
using Rainbow.Framework.Context;
using Rainbow.Framework.Data;
using Rainbow.Framework.Exceptions;

namespace Rainbow.Framework.Core
{
    ///<summary>
    /// Encapsulates version related members
    ///</summary>
    public class VersionController
    {
        public static VersionController Instance = new VersionController();

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        public int DatabaseVersion
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
        public int CodeVersion
        {
            get
            {
                HttpContext httpContext = HttpContext.Current; //TODO: use RainbowContext here
                const string codeVersionParameterName = "CodeVersion";
                if (httpContext != null)
                {
                    if (httpContext.Application[codeVersionParameterName] == null)
                    {
                        FileVersionInfo f =
                            FileVersionInfo.GetVersionInfo(
                                Assembly.GetAssembly(typeof (RainbowContext)).Location);
                        httpContext.Application.Lock();
                        httpContext.Application[codeVersionParameterName] = f.FilePrivatePart;
                        httpContext.Application.UnLock();
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
        public string ProductVersion
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
        internal int DatabaseVersionWithCacheReset
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
                    dbKey = "DatabaseVersion_" + DBHelper.SqlConnection.DataSource + "_" +
                            DBHelper.SqlConnection.Database; // For multidb support
                }
                return dbKey;
            }
        }

        ///<summary>
        /// 3rd Check: is database/code version correct?
        ///</summary>
        public bool CheckDatabaseVersion(string databaseUpdateRedirectRelativePath)
        {
            int versionDelta = DatabaseVersion.CompareTo(CodeVersion);
            HttpContext httpContext = RainbowContext.Current.HttpContext;
            // if DB and code versions do not match
            if (versionDelta != 0)
            {
                Uri requestUri = httpContext.Request.Url;
                string databaseUpdateRedirect = databaseUpdateRedirectRelativePath;
                if (databaseUpdateRedirect.StartsWith("~/"))
                {
                    databaseUpdateRedirect = databaseUpdateRedirect.TrimStart(new char[] { '~' });
                }

                if (
                    !
                    requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture).EndsWith(
                        databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                {
                    // ...and this is not DB Update page
                    string errorMessage = string.Format("Database version: {0} Code version: {1}",
                        DatabaseVersion, CodeVersion);
                    if (versionDelta < 0) // DB Version is behind Code Version
                    {
                        ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                        // Jonathan : WHy wouldnt we redirect to update page?
                        // TODO : Check with people why this was like this....
                        httpContext.Response.Redirect(Path.ApplicationRoot + databaseUpdateRedirect, true);
                        // throw new DatabaseVersionException(errorMessage);
                    }
                    else // DB version is ahead of Code Version
                    {
                        ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                        // Jonathan : WHy wouldnt we redirect to update page?
                        // TODO : Check with people why this was like this....
                        // Who cares ?
                        // throw new CodeVersionException(errorMessage);
                    }
                }
                else // this is already DB Update page... 
                {
                    return false; // so skip creation of PortalSettings
                }
            }
            return true;
        }

    }
}
