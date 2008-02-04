using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Net;
using System.Web;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers.Configuration;
using Rainbow.Framework.Security;

namespace Rainbow.Framework.Providers
{
    ///<summary>
    /// This is interface class for get portal settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public abstract class PortalProvider : ProviderBase
    {
        const string providerType = "portal";

        /// <summary>
        /// Gets default configured Portal provider.
        /// Singleton pattern standard member.
        /// </summary>
        /// <returns>Default instance of Portal Provider class</returns>
        public static PortalProvider Instance
        {
            get
            {
                return ProviderConfiguration.GetDefaultProviderFromCache<PortalProvider>(
                    providerType, HttpContext.Current.Cache);
            }
        }

        ///<summary>
        /// Gets currently loaded portal object
        ///</summary>
        public Portal CurrentPortal
        {
            get
            {
                //TODO: [moudrick] find & encapsulate Items["PortalSettings"] sets. 
                // Find all "Items["PortalSettings"]", Subfolders, Find Results 1, "Entire Solution"
                // Matching lines: 74    Matching files: 52    Total files searched: 1668
                return (Portal)RainbowContext.Current.HttpContext.Items["PortalSettings"];
            }
        }

        /// <summary>
        /// Gets portal custom settings from persistence
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public abstract Hashtable GetPortalCustomSettings(int portalID);

        /// <summary>
        /// Fills brief portal settings for edit
        /// </summary>
        /// <param name="portal"></param>
        /// <param name="portalID"></param>
        public abstract void FillPortalSettingsBrief(Portal portal, int portalID);

        /// <summary>
        /// The PortalSettings Factory Method encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to render
        /// a Portal Page view for a given request.<br/>
        /// These Portal Settings are stored within a SQL database, and are
        /// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        /// This stored procedure returns values as SPROC output parameters,
        /// and using three result sets.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        public abstract Portal InstantiateNewPortal(int pageID, string portalAlias);

        /// <summary>
        /// The PortalSettings Factory Method encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to get
        /// custom setting for a different portal than current (EditPortal.aspx.cs)<br/>
        /// These Portal Settings are stored within a SQL database, and are
        /// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        /// This overload it is used
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        public abstract Portal InstantiateNewPortal(int portalID);

        /// <summary>
        /// The CreatePortal method create a new basic portal based on solutions table.
        /// </summary>
        /// <param name="solutionID">The solution ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        [History("john.mandia@whitelightsolutions.com", "2003/05/26", "Added extra info so that sign in is added to home tab of new portal and lang switcher is added to module list")]
        [History("bja@reedtek.com", "2003/05/16", "Added extra parameter for collpasable window")]
        public abstract int CreatePortal(int solutionID, string portalAlias, string portalName, string portalPath);

        /// <summary>
        /// Creates the portal.
        /// </summary>
        /// <param name="templateID">The template ID.</param>
        /// <param name="templateAlias">The template alias.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        public abstract int CreatePortal(int templateID,
                                string templateAlias,
                                string portalAlias,
                                string portalName,
                                string portalPath);

        /// <summary>
        /// Gets the portals.
        /// </summary>
        /// <returns></returns>
        public abstract IList<PortalAliasItem> GetPortalAliasesList();

        /// <summary>
        /// Removes portal from database. All tabs, modules and data wil be removed.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        public abstract void DeletePortal(int portalID);

        /// <summary>
        /// The UpdatePortalInfo method updates the name and access settings for the portal.<br/>
        /// Uses UpdatePortalInfo Stored Procedure.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <param name="alwaysShow">if set to <c>true</c> [always show].</param>
        public abstract void UpdatePortalInfo(int portalID, string portalName, string portalPath, bool alwaysShow);

        /// <summary>
        /// The GetPortals method returns an ArrayList containing all of the
        /// Portals registered in this database.<br/>
        /// </summary>
        /// <returns></returns>
        public abstract ArrayList GetPortals();

        /// <summary>
        /// Gets the portal base settings.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        public abstract Hashtable GetPortalBaseSettings(string portalPath);

        /// <summary>
        /// Get the proxy parameters as configured in web.config by Phillo 22/01/2003
        /// </summary>
        /// <returns></returns>
        public abstract WebProxy GetProxy();

        /// <summary>
        /// The PortalSettings.GetPortalSettings Method returns a hashtable of
        /// custom Portal specific settings from the database. This method is
        /// used by Portals to access misc settings.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="baseSettings">The _base settings.</param>
        /// <returns></returns>
        public abstract Hashtable GetPortalCustomSettings(int portalID, Hashtable baseSettings);

        /// <summary>
        /// Flushes the base settings cache.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        public abstract void FlushBaseSettingsCache(string portalPath);

        /// <summary>
        /// The get tab root should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public abstract PageStripDetails GetRootPage(int parentPageID, ArrayList tabList);

        /// <summary>
        /// The GetRootPage should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public abstract PageStripDetails GetRootPage(PortalPage tab, ArrayList tabList);

        ///<summary>
        ///</summary>
        public abstract void StartScheduler();

        ///<summary>
        /// ************ 'calculate' response to this request ************
        ///
        /// Test 1 - try requested Alias and requested PageID
        /// Test 2 - try requested Alias and PageID 0
        /// Test 3 - try default Alias and requested PageID
        /// Test 4 - try default Alias and PageID 0
        ///
        /// The UrlToleranceLevel determines how many times the test is allowed to fail before the request is considered
        /// to be "an error" and is therefore redirected:
        ///
        /// UrlToleranceLevel 1 
        ///		- requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, InvalidPageIdRedirect page is shown
        /// 
        /// UrlToleranceLevel 2 
        ///		- requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, PageID 0 (Home page) is shown
        ///
        /// UrlToleranceLevel 3 - !!!!!!!! not working?
        ///		- if requested Alias is invalid, default Alias will be used
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, InvalidPageIdRedirect page is shown
        /// 
        /// UrlToleranceLevel 4 - 
        ///		- if requested Alias is invalid, default Alias will be used
        ///		- if requested PageID is found, it is shown
        ///		- if requested PageID is not found, PageID 0 (Home page) is shown
        ///
        ///</summary>
        ///<exception cref="RainbowRedirectException"></exception>
        public void CalculatePortalResponse()
        {
            Portal portalSettings = null;
            int pageID = RainbowContext.Current.PageID; // Get PageID from QueryString
            string portalAlias = RainbowContext.Current.UniqueID; // Get requested alias from querystring, cookies or hostname
            string defaultAlias = Config.DefaultPortal; // get default portal from config

            // load arrays with values to test
            string[] testAlias = new string[4] { portalAlias, portalAlias, defaultAlias, defaultAlias };
            int[] testPageID = new int[4] { pageID, 0, pageID, 0 };

            int testsAllowed = Config.UrlToleranceLevel;
            int testsToRun = testsAllowed > 2 ? 4 : 2;
            // if requested alias is default alias, limit UrlToleranceLevel to max value of 2 and limit tests to 2
            if (portalAlias == defaultAlias)
            {
                testsAllowed = testsAllowed % 2;
                testsToRun = 2;
            }

            int testsCounter = 1;
            while (testsCounter <= testsToRun)
            {
                //try with current values from arrays
                portalSettings = InstantiateNewPortal(testPageID[testsCounter - 1], testAlias[testsCounter - 1]);

                // test returned result
                if (portalSettings.PortalAlias != null)
                {
                    break; // successful hit
                }
                else
                {
                    testsCounter++; // increment the test counter and continue
                }
            }

            if (portalSettings == null || portalSettings.PortalAlias == null)
            {
                // critical error - neither requested alias nor default alias could be found in DB
                throw new RainbowRedirectException(
                    Config.NoPortalErrorRedirect,
                    LogLevel.Fatal,
                    Config.NoPortalErrorResponse,
                    "Unable to load any portal - redirecting request to ErrorNoPortal page.",
                    null);
            }

            HttpContext httpContext = RainbowContext.Current.HttpContext;
            if (testsCounter <= testsAllowed) // success
            {
                // Portal Settings has passed the test so add it to Context
                httpContext.Items.Add("PortalSettings", portalSettings);
                httpContext.Items.Add("PortalID", portalSettings.PortalID); // jes1111
            }
            else // need to redirect
            {
                if (portalSettings.PortalAlias != portalAlias) // we didn't get the portal we asked for
                {
                    throw new RainbowRedirectException(
                        Config.InvalidAliasRedirect,
                        LogLevel.Info,
                        HttpStatusCode.NotFound,
                        "Invalid Alias specified in request URL - redirecting (404) to InvalidAliasRedirect page.",
                        null);
                }

                if (portalSettings.ActivePage.PageID != pageID) // we didn't get the page we asked for
                {
                    throw new RainbowRedirectException(
                        Config.InvalidPageIdRedirect,
                        LogLevel.Info,
                        HttpStatusCode.NotFound,
                        "Invalid PageID specified in request URL - redirecting (404) to InvalidPageIdRedirect page.",
                        null);
                }
            }

            // Save cookies
            //saveCookie = true; // Jes1111 - why is this always set to true? is it needed?
            //ExtendCookie(settings); 
            //if (saveCookie) // Jes1111 - why is this always set to true? is it needed?
            //{
            httpContext.Response.Cookies["PortalAlias"].Path = "/";
            httpContext.Response.Cookies["PortalAlias"].Value = portalSettings.PortalAlias;
            //}

            //Try to get alias from cookie to determine if alias has been changed
            bool refreshSite = false;
            if (httpContext.Request.Cookies["PortalAlias"] != null &&
                httpContext.Request.Cookies["PortalAlias"].Value.ToLower() != RainbowContext.Current.UniqueID)
            {
                refreshSite = true; //Portal has changed since last page request
            }
            // if switching portals then clean parameters [TipTopWeb]
            // Must be the last instruction in this method 

            // 5/7/2006 Ed Daniel
            // Added hack for Http 302 by extending condition below to check for more than 3 cookies
            if (refreshSite && httpContext.Request.Cookies.Keys.Count > 3)
            {
                // Signout and force the browser to refresh only once to avoid any dead-lock
                if (httpContext.Request.Cookies["refreshed"] == null
                    || (httpContext.Request.Cookies["refreshed"] != null
                        && httpContext.Response.Cookies["refreshed"].Value == "false"))
                {
                    string rawUrl = httpContext.Request.RawUrl;

                    // jes1111 - not needed now
                    //					//by Manu avoid endless loop when portal does not exists
                    //					if (rawUrl.EndsWith("init")) // jes1111: is this still valid/needed?
                    //						context.Response.Redirect("~/app_support/ErrorNoPortal.html", true);
                    //
                    //					// add parameter at the end of the command line to detect the dead-lock 
                    //					if (rawUrl.LastIndexOf(@"?") > 0)
                    //						rawUrl += "&init";
                    //					else rawUrl += "?init";

                    httpContext.Response.Cookies["refreshed"].Value = "true";
                    httpContext.Response.Cookies["refreshed"].Path = "/";
                    httpContext.Response.Cookies["refreshed"].Expires = DateTime.Now.AddMinutes(1);

                    // sign-out, if refreshed param on the command line we will not call it again
                    SignOnController.SignOut(rawUrl, false);
                }
            }

            // invalidate cookie, so the page can be refreshed when needed
            if (httpContext.Request.Cookies["refreshed"] != null && httpContext.Request.Cookies.Keys.Count > 3)
            {
                httpContext.Response.Cookies["refreshed"].Path = "/";
                httpContext.Response.Cookies["refreshed"].Value = "false";
                httpContext.Response.Cookies["refreshed"].Expires = DateTime.Now.AddMinutes(1);
            }
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings persistence.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void UpdatePortalSetting(int portalID, string key, string value)
        {
            UpdatePortalSettingSpecific(portalID, key, value);
            CurrentCache.Remove(Key.PortalSettings());
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings persistence.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected abstract void UpdatePortalSettingSpecific(int portalID, string key, string value);

        /// <summary>
        /// Gets the current image from theme.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="bydefault">The bydefault.</param>
        /// <returns>A string value...</returns>
        public string GetCurrentImageFromTheme (string name, string bydefault) 
        {
            // Obtain PortalSettings from Current Context
            if (CurrentPortal != null)
            {
                return CurrentPortal.GetCurrentTheme().GetImage(name, bydefault).ImageUrl;
            }
            return bydefault;
        }

        protected static void SetCustomSettings(Portal portal, Hashtable customSettings)
        {
            portal.CustomSettings = customSettings;
        }

        protected static Portal GetNew()
        {
            return new Portal();
        }

        protected static void SetPortalPath(PortalPage portalPage, string portalPath)
        {
            // added Thierry (tiptopweb) used for dropdown for layout and theme
            portalPage.PortalPath = portalPath;
        }

        protected static void SetPageID(PortalPage portalPage, int pageID)
        {
            portalPage.PageID = pageID;
        }
    }
}
