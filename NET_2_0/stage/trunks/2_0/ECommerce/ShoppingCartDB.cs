using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Rainbow.Configuration;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// IBS Portal ECommerce module
	/// based on Picture Album module and IBS shop
	/// (c)2003 by Tiptopweb, thierry@tiptopweb.com.au
	/// </summary>
	public class ShoppingCartDB 
	{
		/// <summary>
		/// The GetItems method returns a struct containing
		/// a forward-only, read-only DataReader.  This displays a list of all
		/// items within a shopping cart. The SQLDataReaderResult struct
		/// also returns the SQL connection, which must be explicitly closed
		/// after the data from the DataReader is bound into the controls.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="moduleID"></param>
		/// <param name="version"></param>
		/// <returns></returns>
        public SqlDataReader GetItems(string cartID, int moduleID, WorkFlowVersion version) 
		{
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartList", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            parameterCartID.Value = cartID;
            myCommand.Parameters.Add(parameterCartID);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = version == WorkFlowVersion.Production ? 1 : 0;
			myCommand.Parameters.Add(parameterWorkflowVersion);

			// Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader result
            return result;
        }

		/// <summary>
		/// The AddItem method adds an item into a shopping cart.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="productID"></param>
		/// <param name="quantity"></param>
		/// <param name="moduleID"></param>
        public void AddItem(string cartID, int productID, int quantity, int moduleID, string metadataXml)
		{
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartAddItem", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterProductID = new SqlParameter("@ProductID", SqlDbType.Int, 4);
            parameterProductID.Value = productID;
            myCommand.Parameters.Add(parameterProductID);

            SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            parameterCartID.Value = cartID;
            myCommand.Parameters.Add(parameterCartID);

			SqlParameter parameterQuantity = new SqlParameter("@Quantity", SqlDbType.Int, 4);
            parameterQuantity.Value = quantity;
            myCommand.Parameters.Add(parameterQuantity);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterMetadataXml = new SqlParameter("@MetadataXml", SqlDbType.NVarChar, 3000);
			parameterMetadataXml.Value = metadataXml;
			myCommand.Parameters.Add(parameterMetadataXml);

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
		/// The UpdateItem method updates the quantity of an item
		/// in a shopping cart.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="productID"></param>
		/// <param name="quantity"></param>
		/// <param name="moduleID"></param>
        public void UpdateItem(string cartID, int productID, int quantity, int moduleID, string metadataXml) 
		{
            // throw an exception if quantity is a negative number
            if (quantity < 0) 
			{
                throw new Exception("Quantity cannot be a negative number");
            }

            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartUpdate", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterProductID = new SqlParameter("@ProductID", SqlDbType.Int, 4);
            parameterProductID.Value = productID;
            myCommand.Parameters.Add(parameterProductID);

            SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            parameterCartID.Value = cartID;
            myCommand.Parameters.Add(parameterCartID);

            SqlParameter parameterQuantity = new SqlParameter("@Quantity", SqlDbType.Int, 4);
            parameterQuantity.Value = quantity;
            myCommand.Parameters.Add(parameterQuantity);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterMetadataXml = new SqlParameter("@MetadataXml", SqlDbType.NVarChar, 3000);
			parameterMetadataXml.Value = metadataXml;
			myCommand.Parameters.Add(parameterMetadataXml);

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
		/// The RemoveItem method removes an item from a shopping cart.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="productID"></param>
		/// <param name="moduleID"></param>
		public void RemoveItem(string cartID, int productID, int moduleID, string metadataXml) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand myCommand = new SqlCommand("rb_CartRemoveItem", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterProductID = new SqlParameter("@ProductID", SqlDbType.Int, 4);
			parameterProductID.Value = productID;
			myCommand.Parameters.Add(parameterProductID);

			SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
			parameterCartID.Value = cartID;
			myCommand.Parameters.Add(parameterCartID);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterMetadataXml = new SqlParameter("@MetadataXml", SqlDbType.NVarChar, 3000);
			parameterMetadataXml.Value = metadataXml;
			myCommand.Parameters.Add(parameterMetadataXml);

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

		public void RemoveAllItems(string cartID, int moduleID) 
		{

			// Create Instance of Connection and Command Object
			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand myCommand = new SqlCommand("rb_CartRemoveAllItems", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
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
		/// The GetItemCount method returns the number of items
		/// within a shopping cart.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="moduleID"></param>
		/// <returns></returns>
        public int GetItemCount(string cartID, int moduleID) {

            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartItemCount", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            parameterCartID.Value = cartID;
            myCommand.Parameters.Add(parameterCartID);

            // Add Parameters to SPROC
            SqlParameter parameterItemCount = new SqlParameter("@ItemCount", SqlDbType.Int, 4);
            parameterItemCount.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterItemCount);

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

            // Return the ItemCount (obtained as out paramter of SPROC)
            return ((int)parameterItemCount.Value);
        }

		/// <summary>
		/// The GetTotal method returns the total price of all
		/// items within the shopping cart.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="moduleID"></param>
		/// <param name="version"></param>
		/// <returns></returns>
        public Esperantus.Money GetTotal(string cartID, int moduleID, WorkFlowVersion version, bool includeTaxes) 
		{
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartTotal", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            parameterCartID.Value = cartID;
            myCommand.Parameters.Add(parameterCartID);

            SqlParameter parameterIncludeTaxes = new SqlParameter("@IncludeTaxes", SqlDbType.Bit);
            parameterIncludeTaxes.Value = includeTaxes;
            myCommand.Parameters.Add(parameterIncludeTaxes);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			parameterModuleID.Value = moduleID;
			myCommand.Parameters.Add(parameterModuleID);
			
			SqlParameter parameterTotalCost = new SqlParameter("@TotalCost", SqlDbType.Money, 8);
            parameterTotalCost.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterTotalCost);
			
			SqlParameter parameterISOCurrencySymbol = new SqlParameter("@ISOCurrencySymbol", SqlDbType.Char, 3);
            parameterISOCurrencySymbol.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterISOCurrencySymbol);

			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = version == WorkFlowVersion.Production ? 1 : 0;
			myCommand.Parameters.Add(parameterWorkflowVersion);
			
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

            // Return the Total
            if (parameterTotalCost.Value != DBNull.Value && parameterISOCurrencySymbol.Value != DBNull.Value) 
			{
                return new Esperantus.Money((decimal)parameterTotalCost.Value, parameterISOCurrencySymbol.Value.ToString());
            }
			if (parameterISOCurrencySymbol.Value != DBNull.Value) 
			{
				return new Esperantus.Money(0, parameterISOCurrencySymbol.Value.ToString());
			}
			else
			{
				throw new NullReferenceException("Unable to get currency symbol. Be sure you save module settings even when you didn't change anything.");
			}
        }

//		/// <summary>
//		/// The GetTotalShipping method returns the total shipping price of all 
//		/// items within the shopping cart.
//		/// </summary>
//		/// <remarks>Other relevant sources: rb_CartTotalShipping Stored Procedure</remarks>
//		/// <param name="cartID"></param>
//		/// <param name="CountryID"></param>
//		/// <returns></returns>
//		public decimal GetTotalShipping(string cartID, string CountryID, int moduleID, WorkFlowVersion version) 
//		{
//			// Create Instance of Connection and Command Object
//			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
//			SqlCommand myCommand = new SqlCommand("rb_CartTotalShipping", myConnection);
//
//			// Mark the Command as a SPROC
//			myCommand.CommandType = CommandType.StoredProcedure;
//
//			// Add Parameters to SPROC
//			SqlParameter parameterCartID = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
//			parameterCartID.Value = cartID;
//			myCommand.Parameters.Add(parameterCartID);
//
//			SqlParameter parameterCountryID = new SqlParameter("@CountryID", SqlDbType.NChar, 2);
//			parameterCountryID.Value = CountryID;
//			myCommand.Parameters.Add(parameterCountryID);
//
//			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
//			parameterModuleID.Value = moduleID;
//			myCommand.Parameters.Add(parameterModuleID);
//			
//			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
//			parameterWorkflowVersion.Value = version == WorkFlowVersion.Production ? 1 : 0;
//			myCommand.Parameters.Add(parameterWorkflowVersion);
//			
//			SqlParameter parameterTotalShipping = new SqlParameter("@TotalShipping", SqlDbType.Money, 8);
//			parameterTotalShipping.Direction = ParameterDirection.Output;
//			myCommand.Parameters.Add(parameterTotalShipping);
//
//			// Open the connection and execute the Command
//			myConnection.Open();
//		try
//	{
//		myCommand.ExecuteNonQuery();
//	}
//	finally
//{
//	myConnection.Close();
//}
	//
//			// Return the Total
//			if (parameterTotalShipping.Value.ToString().Length > 0) 
//			{
//				return (decimal)parameterTotalShipping.Value;
//			}
//			else 
//			{
//				return 0;
//			}
//		}

		/// <summary>
		/// The MigrateCart method migrates the items from one
		/// cartID to another.  This is used during the login
		/// and/or registration process to transfer a user's
		/// temporary cart items to a permanent account.
		/// </summary>
		/// <param name="oldCartID"></param>
		/// <param name="newCartID"></param>
        public void MigrateCart(String oldCartID, String newCartID) 
		{

            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartMigrate", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter cart1 = new SqlParameter("@OriginalCartID ", SqlDbType.NVarChar, 50);
            cart1.Value = oldCartID;
            myCommand.Parameters.Add(cart1);

            SqlParameter cart2 = new SqlParameter("@NewCartID ", SqlDbType.NVarChar, 50);
            cart2.Value = newCartID;
            myCommand.Parameters.Add(cart2);

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
		/// The EmptyCart method removes all current items within the shopping cart.
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="moduleID"></param>
        public void EmptyCart(string cartID, int moduleID) {

            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            SqlCommand myCommand = new SqlCommand("rb_CartEmpty", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter cartid = new SqlParameter("@CartID", SqlDbType.NVarChar, 50);
            cartid.Value = cartID;
            myCommand.Parameters.Add(cartid);

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

//		private string GetEmailFromIdentity(string name)
//		{
//			string[] uData = name.Split('|');
//			if (uData.GetUpperBound(0) == 2)
//			{
//				return uData[1];
//			}
//			else if (uData.GetUpperBound(0) == 0)
//			{
//				return uData[0];
//			}
//			else
//				throw new Exception("Invalid user");
//		}

		/// <summary>
        /// The GetShoppingCartID method is used to calculate the
        /// "ShoppingCart" ID key used for a tracking a browser.
        ///
        /// The ShoppingCartID value is either the User's Identity
        /// Name (if they are a registered and authenticated user),
        /// or a random GUID calculated for guest visitors or
        /// customers who have not yet logged in.
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentShoppingCartID() 
		{
			// Obtain current HttpContext of ASP+ Request
			System.Web.HttpContext context = System.Web.HttpContext.Current;

			// If the user is authenticated, use their customerID as a permanent shopping cart id
			//            if (context.User.Identity.Name != "") 
			//			{
			//				// extract the email from it
			//                //return GetEmailFromIdentity(context.User.Identity.Name);
			//            }
			if (context.User.Identity != null && PortalSettings.CurrentUser.Identity.Email.Length > 0) 
			{
				return PortalSettings.CurrentUser.Identity.Email;
			}

            // If user is not authenticated, either fetch (or issue) a new temporary cartID
            if (context.Request.Cookies["Rainbow_CartID"] != null) 
			{
                return context.Request.Cookies["Rainbow_CartID"].Value;
            }
            else 
			{
                // Generate a new random GUID using System.Guid Class
                Guid tempCartID = Guid.NewGuid();

                // Send tempCartID back to client as a cookie
                context.Response.Cookies["Rainbow_CartID"].Value = tempCartID.ToString();

                // Return tempCartID
                return tempCartID.ToString();
            }
        }
    }
}