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



namespace Rainbow.UI.MobileControls
{
	/// <summary>
    /// The ChildPanel Class is a control that inherits from 
    /// System.Web.UI.MobileControls.Panel, and can be placed inside
    /// a MultiPanel control. Even MultiPanel inherits from ChildPanel,
    /// allowing nesting of MultiPanel controls.
	///</summary>
    public class ChildPanel : Panel, IPanelPane, INamingContainer 
    {

		/// <summary>
		/// Returns the title of the pane.
		/// </summary>
        String IPanelPane.Title 
        {
            get 
            {
                return this.Title;
            }
        }



		/// <summary>
		/// Returns the title of the pane.
		/// </summary>
        public String Title 
        {
            get 
            {
				// Load the title from the ViewState property bag, 
                // defaulting to an empty String.
                String s = (String)ViewState["Title"];
                return s != null ? s : String.Empty;
            }

            set 
            {
                // Save the title to the ViewState property bag.
                ViewState["Title"] = value;
            }
        }
		int IPanelPane.TabId
		{
			get 
			{
				return this.TabId;
			}
		}
		/// <summary>
		/// Returns the title of the pane.
		/// </summary>
		public int TabId 
		{
			get 
			{
				// Load the title from the ViewState property bag, 
				// defaulting to an empty String.
				return (ViewState["TabId"] !=null) ? (int)ViewState["TabId"] : 0;
			}

			set 
			{
				// Save the title to the ViewState property bag.
				ViewState["TabId"] = value;
			}
		}


		/// <summary>
		/// The PaginateChildren property controls whether the form
		/// can paginate children of the panel individually. Overriden
		/// to allow contents to be paginated.
		/// </summary>
        protected override bool PaginateChildren 
        {
            get 
            {
                return true;
            }
        }
    }


}

