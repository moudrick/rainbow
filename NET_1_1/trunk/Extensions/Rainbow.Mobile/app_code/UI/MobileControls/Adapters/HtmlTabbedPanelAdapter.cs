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


namespace Rainbow.UI.MobileControls.Adapters
{
	/// <summary>
	/// The HtmlTabbedPanelAdapter provides rendering for the TabbedPanel
	/// class on devices that support HTML and JScript.
	/// </summary>
    public class HtmlTabbedPanelAdapter : HtmlControlAdapter 
    {


		/// <summary>
		/// Returns the attached control, strongly typed as a TabbedPanel.
		/// </summary>
		protected new TabbedPanel Control 
		{
			get 
			{
				return (TabbedPanel) base.Control;
			}
		}

		/// <summary>
		/// Renders the control. The TabbedPanel is rendered as one or more
		/// rows of tabs that the user can click on to move between tabs.
		/// </summary>
		/// <param name="writer"></param>
        public override void Render(HtmlMobileTextWriter writer) 
        {
            IPanelPane activePane = Control.ActivePane;
            int tabsPerRow = Control.TabsPerRow;
            PanelPaneCollection panes = Control.Panes;
            int paneCount = panes.Count;

            // Figure out the number of visible panes.
            int[] visiblePanes = new int[paneCount];
            int visiblePaneCount = 0;
            for (int i = 0; i < paneCount; i++) 
            {
                if (((Control)panes[i]).Visible) 
                {
                    visiblePanes[visiblePaneCount++] = i;
                }
            }

            // Calculate how many rows are necessary.
            int rows = (visiblePaneCount + tabsPerRow - 1) / tabsPerRow;

            // make sure tabsPerRow doesn't exceed the number of visible panes
            tabsPerRow = (Control.TabsPerRow >  visiblePaneCount) ? visiblePaneCount : Control.TabsPerRow;

            // Open the table.
            writer.WriteBeginTag("table");
            writer.WriteAttribute("cellspacing", "0");
            writer.WriteAttribute("cellpadding", "0");
            writer.WriteAttribute("border", "0");
            writer.WriteLine(">");

            for (int row = rows - 1; row >= 0; row--) 
            {
                writer.WriteFullBeginTag("tr");
                writer.WriteLine();
                for (int col = 0; col < tabsPerRow; col++) 
                {
                    int i = row * tabsPerRow + col;
                    if (row > 0 && i >= visiblePaneCount) 
                    {
                        writer.WriteFullBeginTag("td");
                        writer.WriteEndTag("td");
                        continue;
                    }

                    int index = visiblePanes[i];
                    IPanelPane child = panes[index];
                    if (child == activePane) 
                    {
                        writer.WriteBeginTag("td");
						writer.WriteAttribute("bgcolor", GetColorString(Control.ActiveTabStyle.BackColor, "#333333"));
                        writer.Write(">");
						
						writer.Write("&#160;");
						writer.WriteBeginTag("a");
						//writer.WriteAttribute("href",HttpUrlBuilder.BuildUrl(child.TabId));
						//RenderPostBackEventAsAttribute(writer, "href", index.ToString());
						RenderPostBackEventAsAttribute(writer, "href", child.TabId.ToString());

						writer.Write(">");
						writer.EnterStyle(Control.ActiveTabStyle);

						writer.WriteText(child.Title, true);
						writer.ExitStyle(Control.ActiveTabStyle);

						writer.WriteEndTag("a");
						writer.Write("&#160;");
                        writer.WriteEndTag("td");
                        writer.WriteLine();
                    }
                    else 
                    {
                        writer.WriteBeginTag("td");
                        writer.WriteAttribute("bgcolor", GetColorString(Control.TabStyle.BackColor, "#cccccc"));
                        writer.Write(">");
                        writer.Write("&#160;");
                        writer.WriteBeginTag("a");
						//writer.WriteAttribute("href",HttpUrlBuilder.BuildUrl(child.TabId));
						//RenderPostBackEventAsAttribute(writer, "href", index.ToString());
						RenderPostBackEventAsAttribute(writer, "href", child.TabId.ToString());
                        writer.Write(">");

						writer.EnterStyle(Control.TabStyle);
						writer.WriteText(child.Title, true);
						writer.ExitStyle(Control.TabStyle);
					                    
						writer.WriteEndTag("a");
                        writer.Write("&#160;");

                        writer.WriteEndTag("td");
                        writer.WriteLine();
                    }
                }
                writer.WriteEndTag("tr");
                writer.WriteLine();

                if (row > 0) 
                {
                    writer.WriteFullBeginTag("tr");
                    writer.WriteBeginTag("td");
                    writer.WriteAttribute("height", "1");
                    writer.Write(">");
                    writer.WriteEndTag("td");
                    writer.WriteEndTag("tr");
                    writer.WriteLine();
                }
            }

            writer.WriteEndTag("table");
            writer.WriteLine();

            writer.WriteBreak();
        
            ((Control)activePane).RenderControl(writer);
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

