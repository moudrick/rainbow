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
	[History("mario@hartmann.net", "05/06/2004", "")]
	public class MobileTitleDesigner : System.Web.UI.Design.ControlDesigner
	{
		private MobileTitle  thisControl 
		{
			get
			{
				return (MobileTitle) this.Component;
			}
		}

		/// <summary>
		/// Component is the instance of the component or control that
		/// this designer object is associated with. This property is 
		/// inherited from System.ComponentModel.ComponentDesigner.
		/// TabbedPanelDesigner Text = (TabbedPanelDesigner) Component;
		/// </summary>
		/// <returns></returns>
		public override string GetDesignTimeHtml() 
		{
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				//sb.Append ("<table cellspacing=0 cellpadding=0 width='100%' border=0>" );
				//sb.Append ("<tr bgcolor='#808080'><td>");
				sb.Append("<font color='#808080'>");
				sb.Append ("[Rainbow Mobile ModuleTitle]");
				sb.Append("</font>");
				//sb.Append ("</td></tr>") ; 
				//sb.Append ("</table>" ); 
				if (thisControl.BreakAfter )
					sb.Append ("<br>");

				return sb.ToString();
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}	
	}
}