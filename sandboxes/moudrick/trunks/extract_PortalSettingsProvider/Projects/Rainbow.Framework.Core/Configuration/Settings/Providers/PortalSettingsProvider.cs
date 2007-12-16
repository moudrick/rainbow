using System;
using System.Collections;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Site.Configuration;

namespace Rainbow.Framework.Core.Configuration.Settings.Providers
{
    ///<summary>
    /// This is interface class for get portal settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public class PortalSettingsProvider //: ProviderBase
    {
        const string strATPortalID = "@PortalID";
        const string strATPageID = "@PageID";

        public static readonly PortalSettingsProvider Instance = new PortalSettingsProvider();

        /// <summary>
        /// Gets portal custom settings from persistence
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public Hashtable GetPortalCustomSettings(int portalID)
        {
            Hashtable settings = new Hashtable();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalCustomSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterportalID.Value = portalID;
                    myCommand.Parameters.Add(parameterportalID);
                    // Execute the command
                    myConnection.Open();
                    SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    try
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                    finally
                    {
                        dr.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// Fills full portal settings from persistence for use
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="pageID"></param>
        /// <param name="portalAlias"></param>
        public static void FillPortalSettingsFull(PortalSettings settings, int pageID, string portalAlias)
        {
            settings.CurrentLayout = "Default";
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                    myCommand.Parameters.Add(parameterPortalAlias);

                    SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);

                    SqlParameter parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12);
                    parameterPortalLanguage.Value = settings.PortalContentLanguage.Name;
                    myCommand.Parameters.Add(parameterPortalLanguage);

                    // Add out parameters to Sproc
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalID);

                    SqlParameter parameterPortalName = new SqlParameter("@PortalName", SqlDbType.NVarChar, 128);
                    parameterPortalName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalName);

                    SqlParameter parameterPortalPath = new SqlParameter("@PortalPath", SqlDbType.NVarChar, 128);
                    parameterPortalPath.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalPath);

                    SqlParameter parameterEditButton = new SqlParameter("@AlwaysShowEditButton", SqlDbType.Bit, 1);
                    parameterEditButton.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterEditButton);

                    SqlParameter parameterPageName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50);
                    parameterPageName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageName);

                    SqlParameter parameterPageOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterPageOrder.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageOrder);

                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterParentPageID);

                    SqlParameter parameterMobilePageName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobilePageName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterMobilePageName);

                    SqlParameter parameterAuthRoles = new SqlParameter("@AuthRoles", SqlDbType.NVarChar, 256);
                    parameterAuthRoles.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterAuthRoles);

                    SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterShowMobile);

                    SqlDataReader result;
                    try
                    {
                        // Open the database connection and execute the command
                        //						try // jes1111
                        //						{
                        myConnection.Open();
                        result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                        // Read the first resultset -- Desktop Page Information
                        while (result.Read())
                        {
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int)result["PageID"];
                            tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                            tabDetails.PageName = (string)result["PageName"];
                            tabDetails.PageOrder = (int)result["PageOrder"];
                            tabDetails.PageLayout = settings.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            settings.PortalAlias = portalAlias;
                            // Update the AuthorizedRoles Variable
                            settings.DesktopPages.Add(tabDetails);
                        }

                        if (settings.DesktopPages.Count == 0)
                        {
                            return; //Abort load
                            //throw new Exception("The portal you requested has no Pages. PortalAlias: '" + portalAlias + "'", new HttpException(404, "Portal not found"));
                        }
                        // Read the second result --  Mobile Page Information
                        result.NextResult();

                        while (result.Read())
                        {
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int)result["PageID"];
                            tabDetails.PageName = (string)result["MobilePageName"];
                            tabDetails.PageLayout = settings.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            settings.MobilePages.Add(tabDetails);
                        }
                        // Read the third result --  Module Page Information
                        result.NextResult();

                        while (result.Read())
                        {
                            ModuleSettings m = new ModuleSettings();
                            m.ModuleID = (int)result["ModuleID"];
                            m.ModuleDefID = (int)result["ModuleDefID"];
                            m.GuidID = (Guid)result["GeneralModDefID"];
                            m.PageID = (int)result["TabID"];
                            m.PaneName = (string)result["PaneName"];
                            m.ModuleTitle = (string)result["ModuleTitle"];
                            object myValue = result["AuthorizedEditRoles"];
                            m.AuthorizedEditRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedViewRoles"];
                            m.AuthorizedViewRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedAddRoles"];
                            m.AuthorizedAddRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedDeleteRoles"];
                            m.AuthorizedDeleteRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedPropertiesRoles"];
                            m.AuthorizedPropertiesRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            // jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
                            myValue = result["AuthorizedMoveModuleRoles"];
                            m.AuthorizedMoveModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedDeleteModuleRoles"];
                            m.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            // Change by Geert.Audenaert@Syntegra.Com
                            // Date: 6/2/2003
                            myValue = result["AuthorizedPublishingRoles"];
                            m.AuthorizedPublishingRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["SupportWorkflow"];
                            m.SupportWorkflow = !Convert.IsDBNull(myValue) ? (bool)myValue : false;
                            // Date: 27/2/2003
                            myValue = result["AuthorizedApproveRoles"];
                            m.AuthorizedApproveRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["WorkflowState"];
                            m.WorkflowStatus = !Convert.IsDBNull(myValue) ? (WorkflowState)(0 + (byte)myValue) : WorkflowState.Original;

                            // End Change Geert.Audenaert@Syntegra.Com
                            // Start Change bja@reedtek.com
                            try
                            {
                                myValue = result["SupportCollapsable"];
                            }
                            catch
                            {
                                myValue = DBNull.Value;
                            }
                            m.SupportCollapsable = DBNull.Value != myValue ? (bool)myValue : false;

                            // End Change  bja@reedtek.com
                            // Start Change john.mandia@whitelightsolutions.com
                            try
                            {
                                myValue = result["ShowEveryWhere"];
                            }
                            catch
                            {
                                myValue = DBNull.Value;
                            }
                            m.ShowEveryWhere = DBNull.Value != myValue ? (bool)myValue : false;
                            // End Change  john.mandia@whitelightsolutions.com
                            m.CacheTime = int.Parse(result["CacheTime"].ToString());
                            m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());
                            myValue = result["ShowMobile"];
                            m.ShowMobile = !Convert.IsDBNull(myValue) ? (bool)myValue : false;
                            m.DesktopSrc = result["DesktopSrc"].ToString();
                            m.MobileSrc = result["MobileSrc"].ToString();
                            m.Admin = bool.Parse(result["Admin"].ToString());
                            settings.ActivePage.Modules.Add(m);
                        }
                        // Now read Portal out params 
                        result.NextResult();
                        result.Close(); //by Manu, fixed bug 807858

                        settings.PortalID = (int)parameterPortalID.Value;
                        settings.PortalName = (string)parameterPortalName.Value;
                        //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
                        settings.PortalTitle = String.Concat(Config.PortalTitlePrefix, settings.PortalName);
                        //jes1111 - this.PortalPath = Settings.Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
                        settings.PortalPath = Path.WebPathCombine(Config.PortalsDirectory, (string)parameterPortalPath.Value);
                        //jes1111 - this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003
                        settings.PortalSecurePath = Config.PortalSecureDirectory;

                        //ActivePage initialization
                        settings.ActivePage.PageID = pageID;
                        settings.ActivePage.PageLayout = settings.CurrentLayout;
                        settings.ActivePage.ParentPageID = Int32.Parse("0" + parameterParentPageID.Value);
                        settings.ActivePage.PageOrder = (int)parameterPageOrder.Value;
                        settings.ActivePage.MobilePageName = (string)parameterMobilePageName.Value;
                        settings.ActivePage.AuthorizedRoles = (string)parameterAuthRoles.Value;
                        settings.ActivePage.PageName = (string)parameterPageName.Value;
                        settings.ActivePage.ShowMobile = (bool)parameterShowMobile.Value;
                        settings.ActivePage.PortalPath = settings.PortalPath; // thierry@tiptopweb.com.au for page custom layout
                    }
                    catch (SqlException sqlException)
                    {
                        throw new ProviderException("Error load portal", sqlException);
                    }
                    finally
                    {
                        //by Manu fix close bug #2
                        if (myConnection.State == ConnectionState.Open)
                        {
                            myConnection.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fills brief portal settings for edit
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="portalID"></param>
        public void FillPortalSettingsBrief(PortalSettings settings, int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsPortalID", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    // Open the database connection and execute the command
                    myConnection.Open();
                    SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection); //by Manu CloseConnection

                    try
                    {
                        if (result.Read())
                        {
                            settings.PortalID = Int32.Parse(result["PortalID"].ToString());
                            settings.PortalName = result["PortalName"].ToString();
                            settings.PortalAlias = result["PortalAlias"].ToString();
                            //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + result["PortalName"].ToString();
                            settings.PortalTitle = string.Concat(Config.PortalTitlePrefix, result["PortalName"].ToString());
                            settings.PortalPath = result["PortalPath"].ToString();
                            settings.ActivePage.PageID = 0;
                            // added Thierry (tiptopweb) used for dropdown for layout and theme
                            settings.ActivePage.PortalPath = settings.PortalPath;
                            settings.ActiveModule = 0;
                        }
                        else
                        {
                            throw new ProviderException();
                        }
                    }
                    finally
                    {
                        result.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings persistence.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void UpdatePortalSetting(int portalID, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdatePortalSetting", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC

                    SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterportalID.Value = portalID;
                    myCommand.Parameters.Add(parameterportalID);

                    SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                    parameterKey.Value = key;
                    myCommand.Parameters.Add(parameterKey);

                    SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
                    parameterValue.Value = value;
                    myCommand.Parameters.Add(parameterValue);

                    // Execute the command
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
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns></returns>
        public string GetLanguageList(string portalAlias)
        {
            string langlist = string.Empty;
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsLangList", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                    myCommand.Parameters.Add(parameterPortalAlias);
                    // Open the database connection and execute the command
                    myConnection.Open();

                    try
                    {
                        //Better null check here by Manu
                        object tmp = myCommand.ExecuteScalar();

                        if (tmp != null)
                        {
                            langlist = tmp.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Configuration.LogLevel.Warn, "Get languages from db", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "Failed to get languages from database.", ex); // Jes1111
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }
            return langlist;
        }
    }
}
