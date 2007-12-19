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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using Rainbow.UI;
using Rainbow.UI.MobileControls;

using Rainbow.DesktopModules;
using Rainbow.Configuration;

namespace Rainbow.Modules.Announcements
{
	/// <summary>
	///	The Announcements Mobile User Control renders announcement modules in the
	///	portal for mobile devices. 
	///
	/// The control consists of two pieces: a summary panel that is rendered when
	/// portal view shows a summarized view of all modules, and a multi-part panel 
	/// that renders the module details.
	/// </summary>
	public class AnnouncementsMobile : Rainbow.UI.MobileControls.MobilePortalModuleControl
	{
		protected DataSet ds = null;

		protected Rainbow.UI.MobileControls.MobileTitle Title1;
		protected Rainbow.UI.MobileControls.MultiPanel MainView;
		protected System.Web.UI.MobileControls.DeviceSpecific DeviceSpecific1;
		protected System.Web.UI.MobileControls.Panel summary;
		protected Rainbow.UI.MobileControls.MobileTitle multipanelTitle1;
		protected Rainbow.UI.MobileControls.MobileTitle multipanelTitle2;
		protected System.Web.UI.MobileControls.List List1;
		protected System.Web.UI.WebControls.Repeater AnnouncementListRepeater ;
		protected Rainbow.UI.MobileControls.ChildPanel AnnouncementsList;
		protected Rainbow.UI.MobileControls.ChildPanel AnnouncementDetails;
		protected System.Web.UI.MobileControls.TextView TextView1;
		protected Rainbow.UI.MobileControls.Globalized.Link LinkMore;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkBack;
		int currentIndex = 0;


		/// <summary>
		///  The Page_Load event handler on this User Control is used to
		/// obtain a DataSet of announcement information from the Announcements
		/// table, and then databind the results to the module contents.  It uses 
		/// the ASPNetPortal.AnnouncementsDB() data component 
		/// to encapsulate all data functionality.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Page_Load(Object sender, EventArgs e) 
		{
			// Obtain announcement information from Announcements table
			// and bind to the datalist control
			AnnouncementsMobileDB announcements = new AnnouncementsMobileDB ();

			// DataBind Announcements to DataList Control
			ds = announcements.GetAnnouncements(ModuleId);

			// DataBind the User Control
			DataBind();
		}
                  
		/// <summary>
		/// The SummaryView_OnItemCommand event handler is called when the user
		/// clicks on a "More" link in the summary view. It calls the
		/// ShowAnnouncementDetails utility method to show details of the
		/// announcement.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void SummaryView_OnItemCommand(Object sender, RepeaterCommandEventArgs e) 
		{
			ShowAnnouncementDetails(e.Item.ItemIndex);        
		}

		/// <summary>
		/// The AnnouncementsList_OnItemCommand event handler is called when the user
		/// clicks on an item in the list of announcements. It calls the
		/// ShowAnnouncementDetails utility method to show details of the
		/// announcement.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AnnouncementsList_OnItemCommand(Object sender, ListCommandEventArgs e) 
		{
			ShowAnnouncementDetails(e.ListItem.Index);        
		}

		/// <summary>
		/// The DetailsView_OnClick event handler is called when the user 
		/// clicks in the details view to return to the summary view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DetailsView_OnClick(Object sender, EventArgs e) 
		{
			// Make the parent tab show module summaries again.
			Tab.SummaryView = true;
		}

		/// <summary>
		/// The ShowAnnouncementDetails method sets the active pane of
		/// the module to the details view, and shows the details of the
		/// given item.
		/// </summary>
		/// <param name="itemIndex"></param>
		void ShowAnnouncementDetails(int itemIndex) 
		{
			currentIndex = itemIndex;

			// Rebind the details panel
			AnnouncementDetails.DataBind(); 
        
			// Switch the visible pane of the multi-panel view to show
			// announcement details
			MainView.ActivePane = AnnouncementDetails;

			// Make the parent tab switch to details mode, showing this module
			Tab.ShowDetails(this);
		}
    
		/// <summary>
		/// The FormatChildField method returns the selected field as a string,
		/// if the row is not empty.  If empty, it returns String.Empty.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		protected string FormatChildField (string fieldName) 
		{
    
			if (ds.Tables[0].Rows.Count > 0) 
				return ds.Tables[0].Rows[currentIndex][fieldName].ToString();
			else
				return String.Empty;
		}


		#region Web Form Designer generated code
		/// <summary>
		/// OnInit
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();

			base.OnInit(e);
		}
		
		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
