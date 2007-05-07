using System;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// DateDataType
	/// </summary>
	public class DateDataType : StringDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public DateDataType()
		{
			InnerDataType = PropertiesDataType.Date;
			//InitializeComponents();
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
		public override string Value
		{
			get { return base.Value; }
			set
			{
				//Check type
				DateTime i = DateTime.Parse(value);
				base.Value = value;
			}
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
		public override string Description
		{
			get { return "DateTime"; }
		}
	}

}