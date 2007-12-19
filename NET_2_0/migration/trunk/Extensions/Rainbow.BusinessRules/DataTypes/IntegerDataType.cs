using System;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// IntegerDataType
	/// </summary>
	public class IntegerDataType : NumericDataType
	{
		public IntegerDataType()
		{
			InnerDataType = PropertiesDataType.Integer;
			InitializeComponents();
		}

		public override string Value
		{
			get { return base.Value; }
			set
			{
				//Check type
				base.Value = Int32.Parse(value).ToString();
			}
		}

		public override string Description
		{
			get { return "Integer"; }
		}
	}
}