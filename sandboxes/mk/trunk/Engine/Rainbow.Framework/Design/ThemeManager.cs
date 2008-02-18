using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using Rainbow.Framework.Context;

namespace Rainbow.Framework.Design
{
    /// <summary>
    /// The ThemeManager class encapsulates all data logic necessary to
    /// use differents themes across the entire portal.
    /// Manages the Load and Save of the Themes.
    /// Encapsulates a Theme object that contains all the settings
    /// of the current Theme.
    /// </summary>
    public class ThemeManager
    {
        public Theme CurrentTheme = new Theme();

        readonly string portalPath;

        public static RainbowContext current = RainbowContext.Current;

        /// <summary>
        /// The path of the Theme dir (Phisical path)
        /// used ot load Themes
        /// </summary>
        /// <value>The path.</value>
        public static string Path
        {
            get { return (current.HttpContext.Server.MapPath(WebPath)); }
        }

        /// <summary>
        /// The path of the current portal Theme dir (Phisical path)
        /// used to load Themes
        /// </summary>
        /// <value>The portal theme path.</value>
        public string PortalThemePath
        {
            get { return (current.HttpContext.Server.MapPath(PortalWebPath)); }
        }

        /// <summary>
        /// The path of the current portal Theme dir (Web side)
        /// used to reference images
        /// </summary>
        /// <value>The portal web path.</value>
        public string PortalWebPath
        {
            get
            {
                string myPortalWebPath =
                    Framework.Path.WebPathCombine(Framework.Path.ApplicationRoot, portalPath, "/Themes");
                return myPortalWebPath;
            }
        }

        /// <summary>
        /// The path of the Theme dir (Web side)
        /// used to reference images
        /// </summary>
        /// <value>The web path.</value>
        public static string WebPath
        {
            get { return Framework.Path.WebPathCombine(Framework.Path.ApplicationRoot, "/Design/Themes"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeManager"/> class.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        public ThemeManager(string portalPath)
        {
            this.portalPath = portalPath;
        }

        /// <summary>
        /// Clears the cache list.
        /// </summary>
        public void ClearCacheList()
        {
            //Clear cache
            CurrentCache.Remove(Key.ThemeList(Path));
            CurrentCache.Remove(Key.ThemeList(PortalThemePath));
        }

        /// <summary>
        /// Read the Path dir and returns an ArrayList with all the Themes found.
        /// Static because the list is Always the same.
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetPublicThemes()
        {
            ArrayList baseThemeList;

            if (!CurrentCache.Exists(Key.ThemeList(Path)))
            {
                //Initialize array
                baseThemeList = new ArrayList();
                string[] themes;

                // Try to read directories from public theme path
                if (Directory.Exists(Path))
                {
                    themes = Directory.GetDirectories(Path);
                }

                else
                {
                    themes = new string[0];
                }

                for (int i = 0; i < themes.Length; i++)
                {
                    ThemeItem t = new ThemeItem();
                    t.Name = themes[i].Substring(Path.Length + 1);

                    if (t.Name != "CVS" && t.Name != "_svn") //Ignore CVS and _svn folders
                        baseThemeList.Add(t);
                }
                CurrentCache.Insert(Key.ThemeList(Path), baseThemeList, new CacheDependency(Path));
            }

            else
            {
                baseThemeList = (ArrayList) CurrentCache.Get(Key.ThemeList(Path));
            }
            return baseThemeList;
        }

        /// <summary>
        /// Read the Path dir and returns
        /// an ArrayList with all the Themes found, public and privates
        /// </summary>
        /// <returns></returns>
        public ArrayList GetPrivateThemes()
        {
            ArrayList privateThemeList;

            if (!CurrentCache.Exists(Key.ThemeList(PortalThemePath)))
            {
                privateThemeList = new ArrayList();
                string[] themes;

                // Try to read directories from private theme path
                if (Directory.Exists(PortalThemePath))
                {
                    themes = Directory.GetDirectories(PortalThemePath);
                }

                else
                {
                    themes = new string[0];
                }

                for (int i = 0; i <= themes.GetUpperBound(0); i++)
                {
                    ThemeItem t = new ThemeItem();
                    t.Name = themes[i].Substring(PortalThemePath.Length + 1);

                    if (t.Name != "CVS" && t.Name != "_svn") //Ignore CVS
                        privateThemeList.Add(t);
                }

                CurrentCache.Insert(Key.ThemeList(PortalThemePath), privateThemeList,
                                    new CacheDependency(PortalThemePath));
                //Debug.WriteLine("Storing privateThemeList in Cache: item count is " + privateThemeList.Count.ToString());
            }
            else
            {
                privateThemeList = (ArrayList) CurrentCache.Get(Key.ThemeList(PortalThemePath));
                //Debug.WriteLine("Retrieving privateThemeList from Cache: item count is " + privateThemeList.Count.ToString());
            }
            return privateThemeList;
        }

        /// <summary>
        /// Read the Path dir and returns
        /// an ArrayList with all the Themes found, public and privates
        /// </summary>
        /// <returns></returns>
        public ArrayList GetThemes()
        {
            ArrayList themeList;
            ArrayList themeListPrivate;

            themeList = (ArrayList) GetPublicThemes().Clone();
            themeListPrivate = GetPrivateThemes();

            themeList.AddRange(themeListPrivate);

            return themeList;
        }

        /// <summary>
        /// Loads the specified theme name.
        /// </summary>
        /// <param name="ThemeName">Name of the theme.</param>
        public void Load(string ThemeName)
        {
            CurrentTheme = new Theme();
            CurrentTheme.Name = ThemeName;

            //Try loading private theme first
            if (LoadTheme(Framework.Path.WebPathCombine(PortalWebPath, ThemeName)))
                return;

            //Try loading public theme
            if (LoadTheme(Framework.Path.WebPathCombine(WebPath, ThemeName)))
                return;
            //Try default
            CurrentTheme.Name = "default";

            if (LoadTheme(Framework.Path.WebPathCombine(WebPath, "default")))
                return;
            string errormsg = General.GetString("LOAD_THEME_ERROR");
            throw new FileNotFoundException(errormsg.Replace("%1%", "'" + ThemeName + "'"), WebPath + "/" + ThemeName);
        }

        /// <summary>
        /// Called when [remove].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheItem">The cache item.</param>
        /// <param name="reason">The reason.</param>
        public static void OnRemove(string key, object cacheItem, CacheItemRemovedReason reason)
        {
            ErrorHandler.Publish(LogLevel.Info,
                string.Format("The cached value with key '{0}' was removed from the cache.  Reason: {1}", 
                    key, reason));
        }

        /// <summary>
        /// Saves the specified theme name.
        /// </summary>
        /// <param name="ThemeName">Name of the theme.</param>
        public void Save(string ThemeName)
        {
            CurrentTheme.Name = ThemeName;
            CurrentTheme.WebPath = Framework.Path.WebPathCombine(WebPath, ThemeName);
            XmlSerializer serializer = new XmlSerializer(typeof (Theme));

            // Create an XmlTextWriter using a FileStream.
            using (Stream fs = new FileStream(CurrentTheme.ThemeFileName, FileMode.Create))
            using (XmlWriter writer = new XmlTextWriter(fs, new UTF8Encoding()))
            {
                serializer.Serialize(writer, CurrentTheme);
                writer.Close();
            }
        }

        /// <summary>
        /// Loads the theme.
        /// </summary>
        /// <param name="CurrentWebPath">The current web path.</param>
        /// <returns>A bool value...</returns>
        bool LoadTheme(string CurrentWebPath)
        {
            CurrentTheme.WebPath = CurrentWebPath;

            //if (!Rainbow.Framework.Settings.Cache.CurrentCache.Exists (Rainbow.Framework.Settings.Cache.Key.CurrentTheme(CurrentWebPath)))
            if (!CurrentCache.Exists(Key.CurrentTheme(CurrentTheme.Path)))
            {
                if (File.Exists(CurrentTheme.ThemeFileName))
                {
                    if (LoadXml(CurrentTheme.ThemeFileName))
                    {
                        //Rainbow.Framework.Settings.Cache.CurrentCache.Insert(Rainbow.Framework.Settings.Cache.Key.CurrentTheme(CurrentWebPath), CurrentTheme, new CacheDependency(CurrentTheme.ThemeFileName));
                        CurrentCache.Insert(Key.CurrentTheme(CurrentTheme.Path), CurrentTheme,
                                            new CacheDependency(CurrentTheme.Path));
                    }

                    else
                    {
                        // failed
                        return false;
                    }
                }

                else
                {
                    //Return fail
                    return false;
                }
            }

            else
            {
                //CurrentTheme = (Theme) Rainbow.Framework.Settings.Cache.CurrentCache.Get (Rainbow.Framework.Settings.Cache.Key.CurrentTheme(CurrentWebPath));
                CurrentTheme = (Theme) CurrentCache.Get(Key.CurrentTheme(CurrentTheme.Path));
            }
            CurrentTheme.WebPath = CurrentWebPath;
            return true;
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>A bool value...</returns>
        bool LoadXml(string filename)
        {
            XmlTextReader xmlTextReader;
            NameTable nameTable = new NameTable();
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(nameTable);
            xmlNamespaceManager.AddNamespace(string.Empty, "http://www.w3.org/1999/xhtml");
            XmlParserContext _context = new XmlParserContext(nameTable, xmlNamespaceManager, 
                string.Empty, XmlSpace.None);
            bool returnValue = false;

            try
            {
                // Create an XmlTextReader using a FileStream.
                using (Stream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        xmlTextReader = new XmlTextReader(fs, XmlNodeType.Document, _context);
                        xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
                        ThemeImage themeImage;
                        ThemePart themePart = new ThemePart();

                        while (!xmlTextReader.EOF)
                        {
                            if (xmlTextReader.MoveToContent() == XmlNodeType.Element)
                            {
                                switch (xmlTextReader.LocalName)
                                {
                                    case "Name":
                                        CurrentTheme.Name = xmlTextReader.ReadString();
                                        break;

                                    case "Type":
                                        CurrentTheme.Type = xmlTextReader.ReadString();
                                        break;

                                    case "Css":
                                        CurrentTheme.Css = xmlTextReader.ReadString();
                                        break;

                                    case "MinimizeColor":
                                        CurrentTheme.MinimizeColor = xmlTextReader.ReadString();
                                        break;

                                    case "ThemeImage":
                                        themeImage = new ThemeImage();

                                        while (xmlTextReader.MoveToNextAttribute())
                                        {
                                            switch (xmlTextReader.LocalName)
                                            {
                                                case "Name":
                                                    themeImage.Name = xmlTextReader.Value;
                                                    break;

                                                case "ImageUrl":
                                                    themeImage.ImageUrl = xmlTextReader.Value;
                                                    break;

                                                case "Width":
                                                    themeImage.Width = double.Parse(xmlTextReader.Value);
                                                    break;

                                                case "Height":
                                                    themeImage.Height = double.Parse(xmlTextReader.Value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        CurrentTheme.ThemeImages.Add(themeImage.Name, themeImage);
                                        xmlTextReader.MoveToElement();
                                        break;

                                    case "ThemePart":
                                        themePart = new ThemePart();

                                        while (xmlTextReader.MoveToNextAttribute())
                                        {
                                            switch (xmlTextReader.LocalName)
                                            {
                                                case "Name":
                                                    themePart.Name = xmlTextReader.Value;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        xmlTextReader.MoveToElement();
                                        break;

                                    case "HTML":

                                        if (themePart.Name.Length != 0)
                                            themePart.Html = xmlTextReader.ReadString();
                                        //Moved here on load instead on retrival.
                                        //by Manu
                                        string w = string.Concat(CurrentTheme.WebPath, "/");
                                        themePart.Html = themePart.Html.Replace("src='", string.Concat("src='", w));
                                        themePart.Html = themePart.Html.Replace("src=\"", string.Concat("src=\"", w));
                                        themePart.Html =
                                            themePart.Html.Replace("background='", string.Concat("background='", w));
                                        themePart.Html =
                                            themePart.Html.Replace("background=\"", string.Concat("background=\"", w));
                                        CurrentTheme.ThemeParts.Add(themePart.Name, themePart);
                                        break;
                                    default:
                                        //Debug.WriteLine(" - unwanted");
                                        break;
                                }
                            }
                            xmlTextReader.Read();
                        }
                        returnValue = true;
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Error,
                                             "Failed to Load XML Theme : " + filename + " Message was: " + ex.Message);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Error,
                                     "Failed to open XML Theme : " + filename + " Message was: " + ex.Message);
            }
            return returnValue;
        }
    }
}
