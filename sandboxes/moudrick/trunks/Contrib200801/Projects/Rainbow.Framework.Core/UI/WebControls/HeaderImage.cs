using System.Web.UI.WebControls;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// HeaderImage
    /// </summary>
    public class HeaderImage : Image
    {
        /// <summary>
        /// DataBind
        /// </summary>
        public override void DataBind()
        {
            Portal portal = PortalProvider.Instance.CurrentPortal;
            if (portal != null)
            {
                //PortalImage
                if (portal.CustomSettings["SITESETTINGS_LOGO"] != null &&
                    portal.CustomSettings["SITESETTINGS_LOGO"].ToString().Length != 0)
                {
                    ImageUrl =
                        Path.WebPathCombine(Path.ApplicationRoot, portal.PortalPath,
                                            portal.CustomSettings["SITESETTINGS_LOGO"].ToString());
                    // Added by Mario Endara to Reinforce portal Title for Search Engines <mario@softworks.com.uy>
                    if (portal.CustomSettings["SITESETTINGS_PAGE_TITLE"] != null &&
                        portal.CustomSettings["SITESETTINGS_PAGE_TITLE"].ToString().Length != 0)
                    {
                        AlternateText = portal.CustomSettings["SITESETTINGS_PAGE_TITLE"].ToString();
                    }
                    Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }
            base.DataBind();
        }
    }
}
