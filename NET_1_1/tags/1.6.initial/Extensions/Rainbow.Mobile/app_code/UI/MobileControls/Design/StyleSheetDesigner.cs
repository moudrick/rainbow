/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario@hartmann.net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 2004/11/27
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.ComponentModel;
using System.Drawing;
using Rainbow.UI.MobileControls;

namespace Rainbow.UI.MobileControls.Design
{
	/// <summary>
	/// Used to give to DesignModuleTitle needed design time support
	/// </summary>
	public class StyleSheetDesigner : System.Web.UI.Design.ControlDesigner
	{

		private StyleSheet  thisControl 
		{
			get
			{
				return (StyleSheet) this.Component;
			}
		}

		/// <summary>
		/// GetDesignTimeHtml
		/// </summary>
		/// <returns></returns>
		public override string GetDesignTimeHtml() 
		{
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append ("<table cellspacing=0 cellpadding=0 width='300' border=1>" );
				sb.Append ("<tr bgcolor='#808080'><td>");
				sb.Append("<font color='#ffffff'>");
				sb.Append ("[Rainbow Mobile StyleSheet]");
				sb.Append("</font>");
				sb.Append ("</td></tr>") ; 
				sb.Append ("</table>" ); 
				return sb.ToString();
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}	
	}
}