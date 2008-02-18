using System;
using System.Collections;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rainbow.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Security;
using Rainbow.Framework.Site.Data;
using Rainbow.Framework.Web.UI.WebControls;
using HyperLink=Rainbow.Framework.Web.UI.WebControls.HyperLink;
using LinkButton=Rainbow.Framework.Web.UI.WebControls.LinkButton;
using System.Collections.Generic;

namespace Rainbow.Admin 
{
    /// <summary>
    /// Use this page to modify title and permission on portal modules<br />
    /// The ModuleSettings.aspx page is used to enable administrators to view/edit/update
    /// a portal module's settings (title, output cache properties, edit access)
    /// </summary>
    [History( "jviladiu@portalServices.net", "2004/08/19", "Added authMoveModuleRoles & authDeleteModuleRoles propertys" )]
    [History( "Jes1111", "2003/03/04", "Cache flushing now handled by inherited page" )]
    [History( "Jes1111", "2003/04/24", "Added Cacheable property" )]
    public partial class ModuleSettingsPage : Framework.Web.UI.EditItemPage
    {
        protected HyperLink moduleSettingsButton;
        protected LinkButton saveAndCloseButton;
        protected ArrayList portalTabs;

        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to populate the module settings on the page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
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
                RainbowModule m = (RainbowModule) value;

                // Construct Authorized User Roles string

                // Edit Roles
                string editRoles = string.Empty;
                foreach (ListItem editItem in authEditRoles.Items)
                {
                    if (editItem.Selected)
                    {
                        editRoles = editRoles + editItem.Text + ";";
                    }
                }

                // View Roles
                string viewRoles = string.Empty;
                foreach (ListItem viewItem in authViewRoles.Items)
                {
                    if (viewItem.Selected)
                    {
                        viewRoles = viewRoles + viewItem.Text + ";";
                    }
                }

                // Add Roles
                string addRoles = string.Empty;
                foreach (ListItem addItem in authAddRoles.Items)
                {
                    if (addItem.Selected)
                    {
                        addRoles = addRoles + addItem.Text + ";";
                    }
                }

                // Delete Roles
                string deleteRoles = string.Empty;
                foreach (ListItem deleteItem in authDeleteRoles.Items)
                {
                    if (deleteItem.Selected)
                    {
                        deleteRoles = deleteRoles + deleteItem.Text + ";";
                    }
                }

                // Move Module Roles
                string moveModuleRoles = string.Empty;
                foreach (ListItem li in authMoveModuleRoles.Items)
                {
                    if (li.Selected)
                    {
                        moveModuleRoles += li.Text + ";";
                    }
                }

                // Delete Module Roles
                string deleteModuleRoles = string.Empty;
                foreach (ListItem li in authDeleteModuleRoles.Items)
                {
                    if (li.Selected)
                    {
                        deleteModuleRoles += li.Text + ";";
                    }
                }

                // Properties Roles
                string PropertiesRoles = string.Empty;
                foreach (ListItem PropertiesItem in authPropertiesRoles.Items)
                {
                    if (PropertiesItem.Selected)
                    {
                        PropertiesRoles = PropertiesRoles + PropertiesItem.Text + ";";
                    }
                }

                // Change by Geert.Audenaert@Syntegra.Com
                // Date: 6/2/2003
                // Publishing Roles
                string PublishingRoles = string.Empty;
                foreach (ListItem PropertiesItem in authPublishingRoles.Items)
                {
                    if (PropertiesItem.Selected)
                    {
                        PublishingRoles = PublishingRoles + PropertiesItem.Text + ";";
                    }
                }
                // End Change Geert.Audenaert@Syntegra.Com
                // Change by Geert.Audenaert@Syntegra.Com
                // Date: 27/2/2003
                string ApprovalRoles = string.Empty;
                foreach (ListItem PropertiesItem in authApproveRoles.Items)
                {
                    if (PropertiesItem.Selected)
                    {
                        ApprovalRoles = ApprovalRoles + PropertiesItem.Text + ";";
                    }
                }
                // End Change Geert.Audenaert@Syntegra.Com

                RainbowModuleProvider.Instance.UpdateModule(Int32.Parse(tabDropDownList.SelectedItem.Value),
                                     ModuleID,
                                     m.ModuleOrder,
                                     m.PaneName,
                                     moduleTitle.Text,
                                     Int32.Parse(cacheTime.Text),
                                     editRoles,
                                     viewRoles,
                                     addRoles,
                                     deleteRoles,
                                     PropertiesRoles,
                                     moveModuleRoles,
                                     deleteModuleRoles,
                                     ShowMobile.Checked,
                                     PublishingRoles,
                                     enableWorkflowSupport.Checked,
                                     ApprovalRoles,
                                     showEveryWhere.Checked,
                                     allowCollapsable.Checked);
            }
        }

        void saveAndCloseButton_Click(object sender, EventArgs e)
        {
            OnUpdate(e);
            // Navigate back to admin page
            if (Page.IsValid)
            {
                RedirectBackToReferringPage();
            }
        }

        //Used to populate all checklist roles with Roles in portal
        void populateRoles(ref CheckBoxList listRoles, string moduleRoles)
        {
            IList<RainbowRole> roles = AccountSystem.Instance.GetPortalRoles(portalSettings.PortalAlias);

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

            listRoles.DataSource = roles;
            listRoles.DataTextField = "Name";
            listRoles.DataValueField = "Id";
            listRoles.DataBind();

            //Splits up the role string and use array 30/01/2003 - by manudea
            while (moduleRoles.EndsWith(";"))
            {
                moduleRoles = moduleRoles.Substring(0, moduleRoles.Length - 1);
            }
            string[] arrModuleRoles = moduleRoles.Split(';');
            int roleCount = arrModuleRoles.GetUpperBound(0);

            //Cycle every role and select it if needed
            foreach (ListItem ls in listRoles.Items)
            {
                for (int i = 0; i <= roleCount; i++)
                {
                    if (arrModuleRoles[i].ToLower() == ls.Text.ToLower())
                    {
                        ls.Selected = true;
                    }
                }
            }
        }

        static string giveMeFriendlyName(Guid guid)
        {
            string friendlyName = string.Empty;
            using (SqlDataReader auxDr = new ModulesDB().GetSingleModuleDefinition(guid))
            {
                if (auxDr.Read())
                {
                    friendlyName = auxDr["FriendlyName"] as string;
                }
            }
            return friendlyName;
        }

        /// <summary>
        /// The BindData helper method is used to populate a asp:datalist
        /// server control with the current "edit access" permissions
        /// set within the portal configuration system
        /// </summary>
        void BindData() 
        {
            bool useNTLM = HttpContext.Current.User is WindowsPrincipal;
            // add by Jonathan Fong 22/07/2004 to support LDAP
            // jes1111 - useNTLM |= ConfigurationSettings.AppSettings["LDAPLogin"] != null ? true : false;
            useNTLM |= ((Config.LDAPLogin.Length != 0) ? true : false);

            authAddRoles.Visible = 
                authApproveRoles.Visible = 
                authDeleteRoles.Visible =
                authEditRoles.Visible =
                authPropertiesRoles.Visible =
                authPublishingRoles.Visible =
                authMoveModuleRoles.Visible =
                authDeleteModuleRoles.Visible =
                authViewRoles.Visible = !useNTLM;
            object value = GetModule();
            if (value != null)
            {
                RainbowModule module = (RainbowModule)value;
                moduleType.Text = giveMeFriendlyName(module.GuidID);

                // Update Textbox Settings
                moduleTitle.Text = module.ModuleTitle;
                cacheTime.Text = module.CacheTime.ToString();

                portalTabs = PortalPageProvider.Instance.GetPagesFlat(portalSettings.PortalID);
                tabDropDownList.DataBind();
                tabDropDownList.ClearSelection();
                if (tabDropDownList.Items.FindByValue(module.PageID.ToString()) != null)
                {
                    tabDropDownList.Items.FindByValue(module.PageID.ToString()).Selected = true;
                }

                // Change by John.Mandia@whitelightsolutions.com
                //Date: 19/5/2003
                showEveryWhere.Checked = module.ShowEveryWhere;

                // is the window mgmt support enabled
                // jes1111 - allowCollapsable.Enabled = GlobalResources.SupportWindowMgmt;
                allowCollapsable.Enabled = Config.WindowMgmtControls;
                allowCollapsable.Checked = module.SupportCollapsable;

                ShowMobile.Checked = module.ShowMobile;
                // Change by Geert.Audenaert@Syntegra.Com
                // Date: 6/2/2003
                PortalModuleControl pm;
                string controlPath;
                controlPath = Path.WebPathCombine(Path.ApplicationRoot, module.DesktopSrc);

                try
                {
                    pm = (PortalModuleControl)LoadControl(controlPath);
                    if (pm.InnerSupportsWorkflow)
                    {
                        enableWorkflowSupport.Checked = module.SupportWorkflow;
                        authApproveRoles.Enabled = module.SupportWorkflow;
                        authPublishingRoles.Enabled = module.SupportWorkflow;
                        populateRoles(ref authPublishingRoles, module.AuthorizedPublishingRoles);
                        populateRoles(ref authApproveRoles, module.AuthorizedApproveRoles);
                    }
                    else
                    {
                        enableWorkflowSupport.Enabled = false;
                        authApproveRoles.Enabled = false;
                        authPublishingRoles.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    //ErrorHandler.HandleException("There was a problem loading: '" + controlPath + "'", ex);
                    //throw;
                    throw new RainbowException(LogLevel.Error,
                        "There was a problem loading: '" + controlPath + "'", ex);
                }

                // End Change Geert.Audenaert@Syntegra.Com

                // Populate checkbox list with all security roles for this portal
                // and "check" the ones already configured for this module
                populateRoles(ref authEditRoles, module.AuthorizedEditRoles);
                populateRoles(ref authViewRoles, module.AuthorizedViewRoles);
                populateRoles(ref authAddRoles, module.AuthorizedAddRoles);
                populateRoles(ref authDeleteRoles, module.AuthorizedDeleteRoles);
                populateRoles(ref authMoveModuleRoles, module.AuthorizedMoveModuleRoles);
                populateRoles(ref authDeleteModuleRoles, module.AuthorizedDeleteModuleRoles);
                populateRoles(ref authPropertiesRoles, module.AuthorizedPropertiesRoles);

                // Jes1111
                if (!pm.Cacheable)
                {
                    cacheTime.Text = "-1";
                    cacheTime.Enabled = false;
                }
            }
            else // Denied access if Module not in Tab. jviladiu@portalServices.net (2004/07/23)
            {
                PortalSecurity.AccessDenied();
            }
        }

        RainbowModule GetModule()
        {
            foreach (RainbowModule module in portalSettings.ActivePage.Modules)
            {
                if (module.ModuleID == ModuleID)
                {
                    return module;
                }
            }
            return null;
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e ) {
            this.PlaceHolderButtons.EnableViewState = false;
            this.PlaceholderButtons2.EnableViewState = false;

            //Controls must be created here
            updateButton = new LinkButton();
            updateButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add( updateButton );


            // jminond added to top of property page so no need to scroll for save
            LinkButton update2 = new LinkButton();
            update2.CssClass = "CommandButton";
            update2.TextKey = "Apply";
            update2.Text = "Apply";
            update2.Click += new EventHandler( UpdateButton_Click );
            PlaceholderButtons2.Controls.Add( update2 );

            PlaceHolderButtons.Controls.Add( new LiteralControl( "&nbsp;" ) );
            PlaceholderButtons2.Controls.Add( new LiteralControl( "&nbsp;" ) );

            saveAndCloseButton = new LinkButton();
            saveAndCloseButton.TextKey = "OK";
            saveAndCloseButton.Text = "Save and close";
            saveAndCloseButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add( saveAndCloseButton );
            this.saveAndCloseButton.Click += new EventHandler( this.saveAndCloseButton_Click );


            // jminond added to top of property page so no need to scroll for save
            LinkButton saveAndCloseButton2 = new LinkButton();
            saveAndCloseButton2.TextKey = "OK";
            saveAndCloseButton2.Text = "Save and close";
            saveAndCloseButton2.CssClass = "CommandButton";
            PlaceholderButtons2.Controls.Add( saveAndCloseButton2 );
            saveAndCloseButton2.Click += new EventHandler( this.saveAndCloseButton_Click );


            PlaceHolderButtons.Controls.Add( new LiteralControl( "&nbsp;" ) );
            PlaceholderButtons2.Controls.Add( new LiteralControl( "&nbsp;" ) );

            moduleSettingsButton = new HyperLink();
            moduleSettingsButton.TextKey = "MODULESETTINGS_SETTINGS";
            moduleSettingsButton.Text = "Settings";
            moduleSettingsButton.CssClass = "CommandButton";
            moduleSettingsButton.NavigateUrl =
                Rainbow.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Admin/PropertyPage.aspx", PageID, ModuleID);
            PlaceHolderButtons.Controls.Add( moduleSettingsButton );

            // jminond added to top of property page so no need to scroll for save
            HyperLink moduleSettingsButton2 = new HyperLink();
            moduleSettingsButton2.TextKey = "MODULESETTINGS_SETTINGS";
            moduleSettingsButton2.Text = "Settings";
            moduleSettingsButton2.CssClass = "CommandButton";
            moduleSettingsButton2.NavigateUrl =
                Rainbow.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Admin/PropertyPage.aspx", PageID, ModuleID);
            PlaceholderButtons2.Controls.Add( moduleSettingsButton2 );

            PlaceHolderButtons.Controls.Add( new LiteralControl( "&nbsp;" ) );
            PlaceholderButtons2.Controls.Add( new LiteralControl( "&nbsp;" ) );

            cancelButton = new LinkButton();
            cancelButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add( cancelButton );

            // jminond added to top of property page so no need to scroll for save
            LinkButton cancel2 = new LinkButton();
            cancel2.CssClass = "CommandButton";
            cancel2.TextKey = "Cancel";
            cancel2.Text = "Cancel";
            cancel2.Click += new EventHandler( CancelButton_Click );
            PlaceholderButtons2.Controls.Add( cancel2 );

            //			if (((Page) this.Page).IsCssFileRegistered("tabsControl") == false)
            //			{
            //				string themePath = Path.WebPathCombine(this.CurrentTheme.WebPath, "/tabControl.css");
            //				((Page) this.Page).RegisterCssFile("tabsControl", themePath);
            //			}
            if ( !( ( Rainbow.Framework.Web.UI.Page )this.Page ).IsCssFileRegistered( "TabControl" ) )
                ( ( Rainbow.Framework.Web.UI.Page )this.Page ).RegisterCssFile( "TabControl" );

            this.enableWorkflowSupport.CheckedChanged += new EventHandler( this.enableWorkflowSupport_CheckedChanged );
            this.Load += new EventHandler( this.Page_Load );
            base.OnInit( e );
        }

        #endregion

        void enableWorkflowSupport_CheckedChanged(object sender, EventArgs e)
        {
            authApproveRoles.Enabled = enableWorkflowSupport.Checked;
            authPublishingRoles.Enabled = enableWorkflowSupport.Checked;
        }

        void UpdateButton_Click(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel(e);
        }

        void DeleteButton_Click(object sender, EventArgs e)
        {
            OnDelete(e);
        }

        ArrayList allowedModules = null;

        /// <summary>
        /// Only can use this page from tab with original module
        /// jviladiu@portalServices.net (2004/07/22)
        /// </summary>
        protected override ArrayList AllowedModules
        {
            get
            {
                if (allowedModules == null)
                {
                    ArrayList list = new ArrayList();
                    list.Add(RainbowModuleProvider.Instance.GetModuleGuid(ModuleID).ToString());
                    allowedModules = list;
                }
                return allowedModules;
            }
        }
    }
}
