namespace Rainbow.Framework.Logging
{
    using System.Web;

    /// <summary>
    /// The log code version property.
    /// </summary>
    public class LogCodeVersionProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                // TODO: Fix this.
                return string.Empty; // Portal.CodeVersion.ToString();
            }
            catch
            {
                return "not available";
            }
        }

        #endregion
    }

    /// <summary>
    /// The log user name property.
    /// </summary>
    public class LogUserNameProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return HttpContext.Current.User != null
                       ? (HttpContext.Current.User.Identity.Name ?? "not available")
                       : "not available";
        }

        #endregion
    }

    /// <summary>
    /// The log rewritten url property.
    /// </summary>
    public class LogRewrittenUrlProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return HttpContext.Current.Server.HtmlDecode(HttpContext.Current.Request.Url.ToString());
            }
            catch
            {
                return "not available";
            }
        }

        #endregion
    }

    /// <summary>
    /// The log user agent property.
    /// </summary>
    public class LogUserAgentProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            var context = HttpContext.Current;
            return context != null && context.Request.UserAgent != null ? context.Request.UserAgent : "not available";
        }

        #endregion
    }

    /// <summary>
    /// The log user languages property.
    /// </summary>
    public class LogUserLanguagesProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            var context = HttpContext.Current;
            return context != null && context.Request.UserLanguages != null
                       ? string.Join(";", context.Request.UserLanguages)
                       : "not available";
        }

        #endregion
    }

    /// <summary>
    /// The log user ip property.
    /// </summary>
    public class LogUserIpProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
                var context = HttpContext.Current;
                return context.Request.UserHostAddress ?? "not available";
        }

        #endregion
    }

    /// <summary>
    /// The portal alias property.
    /// </summary>
    public class PortalAliasProperty
    {
        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return string.Empty; // Portal.UniqueId;
            }
            catch
            {
                return "not available";
            }
        }

        #endregion
    }
}