#region Usings
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;

// Import log4net classes.
using log4net;
using log4net.Config;
using WinClientEcommerce.exportService;
using WinClientEcommerce.importService;

#endregion

namespace WinClientEcommerce
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		#region Variables
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem FileExit;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem FileDownloadCategories;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem FileUploadProducts;
		private System.Data.OleDb.OleDbConnection msAccessConnection;
		private System.Windows.Forms.MenuItem FileDownloadProducts;
		private System.Windows.Forms.MenuItem FileDownloadAll;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.StatusBar statusBar1;
		private ThreadStart ts;
		Thread t;
		int categoryId;

		// Define a static logger variable so that it references the
		// Logger instance named "MainForm".
		private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));
		private System.Windows.Forms.DataGrid dataGrid;
		private System.Windows.Forms.MenuItem menuItem2;

		// context menu data members
		private OleDbDataAdapter adapter;
		private OleDbCommandBuilder builder;
		DataSet ds;
		DataView dtw;
		private System.Windows.Forms.ContextMenu dataGridContextMenu;
		private System.Windows.Forms.MenuItem contextUpdate;
		private System.Windows.Forms.MenuItem contextUpdateAll;
		private System.Windows.Forms.MenuItem contextRefresh;
		int rowNumber;
		int columnNumber;
		DataGridTableStyle dgts;
		private System.Windows.Forms.MenuItem getProductsMenu;
		private System.Windows.Forms.MenuItem getCategoriesMenu;
		private System.Windows.Forms.MenuItem menuAbort;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		exportService.ExportService expserv;
		importService.ImportService impserv;
		exportService.AuthHeader exportCredentials;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblPassword;
		importService.AuthHeader importCredentials;

		#endregion

		public MainForm()
		{
			// Initialise the logging when the application loads
			DOMConfigurator.Configure();

			// Required for Windows Form Designer support
			InitializeComponent();
			expserv = new exportService.ExportService();
			impserv = new importService.ImportService();
			exportCredentials = new exportService.AuthHeader();
			importCredentials = new importService.AuthHeader();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.FileDownloadAll = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.FileDownloadCategories = new System.Windows.Forms.MenuItem();
			this.FileDownloadProducts = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.FileUploadProducts = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.FileExit = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.getProductsMenu = new System.Windows.Forms.MenuItem();
			this.getCategoriesMenu = new System.Windows.Forms.MenuItem();
			this.menuAbort = new System.Windows.Forms.MenuItem();
			this.msAccessConnection = new System.Data.OleDb.OleDbConnection();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.dataGrid = new System.Windows.Forms.DataGrid();
			this.dataGridContextMenu = new System.Windows.Forms.ContextMenu();
			this.contextUpdate = new System.Windows.Forms.MenuItem();
			this.contextUpdateAll = new System.Windows.Forms.MenuItem();
			this.contextRefresh = new System.Windows.Forms.MenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.lblUsername = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem3,
																					  this.menuItem6,
																					  this.menuItem5,
																					  this.FileExit});
			this.menuItem1.Text = "File";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.FileDownloadAll,
																					  this.menuItem4,
																					  this.FileDownloadCategories,
																					  this.FileDownloadProducts});
			this.menuItem3.Text = "Download";
			// 
			// FileDownloadAll
			// 
			this.FileDownloadAll.Index = 0;
			this.FileDownloadAll.Text = "Sve";
			this.FileDownloadAll.Click += new System.EventHandler(this.FileDownloadAll_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "-";
			// 
			// FileDownloadCategories
			// 
			this.FileDownloadCategories.Index = 2;
			this.FileDownloadCategories.Text = "Only categories";
			this.FileDownloadCategories.Click += new System.EventHandler(this.FileDownloadCategories_Click);
			// 
			// FileDownloadProducts
			// 
			this.FileDownloadProducts.Index = 3;
			this.FileDownloadProducts.Text = "Only products";
			this.FileDownloadProducts.Click += new System.EventHandler(this.FileDownloadProducts_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.FileUploadProducts});
			this.menuItem6.Text = "Upload";
			// 
			// FileUploadProducts
			// 
			this.FileUploadProducts.Index = 0;
			this.FileUploadProducts.Text = "Products";
			this.FileUploadProducts.Click += new System.EventHandler(this.FileUploadProducts_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "-";
			// 
			// FileExit
			// 
			this.FileExit.Index = 3;
			this.FileExit.Text = "Exit";
			this.FileExit.Click += new System.EventHandler(this.FileExit_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.getProductsMenu,
																					  this.getCategoriesMenu,
																					  this.menuAbort});
			this.menuItem2.Text = "Data";
			// 
			// getProductsMenu
			// 
			this.getProductsMenu.Index = 0;
			this.getProductsMenu.Text = "Get Products";
			this.getProductsMenu.Click += new System.EventHandler(this.getProductsMenu_Click);
			// 
			// getCategoriesMenu
			// 
			this.getCategoriesMenu.Index = 1;
			this.getCategoriesMenu.Text = "Get Categories";
			this.getCategoriesMenu.Click += new System.EventHandler(this.getCategoriesMenu_Click);
			// 
			// menuAbort
			// 
			this.menuAbort.Index = 2;
			this.menuAbort.Text = "Abort Any";
			this.menuAbort.Click += new System.EventHandler(this.menuAbort_Click);
			// 
			// msAccessConnection
			// 
			this.msAccessConnection.ConnectionString = ((string)(configurationAppSettings.GetValue("msAccessConnection.ConnectionString", typeof(string))));
			// 
			// progressBar1
			// 
			this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBar1.Location = new System.Drawing.Point(0, 513);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(680, 16);
			this.progressBar1.TabIndex = 0;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 497);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(680, 16);
			this.statusBar1.TabIndex = 1;
			// 
			// dataGrid
			// 
			this.dataGrid.ContextMenu = this.dataGridContextMenu;
			this.dataGrid.DataMember = "";
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid.Location = new System.Drawing.Point(0, 0);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.Size = new System.Drawing.Size(680, 497);
			this.dataGrid.TabIndex = 3;
			// 
			// dataGridContextMenu
			// 
			this.dataGridContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.contextUpdate,
																								this.contextUpdateAll,
																								this.contextRefresh});
			// 
			// contextUpdate
			// 
			this.contextUpdate.Index = 0;
			this.contextUpdate.Text = "Update Category";
			this.contextUpdate.Click += new System.EventHandler(this.contextUpdate_Click);
			// 
			// contextUpdateAll
			// 
			this.contextUpdateAll.Index = 1;
			this.contextUpdateAll.Text = "Upload All Categories";
			this.contextUpdateAll.Click += new System.EventHandler(this.contextUpdateAll_Click);
			// 
			// contextRefresh
			// 
			this.contextRefresh.Index = 2;
			this.contextRefresh.Text = "Refresh Categories";
			this.contextRefresh.Click += new System.EventHandler(this.contextRefresh_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.txtUsername);
			this.panel1.Controls.Add(this.lblUsername);
			this.panel1.Controls.Add(this.txtPassword);
			this.panel1.Controls.Add(this.lblPassword);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 465);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(680, 32);
			this.panel1.TabIndex = 7;
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(72, 8);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.TabIndex = 7;
			this.txtUsername.Text = "";
			// 
			// lblUsername
			// 
			this.lblUsername.Location = new System.Drawing.Point(8, 8);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(56, 16);
			this.lblUsername.TabIndex = 10;
			this.lblUsername.Text = "Username";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(260, 6);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.TabIndex = 8;
			this.txtPassword.Text = "";
			// 
			// lblPassword
			// 
			this.lblPassword.Location = new System.Drawing.Point(184, 9);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 16);
			this.lblPassword.TabIndex = 9;
			this.lblPassword.Text = "Password";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(680, 529);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.dataGrid);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.progressBar1);
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "MainForm";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}


		#region GetModuleID
		private int ModuleId
		{
			get
			{
				string moduleIdSetting = System.Configuration.ConfigurationSettings.AppSettings["moduleId"];
				return int.Parse(moduleIdSetting);
			}
		}
		#endregion
		
		#region Web Services
		exportService.ExportService exportFromService;
		private exportService.ExportService Export
		{
			get
			{
				if(exportFromService == null)
				{
					exportFromService = new exportService.ExportService();
					exportFromService.Timeout = 2000000;
					log.Info("Connecting to exportService: '" + exportFromService.Url + "'");
				}
				return exportFromService;
			}
		}

		importService.ImportService importFromService;
		private importService.ImportService Import
		{
			get
			{
				if(importFromService == null)
				{
					importFromService = new importService.ImportService();
					importFromService.Timeout = 2000000;
					log.Info("Connecting to importService: '" + importFromService.Url + "'");
				}
				return importFromService;
			}
		}
		#endregion
		
		#region ManageException
		private void ManageException(Exception ex)
		{
			log.Error("Error:", ex);
			MessageBox.Show(ex.Message);
		}
		#endregion

		#region FromServerToClient
		private int DeleteAndRecreateCategories(DataSet myCategories)
		{
			OleDbCommand deleteCategories = new OleDbCommand();
			deleteCategories.CommandText = "DELETE FROM Categories";
			deleteCategories.Connection = this.msAccessConnection;

			OleDbCommand insertCategories = new OleDbCommand();
			insertCategories.CommandText = "INSERT INTO Categories(categoryId, categoryName) VALUES (?, ?)";
			insertCategories.Connection = this.msAccessConnection;
			insertCategories.Parameters.Add(new System.Data.OleDb.OleDbParameter("categoryId", System.Data.OleDb.OleDbType.Integer, 0, "categoryId"));
			insertCategories.Parameters.Add(new System.Data.OleDb.OleDbParameter("categoryName", System.Data.OleDb.OleDbType.VarWChar, 50, "categoryName"));

			int count = 0;
			msAccessConnection.Open();
			try
			{
				//Delete all categories
				deleteCategories.ExecuteNonQuery();

				//Insert all categories
				DataTable myDataTable = myCategories.Tables[0];
				foreach(DataRow dr in myDataTable.Rows)
				{
					count++;
					foreach(DataColumn d in myDataTable.Columns)
						insertCategories.Parameters[d.ColumnName].Value = dr[d.ColumnName];
					insertCategories.ExecuteNonQuery();
				}
				return count;
			}
			catch
			{
				throw;
			}
			finally
			{
				msAccessConnection.Close();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryId"></param>
		/// <param name="myCategories">Pass null dataset for delete only</param>
		/// <returns></returns>
		private int DeleteAndRecreateProducts(int moduleId, int categoryId, DataSet myProducts)
		{
			OleDbCommand deleteProducts = new OleDbCommand();
			deleteProducts.Connection = this.msAccessConnection;
			if(categoryId > 0)
			{
				deleteProducts.CommandText = "DELETE FROM Products WHERE CategoryID = ?";
				deleteProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("CategoryID", System.Data.OleDb.OleDbType.Integer, 0, "CategoryID"));
				deleteProducts.Parameters["CategoryId"].Value = categoryId;
			}
			else
			{
				deleteProducts.CommandText = "DELETE FROM Products";
			}

			OleDbCommand insertProducts = new OleDbCommand();
			insertProducts.CommandText = "INSERT INTO Products(CategoryID, DisplayOrder, FeaturedItem, LongDescription, Met" +
				"adataXml, ModelName, ModelNumber, ModuleID, ProductID, ShortDescription, TaxRate" +
				", UnitPrice, Weight) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
			insertProducts.Connection = this.msAccessConnection;
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("CategoryID", System.Data.OleDb.OleDbType.Integer, 0, "CategoryID"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("DisplayOrder", System.Data.OleDb.OleDbType.Integer, 0, "DisplayOrder"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("FeaturedItem", System.Data.OleDb.OleDbType.Boolean, 2, "FeaturedItem"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("LongDescription", System.Data.OleDb.OleDbType.VarWChar, 0, "LongDescription"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("MetadataXml", System.Data.OleDb.OleDbType.VarWChar, 0, "MetadataXml"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("ModelName", System.Data.OleDb.OleDbType.VarWChar, 255, "ModelName"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("ModelNumber", System.Data.OleDb.OleDbType.VarWChar, 255, "ModelNumber"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("ModuleID", System.Data.OleDb.OleDbType.Integer, 0, "ModuleID"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("ProductID", System.Data.OleDb.OleDbType.Integer, 0, "ProductID"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("ShortDescription", System.Data.OleDb.OleDbType.VarWChar, 0, "ShortDescription"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("TaxRate", System.Data.OleDb.OleDbType.Double, 0, "TaxRate"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("UnitPrice", System.Data.OleDb.OleDbType.Currency, 0, "UnitPrice"));
			insertProducts.Parameters.Add(new System.Data.OleDb.OleDbParameter("Weight", System.Data.OleDb.OleDbType.Double, 0, "Weight"));

			int count = 0;
			msAccessConnection.Open();
			try
			{
				//Delete all Products
				deleteProducts.ExecuteNonQuery();

				//Insert all Products
				if(myProducts != null)
				{
					DataTable myDataTable = myProducts.Tables[0];

					//Parameters check consistency
					foreach(DataColumn d in myDataTable.Columns)
						if(insertProducts.Parameters[d.ColumnName] == null)
							throw new Exception("'" + d.ColumnName + "' column not found in parameter collection!");

					foreach(OleDbParameter p in insertProducts.Parameters)
						if(p.ParameterName != "CategoryID" && p.ParameterName != "ModuleID" && myDataTable.Columns[p.ParameterName] == null)
							throw new Exception("'" + p.ParameterName + "' parameter not found in column collection!");

					foreach(DataRow dr in myDataTable.Rows)
					{
						count++;
						insertProducts.Parameters["CategoryID"].Value = categoryId;
						insertProducts.Parameters["ModuleID"].Value = moduleId;
						foreach(DataColumn d in myDataTable.Columns)
							insertProducts.Parameters[d.ColumnName].Value = dr[d.ColumnName];
						insertProducts.ExecuteNonQuery();
					}
				}
				return count;
			}
			catch
			{
				throw;
			}
			finally
			{
				msAccessConnection.Close();
			}
		}

		private DataSet GetCategoryList()
		{
			OleDbDataAdapter myAdapter = new OleDbDataAdapter("SELECT categoryId, categoryName FROM Categories", this.msAccessConnection);

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myAdapter.Fill(myDataSet);
			}
			finally
			{
				msAccessConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}

		private DataSet GetCategoryList(int catetgoryId)
		{
			OleDbDataAdapter myAdapter = new OleDbDataAdapter("SELECT categoryId, categoryName FROM Categories WHERE CategoryId = ?", this.msAccessConnection);
			myAdapter.SelectCommand.Parameters.Add(new System.Data.OleDb.OleDbParameter("CategoryID", System.Data.OleDb.OleDbType.Integer, 0, "categoryID"));
			myAdapter.SelectCommand.Parameters["CategoryId"].Value = categoryId;

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myAdapter.Fill(myDataSet);
			}
			finally
			{
				msAccessConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}
		#endregion			

		#region FromClientToServer		
		private DataSet GetProductList()
		{
			OleDbDataAdapter myAdapter = new OleDbDataAdapter("SELECT * FROM Products", this.msAccessConnection);

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myAdapter.Fill(myDataSet);
			}
			finally
			{
				msAccessConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}
		private DataSet GetProductList(int categoryId, int startRecord, int maxRecord)
		{
			OleDbDataAdapter myAdapter = new OleDbDataAdapter("SELECT * FROM Products WHERE CategoryId = ?", this.msAccessConnection);
			myAdapter.SelectCommand.Parameters.Add(new System.Data.OleDb.OleDbParameter("CategoryID", System.Data.OleDb.OleDbType.Integer, 0, "CategoryID"));
			myAdapter.SelectCommand.Parameters["CategoryId"].Value = categoryId;

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myAdapter.Fill(myDataSet, startRecord, maxRecord, "Products");
			}
			finally
			{
				msAccessConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}

		private DataSet GetProductList(int categoryId)
		{
			OleDbDataAdapter myAdapter = new OleDbDataAdapter("SELECT * FROM Products WHERE CategoryId = ?", this.msAccessConnection);
			myAdapter.SelectCommand.Parameters.Add(new System.Data.OleDb.OleDbParameter("CategoryID", System.Data.OleDb.OleDbType.Integer, 0, "CategoryID"));
			myAdapter.SelectCommand.Parameters["CategoryId"].Value = categoryId;

			//  Create and Fill the DataSet
			DataSet myDataSet = new DataSet();
			try
			{
				myAdapter.Fill(myDataSet);
			}
			finally
			{
				msAccessConnection.Close();
			}

			//  return the DataSet
			return myDataSet;
		}
		#endregion

		#region MenuEvents
		private void FileExit_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		private void menuAbort_Click(object sender, System.EventArgs e)
		{
			// updated: checking for if t is null
			if(t != null && t.IsAlive) 
			{
				t.Abort();
				EnableMenu();
			}
			
		}
		private void FileDownloadCategories_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Cursor.Current =  Cursors.WaitCursor;
			
			try
			{
				exportCredentials.UserName = txtUsername.Text;
				exportCredentials.Password = txtPassword.Text;
				expserv.AuthHeaderValue = exportCredentials;

				int result = 0;
				//Get categories
				DataSet myCategories = expserv.GetCategories(ModuleId);
				
				result += DeleteAndRecreateCategories(myCategories);
				MessageBox.Show("Successfuly updated. Downloaded '" + result.ToString() + "' record(s).");
			}
			catch(Exception ex)
			{
				ManageException(ex);
			}
			finally
			{
				EnableMenu();
			}
		}


		private void FileDownloadProducts_Click(object sender, System.EventArgs e)
		{
			exportCredentials.UserName = txtUsername.Text;
			exportCredentials.Password = txtPassword.Text;
			expserv.AuthHeaderValue = exportCredentials;
			System.Windows.Forms.Cursor.Current =  Cursors.WaitCursor;			
			try
			{
				//First remove all products
				DeleteAndRecreateProducts(ModuleId, 0, null);
				
				//Then read from categories
				int result = 0;
				DataSet myCategories = GetCategoryList();

				progressBar1.Maximum = myCategories.Tables[0].Rows.Count;
				progressBar1.Value = 0;

				foreach(DataRow dr in myCategories.Tables[0].Rows)
				{
					try
					{
						progressBar1.Value++;
						progressBar1.Refresh();
					}
					catch {}
					statusBar1.Text = "Reading: " + dr["categoryName"].ToString().TrimStart('-');
					log.Info(statusBar1.Text); statusBar1.Refresh();
					
					int categoryId = int.Parse(dr["categoryId"].ToString());
					DataSet myProducts = expserv.GetProducts(ModuleId,categoryId);
					result += DeleteAndRecreateProducts(ModuleId, categoryId, myProducts);
				}
				statusBar1.Text = "Successfuly updated records. Downloaded '" + result.ToString() + "' record(s).";
				MessageBox.Show(statusBar1.Text);
				
				progressBar1.Value = 0;
			}
			catch(Exception ex)
			{
				ManageException(ex);
			}
			finally
			{
				System.Windows.Forms.Cursor.Current =  Cursors.Default;
				EnableMenu();
			}		
		}

		private void FileDownloadAll_Click(object sender, System.EventArgs e)
		{
			exportCredentials.UserName = txtUsername.Text;
			exportCredentials.Password = txtPassword.Text;
			expserv.AuthHeaderValue = exportCredentials;
			System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;		
			try
			{
				int cats = 0, prods = 0;
				//Get categories
				DataSet myCategories = expserv.GetCategories(ModuleId);
				cats += DeleteAndRecreateCategories(myCategories);

				//First remove all products
				DeleteAndRecreateProducts(ModuleId, 0, null);

				progressBar1.Maximum = myCategories.Tables[0].Rows.Count;
				progressBar1.Value = 0;
				
				//Then read from categories
				foreach(DataRow dr in myCategories.Tables[0].Rows)
				{
					try
					{
						progressBar1.Value++;
						progressBar1.Refresh();
					}
					catch {}
					statusBar1.Text = "Loading: " + dr["categoryName"].ToString().TrimStart('-');
					log.Info(statusBar1.Text); statusBar1.Refresh();
					
					int categoryId = int.Parse(dr["categoryId"].ToString());					
					DataSet myProducts = expserv.GetProducts(ModuleId, categoryId);
					prods += DeleteAndRecreateProducts(ModuleId, categoryId, myProducts);
				}
				statusBar1.Text = "Successfuly updated records. Downloaded '" + cats.ToString() + "' Categories and '" + prods.ToString() + "' product(s).";
				MessageBox.Show(statusBar1.Text);
				progressBar1.Value = 0;
			}
			catch(Exception ex)
			{
				ManageException(ex);
			}
			finally
			{
				System.Windows.Forms.Cursor.Current =  Cursors.Default;
				EnableMenu();
			}		
		}

		private static readonly int CHUNK_SIZE = (ConfigurationSettings.AppSettings["ChunkSize"] == null ? 50 : int.Parse(ConfigurationSettings.AppSettings["ChunkSize"]));
		private static readonly int MaxRetries = (ConfigurationSettings.AppSettings["MaxRetries"] == null ? 10 : int.Parse(ConfigurationSettings.AppSettings["MaxRetries"]));
		private void FileUploadProducts_Click(object sender, System.EventArgs e)
		{
			ts = new ThreadStart(UploadProducts);
			t = new Thread(ts);
			t.Start();
			if(t.IsAlive) 
			{
				DisableMenu();
			}	
			else 
			{
				EnableMenu();
			}
		}

		public void UploadProducts() 
		{
			System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;		
			
			importCredentials.UserName = txtUsername.Text;
			importCredentials.Password = txtPassword.Text;
			impserv.AuthHeaderValue = importCredentials;

			try
			{
				int cats = 0, prods = 0;
				//Get categories from local
				DataSet myCategories = GetCategoryList();
				
				progressBar1.Maximum = myCategories.Tables[0].Rows.Count;
				progressBar1.Value = 0;

				//Upload products for each category
				foreach(DataRow dr in myCategories.Tables[0].Rows)
				{
					try
					{
						progressBar1.Value++;
						progressBar1.Refresh();
					}
					catch {}
					statusBar1.Text = "Uploading: " + dr["categoryName"].ToString().TrimStart('-');
					log.Info(statusBar1.Text); statusBar1.Refresh();

					cats++;
					int categoryId = int.Parse(dr["categoryId"].ToString());
					DataSet myProducts = GetProductList(categoryId);

					int totalRows = myProducts.Tables[0].Rows.Count;

					if(totalRows > CHUNK_SIZE)
					{
						for(int i = 0; i < totalRows; i += CHUNK_SIZE)
						{
							myProducts = GetProductList(categoryId, i, CHUNK_SIZE);

							if(myProducts.Tables[0].Rows.Count > 0)
							{
								log.Info("Updating '" + myProducts.Tables[0].Rows.Count + "' Products from Category '" + categoryId + "', from " + myProducts.Tables[0].Rows[0]["ProductID"] + " to " + myProducts.Tables[0].Rows[myProducts.Tables[0].Rows.Count-1]["ProductID"]); 
								
								int currentRetry = 0;
								while(true)
								{
									try
									{
										impserv.UpdateProducts(myProducts);
										break;
									}
									catch(Exception ex)
									{
										if(currentRetry++ > MaxRetries)
										{
											log.Error("This Dataset has been failed: '" + myProducts.GetXml() + "'");
											throw;
										}
										else
										{
											log.Error("Dataset failure: '" + currentRetry + "' times. Retrying.");
											log.Error(ex.Message);
										}
									}
								}
								prods += myProducts.Tables[0].Rows.Count;
							}
							else
								log.Info("There is no any data for updating Category '" + categoryId + "'"); 
						}
					}
					else
					{
						impserv.UpdateProducts(myProducts);
						prods+= myProducts.Tables[0].Rows.Count;
					}

					//Refresh local copy with new ids
					myProducts = expserv.GetProducts(ModuleId, categoryId);
					DeleteAndRecreateProducts(ModuleId, categoryId, myProducts);
				}
				statusBar1.Text = "Successfuly updated records. Added '" + cats.ToString() + "' Categories and '" + prods.ToString() + "' products.";
				MessageBox.Show(statusBar1.Text);
			}
			catch(Exception ex)
			{
				ManageException(ex);
			}
			finally
			{
				System.Windows.Forms.Cursor.Current =  Cursors.Default;
				EnableMenu();
			}
		}

		private void getCategoriesMenu_Click(object sender, System.EventArgs e)
		{
			adapter = new OleDbDataAdapter("SELECT CategoryID, categoryName FROM Categories", this.msAccessConnection);
			builder = new OleDbCommandBuilder(adapter);

			// Create and Fill the DataSet
			ds = new DataSet();
			try 
			{
				adapter.Fill(ds, "Categories");
			}
			finally 
			{
				msAccessConnection.Close();
			}
			dtw = ds.Tables[0].DefaultView;
			dataGrid.DataSource = dtw;
			
			if(dataGrid.TableStyles == null) 
			{
				foreach(DataTable dt in ds.Tables) 
				{
					dgts = new DataGridTableStyle();
					dgts.MappingName = dt.TableName;
					dataGrid.TableStyles.Add(dgts);
				}
			}
			dataGrid.AllowSorting = true;
			dataGrid.HeaderFont = new Font("Arial", 10);
			dataGrid.CaptionText = "Categories";
			EnableMenu();
		
		}

		private void getProductsMenu_Click(object sender, System.EventArgs e)
		{
			adapter = new OleDbDataAdapter("SELECT CategoryID, ModelName FROM Products", this.msAccessConnection);
			builder = new OleDbCommandBuilder(adapter);

			// Create and Fill the DataSet
			ds = new DataSet();
			try 
			{
				adapter.Fill(ds, "Products");
			}
			finally 
			{
				msAccessConnection.Close();
			}
			dtw = ds.Tables[0].DefaultView;
			dataGrid.DataSource = dtw;
			
			if(dataGrid.TableStyles == null) 
			{
				foreach(DataTable dt in ds.Tables) 
				{
					dgts = new DataGridTableStyle();
					dgts.MappingName = dt.TableName;
					dataGrid.TableStyles.Add(dgts);
				}
			}
			dataGrid.AllowSorting = true;
			dataGrid.HeaderFont = new Font("Arial", 10);
			dataGrid.CaptionText = "Products";
			EnableMenu();
		}
		#endregion
	
		#region EnableDisableMenu
		private void DisableMenu()
		{
			FileExit.Enabled = true;
			FileDownloadCategories.Enabled = false;
			FileUploadProducts.Enabled = false;
			FileDownloadProducts.Enabled = false;
			FileDownloadAll.Enabled = false;
			getCategoriesMenu.Enabled = false;
			getProductsMenu.Enabled = false;
		}
		private void EnableMenu() 
		{
			FileExit.Enabled = true;
			FileDownloadCategories.Enabled = true;
			FileUploadProducts.Enabled = true;
			FileDownloadProducts.Enabled = true;
			FileDownloadAll.Enabled = true;
			getCategoriesMenu.Enabled = true;
			getProductsMenu.Enabled = true;
		}
		#endregion
	
		#region ContextMenuEvents
		private void contextRefresh_Click(object sender, System.EventArgs e)
		{
			adapter = new OleDbDataAdapter("SELECT CategoryID, categoryName FROM Categories", this.msAccessConnection);
			builder = new OleDbCommandBuilder(adapter);

			// Create and Fill the DataSet
			ds = new DataSet();
			try 
			{
				adapter.Fill(ds, "Categories");
			}
			finally 
			{
				msAccessConnection.Close();
			}
			dtw = ds.Tables[0].DefaultView;
			dataGrid.DataSource = dtw;

			if(dataGrid.TableStyles == null) 
			{
				foreach(DataTable dt in ds.Tables) 
				{
					dgts = new DataGridTableStyle();
					dgts.MappingName = dt.TableName;
					dataGrid.TableStyles.Add(dgts);
				}
			}
			dataGrid.ColumnHeadersVisible = true;
			dataGrid.AllowSorting = true;
			dataGrid.HeaderFont = new Font("Arial", 10);
			dataGrid.CaptionText = "Categories";
		
		}

		public void contextUpdate_Click(object sender, EventArgs e)
		{
			ts = new ThreadStart(ContextThread);
			t = new Thread(ts);
			t.Start();
			if(t.IsAlive) 
			{
				DisableMenu();
			}	
			else 
			{
				EnableMenu();
			}
		}

		private void ContextThread() 
		{
			DataGridCell cell = dataGrid.CurrentCell;
			columnNumber = cell.ColumnNumber;
			rowNumber = cell.RowNumber;
			// string kategorije = ds.Tables[0].Rows[rowNumber][columnNumber].ToString();
			categoryId = (int) ds.Tables[0].Rows[rowNumber][0];

			try
			{
				if(1 == 2)
				{
					// to do: implement selection checking for null row.
					//MessageBox.Show("You mus select one row!", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					//break;
				}
				int cats = 0, prods = 0;
				//Get categories from local
				DataSet myCategories = GetCategoryList(categoryId);
				
				progressBar1.Maximum = myCategories.Tables[0].Rows.Count;
				progressBar1.Value = 0;

				//Upload products for each category
				foreach(DataRow dr in myCategories.Tables[0].Rows)
				{
					try
					{
						progressBar1.Value++;
						progressBar1.Refresh();
					}
					catch {}
					statusBar1.Text = "Sending: " + dr["categoryName"].ToString().TrimStart('-');
					log.Info(statusBar1.Text); statusBar1.Refresh();

					cats++;
					// int categoryId = int.Parse(dr["categoryId"].ToString());
					DataSet myProducts = GetProductList(categoryId);

					int totalRows = myProducts.Tables[0].Rows.Count;

					if(totalRows > CHUNK_SIZE)
					{
						for(int i = 0; i < totalRows; i += CHUNK_SIZE)
						{
							myProducts = GetProductList(categoryId, i, CHUNK_SIZE);

							if(myProducts.Tables[0].Rows.Count > 0)
							{
								log.Info("Updating '" + myProducts.Tables[0].Rows.Count + "' Products from Category '" + categoryId + "', from " + myProducts.Tables[0].Rows[0]["ProductID"] + " to " + myProducts.Tables[0].Rows[myProducts.Tables[0].Rows.Count-1]["ProductID"]); 
								
								int currentRetry = 0;
								while(true)
								{
									try
									{
										impserv.UpdateProducts(myProducts);
										break;
									}
									catch(Exception)
									{
										if(currentRetry++ > MaxRetries)
										{
											log.Error("This Dataset has been failed: '" + myProducts.GetXml() + "'");
											throw;
										}
										else
										{
											log.Error("Dataset failure: '" + currentRetry + "' puta. Retrying.");
										}
									}
								}
								prods += myProducts.Tables[0].Rows.Count;
							}
							else
								log.Info("Tehre is no any data for updating Category  '" + categoryId + "'"); 
						}
					}
					else
					{
						impserv.UpdateProducts(myProducts);
						prods+= myProducts.Tables[0].Rows.Count;
					}

					//Refresh local copy with new ids
					myProducts = expserv.GetProducts(ModuleId, categoryId);
					DeleteAndRecreateProducts(ModuleId, categoryId, myProducts);
				}
				statusBar1.Text = "Successfuly updated. Added '" + cats.ToString() + "' Categories and '" + prods.ToString() + "' Product(s).";
				MessageBox.Show(statusBar1.Text);
			}
			catch(Exception ex)
			{
				ManageException(ex);
			}
			finally
			{
				System.Windows.Forms.Cursor.Current =  Cursors.Default;
			}
		}
		
		private void contextUpdateAll_Click(object sender, System.EventArgs e)
		{
			ts = new ThreadStart(ContextThreadAll);
			t = new Thread(ts);
			t.Start();
			if(t.IsAlive) 
			{
				DisableMenu();
			}	
			else 
			{
				EnableMenu();
			}
		}

		private void ContextThreadAll() 
		{
			try
			{
				int cats = 0, prods = 0;
				//Get categories from local
				DataSet myCategories = GetCategoryList();
				
				progressBar1.Maximum = myCategories.Tables[0].Rows.Count;
				progressBar1.Value = 0;

				//Upload products for each category
				foreach(DataRow dr in myCategories.Tables[0].Rows)
				{
					try
					{
						progressBar1.Value++;
						progressBar1.Refresh();
					}
					catch {}
					statusBar1.Text = "Sending: " + dr["categoryName"].ToString().TrimStart('-');
					log.Info(statusBar1.Text); statusBar1.Refresh();

					cats++;
					int categoryId = int.Parse(dr["categoryId"].ToString());
					DataSet myProducts = GetProductList(categoryId);

					int totalRows = myProducts.Tables[0].Rows.Count;

					if(totalRows > CHUNK_SIZE)
					{
						for(int i = 0; i < totalRows; i += CHUNK_SIZE)
						{
							myProducts = GetProductList(categoryId, i, CHUNK_SIZE);

							if(myProducts.Tables[0].Rows.Count > 0)
							{
								log.Info("Updating '" + myProducts.Tables[0].Rows.Count + "' Products from Category '" + categoryId + "', from " + myProducts.Tables[0].Rows[0]["ProductID"] + " to " + myProducts.Tables[0].Rows[myProducts.Tables[0].Rows.Count-1]["ProductID"]); 
								
								int currentRetry = 0;
								while(true)
								{
									try
									{
										impserv.UpdateProducts(myProducts);
										break;
									}
									catch(Exception)
									{
										if(currentRetry++ > MaxRetries)
										{
											log.Error("Dataset has been failed."); //myProducts.GetXml() 
											throw;
										}
										else
										{
											log.Error("Dataset failure: '" + currentRetry + "' times. Retrying.");
										}
									}
								}
								prods += myProducts.Tables[0].Rows.Count;
							}
							else
								log.Info("There is no any data for updating Category '" + categoryId + "'"); 
						}
					}
					else
					{
						impserv.UpdateProducts(myProducts);
						prods+= myProducts.Tables[0].Rows.Count;
					}

					//Refresh local copy with new ids
					myProducts = expserv.GetProducts(ModuleId, categoryId);
					DeleteAndRecreateProducts(ModuleId, categoryId, myProducts);
				}
				statusBar1.Text = "Successfuly updated. Added '" + cats.ToString() + "' Categoriess and '" + prods.ToString() + "' Product(s).";
				MessageBox.Show(statusBar1.Text);
			}
			catch(Exception ex)
			{
				ManageException(ex);
			}
			finally
			{
				System.Windows.Forms.Cursor.Current =  Cursors.Default;
			}
		}
	}
	#endregion


}
