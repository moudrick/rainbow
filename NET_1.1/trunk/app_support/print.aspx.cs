using System;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.Settings;
using Rainbow.UI;
using Rainbow.UI.WebControls;

namespace Rainbow
{
    /// <summary>
    /// Module print page
    /// </summary>
    public class PrintPage : ViewItemPage
    {
		protected PlaceHolder PrintPlaceHolder;

		private void Page_Load(object sender, EventArgs e)
		{
			foreach (ModuleSettings module in portalSettings.ActivePage.Modules)
			{
				if (this.Request.Params["ModID"] != null && module.ModuleID == int.Parse(this.Request.Params["ModID"]))
				{
					// create an instance of the module
					PortalModuleControl myPortalModule = (PortalModuleControl) LoadControl(Path.ApplicationRoot + "/" + module.DesktopSrc);
					myPortalModule.PortalID = portalSettings.PortalID;                                  
					myPortalModule.ModuleConfiguration = module;

					// add the module to the placeholder
					PrintPlaceHolder.Controls.Add(myPortalModule);

					break;
				}
			}

		}

		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);

		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion
    }
}