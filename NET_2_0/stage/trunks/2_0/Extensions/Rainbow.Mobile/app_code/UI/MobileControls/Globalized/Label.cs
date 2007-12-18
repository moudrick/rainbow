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


namespace Rainbow.UI.MobileControls.Globalized
{
	/// <summary>
	/// Localized label
	/// </summary>
	[ToolboxData("<{0}:Label TextKey='' runat=server></{0}:Label>")]
	public class Label :System.Web.UI.MobileControls.Label , IGlobalizedMobilControls
	{
		public string TextKey
		{
			get
			{
				object txt = ViewState["TextKey"];
				if (txt != null)
					return (String) txt;
				return String.Empty;
			}
			set
			{
				this.ViewState["TextKey"] = value;
			}
		}

		private Style _style;
		protected internal virtual new Style Style
		{
			get
			{
				
				try
				{
					if (_style==null)
						_style = ((MobilePage)base.Page).MobileStyle(StyleReference);
				}
				catch
				{

				}
				if (_style==null)
					_style =new System.Web.UI.MobileControls.Style();
				return _style;
			}
		}
		protected override void Render (HtmlTextWriter writer) 
		{
			if (TextKey != String.Empty)
			{
				if (HttpContext.Current != null)
				{
					// Gets translation from resource dll
					// and automatically update database if needed
					Text = Localize.GetString(TextKey, Text, this);
				}
				else if (Text == string.Empty)
				{
					//design time
					Text = "{" + TextKey + "}";
				}			
			}
			base.Render(writer);
		}
	}
}
