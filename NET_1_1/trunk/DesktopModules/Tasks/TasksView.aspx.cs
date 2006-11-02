using System;
using System.Collections;
using System.Data.SqlClient;
using Esperantus;
using Esperantus.WebControls;
using Rainbow.Security;
using Rainbow.UI;

namespace Rainbow.DesktopModules
{
	/// IBS Portal Tasks Module - Display all info about single task
	/// Writen by: ?
	/// Moved into Rainbow by Jakob Hansen, hansen3000@hotmail.com
	public class TaskView : ViewItemPage
    {
		#region Declarations
		/// <summary>
		/// 
		/// </summary>
        protected System.Web.UI.WebControls.Label TitleField;
		/// <summary>
		/// 
		/// </summary>
        protected System.Web.UI.WebControls.Label longdesc;
		///Chris Farrell, 5/27/04, chris@cftechconsulting.com
		///fix longdesc does not wrap text.
		///protected System.Web.UI.HtmlControls.HtmlGenericControl longdesc;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label PercentCompleteField;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label StatusField;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label PriorityField;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label AssignedField;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label StartField;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label DueField;
		/// <summary>
		/// 
		/// </summary>
        protected System.Web.UI.WebControls.Label CreatedBy;
		/// <summary>
		/// 
		/// </summary>
        protected System.Web.UI.WebControls.Label CreatedDate;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label ModifiedBy;
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.Label ModifiedDate;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal1;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal2;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal3;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal4;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal5;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal6;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal7;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal8;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal9;
		/// <summary>
		/// 
		/// </summary>
		protected Literal CreatedLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Literal OnLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Literal ModifiedLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Literal ModifiedOnLabel;
		/// <summary>
		/// 
		/// </summary>
		protected string EditLink = string.Empty;
		#endregion

		/// <summary>
		/// The Page_Load event on this Page is used to obtain the ModuleID
		/// and ItemID of the task to display.
		/// It then uses the Rainbow.TasksDB() data component
		/// to populate the page's edit controls with the task details.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {

			// Verify that the current user has access to edit this module
			if (PortalSecurity.HasEditPermissions(ModuleID))
			{
				EditLink = "<a href= \"TasksEdit.aspx?ItemID=" + ItemID;
				EditLink += "&mID=" +  ModuleID + "\" class=\"Normal\">Edit</a>";
			}

            if (Page.IsPostBack == false)
            {
				//Chris Farrell, chris@cftechconsulting.com, 5/28/04.
				//Improper Identity seed in the ItemID means that there may be tasks
				//with a ItemID = 0.  This is not the way it should be, but there is no
				//reason to NOT show the task with ItemID = 0 and that helps reduce
				//the pains from this bug for users who already have data present.

				// Obtain a single row of Task information
				TasksDB Tasks = new TasksDB();
            	SqlDataReader dr = Tasks.GetSingleTask(ItemID);

				try
				{
					// Read first row from database
					if(dr.Read())
					{
						TitleField.Text = (string) dr["Title"];
						longdesc.Text = (string) dr["Description"];
						StartField.Text = ((DateTime) dr["StartDate"]).ToShortDateString();
						DueField.Text = ((DateTime) dr["DueDate"]).ToShortDateString();
						CreatedBy.Text = (string) dr["CreatedByUser"];
						ModifiedBy.Text = (string) dr["ModifiedByUser"];
						PercentCompleteField.Text = ((Int32) dr["PercentComplete"]).ToString();
						AssignedField.Text = (string) dr["AssignedTo"];
						CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToString();
						ModifiedDate.Text = ((DateTime) dr["ModifiedDate"]).ToString();
						StatusField.Text = Localize.GetString("TASK_STATE_"+(string) dr["Status"],(string) dr["Status"],StatusField);
						PriorityField.Text = Localize.GetString("TASK_PRIORITY_"+(string) dr["Priority"],(string) dr["Priority"],PriorityField);
						// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
						if (CreatedBy.Text == "unknown")
						{
							CreatedBy.Text = Localize.GetString ( "UNKNOWN", "unknown");
						}
						// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
						if (ModifiedBy.Text == "unknown")
						{
							ModifiedBy.Text = Localize.GetString ( "UNKNOWN", "unknown");
						}
					}
				}
				finally
				{
					dr.Close();
				}
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
				al.Add ("2502DB18-B580-4F90-8CB4-C15E6E531012");
				al.Add ("2502DB18-B580-4F90-8CB4-C15E6E531030"); // Access from portalSearch
				al.Add ("2502DB18-B580-4F90-8CB4-C15E6E531052"); // Access from serviceItemList				
				return al;
			}
		}

		#region Web Form Designer generated code
        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

			// Jes1111
			if ( !((Page)this.Page).IsCssFileRegistered("Mod_Tasks") )
				((Page)this.Page).RegisterCssFile("Mod_Tasks");

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
