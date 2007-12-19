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
	/// WmlTabbedPanelAdapter
	/// 
	/// </summary>	
	public class WmlTabbedPanelAdapter : WmlControlAdapter 
    {

		private List _menu;

		/// <summary>
		/// 
		/// </summary>
        protected new TabbedPanel Control 
        {
            get 
            {
                return (TabbedPanel)base.Control;
            }
        }

		/// <summary>
		/// OnInit Event
		/// </summary>
		/// <param name="e"></param>
        public override void OnInit(EventArgs e) 
        {
            _menu = new List();
			_menu.ItemsAsLinks = false;
            _menu.ItemCommand += new ListCommandEventHandler(OnListItemCommand);
            Control.Controls.AddAt(0, _menu);
        }

		/// <summary>
		/// OnLoad Event
		/// </summary>
		/// <param name="e"></param>
        public override void OnLoad(EventArgs e) 
        {
            _menu.Items.Clear();
            foreach (IPanelPane child in Control.Panes) 
            {
                if (((Control)child).Visible) 
                {
					_menu.Items.Add(new MobileListItem(child.Title, child.TabId.ToString()));
                }
            }
        }

		/// <summary>
		/// Render
		/// </summary>
		/// <param name="writer"></param>
		public override void Render(WmlMobileTextWriter writer) 
        {
			writer.EnterStyle(Control.TabStyle);
			if (_menu.Visible) 
			{
				_menu.RenderControl(writer);
			}
			else 
			{
				((Control)Control.ActivePane).RenderControl(writer);
			}
			writer.ExitStyle(Control.TabStyle);
		}

		/// <summary>
		/// OnListItemCommand
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnListItemCommand(Object sender, ListCommandEventArgs e) 
        {
            _menu.Visible = false;
            Control.RaisePostBackEvent(e.ListItem.Value);
        }

	
		private void ApplyStyle(MobileControl control , Style style)
		{
			control.StyleReference = style.StyleReference;
			
			control.Font.Bold = style.Font.Bold;
			control.Font.Italic = style.Font.Italic;
			control.Font.Name = style.Font.Name;
			control.Font.Size = style.Font.Size;

			control.Alignment = style.Alignment;
			control.BackColor = style.BackColor;
			control.ForeColor = style.ForeColor;
			control.Wrapping = style.Wrapping;
		}
	}


}

