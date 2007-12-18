using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

namespace Rainbow.Mobile 
{
	/// <summary>
	/// The Default.aspx page simply tests 
	/// the browser type and redirects either to
	/// the DesktopDefault or Mobile pages, 
	/// depending on the device type.
	/// </summary>
    public class Default : System.Web.UI.Page 
    {
        private void Page_Load(object sender, System.EventArgs e) 
        {
            if (Request.Browser["IsMobileDevice"] == "true") 
            {
				string _targetM = ConfigurationSettings.AppSettings["MobileTargetUrl"];
				//Server.Transfer(_redirURL, false);
				Response.Redirect(_targetM);
            }
			else if (Request.Browser["IsPocketIE"] == "true" ) 
			{
				string _targetPIE = ConfigurationSettings.AppSettings["PocketIETargetUrl"];
				//Server.Transfer(_redirURL, false);
				Response.Redirect(_targetPIE);
			}
			else 
            {
				Server.Transfer("DesktopDefault.aspx", true);
				//Response.Redirect("DesktopDefault.aspx");
            }
        }

		#region Web Form Designer generated code
		/// <summary>
		/// Raises OnInitEvent
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
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
            this.Load += new System.EventHandler(this.Page_Load);
        }
		#endregion
    }
}