using System;
using System.Collections;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Security;
using Rainbow.UI.WebControls;
using Label = Esperantus.WebControls.Label;

namespace Rainbow.DesktopModules
{
    /// <summary>
    /// Placeable Registration (Full) module
    /// </summary>

    public class LDAPUserProfile : PortalModuleControl, IEditUserProfile
    {
		protected TextBox NameField;
		protected Label NameLabel;
		protected TextBox UseridField;
		protected Label UseridLabel;
		protected Label PageTitleLabel;
		protected HtmlTableRow UserIDRow;
		protected Label EmailLabel;
		protected Label MembershipLabel;
		protected ListBox MembershipListBox;
		protected TextBox EmailField;
		protected TextBox DepartmentField;
		protected Label DepartmentLabel;
		protected Label ErrorMessage;

		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();


			// Add title and configuration
//			ModuleTitle = new DesktopModuleTitle();
			//ModuleConfiguration = new ModuleSettings();
			//ModuleConfiguration.ModuleTitle = Esperantus.Localize.GetString("REGISTER");

//			Controls.AddAt(0, ModuleTitle);

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

        private void Page_Load(object sender, EventArgs e)
        {
			try
			{
				RainbowPrincipal user = HttpContext.Current.User as RainbowPrincipal;
				if (user != null)
				{
					Hashtable userProfile = LDAPHelper.GetUserProfile(user.Identity.ID);
					this.UseridField.Text = ((string[]) userProfile["DN"])[0];
					this.NameField.Text = ((string[]) userProfile["FULLNAME"])[0];
					this.EmailField.Text = ((string[]) userProfile["MAIL"])[0];
					this.DepartmentField.Text = ((string[]) userProfile["OU"])[0];
					this.MembershipListBox.DataSource = userProfile["GROUPMEMBERSHIP"];
					this.MembershipListBox.DataBind();
				}
			}
			catch(Exception ex)
			{
				ErrorMessage.Visible = true;
				ErrorHandler.Publish(LogLevel.Error, "Error retrieving user", ex);
			}
		}

		public override Guid GuidID 
		{
			get
			{
				return new Guid("{224D9473-3AD4-4850-9F07-9055957D7BE7}");
			}
		}

		#region IEditUserProfile Members

		public bool EditMode
		{
			get
			{
				// TODO:  Add LDAPUserProfile.EditMode getter implementation
				return false;
			}
		}

		public string RedirectPage
		{
			get
			{
				// TODO:  Add LDAPUserProfile.RedirectPage getter implementation
				return null;
			}
			set
			{
				// TODO:  Add LDAPUserProfile.RedirectPage setter implementation
			}
		}

		public int SaveUserData()
		{
			// TODO:  Add LDAPUserProfile.SaveUserData implementation
			return 0;
		}

		#endregion
	}
}