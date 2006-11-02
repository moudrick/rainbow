using System;
using System.Collections;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.Settings.Cache;
using Rainbow.UI;
using ImageButton = Esperantus.WebControls.ImageButton;
using Label = Esperantus.WebControls.Label;
using LinkButton = Esperantus.WebControls.LinkButton;
using Literal = Esperantus.WebControls.Literal;
using Page = System.Web.UI.Page;

namespace Rainbow.Admin
{
	public class PageLayout : EditItemPage
	{
		protected ArrayList leftList;
		protected ArrayList contentList;
		protected Literal tab_name;
		protected Literal tab_name1;
		protected TextBox tabName;
		protected Literal roles_auth;
		protected CheckBoxList authRoles;
		protected Literal tab_parent;
		protected DropDownList parentPage;
		protected Literal show_mobile;
		protected CheckBox showMobile;
		protected Literal mobiletab;
		protected TextBox mobilePageName;
		protected Literal addmodule;
		protected Literal module_type;
		protected DropDownList moduleType;
		protected Literal module_name;
		protected TextBox moduleTitle;
		protected LinkButton AddModuleBtn;
		protected Literal organizemodule;

		protected ListBox leftPane;
		protected ImageButton LeftUpBtn;
		protected ImageButton LeftRightBtn;
		protected ImageButton LeftDownBtn;
		protected ImageButton LeftEditBtn;
		protected ImageButton LeftDeleteBtn;

		protected Literal contentpanel;
		protected ListBox contentPane;
		protected ImageButton ContentUpBtn;
		protected ImageButton ContentLeftBtn;
		protected ImageButton ContentRightBtn;
		protected ImageButton ContentDownBtn;
		protected ImageButton ContentEditBtn;
		protected ImageButton ContentDeleteBtn;

		protected Literal rightpanel;
		protected ListBox rightPane;
		protected ImageButton RightUpBtn;
		protected ImageButton RightLeftBtn;
		protected ImageButton RightDownBtn;
		protected ImageButton RightEditBtn;
		protected ImageButton RightDeleteBtn;

		protected SettingsTable EditTable;
		protected Label lblErrorNotAllowed;
		protected ArrayList rightList;
		protected Literal moduleVisibleLabel;
		protected DropDownList viewPermissions;
		protected Literal moduleLocationLabel;

		protected DropDownList paneLocation;
		protected Literal msgError;
		protected Literal LeftPanel;
		protected ADGroupMember memRoles;

		/// <summary>
		/// The Page_Load server event handler on this page is used
		/// to populate a tab's layout settings on the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			// If first visit to the page, update all entries
			if (!Page.IsPostBack) 
			{
				msgError.Visible = false;

				// Set images for buttons from current theme
				LeftUpBtn.ImageUrl		 = this.CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
				LeftRightBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Right", "Right.gif").ImageUrl;
				LeftDownBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
				LeftEditBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
				LeftDeleteBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;

				ContentUpBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
				ContentLeftBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Left", "Left.gif").ImageUrl;
				ContentRightBtn.ImageUrl = this.CurrentTheme.GetImage("Buttons_Right", "Right.gif").ImageUrl;
				ContentDownBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
				ContentEditBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
				ContentDeleteBtn.ImageUrl= this.CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;

				RightUpBtn.ImageUrl		 = this.CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
				RightLeftBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Left", "Left.gif").ImageUrl;
				RightDownBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
				RightEditBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
				RightDeleteBtn.ImageUrl	 = this.CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
				
				BindData();

				SetSecurityAccess ();

				// 2/27/2003 Start - Ender Malkoc
				// After up or down button when the page is refreshed, select the previously selected
				// tab from the list.
				if (Request.Params["selectedmodid"] != null) 
				{
					try
					{
						int modIndex = Int32.Parse(Request.Params["selectedmodid"]);
						SelectModule(leftPane, GetModules("LeftPane"), modIndex);
						SelectModule(contentPane, GetModules("ContentPane"), modIndex);
						SelectModule(rightPane, GetModules("RightPane"), modIndex);
					}
					catch(Exception ex)
					{
						ErrorHandler.Publish(LogLevel.Error, "After up or down button when the page is refreshed, select the previously selected tab from the list.", ex);
					}
				}
				// 2/27/2003 end - Ender Malkoc
			}
			// Binds custom settings to table
			EditTable.DataSource = new SortedList(pageSettings);
			EditTable.DataBind();
		}

		/// <summary>
		/// This method override the security cookie for allow 
		/// to access property pages of selected module in tab.
		/// jviladiu@portalServices.net (2004/07/23)
		/// </summary>
		private void SetSecurityAccess () 
		{
			HttpCookie cookie;
			DateTime time;
			TimeSpan span;
			string guidsInUse = string.Empty;
			Guid guid;

			ModulesDB mdb = new ModulesDB();

			foreach (ListItem li in leftPane.Items) 
			{
				guid = mdb.GetModuleGuid(int.Parse(li.Value));
				if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";

			}

			foreach (ListItem li in contentPane.Items) 
			{
				guid = mdb.GetModuleGuid(int.Parse(li.Value));
				if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";

			}

			foreach (ListItem li in rightPane.Items) 
			{
				guid = mdb.GetModuleGuid(int.Parse(li.Value));
				if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";

			}

			cookie = new HttpCookie ("RainbowSecurity", guidsInUse);
			time = DateTime.Now;
			span = new TimeSpan (0, 2, 0, 0, 0); // 120 minutes to expire
			cookie.Expires = time.Add (span);
			base.Response.AppendCookie (cookie);

		}

		/// <summary>
		/// Given the moduleID of a module, this function selects the right tab in the provided list control
		/// </summary>
		/// <param name="moduleID">moduleID of the module that needs to be selected</param>
		/// <param name="listBox">Listbox that contains the list of modules</param>
		/// <param name="modules">ArrayList containing the Module Items</param>
		private void SelectModule (ListBox listBox, ArrayList modules, int moduleID)
		{
			for(int i = 0 ; i < modules.Count ; i++)
			{
				if(((ModuleItem)modules[i]).ID == moduleID)
				{
					if(listBox.SelectedItem != null) listBox.SelectedItem.Selected = false;
					listBox.Items[i].Selected = true;
					return;
				}
			}
			return;
		}

		private string AppendModuleID (string url, int moduleID)
		{
			int selectedModIDPos = url.IndexOf("&selectedmodid");
			if(selectedModIDPos >= 0)
			{
				int selectedModIDEndPos = url.IndexOf("&", selectedModIDPos + 1);
				if(selectedModIDEndPos >= 0)
				{
					return url.Substring(0, selectedModIDPos) + "&selectedmodid=" + moduleID + url.Substring(selectedModIDEndPos);
				}
				else
				{
					return url.Substring(0, selectedModIDPos) + "&selectedmodid=" + moduleID;
				}
			}
			else
			{
				return url + "&selectedmodid=" + moduleID;
			}
		}

		/// <summary>
		/// The AddModuleToPane_Click server event handler 
		/// on this page is used to add a new portal module 
		/// into the tab
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[History("bja@reedtek.com", "2003/05/16", "Added extra parameter for collpasable window")]
        [History("john.mandia@whitelightsolutions.com", "2003/05/24", "Added extra parameter for ShowEverywhere")]
		private void AddModuleToPane_Click(Object sender, EventArgs e) 
		{
			// All new modules go to the end of the contentpane
			ModuleItem m = new ModuleItem();
			m.Title = moduleTitle.Text;
			m.ModuleDefID = Int32.Parse(moduleType.SelectedItem.Value);
			m.Order = 999;
        
			// save to database
			ModulesDB _mod = new ModulesDB();
			// Change by Geert.Audenaert@Syntegra.Com
			// Date: 6/2/2003
			// Original:             m.ID = _mod.AddModule(tabID, m.Order, "ContentPane", m.Title, m.ModuleDefID, 0, "Admins", "All Users", "Admins", "Admins", "Admins", false);
			// Changed by Mario Endara <mario@softworks.com.uy> (2004/11/09)
			// The new module inherits security from Pages module (current ModuleID) 
			// so who can edit the tab properties/content can edit the module properties/content (except view that remains =)
			m.ID = _mod.AddModule(PageID, m.Order, paneLocation.SelectedItem.Value.ToString(), m.Title, m.ModuleDefID, 0, 
			                      PortalSecurity.GetEditPermissions(ModuleID), viewPermissions.SelectedItem.Value.ToString(),
							PortalSecurity.GetAddPermissions(ModuleID), PortalSecurity.GetDeletePermissions(ModuleID),
							PortalSecurity.GetPropertiesPermissions(ModuleID), PortalSecurity.GetMoveModulePermissions(ModuleID),
							PortalSecurity.GetDeleteModulePermissions(ModuleID), false, 
							PortalSecurity.GetPublishPermissions(ModuleID), false, false, false);
			// End Change Geert.Audenaert@Syntegra.Com

			// reload the portalSettings from the database
			Context.Items["PortalSettings"] = new PortalSettings(PageID, portalSettings.PortalAlias);
			this.portalSettings = (PortalSettings)Context.Items["PortalSettings"];
        
			// reorder the modules in the content pane
			ArrayList modules = GetModules("ContentPane");
			OrderModules(modules);
        
			// resave the order
			foreach (ModuleItem item in modules) 
			{
				_mod.UpdateModuleOrder(item.ID, item.Order, "ContentPane");
			}

			// Redirect to the same page to pick up changes
			Response.Redirect(AppendModuleID(Request.RawUrl, m.ID));
		}

		/// <summary>
		/// The UpDown_Click server event handler on this page is
		/// used to move a portal module up or down on a tab's layout pane
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpDown_Click(Object sender, ImageClickEventArgs e) 
		{
			string cmd = ((System.Web.UI.WebControls.ImageButton)sender).CommandName;
			string pane = ((System.Web.UI.WebControls.ImageButton)sender).CommandArgument;
			ListBox _listbox = (ListBox) Page.FindControl(pane);
               
			ArrayList modules = GetModules(pane);
        
			if (_listbox.SelectedIndex != -1) 
			{
				int delta;
				int selection = -1; 
            
				// Determine the delta to apply in the order number for the module
				// within the list.  +3 moves down one item; -3 moves up one item
				if (cmd == "down") 
				{
					delta = 3;
					if (_listbox.SelectedIndex < _listbox.Items.Count-1)
						selection = _listbox.SelectedIndex + 1;
				}
				else 
				{
					delta = -3;
					if (_listbox.SelectedIndex > 0)
						selection = _listbox.SelectedIndex - 1;
				}

				ModuleItem m;
				m = (ModuleItem) modules[_listbox.SelectedIndex];

				if (PortalSecurity.IsInRoles(PortalSecurity.GetMoveModulePermissions(m.ID))) 
				{

					m.Order += delta; 
            
					// reorder the modules in the content pane
					OrderModules(modules);
        
					// resave the order
					ModulesDB admin = new ModulesDB();
					foreach (ModuleItem item in modules) 
					{
						admin.UpdateModuleOrder(item.ID, item.Order, pane);
					}
        
					// Redirect to the same page to pick up changes
					Response.Redirect(AppendModuleID(Request.RawUrl, m.ID));
				}
				else 
				{
					msgError.Visible = true;
					return;
				}
			}
		}

		/// <summary>
		/// The RightLeft_Click server event handler on this page is
		/// used to move a portal module between layout panes on
		/// the tab page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RightLeft_Click(Object sender, ImageClickEventArgs e) 
		{
			string sourcePane = ((System.Web.UI.WebControls.ImageButton)sender).Attributes["sourcepane"];
			string targetPane = ((System.Web.UI.WebControls.ImageButton)sender).Attributes["targetpane"];
			ListBox sourceBox = (ListBox) Page.FindControl(sourcePane);
			ListBox targetBox = (ListBox) Page.FindControl(targetPane);
         
			if (sourceBox.SelectedIndex != -1) 
			{
				// get source arraylist
				ArrayList sourceList = GetModules(sourcePane);
        
				// get a reference to the module to move
				// and assign a high order number to send it to the end of the target list
				ModuleItem m = (ModuleItem) sourceList[sourceBox.SelectedIndex];

				if (PortalSecurity.IsInRoles(PortalSecurity.GetMoveModulePermissions(m.ID))) 
				{            
					// add it to the database
					ModulesDB admin = new ModulesDB();
					admin.UpdateModuleOrder(m.ID, 99, targetPane);

					// delete it from the source list
					sourceList.RemoveAt(sourceBox.SelectedIndex);

					// reload the portalSettings from the database
					HttpContext.Current.Items["PortalSettings"] = new PortalSettings(PageID, portalSettings.PortalAlias);
					portalSettings = (PortalSettings) Context.Items["PortalSettings"];
            
					// reorder the modules in the source pane
					sourceList = GetModules(sourcePane);
					OrderModules(sourceList);
            
					// resave the order
					foreach (ModuleItem item in sourceList) 
						admin.UpdateModuleOrder(item.ID, item.Order, sourcePane);
            
					// reorder the modules in the target pane
					ArrayList targetList = GetModules(targetPane);
					OrderModules(targetList);
                        
					// resave the order
					foreach (ModuleItem item in targetList) 
						admin.UpdateModuleOrder(item.ID, item.Order, targetPane);
            
					// Redirect to the same page to pick up changes
					Response.Redirect(AppendModuleID(Request.RawUrl, m.ID));
				}
				else 
				{
					msgError.Visible = true;
				}
			}
		}

		/// <summary>
		/// The OnUpdate on this page is used to save 
		/// the current tab settings to the database and
		/// then redirect back to the main admin page. 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnUpdate(EventArgs e) 
		{
			// Only Update if Input Data is Valid
			if (Page.IsValid == true) 
			{
				try
				{
					SavePageData();

					// Flush all tab navigation cache keys. Very important for recovery the changes
					// made in all languages and not get a error if user change the tab parent.
					// jviladiu@portalServices.net (05/10/2004)
					CurrentCache.RemoveAll("_PageNavigationSettings_");

					// redirect back to the admin page
					// int adminIndex = portalSettings.DesktopPages.Count-1;        
					// 3_aug_2004 Cory Isakson use returntabid from QueryString
					// Updated 6_Aug_2004 by Cory Isakson to accomodate addtional Page Management
					string retPage = Request.QueryString["returnPageID"];
					string returnPage;
					
					if (retPage != null) // user is returned to the calling tab.
					{
						returnPage=HttpUrlBuilder.BuildUrl(int.Parse(retPage));
					} 
					else // user is returned to updated tab
					{
						returnPage=HttpUrlBuilder.BuildUrl(PageID);
					}
					Response.Redirect(returnPage);	
				}
				catch
				{
					lblErrorNotAllowed.Visible = true;
				}
			}
		}

		/// <summary>
		/// The PageSettings_Change server event handler on this page is
		/// invoked any time the tab name or access security settings
		/// change.  The event handler in turn calls the "SavePageData"
		/// helper method to ensure that these changes are persisted
		/// to the portal configuration file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PageSettings_Change(Object sender, EventArgs e) 
		{
			// Ensure that settings are saved
			SavePageData();
		}

		/// <summary>
		/// The SavePageData helper method is used to persist the
		/// current tab settings to the database.
		/// </summary>
		private void SavePageData() 
		{
			// Construct Authorized User Roles string
			string authorizedRoles = string.Empty;

			// added by Jonathan Fong 05/08/2004 to support LDAP
			// www.gt.com.au
			bool useMemberList = HttpContext.Current.User is WindowsPrincipal;
			//jes1111 - useMemberList |= ConfigurationSettings.AppSettings["LDAPLogin"] != null ? true : false;
			useMemberList |= Config.LDAPLogin != string.Empty ? true : false;

			if (useMemberList)
				authorizedRoles = memRoles.Members;
			else
				foreach(ListItem item in authRoles.Items) 
				{
					if (item.Selected == true) 
					{
						authorizedRoles = authorizedRoles + item.Text + ";";
					}
				}

			// update Page info in the database
			new PagesDB().UpdatePage(portalSettings.PortalID, PageID, Int32.Parse(parentPage.SelectedItem.Value), tabName.Text, portalSettings.ActivePage.PageOrder, authorizedRoles, mobilePageName.Text, showMobile.Checked);

			// Update custom settings in the database
			EditTable.UpdateControls();
		}

		/// <summary>
		/// The EditBtn_Click server event handler on this page is
		/// used to edit an individual portal module's settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditBtn_Click(Object sender, ImageClickEventArgs e) 
		{
			string pane = ((System.Web.UI.WebControls.ImageButton)sender).CommandArgument;
			ListBox _listbox = (ListBox) Page.FindControl(pane);

			if (_listbox.SelectedIndex != -1) 
			{
				int mid = Int32.Parse(_listbox.SelectedItem.Value);
            
				// Add role control to edit module settings by Mario Endara <mario@softworks.com.uy> (2004/11/09)
				if (PortalSecurity.IsInRoles(PortalSecurity.GetPropertiesPermissions(mid)))
				{
				// Redirect to module settings page
				Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/ModuleSettings.aspx", PageID,  mid));
			}
				else 
				{
					msgError.Visible = true;
					return;
				}
			}
		}

		/// <summary>
		/// The DeleteBtn_Click server event handler on this page is
		/// used to delete a portal module from the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteBtn_Click(Object sender, ImageClickEventArgs e) 
		{
			string pane = ((System.Web.UI.WebControls.ImageButton)sender).CommandArgument;
			ListBox _listbox = (ListBox) Page.FindControl(pane);
			ArrayList modules = GetModules(pane);

			if (_listbox.SelectedIndex != -1) 
			{
				ModuleItem m = (ModuleItem) modules[_listbox.SelectedIndex];
				if (m.ID > -1) 
				{
					// jviladiu@portalServices.net (20/08/2004) Add role control for delete module
					if (PortalSecurity.IsInRoles(PortalSecurity.GetDeleteModulePermissions(m.ID)))
					{
						// must delete from database too
						ModulesDB moddb = new ModulesDB();
						// TODO add userEmail and useRecycler
						moddb.DeleteModule(m.ID);
					}
					else 
					{
						msgError.Visible = true;
						return;
					}
				}
			}

			// Redirect to the same page to pick up changes
			Response.Redirect(Request.RawUrl);
		}

		/// <summary>
		/// The BindData helper method is used to update the tab's
		/// layout panes with the current configuration information
		/// </summary>
		private void BindData() 
		{
			PageSettings tab = portalSettings.ActivePage;

			// Populate Page Names, etc.
			tabName.Text = tab.PageName;
			mobilePageName.Text = tab.MobilePageName;
			showMobile.Checked = tab.ShowMobile;

			// Populate the "ParentPage" Data
			PagesDB t = new PagesDB();
			SqlDataReader dr = t.GetPagesParent(portalSettings.PortalID, PageID);
			parentPage.DataSource = dr;
			parentPage.DataBind();
			dr.Close(); //by Manu, fixed bug 807858

			if (parentPage.Items.FindByValue(tab.ParentPageID.ToString()) != null)
				parentPage.Items.FindByValue(tab.ParentPageID.ToString()).Selected = true;

			// Translate
			if (parentPage.Items.FindByText(" ROOT_LEVEL") != null)
				parentPage.Items.FindByText(" ROOT_LEVEL").Text = Localize.GetString("ROOT_LEVEL", "Root Level", parentPage);

			// added by Jonathan Fong 05/08/2004 to support LDAP
			// www.gt.com.au
			bool useMemberList = HttpContext.Current.User is WindowsPrincipal;
			//jes1111 - useMemberList |= ConfigurationSettings.AppSettings["LDAPLogin"] != null ? true : false;
			useMemberList |= Config.LDAPLogin != string.Empty ? true : false;

			if (useMemberList)
			{
				memRoles.Visible = true;
				authRoles.Visible = false;
				memRoles.Members = tab.AuthorizedRoles;
			}
			else
			{
			// Populate checkbox list with all security roles for this portal
			// and "check" the ones already configured for this tab
			UsersDB users = new UsersDB();
			SqlDataReader roles = users.GetPortalRoles(portalSettings.PortalID);

			// Clear existing items in checkboxlist
			authRoles.Items.Clear();

			ListItem allItem = new ListItem();
			allItem.Text = "All Users";

			if (tab.AuthorizedRoles.LastIndexOf("All Users") > -1) 
			{
				allItem.Selected = true;
			}

			authRoles.Items.Add(allItem);

			// Authenticated user role added
			// 15 nov 2002 - by manudea
			ListItem authItem = new ListItem();
			authItem.Text = "Authenticated Users";

			if (tab.AuthorizedRoles.LastIndexOf("Authenticated Users") > -1) 
			{
				authItem.Selected = true;
			}

			authRoles.Items.Add(authItem);
			// end authenticated user role added

				while(roles.Read()) 
				{
					ListItem item = new ListItem();
					item.Text = (string) roles["RoleName"];
					item.Value = roles["RoleID"].ToString();
            
					if ((tab.AuthorizedRoles.LastIndexOf(item.Text)) > -1)
						item.Selected = true;
                    
					authRoles.Items.Add(item);
				}
				roles.Close(); //by Manu, fixed bug 807858
			}

			// Populate the "Add Module" Data
			ModulesDB m = new ModulesDB();

			SqlDataReader drCurrentModuleDefinitions = m.GetCurrentModuleDefinitions(portalSettings.PortalID);
			try
			{
				while(drCurrentModuleDefinitions.Read())
				{
					if( PortalSecurity.IsInRoles("Admins") == true || 
						!(bool.Parse(drCurrentModuleDefinitions["Admin"].ToString())))
					{
						moduleType.Items.Add(new ListItem(drCurrentModuleDefinitions["FriendlyName"].ToString(),drCurrentModuleDefinitions["ModuleDefID"].ToString()));
					}
				}
			}
			finally
			{
				drCurrentModuleDefinitions.Close();
			}

			// Populate Right Hand Module Data
			rightList = GetModules("RightPane");
			rightPane.DataBind();

			// Populate Content Pane Module Data
			contentList = GetModules("ContentPane");
			contentPane.DataBind();

			// Populate Left Hand Pane Module Data
			leftList = GetModules("LeftPane");
			leftPane.DataBind();
		}

		/// <summary>
		/// The GetModules helper method is used to get the modules
		/// for a single pane within the tab
		/// </summary>
		/// <param name="pane"></param>
		/// <returns></returns>
		private ArrayList GetModules (string pane) 
		{
			ArrayList paneModules = new ArrayList();
        
			foreach (ModuleSettings _module in portalSettings.ActivePage.Modules) 
			{
				if ((_module.PaneName).ToLower() == pane.ToLower() && this.portalSettings.ActivePage.PageID == _module.PageID )
				{
					ModuleItem m = new ModuleItem();
					m.Title = _module.ModuleTitle;
					m.ID = _module.ModuleID;
					m.ModuleDefID = _module.ModuleDefID;
					m.Order = _module.ModuleOrder;
					paneModules.Add(m);
				}
			}
        
			return paneModules;
		}

		/// <summary>
		/// The OrderModules helper method is used to reset the display
		/// order for modules within a pane
		/// </summary>
		/// <param name="list"></param>
		private void OrderModules (ArrayList list) 
		{
			int i = 1;
        
			// sort the arraylist
			list.Sort();
        
			// renumber the order
			foreach (ModuleItem m in list) 
			{
				// number the items 1, 3, 5, etc. to provide an empty order
				// number when moving items up and down in the list.
				m.Order = i;
				i += 2;
			}
		}

		private void EditTable_UpdateControl(object sender, SettingsTableEventArgs e)
		{
			PageSettings.UpdatePageSettings(PageID, e.CurrentItem.EditControl.ID, e.CurrentItem.Value);
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Raises the Init event.
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			//Confirm delete
			if (!(IsClientScriptBlockRegistered("confirmDelete")))
			{
				string[] s = {"CONFIRM_DELETE"};
				RegisterClientScriptBlock("confirmDelete", PortalSettings.GetStringResource("Rainbow.aspnet_client.Rainbow_scripts.confirmDelete.js", s));
			}

			LeftDeleteBtn.Attributes.Add("OnClick","return confirmDelete()");
			RightDeleteBtn.Attributes.Add("OnClick","return confirmDelete()");
			ContentDeleteBtn.Attributes.Add("OnClick","return confirmDelete()");

			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
			this.tabName.TextChanged += new EventHandler(this.PageSettings_Change);   
			this.authRoles.SelectedIndexChanged += new EventHandler(this.PageSettings_Change);   
			this.showMobile.CheckedChanged += new EventHandler(this.PageSettings_Change);   
			this.mobilePageName.TextChanged += new EventHandler(this.PageSettings_Change);   
			this.AddModuleBtn.Click += new EventHandler(this.AddModuleToPane_Click);   
			this.LeftUpBtn.Click += new ImageClickEventHandler(this.UpDown_Click);   
			this.LeftRightBtn.Click += new ImageClickEventHandler(this.RightLeft_Click);   
			this.LeftDownBtn.Click += new ImageClickEventHandler(this.UpDown_Click);   
			this.LeftEditBtn.Click += new ImageClickEventHandler(this.EditBtn_Click);   
			this.LeftDeleteBtn.Click += new ImageClickEventHandler(this.DeleteBtn_Click);   
			this.ContentUpBtn.Click += new ImageClickEventHandler(this.UpDown_Click);   
			this.ContentLeftBtn.Click += new ImageClickEventHandler(this.RightLeft_Click);   
			this.ContentRightBtn.Click += new ImageClickEventHandler(this.RightLeft_Click);   
			this.ContentDownBtn.Click += new ImageClickEventHandler(this.UpDown_Click);   
			this.ContentEditBtn.Click += new ImageClickEventHandler(this.EditBtn_Click);   
			this.ContentDeleteBtn.Click += new ImageClickEventHandler(this.DeleteBtn_Click);   
			this.RightUpBtn.Click += new ImageClickEventHandler(this.UpDown_Click);   
			this.RightLeftBtn.Click += new ImageClickEventHandler(this.RightLeft_Click);   
			this.RightDownBtn.Click += new ImageClickEventHandler(this.UpDown_Click);   
			this.RightEditBtn.Click += new ImageClickEventHandler(this.EditBtn_Click);   
			this.RightDeleteBtn.Click += new ImageClickEventHandler(this.DeleteBtn_Click);   
			this.EditTable.UpdateControl += new UpdateControlEventHandler(this.EditTable_UpdateControl);   
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion

	}
}
