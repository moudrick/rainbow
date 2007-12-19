using System.Collections;
using Rainbow.Core;

namespace Rainbow.Data
{
	/// <summary>
	/// Summary description for Portals.
	/// </summary>
	public abstract class PortalsProvider : DataProvider
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected PortalsProvider()
		{
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Rainbow.Core.Portal value...
		/// </returns>
		public abstract Portal Portal(int portalId);

		/// <summary>
		///     Get a portal by its alias
		/// </summary>
		/// <param name="portalAlias" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Portal value...
		/// </returns>
		public abstract Portal Portal(string portalAlias);

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public abstract Hashtable Settings(int portalId);

		/// <summary>
		///     
		/// </summary>
		/// <param name="mySettingName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public abstract string SettingString(string mySettingName);

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A int value...
		/// </returns>
		public abstract int GetDatabaseVersion();

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public abstract string GetLanguageList();

		/// <summary>
		///     
		/// </summary>
		/// <param name="strLangList" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public abstract string SetLanguageList(string strLangList);

		/// <summary>
		///     
		/// </summary>
		/// <param name="cp" type="Rainbow.Core.Portal">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="mySettingName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="mySettingValue" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public abstract void UpdateSettingString(Portal cp, string mySettingName, string mySettingValue);
	}
}