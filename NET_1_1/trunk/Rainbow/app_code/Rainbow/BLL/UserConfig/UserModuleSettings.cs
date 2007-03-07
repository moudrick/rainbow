using System;
using Rainbow.BLL.Utils;
//===============================================================================
	//
	//	 Business Logic Layer
	//
	//
	//
	// Encapsulates the detailed settings for a specific Module
	//
	//===============================================================================
	//
	// Created By : bja@reedtek.com Date: 26/04/2003
	//===============================================================================
namespace Rainbow.BLL.UserConfig
{
	/// <summary>
	/// *********************************************************************
	/// 
	/// UserModuleSettings Class
	/// 
	/// Class that encapsulates the detailed settings for a specific Module 
	/// in the Portal for the user
	/// 
	/// *********************************************************************
	/// </summary>
	[Serializable()]  
	public sealed class UserModuleSettings 
	{

		#region Public Data

		/// <summary>
		/// 
		/// </summary>
		public int				ModuleID;

		/// <summary>
		/// 
		/// </summary>
		public int				PageID;

		/// <summary>
		/// 
		/// </summary>
		public WindowStateEnum  State;

		#endregion

		#region Public Ctors

		/// <summary>
		/// ctor
		/// </summary>
		public UserModuleSettings(int moduleID,WindowStateEnum state,int pageID ) 
		{
			ModuleID		= moduleID;
			State			= state;
			PageID			= pageID;
		} // end of ctor

		/// <summary>
		/// ctor
		/// </summary>
		public UserModuleSettings(int moduleID) : this(moduleID,WindowStateEnum.Open,-1)
		{
		} // end of ctor

		/// <summary>
		/// ctor
		/// </summary>
		public UserModuleSettings() : this(-1,WindowStateEnum.Open, -1)
		{
		} // end of ctor

		#endregion

	} // end of UserModuleSettings
}
