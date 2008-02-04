using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using Rainbow.Framework.Context;
using Rainbow.Framework.Data;

namespace Rainbow.Framework.Providers.MsSql
{
    ///<summary>
    /// Monitoring provider implementation for MS SQL databases 
    ///</summary>
    public class MsSqlMonitoringProvider : MonitoringProvider
    {
        /// <summary>
        /// Logs the entry.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="userField">The user field.</param>
        public override void LogEntry(Guid userID,
                             int portalID,
                             long pageID,
                             string actionType,
                             string userField)
        {
            // note by manu: This exception is already managed at higher level
            // a nested try catch slows down with no real use
            //return;

            // if a tab id of 0 is received, this is the home page
            // so change the number to 1
            // A page ID of -1 is sent when logging in and out
            if (pageID == 0)
            {
                pageID = 1;
            }

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_AddMonitoringEntry", myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterUsername = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
                    parameterUsername.Value = userID;
                    myCommand.Parameters.Add(parameterUsername);

                    SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);

                    SqlParameter parameterPageID = new SqlParameter("@PageID", SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);

                    SqlParameter parameterActionType = new SqlParameter("@ActivityType", SqlDbType.NVarChar, 50);
                    parameterActionType.Value = actionType;
                    myCommand.Parameters.Add(parameterActionType);

                    SqlParameter parameterUserField = new SqlParameter("@UserField", SqlDbType.NVarChar, 500);
                    parameterUserField.Value = userField;
                    myCommand.Parameters.Add(parameterUserField);

                    // Create the web parameters and set them to defaults.
                    // If we are in the context of a web request then
                    // record the extra information we can get
                    SqlParameter parameterUrlReferrer = new SqlParameter("@Referrer", SqlDbType.NVarChar, 255);
                    parameterUrlReferrer.Value = string.Empty;
                    myCommand.Parameters.Add(parameterUrlReferrer);

                    SqlParameter parameterUserAgent = new SqlParameter("@UserAgent", SqlDbType.NVarChar, 100);
                    parameterUserAgent.Value = string.Empty;
                    myCommand.Parameters.Add(parameterUserAgent);

                    SqlParameter parameterUserHostAddress = new SqlParameter("@UserHostAddress", SqlDbType.NVarChar, 15);
                    parameterUserHostAddress.Value = string.Empty;
                    myCommand.Parameters.Add(parameterUserHostAddress);

                    SqlParameter parameterBrowserType = new SqlParameter("@BrowserType", SqlDbType.NVarChar, 100);
                    parameterBrowserType.Value = string.Empty;
                    myCommand.Parameters.Add(parameterBrowserType);

                    SqlParameter parameterBrowserName = new SqlParameter("@BrowserName", SqlDbType.NVarChar, 100);
                    parameterBrowserName.Value = string.Empty;
                    myCommand.Parameters.Add(parameterBrowserName);

                    SqlParameter parameterBrowserVersion = new SqlParameter("@BrowserVersion", SqlDbType.NVarChar, 100);
                    parameterBrowserVersion.Value = string.Empty;
                    myCommand.Parameters.Add(parameterBrowserVersion);

                    SqlParameter parameterBrowserPlatform =
                        new SqlParameter("@BrowserPlatform", SqlDbType.NVarChar, 100);
                    parameterBrowserPlatform.Value = string.Empty;
                    myCommand.Parameters.Add(parameterBrowserPlatform);

                    SqlParameter parameterBrowserIsAOL = new SqlParameter("@BrowserIsAOL", SqlDbType.Bit, 1);
                    parameterBrowserIsAOL.Value = false;
                    myCommand.Parameters.Add(parameterBrowserIsAOL);


                    // Add the browser info if we have access
                    if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    {
                        if (HttpContext.Current.Request.UrlReferrer != null)
                        {
                            parameterUrlReferrer.Value = HttpContext.Current.Request.UrlReferrer.ToString();
                        }
                        // 09_09_2003 Cory Isakson
                        // Some browsers are not sending a UserAgent header
                        if (HttpContext.Current.Request.UserAgent != null)
                        {
                            parameterUserAgent.Value = HttpContext.Current.Request.UserAgent;
                        }

                        parameterUserHostAddress.Value = HttpContext.Current.Request.UserHostAddress;
                        parameterBrowserType.Value = HttpContext.Current.Request.Browser.Type;
                        parameterBrowserName.Value = HttpContext.Current.Request.Browser.Browser;
                        parameterBrowserVersion.Value = HttpContext.Current.Request.Browser.Version;
                        parameterBrowserPlatform.Value = HttpContext.Current.Request.Browser.Platform;
                        parameterBrowserIsAOL.Value = HttpContext.Current.Request.Browser.AOL;
                    }

                    // Open the database connection and execute SQL Command
                    myConnection.Open();
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// returns the total hit count for a portal
        /// </summary>
        /// <param name="portalID">portal id to get stats for</param>
        /// <returns>
        /// total number of hits to the portal of all types
        /// </returns>
        public override int GetTotalPortalHits(int portalID)
        {
            // TODO: THis should not display, login, logout, or administrator hits.
            string sql = "Select count(ID) as hits " +
                         " from rb_monitoring " +
                         " where [PortalID] = " + portalID + " ";

            return Convert.ToInt32(DBHelper.ExecuteSQLScalar(sql));
        }

        /// <summary>
        /// Get Users Online
        /// Add to the Cache
        /// HttpContext.Current.Cache.Insert("WhoIsOnlineAnonUserCount", anonUserCount, null, DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
        /// HttpContext.Current.Cache.Insert("WhoIsOnlineRegUserCount", regUsersOnlineCount, null, DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
        /// HttpContext.Current.Cache.Insert("WhoIsOnlineRegUsersString", regUsersString, null, DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="minutesToCheckForUsers">The minutes to check for users.</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="anonUserCount">The anon user count.</param>
        /// <param name="regUsersOnlineCount">The reg users online count.</param>
        /// <param name="regUsersString">The reg users string.</param>
        public override void FillUsersOnlineCache(int portalID,
                                                int minutesToCheckForUsers,
                                                int cacheTimeout,
                                                out int anonUserCount,
                                                out int regUsersOnlineCount,
                                                out string regUsersString)
        {


            // Read from the cache if available
            if (HttpContext.Current.Cache["WhoIsOnlineAnonUserCount"] == null ||
                HttpContext.Current.Cache["WhoIsOnlineRegUserCount"] == null ||
                HttpContext.Current.Cache["WhoIsOnlineRegUsersString"] == null)
            {
                // Firstly get the logged in users
                SqlCommand sqlComm1 = new SqlCommand();
                SqlConnection sqlConn1 = new SqlConnection(Config.ConnectionString);
                sqlComm1.Connection = sqlConn1;
                sqlComm1.CommandType = CommandType.StoredProcedure;
                sqlComm1.CommandText = "rb_GetLoggedOnUsers";
                SqlDataReader result;

                // Add Parameters to SPROC
                SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
                parameterPortalID.Value = portalID;
                sqlComm1.Parameters.Add(parameterPortalID);

                SqlParameter parameterMinutesToCheck = new SqlParameter("@MinutesToCheck", SqlDbType.Int, 4);
                parameterMinutesToCheck.Value = minutesToCheckForUsers;
                sqlComm1.Parameters.Add(parameterMinutesToCheck);

                sqlConn1.Open();
                result = sqlComm1.ExecuteReader();

                string onlineUsers = string.Empty;
                int onlineUsersCount = 0;
                try
                {
                    while (result.Read())
                    {
                        if (Convert.ToString(result.GetValue(2)) != "Logoff")
                        {
                            onlineUsersCount++;
                            onlineUsers += Membership.GetUser(result.GetValue(1)).UserName + ", ";
                        }
                    }
                }
                finally
                {
                    result.Close(); //by Manu, fixed bug 807858
                }

                if (onlineUsers.Length > 0)
                {
                    onlineUsers = onlineUsers.Remove(onlineUsers.Length - 2, 2);
                }

                regUsersString = onlineUsers;
                regUsersOnlineCount = onlineUsersCount;

                result.Close();


                // Add Parameters to SPROC
                SqlParameter parameterNumberOfUsers = new SqlParameter("@NoOfUsers", SqlDbType.Int, 4);
                parameterNumberOfUsers.Direction = ParameterDirection.Output;
                sqlComm1.Parameters.Add(parameterNumberOfUsers);

                // Re-use the same result set to get the no of unregistered users
                sqlComm1.CommandText = "rb_GetNumberOfActiveUsers";

                // [The Bitland Prince] 8-1-2005
                // If this query generates an exception, connection might be left open
                try
                {
                    sqlComm1.ExecuteNonQuery();
                }
                catch (Exception Ex)
                {
                    // This takes care to close connection then throws a new
                    // exception (because I don't know if it's safe to go on...)
                    sqlConn1.Close();
                    throw new Exception("Unable to retrieve logged users. Error : " + Ex.Message);
                }

                int allUsersCount = Convert.ToInt32(parameterNumberOfUsers.Value);

                sqlConn1.Close();

                anonUserCount = allUsersCount - onlineUsersCount;
                if (anonUserCount < 0)
                {
                    anonUserCount = 0;
                }

                // Add to the Cache
                HttpContext.Current.Cache.Insert("WhoIsOnlineAnonUserCount", anonUserCount, null,
                                                 DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
                HttpContext.Current.Cache.Insert("WhoIsOnlineRegUserCount", regUsersOnlineCount, null,
                                                 DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
                HttpContext.Current.Cache.Insert("WhoIsOnlineRegUsersString", regUsersString, null,
                                                 DateTime.Now.AddMinutes(cacheTimeout), TimeSpan.Zero);
            }
            else
            {
                anonUserCount = (int)HttpContext.Current.Cache["WhoIsOnlineAnonUserCount"];
                regUsersOnlineCount = (int)HttpContext.Current.Cache["WhoIsOnlineRegUserCount"];
                regUsersString = (string)HttpContext.Current.Cache["WhoIsOnlineRegUsersString"];
            }
        }

    }
}
