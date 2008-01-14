using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Core.Configuration.Settings.Providers; //PortalPageProvider
using Rainbow.Framework.Design;
using Rainbow.Framework.Providers; //PortalProvider, HttpUrlBuilder
using Path = Rainbow.Framework.Path;

using Rainbow.Framework.DataTypes; //PortalUrl
using Rainbow.Framework.Site.Configuration; //PortalPage

#warning move to appropriate namespace
namespace Rainbow.Framework.Core.Configuration.Settings
{
    /// <summary>
    /// PortalSettings Class encapsulates all of the settings 
    /// for the Portal, as well as the configuration settings required 
    /// to execute the current tab view within the portal.
    /// </summary>
    [History("moudrick", "2007/11/16", "extracting provider")]
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [History("gman3001", "2004/09/29",
        "Added the GetCurrentUserProfile method to obtain a hashtable of the current user's profile details."
        )]
    [History("jviladiu@portalServices.net", "2004/08/19",
        "Add support for move & delete module roles")]
    [History("jviladiu@portalServices.net", "2004/07/30", "Added new ActiveModule property")]
    [History("Jes1111", "2003/03/09", "Added new ShowTabs property")]
    [History("Jes1111", "2003/04/02", "Added new DesktopTabsXml property (an XPathDocument)")]
    [History("Thierry", "2003/04/12", "Added PortalSecurePath property")]
    [History("Jes1111", "2003/04/17", "Added new language-related properties and methods")]
    [History("Jes1111", "2003/04/23",
        "Corrected string comparison case problem in language settings")]
    [History("cisakson@yahoo.com", "2003/04/28",
        "Added a custom setting for Windows users to assign a portal Admin")]
    public class Portal
    {
        const string strCustomLayout = "CustomLayout";
        const string strCustomTheme = "CustomTheme";

        readonly string portalPathPrefix;

        readonly PortalPage activePage = new PortalPage();
        ArrayList desktopPages = new ArrayList();
        ArrayList mobilePages = new ArrayList();
        Hashtable customSettings;

        string portalAlias;
        int portalID;
        string portalName;
        string portalTitle;
        bool showPages = true;
        string currentLayout;
        string portalPath = string.Empty;
        string portalSecurePath;
        XmlDocument portalPagesXml;
        Theme currentThemeAlt;
        Theme currentThemeDefault;

        ///<summary>
        /// Gets portal url of the portal
        ///</summary>
        public readonly PortalUrl PortalUrl;

        /// <summary>
        /// Gets the get terms of service.
        /// </summary>
        /// <value>The get terms of service.</value>
        public string GetTermsOfService
        {
            get
            {
                string termsOfService = string.Empty;

                //Verify if we have to show conditions
                if (CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"] != null &&
                    CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString().Length != 0)
                {
                    //				// Attempt to load the required text
                    //				Rainbow.Framework.DataTypes.PortalUrl pt = new Rainbow.Framework.DataTypes.PortalUrl();
                    //				pt.Value = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
                    //				string terms = HttpContext.Current.Server.MapPath(pt.FullPath);
                    //				
                    //				//Try to get localized version
                    //				string localized_terms;
                    //				localized_terms = terms.Replace(".", "_" + Esperantus.Localize.GetCurrentUINeutralCultureName() + ".");
                    //				if (System.IO.File.Exists(localized_terms))
                    //					terms = localized_terms;
                    //Fix by Joerg Szepan - jszepan 
                    // http://sourceforge.net/tracker/index.php?func=detail&aid=852071&group_id=66837&atid=515929
                    // Wrong Terms-File if Dot in Mappath
                    // Attempt to load the required text
                    string terms;
                    terms = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
                    //Try to get localized version
                    string localized_terms = "";
                    // TODO: FIX THIS
                    // localized_terms = terms.Replace(".", "_" + Localize.GetCurrentUINeutralCultureName() + ".");

                    PortalUrl portalUrl = PortalUrl;
                    portalUrl.Value = localized_terms;

                    if (File.Exists(HttpContext.Current.Server.MapPath(portalUrl.FullPath)))
                    {
                        terms = localized_terms;
                    }
                    portalUrl.Value = terms;
                    terms = HttpContext.Current.Server.MapPath(portalUrl.FullPath);

                    //Load conditions
                    if (File.Exists(terms))
                    {
                        //Try to open file
                        using (StreamReader s = new StreamReader(terms, Encoding.Default))
                        {
                            //Get the text of the conditions
                            termsOfService = s.ReadToEnd();
                            // Close Streamreader
                            s.Close();
                        }
                    }
                    else
                    {
                        //If load fails use default
                        termsOfService = "'" + terms + "' not found!";
                    }
                }
                //end Fix by Joerg Szepan - jszepan 
                return termsOfService;
            }
        }

        /// <summary>
        /// PortalLayoutPath is the full path in which all Layout files are
        /// </summary>
        /// <value>The portal layout path.</value>
        public string PortalLayoutPath
        {
            get
            {
                string thisLayoutPath = CurrentLayout;
                string customLayout = string.Empty;

                // Thierry (Tiptopweb), 4 July 2003, switch to custom Layout
                if (ActivePage.CustomSettings[strCustomLayout] != null &&
                    ActivePage.CustomSettings[strCustomLayout].ToString().Length > 0)
                {
                    customLayout = ActivePage.CustomSettings[strCustomLayout].ToString();
                }

                if (customLayout.Length != 0)
                {
                    // we have a custom Layout
                    thisLayoutPath = customLayout;
                }

                // Try to get layout from querystring
                if (HttpContext.Current != null &&
                    HttpContext.Current.Request.Params["Layout"] != null)
                {
                    thisLayoutPath = HttpContext.Current.Request.Params["Layout"];
                }
                // yiming, 18 Aug 2003, get layout from portalWebPath, if no, then WebPath
                LayoutManager layoutManager = new LayoutManager(PortalPath);

                if (Directory.Exists(layoutManager.PortalLayoutPath + "/" + thisLayoutPath + "/"))
                {
                    return layoutManager.PortalWebPath + "/" + thisLayoutPath + "/";
                }
                else
                {
                    return LayoutManager.WebPath + "/" + thisLayoutPath + "/";
                }
                // end yiming, 18 Aug 2003
                //				//by manu
                //				string layoutManagerPath = System.IO.Path.Combine(layoutManager.PortalLayoutPath, ThisLayoutPath);
                //
                //				if (Directory.Exists(layoutManagerPath))                   
                //					return Rainbow.Framework.Settings.Path.WebPathCombine(layoutManager.PortalWebPath, ThisLayoutPath);
                //				else
                //					return Rainbow.Framework.Settings.Path.WebPathCombine(LayoutManager.WebPath, ThisLayoutPath);
            }
        }

        /// <summary>
        /// Gets the portal pages XML.
        /// </summary>
        /// <value>The portal pages XML.</value>
        public XmlDocument PortalPagesXml
        {
            get
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlTextWriter writer = new XmlTextWriter(sw);
                    writer.Formatting = Formatting.None;
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("MenuData"); // start MenuData element
                    writer.WriteStartElement("MenuGroup"); // start top MenuGroup element

                    for (int i = 0; i < DesktopPages.Count; i++)
                    {
                        PageStripDetails myPage = (PageStripDetails) DesktopPages[i];

                        //if ( myPage.ParentPageID == 0 && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
                        if (myPage.ParentPageID == 0)
                        {
                            writer.WriteStartElement("MenuItem"); // start MenuItem element
                            writer.WriteAttributeString("ParentPageId",
                                                        myPage.ParentPageID.ToString());

                            if (HttpUrlBuilder.UrlPageName(myPage.PageID) ==
                                HttpUrlBuilder.DefaultPage)
                            {
                                writer.WriteAttributeString("UrlPageName", myPage.PageName);
                            }
                            else
                            {
                                writer.WriteAttributeString("UrlPageName",
                                                            HttpUrlBuilder.UrlPageName(myPage.PageID)
                                                                .Replace(".aspx", ""));
                            }

                            writer.WriteAttributeString("PageName", myPage.PageName);

                            //writer.WriteAttributeString("Label",myPage.PageName);
                            writer.WriteAttributeString("PageOrder", myPage.PageOrder.ToString());
                            writer.WriteAttributeString("PageIndex", myPage.PageIndex.ToString());
                            writer.WriteAttributeString("PageLayout", myPage.PageLayout);
                            writer.WriteAttributeString("AuthRoles", myPage.AuthorizedRoles);
                            writer.WriteAttributeString("ID", myPage.PageID.ToString());
                            //writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",myPage.PageName,".aspx"),myPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                            RecursePortalPagesXml(myPage, writer);
                            writer.WriteEndElement(); // end MenuItem element
                        }
                    }
                    writer.WriteEndElement(); // end top MenuGroup element
                    writer.WriteEndElement(); // end MenuData element
                    writer.Flush();
                    portalPagesXml = new XmlDocument();
                    portalPagesXml.LoadXml(sw.ToString());
                    writer.Close();
                }
                return portalPagesXml;
            }
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
                {
                    return int.Parse(HttpContext.Current.Request.Cookies["ActiveModule"].Value);
                }
                return 0;
            }
            set { SetActiveModuleCookie(value); }
        }

        /// <summary>
        /// Current Layout
        /// </summary>
        /// <value>The current layout.</value>
        public string CurrentLayout
        {
            get
            {
                //Patch for possible .NET framework bug
                //if returned an empty string caused an endless loop
                if (currentLayout != null && currentLayout.Length != 0)
                {
                    return currentLayout;
                }
                else
                {
                    return "Default";
                }
            }
            set { currentLayout = value; }
        }

        /// <summary>
        /// Gets or sets the portal content language.
        /// </summary>
        /// <value>The portal content language.</value>
        public CultureInfo PortalContentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
        }

        /// <summary>
        /// Gets or sets the portal data formatting culture.
        /// </summary>
        /// <value>The portal data formatting culture.</value>
        public CultureInfo PortalDataFormattingCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
            set { Thread.CurrentThread.CurrentCulture = value; }
        }

        /// <summary>
        /// PortalPath.
        /// Base dir for all portal data, relative to root web dir.
        /// </summary>
        /// <value>The portal full path.</value>
        public string PortalFullPath
        {
            get
            {
                string x = Path.WebPathCombine(portalPathPrefix, portalPath);

                //(portalPathPrefix + _portalPath).Replace("//", "/");
                if (x == "/")
                {
                    return string.Empty;
                }
                return x;
            }
            set
            {
                if (value.StartsWith(portalPathPrefix))
                {
                    portalPath = value.Substring(portalPathPrefix.Length);
                }
                else
                {
                    portalPath = value;
                }
            }
        }

        /// <summary>
        /// PortalPath.
        /// Base dir for all portal data, relative to application
        /// </summary>
        /// <value>The portal path.</value>
        public string PortalPath
        {
            get { return portalPath; }
            set
            {
                portalPath = value;
                //				//by manu
                //				//be sure it starts with "/"
                //				if (_portalPath.Length > 0 && !_portalPath.StartsWith("/"))
                //					_portalPath = Rainbow.Framework.Settings.Path.WebPathCombine("/", _portalPath);
            }
        }

        /// <summary>
        /// PortalSecurePath.
        /// Base dir for SSL
        /// </summary>
        /// <value>The portal secure path.</value>
        public string PortalSecurePath
        {
            get
            {
                if (portalSecurePath == null)
                {
                    PortalSecurePath = Config.PortalSecureDirectory;
                }
                return portalSecurePath;
            }
            set { portalSecurePath = value; }
        }

        /// <summary>
        /// Gets or sets the portal UI language.
        /// </summary>
        /// <value>The portal UI language.</value>
        public CultureInfo PortalUILanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
        }

        /// <summary>
        /// Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID
        {
            get { return portalID; }
            set { portalID = value; }
        }

        /// <summary>
        /// Gets or sets the mobile pages.
        /// </summary>
        /// <value>The mobile pages.</value>
        public ArrayList MobilePages
        {
            get { return mobilePages; }
            set { mobilePages = value; }
        }

        /// <summary>
        /// Gets or sets the active page.
        /// </summary>
        /// <value>The active page.</value>
        public PortalPage ActivePage
        {
            get { return activePage; }
            //set { activePage = value; }
        }

        /// <summary>
        /// Gets or sets the desktop pages.
        /// </summary>
        /// <value>The desktop pages.</value>
        public ArrayList DesktopPages
        {
            get { return desktopPages; }
            set { desktopPages = value; }
        }

        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		public XPathDocument DesktopPagesXml
        //		{
        //			get { return _desktopPagesXml; }
        //			set { _desktopPagesXml = value; }
        //		}

        /// <summary>
        /// Gets or sets the portal alias.
        /// </summary>
        /// <value>The portal alias.</value>
        public string PortalAlias
        {
            get { return portalAlias; }
            set { portalAlias = value; }
        }

        /// <summary>
        /// Gets or sets the name of the portal.
        /// </summary>
        /// <value>The name of the portal.</value>
        public string PortalName
        {
            get { return portalName; }
            set { portalName = value; }
        }

        /// <summary>
        /// Gets or sets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        public string PortalTitle
        {
            get { return portalTitle; }
            set { portalTitle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show pages].
        /// </summary>
        /// <value><c>true</c> if [show pages]; otherwise, <c>false</c>.</value>
        public bool ShowPages
        {
            get { return showPages; }
            set { showPages = value; }
        }

        /// <summary>
        /// Gets or sets the current theme alt.
        /// </summary>
        /// <value>The current theme alt.</value>
        public Theme CurrentThemeAlt
        {
            get { return currentThemeAlt; }
            set { currentThemeAlt = value; }
        }

        /// <summary>
        /// Gets or sets the current theme default.
        /// </summary>
        /// <value>The current theme default.</value>
        public Theme CurrentThemeDefault
        {
            get { return currentThemeDefault; }
            set { currentThemeDefault = value; }
        }

        /// <summary>
        /// Gets or sets the custom settings.
        /// </summary>
        /// <value>The custom settings.</value>
        public Hashtable CustomSettings
        {
            get { return customSettings; }
            //internal set { customSettings = value; }
            set { customSettings = value; }
        }

        public Portal()
        //internal Portal()
        {
            string applicationPath = HttpContext.Current.Request.ApplicationPath;
            portalPathPrefix = applicationPath == "/" ? string.Empty : applicationPath;

            string portalPathPrefixLocal = PortalFullPath;
            if (!portalPathPrefixLocal.EndsWith("/"))
            {
                portalPathPrefixLocal += "/";
            }
            PortalUrl = new PortalUrl(portalPathPrefixLocal);
        }

        // -- Thierry (Tiptopweb), 21 Jun 2003 [START] 
        // -- Thierry (Tiptopweb),  3 Feb 2004, fixed mismatch Alt and Default theme (Alt always returned)
        // Switch the Theme if a custom theme is defined in the tab settings
        // (using custom themes from PageSettings.cs)
        // if not use the theme defined from the portalsettings
        /// <summary>
        /// Theme definition and images
        /// </summary>
        /// <returns></returns>
        public Theme GetCurrentTheme()
        {
            // look for an custom theme
            if (ActivePage.CustomSettings[strCustomTheme] != null &&
                ActivePage.CustomSettings[strCustomTheme].ToString().Length > 0)
            {
                string customTheme = ActivePage.CustomSettings[strCustomTheme].ToString().Trim();
                ThemeManager themeManager = new ThemeManager(PortalPath);
                themeManager.Load(customTheme);
                return themeManager.CurrentTheme;
            }
            // no custom theme
            return CurrentThemeDefault;
        }

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        /// <param name="requiredTheme">The required theme.</param>
        /// <returns></returns>
        public Theme GetCurrentTheme(string requiredTheme)
        {
            switch (requiredTheme)
            {
                case "Alt":

                    // look for an alternate custom theme
                    if (ActivePage.CustomSettings["CustomThemeAlt"] != null &&
                        ActivePage.CustomSettings["CustomThemeAlt"].ToString().Length > 0)
                    {
                        string customTheme =
                            ActivePage.CustomSettings["CustomThemeAlt"].ToString().Trim();
                        ThemeManager themeManager = new ThemeManager(PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }
                    // no custom theme
                    return CurrentThemeAlt;
                default:

                    // look for an custom theme
                    if (ActivePage.CustomSettings[strCustomTheme] != null &&
                        ActivePage.CustomSettings[strCustomTheme].ToString().Length > 0)
                    {
                        string customTheme =
                            ActivePage.CustomSettings[strCustomTheme].ToString().Trim();
                        ThemeManager themeManager = new ThemeManager(PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }
                    // no custom theme
                    return CurrentThemeDefault;
            }
        }

        /// <summary>
        /// Sets the active module cookie.
        /// </summary>
        /// <param name="moduleID">The m ID.</param>
        static void SetActiveModuleCookie(int moduleID)
        {
            HttpCookie cookie;
            DateTime time;
            TimeSpan span;
            cookie = new HttpCookie("ActiveModule", moduleID.ToString());
            time = DateTime.Now;
            span = new TimeSpan(0, 2, 0, 0, 0); // 120 minutes to expire
            cookie.Expires = time.Add(span);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Recurses the portal pages XML.
        /// </summary>
        /// <param name="pageStripDetails">My page.</param>
        /// <param name="writer">The writer.</param>
        static void RecursePortalPagesXml(PageStripDetails pageStripDetails, XmlTextWriter writer)
        {
            PagesBox children = PortalPageProvider.Instance.GetPagesBox(pageStripDetails);
            bool groupElementWritten = false;

            for (int child = 0; child < children.Count; child++)
            {
                //PageStripDetails mySubPage = (PageStripDetails) children[child];
                PageStripDetails mySubPage = children[child];

                //if ( mySubPage.ParentPageID == myPage.PageID && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
                if (mySubPage.ParentPageID == pageStripDetails.PageID)
                {
                    if (!groupElementWritten)
                    {
                        writer.WriteStartElement("MenuGroup"); // start MenuGroup element
                        groupElementWritten = true;
                    }
                    writer.WriteStartElement("MenuItem"); // start MenuItem element
                    writer.WriteAttributeString("ParentPageId", mySubPage.ParentPageID.ToString());
                    //writer.WriteAttributeString("Label",mySubPage.PageName);

                    if (HttpUrlBuilder.UrlPageName(mySubPage.PageID) == HttpUrlBuilder.DefaultPage)
                    {
                        writer.WriteAttributeString("UrlPageName", mySubPage.PageName);
                    }
                    else
                    {
                        writer.WriteAttributeString("UrlPageName",
                            HttpUrlBuilder.UrlPageName(mySubPage.PageID).Replace(".aspx", ""));
                    }
                    writer.WriteAttributeString("PageName", mySubPage.PageName);
                    writer.WriteAttributeString("PageOrder", mySubPage.PageOrder.ToString());
                    writer.WriteAttributeString("PageIndex", mySubPage.PageIndex.ToString());
                    writer.WriteAttributeString("PageLayout", mySubPage.PageLayout);
                    writer.WriteAttributeString("AuthRoles", mySubPage.AuthorizedRoles);
                    writer.WriteAttributeString("ID", mySubPage.PageID.ToString());
                    //writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",mySubPage.PageName,".aspx"), mySubPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                    RecursePortalPagesXml(mySubPage, writer);
                    writer.WriteEndElement(); // end MenuItem element
                }
            }

            if (groupElementWritten)
            {
                writer.WriteEndElement(); // end MenuGroup element
            }
        }
    }
}
