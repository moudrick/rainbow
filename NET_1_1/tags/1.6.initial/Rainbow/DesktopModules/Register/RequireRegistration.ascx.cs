using System;
using System.Web.UI;
using Esperantus.WebControls;

namespace Rainbow.Admin
{
    public class RequireRegistration : UserControl
    {
		protected HyperLink RegisterHyperlink;
		protected Label LabelRegister;
		protected Label LabelAlreadyAccount;
		protected Label LabelRegisterNow;
		protected HyperLink SignInHyperLink;

		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            
            SignInHyperLink.NavigateUrl = HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/Logon.aspx");
            RegisterHyperlink.NavigateUrl = HttpUrlBuilder.BuildUrl("~/DesktopModules/Register/Register.aspx");
		
            base.OnInit(e);
        }

        private void InitializeComponent() 
        {

		}
		#endregion

    }
}