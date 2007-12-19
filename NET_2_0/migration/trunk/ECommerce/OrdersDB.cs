using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Rainbow.Configuration;

// 4 May 2003, new version adapted from code from Manu, old code commented out
namespace Rainbow.ECommerce
{
	/// <summary>
	/// IBS Portal ECommerce module
	/// Business/Data Logic Class that encapsulates all data
	/// logic necessary to query past orders within the
	/// IBuySpy Orders database.
	/// based on IBS shop and Manu Duemetri code
	/// Adapted by Tiptopweb (Thierry)
	/// </summary>
    public class OrdersDB {

		/// <summary>
		/// AddOrder Method
		/// </summary>
		/// <param name="OrderID"></param>
		/// <param name="moduleID"></param>
		/// <param name="userID"></param>
		/// <param name="TotalGoods"></param>
		/// <param name="TotalShipping"></param>
		/// <param name="TotalTaxes"></param>
		/// <param name="TotalExpenses"></param>
		/// <param name="Status"></param>
		/// <param name="DateCreated"></param>
		/// <param name="DateModified"></param>
		/// <param name="PaymentMethod"></param>
		/// <param name="ShippingMethod"></param>
		/// <param name="TotalWeight"></param>
		/// <param name="ShippingData"></param>
		/// <param name="BillingData"></param>
		public void AddOrder(string OrderID, int moduleID, int userID, decimal TotalGoods, decimal TotalShipping, decimal TotalTaxes, decimal TotalExpenses, string ISOCurrencySymbol, OrderStatus Status, DateTime DateCreated, DateTime DateModified, string PaymentMethod, string ShippingMethod, double TotalWeight, string WeightUnit, string ShippingData, string BillingData) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"]);
			SqlCommand myCommand = new SqlCommand("rb_AddOrder", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterOrderID = new SqlParameter("@OrderID", SqlDbType.Char, 24);
			parameterOrderID.Value = OrderID;
			myCommand.Parameters.Add(parameterOrderID);

			SqlParameter parametermoduleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parametermoduleID.Value = moduleID;
			myCommand.Parameters.Add(parametermoduleID);

			SqlParameter parameteruserID = new SqlParameter("@userID", SqlDbType.Int);
			parameteruserID.Value = userID;
			myCommand.Parameters.Add(parameteruserID);

			SqlParameter parameterTotalGoods = new SqlParameter("@TotalGoods", SqlDbType.Money);
			parameterTotalGoods.Value = TotalGoods;
			myCommand.Parameters.Add(parameterTotalGoods);

			SqlParameter parameterTotalShipping = new SqlParameter("@TotalShipping", SqlDbType.Money);
			parameterTotalShipping.Value = TotalShipping;
			myCommand.Parameters.Add(parameterTotalShipping);

			SqlParameter parameterTotalTaxes = new SqlParameter("@TotalTaxes", SqlDbType.Money);
			parameterTotalTaxes.Value = TotalTaxes;
			myCommand.Parameters.Add(parameterTotalTaxes);

			SqlParameter parameterTotalExpenses = new SqlParameter("@TotalExpenses", SqlDbType.Money);
			parameterTotalExpenses.Value = TotalExpenses;
			myCommand.Parameters.Add(parameterTotalExpenses);

			SqlParameter parameterStatus = new SqlParameter("@Status", SqlDbType.Int);
			parameterStatus.Value = Status;
			myCommand.Parameters.Add(parameterStatus);

			SqlParameter parameterDateCreated = new SqlParameter("@DateCreated", SqlDbType.DateTime);
			parameterDateCreated.Value = DateCreated;
			myCommand.Parameters.Add(parameterDateCreated);
	            
			SqlParameter parameterDateModified = new SqlParameter("@DateModified", SqlDbType.DateTime);
			parameterDateModified.Value = DateModified;
			myCommand.Parameters.Add(parameterDateModified);
	            
			SqlParameter parameterPaymentMethod = new SqlParameter("@PaymentMethod", SqlDbType.NVarChar,50);
			parameterPaymentMethod.Value = PaymentMethod;
			myCommand.Parameters.Add(parameterPaymentMethod);

			SqlParameter parameterShippingMethod = new SqlParameter("@ShippingMethod", SqlDbType.NVarChar,50);
			parameterShippingMethod.Value = ShippingMethod;
			myCommand.Parameters.Add(parameterShippingMethod);

			SqlParameter parameterTotalWeight = new SqlParameter("@TotalWeight", SqlDbType.Real);
			parameterTotalWeight.Value = TotalWeight;
			myCommand.Parameters.Add(parameterTotalWeight);
			
			SqlParameter parameterShippingData = new SqlParameter("@ShippingData", SqlDbType.NText);
			parameterShippingData.Value = ShippingData;
			myCommand.Parameters.Add(parameterShippingData);

			SqlParameter parameterBillingData = new SqlParameter("@BillingData", SqlDbType.NText);
			parameterBillingData.Value = BillingData;
			myCommand.Parameters.Add(parameterBillingData);

			SqlParameter parameterISOCurrencySymbol = new SqlParameter("@ISOCurrencySymbol", SqlDbType.Char, 3);
			parameterISOCurrencySymbol.Value = ISOCurrencySymbol;
			myCommand.Parameters.Add(parameterISOCurrencySymbol);

			SqlParameter parameterWeightUnit = new SqlParameter("@WeightUnit", SqlDbType.NVarChar, 15);
			parameterWeightUnit.Value = WeightUnit;
			myCommand.Parameters.Add(parameterWeightUnit);

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

		/// <summary>
		/// UpdateOrder
		/// </summary>
		/// <param name="OrderID"></param>
		/// <param name="userID"></param>
		/// <param name="TotalGoods"></param>
		/// <param name="TotalShipping"></param>
		/// <param name="TotalTaxes"></param>
		/// <param name="TotalExpenses"></param>
		/// <param name="Status"></param>
		/// <param name="PaymentMethod"></param>
		/// <param name="ShippingMethod"></param>
		/// <param name="TotalWeight"></param>
		/// <param name="ShippingData"></param>
		/// <param name="BillingData"></param>
		public void UpdateOrder(string OrderID, int userID, decimal TotalGoods, decimal TotalTaxes, decimal TotalShipping, decimal TotalExpenses, string ISOCurrencySymbol, OrderStatus Status, string PaymentMethod, string ShippingMethod, double TotalWeight, string WeightUnit, string ShippingData, string BillingData) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"]);
			SqlCommand myCommand = new SqlCommand("rb_UpdateOrder", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterOrderID = new SqlParameter("@OrderID", SqlDbType.Char, 24);
			parameterOrderID.Value = OrderID;
			myCommand.Parameters.Add(parameterOrderID);

			SqlParameter parameteruserID = new SqlParameter("@userID", SqlDbType.Int);
			parameteruserID.Value = userID;
			myCommand.Parameters.Add(parameteruserID);

			SqlParameter parameterTotalGoods = new SqlParameter("@TotalGoods", SqlDbType.Money);
			parameterTotalGoods.Value = TotalGoods;
			myCommand.Parameters.Add(parameterTotalGoods);

			SqlParameter parameterTotalShipping = new SqlParameter("@TotalShipping", SqlDbType.Money);
			parameterTotalShipping.Value = TotalShipping;
			myCommand.Parameters.Add(parameterTotalShipping);

			SqlParameter parameterTotalTaxes = new SqlParameter("@TotalTaxes", SqlDbType.Money);
			parameterTotalTaxes.Value = TotalTaxes;
			myCommand.Parameters.Add(parameterTotalTaxes);

			SqlParameter parameterTotalExpenses = new SqlParameter("@TotalExpenses", SqlDbType.Money);
			parameterTotalExpenses.Value = TotalExpenses;
			myCommand.Parameters.Add(parameterTotalExpenses);

			SqlParameter parameterStatus = new SqlParameter("@Status", SqlDbType.Int);
			parameterStatus.Value = Status;
			myCommand.Parameters.Add(parameterStatus);

			SqlParameter parameterPaymentMethod = new SqlParameter("@PaymentMethod", SqlDbType.NVarChar,50);
			parameterPaymentMethod.Value = PaymentMethod;
			myCommand.Parameters.Add(parameterPaymentMethod);

			SqlParameter parameterShippingMethod = new SqlParameter("@ShippingMethod", SqlDbType.NVarChar,50);
			parameterShippingMethod.Value = ShippingMethod;
			myCommand.Parameters.Add(parameterShippingMethod);

			SqlParameter parameterTotalWeight = new SqlParameter("@TotalWeight", SqlDbType.Real);
			parameterTotalWeight.Value = TotalWeight;
			myCommand.Parameters.Add(parameterTotalWeight);
			
			SqlParameter parameterShippingData = new SqlParameter("@ShippingData", SqlDbType.NText);
			parameterShippingData.Value = ShippingData;
			myCommand.Parameters.Add(parameterShippingData);

			SqlParameter parameterBillingData = new SqlParameter("@BillingData", SqlDbType.NText);
			parameterBillingData.Value = BillingData;
			myCommand.Parameters.Add(parameterBillingData);
			
			SqlParameter parameterISOCurrencySymbol = new SqlParameter("@ISOCurrencySymbol", SqlDbType.Char, 3);
			parameterISOCurrencySymbol.Value = ISOCurrencySymbol;
			myCommand.Parameters.Add(parameterISOCurrencySymbol);

			SqlParameter parameterWeightUnit = new SqlParameter("@WeightUnit", SqlDbType.NVarChar, 15);
			parameterWeightUnit.Value = WeightUnit;
			myCommand.Parameters.Add(parameterWeightUnit);

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

		/// <summary>
		/// GetSingleOrder Method
		/// </summary>
		/// <param name="OrderID"></param>
		/// <returns></returns>
		public SqlDataReader GetSingleOrder(string OrderID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSingleOrder", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterOrderID = new SqlParameter("@OrderID", SqlDbType.Char, 24);
			parameterOrderID.Value = OrderID;
			myCommand.Parameters.Add(parameterOrderID);

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            
			// Return the datareader 
			return result;
		}

		/// <summary>
		/// GetOrders
		/// </summary>
		/// <param name="ModuleID"></param>
		/// <returns></returns>
		public SqlDataReader GetOrders(int ModuleID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetOrders", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            
			// Return the datareader 
			return result;
		}

		/// <summary>
		/// GetOrdersByUser
		/// </summary>
		/// <param name="ModuleID"></param>
		/// <param name="UserID"></param>
		/// <returns></returns>
		public SqlDataReader GetOrdersByUser(int ModuleID, int UserID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetOrdersByUser", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
			parameterUserID.Value = UserID;
			myCommand.Parameters.Add(parameterUserID);
            
			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            
			// Return the datareader 
			return result;
		}

		/// <summary>
		/// GetOrderItems
		/// </summary>
		/// <param name="OrderID"></param>
		/// <returns></returns>
		public SqlDataReader GetOrderDetails(string OrderID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetOrderDetails", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterOrderID = new SqlParameter("@OrderID", SqlDbType.Char, 24);
			parameterOrderID.Value = OrderID;
			myCommand.Parameters.Add(parameterOrderID);

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            
			// Return the datareader 
			return result;
		}

		/// <summary>
		/// OrdersDB.AddOrderDetails() Method <a name="AddOrderDetails"></a>
		/// The AddOrderDetails method places an order within the
		/// IBuySpy Orders Database and then clears out the current
		/// items within the shopping cart.
		/// 
		/// OrdersAdd Stored Procedure
		/// </summary>
		/// <param name="orderID"></param>
		/// <param name="moduleID"></param>
		/// <param name="cartID"></param>
        public void AddOrderDetails(string orderID, int moduleID, string cartID) 
        {
            // Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_AddOrderDetails", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterOrderID = new SqlParameter("@OrderID", SqlDbType.Char, 24);
            parameterOrderID.Value = orderID;
            myCommand.Parameters.Add(parameterOrderID);

            SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            parameterCartID.Value = cartID;
            myCommand.Parameters.Add(parameterCartID);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

            // Open the connection and execute the Command
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

		/// <summary>
		/// OrdersDB.GetUserForOrder() Method <a name="GetUserForOrder"></a>
		/// The PlaceOrder method places an order within the
		/// IBuySpy Orders Database and then clears out the current
		/// items within the shopping cart.
		/// 
		/// GetUserForOrder Stored Procedure
		/// </summary>
		/// <param name="orderID"></param>
		/// <returns></returns>
		public int GetUserForOrder(int orderID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetUserForOrder", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC

			SqlParameter parameterOrderID = new SqlParameter("@OrderID", SqlDbType.Int);
			parameterOrderID.Value = orderID;
			myCommand.Parameters.Add(parameterOrderID);

			// output parameter
			SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
			parameterUserID.Direction = ParameterDirection.Output;
			myCommand.Parameters.Add(parameterUserID);

			// Open the connection and execute the Command
			myConnection.Open();
			try
			{
				myCommand.ExecuteNonQuery();
			}
			finally
			{
				myConnection.Close();
			}

			return (int) parameterUserID.Value;
		}

		/// <summary>
		/// Decode the options xml string, extract the selected options.
		/// The formatting is coming from the Option.ascx control
		/// </summary>
		/// <param name="metadataXml"></param>
		/// <returns></returns>
		public static string DecodeOptions(string metadataXml)
		{
			string selectedOptions = "";

			//Create a xml Document
			XmlDocument myXmlDoc = new XmlDocument();

			if(metadataXml != null && metadataXml.Length > 0)
			{
				myXmlDoc.LoadXml(metadataXml);

				XmlNode foundNode1 = myXmlDoc.SelectSingleNode("options/option1/selected");
				if(foundNode1 != null)
				{
					selectedOptions += foundNode1.InnerText;
				}

				XmlNode foundNode2 = myXmlDoc.SelectSingleNode("options/option2/selected");
				if(foundNode2 != null)
				{
					selectedOptions += " - " + foundNode2.InnerText;
				}

				XmlNode foundNode3 = myXmlDoc.SelectSingleNode("options/option3/selected");
				if(foundNode3 != null)
				{
					selectedOptions += " - " + foundNode3.InnerText;
				}
			}

			return selectedOptions;
		}
    }
}