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

using Rainbow.Configuration;

using Esperantus;


namespace Rainbow.UI.MobileControls
{
	/// <summary>
	/// The LinkCommand class is used for a simple custom version of the
	/// Command control. Although the class itself has no added or modified
	/// functionality, it allows a new adapter to be specified. On
	/// HTML devices, this control renders as a hyperlink rather than
	/// a button.
	/// </summary>
	public class LinkCommand : Command 
	{
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


		protected override void Render(HtmlTextWriter writer) 
		{
			if (HttpContext.Current != null)
			{
				try
				{
					//PortalSettings	portalSettings;
					// Obtain PortalSettings from Current Context
					PortalSettings	portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
					if ( ImageUrl.StartsWith("~~/"))
					{
						ImageUrl = portalSettings.GetCurrentTheme().WebPath + ImageUrl.Substring(0).Replace("~~/", "/");
						//ImageUrl = (HttpContext.Current.Request.ApplicationPath + ImageUrl.Substring(1)).Replace("//", "/");
					}
					if (ImageUrl.StartsWith("~/"))
						ImageUrl = (HttpContext.Current.Request.ApplicationPath + ImageUrl.Substring(0)).Replace("~/", "/");

									}
					//			if (HttpContext.Current != null && ImageUrl.StartsWith("~/"))
					//			{
					//				ImageUrl = (HttpContext.Current.Request.ApplicationPath + ImageUrl.Substring(1)).Replace("//", "/");
					//			}

				catch
				{}
				RenderChildren(writer);
				base.Render(writer);
			}

		}

	}
}
	


