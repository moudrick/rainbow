using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Security;
using Rainbow.Context;
using Rainbow.Framework;
using Rainbow.Framework.Core;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Core.DAL;
using Rainbow.Framework.Exceptions;
using Rainbow.Framework.Scheduler;
using Rainbow.Framework.Security;
using Rainbow.Framework.Settings;
using History=Rainbow.Framework.History;
using Path=System.IO.Path;
using Reader=Rainbow.Context.Reader;

namespace Rainbow
{
    /// <summary>
    /// Defines the methods, properties, and events common to all application 
    /// objects within an ASP.NET application. This class is the base class for 
    /// applications defined by the user in the global.asax file.
    /// </summary>
    /// <remarks>
    /// Instances of this class are created in the ASP.NET infrastructure, not by the user directly. One instance is used to process many requests in its lifetime but it can process only one at a time. Thus member variables can be used to store per-request data.
    /// An application executes events that are handled by modules or user code 
    /// defined in the global.asax file in the following sequence:
    /// <list type="string">
    /// <item>BeginRequest</item>
    /// <item>AuthenticateRequest</item>
    /// <item>AuthorizeRequest</item>
    /// <item>ResolveRequestCache</item>
    /// <item>[A handler (a page corresponding to the request URL) is created at this point.]</item>
    /// <item>AcquireRequestState</item>
    /// <item>PreRequestHandlerExecute</item>
    /// <item>[The handler is executed.]</item>
    /// <item>PostRequestHandlerExecute</item>
    /// <item>ReleaseRequestState</item>
    /// <item>[Response filters, if any, filter the output.]</item>
    /// <item>UpdateRequestCache</item>
    /// <item>EndRequest</item>
    /// </list>
    /// </remarks>
    [History("Yannick Smits", "2006/08/27", "added fix to prevent 302 redirects")]
    [History("jonathan minond", "2006/03/04", "cleaned up a bit for rainbow 2.0")]
    [History("jminond", "2005/03/08", "added session timeout ability. controled in rainbow.conif cookieexpire")]
    [History("john.mandia@whitelightsolutions.com", "2004/05/29", "changed portal alias checking behavior")]
    [History("manu", "2002/11/29", "changed portal alias behavior")]
    [History("cory isakson", "2003/02/13", "changed portal alias behavior: always carry portal alias in URL or QS")]
    [History("bill anderson", "2003/05/13", "track user information for anonymous (min/max/close)")]
    [History("cory isakson", "2003/09/10", "added overrides for WebCompile Feature")]
    public class Global : HttpApplication
    {
        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// The Application_BeginRequest method is an ASP.NET event that executes
        /// on each web request into the portal application.  The below method
        /// obtains the current pageIndex and PageID from the querystring of the
        /// request -- and then obtains the configuration necessary to process
        /// and render the request.
        /// This portal configuration is stored within the application's "Context"
        /// object -- which is available to all pages, controls and components
        /// during the processing of a single request.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // TODO: Need to support page name with no id, search db for match
            // TODO: If not, return page with recomended matches..
            RainbowContext.Current.RewritePath();
            RainbowContext.Current.CheckAllPortalsLock();
            
            // 3rd Check: is database/code version correct?
                // don't check database when installer is running
            string executionFilePath = Request.AppRelativeCurrentExecutionFilePath.ToLower();
            if (executionFilePath != Config.InstallerRedirect.ToLower() &&
                executionFilePath != "~/webresource.axd")
            {
                RainbowContext.Current.CheckDatabaseVersion();
                RainbowContext.Current.CalculatePortalResponse();
            }
        } // end of Application_BeginRequest


        /// <summary>
        /// Handles the AuthenticateRequest event of the Application control.
        /// If the client is authenticated with the application, then determine
        /// which security roles he/she belongs to and replace the "User" intrinsic
        /// with a custom IPrincipal security object that permits "User.IsInRole"
        /// role checks within the application
        /// Roles are cached in the browser in an in-memory encrypted cookie.  If the
        /// cookie doesn't exist yet for this session, create it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            Reader contextReader = new Reader(new WebContextReader());
            HttpContext context = contextReader.Current;

            if (context.Items["PortalSettings"] != null) //PortalProvider.Instance.CurrentPortal
            {
                // Obtain PortalSettings from Current Context
                PortalSettings portalSettings = (PortalSettings) context.Items["PortalSettings"];

                // Auto-login a user who has a portal Alias login cookie
                // Try to authenticate the user with the cookie value
                if (!context.Request.IsAuthenticated &&
                    (context.Request.Cookies["Rainbow_" + portalSettings.PortalAlias] != null))
                {
                    if (context.Request.Cookies["Rainbow_" + portalSettings.PortalAlias].Expires > DateTime.Now)
                    {
                        string user;
                        user = context.Request.Cookies["Rainbow_" + portalSettings.PortalAlias.ToLower()].Value;

                        //jminond - option to kill cookie after certain time always
                        int minuteAdd = Config.CookieExpire;

                        // Create the FormsAuthentication cookie
                        FormsAuthentication.SetAuthCookie(user, true);

                        // Create a FormsAuthentication ticket.
                        FormsAuthenticationTicket cTicket = new FormsAuthenticationTicket
                            (
                            1, // version
                            user, // user name
                            DateTime.Now, // issue time
                            DateTime.Now.AddMinutes(minuteAdd),
                            false, // don't persist cookie
                            string.Empty // roles
                            );

                        // Set the current User Security to the FormsAuthenticated User
                        context.User = new RainbowPrincipal(new FormsIdentity(cTicket), null);
                    }
                }
                else
                {
                    // jminond - if user asked to persist, he should have a cookie
                    if ((context.Request.IsAuthenticated) &&
                        (context.Request.Cookies["Rainbow_" + portalSettings.PortalAlias] == null))
                    {
                        PortalSecurity.KillSession();
                    }
                }

                //if (context.Request.IsAuthenticated && !(context.User is WindowsPrincipal))
                //{
                //    // added by Jonathan Fong 22/07/2004 to support LDAP 
                //    //string[] names = Context.User.Identity.Name.Split("|".ToCharArray());
                //    string[] names = context.User.Identity.Name.Split('|');
                //    if (names.Length == 3 && names[2].StartsWith("cn="))
                //    {
                //        context.User = new RainbowPrincipal(
                //            new User(context.User.Identity.Name, "LDAP"), LDAPHelper.GetRoles(names[2]));
                //    }
                //    else
                //    {
                //        // Add our own custom principal to the request containing the roles in the auth ticket
                //        context.User = new RainbowPrincipal(context.User.Identity, PortalSecurity.GetRoles());
                //    }
                //    // Remove Windows specific custom settings
                //    if (portalSettings.CustomSettings != null)
                //        portalSettings.CustomSettings.Remove("WindowsAdmins");
                //}
                //    // bja@reedtek.com - need to get a unique id for user
                //else if (Config.WindowMgmtControls)
                //{
                //    // Need a uid, even for annoymous users
                //    string annoyUser;
                //    // cookie bag
                //    IWebBagHolder abag = BagFactory.instance.create(BagFactory.BagFactoryType.CookieType);
                //    // user data already set
                //    annoyUser = (string) abag[GlobalInternalStrings.UserWinMgmtIndex];
                //    // if no cookie then let's get one
                //    if (annoyUser == null)
                //    {
                //        // new uid for window mgmt
                //        Guid guid = Guid.NewGuid();
                //        // save the data into a cookie bag
                //        abag[GlobalInternalStrings.UserWinMgmtIndex] = guid.ToString();
                //    }
                //}
            }
        } // end of Application_AuthenticateRequest

        /// <summary>
        /// Handler for unhandled exceptions
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // TODO : Uncomment this after magic url's are tested to work.
//#if (!DEBUG)
            ErrorHandler.ProcessUnhandledException();
//#endif
        }

        /// <summary>
        /// Application_Start()
        /// </summary>
        protected void Application_Start()
        {
            ErrorHandler.Publish(LogLevel.Info, "Application Started: code version " 
                + DatabaseUpdater.CodeVersion);

            if (Config.CheckForFilePermission)
            {
                try
                {
                    string newDir = Path.Combine(Framework.Settings.Path.ApplicationPhysicalPath, "_createnewdir");
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                    if (Directory.Exists(newDir))
                    {
                        Directory.Delete(newDir);
                    }
                }
                catch (Exception ex)
                {
                    throw new RainbowException(LogLevel.Fatal, HttpStatusCode.ServiceUnavailable,
                                               "ASPNET Account does not have rights to the filesystem", ex); // Jes1111
                }
            }

            // TODO: Need country region store handling....
            /*
			//Esperantus settings
			//If not found in web.config defaults are provided here
			try
			{
				// Get Esperantus config
				NameValueCollection mySettings = (NameValueCollection) ConfigurationManager.GetSection("Esperantus");

				//Initializes default key and countries store
				if (mySettings != null) //Esperantus section must be specified anyway
				{
					string baseDir = "/";
					if (context.Request.ApplicationPath.Length > 0)
						baseDir = context.Request.ApplicationPath;
					baseDir = Server.MapPath(baseDir);
					string binDir = Path.Combine(baseDir, "bin");
                    string resDir = Path.Combine(baseDir, "App_GlobalResources");
                    // Try to get KeyStore, if not found set defaults
                    //string dllName = Assembly.GetExecutingAssembly().Location + Assembly.GetExecutingAssembly().FullName + ".dll";

					// Try to get KeyStore, if not found set defaults
					if (mySettings["KeysStore"] == null && mySettings["KeysStoreParameters"] == null)
					{
						string esperantusConfig = @"Path=" + resDir + ";FilesPrefix=Rainbow";
						//string esperantusConfig = @"AssemblyName=" + Path.Combine(binDir, "Rainbow.dll") + ";KeysSubStore=Rainbow.Resources.Rainbow;Path=" + resDir + ";FilesPrefix=Rainbow";
						DataFactory.SetDefaultKeysStore("StoreXMLResxFiles", esperantusConfig);
						//DataFactory.SetDefaultKeysStore("Hybrid", esperantusConfig);
					}
					
					// CountryRegionsStore as rainbow default
					if (mySettings["CountryRegionsStore"] == null && mySettings["CountryRegionsStoreParameters"] == null)
					{
						// DataFactory.SetDefaultCountryRegionsStore("Resources", @"AssemblyName=" + Path.Combine(binDir, "Rainbow.AppCode.dll") + ";CountriesSubStore=Resources.Rainbow.Countries.xml");
                        DataFactory.SetDefaultCountryRegionsStore("Resources", @"AssemblyName=" + binDir + @"\App_GlobalResources.dll;CountriesSubStore=App_GlobalResources.Resources.Countries.xml");
					}

					// ***********************************************
					// * Uncomment this code for overwrite Resources/Countries.xml
					// * with you custom database list, then rebuil reainbow
					// * and comment it again
					// ***********************************************
					//				if (mySettings["CountryRegionsStore"] == null && mySettings["CountryRegionsStoreParameters"] == null)
					//				{
					//					// We try to convert a SQLProvider string in a valid OleDb SQL connection string. It is not perfect.
					//					//jes1111 - string toBeTransformedConnectionString = ConfigurationSettings.AppSettings["connectionString"].ToLower();
					//					string toBeTransformedConnectionString = Config.ConnectionString.ToLower();
					//					toBeTransformedConnectionString = toBeTransformedConnectionString.Replace("trusted_connection=true", "Integrated Security=SSPI");
					//					toBeTransformedConnectionString = toBeTransformedConnectionString.Replace("database=", "Initial Catalog=");
					//					toBeTransformedConnectionString = toBeTransformedConnectionString.Replace("server=", "Data Source=");
					//					//Rainbow.Framework.Helpers.LogHelper.Logger.Log(Rainbow.Framework.Site.Configuration.LogLevel.Info, "Running Esperantus Countries from: " + toBeTransformedConnectionString);
					//					Esperantus.Data.DataFactory.SetDefaultCountryRegionsStore("OleDB", "Provider=SQLOLEDB.1;" + toBeTransformedConnectionString);
					//
					//					Esperantus.CountryInfo.SaveCountriesAsXML(Esperantus.CountryTypes.AllCountries, Esperantus.CountryFields.Name, Path.Combine(resDir, "Countries.xml"));
					//				}
				}
			}
			catch (Exception ex)
			{
				throw new RainbowException(Rainbow.Framework.LogLevel.Fatal, HttpStatusCode.ServiceUnavailable, "Error loading Esperantus settings in Application_Start", ex); // Jes1111
			}
             */

            //Start scheduler
            if (Config.SchedulerEnable)
            {
                PortalProvider.Instance.Scheduler = CachedScheduler.GetScheduler(
                    HttpContext.Current.Server.MapPath(Framework.Settings.Path.ApplicationRoot),
                    Config.SqlConnectionString,
                    Config.SchedulerPeriod,
                    Config.SchedulerCacheSize);
                PortalProvider.Instance.Scheduler.Start();
            }
            // Start proxy
            if (Config.UseProxyServerForServerWebRequests)
            {
                WebRequest.DefaultWebProxy = PortalProvider.Instance.GetProxy();
            }
        }

        /// <summary>
        /// Application ends.
        /// </summary>
        public void Application_OnEnd()
        {
            ErrorHandler.Publish(LogLevel.Info, "Application Ended");
        }
    }
}
