using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rainbow.Security;
using Esperantus;

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Summary description for ko.
	/// </summary>
	public class ko : Rainbow.ECommerce.UI.SecurePage
	{
		protected Esperantus.WebControls.LinkButton ContinueBtn;
		protected Esperantus.WebControls.Label Label1;
		protected Esperantus.WebControls.Label Label2;
		protected Esperantus.WebControls.Label Label3;
		protected string Titulo;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			// Added localization 28/7/04 Mario Endara mario@softworks.com.uy
			Titulo = Localize.GetString ("ECOMMERCE_SECURE_TITLE", "Rainbow Secure Server");
		}

		/// <summary>
		/// Set the module guids with free access to this page
		/// </summary>
		protected override ArrayList AllowedModules
		{
			get
			{
				ArrayList al = new ArrayList();
				al.Add ("EC24FABD-FB16-4978-8C81-1ADD39792377");
				return al;
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
			this.ContinueBtn.Click += new System.EventHandler(this.ContinueBtn_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void ContinueBtn_Click(object sender, System.EventArgs e)
		{
			// Redirect user back to the Portal Home Page
			PortalSecurity.PortalHome();
		}
	}
}
