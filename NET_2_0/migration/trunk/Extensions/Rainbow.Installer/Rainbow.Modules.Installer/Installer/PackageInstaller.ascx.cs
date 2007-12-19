/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Initial coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 2005/01/10
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Rainbow;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Security;
using Rainbow.Configuration;

namespace Rainbow.DesktopModules.Installer
{
    /// <summary>
    /// Control to show/edit portals modules (AdminAll)
    /// </summary>
    public class PackageInstaller : PortalModuleControl 
    {
        protected System.Web.UI.WebControls.DataList defsList;

		/// <summary>
		/// Admin Module
		/// </summary>
		public override bool AdminModule
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// The Page_Load server event handler on this user control is used
		/// to populate the current defs settings from the configuration system
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e) 
        {
            if (!Page.IsPostBack) 
                BindData();
        }
    
		/// <summary>
		/// The BindData helper method is used to bind the list of 
		/// module definitions for this portal to an asp:datalist server control
		/// </summary>
        private void BindData() 
        {
            // Get the portal's defs from the database
            defsList.DataSource = new ModulesDB().GetModuleDefinitions();
            defsList.DataBind();
        }
  
		public override Guid GuidID 
		{
			get
			{
				return new Guid("A7D07FC8-CFF6-45ff-ACCC-284EE0110B19");
			}
		}

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
			InitializeComponent();
			this.AddUrl = "~/DesktopModules/Installer/ManagePackages.aspx";
			this.AddText =Esperantus.Localize.GetString("MANAGE_PACKAGES","Manage Packages");
			base.OnInit(e);
		}

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() 
        {
			this.defsList.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.defsList_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// The DefsList_ItemCommand server event handler on this page 
		/// is used to handle the user editing module definitions
		/// from the DefsList &lt;asp:datalist&gt; control
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void defsList_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			Guid GeneralModDefID = new Guid(defsList.DataKeys[e.Item.ItemIndex].ToString());
			
			// Go to edit page
			Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/Installer/EditPortalModules.aspx", TabID, "DefID=" + GeneralModDefID + "&Mid=" + ModuleID));
		}

    }
}