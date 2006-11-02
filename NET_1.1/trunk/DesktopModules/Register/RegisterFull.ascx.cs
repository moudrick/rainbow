using System;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.UI.WebControls;
using CheckBox = Esperantus.WebControls.CheckBox;
using CompareValidator = Esperantus.WebControls.CompareValidator;
using CustomValidator = Esperantus.WebControls.CustomValidator;
using Label = Esperantus.WebControls.Label;
using LinkButton = Esperantus.WebControls.LinkButton;
using RegularExpressionValidator = Esperantus.WebControls.RegularExpressionValidator;
using RequiredFieldValidator = Esperantus.WebControls.RequiredFieldValidator;

namespace Rainbow.DesktopModules
{
    /// <summary>
    /// Placeable Registration (Full) module
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
	[History("john.mandia@whitelightsolutions.com","2005/02/12","Adding handling for unique constraint violation and showed friendlier message.")]
	[History("bill@improvdesign.com","2004/10/23","Added email to admin on registration")]
	[History("john.mandia@whitelightsolutions.com","2003/10/31","Fixed bug where old state list would remain even though new country with no states had been selected.")]
	[History("Mario Hartmann","mario@hartmann.net","1.2","2003/10/08","moved to seperate folder")]
	[History("Jes1111","2003/03/10","Modified from original to be fully placeable module")]
	[History("Jes1111","2003/03/10","Updated to use Globalized controls")]
	//TODO still needs Globalized field validators (Compare and Regex)
    public class RegisterFull : PortalModuleControl, IEditUserProfile
    {
		#region Form controls
		protected Label PageTitleLabel;
		protected Label NameLabel;
		protected TextBox NameField;
		protected RequiredFieldValidator RequiredName;
		protected Label CompanyLabel;
		protected TextBox CompanyField;
		protected Label AddressLabel;
		protected TextBox AddressField;
		protected Label CityLabel;
		protected TextBox CityField;
		protected Label ZipLabel;
		protected TextBox ZipField;
		protected Label CountryLabel;
		protected DropDownList CountryField;
		protected Label StateLabel;
		protected DropDownList StateField;
		protected Label InLabel;
		protected System.Web.UI.WebControls.Label ThisCountryLabel;
		protected Label PhoneLabel;
		protected TextBox PhoneField;
		protected Label FaxLabel;
		protected TextBox FaxField;
		protected Label PIvaLabel;
		protected TextBox PIvaField;
		protected Label CFiscaleLabel;
		protected TextBox CFiscaleField;
		protected Label SendNewsletterLabel;
		protected System.Web.UI.WebControls.CheckBox SendNewsletter;
		protected Label AccountLabel;
		protected Label EmailLabel;
		protected TextBox EmailField;
		protected RegularExpressionValidator ValidEmail;
		protected RequiredFieldValidator RequiredEmail;
		protected Label PasswordLabel;
		protected TextBox PasswordField;
		protected RequiredFieldValidator RequiredPassword;
		protected Label ConfirmPasswordLabel;
		protected TextBox ConfirmPasswordField;
		protected RequiredFieldValidator RequiredConfirm;
		protected Label ConditionsLabel;
		protected TextBox FieldConditions;
		protected CheckBox Accept;
		protected CustomValidator CheckTermsValidator;
		protected LinkButton RegisterBtn;
		protected LinkButton cancelButton;
		protected System.Web.UI.WebControls.Label Message;
		protected HtmlTableRow StateRow;
		protected HtmlTableRow PIvaRow;
		protected HtmlTableRow CFiscaleRow;
		protected Label Label1;
		protected HtmlTableRow EditPasswordRow;
		protected HtmlTableRow ConditionsRow;
		protected LinkButton SaveChangesBtn;
		protected CompareValidator ComparePasswords;
		protected Label UseridLabel;
		protected TextBox UseridField;
		protected HtmlTableRow UserIDRow;
		protected CompareValidator CheckID;
		protected Panel FullProfileInformation;
		#endregion

		#region Private Fields
		private string _redirectPage;
		#endregion
        
		#region Properties
		public bool EditMode
		{
			get
			{ 
				return (userName.Length != 0);
			}
		}
		

		public string RedirectPage
		{
			get
			{
				if (_redirectPage == null)
				{
					// changed by Mario Endara <mario@softworks.com.uy> (2004/11/05)
					// it's necessary the ModuleID in the URL to apply security checking in the target
					return Request.Url.Segments[Request.Url.Segments.Length - 1] + "?TabID=" + PageID + "&mID=" + ModuleID + "&userid=" + UseridField.Text + "&username=" + EmailField.Text;
				}
				return _redirectPage;
			}
			set
			{
				_redirectPage = value; 
			}
		}


		/// <summary>
		/// If we add a parameter AllowEditUserID the control go in Allow ID edit mode
		/// </summary>
		private bool allowEditUserID
		{
			get
			{
//				if (Request.Params["AllowEditUserID"] != null)
//					return true;
//				return false;
				return portalSettings.AllowEditUserID;
			}		
		}
		private string userName
		{
			get
			{
				string uid = string.Empty;
				if (Request.Params["userName"] != null)
					uid = Request.Params["userName"];
				return uid;
			}		
		}

		private int originalUserID
		{
			get
			{
				if(ViewState["originalUserID"] != null)
					return (int) ViewState["originalUserID"];
				else
					return 0;
			}
			set
			{
				ViewState["originalUserID"] = value;
			}
		}

		private string originalPassword
		{
			get
			{
				if(ViewState["originalPassword"] != null)
					return (string) ViewState["originalPassword"];
				else
					return string.Empty;
			}
			set
			{
				ViewState["originalPassword"] = value;
			}
		}
		
		private bool selfEdit
		{
			get
			{
				if(ViewState["selfEdit"] != null)
					return (bool) ViewState["selfEdit"];
				else
					return false;
			}
			set
			{
				ViewState["selfEdit"] = value;
			}
		}
		#endregion

		#region	Methods

		private void BindCountry()
		{
			//Country filter limits contry list
			string CountriesFilter = portalSettings.CustomSettings["SITESETTINGS_COUNTRY_FILTER"].ToString();

			CountryField.Items.Clear();
			if (CountriesFilter.Length != 0)
			{
				CountryField.DataSource = CountryInfo.GetCountries(CountriesFilter);
			}
			else
			{
				CountryField.DataSource = CountryInfo.GetCountries(CountryTypes.InhabitedCountries,CountryFields.DisplayName);
			}
			CountryField.DataBind();
		}

		private void BindState()
		{
			StateRow.Visible = false;
			if (CountryField.SelectedItem != null)
			{
				string currentCountry = CountryField.SelectedItem.Value.ToString();
				//added next line to clear the list. 
				//The stateField seems to remember it's values even when you set the 
				//DataSource to null
				//Michel Barneveld Rainbow@MichelBarneveld.Com
				StateField.Items.Clear();
				StateField.DataSource = new CountryInfo(currentCountry).Childs;
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

		public int SaveUserData()
		{
			int returnID = 0;

			if (PasswordField.Text.Length > 0 || ConfirmPasswordField.Text.Length > 0)
			{
				if (PasswordField.Text != ConfirmPasswordField.Text)
					ComparePasswords.IsValid = false;
			}

			// Only attempt a login if all form fields on the page are valid
			if (Page.IsValid == true)
			{
				UsersDB accountSystem = new UsersDB();

				string CountryID = string.Empty;
				if (CountryField.SelectedItem != null)
					CountryID = CountryField.SelectedItem.Value;

				int StateID = 0;
				if (StateField.SelectedItem != null)
					StateID = Convert.ToInt32(StateField.SelectedItem.Value);
            
				try
				{
					if (userName == string.Empty)
					{
						// Add New User to Portal User Database
						returnID = accountSystem.AddUser(portalSettings.PortalID, NameField.Text, CompanyField.Text, AddressField.Text, CityField.Text, ZipField.Text, CountryID, StateID, PIvaField.Text,  CFiscaleField.Text, PhoneField.Text, FaxField.Text, PasswordField.Text, EmailField.Text, SendNewsletter.Checked);
					}
					else
					{
						// Update user
						if (allowEditUserID && Int32.Parse(UseridField.Text) > 0)
						{
							//If allow id
							int currentUserID = Int32.Parse(UseridField.Text);
							accountSystem.UpdateUser(originalUserID, currentUserID, portalSettings.PortalID, NameField.Text, CompanyField.Text, AddressField.Text, CityField.Text, ZipField.Text, CountryID, StateID, PIvaField.Text,  CFiscaleField.Text, PhoneField.Text, FaxField.Text, PasswordField.Text, EmailField.Text, SendNewsletter.Checked);
						}
						else
						{
							//Update user throws any error occurs
							accountSystem.UpdateUser(originalUserID, portalSettings.PortalID, NameField.Text, CompanyField.Text, AddressField.Text, CityField.Text, ZipField.Text, CountryID, StateID, PIvaField.Text,  CFiscaleField.Text, PhoneField.Text, FaxField.Text, PasswordField.Text, EmailField.Text, SendNewsletter.Checked);
						}
						//If we are here no error occurred
						returnID = 1;
					}
				}
				catch(Exception ex)
				{
					Message.Text = Localize.GetString("REGISTRATION_FAILED", "Registration failed", Message);
					
					if(ex is SqlException)
					{
						if((((SqlException)ex).Number == 2627))
						{
							Message.Text = Localize.GetString("REGISTRATION_FAILED_EXISTING_EMAIL_ADDRESS", "Registration has failed. This email address has already been registered. Please use a different email address or use the 'Send Password' button on the login page.", Message);					
						}
					}
				
					ErrorHandler.Publish(LogLevel.Error, "Error registering user", ex);
				}
				
			}
			return returnID;
		}
      

		/// <summary>
		/// Sends registration information to portal administrator.
		/// </summary>
		public void SendRegistrationNoticeToAdmin()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append("New User Registration\n");
			sb.Append("---------------------\n");
			sb.Append("PORTAL         : " + portalSettings.PortalTitle + "\n");
			sb.Append("Name           : " + NameField.Text + "\n");
			sb.Append("Company        : " + CompanyField.Text + "\n");
			sb.Append("Address        : " + AddressField.Text + "\n");
			sb.Append("                 " + CityField.Text + ", ");
			if(StateField.SelectedItem!=null)
			  sb.Append(StateField.SelectedItem.Text + "  ");
			sb.Append(ZipField.Text + "\n");
			sb.Append("                 " + CountryField.SelectedItem.Text + "\n");
			sb.Append("                 " + PhoneField.Text + "\n");
			sb.Append("Fax            : " + FaxField.Text + "\n");
			sb.Append("Email          : " + EmailField.Text + "\n");
			sb.Append("Send Newsletter: " + SendNewsletter.Checked.ToString() + "\n");
			
			MailHelper.SendMailNoAttachment( 
				portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_TO"].ToString(),
				portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_TO"].ToString(),
				"New User Registration for " + portalSettings.PortalAlias,
				sb.ToString(),
				string.Empty,
				string.Empty,
				Config.SmtpServer);
		
		}

		#endregion

		#region Events
		private void RegisterBtn_Click(object sender, EventArgs e)
		{
			int returnID = SaveUserData();
				
			if (returnID >= 1)  
			{
				if(portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_TO"].ToString().Length > 0)
					SendRegistrationNoticeToAdmin();
				//Full signon
				PortalSecurity.SignOn(EmailField.Text, PasswordField.Text, false, RedirectPage);
			}
		}
		
		private void SaveChangesBtn_Click(object sender, EventArgs e)
		{
			int returnID = SaveUserData();
				
			if (returnID == 1) 
			{
				if(selfEdit)
				{
					//All should be ok now
					//Try logoff user
					PortalSecurity.SignOut(string.Empty, true);
				
					//Logon user again with new settings
					string actualPassword;
					if (PasswordField.Text.Length != 0)
						actualPassword = PasswordField.Text;
					else
						actualPassword = originalPassword;

					//Full signon
					PortalSecurity.SignOn(EmailField.Text, actualPassword, false, RedirectPage);
				}
				else if (RedirectPage == string.Empty)
				{
					// Redirect browser back to home page
					PortalSecurity.PortalHome();
				}
				else
				{
					Response.Redirect(RedirectPage);
				}
			}
		}

		private void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false) 
			{
				//Remove validation for Windows users
				if (HttpContext.Current != null && Context.User is WindowsPrincipal)
				{
					this.ValidEmail.Visible = false;
					this.EmailLabel.TextKey = "WINDOWS_USER_NAME";
					this.EmailLabel.Text = "Windows User Name";
				}

				//If allow id and user is not new show id row
				//When we create an user, id is ignored
				if (allowEditUserID && userName.Length != 0)
					UserIDRow.Visible = true;
			
				BindCountry();

				//Bind to current language country
				CountryField.ClearSelection();
				
				CountryInfo country = CountryInfo.CurrentCountry;
				if (country != null && CountryField.Items.FindByValue(country.Name) != null)
					CountryField.Items.FindByValue(country.Name).Selected = true;
				BindState();


				// Edit check
				if (EditMode) // Someone requested edit this record
				{
					//True is use is editing himself, false if is edited by an admin
					selfEdit = (userName == PortalSettings.CurrentUser.Identity.Email);

					// Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
//					if (PortalSecurity.IsInRoles("Admins") || selfEdit)
					if (PortalSecurity.HasEditPermissions(ModuleID) || PortalSecurity.HasAddPermissions(ModuleID) || selfEdit)
					{
						//We can edit

						// Hide
						RequiredPassword.Visible = false;
						RequiredConfirm.Visible = false;
						EditPasswordRow.Visible = true;
						SaveChangesBtn.Visible = true;
						RegisterBtn.Visible = false;

						// Obtain a single row of event information
						UsersDB accountSystem = new UsersDB();
						SqlDataReader dr = accountSystem.GetSingleUser(userName, portalSettings.PortalID);
                
						try
						{
							// Read first row from database
							if (dr.Read())
							{
								UseridField.Text = dr["UserID"].ToString();
								//stores original user id for later check
								originalUserID = int.Parse(dr["UserID"].ToString());
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
								SendNewsletter.Checked = bool.Parse(dr["SendNewsletter"].ToString()); 
							
								//stores original password for later check
								originalPassword = dr["Password"].ToString();
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
						PortalSecurity.AccessDeniedEdit();
					}
				}
				else
				{
					BindState();

					//No edit
					RequiredPassword.Visible = true;
					RequiredConfirm.Visible = true;
					EditPasswordRow.Visible = false;
					SaveChangesBtn.Visible = false;
					RegisterBtn.Visible = true;
				}

				string termsOfService = portalSettings.GetTermsOfService;

				//Verify if we have to show conditions
				if (termsOfService.Length != 0)
				{
					//Shows conditions
					FieldConditions.Text = termsOfService;
					ConditionsRow.Visible = true;
				}
				else
				{
					//Hides conditions
					ConditionsRow.Visible = false;
				}
			}
		}

		private void CountryField_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindState();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Page.RedirectBackToReferringPage();
		}

		private void CheckTermsValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = Accept.Checked;
		}
		#endregion

		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() 
        {    
			this.CountryField.SelectedIndexChanged += new EventHandler(this.CountryField_SelectedIndexChanged);
			this.CheckTermsValidator.ServerValidate += new ServerValidateEventHandler(this.CheckTermsValidator_ServerValidate);
			this.RegisterBtn.Click += new EventHandler(this.RegisterBtn_Click);
			this.SaveChangesBtn.Click += new EventHandler(this.SaveChangesBtn_Click);
			this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

		public override Guid GuidID 
		{
			get
			{
				return new Guid("{AE419DCC-B890-43ba-B77C-54955F182041}");
			}
		}

    }
}