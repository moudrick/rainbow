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
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.MobileControls.Adapters;
using Rainbow.UI;
using Rainbow.UI.MobileControls;

using Rainbow.Configuration;

[assembly:TagPrefix("Rainbow.UI.MobileControls", "portal")]
namespace Rainbow.UI.MobileControls
{

	/// <summary>
	/// The MobilePortalModuleControl class is the base class used for
	/// each module user control in the mobile portal. Since it implements
	/// the IContentsPane interface, any control inheriting from this class
	/// can be used as a module in a portal tab.
	/// </summary>
	public class MobilePortalModuleControl : System.Web.UI.MobileControls.MobileUserControl, IContentsPane
	{
		private ModuleSettings _moduleConfiguration;
		private Control _summaryControl;
		//private StyleSheet _mobileStyleSheet;

		/// <summary>
		/// Returns the configuration information for this module.
		/// </summary>
		public ModuleSettings ModuleConfiguration 
		{
			get 
			{
				return _moduleConfiguration;
			}
			set 
			{
				_moduleConfiguration = value;
			}
		}

		/// <summary>
		/// Returns the parent portal tab.
		/// </summary>
		public MobilePortalTab Tab 
		{
			get 
			{
				return Parent as MobilePortalTab;
			}
		}

		/// <summary>
		/// Returns the name of this module.
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public String ModuleTitle 
		{
			get 
			{
				return ModuleConfiguration.ModuleTitle;
			}
		}

		/// <summary>
		/// Returns the unique ID of this module.
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ModuleId 
		{
			get 
			{
				return ModuleConfiguration.ModuleID;
			}
		}

		/// <summary>
		/// Returns the TabId of the tab the module is inside.
		/// </summary>
		int IPanelPane.TabId
		{
			get 
			{
				return _moduleConfiguration.TabID;
			}
		}

		/// <summary>
		/// Returns the name of the module, to be used as the pane title
		/// when used inside a tab.
		/// </summary>
		string IPanelPane.Title 
		{
			get 
			{
				return _moduleConfiguration.ModuleTitle;
			}
		}

		/// <summary>
		/// OnSetSummaryMode is called on each child pane when the parent tab
		/// changes from showing summaries to individual details or vice versa.
		/// This method calls the UpdateVisibility utility method to 
		/// update the visibility of child controls.
		/// REVIEW: Probably could be done using an event handler instead.
		/// </summary>
		void IContentsPane.OnSetSummaryMode() 
		{
			UpdateVisibility();
		}

		/// <summary>
		/// OnInit is called when the control is created and added to the 
		/// control tree. OnInit looks for a child control that renders the
		/// summary view of the module, and creates a default one (with a
		/// simple LinkCommand control) if no summary is found.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) 
		{
			base.OnInit(e);

			if (HttpContext.Current == null)
				return;

			// Look for a control that renders the summary.
			_summaryControl = FindControl("summary");

			// There could be no summary control, or the summary control may be
			// an empty panel. If there's no summary UI, automatically generate one.
			if (_summaryControl == null || (_summaryControl is Panel && !_summaryControl.HasControls())) 
			{
				// Create and initialize a new LinkCommand control
				Command command = new LinkCommand();
				command.Text = this.ModuleTitle;

				// Set the command name to the details command, so that
				// event bubbling can recognize it as a command to go to
				// details view.
				command.CommandName = ContentsPanel.DetailsCommand;

				// Add it to the appropriate place.
				if (_summaryControl != null) 
				{
					_summaryControl.Controls.Add(command);
				}
				else 
				{
					Controls.Add(command);
					_summaryControl = command;
				}
			}
		}


		/// <summary>
		/// OnLoad is called when the control is created and added to the 
		/// control tree, after OnInit. OnLoad calls the UpdateVisibility
		/// utility method to update the visibility of child controls.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e) 
		{
			base.OnLoad(e);
			UpdateVisibility();
		}

		/// <summary>
		/// UpdateVisibility updates the visibility of child controls
		/// depending on the current setting. If the module is currently
		/// being shown in summary mode, all children except the summary
		/// control are hidden. If the module is currently being shown
		/// in details mode, only the summary control is hidden.
		/// </summary>
		private void UpdateVisibility() 
		{
			if (HttpContext.Current == null)
				return;

			bool summary = Tab != null && Tab.SummaryView;
            
			foreach (Control child in Controls) 
				child.Visible = !summary;

			if (_summaryControl != null) 
				_summaryControl.Visible = summary;
		}


//		/// <summary>
//		/// 
//		/// </summary>
//		public StyleSheet MobileStyleSheet
//		{
//			get 
//			{
//				return ((Rainbow.UI.MobilePage)Page).ThemeStyleSheet ;
//			}
//		}


		public new Rainbow.UI.MobilePage Page
		{
			get
			{
				return (Rainbow.UI.MobilePage) base.Page;
			}
			set
			{
				base.Page = value;
			}
		}
	
	
	}
}

