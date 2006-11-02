using System;
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;

using Rainbow.Helpers;
using Rainbow.UI.WebControls;


namespace Rainbow.DesktopModules 
{
	/// <summary>
	/// Basic links module
	/// </summary>
	public class Links : PortalModuleControl 
	{
		#region MDF Init
		/// <summary>The name of the core item table for the module.</summary>
		public const string mdfItemTableName = "rb_Links";
		/// <summary>
		/// The name of the field in the core item table that
		/// is considered the item title. Typical value is "Title".
		/// </summary>
		public const string mdfTitleFieldName = "Title";
		/// <summary>
		/// The list of fields in the SQL select. Please separate with comma and prefix with "itm.".
		/// You can get this list from the sproc GetXXXX that feeds data to this module.
		/// </summary>
		public const string mdfSelectFieldList = "itm.ItemID,itm.CreatedByUser,itm.CreatedDate,itm.Title,itm.Url,itm.ViewOrder,itm.Description,itm.Target";
		/// <summary>All fields from the core item table you want to sort on.</summary>
		public const string mdfSortFieldList  = "ModuleID;CreatedByUser;CreatedDate;Title;Url;ViewOrder;Description;Target";
		/// <summary>A single field from mdfSortFieldList.</summary>
		public const string mdfDefaultSortField = "ViewOrder";
		/// <summary>Fields to search - must be nvarchar or ntext type.</summary>
		public const string mdfSearchFieldList = "Title;Url;MobileUrl;Description;CreatedByUser";
		#endregion

		/// <summary>
		/// Datalist of links
		/// </summary>
		protected DataList myDataList;
		/// <summary>
		/// link image
		/// </summary>
		protected string linkImage = string.Empty;
		/// <summary>
		/// link text
		/// </summary>
		protected string linkTextKey = string.Empty;
		/// <summary>
		/// link tooltip
		/// </summary>
		protected string linkAlternateText = string.Empty;

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
		/// The Page_Load event handler on this User Control is used to
		/// obtain a DataReader of link information from the Links
		/// table, and then databind the results to a templated DataList
		/// server control.  It uses the Rainbow.LinkDB()
		/// data component to encapsulate all data functionality.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e) 
		{
			// Set the link image type
			if (IsEditable) 
			{
				Image myImage = portalSettings.GetCurrentTheme().GetImage("Buttons_Edit", "edit.gif");
				linkImage = myImage.ImageUrl;
				linkTextKey="EDIT_THIS_ITEM";
				linkAlternateText="Edit this item";
			}
			else 
			{
				Image myImage = portalSettings.GetCurrentTheme().GetImage("NavLink", "navlink.gif");
				linkImage = myImage.ImageUrl;
				linkTextKey=string.Empty;
				linkAlternateText=string.Empty;
			}

			// Obtain links information from the Links table and bind to the datalist control
			if (MDFSettings.IsMDFApplied(this))
			{
				MDFSettings mdfSet = new MDFSettings(this, mdfItemTableName, mdfTitleFieldName, mdfSelectFieldList, mdfSearchFieldList);
				myDataList.DataSource = mdfSet.GetDataReader(); 
			}
			else
			{
				LinkDB links = new LinkDB();
				myDataList.DataSource = links.GetLinks(ModuleID, Version);
			}
			myDataList.DataBind();
		}
   
		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemID"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		/// <remarks>
		/// Date: 10/2/2003
		/// Change by Geert.Audenaert@Syntegra.Com 
		/// </remarks>
		protected string GetLinkUrl(object itemID, object url)
		{
			if (IsEditable)
			{
				return HttpUrlBuilder.BuildUrl("~/DesktopModules/Links/LinksEdit.aspx","ItemID=" + itemID.ToString() + "&mID=" + ModuleID.ToString());
			}
			else
			{
				return url.ToString();
			}
		}
		// End Change Geert.Audenaert@Syntegra.Com
   
		/// <summary>
		/// 
		/// </summary>
		public Links()
		{
			// Change by Geert.Audenaert@Syntegra.Com
			// Date: 27/2/2003
			SupportsWorkflow = true;
			// End Change Geert.Audenaert@Syntegra.Com

			#region MDF Settings
			// Note: dont modify the code here! Please set mdfXXX consts in top of this file
			this._baseSettings.Add(MDFSettings.NameApplyMDF,      MDFSettings.MakeApplyMDF(MDFSettings.DefaultValueApplyMDF));
			this._baseSettings.Add(MDFSettings.NameDataSource,    MDFSettings.MakeDataSource(MDFSettings.DefaultValueDataSource));
			this._baseSettings.Add(MDFSettings.NameMaxHits,       MDFSettings.MakeMaxHits(MDFSettings.DefaultValueMaxHits));
			this._baseSettings.Add(MDFSettings.NameModuleList,    MDFSettings.MakeModuleList(MDFSettings.DefaultValueModuleList));
			this._baseSettings.Add(MDFSettings.NameAllNotInList,  MDFSettings.MakeAllNotInList(MDFSettings.DefaultValueAllNotInList));
			this._baseSettings.Add(MDFSettings.NameSortField,     MDFSettings.MakeSortFieldList(mdfDefaultSortField, mdfSortFieldList));
			this._baseSettings.Add(MDFSettings.NameSortDirection, MDFSettings.MakeSortDirection(MDFSettings.DefaultValueSortDirection));
			this._baseSettings.Add(MDFSettings.NameSearchString,  MDFSettings.MakeSearchString(MDFSettings.DefaultValueSearchString));
			this._baseSettings.Add(MDFSettings.NameSearchField,   MDFSettings.MakeSearchFieldList(mdfSearchFieldList));
			this._baseSettings.Add(MDFSettings.NameMobileOnly,    MDFSettings.MakeMobileOnly(MDFSettings.DefaultValueMobileOnly));
			#endregion
		}
   
		/// <summary>
		/// Module Guid
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{476CF1CC-8364-479D-9764-4B3ABD7FFABD}");
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
			SearchDefinition s = new SearchDefinition("rb_Links", "Title", "Description", "CreatedByUser", "CreatedDate", searchField);
			
			//Add extra search fields here, this way
			s.ArrSearchFields.Add("itm.Url");
			
			return s.SearchSqlSelect(portalID, userID, searchString);
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Raises Init event
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
        {
            InitializeComponent();

			// View state is not needed here, so we are disabling. - jminond
			this.myDataList.EnableViewState = false;

			this.AddUrl = "~/DesktopModules/Links/LinksEdit.aspx";
			base.OnInit(e);
        }

        /// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion

		# region Install / Uninstall Implementation
		/// <summary>
		/// Install
		/// </summary>
		/// <param name="stateSaver"></param>
		public override void Install(IDictionary stateSaver)
		{
			string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
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
			string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}
		#endregion


	}
}
