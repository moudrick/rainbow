using System.Configuration;
using System.Web;

namespace Rainbow.Framework.ReWrite
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExUrlMappingModule : RegExUrlMappingBaseModule
    {
        /// <summary>
        /// Rewrites the specified requested path.
        /// </summary>
        /// <param name="requestedPath">The requested path.</param>
        /// <param name="app">The app.</param>
        public override void Rewrite(string requestedPath, HttpApplication app)
        {
            RegExUrlMappingConfigHandler config =
                ((RegExUrlMappingConfigHandler) (ConfigurationManager.GetSection("RegExUrlMapping")));

            string pathOld;

            string pathNew;

            if (config.Enabled())
            {
                pathOld = app.Request.Url.ToString();

                string requestedPage = app.Request.Url.ToString().ToLower();

                if (requestedPage.IndexOf("?") > -1)
                {
                    requestedPage = requestedPage.Substring(0, requestedPage.IndexOf("?"));
                }

                string appVirtualPath = app.Request.ApplicationPath;

                if (requestedPage.Length >= appVirtualPath.Length)
                {
                    if (requestedPage.Substring(0, appVirtualPath.Length).ToLower() == appVirtualPath.ToLower())
                    {
                        requestedPage = requestedPage.Substring(appVirtualPath.Length);

                        if (requestedPage.Substring(0, 1) == "/")
                        {
                            requestedPage = "~" + requestedPage;
                        }
                        else
                        {
                            requestedPage = "~/" + requestedPage;
                        }
                    }
                }

                pathNew = config.MappedUrl(requestedPage);

                if (pathNew.Length > 0)
                {
                    if (pathNew.IndexOf("?") > -1)
                    {
                        if (pathOld.IndexOf("?") > -1)
                        {
                            pathNew += "&" +
                                       pathOld.Substring(pathOld.Length - pathOld.IndexOf("?") - 1, pathOld.Length);
                        }
                    }
                    else
                    {
                        if (pathOld.IndexOf("?") > -1)
                        {
                            //pathNew += pathOld.Substring(pathOld.Length - pathOld.IndexOf("?"), pathOld.Length);
                            pathNew += pathOld.Substring( pathOld.IndexOf( "?" ) );
                        }
                    }

                    HttpContext.Current.RewritePath(pathNew);
                }
            }
        }
    }
}