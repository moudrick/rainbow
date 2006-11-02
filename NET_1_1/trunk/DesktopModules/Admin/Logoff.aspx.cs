using System;
using Rainbow.Security;
using Rainbow.UI;

namespace Rainbow.Admin
{
    /// <summary>
	/// The Logoff page is responsible for signing out a user 
	/// from the cookie authentication, and then redirecting 
	/// the user back to the portal home page.
	/// This page is executed when the user	clicks 
	/// the Logoff button at the top of the page.
    /// </summary>
    public class Logoff : Page
    {
        private void Page_Load(object sender, EventArgs e)
        {
			// Signout
        	PortalSecurity.SignOut();
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
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion
    }
}
