using System;
using System.Collections;
using Rainbow.Framework.Core;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Content.Web.Modules
{
    public partial class SiteSettingsmod : PortalModuleControl
    {
        /// <summary>
        /// Admin Module
        /// </summary>
        public override bool AdminModule
        {
            get { return true; }
        }

        /// <summary>
        /// The Page_Load server event handler on this user control is used
        /// to populate the current site settings from the config system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If this is the first visit to the page, populate the site data
            if (Page.IsPostBack == false)
            {
                //We flush cache for enable correct localization of items
                PortalProvider.Instance.FlushBaseSettingsCache(PortalSettings.PortalPath);

                siteName.Text = PortalSettings.PortalName;
                sitePath.Text = PortalSettings.PortalPath;
            }
            EditTable.DataSource = new SortedList(PortalSettings.CustomSettings);
            EditTable.DataBind();
        }

        /// <summary>
        /// Is used to update the Site Settings within the Portal Config System
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdate(EventArgs e)
        {
            // Flush the cache for recovery the changes. jviladiu@portalServices.net (30/07/2004)
            PortalProvider.Instance.FlushBaseSettingsCache(PortalSettings.PortalPath);
            //Call base
            base.OnUpdate(e);

            // Only Update if Input Data is Valid
            if (Page.IsValid)
            {
                //Update main settings and Tab info in the database
                PortalProvider.Instance.UpdatePortalInfo(PortalSettings.PortalID, siteName.Text, sitePath.Text, false);

                // Update custom settings in the database
                EditTable.UpdateControls();

                // Redirect to this site to refresh
                Response.Redirect(Request.RawUrl);
            }
        }

        private void EditTable_UpdateControl(object sender, SettingsTableEventArgs e)
        {
            RainbowContext.Current.UpdatePortalSetting(PortalSettings.PortalID,
                                                       e.CurrentItem.EditControl.ID,
                                                       e.CurrentItem.Value);
        }

        public override Guid GuidID
        {
            get { return new Guid("{EBBB01B1-FBB5-4E79-8FC4-59BCA1D0554E}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if (!this.Page.IsCssFileRegistered("TabControl"))
                this.Page.RegisterCssFile("TabControl");

            this.EditTable.UpdateControl +=
                new Rainbow.Framework.Web.UI.WebControls.UpdateControlEventHandler(this.EditTable_UpdateControl);

            this.updateButton.Click += new EventHandler(updateButton_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        void updateButton_Click(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        #endregion
    }
}
