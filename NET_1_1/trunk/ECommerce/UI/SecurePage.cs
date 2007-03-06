using System;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rainbow.UI;
using Rainbow.UI.WebControls;
using Esperantus;
using Rainbow.DesktopModules;
using Rainbow.Security;
using Rainbow.Configuration;
using Rainbow.UI.DataTypes;
using Rainbow.Helpers;

namespace Rainbow.ECommerce.UI
{
	/// <summary>
	/// SecurePage inherits from Rainbow.UI.SecurePage <br/>
	/// Used for pages in the secure area<br/>
	/// Can be inherited
	/// </summary>
	public class SecurePage : Rainbow.UI.SecurePage
	{
		protected override void Render(HtmlTextWriter writer)
		{
			// BUILD THE HEAD
			this.BuildHtmlHead(writer);

			bool formRendered = false;
			
			// OUTPUT PAGE CONTENT
			//base.Render(writer);
			foreach ( Control c in this.Controls )
			{
				if (c is System.Web.UI.HtmlControls.HtmlForm || formRendered)
				{
					c.RenderControl(writer);
					formRendered = true;
				}
				else
				{
					writer.WriteLine("<!-- Removed Rainbow.Page.Render() ");
					c.RenderControl(writer);
					writer.WriteLine(" -->");
				}
			}
		}
	}
}
