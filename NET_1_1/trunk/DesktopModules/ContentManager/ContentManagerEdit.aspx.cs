using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rainbow.UI;
using Label = Esperantus.WebControls.Label;
using RequiredFieldValidator = Esperantus.WebControls.RequiredFieldValidator;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// Add/Remove modules, assign modules to portals
	/// </summary>
	public class ContentManagerEdit : Page
	{
		protected Label Label1;
		protected Label Label2;
		protected TextBox InstallerFileName;
		protected System.Web.UI.WebControls.Label lblErrorDetail;
		protected HtmlTable tableInstaller;
		protected RequiredFieldValidator Requiredfieldvalidator1;

		/// <summary>
		/// The Page_Load server event handler on this page is used
		/// to populate the role information for the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{

			// Verify that the current user has access to access this page
			// Removed by Mario Endara <mario@softworks.com.uy> (2004/11/13)
			// if (PortalSecurity.IsInRoles("Admins") == false)
			//	PortalSecurity.AccessDeniedEdit();

			// If this is the first visit to the page, bind the definition data
			if (Page.IsPostBack == false)
			{
			     InstallerFileName.Text = "DesktopModules/ContentManager/InstallFiles/";
			}
		}
        /*
		/// <summary>
		/// OnUpdate installs or refresh module definiton on db
		/// </summary>
		/// <param name="e"></param>
		protected override void OnUpdate(EventArgs e)
		{
			if (Page.IsValid)
			{
				try
				{
				    Install Module Here.
					// Redirect back to the portal admin page
					RedirectBackToReferringPage();
				}
				catch(Exception ex)
				{
					lblInvalidModule.Visible = true;
					lblErrorDetail.Text = string.Empty;
					while (ex != null)
					{
						lblErrorDetail.Text += ex.Message + "<br />";
						Rainbow.Helpers.LogHelper.Log.Error("Installing: ", ex);
						ex = ex.InnerException;
					}
				}
			}
		}


		/// <summary>
		/// Delete a Module definition
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDelete(EventArgs e)
		{
			try
			{
				if (!btnUseInstaller.Visible)
					Rainbow.Helpers.ModuleInstall.UninstallGroup(Server.MapPath(PortalSettings.ApplicationPath + "/" + InstallerFileName.Text));
				else
					Rainbow.Helpers.ModuleInstall.Uninstall(DesktopSrc.Text, MobileSrc.Text);

				// Redirect back to the portal admin page
				RedirectBackToReferringPage();
			}
			catch(Exception ex)
			{
				Rainbow.Helpers.LogHelper.Log.Error("Error deleting module", ex);

				lblInvalidModule.Visible = true;
				lblErrorDetail.Text = string.Empty;
				while (ex != null)
				{
					lblErrorDetail.Text += ex.Message + "<br />" ;
					ex = ex.InnerException;
				}
			}
		}
        */
		#region Web Form Designer generated code
		/// <summary>
		/// OnInit
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
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
