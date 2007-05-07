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
    /// The ContentsPanel Class is a control that inherits from MultiPanel,
    /// and can render child panes in one of two views. In Summary View,
    /// the control renders each of its child panes (which, in turn, would
    /// probably show only summarized views of themselves) In Details View
    /// the control only renders the active pane.
	/// </summary>
    public class ContentsPanel : Rainbow.UI.MobileControls.MultiPanel
    {
        // Constants for command names that can be used for
        // event bubbling in custom UI.
        public static readonly String DetailsCommand = "COMMAND_DETAILS";
        public static readonly String SummaryCommand = "COMMAND_SUMMARY";

		/// <summary>
		/// Get or set the view of the panel to either Summary (true) 
		/// or Details (false) view.
		/// </summary>
        public bool SummaryView 
        {
            get 
            {
                // Get the setting from the ViewState property bag, defaulting
                // to true.
                Object o = ViewState["SummaryView"];
                return (o != null) ? (bool)o : true;
            }

            set 
            {
                // Save the setting in the ViewState property bag.
                ViewState["SummaryView"] = value;

                // Notify each child pane of the switched mode.
                foreach (IContentsPane pane in Panes) 
                {
                    pane.OnSetSummaryMode();
                }
            }
        }


		/// <summary>
		/// The ShowDetails method switches the control into Details view,
		/// and makes the specified child pane active. Child panes can
		/// call this method to activate themselves.
		/// </summary>
		/// <param name="pane"></param>
		public void ShowDetails(IPanelPane pane) 
		{
			SummaryView = false;
			ActivePane = pane;
		}


		/// <summary>
		/// Called by the framework to render the control. The behavior differs
		/// depending on whether Summary or Details view is showing.
		/// </summary>
		/// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer) 
        {
            if (SummaryView) 
            {
                // Render all panes in Summary view.
                RenderChildren(writer);
            }
            else 
            {
                // Render only the active pane in Details view.
                ((Control)ActivePane).RenderControl(writer);
            }
        }

        
		/// <summary>
		/// Called by the framework when postback events are bubbled up 
		/// from a child control. If the event source uses the special
		/// command names listed above, this method automatically responds
		/// to the event to change modes. This allows the developer to 
		/// provide UI for showing item details by simply placing a 
		/// control with the appropriate command name in a child pane.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
        protected override bool OnBubbleEvent(Object sender, EventArgs e) 
        {
            bool handled = false;
            System.Web.UI.WebControls.CommandEventArgs commandArgs = e as System.Web.UI.WebControls.CommandEventArgs;
            if (commandArgs != null && commandArgs.CommandName != null) 
            {
                String commandName = commandArgs.CommandName.ToUpper();

                // Look for recognized command names.

                if (commandName == DetailsCommand) 
                {
                    // To show details, first find the child pane in which the
                    // event source is located.
                    Control ctl = (Control)sender;
                    while (ctl != null && ctl != this) 
                    {
                        IPanelPane pane = ctl as IPanelPane;
                        if (pane != null) 
                        {
                            // Make the pane active, and switch into Details view.
                            ActivePane = pane;
                            SummaryView = false;
                            handled = true;
                            break;
                        }
                        ctl = ctl.Parent;
                    }
                }
                else if (commandName == SummaryCommand) 
                {
                    // Switch into Summary view.
                    SummaryView = true;
                    handled = true;
                }
            }
            return handled;
        }


	}
}

