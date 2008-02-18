using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Configuration;

namespace Rainbow.Framework.Data
{
    public partial class Settings
    {
        /// <summary>
        /// The default portal alias
        /// <br/>
        /// Default value: "rainbow"
        /// </summary>
        /// <value>The default portal.</value>
        public static string DefaultPortal
        {
            get { return GetString("DefaultPortal", "rainbow", false).Trim().ToUpperInvariant(); }
        }

        /// <summary>
        /// Enables multiple databases (i.e. 1 per portal on a single codebase)
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable multi db support]; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableMultiDBSupport
        {
            get { return GetBoolean("EnableMultiDbSupport", false); }
        }

        /// <summary>
        /// The database connection string - checks for EnableMultiDbSupport and returns the
        /// correct connection string for this portal.
        /// <br/>
        /// Default value: "server=localhost;database=Rainbow;uid=sa;pwd="
        /// </summary>
        /// <value>The connection string.</value>
        public static string ConnectionString
        {
            get
            {
                // TODO: ENABLE Multi DB SUpport?

                string keyConnection = String.Concat(Portal.UniqueId, "_ConnectionString");
                string siteConnectionString;

                // check cache first
                if (!CurrentCache.Exists(keyConnection)) // not in cache
                {
                    if (EnableMultiDBSupport)
                        // look in web.config for key="[uniqueID]_ConnectionString", default to key="ConnectionString"
                        siteConnectionString = ConfigurationManager.ConnectionStrings[keyConnection].ConnectionString;
                    else
                        // look in web.config for key="ConnectionString"
                        siteConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    // add to cache
                    CurrentCache.Insert(keyConnection, siteConnectionString);
                    // return the right connection string for this portal
                    return siteConnectionString;
                }
                else // already cached
                {
                    // return cached value
                    return (string)CurrentCache.Get(keyConnection);
                }
            }
        }

        /// <summary>
        /// Removes "www." when attempting to derive alias from hostname
        /// <br/>
        /// Default value: true
        /// </summary>
        /// <value><c>true</c> if [remove WWW]; otherwise, <c>false</c>.</value>
        public static bool RemoveWWW
        {
            get { return GetBoolean("RemoveWWW", true); }
        }

        /// <summary>
        /// Removes one- and two-part TLDs when attempting to derive alias from hostname
        /// <br/>
        /// Default value: true
        /// </summary>
        /// <value><c>true</c> if [remove TLD]; otherwise, <c>false</c>.</value>
        public static bool RemoveTLD
        {
            get { return GetBoolean("RemoveTLD", true); }
        }

        /// <summary>
        /// List of possible second level domains to use for removing two-part TLDs when attempting to derive alias from hostname
        /// <br/>
        /// Default value: "aero;biz;com;coop;info;museum;name;net;org;pro;gov;edu;mil;int;co;ac;sch;nhs;police;mod;ltd;plc;me"
        /// </summary>
        /// <value>The second level domains.</value>
        public static string SecondLevelDomains
        {
            get { return GetString("SecondLevelDomains", "aero;biz;com;coop;info;museum;name;net;org;pro;gov;edu;mil;int;co;ac;sch;nhs;police;mod;ltd;plc;me", false); }
        }

        /// <summary>
        /// (Optional) Default PromoCode for Amazon module
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The amazon promo code.</value>
        public static string AmazonPromoCode
        {
            get { return GetString("AmazonPromoCode", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Default DevToken for Amazon module
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The amazon dev token.</value>
        public static string AmazonDevToken
        {
            get { return GetString("AmazonDevToken", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Email address for webMaster (used in RSS feed)
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The web master.</value>
        public static string WebMaster
        {
            get { return GetString("WebMaster", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) ConnectionString for PortalTemplates server
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The portal templates connection string.</value>
        public static string PortalTemplatesConnectionString
        {
            get { return GetString("PortalTemplatesConnectionString", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Folder for Quotes files (used by Quote module)
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The quote file folder.</value>
        public static string QuoteFileFolder
        {
            get { return GetString("QuoteFileFolder", string.Empty, true); }
        }

        /// <summary>
        /// Enables Scheduler
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [scheduler enable]; otherwise, <c>false</c>.</value>
        public static bool SchedulerEnable
        {
            get { return GetBoolean("SchedulerEnable", false); }
        }

        /// <summary>
        /// Enables WebCompile
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [enable web compile]; otherwise, <c>false</c>.</value>
        public static bool EnableWebCompile
        {
            get { return GetBoolean("EnableWebCompile", false); }
        }

        /// <summary>
        /// Cache size for Scheduler
        /// <br/>
        /// Default value: 100
        /// </summary>
        /// <value>The size of the scheduler cache.</value>
        public static int SchedulerCacheSize
        {
            get { return GetInteger("SchedulerCacheSize", 100); }
        }

        /// <summary>
        /// Period for Scheduler
        /// <br/>
        /// Default value: 60000
        /// </summary>
        /// <value>The scheduler period.</value>
        public static int SchedulerPeriod
        {
            get { return GetInteger("SchedulerPeriod", 60000); }
        }

        /// <summary>
        /// SMTP Server for sending emails from modules, registration, etc.
        /// (Set value to empty string for cluster server support).
        /// <br/>
        /// Default value: "localhost"
        /// </summary>
        /// <value>The SMTP server.</value>
        public static string SmtpServer
        {
            get { return GetString("SmtpServer", "localhost", true); }
        }

        /// <summary>
        /// Email "From" address for sending emails from modules, registration, etc.
        /// <br/>
        /// Default value: "portal@localhost.com"
        /// </summary>
        /// <value>The email from.</value>
        public static string EmailFrom
        {
            get { return GetString("EmailFrom", "portal@localhost.com", false); }
        }

        /// <summary>
        /// Enables Password Encryption
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [encrypt password]; otherwise, <c>false</c>.</value>
        public static bool EncryptPassword
        {
            get { return GetBoolean("EncryptPassword", false); }
        }

        /// <summary>
        /// URL for SmartError page
        /// <br/>
        /// Default value: "~/app_support/SmartError.aspx"
        /// </summary>
        /// <value>The smart error redirect.</value>
        public static string SmartErrorRedirect
        {
            get { return GetString("SmartErrorRedirect", "~/app_support/SmartError.aspx", false); }
        }

        /// <summary>
        /// URL for redirect when invalid Alias is detected in request
        /// <br/>
        /// Default value: "~/app_support/SmartError.aspx"
        /// </summary>
        /// <value>The invalid alias redirect.</value>
        public static string InvalidAliasRedirect
        {
            get { return GetString("InvalidAliasRedirect", "~/app_support/SmartError.aspx", false); }
        }

        /// <summary>
        /// URL for redirect when invalid PageID is detected in request
        /// <br/>
        /// Default value: "~/app_support/SmartError.aspx"
        /// </summary>
        /// <value>The invalid page id redirect.</value>
        public static string InvalidPageIdRedirect
        {
            get { return GetString("InvalidPageIdRedirect", "~/app_support/SmartError.aspx", false); }
        }

        /// <summary>
        /// Determines how 'wrong' a request can be, i.e. allow invalid PageID, etc.
        /// Returns an integer between 1 and 4.
        /// See http://support.rainbowportal.net/confluence/display/DOX/Changes+in+Global.asax.cs
        /// <br/>
        /// Default value: 3
        /// </summary>
        /// <value>The URL tolerance level.</value>
        public static int UrlToleranceLevel
        {
            get
            {
                int returnValue = GetInteger("UrlToleranceLevel", 3);
                return returnValue < 1 ? 1 : returnValue > 4 ? 4 : returnValue; // make sure number is between 1 and 4
            }
        }

        /// <summary>
        /// HTTP Status code to use when redirecting to Database Update page
        /// <br/>
        /// Default value: HttpStatusCode.ServiceUnavailable (503)
        /// </summary>
        /// <value>The database update response.</value>
        public static HttpStatusCode DatabaseUpdateResponse
        {
            get { return GetHttpStatusCode("DatabaseUpdateResponse", HttpStatusCode.ServiceUnavailable); }
        }

        /// <summary>
        /// URL for redirect to Database Update page
        /// <br/>
        /// Default value: "~/Setup/Update.aspx"
        /// </summary>
        /// <value>The database update redirect.</value>
        public static string DatabaseUpdateRedirect
        {
            get { return GetString("DatabaseUpdateRedirect", "~/Setup/Update.aspx", false); }
        }

        /// <summary>
        /// URL for redirect to Installer page
        /// <br/>
        /// Default value: "~/Setup/Update.aspx"
        /// </summary>
        /// <value>The installer redirect.</value>
        public static string InstallerRedirect
        {
            get { return GetString("InstallerRedirect", "~/Installer/default.aspx", false); }
        }

        /// <summary>
        /// HTTP Status code to use when redirecting to Database Error Page
        /// <br/>
        /// Default value: HttpStatusCode.ServiceUnavailable (503)
        /// </summary>
        /// <value>The database error response.</value>
        public static HttpStatusCode DatabaseErrorResponse
        {
            get { return GetHttpStatusCode("DatabaseErrorResponse", HttpStatusCode.ServiceUnavailable); }
        }

        /// <summary>
        /// URL for redirect on Database Error
        /// <br/>
        /// Default value: "~/app_support/GeneralError.html"
        /// </summary>
        /// <value>The database error redirect.</value>
        public static string DatabaseErrorRedirect
        {
            get { return GetString("DatabaseErrorRedirect", "~/app_support/GeneralError.html", false); }
        }

        /// <summary>
        /// HTTP Status code to use when redirecting to Code Update page
        /// <br/>
        /// Default value: HttpStatusCode.ServiceUnavailable (503)
        /// </summary>
        /// <value>The code update response.</value>
        public static HttpStatusCode CodeUpdateResponse
        {
            get { return GetHttpStatusCode("CodeUpdateResponse", HttpStatusCode.ServiceUnavailable); }
        }

        /// <summary>
        /// URL for redirect on Code Update
        /// <br/>
        /// Default value: "~/app_support/GeneralError.html"
        /// </summary>
        /// <value>The code update redirect.</value>
        public static string CodeUpdateRedirect
        {
            get { return GetString("CodeUpdateRedirect", "~/app_support/GeneralError.html", false); }
        }

        /// <summary>
        /// HTTP Status code to use when redirecting on Critical Error
        /// <br/>
        /// Default value: HttpStatusCode.ServiceUnavailable (503)
        /// </summary>
        /// <value>The critical error response.</value>
        public static HttpStatusCode CriticalErrorResponse
        {
            get { return GetHttpStatusCode("CriticalErrorResponse", HttpStatusCode.ServiceUnavailable); }
        }

        /// <summary>
        /// URL for redirect on Critical Error
        /// <br/>
        /// Default value: "~/app_support/GeneralError.html"
        /// </summary>
        /// <value>The critical error redirect.</value>
        public static string CriticalErrorRedirect
        {
            get { return GetString("CriticalErrorRedirect", "~/app_support/GeneralError.html", false); }
        }

        /// <summary>
        /// HTTP Status code to use when redirecting on NoPortal Error
        /// <br/>
        /// Default value: HttpStatusCode.NotFound (404)
        /// </summary>
        /// <value>The no portal error response.</value>
        public static HttpStatusCode NoPortalErrorResponse
        {
            get { return GetHttpStatusCode("NoPortalErrorResponse", HttpStatusCode.NotFound); }
        }

        /// <summary>
        /// URL for redirect on NoPortal Error
        /// <br/>
        /// Default value: "~/app_support/GeneralError.html"
        /// </summary>
        /// <value>The no portal error redirect.</value>
        public static string NoPortalErrorRedirect
        {
            get { return GetString("NoPortalErrorRedirect", "~/app_support/ErrorNoPortal.html", false); }
        }

        /// <summary>
        /// Locks all portals - redirects non-KeyHolders to LockRedirect page
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [lock all portals]; otherwise, <c>false</c>.</value>
        public static bool LockAllPortals
        {
            get { return GetBoolean("LockAllPortals", false); }
        }

        /// <summary>
        /// Semi-colon delimited list of IP addresses which are "key holders" - key holders are
        /// allowed into a locked portal and are shown full error details on SmartError page
        /// <br/>
        /// Default value: "127.0.0.1"
        /// </summary>
        /// <value>The lock key holders.</value>
        public static string LockKeyHolders
        {
            get { return GetString("LockKeyHolders", "127.0.0.1", false); }
        }

        /// <summary>
        /// HTTP Status code to use when redirecting to LockRedirect page
        /// <br/>
        /// Default value: HttpStatusCode.ServiceUnavailable (503)
        /// </summary>
        /// <value>The lock response.</value>
        public static HttpStatusCode LockResponse
        {
            get { return GetHttpStatusCode("LockResponse", HttpStatusCode.ServiceUnavailable); }
        }

        /// <summary>
        /// URL for redirect on "All Portals Locked"
        /// <br/>
        /// Default value: "~/app_support/GeneralError.html"
        /// </summary>
        /// <value>The lock redirect.</value>
        public static string LockRedirect
        {
            get { return GetString("LockRedirect", "~/app_support/GeneralError.html", false); }
        }

        /// <summary>
        /// Time in minutes for cookie expiration
        /// <br/>
        /// Default value: 60
        /// </summary>
        /// <value>The cookie expire.</value>
        public static int CookieExpire
        {
            get { return GetInteger("CookieExpire", 60); }
        }

        /// <summary>
        /// If true, cookie will not renew itself, and force login
        /// every 'CookieExpire' minutes
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [force expire]; otherwise, <c>false</c>.</value>
        public static bool ForceExpire
        {
            get { return GetBoolean("ForceExpire", false); }
        }

        /// <summary>
        /// Enables check for correct file permissions for ASPNET user on application start
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [check for file permission]; otherwise, <c>false</c>.
        /// </value>
        public static bool CheckForFilePermission
        {
            get { return GetBoolean("CheckForFilePermission", false); }
        }

        /// <summary>
        /// Enables use of proxy for Web Requests
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use proxy server for server web requests]; otherwise, <c>false</c>.
        /// </value>
        public static bool UseProxyServerForServerWebRequests
        {
            get { return GetBoolean("UseProxyServerForServerWebRequests", false); }
        }

        /// <summary>
        /// Enables Window Management on modules (min, max, etc.)
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [window MGMT controls]; otherwise, <c>false</c>.</value>
        public static bool WindowMgmtControls
        {
            get { return GetBoolean("WindowMgmtControls", false); }
        }

        /// <summary>
        /// Enables Window Management 'close' function on modules
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [window MGMT want close]; otherwise, <c>false</c>.
        /// </value>
        public static bool WindowMgmtWantClose
        {
            get { return GetBoolean("WindowMgmtWantClose", false); }
        }

        /// <summary>
        /// Enables Monitoring
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [enable monitoring]; otherwise, <c>false</c>.</value>
        public static bool EnableMonitoring
        {
            get { return GetBoolean("EnableMonitoring", false); }
        }

        /// <summary>
        /// Enables use of DesktopPagesXml in PortalSettings (for navigation etc.)
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [portal setting desktop pages XML]; otherwise, <c>false</c>.
        /// </value>
        public static bool PortalSettingDesktopPagesXml
        {
            get { return GetBoolean("PortalSettingDesktopPagesXml", false); }
        }

        /// <summary>
        /// (Optional) String to prefix all page titles
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The portal title prefix.</value>
        public static string PortalTitlePrefix
        {
            get { return GetString("PortalTitlePrefix", string.Empty); }
        }

        /// <summary>
        /// The folder in which portal data directories are located
        /// <br/>
        /// Default value: "Portals"
        /// </summary>
        /// <value>The portals directory.</value>
        public static string PortalsDirectory
        {
            //an empty directory is allowed
            get { return GetString("PortalsDirectory", "Portals", true); }
        }

        /// <summary>
        /// The secure directory for Ecommerce system
        /// <br/>
        /// Default value: "ECommerce/Secure"
        /// </summary>
        /// <value>The portal secure directory.</value>
        public static string PortalSecureDirectory
        {
            get { return GetString("PortalSecureDirectory", "ECommerce/Secure", false); }
        }

        /// <summary>
        /// ActiveDirectory Administrator Group
        /// <br/>
        /// Default value: "MyDomain\Administrators"
        /// </summary>
        /// <value>The AD administrator group.</value>
        public static string ADAdministratorGroup
        {
            get { return GetString("ADAdministratorGroup", @"MyDomain\Administrators", false); }
        }

        /// <summary>
        /// ActiveDirectory DNS
        /// <br/>
        /// Default value: "LDAP://DomainControllerName/DC=MyDomain, DC=com; WinNT://MyDomain"
        /// </summary>
        /// <value>The A DDNS.</value>
        public static string ADdns
        {
            get { return GetString("ADdns", @"LDAP://DomainControllerName/DC=MyDomain, DC=com; WinNT://MyDomain", false); }
        }

        /// <summary>
        /// Default DOCTYPE for Zen pages
        /// <br/>
        /// Default value: "&lt;!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01 Transitional//EN' &gt;"
        /// </summary>
        /// <value>The default DOCTYPE.</value>
        public static string DefaultDocType
        {
            get { return GetString("DefaultDOCTYPE", @"&lt;!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01 Transitional//EN' &gt;", false); }
        }

        /// <summary>
        /// Script call for IE7, used for Zen
        /// <br/>
        /// Default value: "/aspnet_client/ie7-08a/ie7-standard-p.js"
        /// </summary>
        /// <value>The ie7 script.</value>
        public static string IE7Script
        {
            get { return GetString("Ie7Script", @"/aspnet_client/ie7-08a/ie7-standard-p.js", false); }
        }

        /// <summary>
        /// LDAP Administrator Group
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The LDAP administrator group.</value>
        public static string LDAPAdministratorGroup
        {
            get { return GetString("LDAPAdministratorGroup", string.Empty, true); }
        }

        /// <summary>
        /// LDAP Login
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The LDAP login.</value>
        public static string LDAPLogin
        {
            get { return GetString("LDAPLogin", string.Empty, true); }
        }

        /// <summary>
        /// LDAP Server
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The LDAP server.</value>
        public static string LDAPServer
        {
            get { return GetString("LDAPServer", string.Empty, true); }
        }

        /// <summary>
        /// LDAP Group
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The LDAP group.</value>
        public static string LDAPGroup
        {
            get { return GetString("LDAPGroup", string.Empty, true); }
        }

        /// <summary>
        /// LDAP Contexts
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The LDAP contexts.</value>
        public static string LDAPContexts
        {
            get { return GetString("LDAPContexts", string.Empty, true); }
        }

        /// <summary>
        /// Default language
        /// <br/>
        /// Default value: "en-US"
        /// </summary>
        /// <value>The default language.</value>
        public static string DefaultLanguage
        {
            get { return GetString("DefaultLanguage", "en-US", false); }
        }

        /// <summary>
        /// (Optional) Username for login to Update page
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The name of the update user.</value>
        public static string UpdateUserName
        {
            get { return GetString("UpdateUserName", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Password for login to Update page
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The update password.</value>
        public static string UpdatePassword
        {
            get { return GetString("UpdatePassword", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Address for Proxy Server to use for Web Requests
        /// <br/>
        /// Default value: "http://127.0.0.1"
        /// </summary>
        /// <value>The proxy server.</value>
        public static string ProxyServer
        {
            get { return GetString("ProxyServer", "http://127.0.0.1", false); }
        }

        /// <summary>
        /// (Optional) UserID for Proxy Server to use for Web Requests
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The proxy user ID.</value>
        public static string ProxyUserID
        {
            get { return GetString("ProxyUserID", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Password for Proxy Server to use for Web Requests
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The proxy password.</value>
        public static string ProxyPassword
        {
            get { return GetString("ProxyPassword", string.Empty, true); }
        }

        /// <summary>
        /// (Optional) Domain for Proxy Server to use for Web Requests
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The proxy domain.</value>
        public static string ProxyDomain
        {
            get { return GetString("ProxyDomain", string.Empty, true); }
        }

        /// <summary>
        /// ActiveDirectory UserName
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The name of the AD user.</value>
        public static string ADUserName
        {
            get { return GetString("ADUserName", string.Empty, true); }
        }

        /// <summary>
        /// ActiveDirectory Password
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The AD user password.</value>
        public static string ADUserPassword
        {
            get { return GetString("ADUserPassword", string.Empty, true); }
        }

        /// <summary>
        /// Enables ActiveDirectory usage
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [enable AD user]; otherwise, <c>false</c>.</value>
        public static bool EnableADUser
        {
            get { return GetBoolean("EnableADUser", false); }
        }

        /// <summary>
        /// Forces all portals to share Portal 0 user table
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [use single user base]; otherwise, <c>false</c>.</value>
        public static bool UseSingleUserBase
        {
            get { return GetBoolean("UseSingleUserBase", false); }
        }

        /// <summary>
        /// Sets default Module Cache time (in seconds) - value here overrides any (cacheable) modules with
        /// local setting of 0
        /// <br/>
        /// Default value: 0
        /// </summary>
        /// <value>The module override cache.</value>
        public static int ModuleOverrideCache
        {
            get { return GetInteger("ModuleOverrideCache", 0); }
        }

        /// <summary>
        /// (Optional) Specifies folder containing XSL files (used by XmlFeed module)
        /// <br/>
        /// Default value: empty string
        /// </summary>
        /// <value>The XML feed XSL folder.</value>
        public static string XMLFeedXSLFolder
        {
            get { return GetString("XMLFeedXSLFolder", string.Empty, true); }
        }

        /// <summary>
        /// Using grouping tabs to display module settings or not
        /// <br/>
        /// Default value: false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use settings grouping tabs]; otherwise, <c>false</c>.
        /// </value>
        public static bool UseSettingsGroupingTabs
        {
            get { return GetBoolean("UseSettingsGroupingTabs", false); }
        }

        /// <summary>
        /// Width of SettingsTable contrl when using grouping tabs to
        /// display module settings
        /// <br/>
        /// Default value: 600
        /// </summary>
        /// <value>The width of the settings grouping.</value>
        public static int SettingsGroupingWidth
        {
            get { return GetInteger("SettingsGroupingWidth", 600); }
        }

        /// <summary>
        /// Height of SettingsTable contrl when using grouping tabs to
        /// display module settings
        /// <br/>
        /// Default value: 350
        /// </summary>
        /// <value>The height of the settings grouping.</value>
        public static int SettingsGroupingHeight
        {
            get { return GetInteger("SettingsGroupingHeight", 350); }
        }
    }
}
