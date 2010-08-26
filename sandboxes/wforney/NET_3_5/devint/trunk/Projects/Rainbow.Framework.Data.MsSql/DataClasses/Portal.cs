using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Xml;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Configuration.Cache;
using Rainbow.Framework.Data.Entities;
using Rainbow.Framework.Data.MsSql.Debugger;
using Rainbow.Framework.Data.Providers;
using Rainbow.Framework.Data.Types;
using Rainbow.Framework.Design;
using Rainbow.Framework.Scheduler;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Framework.Data.MsSql
{
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
    [History("bill@improvtech.com", "2007/12/20", "Gutted PortalSettings and merged into the proper places in the new DAL")]
    partial class Portal : IPortal
    {
        #region legacy (unused)
        // portal name handled by portal title
        //string PortalName { get; set; }

        //portal path information irrelevant to rest of application. should only be used within data implementation
        //string PortalPath { get; set; }
        //string PortalPathFull { get; set; }
        //string PortalPathRelative { get; }
        //string PortalSecurePath { get; }

        //TODO: this is just not implemented... is it in use?
        //XmlDocument PortalPagesXml { get; }

        //private XDocument _portalPagesXml;
        //private XPathDocument _desktopPagesXml;
        #endregion

        #region constants
        private const string LANGUAGE_DEFAULT = "en-US";
        private const string AdminEmail = "admin@rainbowportal.net";
        private const string stradmin = "admin";
        private const string strAdmins = "Admins;";
        private const string strAllUsers = "All Users";
        private const string strGUIDHTMLDocument = "{0B113F51-FEA3-499A-98E7-7B83C192FDBB}";
        private const string strGUIDLanguageSwitcher = "{25E3290E-3B9A-4302-9384-9CA01243C00F}";
        private const string strGUIDLogin = "{A0F1F62B-FDC7-4DE5-BBAD-A5DAF31D960A}";
        private const string strGUIDManageUsers = "{B6A48596-9047-4564-8555-61E3B31D7272}";
        private const string strGUIDModules = "{5E0DB0C7-FD54-4F55-ACF5-6ECF0EFA59C0}";
        private const string strGUIDSecurityRoles = "{A406A674-76EB-4BC1-BB35-50CD2C251F9C}";
        private const string strGUIDSiteSettings = "{EBBB01B1-FBB5-4E79-8FC4-59BCA1D0554E}";
        private const string strGUIDPages = "{1C575D94-70FC-4A83-80C3-2087F726CBB3}";
        private const string strContentPane = "ContentPane";
        private const string strLeftPane = "LeftPane";
        private const string strRightPane = "RightPane";
        #endregion

        partial void OnCreated()
        {
            // Changes culture/language according to settings
            try
            {
                //Moved here for support db call
                Rainbow.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(GetLanguageList(this.PortalAlias), this.PortalAlias);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to load languages, loading defaults. " + ex.Message);
                //ErrorHandler.Publish(Rainbow.Framework.LogLevel.Warn, "Failed to load languages, loading defaults.", ex); // Jes1111
                Rainbow.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(LANGUAGE_DEFAULT, this.PortalAlias);
            }

            //Go to get custom settings
            //CustomSettings = GetPortalCustomSettings(PortalID, GetPortalBaseSettings(PortalPath));
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(PortalPath);
            //Default
            themeManager.Load(string.IsNullOrEmpty(this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_THEME").SettingValue) ?
                "Default" : this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_THEME").SettingValue);
            CurrentThemeDefault = themeManager.CurrentTheme;

            //Alternate
            if (this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_ALT_THEME").SettingValue == CurrentThemeDefault.Name)
            {
                this.CurrentThemeAlternate = CurrentThemeDefault;
            }
            else
            {
                themeManager.Load(string.IsNullOrEmpty(this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_ALT_THEME").SettingValue) ?
                    "Default" : this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_ALT_THEME").SettingValue);
                this.CurrentThemeAlternate = themeManager.CurrentTheme;
            }
            //themeManager.Save(this.CustomSettings["SITESETTINGS_THEME"].ToString());

            //Set layout
            this.CurrentLayout = string.IsNullOrEmpty(this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_PAGE_LAYOUT").SettingValue) ?
                "Default" : this.PortalSettings.SingleOrDefault(cs => cs.SettingName == "SITESETTINGS_PAGE_LAYOUT").SettingValue;

            // Jes1111
            // Generate DesktopPagesXml
            //jes1111 - if (bool.Parse(ConfigurationSettings.AppSettings["PortalSettingDesktopPagesXml"]))
            //if (Config.PortalSettingDesktopPagesXml)
            //	this.DesktopPagesXml = GetDesktopPagesXml();
        }

        private bool _showPages = true; //TODO: What's this for? Can we remove it?
        /// <summary>
        /// Gets or sets a value indicating whether this instance is show pages.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is show pages; otherwise, <c>false</c>.
        /// </value>
        public bool IsShowPages
        {
            get { return _showPages; }
            set { _showPages = value; }
        }

        /// <summary>
        /// Gets or sets the current layout.
        /// </summary>
        /// <value>The current layout.</value>
        public string CurrentLayout { get; set; }

        /// <summary>
        /// Portal Path Prefix
        /// </summary>
        public string PortalPathPrefix = HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath;

        /// <summary>
        /// Gets the portal path.
        /// </summary>
        /// <value>The portal path.</value>
        /// <remarks>
        /// jes1111 - this.PortalPath = Settings.Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
        /// </remarks>
        public string PortalPathRelative { get { return Rainbow.Framework.Configuration.Path.WebPathCombine(Config.PortalsDirectory, this.PortalPath); } }

        /// <summary>
        /// PortalPath.
        /// Base dir for all portal data, relative to root web dir.
        /// </summary>
        /// <value>The portal full path.</value>
        public string PortalPathFull
        {
            get
            {
                string x = Rainbow.Framework.Configuration.Path.WebPathCombine(this.PortalPathPrefix, this.PortalPathRelative);

                //(_portalPathPrefix + _portalPath).Replace("//", "/");
                if (x == "/") return string.Empty;
                return x;
            }
            set
            {
                if (value.StartsWith(this.PortalPathPrefix))
                    this.PortalPath = value.Substring(this.PortalPathPrefix.Length);

                else
                    this.PortalPath = value;
            }
        }

        /// <summary>
        /// Gets the portal secure path.
        /// </summary>
        /// <value>The portal secure path.</value>
        /// <remarks>
        /// jes1111 - this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"];
        /// added Thierry (tiptopweb) 12 Apr 2003
        /// </remarks>
        public string PortalSecurePath { get { return Config.PortalSecureDirectory; } }

        /// <summary>
        /// Default Selected Theme
        /// </summary>
        public Theme CurrentThemeDefault;

        /// <summary>
        /// Alternate Selected Theme
        /// </summary>
        public Theme CurrentThemeAlternate;

        /// <summary>
        /// Scheduler
        /// </summary>
        /// <remarks>
        /// Federico (ifof@libero.it) 18 jun 2003
        /// </remarks>
        protected static IScheduler scheduler;

        /// <summary>
        /// Gets the mobile pages.
        /// </summary>
        /// <value>The mobile pages.</value>
        internal IEnumerable<Page> PagesMobile
        {
            get
            {
                return this.Pages.Where(p => p.IsShowMobile);
            }
        }

        #region Globalization

        private static List<CultureInfo> _rainbowCultures = null;

        /// <summary>
        /// Gets the rainbow cultures.
        /// </summary>
        /// <value>The rainbow cultures.</value>
        private static IEnumerable<CultureInfo> RainbowCultures
        {
            get
            {
                if (_rainbowCultures == null)
                {
                    _rainbowCultures = new List<CultureInfo>();

                    string[] dirs = Directory.GetDirectories(Rainbow.Framework.Configuration.Path.ApplicationPhysicalPath + "bin");
                    char[] separators = { '\\', '/' };

                    var qd = dirs.Where(dir => Directory.GetFiles(dir, "Rainbow.resources.dll").Length == 1);

                    foreach (var d in qd)
                    {
                        _rainbowCultures.Add(new CultureInfo(d.Substring(d.LastIndexOfAny(separators) + 1)));
                    }
                }
                return _rainbowCultures;
            }
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
        /// Gets or sets the portal UI language.
        /// </summary>
        /// <value>The portal UI language.</value>
        public CultureInfo PortalUILanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
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
                string thisparam = "%" + i.ToString() + "%";
                res = res.Replace(thisparam, General.GetString(_localize[i]));
            }
            return res;
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceID">The resource ID.</param>
        /// <returns></returns>
        public static string GetStringResource(string resourceID)
        {
            // TODO: Maybe this is doings something else?
            return General.GetString(resourceID);

            //string res = null;
            //Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceID);
            //StreamReader sr = null;

            //try
            //{
            //    sr = new StreamReader(st);
            //    res = sr.ReadToEnd();
            //}

            //catch (Exception ex)
            //{
            //    //Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Configuration.LogLevel.Debug, "Resource not found: " + resourceID, ex);
            //    //throw new ArgumentNullException("Resource not found: " + resourceID);
            //    throw new RainbowException(LogLevel.Error, "Resource not found: " + resourceID, ex); // jes1111
            //}

            //finally
            //{
            //    if (sr != null)
            //        sr.Close();

            //    if (st != null)
            //        st.Close();
            //}
            //return res;
        }

        /// <summary>
        /// Gets the language list.
        /// </summary>
        /// <param name="addInvariantCulture">if set to <c>true</c> [add invariant culture].</param>
        /// <returns></returns>
        public static List<CultureInfo> GetLanguageList(bool addInvariantCulture)
        {
            return GetLanguageCultureList().ToUICultureArray(addInvariantCulture);
        }

        /// <summary>
        /// Gets the language culture list.
        /// </summary>
        /// <returns></returns>
        public static LanguageCultureCollection GetLanguageCultureList()
        {
            string strLangList = LANGUAGE_DEFAULT; //default for design time

            // Obtain PortalSettings from Current Context
            if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
            {
                //Do not remove these checks!! It fails installing modules on startup
                Portal _portal = (Portal)HttpContext.Current.Items["PortalSettings"];
                strLangList = Portal.CustomSettings(_portal.PortalId).Single(
                    p => p.SettingName == "SITESETTINGS_LANGLIST").SettingValue;
            }
            LanguageCultureCollection langList;
            try
            {
                langList =
                    (LanguageCultureCollection)
                    TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(strLangList,
                                                                                              typeof(
                                                                                                  LanguageCultureCollection
                                                                                                  ));
            }
            catch (Exception ex)
            {
                //ErrorHandler.HandleException("Failed to load languages, loading defaults", ex);
                //ErrorHandler.Publish(LogLevel.Warn, "Failed to load languages, loading defaults", ex);
                Trace.WriteLine("Failed to load languages, loading defaults: " + ex.Message);
                langList =
                    (LanguageCultureCollection)
                    TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertTo(LANGUAGE_DEFAULT,
                                                                                              typeof(
                                                                                                  LanguageCultureCollection
                                                                                                  ));
            }
            return langList;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings database table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdatePortalSetting(int portalId, string key, string value)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            var q = db.PortalSettings.SingleOrDefault(p => p.PortalId == portalId && p.SettingName == key);

            q.PortalId = portalId;
            q.SettingName = key;
            q.SettingValue = value;

            db.SubmitChanges();

            CurrentCache.Remove(Key.PortalSettings());
        }

        /// <summary>
        /// Flushes the base setting cache.
        /// </summary>
        public static void FlushBaseSettingCache()
        {
            CurrentCache.Remove(Key.PortalBaseSettings());
        }

        /// <summary>
        /// Gets the portal base settings.
        /// </summary>
        /// <param name="PortalPath">The portal path.</param>
        /// <returns></returns>
        public static IQueryable<BaseSetting> BaseSettings(string PortalPath)
        {
            DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
            db.Log = new DebuggerWriter();

            //grab base settings table for portal settings
            IQueryable<BaseSetting> _baseSettings;

            if (!CurrentCache.Exists(Key.PortalBaseSettings()))
            {
                _baseSettings = db.BaseSettings.Where(bs => bs.BaseSettingTypeId == 0);

                // fix: Jes1111 - 27-02-2005 - for proper operation of caching
                LayoutManager layoutManager = new LayoutManager(PortalPath);
                ArrayList layoutList = layoutManager.GetLayouts();
                ThemeManager themeManager = new ThemeManager(PortalPath);
                ArrayList themeList = themeManager.GetThemes();

                #region Theme Management

                //Setting Image = new Setting(new UploadedFileDataType(Path.WebPathCombine(Path.ApplicationRoot, PortalPath))); //StringDataType
                //_baseSettings.Add("SITESETTINGS_LOGO", Image);


                ////ArrayList themeList = new ThemeManager(PortalPath).GetThemes();
                //Setting Theme = new Setting(new CustomListDataType(themeList, strName, strName));
                //_baseSettings.Add("SITESETTINGS_THEME", Theme);

                ////Setting ThemeAlt = new Setting(new CustomListDataType(new ThemeManager(PortalPath).GetThemes(), strName, strName));
                //Setting ThemeAlt = new Setting(new CustomListDataType(themeList, strName, strName));
                //_baseSettings.Add("SITESETTINGS_ALT_THEME", ThemeAlt);

                #endregion

                #region Security/User Management

                // Show input for Portal Admins when using Windows Authenication and Multiportal
                // cisakson@yahoo.com 28.April.2003
                // This setting is removed in Global.asa for non-Windows authenticaton sites.
                var windowsAdmins = _baseSettings.SingleOrDefault(bs => bs.SettingName == "WindowsAdmins");
                windowsAdmins.SettingName = "WindowsAdmins";
                windowsAdmins.SettingValue = Config.ADAdministratorGroup;
                db.SubmitChanges(ConflictMode.ContinueOnConflict);

                ////MH: added dynamic load of registertypes depending on the  content in the DesktopModules/Register/ folder
                //// Register
                //Hashtable regPages = new Hashtable();

                //foreach (string registerPage in Directory.GetFiles(HttpContext.Current.Server.MapPath(Rainbow.Framework.Configuration.Path.ApplicationRoot + "/DesktopModules/CoreModules/Register/"), "register*.ascx", SearchOption.AllDirectories))
                //{
                //    string registerPageDisplayName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1, registerPage.LastIndexOf(".") - registerPage.LastIndexOf("\\") - 1);
                //    //string registerPageName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1);
                //    string registerPageName = registerPage.Replace(Path.ApplicationPhysicalPath, "~/").Replace("\\", "/");
                //    regPages.Add(registerPageDisplayName, registerPageName.ToLower());
                //}
                //// Register Layout Setting
                //Setting RegType = new Setting(new CustomListDataType(regPages, "Key", "Value"));
                ////MH:end

                // TODO: We need to bring back a country store of some sort? it should be in resources....
                /*
                 * 
				try
				{
					//Country filter limits country list, leave blank for all
					ArrayList countryList = new ArrayList(CountryInfo.GetCountries(CountryTypes.AllCountries, CountryFields.DisplayName));
					countryList.Insert(0, new CountryInfo());
					Setting CountriesFilter = new Setting(new MultiSelectListDataType(countryList, "DisplayName", strName));
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

                # region Language/Culture Management

                var langList = _baseSettings.SingleOrDefault(bs => bs.SettingName == "SITESETTINGS_LANGLIST");
                langList.SettingName = "SITESETTINGS_LANGLIST";
                langList.SettingValue = Config.DefaultLanguage;
                db.SubmitChanges();

                # endregion

                #region Miscellaneous Settings

                //// Default Image Folder. jviladiu@portalServices.net 29/07/2004
                //Setting DefaultImageFolder = new Setting(new FolderDataType(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + PortalPath + "/images"), "default"));
                //_baseSettings.Add("SITESETTINGS_DEFAULT_IMAGE_FOLDER", DefaultImageFolder);

                #endregion

                // Fix: Jes1111 - 27-02-2005 - incorrect setting for cache dependency
                //CacheDependency settingDependancies = new CacheDependency(null, new string[]{Rainbow.Framework.Configuration.Cache.Key.ThemeList(ThemeManager.Path)});
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
                    CurrentCache.Insert(Key.PortalBaseSettings(), _baseSettings, settingDependencies);
                }
            }
            else
            {
                _baseSettings = (IQueryable<BaseSetting>)CurrentCache.Get(Key.PortalBaseSettings());
            }

            return _baseSettings;
        }

        /// <summary>
        /// The PortalSettings.GetPortalSettings Method returns a hashtable of
        /// custom Portal specific settings from the database. This method is
        /// used by Portals to access misc settings.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="_baseSettings">The _base settings.</param>
        /// <returns></returns>
        public static IQueryable<PortalSetting> CustomSettings(int portalId)
        {
            IQueryable<PortalSetting> _settings;

            if (!CurrentCache.Exists(Key.PortalSettings()))
            {
                DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);
                db.Log = new DebuggerWriter();
                // Get Settings for this Portal from the database
                _settings = db.PortalSettings.Where(p => p.PortalId == portalId);
                var baseSettings = Portal.BaseSettings(db.Portals.Single(p => p.PortalId == portalId).PortalPathRelative);

                foreach (var bs in baseSettings)
                {
                    if (_settings.Count(set => set.SettingName == bs.SettingName) == 0)
                    {
                        PortalSetting ps = new PortalSetting();
                        ps.PortalId = portalId;
                        ps.SettingName = bs.SettingName;
                        ps.SettingValue = bs.SettingValue;

                        db.PortalSettings.InsertOnSubmit(ps);
                    }
                    else if (string.IsNullOrEmpty(_settings.Single(set => set.SettingName == bs.SettingName).SettingValue))
                        _settings.Single(set => set.SettingName == bs.SettingName).SettingValue = bs.SettingValue;
                }

                db.SubmitChanges(); //save the changes back to the database

                // Fix: Jes1111 - 27-02-2005 - change to make PortalSettings cache item dependent on PortalBaseSettings
                //Rainbow.Framework.Configuration.Cache.CurrentCache.Insert(Rainbow.Framework.Configuration.Cache.Key.PortalSettings(), _baseSettings);
                CacheDependency settingDependencies =
                    new CacheDependency(
                        null,
                        new string[]
							{
								Key.PortalBaseSettings()
							});

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalSettings(), _settings, settingDependencies);
                }
                return _settings;
            }
            else
            {
                _settings = (IQueryable<PortalSetting>)CurrentCache.Get(Key.PortalSettings());
                return _settings;
            }
        }

        ///// <summary>
        ///// The PortalSettings.GetCurrentUserProfile Method returns a hashtable of
        ///// all the fields and their values for currently logged user in the users table.
        ///// Used to retrieve a specific profile detail about the current user, without knowing whether the field exists in the user table or not.
        ///// </summary>
        ///// <param name="PortalID">The portal ID.</param>
        ///// <returns>
        ///// A Hashtable with containing all field values for the current user's user record
        ///// </returns>
        ///// <remarks>
        ///// Added by gman3001 9/29/2004
        ///// </remarks>
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the portal path.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        private static void CreatePortalPath(string portalPath)
        {
            portalPath = portalPath.Replace("/", string.Empty);
            portalPath = portalPath.Replace("\\", string.Empty);
            portalPath = portalPath.Replace(".", string.Empty);

            if (!portalPath.StartsWith("_"))
                portalPath = "_" + portalPath;
            // jes1111			
            //			string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
            //
            //			if(pd!=null)
            //			{
            //			if (portalPath.IndexOf (pd) > -1)
            //				portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
            //			}
            string pd = Config.PortalsDirectory;
            if (portalPath.IndexOf(pd) > -1)
                portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

            // jes1111 - string portalPhisicalDir = HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + ConfigurationSettings.AppSettings[strPortalsDirectory] + "/" + portalPath);
            string portalPhisicalDir =
                HttpContext.Current.Server.MapPath(
                    Rainbow.Framework.Configuration.Path.WebPathCombine(Rainbow.Framework.Configuration.Path.ApplicationRoot, Config.PortalsDirectory, portalPath));
            if (!Directory.Exists(portalPhisicalDir))
                Directory.CreateDirectory(portalPhisicalDir);
            // Subdirs
            string[] subdirs = { "images", "polls", "documents", "xml" };

            for (int i = 0; i <= subdirs.GetUpperBound(0); i++)

                if (!Directory.Exists(portalPhisicalDir + "\\" + subdirs[i]))
                    Directory.CreateDirectory(portalPhisicalDir + "\\" + subdirs[i]);
        }

        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns></returns>
        private string GetLanguageList(string portalAlias)
        {
            string langlist = string.Empty;

            if (!CurrentCache.Exists(Key.LanguageList()))
            {
                langlist = (from ps in this.PortalSettings where ps.SettingName == "SITESETTINGS_LANGLIST" select ps.SettingValue).SingleOrDefault();

                if (string.IsNullOrEmpty(langlist))
                    langlist = Config.DefaultLanguage;  //jes1111 - langlist = ConfigurationSettings.AppSettings["DefaultLanguage"]; //default

                CurrentCache.Insert(Key.LanguageList(), langlist);
            }
            else
            {
                langlist = (string)CurrentCache.Get(Key.LanguageList());
            }
            return langlist;
        }

        #endregion

        #region public read-only members

        /// <summary>
        /// Gets the terms of service.
        /// </summary>
        /// <value>The terms of service.</value>
        public string TermsOfService
        {
            get
            {
                string termsOfService = string.Empty;

                //Verify if we have to show conditions
                if (!string.IsNullOrEmpty(this.PortalSettings.SingleOrDefault(p => p.SettingName == "SITESETTINGS_TERMS_OF_SERVICE").SettingValue))
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
                    string terms = this.PortalSettings.SingleOrDefault(p => p.SettingName == "SITESETTINGS_TERMS_OF_SERVICE").SettingValue;
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
            set
            {
                throw new NotImplementedException("Set terms of service not implemented.");
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
                XmlDocument _portalPagesXml;
                using (StringWriter sw = new StringWriter())
                {
                    XmlTextWriter writer = new XmlTextWriter(sw);
                    writer.Formatting = Formatting.None;
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("MenuData"); // start MenuData element
                    writer.WriteStartElement("MenuGroup"); // start top MenuGroup element

                    for (int i = 0; i < this.Pages.Count; i++)
                    {
                        Page myPage = this.Pages[i];

                        //if ( myPage.ParentPageID == 0 && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
                        if (myPage.ParentPageId == 0)
                        {
                            writer.WriteStartElement("MenuItem"); // start MenuItem element
                            writer.WriteAttributeString("ParentPageId", myPage.ParentPageId.ToString());

                            if (HttpUrlBuilder.UrlPageName(myPage.PageId) == HttpUrlBuilder.DefaultPage)
                                writer.WriteAttributeString("UrlPageName", myPage.PageName);
                            else
                                writer.WriteAttributeString("UrlPageName", HttpUrlBuilder.UrlPageName(myPage.PageId).Replace(".aspx", ""));

                            writer.WriteAttributeString("PageName", myPage.PageName);

                            //writer.WriteAttributeString("Label",myPage.PageName);
                            writer.WriteAttributeString("PageOrder", myPage.PageOrder.ToString());
                            //writer.WriteAttributeString("PageIndex", myPage.PageIndex.ToString());
                            writer.WriteAttributeString("PageLayout", myPage.PageLayout.Value.ToString());
                            writer.WriteAttributeString("AuthRoles", myPage.AuthorizedRoles);
                            writer.WriteAttributeString("ID", myPage.PageId.ToString());
                            //writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",myPage.PageName,".aspx"),myPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                            //RecursePortalPagesXml(myPage, writer);
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

        #endregion

        #region IPortal Members


        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public IEnumerable<IPage> IPortal.Pages
        {
            get { return this.Pages as IEnumerable<IPage>; }
        }

        /// <summary>
        /// Gets the mobile pages.
        /// </summary>
        /// <value>The mobile pages.</value>
        public IEnumerable<IPage> IPortal.PagesMobile
        {
            get { return this.PagesMobile as IEnumerable<IPage>; }
        }



        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            return ((Portal)obj).PortalName.CompareTo(this.PortalName);
        }

        #endregion

        #region IComparable<IPortal> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(IPortal other)
        {
            return ((IPortal)other).PortalTitle.CompareTo(this.PortalTitle);
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

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
        /// </returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A double-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 16-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 32-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 64-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 8-bit signed integer equivalent to the value of this instance.
        /// </returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A single-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return this.PortalTitle;
        }

        /// <summary>
        /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified <see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="conversionType">The <see cref="T:System.Type"/> to which the value of this instance is converted.</param>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.
        /// </returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 16-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 32-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.PortalId);
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>
        /// An 64-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.PortalId);
        }

        #endregion

        #region IPortal Members

        /// <summary>
        /// Gets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        /// <remarks>
        /// jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
        /// </remarks>
        public string Title
        {
            get { return String.Concat(Config.PortalTitlePrefix, this.PortalName); }
            set { this.PortalName = value; }
        }

        public string Alias
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ILayout Layout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ITheme ThemePrimary
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ITheme ThemeSecondary
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public CultureInfo ContentLanguage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public CultureInfo UILanguage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public CultureInfo DataFormattingCulture
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IPortalSetting> Settings
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEntity Members

        public Guid Id
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid ObjectTypeId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsDeleted
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime CreatedOn
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime LastModified
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ISecuredEntity Members

        public IEnumerable<IPermission> Permissions
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IPermissionMembership> PermissionMemberships
        {
            get { throw new NotImplementedException(); }
        }

        public Guid LastEditor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
