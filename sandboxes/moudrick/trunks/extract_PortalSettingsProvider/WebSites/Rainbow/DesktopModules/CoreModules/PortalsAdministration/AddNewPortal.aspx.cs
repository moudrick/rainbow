using System;
using System.Collections;
using Rainbow.Framework;
using Rainbow.Framework.Core;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Settings.Cache;
using Rainbow.Framework.Web.UI;
using Rainbow.Framework.Web.UI.WebControls;
using History=Rainbow.Framework.History;

namespace Rainbow.AdminAll
{
    /// <summary>
    /// New portal wizard
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    [History("Mario Endara", "2004/10/14", "Now can create a Portal based on other Portal (Roles, Tabs & Modules)")]
    public partial class AddNewPortal : AddItemPage
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Verify that the current user has access to access this page
            // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
//            if (PortalSecurity.IsInRoles("Admins") == false) 
//                PortalSecurity.AccessDeniedEdit();

            // If this is the first visit to the page, populate the site data
            if (Page.IsPostBack == false)
            {
                // Bind the Portals to the SolutionsList
                SolutionsList.DataSource = PortalProvider.Instance.GetPortalsDataSet();
                SolutionsList.DataBind();

                //Preselect default Portal
                if (SolutionsList.Items.FindByValue("Default") != null)
                {
                    SolutionsList.Items.FindByValue("Default").Selected = true;
                }
            }

            if (chkUseTemplate.Checked == false)
            {
                // Don't use a template portal, so show the EditTable
                // Remove the cache that can be setted by the new Portal, to get a "clean" PortalBaseSetting
                CurrentCache.Remove(Key.PortalBaseSettings());
                EditTable.DataSource = new SortedList(PortalProvider.Instance.GetPortalBaseSettings(null));
                EditTable.DataBind();
                EditTable.Visible = true;
                SolutionsList.Enabled = false;
            }
            else
            {
                EditTable.Visible = false;
                SolutionsList.Enabled = true;
            }
        }

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override ArrayList AllowedModules
        {
            get
            {
                ArrayList al = new ArrayList();
                al.Add("366C247D-4CFB-451D-A7AE-649C83B05841");
                return al;
            }
        }

        /// <summary>
        /// OnUpdate
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);
            if (Page.IsValid)
            {
                //Get Solutions
                try
                {
                    PathField.Text = PathField.Text.Replace("/", string.Empty);
                    PathField.Text = PathField.Text.Replace("\\", string.Empty);
                    PathField.Text = PathField.Text.Replace(".", string.Empty);
                    if (chkUseTemplate.Checked == false)
                    {
                        // Create portal the "old" way
                        int newPortalID = PortalProvider.Instance.CreatePortal(
                            Convert.ToInt32(SolutionsList.SelectedItem.Value), 
                            AliasField.Text,
                            TitleField.Text, 
                            PathField.Text);

                        // Update custom settings in the database
                        EditTable.ObjectID = newPortalID;
                        EditTable.UpdateControls();
                    }
                    else
                    {
                        //Create portal based on the selected portal
                        PortalProvider.Instance.CreatePortal(
                            Convert.ToInt32(SolutionsList.SelectedItem.Value),
                            SolutionsList.SelectedItem.Text, 
                            AliasField.Text, 
                            TitleField.Text,
                            PathField.Text);
                    }

                    // Redirect back to calling page
                    RedirectBackToReferringPage();
                }
                catch (Exception ex)
                {
                    string aux =
                        General.GetString("NEW_PORTAL_ERROR", "There was an error on creating the portal", this);
                    ErrorHandler.Publish(LogLevel.Error, aux, ex);

                    ErrorMessage.Visible = true;
                    ErrorMessage.Text = aux + "<br>";
                }
            }
        }

        /// <summary>
        /// Handles the UpdateControl event of the EditTable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Rainbow.Framework.Web.UI.WebControls.SettingsTableEventArgs"/> instance containing the event data.</param>
        private void EditTable_UpdateControl(object sender, SettingsTableEventArgs e)
        {
            SettingsTable edt = (SettingsTable) sender;
            RainbowContext.Current.UpdatePortalSetting(edt.ObjectID, e.CurrentItem.EditControl.ID, e.CurrentItem.Value);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.EditTable.UpdateControl +=
                new Rainbow.Framework.Web.UI.WebControls.UpdateControlEventHandler(this.EditTable_UpdateControl);
            this.Load += new EventHandler(this.Page_Load);

            //Translations
            RequiredTitle.ErrorMessage = General.GetString("VALID_FIELD");
            RequiredAlias.ErrorMessage = General.GetString("VALID_FIELD");
            RequiredSitepath.ErrorMessage = General.GetString("VALID_FIELD");

            base.OnInit(e);
        }

        #endregion
    }
}
