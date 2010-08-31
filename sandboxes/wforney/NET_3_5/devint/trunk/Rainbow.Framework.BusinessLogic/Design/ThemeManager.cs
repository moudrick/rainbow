namespace Rainbow.Framework.Design
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Xml;
    using System.Xml.Serialization;

    using Rainbow.Framework.Configuration.Cache;

    /// <summary>
    /// The ThemeManager class encapsulates all data logic necessary to
    ///     use differents themes across the entire portal.
    ///     Manages the Load and Save of the Themes.
    ///     Encapsulates a Theme object that contains all the settings
    ///     of the current Theme.
    /// </summary>
    public class ThemeManager
    {
        #region Constants and Fields

        /// <summary>
        /// The current theme.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public Theme CurrentTheme = new Theme();

        /// <summary>
        /// The portal path.
        /// </summary>
        private readonly string portalPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeManager"/> class. 
        /// Initializes a new instance of the <see cref="T:ThemeManager"/> class.
        /// </summary>
        /// <param name="portalPath">
        /// The portal path.
        /// </param>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemeManager(string portalPath)
        {
            this.portalPath = portalPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The path of the Theme dir (Phisical path)
        ///     used ot load Themes
        /// </summary>
        /// <value>The path.</value>
        public static string Path
        {
            get
            {
                return HttpContext.Current.Server.MapPath(WebPath);
            }
        }

        /// <summary>
        ///     The path of the Theme dir (Web side)
        ///     used to reference images
        /// </summary>
        /// <value>The web path.</value>
        public static string WebPath
        {
            get
            {
                return Configuration.Path.WebPathCombine(Configuration.Path.ApplicationRoot, "/Design/Themes");
            }
        }

        /// <summary>
        ///     The path of the current portal Theme dir (Phisical path)
        ///     used to load Themes
        /// </summary>
        /// <value>The portal theme path.</value>
        public string PortalThemePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(this.PortalWebPath);
            }
        }

        /// <summary>
        ///     The path of the current portal Theme dir (Web side)
        ///     used to reference images
        /// </summary>
        /// <value>The portal web path.</value>
        public string PortalWebPath
        {
            get
            {
                string myPortalWebPath = Configuration.Path.WebPathCombine(
                    Configuration.Path.ApplicationRoot, this.portalPath, "/Themes");
                return myPortalWebPath;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read the Path dir and returns an ArrayList with all the Themes found.
        ///     Static because the list is Always the same.
        /// </summary>
        /// <returns>
        /// </returns>
        public static ArrayList GetPublicThemes()
        {
            ArrayList baseThemeList;

            if (!CurrentCache.Exists(Key.ThemeList(Path)))
            {
                // Initialize array
                baseThemeList = new ArrayList();

                // Try to read directories from public theme path
                var themes = Directory.Exists(Path) ? Directory.GetDirectories(Path) : new string[0];

                foreach (var t1 in themes)
                {
                    var t = new ThemeItem();
                    t.Name = t1.Substring(Path.Length + 1);

                    if (t.Name != "CVS" && t.Name != "_svn")
                    {
                        // Ignore CVS and _svn folders
                        baseThemeList.Add(t);
                    }
                }

                CurrentCache.Insert(Key.ThemeList(Path), baseThemeList, new CacheDependency(Path));
            }
            else
            {
                baseThemeList = (ArrayList)CurrentCache.Get(Key.ThemeList(Path));
            }

            return baseThemeList;
        }

        /// <summary>
        /// Called when [remove].
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="cacheItem">
        /// The cache item.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public static void OnRemove(string key, object cacheItem, CacheItemRemovedReason reason)
        {
            Trace.WriteLine(
                string.Format("The cached value with key '{0}' was removed from the cache.  Reason: {1}", key, reason));

            // ErrorHandler.Publish(LogLevel.Info,
            // "The cached value with key '" + key + "' was removed from the cache.  Reason: " +
            // reason.ToString());
        }

        /// <summary>
        /// Clears the cache list.
        /// </summary>
        public void ClearCacheList()
        {
            // Clear cache
            CurrentCache.Remove(Key.ThemeList(Path));
            CurrentCache.Remove(Key.ThemeList(this.PortalThemePath));
        }

        /// <summary>
        /// Read the Path dir and returns
        ///     an ArrayList with all the Themes found, public and privates
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetPrivateThemes()
        {
            ArrayList privateThemeList;

            if (!CurrentCache.Exists(Key.ThemeList(this.PortalThemePath)))
            {
                privateThemeList = new ArrayList();
                string[] themes;

                // Try to read directories from private theme path
                themes = Directory.Exists(this.PortalThemePath)
                             ? Directory.GetDirectories(this.PortalThemePath)
                             : new string[0];

                for (var i = 0; i <= themes.GetUpperBound(0); i++)
                {
                    var t = new ThemeItem();
                    t.Name = themes[i].Substring(this.PortalThemePath.Length + 1);

                    if (t.Name != "CVS" && t.Name != "_svn")
                    {
                        // Ignore CVS
                        privateThemeList.Add(t);
                    }
                }

                CurrentCache.Insert(
                    Key.ThemeList(this.PortalThemePath), privateThemeList, new CacheDependency(this.PortalThemePath));

                // Debug.WriteLine("Storing privateThemeList in Cache: item count is " + privateThemeList.Count.ToString());
            }
            else
            {
                privateThemeList = (ArrayList)CurrentCache.Get(Key.ThemeList(this.PortalThemePath));

                // Debug.WriteLine("Retrieving privateThemeList from Cache: item count is " + privateThemeList.Count.ToString());
            }

            return privateThemeList;
        }

        /// <summary>
        /// Read the Path dir and returns
        ///     an ArrayList with all the Themes found, public and privates
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetThemes()
        {
            ArrayList themeList;
            ArrayList themeListPrivate;

            themeList = (ArrayList)GetPublicThemes().Clone();
            themeListPrivate = this.GetPrivateThemes();

            themeList.AddRange(themeListPrivate);

            return themeList;
        }

        /// <summary>
        /// Loads the specified theme name.
        /// </summary>
        /// <param name="ThemeName">
        /// Name of the theme.
        /// </param>
        public void Load(string ThemeName)
        {
            this.CurrentTheme = new Theme();
            this.CurrentTheme.Name = ThemeName;

            // Try loading private theme first
            if (this.LoadTheme(Configuration.Path.WebPathCombine(this.PortalWebPath, ThemeName)))
            {
                return;
            }

            // Try loading public theme
            if (this.LoadTheme(Configuration.Path.WebPathCombine(WebPath, ThemeName)))
            {
                return;
            }

            // Try default
            this.CurrentTheme.Name = "default";

            if (this.LoadTheme(Configuration.Path.WebPathCombine(WebPath, "default")))
            {
                return;
            }

            var errormsg = General.GetString("LOAD_THEME_ERROR");
            throw new FileNotFoundException(
                errormsg.Replace("%1%", string.Format("'{0}'", ThemeName)), string.Format("{0}/{1}", WebPath, ThemeName));
        }

        /// <summary>
        /// Saves the specified theme name.
        /// </summary>
        /// <param name="ThemeName">
        /// Name of the theme.
        /// </param>
        public void Save(string ThemeName)
        {
            this.CurrentTheme.Name = ThemeName;
            this.CurrentTheme.WebPath = Configuration.Path.WebPathCombine(WebPath, ThemeName);
            var serializer = new XmlSerializer(typeof(Theme));

            // Create an XmlTextWriter using a FileStream.
            using (Stream fs = new FileStream(this.CurrentTheme.ThemeFileName, FileMode.Create))
            {
                XmlWriter writer = new XmlTextWriter(fs, new UTF8Encoding());

                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, this.CurrentTheme);
                writer.Close();

                // Release the file
                writer = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the theme.
        /// </summary>
        /// <param name="currentWebPath">
        /// The current web path.
        /// </param>
        /// <returns>
        /// A bool value...
        /// </returns>
        private bool LoadTheme(Uri currentWebPath)
        {
            this.CurrentTheme.WebPath = currentWebPath;

            // if (!Rainbow.Framework.Settings.Cache.CurrentCache.Exists (Rainbow.Framework.Settings.Cache.Key.CurrentTheme(CurrentWebPath)))
            if (!CurrentCache.Exists(Key.CurrentTheme(this.CurrentTheme.Path)))
            {
                if (File.Exists(this.CurrentTheme.ThemeFileName))
                {
                    if (this.LoadXml(this.CurrentTheme.ThemeFileName))
                    {
                        // Rainbow.Framework.Settings.Cache.CurrentCache.Insert(Rainbow.Framework.Settings.Cache.Key.CurrentTheme(CurrentWebPath), CurrentTheme, new CacheDependency(CurrentTheme.ThemeFileName));
                        CurrentCache.Insert(
                            Key.CurrentTheme(this.CurrentTheme.Path), 
                            this.CurrentTheme, 
                            new CacheDependency(this.CurrentTheme.Path));
                    }
                    else
                    {
                        // failed
                        return false;
                    }
                }
                else
                {
                    // Return fail
                    return false;
                }
            }
            else
            {
                // CurrentTheme = (Theme) Rainbow.Framework.Settings.Cache.CurrentCache.Get (Rainbow.Framework.Settings.Cache.Key.CurrentTheme(CurrentWebPath));
                this.CurrentTheme = (Theme)CurrentCache.Get(Key.CurrentTheme(this.CurrentTheme.Path));
            }

            this.CurrentTheme.WebPath = currentWebPath;
            return true;
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// A bool value...
        /// </returns>
        private bool LoadXml(string filename)
        {
            XmlTextReader xtr;
            var nt = new NameTable();
            var nsm = new XmlNamespaceManager(nt);
            nsm.AddNamespace(string.Empty, "http://www.w3.org/1999/xhtml");
            var context = new XmlParserContext(nt, nsm, string.Empty, XmlSpace.None);
            var returnValue = false;

            try
            {
                // Create an XmlTextReader using a FileStream.
                using (Stream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        xtr = new XmlTextReader(fs, XmlNodeType.Document, context)
                            {
                                WhitespaceHandling = WhitespaceHandling.None 
                            };
                        ThemeImage myImage;
                        var myPart = new ThemePart();

                        while (!xtr.EOF)
                        {
                            if (xtr.MoveToContent() == XmlNodeType.Element)
                            {
                                switch (xtr.LocalName)
                                {
                                    case "Name":
                                        this.CurrentTheme.Name = xtr.ReadString();
                                        break;

                                    case "Type":
                                        this.CurrentTheme.Type = xtr.ReadString();
                                        break;

                                    case "Css":
                                        this.CurrentTheme.Css = xtr.ReadString();
                                        break;

                                    case "MinimizeColor":
                                        this.CurrentTheme.MinimizeColor = System.Drawing.Color.FromName(xtr.ReadString());
                                        break;

                                    case "ThemeImage":
                                        myImage = new ThemeImage();

                                        while (xtr.MoveToNextAttribute())
                                        {
                                            switch (xtr.LocalName)
                                            {
                                                case "Name":
                                                    myImage.Name = xtr.Value;
                                                    break;

                                                case "ImageUrl":
                                                    myImage.ImageUrl = xtr.Value;
                                                    break;

                                                case "Width":
                                                    myImage.Width = double.Parse(xtr.Value);
                                                    break;

                                                case "Height":
                                                    myImage.Height = double.Parse(xtr.Value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        this.CurrentTheme.ThemeImages.Add(myImage.Name, myImage);
                                        xtr.MoveToElement();
                                        break;

                                    case "ThemePart":
                                        myPart = new ThemePart();

                                        while (xtr.MoveToNextAttribute())
                                        {
                                            switch (xtr.LocalName)
                                            {
                                                case "Name":
                                                    myPart.Name = xtr.Value;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        xtr.MoveToElement();
                                        break;

                                    case "HTML":

                                        if (myPart.Name.Length != 0)
                                        {
                                            myPart.Html = xtr.ReadString();
                                        }

                                        // Moved here on load instead on retrival.
                                        // by Manu
                                        var w = string.Concat(this.CurrentTheme.WebPath, "/");
                                        myPart.Html = myPart.Html.Replace("src='", string.Concat("src='", w));
                                        myPart.Html = myPart.Html.Replace("src=\"", string.Concat("src=\"", w));
                                        myPart.Html = myPart.Html.Replace(
                                            "background='", string.Concat("background='", w));
                                        myPart.Html = myPart.Html.Replace(
                                            "background=\"", string.Concat("background=\"", w));
                                        this.CurrentTheme.ThemeParts.Add(myPart.Name, myPart);
                                        break;
                                    default:

                                        // Debug.WriteLine(" - unwanted");
                                        break;
                                }
                            }

                            xtr.Read();
                        }

                        returnValue = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("Failed to Load XML Theme : {0} Message was: {1}", filename, ex.Message));

                        // ErrorHandler.Publish(LogLevel.Error,
                        // "Failed to Load XML Theme : " + filename + " Message was: " + ex.Message);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Failed to open XML Theme : {0} Message was: {1}", filename, ex.Message));

                // ErrorHandler.Publish(LogLevel.Error,
                // "Failed to open XML Theme : " + filename + " Message was: " + ex.Message);
            }

            return returnValue;
        }

        #endregion
    }
}