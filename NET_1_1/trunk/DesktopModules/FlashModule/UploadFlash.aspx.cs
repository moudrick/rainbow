/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Original Coder: Indah Fuldner [indah@die-seitenweber.de]
 *                  modified by Mario Hartmann [mario@hartmann.net // http://mario.hartmann.net/]
 * Version: C#
 * Product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 04/JUN/2004
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/
using System;
using System.Collections;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Button = Esperantus.WebControls.Button;
using HyperLink = Esperantus.WebControls.HyperLink;
using Label = Esperantus.WebControls.Label;
using LinkButton = Esperantus.WebControls.LinkButton;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// 
	/// </summary>
	[History("mario@hartmann.net", "2004/06/04", "Changed Flash movie control]")] 
	[History("mario@hartmann.net", "2004/05/25", "Bug fixed:[ 877885 ] Flash Module - Cannot DELETE")]
	public class UploadFlash :AddEditItemPage
	{
		// Configuration
		private bool	_uploadIsEnabled = true;
		private string	_imageFolder = string.Empty;
		private string _returnPath =string.Empty;

		// Messages
		private string	_noFileMessage = Localize.GetString("NO_FILE_MESSAGE");
		private string	_uploadSuccessMessage = Localize.GetString("UPLOAD_SUCCESS_MESSAGE");
		private string	_noImagesMessage = Localize.GetString("NO_IMAGE_MESSAGE");
		private string  _noFolderSpecifiedMessage = Localize.GetString("NO_FOLDER_SPECIFIED_MESSAGE");

		protected System.Web.UI.WebControls.Label gallerymessage;
		protected System.Web.UI.WebControls.Label uploadlabel;
		protected System.Web.UI.WebControls.Label uploadmessage;
		protected Panel uploadpanel;
		protected HtmlInputFile uploadfile;
		protected Table flashTable;
		protected Label Label5;
		protected Label Label1;
		protected Label Label2;
		protected Label Label3;
		protected Button closeButton;
		protected Button uploadButton;

   
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			// Form the script that is to be registered at client side.
			string scriptString = "<script language=JavaScript>";
			scriptString += "function UpdateOpener(filename) {";
			scriptString += "opener.document.Form1." + Request.QueryString["FieldID"] + ".value = filename;";
			scriptString += "self.close(); return false; }";
			scriptString += "function closeWindow() {";
			scriptString += "opener.focus(); self.close(); return false;}";	
			scriptString += "</script>";

			if(!this.IsClientScriptBlockRegistered("UpdateOpener"))
				this.RegisterClientScriptBlock("UpdateOpener", scriptString);

			uploadpanel.Visible = _uploadIsEnabled;
	    
			_imageFolder=((SettingItem) moduleSettings["FlashPath"]).FullPath;
			if (IOHelper.CreateDirectory(Server.MapPath(_imageFolder)))
				DisplayImages();
			else
				uploadmessage.Text=_noFolderSpecifiedMessage;
    
			Response.Write(_imageFolder);
		}

		/// <summary>
		/// Set the module guids with free access to this page
		/// </summary>
		protected override ArrayList AllowedModules
		{
			get
			{
				ArrayList al = new ArrayList();
				al.Add ("623EC4DD-BA40-421c-887D-D774ED8EBF02");
				return al;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void uploadButton_Command(object sender, CommandEventArgs e)
		{
			if (IOHelper.CreateDirectory(Server.MapPath(_imageFolder)))
			{
				if (uploadfile.PostedFile.FileName.Length != 0)
				{
					try
					{
						string virtualPath = _imageFolder + "/" + Path.GetFileName( uploadfile.PostedFile.FileName);
						string phyiscalPath = Server.MapPath(virtualPath);
						uploadfile.PostedFile.SaveAs(phyiscalPath);
						uploadmessage.Text = _uploadSuccessMessage;
					}
					catch(Exception exe)
					{
						uploadmessage.Text =( exe.Message); 
					}
				}
				else
				{
					uploadmessage.Text = ("_noFileMessage");
				}
			}
			else
			{
				uploadmessage.Text = _noFolderSpecifiedMessage;
			}
			DisplayImages();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Delete_Command(object sender, CommandEventArgs e)
		{
			if (_imageFolder.Length != 0)
			{
				try
				{
					File.Delete( Server.MapPath(_imageFolder) + @"\"+ e.CommandArgument);
				}
				catch{}
				DisplayImages();
			}
		}

		/// <summary>
		/// ReturnFolderContentArray
		/// </summary>
		/// <returns></returns>
		private string[] ReturnFolderContentArray()
		{
			if (_imageFolder.Length != 0)
			{
				try
				{
					string gallerytargetpath = Server.MapPath(_imageFolder);
					string[] galleryfolderarray = IOHelper.GetFiles(gallerytargetpath,"*.swf");
					return galleryfolderarray;
				}
				catch
				{
					return null;
				}
			}
			else
			{
				return null;
			}

		}
    
		/// <summary>
		/// DisplayImages
		/// </summary>
		public void DisplayImages()
		{
			_returnPath = "~~/" + ((SettingItem) moduleSettings["FlashPath"]).Value ;
			string[] galleryfolderarray = ReturnFolderContentArray();
			flashTable.Controls.Clear();
			if (galleryfolderarray == null || galleryfolderarray.Length == 0)
			{
				gallerymessage.Text = _noImagesMessage;
			}
			else
			{
				string galleryfilename = (string.Empty);
        
				TableRow rowItem;        
				TableCell cellItemImage;
				TableCell cellItemSelect;
				TableCell cellItemDelete;
				TableCell cellItemFileName;
				foreach (string galleryfolderarrayitem in galleryfolderarray)
				{ 
					galleryfilename = galleryfolderarrayitem.ToString();
					galleryfilename = galleryfilename.Substring(galleryfilename.LastIndexOf(@"\")+1);
      
					FlashMovie flashMovie = new FlashMovie();							
					flashMovie.MovieName= _imageFolder + "/" + galleryfilename ;
					flashMovie.MovieHeight="150px";
					flashMovie.MovieWidth="150px";

					System.Web.UI.WebControls.Label filenameLbl= new System.Web.UI.WebControls.Label();
					filenameLbl.Text=galleryfilename;
					HyperLink selectCmd= new HyperLink();
					selectCmd.TextKey="SELECT";
					selectCmd.Text="Select"; //by yiming
					selectCmd.CssClass="CommandButton";
					selectCmd.NavigateUrl="javascript:UpdateOpener('"+_returnPath + "/" + galleryfilename +"');self.close();";
					LinkButton deleteCmd= new LinkButton();
					deleteCmd.TextKey="DELETE";
					deleteCmd.Text="Delete";
					deleteCmd.CommandName="DELETE";
					deleteCmd.CssClass="CommandButton";
					deleteCmd.CommandArgument=galleryfilename;
					deleteCmd.Command +=new CommandEventHandler(Delete_Command);

					rowItem = new TableRow();       
         
					cellItemImage = new TableCell();
					cellItemSelect = new TableCell();
					cellItemDelete = new TableCell();
					cellItemFileName =new TableCell();
					cellItemImage.Controls.Add(flashMovie);
					cellItemFileName.Controls.Add(filenameLbl);
					cellItemSelect.Controls.Add(selectCmd);
					cellItemDelete.Controls.Add(deleteCmd);
          
           
					rowItem.Controls.Add(cellItemImage);
					rowItem.Controls.Add(cellItemFileName);
					rowItem.Controls.Add(cellItemSelect);
					rowItem.Controls.Add(cellItemDelete);
               
					flashTable.Controls.Add(rowItem);
					gallerymessage.Text = string.Empty;
				}
			}
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.uploadButton.Command += new CommandEventHandler(this.uploadButton_Command);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion 
	}
}
