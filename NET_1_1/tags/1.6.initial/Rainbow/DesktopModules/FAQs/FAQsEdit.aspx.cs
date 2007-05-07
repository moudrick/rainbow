//Namespaces added for editor and config settings
//Chris Farrell, 10/27/03, chris@cftechconsulting.com
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.UI;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;
using Literal = Esperantus.WebControls.Literal;
using RequiredFieldValidator = Esperantus.WebControls.RequiredFieldValidator;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// IBS Portal FAQ module - Edit page part
	/// (c)2002 by Christopher S Judd, CDP &amp; Horizons, LLC
	/// Moved into Rainbow by Jakob Hansen, hansen3000@hotmail.com
	/// </summary>
	public class FAQsEdit : EditItemPage
	{
		#region Declarations
		/// <summary>
		/// 
		/// </summary>
		protected TextBox Question;
		/// <summary>
		/// 
		/// </summary>
		protected RequiredFieldValidator RequiredFieldValidatorQuestion;
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
		protected Label CreatedBy;
		/// <summary>
		/// 
		/// </summary>
		protected Label CreatedDate;
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
		int itemID = -1;

		/*Editor added 10/27/03 by Chris Farrell, chris@cftechconsulting.com*/
		//protected System.Web.UI.WebControls.TextBox Answer;
		//protected Esperantus.WebControls.RequiredFieldValidator RequiredFieldValidatorAnswer;
		/// <summary>
		/// 
		/// </summary>
		protected PlaceHolder PlaceHolderHTMLEditor;
		/// <summary>
		/// 
		/// </summary>
		protected IHtmlEditor DesktopText;
		#endregion


		private void Page_Load(object sender, EventArgs e)
		{
			//Editor placeholder setup
			HtmlEditorDataType h = new HtmlEditorDataType();
			h.Value = moduleSettings["Editor"].ToString();
			DesktopText = h.GetEditor(PlaceHolderHTMLEditor, ModuleID, bool.Parse(moduleSettings["ShowUpload"].ToString()), portalSettings);

			DesktopText.Width = new Unit(moduleSettings["Width"].ToString());
			DesktopText.Height = new Unit(moduleSettings["Height"].ToString());


			//  Determine itemID of FAQ to Update
			if (Request.Params["itemID"] != null)
			{
				itemID = Int32.Parse(Request.Params["itemID"]);
			}

			//	populate with FAQ Details  
			if (Page.IsPostBack == false)
			{
				if (itemID != -1)
				{
					//  get a single row of FAQ info
					FAQsDB questions = new FAQsDB();
					SqlDataReader dr = questions.GetSingleFAQ(itemID);

					try
					{
						//  Read database
						dr.Read();
						Question.Text = (string) dr["Question"];
						//Answer.Text = (string) dr["Answer"];
						DesktopText.Text = (string) dr["Answer"];
						CreatedBy.Text = (string) dr["CreatedByUser"];
						CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
						// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
						if (CreatedBy.Text == "unknown" || CreatedBy.Text == string.Empty)
						{
							CreatedBy.Text = Localize.GetString("UNKNOWN", "unknown");
						}
					}
					finally
					{
						dr.Close();
					}
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
				al.Add("2502DB18-B580-4F90-8CB4-C15E6E531000");
				return al;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnUpdate(EventArgs e)
		{
			base.OnUpdate(e);

			// Don't Allow empty data
			if (Question.Text == string.Empty || DesktopText.Text == string.Empty)
				return;

			//  Update only if entered data is valid
			if (Page.IsValid == true)
			{
				FAQsDB questions = new FAQsDB();

				if (itemID == -1)
				{
					//  Add the question within the questions table
					questions.AddFAQ(ModuleID, itemID, PortalSettings.CurrentUser.Identity.Email, Question.Text, DesktopText.Text);
				}
				else
				{
					//  Update the question within the questions table
					questions.UpdateFAQ(ModuleID, itemID, PortalSettings.CurrentUser.Identity.Email, Question.Text, DesktopText.Text);
				}

				this.RedirectBackToReferringPage();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnDelete(EventArgs e)
		{
			base.OnDelete(e);

			//  Only attempt to delete the item if it is an existing item
			//  (new items will have "itemID" of -1)
			if (itemID != -1)
			{
				FAQsDB questions = new FAQsDB();
				questions.DeleteFAQ(itemID);
			}
			this.RedirectBackToReferringPage();
		}

		#region Web Form Designer generated code

		/// <summary>
		/// Raises OnInitEvent
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
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

		}

		#endregion
	}
}
