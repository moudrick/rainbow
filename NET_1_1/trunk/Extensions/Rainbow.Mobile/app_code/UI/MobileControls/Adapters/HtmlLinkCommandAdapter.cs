/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 2004/11/29
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.MobileControls.Adapters;

using Esperantus;


namespace Rainbow.UI.MobileControls.Adapters
{
	/// <summary>
	///  The HtmlLinkCommandAdapter class is used to render the LinkCommand
	/// control on an HTML device. Unlike the Command control, which renders
	/// as a button, the HtmlLinkCommandAdapter renders a LinkCommand as
	/// a hyperlink. Only the Render method needs to be overriden.
	/// </summary>
	public class HtmlLinkCommandAdapter : HtmlCommandAdapter 
	{
		/// <summary>
		/// Returns the attached control, strongly typed as a TabbedPanel.
		/// </summary>
		protected new LinkCommand Control 
		{
			get 
			{
				return (LinkCommand) base.Control;
			}
		}


		/// <summary>
		/// The Render method performs rendering of the LinkCommand control.
		/// </summary>
		/// <param name="writer"></param>
		public override void Render(HtmlMobileTextWriter writer) 
		{
			// Render a postback event as an anchor.
			//RenderPostBackEventAsAnchor(writer, null, Control.Text);

			string fontFace = Control.Style.Font.Name ;
			string fontSize = Control.Style.Font.Size.ToString() ;
			string fontColor = GetColorString(Control.Style.ForeColor, "");

			//
			//
			//
			//writer.Write("&#160;");
			writer.WriteBeginTag("a");
			//writer.WriteAttribute("href",HttpUrlBuilder.BuildUrl(child.TabId));
			RenderPostBackEventAsAttribute(writer, "href",Control.CommandArgument);
			writer.Write(">");

			writer.WriteBeginTag("font");
            if (fontFace!="") writer.WriteAttribute("face", fontFace);
			if (fontSize!="") writer.WriteAttribute("size", fontSize);
			if (fontColor!="")writer.WriteAttribute("color", fontColor);
			writer.Write(">");
			writer.WriteText(Control.Text, true);

			writer.WriteEndTag("font");
			writer.WriteEndTag("a");
			//

			// Write a break, if necessary.
			writer.WriteBreak();
		}
    
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="defaultColor"></param>
		/// <returns></returns>
		private static String GetColorString(Color color, String defaultColor) 
		{
			return color != Color.Empty ? ColorTranslator.ToHtml(color) : defaultColor;
		}

	}

}

