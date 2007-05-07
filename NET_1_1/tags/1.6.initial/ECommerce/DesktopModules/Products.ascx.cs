//  Based on IBuySpy Store and Picture Album from Rainbow
//  Adapted to Rainbow project by Thierry (thierry@tiptopweb.com.au)
//  Some code added outside the module in login stuff to migrate the cart when
//  the user is login-in
//  
//  History: 
//  --------
//  * march 2003,  Thierry (thierry@tiptopweb.com.au)
//    first version
//
//  * july 2003, Manu (Duemetri)
//    fixes for Esperantus and set properties
//    moved into /ECommerce/DesktopModules
//
//  * 12 sept 2003, Thierry (thierry@tiptopweb.com.au)
//    - rewritten to use events instead of redirections (the dot net way), this should make it work with framework 1.1
//    as there is no more parameters to decode from the command line beside the usual PageID and ItemID
//    - added webcontrols ProductAddToCart, ProductImageGoToDetails and ProductLinkGoToDetails to be
//    used in /Design/ProductLayouts
//    This controls are raising custom events that BUBBLE to this product.ascx control (see OnBubbleEvent)
//    - NOTE: the ProductAddToCart will NOT work if used in DefaultProductView.ascx
//    it has been added as a LinkButton with an event handler at the bottom of the html page and is
//    displayed when displaying a product details
// --------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.Security;
using System.Xml;
using System.Xml.Schema;
using Esperantus;
using Rainbow;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.UI.DataTypes;
using Rainbow.Design;

using Rainbow.ECommerce;
using Rainbow.ECommerce.Gateways;

namespace Rainbow.DesktopModules
{
	public class Products : PortalModuleControl
	{
		//---properties using viewstate
		#region properties, using viewstate

		// if no category indicated = display featured items
		// NOTE: do NOT use DisplayFeaturedItems here as CategoryID is used in the property DisplayFeaturedItems
		private int mCategoryID = 0;  
		protected int CategoryID
		{
			get
			{
				if (mCategoryID == 0) 
				{
					// when coming from the top menu, itemID is not set! 
					// in that case, set it to PageID
					if ((Request.Params["ItemID"] != null) && (Request.Params["ItemID"].Length != 0)) 
						mCategoryID = int.Parse(Request.Params["ItemID"]);
					else
						mCategoryID = -1;
				}
				return mCategoryID;
			}		
		}

		// get all the logic here
		// we display the featured items if:
		// - requested
		// - categoryID == -1
		// - categoryID == PageID
		private int mbDisplayFeaturedItems = -1;  
		protected bool DisplayFeaturedItems
		{
			get
			{
				if (mbDisplayFeaturedItems == -1)
				{
					if (ViewState["displayfeatureditems"] != null) 
						mbDisplayFeaturedItems = ((bool)(ViewState["displayfeatureditems"]) ? 1 : 0);
					else if(CategoryID == -1 || CategoryID == PageID)
						mbDisplayFeaturedItems = 1;
					else
						mbDisplayFeaturedItems = 0;
				}
				return (bool)((mbDisplayFeaturedItems == 0)? false : true);
			}
			set
			{
				ViewState["displayfeatureditems"] = value;
				mbDisplayFeaturedItems = (value)? 1 : 0;
			}
		}

		private int mProductID = -1;
		protected int ProductID
		{
			get
			{
				if (mProductID == -1)
				{
					if (ViewState["productid"] != null) 
						mProductID = (int)ViewState["productid"];
					else
						mProductID = 0;
				}
				return mProductID;
			}
			set
			{
				ViewState["productid"] = value;
				mProductID = value;
			}
		}

		//		// the state of the module : what it is displaying
		//		// from a postback, this can also be the state where the module was before
		//		// the postback 
		//		public enum DisplayType
		//		{
		//			ProductList = 1,
		//			ProductDetails = 2,
		//			Cart = 3,
		//			OrderList = 4,
		//			OrderDetails = 5
		//		}
		//		private int mDisplayType = -1;
		//		protected DisplayType MyDisplayType
		//		{
		//			get
		//			{
		//				if (mDisplayType == -1)
		//				{
		//					if (ViewState["displaytype"] != null) 
		//						mDisplayType = (int)ViewState["displaytype"];
		//					else
		//						mDisplayType = (int)DisplayType.ProductList;
		//				}
		//				return (DisplayType)mDisplayType;
		//			}
		//			set
		//			{
		//				ViewState["displaytype"] = (int)value;
		//				mDisplayType = (int)value;
		//			}
		//		}

		private string mMessageToDisplay = null;
		protected string MessageToDisplay
		{
			get
			{
				if (mMessageToDisplay == null)
				{
					if (ViewState["messageToDisplay"] != null) 
						mMessageToDisplay = (string)ViewState["messageToDisplay"];
					else
						mMessageToDisplay = "";
				}
				return mMessageToDisplay;
			}
			set
			{
				ViewState["messageToDisplay"] = value;
				mMessageToDisplay = value;
			}
		}

		private string mMessageKeyToDisplay = "";
		protected string MessageKeyToDisplay
		{
			get
			{
				if (mMessageKeyToDisplay == "")
				{
					if (ViewState["messageKeyToDisplay"] != null) 
						mMessageKeyToDisplay = (string)ViewState["messageKeyToDisplay"];
					else
						mMessageKeyToDisplay = "";
				}
				return mMessageKeyToDisplay;
			}
			set
			{
				ViewState["messageKeyToDisplay"] = value;
				mMessageKeyToDisplay = value;
			}
		}

		protected bool UserLogged
		{
			get
			{
				return (HttpContext.Current.User.Identity.Name.Length > 0);
			}
		}

		private int m_version = -1;
	
		protected WorkFlowVersion MyVersion
		{
			get
			{
				if (m_version == -1)
				{
					// set the workflow version back to staging if necessary as there is a reset when page redisplayed
					// cannot set this.Version as we cannot set it if not logged in (permission problem)
					if (ViewState["wversion"] != null) 
						m_version = (int)ViewState["wversion"];
					else
						m_version = (int)WorkFlowVersion.Staging;
				}
				return (WorkFlowVersion)m_version;
			}
			set
			{
				ViewState["wversion"] = (int)value;
				m_version = (int)value;
			}
		}

		private string mOrderID = null;
		protected string OrderID
		{
			get
			{
				if (mOrderID == null)
				{
					if (ViewState["orderid"] != null) 
						mOrderID = (string)ViewState["orderid"];
					else
						mOrderID = "";
				}
				return mOrderID;
			}		
			set
			{
				ViewState["orderid"] = value;
				mOrderID = value;
			}
		}
		#endregion
		
		//---controls
		protected Esperantus.WebControls.Literal pleaseLogon;
		protected Esperantus.WebControls.Label lblTitle;
		protected Esperantus.WebControls.Label lblMessage;
		protected Esperantus.WebControls.Label txtTotalTaxes;
		protected Esperantus.WebControls.Label txtTotalWithTaxes;
		protected Esperantus.WebControls.Label txtTotal;
		protected System.Web.UI.WebControls.Label lblTotalWithTaxes;
		protected System.Web.UI.WebControls.Label lblTotalTaxes;
		protected System.Web.UI.WebControls.Label lblTotal;

		protected System.Web.UI.WebControls.DataList ProductList;
		protected Rainbow.UI.WebControls.Paging pgProducts;
		protected System.Web.UI.WebControls.PlaceHolder ProductDetails;
		protected System.Web.UI.WebControls.PlaceHolder OrderDetails;
		protected System.Web.UI.WebControls.DataGrid CartList;
		protected System.Web.UI.WebControls.DataGrid OrderList;

		// LinkButtons using postback
		protected Esperantus.WebControls.LinkButton UpdateBtn;
		protected Esperantus.WebControls.LinkButton CheckoutBtn;
		protected Esperantus.WebControls.LinkButton AccountBtn;
		protected Esperantus.WebControls.LinkButton ViewCartBtn;
		protected Esperantus.WebControls.LinkButton ContinueBtn;
		
		// to control visibility of areas of the display
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayTitlePanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayMessagePanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayHeaderPanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayProductListPanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayProductPanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayCartPanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayOrderPanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayOrderListPanel;
		protected System.Web.UI.HtmlControls.HtmlTableRow DisplayFooterPanel;
		private StringBuilder builder = new StringBuilder();

		/// <summary>
		/// Resize Options
		/// NoResize : Do not resize the picture
		/// FixedWidthHeight : Use the width and height specified. 
		/// MaintainAspectWidth : Use the specified height and calculate height using the original aspect ratio
		/// MaintainAspectHeight : Use the specified width and calculate width using the original aspect ration
		/// </summary>
		public enum ResizeOption
		{
			NoResize,
			FixedWidthHeight,
			MaintainAspectWidth,
			MaintainAspectHeight
		}

		// Bubble events have been raised from child controls from the ProductLayout
		// By overriding the OnBubbleEvent, we do not need provide explicit event handler
		// for all the AddToCart buttons etc etc
		#region Bubble event handler from custom controls dynamically loaded from layouts
		protected override bool OnBubbleEvent(object sender, EventArgs e)
		{
			// test is coming from AddToCart
			if(e is AddToCartEventArgs)
			{
				try
				{
					// get the productId as param of the event
					ProductID = ((AddToCartEventArgs)e).ProductID;

					// need to find the selected options
					string optionsXmlString = "";
					bool found = false;
					foreach(DataListItem item in ProductList.Items)
					{
						ProductItem productItem = (ProductItem)item.Controls[0];
						if(productItem.ProductID == ProductID)
						{
							// get the selected options
							Rainbow.ECommerce.Design.Options options = (Rainbow.ECommerce.Design.Options) productItem.FindControl("ctlOptions");
							optionsXmlString = options.SetOptions;

							found = true;
							break;
						}
					}

					// only add the item if we were able to find the options
					if( found )
					{
						ShoppingCartDB cart = new ShoppingCartDB();

						// Obtain current users shopping cart ID
						// GUID if not logged, UserID if logged
						string cartID = ShoppingCartDB.GetCurrentShoppingCartID();
						cart.AddItem(cartID, ProductID, 1, ModuleID, optionsXmlString);

						DisplayCart();
					}
					else
					{
						CheckoutBtn.Visible = false;
						lblMessage.TextKey = "PRODUCT_ADD_TO_CART_NO_OPTIONS";
						lblMessage.Text = "Please select the options";
					}
				}
				catch(Exception ex)
				{
					CheckoutBtn.Visible = false;
					lblMessage.TextKey = "PRODUCT_ADD_TO_CART_PROBLEM";
					lblMessage.Text = "Could not add the product to the cart";

					Rainbow.Configuration.ErrorHandler.Publish(LogLevel.Error,lblMessage.Text, ex);
				}
				// stop from bubbling up
				return true;
			}

			// test is coming from ImageGoToDetails
			else if(e is ImageGoToDetailsEventArgs)
			{
				try
				{
					// get the productID as param of the event
					ProductID = ((ImageGoToDetailsEventArgs)e).ProductID;

					// Build and display the product details
					DisplayProductDetails();
				}
				catch(Exception ex)
				{
					Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, "test is coming from ImageGoToDetails", ex);
				}
				// stop from bubbling up
				return true;
			}

			// test is coming from LinkGoToDetails
			else if(e is LinkGoToDetailsEventArgs)
			{
				try
				{
					// get the productID as param of the event
					ProductID = ((LinkGoToDetailsEventArgs)e).ProductID;

					// Build and display the product details
					DisplayProductDetails();
				}
				catch(Exception ex)
				{
					Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, " test is coming from LinkGoToDetails", ex);
				}
				// stop from bubbling up
				return true;
			}
			return false;
		}
		#endregion 

		private void Page_Load(object sender, System.EventArgs e)
		{ 
			// also needed in Metadata
			string themeName;
			if( Int32.Parse(Settings["MODULESETTINGS_THEME"].ToString()) == (int)ThemeList.Default)
				themeName = "Default";
			else
				themeName = "Alt";
			Rainbow.Design.Theme CurrentTheme = portalSettings.GetCurrentTheme(themeName);

			// set images using the themes:
			// - if the image exists in the theme, 
			// add it to ImageUrl of hyperlinks and buttons of this page, for the
			// hyperlinks in the ProductLayout transmit the url in Metadata
			// - if no image, use text for the hyperlink
			//btnAccount.ImageUrl = CurrentTheme.GetImage("Buttons_Account").ImageUrl;
			//btnViewCart.ImageUrl = CurrentTheme.GetImage("Buttons_ViewCart").ImageUrl;
			//UpdateBtn.ImageUrl = CurrentTheme.GetImage("Buttons_UpdateCart").ImageUrl;
			//CheckoutBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Checkout").ImageUrl;
			//btnContinue.ImageUrl = CurrentTheme.GetImage("Buttons_ContShopping").ImageUrl;

			// view cart is always visible, will be migrated when user is login in
			ViewCartBtn.Visible = true; 

			// set visibility of view orders depending of authentication of the user
			AccountBtn.Visible = UserLogged;
			CheckoutBtn.Visible = UserLogged;
			pleaseLogon.Visible = !UserLogged;

			// add message to display if one was sent
			lblMessage.TextKey = MessageKeyToDisplay;  // saved in Viewstate
			lblMessage.Text = MessageToDisplay;

			// display the title, the logic of the CategoryID, ItemID, FeaturedItem is within the 
			// properties, do not add any logic here
			if( DisplayFeaturedItems )
			{
				lblTitle.TextKey = "PRODUCT_SHOP_HOME";
				lblTitle.Text = "Shop home";
			}
			else
			{
				// get the name of the tab to display it as a title
				int idToFind = CategoryID;
				bool found = false;
				for(int i=0 ; i< portalSettings.DesktopPages.Count ; i++)
				{
					if(((PageStripDetails)portalSettings.DesktopPages[i]).PageID == idToFind)
					{
						lblTitle.TextKey = ((PageStripDetails)portalSettings.DesktopPages[i]).PageName;
						lblTitle.Text = ((PageStripDetails)portalSettings.DesktopPages[i]).PageName;
						found = true;
						break;
					}
				}
				if(!found)
				{
					lblTitle.TextKey = "PRODUCT_CATEGORY_NOT_FOUND";
					lblTitle.Text = "Category not found";
				}
			}

			// default : set all to invisible except header and message
			DisplayHeaderPanel.Visible = true;
			DisplayMessagePanel.Visible = true;
			DisplayProductListPanel.Visible = false;
			DisplayProductPanel.Visible = false;
			DisplayCartPanel.Visible = false;
			DisplayTitlePanel.Visible = false;
			DisplayFooterPanel.Visible = false;
			DisplayOrderPanel.Visible = false;  
			DisplayOrderListPanel.Visible = false;  

			// always build the product list, continue shopping button will set it visible for example
			// if the user clicked on addToCart for example, the control needs to be redisplayed
			// so that it can raise the event and bubble it to this page!
			// This is only for the Controls loaded dynamically and using custom events
			// we do not have any problem with static controls that have an event handler on this page
			BuildProductList();

			// by default display the product list, it is built as MyDisplayType is ProductList by default
			SetProductListVisible(true);

			if(! Page.IsPostBack ) 
			{
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			// Create a new Title the control
			//ModuleTitle = new DesktopModuleTitle();
			// Set here title properties
			// Add support for the edit page
			this.AddText = "ADD"; //"Add New Product"
			this.AddUrl = "~/ECommerce/DesktopModules/ProductsEdit.aspx";

			// Add title ad the very begining of 
			// the control's controls collection
			//Controls.AddAt(0, ModuleTitle);
		
			base.OnInit(e);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AccountBtn.Click += new System.EventHandler(this.AccountBtn_Click);
			this.ViewCartBtn.Click += new System.EventHandler(this.ViewCartBtn_Click);
			this.ProductList.ItemDataBound += new System.Web.UI.WebControls.DataListItemEventHandler(this.ProductList_ItemDataBound);
			this.pgProducts.OnMove += new System.EventHandler(this.Page_Changed);
			this.CartList.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CartList_Command);
			this.CartList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.CartList_ItemDataBound);
			this.UpdateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
			this.CheckoutBtn.Click += new System.EventHandler(this.CheckoutBtn_Click);
			this.OrderList.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.OrderList_Command);
			this.OrderList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.OrderList_ItemDataBound);
			this.ContinueBtn.Click += new System.EventHandler(this.ContinueBtn_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Build Products / Orders / Cart and Display Functions

		// Product list is build and set invisible
		// use this function to set it visible
		private void SetProductListVisible(bool value)
		{
			// set visible?
			DisplayProductListPanel.Visible = value;
			DisplayTitlePanel.Visible = value;
//			if(value) MyDisplayType = DisplayType.ProductList;
		}
	
		// Build the list of all the products in the specified category

		private void BuildProductList()
		{
			//  Obtain products and databind to an asp:datalist control
					
			ProductList.RepeatDirection = (Settings["RepeatDirectionSetting"] == null ? RepeatDirection.Horizontal : 
				(RepeatDirection)int.Parse(((SettingItem) _baseSettings["RepeatDirectionSetting"])));
			ProductList.RepeatColumns = int.Parse(((SettingItem) Settings["RepeatColumns"]));
			//ProductList.ItemDataBound += new DataListItemEventHandler(ProductList_ItemDataBound); //by Manu - set in initalize components

			pgProducts.RecordsPerPage = int.Parse(Settings["ProductsPerPage"].ToString());
			//pgProducts.OnMove += new EventHandler(Page_Changed); //by Manu - set in initialize components

			//  Obtain products and databind to an asp:datalist control
			BindProductsList(pgProducts.PageNumber, CategoryID); //Always - even if postback

			// do not show
			SetProductListVisible(false);
		}

		// Build the details of a specified product
		private void DisplayProductDetails()
		{
			SetProductListVisible(false);

			// Obtain details of the product 
			ProductsDB products  = new ProductsDB();
			SqlDataReader di = products.GetSingleProduct(ProductID, MyVersion);
            
			ProductItem productItem;
			XmlDocument metadata = new XmlDocument();

			try
			{
				// Read first row from database
				di.Read();
			
				productItem = (ProductItem) Page.LoadControl(Rainbow.Settings.Path.ApplicationRoot + "/ECommerce/Design/ProductLayouts/" + Settings["ProductLayout"]);
	
				//Set correct symbol - by Manu
				productItem.CurrencySymbol = Settings["Currency"].ToString();

				if(di["MetadataXml"] == DBNull.Value) metadata.LoadXml("<Metadata></Metadata>");
				else metadata.LoadXml((string)di["MetadataXml"]);

				XmlAttribute shortDescription = metadata.CreateAttribute("ShortDescription");
				if(di["ShortDescription"] == DBNull.Value) shortDescription.Value = "";	
				else shortDescription.Value = (string) di["ShortDescription"]; 

				XmlAttribute longDescription = metadata.CreateAttribute("LongDescription");
				if(di["LongDescription"] == DBNull.Value) longDescription.Value = "";	
				else longDescription.Value = (string) di["LongDescription"]; 

				XmlAttribute modelName = metadata.CreateAttribute("ModelName");
				if(di["ModelName"] == DBNull.Value) modelName.Value = "";	
				else modelName.Value = (string) di["ModelName"];

				XmlAttribute modelNumber = metadata.CreateAttribute("ModelNumber");
				if(di["ModelNumber"] == DBNull.Value) modelNumber.Value = "";	
				else modelNumber.Value = (string) di["ModelNumber"];

				XmlAttribute unitPrice = metadata.CreateAttribute("UnitPrice");
				if(di["UnitPrice"] == DBNull.Value) unitPrice.Value = "0";	
				else unitPrice.Value = Convert.ToDecimal(di["UnitPrice"]).ToString(); //.ToString("c");

				XmlAttribute weight = metadata.CreateAttribute("Weight");
				if(di["Weight"] == DBNull.Value) weight.Value = "0";	
				else weight.Value = Convert.ToDouble(di["Weight"]).ToString(); 

				XmlAttribute taxRate = metadata.CreateAttribute("TaxRate");
				if(di["TaxRate"] == DBNull.Value) modelNumber.Value = "0";	
				else taxRate.Value = Convert.ToDouble(di["TaxRate"]).ToString();

				metadata.DocumentElement.Attributes.Append(shortDescription);
				metadata.DocumentElement.Attributes.Append(longDescription);
				metadata.DocumentElement.Attributes.Append(modelName);
				metadata.DocumentElement.Attributes.Append(modelNumber);
				metadata.DocumentElement.Attributes.Append(unitPrice);
				metadata.DocumentElement.Attributes.Append(weight);
				metadata.DocumentElement.Attributes.Append(taxRate);

				// add additional metadata to be used in the ProductItem control
				// from the ProductLayout
				XmlAttribute shopPath = metadata.CreateAttribute("ShopPath");
				shopPath.Value = ((SettingItem) Settings["ShopPath"]).FullPath;

				XmlAttribute productID = metadata.CreateAttribute("ProductID");
				productID.Value = ((int) di["ProductID"]).ToString();

				XmlAttribute moduleID = metadata.CreateAttribute("ModuleID");
				moduleID.Value = ModuleID.ToString();

				XmlAttribute PageID = metadata.CreateAttribute("PageID");
				PageID.Value = this.PageID.ToString();

				XmlAttribute isEditable = metadata.CreateAttribute("IsEditable");
				isEditable.Value = this.IsEditable.ToString();

				XmlAttribute categoryID = metadata.CreateAttribute("CategoryID");
				categoryID.Value = CategoryID.ToString();

				XmlAttribute itemID = metadata.CreateAttribute("ItemID");
				itemID.Value = CategoryID.ToString();  

				XmlAttribute featuredItem = metadata.CreateAttribute("FeaturedItem");
				featuredItem.Value = DisplayFeaturedItems.ToString();  

				XmlAttribute cmdDisplayProduct = metadata.CreateAttribute("CmdDisplayProduct");
				cmdDisplayProduct.Value = "itemid=" + CategoryID.ToString() + "&actionid=1" + "&productid=" + ((int) di["ProductID"]).ToString() + "&wversion=" + MyVersion.ToString().ToLower();

				XmlAttribute cmdEditProduct = metadata.CreateAttribute("CmdEditProduct");
				cmdEditProduct.Value = "itemid=" + ((int) di["ProductID"]).ToString() + "&mID=" + ModuleID.ToString();

				XmlAttribute cmdAddToCart = metadata.CreateAttribute("CmdAddToCart");
				cmdAddToCart.Value = "itemid=" + CategoryID.ToString() + "&actionid=3" + "&productid=" + ((int) di["ProductID"]).ToString() + "&wversion=" + MyVersion.ToString().ToLower();

				//			XmlAttribute imageUrlAddToCart = metadata.CreateAttribute("ImageUrlAddToCart");
				//			imageUrlAddToCart.Value = CurrentTheme.GetImage("Buttons_AddItem").ImageUrl;

				metadata.DocumentElement.Attributes.Append(shopPath);
				metadata.DocumentElement.Attributes.Append(productID);
				metadata.DocumentElement.Attributes.Append(moduleID);
				metadata.DocumentElement.Attributes.Append(PageID);
				metadata.DocumentElement.Attributes.Append(isEditable);
				metadata.DocumentElement.Attributes.Append(categoryID);
				metadata.DocumentElement.Attributes.Append(itemID);
				metadata.DocumentElement.Attributes.Append(featuredItem);
				metadata.DocumentElement.Attributes.Append(cmdDisplayProduct);		
				metadata.DocumentElement.Attributes.Append(cmdEditProduct);		
				metadata.DocumentElement.Attributes.Append(cmdAddToCart);		
				//			metadata.DocumentElement.Attributes.Append(imageUrlAddToCart);		

				if(MyVersion == WorkFlowVersion.Production)
				{
					XmlNode modifiedFilenameNode = metadata.DocumentElement.SelectSingleNode("@ModifiedFilename");
					XmlNode thumbnailFilenameNode = metadata.DocumentElement.SelectSingleNode("@ThumbnailFilename");

					modifiedFilenameNode.Value = modifiedFilenameNode.Value + ".Production";
					thumbnailFilenameNode.Value = thumbnailFilenameNode.Value + ".Production";
				}

				productItem.Metadata = metadata;
				productItem.DataBind();
			

				// add to the place holder
				ProductDetails.Controls.Add(productItem);
			}
			finally
			{
				// Close datareader
				di.Close(); 
			}

			// set visible
			DisplayTitlePanel.Visible = true;
			DisplayProductPanel.Visible = true;
			DisplayFooterPanel.Visible = true;	
		}

		// Display the current shopping cart of the client (logged or not)

		private void DisplayCart()
		{
			SetProductListVisible(false);
			
			if ((SupportsWorkflow && MyVersion == WorkFlowVersion.Production) || !SupportsWorkflow)		
			{
//				MyDisplayType = DisplayType.Cart;

				DisplayCartPanel.Visible = true;
				DisplayHeaderPanel.Visible = false;
				DisplayFooterPanel.Visible = true;
				PopulateShoppingCartList();  // will overwrite visibility of cart
			}
			else
			{
				lblMessage.TextKey = "PRODUCT_PRODUCTION_ONLY";
				lblMessage.Text = "Function only available on production!";
			}		
		}

		// Display the list of all the orders for a specified client (need to be logged)

		private void DisplayOrderList()
		{
			SetProductListVisible(false);

			if ((SupportsWorkflow && MyVersion == WorkFlowVersion.Production) || !SupportsWorkflow)		
			{
//				MyDisplayType = DisplayType.OrderList;

				DisplayOrderListPanel.Visible = true;
				DisplayHeaderPanel.Visible = false;
				DisplayFooterPanel.Visible = true;

				// Obtain and bind a list of all orders ever placed by visiting customer
				OrdersDB orderHistory = new OrdersDB();
	    
				int userID = int.Parse(PortalSettings.CurrentUser.Identity.ID);

				OrderList.DataSource = orderHistory.GetOrdersByUser(ModuleID, userID);
				OrderList.DataBind();

				// Hide the list and display a message if no orders have ever been made
				if (OrderList.Items.Count == 0) 
				{
					lblMessage.TextKey = "PRODUCTS_NO_ORDERS_TO_DISPLAY";
					lblMessage.Text = "You have no orders to display.";
					OrderList.Visible = false;
				}
				else
				{
					lblMessage.TextKey = "PRODUCTS_ORDER_HISTORY_DISPLAY";
					lblMessage.Text = "Displaying your orders history. Click on the order ID to display the details of the corresponding order";
				}
			}
			else
			{
				lblMessage.TextKey = "PRODUCT_PRODUCTION_ONLY";
				lblMessage.Text = "Function only available on production!";
			}		
		}


		/// <summary>
		/// Display the parameters of the specified order
		/// </summary>
		private void DisplayOrderDetails(string orderID)
		{
			if ((SupportsWorkflow && MyVersion == WorkFlowVersion.Production) || !SupportsWorkflow)		
			{
				DisplayOrderPanel.Visible = true;
				DisplayHeaderPanel.Visible = false;
				DisplayFooterPanel.Visible = true;

				try
				{
					// get orderID
					//string orderID = Request.Params["orderid"];

					// load the order
					Order order = new Order();
					order.Load(orderID);
					OrderDetails.Controls.Add(new LiteralControl(order.HTML));

					lblMessage.TextKey = "PRODUCTS_DISPLAYING_ORDER";
					lblMessage.Text = "Displaying order: ";
				}
				catch(Exception t)
				{
					lblMessage.TextKey = "PRODUCTS_CANNOT_DISPLAY_ORDER";
					lblMessage.Text = "Cannot display the selected order.";
					Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Warn, lblMessage.Text, t);
				}
			}
			else
			{
				lblMessage.TextKey = "PRODUCT_PRODUCTION_ONLY";
				lblMessage.Text = "Function only available on production!";
			}        
		}

		#endregion

		#region Event and Command handlers	
		private void AccountBtn_Click(object sender, System.EventArgs e)
		{
			DisplayOrderList();
		}

		private void ViewCartBtn_Click(object sender, System.EventArgs e)
		{
			DisplayCart();
		}

		private void ContinueBtn_Click(object sender, System.EventArgs e)
		{
			SetProductListVisible(true);
		}

		private void UpdateBtn_Click(object sender, System.EventArgs e)
		{
			if ((SupportsWorkflow && MyVersion == WorkFlowVersion.Production) || !SupportsWorkflow)		
			{
				// Update the Shopping Cart and then Repopulate the List
				UpdateShoppingCartDatabase();
				PopulateShoppingCartList();
			}
			else
			{
				lblMessage.TextKey = "PRODUCTS_ONLY_AVAILABLE_ON_PRODUCTION";
				lblMessage.Text = "Function only available on production!";
			}

			// redisplay the cart
			DisplayCart();
		}

		private void CheckoutBtn_Click(object sender, System.EventArgs e)
		{
			if ((SupportsWorkflow && MyVersion == WorkFlowVersion.Production) || !SupportsWorkflow)		
			{
				// Update Shopping Cart
				UpdateShoppingCartDatabase();
				PopulateShoppingCartList();

				// If cart is not empty, proceed on to checkout page
				ShoppingCartDB cart = new ShoppingCartDB();

				// Calculate shopping cart ID
				String cartID = ShoppingCartDB.GetCurrentShoppingCartID();

				// If the cart isn't empty, navigate to checkout page
				if (cart.GetItemCount(cartID, this.ModuleID) != 0) 
				{
					// check if user is logged
					if(UserLogged) 
						RedirectToCheckOut();
					else
					{
						// redirect to login panel (special action indicating checkout requested)
						lblMessage.TextKey = "PRODUCT_MUST_LOG_IN_FOR_CHECKOUT";
						lblMessage.Text = "You must first login or register to check out!";
					}
				}
				else 
				{
					lblMessage.TextKey = "PRODUCT_CHECKOUT_WITH_EMPTY_CART";
					lblMessage.Text = "Cannot proceed to the Check Out page with an empty cart.";
				}
			}
			else
			{
				lblMessage.TextKey = "PRODUCTS_ONLY_AVAILABLE_ON_PRODUCTION";
				lblMessage.Text = "Function only available on production!";
			}

			// redisplay the cart
			DisplayCart();
		}

		// Display the product info when clicking the item's description in the cart datagrid
		private void CartList_Command(object sender, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				// Display details of a product
				if (e.CommandName == "CartItemInfo")
				{
					// get the product ID from the invisible cell
					Label lblProductID = (Label) e.Item.Cells[0].FindControl("ProductID");
					ProductID = int.Parse(lblProductID.Text);

					// Build and display the product 
					DisplayProductDetails();
				}

			}
			catch
			{}
		}


		/// <summary>
		/// Display the order info when clicking the orderID in the orderlist datagrid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OrderList_Command(object sender, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				// Display details of a product
				if (e.CommandName == "OrderItemInfo")
				{
					// get the product ID from the invisible cell
					LinkButton lblOrderID = (LinkButton) e.Item.Cells[0].FindControl("OrderID");
					OrderID = lblOrderID.Text.ToString().Trim();

					// Display the product 
					DisplayOrderDetails(OrderID);
				}
			}
			catch
			{}
		}

		private void OrderList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			try
			{
				e.Item.Cells[3].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[3].Text), Settings["Currency"].ToString()).ToString();
			}
			catch
			{}
		}

		private void Page_Changed(object sender, System.EventArgs e)
		{
			// rebind the product list
			BindProductsList(pgProducts.PageNumber, CategoryID);

			// set the list visible!
			SetProductListVisible(true);
		}

		private void BindProductsList(int page, int catId)
		{
			ProductsDB products = new ProductsDB();
			DataSet dsProducts = products.GetProductsPaged(ModuleID, catId, page, int.Parse(Settings["ProductsPerPage"].ToString()), MyVersion);

			if(dsProducts.Tables.Count > 0 && dsProducts.Tables[0].Rows.Count > 0)
			{
				pgProducts.RecordCount = (int)(dsProducts.Tables[0].Rows[0]["RecordCount"]);
			}

			ProductList.DataSource = dsProducts;
			ProductList.DataBind();
		}

		private void ProductList_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			// get layout from the Design/ProductsLayout directory
			ProductItem productItem = (ProductItem) Page.LoadControl(Rainbow.Settings.Path.ApplicationRoot + "/ECommerce/Design/ProductLayouts/" + Settings["ThumbnailLayout"]);
			DataRowView di = (DataRowView)e.Item.DataItem;
			
			// set currency
			productItem.CurrencySymbol = Settings["Currency"].ToString();

			// metadata from the edit product module
			XmlDocument metadata = new XmlDocument();
			if(di["MetadataXml"] == DBNull.Value || ValidateXml(di["MetadataXml"].ToString()) == false) 
			{
				metadata.LoadXml("<Metadata></Metadata>");
			}
			else 
			{ 
				metadata.LoadXml((string)di["MetadataXml"]);
			}
 
			XmlAttribute shortDescription = metadata.CreateAttribute("ShortDescription");
			if(di["ShortDescription"] == DBNull.Value) shortDescription.Value = "";	
			else shortDescription.Value = (string) di["ShortDescription"]; 

			XmlAttribute longDescription = metadata.CreateAttribute("LongDescription");
			if(di["LongDescription"] == DBNull.Value) longDescription.Value = "";	
			else longDescription.Value = (string) di["LongDescription"]; 

			XmlAttribute modelName = metadata.CreateAttribute("ModelName");
			if(di["ModelName"] == DBNull.Value) modelName.Value = "";	
			else modelName.Value = (string) di["ModelName"];

			XmlAttribute modelNumber = metadata.CreateAttribute("ModelNumber");
			if(di["ModelNumber"] == DBNull.Value) modelNumber.Value = "";	
			else modelNumber.Value = (string) di["ModelNumber"];

			XmlAttribute unitPrice = metadata.CreateAttribute("UnitPrice");
			if(di["UnitPrice"] == DBNull.Value) unitPrice.Value = "0";	
			else unitPrice.Value = Convert.ToDecimal(di["UnitPrice"]).ToString(); //.ToString("c");

			XmlAttribute weight = metadata.CreateAttribute("Weight");
			if(di["Weight"] == DBNull.Value) weight.Value = "0";	
			else weight.Value = Convert.ToDouble(di["Weight"]).ToString(); 

			XmlAttribute taxRate = metadata.CreateAttribute("TaxRate");
			if(di["TaxRate"] == DBNull.Value) modelNumber.Value = "0";	
			else taxRate.Value = Convert.ToDouble(di["TaxRate"]).ToString();	
			
			metadata.DocumentElement.Attributes.Append(shortDescription);
			metadata.DocumentElement.Attributes.Append(longDescription);
			metadata.DocumentElement.Attributes.Append(modelName);
			metadata.DocumentElement.Attributes.Append(modelNumber);
			metadata.DocumentElement.Attributes.Append(unitPrice);
			metadata.DocumentElement.Attributes.Append(weight);
			metadata.DocumentElement.Attributes.Append(taxRate);

			// add additional metadata to be used in the ProductItem control
			// from the ProductLayout
			XmlAttribute shopPath = metadata.CreateAttribute("ShopPath");
			shopPath.Value = ((SettingItem) Settings["ShopPath"]).FullPath;

			XmlAttribute productID = metadata.CreateAttribute("ProductID");
			productID.Value = ((int) di["ProductID"]).ToString();

			XmlAttribute moduleID = metadata.CreateAttribute("ModuleID");
			moduleID.Value = this.ModuleID.ToString();

			XmlAttribute PageID = metadata.CreateAttribute("PageID");
			PageID.Value = this.PageID.ToString();

			XmlAttribute isEditable = metadata.CreateAttribute("IsEditable");
			isEditable.Value = this.IsEditable.ToString();

			XmlAttribute categoryID = metadata.CreateAttribute("CategoryID");
			categoryID.Value = CategoryID.ToString();

//			XmlAttribute actionID = metadata.CreateAttribute("ActionID");
//			actionID.Value = ActionID.ToString(); 

			XmlAttribute itemID = metadata.CreateAttribute("ItemID");
			itemID.Value = CategoryID.ToString();  

			XmlAttribute featuredItem = metadata.CreateAttribute("FeaturedItem");
			featuredItem.Value = DisplayFeaturedItems.ToString();  

//			XmlAttribute customAttributes = metadata.CreateAttribute("NavigateAttribute");
//			customAttributes.Value = _customAttributes;  
//
			XmlAttribute cmdDisplayProduct = metadata.CreateAttribute("CmdDisplayProduct");
			cmdDisplayProduct.Value = "itemid=" + CategoryID.ToString() + "&actionid=1" + "&productid=" + ((int) di["ProductID"]).ToString() + "&wversion=" + MyVersion.ToString().ToLower();

			XmlAttribute cmdEditProduct = metadata.CreateAttribute("CmdEditProduct");
			cmdEditProduct.Value = "itemid=" + ((int) di["ProductID"]).ToString() + "&mID=" + ModuleID.ToString();

			XmlAttribute cmdAddToCart = metadata.CreateAttribute("CmdAddToCart");
			cmdAddToCart.Value = "itemid=" + CategoryID.ToString() + "&actionid=3" + "&productid=" + ((int) di["ProductID"]).ToString() + "&wversion=" + MyVersion.ToString().ToLower();

			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
			string themeName;
			if( Int32.Parse(Settings["MODULESETTINGS_THEME"].ToString()) == (int)ThemeList.Default)
				themeName = "Default";
			else
				themeName = "Alt";
			Rainbow.Design.Theme CurrentTheme = portalSettings.GetCurrentTheme(themeName);

			XmlAttribute imageUrlAddToCart = metadata.CreateAttribute("ImageUrlAddToCart");
			imageUrlAddToCart.Value = CurrentTheme.GetImage("Buttons_AddItem", "Add.gif").ImageUrl;

			metadata.DocumentElement.Attributes.Append(shopPath);
			metadata.DocumentElement.Attributes.Append(productID);
			metadata.DocumentElement.Attributes.Append(moduleID);
			metadata.DocumentElement.Attributes.Append(PageID);
			metadata.DocumentElement.Attributes.Append(isEditable);
			metadata.DocumentElement.Attributes.Append(categoryID);
//			metadata.DocumentElement.Attributes.Append(actionID);
			metadata.DocumentElement.Attributes.Append(itemID);
			metadata.DocumentElement.Attributes.Append(featuredItem);
//			metadata.DocumentElement.Attributes.Append(customAttributes);
			metadata.DocumentElement.Attributes.Append(cmdDisplayProduct);		
			metadata.DocumentElement.Attributes.Append(cmdEditProduct);		
			metadata.DocumentElement.Attributes.Append(cmdAddToCart);		
			metadata.DocumentElement.Attributes.Append(imageUrlAddToCart);		

			// workflow support: add .Production at the end of the filename
			if(MyVersion == WorkFlowVersion.Production)
			{
				XmlNode modifiedFilenameNode = metadata.DocumentElement.SelectSingleNode("@ModifiedFilename");
				XmlNode thumbnailFilenameNode = metadata.DocumentElement.SelectSingleNode("@ThumbnailFilename");

				modifiedFilenameNode.Value = modifiedFilenameNode.Value + ".Production";
				thumbnailFilenameNode.Value = thumbnailFilenameNode.Value + ".Production";
			}

			productItem.Metadata = metadata;
			productItem.DataBind();
			e.Item.Controls.Add(productItem);

		}
		#endregion

		#region Build Cart

		// event to build lines of the grid represneting the cart
		private void CartList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			try
			{
				// add the options to the name of the product
				Label lblMetadataXml = (Label)e.Item.FindControl("MetadataXml");
				// decode the option string from the metadata
				lblMetadataXml.Text = DecodeOptions(lblMetadataXml.Text);

				// removed total without taxes
				e.Item.Cells[4].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[4].Text), Settings["Currency"].ToString()).ToString();
				e.Item.Cells[5].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[5].Text), Settings["Currency"].ToString()).ToString();
				e.Item.Cells[6].Text = new Esperantus.Money(decimal.Parse(e.Item.Cells[6].Text), Settings["Currency"].ToString()).ToString();
			}
			catch // this catch the header of the grid
			{}
		}

		// decode the options xml string, extract the selected options
		// the formatting is coming from the Option.ascx control
		private string DecodeOptions(string metadataXml)
		{
			string selectedOptions = "";
			
			if(metadataXml != null && metadataXml.Length > 0)
			{
				//Create a xml Document
				XmlDocument myXmlDoc = new XmlDocument();


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

		/// <summary>
		/// The PopulateShoppingCartList helper method is used to
		/// dynamically populate a GridControl with the contents of
		/// the current user's shopping cart.
		/// </summary>
		void PopulateShoppingCartList() 
		{
			ShoppingCartDB cart = new ShoppingCartDB();

			// Obtain current user's shopping cart ID
			String cartID = ShoppingCartDB.GetCurrentShoppingCartID();

			// If no items, hide details and display message
			if (cart.GetItemCount(cartID, this.ModuleID) == 0) 
			{
				DisplayCartPanel.Visible = false;
				lblMessage.TextKey = "PRODUCT_EMPTY_CART";
				lblMessage.Text = "There are currently no items in your shopping cart.";
			}
			else 
			{
				// Databind Gridcontrol with Shopping Cart Items
				CartList.DataSource = cart.GetItems(cartID, this.ModuleID, MyVersion);
				CartList.DataBind();
				
				// add totals under the grid
				string currencySymbol = Settings["Currency"].ToString();
				
				decimal currentPrice, currentPriceWithTaxes;
				try
				{
					currentPrice = cart.GetTotal(cartID, this.ModuleID, MyVersion, false);
					lblTotal.Text = new Esperantus.Money(currentPrice, currencySymbol).ToString();
					
					currentPriceWithTaxes = cart.GetTotal(cartID, this.ModuleID, MyVersion, true);
					lblTotalWithTaxes.Text = new Esperantus.Money(currentPriceWithTaxes, currencySymbol).ToString();
					
					lblTotalTaxes.Text = new Esperantus.Money(currentPriceWithTaxes - currentPrice, currencySymbol).ToString();
				}
				catch(NullReferenceException ex)
				{
					lblMessage.Text = ex.Message;
					lblMessage.ForeColor = System.Drawing.Color.Red;
				}
			}
		}

		/// <summary>
		/// The UpdateShoppingCartDatabase helper method is used to
		/// update a user's items within the shopping cart database
		/// using client input from the GridControl.
		/// </summary>
		void UpdateShoppingCartDatabase() 
		{
			ShoppingCartDB cart = new ShoppingCartDB();

			// Obtain current user's shopping cart ID
			String cartID = ShoppingCartDB.GetCurrentShoppingCartID();

			// Iterate through all rows within shopping cart list
			for (int i=0; i < CartList.Items.Count; i++) 
			{
				// Obtain references to row's controls
				TextBox quantityTxt = (TextBox) CartList.Items[i].FindControl("Quantity");
				CheckBox remove = (CheckBox) CartList.Items[i].FindControl("Remove");

				// Wrap in try/catch block to catch errors in the event that someone types in
				// an invalid value for quantity
				int quantity;
				try 
				{
					quantity = Int32.Parse(quantityTxt.Text);

					// Thierry: this is creating an exception
					//Label lblProductID = (Esperantus.WebControls.Label) CartList.Items[i].FindControl("ProductID");
					Label lblProductID = (Label) CartList.Items[i].FindControl("ProductID");
					Label lblSavedMetadataXml = (Label) CartList.Items[i].FindControl("SavedMetadataXml");

					if (quantity == 0 || remove.Checked == true) 
					{
						cart.RemoveItem(cartID, Int32.Parse(lblProductID.Text), this.ModuleID, lblSavedMetadataXml.Text);
					}
					else 
					{
						cart.UpdateItem(cartID, Int32.Parse(lblProductID.Text), quantity, this.ModuleID, lblSavedMetadataXml.Text);
					}
				}
				catch(Exception ex)
				{
					#if DEBUG
						lblMessage.TextKey = "";
						lblMessage.Text = ex.Message;
					#else
						lblMessage.TextKey = "PRODUCT_INPUT_PROBLEM";
						lblMessage.Text = "There has been a problem with one or more of your inputs.";
						lblMessage.Text += "<br>" + ex.Message;
					#endif
				}
			}
		}

		#endregion
		
		private void RedirectToCheckOut()
		{
			string checkoutPage = Rainbow.Settings.Path.WebPathCombine(portalSettings.PortalSecurePath, "ProductsCheckOut.aspx?mID=" + this.ModuleID);

			Response.Redirect(checkoutPage);
		}
		
		//---------------------------------------------------------------------
		// Constructor: 
		/// <summary>
		/// Set the properties of the module
		/// </summary>
		public Products()
		{
			int _groupOrderBase;
			SettingItemGroup _Group;

			// Add support for workflow
			SupportsWorkflow = true;

			#region Ecommerce specific settings
			_Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			_groupOrderBase = (int)_Group;

			//Currency list
			SettingItem Currency = new SettingItem(new Rainbow.UI.DataTypes.CustomListDataType(Esperantus.CurrencyInfo.GetCurrencies(), "DisplayName", "ISOCurrencySymbol"));
			Currency.Order = 2;
			Currency.Value = "EUR";
			Currency.Order = _groupOrderBase + 1;
			Currency.EnglishName = "Currency";
			Currency.Description = "Select the currency to be used by this module. Please note that this will not change the existing prices but it will change the currency symbol (please update the prices as required).";
			this._baseSettings.Add("Currency", Currency);
			#endregion

			#region Ecommerce misc settings
			_Group = SettingItemGroup.MISC_SETTINGS;
			_groupOrderBase = (int)_Group;
			
			// GatewaysData
			
			// Get a list of available gateways
			Rainbow.ECommerce.Gateways.GatewayBase[] gateways = GatewayManager.GetGateways();
			
			DataSet GatewaysList = new DataSet();
			try
			{
				GatewaysList = Rainbow.ECommerce.BusinessLayer.EcommerceMerchants.SelectGateways();
				//Add empty row at start
				DataTable myTable = GatewaysList.Tables[0];
				DataRow dr = myTable.NewRow();
				dr["Name"] = "Do not use this";
				dr["MerchantID"] = "";
				myTable.Rows.InsertAt(dr,0);
			}
			catch(Exception ex)
			{
				Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Error, "GatewaysList build failed. This is normal at installation time.", ex);
			}
			
			// For each Available gateway adds a string property
			// This list is Dynamic and reflects installed gateways in web.config
			int gatewayCounter = 2;
			foreach(Rainbow.ECommerce.Gateways.GatewayBase g in gateways)
			{
				if (GatewaysList != null && g != null)
				{
					SettingItem GatewayProperties = new SettingItem(new CustomListDataType(GatewaysList, "Name", "MerchantID"));
					GatewayProperties.EnglishName = g.Name;
					GatewayProperties.Order = _groupOrderBase + gatewayCounter;
					GatewayProperties.Description = "Insert a reference to MerchantID";
					this._baseSettings.Add(g.Name, GatewayProperties);
					gatewayCounter++;
				}
				else
				{
					if(GatewaysList == null)
						Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Error, "GatewaysList " + gatewayCounter.ToString() + " was null. Counter not incremented.");
					if(g == null)
						Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Error, "Gateway " + gatewayCounter.ToString() + " was null. Counter not incremented.");
				}
			}

			// ShippingsData
			
			// Get a list of available Shippings
			ShippingBase[] Shippings = ShippingManager.GetShippings();
			
			DataSet ShippingList = new DataSet();
			try
			{
				ShippingList = Rainbow.ECommerce.BusinessLayer.EcommerceMerchants.SelectShipping();

				//Add empty row at start
				DataRow dr = ShippingList.Tables[0].NewRow();
				dr["Name"] = "Do not use this";
				dr["MerchantID"] = "";
				ShippingList.Tables[0].Rows.InsertAt(dr,0);
			}
			catch(Exception ex)
			{
				Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Error, "ShippingList build failed. This is normal at installation time.", ex);
			}

			// For each Available Shipping adds a string property
			// This list is Dynamic and reflects installed Shippings in web.config
			int shippingCounter = 1000;
			foreach(ShippingBase g in Shippings)
			{
				if (ShippingList != null && g != null)
				{
					SettingItem ShippingProperties = new SettingItem(new CustomListDataType(ShippingList, "Name", "MerchantID"));
					ShippingProperties.EnglishName = g.Name;
					ShippingProperties.Order = _groupOrderBase + shippingCounter;
					ShippingProperties.Description = "Insert a reference to MerchantID";
					this._baseSettings.Add(g.Name, ShippingProperties);
					shippingCounter++;
				}
				else
				{
					if(ShippingList == null)
						Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Error, "ShippingList " + shippingCounter.ToString() + " was null. Counter not incremented.");
					if(g == null)
						Rainbow.Helpers.LogHelper.Logger.Log(Rainbow.Configuration.LogLevel.Error, "Shipping " + shippingCounter.ToString() + " was null. Counter not incremented.");
				}
			}
			#endregion

			#region Buttons
			_Group = SettingItemGroup.BUTTON_DISPLAY_SETTINGS;
			_groupOrderBase = (int)_Group;

			// Album Path Setting
			SettingItem ShopPath = new SettingItem(new PortalUrlDataType());
			ShopPath.Required = true;
			ShopPath.Value = "Shop";
			ShopPath.Group = _Group;
			ShopPath.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ShopPath", ShopPath);

			// Thumbnail Resize Options
			ArrayList thumbnailResizeOptions = new ArrayList();
			thumbnailResizeOptions.Add(new Option((int)ResizeOption.FixedWidthHeight , "Fixed width and height"));
			thumbnailResizeOptions.Add(new Option((int)ResizeOption.MaintainAspectWidth , "Maintain aspect fixed width"));
			thumbnailResizeOptions.Add(new Option((int)ResizeOption.MaintainAspectHeight , "Maintain aspect fixed height"));

			// Thumbnail Resize Settings
			SettingItem ThumbnailResize = new SettingItem(new CustomListDataType(thumbnailResizeOptions, "Name", "Val"));
			ThumbnailResize.Required = true;
			ThumbnailResize.Value = ((int)ResizeOption.FixedWidthHeight).ToString();
			ThumbnailResize.Group = _Group;
			ThumbnailResize.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ThumbnailResize", ThumbnailResize);

			// Thumbnail Width Setting
			SettingItem ThumbnailWidth = new SettingItem(new IntegerDataType());
			ThumbnailWidth.Required = true;
			ThumbnailWidth.Value = "100";
			ThumbnailWidth.MinValue = 2;
			ThumbnailWidth.MaxValue = 9999;
			ThumbnailWidth.Group = _Group;
			ThumbnailWidth.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ThumbnailWidth", ThumbnailWidth);

			// Thumbnail Height Setting
			SettingItem ThumbnailHeight = new SettingItem(new IntegerDataType());
			ThumbnailHeight.Required = true;
			ThumbnailHeight.Value = "75";
			ThumbnailHeight.MinValue = 2;
			ThumbnailHeight.MaxValue = 9999;
			ThumbnailHeight.Group = _Group;
			ThumbnailHeight.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ThumbnailHeight", ThumbnailHeight);

			// Original Resize Options
			ArrayList originalResizeOptions = new ArrayList();
			originalResizeOptions.Add(new Option((int)ResizeOption.NoResize , "Don't Resize"));
			originalResizeOptions.Add(new Option((int)ResizeOption.FixedWidthHeight , "Fixed width and height"));
			originalResizeOptions.Add(new Option((int)ResizeOption.MaintainAspectWidth , "Maintain aspect fixed width"));
			originalResizeOptions.Add(new Option((int)ResizeOption.MaintainAspectHeight , "Maintain aspect fixed height"));

			// Original Resize Settings
			SettingItem OriginalResize = new SettingItem(new CustomListDataType(originalResizeOptions, "Name", "Val"));
			OriginalResize.Required = true;
			OriginalResize.Value = ((int)ResizeOption.MaintainAspectWidth).ToString();
			OriginalResize.Group = _Group;
			OriginalResize.Order = _groupOrderBase + 1;
			this._baseSettings.Add("OriginalResize", OriginalResize);

			// Original Width Settings
			SettingItem OriginalWidth = new SettingItem(new IntegerDataType());
			OriginalWidth.Required = true;
			OriginalWidth.Value = "800";
			OriginalWidth.MinValue = 2;
			OriginalWidth.MaxValue = 9999;
			OriginalWidth.Group = _Group;
			OriginalWidth.Order = _groupOrderBase + 1;
			this._baseSettings.Add("OriginalWidth", OriginalWidth);

			// Original Width Settings
			SettingItem OriginalHeight = new SettingItem(new IntegerDataType());
			OriginalHeight.Required = true;
			OriginalHeight.Value = "600";
			OriginalHeight.Order = 9;
			OriginalHeight.MinValue = 2;
			OriginalHeight.MaxValue = 9999;
			OriginalHeight.Group = _Group;
			OriginalHeight.Order = _groupOrderBase + 1;
			this._baseSettings.Add("OriginalHeight", OriginalHeight);

			// Repeat Direction Options
			ArrayList repeatDirectionOptions = new ArrayList();
			repeatDirectionOptions.Add(new Option((int)RepeatDirection.Horizontal , "Horizontal"));
			repeatDirectionOptions.Add(new Option((int)RepeatDirection.Vertical , "Vertical"));

			// Repeat Direction Setting
			SettingItem RepeatDirectionSetting = new SettingItem(new CustomListDataType(repeatDirectionOptions, "Name", "Val"));
			RepeatDirectionSetting.Required = true;
			RepeatDirectionSetting.Value = ((int)RepeatDirection.Horizontal).ToString();
			RepeatDirectionSetting.Group = _Group;
			RepeatDirectionSetting.Order = _groupOrderBase + 1;
			this._baseSettings.Add("RepeatDirection", RepeatDirectionSetting);

			// Repeat Columns Setting
			SettingItem RepeatColumns = new SettingItem(new IntegerDataType());
			RepeatColumns.Required = true;
			RepeatColumns.Value = "2";
			RepeatColumns.MinValue = 1;
			RepeatColumns.MaxValue = 200;
			RepeatColumns.Group = _Group;
			RepeatColumns.Order = _groupOrderBase + 1;
			this._baseSettings.Add("RepeatColumns", RepeatColumns);

			
			// ProductsPerPage
			SettingItem ProductsPerPage = new SettingItem(new IntegerDataType());
			ProductsPerPage.Required = true;
			ProductsPerPage.Value = "9999";
			ProductsPerPage.Order = 10;
			ProductsPerPage.MinValue = 1;
			ProductsPerPage.MaxValue = 9999;
			ProductsPerPage.Group = _Group;
			ProductsPerPage.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ProductsPerPage", ProductsPerPage);
			#endregion

			#region Layouts
			_Group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
			_groupOrderBase = (int)_Group;

			// Layouts
			Hashtable layouts = new Hashtable();
			foreach(string layoutControl in System.IO.Directory.GetFiles(HttpContext.Current.Server.MapPath(Rainbow.Settings.Path.ApplicationRoot + "/ECommerce/Design/ProductLayouts"), "*.ascx"))
			{
				string layoutControlDisplayName = layoutControl.Substring(layoutControl.LastIndexOf("\\") + 1, layoutControl.LastIndexOf(".") - layoutControl.LastIndexOf("\\") - 1);
				string layoutControlName = layoutControl.Substring(layoutControl.LastIndexOf("\\") + 1);
				layouts.Add(layoutControlDisplayName, layoutControlName);
			}

			// Thumbnail Layout Setting
			SettingItem ThumbnailLayoutSetting = new SettingItem(new CustomListDataType(layouts, "Key", "Value"));
			ThumbnailLayoutSetting.Required = true;
			ThumbnailLayoutSetting.Value = "DefaultThumbnailView.ascx";
			ThumbnailLayoutSetting.Order = 8;
			ThumbnailLayoutSetting.Group = _Group;
			ThumbnailLayoutSetting.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ThumbnailLayout", ThumbnailLayoutSetting);

			// Product Layout Setting
			SettingItem ProductLayoutSetting = new SettingItem(new CustomListDataType(layouts, "Key", "Value"));
			ProductLayoutSetting.Required = true;
			ProductLayoutSetting.Value = "DefaultProductView.ascx";
			ProductLayoutSetting.Order = 9;
			ProductLayoutSetting.Group = _Group;
			ProductLayoutSetting.Order = _groupOrderBase + 1;
			this._baseSettings.Add("ProductLayout", ProductLayoutSetting);
			#endregion
		}

		private bool ValidateXml(string xml)
		{
			bool validXml = false;
			//Load the XML data into memory
			XmlValidatingReader valReader = new 
				XmlValidatingReader(xml,XmlNodeType.Document,null);
			//valReader.Schemas.Add(null, Server.MapPath("Categories.xsd"));
			valReader.ValidationType = ValidationType.Auto;
			//Loop through the XML file 
			try 
			{
				while(valReader.Read())
				{}
			}
			catch 
			{
				validXml = false;
				return validXml;
			}
			if (builder.Length > 0)
				validXml = false;
			else
				validXml = true;
			valReader.Close();
			return validXml;
		}


		//---------------------------------------------------------------------
		/// <summary>
		/// Overriden from PortalModuleControl, 
		/// this override deletes unnecessary 
		/// picture files from the system
		/// </summary>		
		protected override void Publish()
		{
			string pathToDelete = Server.MapPath(((SettingItem) Settings["ShopPath"]).FullPath) + "\\";

			DirectoryInfo albumDirectory = new DirectoryInfo(pathToDelete);

			foreach(FileInfo fi in albumDirectory.GetFiles(ModuleID.ToString() + "m*.Production"))
			{
				try
				{
					System.IO.File.Delete(fi.FullName);
				}
				catch {}
			}

			foreach(FileInfo fi in albumDirectory.GetFiles(ModuleID.ToString() + "m*"))
			{
				try
				{
					System.IO.File.Copy(fi.FullName, fi.FullName + ".Production", true);
				}
				catch {}
			}

			base.Publish();
		}

		/// <summary>
		/// Given a key returns the value
		/// </summary>
		/// <param name="MetadataXml">XmlDocument containing key value pairs in attributes</param>
		/// <param name="key">key of the pair</param>
		/// <returns>value</returns>
		protected string GetMetadata(object MetadataXml, string key)
		{
			XmlDocument Metadata = new XmlDocument();
			XmlNode targetNode = null;

			if(MetadataXml != null)
			{
				string strMetadataXml = (string) MetadataXml;
				Metadata.LoadXml(strMetadataXml);
				targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);
			}

			if (targetNode == null)
			{
				return null;
			}
			else
			{
				return targetNode.Value;
			}
		}

		#region Global Implementation
		/// <summary>
		/// GuidID
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{EC24FABD-FB16-4978-8C81-1ADD39792377}");
			}
		}

		#region Search Implementation
		/// <summary>
		/// Searchable module
		/// </summary>
		public override bool Searchable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Searchable module implementation
		/// </summary>
		/// <param name="portalID">The portal ID</param>
		/// <param name="userID">ID of the user is searching</param>
		/// <param name="searchString">The text to search</param>
		/// <param name="searchField">The fields where perfoming the search</param>
		/// <returns>The SELECT sql to perform a search on the current module</returns>
		public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
		{
			// Parameters:
			// Table Name: the table that holds the data
			// Title field: the field that contains the title for result, must be a field in the table
			// Abstract field: the field that contains the text for result, must be a field in the table
			// Search field: pass the searchField parater you recieve.

			Rainbow.Helpers.SearchDefinition s = new Rainbow.Helpers.SearchDefinition("rb_Products", "itm.CategoryID", "ProductID", "ModelName", "ShortDescription", null, null, searchField);
			
			//Add here extra search fields, this way
			s.ArrSearchFields.Add("itm.LongDescription");
			s.ArrSearchFields.Add("itm.ModelNumber");

			// Builds and returns the SELECT query
			return s.SearchSqlSelect(portalID, userID, searchString, true);
		}
		#endregion

		# region Install / Uninstall Implementation
		public override void Install(System.Collections.IDictionary stateSaver)
		{
			string currentScriptName = Server.MapPath(this.TemplateSourceDirectory + "/Install.sql");
			ArrayList errors = Rainbow.Helpers.DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}

		public override void Uninstall(System.Collections.IDictionary stateSaver)
		{
			string currentScriptName = Server.MapPath(this.TemplateSourceDirectory + "/Uninstall.sql");
			ArrayList errors = Rainbow.Helpers.DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}
		# endregion

		#endregion

		/// <summary>
		/// Structure used for list settings in the Constructor
		/// </summary>
		public struct Option
		{
			private int val;
			private string name;

			public int Val 
			{
				get { return this.val; }
				set { this.val = value; }
			}

			public string Name
			{
				get { return this.name; }
				set { this.name = value; }
			}

			public Option(int aVal, string aName)
			{
				val = aVal;
				name = aName;
			}
		}
	}
}
