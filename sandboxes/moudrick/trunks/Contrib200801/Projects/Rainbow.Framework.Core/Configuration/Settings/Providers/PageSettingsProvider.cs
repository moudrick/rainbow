using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Rainbow.Framework.Data;
using Rainbow.Framework.Site.Configuration;

namespace Rainbow.Framework.Core.Configuration.Settings.Providers
{
    ///<summary>
    /// This is interface class for get module settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public class PageSettingsProvider
    {
        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hashtable of
        /// custom Page specific settings from the database.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static Hashtable GetPageCustomSettings(int pageID)
        {
            // Get Settings for this Page from the database
            Hashtable _settings = new Hashtable();

            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabCustomSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterPageID = new SqlParameter("@TabID", SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);

                    myConnection.Open();
                    SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    try
                    {
                        while (dr.Read())
                        {
                            _settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                    finally
                    {
                        dr.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
            return _settings;
        }

        /// <summary>
        /// Read Current Page subtabs
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>PagesBox</returns>
        public static PagesBox GetPageSettingsPagesBox(int pageID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    //PageID passed type FIXED by Bill Anderson (reedtek)
                    //see: http://sourceforge.net/tracker/index.php?func=detail&aid=813789&group_id=66837&atid=515929
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter("@PageID", SqlDbType.Int);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    // The new paramater "PortalLanguage" has been added to sp rb_GetPageSettings  
                    // Onur Esnaf
                    SqlParameter parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12);
                    parameterPortalLanguage.Value = Thread.CurrentThread.CurrentUICulture.Name;
                    myCommand.Parameters.Add(parameterPortalLanguage);
                    // Open the database connection and execute the command
                    myConnection.Open();

                    using (SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        PagesBox tabs = new PagesBox();
                        try
                        {
                            while (result.Read())
                            {
                                PageStripDetails tabDetails = new PageStripDetails();
                                tabDetails.PageID = (int)result["PageID"];
                                Hashtable cts = new PageSettings().GetPageCustomSettings(tabDetails.PageID);
                                tabDetails.PageImage = cts["CustomMenuImage"].ToString();
                                tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                                tabDetails.PageName = (string)result["PageName"];
                                tabDetails.PageOrder = (int)result["PageOrder"];
                                tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                                tabs.Add(tabDetails);
                            }
                        }
                        finally
                        {
                            result.Close(); //by Manu, fixed bug 807858
                        }
                        return tabs;
                    }
                }
            }
        }
        
        /// <summary>
        /// Update Page Custom Settings
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdatePageSettings(int pageID, string key, string value)
        {
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateTabCustomSettings", myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterPageID = new SqlParameter("@TabID", SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                    parameterKey.Value = key;
                    myCommand.Parameters.Add(parameterKey);
                    SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
                    parameterValue.Value = value;
                    myCommand.Parameters.Add(parameterValue);
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
    }
}
