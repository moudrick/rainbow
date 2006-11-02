using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Esperantus;
using Rainbow.Helpers;
using Rainbow.Settings;
using Rainbow.Settings.Cache;

namespace Rainbow.Configuration
{

	/// <summary>
	/// Class that encapsulates all data logic necessary to add/query/delete
	/// configuration, layout and security settings values within the Portal database.
	/// </summary>
	public class ModulesDB 
	{
		const string strATAddRoles = "@AddRoles";
		const string strATAdmin = "@Admin";
		const string strATApprovalRoles = "@ApprovalRoles";
		const string strATAssemblyName = "@AssemblyName";
		const string strATCacheTime = "@CacheTime";
		const string strATClassName = "@ClassName";
		const string strATDeleteModuleRoles = "@DeleteModuleRoles";
		const string strATDeleteRoles = "@DeleteRoles";
		const string strATDesktopSrc = "@DesktopSrc";
		const string strATEditRoles = "@EditRoles";
		const string strATFriendlyName = "@FriendlyName";
		const string strATGeneralModDefID = "@GeneralModDefID";
		const string strATGuid = "@Guid";
		const string strATMobileSrc = "@MobileSrc";
		const string strATModuleDefID = "@ModuleDefID";
		const string strATModuleID = "@ModuleID";
		const string strATModuleOrder = "@ModuleOrder";
		const string strATModuleTitle = "@ModuleTitle";
		const string strATMoveModuleRoles = "@MoveModuleRoles";
		const string strATPaneName = "@PaneName";
		const string strATPortalID = "@PortalID";
		const string strATPropertiesRoles = "@PropertiesRoles";
		const string strATPublishingRoles = "@PublishingRoles";
		const string strATSearchable = "@Searchable";
		const string strATShowEveryWhere = "@ShowEveryWhere";
		const string strATShowMobile = "@ShowMobile";
		const string strATSupportCollapsable = "@SupportCollapsable";
		const string strATSupportWorkflow = "@SupportWorkflow";
		const string strATPageID = "@TabID";
		const string strATViewRoles = "@ViewRoles";
		//const string strGUID = "GUID";
		const string strNoModule = "NO_MODULE";
		const string strrb_GetModulesInPage = "rb_GetModulesInTab";

		/// <summary>
		/// AddGeneralModuleDefinitions
		/// </summary>
		/// <param name="GeneralModDefID">GeneralModDefID</param>
		/// <param name="Admin"></param>
		/// <param name="AssemblyName"></param>
		/// <param name="ClassName"></param>
		/// <param name="DesktopSrc"></param>
		/// <param name="FriendlyName"></param>
		/// <param name="MobileSrc"></param>
		/// <param name="Searchable"></param>
		/// <returns>The newly created ID</returns>
		public Guid AddGeneralModuleDefinitions(Guid GeneralModDefID, string FriendlyName, string DesktopSrc, string MobileSrc, string AssemblyName, string ClassName, bool Admin, bool Searchable)
		{
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_AddGeneralModuleDefinitions", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterGeneralModDefID = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
					parameterGeneralModDefID.Value = GeneralModDefID;
					myCommand.Parameters.Add(parameterGeneralModDefID);
					SqlParameter parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
					parameterFriendlyName.Value = FriendlyName;
					myCommand.Parameters.Add(parameterFriendlyName);
					SqlParameter parameterDesktopSrc = new SqlParameter(strATDesktopSrc, SqlDbType.NVarChar, 256);
					parameterDesktopSrc.Value = DesktopSrc;
					myCommand.Parameters.Add(parameterDesktopSrc);
					SqlParameter parameterMobileSrc = new SqlParameter(strATMobileSrc, SqlDbType.NVarChar, 256);
					parameterMobileSrc.Value = MobileSrc;
					myCommand.Parameters.Add(parameterMobileSrc);
					SqlParameter parameterAssemblyName = new SqlParameter(strATAssemblyName, SqlDbType.VarChar, 50);
					parameterAssemblyName.Value = AssemblyName;
					myCommand.Parameters.Add(parameterAssemblyName);
					SqlParameter parameterClassName = new SqlParameter(strATClassName, SqlDbType.NVarChar, 128);
					parameterClassName.Value = ClassName;
					myCommand.Parameters.Add(parameterClassName);
					SqlParameter parameterAdmin = new SqlParameter(strATAdmin, SqlDbType.Bit);
					parameterAdmin.Value = Admin;
					myCommand.Parameters.Add(parameterAdmin);
					SqlParameter parameterSearchable = new SqlParameter(strATSearchable, SqlDbType.Bit);
					parameterSearchable.Value = Searchable;
					myCommand.Parameters.Add(parameterSearchable);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in AddGeneralModuleDefinitions. ", ex);
					}

					// Return the newly created ID
					return new Guid(parameterGeneralModDefID.Value.ToString());
				}
			}
		}

		/// <summary>
		/// The AddModule method updates a specified Module within the Modules database table.
		/// If the module does not yet exist,the stored procedure adds it.<br />
		/// AddModule Stored Procedure
		/// </summary>
		/// <param name="pageID"></param>
		/// <param name="moduleOrder"></param>
		/// <param name="paneName"></param>
		/// <param name="title"></param>
		/// <param name="moduleDefID"></param>
		/// <param name="cacheTime"></param>
		/// <param name="editRoles"></param>
		/// <param name="viewRoles"></param>
		/// <param name="addRoles"></param>
		/// <param name="deleteRoles"></param>
		/// <param name="PropertiesRoles"></param>
		/// <param name="moveModuleRoles"></param>
		/// <param name="deleteModuleRoles"></param>
		/// <param name="showMobile"></param>
		/// <param name="publishingRoles"></param>
		/// <param name="supportWorkflow"></param>
		/// <param name="showEveryWhere"></param>
		/// <param name="supportCollapsable"></param>
		/// <returns></returns>
		[History("jviladiu@portalServices.net","2004/08/19","Added support for move & delete modules roles")]
		[History("john.mandia@whitelightsolutions.com","2003/05/24","Added support for showEveryWhere")]
		[History("bja@reedtek.com","2003/05/16","Added support for win. mgmt min/max/close -- supportCollapsable")]
		public int AddModule(int pageID, int moduleOrder, string paneName, string title, int moduleDefID, int cacheTime,
			string editRoles, string viewRoles, string addRoles, string deleteRoles, string PropertiesRoles,
			string moveModuleRoles, string deleteModuleRoles, bool showMobile, string publishingRoles,
			bool supportWorkflow, bool showEveryWhere, bool supportCollapsable) 
		{
			// Changes by Geert.Audenaert@Syntegra.Com Date: 6/2/2003
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{
				using (SqlCommand myCommand = new SqlCommand("rb_AddModule", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterModuleID);
					SqlParameter parameterModuleDefinitionID = new SqlParameter(strATModuleDefID, SqlDbType.Int, 4);
					parameterModuleDefinitionID.Value = moduleDefID;
					myCommand.Parameters.Add(parameterModuleDefinitionID);
					SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
					parameterPageID.Value = pageID;
					myCommand.Parameters.Add(parameterPageID);
					SqlParameter parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
					parameterModuleOrder.Value = moduleOrder;
					myCommand.Parameters.Add(parameterModuleOrder);
					SqlParameter parameterTitle = new SqlParameter(strATModuleTitle, SqlDbType.NVarChar, 256);
					parameterTitle.Value = title;
					myCommand.Parameters.Add(parameterTitle);
					SqlParameter parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
					parameterPaneName.Value = paneName;
					myCommand.Parameters.Add(parameterPaneName);
					SqlParameter parameterCacheTime = new SqlParameter(strATCacheTime, SqlDbType.Int, 4);
					parameterCacheTime.Value = cacheTime;
					myCommand.Parameters.Add(parameterCacheTime);
					SqlParameter parameterEditRoles = new SqlParameter(strATEditRoles, SqlDbType.NVarChar, 256);
					parameterEditRoles.Value = editRoles;
					myCommand.Parameters.Add(parameterEditRoles);
					SqlParameter parameterViewRoles = new SqlParameter(strATViewRoles, SqlDbType.NVarChar, 256);
					parameterViewRoles.Value = viewRoles;
					myCommand.Parameters.Add(parameterViewRoles);
					SqlParameter parameterAddRoles = new SqlParameter(strATAddRoles, SqlDbType.NVarChar, 256);
					parameterAddRoles.Value = addRoles;
					myCommand.Parameters.Add(parameterAddRoles);
					SqlParameter parameterDeleteRoles = new SqlParameter(strATDeleteRoles, SqlDbType.NVarChar, 256);
					parameterDeleteRoles.Value = deleteRoles;
					myCommand.Parameters.Add(parameterDeleteRoles);
					SqlParameter parameterPropertiesRoles = new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
					parameterPropertiesRoles.Value = PropertiesRoles;
					myCommand.Parameters.Add(parameterPropertiesRoles);
					// Added by jviladiu@portalservices.net (19/08/2004)
					SqlParameter parameterMoveModuleRoles = new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
					parameterMoveModuleRoles.Value = moveModuleRoles;
					myCommand.Parameters.Add(parameterMoveModuleRoles);
					// Added by jviladiu@portalservices.net (19/08/2004)
					SqlParameter parameterDeleteModuleRoles = new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
					parameterDeleteModuleRoles.Value = deleteModuleRoles;
					myCommand.Parameters.Add(parameterDeleteModuleRoles);
					// Change by Geert.Audenaert@Syntegra.Com
					// Date: 6/2/2003
					SqlParameter parameterPublishingRoles = new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
					parameterPublishingRoles.Value = publishingRoles;
					myCommand.Parameters.Add(parameterPublishingRoles);
					SqlParameter parameterSupportWorkflow = new SqlParameter(strATSupportWorkflow, SqlDbType.Bit, 1);
					parameterSupportWorkflow.Value = supportWorkflow;
					myCommand.Parameters.Add(parameterSupportWorkflow);
					// End Change Geert.Audenaert@Syntegra.Com
					SqlParameter parameterShowMobile = new SqlParameter(strATShowMobile, SqlDbType.Bit, 1);
					parameterShowMobile.Value = showMobile;
					myCommand.Parameters.Add(parameterShowMobile);
					// Start Change john.mandia@whitelightsolutions.com
					SqlParameter parameterShowEveryWhere = new SqlParameter(strATShowEveryWhere, SqlDbType.Bit, 1);
					parameterShowEveryWhere.Value = showEveryWhere;
					myCommand.Parameters.Add(parameterShowEveryWhere);
					// End Change  john.mandia@whitelightsolutions.com
					// Start Change bja@reedtek.com
					SqlParameter parameterSupportCollapsable = new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
					parameterSupportCollapsable.Value = supportCollapsable;
					myCommand.Parameters.Add(parameterSupportCollapsable);
					// End Change  bja@reedtek.com
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						//LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in AddModule. ", ex);
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in AddModule. ", ex);
					}

					return (int) parameterModuleID.Value;
				}
			}
		}

		/// <summary>
		/// The DeleteModule method deletes a specified Module from the Modules database table.<br />
		/// DeleteModule Stored Procedure
		/// </summary>
		/// <param name="moduleID"></param>
		[History("JB - john@bowenweb.com","2005/05/12","Added support for Recycler module")]
		public void DeleteModule(int moduleID) 
		{
			//BOWEN 11 June 2005 - BEGIN
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
			bool useRecycler = bool.Parse(PortalSettings.GetPortalCustomSettings(portalSettings.PortalID,PortalSettings.GetPortalBaseSettings(portalSettings.PortalPath))["SITESETTINGS_USE_RECYCLER"].ToString());
			
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand((useRecycler ? "rb_DeleteModuleToRecycler" : "rb_DeleteModule"), myConnection))
				{			
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC

					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);

					if (useRecycler)  //Recycler needs some extra params for entry
					{
						// Add Recycler-specific Parameters to SPROC
						SqlParameter paramDeletedBy = new SqlParameter("@DeletedBy", SqlDbType.NVarChar, 250);
						paramDeletedBy.Value = MailHelper.GetCurrentUserEmailAddress();
						myCommand.Parameters.Add(paramDeletedBy);

						SqlParameter paramDeletedDate = new SqlParameter("@DateDeleted", SqlDbType.DateTime, 8);
						paramDeletedDate.Value = DateTime.Now;
						myCommand.Parameters.Add(paramDeletedDate);
					}
					//BOWEN 11 June 2005 - END
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in DeleteModule. Parameter : " + moduleID.ToString(), ex);
					}
				}
			}
		}

		/// <summary>
		/// The DeleteModuleDefinition method deletes the specified 
		/// module type definition from the portal.
		/// </summary>
		/// <remarks>Other relevant sources: DeleteModuleDefinition Stored Procedure</remarks>
		/// <param name="defID"></param>
		public void DeleteModuleDefinition(Guid defID)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_DeleteModuleDefinition", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterModuleDefID = new SqlParameter(strATModuleDefID, SqlDbType.UniqueIdentifier);
					parameterModuleDefID.Value = defID;
					myCommand.Parameters.Add(parameterModuleDefID);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						//LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in DeleteModuleDefinition. Parameter : " + defID.ToString(), ex);
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in DeleteModuleDefinition. Parameter : " + defID.ToString(), ex);
					}
				}
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		public bool ExistModuleProductsInPage (int tabID, int portalID) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{
				using (SqlCommand myCommand = new SqlCommand(strrb_GetModulesInPage, myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
					parameterPageID.Value = tabID;
					myCommand.Parameters.Add(parameterPageID);
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);

					myConnection.Open();
					Guid moduleGuid = new Guid("{EC24FABD-FB16-4978-8C81-1ADD39792377}");
					bool retorno = false;

					using (SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection)) 
					{
						while (result.Read()) 
						{
							if (moduleGuid.Equals(result.GetGuid(1))) retorno = true;
						}
					}
					return retorno;
				}
			}
		}

		/// <summary>
		/// Find module id defined by the guid in a tab in the portal
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		public ArrayList FindModuleItemsByGuid(int portalID, Guid guid)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_FindModulesByGuid", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
					parameterFriendlyName.Value = guid;
					myCommand.Parameters.Add(parameterFriendlyName);
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					// Open the database connection and execute the command
					myConnection.Open();
					ArrayList modList = new ArrayList();

					using (SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection)) 
					{
						ModuleItem m = null;
						while(result.Read()) 
						{
							m = new ModuleItem();
							m.ID=(int)result["ModuleId"];
							modList.Add(m);
						}
					}
					return modList;
				}
			}
		}

		/// <summary>
		/// Find modules defined by the guid in a tab in the portal
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		[Obsolete("Use FindModuleItemsByGuid instead.")]
		public SqlDataReader FindModulesByGuid(int portalID, Guid guid)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_FindModulesByGuid", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
			parameterFriendlyName.Value = guid;
			myCommand.Parameters.Add(parameterFriendlyName);
			SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// The GetModuleDefinitions method returns a list of all module type definitions.
		/// </summary>
		/// <remarks>Other relevant sources: GetModuleDefinitions Stored Procedure</remarks>
		/// <param name="portalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetCurrentModuleDefinitions(int portalID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetCurrentModuleDefinitions", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// The GetGeneralModuleDefinitionByName method returns the id of the Module
		/// that matches the named Module in general list.
		/// </summary>
		/// <param name="moduleName"></param>
		/// <returns></returns>
		public Guid GetGeneralModuleDefinitionByName(string moduleName)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetGeneralModuleDefinitionByName", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterFriendlyName = new SqlParameter("@FriendlyName", SqlDbType.NVarChar, 128);
					parameterFriendlyName.Value = moduleName;
					myCommand.Parameters.Add(parameterFriendlyName);
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.UniqueIdentifier);
					parameterModuleID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterModuleID);
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

					if (parameterModuleID.Value != null && parameterModuleID.Value.ToString().Length != 0)
					{

						try
						{
							return new Guid(parameterModuleID.Value.ToString());
						}

						catch(Exception ex)
						{
							throw new Exception("'" + parameterModuleID.Value.ToString() + "' seems not a valid Module GUID.",ex); // Jes1111
							//Rainbow.Configuration.ErrorHandler.HandleException("'" + parameterModuleID.Value.ToString() + "' seems not a valid GUID.", ex);
							//throw;
						}
					}

					else
					{
						throw new ArgumentException("Null Module GUID!"); // Jes1111
						//Rainbow.Configuration.ErrorHandler.HandleException("Null GUID!.", new ArgumentException("Null GUID!", strGUID));
					}
					//throw new ArgumentException("Invalid GUID", strGUID);
				}
			}
		}

		/// <summary>
		/// The GetModuleDefinitionByGUID method returns the id of the Module
		/// that matches the named Module for the specified Portal.
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="guid"></param>
		/// <returns></returns>
		public int GetModuleDefinitionByGuid(int portalID, Guid guid)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetModuleDefinitionByGuid", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
					parameterFriendlyName.Value = guid;
					myCommand.Parameters.Add(parameterFriendlyName);
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterModuleID);

					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in GetModuleDefinitionByGuid. Parameter : " + guid.ToString(), ex);
					}
					return (int) parameterModuleID.Value;
				}
			}
		}

		/// <summary>
		/// The GetModuleDefinitionByName method returns the id of the Module
		/// that matches the named Module for the specified Portal.
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="moduleName"></param>
		/// <returns></returns>
		public int GetModuleDefinitionByName(int portalID, string moduleName)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetModuleDefinitionByName", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
					parameterFriendlyName.Value = moduleName;
					myCommand.Parameters.Add(parameterFriendlyName);
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterModuleID);

					myConnection.Open();
					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in GetModuleDefinitionByName. Parameter : " + moduleName, ex);
					}
					return (int) parameterModuleID.Value;
				}
			}
		}

		/// <summary>
		/// The GetModuleDefinitions method returns a list of all module type 
		/// definitions for the portal.<br />
		/// GetModuleDefinitions Stored Procedure
		/// </summary>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetModuleDefinitions()
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetModuleDefinitions", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="moduleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Guid value...
		/// </returns>
		public Guid GetModuleGuid (int moduleID) 
		{
			Guid moduleGuid = Guid.Empty;
			string cacheGuid = Key.ModuleSettings(moduleID) + "GUID";
			if (CurrentCache.Get(cacheGuid) == null) 
			{
				using (SqlConnection myConnection = Config.SqlConnectionString)
				{
					using (SqlCommand myCommand = new SqlCommand("rb_GetGuid", myConnection))
					{
						myCommand.CommandType = CommandType.StoredProcedure;
						SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
						parameterModuleID.Value = moduleID;
						myCommand.Parameters.Add(parameterModuleID);
						myConnection.Open();
						using (SqlDataReader dr = myCommand.ExecuteReader())
						{
							if (dr.Read())
								moduleGuid = dr.GetGuid(0);
						}
					}
				}
				CurrentCache.Insert(cacheGuid, moduleGuid);
			}
			else 
				moduleGuid = (Guid) CurrentCache.Get(cacheGuid);
			return moduleGuid;
		}

		/// <summary>
		/// The GetModuleInUse method returns a list of modules in use with this portal<br />
		/// GetModuleInUse Stored Procedure
		/// </summary>
		/// <param name="defID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetModuleInUse(Guid defID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetModuleInUse", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterdefID = new SqlParameter(strATModuleID, SqlDbType.UniqueIdentifier);
			parameterdefID.Value = defID;
			myCommand.Parameters.Add(parameterdefID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// GetModulesAllPortals
		/// </summary>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public DataTable GetModulesAllPortals()
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetModulesAllPortals", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

					// Create and Fill the DataSet
					using (DataTable myDataTable = new DataTable()) 
					{
						try
						{
							myCommand.Fill(myDataTable);
						}
						finally
						{
							myConnection.Close(); //by Manu fix close bug #2
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

		/// <summary>
		/// The GetModuleByName method returns a list of all module with
		/// the specified Name (Type) within the Portal. 
		/// It is used to get all instances of a specified module used in a Portal.
		///  e.g. All Image Gallery
		/// </summary>
		/// <remarks>Other relevant sources: GetModuleByName Stored Procedure</remarks>
		/// <param name="moduleName"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetModulesByName(string moduleName, int portalID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetModulesByName", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterFriendlyName = new SqlParameter("@moduleName", SqlDbType.NVarChar, 128);
			parameterFriendlyName.Value = moduleName;
			myCommand.Parameters.Add(parameterFriendlyName);
			SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="pageID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetModulesInPage(int portalID, int pageID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand(strrb_GetModulesInPage, myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Add Parameters to SPROC
			SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
			parameterPageID.Value = pageID;
			myCommand.Parameters.Add(parameterPageID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// GetModulesSinglePortal
		/// </summary>
		/// <param name="PortalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public DataTable GetModulesSinglePortal(int PortalID)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetModulesSinglePortal", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int);
					parameterPortalID.Value = PortalID;
					myCommand.SelectCommand.Parameters.Add(parameterPortalID);

					// Create and Fill the DataSet
					using (DataTable myDataTable = new DataTable()) 
					{
						try
						{
							myCommand.Fill(myDataTable);
						}
						finally
						{
							myConnection.Close(); //by Manu fix close bug #2
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

		/// <summary>
		/// The GetSingleModuleDefinition method returns a SqlDataReader 
		/// containing details about a specific module definition 
		/// from the ModuleDefinitions table.
		/// </summary>
		/// <remarks>Other relevant sources: GetSingleModuleDefinition Stored Procedure</remarks>
		/// <param name="GeneralModDefID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetSingleModuleDefinition(Guid GeneralModDefID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSingleModuleDefinition", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterGeneralModDefID = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
			parameterGeneralModDefID.Value = GeneralModDefID;
			myCommand.Parameters.Add(parameterGeneralModDefID);
			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader 
			return result;
		}

		/// <summary>
		/// The GetSolutionModuleDefinitions method returns a list of all module type definitions.<br></br>
		/// GetSolutionModuleDefinitions Stored Procedure
		/// </summary>
		/// <param name="solutionID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetSolutionModuleDefinitions(int solutionID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSolutionModuleDefinitions", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterSolutionID = new SqlParameter("@SolutionID", SqlDbType.Int, 4);
			parameterSolutionID.Value = solutionID;
			myCommand.Parameters.Add(parameterSolutionID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// The GetSolutions method returns the Solution list.
		/// </summary>
		/// <remarks>Other relevant sources: GetUsers Stored Procedure</remarks>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetSolutions()
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSolutions", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// UpdateGeneralModuleDefinitions
		/// </summary>
		/// <param name="GeneralModDefID">GeneralModDefID</param>
		/// <param name="Admin"></param>
		/// <param name="AssemblyName"></param>
		/// <param name="ClassName"></param>
		/// <param name="DesktopSrc"></param>
		/// <param name="FriendlyName"></param>
		/// <param name="MobileSrc"></param>
		/// <param name="Searchable"></param>
		/// <returns>Void</returns>
		public void UpdateGeneralModuleDefinitions(Guid GeneralModDefID, string FriendlyName, string DesktopSrc, string MobileSrc, string AssemblyName, string ClassName, bool Admin, bool Searchable)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateGeneralModuleDefinitions", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameterGeneralModDefID = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
					parameterGeneralModDefID.Value = GeneralModDefID;
					myCommand.Parameters.Add(parameterGeneralModDefID);
					SqlParameter parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
					parameterFriendlyName.Value = FriendlyName;
					myCommand.Parameters.Add(parameterFriendlyName);
					SqlParameter parameterDesktopSrc = new SqlParameter(strATDesktopSrc, SqlDbType.NVarChar, 256);
					parameterDesktopSrc.Value = DesktopSrc;
					myCommand.Parameters.Add(parameterDesktopSrc);
					SqlParameter parameterMobileSrc = new SqlParameter(strATMobileSrc, SqlDbType.NVarChar, 256);
					parameterMobileSrc.Value = MobileSrc;
					myCommand.Parameters.Add(parameterMobileSrc);
					SqlParameter parameterAssemblyName = new SqlParameter(strATAssemblyName, SqlDbType.VarChar, 50);
					parameterAssemblyName.Value = AssemblyName;
					myCommand.Parameters.Add(parameterAssemblyName);
					SqlParameter parameterClassName = new SqlParameter(strATClassName, SqlDbType.NVarChar, 128);
					parameterClassName.Value = ClassName;
					myCommand.Parameters.Add(parameterClassName);
					SqlParameter parameterAdmin = new SqlParameter(strATAdmin, SqlDbType.Bit);
					parameterAdmin.Value = Admin;
					myCommand.Parameters.Add(parameterAdmin);
					SqlParameter parameterSearchable = new SqlParameter(strATSearchable, SqlDbType.Bit);
					parameterSearchable.Value = Searchable;
					myCommand.Parameters.Add(parameterSearchable);
					// Execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						//LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex));
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex);
					}
				}
			}
		}

		/// <summary>
		/// The UpdateModule method updates a specified Module within the Modules database table.
		/// If the module does not yet exist, the stored procedure adds it.<br />
		/// UpdateModule Stored Procedure
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="moduleOrder"></param>
		/// <param name="paneName"></param>
		/// <param name="title"></param>
		/// <param name="cacheTime"></param>
		/// <param name="editRoles"></param>
		/// <param name="viewRoles"></param>
		/// <param name="addRoles"></param>
		/// <param name="deleteRoles"></param>
		/// <param name="PropertiesRoles"></param>
		/// <param name="moveModuleRoles"></param>
		/// <param name="deleteModuleRoles"></param>
		/// <param name="showMobile"></param>
		/// <param name="publishingRoles"></param>
		/// <param name="supportWorkflow"></param>
		/// <param name="showEveryWhere"></param>
		/// <param name="supportCollapsable"></param>
		/// <param name="ApprovalRoles"></param>
		/// <param name="pageID"></param>
		/// <returns></returns>
		public int UpdateModule(int pageID, int moduleID, int moduleOrder, string paneName, string title, int cacheTime,
			string editRoles, string viewRoles, string addRoles, string deleteRoles, string PropertiesRoles,
			string moveModuleRoles, string deleteModuleRoles, bool showMobile, string publishingRoles,
			bool supportWorkflow, string ApprovalRoles, bool showEveryWhere, bool supportCollapsable) 
		{
			// Changes by Geert.Audenaert@Syntegra.Com Date: 6/2/2003
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModule", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);
					SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
					parameterPageID.Value = pageID;
					myCommand.Parameters.Add(parameterPageID);
					SqlParameter parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
					parameterModuleOrder.Value = moduleOrder;
					myCommand.Parameters.Add(parameterModuleOrder);
					SqlParameter parameterTitle = new SqlParameter(strATModuleTitle, SqlDbType.NVarChar, 256);
					parameterTitle.Value = title;
					myCommand.Parameters.Add(parameterTitle);
					SqlParameter parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
					parameterPaneName.Value = paneName;
					myCommand.Parameters.Add(parameterPaneName);
					SqlParameter parameterCacheTime = new SqlParameter(strATCacheTime, SqlDbType.Int, 4);
					parameterCacheTime.Value = cacheTime;
					myCommand.Parameters.Add(parameterCacheTime);
					SqlParameter parameterEditRoles = new SqlParameter(strATEditRoles, SqlDbType.NVarChar, 256);
					parameterEditRoles.Value = editRoles;
					myCommand.Parameters.Add(parameterEditRoles);
					SqlParameter parameterViewRoles = new SqlParameter(strATViewRoles, SqlDbType.NVarChar, 256);
					parameterViewRoles.Value = viewRoles;
					myCommand.Parameters.Add(parameterViewRoles);
					SqlParameter parameterAddRoles = new SqlParameter(strATAddRoles, SqlDbType.NVarChar, 256);
					parameterAddRoles.Value = addRoles;
					myCommand.Parameters.Add(parameterAddRoles);
					SqlParameter parameterDeleteRoles = new SqlParameter(strATDeleteRoles, SqlDbType.NVarChar, 256);
					parameterDeleteRoles.Value = deleteRoles;
					myCommand.Parameters.Add(parameterDeleteRoles);
					SqlParameter parameterPropertiesRoles = new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
					parameterPropertiesRoles.Value = PropertiesRoles;
					myCommand.Parameters.Add(parameterPropertiesRoles);
					// Added by jviladiu@portalservices.net (19/08/2004)
					SqlParameter parameterMoveModuleRoles = new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
					parameterMoveModuleRoles.Value = moveModuleRoles;
					myCommand.Parameters.Add(parameterMoveModuleRoles);
					// Added by jviladiu@portalservices.net (19/08/2004)
					SqlParameter parameterDeleteModuleRoles = new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
					parameterDeleteModuleRoles.Value = deleteModuleRoles;
					myCommand.Parameters.Add(parameterDeleteModuleRoles);
					// Change by Geert.Audenaert@Syntegra.Com
					// Date: 6/2/2003
					SqlParameter parameterPublishingRoles = new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
					parameterPublishingRoles.Value = publishingRoles;
					myCommand.Parameters.Add(parameterPublishingRoles);
					SqlParameter parameterSupportWorkflow = new SqlParameter(strATSupportWorkflow, SqlDbType.Bit, 1);
					parameterSupportWorkflow.Value = supportWorkflow;
					myCommand.Parameters.Add(parameterSupportWorkflow);
					// End Change Geert.Audenaert@Syntegra.Com
					// Change by Geert.Audenaert@Syntegra.Com
					// Date: 27/2/2003
					SqlParameter parameterApprovalRoles = new SqlParameter(strATApprovalRoles, SqlDbType.NVarChar, 256);
					parameterApprovalRoles.Value = ApprovalRoles;
					myCommand.Parameters.Add(parameterApprovalRoles);
					// End Change Geert.Audenaert@Syntegra.Com
					SqlParameter parameterShowMobile = new SqlParameter(strATShowMobile, SqlDbType.Bit, 1);
					parameterShowMobile.Value = showMobile;
					myCommand.Parameters.Add(parameterShowMobile);
					// Addition by john.mandia@whitelightsolutions.com to add show on every page functionality
					SqlParameter parameterShowEveryWhere = new SqlParameter(strATShowEveryWhere, SqlDbType.Bit, 1);
					parameterShowEveryWhere.Value = showEveryWhere;
					myCommand.Parameters.Add(parameterShowEveryWhere);
					// Change by baj@reedtek.com
					// Date: 16/5/2003
					SqlParameter parameterSupportCollapsable = new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
					parameterSupportCollapsable.Value = supportCollapsable;
					myCommand.Parameters.Add(parameterSupportCollapsable);
					// End Change baj@reedtek.com
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						//LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
					}
					return (int) parameterModuleID.Value;
				}
			}
		}

		/// <summary>
		/// The UpdateModuleDefinitions method updates 
		/// all module definitions in every portal
		/// </summary>
		/// <param name="GeneralModDefID"></param>
		/// <param name="portalID"></param>
		/// <param name="ischecked"></param>
		public void UpdateModuleDefinitions(Guid GeneralModDefID, int portalID, bool ischecked) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleDefinitions", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterGeneralModDefID = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
					parameterGeneralModDefID.Value = GeneralModDefID;
					myCommand.Parameters.Add(parameterGeneralModDefID);
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterischecked = new SqlParameter("@ischecked", SqlDbType.Bit);
					parameterischecked.Value = ischecked;
					myCommand.Parameters.Add(parameterischecked);

					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						//LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
					}
				}
			}
		}

		/// <summary>
		/// The UpdateModuleOrder method update Modules Order.<br />
		/// UpdateModuleOrder Stored Procedure
		/// </summary>
		/// <param name="ModuleID"></param>
		/// <param name="ModuleOrder"></param>
		/// <param name="pane"></param>
		public void UpdateModuleOrder (int ModuleID, int ModuleOrder, string pane) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString) 
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleOrder", myConnection)) 
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
					parameterModuleID.Value = ModuleID;
					myCommand.Parameters.Add(parameterModuleID);
					SqlParameter parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
					parameterModuleOrder.Value = ModuleOrder;
					myCommand.Parameters.Add(parameterModuleOrder);
					SqlParameter parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
					parameterPaneName.Value = pane;
					myCommand.Parameters.Add(parameterPaneName);
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						//LogHelper.Logger.Log(LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
						ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
					}
				}
			}
		}

		//UpdateModuleSetting moved in ModuleConfiguration
		/// <summary>
		/// The UpdateModuleSetting Method updates a single module setting 
		/// in the ModuleSettings database table.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		[Obsolete("UpdateModuleSetting was moved in ModuleSettings.UpdateModuleSetting", false)]
		public void UpdateModuleSetting(int moduleID, string key, string value) 
		{
			ModuleSettings.UpdateModuleSetting(moduleID, key, value);
		}
	}
}
