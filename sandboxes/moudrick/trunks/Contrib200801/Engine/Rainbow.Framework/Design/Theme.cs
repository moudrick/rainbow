using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.Framework.Design
{
    /// <summary>
    /// The Theme class encapsulates all the settings
    /// of the currently selected theme
    /// </summary>
    public class Theme
    {
        const string DefaultButtonPath = "~/Design/Themes/Default/icon";
        const string DefaultModuleImagePath = "~/Design/Themes/Default/img";
        const string DefaultModuleCSSPath = "~/Design/Themes/Default/mod";

        string Òss = "Portal.css";
        string minimize—olor = string.Empty; //(FUTURE) [bja:C1] //    [History("bja", "2003/04/26", "C1: [Future] Added minimize color for title bar")]
        string name;
        string webPath;
        string type = "classic";

        /// <summary>
        ///     
        /// </summary>
        public Hashtable ThemeImages = new Hashtable();

        /// <summary>
        ///     
        /// </summary>
        public Hashtable ThemeParts = new Hashtable();

        /// <summary>
        /// Gets the HTML part.
        /// </summary>
        /// <param name="themePartName">The name.</param>
        /// <returns>A string value...</returns>
        public string GetHTMLPart(string themePartName)
        {
            //			string html = GetThemePart(themePartName);
            //			string w = string.Concat(WebPath, "/");
            //			html = html.Replace("src='", string.Concat("src='", w));
            //			html = html.Replace("src=\"", string.Concat("src=\"", w));
            //			html = html.Replace("background='", string.Concat("background='", w));
            //			html = html.Replace("background=\"", string.Concat("background=\"", w));
            //			return html;
            return GetThemePart(themePartName);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageName">The name.</param>
        /// <param name="defaultImagePath">The default image path.</param>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        public Image GetImage(string imageName, string defaultImagePath)
        {
            Image image;
            if (ThemeImages.ContainsKey(imageName))
            {
                image = ((ThemeImage) ThemeImages[imageName]).GetImage();
                image.ImageUrl = Framework.Path.WebPathCombine(WebPath, image.ImageUrl);
            }
            else
            {
                image = new Image();
                image.ImageUrl =
                    Framework.Path.WebPathCombine(
                        DefaultButtonPath.Replace("~", Framework.Path.ApplicationRoot),
                        defaultImagePath);
            }
            return image;
        }

        /// <summary>
        /// Get module specific image
        /// </summary>
        /// <param name="image_file_name">The image_file_name.</param>
        /// <returns></returns>
        public string GetModuleImageSRC(string image_file_name)
        {
            string imagePath;

            // check if image file exists in current theme img folder
            // else fall back to default theme img folder
            // else fall back to module img folder
            // else use default spacer img
            if (File.Exists(HttpContext.Current.Server.MapPath(WebPath + "/img/" + image_file_name)))
            {
                imagePath = Framework.Path.WebPathCombine(WebPath, "/img/" + image_file_name);
            }
            else if (
                File.Exists(
                    HttpContext.Current.Server.MapPath(DefaultModuleImagePath + image_file_name)))
            {
                imagePath =
                    Framework.Path.WebPathCombine(
                        DefaultModuleImagePath.Replace("~", Framework.Path.ApplicationRoot),
                        image_file_name);
            }
                // TODO: Not Sure how to get current module path here
                //else if(File.Exists(HttpContext.Current.Server.MapPath(WebPath + "/img/" + image_file_name)))
                //{
                // DefaultModuleImagePath = "~/Design/Themes/Default/img";
                // Not Sure how to get current module path here
                // imagePath = Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, "/desktopmodules/"+   ;			
                //}
            else
            {
                imagePath =
                    Framework.Path.WebPathCombine(
                        DefaultModuleImagePath.Replace("~", Framework.Path.ApplicationRoot),
                        "1x1.gif");
            }

            return imagePath;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageName">The name.</param>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        [Obsolete("You are strongly invited to use the new overload the takes default as parameter")
        ]
        public Image GetImage(string imageName)
        {
            return GetImage(imageName, "NoImage.gif");
        }

        /// <summary>
        /// Gets the literal control.
        /// </summary>
        /// <param name="htmlPartName">The name.</param>
        /// <returns>A System.Web.UI.LiteralControl value...</returns>
        public LiteralControl GetLiteralControl(string htmlPartName)
        {
            return new LiteralControl(GetHTMLPart(htmlPartName));
        }

        /// <summary>
        /// Gets the literal image.
        /// </summary>
        /// <param name="imageName">The name.</param>
        /// <param name="defaultImagePath">The default image path.</param>
        /// <returns>A string value...</returns>
        public string GetLiteralImage(string imageName, string defaultImagePath)
        {
            Image img = GetImage(imageName, defaultImagePath);
            return string.Format("<img src='{0}' width='{1}' height='{2}'>",
                                 img.ImageUrl,
                                 img.Width,
                                 img.Height);
        }

        /// <summary>
        /// Gets the theme part.
        /// </summary>
        /// <param name="themePartName">The name.</param>
        /// <returns></returns>
        /// <remarks>
        /// added: Jes1111 - 2004/08/27
        /// Part of Zen support
        /// </remarks>
        public string GetThemePart(string themePartName)
        {
            if (ThemeParts.ContainsKey(themePartName))
            {
                ThemePart part = (ThemePart) ThemeParts[themePartName];
                return part.Html;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the Css file name without any path.
        /// </summary>
        /// <value>The CSS.</value>
        public string Css
        {
            get { return Òss; }
            set { Òss = value; }
        }

        /// <summary>
        /// Get the Css phisical file name.
        /// Set at runtime using Web Path.
        /// </summary>
        public string CssFile
        {
            get { return Framework.Path.WebPathCombine(WebPath, Òss); }
        }

        /// <summary>
        /// Get the Css phisical file name.
        /// Set at runtime using Web Path.
        /// </summary>
        /// <param name="cssfilename">The cssfilename.</param>
        /// <returns></returns>
        public string Module_CssFile(string cssfilename)
        {
            string cssfilPath = string.Empty;

            if (File.Exists(HttpContext.Current.Server.MapPath(WebPath + "/mod/" + cssfilename)))
            {
                cssfilPath = Framework.Path.WebPathCombine(WebPath, "/mod/" + cssfilename);
            }
            else if (
                File.Exists(
                    HttpContext.Current.Server.MapPath(DefaultModuleCSSPath + "/" + cssfilename)))
            {
                cssfilPath =
                    Framework.Path.WebPathCombine(
                        DefaultModuleCSSPath.Replace("~", Framework.Path.ApplicationRoot),
                        cssfilename);
            }

            return cssfilPath;
        }


        /// <summary>
        /// [START FUTURE bja:C1]
        /// The Theme minimize color
        /// </summary>
        /// <value>The color of the minimize.</value>
        public string MinimizeColor
        {
            get { return minimize—olor; }
            set { minimize—olor = value; }
        } //end of MinimizeColor

        /// <summary>
        /// The Theme Name (must be the directory in which is located)
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Current Phisical Path. Readonly.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return (HttpContext.Current.Server.MapPath(WebPath)); }
        }

        /// <summary>
        /// Get the Theme physical file name.
        /// Set at runtime using Physical Path. NonSerialized.
        /// </summary>
        /// <value>The name of the theme file.</value>
        public string ThemeFileName
        {
            get
            {
                if (string.IsNullOrEmpty(WebPath))
                {
                    throw new ArgumentNullException("WebPath", "Value cannot be null!");
                }
                //Try to get current theme from public folder
                return System.IO.Path.Combine(Path, "Theme.xml");
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks>
        /// </remarks>
        public string Type
        {
            get { return type.ToLower(); }
            set { type = value.ToLower(); }
        }

        /// <summary>
        /// Current Web Path.
        /// It is set at runtime and therefore is not serialized
        /// </summary>
        /// <value>The web path.</value>
        public string WebPath
        {
            get { return webPath; }
            set { webPath = value; }
        }
    }
}