using System;

namespace Rainbow.Design
{

	/// <summary>
	/// ThemeItem encapsulates the items of Theme list.
	/// Uses IComparable interface to allow sorting by name.
	/// </summary>
	public class ThemeItem : IComparable 
	{

		private string _name;

		/// <summary>
		/// The name of the theme
		/// </summary>
		public string Name 
		{
			get 
			{
				return _name;
			}
			set 
			{
				_name = value;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="value" type="object">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		public int CompareTo(object value) 
		{
			return this.CompareTo((object) Name);
		}
	}
}