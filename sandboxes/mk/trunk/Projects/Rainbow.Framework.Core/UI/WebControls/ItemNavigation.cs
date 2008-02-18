using DUEMETRI.UI.WebControls.HWMenu;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Security;

namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// ItemNavigation inherits from MenuNavigation
    /// and adds the functionality of the ShopNavigation with ALL types of binding.
    /// all subcategories are added as an ItemID property.
    /// 
    /// mario@hartmann.net: 24/07/2003
    /// modified from MenuNavigation
    /// the navigation will not be effective and instead we navigate to the same page
    /// and transmit the PageID as a ItemID.
    ///
    /// thierry@tiptopweb.com.au: 17/09/2003
    /// replace Default.aspx by DesktopDefault.aspx as we are loosing the parameters
    /// when transfering from Default.aspx to DesktopDefault.aspx and not using the UrlBuilder
    /// </summary>
    public class ItemNavigation : MenuNavigation
    {
        /// <summary>
        /// Do databind.
        /// </summary>
        public override void DataBind()
        {
            // add the root!
            AddRootNode();
            base.DataBind();
        }

        /// <summary>
        /// Add the current tab as top menu item.
        /// </summary>
        void AddRootNode()
        {
            Portal portalSettings = PortalProvider.Instance.CurrentPortal;
            PortalPage tabItemsRoot = portalSettings.ActivePage;

            using (MenuTreeNode mn = new MenuTreeNode(tabItemsRoot.PageName))
            {
                // change the link to stay on the same page and call a category product
                mn.Link = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabItemsRoot.PageID);
                mn.Width = Width;
                Childs.Add(mn);
            }
        }

        /// <summary>
        /// Add a Menu Tree Node if user in in the list of Authorized roles.
        /// Thanks to abain for fixing authorization bug.
        /// </summary>
        /// <param name="tabIndex">Index of the tab</param>
        /// <param name="myTab">Tab to add to the MenuTreeNodes collection</param>
        protected override void AddMenuTreeNode(int tabIndex, PageStripDetails myTab)
        {
            if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles))
            {
                Portal portalSettings = PortalProvider.Instance.CurrentPortal;
                int tabIDItemsRoot = portalSettings.ActivePage.PageID;

                MenuTreeNode menuTreeNode = new MenuTreeNode(myTab.PageName);

                // change the link to stay on the same page and call a category product
                menuTreeNode.Link = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabIDItemsRoot, "ItemID=" + myTab.PageID);
                //fixed by manu
                menuTreeNode.Width = Width;
                menuTreeNode = RecourseMenu(tabIDItemsRoot, PortalPageProvider.Instance.GetPagesBox(myTab), menuTreeNode);
                Childs.Add(menuTreeNode);
            }
        }


        /// <summary>
        /// modified to transmit the PageID and TabIndex for the item page
        /// </summary>
        /// <param name="tabIDItemsRoot">The tab ID items root.</param>
        /// <param name="pagesBoxTabs">The t.</param>
        /// <param name="menuTreeNode">The mn.</param>
        /// <returns></returns>
        protected override MenuTreeNode RecourseMenu(int tabIDItemsRoot, 
            PagesBox pagesBoxTabs, MenuTreeNode menuTreeNode)
        {
            if (pagesBoxTabs.Count > 0)
            {
                for (int c = 0; c < pagesBoxTabs.Count; c++)
                {
                    PageStripDetails subTab = pagesBoxTabs[c];

                    if (PortalSecurity.IsInRoles(subTab.AuthorizedRoles))
                    {
                        MenuTreeNode mnc = new MenuTreeNode(subTab.PageName);

                        // change PageID into ItemID for the product module on the same page
                        mnc.Link =
                            HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabIDItemsRoot, "ItemID=" + subTab.PageID);
                        //by manu
                        mnc.Width = menuTreeNode.Width;
                        mnc = RecourseMenu(tabIDItemsRoot, PortalPageProvider.Instance.GetPagesBox(subTab), mnc);
                        menuTreeNode.Childs.Add(mnc);
                    }
                }
            }
            return menuTreeNode;
        }
    }
}
