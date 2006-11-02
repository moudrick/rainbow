using System;

namespace Rainbow.Design
{	

	/// <summary>
	/// LayoutItem encapsulates the items of Layout list.
	/// Uses IComparable interface to allow sorting by name.
	/// </summary>
	/// <remarks>by Cory Isakson</remarks>
	public class LayoutItem : IComparable 
	{

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string _name;

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

		/// <summary>
		/// The name of the layout
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
	}
}
