using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Rainbow.Data;
using Rainbow.Data.GentleNET;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Class that encapsulates all data logic necessary to add/query/delete
	/// Portals within the Portal database.
	/// </summary>
	public class TabsDB
	{
		private const string strAllUsers = "All Users;";
		private const string strPortalID = "@PortalID";
		private const string strTabID = "@TabID";

		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		public int AddTab(int portalID, String tabName, int tabOrder)
		{
			return AddTab(portalID, tabName, strAllUsers, tabOrder);
		}

		
		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="Roles"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		public int AddTab(int portalID, String tabName, string roles, int tabOrder)
		{
			using (Pages t = new Pages())
			{
				return t.Add(portalID,tabName,roles,tabOrder);
			}

			#region deprecated
			//			// Create Instance of Connection and Command Object
			//			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			//			{
			//				using (SqlCommand myCommand = new SqlCommand("rb_AddTab", myConnection))
			//				{
			//					// Mark the Command as a SPROC
			//					myCommand.CommandType = CommandType.StoredProcedure;
			//					// Add Parameters to SPROC
			//					SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
			//					parameterPortalID.Value = portalID;
			//					myCommand.Parameters.Add(parameterPortalID);
			//
			//					//Fixes a missing tab name
			//					if (tabName == null || tabName.Length == 0) tabName = "New Tab";
			//					SqlParameter parameterTabName = new SqlParameter("@TabName", SqlDbType.NVarChar, 50);
			//
			//					if (tabName.Length > 50) parameterTabName.Value = tabName.Substring(0, 49);
			//
			//					else parameterTabName.Value = tabName.ToString();
			//					myCommand.Parameters.Add(parameterTabName);
			//					SqlParameter parameterTabOrder = new SqlParameter("@TabOrder", SqlDbType.Int, 4);
			//					parameterTabOrder.Value = tabOrder;
			//					myCommand.Parameters.Add(parameterTabOrder);
			//					SqlParameter parameterAuthRoles = new SqlParameter("@AuthorizedRoles", SqlDbType.NVarChar, 256);
			//					parameterAuthRoles.Value = Roles;
			//					myCommand.Parameters.Add(parameterAuthRoles);
			//					SqlParameter parameterMobileTabName = new SqlParameter("@MobileTabName", SqlDbType.NVarChar, 50);
			//					parameterMobileTabName.Value = string.Empty;
			//					myCommand.Parameters.Add(parameterMobileTabName);
			//					SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
			//					parameterTabID.Direction = ParameterDirection.Output;
			//					myCommand.Parameters.Add(parameterTabID);
			//					myConnection.Open();
			//
			//					try
			//					{
			//						myCommand.ExecuteNonQuery();
			//					}
			//
			//					finally
			//					{
			//						myConnection.Close();
			//					}
			//					return (int) parameterTabID.Value;
			//				}
			//			}
			#endregion
		}

		
		/// <summary>
		/// The DeleteTab method deletes the selected tab from the portal.<br />
		/// DeleteTab Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		public void DeleteTab(int tabID)
		{
			using (Pages t = new Pages())
			{
				t.Remove(tabID);
			}

			#region deprecated
			//			// Create Instance of Connection and Command Object
			//			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			//			{
			//				using (SqlCommand myCommand = new SqlCommand("rb_DeleteTab", myConnection))
			//				{
			//					// Mark the Command as a SPROC
			//					myCommand.CommandType = CommandType.StoredProcedure;
			//					// Add Parameters to SPROC
			//					SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
			//					parameterTabID.Value = tabID;
			//					myCommand.Parameters.Add(parameterTabID);
			//					// Open the database connection and execute the command
			//					myConnection.Open();
			//
			//					try
			//					{
			//						myCommand.ExecuteNonQuery();
			//					}
			//
			//					finally
			//					{
			//						myConnection.Close();
			//					}
			//				}
			//			}
			#endregion
		}


		/// <summary>
		/// This user control will render the breadcrumb navigation for the current tab.
		/// Ver. 1.0 - 24. dec 2002 - First realase by Cory Isakson
		/// </summary>
		/// <param name="tabID">ID of the tab</param>
		/// <returns></returns>
		public ArrayList GetTabCrumbs(int tabID)
		{
			using (Pages t = new Pages())
			{
				return t.Crumbs(tabID);
			}

			#region deprecated
			//			// Create Instance of Connection and Command Object
			//			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			//			{
			//				using (SqlCommand myCommand = new SqlCommand("rb_GetTabCrumbs", myConnection))
			//				{
			//					// Mark the Command as a SPROC
			//					myCommand.CommandType = CommandType.StoredProcedure;
			//					// Add Parameters to SPROC
			//					SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
			//					parameterTabID.Value = tabID;
			//					myCommand.Parameters.Add(parameterTabID);
			//					SqlParameter parameterCrumbs = new SqlParameter("@CrumbsXML", SqlDbType.NVarChar, 4000);
			//					parameterCrumbs.Direction = ParameterDirection.Output;
			//					myCommand.Parameters.Add(parameterCrumbs);
			//					// Execute the command
			//					myConnection.Open();
			//
			//					try
			//					{
			//						myCommand.ExecuteNonQuery();
			//					}
			//
			//					finally
			//					{
			//						myConnection.Close();
			//					}
			//					// Build a Hashtable from the XML string returned
			//					ArrayList Crumbs = new ArrayList();
			//					XmlDocument CrumbXML = new XmlDocument();
			//					CrumbXML.LoadXml(parameterCrumbs.Value.ToString().Replace("&", "&amp;"));
			//
			//					//Iterate through the Crumbs XML
			//					foreach (XmlNode node in CrumbXML.FirstChild.ChildNodes)
			//					{
			//						TabItem tab = new TabItem();
			//						tab.ID = Int16.Parse(node.Attributes.GetNamedItem("tabID").Value);
			//						tab.Name = node.InnerText;
			//						tab.Order = Int16.Parse(node.Attributes.GetNamedItem("level").Value);
			//						Crumbs.Add(tab);
			//					}
			//					//Return the Crumb Tab Items as an arraylist 
			//					return Crumbs;
			//				}
			//			}
			#endregion
		}


		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		[Obsolete("Replace me")]
		public SqlDataReader GetTabsByPortal(int portalID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = PortalSettings.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetTabsByPortal", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader 
			return result;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.ArrayList value...
		/// </returns>
		public ArrayList GetTabsFlat(int portalID)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetTabsFlat", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					// Execute the command
					myConnection.Open();
					SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
					ArrayList DesktopTabs = new ArrayList();

					// Read the resultset
					try
					{
						while (result.Read())
						{
							TabItem tabItem = new TabItem();
							tabItem.ID = (int) result["TabID"];
							tabItem.Name = (string) result["TabName"];
							tabItem.Order = (int) result["TabOrder"];
							tabItem.NestLevel = (int) result["NestLevel"];
							DesktopTabs.Add(tabItem);
						}
					}

					finally
					{
						result.Close(); //by Manu, fixed bug 807858
					}
					return DesktopTabs;
				}
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.ArrayList value...
		/// </returns>
		public ArrayList GetTabsinTab(int portalID, int tabID)
		{
			ArrayList DesktopTabs = new ArrayList();

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetTabsinTab", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
					parameterTabID.Value = tabID;
					myCommand.Parameters.Add(parameterTabID);
					// Execute the command
					myConnection.Open();
					SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

					// Read the resultset
					try
					{
						while (result.Read())
						{
							TabStripDetails tabDetails = new TabStripDetails();
							tabDetails.TabID = (int) result["TabID"];
							tabDetails.ParentTabID = Int32.Parse("0" + result["ParentTabID"]);
							tabDetails.TabName = (string) result["TabName"];
							tabDetails.TabOrder = (int) result["TabOrder"];
							tabDetails.AuthorizedRoles = (string) result["AuthorizedRoles"];
							// Update the AuthorizedRoles Variable
							DesktopTabs.Add(tabDetails);
						}
					}

					finally
					{
						result.Close(); //by Manu, fixed bug 807858
					}
				}
			}
			return DesktopTabs;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		[Obsolete("Replace me")]
		public SqlDataReader GetTabsParent(int portalID, int tabID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = PortalSettings.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetTabsParent", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
			parameterTabID.Value = tabID;
			myCommand.Parameters.Add(parameterTabID);
			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader 
			return result;
		}

		/// <summary>
		/// UpdateTab Method<br />
		/// UpdateTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabID"></param>
		/// <param name="parentTabID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <param name="authorizedRoles"></param>
		/// <param name="mobileTabName"></param>
		/// <param name="showMobile"></param>
		public void UpdateTab(int portalID, int tabID, int parentTabID,
			String tabName, int tabOrder, String authorizedRoles,
			String mobileTabName, bool showMobile)
		{
			using (Pages t = new Pages())
			{
				t.Update(portalID,tabID,parentTabID,tabName,tabOrder,
					authorizedRoles,mobileTabName,showMobile);
			}

			#region deprecated
			//			// Create Instance of Connection and Command Object
			//			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			//			{
			//				using (SqlCommand myCommand = new SqlCommand("rb_UpdateTab", myConnection))
			//				{
			//					// Mark the Command as a SPROC
			//					myCommand.CommandType = CommandType.StoredProcedure;
			//					// Add Parameters to SPROC
			//					SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
			//					parameterPortalID.Value = portalID;
			//					myCommand.Parameters.Add(parameterPortalID);
			//					SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
			//					parameterTabID.Value = tabID;
			//					myCommand.Parameters.Add(parameterTabID);
			//					SqlParameter parameterParentTabID = new SqlParameter("@ParentTabID", SqlDbType.Int, 4);
			//					parameterParentTabID.Value = parentTabID;
			//					myCommand.Parameters.Add(parameterParentTabID);
			//
			//					//Fixes a missing tab name
			//					if (tabName == null || tabName.Length == 0) tabName = "&nbsp;";
			//					SqlParameter parameterTabName = new SqlParameter("@TabName", SqlDbType.NVarChar, 50);
			//
			//					if (tabName.Length > 50) parameterTabName.Value = tabName.Substring(0, 49);
			//
			//					else parameterTabName.Value = tabName.ToString();
			//					myCommand.Parameters.Add(parameterTabName);
			//					SqlParameter parameterTabOrder = new SqlParameter("@TabOrder", SqlDbType.Int, 4);
			//					parameterTabOrder.Value = tabOrder;
			//					myCommand.Parameters.Add(parameterTabOrder);
			//					SqlParameter parameterAuthRoles = new SqlParameter("@AuthorizedRoles", SqlDbType.NVarChar, 256);
			//					parameterAuthRoles.Value = authorizedRoles;
			//					myCommand.Parameters.Add(parameterAuthRoles);
			//					SqlParameter parameterMobileTabName = new SqlParameter("@MobileTabName", SqlDbType.NVarChar, 50);
			//					parameterMobileTabName.Value = mobileTabName;
			//					myCommand.Parameters.Add(parameterMobileTabName);
			//					SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
			//					parameterShowMobile.Value = showMobile;
			//					myCommand.Parameters.Add(parameterShowMobile);
			//					myConnection.Open();
			//
			//					try
			//					{
			//						myCommand.ExecuteNonQuery();
			//					}
			//
			//					finally
			//					{
			//						myConnection.Close();
			//					}
			//				}
			//			}
			#endregion
		}

		/// <summary>
		/// The UpdateTabOrder method changes the position of the tab with respect
		/// to other tabs in the portal.<br />
		/// UpdateTabOrder Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		/// <param name="tabOrder"></param>
		public void UpdateTabOrder(int tabID, int tabOrder)
		{
			using (Pages t = new Pages())
			{
				t.UpdateOrder(tabID,tabOrder);
			}

			#region deprecated
			//			// Create Instance of Connection and Command Object
			//			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			//			{
			//				using (SqlCommand myCommand = new SqlCommand("rb_UpdateTabOrder", myConnection))
			//				{
			//					// Mark the Command as a SPROC
			//					myCommand.CommandType = CommandType.StoredProcedure;
			//					// Add Parameters to SPROC
			//					SqlParameter parameterTabID = new SqlParameter(strTabID, SqlDbType.Int, 4);
			//					parameterTabID.Value = tabID;
			//					myCommand.Parameters.Add(parameterTabID);
			//					SqlParameter parameterTabOrder = new SqlParameter("@TabOrder", SqlDbType.Int, 4);
			//					parameterTabOrder.Value = tabOrder;
			//					myCommand.Parameters.Add(parameterTabOrder);
			//					myConnection.Open();
			//
			//					try
			//					{
			//						myCommand.ExecuteNonQuery();
			//					}
			//
			//					finally
			//					{
			//						myConnection.Close();
			//					}
			//				}
			//			}
			#endregion
		}
	}
}