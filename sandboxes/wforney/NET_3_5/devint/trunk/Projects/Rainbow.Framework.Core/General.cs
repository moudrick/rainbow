namespace Rainbow.Framework
{
    using System;
    using System.Diagnostics;
    using System.Web;

    /// <summary>
    /// Static helper methods for one line calls
    /// </summary>
    /// <remarks>
    /// <list type="string">
    /// <item>
    /// GetString
    /// </item>
    /// </list>
    /// </remarks>
    public static class General
    {
        #region Public Methods

        /// <summary>
        /// Get a resource string value
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns>
        /// The get string.
        /// </returns>
        public static string GetString(string key)
        {
            return GetString(key, string.Empty);
        }

        /// <summary>
        /// Get a resource string value
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The get string.
        /// </returns>
        public static string GetString(string key, string defaultValue, object o)
        {
            // TODO: What are objects passed around for?
            return GetString(key, defaultValue);
        }

        /// <summary>
        /// Get a resource string value
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        /// <returns>
        /// The get string.
        /// </returns>
        public static string GetString(string key, string defaultValue)
        {
            if (HttpContext.Current == null)
            {
                var ne = new Exception("HttpContext.Current not an object");

                // TODO: Fix the error handler so it isn't dependent creating circular references
                // ErrorHandler.Publish(LogLevel.Warn, "Problem with Global Resources - could not get key: " + key, ne);
                return "<span class='error'>Could not get key: " + key + "</span>";
            }

            try
            {
                // TODO: Should we be using cached reourceset per language?
#if DEBUG
                HttpContext.Current.Trace.Warn("GetString(" + key + ")");
#endif

                // userCulture = Thread.CurrentThread.CurrentCulture.Name;
                var str = HttpContext.GetGlobalResourceObject("Rainbow", key);

                // string str = ((Rainbow.Framework.Web.UI.Page)System.Web.UI.Page).UserCultureSet.GetString(key);
                var ret = string.Empty;

                if (str != null)
                {
                    ret = str.ToString();
#if DEBUG
                    HttpContext.Current.Trace.Warn(
                        ret.Length > 0 ? "We got localized  version" : "Localized return empty, use default");

#endif
                }

                if (ret.Length == 0)
                {
                    return defaultValue;
                }

                HttpContext.Current.Trace.Warn("GetString  = " + ret);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Problem with Global Resources - could not get key: " + key + Environment.NewLine + ex.Message);

                // ErrorHandler.Publish(LogLevel.Warn, "Problem with Global Resources - could not get key: " + key, ex);
                return defaultValue;
            }
        }

        #endregion
    }
}