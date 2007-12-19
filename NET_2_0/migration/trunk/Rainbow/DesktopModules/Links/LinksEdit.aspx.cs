// Change by Geert.Audenaert@Syntegra.Com
// Date: 7/2/2003
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.UI;
using CompareValidator = Esperantus.WebControls.CompareValidator;
using Literal = Esperantus.WebControls.Literal;
using RequiredFieldValidator = Esperantus.WebControls.RequiredFieldValidator;
// End Change Geert.Audenaert@Syntegra.Com

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// 
	/// </summary>
	[History("jminond", "2004/04/5", "Cleaned up using methods")]
	[History("CIsakson", "2003/03/10", "Added Target Field")]
	[History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
	public class LinksEdit : AddEditItemPage
	{
		#region Declarations
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
		protected TextBox TitleField;
		/// <summary>
		/// 
		/// </summary>
		protected RequiredFieldValidator Req1;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal3;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox UrlField;
		/// <summary>
		/// 
		/// </summary>
		protected RequiredFieldValidator Req2;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal4;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox MobileUrlField;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal5;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox DescriptionField;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal6;
		/// <summary>
		/// 
		/// </summary>
		protected DropDownList TargetField;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal7;
		/// <summary>
		/// 
		/// </summary>
		protected TextBox ViewOrderField;
		/// <summary>
		/// 
		/// </summary>
		protected RequiredFieldValidator RequiredViewOrder;
		/// <summary>
		/// 
		/// </summary>
		protected CompareValidator VerifyViewOrder;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal8;
		/// <summary>
		/// 
		/// </summary>
		protected Label CreatedBy;
		/// <summary>
		/// 
		/// </summary>
		protected Literal Literal9;
		/// <summary>
		/// 
		/// </summary>
		protected Label CreatedDate;
		#endregion

		/// <summary>
		/// The Page_Load event on this Page is used to obtain the 
		/// ItemID of the link to edit.
		/// It then uses the Rainbow.LinkDB() data component
		/// to populate the page's edit controls with the links details.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			// If the page is being requested the first time, determine if an
			// link itemID value is specified, and if so populate page
			// contents with the link details

			if (Page.IsPostBack == false)
			{
				TargetField.Items.Add("_new");
				TargetField.Items.Add("_blank");
				TargetField.Items.Add("_parent");
				TargetField.Items.Add("_self");
				TargetField.Items.Add("_top");

				if (ItemID != 0)
				{
					// Obtain a single row of link information
					LinkDB links = new LinkDB();
					SqlDataReader dr = links.GetSingleLink(ItemID, WorkFlowVersion.Staging);

					try
					{
						// Read in first row from database
						if (dr.Read())
						{
							TitleField.Text = dr["Title"].ToString();
							DescriptionField.Text = dr["Description"].ToString();
							UrlField.Text = dr["Url"].ToString();
							MobileUrlField.Text = dr["MobileUrl"].ToString();
							ViewOrderField.Text = dr["ViewOrder"].ToString();
							CreatedBy.Text = dr["CreatedByUser"].ToString();
							CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
							TargetField.Items.FindByText((string) dr["Target"]).Selected = true;
							// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
							if (CreatedBy.Text == "unknown")
							{
								CreatedBy.Text = Localize.GetString("UNKNOWN", "unknown");
							}
						}
					}
					finally
					{
						// Close datareader
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
				al.Add("476CF1CC-8364-479D-9764-4B3ABD7FFABD");
				return al;
			}
		}

		/// <summary>
		/// The UpdateBtn_Click event handler on this Page is used to either
		/// create or update a link.  It  uses the Rainbow.LinkDB()
		/// data component to encapsulate all data functionality.
		/// </summary>
		override protected void OnUpdate(EventArgs e)
		{
			base.OnUpdate(e);

			if (Page.IsValid == true)
			{
				// Create an instance of the Link DB component
				LinkDB links = new LinkDB();

				if (ItemID == 0)
				{
					// Add the link within the Links table
					links.AddLink(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, TitleField.Text, UrlField.Text, MobileUrlField.Text, Int32.Parse(ViewOrderField.Text), DescriptionField.Text, TargetField.SelectedItem.Text);
				}
				else
				{
					// Update the link within the Links table
					links.UpdateLink(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, TitleField.Text, UrlField.Text, MobileUrlField.Text, Int32.Parse(ViewOrderField.Text), DescriptionField.Text, TargetField.SelectedItem.Text);
				}

				// Redirect back to the portal home page
				this.RedirectBackToReferringPage();
			}
		}

		/// <summary>
		/// The DeleteBtn_Click event handler on this Page is used to delete 
		/// a link.  It  uses the Rainbow.LinksDB()
		/// data component to encapsulate all data functionality.
		/// </summary>
		override protected void OnDelete(EventArgs e)
		{
			base.OnDelete(e);

			// Only attempt to delete the item if it is an existing item
			// (new items will have "ItemID" of 0)
			if (ItemID != 0)
			{
				LinkDB links = new LinkDB();
				links.DeleteLink(ItemID);
			}

			// Redirect back to the portal home page
			this.RedirectBackToReferringPage();
		}

		#region Web Form Designer generated code

		/// <summary>
		/// Raises OnInitEvent
		/// </summary>
		/// <param name="e"></param>
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

		}

		#endregion
	}
}
