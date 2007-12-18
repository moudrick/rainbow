using System;
using Rainbow.Core;
using Rainbow.Data;

namespace Rainbow.BusinessRules
{
	/// <summary>
	/// Portals
	/// -------
	/// This class is derived from PortalArrayList and should contain
	/// an arraylist of Portal objects, and also implement any methods
	/// and properties specific to multiple instances of a Portal object.
	/// </summary>
	public class Portals : PortalArrayList, IDisposable
	{
		/// <summary>
		/// If true all users will be loaded from portal 0 instance
		/// </summary>
		public static bool UseSingleUserBase
		{
			get { return Helper.Portals.ConfigData.UseSingleUserBase; }
			set { Helper.Portals.ConfigData.UseSingleUserBase = value; }
		}

		private static readonly bool enableMultiDbSupport = Helper.Portals.ConfigData.EnableMultiDbSupport;
		//			(ConfigurationSettings.AppSettings["EnableMultiDbSupport"] == null
		//				: bool.Parse(ConfigurationSettings.AppSettings["EnableMultiDbSupport"]));
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static bool EnableMultiDbSupport
		{
			get { return enableMultiDbSupport; }
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static string DefaultPortal
		{
			get { return Helper.Portals.ConfigData.DefaultPortal; }
			set { Helper.Portals.ConfigData.DefaultPortal = value; }
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static bool CheckForFilePermissions
		{
			get { return Helper.Portals.ConfigData.CheckForFilePermissions; }
			set { Helper.Portals.ConfigData.CheckForFilePermissions = value; }
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static bool UseProxyServerForServerWebRequests
		{
			get { return Helper.Portals.ConfigData.UseProxyServerForServerWebRequests; }
			set { Helper.Portals.ConfigData.UseProxyServerForServerWebRequests = value; }
		}


		#region IDisposable Members

		public void Dispose()
		{
			// TODO:  Add Portals.Dispose implementation
		}

		#endregion
	}
}