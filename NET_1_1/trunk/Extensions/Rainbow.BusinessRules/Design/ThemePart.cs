using System;
using System.Xml.Serialization;

namespace Rainbow.Design
{
	/// <summary>
	/// A single named HTML fragment
	/// </summary>
	[Serializable]
	public class ThemePart
	{
		private string _HTML;
		private string _Name;

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public ThemePart()
		{
			_Name = string.Empty;
			_HTML = string.Empty;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="html" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public ThemePart(string name, string html)
		{
			_Name = name;
			_HTML = html;
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
		public string HTML
		{
			get { return _HTML; }
			set { _HTML = value; }
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
		[XmlAttribute(DataType = "string")]
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
	}
}