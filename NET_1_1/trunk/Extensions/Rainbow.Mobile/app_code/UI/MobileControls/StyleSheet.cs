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
using System.Web;
using System.Web.UI;
using System.ComponentModel;

using Rainbow;
using Rainbow.Configuration ;


namespace Rainbow.UI.MobileControls
{
	/// <summary>
	/// Summary description for MobileStyleSheet.
	/// </summary>
	[Description("The Mobile Rainbow StyleSheet Class."),
	ToolboxData("<{0}:StyleSheet runat=\"server\" />"),
	Designer("Rainbow.UI.MobileControls.Design.StyleSheetDesigner")	]
	public class StyleSheet : System.Web.UI.MobileControls.StyleSheet
	{
		/// <summary>
		/// 
		/// </summary>
		public StyleSheet()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
		
			string  path ="";

			if (Context !=null) 
				path =portalSettings.GetCurrentTheme().WebPath + "/MobileStyle.ascx" ;	
	
			if (path==null || !System.IO.File.Exists(Context.Server.MapPath(path)))
				path = "~/Design/Theme/Default/MobileStyle.aspx";

			base.ReferencePath =  path;
			
			base.OnInit(e);
		}

		[System.ComponentModel.DefaultValue("~/Design/Theme/Default/MobileStyle.aspx"),
		System.ComponentModel.Browsable(false)]
		public new string  ReferencePath
		{
			get
			{
                    return base.ReferencePath ;
			}
		}
			/// <summary>
			/// Stores current portal settings 
			/// </summary>
			public PortalSettings portalSettings
		{
			get
			{
				// Obtain PortalSettings from Current Context
				if (HttpContext.Current != null)
					return (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				else
					return null;
			}
		}

	}
}
