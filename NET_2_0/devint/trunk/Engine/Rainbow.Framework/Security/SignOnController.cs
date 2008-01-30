using System;
using System.Web;
using System.Web.Security;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Security
{
    ///<summary>
    /// Encapsulates SignOn/SignOut and related methods
    ///</summary>
    public class SignOnController
    {
        /// <summary>
        /// Single point for logging on an user.
        /// </summary>
        /// <param name="user">Username or email</param>
        /// <param name="password">Password</param>
        /// <param name="persistent">Use a cookie to make it persistent</param>
        /// <returns></returns>
        public static string SignOn(string user, string password, bool persistent)
        {
            return SignOn(user, password, persistent, null);
        }

        /// <summary>
        /// Single point for logging on an user.
        /// </summary>
        /// <param name="login">Username or email</param>
        /// <param name="password">Password</param>
        /// <param name="persistent">Use a cookie to make it persistent</param>
        /// <param name="redirectPage">The redirect page.</param>
        /// <returns></returns>
        [History("bja@reedtek.com", "2003/05/16", "Support for collapsable")]
        public static string SignOn(string login,
                                    string password,
                                    bool persistent,
                                    string redirectPage)
        {
            Portal portal = PortalProvider.Instance.CurrentPortal;
            // Attempt to Validate User Credentials using UsersDB
            MembershipUser user = Login(login, password);

            // Thierry (tiptopweb), 12 Apr 2003: Save old ShoppingCartID
            //			ShoppingCartDB shoppingCart = new ShoppingCartDB();
            //			string tempCartID = ShoppingCartDB.GetCurrentShoppingCartID();

            if (user != null)
            {
                // Ender, 31 July 2003: Support for the monitoring module by Paul Yarrow
                if (Config.EnableMonitoring)
                {
                    try
                    {
                        MonitoringProvider.Instance.LogEntry((Guid) user.ProviderUserKey,
                                                             portal.PortalID,
                                                             -1,
                                                             "Logon",
                                                             string.Empty);
                    }
                    catch
                    {
                        ErrorHandler.Publish(LogLevel.Info,
                                             "Cannot monitoring login user " + user.UserName);
                    }
                }

                // Use security system to set the UserID within a client-side Cookie
                FormsAuthentication.SetAuthCookie(user.ToString(), persistent);

                // Rainbow Security cookie Required if we are sharing a single domain 
                // with portal Alias in the URL

                // Set a cookie to persist authentication for each portal 
                // so user can be reauthenticated 
                // automatically if they chose to Remember Login					
                HttpCookie hck = RainbowContext.Current.HttpContext.Response
                    .Cookies["Rainbow_" + portal.PortalAlias.ToLower()];
                hck.Value = user.ToString(); //Fill all data: name + email + id
                hck.Path = "/";

                if (persistent) // Keep the cookie?
                {
                    hck.Expires = DateTime.Now.AddYears(50);
                }
                else
                {
                    //jminond - option to kill cookie after certain time always
// jes1111
//					if(ConfigurationSettings.AppSettings["CookieExpire"] != null)
//					{
//						int minuteAdd = int.Parse(ConfigurationSettings.AppSettings["CookieExpire"]);
                    int minuteAdd = Config.CookieExpire;

                    DateTime time = DateTime.Now;
                    TimeSpan span = new TimeSpan(0, 0, minuteAdd, 0, 0);

                    hck.Expires = time.Add(span);
//					}
                }

                HttpContext httpContext = RainbowContext.Current.HttpContext;
                if (redirectPage == null || redirectPage.Length == 0)
                {
                    // Redirect browser back to originating page
                    if (httpContext.Request.UrlReferrer != null)
                    {
                        httpContext.Response.Redirect(httpContext.Request.UrlReferrer.ToString());
                    }
                    else
                    {
                        httpContext.Response.Redirect(Path.ApplicationRoot);
                    }
                    return user.Email;
                }
                else
                {
                    httpContext.Response.Redirect(redirectPage);
                }
            }
            return null;
        }

        /// <summary>
        /// Single point logoff
        /// </summary>
        public static void SignOut()
        {
            SignOut(HttpUrlBuilder.BuildUrl("~/Default.aspx"), true);
        }

        /// <summary>
        /// Single point logoff
        /// </summary>
        public static void SignOut(string urlToRedirect, bool removeLogin)
        {
            // Log User Off from Cookie Authentication System
            FormsAuthentication.SignOut();
      
            // Invalidate roles token
            HttpContext httpContext = RainbowContext.Current.HttpContext;
            HttpCookie hck = httpContext.Response.Cookies["portalroles"];
            hck.Value = null;
            hck.Expires = new DateTime(1999, 10, 12);
            hck.Path = "/";

            if (removeLogin)
            {
                // Obtain PortalSettings from Current Context
                Portal portalSettings = PortalProvider.Instance.CurrentPortal;

                // Invalidate Portal Alias Cookie security
                HttpCookie xhck = httpContext.Response.Cookies["Rainbow_" + portalSettings.PortalAlias.ToLower()];
                xhck.Value = null;
                xhck.Expires = new DateTime(1999, 10, 12);
                xhck.Path = "/";
            }

            // [START]  bja@reedtek.com remove user window information
            // User Information
            // valid user
            if (httpContext.User != null)
            {
                //Ender 4 July 2003: Added to support the Monitoring module by Paul Yarrow
                Portal portal = PortalProvider.Instance.CurrentPortal;
                MembershipUser user = RainbowMembershipProvider.Instance
                    .GetSingleUser(portal.PortalAlias, httpContext.User.Identity.Name);

                Guid userId = (Guid) user.ProviderUserKey;
                if (!userId.Equals(Guid.Empty))
                {
                    try
                    {
                        if (Config.EnableMonitoring)
                        {
                            MonitoringProvider.Instance.LogEntry(userId,
                                                                 portal.PortalID,
                                                                 -1,
                                                                 "Logoff",
                                                                 string.Empty);
                        }
                    }
                    catch {;}
                }
            }
            // [END ]  bja@reedtek.com remove user window information

            //Redirect user back to the Portal Home Page
            if (urlToRedirect.Length > 0)
            {
                httpContext.Response.Redirect(urlToRedirect);
            }
        }

        /// <summary>
        /// ExtendCookie
        /// </summary>
        /// <param name="portalSettings">The portal settings.</param>
        /// <param name="minuteAdd">The minute add.</param>
        public static void ExtendCookie(Portal portalSettings, int minuteAdd)
        {
            DateTime time = DateTime.Now;
            TimeSpan span = new TimeSpan(0, 0, minuteAdd, 0, 0); 

            HttpContext.Current.Response.Cookies["Rainbow_" + portalSettings.PortalAlias].Expires = time.Add(span);

            return;
        }

        /// <summary>
        /// ExtendCookie
        /// </summary>
        /// <param name="portalSettings">The portal settings.</param>
        public static void ExtendCookie(Portal portalSettings)
        {
            int minuteAdd = Config.CookieExpire;
            ExtendCookie(portalSettings, minuteAdd);
            return;
        }

        /// <summary>
        /// Kills session after timeout
        /// jminond - fix kill session after timeout.
        /// </summary>
        public static void KillSession()
        {
			
            SignOut(HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Admin/Logon.aspx"), true);

            //HttpContext.Current.Response.Redirect(urlToRedirect);
            //PortalSecurity.AccessDenied();
        }

        /// <summary>
        /// The Login method validates a email/password hash pair against credentials
        /// stored in the users database.  If the email/password hash pair is valid,
        /// the method returns user's name.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <remarks>UserLogin Stored Procedure</remarks>
        static MembershipUser Login(string email, string password)
        {
            string userName = RainbowMembershipProvider.Instance
                .GetUserNameByEmail(PortalProvider.Instance.CurrentPortal.PortalAlias, email);

            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            RainbowUser user = (RainbowUser) RainbowMembershipProvider.Instance.GetUser(userName, true);
            bool isValid = RainbowMembershipProvider.Instance.ValidateUser(user.UserName, password);
            return isValid ? user : null;
        }
    }
}