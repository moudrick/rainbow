using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.UI.WebControls;

namespace Rainbow.DesktopModules
	{
	/// <summary>
	///		Summary description for Backlog.
	/// </summary>
	public class Recycler : PortalModuleControl
	{
		protected DataGrid DataGrid1;
		
		private void Page_Load(object sender, EventArgs e)
		{
			if(!Page.IsPostBack)
			{
                BindData("ModuleTitle");
			}
		}	
		protected void BindData(string SortField)
		{
			DataTable dt = RecyclerDB.GetModulesInRecycler(this.portalSettings.PortalID,SortField);
			this.DataGrid1.DataSource = dt;

			this.DataGrid1.DataBind();				
		}
		/// <summary>
		/// The DeleteModuleButton_Click server event handler on this page is
		/// used to delete a portal module
		/// This method is copied directly from PortalModuleControl (exists in 
		/// both places!! ugh.)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteModuleButton_Click(Object sender, EventArgs e) 
		{
			ModulesDB admin = new ModulesDB();
			
			//admin.DeleteModule(sender.ToString());
			Response.Write("Sending module is " + sender.ToString());
			// Redirect to the same page to pick up changes
			Page.Response.Redirect(Page.Request.RawUrl);
		}

		/// <summary>
		/// The DataGrid1_SortCommand server event handler on this page is
		/// used to sort data in the datagrid based upon the SortCommand method of the
		/// DataGrid.  
		/// </summary>
		/// <param name=""></param>
		/// <param name="e"></param>
		private void DataGrid1_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			string SortField = e.SortExpression.ToString();
			BindData(SortField);
		}

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
				return new Guid("{E928F47B-A131-4a33-88D5-D5D6E7A94B36}");
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		
			this.DataGrid1.SortCommand +=new DataGridSortCommandEventHandler(this.DataGrid1_SortCommand);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

		#region Install / Uninstall Implementation
		public override void Install(IDictionary stateSaver)
		{
//			string currentScriptName = Server.MapPath(this.TemplateSourceDirectory + "/Install.sql");
//			ArrayList errors = Rainbow.Helpers.DBHelper.ExecuteScript(currentScriptName, true);
//			if (errors.Count > 0)
//			{
//				// Call rollback
//				throw new Exception("Error occurred:" + errors[0].ToString());
//			}
		}

		public override void Uninstall(IDictionary stateSaver)
		{
//			string currentScriptName = Server.MapPath(this.TemplateSourceDirectory + "/Uninstall.sql");
//			ArrayList errors = Rainbow.Helpers.DBHelper.ExecuteScript(currentScriptName, true);
//			if (errors.Count > 0)
//			{
//				// Call rollback
//				throw new Exception("Error occurred:" + errors[0].ToString());
//			}
		}

		#endregion
	}
}
