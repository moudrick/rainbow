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
using Rainbow.Installer;

using Esperantus;

namespace Rainbow.DesktopModules.Installer
{
	/// <summary>
	/// Add/Remove modules, assign modules to portals
	/// </summary>
	public class ManagePackages : Rainbow.UI.AddEditItemPage // Rainbow.UI.EditItemPage
	{
		protected Esperantus.WebControls.Label Label1;
		protected Esperantus.WebControls.Label Label7;
		protected Esperantus.WebControls.RequiredFieldValidator Requiredfieldvalidator1;
		protected System.Web.UI.HtmlControls.HtmlTable tablePackageInfo;
		protected System.Web.UI.WebControls.Label lblErrorDetail;
		protected System.Web.UI.WebControls.ListBox InstallerFileList;
		protected Esperantus.WebControls.Label Label2;
		protected Esperantus.WebControls.Label Label3;
		protected Esperantus.WebControls.Label Label4;
		protected System.Web.UI.WebControls.Label lblPackageName;
		protected System.Web.UI.WebControls.Label lblPackageDescription;
		protected System.Web.UI.WebControls.HyperLink linkPackageInformationUrl;
		protected System.Web.UI.WebControls.Label lblPackageAuthor;
		protected Esperantus.WebControls.Label Label5;
		protected System.Web.UI.WebControls.Label lblPackageType;
		protected Esperantus.WebControls.Label Label6;
		protected System.Web.UI.WebControls.Label lblPackageVersion;
		protected System.Web.UI.WebControls.Label lblIsBeta;

		protected Esperantus.WebControls.Label Label8;
		protected System.Web.UI.WebControls.ListBox UninstallerFileList;
		protected Esperantus.WebControls.RequiredFieldValidator Requiredfieldvalidator2;

//		protected Esperantus.WebControls.LinkButton updateButton;
//		protected Esperantus.WebControls.LinkButton cancelButton;
//		protected Esperantus.WebControls.LinkButton deleteButton;


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
			//			if (PortalSecurity.IsInRoles("Admins") == false) 
			//				PortalSecurity.AccessDeniedEdit();

			//hide the error label
			lblErrorDetail.Visible = false; 

			// Calculate security defID
			if (Request.Params["defID"] != null) 
				defID = new Guid(Request.Params["defID"]);
			ModulesDB modules = new ModulesDB();

			// If this is the first visit to the page
			if (!Page.IsPostBack)
			{
				//if (defID.ToString() == "00000000-0000-0000-0000-000000000000") 

				// hide the Info table;
				tablePackageInfo.Visible =false;
				// new module packages
				SetInstallList();
				// new module definition
				SetUninstallList();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void SetUninstallList()
		{
			// new module definition
			UninstallerFileList.Items.Clear();
			//string[] _uninstallFiles = System.IO.Directory.GetFiles(PhysicalFolders.Uninstall ,"*.rbd");
			string[] _uninstallFiles = System.IO.Directory.GetFiles(PhysicalFolders.Uninstall ,"*.rbp");
			foreach (string _fileName in _uninstallFiles)
			{
				System.IO.FileInfo _fi = new System.IO.FileInfo(_fileName);
				UninstallerFileList.Items.Add(new ListItem(_fi.Name,_fi.FullName));
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void SetInstallList()
		{
			// new module definition
			InstallerFileList.Items.Clear();
			string[] _installFiles = System.IO.Directory.GetFiles(PhysicalFolders.Install ,"*.rbp");
			foreach (string _fileName in _installFiles)
			{
				System.IO.FileInfo _fi = new System.IO.FileInfo(_fileName);
				InstallerFileList.Items.Add(new ListItem(_fi.Name,_fi.FullName));
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
				string _packageFile = InstallerFileList.SelectedItem.Value;
				Rainbow.Installer.InstallManager _manager = null;
				try
				{
					_manager = new Rainbow.Installer.InstallManager(_packageFile);
					Rainbow.Installer.Package.PackageInformation _info= _manager.Package.Information;
					_manager.Install();
					System.IO.FileInfo _packageFileInfo = new System.IO.FileInfo(_packageFile);
					System.IO.File.Copy(PhysicalFolders.Install + _packageFileInfo.Name , PhysicalFolders.Uninstall + _packageFileInfo.Name);
					//System.IO.FileStream _filestream = System.IO.File.Create(PhysicalFolders.Uninstall + _manager.Package.InstallerDefinitionFile.FileName);
					//_filestream.Write (_manager.Package.InstallerDefinitionFile.FileBuffer,0,_manager.Package.InstallerDefinitionFile.FileBuffer.Length);
					//_filestream.Close();
					lblErrorDetail.Text = InstallerFileList.SelectedItem.Text + " has been installed.";
					lblErrorDetail.Visible = true;
				}
				catch(Exception ex)
				{
					lblErrorDetail.Text = Esperantus.Localize.GetString("MODULE_DEFINITIONS_INSTALLING", "An error occurred installing.", this) + "<br>";
					lblErrorDetail.Text += " Installer: " + InstallerFileList.SelectedItem.Value + "<br>";
					lblErrorDetail.Text += " Error: " + ex.Message + "<br>";

					foreach(Rainbow.Installer.Log.LogEntry _entry in _manager.Log)
					{
						lblErrorDetail.Text += "<br>"+_entry.TimeStamp.ToString()+":"+_entry.LogType.ToString() +":"+ _entry.Description ; 
						if (_entry.InnerException != null)
							lblErrorDetail.Text += "("+_entry.InnerException.Message+")";
					}
					lblErrorDetail.Visible = true;
					Rainbow.Configuration.ErrorHandler.HandleException(lblErrorDetail.Text, ex);
				}
				SetInstallList();
				SetUninstallList();
			}
		}

		/// <summary>
		/// Delete a definition
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDelete(EventArgs e)
		{
			try
			{


				string _packageFile = UninstallerFileList.SelectedItem.Value;
				Rainbow.Installer.InstallManager _manager = new InstallManager(_packageFile);
				_manager.Uninstall();
				System.IO.FileInfo _packageFileInfo = new System.IO.FileInfo(_packageFile);
				System.IO.File.Delete(_packageFileInfo.FullName);

				// renew the definitionlist
				SetUninstallList();
			}
			catch(Exception ex)
			{
				lblErrorDetail.Text = Esperantus.Localize.GetString("MODULE_DEFINITIONS_DELETE_ERROR", "An error occurred deleting module.", this) + "<br>";
				lblErrorDetail.Text += " Error: " + ex.Message;
				lblErrorDetail.Visible = true;
				Rainbow.Configuration.ErrorHandler.HandleException(lblErrorDetail.Text, ex);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InstallerFileList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (InstallerFileList.SelectedIndex > -1 )
				{
					string _installPackageFile = InstallerFileList.SelectedItem.Value;
					Rainbow.Installer.InstallManager _manager = new Rainbow.Installer.InstallManager(_installPackageFile);
					Rainbow.Installer.Package.PackageInformation _info= _manager.Package.Information;
					SetPackeInfo(_info);
					updateButton.Enabled = true;
					deleteButton.Enabled = false;
					UninstallerFileList.SelectedIndex = -1;
				}
				else
				{
					deleteButton.Enabled= false;

				}
			}
			catch(Exception ex)
			{
				lblErrorDetail.Text = Esperantus.Localize.GetString("MODULE_DEFINITIONS_INSTALLING", "An error occurred installing.", this) + "<br>";
				lblErrorDetail.Text += " Readind Package Information [" + InstallerFileList.SelectedItem.Text +"] failed!";
				lblErrorDetail.Visible = true;

				Rainbow.Configuration.ErrorHandler.HandleException(lblErrorDetail.Text, ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UninstallerFileList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (UninstallerFileList.SelectedIndex > -1)
				{
					string _uninstallPackageFile = UninstallerFileList.SelectedItem.Value;
					//Rainbow.Installer.Package.PackageInformation _info= new  Rainbow.Installer.Package.PackageInformation(_rbdFile);
					Rainbow.Installer.InstallManager _manager = new Rainbow.Installer.InstallManager(_uninstallPackageFile);
					Rainbow.Installer.Package.PackageInformation _info= _manager.Package.Information;
					this.SetPackeInfo(_info);
					deleteButton.Enabled = true;
					InstallerFileList.SelectedIndex = -1;
				}
				else
				{
					deleteButton.Enabled = false;
				}
			}
			catch(Exception ex)
			{
				lblErrorDetail.Text = Esperantus.Localize.GetString("MODULE_DEFINITIONS_INSTALLING", "An error occurred installing.", this) + "<br>";
				lblErrorDetail.Text += " Readind Package Information failed!";
				lblErrorDetail.Visible = true;

				Rainbow.Configuration.ErrorHandler.HandleException(lblErrorDetail.Text, ex);
			}
		}

		/// <summary>
		/// set the packageinformation on screen
		/// </summary>
		/// <param name="packageInfo"></param>
		private void SetPackeInfo(Rainbow.Installer.Package.PackageInformation packageInfo)
		{
			lblPackageName.Text = packageInfo.Caption;
			lblPackageVersion.Text = packageInfo.Version;
			lblIsBeta.Visible = packageInfo.IsBeta;
			lblPackageAuthor.Text = packageInfo.Author;
			lblPackageDescription.Text  = packageInfo.Description;
			linkPackageInformationUrl.Text	 = packageInfo.InformatioUrl;
			lblPackageType.Text	 = packageInfo.PackageType;
			tablePackageInfo.Visible =true;
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
			this.InstallerFileList.SelectedIndexChanged += new System.EventHandler(this.InstallerFileList_SelectedIndexChanged);
			this.UninstallerFileList.SelectedIndexChanged += new System.EventHandler(this.UninstallerFileList_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}