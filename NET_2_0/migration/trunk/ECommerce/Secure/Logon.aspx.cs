using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.Mail;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rainbow.Security;
using Rainbow.Configuration;
using Rainbow.Admin;
using Esperantus;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Single click logon, useful for email and newsletters
	/// </summary>
	public class LogonPage : Rainbow.ECommerce.UI.SecurePage
	{
		protected System.Web.UI.WebControls.TextBox email;
		protected System.Web.UI.WebControls.TextBox password;
		protected Esperantus.WebControls.Button LoginBtn;
		protected Esperantus.WebControls.LinkButton RegisterBtn;
		protected Esperantus.WebControls.LinkButton CancelBtn;
		protected Esperantus.WebControls.LinkButton SendPasswordBtn;
		protected System.Web.UI.WebControls.Label Message;
		protected System.Web.UI.HtmlControls.HtmlForm Logon;
		protected Esperantus.WebControls.Literal Literal1;
		protected Esperantus.WebControls.Literal Literal2;
		protected Esperantus.WebControls.CheckBox Checkbox1;
		protected Esperantus.WebControls.Label Label1;
		protected Esperantus.WebControls.Label Label2;
		protected System.Web.UI.WebControls.PlaceHolder signIn;
		protected Esperantus.WebControls.Label Label3;
		protected string Titulo;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Added localization 28/7/04 Mario Endara mario@softworks.com.uy
			Titulo = Localize.GetString ("ECOMMERCE_SECURE_TITLE", "Rainbow Secure Server");
			// simpler version than the one in Admin as we do not want people
			//if user has logged on
			if(Request.IsAuthenticated)
			{
				// Redirect user back to the Portal Home Page
				PortalSecurity.PortalHome();
			}
			else
			{
			}
		}

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
			this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
			this.SendPasswordBtn.Click += new System.EventHandler(this.SendPasswordBtn_Click);
			this.RegisterBtn.Click += new System.EventHandler(this.RegisterBtn_Click);
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void LoginBtn_Click(object sender, System.EventArgs e)
		{
			if (PortalSecurity.SignOn(email.Text, password.Text, Checkbox1.Checked, "ok.aspx") == null)
			{
				// Added localization 28/7/04 Mario Endara mario@softworks.com.uy
				Message.Text = Localize.GetString ("ECOMMERCE_LOGIN_FAILED", "Login failed");
			}
		}

		private void RegisterBtn_Click(object sender, System.EventArgs e)
		{
			// stay in the secure area
			Response.Redirect("Register.aspx");
		}

		private void CancelBtn_Click(object sender, System.EventArgs e)
		{
			// stay in the secure area, confirm leaving secure site
			Response.Redirect("ko.aspx");
		}

		private void SendPasswordBtn_Click(object sender, System.EventArgs e)
		{
			if (email.Text == "")
			{
				// Added localization 28/7/04 Mario Endara mario@softworks.com.uy
				Message.Text = Localize.GetString ("ECOMMERCE_LOGIN_ENTEREMAIL", "Please enter you email address");
				return;
			}
			
			Security.UsersDB usersDB = new Security.UsersDB();

			//Obtain single row of User information
			SqlDataReader dr = usersDB.GetSingleUser(email.Text, portalSettings.PortalID);
			try
			{
				if (dr.Read())
				{  
					String AppName = portalSettings.PortalName;
					String Pswrd = (String) dr["password"];
					String Name = (String) dr["Name"];
					MailMessage mail = new System.Web.Mail.MailMessage();
				
					// Added localization 28/7/04 Mario Endara mario@softworks.com.uy
					mail.From = System.Configuration.ConfigurationSettings.AppSettings["EmailFrom"].ToString();
					mail.To = email.Text;
					mail.Subject = Localize.GetString ("ECOMMERCE_LOGIN_EMAILSUBJECTOK", "Your password on portal.net");

					System.Text.StringBuilder sb = new System.Text.StringBuilder();

					sb.Append(Name);
					sb.Append(",");
					sb.Append("\n\n");
					sb.Append(Localize.GetString("SIGNIN_PWD_REQUESTED", "This is the password you requested", this));
					sb.Append(":");
					sb.Append(" ");
					sb.Append(Pswrd);
					sb.Append("\n\n");

					mail.Body = sb.ToString();

					System.Web.Mail.SmtpMail.SmtpServer = Rainbow.Settings.Config.SmtpServer;
					System.Web.Mail.SmtpMail.Send(mail);

					Message.Text = Localize.GetString("SIGNIN_PWD_WAS_SEND", "Your password was sent to the addess you provided", this);
				}
				else 
				{
					Message.Text = Localize.GetString("SIGNIN_PWD_MISSING_IN_DB", "The email you specified does not exists on our database", this);
				}
			}
			finally
			{
				dr.Close(); //by Manu, fixed bug 807858
			}
		}
	}
}
