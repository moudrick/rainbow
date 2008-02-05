using System;
using System.Data;
using System.Data.SqlClient;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Data;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Site.Data
{
    /// <summary>
    /// Summary description for recycler.
    /// </summary>
    public class RecyclerDB
    {
        private const string strNoModule = "NO_MODULE";

        /// <summary>
        /// MoveModuleToNewTab assigns the given module to the given tab
        /// </summary>
        /// <param name="tabID">The tab ID.</param>
        /// <param name="moduleID">The module ID.</param>
        public static void MoveModuleToNewTab(int tabID, int moduleID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection MyConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand MyCommand = new SqlCommand("rb_MoveModuleToNewTab", MyConnection))
                {
                    // Mark the Command as a SPROC
                    MyCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    SqlParameter ParameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
                    ParameterModuleID.Value = moduleID;
                    MyCommand.Parameters.Add(ParameterModuleID);

                    SqlParameter ParameterTabID = new SqlParameter("@TabID", SqlDbType.Int, 4);
                    ParameterTabID.Value = tabID;
                    MyCommand.Parameters.Add(ParameterTabID);

                    MyConnection.Open();
                    try
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Warn,
                                             "An Error Occurred in MoveModuleToNewTab. Parameter : " +
                                             moduleID, ex);
                    }
                }
            }
        }

        /// <summary>
        /// The GetModulesInRecycler method returns a SqlDataReader containing all of the
        /// Modules for a specific portal module that have been 'deleted' to the recycler.
        /// <a href="GetModulesInRecycler.htm" style="color:green">GetModulesInRecycler Stored Procedure</a>
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="SortField">The sort field.</param>
        /// <returns>SqlDataReader</returns>
        public static DataTable GetModulesInRecycler(int portalID, string SortField)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection MyConnection = DBHelper.SqlConnection)
            {
                using (SqlDataAdapter MyCommand = new SqlDataAdapter("rb_GetModulesInRecycler", MyConnection))
                {
                    // Mark the Command as a SPROC
                    MyCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    SqlParameter ParameterModuleID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
                    ParameterModuleID.Value = portalID;
                    MyCommand.SelectCommand.Parameters.Add(ParameterModuleID);

                    SqlParameter ParameterSortField = new SqlParameter("@SortField", SqlDbType.VarChar, 50);
                    ParameterSortField.Value = SortField;
                    MyCommand.SelectCommand.Parameters.Add(ParameterSortField);


                    // Create and Fill the DataSet
                    using (DataTable myDataTable = new DataTable())
                    {
                        try
                        {
                            MyCommand.Fill(myDataTable);
                        }
                        finally
                        {
                            MyConnection.Close(); //by Manu fix close bug #2
                        }

                        // Translate
                        foreach (DataRow dr in myDataTable.Rows)
                        {
                            if (dr[1].ToString() == strNoModule)
                            {
                                dr[1] = General.GetString(strNoModule);
                                break;
                            }
                        }
                        // Return the datareader
                        return myDataTable;
                    }
                }
            }
        }

        /// <summary>
        /// Modules the is in recycler.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns></returns>
        public static bool ModuleIsInRecycler(int moduleID)
        {
            RainbowModule ms = GetModuleSettingsForIndividualModule(moduleID);
            return ms.PageID == 0;
        }

        /// <summary>
        /// MOST OF THIS METHOD'S CODE IS COPIED DIRECTLY FROM THE PortalSettings() CLASS
        /// THE RECYCLER NEEDS TO BE ABLE TO RETRIEVE A MODULE'S ModuleSettings INDEPENDENT
        /// OF THE TAB THE MODULE IS LOCATED ON (AND INDEPENDENT OF THE CURRENT 'ActiveTab'
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns></returns>
        public static RainbowModule GetModuleSettingsForIndividualModule(int moduleID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection MyConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand MyCommand = new SqlCommand("rb_GetModuleSettingsForIndividualModule", MyConnection))
                {
                    // Mark the Command as a SPROC
                    MyCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    SqlParameter ParameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
                    ParameterModuleID.Value = moduleID;
                    MyCommand.Parameters.Add(ParameterModuleID);

                    // Open the database connection and execute the command
                    MyConnection.Open();
                    SqlDataReader result;
                    result = MyCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    RainbowModule module = RainbowModuleProvider.Instance.CreateModuleSettings();

                    // Read the resultset -- There is only one row!
                    while (result.Read())
                    {
                        module.ModuleID = (int) result["ModuleID"];
                        module.ModuleDefID = (int) result["ModuleDefID"];
                        module.PageID = (int) result["TabID"];
                        module.PaneName = (string) result["PaneName"];
                        module.ModuleTitle = (string) result["ModuleTitle"];

                        object myValue = result["AuthorizedEditRoles"];
                        module.AuthorizedEditRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["AuthorizedViewRoles"];
                        module.AuthorizedViewRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["AuthorizedAddRoles"];
                        module.AuthorizedAddRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["AuthorizedDeleteRoles"];
                        module.AuthorizedDeleteRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["AuthorizedPropertiesRoles"];
                        module.AuthorizedPropertiesRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        // jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
                        myValue = result["AuthorizedMoveModuleRoles"];
                        module.AuthorizedMoveModuleRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["AuthorizedDeleteModuleRoles"];
                        module.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        // Change by Geert.Audenaert@Syntegra.Com
                        // Date: 6/2/2003
                        myValue = result["AuthorizedPublishingRoles"];
                        module.AuthorizedPublishingRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["SupportWorkflow"];
                        module.SupportWorkflow = !Convert.IsDBNull(myValue) ? (bool) myValue : false;

                        // Date: 27/2/2003
                        myValue = result["AuthorizedApproveRoles"];
                        module.AuthorizedApproveRoles = !Convert.IsDBNull(myValue) ? (string) myValue : "";

                        myValue = result["WorkflowState"];
                        module.WorkflowStatus = !Convert.IsDBNull(myValue)
                                               ? (WorkflowState) (0 + (byte) myValue)
                                               : WorkflowState.Original;
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
                        module.SupportCollapsable = DBNull.Value != myValue ? (bool) myValue : false;
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
                        module.ShowEveryWhere = DBNull.Value != myValue ? (bool) myValue : false;
                        // End Change  john.mandia@whitelightsolutions.com

                        module.CacheTime = int.Parse(result["CacheTime"].ToString());
                        module.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());

                        myValue = result["ShowMobile"];
                        module.ShowMobile = !Convert.IsDBNull(myValue) ? (bool) myValue : false;

                        module.DesktopSrc = result["DesktopSrc"].ToString();
                        module.MobileSrc = result["MobileSrc"].ToString();
                        module.Admin = bool.Parse(result["Admin"].ToString());
                    }
                    return module;
                }
            }
        }
    }
}
