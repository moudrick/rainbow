using System;
using System.Collections;
using System.Data.SqlClient;

namespace Rainbow.Configuration
{

	/// <summary>
	/// Class that encapsulates all data logic necessary to add/query/delete
	/// Portals within the Portal database.
	/// </summary>
	[History("jminond", "2005/03/10", "Tab to page conversion")]
	public class TabsDB
	{
		const string strAllUsers = "All Users;";
		const string strPortalID = "@PortalID";
		const string strPageID = "@TabID";

		// New const for new method AddTab defaults
		// Mike Stone 30/12/2004
		const int intParentPageID = 0;  //SP will convert to NULL if 0
		const bool boolShowMobile = false;
		const string strMobileTabName = ""; // NULL NOT ALLOWED IN TABLE.


		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		[Obsolete("Please use pagesDB")]
		public int AddTab(int portalID, string tabName, int tabOrder) 
		{
			return new PagesDB().AddPage(portalID, tabName, strAllUsers, tabOrder);
		}

		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="Roles"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		[Obsolete("Please use pagesDB")]
		public int AddTab(int portalID, string tabName, string Roles, int tabOrder) 
		{
			// Change Method to use new all parms method below
			// SP call moved to new method AddTab below.
			// Mike Stone - 30/12/2004
			return new PagesDB().AddPage(portalID, intParentPageID, tabName, tabOrder, strAllUsers, boolShowMobile, strMobileTabName);
		}

		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="parentPageID"></param> 
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <param name="authorizedRoles"></param>
		/// <param name="showMobile"></param>
		/// <param name="mobileTabName"></param>
		/// <returns></returns>
		[Obsolete("Please use pagesDB")]
		public int AddTab(int portalID, int parentPageID, string tabName, int tabOrder, string authorizedRoles, bool showMobile, string mobileTabName) 
		{

			return new PagesDB().AddPage(portalID, parentPageID, tabName, tabOrder, authorizedRoles, showMobile, mobileTabName);
		}


		/// <summary>
		/// The DeleteTab method deletes the selected tab from the portal.<br />
		/// DeleteTab Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		[Obsolete("Please use pagesDB")]
		public void DeleteTab(int tabID) 
		{
			new PagesDB().DeletePage(tabID);
		}

		/// <summary>
		/// This user control will render the breadcrumb navigation for the current tab.
		/// Ver. 1.0 - 24. dec 2002 - First realase by Cory Isakson
		/// </summary>
		/// <param name="tabID">ID of the tab</param>
		/// <returns></returns>
		[Obsolete("Please use pagesDB")]
		public ArrayList GetTabCrumbs(int tabID) 
		{
			return new PagesDB().GetPageCrumbs(tabID);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		[Obsolete("Please use pagesDB")]
		public SqlDataReader GetTabsByPortal(int portalID) 
		{
			return new PagesDB().GetPagesByPortal(portalID);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.ArrayList value...
		/// </returns>
		[Obsolete("Please use pagesDB")]
		public ArrayList GetTabsFlat(int portalID) 
		{

			return new PagesDB().GetPagesFlat(portalID);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.ArrayList value...
		/// </returns>
		[Obsolete("Please use pagesDB")]
		public ArrayList GetTabsinTab(int portalID, int tabID) 
		{
			ArrayList DesktopTabs;
			DesktopTabs = new PagesDB().GetPagesinPage(portalID, tabID);
			return DesktopTabs;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		[Obsolete("Please use pagesDB")]
		public SqlDataReader GetTabsParent(int portalID, int tabID) 
		{
			return new PagesDB().GetPagesParent(portalID, tabID);
		}

		/// <summary>
		/// UpdateTab Method<br />
		/// UpdateTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabID"></param>
		/// <param name="parentPageID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <param name="authorizedRoles"></param>
		/// <param name="mobileTabName"></param>
		/// <param name="showMobile"></param>
		[Obsolete("Please use pagesDB")]
		public void UpdateTab (int portalID, int tabID, int parentPageID, string tabName, int tabOrder, string authorizedRoles, string mobileTabName, bool showMobile) 
		{
			new PagesDB().UpdatePage(portalID, tabID, parentPageID, tabName, tabOrder, authorizedRoles, mobileTabName, showMobile);
		}

		/// <summary>
		/// Update Tab Custom Settings
		/// </summary>
		/// <param name="tabID"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		[Obsolete("Please use PageSettings.UpdatePageSettings")]
		public void UpdateTabCustomSettings(int tabID, string key, string value) 
		{
			PageSettings.UpdatePageSettings(tabID, key, value);
		}

		/// <summary>
		/// The UpdateTabOrder method changes the position of the tab with respect
		/// to other tabs in the portal.<br />
		/// UpdateTabOrder Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		/// <param name="tabOrder"></param>
		[Obsolete("Please use pagesDB")]
		public void UpdateTabOrder (int tabID, int tabOrder) 
		{

			new PagesDB().UpdatePageOrder(tabID, tabOrder);
		}
	}
}
