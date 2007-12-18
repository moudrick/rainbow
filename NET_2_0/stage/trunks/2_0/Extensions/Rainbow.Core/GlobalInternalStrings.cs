 //===============================================================================
//
// Globalized Internal Strings (Portal Related )
//	
//
//===============================================================================
//
// This class holds commonly used internal strings. Place strings, constants, etc.
// here to better reuse and manage the data.
//===============================================================================

namespace Rainbow.Core
{
	/// <summary>
	/// Summary description for GlobalInternalStrings.
	/// </summary>
	public sealed class /* struct*/ GlobalInternalStrings
	{
		static GlobalInternalStrings()
		{
		}

		private static readonly string allUsers = "All Users";

		/// <summary>
		/// The role of all  users
		/// </summary>
		public static string AllUsers
		{
			get { return allUsers; }
		}


		private static readonly string admins = "Admins";

		/// <summary>
		/// The name of the admin role
		/// </summary>
		public static string Admins
		{
			get { return admins; }
		}


		private static readonly string roleDelimiter = ";";

		/// <summary>
		/// The Persistent/DB delimter
		/// </summary>
		public static string RoleDelimiter
		{
			get { return roleDelimiter; }
		}


		private static readonly string anonymous = "anonymous";

		/// <summary>
		/// Anonymous user name
		/// </summary>
		public static string Anonymous
		{
			get { return anonymous; }
		}


		private static readonly string portalPrefix = "Rainbow_";

		/// <summary>
		/// the portal prefix
		/// </summary>
		public static string PortalPrefix
		{
			get { return portalPrefix; }
		}


		private static readonly string userWinMgmtIndex = GlobalInternalStrings.PortalPrefix + "WinMgmt";

		/// <summary>
		/// the cookie id used for getting the uid ( dependency above !)
		/// </summary>
		public static string UserWinMgmtIndex
		{
			get { return userWinMgmtIndex; }
		}


		private static readonly string cookiePath = "/";

		/// <summary>
		/// the cookie path used for window informaton ( dependency above !)
		/// </summary>
		public static string CookiePath
		{
			get { return cookiePath; }
		}


		private static readonly string cVS = "CVS";

		/// <summary>
		/// the CVS directory
		/// </summary>
		public static string CVS
		{
			get { return cVS; }
		}


		private static readonly string leftPane = "leftpane";

		/// <summary>
		/// left pane
		/// </summary>
		public static string LeftPane
		{
			get { return leftPane; }
		}


		private static readonly string rightPane = "rightpane";

		/// <summary>
		/// right pane
		/// </summary>
		public static string RightPane
		{
			get { return rightPane; }
		}


		private static readonly string contentPane = "contentpane";

		/// <summary>
		/// context pane
		/// </summary>
		public static string ContentPane
		{
			get { return contentPane; }
		}


		private static readonly string headerPane = "headerpane";

		/// <summary>
		/// header pane [FUTURE?]
		/// </summary>
		public static string HeaderPane
		{
			get { return headerPane; }
		}


		private static readonly string footerPane = "footerpane";

		/// <summary>
		/// footer pane [FUTURE?]
		/// </summary>
		public static string FooterPane
		{
			get { return footerPane; }
		}


		private static readonly string[] currentPanes = {LeftPane, ContentPane, RightPane};

		/// <summary>
		/// current supported panes
		/// </summary>
		public static string[] CurrentPanes
		{
			get { return currentPanes; }
		}


	} // end of GlobalInternalStrings
}