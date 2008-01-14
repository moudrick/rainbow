using System.Collections;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Threading;
using System.Web;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Providers
{
    ///<summary>
    ///</summary>
    public abstract class PortalPageProvider : ProviderBase
    {
        const string providerType = "portalPage";
        const string strAllUsers = "All Users;";
        const int intParentPageID = 0; //SP will convert to NULL if 0
        const bool boolShowMobile = false;
        const string strMobilePageName = ""; // NULL NOT ALLOWED IN TABLE.

        /// <summary>
        /// Gets default configured Portal provider.
        /// Singleton pattern standard member.
        /// </summary>
        /// <returns>Default instance of Portal Provider class</returns>
        public static PortalPageProvider Instance
        {
            get
            {
                return ProviderConfiguration.GetDefaultProviderFromCache<PortalPageProvider>(
                    providerType, HttpContext.Current.Cache);
            }
        }

        /// <summary>
        /// Read Current Page subtabs
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>PagesBox</returns>
        protected abstract PagesBox GetPageSettingsPagesBox(int pageID);

        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hashtable of
        /// custom Page specific settings from the database.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        protected abstract Hashtable GetPageCustomSettings(int pageID);

        /// <summary>
        /// Changed by Thierry@tiptopweb.com.au
        /// Page are different for custom page layout an theme, this cannot be static
        /// Added by john.mandia@whitelightsolutions.com
        /// Cache by Manu
        /// non static function, Thierry : this is necessary for page custom layout and themes
        /// </summary>
        /// <returns>A System.Collections.Hashtable value...</returns>
        protected abstract Hashtable GetPageBaseSettings(PortalPage portalPage);

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        /// <remarks>
        /// </remarks>
        public PagesBox GetPagesBox(PageStripDetails pageStripDetails)
        {
            string cacheKey = Key.TabNavigationSettings(pageStripDetails.PageID,
                                                        Thread.CurrentThread.CurrentUICulture.ToString());
            PagesBox tabs;

            if (!CurrentCache.Exists(cacheKey))
            {
                tabs = GetPageSettingsPagesBox(pageStripDetails.PageID);
                CurrentCache.Insert(cacheKey, tabs);
            }
            else
            {
                tabs = (PagesBox) CurrentCache.Get(cacheKey);
            }
            return tabs;
        }

        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hashtable of
        /// custom Page specific settings from the database. This method is
        /// used by Portals to access misc Page settings.
        /// </summary>
        /// <param name="portalPage"></param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public Hashtable GetPageCustomSettings(PortalPage portalPage, int pageID)
        {
            Hashtable baseSettings;
            if (CurrentCache.Exists(Key.TabSettings(pageID)))
            {
                baseSettings = (Hashtable) CurrentCache.Get(Key.TabSettings(pageID));
            }
            else
            {
                baseSettings = GetPageBaseSettings(portalPage);
                Hashtable settings = GetPageCustomSettings(pageID);

                // Thierry (Tiptopweb)
                // TODO : put back the cache in GetPageBaseSettings() and reset values not found in the database
                foreach (string key in baseSettings.Keys)
                {
                    if (settings[key] != null)
                    {
                        SettingItem s = ((SettingItem) baseSettings[key]);

                        if (settings[key].ToString().Length != 0)
                        {
                            s.Value = settings[key].ToString();
                        }
                    }
                    else //by Manu
                        // Thierry (Tiptopweb), see the comment in Hashtable GetPageBaseSettings()
                        // this is not resetting key not found in the database
                    {
                        //SettingItem s = ((SettingItem)baseSettings[key]);
                        //s.Value = string.Empty; 3_aug_2004 Cory Isakson.  This line caused an error with booleans
                    }
                }
                CurrentCache.Insert(Key.TabSettings(pageID), baseSettings);
            }
            return baseSettings;
        }

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <returns></returns>
        public int AddPage(int portalID, string pageName, int pageOrder)
        {
            return AddPage(portalID, pageName, strAllUsers, pageOrder);
        }

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="Roles">The roles.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <returns></returns>
        public int AddPage(int portalID, string pageName, string Roles, int pageOrder)
        {
            // Change Method to use new all parms method below
            // SP call moved to new method AddPage below.
            // Mike Stone - 30/12/2004
            return AddPage(portalID, intParentPageID, pageName, pageOrder, strAllUsers, boolShowMobile, strMobilePageName);
        }

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <param name="authorizedRoles">The authorized roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="mobilePageName">Name of the mobile page.</param>
        /// <returns></returns>
        public abstract int AddPage(int portalID,
                                    int parentPageID,
                                    string pageName,
                                    int pageOrder,
                                    string authorizedRoles,
                                    bool showMobile,
                                    string mobilePageName);

        /// <summary>
        /// The DeletePage method deletes the selected tab from the portal.<br/>
        /// DeletePage Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public abstract void DeletePage(int pageID);

        /// <summary>
        /// UpdatePage Method<br/>
        /// UpdatePage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <param name="authorizedRoles">The authorized roles.</param>
        /// <param name="mobilePageName">Name of the mobile page.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        public abstract void UpdatePage(int portalID,
                                        int pageID,
                                        int parentPageID,
                                        string pageName,
                                        int pageOrder,
                                        string authorizedRoles,
                                        string mobilePageName,
                                        bool showMobile);

        /// <summary>
        /// The UpdatePageOrder method changes the position of the tab with respect
        /// to other tabs in the portal.<br/>
        /// UpdatePageOrder Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="pageOrder">The page order.</param>
        public abstract void UpdatePageOrder(int pageID, int pageOrder);

        /// <summary>
        /// Gets the pages flat.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public abstract ArrayList GetPagesFlat(int portalID);

        ///<summary>
        ///</summary>
        ///<param name="pageID"></param>
        ///<returns></returns>
        public PortalPage InstantiateNewPortalPage(int pageID)
        {
            PortalPage portalPage = new PortalPage();
            portalPage.CustomSettings = GetPageCustomSettings(portalPage, pageID);
            return portalPage;
        }

        ///<summary>
        ///</summary>
        ///<param name="activePortalPage"></param>
        ///<param name="pageID"></param>
        ///<param name="pageLayout"></param>
        ///<param name="parentPageId"></param>
        ///<param name="pageOrder"></param>
        ///<param name="mobilePageName"></param>
        ///<param name="authorizedRoles"></param>
        ///<param name="pageName"></param>
        ///<param name="showMobile"></param>
        ///<param name="portalPath"></param>
        public static void FillPortalPage(PortalPage activePortalPage,
                                          int pageID,
                                          string pageLayout,
                                          int parentPageId,
                                          int pageOrder,
                                          string mobilePageName,
                                          string authorizedRoles,
                                          string pageName,
                                          bool showMobile,
                                          string portalPath)
        {
            activePortalPage.PageID = pageID;
            //activePortalPage.PageLayout = pageLayout;
            activePortalPage.ParentPageID = parentPageId;
            activePortalPage.PageOrder = pageOrder;
            activePortalPage.MobilePageName = mobilePageName;
            activePortalPage.AuthorizedRoles = authorizedRoles;
            activePortalPage.PageName = pageName;
            activePortalPage.ShowMobile = showMobile;
            activePortalPage.PortalPath = portalPath; // thierry@tiptopweb.com.au for page custom layout
        }

        /// <summary>
        /// Return the portal home page in case you are on pageid = 0
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public abstract int PortalHomePageID(int portalID);


        /// <summary>
        /// Gets the pages flat table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
#warning //TODO: use PageTreeItem[] instead of DataTable  
        //public DataTable GetPagesFlatTable(int portalID)
        public abstract object GetPagesFlatTable(int portalID);

        /// <summary>
        /// Gets the pages parent.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        public abstract IList<PageItem> GetPagesParent(int portalID, int pageID);

        /// <summary>
        /// Update Page Custom Settings
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public abstract void UpdatePageSettings(int pageID, string key, string value);
    }
}