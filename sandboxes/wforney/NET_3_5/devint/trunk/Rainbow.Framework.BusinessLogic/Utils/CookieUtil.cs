// ===============================================================================
// Base Logic Layer
// Rainbow.Framework.BLL.Utils
// ===============================================================================
// Cookie Utility
// ===============================================================================

namespace Rainbow.Framework.BLL.Utils
{
    using System;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// This class manages cookies
    /// </summary>
    internal sealed class CookieUtil
    {
        // minutes
        #region Constants and Fields

        /// <summary>
        /// The expire.
        /// </summary>
        private static TimeSpan expire = new TimeSpan(0, 0, 25, 0);

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets Cookie Expiration
        /// </summary>
        /// <value>The expiration.</value>
        public static TimeSpan Expiration
        {
            get
            {
                return expire;
            }

            set
            {
                Monitor.Enter(expire);
                expire = value;
                Monitor.Exit(expire);
            }
        }

        #endregion

        // end of Expiration
        #region Public Methods

        /// <summary>
        /// Add the cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void Add(string name, object value)
        {
            // is it a string
            if (value is string)
            {
                AddImpl(name, (string)value);
            }
        }

        // end of Add

        /// <summary>
        /// Add the cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void Add(int name, object value)
        {
            // is it a string
            if (value != null && value is string)
            {
                AddImpl(name.ToString(), (string)value);
            }
        }

        // end of Add

        /// <summary>
        /// Remove a cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public static void Remove(int name)
        {
            RemoveImpl(name.ToString());
        }

        // end of Remove

        /// <summary>
        /// Remove a Cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public static void Remove(string name)
        {
            RemoveImpl(name);
        }

        // end of Remove

        /// <summary>
        /// Retrieve a cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The retrieve.
        /// </returns>
        public static object Retrieve(int name)
        {
            return RetrieveImpl(name.ToString());
        }

        // end of Retrieve

        /// <summary>
        /// Retrieve a Cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The retrieve.
        /// </returns>
        public static object Retrieve(string name)
        {
            return RetrieveImpl(name);
        }

        #endregion

        // end of Retrieve

        // Implementation
        #region Methods

        /// <summary>
        /// Implementation of the add cookie
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void AddImpl(string name, string value)
        {
            // create cookie
            var hcookie = new HttpCookie(name, value);
            SetCookie(ref hcookie);
        }

        // end of addImpl

        // end of setCookie

        /// <summary>
        /// Clear the cookie
        /// </summary>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        private static void ClearCookie(ref HttpCookie cookie)
        {
            cookie.Expires = new DateTime(1999, 10, 12);
            cookie.Value = null;

            // HttpContext.Current.Response.Cookies.Remove(cookie.Name);
        }

        /// <summary>
        /// Implemented the remove functionality
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        private static void RemoveImpl(string name)
        {
            var hcookie = HttpContext.Current.Response.Cookies[name];

            if (hcookie != null)
            {
                // clear the cookie
                ClearCookie(ref hcookie);
            }
        }

        /// <summary>
        /// Implemented the remove functionality
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The retrieve impl.
        /// </returns>
        private static object RetrieveImpl(string name)
        {
            return HttpContext.Current.Request.Cookies[name];
        }

        /// <summary>
        /// Set cookie
        /// </summary>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        private static void SetCookie(ref HttpCookie cookie)
        {
            // expire in timespan
            cookie.Expires = DateTime.Now + expire;
            cookie.Path = Configuration.Properties.Resources.CookiePath;

            // see if cookie exists, otherwise create it
            if (HttpContext.Current.Response.Cookies[cookie.Name] != null)
            {
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        #endregion

        // end of clearCookie
    }
}