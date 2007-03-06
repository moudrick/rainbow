using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Text;
using Rainbow.Data;

namespace Rainbow.Data.GentleNET
{
	/// <summary>
	/// Summary description for GentleNETDataProvider.
	/// </summary>
	public class GentleNETDataProvider : Rainbow.Data.DataProvider
	{
		#region Provider
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public GentleNETDataProvider()
		{}

		/// <summary>
		/// init
		/// </summary>
		/// <param name="name"></param>
		/// <param name="configValue"></param>
		public override void Initialize(string name, NameValueCollection configValue)
		{
		}
		#endregion
	}
}
