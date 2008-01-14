using System.Collections;

namespace Rainbow.Framework.BusinessObjects
{
    /// <summary>
    /// PageSettings Class encapsulates the detailed settings 
    /// for a specific Page in the Portal
    /// </summary>
    //TODO: [moudrick] rename it to PortalPage
    public class PortalPage
    {
        // Jes1111
        //		public int			TemplateId;
        /// <remarks>
        /// thierry (tiptopweb)
        /// to have dropdown list for the themes and layout, we need the data path for the portal (for private theme and layout)
        /// we need the portalPath here for this use and it has to be set from the current portalSettings before getting the
        /// CustomSettings for a tab
        /// </remarks>
        string portalPath = null;

        /// <summary>
        /// Gets or sets the portal path.
        /// </summary>
        /// <value>The portal path.</value>
        /// <remarks>
        /// </remarks>
        public string PortalPath
        {
            get { return portalPath; }
            //internal set
            set
            {
                portalPath = value;
                if (!portalPath.EndsWith("/"))
                {
                    portalPath += "/";
                }
            }
        }

        Hashtable customSettings;

        string authorizedRoles;
        string mobilePageName;
        int parentPageID;
        bool showMobile;
        int pageID;
        string pageName;
        int pageOrder;

        /// <summary>
        /// </summary>
        public string AuthorizedRoles
        {
            get { return authorizedRoles; }
            internal set { authorizedRoles = value; }
        }

        /// <summary>
        /// </summary>
        public string MobilePageName
        {
            get { return mobilePageName; }
            internal set { mobilePageName = value; }
        }

        /// <summary>
        /// </summary>
        public readonly ArrayList Modules = new ArrayList();

        /// <summary>
        /// </summary>
        public int ParentPageID
        {
            get { return parentPageID; }
            internal set { parentPageID = value; }
        }

        /// <summary>
        /// </summary>
        public bool ShowMobile
        {
            get { return showMobile; }
            internal set { showMobile = value; }
        }

        /// <summary>
        /// </summary>
        public int PageID
        {
            get { return pageID; }
            set { pageID = value; }
            //internal set { pageID = value; }
        }

        /// <summary>
        /// </summary>
        public int PageOrder
        {
            get { return pageOrder; }
            internal set { pageOrder = value; }
        }

//		/// <summary>
//		/// </summary>
//		public string PageLayout;


        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>The name of the page.</value>
        /// <remarks>
        /// </remarks>
        public string PageName
        {
            get { return pageName; }
            internal set { pageName = value; }
        }

        /// <summary>
        /// Page Settings For Search Engines
        /// </summary>
        /// <value>The custom settings.</value>
        public Hashtable CustomSettings
        {
            get
            {
                return customSettings;
            }
            internal set
            {
                //TODO: [moudrick] make it readonly
                customSettings = value;
            }
        }

        ///<summary>
        ///</summary>
        public string CustomMenuImage
        {
            get { return CustomSettings["CustomMenuImage"].ToString(); }
        }

        ///<summary>
        ///</summary>
        //internal PortalPage()
        public PortalPage()
        {
        }
    }
}
