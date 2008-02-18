// Change by Geert.Audenaert@Syntegra.Com
// Date: 7/2/2003
using System;
using System.Collections;
using System.Data.SqlClient;
using Rainbow.Framework;
using Rainbow.Framework.Content.Data;
using Rainbow.Framework.Core;
using Rainbow.Framework.Security;
using Rainbow.Framework.Web.UI;
using History=Rainbow.Framework.History;
// End Change Geert.Audenaert@Syntegra.Com

namespace Rainbow.Content.Web.Modules
{
    /// <summary>
    /// 
    /// </summary>
    [History("jminond", "2004/04/5", "Cleaned up using methods")]
    [History("CIsakson", "2003/03/10", "Added Target Field")]
    [History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
    public partial class LinksEdit : AddEditItemPage
    {
        #region Declarations

        #endregion

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the
        /// ItemID of the link to edit.
        /// It then uses the Rainbow.LinkDB() data component
        /// to populate the page's edit controls with the links details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if an
            // link itemID value is specified, and if so populate page
            // contents with the link details

            if (Page.IsPostBack == false)
            {
                TargetField.Items.Add("_new");
                TargetField.Items.Add("_blank");
                TargetField.Items.Add("_parent");
                TargetField.Items.Add("_self");
                TargetField.Items.Add("_top");

                if (ItemID != 0)
                {
                    // Obtain a single row of link information
                    LinkDB links = new LinkDB();
                    SqlDataReader dr = links.GetSingleLink(ItemID, WorkFlowVersion.Staging);

                    try
                    {
                        // Read in first row from database
                        if (dr.Read())
                        {
                            TitleField.Text = dr["Title"].ToString();
                            DescriptionField.Text = dr["Description"].ToString();
                            UrlField.Text = dr["Url"].ToString();
                            MobileUrlField.Text = dr["MobileUrl"].ToString();
                            ViewOrderField.Text = dr["ViewOrder"].ToString();
                            CreatedBy.Text = dr["CreatedByUser"].ToString();
                            CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                            TargetField.Items.FindByText((string) dr["Target"]).Selected = true;
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (CreatedBy.Text == "unknown")
                            {
                                CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }
                        }
                    }
                    finally
                    {
                        // Close datareader
                        dr.Close();
                    }
                }
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
                al.Add("476CF1CC-8364-479D-9764-4B3ABD7FFABD");
                return al;
            }
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to either
        /// create or update a link.  It  uses the Rainbow.LinkDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            if (Page.IsValid)
            {
                // Create an instance of the Link DB component
                LinkDB links = new LinkDB();

                if (ItemID == 0)
                {
                    // Add the link within the Links table
                    links.AddLink(ModuleID, ItemID, RainbowPrincipal.CurrentUser.Identity.Email, TitleField.Text,
                                  UrlField.Text, MobileUrlField.Text, Int32.Parse(ViewOrderField.Text),
                                  DescriptionField.Text, TargetField.SelectedItem.Text);
                }
                else
                {
                    // Update the link within the Links table
                    links.UpdateLink(ModuleID, ItemID, RainbowPrincipal.CurrentUser.Identity.Email, TitleField.Text,
                                     UrlField.Text, MobileUrlField.Text, Int32.Parse(ViewOrderField.Text),
                                     DescriptionField.Text, TargetField.SelectedItem.Text);
                }

                // Redirect back to the portal home page
                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// The DeleteBtn_Click event handler on this Page is used to delete
        /// a link.  It  uses the Rainbow.LinksDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);

            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)
            if (ItemID != 0)
            {
                LinkDB links = new LinkDB();
                links.DeleteLink(ItemID);
            }

            // Redirect back to the portal home page
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}