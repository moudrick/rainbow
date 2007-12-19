using System;
using System.Collections;
using Rainbow.Core;

namespace Rainbow.Data
{
	public abstract class ModulesProvider : DataProvider
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected ModulesProvider()
		{
		}

		/// <summary>
		///     Gets a Page object based on the page id #
		/// </summary>
		/// <param name="pageId" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Page value...
		/// </returns>
		public abstract Module Module(int moduleId);


		/// <summary>
		/// Remove a module instance.
		/// </summary>
		/// <param name="tabID"></param>
		public abstract void Remove(int moduleId);

		/// <summary>
		///     Gets the settings property for the module in moduleId
		/// </summary>
		/// <param name="tabID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public abstract Hashtable Settings(int moduleId);
	}
}