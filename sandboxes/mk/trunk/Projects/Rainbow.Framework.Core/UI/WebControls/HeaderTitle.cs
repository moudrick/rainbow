using System.Web;
using System.Web.UI.WebControls;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Web.UI.WebControls
{
    ///<summary>
    ///</summary>
    public class HeaderTitle : Label
    {
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        override public void DataBind()
        {
            if (HttpContext.Current != null)
            {
                // Dynamically Populate the Portal Site Name
                Text = PortalProvider.Instance.CurrentPortal.PortalName;
            }
            base.DataBind();
        }
    }
}
