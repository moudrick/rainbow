using System;
using System.Collections;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using HyperLink = Esperantus.WebControls.HyperLink;
using LinkButton = Esperantus.WebControls.LinkButton;
using Literal = Esperantus.WebControls.Literal;
using Page = Rainbow.UI.Page;

namespace Rainbow.Admin
{
	/// <summary>
	/// Use this page to modify title and permission on portal modules<br />
	/// The ModuleSettings.aspx page is used to enable administrators to view/edit/update
	/// a portal module's settings (title, output cache properties, edit access)
	/// </summary>
	[History("jviladiu@portalServices.net", "2004/08/19", "Added authMoveModuleRoles & authDeleteModuleRoles propertys")]
	[History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
	[History("Jes1111", "2003/04/24", "Added Cacheable property")]
	public class ModuleSettingsPage : EditItemPage
	{
		protected System.Web.UI.WebControls.Label moduleType;
		protected TextBox moduleTitle;
		protected TextBox cacheTime;
		protected CheckBoxList authEditRoles;
		protected CheckBoxList authViewRoles;
		protected CheckBoxList authAddRoles;
		protected CheckBoxList authPropertiesRoles;

		protected CheckBoxList authMoveModuleRoles;
		protected CheckBoxList authDeleteModuleRoles;

		protected CheckBox ShowMobile;

		// Changed Section By Geert.Audenaert@Syntegra.Com
		// Date: 6/2/2003
		protected CheckBoxList authPublishingRoles;
		protected CheckBoxList authApproveRoles;
		protected DropDownList tabDropDownList;
		protected CheckBox enableWorkflowSupport;
		// End Change Geert.Audenaert@Syntegra.Com

		protected ADGroupMember memEditRoles;
		protected ADGroupMember memViewRoles;
		protected ADGroupMember memAddRoles;
		protected ADGroupMember memDeleteRoles;
		protected ADGroupMember memPropertiesRoles;
		protected ADGroupMember memMoveModuleRoles;
		protected ADGroupMember memDeleteModuleRoles;
		protected ADGroupMember memPublishingRoles;
		protected ADGroupMember memApproveRoles;

		protected Literal Literal1;
		protected Literal Literal2;
		protected Literal Literal3;
		protected Literal Literal4;
		protected Literal Literal5;
		protected Literal Literal6;
		protected Literal Literal7;
		protected Literal Literal8;
		protected Literal Literal9;
		protected Literal Literal10;
		protected Literal Literal11;
		protected Literal Literal12;
		protected Literal Literal13;
		protected Literal Literal14;
		protected Literal Literal15;
		protected Literal Literal16;
		protected Literal Literal17;

		protected HyperLink moduleSettingsButton;
		protected LinkButton saveAndCloseButton;
		protected PlaceHolder PlaceHolderButtons;
		protected CheckBox showEveryWhere;

		// [START] Changed Section By bja@reedtek.com
		// Date: 16/5/2003
		protected CheckBox allowCollapsable;
		protected CheckBoxList authDeleteRoles;
		protected Literal Literal18;
		protected PlaceHolder PlaceholderButtons2;
		// [END] - bja@reedtek.com 


		protected ArrayList portalTabs;

		/// <summary>
		/// The Page_Load server event handler on this page is used
		/// to populate the module settings on the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindData();
		}

		/// <summary>
		/// The ApplyChanges_Click server event handler on this page is used
		/// to save the module settings into the portal configuration system.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnUpdate(EventArgs e)
		{
			base.OnUpdate(e);
			bool useNTLM = HttpContext.Current.User is WindowsPrincipal;

			// add by Jonathan Fong 22/07/2004 to support LDAP
			// jes1111 - useNTLM |= ConfigurationSettings.AppSettings["LDAPLogin"] != null ? true : false;
			useNTLM |= Config.LDAPLogin.Length != 0 ? true : false;

			object value = GetModule();
			if (value != null)
			{
				ModuleSettings m = (ModuleSettings) value;

				// Construct Authorized User Roles string

				// Edit Roles
				string editRoles = string.Empty;
				if (useNTLM)
					editRoles = memEditRoles.Members;
				else
					foreach (ListItem editItem in authEditRoles.Items)
					{
						if (editItem.Selected == true)
						{
							editRoles = editRoles + editItem.Text + ";";
						}
					}

				// View Roles
				string viewRoles = string.Empty;
				if (useNTLM)
					viewRoles = memViewRoles.Members;
				else
					foreach (ListItem viewItem in authViewRoles.Items)
					{
						if (viewItem.Selected == true)
						{
							viewRoles = viewRoles + viewItem.Text + ";";
						}
					}

				// Add Roles
				string addRoles = string.Empty;
				if (useNTLM)
					addRoles = memAddRoles.Members;
				else
					foreach (ListItem addItem in authAddRoles.Items)
					{
						if (addItem.Selected == true)
						{
							addRoles = addRoles + addItem.Text + ";";
						}
					}

				// Delete Roles
				string deleteRoles = string.Empty;
				if (useNTLM)
					deleteRoles = memDeleteRoles.Members;
				else
					foreach (ListItem deleteItem in authDeleteRoles.Items)
					{
						if (deleteItem.Selected == true)
						{
							deleteRoles = deleteRoles + deleteItem.Text + ";";
						}
					}

				// Move Module Roles
				string moveModuleRoles = string.Empty;
				if (useNTLM)
					moveModuleRoles = memMoveModuleRoles.Members;
				else
					foreach (ListItem li in authMoveModuleRoles.Items)
					{
						if (li.Selected == true)
						{
							moveModuleRoles += li.Text + ";";
						}
					}

				// Delete Module Roles
				string deleteModuleRoles = string.Empty;
				if (useNTLM)
					deleteModuleRoles = memDeleteModuleRoles.Members;
				else
					foreach (ListItem li in authDeleteModuleRoles.Items)
					{
						if (li.Selected == true)
						{
							deleteModuleRoles += li.Text + ";";
						}
					}

				// Properties Roles
				string PropertiesRoles = string.Empty;
				if (useNTLM)
					PropertiesRoles = memPropertiesRoles.Members;
				else
					foreach (ListItem PropertiesItem in authPropertiesRoles.Items)
					{
						if (PropertiesItem.Selected == true)
						{
							PropertiesRoles = PropertiesRoles + PropertiesItem.Text + ";";
						}
					}

				// Change by Geert.Audenaert@Syntegra.Com
				// Date: 6/2/2003
				// Publishing Roles
				string PublishingRoles = string.Empty;
				if (useNTLM)
					PublishingRoles = memPublishingRoles.Members;
				else
					foreach (ListItem PropertiesItem in authPublishingRoles.Items)
					{
						if (PropertiesItem.Selected == true)
						{
							PublishingRoles = PublishingRoles + PropertiesItem.Text + ";";
						}
					}
				// End Change Geert.Audenaert@Syntegra.Com
				// Change by Geert.Audenaert@Syntegra.Com
				// Date: 27/2/2003
				string ApprovalRoles = string.Empty;
				if (useNTLM)
					ApprovalRoles = memApproveRoles.Members;
				else
					foreach (ListItem PropertiesItem in authApproveRoles.Items)
					{
						if (PropertiesItem.Selected == true)
						{
							ApprovalRoles = ApprovalRoles + PropertiesItem.Text + ";";
						}
					}

				// End Change Geert.Audenaert@Syntegra.Com

				// update module
				ModulesDB modules = new ModulesDB();
				modules.UpdateModule(Int32.Parse(tabDropDownList.SelectedItem.Value), ModuleID, m.ModuleOrder, m.PaneName, moduleTitle.Text, Int32.Parse(cacheTime.Text), editRoles, viewRoles, addRoles, deleteRoles, PropertiesRoles, moveModuleRoles, deleteModuleRoles, ShowMobile.Checked, PublishingRoles, enableWorkflowSupport.Checked, ApprovalRoles, showEveryWhere.Checked, allowCollapsable.Checked);
			}
		}

		private void saveAndCloseButton_Click(object sender, EventArgs e)
		{
			OnUpdate(e);
			// Navigate back to admin page
			if (Page.IsValid == true)
				RedirectBackToReferringPage();
		}

		//Used to populate all checklist roles with Roles in portal
		private void populateRoles(ref CheckBoxList listRoles, string moduleRoles)
		{
			//Get roles from db
			UsersDB users = new UsersDB();
			SqlDataReader roles = users.GetPortalRoles(portalSettings.PortalID);

			// Clear existing items in checkboxlist
			listRoles.Items.Clear();

			//All Users
			ListItem allItem = new ListItem("All Users");
			listRoles.Items.Add(allItem);

			// Authenticated user role added 15 nov 2002 - by manudea
			ListItem authItem = new ListItem("Authenticated Users");
			listRoles.Items.Add(authItem);

			// Unauthenticated user role added 30/01/2003 - by manudea
			ListItem unauthItem = new ListItem("Unauthenticated Users");
			listRoles.Items.Add(unauthItem);

			//Add roles from DB
			while (roles.Read())
			{
				ListItem item = new ListItem();
				item.Text = (string) roles["RoleName"];
				item.Value = roles["RoleID"].ToString();
				listRoles.Items.Add(item);
			}
			roles.Close(); //by Manu, fixed bug 807858

			//Splits up the role string and use array 30/01/2003 - by manudea
			while (moduleRoles.EndsWith(";"))
				moduleRoles = moduleRoles.Substring(0, moduleRoles.Length - 1);
			string[] arrModuleRoles = moduleRoles.Split(';');
			int roleCount = arrModuleRoles.GetUpperBound(0);

			//Cycle every role and select it if needed
			foreach (ListItem ls in listRoles.Items)
			{
				for (int i = 0; i <= roleCount; i++)
				{
					if (arrModuleRoles[i].ToLower() == ls.Text.ToLower())
						ls.Selected = true;
				}
			}
		}

		private string giveMeFriendlyName(Guid guid)
		{
			string friendlyName = string.Empty;
			using (SqlDataReader auxDr = new ModulesDB().GetSingleModuleDefinition(guid))
			{
				if (auxDr.Read())
					friendlyName = auxDr["FriendlyName"] as string;
			}
			return friendlyName;
		}

		/// <summary>
		/// The BindData helper method is used to populate a asp:datalist
		/// server control with the current "edit access" permissions
		/// set within the portal configuration system
		/// </summary>
		private void BindData()
		{
			bool useNTLM = HttpContext.Current.User is WindowsPrincipal;
			// add by Jonathan Fong 22/07/2004 to support LDAP
			// jes1111 - useNTLM |= ConfigurationSettings.AppSettings["LDAPLogin"] != null ? true : false;
			useNTLM |= Config.LDAPLogin.Length != 0 ? true : false;

			authAddRoles.Visible = authApproveRoles.Visible = authDeleteRoles.Visible =
				authEditRoles.Visible = authPropertiesRoles.Visible = authPublishingRoles.Visible =
					authMoveModuleRoles.Visible = authDeleteModuleRoles.Visible =
						authViewRoles.Visible = !useNTLM;
			memAddRoles.Visible = memApproveRoles.Visible = memDeleteRoles.Visible =
				memEditRoles.Visible = memPropertiesRoles.Visible = memPublishingRoles.Visible =
					memMoveModuleRoles.Visible = memDeleteModuleRoles.Visible =
						memViewRoles.Visible = useNTLM;
			object value = GetModule();
			if (value != null)
			{
				ModuleSettings m = (ModuleSettings) value;

				moduleType.Text = giveMeFriendlyName(m.GuidID);

				// Update Textbox Settings
				moduleTitle.Text = m.ModuleTitle;
				cacheTime.Text = m.CacheTime.ToString();

				portalTabs = new PagesDB().GetPagesFlat(portalSettings.PortalID);
				tabDropDownList.DataBind();
				tabDropDownList.ClearSelection();
				if (tabDropDownList.Items.FindByValue(m.PageID.ToString()) != null)
					tabDropDownList.Items.FindByValue(m.PageID.ToString()).Selected = true;

				// Change by John.Mandia@whitelightsolutions.com
				//Date: 19/5/2003
				showEveryWhere.Checked = m.ShowEveryWhere;

				// is the window mgmt support enabled
				// jes1111 - allowCollapsable.Enabled = GlobalResources.SupportWindowMgmt;
				allowCollapsable.Enabled = Config.WindowMgmtControls;
				allowCollapsable.Checked = m.SupportCollapsable;

				ShowMobile.Checked = m.ShowMobile;
				// Change by Geert.Audenaert@Syntegra.Com
				// Date: 6/2/2003
				PortalModuleControl pm;
				string controlPath;
				controlPath = Path.WebPathCombine(Path.ApplicationRoot, m.DesktopSrc);

				try
				{
					pm = (PortalModuleControl) LoadControl(controlPath);
					if (pm.InnerSupportsWorkflow)
					{
						enableWorkflowSupport.Checked = m.SupportWorkflow;
						if (useNTLM)
						{
							memApproveRoles.Enabled = m.SupportWorkflow;
							memPublishingRoles.Enabled = m.SupportWorkflow;
							memApproveRoles.Members = m.AuthorizedApproveRoles;
							memPublishingRoles.Members = m.AuthorizedPublishingRoles;
						}
						else
						{
							authApproveRoles.Enabled = m.SupportWorkflow;
							authPublishingRoles.Enabled = m.SupportWorkflow;
							populateRoles(ref authPublishingRoles, m.AuthorizedPublishingRoles);
							populateRoles(ref authApproveRoles, m.AuthorizedApproveRoles);
						}
					}
					else
					{
						enableWorkflowSupport.Enabled = false;
						if (useNTLM)
						{
							memApproveRoles.Enabled = false;
							memPublishingRoles.Enabled = false;
						}
						else
						{
							authApproveRoles.Enabled = false;
							authPublishingRoles.Enabled = false;
						}
					}
				}
				catch (Exception ex)
				{
					//ErrorHandler.HandleException("There was a problem loading: '" + controlPath + "'", ex);
					//throw;
					throw new RainbowException(LogLevel.Error, "There was a problem loading: '" + controlPath + "'", ex);
				}


				// End Change Geert.Audenaert@Syntegra.Com

				// Populate checkbox list with all security roles for this portal
				// and "check" the ones already configured for this module
				if (useNTLM)
				{
					memEditRoles.Members = m.AuthorizedEditRoles;
					memViewRoles.Members = m.AuthorizedViewRoles;
					memAddRoles.Members = m.AuthorizedAddRoles;
					memDeleteRoles.Members = m.AuthorizedDeleteRoles;
					memMoveModuleRoles.Members = m.AuthorizedMoveModuleRoles;
					memDeleteModuleRoles.Members = m.AuthorizedDeleteModuleRoles;
					memPropertiesRoles.Members = m.AuthorizedPropertiesRoles;
				}
				else
				{
					populateRoles(ref authEditRoles, m.AuthorizedEditRoles);
					populateRoles(ref authViewRoles, m.AuthorizedViewRoles);
					populateRoles(ref authAddRoles, m.AuthorizedAddRoles);
					populateRoles(ref authDeleteRoles, m.AuthorizedDeleteRoles);
					populateRoles(ref authMoveModuleRoles, m.AuthorizedMoveModuleRoles);
					populateRoles(ref authDeleteModuleRoles, m.AuthorizedDeleteModuleRoles);
					populateRoles(ref authPropertiesRoles, m.AuthorizedPropertiesRoles);
				}

				// Jes1111
				if (!pm.Cacheable)
				{
					cacheTime.Text = "-1";
					cacheTime.Enabled = false;
				}
			}
			else // Denied access if Module not in Tab. jviladiu@portalServices.net (2004/07/23)
				PortalSecurity.AccessDenied();
		}

		private ModuleSettings GetModule()
		{
			// Obtain selected module data
			foreach (ModuleSettings _module in portalSettings.ActivePage.Modules)
			{
				if (_module.ModuleID == ModuleID)
					return _module;
			}
			return null;
		}

		#region Web Form Designer generated code

		/// <summary>
		/// Raises the Init event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.PlaceHolderButtons.EnableViewState = false;
			this.PlaceholderButtons2.EnableViewState = false;

			//Controls must be created here
			updateButton = new LinkButton();
			updateButton.CssClass = "CommandButton";
			PlaceHolderButtons.Controls.Add(updateButton);


			// jminond added to top of property page so no need to scroll for save
			LinkButton update2 = new LinkButton();
			update2.CssClass = "CommandButton";
			update2.TextKey = "Apply";
			update2.Text = "Apply";
			update2.Click += new EventHandler(UpdateButton_Click);
			PlaceholderButtons2.Controls.Add(update2);

			PlaceHolderButtons.Controls.Add(new LiteralControl("&nbsp;"));
			PlaceholderButtons2.Controls.Add(new LiteralControl("&nbsp;"));

			saveAndCloseButton = new LinkButton();
			saveAndCloseButton.TextKey = "OK";
			saveAndCloseButton.Text = "Save and close";
			saveAndCloseButton.CssClass = "CommandButton";
			PlaceHolderButtons.Controls.Add(saveAndCloseButton);
			this.saveAndCloseButton.Click += new EventHandler(this.saveAndCloseButton_Click);


			// jminond added to top of property page so no need to scroll for save
			LinkButton saveAndCloseButton2 = new LinkButton();
			saveAndCloseButton2.TextKey = "OK";
			saveAndCloseButton2.Text = "Save and close";
			saveAndCloseButton2.CssClass = "CommandButton";
			PlaceholderButtons2.Controls.Add(saveAndCloseButton2);
			saveAndCloseButton2.Click += new EventHandler(this.saveAndCloseButton_Click);


			PlaceHolderButtons.Controls.Add(new LiteralControl("&nbsp;"));
			PlaceholderButtons2.Controls.Add(new LiteralControl("&nbsp;"));

			moduleSettingsButton = new HyperLink();
			moduleSettingsButton.TextKey = "MODULESETTINGS_SETTINGS";
			moduleSettingsButton.Text = "Settings";
			moduleSettingsButton.CssClass = "CommandButton";
			moduleSettingsButton.NavigateUrl = HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/PropertyPage.aspx", PageID, ModuleID);
			PlaceHolderButtons.Controls.Add(moduleSettingsButton);

			// jminond added to top of property page so no need to scroll for save
			HyperLink moduleSettingsButton2 = new HyperLink();
			moduleSettingsButton2.TextKey = "MODULESETTINGS_SETTINGS";
			moduleSettingsButton2.Text = "Settings";
			moduleSettingsButton2.CssClass = "CommandButton";
			moduleSettingsButton2.NavigateUrl = HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/PropertyPage.aspx", PageID, ModuleID);
			PlaceholderButtons2.Controls.Add(moduleSettingsButton2);

			PlaceHolderButtons.Controls.Add(new LiteralControl("&nbsp;"));
			PlaceholderButtons2.Controls.Add(new LiteralControl("&nbsp;"));

			cancelButton = new LinkButton();
			cancelButton.CssClass = "CommandButton";
			PlaceHolderButtons.Controls.Add(cancelButton);

			// jminond added to top of property page so no need to scroll for save
			LinkButton cancel2 = new LinkButton();
			cancel2.CssClass = "CommandButton";
			cancel2.TextKey = "Cancel";
			cancel2.Text = "Cancel";
			cancel2.Click += new EventHandler(CancelButton_Click);
			PlaceholderButtons2.Controls.Add(cancel2);

//			if (((Page) this.Page).IsCssFileRegistered("tabsControl") == false)
//			{
//				string themePath = Path.WebPathCombine(this.CurrentTheme.WebPath, "/tabControl.css");
//				((Page) this.Page).RegisterCssFile("tabsControl", themePath);
//			}
			if(!((UI.Page)this.Page).IsCssFileRegistered("TabControl"))
				((UI.Page)this.Page).RegisterCssFile("TabControl");

			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.enableWorkflowSupport.CheckedChanged += new EventHandler(this.enableWorkflowSupport_CheckedChanged);
			this.Load += new EventHandler(this.Page_Load);

		}

		#endregion

		private void enableWorkflowSupport_CheckedChanged(object sender, EventArgs e)
		{
			bool useNTLM = HttpContext.Current.User is WindowsPrincipal;
			// add by Jonathan Fong 22/07/2004 to support LDAP
			// jes1111 - useNTLM |= ConfigurationSettings.AppSettings["LDAPLogin"] != null ? true : false;
			useNTLM |= Config.LDAPLogin.Length != 0 ? true : false;

			if (useNTLM)
			{
				memApproveRoles.Enabled = enableWorkflowSupport.Checked;
				memPublishingRoles.Enabled = enableWorkflowSupport.Checked;
			}
			else
			{
				authApproveRoles.Enabled = enableWorkflowSupport.Checked;
				authPublishingRoles.Enabled = enableWorkflowSupport.Checked;
			}
		}

		private void UpdateButton_Click(object sender, EventArgs e)
		{
			this.OnUpdate(e);
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.OnCancel(e);
		}

//		private void DeleteButton_Click(object sender, EventArgs e)
//		{
//			this.OnDelete(e);
//		}

		private ArrayList _allowedModules = null;

		/// <summary>
		/// Only can use this page from tab with original module
		/// jviladiu@portalServices.net (2004/07/22)
		/// </summary>
		protected override ArrayList AllowedModules
		{
			get
			{
				if (_allowedModules == null)
				{
					ModulesDB mdb = new ModulesDB();
					ArrayList al = new ArrayList();
					al.Add(mdb.GetModuleGuid(ModuleID).ToString());
					_allowedModules = al;
				}
				return _allowedModules;
			}
		}

	}
}