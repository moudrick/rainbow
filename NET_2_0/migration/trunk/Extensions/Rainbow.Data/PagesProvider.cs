using System;
using System.Collections;
using Rainbow.Core;

namespace Rainbow.Data
{
	public abstract class PagesProvider : DataProvider
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected PagesProvider()
		{
		}

		/// <summary>
		///     Gets a Page object based on the page id #
		/// </summary>
		/// <param name="pageId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Page value...
		/// </returns>
		public abstract Page Page(int pageId);

		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		public abstract int Add(int portalID, String tabName, int tabOrder);

		/// <summary>
		/// The AddTab method adds a new tab to the portal.<br />
		/// AddTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabName"></param>
		/// <param name="roles"></param>
		/// <param name="tabOrder"></param>
		/// <returns></returns>
		public abstract int Add(int portalID, String tabName, string roles, int tabOrder);

		/// <summary>
		/// The DeleteTab method deletes the selected tab from the portal.<br />
		/// DeleteTab Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		public abstract void Remove(int tabID);

		/// <summary>
		/// Update Method<br />
		/// was UpdateTab Stored Procedure
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="tabID"></param>
		/// <param name="parentTabID"></param>
		/// <param name="tabName"></param>
		/// <param name="tabOrder"></param>
		/// <param name="authorizedRoles"></param>
		/// <param name="mobileTabName"></param>
		/// <param name="showMobile"></param>
		public abstract void Update(int portalID, int tabID, int parentTabID,
		                            String tabName, int tabOrder, String authorizedRoles,
		                            String mobileTabName, bool showMobile);

		/// <summary>
		/// The UpdateTabOrder method changes the position of the tab with respect
		/// to other tabs in the portal.<br />
		/// UpdateTabOrder Stored Procedure
		/// </summary>
		/// <param name="tabID"></param>
		/// <param name="tabOrder"></param>
		public abstract void UpdateOrder(int tabID, int tabOrder);

		/// <summary>
		/// This user control will render the breadcrumb navigation for the current tab.
		/// Ver. 1.0 - 24. dec 2002 - First realase by Cory Isakson
		/// </summary>
		/// <param name="tabID">ID of the tab</param>
		/// <returns></returns>
		public abstract ArrayList Crumbs(int tabID);

		/// <summary>
		///     
		/// </summary>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public abstract Hashtable Settings(int tabID);
	}
}