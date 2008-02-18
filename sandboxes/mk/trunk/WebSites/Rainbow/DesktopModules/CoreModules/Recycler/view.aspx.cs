using System;
using System.Data;
using Rainbow.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Site.Data;
using Rainbow.Framework.Web.UI;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow
{
    /// <summary>
    /// Module print page
    /// </summary>
    public partial class recyclerViewPage : ViewItemPage
    {
        int moduleID;

        // TODO check if this works
        //protected ArrayList portalTabs;
        protected DataTable portalTabs;
        protected RainbowModule module;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            deleteButton.Visible = true;
            updateButton.Visible = false;

            try
            {
                moduleID = int.Parse(Request.Params["mID"]);

                module = RecyclerDB.GetModuleSettingsForIndividualModule(moduleID);
                if (RecyclerDB.ModuleIsInRecycler(moduleID))
                {
                    if (!Page.IsPostBack)
                    {
                        //load tab names for the dropdown list, then bind them
                        // TODO check if this works
                        //portalTabs = new PagesDB().GetPagesFlat(portalSettings.PortalID);
                        //portalTabs = PortalPageProvider.Instance.GetPagesFlatTable(portalSettings.PortalID);
                        ddTabs.DataSource = PortalPageProvider.Instance.GetPagesFlatTable(portalSettings.PortalID);
                        ddTabs.DataBind();

                        //on initial load, disable the restore button until they make a selection
                        restoreButton.Enabled = false;
                        ddTabs.Items.Insert(0, "--Choose a Tab to Restore this Module--");
                    }

                    // create an instance of the module
                    PortalModuleControl myPortalModule =
                        (PortalModuleControl) LoadControl(Path.ApplicationRoot + "/" + module.DesktopSrc);
                    myPortalModule.PortalID = portalSettings.PortalID;
                    myPortalModule.ModuleConfiguration = module;

                    // add the module to the placeholder
                    PrintPlaceHolder.Controls.Add(myPortalModule);
                }
                else
                    //they're trying to view a module that isn't in the recycler - maybe a manual manipulation of the url...?
                {
                    pnlMain.Visible = false;
                    pnlError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Handles OnDelete
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);

            // TODO add userEmail and useRecycler
            RainbowModuleProvider.Instance.DeleteModule(moduleID);
            moduleID = 0;
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.ddTabs.SelectedIndexChanged += new EventHandler(this.ddTabs_SelectedIndexChanged);
            this.restoreButton.Click += new EventHandler(this.restoreButton_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddTabs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void ddTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddTabs.SelectedIndex == 0)
            {
                restoreButton.Enabled = false;
            }
            else
            {
                restoreButton.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the restoreButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void restoreButton_Click(object sender, EventArgs e)
        {
            RecyclerDB.MoveModuleToNewTab(int.Parse(ddTabs.SelectedValue), moduleID);
            RedirectBackToReferringPage();
        }
    }
}
