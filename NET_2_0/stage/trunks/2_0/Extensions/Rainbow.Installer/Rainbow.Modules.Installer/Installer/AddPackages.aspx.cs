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
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
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
	public class AddPackages : Rainbow.UI.AddEditItemPage // Rainbow.UI.EditItemPage
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
		protected Esperantus.WebControls.RequiredFieldValidator Requiredfieldvalidator2;
		protected Esperantus.WebControls.Label Label9;

		protected Rainbow.UI.WebControls.UploadDialogTextBox uploadDialog;
		protected Esperantus.WebControls.Label Label8;
		protected Esperantus.WebControls.Label Label10;
		protected System.Web.UI.WebControls.TextBox txtPackageURL;
		protected Esperantus.WebControls.Button btnGetPackage;
		protected System.Web.UI.WebControls.Label lblModulesInPackage;
		protected Esperantus.WebControls.Label Label11;
		protected Esperantus.WebControls.LinkButton cancelButton;

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
			//hide the error label
			lblErrorDetail.Visible = false; 

			// Calculate security defID
			if (Request.Params["defID"] != null) 
				defID = new Guid(Request.Params["defID"]);

			// If this is the first visit to the page
			if (!Page.IsPostBack)
			{
				//if (defID.ToString() == "00000000-0000-0000-0000-000000000000") 

				// hide the Info table;
				tablePackageInfo.Visible =false;
				// new module packages
				SetInstallList();
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
				al.Add ("{3F9918E2-BF1C-4538-8DD2-8FA30D648A2D}"); // PackageManager.ascx
				return al;
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
					updateButton.Enabled = true;
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

			lblModulesInPackage.Text = string.Empty;
			foreach (string _entry in package.ModuleList)
				lblModulesInPackage.Text += _entry +"<br>";
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
			this.uploadDialog.TextChanged += new System.EventHandler(this.uploadDialog_TextChanged);
			this.btnGetPackage.Click += new System.EventHandler(this.btnGetPackage_Click);
			this.InstallerFileList.SelectedIndexChanged += new System.EventHandler(this.InstallerFileList_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnGetPackage_Click(object sender, System.EventArgs e)
		{
			try
			{
				string  _downloadUrl = txtPackageURL.Text ;
				Uri _url;
				_url = new Uri(_downloadUrl);
				string _destFileName = PhysicalFolders.Install +_downloadUrl.Substring(_downloadUrl.LastIndexOf("/")+1);
				this.GetFileFromWeb (_downloadUrl,_destFileName,true);
			}
			catch (Exception ex)
			{
				this.lblErrorDetail.Text = ex.Message;
				this.lblErrorDetail.Visible = true;
			}
			this.SetInstallList();
		}
		
		private  void GetFileFromWeb (string sourceWebUrl, string destinationFilePath, bool overwrite)
		{

//			System.Net.WebRequest _req;
//			System.Net.WebResponse _resp;
			System.IO.Stream _stream ;

			FileInfo _destinationFileInfo = new FileInfo(destinationFilePath);
			FileStream _fs ;

			if (File.Exists(destinationFilePath))
			{
				if(overwrite)
					File.Delete(destinationFilePath);
				else
					throw new Exception ("File already exist. ["+ _destinationFileInfo.Name +"]");
			}

			try
			{
//				_req	= System.Net.HttpWebRequest.Create(sourceWebUrl);
//				_req.Timeout = 50000;
//
//				// Get the response
//				_resp = _req.GetResponse();
//				_stream = _resp.GetResponseStream();

				_stream = new Helpers.HttpHelper().GetHttpStream(sourceWebUrl,15);
				_fs = new FileStream(destinationFilePath,FileMode.Create);
				int _b ;
				while ((_b = _stream.ReadByte()) != -1 )
				{
					_fs.WriteByte(Convert.ToByte(_b));
				}
				
				//byte[] _buffer = null ;
				//_stream.Read(_buffer,0,(int)_resp.ContentLength);
				
				//_fs.Write(_buffer,0,_buffer.Length);
				_fs.Close();
				_stream.Close();
//				_resp.Close();
			}
			catch (System.Net.WebException wex)
			{
//				_req = null;
				_fs=null;
				_stream = null;
//				_resp = null;
				throw;
			}
			catch (Exception ex)
			{
//				_req = null;
				_fs=null;
				_stream = null;
//				_resp = null;
				throw;
			}
		}



		private void uploadDialog_TextChanged(object sender, System.EventArgs e)
		{
		
		}

	}
}