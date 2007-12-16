using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Rainbow.Framework.Core.BLL;
using Rainbow.Framework.Data.MsSql;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Settings.Cache;
using Rainbow.Framework.Site.Configuration;

namespace Rainbow.Framework.Site.Data
{
    /// <summary>
    /// Class that encapsulates all data logic necessary to add/query/delete
    /// configuration, layout and security settings values within the Portal database.
    /// </summary>
    public class ModulesDB
    {
        #region constants

        private const string strATAddRoles = "@AddRoles";
        private const string strATAdmin = "@Admin";
        private const string strATApprovalRoles = "@ApprovalRoles";
        private const string strATAssemblyName = "@AssemblyName";
        private const string strATCacheTime = "@CacheTime";
        private const string strATClassName = "@ClassName";
        private const string strATDeleteModuleRoles = "@DeleteModuleRoles";
        private const string strATDeleteRoles = "@DeleteRoles";
        private const string strATDesktopSrc = "@DesktopSrc";
        private const string strATEditRoles = "@EditRoles";
        private const string strATFriendlyName = "@FriendlyName";
        private const string strATGeneralModDefID = "@GeneralModDefID";
        private const string strATGuid = "@Guid";
        private const string strATMobileSrc = "@MobileSrc";
        private const string strATModuleDefID = "@ModuleDefID";
        private const string strATModuleID = "@ModuleID";
        private const string strATModuleOrder = "@ModuleOrder";
        private const string strATModuleTitle = "@ModuleTitle";
        private const string strATMoveModuleRoles = "@MoveModuleRoles";
        private const string strATPaneName = "@PaneName";
        private const string strATPortalID = "@PortalID";
        private const string strATPropertiesRoles = "@PropertiesRoles";
        private const string strATPublishingRoles = "@PublishingRoles";
        private const string strATSearchable = "@Searchable";
        private const string strATShowEveryWhere = "@ShowEveryWhere";
        private const string strATShowMobile = "@ShowMobile";
        private const string strATSupportCollapsable = "@SupportCollapsable";
        private const string strATSupportWorkflow = "@SupportWorkflow";
        private const string strATPageID = "@TabID";
        private const string strATViewRoles = "@ViewRoles";
        //const string strGUID = "GUID";
        private const string strNoModule = "NO_MODULE";
        private const string strrb_GetModulesInPage = "rb_GetModulesInTab";

        #endregion

        DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);

        /// <summary>
        /// AddGeneralModuleDefinitions
        /// </summary>
        /// <param name="GeneralModDefID">GeneralModDefID</param>
        /// <param name="FriendlyName">Name of the friendly.</param>
        /// <param name="DesktopSrc">The desktop SRC.</param>
        /// <param name="MobileSrc">The mobile SRC.</param>
        /// <param name="AssemblyName">Name of the assembly.</param>
        /// <param name="ClassName">Name of the class.</param>
        /// <param name="Admin">if set to <c>true</c> [admin].</param>
        /// <param name="Searchable">if set to <c>true</c> [searchable].</param>
        /// <returns>The newly created ID</returns>
        [History("bill@improvtech.com", "2007/12/16", "Updated for LINQ")]
        public Guid AddGeneralModuleDefinitions(Guid GeneralModDefID, string FriendlyName, string DesktopSrc,
                                                string MobileSrc, string AssemblyName, string ClassName, bool Admin,
                                                bool Searchable)
        {
            try
            {
                rb_GeneralModuleDefinition row = db.rb_GeneralModuleDefinitions.Where(d => d.GeneralModDefID == GeneralModDefID).Single();

                //if it hasn't thrown an exception that means the row exists, which is bad.
                ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in AddGeneralModuleDefinitions: The definition you tried to add already exists.");
            }
            catch (ArgumentNullException)
            {
                //this is good, probably change this method later since this is the rule and not really an exception,
                //but this should get thrown and ignored if the row isn't there... we are adding it after all...

                db.rb_AddGeneralModuleDefinitions((Guid?)GeneralModDefID, FriendlyName, DesktopSrc, MobileSrc, AssemblyName,
                ClassName, (bool?)Admin, (bool?)Searchable);
            }

            return GeneralModDefID;
        }

        /// <summary>
        /// The AddModule method updates a specified Module within the Modules database table.
        /// If the module does not yet exist,the stored procedure adds it.<br/>
        /// AddModule Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="moduleOrder">The module order.</param>
        /// <param name="paneName">Name of the pane.</param>
        /// <param name="title">The title.</param>
        /// <param name="moduleDefID">The module def ID.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="editRoles">The edit roles.</param>
        /// <param name="viewRoles">The view roles.</param>
        /// <param name="addRoles">The add roles.</param>
        /// <param name="deleteRoles">The delete roles.</param>
        /// <param name="PropertiesRoles">The properties roles.</param>
        /// <param name="moveModuleRoles">The move module roles.</param>
        /// <param name="deleteModuleRoles">The delete module roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="publishingRoles">The publishing roles.</param>
        /// <param name="supportWorkflow">if set to <c>true</c> [support workflow].</param>
        /// <param name="showEveryWhere">if set to <c>true</c> [show every where].</param>
        /// <param name="supportCollapsable">if set to <c>true</c> [support collapsable].</param>
        /// <returns></returns>
        [History("jviladiu@portalServices.net", "2004/08/19", "Added support for move & delete modules roles")]
        [History("john.mandia@whitelightsolutions.com", "2003/05/24", "Added support for showEveryWhere")]
        [History("bja@reedtek.com", "2003/05/16", "Added support for win. mgmt min/max/close -- supportCollapsable")]
        [History("bill@improvtech.com", "2007/12/16", "Updated for LINQ")]
        public int AddModule(int pageID, int moduleOrder, string paneName, string title, int moduleDefID, int cacheTime,
                             string editRoles, string viewRoles, string addRoles, string deleteRoles,
                             string PropertiesRoles,
                             string moveModuleRoles, string deleteModuleRoles, bool showMobile, string publishingRoles,
                             bool supportWorkflow, bool showEveryWhere, bool supportCollapsable)
        {
            int? moduleID = null;

            db.rb_AddModule((int?)pageID, (int?)moduleOrder, title, paneName, (int?)moduleDefID, (int?)cacheTime, editRoles, addRoles, viewRoles, deleteRoles,
                PropertiesRoles, moveModuleRoles, deleteModuleRoles, (bool?)showMobile, publishingRoles, (bool?)supportWorkflow, (bool?)showEveryWhere,
                (bool?)supportCollapsable, ref moduleID);

            return (int)moduleID;
        }

        /// <summary>
        /// The DeleteModule method deletes a specified Module from the Modules database table.<br/>
        /// DeleteModule Stored Procedure
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        [History("JB - john@bowenweb.com", "2005/05/12", "Added support for Recycler module")]
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public void DeleteModule(int moduleID)
        {
            //BOWEN 11 June 2005 - BEGIN
            PortalSettings portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            bool useRecycler =
                bool.Parse(
                    PortalSettings.GetPortalCustomSettings(portalSettings.PortalID,
                                                           PortalSettings.GetPortalBaseSettings(
                                                               portalSettings.PortalPath))["SITESETTINGS_USE_RECYCLER"].ToString());

            // TODO: THIS LINE DISABLES THE RECYCLER DUE SOME TROUBLES WITH IT !!!!!! Fix those troubles and then discomment.
            useRecycler = false;

            if (useRecycler)
            {
                db.rb_DeleteModuleToRecycler((int?)moduleID, MailHelper.GetCurrentUserEmailAddress(), (DateTime?)DateTime.Now);
            }
            else
            {
                db.rb_DeleteModule((int?)moduleID);
            }
        }

        /// <summary>
        /// The DeleteModuleDefinition method deletes the specified
        /// module type definition from the portal.
        /// </summary>
        /// <param name="defID">The def ID.</param>
        /// <remarks>Other relevant sources: DeleteModuleDefinition Stored Procedure</remarks>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public void DeleteModuleDefinition(Guid defID)
        {
            db.rb_DeleteModuleDefinition((Guid?)defID);
        }

        /// <summary>
        /// Exists the module products in page.
        /// </summary>
        /// <param name="tabID">The tab ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>A bool value...</returns>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public bool ExistModuleProductsInPage(int tabID, int portalID)
        {
            var mods = db.rb_GetModulesInTab((int?)portalID, (int?)tabID);

            Guid moduleGuid = new Guid("{EC24FABD-FB16-4978-8C81-1ADD39792377}");
            bool retorno = false;

            foreach (rb_GetModulesInTabResult mod in mods)
            {
                if (moduleGuid.Equals(mod.GeneralModDefID))
                    retorno = true;
            }

            return retorno;
        }

        /// <summary>
        /// Find module id defined by the guid in a tab in the portal
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public ArrayList FindModuleItemsByGuid(int portalID, Guid guid)
        {
            var mods = db.rb_FindModulesByGuid((int?)portalID, (Guid?)guid);
            ArrayList modList = new ArrayList();

            ModuleItem m = null;

            foreach (var mod in mods)
            {
                m = new ModuleItem();
                m.ID = mod.ModuleID;
                modList.Add(m);
            }

            return modList;
        }

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
            SqlConnection myConnection = (SqlConnection)db.Connection;
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
        /// The GetModuleDefinitions method returns a list of all module type definitions.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetModuleDefinitions Stored Procedure</remarks>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public IList<GeneralModuleDefinition> GetCurrentModuleDefinitionsList(int portalID)
        {
            var defs = db.rb_GetCurrentModuleDefinitions((int?)portalID);

            IList<GeneralModuleDefinition> result = new List<GeneralModuleDefinition>();
            GeneralModuleDefinition genModDef = null;

            foreach (var def in defs)
            {
                genModDef = new GeneralModuleDefinition();

                genModDef.FriendlyName = def.FriendlyName;
                genModDef.DesktopSource = def.DesktopSrc;
                genModDef.MobileSource = def.MobileSrc;
                genModDef.Admin = (bool)def.Admin;
                genModDef.GeneralModDefID = def.ModuleDefID;

                result.Add(genModDef);
            }

            return result;
        }

        /// <summary>
        /// The GetGeneralModuleDefinitionByName method returns the id of the Module
        /// that matches the named Module in general list.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns></returns>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public Guid GetGeneralModuleDefinitionByName(string moduleName)
        {
            Guid? moduleId = null;
            db.rb_GetGeneralModuleDefinitionByName(moduleName, ref moduleId);

            if (moduleId.HasValue)
            {
                return (Guid)moduleId;
            }
            else
            {
                throw new ArgumentException("Null Module GUID!"); // Jes1111
                //Rainbow.Framework.Configuration.ErrorHandler.HandleException("Null GUID!.", new ArgumentException("Null GUID!", strGUID));
            }
            //throw new ArgumentException("Invalid GUID", strGUID);
        }

        /// <summary>
        /// The GetModuleDefinitionByGUID method returns the id of the Module
        /// that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ")]
        public int GetModuleDefinitionByGuid(int portalID, Guid guid)
        {
            int? moduleId = null;
            db.rb_GetModuleDefinitionByGuid((int?)portalID, (Guid?)guid, ref moduleId);

            return (int)moduleId.Value;
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
                        ErrorHandler.Publish(LogLevel.Warn,
                                             "An Error Occurred in GetModuleDefinitionByName. Parameter : " + moduleName,
                                             ex);
                    }
                    return (int)parameterModuleID.Value;
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
        /// Gets the module GUID.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns>A System.Guid value...</returns>
        public Guid GetModuleGuid(int moduleID)
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
                moduleGuid = (Guid)CurrentCache.Get(cacheGuid);
            return moduleGuid;
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
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
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
        /// <param name="PortalID">The portal ID.</param>
        /// <returns></returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
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
        /// <param name="solutionID">The solution ID.</param>
        /// <returns></returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
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
        /// <returns></returns>
        /// <remarks>Other relevant sources: GetUsers Stored Procedure</remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
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
        /// <param name="FriendlyName">Name of the friendly.</param>
        /// <param name="DesktopSrc">The desktop SRC.</param>
        /// <param name="MobileSrc">The mobile SRC.</param>
        /// <param name="AssemblyName">Name of the assembly.</param>
        /// <param name="ClassName">Name of the class.</param>
        /// <param name="Admin">if set to <c>true</c> [admin].</param>
        /// <param name="Searchable">if set to <c>true</c> [searchable].</param>
        public void UpdateGeneralModuleDefinitions(Guid GeneralModDefID, string FriendlyName, string DesktopSrc,
                                                   string MobileSrc, string AssemblyName, string ClassName, bool Admin,
                                                   bool Searchable)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateGeneralModuleDefinitions", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Update Parameters to SPROC
                    SqlParameter parameterGeneralModDefID =
                        new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
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
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex));
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex);
                    }
                }
            }
        }

        /// <summary>
        /// The UpdateModule method updates a specified Module within the Modules database table.
        /// If the module does not yet exist, the stored procedure adds it.<br/>
        /// UpdateModule Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="moduleOrder">The module order.</param>
        /// <param name="paneName">Name of the pane.</param>
        /// <param name="title">The title.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="editRoles">The edit roles.</param>
        /// <param name="viewRoles">The view roles.</param>
        /// <param name="addRoles">The add roles.</param>
        /// <param name="deleteRoles">The delete roles.</param>
        /// <param name="PropertiesRoles">The properties roles.</param>
        /// <param name="moveModuleRoles">The move module roles.</param>
        /// <param name="deleteModuleRoles">The delete module roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="publishingRoles">The publishing roles.</param>
        /// <param name="supportWorkflow">if set to <c>true</c> [support workflow].</param>
        /// <param name="ApprovalRoles">The approval roles.</param>
        /// <param name="showEveryWhere">if set to <c>true</c> [show every where].</param>
        /// <param name="supportCollapsable">if set to <c>true</c> [support collapsable].</param>
        /// <returns></returns>
        public int UpdateModule(int pageID, int moduleID, int moduleOrder, string paneName, string title, int cacheTime,
                                string editRoles, string viewRoles, string addRoles, string deleteRoles,
                                string PropertiesRoles,
                                string moveModuleRoles, string deleteModuleRoles, bool showMobile,
                                string publishingRoles,
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
                    SqlParameter parameterPropertiesRoles =
                        new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
                    parameterPropertiesRoles.Value = PropertiesRoles;
                    myCommand.Parameters.Add(parameterPropertiesRoles);
                    // Added by jviladiu@portalservices.net (19/08/2004)
                    SqlParameter parameterMoveModuleRoles =
                        new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
                    parameterMoveModuleRoles.Value = moveModuleRoles;
                    myCommand.Parameters.Add(parameterMoveModuleRoles);
                    // Added by jviladiu@portalservices.net (19/08/2004)
                    SqlParameter parameterDeleteModuleRoles =
                        new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteModuleRoles.Value = deleteModuleRoles;
                    myCommand.Parameters.Add(parameterDeleteModuleRoles);
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 6/2/2003
                    SqlParameter parameterPublishingRoles =
                        new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
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
                    SqlParameter parameterSupportCollapsable =
                        new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
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
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
                    }
                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The UpdateModuleDefinitions method updates
        /// all module definitions in every portal
        /// </summary>
        /// <param name="GeneralModDefID">The general mod def ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="ischecked">if set to <c>true</c> [ischecked].</param>
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
                    SqlParameter parameterGeneralModDefID =
                        new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
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
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                    }
                }
            }
        }

        /// <summary>
        /// The UpdateModuleOrder method update Modules Order.<br/>
        /// UpdateModuleOrder Stored Procedure
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="ModuleOrder">The module order.</param>
        /// <param name="pane">The pane.</param>
        public void UpdateModuleOrder(int ModuleID, int ModuleOrder, string pane)
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
                        //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
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
        /// <param name="moduleID">The module ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [Obsolete("UpdateModuleSetting was moved in ModuleSettings.UpdateModuleSetting", false)]
        public void UpdateModuleSetting(int moduleID, string key, string value)
        {
            ModuleSettings.UpdateModuleSetting(moduleID, key, value);
        }
    }
}