namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// DoubleDataType
	/// </summary>
	public class DoubleDataType : NumericDataType
	{
		public DoubleDataType()
		{
			InnerDataType = PropertiesDataType.Double;
			//InitializeComponents();
		}

		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				//Check type
				double i = double.Parse(value);
				base.Value = value;
			}
		}

		public override string Description
		{
			get
			{
				return "Double";
			}
		}
	}

}