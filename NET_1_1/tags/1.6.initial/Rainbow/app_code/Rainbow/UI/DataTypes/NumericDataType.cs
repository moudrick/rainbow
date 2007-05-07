namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// NumericDataType
	/// </summary>
	public class NumericDataType : BaseDataType
	{
		protected new string innerValue = "0";

		public NumericDataType()
		{
			InnerDataType = PropertiesDataType.Double;
			//InitializeComponents();
		}

		public override string Value
		{
			get
			{
				return(innerValue);
			}
			set
			{
				//Type check
				innerValue = double.Parse(value).ToString();
			}
		}

		public override string Description
		{
			get
			{
				return "Numeric";
			}
		}
	}
}