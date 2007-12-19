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

using Esperantus;
using Rainbow.Configuration;


namespace Rainbow.UI.MobileControls
{
	/// <summary>
	/// The Image class is used for a simple custom version of the Image Control
	/// </summary>
	[ToolboxData("<{0}:Image  runat=server></{0}:Image>")]
	public class Image :System.Web.UI.MobileControls.Image
	{
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
					
					ImageUrl = ImageUrl.Replace("//","/");
				}
				catch
				{}
				base.Render(writer);
			}

		}
	}
}

