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
	
namespace Rainbow.ECommerce.DesktopModules
{
	/// <summary>
	///	Descrizione di riepilogo per EcommUpdateProfile.
	/// </summary>
	public abstract class EcommUpdateProfile : System.Web.UI.UserControl
	{
		protected Esperantus.WebControls.Label MyError;

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
		protected Esperantus.WebControls.Label PartitaIvaLabel;
		protected System.Web.UI.WebControls.TextBox PartitaIvaField;
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
		protected System.Web.UI.WebControls.Panel PanelStep1;
		protected System.Web.UI.HtmlControls.HtmlTableRow StateRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow PartitaIvaRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow CFiscaleRow;

		PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
		
		#region Events
		private void Page_Load(object sender, System.EventArgs e)
		{
			// readonly for the email field!
			// the user will have to go to change profile
			EmailField.Enabled = false;

			BindCountry();

			//Bind to current language country
			CountryField.ClearSelection();
			Esperantus.CountryInfo country = Esperantus.CountryInfo.CurrentCountry;
			if (country != null && CountryField.Items.FindByValue(country.Name) != null)
				CountryField.Items.FindByValue(country.Name).Selected = true;

			BindState();
		}


		private void UpdateProfileBtn_Click(object sender, System.EventArgs e)
		{
			string updateProfilePage = "Register.aspx?UserName=" + PortalSettings.CurrentUser.Identity.Email + "&mID=" + Request.QueryString["mID"]; //by Manu. We need mID to come back
			Response.Redirect(updateProfilePage); 
		}

		#endregion

		#region Bindings
		private void BindData()
		{
			BindCountry();
			BindState();
		}


		private void BindCountry()
		{
			//Country filter limits contry list
			string CountriesFilter = portalSettings.CustomSettings["SITESETTINGS_COUNTRY_FILTER"].ToString();

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

				//Hides Codfis e PartitaIva if not in Italy
				if (CountryField.SelectedItem.Value == "IT")
				{
					CFiscaleRow.Visible = true;
					PartitaIvaRow.Visible = true;
				}
				else
				{
					CFiscaleRow.Visible = false;
					PartitaIvaRow.Visible = false;
				}
			}
		}
		#endregion

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
			this.UpdateProfileBtn.Click += new System.EventHandler(this.UpdateProfileBtn_Click);
			this.PanelStep1.Load += new System.EventHandler(this.PanelStep1_Load);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Persists the data
		/// </summary>
		public virtual void UpdateUserData()
		{
			string CountryID = "";
			if (CountryField.SelectedItem != null)
				CountryID = CountryField.SelectedItem.Value;

			int StateID = 0;
			if (StateField.SelectedItem != null)
				StateID = Convert.ToInt32(StateField.SelectedItem.Value);
		
			// Update user
			UsersDB user = new UsersDB();
			int userID = user.GetCurrentUserID(PortalSettings.CurrentUser.Identity.Email, portalSettings.PortalID);
			user.UpdateUser(userID, portalSettings.PortalID, NameField.Text, CompanyField.Text, AddressField.Text, CityField.Text, ZipField.Text, CountryID, StateID, PartitaIvaField.Text, CFiscaleField.Text, PhoneField.Text, FaxField.Text, EmailField.Text, SendNewsletter.Checked);
		}

		/// <summary>
		/// Get data FROM the db
		/// </summary>
		public virtual void LoadUserData()
		{
			// Edit check
			string userID = PortalSettings.CurrentUser.Identity.Email;
			if (userID != string.Empty) 
			{
				// Obtain a single row of event information
				UsersDB accountSystem = new UsersDB();
				SqlDataReader dr = accountSystem.GetSingleUser(userID, portalSettings.PortalID);
        
				try
				{
					// Read first row FROM database
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
						PartitaIvaField.Text = dr["PIva"].ToString();
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

		/// <summary>
		/// Returns a Shopperdata object FROM current screen
		/// </summary>
		/// <returns></returns>
		public ShopperData GetShopperData()
		{
			//Create Shipping ShopperData Object
			ShopperData sd = new ShopperData();
			sd.FullName = NameField.Text;
			sd.Company = CompanyField.Text;
			sd.Address = AddressField.Text;
			sd.City = CityField.Text;
			sd.ZipCode = ZipField.Text;
			sd.Phone = PhoneField.Text;
			sd.Country = CountryField.SelectedItem.Text;
			sd.State = StateField.SelectedItem.Text;
			sd.StateID = StateField.SelectedValue;
			sd.EMail = EmailField.Text;
			sd.Note = string.Empty; //TODO: save customer note
			return sd;		
		}

		private void PanelStep1_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				LoadUserData();
		}
	}
}