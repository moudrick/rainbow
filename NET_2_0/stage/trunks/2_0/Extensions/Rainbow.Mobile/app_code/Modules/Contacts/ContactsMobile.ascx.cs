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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using Rainbow.UI;
using Rainbow.UI.MobileControls;


using Rainbow.DesktopModules;
using Rainbow.Configuration;

namespace Rainbow.Modules.Contacts
{
	/// <summary>
	///	The Contacts Mobile User Control renders Contacts modules in the
	///	mobile portal. 
	///	The control consists of two pieces: a summary panel that is rendered when
	///	portal view shows a summarized view of all modules, and a multi-part panel 
	///	that renders the module details.
	/// </summary>
	public class ContactsMobile : Rainbow.UI.MobileControls.MobilePortalModuleControl
	{
		protected ChildPanel ContactDetails;
		protected ChildPanel ContactsList;
		protected MultiPanel MainView;

		protected DataSet ds = null;
		protected Rainbow.UI.MobileControls.StyleSheet MobileStyleSheet;
		protected System.Web.UI.MobileControls.DeviceSpecific DeviceSpecific1;
		protected System.Web.UI.MobileControls.Panel summary;
		protected Rainbow.UI.MobileControls.MobileTitle Mobiletitle2;
		protected System.Web.UI.MobileControls.List List1;
		protected Rainbow.UI.MobileControls.MobileTitle Mobiletitle3;
		protected Rainbow.UI.MobileControls.Globalized.Label Label2;
		protected System.Web.UI.MobileControls.Link Link1;
		protected Rainbow.UI.MobileControls.Globalized.Label Label3;
		protected System.Web.UI.MobileControls.PhoneCall PhoneCall1;
		protected System.Web.UI.MobileControls.PhoneCall PhoneCall2;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand1;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand2;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand3;
		protected Rainbow.UI.MobileControls.Globalized.LinkCommand LinkCommand4;
		protected Rainbow.UI.MobileControls.Globalized.Label LabelA1;
		protected System.Web.UI.MobileControls.Label Label1;
		int currentIndex = 0;
    
		/// <summary>
		/// The Page_Load event handler on this User Control is used to
		/// obtain a DataSet of contact information from the Contacts
		/// database, and then databind the results to the module contents.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Page_Load(Object sender, EventArgs e) 
		{
			// Obtain contact information from Contacts table
			// and bind to the DataGrid Control
			ContactsMobileDB contacts = new ContactsMobileDB();

			ds = contacts.GetContacts(ModuleId);

			// DataBind User Control
			DataBind();
			
		}
                  
		/// <summary>
		/// The SummaryView_OnClick event handler is called when the user
		/// clicks on the link in the summary view. It shows the list of
		/// contacts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void SummaryView_OnClick(Object sender, EventArgs e) 
		{
    
			// Switch the visible pane of the multi-panel view to show
			// the list of contacts.
			MainView.ActivePane = ContactsList;

			// Make the parent tab switch to details mode, showing this module.
			Tab.ShowDetails(this);
		}

		/// <summary>
		/// The MainView_OnClick event handler is called when the user
		/// clicks on the back link in the main view. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MainView_OnClick(Object sender, EventArgs e) 
		{
			ContactsList.DataBind();

			// Switch the visible pane of the multi-panel view to show
			// the list of contacts.
			MainView.ActivePane = ContactsList;

			// Make the parent tab show module summaries again.
			Tab.SummaryView = true;
		}

		/// <summary>
		/// The ContactsList_OnItemCommand event handler is called when the user
		/// clicks on a contact in the contact list. It shows the details of the
		/// contact.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ContactsList_OnItemCommand(Object sender, ListCommandEventArgs e) 
		{
    		currentIndex = e.ListItem.Index;
			ContactDetails.DataBind();

			// Switch the visible pane of the multi-panel view to show
			// contact details.
			MainView.ActivePane = ContactDetails;
            
			// rebind the details panel
			ContactDetails.DataBind();
		}

		/// <summary>
		/// The DetailsView_OnClick event handler is called when the user
		/// clicks on a link in the contact details view to return to the
		/// list of contacts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DetailsView_OnClick(Object sender, EventArgs e) 
		{
			ContactsList.DataBind();

			// Switch the visible pane of the multi-panel view to show
			// the list of contacts.
			MainView.ActivePane = ContactsList;
		}

		/// <summary>
		/// The GetPhoneNumber method extracts a phone number from a contact
		/// entry, if possible, using a regular expression search.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		protected String GetPhoneNumber(String s) 
		{
			if (s != String.Empty) 
			{
				// Look for a phone number.
				Match phoneMatch = Regex.Match(s, "\\+?[\\d\\(\\)\\.-]+");
				s = phoneMatch.Success ? phoneMatch.ToString() : String.Empty;
			}
			return s; 
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
