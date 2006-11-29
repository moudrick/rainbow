using System;
using System.Configuration;
//===============================================================================
	//
	//	Base Logic Layer
	//
	//	Rainbow.BLL.Utils
	//
	//
	//===============================================================================
	// Encapsulate resources -- it can come from anywhere
	//===============================================================================
namespace Rainbow.BLL.Utils
{
	/// <summary>
	/// Summary description for GlobalResources.
	/// </summary>
	[Obsolete("use Rainbow.Settings.Config")]
	public class GlobalResources
	{

		// jes1111 - moved to GlobalInternalStrings
//		/// <summary>
//		/// non breakable html space character
//		/// </summary>
//		public  const string HTML_SPACE = "&nbsp;";

		/// <summary>
		/// Does the Portal support WIndow Mgmt Functions/Controls
		/// </summary>
		public static bool SupportWindowMgmt
		{
			get 
			{
				return SafeBoolean("WindowMgmtControls", false);
			}
		} // end of SupportWindowMgmt

		/// <summary>
		/// Do we support the close button
		/// </summary>
		public static bool SupportWindowMgmtClose
		{
			get 
			{
				return SafeBoolean("WindowMgmtWantClose", false);
			}
		} // end of SupportWindowMgmtClose

		/// <summary>
		/// Get Boolean Resource
		/// </summary>
		/// <param name="name"></param>
		/// <param name="default_ret"></param>
		/// <returns></returns>
		public static bool SafeBoolean(string name,bool default_ret)
		{
			object obj = ConfigurationSettings.AppSettings[name] ;

			try 
			{

				if ( obj != null )
					return Convert.ToBoolean(obj);
			} 

			catch {}
			return default_ret;
		} // end of SafeBoolean

		/// <summary>
		/// Get Integer Resource
		/// </summary>
		/// <param name="name"></param>
		/// <param name="default_ret"></param>
		/// <returns></returns>
		public static int SafeInt(string name,int default_ret)
		{
			object obj = ConfigurationSettings.AppSettings[name] ;

			try 
			{

				if ( obj != null )
					return Int32.Parse((string)obj);
			} 

			catch {}
			return default_ret;
		} // end of SafeInt

		/// <summary>
		/// Get string Resource
		/// </summary>
		/// <param name="name"></param>
		/// <param name="default_ret"></param>
		/// <returns></returns>
		public static string SafeString(string name,string default_ret)
		{
			object obj = ConfigurationSettings.AppSettings[name] ;

			try 
			{

				if ( obj != null )
					return (string)obj;
			} 

			catch {}
			return default_ret;
		} // end of SafeString
	}
}
