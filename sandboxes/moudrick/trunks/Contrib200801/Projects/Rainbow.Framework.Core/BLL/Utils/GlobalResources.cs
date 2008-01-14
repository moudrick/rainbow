using System;
using System.Configuration;
using Rainbow.Framework.Context;

//===============================================================================
//
//	Base Logic Layer
//
//	Rainbow.Framework.BLL.Utils
//
//
//===============================================================================
// Encapsulate resources -- it can come from anywhere
//===============================================================================
namespace Rainbow.Framework.BLL.Utils
{
	/// <summary>
	/// Summary description for GlobalResources.
	/// </summary>
	[Obsolete("use Rainbow.Framework.Settings.Config")]
	public class GlobalResources
	{
		/// <summary>
		/// Does the Portal support WIndow Mgmt Functions/Controls
		/// </summary>
		/// <value><c>true</c> if [support window MGMT]; otherwise, <c>false</c>.</value>
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
		/// <value>
		/// 	<c>true</c> if [support window MGMT close]; otherwise, <c>false</c>.
		/// </value>
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
		/// <param name="name">The name.</param>
		/// <param name="default_ret">if set to <c>true</c> [default_ret].</param>
		/// <returns></returns>
		public static bool SafeBoolean(string name, bool default_ret)
		{
		    string value = RainbowContext.Current.GetAppSetting(name);
		    bool result;
		    return bool.TryParse(value, out result) ? result : default_ret;
		}

		/// <summary>
		/// Get Integer Resource
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="default_ret">The default_ret.</param>
		/// <returns></returns>
		public static int SafeInt(string name, int default_ret)
		{
			object obj = ConfigurationSettings.AppSettings[name];

			try
			{

				if (obj != null)
					return Int32.Parse((string)obj);
			}

			catch { }
			return default_ret;
		} // end of SafeInt

		/// <summary>
		/// Get string Resource
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="default_ret">The default_ret.</param>
		/// <returns></returns>
		public static string SafeString(string name, string default_ret)
		{
			object obj = ConfigurationSettings.AppSettings[name];

			try
			{

				if (obj != null)
					return (string)obj;
			}

			catch { }
			return default_ret;
		} // end of SafeString
	}
}
