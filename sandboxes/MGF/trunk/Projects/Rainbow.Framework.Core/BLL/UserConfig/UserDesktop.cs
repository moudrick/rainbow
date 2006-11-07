using System;
using System.Web;
using Rainbow.Framework.BLL.Base;
using Rainbow.Framework.BLL.User;
using Rainbow.Framework.BLL.Utils;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Site.Data;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Users.Data;
using Rainbow.Framework.Security;
using System.Web.Security;
//===============================================================================
//
//	 Business Logic Layer
//
//
//
// Class to manage the User's Desktop
//
// At Some point this may change to a DB implementation. Where the bag will
// be results returned back from a database. At this time it uses cookies to
// persist the data until the signed in user saves changes There are no ways 
// that I know of to ask if the session is cookieless w/o a roundtrip call
// ( set/get cookie). So this may change.
// 
//===============================================================================
//
// Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
namespace Rainbow.Framework.BLL.UserConfig
{
	/// <summary>
	/// *********************************************************************
	/// 
	/// UserDesktop Class
	/// 
	/// Class that encapsulates the detailed settings for a User Desktop
	/// 
	/// *********************************************************************
	/// </summary>
	public class UserDesktop : BLLBase
	{

		#region Static Methods

		#region Public Static Methods

		/// <summary>
		/// All the user windows are stored in a  hash table
		/// with the userid as the key and the value as another hashtable of modules. (key/value pair)
		/// </summary>
		/// <value>The user windows.</value>
		/// All these methods are static as the holder of information is as well.
		/// This means this is held for ALL users. Thus, it could be used to get
		/// a list of users onlined, what they are working on, where they have been, etc.
		/// For now, it just holds state on their desktop
		public static UserWindowMgmt UserWindows
		{
			get
			{
				HttpContext ctx = HttpContext.Current;

                UsersDB users = new UsersDB();

				// User Information
                MembershipUser user = users.GetSingleUser( ctx.User.Identity.Name );
				// get user id
				return FindUserWindows(ref user);
			}
		} // end of UserWindows

        /// <summary>
        /// Is this module id present
        /// </summary>
        /// <param name="module_id">The module_id.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified module_id]; otherwise, <c>false</c>.
        /// </returns>
		public static bool Contains(int module_id)
		{
			UserWindowMgmt uwm = UserWindows;
			return uwm != null ? uwm.Contains(module_id) : false;
		} // end of Contains

		/// <summary>
		/// update the desktop
		/// </summary>
		/// <param name="module_id">The module_id.</param>
		/// <param name="state">The state.</param>
		/// <param name="pageID">The page ID.</param>
		public static void UpdateUserDesktop(int module_id, WindowStateEnum state, int pageID)
		{
			// get the known user windows
			UserWindowMgmt ulmgr = UserWindows;

			// valid information
			if (ulmgr == null)
				return;
			UserModuleSettings ums = (UserModuleSettings)ulmgr[module_id];

			// If we are going into a max. state
			// then all we need to do is remove ourselves
			// from the list as the default is maximized.
			if (ums != null && state == WindowStateEnum.Maximized)
			{
				ulmgr.Remove(module_id);
				// if not present then add
			}
			else if (ums == null)
			{
				// new setting
				ums = new UserModuleSettings(module_id, state, pageID);
				ulmgr.Add(ums);
			}

			else
			{
				// set the new state for this entry
				ums.State = state;
				ums.ModuleID = module_id;
				ums.PageID = pageID;
			}
		} // end of UpdateUserDesktop

		/// <summary>
		/// is the window/module closed
		/// </summary>
		/// <param name="module_id">The module_id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified module_id is closed; otherwise, <c>false</c>.
		/// </returns>
		public static bool isClosed(int module_id)
		{
			return isState(module_id, WindowStateEnum.Closed);
		} // end of isClosed

		/// <summary>
		/// is the window/module minimized
		/// </summary>
		/// <param name="module_id">The module_id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified module_id is minimized; otherwise, <c>false</c>.
		/// </returns>
		public static bool isMinimized(int module_id)
		{
			return isState(module_id, WindowStateEnum.Minimized);
		} // end of isMinimized

		/// <summary>
		/// is the window maximized
		/// </summary>
		/// <param name="module_id">The module_id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified module_id is maximized; otherwise, <c>false</c>.
		/// </returns>
		public static bool isMaximized(int module_id)
		{
			return isState(module_id, WindowStateEnum.Maximized);
		} // end of isMaximized

		/// <summary>
		/// Get the user configuraton
		/// </summary>
		/// <param name="userid">The userid.</param>
		/// <param name="portalID">The portal ID.</param>
		public static void ConfigureDesktop(Guid userid, int portalID)
		{
			// open db
			UserWinMgmtDB dsktp_db = new UserWinMgmtDB();
			UserLayoutMgr ulm = dsktp_db.GetUserDesktop(userid, portalID);

			// iterate over user's layout
			foreach (UserModuleSettings ums in ulm.Modules)
			{
				// update the user desktop
				UpdateUserDesktop(ums.ModuleID, ums.State, ums.PageID);
			}
		} // end of ConfigureDesktop

		/// <summary>
		/// Reset the user's settings as they may have changed
		/// since we last retrieved them
		/// </summary>
		/// <param name="userid">The userid.</param>
		/// <param name="portalID">The portal ID.</param>
		/// <returns></returns>
		public static bool ResetPersistentDesktop(Guid userid, int portalID)
		{
			// open db
			UserWinMgmtDB dsktp_db = new UserWinMgmtDB();
			return dsktp_db.DeleteUserDeskTop(userid, portalID);
		} // end of ResetPersistentDesktop

		/// <summary>
		/// clear ALL entries from the user windows
		/// </summary>
		/// <param name="uid">The uid.</param>
		public static void ResetDesktop(Guid uid)
		{

			using (UserWindowMgmt uwm = UserDesktop.FindUserWindows(uid))
			{

				if (uwm != null)
					uwm.Modules.Clear();
			}
		} // end of ResetDesktop

		/// <summary>
		/// Save the user desktop
		/// </summary>
		public static void SaveUserDesktop()
		{
            // Obtain PortalSettings from Current Context
            PortalSettings portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

			HttpContext ctx = HttpContext.Current;

			// User Information
            
            /// TODO que pasa con el portalID en un sitio multiportal??????
            UsersDB users = new UsersDB();
            MembershipUser user = users.GetSingleUser( ctx.User.Identity.Name );

			// valida user?
			if (ctx.Request.IsAuthenticated && (!user.ProviderUserKey.Equals(Guid.Empty)) && user.Email.Length > 0)
				// save the desktop window configuration
				SaveUserState();
		} // end of SaveUserDesktop

		#endregion

		#region Private Static Methods

		/// <summary>
		/// Find the User Window Information based on user id
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
        private static UserWindowMgmt FindUserWindows( ref MembershipUser user )
		{
			// find user's information
			return FindUserWindows((Guid)user.ProviderUserKey);
		} // end of FindUserWindows

		/// <summary>
		/// Find the User Window Information based on user id
		/// </summary>
		/// <param name="uid">The uid.</param>
		/// <returns></returns>
		private static UserWindowMgmt FindUserWindows(Guid uid)
		{
			//HttpContext ctx =  HttpContext.Current;
			// get user id
			string userid = uid.ToString();

			// if there is no user (anonymous)
			if (uid != Guid.Empty)
			{
				// need data bag for the session
				IWebBagHolder bag = BagFactory.instance.create(BagFactory.BagFactoryType.CookieType);
				// set the user id information
				userid = (string)bag[GlobalInternalStrings.UserWinMgmtIndex];
			}
			// get the user's window information
			return new UserWindowMgmt(userid);
		} // end of FindUserWindows

		///////////////////////////////////////////////////////////////////////////
		//  IMPLEMENTATION LEVEL
		//  
		///////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Is a certain state active or disabled
		/// </summary>
		/// <param name="module_id">The module_id.</param>
		/// <param name="state">The state.</param>
		/// <returns>
		/// 	<c>true</c> if the specified module_id is state; otherwise, <c>false</c>.
		/// </returns>
		private static bool isState(int module_id, WindowStateEnum state)
		{
			UserWindowMgmt ulmgr = UserWindows;
			UserModuleSettings ums = ulmgr[module_id];
			return (ums != null) ? ulmgr[module_id].State == state : false;
		} // end of isState

		/// <summary>
		/// Persist the user window/modules settings
		/// </summary>
		private static void SaveUserState()
		{
			// Obtain PortalSettings from Current Context
			PortalSettings portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			
            // User Information
            /// TODO que pasa con el portalID en un sitio multiportal??????
            UsersDB users = new UsersDB();
            MembershipUser user = users.GetSingleUser( HttpContext.Current.User.Identity.Name );

			// get the windows, if any
			UserWindowMgmt uwm = UserWindows;

			// valid user
            if ( ( !user.ProviderUserKey.Equals( Guid.Empty ) ) && uwm != null )
			{
				// portal id
				int portalID = portalSettings.PortalID;
				// open db
				UserWinMgmtDB dsktp_db = new UserWinMgmtDB();

				// before we add the new settings we need to delete
				// the old ones.
				if (ResetPersistentDesktop((Guid)user.ProviderUserKey, portalID) == true)
				{

					// iterate through  controls and persist
					foreach (UserModuleSettings ms in uwm.Modules.Values)
					{

						if (ms.State != WindowStateEnum.Maximized)
							dsktp_db.SaveUserDesktop(portalID
								, (Guid)user.ProviderUserKey
								, ms.ModuleID
								, ms.PageID
								, ms.State);
					}
				}
			} // end if 
		} // SaveUserState

		#endregion

		#endregion

	} // end of UserDesktop
}
