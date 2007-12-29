using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Configuration.Cache;
using System.Threading;
using System.Web;
using Rainbow.Framework.Design;
using System.IO;
using System.Xml;
using Rainbow.Framework.Data.Types;
using System.Data.Linq;
using Rainbow.Framework.Configuration;
using System.Globalization;
using System.Collections;
using Rainbow.Framework.Data.Providers;
using Rainbow.Framework.Data.MsSql.Debugger;
using Rainbow.Framework.Data.Entities;

namespace Rainbow.Framework.Data.MsSql
{
    partial class Page : IPage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                var spn = (from ps in this.PageSettings
                           where ps.SettingName == this.Portal.PortalContentLanguage.ToString()
                           select ps.SettingValue).SingleOrDefault();

                return string.IsNullOrEmpty(spn) ? this.PageName : spn;
            }
            set
            {
                var pageName = (from ps in this.PageSettings
                                where ps.SettingName == this.Portal.PortalContentLanguage.ToString()
                                select ps).SingleOrDefault();

                if (pageName.SettingValue != value)
                {
                    pageName.SettingValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom menu image.
        /// </summary>
        /// <value>The custom menu image.</value>
        public string MenuImage
        {
            get
            {
                return this.CustomSettings.SingleOrDefault(n => n.EnglishName == "CustomMenuImage").SettingValue;
            }
            set
            {
                var cmi = (from ps in this.PageSettings
                           where ps.SettingName == "CustomMenuImage"
                           select ps).SingleOrDefault();

                if (cmi.SettingValue != value)
                {
                    cmi.SettingValue = value;
                }
            }
        }

        #region Page Settings

        const string strSiteSettingsPageLayout = "SITESETTINGS_PAGE_LAYOUT";
        /// <summary>
        /// Gets or sets the page layout.
        /// </summary>
        /// <value>The page layout.</value>
        public string Layout
        {
            get
            {
                return GetSettingValue(strSiteSettingsPageLayout);
            }
            set
            {
                SetSettingValue(strSiteSettingsPageLayout, value);
            }
        }

        const string strPageNestLevel = "PAGE_NEST_LEVEL";
        /// <summary>
        /// NestLevel
        /// </summary>
        /// <value>The nest level.</value>
        public int NestLevel
        {
            get
            {
                return Int32.Parse(GetSettingValue(strPageNestLevel));
            }
            set
            {
                SetSettingValue(strPageNestLevel, value);
            }
        }

        #region Page Setting Access Methods

        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns></returns>
        public string GetSettingValue(string settingName)
        {
            return this.PageSettings.SingleOrDefault(ps => ps.SettingName == settingName).SettingValue;
        }

        /// <summary>
        /// Sets the setting value.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="value">The value.</param>
        public void SetSettingValue(string settingName, object value)
        {
            var setting = this.PageSettings.SingleOrDefault(ps => ps.SettingName == settingName);

            if (setting.SettingValue != value.ToString())
            {
                this.OnSettingChanging(value.ToString());
                this.SendPropertyChanging();

                setting.PageId = this.PageId;
                setting.SettingName = settingName;
                setting.SettingValue = value.ToString();

                this.SendPropertyChanged(settingName);
                this.OnSettingChanged();
            }

            //TODO: check whether this is needed or if it is handled by the above block automatically...
            UpdatePageSetting(this.PageId, settingName, value.ToString());

            //Invalidate cache
            if (CurrentCache.Exists(Key.TabSettings(this.PageId)))
                CurrentCache.Remove(Key.TabSettings(this.PageId));

            // Clear url builder elements
            HttpUrlBuilder.Clear(this.PageId);
        }
        partial void OnSettingChanging(string value);
        partial void OnSettingChanged();

        /// <summary>
        /// Update Page Setting In Database
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdatePageSetting(int pageId, string key, string value)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();
            var q = db.PageSettings.SingleOrDefault(p => p.PageId == pageId && p.SettingName == key);

            if (q.SettingValue != value)
            {
                q.PageId = pageId;
                q.SettingName = key;
                q.SettingValue = value;

                db.SubmitChanges();

                //Invalidate cache
                if (CurrentCache.Exists(Key.TabSettings(pageId)))
                    CurrentCache.Remove(Key.TabSettings(pageId));

                // Clear url builder elements
                HttpUrlBuilder.Clear(pageId);
            }
        }

        #endregion

        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hashtable of
        /// custom Page specific settings from the database. This method is
        /// used by Portals to access misc Page settings.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public EntitySet<PageSetting> CustomSettings
        {
            get
            {
                if (!CurrentCache.Exists(Key.TabSettings(this.PageId)))
                {
                    using (DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString))
                    {
                        db.Log = new DebuggerWriter();

                        foreach (var bs in this.BaseSettings)
                        {
                            if (db.PageSettings.Count(set => set.SettingName == bs.EnglishName) > 0)
                                db.PageSettings.Single(set => set.SettingName == bs.EnglishName).SettingValue = bs.SettingValue;
                            else
                            {
                                if (db.PageSettings.Count(set =>
                                    (set.SettingName == bs.SettingName || set.EnglishName == bs.EnglishName)) < 1)
                                {
                                    var ps = db.PageSettings.SingleOrDefault(set => set.SettingName == bs.EnglishName);
                                    ps.BaseSetting = bs;
                                    ps.DataType = bs.DataType;
                                    ps.Description = bs.Description;
                                    ps.EnglishName = bs.EnglishName;
                                    ps.IsRequired = bs.IsRequired;
                                    ps.MaxValue = bs.MaxValue;
                                    ps.MinValue = bs.MinValue.Value;
                                    ps.PageId = this.PageId;
                                    ps.SettingGroupId = bs.SettingGroupId;
                                    ps.SettingName = bs.SettingName;
                                    ps.SettingOrder = bs.SettingOrder;
                                    ps.SettingValue = bs.SettingValue;
                                }
                            }
                        }

                        db.SubmitChanges();
                    }

                    CurrentCache.Insert(Key.TabSettings(this.PageId), this.PageSettings);
                }
                else
                {
                    return (EntitySet<PageSetting>)CurrentCache.Get(Key.TabSettings(this.PageId));
                }
                return this.PageSettings;
            }
            set
            {
                this.PageSettings = value;
            }
        }

        /// <summary>
        /// Changed by Thierry@tiptopweb.com.au
        /// Page are different for custom page layout an theme, this cannot be static
        /// Added by john.mandia@whitelightsolutions.com
        /// Cache by Manu
        /// non static function, Thierry : this is necessary for page custom layout and themes
        /// </summary>
        /// <value>The base settings.</value>
        /// <returns></returns>
        private IQueryable<BaseSetting> BaseSettings
        {
            get
            {
                using (DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString))
                {
                    db.Log = new DebuggerWriter();

                    var qbs = db.BaseSettings.Where(bs => bs.BaseSettingType.BaseSettingTypeName == "Page");

                    #region Layout and Theme

                    //TODO: Figure out the best way to handle these settings of CustomListDataType... maybe push the values into xml and store them that way.

                    //// get the list of available layouts
                    //// changed: Jes1111 - 2004-08-06
                    //ArrayList layoutsList = new ArrayList(new LayoutManager(this.Portal.PortalPathRelative).GetLayouts());
                    //LayoutItem _noCustomLayout = new LayoutItem();
                    //_noCustomLayout.Name = string.Empty;
                    //layoutsList.Insert(0, _noCustomLayout);
                    //// get the list of available themes
                    //// changed: Jes1111 - 2004-08-06
                    //ArrayList themesList = new ArrayList(new ThemeManager(this.Portal.PortalPathRelative).GetThemes());
                    //ThemeItem _noCustomTheme = new ThemeItem();
                    //_noCustomTheme.Name = string.Empty;
                    //themesList.Insert(0, _noCustomTheme);
                    //// changed: Jes1111 - 2004-08-06
                    //Setting CustomLayout = new Setting(new CustomListDataType(layoutsList, "Name", "Name"));
                    //CustomLayout.Group = _Group;
                    //CustomLayout.Order = _groupOrderBase + 11;
                    //CustomLayout.EnglishName = "Custom Layout";
                    //CustomLayout.Description = "Set a custom layout for this tab only";
                    //_baseSettings.Add("CustomLayout", CustomLayout);
                    ////Setting CustomTheme = new Setting(new StringDataType());
                    //// changed: Jes1111 - 2004-08-06
                    //Setting CustomTheme = new Setting(new CustomListDataType(themesList, "Name", "Name"));
                    //CustomTheme.Group = _Group;
                    //CustomTheme.Order = _groupOrderBase + 12;
                    //CustomTheme.EnglishName = "Custom Theme";
                    //CustomTheme.Description = "Set a custom theme for the modules in this tab only";
                    //_baseSettings.Add("CustomTheme", CustomTheme);
                    ////Setting CustomThemeAlt = new Setting(new StringDataType());
                    //// changed: Jes1111 - 2004-08-06
                    //Setting CustomThemeAlt = new Setting(new CustomListDataType(themesList, "Name", "Name"));
                    //CustomThemeAlt.Group = _Group;
                    //CustomThemeAlt.Order = _groupOrderBase + 13;
                    //CustomThemeAlt.EnglishName = "Custom Alt Theme";
                    //CustomThemeAlt.Description = "Set a custom alternate theme for the modules in this tab only";
                    //_baseSettings.Add("CustomThemeAlt", CustomThemeAlt);

                    //Setting CustomMenuImage = new Setting(new CustomListDataType(GetImageMenu(), "Key", "Value"));
                    //CustomMenuImage.Group = _Group;
                    //CustomMenuImage.Order = _groupOrderBase + 14;
                    //CustomMenuImage.EnglishName = "Custom Image Menu";
                    //CustomMenuImage.Description = "Set a custom menu image for this tab";
                    //_baseSettings.Add("CustomMenuImage", CustomMenuImage);

                    #endregion

                    #region Language/Culture Management

                    var cultureSettingGroup = db.SettingGroups.Single(sg => sg.SettingGroupName == "CULTURE_SETTINGS");

                    List<CultureInfo> cultureList = Portal.GetLanguageList(true);
                    int counter = 1;
                    foreach (CultureInfo c in cultureList)
                    {
                        //Ignore invariant
                        if (c != CultureInfo.InvariantCulture && !(qbs.Where(bs => bs.EnglishName.Contains(c.Name)).Count() > 0))
                        {
                            var q = db.BaseSettings.SingleOrDefault(bs => bs.EnglishName == "Tab Key Phrase (" + c.Name + ")");
                            q.BaseSettingTypeId = 1;
                            q.SettingName = "TabKeyPhrase_" + c.Name;
                            q.EnglishName = "Tab Key Phrase (" + c.Name + ")";
                            q.SettingOrder = counter;
                            q.SettingGroupId = cultureSettingGroup.SettingGroupId;
                            q.Description = "Key Phrase this Tab/Page for " + c.EnglishName + " culture.";

                            var q1 = db.BaseSettings.SingleOrDefault(bs => bs.EnglishName == "Title (" + c.Name + ")");
                            q1.BaseSettingTypeId = 1;
                            q1.SettingName = c.Name;
                            q1.EnglishName = "Title (" + c.Name + ")";
                            q1.SettingOrder = counter;
                            q1.SettingGroupId = cultureSettingGroup.SettingGroupId;
                            q1.Description = "Set title for " + c.EnglishName + " culture.";

                            db.SubmitChanges(); //save additions/updates to database

                            counter++;
                        }
                    }

                    #endregion

                    return qbs;
                }
            }
        }

        #endregion

        /// <summary>
        /// LayoutPath is the full path in which all Layout files are stored
        /// </summary>
        /// <value>The layout path.</value>
        public string LayoutPath
        {
            get
            {
                //Grab the portal's layout
                string ThisLayoutPath = this.Portal.CurrentLayout;

                var qPageSettings = from ps in this.PageSettings
                                    where ps.PageId == this.PageId
                                    where ps.SettingName == "CustomLayout"
                                    select ps;

                if (qPageSettings.Count() > 0)
                {
                    ThisLayoutPath = qPageSettings.Single().SettingValue;
                }

                // Try to get layout from querystring
                if (HttpContext.Current != null && HttpContext.Current.Request.Params["Layout"] != null)
                    ThisLayoutPath = HttpContext.Current.Request.Params["Layout"];

                // yiming, 18 Aug 2003, get layout from portalWebPath, if no, then WebPath
                LayoutManager layoutManager = new LayoutManager(this.Portal.PortalPath);

                if (Directory.Exists(layoutManager.PortalLayoutPath + "/" + ThisLayoutPath + "/"))
                    return layoutManager.PortalWebPath + "/" + ThisLayoutPath + "/";

                else
                    return LayoutManager.WebPath + "/" + ThisLayoutPath + "/";
                // end yiming, 18 Aug 2003

                //				//by manu
                //				string layoutManagerPath = System.IO.Path.Combine(layoutManager.PortalLayoutPath, ThisLayoutPath);
                //
                //				if (Directory.Exists(layoutManagerPath))                   
                //					return Rainbow.Framework.Configuration.Path.WebPathCombine(layoutManager.PortalWebPath, ThisLayoutPath);
                //				else
                //					return Rainbow.Framework.Configuration.Path.WebPathCombine(LayoutManager.WebPath, ThisLayoutPath);
            }
        }

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        /// <param name="requiredTheme">The required theme.</param>
        /// <returns></returns>
        public Theme CurrentTheme(string requiredTheme)
        {
            switch (requiredTheme)
            {
                case "Alt":
                    // look for an alternate custom theme
                    var alt = (from cs in Portal.CustomSettings(this.PortalId)
                               where !string.IsNullOrEmpty(cs.SettingValue)
                               where cs.SettingName == "CustomThemeAlt"
                               select cs.SettingValue);

                    if (alt.Count() > 0)
                    {
                        ThemeManager themeManager = new ThemeManager(this.Portal.PortalPathRelative);
                        themeManager.Load(alt.Single());
                        return themeManager.CurrentTheme;
                    }

                    // no custom theme
                    return Portal.CurrentThemeAlternate;

                default:
                    // look for a custom theme
                    var def = (from cs in Portal.CustomSettings(this.PortalId)
                               where !string.IsNullOrEmpty(cs.SettingValue)
                               where cs.SettingName == "CustomTheme"
                               select cs.SettingValue);

                    if (def.Count() > 0)
                    {
                        ThemeManager themeManager = new ThemeManager(this.Portal.PortalPathRelative);
                        themeManager.Load(def.Single());
                        return themeManager.CurrentTheme;
                    }

                    // no custom theme
                    return Portal.CurrentThemeDefault;
            }
        }

        /// <summary>
        /// Theme definition and images
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// -- Thierry (Tiptopweb), 21 Jun 2003 [START] 
        /// -- Thierry (Tiptopweb),  3 Feb 2004, fixed mismatch Alt and Default theme (Alt always returned)
        /// Switch the Theme if a custom theme is defined in the tab settings
        /// (using custom themes from PageSettings.cs)
        /// if not use the theme defined from the portalsettings
        /// </remarks>
        public Theme CurrentTheme()
        {
            // look for a custom theme
            var def = (from cs in Portal.CustomSettings(this.PortalId)
                       where !string.IsNullOrEmpty(cs.SettingValue)
                       where cs.SettingName == "CustomTheme"
                       select cs.SettingValue);

            if (def.Count() > 0)
            {
                ThemeManager themeManager = new ThemeManager(this.Portal.PortalPathRelative);
                themeManager.Load(def.Single());
                return themeManager.CurrentTheme;
            }

            // no custom theme
            return Portal.CurrentThemeDefault;
        }

        /// <summary>
        /// Gets or sets the active module.
        /// </summary>
        /// <value>The active module.</value>
        public int ActiveModule
        {
            get
            {
                if (HttpContext.Current.Request.Params["mID"] != null)
                {
                    SetActiveModuleCookie(int.Parse(HttpContext.Current.Request.Params["mID"]));
                    return int.Parse(HttpContext.Current.Request.Params["mID"]);
                }

                if (HttpContext.Current.Request.Cookies["ActiveModule"] != null)
                    return int.Parse(HttpContext.Current.Request.Cookies["ActiveModule"].Value);
                return 0;
            }
            set { SetActiveModuleCookie(value); }
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public IEnumerable<Page> MenuGroup
        {
            get
            {
                string cacheKey = Key.TabNavigationSettings(this.PageId, Thread.CurrentThread.CurrentUICulture.ToString());
                IEnumerable<Page> tabs = null;

                if (!CurrentCache.Exists(cacheKey))
                {
                    tabs = this.Pages;
                    CurrentCache.Insert(cacheKey, tabs);
                }
                else
                {
                    tabs = (IEnumerable<Page>)CurrentCache.Get(cacheKey);
                }
                return tabs;
            }
        }

        #region Static Methods

        /// <summary>
        /// The GetRootPage should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public static Page GetRootPage(Page page, IEnumerable<Page> pageList)
        {
            return GetRootPage(page.PageId, pageList);
        }

        /// <summary>
        /// The get tab root should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public static Page GetRootPage(int parentPageId, IEnumerable<Page> pageList)
        {
            //Changes Indah Fuldner 25.04.2003 (With assumtion that the rootlevel tab has ParentPageID = 0)
            //Search for the root tab in current array

            foreach (Page rootPage in pageList)
            {
                // return rootPage;
                if (rootPage.PageId == parentPageId)
                {
                    parentPageId = rootPage.ParentPageId.Value;
                    //string parentName=rootPage.PageName;

                    //if (parentPageId != 0)
                    //    i = -1;
                    //else
                    if (parentPageId == 0)
                        return rootPage;
                }
            }

            //End Indah Fuldner
            throw new ArgumentOutOfRangeException("Page", "Root not found");
        }

        #endregion

        /// <summary>
        /// Recurses the portal pages XML.
        /// </summary>
        /// <param name="myPage">My page.</param>
        /// <param name="writer">The writer.</param>
        private void RecursePagesXml(Page myPage, XmlTextWriter writer)
        {
            bool _groupElementWritten = false;

            foreach (Page mySubPage in myPage.Pages)
            {
                //if ( mySubPage.ParentPageID == myPage.PageID && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
                if (mySubPage.ParentPageId == this.PageId)
                {
                    if (!_groupElementWritten)
                    {
                        writer.WriteStartElement("MenuGroup"); // start MenuGroup element
                        _groupElementWritten = true;
                    }

                    writer.WriteStartElement("MenuItem"); // start MenuItem element
                    writer.WriteAttributeString("ParentPageId", mySubPage.ParentPageId.ToString());
                    //writer.WriteAttributeString("Label",mySubPage.PageName);

                    if (HttpUrlBuilder.UrlPageName(mySubPage.PageId) == HttpUrlBuilder.DefaultPage)
                        writer.WriteAttributeString("UrlPageName", mySubPage.Name);
                    else
                        writer.WriteAttributeString("UrlPageName", HttpUrlBuilder.UrlPageName(mySubPage.PageId).Replace(".aspx", ""));

                    writer.WriteAttributeString("PageName", mySubPage.Name);

                    writer.WriteAttributeString("PageOrder", mySubPage.PageOrder.ToString());
                    //writer.WriteAttributeString("PageIndex", mySubPage.PageIndex.ToString());
                    writer.WriteAttributeString("PageLayout", mySubPage.PageLayout.Value.ToString());
                    writer.WriteAttributeString("AuthRoles", mySubPage.AuthorizedRoles);
                    writer.WriteAttributeString("ID", mySubPage.PageId.ToString());
                    //writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",mySubPage.PageName,".aspx"), mySubPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                    RecursePagesXml(mySubPage, writer);
                    writer.WriteEndElement(); // end MenuItem element
                }
            }

            if (_groupElementWritten)
                writer.WriteEndElement(); // end MenuGroup element
        }

        partial void OnCreated()
        {

        }



        /// <summary>
        /// Gets the image menu.
        /// </summary>
        /// <returns>A System.Collections.Hashtable value...</returns>
        private Hashtable GetImageMenu()
        {
            Hashtable imageMenuFiles;

            if (!CurrentCache.Exists(Key.ImageMenuList(Portal.CurrentLayout)))
            {
                imageMenuFiles = new Hashtable();
                imageMenuFiles.Add("-Default-", string.Empty);
                string menuDirectory = string.Empty;
                LayoutManager layoutManager = new LayoutManager(this.Portal.PortalPathRelative);

                menuDirectory = Rainbow.Framework.Configuration.Path.WebPathCombine(layoutManager.PortalLayoutPath, Portal.CurrentLayout);
                if (Directory.Exists(menuDirectory))
                {
                    menuDirectory = Rainbow.Framework.Configuration.Path.WebPathCombine(menuDirectory, "menuimages");
                }
                else
                {
                    menuDirectory = Rainbow.Framework.Configuration.Path.WebPathCombine(LayoutManager.Path, Portal.CurrentLayout, "menuimages");
                }

                if (Directory.Exists(menuDirectory))
                {
                    FileInfo[] menuImages = (new DirectoryInfo(menuDirectory)).GetFiles("*.gif");

                    foreach (FileInfo fi in menuImages)
                    {
                        if (fi.Name != "spacer.gif" && fi.Name != "icon_arrow.gif")
                            imageMenuFiles.Add(fi.Name, fi.Name);
                    }
                }
                CurrentCache.Insert(Key.ImageMenuList(Portal.CurrentLayout), imageMenuFiles, null);
            }
            else
            {
                imageMenuFiles = (Hashtable)CurrentCache.Get(Key.ImageMenuList(Portal.CurrentLayout));
            }
            return imageMenuFiles;
        }


        #region GetDesktopPagesXml

        //		/// <summary>
        //		/// Massages existing DesktopPages ArrayList a bit, 
        //		/// so that it can be serialized into an XPathDocument
        //		/// which can be used to retrieve tabs hierarchy data
        //		/// without hitting the database
        //		/// </summary>
        //		/// <returns>xpd</returns>
        //		/// <remarks>
        //		///  Jes1111
        //		/// </remarks>
        //		public XPathDocument GetDesktopPagesXml()
        //		{
        //			// make a new PagesBox, because we want the top level PageStripDetails
        //			// to be inside a PagesBox, for the serialization
        //			PagesBox menuGroup = new PagesBox();
        //			// create a MenuData object to hold the PagesBox
        //			MenuData md = new MenuData();
        //			// transfer DesktopPages to menuGroup
        //			//			foreach (PageStripDetails t in this.DesktopPages)
        //			//				menuGroup.Add(t);
        //			int tabCount = DesktopPages.Count;
        //
        //			for (int i = 0; i < tabCount; i++)
        //				menuGroup.Add((PageStripDetails) DesktopPages[i]);
        //			// Put the PagesBox into its holder
        //			md.PagesBox = menuGroup;
        //			// Create a Type array for the MenuData object
        //			Type[] extraTypes = new Type[2];
        //			extraTypes[0] = typeof (PagesBox);
        //			extraTypes[1] = typeof (PageStripDetails);
        //			// serialize the MenuData object
        //			XmlSerializer serializer = new XmlSerializer(typeof (MenuData), extraTypes);
        //
        //			using (TextWriter tw = new StringWriter())
        //			{
        //				XmlWriter xw = new XmlTextWriter(tw);
        //				serializer.Serialize(xw, md);
        //				// create the XPathDocument
        //				XPathDocument xpd = new XPathDocument(new XmlTextReader(new StringReader(tw.ToString())));
        //				return xpd;
        //			}
        //		}

        #endregion


        /// <summary>
        /// Does the products module exist in page?
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        [History("Bill - bill@improvtech.com", "2007/12/16", "Updated to use LINQ"),
         History("Bill - bill@improvtech.com", "2007/12/26", "Moved to Page from ModulesDB")]
        public static bool DoesProductsModuleExistInPage(int pageID, int portalID)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);

            Guid moduleGuid = new Guid("{EC24FABD-FB16-4978-8C81-1ADD39792377}");

            return (db.Modules.Count(m => m.PageId == pageID
                && m.Page.PortalId == portalID
                && m.ModuleDefinition.GeneralModDefId == moduleGuid) > 0);
        }

        /// <summary>
        /// Sets the active module cookie.
        /// </summary>
        /// <param name="mID">The m ID.</param>
        public void SetActiveModuleCookie(int mID)
        {
            HttpCookie cookie;
            DateTime time;
            TimeSpan span;
            cookie = new HttpCookie("ActiveModule", mID.ToString());
            time = DateTime.Now;
            span = new TimeSpan(0, 2, 0, 0, 0); // 120 minutes to expire
            cookie.Expires = time.Add(span);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        #region IPage Members

        IEnumerable<IPage> IPage.MenuGroup
        {
            get { return this.MenuGroup as IEnumerable<IPage>; }
        }

        IEnumerable<IModule> IPage.Modules
        {
            get
            {
                return this.Modules as IEnumerable<IModule>;
            }
            set
            {
                this.Modules = value as IEnumerable<Module>;
            }
        }

        IEnumerable<IPage> IPage.Pages
        {
            get
            {
                return this.Pages as IEnumerable<IPage>;
            }
            set
            {
                this.Pages = value as IEnumerable<Page>;
            }
        }

        IEnumerable<IPageSetting> IPage.PageSettings
        {
            get
            {
                return this.PageSettings as IEnumerable<IPageSetting>;
            }
            set
            {
                this.PageSettings = value as IEnumerable<PageSetting>;
            }
        }

        IPage IPage.ParentPage
        {
            get
            {
                return this.ParentPage as IPage;
            }
            set
            {
                this.ParentPage = value as Page;
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="value"/> is not the same type as this instance. </exception>
        public int CompareTo(object value)
        {
            if (value == null) return 1;

            int compareOrder = ((Page)value).PageOrder;

            if (this.PageOrder == compareOrder) return 0;
            if (this.PageOrder < compareOrder) return -1;
            if (this.PageOrder > compareOrder) return 1;
            return 0;
        }

        #endregion

        #region IComparable<IPage> Members

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public int CompareTo(IPage other)
        {
            if (other == null) return 1;

            int compareOrder = (other as Page).Order;
            if (this.Order == compareOrder) return 0;
            if (this.Order < compareOrder) return -1;
            if (this.Order > compareOrder) return 1;
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
