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

namespace Rainbow.ECommerce
{
	/// <summary>
	/// Summary description for Register.
	/// </summary>
	public class Register : Rainbow.ECommerce.UI.SecurePage
	{
		protected System.Web.UI.WebControls.PlaceHolder registerPlaceHolder;
	
	
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
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			IEditUserProfile EditControl;
			string RegisterPage;

//			//Select the actual register page
//			if (portalSettings.CustomSettings["Register"] != null)
//				RegisterPage = portalSettings.CustomSettings["Register"].ToString();
//			else
//				RegisterPage = "Register";
//
//			Control myControl = this.LoadControl("../DesktopModules/" + RegisterPage + ".ascx");

			//Select the actual register page
			if (portalSettings.CustomSettings["SITESETTINGS_REGISTER_TYPE"] != null &&
				portalSettings.CustomSettings["SITESETTINGS_REGISTER_TYPE"].ToString() != "Register.ascx" )
				RegisterPage = portalSettings.CustomSettings["SITESETTINGS_REGISTER_TYPE"].ToString();
			else
				RegisterPage = "Register.ascx";

			Control myControl = this.LoadControl(Rainbow.Settings.Path.ApplicationRoot + "/DesktopModules/Register/" + RegisterPage);
			EditControl = ((IEditUserProfile) myControl);
//
//			if (Request.UrlReferrer != null)
//				EditControl.RedirectPage = Request.UrlReferrer.ToString();
//			else
			EditControl.RedirectPage = Rainbow.Settings.Path.WebPathCombine(portalSettings.PortalSecurePath, "ProductsCheckOut.aspx?mID=" + Request.QueryString["mID"]);

			registerPlaceHolder.Controls.Add(myControl);
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
