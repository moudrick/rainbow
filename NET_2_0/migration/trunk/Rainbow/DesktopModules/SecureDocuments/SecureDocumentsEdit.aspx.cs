using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.UI;
using Path = Rainbow.Settings.Path;

namespace Rainbow.DesktopModules 
{
	/// <summary>
	/// Update and edit documents.
	/// </summary>
	[History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
	[History("jviladiu@portalServices.net", "2004/07/02", "Corrections for save documents in database")]
	public class SecureDocumentsEdit : AddEditItemPage
	{
		#region Declarations
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox NameField;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox CategoryField;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox PathField;
		/// <summary>
		/// 
		/// </summary>
		protected HtmlInputFile FileUpload;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label PageTitleLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label FileNameLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label CategoryLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label UrlLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label OrLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label UploadLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Literal CreatedLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Literal OnLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Label CreatedBy;
		/// <summary>
		/// 
		/// </summary>
		protected Label CreatedDate;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label Message;
		/// <summary>
		/// 
		/// </summary>
		protected CheckBoxList rolesList;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label Label1;
		/// <summary>
		/// 
		/// </summary>
		protected DataGrid DataGridFiles;
		/// <summary>
		/// 
		/// </summary>
		protected Esperantus.WebControls.Label Label2;
		/// <summary>
		/// 
		/// </summary>
		protected LinkButton LinkButtonBack;

		/// <summary>
		/// 
		/// </summary>
		string PathToSave;
		#endregion

		/// <summary>
		/// The Page_Load event on this Page is used to obtain the ModuleID
		/// and ItemID of the document to edit.
		/// It then uses the DocumentDB() data component
		/// to populate the page's edit controls with the document details.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e) 
		{
			// If the page is being requested the first time, determine if an
			// document itemID value is specified, and if so populate page
			// contents with the document details

			if (!Page.IsPostBack) 
			{
				if (ModuleID > 0)
					PathToSave = ((SettingItem) moduleSettings["DocumentPath"]).FullPath;

				if (ItemID > 0) 
				{
					// Obtain a single row of document information
					SecureDocumentDB documents = new SecureDocumentDB();
					SqlDataReader dr = documents.GetSingleDocument(ItemID, WorkFlowVersion.Staging);
                
					try
					{
						// Load first row into Datareader
						if(dr.Read())
						{
							NameField.Text = (string) dr["FileFriendlyName"];
							CategoryField.Text = (string) dr["Category"];
							CreatedBy.Text = (string) dr["CreatedByUser"];
							CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
							// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
							if (CreatedBy.Text == "unknown")
							{
								CreatedBy.Text = Localize.GetString ( "UNKNOWN", "unknown");
							}
						}
					}
					finally
					{
						dr.Close();
					}

					BindFiles();
				}

				populateRoles(ref rolesList, SecureDocumentDB.GetDocumentRoles(ItemID));
			}
		}

		/// <summary>
		/// Used to populate all checklist roles with Roles in portal
		///  -- taken from admin/modulesettings.cs
		///  -- modified to accept a string of RoleID instead of RoleName
		/// </summary>
		/// <param name="listRoles"></param>
		/// <param name="moduleRoles"></param>
		private void populateRoles(ref CheckBoxList listRoles, string moduleRoles)
		{
			//Get roles from db
			UsersDB users = new UsersDB();
			SqlDataReader roles = users.GetPortalRoles(portalSettings.PortalID);

			// Clear existing items in checkboxlist
			listRoles.Items.Clear();

			// add special roles
			SpecialRoles.populateSpecialRoles(ref listRoles);

			//Add roles from DB
			while(roles.Read()) 
			{
				ListItem item = new ListItem();
				item.Text = (string) roles["RoleName"];
				item.Value = roles["RoleID"].ToString();
				listRoles.Items.Add(item);
			}
			roles.Close(); //by Manu, fixed bug 807858

			//Splits up the role string and use array 30/01/2003 - by manudea
			while (moduleRoles.EndsWith(";"))
				moduleRoles = moduleRoles.Substring(0, moduleRoles.Length - 1);
			string[] arrModuleRoles = moduleRoles.Split(';');
			int roleCount = arrModuleRoles.GetUpperBound(0);
			
			//Cycle every role and select it if needed
			foreach(ListItem ls in listRoles.Items)
			{
				for(int i=0; i<=roleCount; i++)
				{
					if(arrModuleRoles[i].ToLower() == ls.Text.ToLower())
						ls.Selected = true;
				}			
			}
		}


		/// <summary>
		/// Bind the list of files
		/// </summary>
		private void BindFiles()
		{
			// get list of files for this doc
			DataGridFiles.DataSource = SecureDocumentDB.GetDocumentFiles(ItemID, WorkFlowVersion.Staging);
			DataGridFiles.DataBind();
		}

		/// <summary>
		/// Set the module guids with free access to this page
		/// </summary>
		protected override ArrayList AllowedModules
		{
			get
			{
				ArrayList al = new ArrayList();
				al.Add ("08E7FDAD-3033-49b8-9B10-BA7EC7AF5415");
				return al;
			}
		}

		/// <summary>
		/// The UpdateBtn_Click event handler on this Page is used to either
		/// create or update an document.  It  uses the DocumentDB()
		/// data component to encapsulate all data functionality.
		/// </summary>
		[History("jviladiu@portalServices.net", "2004/07/02", "Corrections for save documents in database")]
		override protected void OnUpdate(EventArgs e) 
		{
			base.OnUpdate(e);
			byte [] buffer = new byte[0];
			int size = 0;

			// Only Update if Input Data is Valid
			if (Page.IsValid) 
			{
				// Create an instance of the Document DB component
				SecureDocumentDB documents = new SecureDocumentDB();

				// Determine whether a file was uploaded
				FileInfo fInfo=null;
				if (FileUpload.PostedFile.FileName.Length != 0) 
				{
					fInfo = new FileInfo(FileUpload.PostedFile.FileName);
					if (bool.Parse(moduleSettings["DOCUMENTS_DBSAVE"].ToString())) 
					{
						Stream stream = FileUpload.PostedFile.InputStream;
						buffer  = new byte[FileUpload.PostedFile.ContentLength];
						size = FileUpload.PostedFile.ContentLength;
						try
						{
							stream.Read(buffer, 0, size);
							PathField.Text = fInfo.Name;
						}
						finally
						{
							stream.Close(); //by manu
						}
					} 
					else 
					{
						PathToSave = ((SettingItem) moduleSettings["DocumentPath"]).FullPath;
						// jviladiu@portalServices.net (02/07/2004). Create the Directory if not exists.
						if (!Directory.Exists(Server.MapPath(PathToSave)))
							Directory.CreateDirectory(Server.MapPath(PathToSave));
			
						string virtualPath = Path.WebPathCombine(PathToSave, fInfo.Name);
						string phyiscalPath = Server.MapPath(virtualPath);

						while(File.Exists(phyiscalPath))
						{
							// Calculate virtualPath of the newly uploaded file
							virtualPath = Path.WebPathCombine(PathToSave, Guid.NewGuid().ToString() + fInfo.Extension);

							// Calculate physical path of the newly uploaded file
							phyiscalPath = Server.MapPath(virtualPath);
						}

						try
						{
							// Save file to uploads directory
							FileUpload.PostedFile.SaveAs(phyiscalPath);

							// Update PathFile with uploaded virtual file location
							PathField.Text = virtualPath;
							
						}
						catch(Exception ex)
						{
							Message.Text = Localize.GetString ("ERROR_FILE_NAME", "Invalid file name!<br>") +  ex.Message;
							return;                            
						}
					}
				}
				// Change for save contenType and document buffer
				// documents.UpdateDocument(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, NameField.Text, PathField.Text, CategoryField.Text, new byte[0], 0, string.Empty );
				string contentType = PathField.Text.Substring(PathField.Text.LastIndexOf(".") + 1).ToLower();
				
				// add the record, get the Id of the record added
				int recordId = documents.UpdateDocument(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, NameField.Text, CategoryField.Text);

				// get the short filename to use
				string filename = string.Empty;
				if( fInfo != null)
					filename = fInfo.Name; // a file was uploaded, add it
				else 
					filename = PathField.Text.Trim(); // use the url
					
				documents.AddDocumentFile(ModuleID, recordId, filename, PathField.Text, buffer, size, contentType );
					

				// file permissions

				// delete existing records
				SecureDocumentDB.DeleteDocumentAccess(ItemID);

				// add access for each selected role
				for( int i = 0; i < rolesList.Items.Count; i++ )
				{
					if( rolesList.Items[i].Selected )
						SecureDocumentDB.AddDocumentAccess( recordId, 
							(int)(Enum.Parse(typeof(SpecialRoles.SpecialPortalRoles), rolesList.Items[i].Value)));

				}

				//update the file list
				this.RedirectBackToReferringPage();
			}
		}
    
		/// <summary>
		/// The DeleteBtn_Click event handler on this Page is used to delete an
		/// a document. It uses the Rainbow.DocumentsDB()
		/// data component to encapsulate all data functionality.
		/// </summary>
		override protected void OnDelete(EventArgs e) 
		{
			base.OnDelete(e);

			// Only attempt to delete the item if it is an existing item
			// (new items will have "ItemID" of 0)
			//TODO: Ask confim before delete
			if (ItemID != 0) 
			{
				SecureDocumentDB documents = new SecureDocumentDB();
				documents.DeleteDocument(ItemID, Server.MapPath(PathField.Text));
			}
			this.RedirectBackToReferringPage();
		}

		/// <summary>
		/// Process this command when a file is deleted from the list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DataGridFiles_OnDeleteCommand( object sender, DataGridCommandEventArgs e )
		{
			// get the id from the command
			int fileItemId = int.Parse(e.CommandArgument.ToString());

			// delete the file
			SecureDocumentDB.DeleteDocumentFile(fileItemId);

			// refresh the file list
			BindFiles();
		}

		/// <summary>
		/// Item data binding.  Used to add column names that items will correspond too.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DataGridFiles_OnItemDataBound( object sender, DataGridItemEventArgs e )
		{

			if( e.Item.Cells[0].Text == "&nbsp;" )
				return;

			// add the extra column for each file
			if( moduleSettings["DOCUMENTS_COLUMNNAMES"] != null )
			{
				// get the current row
				int rowIndex = e.Item.DataSetIndex;

				// get array of extra columns
				string [] extraColumnNames = moduleSettings["DOCUMENTS_COLUMNNAMES"].ToString().Split(";".ToCharArray());

				// if there is one for this index, display it
				if( rowIndex < extraColumnNames.Length )
					e.Item.Cells[3].Text = "&nbsp;&nbsp;&nbsp(" + extraColumnNames[rowIndex] + ")";
			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Raises OnInitEvent
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			InitializeComponent();

			//Translate
			CreatedLabel.Text = Localize.GetString("CREATED_BY");
			OnLabel.Text = Localize.GetString("ON");

			PageTitleLabel.Text = Localize.GetString("DOCUMENT_DETAILS");
			FileNameLabel.Text = Localize.GetString("FILE_NAME");
			CategoryLabel.Text = Localize.GetString("CATEGORY");
			UrlLabel.Text = Localize.GetString("URL");
			UploadLabel.Text = Localize.GetString("UPLOAD_FILE");
			OrLabel.Text = "---" + Localize.GetString("OR") + "---";

			RequiredFieldValidator1.Text = Localize.GetString("VALID_FILE_NAME");
		
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
