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


namespace Rainbow.UI.MobileControls.Adapters
{
	/// <summary>
	/// ChtmlTabbedPanelAdapter
	/// </summary>	
	public class ChtmlTabbedPanelAdapter : HtmlControlAdapter  
	{ 
		/// <summary>
		/// TabbedPanel
		/// </summary>
        protected new TabbedPanel Control 
        {
            get 
            {
                return (TabbedPanel)base.Control;
            }
        }

		/// <summary>
		/// Render
		/// </summary>
		/// <param name="writer"></param>
        public override void Render(HtmlMobileTextWriter writer) 
        {
            writer.EnterStyle(Control.TabStyle);

            IPanelPane activePane = Control.ActivePane;

			writer.EnterStyle(Control.TabStyle);
			writer.Write("[ ");

            int index = 0;
            foreach (IPanelPane child in Control.Controls) 
            {
                if (!((Control)child).Visible) 
                {
                    index++;
                    continue;
                }
                if (index > 0) 
                {
					writer.Write(" | ");
                }

                if (child == activePane) 
                {
					writer.EnterStyle(Control.ActiveTabStyle);
                    writer.WriteText(child.Title, true);
					writer.ExitStyle(Control.ActiveTabStyle);
                }
                else 
                {
                    writer.WriteBeginTag("a");
					RenderPostBackEventAsAttribute(writer, "href", child.TabId.ToString());
//					writer.WriteAttribute("href" , HttpUrlBuilder.BuildUrl("MobileDefault2.aspx", child.TabId, "ti=" + index.ToString() + "," + child.TabId.ToString()));
					//writer.WriteAttribute("href" , "MobileDefault.aspx?ti=" + index.ToString() + "," + child.TabId.ToString());
					writer.Write(">");
					writer.EnterStyle(Control.TabStyle);
					writer.WriteText("C:"+ child.Title, true);
					writer.ExitStyle(Control.TabStyle);
                    writer.WriteEndTag("a");
                }

                index++;
            }
            writer.Write(" ]");
			writer.ExitStyle(Control.TabStyle);
            writer.WriteBreak();
            ((Control)activePane).RenderControl(writer);
        }
    }


}

