/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * This code is partially based on the IbuySpy Mobile Portal Code. 
 * Last updated Date: 2004/11/29
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using Rainbow.UI;
using Rainbow.UI.MobileControls;

using Rainbow.DesktopModules;
using Rainbow.Configuration;

namespace Rainbow.Modules.Text
{
	/// <summary>
	///	The Text Mobile User Control renders text modules in the mobile portal. 
	/// The control consists of two pieces: a summary panel that is rendered when
	/// portal view shows a summarized view of all modules, and a multi-part panel 
	/// that renders the module details.
	/// </summary>
	public class TextMobile : Rainbow.UI.MobileControls.MobilePortalModuleControl
	{
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand2;
		protected System.Web.UI.MobileControls.TextView TextView1;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand1;

		protected string mobileSummary = "";
		protected Rainbow.UI.MobileControls.MobileTitle MobileTitle2;
		protected System.Web.UI.MobileControls.Panel summary;
		protected System.Web.UI.MobileControls.DeviceSpecific DeviceSpecific1;
		protected string mobileDetails = "";
   
		/// <summary>
		/// Page_Load Event Handler
		///
		/// The Page_Load event handler on this User Control is used to
		/// load the contents of the text message from a file, and databind
		/// the message to the module contents.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Page_Load(Object sender, EventArgs e) 
		{
			// Obtain the selected item from the HtmlText table
			TextMobileDB text = new TextMobileDB();

			IDataReader dr = text.GetHtmlText(ModuleId);
        
			if (dr.Read()) 
			{
				// Dynamically add the file content into the page
				mobileSummary = Server.HtmlDecode((String) dr["MobileSummary"]);
				mobileDetails = Server.HtmlDecode((String) dr["MobileDetails"]);
			}
        
			DataBind();
       
			// Close the datareader
			dr.Close();       
		}

		#region Web Form Designer generated code
		/// <summary>
		/// OnInit
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
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
