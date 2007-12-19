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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Security;
using Rainbow.Configuration;
using Esperantus;

namespace Rainbow.DesktopModules.Installer 
{
	/// <summary>
	/// Add/Remove modules, assign modules to portals
	/// </summary>
	public class EditPortalModules : Rainbow.UI.EditItemPage
	{
		protected Esperantus.WebControls.Label Label1;
		protected Esperantus.WebControls.Label Label2;
		protected Esperantus.WebControls.Label Label3;
		protected Esperantus.WebControls.Label Label4;
		protected Esperantus.WebControls.Label Label5;
		protected System.Web.UI.WebControls.Label lblGUID;
		protected Esperantus.WebControls.LinkButton selectAllButton;
		protected Esperantus.WebControls.LinkButton selectNoneButton;
		protected Esperantus.WebControls.Label Label6;
		protected System.Web.UI.WebControls.CheckBoxList PortalsName;
		protected System.Web.UI.HtmlControls.HtmlTable tableManual;
		protected System.Web.UI.WebControls.Label lblErrorDetail;
		protected System.Web.UI.WebControls.Label MobileSrc;
		protected System.Web.UI.WebControls.Label DesktopSrc;
		protected System.Web.UI.WebControls.Label FriendlyName;

		//protected Esperantus.WebControls.LinkButton updateButton;
		Guid defID;

		/// <summary>
		/// The Page_Load server event handler on this page is used
		/// to populate the role information for the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Verify that the current user has access to access this page
			// Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
			if (PortalSecurity.IsInRoles("Admins") == false) 
				PortalSecurity.AccessDeniedEdit();

			// Calculate security defID
			if (Request.Params["defID"] != null) 
				defID = new Guid(Request.Params["defID"]);

			ModulesDB modules = new ModulesDB();
            
			// If this is the first visit to the page, bind the definition data 
			if (Page.IsPostBack == false)
			{

				if (defID.ToString() == "00000000-0000-0000-0000-000000000000") 
				{
					// blank definition
					FriendlyName.Text = ".";
					DesktopSrc.Text = "..";
					MobileSrc.Text = "..";
				}
				else 
				{
					// Obtain the module definition to edit from the database
					SqlDataReader dr = modules.GetSingleModuleDefinition(defID);
                
					// Read in first row from database
					while(dr.Read())
					{
						FriendlyName.Text = (string) dr["FriendlyName"].ToString();
						DesktopSrc.Text = (string) dr["DesktopSrc"].ToString();
						MobileSrc.Text = (string) dr["MobileSrc"].ToString();
						lblGUID.Text = dr["GeneralModDefID"].ToString();
					}
					dr.Close(); //by Manu, fixed bug 807858
				}

				// Populate checkbox list with all portals
				// and "check" the ones already configured for this tab
				SqlDataReader portals = modules.GetModuleInUse(defID);

				// Clear existing items in checkboxlist
				PortalsName.Items.Clear();

				while(portals.Read()) 
				{
					if (Convert.ToInt32(portals["PortalID"]) >= 0)
					{
						ListItem item = new ListItem();
						item.Text = (string) portals["PortalName"];
						item.Value = portals["PortalID"].ToString();
            
						if ((portals["checked"].ToString()) == "1") 
							item.Selected = true;
						else
							item.Selected = false;

						PortalsName.Items.Add(item);
					}
				}
				portals.Close(); //by Manu, fixed bug 807858
			}
		}

		/// <summary>
		/// Set the module guids with free access to this page
		/// </summary>
		protected override ArrayList AllowedModules
		{
			get
			{
				ArrayList al = new ArrayList();
				al.Add ("A7D07FC8-CFF6-45ff-ACCC-284EE0110B19"); // PackageInstaller.ascx
				return al;
			}
		}

		/// <summary>
		/// OnUpdate installs or refresh module definiton on db
		/// </summary>
		/// <param name="e"></param>
		protected override void OnUpdate(EventArgs e)
		{
			if (Page.IsValid) 
			{
				try
				{
					ModulesDB modules = new ModulesDB();

					// Update the module definition
					for (int i = 0; i < PortalsName.Items.Count; i++)
					{
						modules.UpdateModuleDefinitions(defID, Convert.ToInt32(PortalsName.Items[i].Value), (bool) PortalsName.Items[i].Selected);
					}
	            
					// Redirect back to the portal admin page
					RedirectBackToReferringPage();
				}
				catch(Exception ex)
				{
					lblErrorDetail.Text = Esperantus.Localize.GetString("MODULE_DEFINITIONS_INSTALLING", "An error occurred installing.", this) + "<br>";
					lblErrorDetail.Text += " Module: '" + FriendlyName.Text + "' - Source: '" + DesktopSrc.Text + "' - Mobile: '" + MobileSrc.Text + "'";
					lblErrorDetail.Visible = true;

					Rainbow.Configuration.ErrorHandler.HandleException(lblErrorDetail.Text, ex);
				}
			}
		}


	
		private void selectAllButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < PortalsName.Items.Count; i++)
			{
				PortalsName.Items[i].Selected = true;
			}
		}

		private void selectNoneButton_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < PortalsName.Items.Count; i++)
			{
				PortalsName.Items[i].Selected = false;
			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// OnInit
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
			this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
			this.selectNoneButton.Click += new System.EventHandler(this.selectNoneButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}