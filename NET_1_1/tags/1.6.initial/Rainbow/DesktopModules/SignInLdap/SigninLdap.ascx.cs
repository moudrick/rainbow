using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mail;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;
using Button = Esperantus.WebControls.Button;
using CheckBox = Esperantus.WebControls.CheckBox;
using Label = Esperantus.WebControls.Label;
using LinkButton = Esperantus.WebControls.LinkButton;
using Literal = Esperantus.WebControls.Literal;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// The SignIn User Control enables clients to authenticate themselves using 
	/// the ASP.NET Forms based authentication system.
	///
	/// When a client enters their username/password within the appropriate
	/// textboxes and clicks the "Login" button, the LoginBtn_Click event
	/// handler executes on the server and attempts to validate their
	/// credentials against a SQL database.
	///
	/// If the password check succeeds, then the LoginBtn_Click event handler
	/// sets the customers username in an encrypted cookieID and redirects
	/// back to the portal home page.
	/// 
	/// If the password check fails, then an appropriate error message
	/// is displayed.
	/// </summary>
	public class SigninLdap : PortalModuleControl
	{
		protected Literal LoginTitle;
		protected Literal EmailLabel;
		protected TextBox email;
		protected Literal PasswordLabel;
		protected TextBox password;
		protected CheckBox RememberCheckBox;
		protected Button LoginBtn;
		protected LinkButton RegisterBtn;
		protected LinkButton SendPasswordBtn;
		protected Literal Group;
		protected DropDownList GroupList;
		protected HtmlTableRow TRGroup;
		protected HtmlTableRow TRGroupList;
		protected CheckBox LDAPCheckBox;
		protected Label Message;		

		private void LoginBtn_Click(Object sender, EventArgs e) 
		{
			// modified by Jonathan Fong
			// www.gt.com.au
			string id = null;
			if (LDAPCheckBox.Checked && GroupList.SelectedValue != "Default")
			{
				string dn = "cn=" + email.Text + "," + GroupList.SelectedValue;
				LDAPHelper.SignOn(dn, password.Text, RememberCheckBox.Checked, null);
			}
			else
			{
				id = PortalSecurity.SignOn(email.Text, password.Text, RememberCheckBox.Checked);
			}

			if (id == null)
			{
				Message.Text = "Login failed";
				Message.TextKey = "LOGIN_FAILED";
			}
		}
        
		private void RegisterBtn_Click(object sender, EventArgs e)
		{
			Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/Register/Register.aspx"));
		}

		
		private void SendPasswordBtn_Click(object sender, EventArgs e)
		{
			if (email.Text == string.Empty)
			{
				Message.Text = "Please enter you email address";
				Message.TextKey = "SIGNIN_ENTER_EMAIL_ADDR";
				return;
			}
			
			// generate random password
			

			CryptoHelper crypthelp = new CryptoHelper();
			UsersDB usersDB = new UsersDB();
			string randomPassword = RandomPassword.Generate(8, 10);
			//Obtain single row of User information
			SqlDataReader dr = usersDB.GetSingleUser(email.Text, portalSettings.PortalID);
			try
			{
				if (dr.Read())
				{
					string Pswrd;
					string AppName = portalSettings.PortalName;
					bool encrypted = Config.EncryptPassword;
					string Name = (string) dr["Email"];
					if(encrypted) 
					{
						Pswrd = randomPassword;
						crypthelp.ResetPassword(Name, randomPassword);
					}
					else 
					{
						Pswrd = (string)dr["Password"];
					}

					string LoginUrl = Path.ApplicationFullPath + "DesktopModules/Admin/Logon.aspx?Usr=" + Name + "&Pwd=" + Pswrd + "&Alias=" + portalSettings.PortalAlias;
					MailMessage mail = new MailMessage();
				
					// Geert.Audenaert@Syntegra.Com
					// Date 19 March 2003
					// We have to use a correct sender address, 
					// because most SMTP servers reject it otherwise
					//jes1111 - mail.From = ConfigurationSettings.AppSettings["EmailFrom"].ToString();
					mail.From = Config.EmailFrom;
					mail.To = email.Text;
					mail.Subject = AppName + " - " + Localize.GetString("SIGNIN_SEND_PWD", "Send me password", this);

					StringBuilder sb = new StringBuilder();

					sb.Append(Name);
					sb.Append(",");
					sb.Append("\r\n\r\n");
					sb.Append(Localize.GetString("SIGNIN_PWD_REQUESTED", "This is the password you requested", this));
					sb.Append(" ");
					sb.Append(Pswrd);
					sb.Append("\r\n\r\n");
					sb.Append(Localize.GetString("SIGNIN_THANK_YOU", "Thanks for your visit.", this));
					sb.Append(" ");
					sb.Append(AppName);
					sb.Append("\r\n\r\n");
					sb.Append(Localize.GetString("SIGNIN_YOU_CAN_LOGIN_FROM", "You can login from", this));
					sb.Append(":");
					sb.Append("\r\n");
					sb.Append(Path.ApplicationFullPath);
					sb.Append("\r\n\r\n");
					sb.Append(Localize.GetString("SIGNIN_USE_DIRECT_URL", "Or using direct url", this));
					sb.Append("\r\n");
					sb.Append(LoginUrl);
					sb.Append("\r\n\r\n");
					sb.Append(Localize.GetString("SIGNIN_URL_WARNING", "NOTE: The address above may not show up on your screen as one line. This would prevent you from using the link to access the web page. If this happens, just use the 'cut' and 'paste' options to join the pieces of the URL.", this));

					mail.Body = sb.ToString();
					mail.BodyFormat = MailFormat.Text;

					SmtpMail.SmtpServer = Config.SmtpServer;
					SmtpMail.Send(mail);

					Message.Text = Localize.GetString("SIGNIN_PWD_WAS_SENT", "Your password was sent to the addess you provided", this);
					Message.TextKey = "SIGNIN_PWD_WAS_SENT";
				}
				else 
				{
					Message.Text = Localize.GetString("SIGNIN_PWD_MISSING_IN_DB", "The email you specified does not exists on our database", this);
					Message.TextKey = "SIGNIN_PWD_MISSING_IN_DB";
				}
			}
			finally
			{
				dr.Close(); //by Manu, fixed bug 807858
			}
		}

		public SigninLdap()
		{
			SettingItem HideAutomatically = new SettingItem(new BooleanDataType());
			HideAutomatically.Value = "True";
			HideAutomatically.EnglishName = "Hide automatically";
			this._baseSettings.Add("SIGNIN_AUTOMATICALLYHIDE", HideAutomatically);

			// 18/08/2004 Jonathan
			SettingItem LDAPCheck = new SettingItem(new BooleanDataType());
			LDAPCheck.Value = "False";
			LDAPCheck.EnglishName = "LDAP";
			this._baseSettings.Add("LDAP", LDAPCheck);


			//1.2.8.1743b - 09/10/2003
			//New setting on Signin fo disable IE autocomplete by Mike Stone
			//If you uncheck this setting IE will not remember user name and passwords. 
			//Note that users who have memorized passwords will not be effected until their computer 
			//is reset, only new users and/or computers will honor this. 
			SettingItem AutoComplete = new SettingItem(new BooleanDataType());
			AutoComplete.Value = "True";
			AutoComplete.EnglishName = "Allow IE Autocomplete";
			AutoComplete.Description = "If Checked IE Will try to remember logins";
			this._baseSettings.Add("SIGNIN_ALLOW_AUTOCOMPLETE", AutoComplete); 
		}

		/// <summary>
		/// Overrides ModuleSetting to render this module type un-cacheable
		/// </summary>
		public override bool Cacheable
		{
			get
			{
				return false;
			}
		}

		#region General Implementation
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{64EE6673-434A-47fe-A54F-43AD9B2F2C05}");
			}
		}
		#endregion

		#region Web Form Designer generated code
		/// <summary>
		/// On init
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			// use "View = Unauthenticated Users" instead
			//			//Hide control if not needed
			//			if (Request.IsAuthenticated)
			//				this.Visible = false;

			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			this.LoginBtn.Click += new EventHandler(this.LoginBtn_Click);
			this.RegisterBtn.Click += new EventHandler(this.RegisterBtn_Click);
			this.SendPasswordBtn.Click += new EventHandler(this.SendPasswordBtn_Click);
			this.Load += new EventHandler(this.Signin_Load);

		}
		#endregion

		private void Signin_Load(object sender, EventArgs e)
		{
			if(!bool.Parse(portalSettings.CustomSettings["SITESETTINGS_ALLOW_NEW_REGISTRATION"].ToString()))
				RegisterBtn.Visible = false;

	
			if(bool.Parse(Settings["SIGNIN_AUTOMATICALLYHIDE"].ToString()) && Request.IsAuthenticated)
			{
				this.Visible = false;
			}
			else if(!bool.Parse(Settings["SIGNIN_ALLOW_AUTOCOMPLETE"].ToString()))
			{
				//New setting on Signin fo disable IE autocomplete by Mike Stone
				password.Attributes.Add("autocomplete", "off");
			}

			if (!IsPostBack)
			{
				GroupList.Items.Add(new ListItem("Default", "Default"));

				//jes1111 - string contexts = ConfigurationSettings.AppSettings["LDAPContexts"];
				string contexts = Config.LDAPContexts;
				//jes111 - if (contexts != null)
				if (contexts.Length != 0)
				{
					string[] names = contexts.Split(";".ToCharArray());
					for (int i=0; i < names.Length; i++)
					{
						GroupList.Items.Add(new ListItem(LDAPHelper.GetContext(names[i]), names[i]));
					}
				}

				LDAPCheckBox.Checked = "True" == Settings["LDAP"].ToString();
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			TRGroup.Visible = LDAPCheckBox.Checked;
			TRGroupList.Visible = LDAPCheckBox.Checked;
			SendPasswordBtn.Visible = !LDAPCheckBox.Checked;
			EmailLabel.TextKey = LDAPCheckBox.Checked ? "USERNAME" : "EMAIL";

			base.OnPreRender (e);
		}
	}
}
