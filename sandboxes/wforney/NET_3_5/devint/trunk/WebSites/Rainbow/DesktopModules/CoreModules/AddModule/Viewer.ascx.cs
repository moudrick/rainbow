using Path = Rainbow.Framework.Settings.Path;

namespace Rainbow.Content.Web.Modules.AddModule
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using Rainbow.Framework;
    using Rainbow.Framework.Data.MsSql;
    using Rainbow.Framework.Web.UI.WebControls;

    using Localize = Rainbow.Framework.Web.UI.WebControls.Localize;

    /// <summary>
    /// This module has been built by John Mandia (www.whitelightsolutions.com)
    ///     It allows administrators to give permission to selected roles to add modules to pages
    /// </summary>
    [History("jminond", "2006/03/25", "Converted to partial class")]
    [History("jminond", "2006/03/19", "Corrected adding module to root page for site")]
    [History("jminond", "2005/03/10", "Changes for moving Tab to Page")]
    public partial class Viewer : PortalModuleControl
    {
        #region Constants and Fields

        /// <summary>
        ///     Localized label for add module
        /// </summary>
        protected Localize Addmodule;

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
                return new Guid("{350CED6F-6739-43f3-8BF1-1D95187CA0BF}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent();

            // Create a new Title the control
            //    ModuleTitle = new DesktopModuleTitle();
            // Set here title properties
            // Add title ad the very beginning of
            // the control's controls collection
            //    Controls.AddAt(0, ModuleTitle);

            // Call base init procedure
            base.OnInit(e);
        }

        /// <summary>
        /// Each time the module selection is changed it checks to see if that particular module has a help file.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        /// <param name="e">
        /// E.
        /// </param>
        protected void moduleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetHelpPath();
            this.SetModuleName();
        }

        /*[Ajax.AjaxMethod]*/
        /*
        public System.Collections.Specialized.StringCollection ModuleChangeStrings(string moduleType, string moduleName)
        {
            SetDatata(moduleType);
            moduleTitle.Text = moduleName;

            System.Collections.Specialized.StringCollection s = new System.Collections.Specialized.StringCollection();

            s.Add(moduleTitle.Text);

            if (AddModuleHelp.Visible)
            {
                s.Add(AddModuleHelp.Attributes["onclick"].ToString());
                s.Add(AddModuleHelp.NavigateUrl);
                s.Add(AddModuleHelp.ImageUrl);
                s.Add(AddModuleHelp.ToolTip);
            }

            return s;
        }
         * */

        /// <summary>
        /// The AddModule_Click server event handler 
        ///     on this page is used to add a new portal module 
        ///     into the tab
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void AddModule_Click(object sender, EventArgs e)
        {
            // TODO: IF PAGE ID = 0 Then we know it's home page, cant we get from db the id?
            // PagesDB _d = new PagesDB();
            var pid = this.PageID;
            if (pid == 0)
            {
                pid = PagesDB.PortalHomePageID(this.PortalID);
            }

            if (pid != 0)
            {
                // All new modules go to the end of the contentpane
                var selectedModule = this.moduleType.SelectedItem.Value;
                var start = selectedModule.IndexOf("|");
                var moduleID = Convert.ToInt32(selectedModule.Substring(0, start).Trim());

                // Hide error message in case there was a previous error.
                this.moduleError.Visible = false;

                // This allows the user to pick what type of people can view the module being added.
                // If Authorised Roles is selected from the dropdown then every role that has view permission for the
                // Add Role module will be added to the view permissions of the module being added.
                var viewPermissionRoles = this.viewPermissions.SelectedValue;
                if (viewPermissionRoles == "Authorised Roles")
                {
                    viewPermissionRoles = PortalSecurity.GetViewPermissions(ModuleID);
                }

                try
                {
                    var m = new ModuleItem();
                    m.Title = this.moduleTitle.Text;
                    m.ModuleDefID = moduleID;
                    m.Order = 999;

                    // save to database
                    var mod = new ModulesDB();
                    m.ID = mod.AddModule(
                        pid, 
                        m.Order, 
                        this.paneLocation.SelectedValue, 
                        m.Title, 
                        m.ModuleDefID, 
                        0, 
                        PortalSecurity.GetEditPermissions(ModuleID), 
                        viewPermissionRoles, 
                        PortalSecurity.GetAddPermissions(ModuleID), 
                        PortalSecurity.GetDeletePermissions(ModuleID), 
                        PortalSecurity.GetPropertiesPermissions(ModuleID), 
                        PortalSecurity.GetMoveModulePermissions(ModuleID), 
                        PortalSecurity.GetDeleteModulePermissions(ModuleID), 
                        false, 
                        PortalSecurity.GetPublishPermissions(ModuleID), 
                        false, 
                        false, 
                        false);
                }
                catch (Exception ex)
                {
                    this.moduleError.Visible = true;
                    ErrorHandler.Publish(
                        LogLevel.Error, 
                        "There was an error with the Add Module Module while trying to add a new module.", 
                        ex);
                }
                finally
                {
                    if (this.moduleError.Visible == false)
                    {
                        // Reload page to pick up changes
                        this.Response.Redirect(this.Request.RawUrl, false);
                    }
                }
            }
            else
            {
                // moduleError.TextKey = "ADDMODULE_HOMEPAGEERROR";
                this.moduleError.Text = General.GetString(
                    "ADDMODULE_HOMEPAGEERROR", 
                    "You are currently on the homepage using the default virtual ID (The default ID is set when no specific page is selected. e.g. www.yourdomain.com. Please select your homepage from the Navigation menu e.g. 'Home' so that you can add a module against the page's actual ID.");
                this.moduleError.Visible = true;
            }
        }

        /// <summary>
        /// The BindData helper method is used to update the tab's
        ///     layout panes with the current configuration information
        /// </summary>
        private void BindData()
        {
            // Populate the "Add Module" Data
            var m = new ModulesDB();

            List<GeneralModuleDefinition> currentModuleDefinitions =
                m.GetCurrentModuleDefinitions(this.portalSettings.PortalID);

            // 				if(this.ArePropertiesEditable)
            // 				{
            // 					while(drCurrentModuleDefinitions.Read())
            // 					{
            // 						moduleType.Items.Add(new ListItem(drCurrentModuleDefinitions["FriendlyName"].ToString(),drCurrentModuleDefinitions["ModuleDefID"].ToString() + "|" + GetHelpPath(drCurrentModuleDefinitions["DesktopSrc"].ToString())));
            // 					}
            // 				}
            // 				else
            // 				{
            // Added by Mario Endara <mario@softworks.com.uy> 2004/11/04
            // only users members of the "Amins" role can add Admin modules to a Tab
            foreach (var gmd in currentModuleDefinitions.Where(gmd => PortalSecurity.IsInRoles("Admins") || !gmd.Admin))
            {
                this.moduleType.Items.Add(
                    new ListItem(gmd.FriendlyName, string.Format("{0}|{1}", gmd.GeneralModDefID, GetHelpPath(gmd.DesktopSource))));
            }

            // 				}
        }

        /// <summary>
        /// Gets the folder help path.
        /// </summary>
        /// <param name="desktopSrc">
        /// Desktop SRC.
        /// </param>
        /// <returns>
        /// The name of the help folder in the correct format
        /// </returns>
        private static string GetHelpPath(string desktopSrc)
        {
            var helpPath = desktopSrc.Replace(".", "_");
            return string.Format("Rainbow/{0}", helpPath);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            this.moduleType.SelectedIndexChanged += this.moduleType_SelectedIndexChanged;
            this.AddModuleBtn.Click += new EventHandler(this.AddModule_Click);
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
            this.SetHelpPath();
            this.SetModuleName();
        }

        /// <summary>
        /// The set datata.
        /// </summary>
        /// <param name="modulePath">
        /// The module path.
        /// </param>
        private void SetDatata(string modulePath)
        {
            var folderName = modulePath;
            var start = folderName.IndexOf("|");
            folderName = folderName.Substring(start + 1);
            var fileNameStart = folderName.LastIndexOf("/");
            var fileName = folderName.Substring(fileNameStart + 1);
            var completePath = string.Format("{0}/{1}", folderName, fileName);

            if (
                File.Exists(
                    HttpContext.Current.Server.MapPath(
                        string.Format("{0}/rb_documentation/{1}.xml", Path.ApplicationRoot, completePath))))
            {
                this.AddModuleHelp.Visible = true;
                var javaScript = string.Format("HelpWindow=window.open('{0}/rb_documentation/Viewer.aspx?loc={1}&src={2}','HelpWindow','toolbar=no,location=no,directories=no,status=no,menubar=yes,scrollbars=yes,resizable=yes,width=640,height=480,left=15,top=15'); return false;", Path.ApplicationRoot, folderName, fileName);
                this.AddModuleHelp.Attributes.Add("onclick", javaScript);
                this.AddModuleHelp.Attributes.Add("style", "cursor: hand;");
                this.AddModuleHelp.NavigateUrl = string.Empty;
                this.AddModuleHelp.ImageUrl = this.CurrentTheme.GetImage("Buttons_Help", "Help.gif").ImageUrl;
                this.AddModuleHelp.ToolTip = string.Format("{0} Help", this.moduleType.SelectedItem.Text);
            }
            else
            {
                this.AddModuleHelp.Visible = false;
            }
        }

        /// <summary>
        /// Sets the help path. This method checks to see whether the currently selected module has a
        ///     help file associated with it. If it does then it shows the help icon. If it doesn't then it
        ///     hides it.
        /// </summary>
        private void SetHelpPath()
        {
            this.SetDatata(this.moduleType.SelectedValue);
        }

        /// <summary>
        /// The set module name.
        /// </summary>
        private void SetModuleName()
        {
            // by Manu, set title like module name
            if (this.moduleType.SelectedItem != null)
            {
                this.moduleTitle.Text = this.moduleType.SelectedItem.Text;
            }
        }

        #endregion
    }
}