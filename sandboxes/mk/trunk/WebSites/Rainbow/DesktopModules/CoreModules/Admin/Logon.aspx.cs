using System;
using Rainbow.Framework;
using Rainbow.Framework.Security;
using Rainbow.Framework.Web.UI;

namespace Rainbow.Admin
{
    /// <summary>
    /// Single click logon, useful for email and newsletters
    /// </summary>
    public partial class LogonPage : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string password = string.Empty;
            string alias = string.Empty;

            // Get Login User from querystring
            if (Request.Params["usr"] != null)
            {
                string user = Request.Params["usr"];
                // Get Login Password from querystring
                if (Request.Params["pwd"] != null)
                {
                    password = Request.Params["pwd"];
                }
                // Get portalaias
                if (Request.Params["alias"] != null)
                {
                    alias = HttpUrlBuilder.BuildUrl("~/Default.aspx", 0, string.Empty, Request.Params["alias"]);
                }
                //try to validate logon
                if (SignOnController.SignOn(user, password, false, alias) == null)
                {
                    // Login failed
                    PortalSecurity.AccessDenied();
                }
            }
            else
            {
                //if user has logged on
                if (Request.IsAuthenticated)
                {
                    // Redirect user back to the Portal Home Page
                    PortalSecurity.PortalHome();
                }
                else
                {
                    //User not provided, display logon
                    signIn.Controls.Add(LoadControl("~/DesktopModules/CoreModules/SignIn/Signin.ascx"));
                }
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }
        #endregion
    }
}