using System;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.UI;
using Literal = Esperantus.WebControls.Literal;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// Author:					Joe Audette
	/// Created:				1/18/2004
	/// Last Modified:			2/18/2004 (Jakob Hansen did localizing)
	/// </summary>
	[History("jminond", "2004/04/5", "Changes for moving Tab to Page")]
	public class ArchiveView : ViewItemPage
	{
		#region Declarations
		/// <summary>
		/// 
		/// </summary>
		protected DataList myDataList;
		/// <summary>
		/// 
		/// </summary>
		protected HtmlAnchor lnkRSS;
		/// <summary>
		/// 
		/// </summary>
		protected HtmlImage imgRSS;
		/// <summary>
		/// 
		/// </summary>
		protected Label lblEntryCount;
		/// <summary>
		/// 
		/// </summary>
		protected Label lblCommentCount;
		/// <summary>
		/// 
		/// </summary>
		protected Repeater dlArchive;
		/// <summary>
		/// 
		/// </summary>
		protected Label lblHeader;
		/// <summary>
		/// 
		/// </summary>
		protected Literal BlogPageLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Literal SyndicationLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Literal StatisticsLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Literal ArchivesLabel;
		/// <summary>
		/// 
		/// </summary>
		protected Label lblCopyright;
		/// <summary>
		/// 
		/// </summary>
		protected string Feedback;
		protected string sortField = "Title";
		protected string sortOrder = "ASC";
		#endregion

		private void Page_Load(object sender, EventArgs e)
		{
		
			// Added EsperantusKeys for Localization 
			// Mario Endara mario@softworks.com.uy june-1-2004 
			Feedback = Localize.GetString ("BLOG_FEEDBACK");

			if(!IsPostBack)
			{
				BindArchive();
			}
		}

		private void BindArchive()
		{
			lnkRSS.HRef = HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/RSS.aspx",PageID,"&mID=" + ModuleID );
			imgRSS.Src = HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/xml.gif");
			lblCopyright.Text = moduleSettings["Copyright"].ToString();
		
			BlogDB blogDB = new BlogDB();
			int month = -1;
			int year = -1;
			try
			{
				month = int.Parse(Request.Params.Get("month"));
				year = int.Parse(Request.Params.Get("year"));
			}
			catch{}

			if((month > -1)&&(year > -1))
			{
				this.lblHeader.Text = Localize.GetString("BLOG_POSTSFROM", "Posts From", null) +
					" " + DateTime.Parse(month.ToString() + "/1/" + year.ToString()).ToString("MMMM, yyyy");
				myDataList.DataSource = blogDB.GetBlogEntriesByMonth(month, year, ModuleID);
			}
			else
			{
				myDataList.DataSource = blogDB.GetBlogs(ModuleID);
			}
			myDataList.DataBind();

			dlArchive.DataSource = blogDB.GetBlogMonthArchive(ModuleID);
			dlArchive.DataBind();

			SqlDataReader dataReader = blogDB.GetBlogStats(ModuleID);
			try
			{
				if (dataReader.Read())
				{
					lblEntryCount.Text = Localize.GetString("BLOG_ENTRIES", "Entries", null) + 
						" (" + dataReader["EntryCount"].ToString() + ")";
					lblCommentCount.Text = Localize.GetString("BLOG_COMMENTS", "Comments", null) +
						" (" + dataReader["CommentCount"].ToString() + ")";
					
				}
			}
			finally
			{
				dataReader.Close();
			}
		}

		#region Sorting
		protected void btnSortArchiveByTitle(object sender, EventArgs e)
		{
			this.sortField = "Title";
			BindArchive();
		}
		protected void btnSortArchiveByDate(object sender, EventArgs e)
		{

			this.sortField = "Date";
			BindArchive();
		}
		protected void btnSortArchiveByComments(object sender, EventArgs e)
		{

			this.sortField = "Comments";
			BindArchive();
		}
		#endregion

		#region Web Form Designer generated code
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
			this.Load += new EventHandler(this.Page_Load);
			if ( !this.IsCssFileRegistered("Mod_Blogs") )
				this.RegisterCssFile("Mod_Blogs");
		}
		#endregion
	}
}
