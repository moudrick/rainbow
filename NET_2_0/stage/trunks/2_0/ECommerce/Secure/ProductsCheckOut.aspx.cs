using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Mail;
using System.Configuration;
using System.Xml;

using Esperantus;
using Rainbow.Security;
using Rainbow.Configuration;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Admin;
using Rainbow.DesktopModules;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Summary description for ProductsCheckOut.
	/// </summary>
	public class ProductsCheckOut : Rainbow.ECommerce.UI.SecurePage
	{
		private int _moduleID;

		protected Esperantus.WebControls.Label MyError;
		protected Esperantus.WebControls.Label Message01;
		protected Esperantus.WebControls.Label Panel1Label;
		protected Esperantus.WebControls.Label NameLabel;
		protected System.Web.UI.WebControls.TextBox NameField;
		protected Esperantus.WebControls.RequiredFieldValidator RequiredName;
		protected Esperantus.WebControls.Label CompanyLabel;
		protected System.Web.UI.WebControls.TextBox CompanyField;
		protected Esperantus.WebControls.Label AddressLabel;
		protected System.Web.UI.WebControls.TextBox AddressField;
		protected Esperantus.WebControls.Label CityLabel;
		protected System.Web.UI.WebControls.TextBox CityField;
		protected Esperantus.WebControls.Label ZipLabel;
		protected System.Web.UI.WebControls.TextBox ZipField;
		protected Esperantus.WebControls.Label CountryLabel;
		protected System.Web.UI.WebControls.DropDownList CountryField;
		protected Esperantus.WebControls.Label StateLabel;
		protected System.Web.UI.WebControls.DropDownList StateField;
		protected Esperantus.WebControls.Label InLabel;
		protected Esperantus.WebControls.Label ThisCountryLabel;
		protected Esperantus.WebControls.Label PhoneLabel;
		protected System.Web.UI.WebControls.TextBox PhoneField;
		protected Esperantus.WebControls.Label FaxLabel;
		protected System.Web.UI.WebControls.TextBox FaxField;
		protected Esperantus.WebControls.Label PIvaLabel;
		protected System.Web.UI.WebControls.TextBox PIvaField;
		protected Esperantus.WebControls.Label CFiscaleLabel;
		protected System.Web.UI.WebControls.TextBox CFiscaleField;
		protected Esperantus.WebControls.Label SendNewsletterLabel;
		protected System.Web.UI.WebControls.CheckBox SendNewsletter;
		protected Esperantus.WebControls.Label EmailLabel;
		protected System.Web.UI.WebControls.TextBox EmailField;
		protected Esperantus.WebControls.RegularExpressionValidator ValidEmail;
		protected Esperantus.WebControls.RequiredFieldValidator RequiredEmail;
		protected Esperantus.WebControls.Literal addressCorrect;
		protected Esperantus.WebControls.Literal addressCorrect2;
		protected Esperantus.WebControls.LinkButton UpdateProfileBtn;
		protected Esperantus.WebControls.LinkButton Panel1BackBtn;
		protected Esperantus.WebControls.LinkButton Panel1CancelBtn;
		protected Esperantus.WebControls.LinkButton Panel1ContinueBtn;
		protected System.Web.UI.WebControls.Panel PanelStep1;
		protected Esperantus.WebControls.Label Panel2Label;
		protected Esperantus.WebControls.LinkButton CopyShippingBtn;
		protected Esperantus.WebControls.Label ShipNameLabel;
		protected Esperantus.WebControls.RequiredFieldValidator ShipRequiredName;
		protected Esperantus.WebControls.Label ShipCompanyLabel;
		protected System.Web.UI.WebControls.TextBox ShipCompanyField;
		protected Esperantus.WebControls.Label ShipAddressLabel;
		protected System.Web.UI.WebControls.TextBox ShipAddressField;
		protected Esperantus.WebControls.Label ShipCityLabel;
		protected System.Web.UI.WebControls.TextBox ShipCityField;
		protected Esperantus.WebControls.Label ShipZipLabel;
		protected System.Web.UI.WebControls.TextBox ShipZipField;
		protected Esperantus.WebControls.Label ShipCountryLabel;
		protected System.Web.UI.WebControls.DropDownList ShipCountryField;
		protected Esperantus.WebControls.Label ShipStateLabel;
		protected System.Web.UI.WebControls.DropDownList ShipStateField;
		protected Esperantus.WebControls.Label ShipInLabel;
		protected Esperantus.WebControls.Label ShipThisCountryLabel;
		protected Esperantus.WebControls.Label SPhoneLabel;
		protected System.Web.UI.WebControls.TextBox ShipPhoneField;
		protected Esperantus.WebControls.Label SFaxLabel;
		protected System.Web.UI.WebControls.TextBox ShipFaxField;
		protected Esperantus.WebControls.LinkButton Panel2BackBtn;
		protected Esperantus.WebControls.LinkButton Panel2CancelBtn;
		protected Esperantus.WebControls.LinkButton Panel2ContinueBtn;
		protected System.Web.UI.WebControls.Panel PanelStep2;
		protected Esperantus.WebControls.Label Panel3Label;
		protected System.Web.UI.WebControls.DataGrid MyCartList;
		protected Esperantus.WebControls.Label lblTotalShippingLabel;
		protected Esperantus.WebControls.Label lblSelectPayment;
		protected System.Web.UI.WebControls.DropDownList ddSelectPayment;
		protected Esperantus.WebControls.LinkButton Panel3BackBtn;
		protected Esperantus.WebControls.LinkButton Panel3CancelBtn;
		protected Esperantus.WebControls.LinkButton Panel3ContinueBtn;
		protected System.Web.UI.WebControls.Panel PanelStep3;
		protected System.Web.UI.WebControls.Panel GatewayForm;
		protected System.Web.UI.HtmlControls.HtmlTableRow StateRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow PIvaRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow CFiscaleRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow ShipStateRow;
		protected System.Web.UI.WebControls.TextBox ShipNameField;
		protected Esperantus.WebControls.Label Message02;
		protected System.Web.UI.HtmlControls.HtmlTableRow ChooseCheckoutType;
		protected Esperantus.WebControls.Label lblTotalShippingField;
		protected System.Web.UI.WebControls.RadioButtonList ShippingList;
		protected Esperantus.WebControls.Label TotalLabel;
		protected System.Web.UI.WebControls.Label lblTotalWithTaxes;
		protected System.Web.UI.WebControls.Label lblTotalTaxes;
		protected System.Web.UI.WebControls.Label lblTotal;
		protected Esperantus.WebControls.Label Label1;
		protected Esperantus.WebControls.Label Label2;

		private string currencySymbol
		{
			get
			{
				return moduleSettings["Currency"].ToString();
			}		
		}

		/// <summary>
		/// Set the module guids with free access to this page
		/// </summary>
		protected override ArrayList AllowedModules
		{
			get
			{
				ArrayList al = new ArrayList();
				al.Add ("EC24FABD-FB16-4978-8C81-1ADD39792377");
				return al;
			}
		}

		// Thierry (Tiptopweb), 4 May 2003
		//***************************************************************
		/// <summary>
		/// IMPORTANT: we need the workflow version to read the cart
		/// the productID will be found in rb_ProductStaging or rb_Product depending
		/// if workflow enabled or not.
		/// we do not want to propagate the wversion, find a better solution
		/// for time being check out not with workflow... using rb_ProductStaging
		/// </summary>
		private WorkFlowVersion _version = WorkFlowVersion.Staging;

		#region Panels Visible
		private void ShowPanelStep(int step)
		{
			// Switch between steps, navigation buttons with the panel
			// navigation for step3 in the Gateway form
			switch(step)
			{
				case 0:
					Message01.Visible = false;  
					Message02.Visible = false;  
					PanelStep1.Visible = false;    
					PanelStep2.Visible = false;   
					PanelStep3.Visible = false;   
//					PanelStep4.Visible = false;   
					GatewayForm.Visible = false;
					break;
				case 1:
					Message01.Visible = true;  
					Message02.Visible = true;  
					PanelStep1.Visible = true;    
					PanelStep2.Visible = false;   
					PanelStep3.Visible = false;   
//					PanelStep4.Visible = false;   
					GatewayForm.Visible = false;
					break;
				case 2:
					PanelStep1.Visible = false;    
					PanelStep2.Visible = true;   
					PanelStep3.Visible = false;   
//					PanelStep4.Visible = false;   
//					GatewayForm.Visible = false;
					break;
				case 3:
					PanelStep1.Visible = false;    
					PanelStep2.Visible = false;   
					PanelStep3.Visible = true;   
					ChooseCheckoutType.Visible = true;
					Panel3BackBtn.Visible = true;
//					PanelStep4.Visible = false;   
//					GatewayForm.Visible = false; //by Manu: was true
					break;
				case 4:
					PanelStep1.Visible = false;    
					PanelStep2.Visible = false;   
					PanelStep3.Visible = true;  
					ChooseCheckoutType.Visible = false; //by Manu: this is the step 4
					Panel3BackBtn.Visible = false; //Cannot go back
//					PanelStep4.Visible = true;   
					GatewayForm.Visible = true;
					break;
				default:  // pb, all invisible
					PanelStep1.Visible = false;    
					PanelStep2.Visible = false;   
					PanelStep3.Visible = false;   
//					PanelStep4.Visible = false;   
//					GatewayForm.Visible = false;
					break;
			}
		}
		#endregion

		/// <summary>
		/// Structure used for list settings
		/// </summary>
		public struct KeyName
		{
			private string key;
			private string name;

			public string Key 
			{
				get { return this.key; }
				set { this.key = value; }
			}

			public string Name
			{
				get { return this.name; }
				set { this.name = value; }
			}

			public KeyName(string key, string name)
			{
				this.key = key;
				this.name = name;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			//Prevent back button by Manu
			//We want to avoid using back button of browser....
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			// get shop moduleID
			_moduleID = int.Parse(Request.Params["mID"]);
			_version = WorkFlowVersion.Staging;

			// TODO: add some security check like checking that this module is a shop
			if (Page.IsPostBack == false) 
			{
				// readonly for the email field!
				// the user will have to go to change profile
				EmailField.Enabled = false;

				try
				{
					try
					{
						#region Binding the list of available Gateways
						// This list is Dynamic and reflects installed gateways in web.config
						Rainbow.ECommerce.Gateways.GatewayBase[] gateways = Rainbow.ECommerce.GatewayManager.GetGateways();

						// Now we initialize all gateways that have a setting in this product module.
						ArrayList availableGateways = new ArrayList();

						// For each Available gateway check if properties are set
						foreach(Rainbow.ECommerce.Gateways.GatewayBase g in gateways)
						{
							string currentGatewayProperties = moduleSettings[g.Name].ToString();
							// We add gateway if properties are set
							if (currentGatewayProperties.Length > 0)
							{
								// Get a reference to specified gateway and load data for specified merchant in it
								Rainbow.ECommerce.Gateways.GatewayBase gw = GatewayManager.GetGatewayMerchant(g.Name, currentGatewayProperties.ToString());
								availableGateways.Add(new KeyName(gw.Name, gw.MerchantName));
							}
						}

						// Do we have at least one gateway?
						if (!(availableGateways.Count > 0))
							throw new ArgumentNullException("Gateways", "Unable to find a suitable gateway. Provide some parameters for at least one gateway.");

						ddSelectPayment.DataTextField = "Name";
						ddSelectPayment.DataValueField = "Key";
						ddSelectPayment.DataSource = availableGateways;
						ddSelectPayment.DataBind();
						#endregion
					}
					catch(Exception ex)
					{
						string gatewayErrorMessage = "There was a problem Binding Gateways. Be sure at least one gateway is selected in module properties.";
						Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error,gatewayErrorMessage, ex);
						throw new Exception(gatewayErrorMessage, ex);
					}

					try
					{
						#region Binding the list of available Shipping
						// This list is Dynamic and reflects installed Shippings in web.config
						ShippingBase[] Shippings = ShippingManager.GetShippings();

						// Now we initialize all Shippings that have a setting in this product module.
						ArrayList availableShippings = new ArrayList();

						// For each Available Shipping check if properties are set
						foreach(ShippingBase g in Shippings)
						{
							string currentShippingProperties = moduleSettings[g.Name].ToString();

							// We add Shipping if properties are set
							if (currentShippingProperties.Length > 0)
								availableShippings.Add(g);
						}

						// Do we have at least one Shipping?
						if (!(availableShippings.Count > 0))
							throw new ArgumentNullException("Shippings", "Unable to find a suitable Shipping. Provide some parameters for at least one Shipping.");

						ShippingList.DataTextField = "Name";
						ShippingList.DataValueField = "Name";
						ShippingList.DataSource = availableShippings;
						ShippingList.DataBind();
						ShippingList.Items[0].Selected = true; //select first
						#endregion
					}
					catch(Exception ex)
					{
						string shippingErrorMessage = "There was a problem Binding Shipping. Be sure at least one Shipping is selected in module properties.";
						Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error,shippingErrorMessage, ex);
						throw new Exception(shippingErrorMessage, ex);
					}
					
					// First step
					DisplayStep1();   
				}
				catch(Exception ex)
				{
					this.MyError.Text = ex.Message;
					ShowPanelStep(0); //Hide all panels... we cannot go on
				}

			}
		}


		#region STEP 1/4 : Display account

		// display account informations
		private void DisplayStep1()  
		{
			// show panel 1, hide others
			ShowPanelStep(1);

			BindCountry();

			//Bind to current language country
			CountryField.ClearSelection();
			Esperantus.CountryInfo country = Esperantus.CountryInfo.CurrentCountry;
			if (country != null && CountryField.Items.FindByValue(country.Name) != null)
				CountryField.Items.FindByValue(country.Name).Selected = true;

			BindState();

			// Edit check
			string userID = PortalSettings.CurrentUser.Identity.Email;
			if (userID != string.Empty) 
			{
				// Obtain a single row of event information
				UsersDB accountSystem = new UsersDB();
				SqlDataReader dr = accountSystem.GetSingleUser(userID, portalSettings.PortalID);
        
				try
				{
					// Read first row from database
					if (dr.Read())
					{
						NameField.Text = dr["Name"].ToString();
						EmailField.Text = dr["Email"].ToString();
						CompanyField.Text = dr["Company"].ToString();
						AddressField.Text = dr["Address"].ToString();
						ZipField.Text = dr["Zip"].ToString();
						CityField.Text = dr["City"].ToString();

						CountryField.ClearSelection();
						if (CountryField.Items.FindByValue(dr["CountryID"].ToString()) != null)
							CountryField.Items.FindByValue(dr["CountryID"].ToString()).Selected = true;
						BindState();
						StateField.ClearSelection();
						if (StateField.Items.Count > 0 && StateField.Items.FindByValue(dr["StateID"].ToString()) != null)
							StateField.Items.FindByValue(dr["StateID"].ToString()).Selected = true;

						FaxField.Text = dr["Fax"].ToString();
						PhoneField.Text = dr["Phone"].ToString();
						CFiscaleField.Text = dr["CFiscale"].ToString();
						PIvaField.Text = dr["PIva"].ToString();
					}
				}
				finally
				{
					dr.Close();
				}
			}
			else
			{
				//We do not have rights to do it!
				Security.PortalSecurity.AccessDeniedEdit();
			}
		}

		private void Panel1ContinueBtn_Click(object sender, System.EventArgs e)
		{
			// update the user and progress to step 2
			UsersDB user = new UsersDB();
			string userEmail = PortalSettings.CurrentUser.Identity.Email;
			int userID = -2;

			//Null state id bug for countries with no states
			//Corrected Manudea 14/Nov/2002
			string CountryID = "";
			if (CountryField.SelectedItem != null)
				CountryID = CountryField.SelectedItem.Value;

			int StateID = 0;
			if (StateField.SelectedItem != null)
				StateID = Convert.ToInt32(StateField.SelectedItem.Value);
		
			// Update user
			userID = user.GetCurrentUserID(userEmail, portalSettings.PortalID);
			user.UpdateUser(userID, portalSettings.PortalID, NameField.Text, CompanyField.Text, AddressField.Text, CityField.Text, ZipField.Text, CountryID, StateID, PIvaField.Text, CFiscaleField.Text, PhoneField.Text, FaxField.Text, EmailField.Text, SendNewsletter.Checked);
			
			// step 2
			DisplayStep2();                
		}

		private void UpdateProfileBtn_Click(object sender, System.EventArgs e)
		{
			string updateProfilePage = "Register.aspx?UserName=" + PortalSettings.CurrentUser.Identity.Email + "&mID=" + Request.QueryString["mID"]; //by Manu. We need mID to come back
			Response.Redirect(updateProfilePage); 
		}
		#endregion

		#region STEP 2/4 : Shipping info, select shipping method
		// display shipping informations
		private void DisplayStep2()  
		{
			// show panel 2, hide others
			ShowPanelStep(2);

			//Rebind countries list
			ShipBindCountry();

			//Bind to current language country
			ShipCountryField.ClearSelection();
			Esperantus.CountryInfo country = Esperantus.CountryInfo.CurrentCountry;
			if (country != null && ShipCountryField.Items.FindByValue(country.Name) != null)
				ShipCountryField.Items.FindByValue(country.Name).Selected = true;

			ShipBindState();

			// Edit check
			string userID = PortalSettings.CurrentUser.Identity.Email;
			if (userID != string.Empty) 
			{
				// copy the account info into the shipping info
				CopyShipping();
			}
			else
			{
				//We do not have rights to do it!
				Security.PortalSecurity.AccessDeniedEdit();
			}
		}

		private void Panel2ContinueBtn_Click(object sender, System.EventArgs e)
		{
			DisplayStep3();                
		}

		#endregion

		private Money TotalWithTaxes
		{
			get
			{
				if(ViewState["TotalWithTaxes"] != null)
					return (Money) ViewState["TotalWithTaxes"];
				else
					return new Esperantus.Money(0, this.currencySymbol);
			}
			set
			{
				ViewState["TotalWithTaxes"] = value;
			}
		}

		private Money TotalTaxes
		{
			get
			{
				if(ViewState["TotalTaxes"] != null)
					return (Money) ViewState["TotalTaxes"];
				else
					return new Esperantus.Money(0, this.currencySymbol);
			}
			set
			{
				ViewState["TotalTaxes"] = value;
			}
		}

		private Money Total
		{
			get
			{
				if(ViewState["Total"] != null)
					return (Money) ViewState["Total"];
				else
					return new Esperantus.Money(0, this.currencySymbol);
			}
			set
			{
				ViewState["Total"] = value;
			}
		}

		private Money TotalShipping
		{
			get
			{
				if(ViewState["TotalShipping"] != null)
					return (Money) ViewState["TotalShipping"];
				else
					return new Esperantus.Money(0, this.currencySymbol);
			}
			set
			{
				ViewState["TotalShipping"] = value;
			}
		}

		#region STEP 3/4 : Calculate final price, select payment method
		// display payment 
		private void DisplayStep3()  
		{
			// show panel 3, hide others
			ShowPanelStep(3);

			// hide access to payment as long as not ok
			ChooseCheckoutType.Visible = false;

			//TODO: for Thierry. Must hide the cart itself also with all totals
			//TODO: for Thierry. Only back button should remain visible

			ShoppingCartDB cart = new ShoppingCartDB();

			// Obtain current user's shopping cart ID
			String cartID = ShoppingCartDB.GetCurrentShoppingCartID();

			// If no items, hide details and display message
			if (cart.GetItemCount(cartID, _moduleID) == 0) 
			{
				Message01.TextKey = "CHECKOUT_CART_IS_EMPTY";
				Message01.Text = "There are currently no items in your shopping cart!";
				Message02.TextKey = string.Empty;
				Message02.Text = string.Empty;
			}
			else 
			{
				try 
				{
					// Databind Gridcontrol with Shopping Cart Items
					MyCartList.DataSource = cart.GetItems(cartID, _moduleID, _version);
					MyCartList.DataBind();

					decimal currentPrice = cart.GetTotal(cartID, this.ModuleID, _version, false);
					Total = new Esperantus.Money(currentPrice, currencySymbol);
					lblTotal.Text = Total.ToString();

					decimal currentPriceWithTaxes = cart.GetTotal(cartID, this.ModuleID, _version, true);
					TotalWithTaxes = new Esperantus.Money(currentPriceWithTaxes, currencySymbol);
					lblTotalWithTaxes.Text = TotalWithTaxes.ToString();

					TotalTaxes = new Esperantus.Money(currentPriceWithTaxes - currentPrice, currencySymbol);
					lblTotalTaxes.Text = TotalTaxes.ToString();
				}
				catch(Exception ex)
				{
					MyError.TextKey = "CHECKOUT_ERROR_PROCESSING_CART";
					MyError.Text = "Error processing the cart information";
					
					Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error, "Error processing the cart information", ex);
				}

				try
				{
					//Create a ShippingObject
					ShippingBase shipping;

					//TODO: NO error check for now... BE AWARE
					if (ShippingList.SelectedItem != null)
					{
						// Get Shipping type from the current user selection
						string shippingType = ShippingList.SelectedItem.Value;

						// Get parameters specified in module properties
						string merchantID = moduleSettings[shippingType].ToString();

						// Get a reference to specified Shipping and load data for specified merchant in it
						shipping = ShippingManager.GetShippingMerchant(shippingType, merchantID);

						// Add Shipping information to the page
						decimal shippingCost = shipping.Calculate(cartID, ShipCountryField.SelectedItem.Value, _moduleID, _version);
						
						//Esperantus.Money myshipCurrencyInfo = new Esperantus.Money(shippingCost, currencySymbol);
						TotalShipping = new Esperantus.Money(shippingCost, currencySymbol);
						lblTotalShippingField.Text = TotalShipping.ToString();
					}
					else
					{
						throw new ArgumentNullException("Shipping", "Unable to find a suitable ShippingObject. Provide some parameters for at least one ShippingObject.");
					}

					//If all fine show final checkout button
					ChooseCheckoutType.Visible = true;
				}
				catch(Exception ex)
				{
					MyError.TextKey = "CHECKOUT_ERROR_PROCESSING_SHIPPING";
					MyError.Text = "Error processing the shipping information";

					Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error,MyError.Text, ex);
				}
			}
		}

		private void MyCartList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			try
			{
				// add the options to the name of the product
				Label lblMetadataXml = (Label)e.Item.FindControl("MetadataXml");
				// decode the option string from the metadata
				lblMetadataXml.Text = DecodeOptions(lblMetadataXml.Text);

				// removed total without taxes
				e.Item.Cells[3].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[3].Text), currencySymbol).ToString();
				e.Item.Cells[4].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[4].Text), currencySymbol).ToString();
				e.Item.Cells[5].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[5].Text), currencySymbol).ToString();
			}
			catch // this catch the header of the grid
			{}
		}

		// decode the options xml string, extract the selected options
		// the formatting is coming from the Option.ascx control
		private string DecodeOptions(string metadataXml)
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
					selectedOptions += foundNode1.InnerText + "<br/>";
				}

				XmlNode foundNode2 = myXmlDoc.SelectSingleNode("options/option2/selected");
				if(foundNode2 != null)
				{
					selectedOptions += foundNode2.InnerText + "<br/>";
				}

				XmlNode foundNode3 = myXmlDoc.SelectSingleNode("options/option3/selected");
				if(foundNode3 != null)
				{
					selectedOptions += foundNode3.InnerText + "<br/>";
				}
			}

			return selectedOptions;
		}

		private void Panel3ContinueBtn_Click(object sender, System.EventArgs e)
		{
			DisplayStep4();                
		}
		#endregion

		#region STEP 4/4 : Process Payment
		// display receipt 
		private void DisplayStep4()  
		{
			// show panel 4, hide others
			ShowPanelStep(4);

			// create the gateway 
			// and display the form to collect payment information

			//by christian Create Billing ShopperData Object
			ShopperData bd = new ShopperData();
			bd.FullName = NameField.Text;
			bd.Company = CompanyField.Text;
			bd.Address = AddressField.Text;
			bd.City = CityField.Text;
			bd.ZipCode = ZipField.Text;
			bd.Phone = PhoneField.Text;
			bd.Country = CountryField.SelectedItem.Text;
			bd.StateID = StateField.SelectedItem.Value;
			bd.EMail = EmailField.Text;
			bd.Note = string.Empty; //TODO: save customer note

			//Create Shipping ShopperData Object
			ShopperData sd = new ShopperData();
			sd.FullName = ShipNameField.Text;
			sd.Company = ShipCompanyField.Text;
			sd.Address = ShipAddressField.Text;
			sd.City = ShipCityField.Text;
			sd.ZipCode = ShipZipField.Text;
			sd.Phone = ShipPhoneField.Text;
			sd.Country = ShipCountryField.SelectedItem.Text;
			sd.State = ShipStateField.SelectedItem.Text;
			sd.StateID = ShipStateField.SelectedItem.Value;
			sd.EMail = EmailField.Text;
			sd.Note = string.Empty; //TODO: save customer note

			// Calculate end-user's shopping cart ID
			ShoppingCartDB cart = new ShoppingCartDB();
			String cartID = ShoppingCartDB.GetCurrentShoppingCartID();

			//Create a GATEWAY object
			Rainbow.ECommerce.Gateways.GatewayBase gateway;

			//TODO: NO error check for now... BE AWARE
			if (ddSelectPayment.SelectedItem != null)
			{
				// Get gateway type from the current user selection
				String gatewayType = ddSelectPayment.SelectedItem.Value;

				// Get parameters specified in module properties
				string merchantID = moduleSettings[gatewayType].ToString();

				// Get a reference to specified gateway and load data for specified merchant in it
				gateway = GatewayManager.GetGatewayMerchant(gatewayType, merchantID);

				// Create a new order number for this order
				gateway.OrderID = gateway.GetNewOrderID(); //Note: you can provide a different one if you need

				// Create new order object
				Order o = new Order();
				// Payment method is the selected gateway name
				o.PaymentMethod = gateway.Name;
				// ID of the order
				o.OrderID = gateway.OrderID;
				// ID of logged user
				o.UserID = int.Parse(PortalSettings.CurrentUser.Identity.ID);
				// Status of the order
				o.Status = OrderStatus.ToBeCompleted;
				o.DateCreated = DateTime.Now;
				o.DateModified = DateTime.Now;
				// Get shipping
				o.ShippingMethod = "Fixed Rate"; //TODO: Shipping not available now
				// Load shipping data
				o.ShippingData = sd;
				// Load billing data
				o.BillingData = bd; //by christian Load billing data
				o.TotalWeight = 0; //TODO: The weight should be set
				// AuthCode will be set by gateway
				o.AuthCode = "";
				// TransactionID will be set by gateway
				o.TransactionID = "";
				// Thierry : enable viewstate on the lblTotals !
//				o.TotalGoods = new Esperantus.Money(Esperantus.Money.ParseAmount(lblTotal.Text), currencySymbol);
//				o.TotalTaxes = new Esperantus.Money(Esperantus.Money.ParseAmount(lblTotalTaxes.Text), currencySymbol);
//				o.TotalShipping = new Esperantus.Money(Esperantus.Money.ParseAmount(lblTotalShippingField.Text), currencySymbol);
				o.TotalGoods = Total;
				o.TotalTaxes = TotalTaxes;
				o.TotalShipping = TotalShipping;
				o.TotalExpenses = new Esperantus.Money(0m, currencySymbol); //TODO: Expenses are 0 right now

				// Load items from cart
				o.LoadItemFromCart(cartID, ModuleID, _version);

				// Persist cart on db
				o.Save(cartID, ModuleID);
            
				// Clear cart
				cart.EmptyCart(cartID, ModuleID);

				// Add gateway information to the page
				gateway.Price = o.Total;
				GatewayForm.Controls.Clear();
				LiteralControl gatewayForm = gateway.GetForm();
				//Response.Write(Server.HtmlEncode(gatewayForm.Text));
				GatewayForm.Controls.Add(gatewayForm);
				GatewayForm.Visible = true;
			}
			else
			{
				throw new ArgumentNullException("Gateways", "Unable to find a suitable gateway. Provide some parameters for at least one gateway.");
			}
		}	

		#endregion

		#region Back Btn Callback
		private void Panel1BackBtn_Click(object sender, System.EventArgs e)
		{
			ExitCheckOut();
		}

		private void Panel2BackBtn_Click(object sender, System.EventArgs e)
		{
			DisplayStep1();                
		}

		private void Panel3BackBtn_Click(object sender, System.EventArgs e)
		{
			DisplayStep2();                
		}


		#endregion

		#region Cancel Btn Callbacks
		private void Panel1CancelBtn_Click(object sender, System.EventArgs e)
		{
			ExitCheckOut();
		}

		private void Panel2CancelBtn_Click(object sender, System.EventArgs e)
		{
			ExitCheckOut();
		}

		private void Panel3CancelBtn_Click(object sender, System.EventArgs e)
		{
			ExitCheckOut();
		}



		private void ExitCheckOut()
		{
			//this.Page.RedirectBackToReferringPage();
			// stay in the secure area, confirm leaving secure site
			Response.Redirect("ko.aspx");
		}
		#endregion

		#region Copy Shipping
		private void CopyShipping()
		{
			// copy all textFields
			ShipNameField.Text = NameField.Text;
			ShipCompanyField.Text = CompanyField.Text;
			ShipAddressField.Text = AddressField.Text;
			ShipCityField.Text = CityField.Text;
			ShipZipField.Text = ZipField.Text;
			ShipPhoneField.Text = PhoneField.Text;
			ShipFaxField.Text = FaxField.Text;

			// set up dropdown for Countries and States
			// set all the countries in dropdown
			ShipCountryField.ClearSelection();
			try 
			{ 
				ShipCountryField.Items.FindByValue(CountryField.SelectedItem.Value).Selected = true; 
			}
			catch
			{
			}

			// set the states for this country
			ShipBindState();
			// find the selection
			ShipStateField.ClearSelection();
			if (ShipStateField.Items.FindByValue(StateField.SelectedItem.Value) != null)
				ShipStateField.Items.FindByValue(StateField.SelectedItem.Value).Selected = true; 
		}

		private void CopyShippingBtn_Click(object sender, System.EventArgs e)
		{
			CopyShipping();
		}
		#endregion
		
		#region State and Country
		private void CountryField_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindState();
		}

		private void SCountryField_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ShipBindState();
		}

		/// <summary>
		/// The BindData helper method is used to bind the list
		/// </summary>
		private void BindData()
		{
			BindCountry();
			BindState();
		}

		private void BindCountry()
		{
			//Country filter limits contry list
			string CountriesFilter = portalSettings.CustomSettings["SITESETTINGS_COUNTRY_FILTER"].ToString();

			CountryField.Items.Clear();
			if (CountriesFilter != string.Empty)
			{
				CountryField.DataSource = Esperantus.CountryInfo.GetCountries(CountriesFilter);
			}
			else
			{
				CountryField.DataSource = Esperantus.CountryInfo.GetCountries(Esperantus.CountryTypes.InhabitedCountries,Esperantus.CountryFields.DisplayName);
			}
			CountryField.DataBind();
		}

		private void BindState()
		{
			StateRow.Visible = false;
			UsersDB accountSystem = new UsersDB();
			if (CountryField.SelectedItem != null)
			{
				string currentCountry = CountryField.SelectedItem.Value.ToString();
				StateField.Items.Clear();
				StateField.DataSource = new Esperantus.CountryInfo(currentCountry).Childs;
				StateField.DataBind();
				if (StateField.Items.Count > 0)
				{
					StateRow.Visible = true;
					ThisCountryLabel.Text = CountryField.SelectedItem.Text;
				}
				else
				{
					StateRow.Visible = false;
				}

				//Hides Codfis e Piva if not in Italy
				if (CountryField.SelectedItem.Value == "IT")
				{
					CFiscaleRow.Visible = true;
					PIvaRow.Visible = true;
				}
				else
				{
					CFiscaleRow.Visible = false;
					PIvaRow.Visible = false;
				}
			}
		}

		/// <summary>
		/// The BindData helper method is used to bind the list
		/// </summary>
		private void ShipBindData()
		{
			ShipBindCountry();
			ShipBindState();
		}

		private void ShipBindCountry()
		{
			//Country filter limits country list
			string CountriesFilter = portalSettings.CustomSettings["SITESETTINGS_COUNTRY_FILTER"].ToString();

			if (CountriesFilter != string.Empty)
			{
				ShipCountryField.DataSource = Esperantus.CountryInfo.GetCountries(CountriesFilter);
			}
			else
			{
				ShipCountryField.DataSource = Esperantus.CountryInfo.GetCountries(Esperantus.CountryTypes.InhabitedCountries,Esperantus.CountryFields.DisplayName);
			}
			ShipCountryField.DataBind();
		}

		private void ShipBindState()
		{
			ShipStateRow.Visible = false;
			if (ShipCountryField.SelectedItem != null)
			{
//				string currentShipCountry = CountryField.SelectedItem.Value.ToString();
				string currentShipCountry = ShipCountryField.SelectedItem.Value.ToString(); // by Christian
				ShipStateField.Items.Clear(); // by Christian
				ShipStateField.DataSource = new Esperantus.CountryInfo(currentShipCountry).Childs;
				ShipStateField.DataBind();
				if (ShipStateField.Items.Count > 0)
				{
					ShipStateRow.Visible = true;
//					ShipThisCountryLabel.Text = CountryField.SelectedItem.Text;
					ShipThisCountryLabel.Text = ShipCountryField.SelectedItem.Text; // by Christian
				}
				else
				{
					ShipStateRow.Visible = false;
				}
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.CountryField.SelectedIndexChanged += new System.EventHandler(this.CountryField_SelectedIndexChanged);
			this.UpdateProfileBtn.Click += new System.EventHandler(this.UpdateProfileBtn_Click);
			this.Panel1BackBtn.Click += new System.EventHandler(this.Panel1BackBtn_Click);
			this.Panel1CancelBtn.Click += new System.EventHandler(this.Panel1CancelBtn_Click);
			this.Panel1ContinueBtn.Click += new System.EventHandler(this.Panel1ContinueBtn_Click);
			this.CopyShippingBtn.Click += new System.EventHandler(this.CopyShippingBtn_Click);
			this.ShipCountryField.SelectedIndexChanged += new System.EventHandler(this.SCountryField_SelectedIndexChanged);
			this.Panel2BackBtn.Click += new System.EventHandler(this.Panel2BackBtn_Click);
			this.Panel2CancelBtn.Click += new System.EventHandler(this.Panel2CancelBtn_Click);
			this.Panel2ContinueBtn.Click += new System.EventHandler(this.Panel2ContinueBtn_Click);
			this.MyCartList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.MyCartList_ItemDataBound);
			this.Panel3ContinueBtn.Click += new System.EventHandler(this.Panel3ContinueBtn_Click);
			this.Panel3BackBtn.Click += new System.EventHandler(this.Panel3BackBtn_Click);
			this.Panel3CancelBtn.Click += new System.EventHandler(this.Panel3CancelBtn_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}