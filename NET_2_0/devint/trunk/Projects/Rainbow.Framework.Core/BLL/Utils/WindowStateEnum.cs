using System;
//===============================================================================
//
//	Business Logic Layer
//
//	Rainbow.Framework.BLL.Utils
//
//
// Encapsulates the common information for user desktop
//
//===============================================================================
//
// Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
namespace Rainbow.Framework.BLL.Utils
{
	/// <summary>
	/// The states a window can be in. Information is later persisted to
	/// the database
	/// </summary>
	// This state information persists in the rb_UserConfiguration[db]
	[Serializable]
	public enum WindowStateEnum : short
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		None = 0,
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		Closed = 1,
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		Open = 2,
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		Minimized = 3,
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		Maximized = 4
	} // end of WindowStateEnum

	/// <summary>
	/// Common Strings used for the window state
	/// </summary>
	public struct WindowStateStrings
	{

		/// <summary>
		///     User these names for localization lookup 
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static readonly string ButtonMinLocalized = "SWI_BUTTON_MIN";

		/// <summary>
		///     User these names for localization lookup 
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static readonly string ButtonMaxLocalized = "SWI_BUTTON_MAX";

		/// <summary>
		///     User these names for localization lookup 
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static readonly string ButtonClosedLocalized = "DELETE";

		/// <summary>
		///     use these names for theme lookup and attribute
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static readonly string ButtonMinName = "Buttons_Min";

		/// <summary>
		///     use these names for theme lookup and attribute
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static readonly string ButtonMaxName = "Buttons_Max";

		/// <summary>
		///     use these names for theme lookup and attribute
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static readonly string ButtonCloseName = "Buttons_Close";
	} // end of WindowStateStrings
}
