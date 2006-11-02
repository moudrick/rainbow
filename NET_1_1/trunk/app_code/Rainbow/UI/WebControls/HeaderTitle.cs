using System.Web;
using System.Web.UI.WebControls;
using Rainbow.Configuration;

namespace Rainbow.UI.WebControls
{
    public class HeaderTitle : Label
    {
        override public void DataBind()
        {
            if(HttpContext.Current != null)
            {
                // Obtain PortalSettings from Current Context
            	PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

                // Dynamically Populate the Portal Site Name
                this.Text = portalSettings.PortalName;
            }

            base.DataBind();
        }
    }
}