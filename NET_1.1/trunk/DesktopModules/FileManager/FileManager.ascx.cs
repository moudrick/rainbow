using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;
using Path = Rainbow.Settings.Path;

namespace Rainbow.DesktopModules
{
	/// <summary>
	///		:::::::::::::::::::
	///		::  FileManager  ::
	///		:::::::::::::::::::
	///		
	///		Implemented by: Rob Siera, www.xtrasite.be 
	///		based on free code at http://www.seekdotnet.com/freeware.aspx
	///		Module to manage files of portal.
	/// </summary>
	/// 
	///		::::::::::::::::::::::
	///		::  Module History  ::
	///		::::::::::::::::::::::
	///		
	///		14 Nov 2004	First Release - Rob Siera

	public abstract class FileManager : PortalModuleControl 
	{
		#region Declarations
		/// <summary>
		/// 
		/// </summary>
		protected Label lblData;
		/// <summary>
		/// 
		/// </summary>
		protected Label lblCounter;
		/// <summary>
		/// 
		/// </summary>
		protected ImageButton btnGoUp;
		/// <summary>
		/// 
		/// </summary>
		protected DataGrid dgFile;
		/// <summary>
		/// 
		/// </summary>
		protected ImageButton btnDelete;
		/// <summary>
		/// 
		/// </summary>
		protected Label lblError;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox txtNewDirectory;
		/// <summary>
		/// 
		/// </summary>
		protected Button btnUpload;
		/// <summary>
		/// 
		/// </summary>
		protected HtmlInputFile file1;
		/// <summary>
		/// 
		/// </summary>
		protected HtmlInputFile file2;
		/// <summary>
		/// 
		/// </summary>
		protected HtmlInputFile file3;
		/// <summary>
		/// 
		/// </summary>
		protected ImageButton btnNewFolder;
		/// <summary>
		/// 
		/// </summary>
		protected PlaceHolder myPlaceHolder;
		/// <summary>
		/// 
		/// </summary>
		protected ArrayList arrExtentions ;
		/// <summary>
		/// 
		/// </summary>
		protected string imgroot;
		/// <summary>
		/// 
		/// </summary>
		protected string root;
		/// <summary>
		/// 
		/// </summary>
		protected string FullDir;
		#endregion

		private string baseImageDIR = string.Empty;
		private Hashtable availExtensions = new Hashtable();

		private void LoadAvailableImageList()
		{
			string bDir = Server.MapPath(this.baseImageDIR);
			DirectoryInfo di = new DirectoryInfo(bDir);
			FileInfo[] rgFiles = di.GetFiles("*.gif");
			string ext = string.Empty;
			string nme = string.Empty;
			string f_Name = string.Empty;

			foreach(FileInfo fi in rgFiles)
			{
				f_Name = fi.Name;
				ext = fi.Extension.ToLower();
				nme = f_Name.Substring(0, (f_Name.Length - ext.Length));
				availExtensions.Add(nme, f_Name);
			}

		}

		private string imageAsign (string contentType) 
		{
			if(availExtensions.ContainsKey(contentType.ToLower()))
			{
				return availExtensions[contentType].ToString();
			}
			else
			{
				return "unknown.gif";
			}
		}

		private void Page_Load(object sender, EventArgs e)
		{ 
			LoadAvailableImageList();

			lblError.Text= "";
			FullDir = InitDir();
			InitImages();

			if(!IsPostBack) 
			{
				btnDelete.Attributes.Add("OnClick","return btnDelete_Click()");
				BindData();
			}
		} 

		private void InitImages()
		{
			imgroot = Path.WebPathCombine(this.CurrentTheme.WebPath,"/img/");
			btnDelete.ImageUrl = this.CurrentTheme.GetModuleImageSRC("delete.png");
			btnGoUp.ImageUrl = this.CurrentTheme.GetModuleImageSRC("FolderUp.gif");
			btnNewFolder.ImageUrl = this.CurrentTheme.GetModuleImageSRC("newfolder.gif");
		}

		private string InitDir()
		{
			//Current Portal root or deeper is allowed
			string tmpDir = Path.WebPathCombine(Path.ApplicationPhysicalPath,Settings["FM_DIRECTORY"].ToString());
			tmpDir=tmpDir.Replace("\\/","\\");
			tmpDir=tmpDir.Replace("/","\\");
			if(!tmpDir.StartsWith(DefaultDir()))
				tmpDir=DefaultDir();
			return tmpDir;
		}

		private string DefaultDir()
		{
			string tmpDir = Path.WebPathCombine(Path.ApplicationPhysicalPath,portalSettings.PortalPath);
			tmpDir=tmpDir.Replace("\\/","\\");
			tmpDir=tmpDir.Replace("/","\\");
			return tmpDir;
		}
		/// <summary>
		/// Constructor - Load Module Settings
		/// </summary>
		public FileManager() 
		{
			// Modified by Hongwei Shen
			SettingItemGroup group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			int groupBase = (int)group;

			SettingItem directory = new SettingItem(new StringDataType());
			directory.EnglishName = "Directory Path";
			directory.Required = false;
			directory.Group = group;
			directory.Order = groupBase + 20;//1;
			if (portalSettings!=null)
				directory.Value = portalSettings.PortalPath;
			else
				directory.Value = string.Empty;
			this._baseSettings.Add("FM_DIRECTORY", directory);

			SettingItem DownloadableExt = new SettingItem(new StringDataType());
			DownloadableExt.EnglishName = "Downloadable extentions";
			DownloadableExt.Group = group;
			DownloadableExt.Order = groupBase + 25; //2;
			DownloadableExt.Value = "";
			DownloadableExt.Description = "Provide a comma-delimited list of file extentions that you can download On Click";
			this._baseSettings.Add("FM_DOWNLOADABLEEXT", DownloadableExt);
		}

		private void BindData() 
		{
			dgFile.DataSource = GetFiles();
			dgFile.DataKeyField="type";
			dgFile.DataBind();
			lblCounter.Text = dgFile.Items.Count.ToString() + " object(s)";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected DataTable GetFiles() 
		{
			DirectoryInfo dirInfo;
			FileInfo[] info;
			try
			{
				dirInfo=new DirectoryInfo(GetCurDir());
				info=dirInfo.GetFiles();
			}
			catch
			{
				lblError.Text = "Directory not found or not allowed. Using default directory (Portal root dir)."; //ex.Message;
				FullDir = DefaultDir();
				SetCurDir(FullDir);
				dirInfo=new DirectoryInfo(GetCurDir());
				info=dirInfo.GetFiles();
			}

			
			DirectoryInfo[] dirs=dirInfo.GetDirectories();
			DataTable dt=CreateDataSource();
			DataRow dr;
			foreach(DirectoryInfo dir in dirs)
			{
				if(dir.Name.ToLower()!="_svn" && dir.Name.ToLower()!="_cvn")
				{
					dr=dt.NewRow();
					dr["filename"]=dir.Name;
					dr["size"]="0";
					dr["type"]="0";
					dt.Rows.Add(dr);
				}
			}
			foreach(FileInfo file in info)
			{
						dr=dt.NewRow();
						dr["filename"]=file.Name;
						dr["size"]=(int)file.Length/1024;
						dr["type"]="1";
						dr["modified"]=DateTime.Parse(file.LastWriteTime.ToString()).ToString("dd/MM/yyyy hh:mm tt");
						dt.Rows.Add(dr);
			}
			dt.AcceptChanges();
			return dt;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected DataTable CreateDataSource()
		{
			DataTable dt=new DataTable();
			dt.Columns.Add("filename", typeof(string));
			dt.Columns.Add("size", typeof(int));
			dt.Columns.Add("type", typeof(int));
			dt.Columns.Add("modified", typeof(string));
			return dt;
		}

		private String GetCurDir() 
		{
			object obj=ViewState["curDir"];
			if(obj==null) 
			{
				return FullDir;
			}
			else 
			{
				return (string)obj;
			}
			
		}
		private void SetCurDir(string dir)
		{
			ViewState["curDir"]=dir;
			ViewState["curUserDir"]=dir;
		}
		
		private void dgFile_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if(e.Item.ItemType==ListItemType.Item || e.Item.ItemType==ListItemType.AlternatingItem || e.Item.ItemType==ListItemType.EditItem)
			{
				Image imgType=(Image)e.Item.FindControl("imgType");
				//PlaceHolder plhImgEdit = (PlaceHolder)e.Item.FindControl("plhImgEdit");
				LinkButton lnkName=(LinkButton)e.Item.FindControl("lnkName");
				//HyperLink imgACL = (HyperLink)e.Item.FindControl("imgACL");
				
				
				
				//HyperLink for Edit Text
				HyperLink hlImgEdit = new HyperLink();
				hlImgEdit.ImageUrl = this.CurrentTheme.GetModuleImageSRC("btnEdit.gif");
				hlImgEdit.NavigateUrl = Path.ApplicationFullPath+"Desktopmodules/Filemanager/EditFile.aspx?ID=" + GetCurDir() + "\\" + DataBinder.Eval(e.Item.DataItem,"filename");
				//----
				
				int type=int.Parse(DataBinder.Eval(e.Item.DataItem, "type", "{0}"));
				if(type==0)
				{  
					imgType.ImageUrl=this.CurrentTheme.GetModuleImageSRC("dir.gif");
					e.Item.Cells[2].Text="";
					e.Item.Cells[3].Text="";
				}
				else
				{
					string name=DataBinder.Eval(e.Item.DataItem, "filename", "{0}").Trim().ToLower();
					lnkName.Enabled=IsDownloadable(name);
					string ext = name.Substring(name.LastIndexOf(".")+1);
					imgType.ImageUrl= Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/Ext/" + imageAsign(ext));
				}	 
			}
		}

		private bool IsDownloadable(string filename)
		{
			Boolean isInList = false;
			if(Settings["FM_DOWNLOADABLEEXT"].ToString()!="") 
			{
				String[] strExtensions = Settings["FM_DOWNLOADABLEEXT"].ToString().Split(';');

				for (int i = 0; i < strExtensions.Length; i++) 
				{
					String strEx = (String)strExtensions[i];
					if(!strEx.StartsWith(".")) 
					{
						strEx = "." + strEx;
					}
					if(filename.Trim().ToLower().EndsWith(strEx))
					{
						isInList = true;
					}
				}
			}
			return isInList;
		}

		private void dgFile_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if(e.CommandName=="ItemClicked")
			{
				dgFile.EditItemIndex=-1;
				LinkButton lnkName=(LinkButton)e.Item.FindControl("lnkName");
				int type=int.Parse(dgFile.DataKeys[e.Item.ItemIndex].ToString());
				if(type==0)
				{
					SetCurDir(System.IO.Path.Combine(GetCurDir(), lnkName.Text));
				}
				else if(Settings["FM_DOWNLOADABLEEXT"].ToString()!="")
				{
					string filename=System.IO.Path.Combine(GetCurDir(), lnkName.Text);
					Response.AddHeader("Content-Disposition", "attachment; filename="+lnkName.Text.Trim());
					Response.WriteFile(filename);
					Response.End();
				}
				
				if(type==0)
				{
					try
					{
						BindData();
					}
					catch(Exception ex)
					{
						lblError.Text = ex.Message;
					}
				}
			}
		}

		private void dgFile_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TextBox txtEditName=(TextBox)e.Item.FindControl("txtEditName");
				if(txtEditName.Text.Trim().Length<1)
					return;

				int type=int.Parse(dgFile.DataKeys[e.Item.ItemIndex].ToString());
				if(type==0)
				{
					Directory.Move(System.IO.Path.Combine(GetCurDir(), System.IO.Path.GetFileName(ViewState["lastSelection"].ToString())),System.IO.Path.Combine(GetCurDir(), System.IO.Path.GetFileName(txtEditName.Text)));
				}
				else
				{
					File.Move(System.IO.Path.Combine(GetCurDir(), System.IO.Path.GetFileName(ViewState["lastSelection"].ToString())),System.IO.Path.Combine(GetCurDir(), System.IO.Path.GetFileName(txtEditName.Text)));
				}
				dgFile.EditItemIndex=-1;
				BindData();
			}
			catch(Exception ex)
			{
				lblError.Text = ex.Message;
			}
		}

		private void dgFile_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgFile.EditItemIndex=e.Item.ItemIndex;
			LinkButton lnkName=(LinkButton)e.Item.FindControl("lnkName");
			ViewState["lastSelection"]=lnkName.Text;
			BindData();
		}

		private void dgFile_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgFile.EditItemIndex = -1;
			BindData();
		}

		private void btnGoUp_Click(object sender, ImageClickEventArgs e)
		{
			MoveUp();
			BindData();
		}
		private void MoveUp()
		{
			if(!CanMoveUp())
			{
				lblError.Text = "Root directory reached";
				return;
			}
			string dir=System.IO.Path.GetDirectoryName(GetCurDir());
			SetCurDir(dir);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool CanMoveUp()
		{
			string dir=GetCurDir().ToLower().Trim();
		
			if(dir.Length>FullDir.Length)
				return true;
			return false;
		}
		private void DeleteItem(DataGridItem e)
		{
			LinkButton lnkName=(LinkButton)e.FindControl("lnkName");
			int type=int.Parse(dgFile.DataKeys[e.ItemIndex].ToString());
			if(type==0)
			{
				Directory.Delete(System.IO.Path.Combine(GetCurDir(), lnkName.Text), true);
			}
			else
			{
				File.Delete(System.IO.Path.Combine(GetCurDir(), lnkName.Text));
			}					
		}

		private void btnDelete_Click(object sender, ImageClickEventArgs e)
		{
			try
			{
				bool yes=false;
				foreach(DataGridItem dgi in dgFile.Items)
				{
					CheckBox chkChecked=(CheckBox)dgi.FindControl("chkChecked");
					if(chkChecked.Checked)
					{
						yes=true;
						DeleteItem(dgi);
					}
				}
				if(yes)
				{
					dgFile.CurrentPageIndex=0;
					dgFile.EditItemIndex=-1;
					BindData();
				}
			}
			catch(Exception ex)
			{
				lblError.Text=ex.Message;
			}
		}

		private void Imagebutton1_Click(object sender, ImageClickEventArgs e)
		{
			try
			{
				Directory.CreateDirectory(System.IO.Path.Combine(GetCurDir(), System.IO.Path.GetFileName(txtNewDirectory.Text)));
				txtNewDirectory.Text= string.Empty;
				BindData();
			}
			catch(Exception ex)
			{
				lblError.Text=ex.Message;
			}
		}

		private void dgFile_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if(ViewState["SortBy"] == null)
			{
				ViewState["SortBy"] = "ASC";
			}
			else if(ViewState["SortBy"].ToString().Equals("ASC")) 
			{
				ViewState["SortBy"] = "DESC";
			}
			else 
			{
				ViewState["SortBy"] = "ASC";
			}
			
			dgFile.Columns[1].HeaderText = "FileName";
			dgFile.Columns[2].HeaderText = "Size(KB)";
			dgFile.Columns[3].HeaderText = "Modified";
			for(int i=0;i<dgFile.Columns.Count;i++)
			{
				if(dgFile.Columns[i].SortExpression.Trim().Equals(e.SortExpression.Trim())) 
				{
					
					if(ViewState["SortBy"].ToString().Trim().Equals("ASC")) 
					{
						dgFile.Columns[i].HeaderText = dgFile.Columns[i].HeaderText + "<span style='font-family:webdings;'>5</span>";
					}
					else 
					{
						dgFile.Columns[i].HeaderText = dgFile.Columns[i].HeaderText + "<span style='font-family:webdings;'>6</span>";
					}

				}
			}
			
			DataView dv = new DataView(GetFiles());
			dv.Sort = e.SortExpression + " " + ViewState["SortBy"];
			dgFile.DataSource = dv;
			dgFile.DataBind();
			
		}

		private void btnUpload_Click(object sender, EventArgs e)
		{
			try
			{
				for(int i=0;i<Request.Files.Count;i++) 
				{
					if(Request.Files[i].FileName.Trim().Length >0) 
					{
						HttpPostedFile file=Request.Files[i];
						if(file!=null && file.FileName.Length>0)
							file.SaveAs(System.IO.Path.Combine(GetCurDir(), System.IO.Path.GetFileName(file.FileName)));
					}
				}
				BindData();
			}
			catch(Exception ex)
			{
				lblError.Text=ex.Message;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public override bool AdminModule
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Guid GuidID
		{
			get
			{
				return new Guid("{DE97F04D-FB0A-445d-829A-61E4FA69ADB2}");
			}
		}


		#region Web Form Designer generated code
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			this.baseImageDIR = Path.ApplicationRoot + "/aspnet_client/Ext/";
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnGoUp.Click += new ImageClickEventHandler(this.btnGoUp_Click);
			this.dgFile.ItemCommand += new DataGridCommandEventHandler(this.dgFile_ItemCommand);
			this.dgFile.CancelCommand += new DataGridCommandEventHandler(this.dgFile_CancelCommand);
			this.dgFile.EditCommand += new DataGridCommandEventHandler(this.dgFile_EditCommand);
			this.dgFile.SortCommand += new DataGridSortCommandEventHandler(this.dgFile_SortCommand);
			this.dgFile.UpdateCommand += new DataGridCommandEventHandler(this.dgFile_UpdateCommand);
			this.dgFile.ItemDataBound += new DataGridItemEventHandler(this.dgFile_ItemDataBound);
			this.btnDelete.Click += new ImageClickEventHandler(this.btnDelete_Click);
			this.btnNewFolder.Click += new ImageClickEventHandler(this.Imagebutton1_Click);
			this.btnUpload.Click += new EventHandler(this.btnUpload_Click);
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion

	}
}
