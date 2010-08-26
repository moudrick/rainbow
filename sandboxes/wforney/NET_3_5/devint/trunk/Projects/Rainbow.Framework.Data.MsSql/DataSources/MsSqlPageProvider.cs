using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.DataSources;
using Rainbow.Framework.Data.Entities;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Data.MsSql.Debugger;
using System.Xml.Linq;
using Rainbow.Framework.Design;
using System.Collections;

namespace Rainbow.Framework.Data.MsSql.DataSources
{
    public class MsSqlPageProvider : PageProvider
    {
        #region Provider

        /// <summary>
        /// The initialize method lets you retrieve provider specific settings from web.config
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configValue">The config value.</param>
        public override void Initialize(string name, NameValueCollection configValue)
        {
            base.Initialize(name, configValue);

            //// For legacy support first check provider settings then web.config/rainbow.config legacy settings
            //if (configValue["handlersplitter"] != null)
            //{
            //    _defaultSplitter = configValue["handlersplitter"].ToString();
            //}
            //else
            //{
            //    if (ConfigurationManager.AppSettings["HandlerDefaultSplitter"] != null)
            //        _defaultSplitter = ConfigurationManager.AppSettings["HandlerDefaultSplitter"];
            //}
        }

        #endregion

        DataClassesDataContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlPageProvider"/> class.
        /// </summary>
        public MsSqlPageProvider()
        {
            db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();
        }

        #region IPageDataSource Members

        /// <summary>
        /// Adds the specified new Page.
        /// </summary>
        /// <param name="newPage">The new Page.</param>
        /// <returns>The newly created IPage object.</returns>
        public void Add(ref IPage newPage)
        {

            int i = Add(newPage.PageId, newPage.PageTitle, newPage.PageAlias);
            newPage = db.Pages.Single(p => p.PageId == i) as IPage;
        }

        /// <summary>
        /// Adds the Page based on specified solution id.
        /// </summary>
        /// <param name="solutionId">The solution id.</param>
        /// <param name="newPage">The new Page.</param>
        public void Add(int solutionId, ref IPage newPage)
        {
            int i = AddFromSolution(solutionId, newPage.PageAlias, newPage.PageTitle, newPage.PageAlias);
            newPage = db.Pages.Single(p => p.PageId == i) as IPage;
        }

        /// <summary>
        /// Creates the new Page.
        /// </summary>
        /// <returns>IPage</returns>
        public IPage CreateNew()
        {
            return new Page() as IPage;
        }

        /// <summary>
        /// Gets the Page by id.
        /// </summary>
        /// <param name="PageId">The Page id.</param>
        /// <returns>IPage</returns>
        public IPage GetById(int PageId)
        {
            var p = db.Pages.Single(pid => pid.PageId == PageId);

            //grab layout
            p.CurrentLayout = p.PageSettings.Single(cs => cs.SettingName == "SITESETTINGS_PAGE_LAYOUT").SettingValue;
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(p.PagePathRelative);
            //Default
            themeManager.Load(p.PageSettings.Single(cs => cs.SettingName == "SITESETTINGS_THEME").SettingValue);
            p.CurrentThemeDefault = themeManager.CurrentTheme;
            //Alternate
            themeManager.Load(p.PageSettings.Single(cs => cs.SettingName == "SITESETTINGS_ALT_THEME").SettingValue);
            p.CurrentThemeAlternate = themeManager.CurrentTheme;

            return p as IPage;
        }

        /// <summary>
        /// Gets all Pages.
        /// </summary>
        /// <returns>IEnumerable&lt;IPage&gt;</returns>
        public IEnumerable<IPage> GetAll()
        {
            var q = from p in db.Pages select p;

            return q as IEnumerable<IPage>;
        }

        /// <summary>
        /// Gets the portal home page.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <returns></returns>
        public IPage GetPortalHomePage(int portalId)
        {
            return GetById(GetPortalHomePageId(portalId));
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public override IPage GetById(Guid Id)
        {
            return db.Pages.Single(p => p.Id == Id);
        }

        /// <summary>
        /// Updates the specified Page.
        /// </summary>
        /// <param name="Page">IPage</param>
        public void Update(IPage Page)
        {
            Page p = db.Pages.Single(pt => pt.PageId == Page.PageId);

            p.PortalId = Page.PortalId;
            p.PageId = Page.Id;
            p.ParentId = Page.ParentId;

            Page.Name = (string.IsNullOrEmpty(Page.Name) ? "&nbsp;" : Page.Name);
            if (Page.Name.Length > 50) Page.Name = Page.Name.Substring(0, 49);
            p.Name = Page.Name;
            p.Order = Page.Order;
            p.AuthorizedRoles = Page.AuthorizedRoles;
            p.NameMobile = Page.NameMobile;
            p.IsShowMobile = Page.IsShowMobile;
        }

        /// <summary>
        /// Removes the specified Page id.
        /// </summary>
        /// <param name="Page">IPage</param>
        public void Remove(IPage Page)
        {
            Remove(Page.PageId);
        }

        public void CommitChanges()
        {
            db.SubmitChanges();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <returns></returns>
        private int Add(int portalID, string pageName, int pageOrder)
        {
            return Add(portalID, pageName, strAllUsers, pageOrder);
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
        private int Add(int portalID, string pageName, string Roles, int pageOrder)
        {
            // Change Method to use new all parms method below
            // SP call moved to new method AddPage below.
            // Mike Stone - 30/12/2004
            return Add(portalID, intParentPageID, pageName, pageOrder, strAllUsers, boolShowMobile, strMobilePageName);
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
        private int Add(int portalID, int parentPageID, string pageName, int pageOrder, string authorizedRoles,
                           bool showMobile, string mobilePageName)
        {
            pageName = (string.IsNullOrEmpty(pageName) ? "New Page" : pageName); // fix empty page name
            if (pageName.Length > 50) pageName = pageName.Substring(0, 49);   // fix long page name

            Page p = new Page()
            {
                ParentId = parentPageID,
                Order = pageOrder,
                Name = pageName,
                NameMobile = mobilePageName,
                AuthorizedRoles = authorizedRoles,
                IsShowMobile = showMobile
            };

            Add(ref p as IPage);

            return p.PageId;
        }

        /// <summary>
        /// Removes Page from database. All tabs, modules and data will be removed.
        /// </summary>
        /// <param name="PageID">The Page ID.</param>
        private void Remove(int PageId)
        {
            var q = db.Pages.Single(p => p.PageId == PageId);

            db.Pages.DeleteAllOnSubmit(q.Pages);
            db.Pages.DeleteOnSubmit(q);
        }

        /// <summary>
        /// Return the portal home page in case you are on pageid = 0
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <returns></returns>
        private int GetPortalHomePageId(int portalId)
        {
            return db.Pages.Single(p => p.PortalId == portalId && p.ParentId == null && p.Id > 0 && p.Order < 2).Id;
        }

        /// <summary>
        /// This user control will render the breadcrumb navigation for the current tab.
        /// Ver. 1.0 - 24. dec 2002 - First realase by Cory Isakson
        /// </summary>
        /// <param name="pageID">ID of the page</param>
        /// <returns></returns>
        public List<Page> GetPageCrumbs(int pageId)
        {
            XDocument crumbs = new XDocument();
            int parentPageId;
            string pageName;
            int level;

            //--First Child in the branch is Crumb 20.  
            level = 20;

            //--Get First Parent Page ID if there is one
            parentPageId = db.Pages.Single(p => p.Id == pageId).ParentId;

            //--Get PageName of Lowest Child
            pageName = db.Pages.Single(p => p.Id == pageId).Name;

            //--Build first Crumb
            crumbs.Add(
                new XElement("root",
                    new XElement("crumb",
                        new XAttribute("TabID", pageId.ToString()),
                        new XAttribute("Level", level),
                        pageName
                    )
                )
            );

            List<XElement> crumbList = new List<XElement>();

            while (parentPageId != null || parentPageId != 0)
            {
                level = level - 1;
                pageId = parentPageId;
                parentPageId = db.Pages.Single(p => p.Id == pageId).ParentId;
                pageName = db.Pages.Single(p => p.Id = pageId).Name;
                crumbList.Add(
                    new XElement("crumb",
                        new XAttribute("TabID", pageId.ToString()),
                        new XAttribute("Level", level),
                        pageName
                    )
                );
            }

            crumbs.Element("root").Add(crumbList.ToArray<XElement>());

            List<Page> crumbsList = new List<Page>();

            foreach (XNode node in crumbs.Nodes)
            {
                Page tab = new Page();
                tab.ID = Int16.Parse(node.Attributes.GetNamedItem("TabID").Value);
                tab.Name = node.InnerText;
                tab.Order = Int16.Parse(node.Attributes.GetNamedItem("Level").Value);
                crumbsList.Add(tab);
            }
            //Return the Crumb Page Items as an arraylist 
            return crumbsList;
        }

        /// <summary>
        /// Gets the pages flat table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public List<Page> GetPagesFlatTable(int portalId)
        {
            //-- Get the hierarchy
            //create table #tree (id int, sequence varchar(1000), levelNo int, TabOrder int)
            //-- insert top level (to get sub tree just insert relevent id here)
            var qTop = from pt in db.Pages
                       where pt.PortalId == portalId && pt.ParentId == null
                       orderby pt.Order
                       select new Page
                       {
                           Id = pt.Id,
                           Name = pt.Order.ToString().Length.ToString() + '.' + pt.Order.ToString(),
                           NestLevel = 1,
                           Order = pt.Order
                       };

            int i = 0;

            List<Page> tree = new List<Page>();
            tree.AddRange(qTop.ToList());

            foreach (var rTop in qTop)
            {
                i = i + 1;
                //     insert #tree
                //     -- Get all children of previous level

                //     select rb_Tabs.TabID, 
                //    #tree.sequence + '.'+ convert(varchar(10),Len(rb_Tabs.TabOrder)) + '.' + convert(varchar(10),rb_Tabs.TabOrder), 
                //    @i + 1, 
                //    rb_Tabs.TabOrder
                //     from rb_Tabs, #tree 
                //     where #tree.levelNo = @i
                //         and rb_Tabs.ParentTabID = #tree.id
                //     Order BY rb_Tabs.TabOrder

                //TODO: what is equivalent to #tree.sequence above?
                var qChild = from p in db.Pages
                             where p.ParentId == rTop.Id
                             orderby p.Order
                             select new Page
                             {
                                 Id = p.Id,
                                 Name = p.Order.ToString().Length.ToString() + '.' + p.Order.ToString(),
                                 NestLevel = i + 1,
                                 Order = p.Order
                             };
                tree.AddRange(qChild.ToList());
            }

            //-- output with hierarchy formatted
            List<Page> pages = new List<Page>();

            foreach (var t in tree)
            {
                //select rb_Tabs.TabID, 
                //    rb_Tabs.ParentTabID, 
                //    rb_Tabs.TabOrder, 
                //    rb_Tabs.TabName, 
                //    #tree.levelNo , 
                //    Replicate('-', (#tree.levelNo) * 2) + rb_Tabs.TabName as PageOrder
                //    --, #tree.sequence
                //from #tree, rb_Tabs
                //where #tree.id = rb_Tabs.TabID
                //order by #tree.sequence--, rb_Tabs.TabOrder

                var qTree = from p in db.Pages
                            where p.Id == t.Id
                            orderby p.Order
                            select new Page
                            {
                                Id = p.Id,
                                ParentId = p.ParentId,
                                Order = p.Order,
                                Name = p.Name,
                                NestLevel = p.NestLevel,
                                Description = delegate()
                                {
                                    string d = string.Empty;
                                    for (int i = 0; i < p.NestLevel * 2; i++) d = d + '-';
                                    d = d + p.Name;
                                    return d;
                                }
                            };

                pages.AddRange(qTree);
            }
            return pages;
        }

        /// <summary>
        /// Gets the pages flat.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public List<Page> GetPagesFlat(int portalID)
        {
            //--Create a Temporary table to hold the tabs for this query
            List<Page> pageTree = new List<Page>();
            //CREATE TABLE #TabTree
            //(
            //        [TabID] [int],
            //        [TabName] [nvarchar] (100),
            //        [ParentTabID] [int],
            //        [TabOrder] [int],
            //        [NestLevel] [int],
            //        [TreeOrder] [varchar] (1000)
            //)
            int lastLevel = 0;
            //DECLARE @LastLevel smallint
            //SET @LastLevel = 0

            //-- First, the parent levels
            //INSERT INTO     #TabTree
            var qParents = from p in db.Pages
                           where p.ParentId == null && p.PortalId == portalID
                           orderby p.Order
                           select new Page
                           {
                               Id = p.Id,
                               Name = p.Name,
                               ParentId = p.ParentId,
                               Order = p.Order,
                               NestLevel = 0,
                               Description = (100000000 + p.Order).ToString()
                           };
            //SELECT  TabID,
            //        TabName,
            //        ParentTabID,
            //        TabOrder,
            //        0,
            //        cast(100000000 + TabOrder as varchar)
            //FROM    rb_Tabs
            //WHERE   ParentTabID IS NULL AND PortalID =@PortalID
            //ORDER BY TabOrder
            pageTree.AddRange(qParents);

            foreach (var page in qParents)
            {
                //-- Next, the children levels
                //WHILE (@@rowcount > 0)
                //BEGIN
                lastLevel = lastLevel + 1;
                //  SET @LastLevel = @LastLevel + 1
                //  INSERT        #TabTree (TabID, TabName, ParentTabID, TabOrder, NestLevel, TreeOrder) 
                var qChildren = from p in page.Pages
                                where p.NestLevel == lastLevel - 1
                                orderby p.Order
                                select new Page
                                {
                                    Id = p.Id,
                                    Name = delegate()
                                    {
                                        string d = string.Empty;
                                        for (int i = 0; i < lastLevel * 2; i++) d = d + "-";
                                        d = d + p.Name;
                                    },
                                    ParentId = p.ParentId,
                                    Order = p.Order,
                                    NestLevel = lastLevel,
                                    Description = p.Description + "." + (100000000 + p.Order).ToString()
                                };
                //                SELECT  rb_Tabs.TabID,
                //                        Replicate('-', @LastLevel *2) + rb_Tabs.TabName,
                //                        rb_Tabs.ParentTabID,
                //                        rb_Tabs.TabOrder,
                //                        @LastLevel,
                //                        cast(#TabTree.TreeOrder as varchar) + '.' + cast(100000000 + rb_Tabs.TabOrder as varchar)
                //                FROM    rb_Tabs join #TabTree on rb_Tabs.ParentTabID= #TabTree.TabID
                //                WHERE   EXISTS (SELECT 'X' FROM #TabTree WHERE TabID = rb_Tabs.ParentTabID AND NestLevel = @LastLevel - 1)
                //                 AND PortalID =@PortalID
                //                ORDER BY #TabTree.TabOrder
                pageTree.AddRange(qChildren);
                //END
            }

            //--Get the Orphans
            //  INSERT        #TabTree (TabID, TabName, ParentTabID, TabOrder, NestLevel, TreeOrder) 
            var qOrphans = from p in db.Pages
                           where !pageTree.Contains(p)
                           select new Page
                           {
                               Id = p.Id,
                               Name = "(Orphan)" + p.Name,
                               ParentId = p.ParentId,
                               Order = p.Order,
                               NestLevel = 999999999,
                               Description = "999999999"
                           };
            //                SELECT  rb_Tabs.TabID,
            //                        '(Orphan)' + rb_Tabs.TabName,
            //                        rb_Tabs.ParentTabID,
            //                        rb_Tabs.TabOrder,
            //                        999999999,
            //                        '999999999'
            //                FROM    rb_Tabs 
            //                WHERE   NOT EXISTS (SELECT 'X' FROM #TabTree WHERE TabID = rb_Tabs.TabID)
            //                         AND PortalID =@PortalID
            pageTree.AddRange(qOrphans);

            //-- Reorder the tabs by using a 2nd Temp table AND an identity field to keep them straight.
            pageTree.Sort();
            //select IDENTITY(int,1,2) as ord , cast(TabID as varchar) as TabID into #tabs
            //from #TabTree
            //order by nestlevel, TreeOrder
            //-- Change the TabOrder in the sirt temp table so that tabs are ordered in sequence
            //update #TabTree set TabOrder=(select ord from #tabs WHERE cast(#tabs.TabID as int)=#TabTree.TabID) 
            //-- Return Temporary Table
            //SELECT TabID, parenttabID, tabname, TabOrder, NestLevel
            //FROM #TabTree 
            //order by TreeOrder
            return pageTree;
        }

        #endregion


    }
}
