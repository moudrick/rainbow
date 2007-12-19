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
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Security;
using Rainbow.Configuration;
using Rainbow.Installer;

using Esperantus;

namespace Rainbow.DesktopModules.Installer
{
    /// <summary>
    /// Control to show/edit portals modules (AdminAll)
    /// </summary>
    public class PackageManager : PortalModuleControl 
    {
		protected Esperantus.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label lblPackageName;
		protected Esperantus.WebControls.Label Label6;
		protected System.Web.UI.WebControls.Label lblPackageVersion;
		protected System.Web.UI.WebControls.Label lblIsBeta;
		protected Esperantus.WebControls.Label Label3;
		protected System.Web.UI.WebControls.Label lblPackageDescription;
		protected Esperantus.WebControls.Label Label9;
		protected System.Web.UI.WebControls.Label lblPackageAuthor;
		protected Esperantus.WebControls.Label Label4;
		protected System.Web.UI.WebControls.HyperLink linkPackageInformationUrl;
		protected Esperantus.WebControls.Label Label5;
		protected System.Web.UI.WebControls.Label lblPackageType;
		protected System.Web.UI.WebControls.Label lblErrorDetail;
		protected System.Web.UI.HtmlControls.HtmlTable tablePackageInfo;
        protected System.Web.UI.WebControls.DataList defsList;

		protected Esperantus.WebControls.Label Label1;
		protected Esperantus.WebControls.Label Label7;
		protected System.Web.UI.WebControls.ListBox InstallerFileList;
		protected Esperantus.WebControls.Label Label8;
		protected System.Web.UI.WebControls.ListBox UninstallerFileList;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;

		protected Esperantus.WebControls.LinkButton InstallBtn;
		protected Esperantus.WebControls.Label Label10;
		protected System.Web.UI.WebControls.Label lblModulesInPackage;
		protected Esperantus.WebControls.LinkButton UninstallBtn;

		/// <summary>
		/// The Page_Load server event handler on this user control is used
		/// to populate the current defs settings from the configuration system
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e) 
        {
			//hide the error label
			lblErrorDetail.Visible = false; 

			if (!Page.IsPostBack) 
			{
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
		/// installs or refresh module definiton on db
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InstallBtn_Click(object sender, System.EventArgs e)
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
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UninstallBtn_Click(object sender, System.EventArgs e)
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
					this.SetPackageInfo(_manager.Package);
					InstallBtn.Enabled = true;

					UninstallBtn.Enabled = false;
					UninstallerFileList.SelectedIndex = -1;
				}
				else
				{
					UninstallBtn.Enabled= false;

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
					this.SetPackageInfo(_manager.Package);
					UninstallBtn.Enabled = true;
					InstallBtn.Enabled= false;
					InstallerFileList.SelectedIndex = -1;
				}
				else
				{
					UninstallBtn.Enabled = false;
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
		private void SetPackageInfo(Rainbow.Installer.Package.InstallPackage package)
		{
			lblPackageName.Text = package.Information.Caption;
			lblPackageVersion.Text = package.Information.Version;
			lblIsBeta.Visible = package.Information.IsBeta;
			lblPackageAuthor.Text = package.Information.Author;
			lblPackageDescription.Text  = package.Information.Description;
			linkPackageInformationUrl.Text	 = package.Information.InformatioUrl;
			lblPackageType.Text	 = package.Information.PackageType;
			tablePackageInfo.Visible =true;

			lblModulesInPackage.Text =string.Empty;
			foreach (string _entry in package.ModuleList)
				lblModulesInPackage.Text += _entry +"<br>";
		}
	



		#region General Implementation

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


		public override Guid GuidID 
		{
			get
			{
				return new Guid("{3F9918E2-BF1C-4538-8DD2-8FA30D648A2D}");
			}
		}

		public PackageManager ()
		{
		}
		#endregion

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
			InitializeComponent();
			this.AddUrl = "~/DesktopModules/Installer/AddPackages.aspx";
			this.AddText =Esperantus.Localize.GetString("PACKAGES_ADD","Add Packages");
			base.OnInit(e);
		}

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() 
        {
			this.InstallerFileList.SelectedIndexChanged += new System.EventHandler(this.InstallerFileList_SelectedIndexChanged);
			this.UninstallerFileList.SelectedIndexChanged += new System.EventHandler(this.UninstallerFileList_SelectedIndexChanged);
			this.InstallBtn.Click += new System.EventHandler(this.InstallBtn_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
    }
}