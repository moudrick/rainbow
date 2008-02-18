using System.Web;
using Rainbow.Framework.Context;
using Rainbow.Framework.Core;

namespace Rainbow.Framework.Logging
{
    class LogCodeVersionProperty
    {
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
                return VersionController.Instance.CodeVersion.ToString();
            }
            catch
            {
                return "not available";
            }
        }
    }
    
    class LogUserNameProperty
    {
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
                return RainbowContext.Current.HttpContext.User.Identity.Name;
            }
            catch
            {
                return "not available";
            }
        }
    }

    class LogRewrittenUrlProperty
    {
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
                return HttpUtility.HtmlDecode(RainbowContext.Current.HttpContext.Request.Url.ToString());
            }
            catch
            {
                return "not available";
            }
        }
    }

    class LogUserAgentProperty
    {
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
                return RainbowContext.Current.HttpContext.Request.UserAgent;
            }
            catch
            {
                return "not available";
            }
        }
    }

    class LogUserLanguagesProperty
    {
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
                return string.Join(";", RainbowContext.Current.HttpContext.Request.UserLanguages);
            }
            catch
            {
                return "not available";
            }
        }
    }

    class LogUserIpProperty
    {
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
                return RainbowContext.Current.HttpContext.Request.UserHostAddress;
            }
            catch
            {
                return "not available";
            }
        }
    }

    /*
    public class PortalAliasProperty
    {
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
                return RainbowContext.Current.UniqueID;
            }
            catch
            {
                return "not available";
            }
        }
    }
     */
}
