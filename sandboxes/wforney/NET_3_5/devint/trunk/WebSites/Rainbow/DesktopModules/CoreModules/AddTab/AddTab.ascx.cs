namespace Rainbow.Content.Web.Modules.AddModule
{
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using Rainbow.Framework;
    using Rainbow.Framework.Data.Providers;
    using Rainbow.Framework.Providers.RainbowSiteMapProvider;
    using Rainbow.Framework.Web.UI.WebControls;

    using Label = System.Web.UI.WebControls.Label;
    using LinkButton = System.Web.UI.WebControls.LinkButton;
    using Localize = Rainbow.Framework.Web.UI.WebControls.Localize;

    /// <summary>
    /// This module has been built by John Mandia (www.whitelightsolutions.com)
    ///     It allows administrators to give permission to selected roles to add modules to pages
    /// </summary>
    [History("jminond", "12/5/2005", "Added DropDownList to combos")]
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    public class AddPage : PortalModuleControl
    {
        #region Constants and Fields

        /// <summary>
        /// The add tab button.
        /// </summary>
        protected LinkButton AddTabButton;

        /// <summary>
        /// The literal 1.
        /// </summary>
        protected Localize Literal1;

        /// <summary>
        /// The permission drop down.
        /// </summary>
        protected DropDownList PermissionDropDown;

        /// <summary>
        /// The tab title text box.
        /// </summary>
        protected TextBox TabTitleTextBox;

        /// <summary>
        /// The cb_ show mobile.
        /// </summary>
        protected CheckBox cb_ShowMobile;

        /// <summary>
        /// The lbl error not allowed.
        /// </summary>
        protected Label lblErrorNotAllowed;

        /// <summary>
        /// The lbl_ mobile tab name.
        /// </summary>
        protected Localize lbl_MobileTabName;

        /// <summary>
        /// The lbl_ show mobile.
        /// </summary>
        protected Localize lbl_ShowMobile;

        /// <summary>
        /// The module error.
        /// </summary>
        protected Localize moduleError;

        /// <summary>
        /// The parent tab drop down.
        /// </summary>
        protected DropDownList parentTabDropDown;

        /// <summary>
        /// The rbl_ jump to tab.
        /// </summary>
        protected RadioButtonList rbl_JumpToTab;

        /// <summary>
        /// The tab parent label.
        /// </summary>
        protected Localize tabParentLabel;

        /// <summary>
        /// The tab title label.
        /// </summary>
        protected Localize tabTitleLabel;

        /// <summary>
        /// The tab visible label.
        /// </summary>
        protected Localize tabVisibleLabel;

        /// <summary>
        /// The tb_ mobile tab name.
        /// </summary>
        protected TextBox tb_MobileTabName;

        #endregion

        #region Properties

        /// <summary>
        ///     Marks This Module To Be An Admin Module
        /// </summary>
        public override bool AdminModule
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets the GUID for this module.
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{A1E37A0F-4EE9-4b83-9482-43466FC21E08}");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind data and load miscellanous data
        /// </summary>
        public void LoadAddPageModule()
        {
            this.BindData();
            this.TabTitleTextBox.Text = General.GetString("TAB_NAME", "New Tab Name");
            this.AddTabButton.Click += this.AddTabButton_Click;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.PermissionDropDown.Items.Add("All Users");
            this.PermissionDropDown.Items.Add("Authenticated Users");
            this.PermissionDropDown.Items.Add("Unauthenticated Users");
            this.PermissionDropDown.Items.Add("Authorised Roles");
            this.PermissionDropDown.SelectedIndex = 1;

            this.AddTabButton.Click += this.AddTabButton_Click;
            this.Load += this.Page_Load;

            // Call base init procedure
            base.OnInit(e);
        }

        /// <summary>
        /// The AddTabButton_Click server event handler 
        ///     on this page is used to add a new portal module 
        ///     into the tab
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void AddTabButton_Click(object sender, EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            // Hide error message in case there was a previous error.
            this.moduleError.Visible = false;

            // This allows the user to pick what type of people can view the module being added.
            // If Authorised Roles is selected from the dropdown then every role that has view permission for the
            // Add Role module will be added to the view permissions of the module being added.
            var viewPermissionRoles = this.PermissionDropDown.SelectedValue;
            if (viewPermissionRoles == "Authorised Roles")
            {
                viewPermissionRoles = PortalSecurity.GetViewPermissions(ModuleID);
            }

            try
            {
                // New tabs go to the end of the list
                var t = new PageItem();
                t.Name = this.TabTitleTextBox.Text;
                t.ID = -1;
                t.Order = 990000;

                // Get Parent Tab Id Convert only once used many times
                var parentTabId = int.Parse(this.parentTabDropDown.SelectedValue);

                // write tab to database
                var tabs = new PagesDB();

                // t.ID = tabs.AddTab(portalSettings.PortalID, t.Name, viewPermissionRoles, t.Order);

                // Changed to use new method in TabsDB.cs now all parms are possible 
                // By Mike Stone (mstone@kaskaskia.edu) - 30/12/2004
                t.ID = tabs.AddPage(
                    this.portalSettings.PortalID, 
                    parentTabId, 
                    t.Name, 
                    t.Order, 
                    viewPermissionRoles, 
                    this.cb_ShowMobile.Checked, 
                    this.tb_MobileTabName.Text);

                // TODO.. the only way to update a parent id is throught update :S
                // Changed to AddTab method now supports the parm
                // Mike Stone - 30/12/2004
                // tabs.UpdateTab(portalSettings.PortalID, t.ID, parentTabID, t.Name, t.Order, viewPermissionRoles, t.Name, false);

                // Invalidate cache
                // Changed to access form directly 
                // mike stone - 30/12/2004
                // Cache.Remove(Rainbow.Framework.Settings.Cache.Key.TabSettings(parentTabID));
                // Copied to here 29/12/2004 by Mike Stone
                CurrentCache.RemoveAll("_TabNavigationSettings_");

                // Debug.WriteLine("************* Remove " + Key.TabSettings(parentTabID));

                // Clear SiteMaps Cache
                RainbowSiteMapProvider.ClearAllRainbowSiteMapCaches();

                // Jump to Page option
                string returnTab;
                if (this.rbl_JumpToTab.SelectedValue == "Yes")
                {
                    // Redirect to New Page/Tab - Mike Stone 30/12/2004
                    // modified by Hongwei Shen 9/25/2005
                    // returnTab = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", t.ID, "SelectedTabID=" + t.ID.ToString());
                    var newPage = string.Format("~/{0}.aspx", t.Name.Trim().Replace(" ", "_"));
                    returnTab = HttpUrlBuilder.BuildUrl(newPage, t.ID);
                }
                else
                {
                    // Do NOT Redirect to New Form - Mike Stone 30/12/2004
                    // I guess every .aspx page needs to have a module tied to it. 
                    // or you will get an error about edit access denied.

                    // Modified by Hongwei Shen 9/25/2005 to fix: QueryString["tabID"] maybe null.
                    // returnTab = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", int.Parse(Request.QueryString["tabID"]), "SelectedTabID=" + t.ID.ToString());
                    returnTab = HttpUrlBuilder.BuildUrl(
                        "~/DesktopDefault.aspx", this.PageID, "SelectedTabID=" + t.ID.ToString());
                }

                this.Response.Redirect(returnTab);
            }
            catch (Exception ex)
            {
                this.moduleError.Visible = true;
                ErrorHandler.Publish(
                    LogLevel.Error, "There was an error with the Add Tab Module while trying to add a new tab.", ex);
                return;
            }

            // Reload page to pick up changes
            this.Response.Redirect(this.Request.RawUrl, false);
        }

        /// <summary>
        /// The BindData helper method is used to update the tab's
        ///     layout panes with the current configuration information
        /// </summary>
        private void BindData()
        {
            // Populate the "ParentTab" Data
            var dtPages = new PagesDB().GetPagesFlatTable(this.portalSettings.PortalID);
            var keys = new DataColumn[2];
            keys[0] = dtPages.Columns["PageID"];
            dtPages.PrimaryKey = keys;

            this.parentTabDropDown.DataSource = dtPages;

            // parentTabDropDown.DataValueField = "PageID";
            // parentTabDropDown.DataTextField = "PageOrder";
            this.parentTabDropDown.DataBind();
            this.parentTabDropDown.Items.Insert(1, "Root");

            // dt_Pages = null;

            // Preselects current tab as parent
            // Changes for Grischa Brockhaus copied by Mike Stone 7/1/2005
            if (this.parentTabDropDown.SelectedIndex <= 0)
            {
                int currentTab = this.portalSettings.ActivePage.PageID;
                this.parentTabDropDown.SelectedValue = currentTab.ToString();
            }

            // parentTabDropDown.Items.FindByValue(currentTab .ToString()).Selected = true; 

            // Translate
            // parentTabDropDown.Item(0).Text =General.GetString("ROOT_LEVEL", "Root Level", parentTabDropDown);
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // If first visit to the page, update all entries
            if (this.Page.IsPostBack)
            {
                return;
            }

            this.BindData();
            this.TabTitleTextBox.Text = General.GetString("TAB_NAME", "New Tab Name");
        }

        #endregion
    }
}