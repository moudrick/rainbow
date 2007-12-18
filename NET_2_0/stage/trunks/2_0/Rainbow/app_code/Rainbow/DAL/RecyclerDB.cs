using System;
using System.Data;
using System.Data.SqlClient;
using Esperantus;
using Rainbow.Helpers;
using Rainbow.Settings;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Summary description for recycler.
	/// </summary>
	public class RecyclerDB
	{
		const string strNoModule = "NO_MODULE";

		/// <summary>
		/// MoveModuleToNewTab assigns the given module to the given tab
		/// </summary>
		/// <param name="TabID"></param>
		/// <param name="ModuleID"></param>
		public static void MoveModuleToNewTab(int TabID, int ModuleID) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection MyConnection = Config.SqlConnectionString)
			{
				using (SqlCommand MyCommand = new SqlCommand("rb_MoveModuleToNewTab", MyConnection))
				{

					// Mark the Command as a SPROC
					MyCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter ParameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
					ParameterModuleID.Value = ModuleID;
					MyCommand.Parameters.Add(ParameterModuleID);

					SqlParameter ParameterTabID = new SqlParameter("@TabID", SqlDbType.Int, 4);
					ParameterTabID.Value = TabID;
					MyCommand.Parameters.Add(ParameterTabID);

					MyConnection.Open();
					try
					{
						MyCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in MoveModuleToNewTab. Parameter : " + ModuleID.ToString(), ex);
					}
				}
			}
		}

		/// <summary>
		/// The GetModulesInRecycler method returns a SqlDataReader containing all of the
		/// Modules for a specific portal module that have been 'deleted' to the recycler.
		/// <a href="GetModulesInRecycler.htm" style="color:green">GetModulesInRecycler Stored Procedure</a>
		/// </summary>
		/// <param name="PortalID"></param>
		/// <param name="SortField"></param>
		/// <returns>SqlDataReader</returns>
//		public SqlDataReader GetModulesInRecycler(int PortalID,string SortField)
//		{
//			// Create Instance of Connection and Command Object
//			SqlConnection MyConnection = Config.SqlConnectionString;
//			SqlCommand MyCommand = new SqlCommand("rb_GetModulesInRecycler", MyConnection);
//				
//			// Mark the Command as a SPROC
//			MyCommand.CommandType = CommandType.StoredProcedure;
//
//			// Add Parameters to SPROC
//			SqlParameter ParameterModuleID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
//			ParameterModuleID.Value = PortalID;
//			MyCommand.Parameters.Add(ParameterModuleID);
//
//			SqlParameter ParameterSortField = new SqlParameter("@SortField", SqlDbType.VarChar, 50);
//			ParameterSortField.Value = SortField;
//			MyCommand.Parameters.Add(ParameterSortField);
//
//			// Execute the command
//			MyConnection.Open();
//			return MyCommand.ExecuteReader(CommandBehavior.CloseConnection);
//		}

		public static DataTable GetModulesInRecycler(int PortalID,string SortField)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection MyConnection = Config.SqlConnectionString)
			{
				using (SqlDataAdapter MyCommand = new SqlDataAdapter("rb_GetModulesInRecycler", MyConnection))
				{
				
					// Mark the Command as a SPROC
					MyCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter ParameterModuleID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
					ParameterModuleID.Value = PortalID;
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
						foreach(DataRow dr in myDataTable.Rows)
						{
							if (dr[1].ToString() == strNoModule)
							{
								dr[1] = Localize.GetString(strNoModule);
								break;
							}
						}
						// Return the datareader
						return myDataTable;
					}
				}
			}
		}

		public static bool ModuleIsInRecycler(int ModuleID)
		{
			ModuleSettings ms = GetModuleSettingsForIndividualModule(ModuleID);
			if (ms.PageID == 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// MOST OF THIS METHOD'S CODE IS COPIED DIRECTLY FROM THE PortalSettings() CLASS
		///THE RECYCLER NEEDS TO BE ABLE TO RETRIEVE A MODULE'S ModuleSettings INDEPENDENT
		///OF THE TAB THE MODULE IS LOCATED ON (AND INDEPENDENT OF THE CURRENT 'ActiveTab'
		/// </summary>
		/// <param name="ModuleID"></param>
		/// <returns></returns>
		public static ModuleSettings GetModuleSettingsForIndividualModule(int ModuleID)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection MyConnection = Config.SqlConnectionString)
				   {
				using (SqlCommand MyCommand = new SqlCommand("rb_GetModuleSettingsForIndividualModule", MyConnection))
				{

					// Mark the Command as a SPROC
					MyCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter ParameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
					ParameterModuleID.Value = ModuleID;
					MyCommand.Parameters.Add(ParameterModuleID);

					// Open the database connection and execute the command
					MyConnection.Open();
					SqlDataReader result;
					object myValue;
					result = MyCommand.ExecuteReader(CommandBehavior.CloseConnection);

					ModuleSettings m = new ModuleSettings();

					// Read the resultset -- There is only one row!
					while(result.Read()) 
					{
						m.ModuleID = (int) result["ModuleID"];
						m.ModuleDefID = (int) result["ModuleDefID"];
						m.PageID = (int) result["TabID"];
						m.PaneName = (string) result["PaneName"];
						m.ModuleTitle = (string) result["ModuleTitle"];
					
						myValue = result["AuthorizedEditRoles"];
						m.AuthorizedEditRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						myValue = result["AuthorizedViewRoles"];
						m.AuthorizedViewRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						myValue = result["AuthorizedAddRoles"];
						m.AuthorizedAddRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						myValue = result["AuthorizedDeleteRoles"];
						m.AuthorizedDeleteRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						myValue = result["AuthorizedPropertiesRoles"];
						m.AuthorizedPropertiesRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						// jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
						myValue = result["AuthorizedMoveModuleRoles"];
						m.AuthorizedMoveModuleRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";
				
						myValue = result["AuthorizedDeleteModuleRoles"];
						m.AuthorizedDeleteModuleRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						// Change by Geert.Audenaert@Syntegra.Com
						// Date: 6/2/2003
						myValue = result["AuthorizedPublishingRoles"];
						m.AuthorizedPublishingRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						myValue = result["SupportWorkflow"];
						m.SupportWorkflow = ! Convert.IsDBNull(myValue) ? (bool)myValue : false;

						// Date: 27/2/2003
						myValue = result["AuthorizedApproveRoles"];
						m.AuthorizedApproveRoles = ! Convert.IsDBNull(myValue) ? (string)myValue : "";

						myValue = result["WorkflowState"];
						m.WorkflowStatus = ! Convert.IsDBNull(myValue) ? (WorkflowState)(0 + (byte)myValue) : WorkflowState.Original;
						// End Change Geert.Audenaert@Syntegra.Com

						// Start Change bja@reedtek.com
						try {myValue = result["SupportCollapsable"];}
						catch{myValue = DBNull.Value;}
						m.SupportCollapsable = DBNull.Value != myValue ? (bool)myValue : false;
						// End Change  bja@reedtek.com

						// Start Change john.mandia@whitelightsolutions.com
						try {myValue = result["ShowEveryWhere"];}
						catch{myValue = DBNull.Value;}
						m.ShowEveryWhere = DBNull.Value != myValue ? (bool)myValue : false;
						// End Change  john.mandia@whitelightsolutions.com

						m.CacheTime = int.Parse(result["CacheTime"].ToString());
						m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());

						myValue = result["ShowMobile"];
						m.ShowMobile = ! Convert.IsDBNull(myValue) ? (bool)myValue : false;

						m.DesktopSrc = result["DesktopSrc"].ToString();
						m.MobileSrc = result["MobileSrc"].ToString();
						m.Admin = bool.Parse(result["Admin"].ToString());
					}		
					return m;
				}
			}
		}
	}
}
