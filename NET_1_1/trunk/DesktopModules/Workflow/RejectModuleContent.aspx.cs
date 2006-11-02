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
	/// Summary description for RejectModuleContent.
	/// </summary>
	public class RejectModuleContent : Page 
	{
		/// <summary>
		/// 
		/// </summary>
		protected LinkButton btnReject;
		/// <summary>
		/// 
		/// </summary>
		protected LinkButton btnRejectAndSendMail;
		/// <summary>
		/// 
		/// </summary>
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
				string tmp = ms.AuthorizedAddRoles.Trim();
				tmp += ms.AuthorizedEditRoles.Trim();
				tmp += ms.AuthorizedDeleteRoles.Trim();
				string[] emails = MailHelper.GetEmailAddressesInRoles(tmp.Split(";".ToCharArray()), portalSettings.PortalID);
				for ( int i=0; i < emails.Length; i++)
					emailForm.To.Add(emails[i]);
				// Subject
				emailForm.Subject = Localize.GetString ("SWI_REJECT_SUBJECT1", "The new content of ") + "'" + ms.ModuleTitle + "'" + Localize.GetString ("SWI_REJECT_SUBJECT2", " has been rejected");
				// Message
				emailForm.HtmlBodyText = Localize.GetString ("SWI_REJECT_BODY", "You can find the rejected content at:") + "<br><br><a href='" + UrlReferrer + "'>" + UrlReferrer + "</a>";
			}

		}

		#region Web Form Designer generated code
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
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
			this.btnRejectAndSendMail.Click += new EventHandler(this.btnRejectAndSendMail_Click);
			this.btnReject.Click += new EventHandler(this.btnReject_Click);
			this.cancelButton.Click += new EventHandler (this.cancelButton_Click); 
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.RedirectBackToReferringPage();
		}

		private void btnReject_Click(object sender, EventArgs e)
		{
			Reject();
		}

		private void btnRejectAndSendMail_Click(object sender, EventArgs e)
		{
			if ( emailForm.AllEmailAddressesOk )
			{
				// Send mail
				MailMessage mm = new MailMessage();
				//jes1111 - mm.From = MailHelper.GetCurrentUserEmailAddress(ConfigurationSettings.AppSettings["EmailFrom"]);
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
				Reject();
			}			
		}

		private void Reject()
		{
			WorkFlowDB.Reject(ModuleID);
			this.RedirectBackToReferringPage();
		}

	}
}
