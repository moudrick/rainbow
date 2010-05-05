using System.Web;
using FredCK.FCKeditorV2;
using Rainbow.Framework.Site.Configuration;

namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Syrinx is a wrapper for CkEditor.
    /// </summary>
    [History("jviladiu@portalservices.net", "2004/11/09", "Implementation of FCKEditor Version 2 in Rainbow")]
    [History("ramseur@gmail.com", "2010/04/28", "Implementation of CKEditor  in Rainbow")]
    public class FCKTextBoxV2 : Syrinx.Gui.AspNet.CkEditor, IHtmlEditor
    {
        /// <summary>
        /// Control Text
        /// </summary>
        /// <value></value>
        //public string Text
        //{
        //    get { return Text; }
        //    set {  = value; }

        //}

       

        private string _imageFolder = string.Empty;

        /// <summary>
        /// Control Image Folder
        /// </summary>
        /// <value></value>
        public string ImageFolder
        {
            get
            {
                if (_imageFolder == string.Empty)
                {
                    PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                    if (pS.CustomSettings != null)
                    {
                        if (pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null)
                        {
                            _imageFolder = pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"].ToString();
                        }
                    }
                }
                return "/images/" + _imageFolder;
            }
            set { _imageFolder = value; }
        }
    }
}