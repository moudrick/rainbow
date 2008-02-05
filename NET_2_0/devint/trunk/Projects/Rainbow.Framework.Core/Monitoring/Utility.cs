using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Rainbow.Framework.Data;

namespace Rainbow.Framework.Monitoring
{
    /// <summary>
    /// Utility Helper class for Rainbow Framework Monitoring purposes.
    /// You get some static methods like
    /// <list type="string">
    /// <item>int GetTotalPortalHits(int portalID)</item>
    /// <item>DataSet GetMonitoringStats(DateTime startDate</item>
    /// </list>
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Return a dataset of stats for a given data range and portal
        /// Written by Paul Yarrow, paul@paulyarrow.com
        /// </summary>
        /// <param name="startDate">the first date you want to see stats from</param>
        /// <param name="endDate">the last date you want to see stats up to</param>
        /// <param name="reportType">type of report you are requesting</param>
        /// <param name="currentTabID">page id you are requesting stats for</param>
        /// <param name="includeMonitoringPage">include the monitoring page in the stats</param>
        /// <param name="includeAdminUser">include hits by admin users</param>
        /// <param name="includePageRequests">include page hits</param>
        /// <param name="includeLogon">include the logon page</param>
        /// <param name="includeLogoff">inlcude logogg page</param>
        /// <param name="includeMyIPAddress">include the current ip address</param>
        /// <param name="portalID">portal id to get stats for</param>
        /// <returns></returns>
        //TODO: [moudrick] change return type to a Monitoring Items collection
        //TODO: [moudrick] then move these methods to Monitoring Provider
        public static DataSet GetMonitoringStats(DateTime startDate,
                                                 DateTime endDate,
                                                 string reportType,
                                                 long currentTabID,
                                                 bool includeMonitoringPage,
                                                 bool includeAdminUser,
                                                 bool includePageRequests,
                                                 bool includeLogon,
                                                 bool includeLogoff,
                                                 bool includeMyIPAddress,
                                                 int portalID)
        {
            endDate = endDate.AddDays(1);

            // Firstly get the logged in users
            SqlConnection myConnection = DBHelper.SqlConnection;
            SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetMonitoringEntries", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.SelectCommand.Parameters.Add(parameterPortalID);

            SqlParameter parameterStartDate = new SqlParameter("@StartDate", SqlDbType.DateTime, 8);
            parameterStartDate.Value = startDate;
            myCommand.SelectCommand.Parameters.Add(parameterStartDate);

            SqlParameter parameterEndDate = new SqlParameter("@EndDate", SqlDbType.DateTime, 8);
            parameterEndDate.Value = endDate;
            myCommand.SelectCommand.Parameters.Add(parameterEndDate);

            SqlParameter parameterReportType = new SqlParameter("@ReportType", SqlDbType.VarChar, 50);
            parameterReportType.Value = reportType;
            myCommand.SelectCommand.Parameters.Add(parameterReportType);

            SqlParameter parameterCurrentTabID = new SqlParameter("@CurrentTabID", SqlDbType.BigInt, 8);
            parameterCurrentTabID.Value = currentTabID;
            myCommand.SelectCommand.Parameters.Add(parameterCurrentTabID);

            SqlParameter parameterIncludeMoni = new SqlParameter("@IncludeMonitorPage", SqlDbType.Bit, 1);
            parameterIncludeMoni.Value = includeMonitoringPage;
            myCommand.SelectCommand.Parameters.Add(parameterIncludeMoni);

            SqlParameter parameterIncludeAdmin = new SqlParameter("@IncludeAdminUser", SqlDbType.Bit, 1);
            parameterIncludeAdmin.Value = includeAdminUser;
            myCommand.SelectCommand.Parameters.Add(parameterIncludeAdmin);

            SqlParameter parameterIncludePageRequests = new SqlParameter("@IncludePageRequests", SqlDbType.Bit, 1);
            parameterIncludePageRequests.Value = includePageRequests;
            myCommand.SelectCommand.Parameters.Add(parameterIncludePageRequests);

            SqlParameter parameterIncludeLogon = new SqlParameter("@IncludeLogon", SqlDbType.Bit, 1);
            parameterIncludeLogon.Value = includeLogon;
            myCommand.SelectCommand.Parameters.Add(parameterIncludeLogon);

            SqlParameter parameterIncludeLogoff = new SqlParameter("@IncludeLogoff", SqlDbType.Bit, 1);
            parameterIncludeLogoff.Value = includeLogoff;
            myCommand.SelectCommand.Parameters.Add(parameterIncludeLogoff);

            SqlParameter parameterIncludeIPAddress = new SqlParameter("@IncludeIPAddress", SqlDbType.Bit, 1);
            parameterIncludeIPAddress.Value = includeMyIPAddress;
            myCommand.SelectCommand.Parameters.Add(parameterIncludeIPAddress);

            SqlParameter parameterIPAddress = new SqlParameter("@IPAddress", SqlDbType.VarChar, 16);
            parameterIPAddress.Value = HttpContext.Current.Request.UserHostAddress;
            myCommand.SelectCommand.Parameters.Add(parameterIPAddress);

            // Create and Fill the DataSet
            DataSet myDataSet = new DataSet();
            try
            {
                myCommand.Fill(myDataSet);
            }
            finally
            {
                myConnection.Close();
            }

            // Return the DataSet
            return myDataSet;
        }
    }
}
