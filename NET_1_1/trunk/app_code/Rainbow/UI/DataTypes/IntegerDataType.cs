using System;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// IntegerDataType
	/// </summary>
	public class IntegerDataType : NumericDataType
	{
		/// <summary>
		/// 
		/// </summary>
		public IntegerDataType()
		{
			InnerDataType = PropertiesDataType.Integer;
			InitializeComponents();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				//Check type
				base.Value = Int32.Parse(value).ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Description
		{
			get
			{
				return "Integer";
			}
		}
	}
}