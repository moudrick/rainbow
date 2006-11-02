using System.Web;
using FredCK;
using Rainbow.Configuration;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// FCKTextBox is a wrapper for FredCK.FCKeditor.
	/// </summary>
	[History("jviladiu@portalservices.net", "2004/06/09", "First Implementation FCKEditor in Rainbow")]
	public class FCKTextBox : FCKeditor, IHtmlEditor
	{
		/// <summary>
		/// Control Text
		/// </summary>
		public string Text
		{
			get 
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		private string _imageFolder = string.Empty;
		/// <summary>
		/// Control Image Folder
		/// </summary>
		public string ImageFolder
		{
			get 
			{
				if (_imageFolder == string.Empty) 
				{
					PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
					if(pS.CustomSettings != null) 
					{
						if (pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null) 
						{
							_imageFolder = pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"].ToString();
						}
					}
				}
				return "/images/" + _imageFolder;
			}
			set
			{
				_imageFolder = value;
			}
		}
	}
}