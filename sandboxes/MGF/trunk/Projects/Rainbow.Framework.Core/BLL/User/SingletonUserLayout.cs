using System;
using Rainbow.Framework.BLL.Utils;
//===============================================================================
//
//	Business Logic Layer
//
//	Rainbow.Framework.BLL.User
//
// Singleton Pattern used for getting user layout
//
//===============================================================================
//
// Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
namespace Rainbow.Framework.BLL.User
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public struct UserFrameAppearanceStruct
	{

		/// <summary>
		/// 
		/// </summary>
		public string MinimizeColor;

		/// <summary>
		/// 
		/// </summary>
		public string FontColor;

		/// <summary>
		/// 
		/// </summary>
		public string FontSize;

		/// <summary>
		/// 
		/// </summary>
		public string Font;
	};

	/// <summary>
	/// Summary description for SingletonUserLayout. This provides
	/// a fallback to resources if not found from persistant store
	/// </summary>
	[Obsolete("not used?")]
	public sealed class SingletonUserLayout
	{
		/// <summary>
		/// Data Section
		/// </summary>
		UserFrameAppearanceStruct usr_app_;

		/// <summary>
		/// Frame font size
		/// </summary>
		/// <value>The size of the module font.</value>
		public string ModuleFontSize
		{
			get { return usr_app_.FontSize; }
		}

		/// <summary>
		/// Frame font
		/// </summary>
		/// <value>The module font.</value>
		public string ModuleFont
		{
			get { return usr_app_.Font; }
		}

		/// <summary>
		/// Frame font color
		/// </summary>
		/// <value>The color of the module font.</value>
		public string ModuleFontColor
		{
			get { return usr_app_.FontColor; }
		}

		//
		/// <summary>
		/// Frame minimize color
		/// </summary>
		/// <value>The color of the module minimize.</value>
		public string ModuleMinimizeColor
		{
			get { return usr_app_.MinimizeColor; }
		}

		/// <summary>
		/// Get the user appeareance
		/// </summary>
		/// <value>The user appearance.</value>
		public UserFrameAppearanceStruct UserAppearance
		{
			get { return usr_app_; }
		}

		/// <summary>
		/// the singleton class instance (ONLY ONE -- APPLICATION LEVEL )
		/// </summary>
		public static readonly SingletonUserLayout instance = new SingletonUserLayout();

		/// <summary>
		/// the singleton class
		/// </summary>
		private SingletonUserLayout()
		{
			// set the default properties -- extracting the information from the config
			// file if present, otherwise use the default
			usr_app_.FontColor = GlobalResources.SafeString("ModuleFontColor", "#ffffff");
			usr_app_.FontSize = GlobalResources.SafeString("ModuleFontSize", "10");
			usr_app_.Font = GlobalResources.SafeString("ModuleFont", "Verdana");
			usr_app_.MinimizeColor = GlobalResources.SafeString("MinimizeColor", "#000000");
		} // end of ctor
	} // end of SingletonUserLayout
}
