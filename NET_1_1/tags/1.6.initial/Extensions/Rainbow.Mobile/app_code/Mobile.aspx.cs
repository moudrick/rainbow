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
 *  
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Mobile;

using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Data.SqlClient;

using Rainbow.UI;
using Rainbow.UI.MobileControls;

using Rainbow.Configuration;
using Rainbow.Security;


namespace Rainbow
{
	/// <summary>
	/// The MobileDefault.aspx page is used to load and populate each Mobile Portal View. 
	/// It accomplishes this by reading the layout configuration of the portal 
	/// from the Portal Configuration
	/// system. At the top level is a tab view, implemented using a TabbedPanel custom control. 
	/// Each portal view is inserted into this control, and portal modules (each implemented 
	/// as an ASP.NET user control) are instantiated and inserted into tabs.
	/// </summary>
	public class MobileDefault : Rainbow.UI.MobilePage
	{
		protected Rainbow.UI.MobileControls.TabbedPanel TabView;
		protected System.Web.UI.MobileControls.DeviceSpecific DeviceSpecific1;
		protected System.Web.UI.MobileControls.Image portalImage;
		protected System.Web.UI.MobileControls.Form PortalForm;
		protected Rainbow.UI.MobileControls.Image HeaderImage;
		protected Rainbow.UI.MobileControls.StyleSheet MobileStyleSheet;
	

		/// <summary>
		/// The PopulateTabStrip method is used to dynamically create and add
		/// tabs for each tab view defined in the portal configuration.
		/// </summary>
		private void PopulateTabStrip() 
		{
			if (base.portalSettings.MobileTabs.Count == 0 )
				throw new  HttpException(Esperantus.Localize.GetString("MOBILE_NO_PAGES_DEFINED", "Sorry, this site has no mobile pages defined!"));
				
			for (int i=0;i < base.portalSettings.MobileTabs.Count; i++) 
			{
				// Create a MobilePortalTab control for the tab,
				// and add it to the tab view.
				TabStripDetails tab = (TabStripDetails)portalSettings.MobileTabs[i];

				if (PortalSecurity.IsInRoles(tab.AuthorizedRoles)) 
				{
					MobilePortalTab tabPanel = new MobilePortalTab();
					tabPanel.Title = Server.HtmlDecode(tab.TabName);
					tabPanel.TabId = tab.TabID;
					TabView.Panes.Add(tabPanel);
				}
			}
		}
		/// <summary>
		/// The PopulateTabView method dynamically populates a portal tab
		/// with each module defined in the portal configuration.
		/// </summary>
		/// <param name="tabIndex"></param>
		private void PopulateTabViewWithTabId(int tabId) 
		{
			// Obtain reference to container mobile tab
			TabView.ActivePaneTabID = tabId;
			MobilePortalTab view = (MobilePortalTab) TabView.Panes.GetPane(tabId);

			TabSettings activeTab =  portalSettings.ActiveTab;

			PortalForm.Title =  Server.HtmlDecode(portalSettings.PortalTitle);

			// Dynamically populate the view
			if (activeTab.Modules.Count > 0) 
			{
				// Loop through each entry in the configuration system for this tab
				foreach (ModuleSettings moduleSettings in activeTab.Modules) 
				{
					// NEW MODULE_VIEW PERMISSIONS ADDED
					// Ensure that the visiting user has access to view the current module
					if (PortalSecurity.IsInRoles(moduleSettings.AuthorizedViewRoles) == true) 
					{ 
						// Only add the module if it support Mobile devices and have a sourcefile declared.
						if (moduleSettings.ShowMobile && moduleSettings.MobileSrc.EndsWith (".ascx") )
						{
							MobilePortalModuleControl moduleControl = (MobilePortalModuleControl) Page.LoadControl(moduleSettings.MobileSrc);
							moduleControl.ModuleConfiguration = moduleSettings;
							view.Panes.Add(moduleControl);
						}
					}
				}
			}
		}

		/// <summary>
		/// The TabView_OnActivate event handler executes when the user switches
		/// tabs in the tab view. It calls the PopulateTabView utility
		/// method to dynamically populate the newly activated view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TabView_OnTabActivate(Object sender, TabbedPanel.TabEventArgs e) 
		{
			if   (!(TabIndex == e.TabIndex && TabId == e.TabId) )
			{
				// Check to see if portal settings need reloading
				LoadPortalSettings(e.TabIndex,e.TabId);

				// Populate the newly active tab.
				//PopulateTabView(TabIndex);
				PopulateTabViewWithTabId (e.TabId);
			}
			// Set the view to summary mode, where a summary of all the modules are shown.
			((MobilePortalTab)TabView.ActivePane).SummaryView = true;
		}


		protected override void OnLoad(EventArgs e)
		{
		
			if (!Page.IsPostBack)
			{

			}
	
			base.OnLoad (e);
		}


		#region Web Form Designer generated code
		/// <summary>
		/// The Page_Init event handler executes at the very beginning of each page
		/// request (immediately before Page_Load).
		/// The Page_Init event handler calls the PopulateTabs utility method
		/// to insert empty tabs into the tab view. It then determines the tab
		/// index of the currently requested portal, and then calls the
		/// PopulateTabView utility method to dynamically populate the
		/// active portal view.
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();

			if (MobileStyleSheet == null)
				MobileStyleSheet = new Rainbow.UI.MobileControls.StyleSheet();

			// Obtain PortalSettings from Current Context
			LoadPortalSettings(TabIndex,TabId);
			
			// Populate tab list with empty tabs
			PopulateTabStrip();

			// Populate the current tab view
			PopulateTabViewWithTabId (TabId);
			
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.TabView.TabActivate += new Rainbow.UI.MobileControls.TabbedPanel.TabbedPanelEventHandler(this.TabView_OnTabActivate);
		}
		#endregion
	}
}
