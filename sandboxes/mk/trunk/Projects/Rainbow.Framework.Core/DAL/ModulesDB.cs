using System;
using System.Data;
using System.Data.SqlClient;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Data;
using Rainbow.Framework.Providers;
using System.Collections.Generic;

namespace Rainbow.Framework.Site.Data
{
    /// <summary>
    /// Class that encapsulates all data logic necessary to add/query/delete
    /// configuration, layout and security settings values within the Portal database.
    /// </summary>
    public class ModulesDB
    {
        private const string strATFriendlyName = "@FriendlyName";
        private const string strATGeneralModDefID = "@GeneralModDefID";
        private const string strATGuid = "@Guid";
        private const string strATModuleID = "@ModuleID";
        private const string strATPortalID = "@PortalID";
        private const string strATPageID = "@TabID";
        //const string strGUID = "GUID";
        private const string strNoModule = "NO_MODULE";
        private const string strrb_GetModulesInPage = "rb_GetModulesInTab";

        /// <summary>
        /// Find modules defined by the guid in a tab in the portal
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        [Obsolete("Use FindModuleItemsByGuid instead.")]
        public SqlDataReader FindModulesByGuid(int portalID, Guid guid)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetModuleDefinitions Stored Procedure</remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetCurrentModuleDefinitions(int portalID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        /// The GetModuleDefinitions method returns a list of all module type definitions.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetModuleDefinitions Stored Procedure</remarks>
        public IList<RainbowModuleDefinition> GetCurrentModuleDefinitionsList(int portalID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
            SqlCommand myCommand = new SqlCommand("rb_GetCurrentModuleDefinitions", myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);
            // Open the database connection and execute the command
            myConnection.Open();

            IList<RainbowModuleDefinition> result = new List<RainbowModuleDefinition>();
            RainbowModuleDefinition genModDef;
            using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dr.Read())
                {
                    genModDef = new RainbowModuleDefinition();

                    genModDef.FriendlyName = dr.GetString(0);
                    genModDef.DesktopSource = dr.GetString(1);
                    genModDef.MobileSource = dr.GetString(2);
                    genModDef.Admin = dr.GetBoolean(3);
                    genModDef.GeneralModDefID = dr.GetInt32(4);

                    result.Add(genModDef);
                }
            }
            return result;
        }

        /// <summary>
        /// The GetGeneralModuleDefinitionByName method returns the id of the Module
        /// that matches the named Module in general list.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns></returns>
        public Guid GetGeneralModuleDefinitionByName(string moduleName)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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

                        catch (Exception ex)
                        {
                            throw new Exception(
                                string.Format("'{0}' seems not a valid Module GUID.", parameterModuleID.Value), 
                                ex);
                            // Jes1111
                            //Rainbow.Framework.Configuration.ErrorHandler.HandleException("'" + parameterModuleID.Value.ToString() + "' seems not a valid GUID.", ex);
                            //throw;
                        }
                    }

                    else
                    {
                        throw new ArgumentException("Null Module GUID!"); // Jes1111
                        //Rainbow.Framework.Configuration.ErrorHandler.HandleException("Null GUID!.", new ArgumentException("Null GUID!", strGUID));
                    }
                    //throw new ArgumentException("Invalid GUID", strGUID);
                }
            }
        }

        /// <summary>
        /// The GetModuleDefinitionByName method returns the id of the Module
        /// that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns></returns>
        public int GetModuleDefinitionByName(int portalID, string moduleName)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
                        ErrorHandler.Publish(LogLevel.Warn,
                                             "An Error Occurred in GetModuleDefinitionByName. Parameter : " + moduleName,
                                             ex);
                    }
                    return (int) parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The GetModuleDefinitions method returns a list of all module type
        /// definitions for the portal.<br/>
        /// GetModuleDefinitions Stored Procedure
        /// </summary>
        /// <returns></returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModuleDefinitions()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        /// The GetModuleInUse method returns a list of modules in use with this portal<br/>
        /// GetModuleInUse Stored Procedure
        /// </summary>
        /// <param name="defID">The def ID.</param>
        /// <returns></returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModuleInUse(Guid defID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public DataTable GetModulesAllPortals()
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        /// The GetModuleByName method returns a list of all module with
        /// the specified Name (Type) within the Portal.
        /// It is used to get all instances of a specified module used in a Portal.
        /// e.g. All Image Gallery
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetModuleByName Stored Procedure</remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModulesByName(string moduleName, int portalID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        /// Gets the modules in page.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModulesInPage(int portalID, int pageID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        /// <param name="PortalID">The portal ID.</param>
        /// <returns></returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public DataTable GetModulesSinglePortal(int PortalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
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
        /// The GetSingleModuleDefinition method returns a SqlDataReader
        /// containing details about a specific module definition
        /// from the ModuleDefinitions table.
        /// </summary>
        /// <param name="GeneralModDefID">The general mod def ID.</param>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetSingleModuleDefinition Stored Procedure</remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetSingleModuleDefinition(Guid GeneralModDefID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
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
        /// The GetSolutions method returns the Solution list.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetUsers Stored Procedure</remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetSolutions()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
            SqlCommand myCommand = new SqlCommand("rb_GetSolutions", myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Open the database connection and execute the command
            myConnection.Open();
            SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            // Return the datareader
            return dr;
        }

        //UpdateModuleSetting moved in ModuleConfiguration
        /// <summary>
        /// The UpdateModuleSetting Method updates a single module setting
        /// in the ModuleSettings database table.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [Obsolete("UpdateModuleSetting was moved in ModuleSettings.UpdateModuleSetting", false)]
        public void UpdateModuleSetting(int moduleID, string key, string value)
        {
            RainbowModuleProvider.Instance.UpdateModuleSetting(moduleID, key, value);
        }
    }
}
