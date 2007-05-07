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

namespace Rainbow.Modules.Events
{
	/// <summary>
	///	The Events Mobile User Control renders event modules in the portal. 
	///	The control consists of two pieces: a summary panel that is rendered when
	///	portal view shows a summarized view of all modules, and a multi-part panel 
	///	that renders the module details.
	/// </summary>
	public class EventsMobile : Rainbow.UI.MobileControls.MobilePortalModuleControl
	{
		protected ChildPanel EventDetails;
		protected MultiPanel MainView;

		protected DataSet ds = null;
        protected System.Web.UI.MobileControls.Panel summary;
        protected System.Web.UI.MobileControls.DeviceSpecific DeviceSpecific1;
        protected System.Web.UI.MobileControls.List List1;
        protected Rainbow.UI.MobileControls.ChildPanel EventsList;
        protected System.Web.UI.MobileControls.Label Label1;
        protected System.Web.UI.MobileControls.TextView TextView1;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand1;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand2;
		protected Rainbow.UI.MobileControls.MobileTitle Title2;
		protected Rainbow.UI.MobileControls.MobileTitle Title3;
		protected Rainbow.UI.MobileControls.StyleSheet MobileStyleSheet;
		protected Rainbow.UI.MobileControls.MobileTitle Title1;
		int currentIndex = 0;


		/// <summary>
		/// The Page_Load event handler on this User Control is used to
		/// obtain a DataSet of announcement information from the Events
		/// table, and then databind the results to the module contents.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Page_Load(Object sender, EventArgs e) 
		{

			// Obtain announcement information from Events table
			EventsMobileDB ev = new EventsMobileDB();

			ds = ev.GetEvents(ModuleId);

			// DataBind User Control
			DataBind();
		}
                  
                  
		/// <summary>
		/// The SummaryView_OnItemCommand event handler is called when the user
		/// clicks on a "More" link in the summary view. It calls the 
		/// ShowEventDetails utility method to show details of the event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void SummaryView_OnItemCommand(Object sender, RepeaterCommandEventArgs e) 
		{
			ShowEventDetails(e.Item.ItemIndex);
		}

		/// <summary>
		/// The EventsList_OnItemCommand event handler is called when the user
		/// clicks on an item in the list of events. It calls the
		/// ShowEventDetails utility method to show details of the event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void EventsList_OnItemCommand(Object sender, ListCommandEventArgs e) 
		{
			ShowEventDetails(e.ListItem.Index);
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
		/// The ShowEventDetails method sets the active pane of
		/// the module to the details view, and shows the details of the
		/// given item.
		/// </summary>
		/// <param name="itemIndex"></param>
		protected void ShowEventDetails(int itemIndex) 
		{
    
			currentIndex = itemIndex;

			// Switch the visible pane of the multi-panel view to show
			// event details.
			MainView.ActivePane = EventDetails;

			// rebind the details panel
			EventDetails.DataBind();

			// Make the parent tab switch to details mode, showing this module.
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
