//  Originally Based on IBuySpy Store
//  Modified by Tiptopweb for Rainbow using the Picture Album Module from Rainbow
//  SP: Shop_ProductCategoryList (based on GetTabsFlat by Cory Isakson)
// --------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using Rainbow.Configuration;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// IBS Portal ECommerce module
	/// based on Picture Album module and IBS shop
	/// (c)2003 by Tiptopweb, thierry@tiptopweb.com.au
	/// </summary>
	public class ProductsDB
	{
		public SqlDataReader GetProductsCategoryList(int ModuleID)
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand myCommand = new SqlCommand("rb_GetProductsCategoryList", myConnection);

			//  Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterModuleID= new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			//  Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			//  return the datareader result
			return result;
		}

		public DataSet GetProductsCategoryListDataSet(int ModuleID)
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetProductsCategoryList", myConnection);
			myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

			//  Mark the Command as a SPROC
			myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterModuleID= new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = ModuleID;
			myCommand.SelectCommand.Parameters.Add(parameterModuleID);

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myCommand.Fill(myDataSet);
			}
			finally
			{
				myConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}

		public DataSet GetProducts(int moduleID, int categoryID, WorkFlowVersion version) 
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetProducts", myConnection);
			myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.SelectCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterCategoryID = new SqlParameter("@CategoryID", SqlDbType.Int, 4);
			parameterCategoryID.Value = categoryID;
			myCommand.SelectCommand.Parameters.Add(parameterCategoryID);

			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = version == WorkFlowVersion.Production ? 1 : 0;
			myCommand.SelectCommand.Parameters.Add(parameterWorkflowVersion);

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myCommand.Fill(myDataSet);
			}
			finally
			{
				myConnection.Close();
			}


			//  return the DataSet
			return myDataSet;
		}

		public DataSet GetProductsPaged(int moduleID, int categoryID, int page, int recordsPerPage, WorkFlowVersion version) 
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetProductsPaged", myConnection);
			myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.SelectCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterCategoryID = new SqlParameter("@CategoryID", SqlDbType.Int, 4);
			parameterCategoryID.Value = categoryID;
			myCommand.SelectCommand.Parameters.Add(parameterCategoryID);

			SqlParameter parameterPage = new SqlParameter("@Page", SqlDbType.Int, 4);
			parameterPage.Value = page;
			myCommand.SelectCommand.Parameters.Add(parameterPage);

			SqlParameter parameterRecordsPerPage = new SqlParameter("@RecordsPerPage", SqlDbType.Int, 4);
			parameterRecordsPerPage.Value = recordsPerPage;
			myCommand.SelectCommand.Parameters.Add(parameterRecordsPerPage);

			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = version == WorkFlowVersion.Production ? 1 : 0;
			myCommand.SelectCommand.Parameters.Add(parameterWorkflowVersion);

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myCommand.Fill(myDataSet);
			}
			finally
			{
				myConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}

		public SqlDataReader GetSingleProduct(int productID, WorkFlowVersion version) 
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand  = new SqlCommand("rb_GetSingleProduct", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterProductID  = new SqlParameter("@ProductID", SqlDbType.Int, 4);
			parameterProductID.Value = productID;
			myCommand.Parameters.Add(parameterProductID);

			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = version == WorkFlowVersion.Production ? 1 : 0;
			myCommand.Parameters.Add(parameterWorkflowVersion);

			//  Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			//  return the datareader 
			return result;
		}

		public void UpdateProduct(int moduleID, int productID, int displayOrder, String metadataXml, String shortDescription, String longDescription, String modelName, String modelNumber, double unitPrice, Byte featuredItem, int categoryID, double weight, double taxRate) 
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand  = new SqlCommand("rb_UpdateProduct", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterProductID  = new SqlParameter("@ProductID", SqlDbType.Int, 4);
			parameterProductID.Value = productID;
			myCommand.Parameters.Add(parameterProductID);

			SqlParameter parameterModuleID  = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterCategoryID  = new SqlParameter("@CategoryID", SqlDbType.Int, 4);
			parameterCategoryID.Value = categoryID;
			myCommand.Parameters.Add(parameterCategoryID);

			SqlParameter parameterModelName  = new SqlParameter("@ModelName", SqlDbType.NVarChar, 256);
			parameterModelName.Value = modelName;
			myCommand.Parameters.Add(parameterModelName);

			SqlParameter parameterModelNumber  = new SqlParameter("@ModelNumber", SqlDbType.NVarChar, 256);
			parameterModelNumber.Value = modelNumber;
			myCommand.Parameters.Add(parameterModelNumber);

			SqlParameter parameterUnitPrice  = new SqlParameter("@UnitPrice", SqlDbType.Money);
			parameterUnitPrice.Value = unitPrice;
			myCommand.Parameters.Add(parameterUnitPrice);

			SqlParameter parameterFeaturedItem  = new SqlParameter("@FeaturedItem", SqlDbType.Bit);
			parameterFeaturedItem.Value = featuredItem;
			myCommand.Parameters.Add(parameterFeaturedItem);

			SqlParameter parameterDisplayOrder = new SqlParameter("@DisplayOrder", SqlDbType.Int, 4);
			parameterDisplayOrder.Value = displayOrder;
			myCommand.Parameters.Add(parameterDisplayOrder);

			SqlParameter parameterMetadataXml = new SqlParameter("@MetadataXml", SqlDbType.NText);
			parameterMetadataXml.Value = metadataXml;
			myCommand.Parameters.Add(parameterMetadataXml);

			SqlParameter parameterShortDescription = new SqlParameter("@ShortDescription", SqlDbType.NText);
			parameterShortDescription.Value = shortDescription;
			myCommand.Parameters.Add(parameterShortDescription);

			SqlParameter parameterLongDescription = new SqlParameter("@LongDescription", SqlDbType.NText);
			parameterLongDescription.Value = longDescription;
			myCommand.Parameters.Add(parameterLongDescription);

			SqlParameter parameterWeight  = new SqlParameter("@Weight", SqlDbType.Float);
			parameterWeight.Value = weight;
			myCommand.Parameters.Add(parameterWeight);

			SqlParameter parameterTaxRate  = new SqlParameter("@TaxRate", SqlDbType.Float);
			parameterTaxRate.Value = taxRate;
			myCommand.Parameters.Add(parameterTaxRate);

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

		public void DeleteProduct(int productID)
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand  = new SqlCommand("rb_DeleteProduct", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterProductID  = new SqlParameter("@ProductID", SqlDbType.Int, 4);
			parameterProductID.Value = productID;
			myCommand.Parameters.Add(parameterProductID);

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

		public void MoveProducts(String categoryID, String newCategoryID)
		{
			//  Create Instance of Connection and Command Object
			SqlConnection myConnection  = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"]);
			SqlCommand myCommand  = new SqlCommand("rb_ProductsChangeCategory", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			//  Add Parameters to SPROC
			SqlParameter parameterCategoryID  = new SqlParameter("@CategoryID", SqlDbType.Int, 4);
			parameterCategoryID.Direction = ParameterDirection.Input;
			parameterCategoryID.Value = categoryID;
			myCommand.Parameters.Add(parameterCategoryID);

			SqlParameter parameterNewCategoryID  = new SqlParameter("@NewCategoryID", SqlDbType.Int, 4);
			parameterNewCategoryID.Direction = ParameterDirection.Input;
			parameterNewCategoryID.Value = newCategoryID;
			myCommand.Parameters.Add(parameterNewCategoryID);

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
