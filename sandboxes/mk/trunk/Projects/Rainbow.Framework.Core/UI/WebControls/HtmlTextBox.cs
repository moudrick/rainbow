using ActiveUp.WebControls.HtmlTextBox;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// HtmlTextBox is a wrapper for ActiveUp.HtmlTextBox.HtmlTextBox.
    /// </summary>
    public class HtmlTextBox : Editor, IHtmlEditor
    {
        string imageFolder = string.Empty;

        /// <summary>
        /// Control Image Folder
        /// </summary>
        /// <value></value>
        public string ImageFolder
        {
            get
            {
                if (imageFolder == string.Empty)
                {
                    Portal portal = PortalProvider.Instance.CurrentPortal;
                    if (portal.CustomSettings != null)
                    {
                        if (portal.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null)
                        {
                            imageFolder = portal.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"].ToString();
                        }
                    }
                }
                return "/images/" + imageFolder;
            }
            set { imageFolder = value; }
        }
    }
}
