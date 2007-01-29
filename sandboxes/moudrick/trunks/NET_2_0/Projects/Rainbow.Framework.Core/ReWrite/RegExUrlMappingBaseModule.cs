using System;
using System.Web;

namespace Rainbow.Framework.ReWrite
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExUrlMappingBaseModule : IHttpModule
    {
        /// <summary>
        /// Inits the specified app.
        /// </summary>
        /// <param name="app">The app.</param>
        public void Init(HttpApplication app)
        {
            app.AuthorizeRequest += new EventHandler(BaseModuleRewriter_AuthorizeRequest);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Handles the AuthorizeRequest event of the BaseModuleRewriter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        public void BaseModuleRewriter_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = ((HttpApplication) (sender));

            Rewrite(app.Request.Path, app);
        }

        /// <summary>
        /// Rewrites the specified requested path.
        /// </summary>
        /// <param name="requestedPath">The requested path.</param>
        /// <param name="app">The app.</param>
        public virtual void Rewrite(string requestedPath, HttpApplication app)
        {
        }
    }
}