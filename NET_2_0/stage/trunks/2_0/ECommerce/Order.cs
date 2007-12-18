using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mail;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;

using Rainbow.DesktopModules;
using Rainbow.ECommerce.Gateways;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// A data class that encapsulates an order
	/// </summary>
	[Serializable()]      
	public class Order 
	{
		public string OrderID;
		public int UserID;

		public ShopperData BillingData;   // Account info
		public ShopperData ShippingData;

		private Esperantus.Money _totalGoods;
		//[XmlElement(DataType = "decimal", Type = typeof(decimal), ElementName = "TotalGoods")]
		public Esperantus.Money TotalGoods
		{
			get
			{
				return _totalGoods;
			}
			set
			{
				_totalGoods = value;
			}
		}
		
		private Esperantus.Money _totalShipping;
		//[XmlElement(DataType = "decimal", Type = typeof(decimal), ElementName = "TotalShipping")]
		public Esperantus.Money TotalShipping
		{
			get
			{
				return _totalShipping;
			}
			set
			{
				_totalShipping = value;
			}
		}
	
		private Esperantus.Money _totalTaxes;
		//[XmlElement(DataType = "decimal", Type = typeof(decimal), ElementName = "TotalTaxes")]
		public Esperantus.Money TotalTaxes
		{
			get
			{
				return _totalTaxes;
			}
			set
			{
				_totalTaxes = value;
			}
		}
		
		private Esperantus.Money _totalExpenses;
		//[XmlElement(DataType = "decimal", Type = typeof(decimal), ElementName = "TotalExpenses")]
		public Esperantus.Money TotalExpenses
		{
			get
			{
				return _totalExpenses;
			}
			set
			{
				_totalExpenses = value;
			}
		}	
	
		public OrderStatus Status;
		public DateTime DateCreated;
		public DateTime DateModified;
		public string PaymentMethod;
		public string ShippingMethod;

		public double TotalWeight
		{
			get{return Items.TotalWeight;}
			set
			{
				// It is read-only because it is a calculated field.
				// We must specify Set because we want it be serialized
			}
		}
		
		
		public string TransactionID;
		public string AuthCode;

		/// <summary>
		/// Total of order
		/// </summary>
		[XmlElement(ElementName = "TotalOrder")]
		public Esperantus.Money Total
		{
			get
			{
				Esperantus.Money mTotal = TotalGoods + TotalTaxes + TotalExpenses + TotalShipping;
				return mTotal;
			}
			set	
			{
				// It is read-only because it is a calculated field.
				// We must specify Set because we want it be serialized
			}
		}

		/// <summary>
		/// Fill the current order with Items in the cart
		/// </summary>
		/// <param name="cartID"></param>
		public void LoadItemFromCart(string cartID, int moduleID, Rainbow.Configuration.WorkFlowVersion version)
		{
			Items.Clear();

			ShoppingCartDB Sh = new ShoppingCartDB();

			SqlDataReader drItems = Sh.GetItems(cartID, moduleID, version);
			try
			{
				while(drItems.Read())
				{
					//Items.Add(new OrderDetail(drItems["ProductID"].ToString(), drItems["ModelName"].ToString(), drItems["ModelNumber"].ToString(), int.Parse(drItems["Quantity"].ToString()), new Esperantus.Money((decimal) drItems["ExtendedAmount"], (string) drItems["ISOCurrencySymbol"]), (double) drItems["Weight"]));
					// Tiptopweb : add the options
					// DecodeOptions(drItems["MetadataXml"].ToString())
					Items.Add(new OrderDetail(drItems["ProductID"].ToString(), drItems["ModelName"].ToString(), OrdersDB.DecodeOptions(drItems["MetadataXml"].ToString()), drItems["ModelNumber"].ToString(), int.Parse(drItems["Quantity"].ToString()), new Esperantus.Money((decimal) drItems["ExtendedAmount"], (string) drItems["ISOCurrencySymbol"]), (double) drItems["Weight"]));
				}
			}
			finally
			{
				drItems.Close();
			}
		}

		/// <summary>
		/// Order details
		/// </summary>
		public OrderDetails Items = new OrderDetails();

		/// <summary>
		/// Contains an HTML representation of the current order
		/// </summary>
		public string HTML
		{
			get
			{
				try
				{
					XmlDocument x = new XmlDocument();
					if (HttpContext.Current == null || HttpContext.Current.Server.MapPath("~/ECommerce/Design/OrderTemplates/HTMLReceipt.xslt").Length <=0)
					{
						Stream st = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Rainbow.ECommerce.Design.OrderTemplates.HTMLReceipt.xslt");
						StreamReader sr = new StreamReader(st);
						x.LoadXml(sr.ReadToEnd());
					}
					else
					{
						// The xsl is in design directory
						x.Load(HttpContext.Current.Server.MapPath("~/ECommerce/Design/OrderTemplates/HTMLReceipt.xslt"));
					}
					return(OrderManager.GetHTML(this, x));
				}
				catch(Exception ex)
				{
					if (OrderID != null && OrderID.Length > 0)
					{
						Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, "Error processing the receipt: " + this.OrderID, ex);
						return("Error processing the receipt: " + this.OrderID);
					}
					else
					{
						Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, "Error processing the receipt: <NO_ORDER>", ex);
						return("Error processing the receipt");
					}
				}
			}
		}

		private string SerializeShopperData(ShopperData sd)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ShopperData));
			MemoryStream m = new MemoryStream();
			byte[] by;
			string stShopperData = "";

			serializer.Serialize(m, sd);
			by = m.ToArray();
			for(int i=0; i<m.Length; i++)
				stShopperData += Convert.ToChar(by[i]);
        
			return stShopperData;
		}			

		private ShopperData DeserializeShopperData(string sd)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ShopperData));
			byte[] by = new byte[sd.Length];
			for(int i=0; i<sd.Length; i++)
				by[i] = Convert.ToByte(Convert.ToChar(sd.Substring(i,1)));
			MemoryStream m = new MemoryStream(by);
			XmlReader Reader = new XmlTextReader(m);
			ShopperData sdata;
			sdata = (ShopperData) serializer.Deserialize(Reader);
			return sdata;
		} 

		/// <summary>
		/// Create an order from the current cart
		/// </summary>
		/// <param name="cartID"></param>
		/// <param name="moduleID"></param>
		public void Save(string cartID, int moduleID)
		{
			OrdersDB order = new OrdersDB();
			// Add New Order
			order.AddOrder(OrderID, moduleID, UserID, TotalGoods, TotalShipping, TotalTaxes, TotalExpenses, TotalGoods.Info.ISOCurrencySymbol, Status, DateTime.Now, DateTime.Now, PaymentMethod, ShippingMethod, TotalWeight, "g", SerializeShopperData(ShippingData), SerializeShopperData(BillingData)); //TODO: Do not 

			// this will migrate the cart to the order details and clear the cart
			order.AddOrderDetails(OrderID, moduleID, cartID);

			// clear and reload the item if we need to use them
			Items.Clear();
			Items.Load(OrderID);
		}

		/// <summary>
		/// Update order, persisting to db
		/// </summary>
		public void Update()
		{
			OrdersDB order = new OrdersDB();
			// Add New Order
			// by Manu fixed bug [ 925881 ] ECommerce CheckOut Bug: rb_Orders Table
			order.UpdateOrder(OrderID, UserID, TotalGoods, TotalTaxes, TotalShipping, TotalExpenses, TotalGoods.Info.ISOCurrencySymbol, Status, PaymentMethod, ShippingMethod, TotalWeight, "g", SerializeShopperData(ShippingData), SerializeShopperData(BillingData)); //Todo: weight "g" is hardcoded here
		}

		/// <summary>
		/// Load order from db
		/// </summary>
		/// <param name="_OrderID"></param>
		public void Load(string _OrderID)
		{
			OrdersDB order = new OrdersDB();
			SqlDataReader drOrder = order.GetSingleOrder(_OrderID);
			try
			{
				if (drOrder.Read())
				{
					OrderID = drOrder["OrderID"].ToString().Trim(); //fixed length bug fixed by Manu
					UserID = Int32.Parse(drOrder["UserID"].ToString());
					ShippingData = DeserializeShopperData(drOrder["ShippingData"].ToString());
					BillingData = DeserializeShopperData(drOrder["BillingData"].ToString());
					TotalGoods = new Esperantus.Money((decimal) drOrder["TotalGoods"], drOrder["ISOCurrencySymbol"].ToString());
					TotalTaxes = new Esperantus.Money((decimal) drOrder["TotalTaxes"], drOrder["ISOCurrencySymbol"].ToString());
					TotalShipping = new Esperantus.Money((decimal) drOrder["TotalShipping"], drOrder["ISOCurrencySymbol"].ToString());
					TotalExpenses = new Esperantus.Money((decimal) drOrder["TotalExpenses"], drOrder["ISOCurrencySymbol"].ToString());
					Status = (OrderStatus) Int32.Parse(drOrder["Status"].ToString());
					DateCreated = DateTime.Parse(drOrder["DateCreated"].ToString());
					DateModified = DateTime.Parse(drOrder["DateModified"].ToString());
					PaymentMethod = drOrder["PaymentMethod"].ToString();
					ShippingMethod = drOrder["ShippingMethod"].ToString();
					TotalWeight = double.Parse(drOrder["TotalWeight"].ToString());
					TransactionID = drOrder["TransactionID"].ToString();
					AuthCode = drOrder["AuthCode"].ToString();
				}
				else
				{
					throw new System.ArgumentException("Missing Order!");
				}
			}
			finally
			{
				drOrder.Close(); //by Manu, fixed bug 807858
			}

			Items.Load(OrderID);
		}

		private string MerchantEmail;
		private string TechnicalEmail;

		private void SendInfoEmail(string Subject, string Body, bool SendToShopper, bool SendToMerchant, bool SendToTechnical)
		{
			//MerchantEmail
			string localMerchantEmail = MerchantEmail;
			if(localMerchantEmail == null || localMerchantEmail == string.Empty)
				throw new ArgumentNullException("MerchantEmail", "MerchantEmail cannot be null!");
			localMerchantEmail += ";";

			//ShopperEmail
			string localShopperEmail = ShippingData.EMail;
			if(localShopperEmail == null || localShopperEmail == string.Empty)
				throw new ArgumentNullException("ShopperEmail", "ShopperEmail cannot be null!");
			localShopperEmail += ";";

			if (BillingData.EMail != localShopperEmail && (BillingData.EMail == null || BillingData.EMail == string.Empty))
				localMerchantEmail += BillingData.EMail + ";";

			//TechnicalEmail
			string localTechnicalEmail = TechnicalEmail;
			if(localTechnicalEmail == null || localTechnicalEmail == string.Empty)
				throw new ArgumentNullException("TechnicalEmail", "TechnicalEmail cannot be null!");
			localTechnicalEmail += ";";
        
			//Prepare Email
			MailMessage mail = new MailMessage();
			mail.From = localMerchantEmail;

			if(SendToShopper)
				mail.To += localShopperEmail;
			if(SendToMerchant)
				mail.Bcc += localMerchantEmail;
			if(SendToTechnical)
				mail.Bcc += localTechnicalEmail;

			//TODO: Provide an alternate way to show order: plain text?
			mail.Subject = Subject;
			mail.Body = Body;
			mail.BodyFormat = MailFormat.Html;
			mail.BodyEncoding = System.Text.UTF8Encoding.UTF8;
            
			//Send Mail
			SmtpMail.SmtpServer = Rainbow.Settings.Config.SmtpServer;
			SmtpMail.Send(mail);      
		}

		/// <summary>
		/// Finalizing order:
		/// The only difference between a real page that finalizes
		/// the order and a simple page that shows only finalizing
		/// informations is in the second parameter that can be true or false.
		/// If false no processing occurs, if true the order is processed 
		/// and confirmation email are sent.
		/// Please note that if something goes wrong an email to the thech
		/// support is ALWAYS sent and if there are two pages
		/// (one false and one true) tech support may receive two email 
		/// of comunication of failed operation.
		/// </summary>
		/// <param name="gateway"></param>
		/// <param name="ProcessOrder">
		/// If false no processing occurs, if true the order is processed 
		/// and confirmation email are sent.
		/// </param>
		/// <returns>"OK your order has been sent" or throws an exception</returns>
		public string Finalize(Rainbow.ECommerce.Gateways.GatewayBase gateway, bool ProcessOrder)
		{
			string OnScreenMessage = "";
			string OnMailMessage = "";

			try
			{
				// IMPORTANT: you must get MerchantID BEFORE this point
				// because we need to load some important settings from configuration
				// that we may need to analyze the url, like secret key for mac...

				// Get Merchant data
				gateway = MerchantManager.LoadMerchantData(gateway);

				// Gets mails from gateway
				MerchantEmail = gateway.MerchantEmail;
				TechnicalEmail = gateway.TechnicalEmail;

				// Check if order is valid
				gateway.AnalyzeURL();

				// Raises an error if gateway returns an error
				// We do it here... right after analyzing url
				if (gateway.ErrorCode != "" && gateway.ErrorCode != "0")
					throw new System.ArgumentException("Geteway error: " + gateway.ErrorCode + "-" + gateway.ErrorDescription);

				// Load order from db
				Load(gateway.OrderID);

				// See if we can find it ant it is valid
				if (OrderID != gateway.OrderID) 
					throw new System.ArgumentException("Unknown Order!");

				// Checks if loaded total matches gateway total
				if (Total != gateway.Price)
					throw new ArgumentOutOfRangeException("Price", gateway.Price, "Wrong Price!");

				// Order completed
				// Process order. It checks status to avoid the order be processed twice
				if (Status != OrderStatus.ToBeCompleted) //It may be the order was submitted twice
					throw new System.ArgumentOutOfRangeException("Status", Status, "Status Failed!");

				//Set Transaction ID and auth code
				TransactionID = gateway.TransactionID;
				AuthCode = gateway.AuthCode;

				//TODO: a better error management. 
				//e.g.: Inform user that the order was already processed
			
				// Set status to order complete
				Status = OrderStatus.SuccessfullyCompleted;	

				// Update Order 
				Update();

				// Send Email To Customer/Merchant/Administrator
				OnMailMessage += HTML;
				SendInfoEmail(Esperantus.Localize.GetString("ORDER_RECEIPT_NUMBER", "Receipt number", null) + ": " + OrderID, OnMailMessage, true, true, true);
			}

			//Something gone wrong!
			catch(Exception ex)
			{
				//if an order is available
				if (OrderID != null && OrderID.Length > 0)
				{
					Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, "Error processing the receipt: " + this.OrderID, ex);

					// Set status to order failed
					Status = OrderStatus.Failed;
	
					// Update Order only 
					Update();

					//Send an email
					SendInfoEmail("Ecommerce fail!", ex.Message + "<br>" + HTML, false, false, true);
				}

				Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, "There was an error processing your order!", ex);

				// Display "Order failed"
				OnScreenMessage = Esperantus.Localize.GetString("ORDER_KO", "There was an error processing your order!", null) + "<p>";
				OnScreenMessage += ex.Message + "<p>";

				// Rethrow exception
				throw new Exception(OnScreenMessage, ex);
			}

			// Display "OK your order has been sent"
			OnScreenMessage = Esperantus.Localize.GetString("ORDER_OK", "Your order was succesfully sent.", null) + "<p>";
			OnScreenMessage += Esperantus.Localize.GetString("ORDER_WAIT_CONFIRM", "Please wait for our Confirm by email.", null) + "<p>";
			OnScreenMessage += HTML;

			return(OnScreenMessage);
		}
	}
}