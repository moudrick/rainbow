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
using Rainbow.Admin;
using Rainbow.Security;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.ECommerce;
using Rainbow.ECommerce.Gateways;
using Rainbow.ECommerce.DesktopModules;
	
namespace Rainbow.ECommerce.DesktopModules
{
	/// <summary>
	///	Descrizione di riepilogo per EcommSelectPayment.
	/// </summary>
	public abstract class EcommSelectPayment : System.Web.UI.UserControl
	{
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

		protected Esperantus.WebControls.Label lblSelectPayment;
		protected Esperantus.WebControls.LinkButton Panel3ContinueBtn;
		protected System.Web.UI.WebControls.DropDownList ddSelectPayment;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindGateways();
		}

		private Hashtable _moduleSettings;
		public Hashtable moduleSettings
		{
			get
			{
				return _moduleSettings;
			}
			set
			{
				_moduleSettings = value;
			}
		}

		/// <summary>
		/// Binding the list of available Gateways
		/// </summary>
		public void BindGateways()
		{
			// This list is Dynamic and reflects installed gateways in web.config
			Rainbow.ECommerce.Gateways.GatewayBase[] gateways = GatewayManager.GetGateways();

			// Now we initialize all gateways that have a setting in this product module.
			ArrayList availableGateways = new ArrayList();

			// For each Available gateway check if properties are set
			foreach(Rainbow.ECommerce.Gateways.GatewayBase g in gateways)
			{
				// We add gateway if properties are set
				if (moduleSettings[g.Name] != null && moduleSettings[g.Name].ToString().Length > 0)
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
			}

			// Do we have at least one gateway?
			if (!(availableGateways.Count > 0))
				throw new ArgumentNullException("Gateways", "Unable to find a suitable gateway. Provide some parameters for at least one gateway.");

			ddSelectPayment.DataSource = availableGateways;
			ddSelectPayment.DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: questa chiamata è richiesta da Progettazione Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Metodo necessario per il supporto della finestra di progettazione. Non modificare
		///		il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			this.Panel3ContinueBtn.Click += new System.EventHandler(this.Panel3ContinueBtn_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Purpose: Notify when Gateway has been Selected.
		/// </summary>
		/// <delegate>EventHandler</delegate>
		[field:NonSerialized()]
		public event EventHandler GatewaySelected;

		private Rainbow.ECommerce.Gateways.GatewayBase _currentGateway;
		public Rainbow.ECommerce.Gateways.GatewayBase CurrentGateway
		{
			get
			{
				return _currentGateway;
			}
			set
			{
				_currentGateway = value;
			}
		}

		public Order Order
		{
			get
			{
				if (ViewState["Order"] == null)
					ViewState["Order"] = new Order();
                return (Order) ViewState["Order"];
			}
			set
			{
				ViewState["Order"] = value;
			}
		}

		private void SelectGateway()
		{
			if (Order == null)
				throw new ArgumentNullException("Order", "Order cannot be null");

			if (ddSelectPayment.SelectedItem == null)
				throw new ArgumentNullException("Gateways", "Unable to find a suitable gateway. Provide some parameters for at least one gateway.");

			// Get gateway type FROM the current user selection
			string gatewayType = ddSelectPayment.SelectedValue;

			// Get parameters specified in module properties
			string merchantSettings = moduleSettings[gatewayType].ToString();

			// Get a reference to specified gateway and load data for specified merchant in it
			CurrentGateway = GatewayManager.GetGatewayMerchant(gatewayType, merchantSettings);

			// Add gateway information to the page
			CurrentGateway.Price = Order.Total;
			CurrentGateway.OrderID = Order.OrderID;

			if (GatewaySelected != null) //calls delegate
				GatewaySelected(CurrentGateway, null);
		}

		private void Panel3ContinueBtn_Click(object sender, System.EventArgs e)
		{
			SelectGateway();
		}
	}
}
