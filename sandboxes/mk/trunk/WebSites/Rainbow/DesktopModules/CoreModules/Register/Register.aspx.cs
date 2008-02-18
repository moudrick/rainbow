using System;
using System.Web;
using System.Web.UI;
using Rainbow.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Security;
using Rainbow.Framework.Web.UI.WebControls;
using History=Rainbow.Framework.History;
using Page=Rainbow.Framework.Web.UI.Page;

namespace Rainbow.Admin
{
    /// <summary>
    /// Summary description for Register.
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    [History("gman3001", "2004/10/06",
            "Modified GetCurrentProfileControl method to properly obtain a custom register control as specified by the 'Register Module ID' setting.")]
    [History("John.Mandia@whitelightsolutions.com", "2003/10/31",
            "Fixed Bug 799945 in sourceforge. After allow no new registrations is ticked users cannot edit their profile")]
    [History("Manu", "2003/04/04", "Only one register page can load multiple modules")]
    [History("Jes1111", "2003/03/10", "Modified from original page to use Register module")]
    public partial class Register : Page
    {
        protected IEditUserProfile EditControl;

        /// <summary>
        /// Gets a value indicating whether [edit mode].
        /// </summary>
        /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
        public bool EditMode
        {
            get { return (userName.Length != 0); }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        private string userName
        {
            get
            {
                string _userName = string.Empty;
                if (Request.Params["userName"] != null)
                    _userName = Request.Params["userName"];
                return _userName;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            if (!EditMode &&
                !bool.Parse(portalSettings.CustomSettings["SITESETTINGS_ALLOW_NEW_REGISTRATION"].ToString()))
                PortalSecurity.AccessDeniedEdit();

            Control myControl = GetCurrentProfileControl();

            EditControl = ((IEditUserProfile) myControl);
            EditControl.RedirectPage = HttpUrlBuilder.BuildUrl(PageID);

            register.Controls.Add(myControl);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion

        /// <summary>
        /// Gets the current profile control.
        /// </summary>
        /// <returns></returns>
        public static Control GetCurrentProfileControl() 
        {
            //default
            string RegisterPage = "Register.ascx";

            // 19/08/2004 Jonathan Fong 
            // www.gt.com.au
            RainbowPrincipal user = HttpContext.Current.User as RainbowPrincipal;
            Portal portalSettings = PortalProvider.Instance.CurrentPortal;

            //Select the actual register page
            if (portalSettings.CustomSettings["SITESETTINGS_REGISTER_TYPE"] != null &&
                portalSettings.CustomSettings["SITESETTINGS_REGISTER_TYPE"].ToString() != "Register.ascx" ) {
                RegisterPage = portalSettings.CustomSettings["SITESETTINGS_REGISTER_TYPE"].ToString();
            }
            System.Web.UI.Page page = new System.Web.UI.Page();

            // Modified by gman3001 10/06/2004, to support proper loading of a register module specified by 'Register Module ID' setting in the Portal Settings admin page
            int moduleID = int.Parse( portalSettings.CustomSettings["SITESETTINGS_REGISTER_MODULEID"].ToString() );
            string moduleDesktopSrc = string.Empty;
            if (moduleID > 0)
            {
                moduleDesktopSrc = RainbowModuleProvider.Instance.GetModuleDesktopSrc(moduleID);
            }
            if (moduleDesktopSrc.Length == 0)
            {
                moduleDesktopSrc = RegisterPage;
            }
            //TODO: [moudrick] this line breaks HttpSimulator tests
            Control control = page.LoadControl(moduleDesktopSrc);
            // End Modification by gman3001

            PortalModuleControl portalModuleControl = ((PortalModuleControl) control);
            //p.ModuleID = int.Parse(portalSettings.CustomSettings["SITESETTINGS_REGISTER_MODULEID"].ToString());
            portalModuleControl.ModuleID = moduleID;
            if (portalModuleControl.ModuleID == 0)
            {
                ((SettingItem) portalModuleControl.Settings["MODULESETTINGS_SHOW_TITLE"]).Value = "false";
            }
            return portalModuleControl;
        }
    }
}
