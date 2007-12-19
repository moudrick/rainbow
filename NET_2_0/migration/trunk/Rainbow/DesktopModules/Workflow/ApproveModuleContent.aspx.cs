using System;
using System.Web.Mail;
using Esperantus;
using Esperantus.WebControls;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.UI;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// Summary description for ApproveModuleContent.
	/// </summary>
	[History("Jes1111", "2003/03/04", "Added OnUpdate call to base page to handle cache flushing")]
	[History("Geert.Audenaert@Syntegra.Com", "2003/03/10", "Commented call from Jes, because it caused an error, and it wasn't necessary too")]
	[History("Geert.Audenaert@Syntegra.Com", "2003/03/11", "Added default destinators and text in the email form")]
	public class ApproveModuleContent : Page 
	{
		protected LinkButton btnApprove;
		protected LinkButton btnApproveAndSendMail;
		protected EmailForm emailForm;
	
		private void Page_Load(object sender, EventArgs e)
		{
			// Check if the user is authorized
			if ( ! (PortalSecurity.HasApprovePermissions(ModuleID)) )
				PortalSecurity.AccessDeniedEdit();
	
			// Fill email form with default 
			if ( ! IsPostBack )
			{
				// Destinators
				ModuleSettings ms = null;
				for (int i=0; i < portalSettings.ActivePage.Modules.Count; i ++)
				{
					ms = (ModuleSettings)portalSettings.ActivePage.Modules[i];
					if ( ms.ModuleID == ModuleID )
						break;
				}
				string[] emails = MailHelper.GetEmailAddressesInRoles(ms.AuthorizedPublishingRoles.Split(";".ToCharArray()), portalSettings.PortalID);
				for ( int i=0; i < emails.Length; i++)
					emailForm.To.Add(emails[i]);
				// Subject
				emailForm.Subject = Localize.GetString ("SWI_REQUEST_PUBLISH_SUBJECT", "Request publishing for the new content of '") + ms.ModuleTitle + "'";
				// Message
				emailForm.HtmlBodyText = Localize.GetString ("SWI_REQUEST_BODY", "You can find the new content at:") + "<br><br><a href='" + UrlReferrer + "'>" + UrlReferrer + "</a>";
			}
		}

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
			this.btnApproveAndSendMail.Click += new EventHandler(this.btnApproveAndSendMail_Click);
			this.btnApprove.Click += new EventHandler(this.btnApprove_Click);
			this.cancelButton.Click += new EventHandler (this.cancelButton_Click); 
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.RedirectBackToReferringPage();
		}

		private void btnApprove_Click(object sender, EventArgs e)
		{
			Approve(e);
		}

		private void btnApproveAndSendMail_Click(object sender, EventArgs e)
		{
			if ( emailForm.AllEmailAddressesOk )
			{
				// Send mail
				MailMessage mm = new MailMessage();
				// jes1111 - mm.From = MailHelper.GetCurrentUserEmailAddress(ConfigurationSettings.AppSettings["EmailFrom"]);
				mm.From = MailHelper.GetCurrentUserEmailAddress(Config.EmailFrom);
				mm.To = string.Join(";",(string[])emailForm.To.ToArray(typeof(string)));
				mm.Cc = string.Join(";",(string[])emailForm.Cc.ToArray(typeof(string)));
				mm.Bcc = string.Join(";",(string[])emailForm.Bcc.ToArray(typeof(string)));
				mm.BodyFormat = MailFormat.Html;
				mm.Body = emailForm.BodyText;
				mm.Subject = emailForm.Subject;

				//jes1111 - SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["SmtpServer"];
				SmtpMail.SmtpServer = Config.SmtpServer;
				SmtpMail.Send(mm);

				// Request approval
				Approve(e);
			}			
		}

		private void Approve(EventArgs e)
		{
			// Geert.Audenaert
			// 10/03/2003
			// This is not necessary
			//base.OnUpdate(e);

			WorkFlowDB.Approve(ModuleID);
			this.RedirectBackToReferringPage();
		}
	}
}
