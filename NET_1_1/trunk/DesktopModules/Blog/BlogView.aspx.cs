using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.UI;
using Button = Esperantus.WebControls.Button;
using ImageButton = Esperantus.WebControls.ImageButton;
using Literal = Esperantus.WebControls.Literal;

namespace Rainbow.DesktopModules
{
	[History("jminond", "march 2005", "Changes for moving Tab to Page")]
    public class BlogView : ViewItemPage
	{
		protected System.Web.UI.WebControls.Label Title;
		protected System.Web.UI.WebControls.Label StartDate;
		protected Literal OnLabel;
		protected Literal CreatedLabel;
		protected System.Web.UI.WebControls.Label Description;
		protected TextBox txtTitle;
		protected TextBox txtName;
		protected TextBox txtURL;
		protected TextBox txtComments;
		protected CheckBox chkRememberMe;
		protected DataList dlComments;
		protected HtmlAnchor lnkRSS;
		protected HtmlImage imgRSS;
		protected Label lblEntryCount;
		protected Label lblCommentCount;
		protected Repeater dlArchive;
		protected Literal BlogPageLabel;
		protected Literal SyndicationLabel;
		protected Literal StatisticsLabel;
		protected Literal ArchivesLabel;
		protected Label lblCopyright;
		protected HyperLink deleteLink;
		protected ImageButton btnDelete;
		// Added EsperantusKeys for Localization 
		// Mario Endara mario@softworks.com.uy june-1-2004 
		protected Literal Literal4;
		protected Literal Literal1;
		protected Literal Literal2;
		protected Literal Literal3;
		protected Literal Literal5;
		protected Literal Literal6;
		protected Button btnPostComment;
		public bool IsDeleteable = false;
		

		/// <summary>
		/// Author:					Joe Audette
		/// Created:				1/18/2004
		/// Last Modified:			2/8/2004
		/// 
		/// The Page_Load server event handler on this page is used
		/// to obtain the Blogs list, and to then display 
		/// the message contents.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
		{
     
            if (!Page.IsPostBack && ModuleID > 0 && ItemID > 0 )
			{
				if(Context.User.Identity.IsAuthenticated)
				{
					char[] separator = {';'};
					string[] deleteRoles = this.Module.AuthorizedDeleteRoles.Split(separator);
					foreach(string role in deleteRoles)
					{
						if(role.Length > 0)
						{
							if(Context.User.IsInRole(role))
							{
								IsDeleteable = true;
							}
						}
					}
				}
				lnkRSS.HRef = HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/RSS.aspx",PageID,"&mID=" + ModuleID );
				imgRSS.Src = HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/xml.gif");
				lblCopyright.Text = moduleSettings["Copyright"].ToString();
			
                BindData();
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
				al.Add ("55EF407B-C9D6-47e3-B627-EFA6A5EEF4B2");
				return al;
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			// Redirect back to the blog page
			this.RedirectBackToReferringPage();

		}

		protected void dlComments_ItemCommand(object source, DataListCommandEventArgs e)
		{
			if(e.CommandName == "DeleteComment")
			{
				BlogDB blogDB = new BlogDB();
				blogDB.DeleteBlogComment(int.Parse(e.CommandArgument.ToString()));
				Response.Clear();
				Response.Redirect(Request.Url.ToString());
			}

		}

		protected void btnPostComment_Click(object sender, EventArgs e)
		{
			if(IsValidComment())
			{
				if(this.chkRememberMe.Checked)
				{
					SetCookies();
				}
				BlogDB blogDB = new BlogDB();
				blogDB.AddBlogComment(ModuleID, ItemID, this.txtName.Text, 
					this.txtTitle.Text, this.txtURL.Text, this.txtComments.Text);
				Response.Redirect(Request.Url.ToString());
			}
		}

		private bool IsValidComment()
		{
			bool result = true;
			//TODO do we need validation?

			return result;
		}

		private void SetCookies()
		{
			HttpCookie blogUserCookie = new HttpCookie("blogUser", this.txtName.Text);
			HttpCookie blogUrlCookie = new HttpCookie("blogUrl", this.txtURL.Text);
			blogUserCookie.Expires = DateTime.Now.AddMonths(1);
			blogUrlCookie.Expires = DateTime.Now.AddMonths(1);
			Response.Cookies.Add(blogUserCookie);
			Response.Cookies.Add(blogUrlCookie);
		}

		/// <summary>
		/// The BindData method is used to obtain details of a message
		/// from the Blogs table, and update the page with
		/// the message content.
		/// </summary>
        void BindData()
		{
            // Obtain the selected item from the Blogs table
            BlogDB blogDB = new BlogDB();
			SqlDataReader dataReader = blogDB.GetSingleBlog(ItemID);
        
			try
			{
				// Load first row from database
				if (dataReader.Read())
				{
					// Update labels with message contents
					Title.Text = (string) dataReader["Title"].ToString();
					txtTitle.Text = "re: " + (string) dataReader["Title"].ToString();
					StartDate.Text = ((DateTime) dataReader["StartDate"]).ToString("dddd MMMM d yyyy hh:mm tt");
					Description.Text = Server.HtmlDecode((string) dataReader["Description"].ToString());
				
				}
			}
			finally
			{
				dataReader.Close();
			}
			dlComments.DataSource = blogDB.GetBlogComments(ModuleID, ItemID);
			dlComments.DataBind();

			if(Request.Params.Get("blogUser") != null)
			{
				this.txtName.Text = Request.Params.Get("blogUser");
			}
			if(Request.Params.Get("blogUrl") != null)
			{
				this.txtURL.Text = Request.Params.Get("blogUrl");
			}

			dlArchive.DataSource = blogDB.GetBlogMonthArchive(ModuleID);
			dlArchive.DataBind();
	
			dataReader = blogDB.GetBlogStats(ModuleID);
			try
			{
				if (dataReader.Read())
				{
					lblEntryCount.Text = Localize.GetString("BLOG_ENTRIES", "Entries", null) + 
						" (" + (string) dataReader["EntryCount"].ToString() + ")";
					lblCommentCount.Text = Localize.GetString("BLOG_COMMENTS", "Comments", null) +
						" (" + (string) dataReader["CommentCount"].ToString() + ")";
				}
			}
			finally
			{
				dataReader.Close();
			}
        }

		#region Web Form Designer generated code
		/// <summary>
		/// Raises OnInitEvent
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			//  - jminond
			// View item pages in general have no need for viewstate
			// Especailly big texts.
			this.Page.EnableViewState = false;

			base.OnInit(e);
		}

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
			this.dlComments.ItemCommand += new DataListCommandEventHandler(this.dlComments_ItemCommand);
			this.btnPostComment.Click += new EventHandler(this.btnPostComment_Click);
			this.Load += new EventHandler(this.Page_Load);
			if ( !this.IsCssFileRegistered("Mod_Blogs") )
				this.RegisterCssFile("Mod_Blogs");
		}
		#endregion

		
	}
}
