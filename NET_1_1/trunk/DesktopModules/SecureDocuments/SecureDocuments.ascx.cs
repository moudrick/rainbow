using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;
using Path = Rainbow.Settings.Path;

namespace Rainbow.DesktopModules 
{
	/// <summary>
	/// Secure Documents is an enhanced version of the regular documents module.
	/// IT supports moultiple files in one item. As well as per item permissions.
	/// </summary>
	public class SecureDocuments : PortalModuleControl 
	{
		/// <summary>
		/// 
		/// </summary>
		protected DataGrid myDataGrid;
		/// <summary>
		/// 
		/// </summary>
		protected DataView myDataView;
		/// <summary>
		/// 
		/// </summary>
		protected string sortField;
		/// <summary>
		/// 
		/// </summary>
		protected string sortDirection;
		/// <summary>
		/// 
		/// </summary>
		protected int normalColCount;

		// Jminond - addded to upgraded extension set
		private string baseImageDIR = string.Empty;
		private Hashtable availExtensions = new Hashtable();

		/// <summary>
		/// Constructor
		/// </summary>
		public SecureDocuments() 
		{
			// 17/12/2004 added localization for new settings by José Viladiu (jviladiu@portalServices.net)

			#region Document Setting Items

			SettingItem DocumentPath = new SettingItem(new PortalUrlDataType());
			DocumentPath.Required = true;
			DocumentPath.Value = "SecureDocuments";
			DocumentPath.Order = 1;
			DocumentPath.EnglishName = "Document path";
			DocumentPath.Description = "Folder for store the documents";
			this._baseSettings.Add("DocumentPath", DocumentPath);

			// Add new functionalities by jviladiu@portalServices.net (02/07/2004)
			SettingItem ShowImages = new SettingItem(new BooleanDataType());
			ShowImages.Value = "true";
			ShowImages.Order = 5;
			ShowImages.EnglishName = "Show Image Icons?";
			ShowImages.Description = "Mark this if you like see Image Icons";
			this._baseSettings.Add("DOCUMENTS_SHOWIMAGES", ShowImages);

			SettingItem SaveInDataBase = new SettingItem(new BooleanDataType());
			SaveInDataBase.Value = "false";
			SaveInDataBase.Order = 10;
			SaveInDataBase.EnglishName = "Save files in DataBase?";
			SaveInDataBase.Description = "Mark this if you like save files in DataBase";
			this._baseSettings.Add("DOCUMENTS_DBSAVE", SaveInDataBase);

			// Added sort by fields by Chris Thames [icecold_2@hotmail.com] (11/17/2004)
			// globalized by brian kierstead 4/25/2005
			string colText = Localize.GetString("TITLE", "Title") + ";"
				+ Localize.GetString("DOCUMENT_OWNER", "Owner") + ";"
				+ Localize.GetString("DOCUMENT_AREA", "Area") + ";"
				+ Localize.GetString("DOCUMENT_LAST_UPDATED", "Last Updated");

			SettingItem	SortByField	= new SettingItem(new ListDataType(colText));
			SortByField.Required=true;
			SortByField.Value =	Localize.GetString("TITLE", "Title");
			SortByField.Order = 11;
			SortByField.EnglishName = "Default Sort Field";
			SortByField.Description = "Sort by this field when the page first loads";
			this._baseSettings.Add("DOCUMENTS_SORTBY_FIELD", SortByField);


			SettingItem SortByDirection = new SettingItem(new ListDataType(Localize.GetString("DOCUMENTS_SORTBY_DIRECTION_LIST", "Ascending;Descending")));
			SortByDirection.Value = "Ascending";
			SortByDirection.Order = 12;
			SortByDirection.EnglishName = "Sort ascending or descending?";
			SortByDirection.Description = "Ascending: A to Z or 0 - 9. Descending: Z - A or 9 - 0.";
			this._baseSettings.Add("DOCUMENTS_SORTBY_DIRECTION", SortByDirection);
			// End

			// Added by Jakob Hansen 07/07/2004
			SettingItem showTitle = new SettingItem(new BooleanDataType());
			showTitle.Value = "true";
			showTitle.Order = 15;
			showTitle.EnglishName = "Show Title column?";
			showTitle.Description = "Mark this if the title column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWTITLE", showTitle);

			SettingItem showOwner = new SettingItem(new BooleanDataType());
			showOwner.Value = "true";
			showOwner.Order = 16;
			showOwner.EnglishName = "Show Owner column?";
			showOwner.Description = "Mark this if the owner column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWOWNER", showOwner);

			SettingItem showArea = new SettingItem(new BooleanDataType());
			showArea.Value = "true";
			showArea.Order = 17;
			showArea.EnglishName = "Show Area column";
			showArea.Description = "Mark this if the area column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWAREA", showArea);

			SettingItem showLastUpdated = new SettingItem(new BooleanDataType());
			showLastUpdated.Value = "true";
			showLastUpdated.Order = 18;
			showLastUpdated.EnglishName = "Show Last Updated column";
			showLastUpdated.Description = "Mark this if the Last Updated column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWLASTUPDATED", showLastUpdated);
			// End Change Jakob Hansen

			// added by brian kierstead 4/12/2005
			// add settings to allow multiple docs per row
			SettingItem settingColumnName = new SettingItem(new StringDataType());
			settingColumnName.Value = "File(s)";
			settingColumnName.Order = 19;
			settingColumnName.EnglishName = "File Column Titles";
			settingColumnName.Description = "Names to be used for each file column.";
			this._baseSettings.Add("DOCUMENTS_COLUMNNAMES", settingColumnName);
			// end by brian kierstead 4/12/2005

			#endregion

			// Change by Geert.Audenaert@Syntegra.Com
			// Date: 27/2/2003
			SupportsWorkflow = true;
			// End Change Geert.Audenaert@Syntegra.Com
		}

		/// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// obtain a SqlDataReader of document information from the 
        /// Documents table, and then databind the results to a DataGrid
        /// server control.  It uses the Rainbow.DocumentDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e) 
		{
			LoadAvailableImageList();

			if (!Page.IsPostBack) 
			{
				string sortFieldOption		= Settings["DOCUMENTS_SORTBY_FIELD"].ToString();
				string sortDirectionOption	= Settings["DOCUMENTS_SORTBY_DIRECTION"].ToString();
				if (sortFieldOption.Length > 0)
				{
					// get the default sort field
					if( sortFieldOption == Localize.GetString("DOCUMENT_AREA", "Area"))
						sortField = "Category";
					else if( sortFieldOption ==  Localize.GetString("DOCUMENT_OWNER", "Owner") )
						sortField = "CreatedByUser";
					else if( sortFieldOption == Localize.GetString("DOCUMENT_LAST_UPDATED", "Last Updated"))
						sortField = "CreatedDate";
					else	// TITLE
						sortField = "FileFriendlyName";

					// get sort direction
					if (Localize.GetString("DOCUMENTS_SORTBY_DIRECTION_LIST", "Ascending;Descending").IndexOf(sortDirectionOption) > 0)
						sortDirection = "DESC";
					else
						sortDirection = "ASC";
				}
				else
				{
					sortField = "FileFriendlyName";
					sortDirection = "ASC";
					if (sortField == "DueDate")
						sortDirection = "DESC";
				}
				ViewState["SortField"] = sortField;
				ViewState["SortDirection"] = sortDirection;

				// store the number of columns that are part of the
				// record set so we know which column to start with
				// to add in the files data.
				normalColCount = myDataGrid.Columns.Count;
				ViewState["NormalColCount"] = normalColCount;
			}
			else
			{
				sortField = (string) ViewState["SortField"];
				sortDirection = (string) ViewState["sortDirection"];
				normalColCount = int.Parse(ViewState["NormalColCount"].ToString());
			}

			myDataView = new DataView();

			// Obtain Document Data from Documents table
			// and bind to the datalist control
			SecureDocumentDB documents = new SecureDocumentDB();

			// DataSet documentsData = documents.GetDocuments(ModuleID, Version);
			// myDataView = documentsData.Tables[0].DefaultView;
			setDataView (documents.GetDocuments(ModuleID, Version,int.Parse(PortalSettings.CurrentUser.Identity.ID)));

			if (!Page.IsPostBack)
				myDataView.Sort = sortField + " " + sortDirection;
			
			addExtraColumns();
			BindGrid();
		}

		/// <summary>
		/// Bind the data grid
		/// </summary>
		protected void BindGrid()
		{

			myDataGrid.DataSource = myDataView;
			
			myDataGrid.Columns[0].Visible = false;
			myDataGrid.Columns[1].Visible = IsEditable;
			myDataGrid.Columns[2].Visible = bool.Parse(Settings["DOCUMENTS_SHOWTITLE"].ToString());
			myDataGrid.Columns[3].Visible = bool.Parse(Settings["DOCUMENTS_SHOWOWNER"].ToString());
			myDataGrid.Columns[4].Visible = bool.Parse(Settings["DOCUMENTS_SHOWAREA"].ToString());
			myDataGrid.Columns[5].Visible = bool.Parse(Settings["DOCUMENTS_SHOWLASTUPDATED"].ToString());
			
			myDataGrid.DataBind();
		}

		/// <summary>
		/// Add any extra columns set in the module settings
		/// </summary>
		private void addExtraColumns()
		{
			// is there a setting?
			if( Settings["DOCUMENTS_COLUMNNAMES"] == null )
				return;

			string columnNames = Settings["DOCUMENTS_COLUMNNAMES"].ToString().Trim();
			
			// is there a column name in the settings?
			if( columnNames == string.Empty )
				return;

			// is it a single value
			if( columnNames.IndexOf(";") == -1 )
				addExtraColumn(columnNames);

				// is it a ; delimited list
			else
			{
				// split the list of names
				string [] extraColumnNames = Settings["DOCUMENTS_COLUMNNAMES"].ToString().Split(";".ToCharArray());
				
				// add each column to the grid
				for( int i = 0; i < extraColumnNames.Length; i++ )
					addExtraColumn(extraColumnNames[i]);
			}
		}

		/// <summary>
		/// Add a single column to the data grid
		/// </summary>
		/// <param name="name"></param>
		private void addExtraColumn(string name)
		{
			TemplateColumn col = new TemplateColumn();
					
			col.HeaderText = name;
			col.HeaderStyle.CssClass="NormalBold";
			col.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
			col.HeaderStyle.Width = Unit.Pixel(50);

			col.ItemStyle.HorizontalAlign=HorizontalAlign.Center;
			col.ItemStyle.Width = Unit.Pixel(50);
					
			myDataGrid.Columns.Add(col);
		}


		#region Image documents by jviladiu@portalservices.net (02/07/2004)

		private void setDataView (DataSet documentsData) 
		{
			myDataView = documentsData.Tables[0].DefaultView;
		}

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
				ext = fi.Extension;
				nme = f_Name.Substring(0, (f_Name.Length - ext.Length));
				availExtensions.Add(nme, f_Name);
			}

		}

		private string imageAsign (string contentType) 
		{
			// jminond - switched to use extensions pack
			if(availExtensions.ContainsKey(contentType))
			{
				return availExtensions[contentType].ToString();
			}
			else
			{
				return "unknown.gif";
			}
		}
		#endregion

		/// <summary>
        /// GetBrowsePath() is a helper method used to create the url   
        /// to the document.  If the size of the content stored in the   
        /// database is non-zero, it creates a path to browse that.   
        /// Otherwise, the FileNameUrl value is used.
        ///
        /// This method is used in the databinding expression for
        /// the browse Hyperlink within the DataGrid, and is called 
        /// for each row when DataGrid.DataBind() is called.  It is 
        /// defined as a helper method here (as opposed to inline 
        /// within the template) to improve code organization and
        /// avoid embedding logic within the content template.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="size"></param>
        /// <param name="documentID"></param>
        /// <returns></returns>
		protected string GetBrowsePath(string url, object size, int documentID) 
		{
			if (size != DBNull.Value && (int) size > 0) 
			{
				// if there is content in the database, create an url to browse it
				// Add ModuleID into url for correct security access. jviladiu@portalServices.net (02/07/2004)
				return (HttpUrlBuilder.BuildUrl("~/DesktopModules/SecureDocuments/SecureDocumentsView.aspx", "ItemID=" + documentID.ToString() + "&MId=" + ModuleID.ToString() + "&wversion=" + Version.ToString()));
			}
			else 
			{
				// otherwise, return the FileNameUrl
				return url;
			}
		}

		#region General Implementation
		/// <summary>
		/// General Module def GUID
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{08E7FDAD-3033-49b8-9B10-BA7EC7AF5415}");
			}
		}

		#region Search Implementation

		/// <summary>
		/// Searchable module
		/// </summary>
		public override bool Searchable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Searchable module implementation
		/// </summary>
		/// <param name="portalID">The portal ID</param>
		/// <param name="userID">ID of the user is searching</param>
		/// <param name="searchString">The text to search</param>
		/// <param name="searchField">The fields where perfoming the search</param>
		/// <returns>The SELECT sql to perform a search on the current module</returns>
		public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
		{
			SearchDefinition s = new SearchDefinition("rb_Documents", "FileFriendlyName", "FileNameUrl", "CreatedByUser", "CreatedDate", searchField);
			
			//Add extra search fields here, this way
			s.ArrSearchFields.Add("itm.Category");

			return s.SearchSqlSelect(portalID, userID, searchString);
		}
		#endregion

		# region Install / Uninstall Implementation
		/// <summary>
		/// install
		/// </summary>
		/// <param name="stateSaver"></param>
		public override void Install(IDictionary stateSaver)
		{
			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}

		/// <summary>
		/// Uninstall
		/// </summary>
		/// <param name="stateSaver"></param>
		public override void Uninstall(IDictionary stateSaver)
		{
			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}
		# endregion

		#endregion          

		#region Web Form Designer generated code
		/// <summary>
		/// Raises Init event
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			// extended image for extensions
			this.baseImageDIR = Path.ApplicationRoot + "/aspnet_client/Ext/";
			
			this.AddText = "ADD_DOCUMENT";
			this.AddUrl = "~/DesktopModules/SecureDocuments/SecureDocumentsEdit.aspx";
		
			base.OnInit(e);
		}

		private void InitializeComponent() 
		{
			this.myDataGrid.SortCommand += new DataGridSortCommandEventHandler(this.myDataGrid_SortCommand);
			this.myDataGrid.ItemDataBound += new DataGridItemEventHandler(this.myDataGrid_ItemDataBound);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

		private void myDataGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (sortField == e.SortExpression)
			{
				if (sortDirection == "ASC")
					sortDirection = "DESC";
				else
					sortDirection = "ASC";
			}

			ViewState["SortField"] = e.SortExpression;
			ViewState["sortDirection"] = sortDirection;

			myDataView.Sort = e.SortExpression + " " + sortDirection;
			BindGrid();
		}

		private void myDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
			if (e.Item.Cells [3].Text == "unknown")
			{
				e.Item.Cells [3].Text = Localize.GetString ( "UNKNOWN", "unknown");
			}
			
			// Added by brian kierstead 4/13/2005 to accomadate multiple files
			// per document entry
			// make sure we have some data
			if( e.Item.Cells[0].Text == "&nbsp;" )
				return;
			
			// get the non-extra column count
			int i = normalColCount;
			
			// if there are no extra columns bail
			if( i == e.Item.Cells.Count )
				return;

			// do the look up for the files with this item
			SqlDataReader dr = SecureDocumentDB.GetDocumentFiles(int.Parse(e.Item.Cells[0].Text), Version);
			try
			{
				// if there is only 1 column, then add all the files to that cell
				if( (i + 1) == e.Item.Cells.Count )
					while(dr.Read())
					{
						e.Item.Cells[i].Controls.Add(createHyperLink(dr));
						e.Item.Cells[i].Controls.Add(new LiteralControl("<br/>"));
					}
					
					// if there are more than 1, add 1 to each until we run out
					// of columns or records
				else
					while(dr.Read() && i < e.Item.Cells.Count)
					{
						e.Item.Cells[i].Controls.Add(createHyperLink(dr));
						i++;
					}
			}
			finally
			{
				dr.Close();
			}
			// End 
		}

		/// <summary>
		/// Create a hyperlink to the file based on the datareader contents.
		/// </summary>
		/// <param name="dr"></param>
		/// <returns></returns>
		private HyperLink createHyperLink( SqlDataReader dr )
		{
			HyperLink hl = new HyperLink();
			hl.ImageUrl =  this.baseImageDIR + imageAsign(dr["ContentType"].ToString());
			hl.NavigateUrl = GetBrowsePath(dr["FileNameUrl"].ToString(), dr["ContentSize"], int.Parse(dr["ItemID"].ToString()));
			hl.Target = "_new";

			return hl;
		}

	}
}
