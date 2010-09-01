namespace Rainbow.Framework.Configuration
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using System.Web;

    using Rainbow.Framework.Configuration.Properties;

    /// <summary>
    /// This class keeps some useful path information for Module, Core and Extension Developers
    /// </summary>
    public static class Path
    {
        #region Constants and Fields

        /// <summary>
        ///     application root
        /// </summary>
        private static string applicationRoot; // = null;

        #endregion

        #region Properties

        /// <summary>
        ///    Gets ApplicationPath, Application dependent.
        ///     Used by newsletter. Needed if you want to reference a page
        ///     from an external resource (an email for example)
        ///     Since it is common for all portals is declared as static.
        ///     e.g. http://www.mysite.com/rainbow/
        /// </summary>
        /// <value>The application full path.</value>
        public static string ApplicationFullPath
        {
            get
            {
                var req = HttpContext.Current.Request;
                var appFullpath = string.Format(
                    CultureInfo.InvariantCulture, "http://{0}{1}", req.Url.Host, req.ApplicationPath);

                if (req.Url.Port != 80)
                {
                    appFullpath = string.Format(
                        CultureInfo.InvariantCulture, 
                        "http://{0}:{1}{2}", 
                        req.Url.Host, 
                        req.Url.Port.ToString(CultureInfo.InvariantCulture), 
                        req.ApplicationPath);
                }

                return appFullpath;
            }
        }

        /// <summary>
        ///     Gets ApplicationPhysicalPath.
        ///     File system property
        /// </summary>
        /// <value>The application physical path.</value>
        public static string ApplicationPhysicalPath
        {
            get
            {
                return HttpContext.Current.Request.PhysicalApplicationPath;
            }
        }

        /// <summary>
        ///     Gets ApplicationRoot
        ///     Base dir for all portal code
        ///     Since it is common for all portals is declared as static
        /// </summary>
        /// <value>The application root.</value>
        public static string ApplicationRoot
        {
            get
            {
                // Build the relative Application Path
                if (applicationRoot == null)
                {
                    var req = HttpContext.Current.Request;
                    applicationRoot = (req.ApplicationPath == "/") ? string.Empty : req.ApplicationPath;
                }

                return applicationRoot;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// ApplicationRoot based path
        ///     Since it is common for all portals is declared as static
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The application root path.
        /// </returns>
        public static string ApplicationRootPath(string value)
        {
            return WebPathCombine(ApplicationRoot, value);
        }

        /// <summary>
        /// ApplicationRoot based path
        ///     Since it is common for all portals is declared as static
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The application root path.
        /// </returns>
        public static string ApplicationRootPath(params string[] values)
        {
            var fullValues = new ArrayList(values);
            fullValues.Insert(0, ApplicationRoot);
            return WebPathCombine((string[])fullValues.ToArray(Type.GetType("String")));
        }

        /// <summary>
        /// WebPathCombine ensures that combined path is a valid url
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The web path combine.
        /// </returns>
        public static string WebPathCombine(params string[] values)
        {
            const string WebPathSeparator = "/";
            const string DoubleWebPathSeparator = "//";

            if (values == null)
            {
                throw new ArgumentNullException("values", Resources.Path_WebPathCombine_Path_cannot_be_null);
            }

            var s = new StringBuilder();
            for (var i = 0; i < values.Length; i++)
            {
                if (i != 0)
                {
                    s.Append(WebPathSeparator);
                }

                if (values[i] != null && values[i].Length > 0)
                {
                    s.Append(values[i]);
                }
            }

            var returnPath = s.ToString();

            while (returnPath.IndexOf(DoubleWebPathSeparator, StringComparison.OrdinalIgnoreCase) > -1)
            {
                returnPath = returnPath.Replace(DoubleWebPathSeparator, WebPathSeparator);
            }

            returnPath = returnPath.Replace(":/", "://");

            return returnPath;
        }

        #endregion
    }
}