using System;
using System.Collections;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Rainbow.Framework;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Design;
using Rainbow.Framework.Scheduler;
using Rainbow.Framework.Security;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Settings.Cache;
using Rainbow.Framework.DataTypes;
using Rainbow.Framework.Site.Configuration;
using Path = Rainbow.Framework.Settings.Path;

namespace Rainbow.Framework.Core.Configuration.Settings
{
    /// <summary>
    /// PortalSettings Class encapsulates all of the settings 
    /// for the Portal, as well as the configuration settings required 
    /// to execute the current tab view within the portal.
    /// </summary>
    [History("moudrick", "2007/11/16", "extracting provider")]
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [History("gman3001", "2004/09/29", "Added the GetCurrentUserProfile method to obtain a hashtable of the current user's profile details.")]
    [History("jviladiu@portalServices.net", "2004/08/19", "Add support for move & delete module roles")]
    [History("jviladiu@portalServices.net", "2004/07/30", "Added new ActiveModule property")]
    [History("Jes1111", "2003/03/09", "Added new ShowTabs property")]
    [History("Jes1111", "2003/04/02", "Added new DesktopTabsXml property (an XPathDocument)")]
    [History("Thierry", "2003/04/12", "Added PortalSecurePath property")]
    [History("Jes1111", "2003/04/17", "Added new language-related properties and methods")]
    [History("Jes1111", "2003/04/23", "Corrected string comparison case problem in language settings")]
    [History("cisakson@yahoo.com", "2003/04/28", "Added a custom setting for Windows users to assign a portal Admin")]
    public class PortalSettings
    {
        const string strCustomLayout = "CustomLayout";
        const string strCustomTheme = "CustomTheme";
        const string strName = "Name";

        #region private members
        private readonly string portalPathPrefix;
        private PageSettings _activePage = new PageSettings();
        private Hashtable _customSettings;
        private ArrayList _desktopPages = new ArrayList();
        private ArrayList _mobilePages = new ArrayList();
        private string _portalAlias;
        private int _portalID;
        private string _portalName;
        private string _portalTitle;
        private bool _showPages = true;
        private string _currentLayout;
        private string _portalPath = string.Empty;
        private string _portalSecurePath;
        private XmlDocument _portalPagesXml;
        private Theme _currentThemeAlt;
        private Theme _currentThemeDefault;
        private static CultureInfo[] _rainbowCultures = null;
        private static IScheduler scheduler; // Federico (ifof@libero.it) 18 jun 2003

        /// <summary>
        /// Gets the rainbow cultures.
        /// </summary>
        /// <value>The rainbow cultures.</value>
        private static CultureInfo[] RainbowCultures
        {
            get
            {
                if (_rainbowCultures == null)
                {
                    string baseDir = Path.ApplicationPhysicalPath + "bin";
                    string[] dirs = Directory.GetDirectories(baseDir);
                    char[] separators = { '\\', '/' };

                    ArrayList rainbowCulturesArray = new ArrayList();
                    foreach (string str in dirs)
                    {
                        if ((Directory.GetFiles(str, "Rainbow.resources.dll")).Length == 1)
                        {
                            string lang = str.Substring(str.LastIndexOfAny(separators) + 1);
                            rainbowCulturesArray.Add(new CultureInfo(lang));
                        }

                    }
                    _rainbowCultures = new CultureInfo[rainbowCulturesArray.Count];
                    rainbowCulturesArray.CopyTo(_rainbowCultures);
                }
                return _rainbowCultures;
            }
        }

        #endregion

        #region constructors

        private PortalSettings()
        {
            HttpRequest request = HttpContext.Current.Request;
            portalPathPrefix = request.ApplicationPath == "/" ? string.Empty : request.ApplicationPath;}
        /// <summary>
        /// The PortalSettings Constructor encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to render
        /// a Portal Page view for a given request.<br/>
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        internal PortalSettings(int pageID, string portalAlias)
            : this()
        {
            // Changes culture/language according to settings
            try
            {
                //Moved here for support db call
                Rainbow.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(GetLanguageList(portalAlias), portalAlias);
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Warn, "Failed to load languages, loading defaults.", ex); // Jes1111
                Rainbow.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(Localization.LanguageSwitcher.LANGUAGE_DEFAULT, portalAlias);
            }

            try
            {
                PortalProvider.Instance.FillPortalSettingsFull(this, pageID, portalAlias);
            }
            catch (ProviderException ex)
            {
                Uri requestUri = HttpContext.Current.Request.Url;
                string databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
                if (databaseUpdateRedirect.StartsWith("~/"))
                {
                    databaseUpdateRedirect = databaseUpdateRedirect.TrimStart(new char[] { '~' });
                }
                if (!requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture).EndsWith(databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                {
                    throw new DatabaseUnreachableException("This may be a new db", ex.InnerException);
                }
                else
                {
                    ErrorHandler.Publish(LogLevel.Warn, "This may be a new db"); // Jes1111
                }
                return;
            }

            //Provide a valid tab id if it is missing
            if (ActivePage.PageID == 0)
            {
                ActivePage.PageID = ((PageStripDetails) DesktopPages[0]).PageID;
            }
            //Go to get custom settings
            CustomSettings = GetPortalCustomSettings(PortalID, GetPortalBaseSettings(PortalPath));
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(PortalPath);
            //Default
            themeManager.Load(CustomSettings["SITESETTINGS_THEME"].ToString());
            CurrentThemeDefault = themeManager.CurrentTheme;

            //Alternate
            if (CustomSettings["SITESETTINGS_ALT_THEME"].ToString() == CurrentThemeDefault.Name)
            {
                CurrentThemeAlt = CurrentThemeDefault;
            }
            else
            {
                themeManager.Load(CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
                CurrentThemeAlt = themeManager.CurrentTheme;
            }
            //themeManager.Save(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            //Set layout
            CurrentLayout = CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();

            // Jes1111
            // Generate DesktopPagesXml
            //jes1111 - if (bool.Parse(ConfigurationSettings.AppSettings["PortalSettingDesktopPagesXml"]))
            //if (Config.PortalSettingDesktopPagesXml)
            //	this.DesktopPagesXml = GetDesktopPagesXml();
        }

        /// <summary>
        /// The PortalSettings Constructor encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to get
        /// custom setting for a different portal than current (EditPortal.aspx.cs)<br/>
        /// This overload it is used
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        internal PortalSettings(int portalID) : this()
        {
            try
            {
                PortalProvider.Instance.FillPortalSettingsBrief(this, portalID);
            }
            catch (ProviderException)
            {
                throw new Exception("The portal you requested cannot be found. PortalID: " + portalID,
                    new HttpException(404, "Portal not found"));
            }

            //Go to get custom settings
            CustomSettings = GetPortalCustomSettings(portalID, GetPortalBaseSettings(PortalPath));
            CurrentLayout = CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(PortalPath);
            //Default
            themeManager.Load(CustomSettings["SITESETTINGS_THEME"].ToString());
            CurrentThemeDefault = themeManager.CurrentTheme;
            //Alternate
            themeManager.Load(CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
            CurrentThemeAlt = themeManager.CurrentTheme;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Flushes the base settings cache.
        /// </summary>
        /// <param name="PortalPath">The portal path.</param>
        public static void FlushBaseSettingsCache(string PortalPath)
        {
            CurrentCache.Remove(Key.PortalBaseSettings());
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
            if (ActivePage.CustomSettings[strCustomTheme] != null && ActivePage.CustomSettings[strCustomTheme].ToString().Length > 0)
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
                    if (ActivePage.CustomSettings["CustomThemeAlt"] != null && ActivePage.CustomSettings["CustomThemeAlt"].ToString().Length > 0)
                    {
                        string customTheme = ActivePage.CustomSettings["CustomThemeAlt"].ToString().Trim();
                        ThemeManager themeManager = new ThemeManager(PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }
                    // no custom theme
                    return CurrentThemeAlt;
                default:

                    // look for an custom theme
                    if (ActivePage.CustomSettings[strCustomTheme] != null && ActivePage.CustomSettings[strCustomTheme].ToString().Length > 0)
                    {
                        string customTheme = ActivePage.CustomSettings[strCustomTheme].ToString().Trim();
                        ThemeManager themeManager = new ThemeManager(PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }
                    // no custom theme
                    return CurrentThemeDefault;
            }
        }

        // /// <summary>
        // /// The PortalSettings.GetCurrentUserProfile Method returns a hashtable of
        // /// all the fields and their values for currently logged user in the users table.
        // /// Used to retrieve a specific profile detail about the current user, without knowing whether the field exists in the user table or not.
        // /// </summary>
        // /// <param name="PortalID">The portal ID.</param>
        // /// <returns>
        // /// A Hashtable with containing all field values for the current user's user record
        // /// </returns>
        // /// <remarks>
        // /// Added by gman3001 9/29/2004
        // /// </remarks>
        //public static Hashtable GetCurrentUserProfile(int PortalID)
        //{
        //    Hashtable userSettings = new Hashtable();
        //    // Obtain all current User's Information in the rb_users table into a hash table, with field name as the key
        //    UsersDB accountSystem = new UsersDB();

        //    using (SqlDataReader dr = accountSystem.GetSingleUser(CurrentUser.Identity.Email, PortalID))
        //    {
        //        // Read first row from database
        //        if (dr.Read())
        //        {
        //            for (int i = 0; i < dr.FieldCount; i++)
        //                userSettings.Add(dr.GetName(i), dr.GetValue(i).ToString());
        //        }
        //        dr.Close();
        //    }
        //    return userSettings;
        //}

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

        /// <summary>
        /// Get the ParentPageID of a certain Page 06/11/2004 Rob Siera
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public static int GetParentPageID(int pageID, ArrayList tabList)
        {
            PageStripDetails tmpPage;

            for (int i = 0; i < tabList.Count; i++)
            {
                tmpPage = (PageStripDetails)tabList[i];

                if (tmpPage.PageID == pageID)
                {
                    return tmpPage.ParentPageID;
                }
            }
            throw new ArgumentOutOfRangeException("pageID", "Root not found");
        }

        /// <summary>
        /// Gets the portal base settings.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        public static Hashtable GetPortalBaseSettings(string portalPath)
        {
            Hashtable baseSettings;

            if (!CurrentCache.Exists(Key.PortalBaseSettings()))
            {
                // fix: Jes1111 - 27-02-2005 - for proper operation of caching
                LayoutManager layoutManager = new LayoutManager(portalPath);
                ArrayList layoutList = layoutManager.GetLayouts();
                ThemeManager themeManager = new ThemeManager(portalPath);
                ArrayList themeList = themeManager.GetThemes();

                //Define base settings
                baseSettings = new Hashtable();
                int groupOrderBase;
                SettingItemGroup group;

                #region Theme Management

                group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
                groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;

                SettingItem image = new SettingItem(new UploadedFileDataType(Path.WebPathCombine(Path.ApplicationRoot, portalPath))); //StringDataType
                image.Order = groupOrderBase + 5;
                image.Group = group;
                image.EnglishName = "Logo";
                image.Description = "Enter the name of logo file here. The logo will be searched in your portal dir. For the default portal is (~/_Rainbow).";
                baseSettings.Add("SITESETTINGS_LOGO", image);

                //ArrayList layoutList = new LayoutManager(portalPath).GetLayouts();
                SettingItem tabLayoutSetting = new SettingItem(new CustomListDataType(layoutList, strName, strName));
                tabLayoutSetting.Value = "Default";
                tabLayoutSetting.Order = groupOrderBase + 10;
                tabLayoutSetting.Group = group;
                tabLayoutSetting.EnglishName = "Page layout";
                tabLayoutSetting.Description = "Specify the site level page layout here.";
                baseSettings.Add("SITESETTINGS_PAGE_LAYOUT", tabLayoutSetting);

                //ArrayList themeList = new ThemeManager(portalPath).GetThemes();
                SettingItem theme = new SettingItem(new CustomListDataType(themeList, strName, strName));
                theme.Required = true;
                theme.Order = groupOrderBase + 15;
                theme.Group = group;
                theme.EnglishName = "Theme";
                theme.Description = "Specify the site level theme here.";
                baseSettings.Add("SITESETTINGS_THEME", theme);

                //SettingItem ThemeAlt = new SettingItem(new CustomListDataType(new ThemeManager(portalPath).GetThemes(), strName, strName));
                SettingItem themeAlt = new SettingItem(new CustomListDataType(themeList, strName, strName));
                themeAlt.Required = true;
                themeAlt.Order = groupOrderBase + 20;
                themeAlt.Group = group;
                themeAlt.EnglishName = "Alternate theme";
                themeAlt.Description = "Specify the site level alternate theme here.";
                baseSettings.Add("SITESETTINGS_ALT_THEME", themeAlt);

                // Jes1111 - 2004-08-06 - Zen support
                SettingItem allowModuleCustomThemes = new SettingItem(new BooleanDataType());
                allowModuleCustomThemes.Order = groupOrderBase + 25;
                allowModuleCustomThemes.Group = group;
                allowModuleCustomThemes.Value = "False";
                allowModuleCustomThemes.EnglishName = "Allow Module Custom Themes?";
                allowModuleCustomThemes.Description = "Select to allow Custom Theme to be set on Modules.";
                baseSettings.Add("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES", allowModuleCustomThemes);

                #endregion

                #region Security/User Management

                groupOrderBase = (int)SettingItemGroup.SECURITY_USER_SETTINGS;
                group = SettingItemGroup.SECURITY_USER_SETTINGS;
                // Show input for Portal Admins when using Windows Authenication and Multiportal
                // cisakson@yahoo.com 28.April.2003
                // This setting is removed in Global.asa for non-Windows authenticaton sites.
                SettingItem PortalAdmins = new SettingItem(new StringDataType());
                PortalAdmins.Order = groupOrderBase + 5;
                PortalAdmins.Group = group;
                //jes1111 - PortalAdmins.Value = ConfigurationSettings.AppSettings["ADAdministratorGroup"];
                PortalAdmins.Value = Config.ADAdministratorGroup;
                PortalAdmins.Required = false;
                PortalAdmins.Description = "Show input for Portal Admins when using Windows Authenication and Multiportal";
                baseSettings.Add("WindowsAdmins", PortalAdmins);
                // Allow new registrations?
                SettingItem AllowNewRegistrations = new SettingItem(new BooleanDataType());
                AllowNewRegistrations.Order = groupOrderBase + 10;
                AllowNewRegistrations.Group = group;
                AllowNewRegistrations.Value = "True";
                AllowNewRegistrations.EnglishName = "Allow New Registrations?";
                AllowNewRegistrations.Description = "Check this to allow users register themselves. Leave blank for register through User Manager only.";
                baseSettings.Add("SITESETTINGS_ALLOW_NEW_REGISTRATION", AllowNewRegistrations);
                //MH: added dynamic load of registertypes depending on the  content in the DesktopModules/Register/ folder
                // Register
                Hashtable regPages = new Hashtable();

                foreach (string registerPage in Directory.GetFiles(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/DesktopModules/CoreModules/Register/"), "register*.ascx", SearchOption.AllDirectories))
                {
                    string registerPageDisplayName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1, registerPage.LastIndexOf(".") - registerPage.LastIndexOf("\\") - 1);
                    //string registerPageName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1);
                    string registerPageName = registerPage.Replace( Path.ApplicationPhysicalPath, "~/" ).Replace( "\\", "/" );
                    regPages.Add(registerPageDisplayName, registerPageName.ToLower());
                }
                // Register Layout Setting
                SettingItem RegType = new SettingItem(new CustomListDataType(regPages, "Key", "Value"));
                RegType.Required = true;
                RegType.Value = "Register.ascx";
                RegType.EnglishName = "Register Type";
                RegType.Description = "Choose here how Register Page should look like.";
                RegType.Order = groupOrderBase + 15;
                RegType.Group = group;
                baseSettings.Add("SITESETTINGS_REGISTER_TYPE", RegType);
                //MH:end
                // Register Layout Setting module id reference by manu
                SettingItem RegModuleID = new SettingItem(new IntegerDataType());
                RegModuleID.Value = "0";
                RegModuleID.Required = true;
                RegModuleID.Order = groupOrderBase + 16;
                RegModuleID.Group = group;
                RegModuleID.EnglishName = "Register Module ID";
                RegModuleID.Description = "Some custom registration may require additional settings, type here the ID of the module from where we should load settings (0= not used). Usually this module is added in an hidden area.";
                baseSettings.Add("SITESETTINGS_REGISTER_MODULEID", RegModuleID);
                // Send mail on new registration to
                SettingItem OnRegisterSendTo = new SettingItem(new StringDataType());
                OnRegisterSendTo.Value = string.Empty;
                OnRegisterSendTo.Required = false;
                OnRegisterSendTo.Order = groupOrderBase + 17;
                OnRegisterSendTo.Group = group;
                OnRegisterSendTo.EnglishName = "Send Mail To";
                OnRegisterSendTo.Description = "On new registration a mail will be send to the email address you provide here.";
                baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_TO", OnRegisterSendTo);

                // Send mail on new registration to User from
                SettingItem OnRegisterSendFrom = new SettingItem(new StringDataType());
                OnRegisterSendFrom.Value = string.Empty;
                OnRegisterSendFrom.Required = false;
                OnRegisterSendFrom.Order = groupOrderBase + 18;
                OnRegisterSendFrom.Group = group;
                OnRegisterSendFrom.EnglishName = "Send Mail From";
                OnRegisterSendFrom.Description = "On new registration a mail will be send to the new user from the email address you provide here.";
                baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_FROM", OnRegisterSendFrom);

                //Terms of service
                SettingItem TermsOfService = new SettingItem(new PortalUrlDataType());
                TermsOfService.Order = groupOrderBase + 20;
                TermsOfService.Group = group;
                TermsOfService.EnglishName = "Terms file name";
                TermsOfService.Description = "Type here a file name used for showing terms and condition in each register page. Provide localized version adding _<culturename>. E.g. Terms.txt, will search for Terms.txt and for Terms_en-US.txt";
                baseSettings.Add("SITESETTINGS_TERMS_OF_SERVICE", TermsOfService);

                // TODO: We need to bring back a country store of some sort? it should be in resources....
                /*
                 * 
				try
				{
					//Country filter limits country list, leave blank for all
					ArrayList countryList = new ArrayList(CountryInfo.GetCountries(CountryTypes.AllCountries, CountryFields.DisplayName));
					countryList.Insert(0, new CountryInfo());
					SettingItem CountriesFilter = new SettingItem(new MultiSelectListDataType(countryList, "DisplayName", strName));
					CountriesFilter.Order = _groupOrderBase + 25;
					CountriesFilter.Group = _Group;
					CountriesFilter.EnglishName = "Allowed countries";
					CountriesFilter.Description = "Allowed countries limits country list in RegisterFull page, select 'World' for no filter.";
					_baseSettings.Add("SITESETTINGS_COUNTRY_FILTER", CountriesFilter);
				}

				catch (NullReferenceException ex)
				{
					//ErrorHandler.HandleException(ex);
					ErrorHandler.Publish(Rainbow.Framework.LogLevel.Error, "Failed to create 'CountriesFilter' in PortalSettings.", ex); // Jes1111
				}
                */
                #endregion

                #region HTML Header Management

                groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
                group = SettingItemGroup.META_SETTINGS;
                // added: Jes1111 - page DOCTYPE setting
                SettingItem DocType = new SettingItem(new StringDataType());

                DocType.Order = groupOrderBase + 5;

                DocType.Group = group;

                DocType.EnglishName = "DOCTYPE string";

                DocType.Description = "Allows you to enter a DOCTYPE string which will be inserted as the first line of the HTML output page (i.e. above the <html> element). Use this to force Quirks or Standards mode, particularly in IE. See <a href=\"http://gutfeldt.ch/matthias/articles/doctypeswitch/table.html\" target=\"_blank\">here</a> for details. NOTE: Rainbow.Zen requires a setting that guarantees Standards mode on all browsers.";

                DocType.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_DOCTYPE", DocType);
                //by John Mandia <john.mandia@whitelightsolutions.com>
                SettingItem TabTitle = new SettingItem(new StringDataType());
                TabTitle.Order = groupOrderBase + 10;
                TabTitle.Group = group;
                TabTitle.EnglishName = "Page title";
                TabTitle.Description = "Allows you to enter a default tab / page title (Shows at the top of your browser).";
                baseSettings.Add("SITESETTINGS_PAGE_TITLE", TabTitle);
                /*
				 * John Mandia: Removed This Setting. Now You can define specific Url Keywords via Tab Settings only. This is to speed up url building.
				 * 
				SettingItem TabUrlKeyword = new SettingItem(new StringDataType());
				TabUrlKeyword.Order = _groupOrderBase + 15;
				TabUrlKeyword.Group = _Group;
				TabUrlKeyword.Value = "Portal";
				TabUrlKeyword.EnglishName = "Keyword to Identify all pages";
				TabUrlKeyword.Description = "This setting is not fully implemented yet. It was to help with search engine optimisation by allowing you to specify a default keyword that would appear in your url."; 
				_baseSettings.Add("SITESETTINGS_PAGE_URL_KEYWORD", TabUrlKeyword);
				*/
                SettingItem TabMetaKeyWords = new SettingItem(new StringDataType());
                TabMetaKeyWords.Order = groupOrderBase + 15;
                TabMetaKeyWords.Group = group;
                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want Meta Keywords; http://sourceforge.net/tracker/index.php?func=detail&aid=915614&group_id=66837&atid=515929
                TabMetaKeyWords.EnglishName = "Page keywords";
                TabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what your site is about.";
                baseSettings.Add("SITESETTINGS_PAGE_META_KEYWORDS", TabMetaKeyWords);
                SettingItem TabMetaDescription = new SettingItem(new StringDataType());
                TabMetaDescription.Order = groupOrderBase + 20;
                TabMetaDescription.Group = group;
                TabMetaDescription.EnglishName = "Page description";
                TabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a default description (Not too long though. 1 paragraph is enough) that describes your portal.";
                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want a defautl descripton
                baseSettings.Add("SITESETTINGS_PAGE_META_DESCRIPTION", TabMetaDescription);
                SettingItem TabMetaEncoding = new SettingItem(new StringDataType());
                TabMetaEncoding.Order = groupOrderBase + 25;
                TabMetaEncoding.Group = group;
                TabMetaEncoding.EnglishName = "Page encoding";
                TabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the default content type.";
                TabMetaEncoding.Value = "<META http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\" />";
                baseSettings.Add("SITESETTINGS_PAGE_META_ENCODING", TabMetaEncoding);
                SettingItem TabMetaOther = new SettingItem(new StringDataType());
                TabMetaOther.Order = groupOrderBase + 30;
                TabMetaOther.Group = group;
                TabMetaOther.EnglishName = "Default Additional Meta Tag Entries";
                TabMetaOther.Description = "This setting allows you to enter new tags into the Tab / Page's HEAD Tag. As an example we have added a portal tag to identify the version, but you could have a meta refresh tag or something else like a css reference instead.";
                TabMetaOther.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_PAGE_META_OTHERS", TabMetaOther);
                SettingItem TabKeyPhrase = new SettingItem(new StringDataType());
                TabKeyPhrase.Order = groupOrderBase + 35;
                TabKeyPhrase.Group = group;
                TabKeyPhrase.EnglishName = "Default Page Keyphrase";
                TabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a common message for the entire portal e.g. Welcome to x portal! This can be used for search engine optimisation. It allows you to define a keyword rich phrase to be used throughout your portal.";
                TabKeyPhrase.Value = "Enter your default keyword rich Tab / Page phrase here. ";
                baseSettings.Add("SITESETTINGS_PAGE_KEY_PHRASE", TabKeyPhrase);
                // added: Jes1111 - <body> element attributes setting
                SettingItem BodyAttributes = new SettingItem(new StringDataType());
                BodyAttributes.Order = groupOrderBase + 45;
                BodyAttributes.Group = group;
                BodyAttributes.EnglishName = "&lt;body&gt; attributes";
                BodyAttributes.Description = "Allows you to enter a string which will be inserted within the <body> element, e.g. leftmargin=\"0\" bottommargin=\"0\", etc. NOTE: not advisable to use this to inject onload() function calls as there is a programmatic function for that. NOTE also that is your CSS is well sorted you should not need anything here.";
                BodyAttributes.Required = false;
                baseSettings.Add("SITESETTINGS_BODYATTS", BodyAttributes);

                //end by John Mandia <john.mandia@whitelightsolutions.com>

                #endregion

                # region Language/Culture Management

                //groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
                group = SettingItemGroup.CULTURE_SETTINGS;

                SettingItem langList = new SettingItem(new MultiSelectListDataType(RainbowCultures, "DisplayName", "Name"));
                langList.Group = group;
                langList.EnglishName = "Language list";
                //jes1111 - LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; 
                langList.Value = Config.DefaultLanguage;
                langList.Required = false;
                langList.Description = "This is a list of the languages that the site will support. You can select multiples languages by pressing shift in your keyboard";
                baseSettings.Add("SITESETTINGS_LANGLIST", langList);

                # endregion

                #region Miscellaneous Settings

                groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                group = SettingItemGroup.MISC_SETTINGS;
                // Show modified by summary on/off
                SettingItem ShowModifiedBy = new SettingItem(new BooleanDataType());
                ShowModifiedBy.Order = groupOrderBase + 10;
                ShowModifiedBy.Group = group;
                ShowModifiedBy.Value = "False";
                ShowModifiedBy.EnglishName = "Show modified by";
                ShowModifiedBy.Description = "Check to show by whom the module is last modified.";
                baseSettings.Add("SITESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy);
                // Default Editor Configuration used for new modules and workflow modules. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultEditor = new SettingItem(new HtmlEditorDataType());
                DefaultEditor.Order = groupOrderBase + 20;
                DefaultEditor.Group = group;
                DefaultEditor.Value = "FreeTextBox";
                DefaultEditor.EnglishName = "Default Editor";
                DefaultEditor.Description = "This Editor is used by workflow and is the default for new modules.";
                baseSettings.Add("SITESETTINGS_DEFAULT_EDITOR", DefaultEditor);
                // Default Editor Width. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultWidth = new SettingItem(new IntegerDataType());
                DefaultWidth.Order = groupOrderBase + 25;
                DefaultWidth.Group = group;
                DefaultWidth.Value = "700";
                DefaultWidth.EnglishName = "Editor Width";
                DefaultWidth.Description = "Default Editor Width";
                baseSettings.Add("SITESETTINGS_EDITOR_WIDTH", DefaultWidth);
                // Default Editor Height. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultHeight = new SettingItem(new IntegerDataType());
                DefaultHeight.Order = groupOrderBase + 30;
                DefaultHeight.Group = group;
                DefaultHeight.Value = "400";
                DefaultHeight.EnglishName = "Editor Height";
                DefaultHeight.Description = "Default Editor Height";
                baseSettings.Add("SITESETTINGS_EDITOR_HEIGHT", DefaultHeight);
                //Show Upload (Active up editor only). jviladiu@portalServices.net 13/07/2004
                SettingItem ShowUpload = new SettingItem(new BooleanDataType());
                ShowUpload.Value = "true";
                ShowUpload.Order = groupOrderBase + 35;
                ShowUpload.Group = group;
                ShowUpload.EnglishName = "Upload?";
                ShowUpload.Description = "Only used if Editor is ActiveUp HtmlTextBox";
                baseSettings.Add("SITESETTINGS_SHOWUPLOAD", ShowUpload);
                // Default Image Folder. jviladiu@portalServices.net 29/07/2004
                SettingItem DefaultImageFolder = new SettingItem(new FolderDataType(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + portalPath + "/images"), "default"));
                DefaultImageFolder.Order = groupOrderBase + 40;
                DefaultImageFolder.Group = group;
                DefaultImageFolder.Value = "default";
                DefaultImageFolder.EnglishName = "Default Image Folder";
                DefaultImageFolder.Description = "Set the default image folder used by Current Editor";
                baseSettings.Add("SITESETTINGS_DEFAULT_IMAGE_FOLDER", DefaultImageFolder);
                groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                group = SettingItemGroup.MISC_SETTINGS;
                // Show module arrows to an administrator
                SettingItem ShowModuleArrows = new SettingItem(new BooleanDataType());
                ShowModuleArrows.Order = groupOrderBase + 50;
                ShowModuleArrows.Group = group;
                ShowModuleArrows.Value = "True";
                ShowModuleArrows.EnglishName = "Show module arrows";
                ShowModuleArrows.Description = "Check to show the arrows in the module title to move modules.";
                baseSettings.Add("SITESETTINGS_SHOW_MODULE_ARROWS", ShowModuleArrows);

                //BOWEN 11 June 2005
                // Use Recycler Module for deleted modules
                SettingItem UseRecycler = new SettingItem(new BooleanDataType());
                UseRecycler.Order = groupOrderBase + 55;
                UseRecycler.Group = group;
                UseRecycler.Value = "True";
                UseRecycler.EnglishName = "Use Recycle Bin for Deleted Modules";
                UseRecycler.Description = "Check to make deleted modules go to the recycler instead of permanently deleting them.";
                baseSettings.Add("SITESETTINGS_USE_RECYCLER", UseRecycler);
                //BOWEN 11 June 2005

                #endregion

                // Fix: Jes1111 - 27-02-2005 - incorrect setting for cache dependency
                //CacheDependency settingDependancies = new CacheDependency(null, new string[]{Rainbow.Framework.Settings.Cache.Key.ThemeList(ThemeManager.Path)});
                // set up a cache dependency object which monitors the four folders we are interested in
                CacheDependency settingDependencies =
                    new CacheDependency(
                        new string[]
                            {
                                LayoutManager.Path,
                                layoutManager.PortalLayoutPath,
                                ThemeManager.Path,
                                themeManager.PortalThemePath
                            });

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalBaseSettings(), baseSettings, settingDependencies);
                }
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.PortalBaseSettings());
            }
            return baseSettings;
        }

        /// <summary>
        /// The PortalSettings.GetPortalSettings Method returns a hashtable of
        /// custom Portal specific settings from the database. This method is
        /// used by Portals to access misc settings.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="baseSettings">The _base settings.</param>
        /// <returns></returns>
        public static Hashtable GetPortalCustomSettings(int portalID, Hashtable baseSettings)
        {
            if (!CurrentCache.Exists(Key.PortalSettings()))
            {
                // Get Settings for this Portal from the database
                Hashtable settings = PortalProvider.Instance.GetPortalCustomSettings(portalID);
                foreach (string key in baseSettings.Keys)
                {
                    if (settings[key] != null)
                    {
                        SettingItem s = ((SettingItem)baseSettings[key]);

                        if (settings[key].ToString().Length != 0)
                            s.Value = settings[key].ToString();
                    }
                }
                // Fix: Jes1111 - 27-02-2005 - change to make PortalSettings cache item dependent on PortalBaseSettings
                //Rainbow.Framework.Settings.Cache.CurrentCache.Insert(Rainbow.Framework.Settings.Cache.Key.PortalSettings(), baseSettings);
                CacheDependency settingDependencies =
                    new CacheDependency(
                        null,
                        new string[]
                            {
                                Key.PortalBaseSettings()
                            });

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalSettings(), baseSettings, settingDependencies);
                }
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.PortalSettings());
            }
            return baseSettings;
        }

        /// <summary>
        /// Get the proxy parameters as configured in web.config by Phillo 22/01/2003
        /// </summary>
        /// <returns></returns>
        public static WebProxy GetProxy()
        {
            //jes1111 - if(ConfigurationSettings.AppSettings["ProxyServer"].Length > 0) 
            //if(Config.ProxyServer.Length > 0) 
            //{ 
            WebProxy myProxy = new WebProxy();
            NetworkCredential myCredential = new NetworkCredential();
            //myCredential.Domain = ConfigurationSettings.AppSettings["ProxyDomain"]; 
            //myCredential.UserName = ConfigurationSettings.AppSettings["ProxyUserID"]; 
            //myCredential.Password = ConfigurationSettings.AppSettings["ProxyPassword"]; 
            myCredential.Domain = Config.ProxyDomain;
            myCredential.UserName = Config.ProxyUserID;
            myCredential.Password = Config.ProxyPassword;
            myProxy.Credentials = myCredential;
            //myProxy.Address = new Uri(ConfigurationSettings.AppSettings["ProxyServer"]); 
            myProxy.Address = new Uri(Config.ProxyServer);
            return (myProxy);
            //} 

            //else 
            //{ 
            //	return(null); 
            //} 
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
        public static PageStripDetails GetRootPage(int parentPageID, ArrayList tabList)
        {
            //Changes Indah Fuldner 25.04.2003 (With assumtion that the rootlevel tab has ParentPageID = 0)
            //Search for the root tab in current array
            PageStripDetails rootPage;

            for (int i = 0; i < tabList.Count; i++)
            {
                rootPage = (PageStripDetails)tabList[i];

                // return rootPage;
                if (rootPage.PageID == parentPageID)
                {
                    parentPageID = rootPage.ParentPageID;
                    //string parentName=rootPage.PageName;

                    if (parentPageID != 0)
                        i = -1;

                    else
                        return rootPage;
                }
            }
            //End Indah Fuldner
            throw new ArgumentOutOfRangeException("parentPageID", "Root not found");
        }

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
        public static PageStripDetails GetRootPage(PageSettings tab, ArrayList tabList)
        {
            return GetRootPage(tab.PageID, tabList);
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceID">The resource ID.</param>
        /// <returns></returns>
        public static string GetStringResource(string resourceID)
        {
            // TODO: MAybe this is doins something else?
            return General.GetString(resourceID);

//          string res = null;
//			Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceID);
//			StreamReader sr = null;
//
//			try
//			{
//				sr = new StreamReader(st);
//				res = sr.ReadToEnd();
//			}
//
//			catch (Exception ex)
//			{
//				//Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Configuration.LogLevel.Debug, "Resource not found: " + resourceID, ex);
//				//throw new ArgumentNullException("Resource not found: " + resourceID);
//				throw new RainbowException(LogLevel.Error, "Resource not found: " + resourceID, ex); // jes1111
//			}
//
//			finally
//			{
//				if (sr != null)
//					sr.Close();
//
//				if (st != null)
//					st.Close();
//			}
//			return res;
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceID">The resource ID.</param>
        /// <param name="_localize">The _localize.</param>
        /// <returns></returns>
        public static string GetStringResource(string resourceID, string[] _localize)
        {
            string res = General.GetString(resourceID);

            for (int i = 0; i <= _localize.GetUpperBound(0); i++)
            {
                string thisparam = "%" + i + "%";
                res = res.Replace(thisparam, General.GetString(_localize[i]));
            }
            return res;
        }


        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings persistence.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdatePortalSetting(int portalID, string key, string value)
        {
            PortalProvider.Instance.UpdatePortalSetting(portalID, key, value);
            CurrentCache.Remove(Key.PortalSettings());
        }
        #endregion

        #region private methods
        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns></returns>
        private static string GetLanguageList(string portalAlias)
        {
            string langlist;
            if (CurrentCache.Exists(Key.LanguageList()))
            {
                langlist = (string) CurrentCache.Get(Key.LanguageList());
            }
            else
            {
                langlist = PortalProvider.Instance.GetLanguageList(portalAlias);
                if (langlist.Length == 0)
                {
                    //jes1111 - langlist = ConfigurationSettings.AppSettings["DefaultLanguage"]; //default
                    langlist = Config.DefaultLanguage; //default
                }
                CurrentCache.Insert(Key.LanguageList(), langlist);
            }
            return langlist;
        }


        /// <summary>
        /// Sets the active module cookie.
        /// </summary>
        /// <param name="mID">The m ID.</param>
        private static void SetActiveModuleCookie(int mID)
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

        /// <summary>
        /// Recurses the portal pages XML.
        /// </summary>
        /// <param name="myPage">My page.</param>
        /// <param name="writer">The writer.</param>
        private static void RecursePortalPagesXml(PageStripDetails myPage, XmlTextWriter writer)
        {
            PagesBox children = myPage.Pages;
            bool _groupElementWritten = false;

            for (int child = 0; child < children.Count; child++)
            {
                //PageStripDetails mySubPage = (PageStripDetails) children[child];
                PageStripDetails mySubPage = children[child];

                //if ( mySubPage.ParentPageID == myPage.PageID && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
                if (mySubPage.ParentPageID == myPage.PageID)
                {
                    if (!_groupElementWritten)
                    {
                        writer.WriteStartElement("MenuGroup"); // start MenuGroup element
                        _groupElementWritten = true;
                    }
                    writer.WriteStartElement("MenuItem"); // start MenuItem element
                    writer.WriteAttributeString("ParentPageId", mySubPage.ParentPageID.ToString());
                    //writer.WriteAttributeString("Label",mySubPage.PageName);

                    if (HttpUrlBuilder.UrlPageName(mySubPage.PageID) == HttpUrlBuilder.DefaultPage)
                        writer.WriteAttributeString("UrlPageName", mySubPage.PageName);
                    else
                        writer.WriteAttributeString("UrlPageName", HttpUrlBuilder.UrlPageName(mySubPage.PageID).Replace(".aspx", ""));

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

            if (_groupElementWritten)
                writer.WriteEndElement(); // end MenuGroup element
        }

        #endregion

        #region public read-only members
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
                if (CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"] != null && CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString().Length != 0)
                {
                    //				// Attempt to load the required text
                    //				Rainbow.Framework.DataTypes.PortalUrlDataType pt = new Rainbow.Framework.DataTypes.PortalUrlDataType();
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
                    PortalUrlDataType pt = new PortalUrlDataType();
                    pt.Value = localized_terms;

                    if (File.Exists(HttpContext.Current.Server.MapPath(pt.FullPath)))
                        terms = localized_terms;
                    pt.Value = terms;
                    terms = HttpContext.Current.Server.MapPath(pt.FullPath);

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
                if (ActivePage.CustomSettings[strCustomLayout] != null && ActivePage.CustomSettings[strCustomLayout].ToString().Length > 0)
                    customLayout = ActivePage.CustomSettings[strCustomLayout].ToString();

                if (customLayout.Length != 0)
                {
                    // we have a custom Layout
                    thisLayoutPath = customLayout;
                }

                // Try to get layout from querystring
                if (HttpContext.Current != null && HttpContext.Current.Request.Params["Layout"] != null)
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
                        PageStripDetails myPage = (PageStripDetails)DesktopPages[i];

                        //if ( myPage.ParentPageID == 0 && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
                        if (myPage.ParentPageID == 0)
                        {
                            writer.WriteStartElement("MenuItem"); // start MenuItem element
                            writer.WriteAttributeString("ParentPageId", myPage.ParentPageID.ToString());

                            if (HttpUrlBuilder.UrlPageName(myPage.PageID) == HttpUrlBuilder.DefaultPage)
                                writer.WriteAttributeString("UrlPageName", myPage.PageName);
                            else
                                writer.WriteAttributeString("UrlPageName", HttpUrlBuilder.UrlPageName(myPage.PageID).Replace(".aspx", ""));

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
                    _portalPagesXml = new XmlDocument();
                    _portalPagesXml.LoadXml(sw.ToString());
                    writer.Close();
                }
                return _portalPagesXml;
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public static string ProductVersion
        {
            get
            {
                if (HttpContext.Current.Application["ProductVersion"] == null)
                {
                    FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
                    HttpContext.Current.Application.UnLock();
                }
                return (string)HttpContext.Current.Application["ProductVersion"];
            }
        }

        #endregion

        #region public read/write members

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
                if (_currentLayout != null && _currentLayout.Length != 0)
                    return _currentLayout;
                else
                    return "Default";
            }
            set { _currentLayout = value; }
        }

        /// <summary>
        /// CurrentUser
        /// </summary>
        /// <value>The current user.</value>
        public static RainbowPrincipal CurrentUser
        {
            get
            {
                RainbowPrincipal r;

                if (HttpContext.Current.User is RainbowPrincipal)
                    r = (RainbowPrincipal)HttpContext.Current.User;
                else
                    r = new RainbowPrincipal(HttpContext.Current.User.Identity, null);
                return r;
            }
            set { HttpContext.Current.User = value; }
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
                string x = Path.WebPathCombine(portalPathPrefix, _portalPath);

                //(portalPathPrefix + _portalPath).Replace("//", "/");
                if (x == "/") return string.Empty;
                return x;
            }
            set
            {
                if (value.StartsWith(portalPathPrefix))
                    _portalPath = value.Substring(portalPathPrefix.Length);

                else
                    _portalPath = value;
            }
        }



        /// <summary>
        /// PortalPath.
        /// Base dir for all portal data, relative to application
        /// </summary>
        /// <value>The portal path.</value>
        public string PortalPath
        {
            get { return _portalPath; }
            set
            {
                _portalPath = value;
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
                if (_portalSecurePath == null)
                {
                    PortalSecurePath = Config.PortalSecureDirectory;
                }
                return _portalSecurePath;
            }
            set { _portalSecurePath = value; }
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
            get { return _portalID; }
            set { _portalID = value; }
        }

        /// <summary>
        /// Gets or sets the mobile pages.
        /// </summary>
        /// <value>The mobile pages.</value>
        public ArrayList MobilePages
        {
            get { return _mobilePages; }
            set { _mobilePages = value; }
        }

        /// <summary>
        /// Gets or sets the active page.
        /// </summary>
        /// <value>The active page.</value>
        public PageSettings ActivePage
        {
            get { return _activePage; }
            set { _activePage = value; }
        }

        /// <summary>
        /// Gets or sets the custom settings.
        /// </summary>
        /// <value>The custom settings.</value>
        public Hashtable CustomSettings
        {
            get { return _customSettings; }
            set { _customSettings = value; }
        }

        /// <summary>
        /// Gets or sets the desktop pages.
        /// </summary>
        /// <value>The desktop pages.</value>
        public ArrayList DesktopPages
        {
            get { return _desktopPages; }
            set { _desktopPages = value; }
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
            get { return _portalAlias; }
            set { _portalAlias = value; }
        }

        /// <summary>
        /// Gets or sets the name of the portal.
        /// </summary>
        /// <value>The name of the portal.</value>
        public string PortalName
        {
            get { return _portalName; }
            set { _portalName = value; }
        }

        /// <summary>
        /// Gets or sets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        public string PortalTitle
        {
            get { return _portalTitle; }
            set { _portalTitle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show pages].
        /// </summary>
        /// <value><c>true</c> if [show pages]; otherwise, <c>false</c>.</value>
        public bool ShowPages
        {
            get { return _showPages; }
            set { _showPages = value; }
        }

        /// <summary>
        /// Gets or sets the current theme alt.
        /// </summary>
        /// <value>The current theme alt.</value>
        public Theme CurrentThemeAlt
        {
            get { return _currentThemeAlt; }
            set { _currentThemeAlt = value; }
        }

        /// <summary>
        /// Gets or sets the current theme default.
        /// </summary>
        /// <value>The current theme default.</value>
        public Theme CurrentThemeDefault
        {
            get { return _currentThemeDefault; }
            set { _currentThemeDefault = value; }
        }

        /// <summary>
        /// Gets or sets the scheduler.
        /// </summary>
        /// <value>The scheduler.</value>
        public static IScheduler Scheduler
        {
            get { return scheduler; }
            set { scheduler = value; }
        }

        #endregion

        #region obsolete
        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The name of the AD user.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Config.ADUserName")]
        public static string ADUserName
        {
            get { return Config.ADUserName; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The AD user password.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Config.ADUserPassword")]
        public static string ADUserPassword
        {
            get { return Config.ADUserPassword; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value><c>true</c> if [encrypt password]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Config.EncryptPassword")]
        public static bool EncryptPassword
        {
            get { return Config.EncryptPassword; }
        }

        /// <summary>
        /// ApplicationPath, Application dependent.
        /// Used by newsletter. Needed if you want to reference a page
        /// from an external resource (an email for example)
        /// Since it is common for all portals is declared as static.
        /// </summary>
        /// <value>The application full path.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Path.ApplicationFullPath")]
        public static string ApplicationFullPath
        {
            get { return Path.ApplicationFullPath; }
        }

        /// <summary>
        /// ApplicationPath, Application dependent relative Application Path.
        /// Base dir for all portal code
        /// Since it is common for all portals is declared as static
        /// </summary>
        /// <value>The application path.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Path.ApplicationRoot")]
        public static string ApplicationPath
        {
            get { return Path.ApplicationRoot; }
        }

        /// <summary>
        /// ApplicationPhisicalPath.
        /// File system property
        /// </summary>
        /// <value>The application phisical path.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Path.ApplicationPhysicalPath")]
        public static string ApplicationPhisicalPath
        {
            get { return Path.ApplicationPhysicalPath; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The code version.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Portal.CodeVersion")]
        public static int CodeVersion
        {
            get
            {
                return Portal.CodeVersion;
            }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value><c>true</c> if [enable AD user]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Config.EnableADUser")]
        public static bool EnableADUser
        {
            get { return Config.EnableADUser; }
        }

        /// <summary>
        /// This static string fetches the portal's alias either via querystring, cookie or domain and returns it
        /// </summary>
        /// <value>The get portal unique ID.</value>
        [Obsolete("Use Rainbow.Framework.Settings.Portal.UniqueID")]
        public static string GetPortalUniqueID
        {
            get { return Portal.UniqueID; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monitoring enabled; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Please use Rainbow.Framework.Settings.Config.EnableMonitoring")]
        public static bool IsMonitoringEnabled
        {
            get { return Config.EnableMonitoring; }
        }

        /// <summary>
        /// If true all users will be loaded from portal 0 instance
        /// </summary>
        /// <value><c>true</c> if [use single user base]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Rainbow.Framework.Settings.Config.UseSingleUserBase")]
        public static bool UseSingleUserBase
        {
            get { return Config.UseSingleUserBase; }
        }
        #endregion
    }
}
