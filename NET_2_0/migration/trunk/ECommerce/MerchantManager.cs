using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;

using Rainbow.DesktopModules;
using Rainbow.Configuration;
using Rainbow.ECommerce.Gateways;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// This class manges all data of Merchant
	/// </summary>
	public class MerchantManager
	{
		public static MerchantData GetMerchant(string merchantID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Rainbow.Settings.Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_EcommerceGetMerchant", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterMerchantID = new SqlParameter("@MerchantID", SqlDbType.NVarChar, 25);
			parameterMerchantID.Value = merchantID;
			myCommand.Parameters.Add(parameterMerchantID);

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            
			MerchantData myMerchantData = null;

			try
			{
				// Read the datareader
				while (result.Read())
				{
					myMerchantData = new MerchantData();

					myMerchantData.MerchantID = merchantID;
					myMerchantData.MerchantName = (string)result["Name"];
					myMerchantData.MerchantEmail = (string)result["MerchantEmail"];
					myMerchantData.TechnicalEmail = (string)result["TechnicalEmail"];

					XmlDocument metadata = new XmlDocument();
					if (result["MetadataXml"] == DBNull.Value)
						metadata.LoadXml("<Metadata></Metadata>");
					else
						metadata.LoadXml((string)result["MetadataXml"]);

					//Loading all custom properties
					XmlElement myElement = metadata.DocumentElement;
					foreach (XmlAttribute property in myElement.Attributes)
					{
						//Cycle all attributes and load it on myMerchantData.CustomSettings
						myMerchantData.CustomSettings[property.Name] = property.Value;
					}
				}
			}
			finally
			{
				result.Close(); //by Manu, fixed bug 807858
			}

			return myMerchantData;
		}

		/// <summary>
		/// Get a reference to specified gateway and
		/// load data for specified merchant in it
		/// </summary>
		/// <param name="gateway"></param>
		/// <returns></returns>
		public static Rainbow.ECommerce.Gateways.GatewayBase LoadMerchantData(Rainbow.ECommerce.Gateways.GatewayBase gateway)
		{
			// If no merchant exists...
			if (gateway.MerchantID == null || gateway.MerchantID.Length == 0)
				throw new ArgumentNullException("MerchantID", "Merchant Not Found");

			// Try to load vendor data from Merchant Table (these are our defaults)
			MerchantData myMerchant = MerchantManager.GetMerchant(gateway.MerchantID);

			gateway.MerchantName = myMerchant.MerchantName;

			// Get Merchant email from Merchant table
			gateway.MerchantEmail = myMerchant.MerchantEmail;
			gateway.TechnicalEmail = myMerchant.TechnicalEmail;

			// Get a type reference to current gateway
			Type MyType = gateway.GetType();
			
			// Now we use reflection for setting all properties
			foreach(string param in myMerchant.CustomSettings.Keys)
			{
				// Seach for a property with this name
				System.Reflection.PropertyInfo Mypropertyinfo = MyType.GetProperty(param);

				// Use the SetValue method to change it.
				if (Mypropertyinfo != null)
				{
					//If we find it we set it (we have to cast to proper type for non string values like booleans)
					Mypropertyinfo.SetValue(gateway, Convert.ChangeType(myMerchant.CustomSettings[param], Mypropertyinfo.PropertyType), null);
				}
				else
				{
					//If we do not find it we add to custom properties collection
					gateway.CustomSettings[param] = myMerchant.CustomSettings[param];
				}
			}

			return gateway;
		}
		
		/// <summary>
		/// Get a reference to specified Shipping and
		/// load data for specified merchant in it
		/// </summary>
		/// <param name="Shipping"></param>
		/// <returns></returns>
		public static ShippingBase LoadMerchantData(ShippingBase Shipping)
		{
			// If no merchant exists...
			if (Shipping.MerchantID == null || Shipping.MerchantID.Length == 0)
				throw new ArgumentNullException("MerchantID", "Merchant Not Found");

			// Try to load vendor data from Merchant Table (these are our defaults)
			MerchantData myMerchant = MerchantManager.GetMerchant(Shipping.MerchantID);

			// Get a type reference to current Shipping
			Type MyType = Shipping.GetType();
			
			// Now we use reflection for setting all properties
			foreach(string param in myMerchant.CustomSettings.Keys)
			{
				// Seach for a property with this name
				System.Reflection.PropertyInfo Mypropertyinfo = MyType.GetProperty(param);

				// Use the SetValue method to change it.
				if (Mypropertyinfo != null)
				{
					if (Mypropertyinfo.PropertyType == typeof(Rainbow.UI.DataTypes.Percentage))
					{
						Mypropertyinfo.SetValue(Shipping, new Rainbow.UI.DataTypes.Percentage(byte.Parse(myMerchant.CustomSettings[param])), null);
					}
					else if (Mypropertyinfo.PropertyType == typeof(Esperantus.Money))
					{
						try
						{
							Mypropertyinfo.SetValue(Shipping, Esperantus.Money.Parse(myMerchant.CustomSettings[param]), null);
						}
						catch(Exception ex)
						{
							Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error,"Shipping: " + Shipping.Name + " - param: " + param  + " - myMerchant[param]: " + myMerchant.CustomSettings[param], ex);
							throw;
						}
					}
					else
					{
						//If we find it we set it (we have to cast to proper type for non string values like booleans)
						Mypropertyinfo.SetValue(Shipping, Convert.ChangeType(myMerchant.CustomSettings[param], Mypropertyinfo.PropertyType), null);
					}
				}
				else
				{
					//If we do not find it we add to custom properties collection
					Shipping.CustomSettings[param] = myMerchant.CustomSettings[param];
				}
			}

			return Shipping;
		}
	}
}