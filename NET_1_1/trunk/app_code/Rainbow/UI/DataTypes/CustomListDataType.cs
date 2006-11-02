namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// CustomListDataType
	/// </summary>
	public class CustomListDataType : ListDataType
	{
		public CustomListDataType(object dataSource, string dataTextField, string dataValueField)
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = dataSource;
			DataValueField = dataValueField;
			DataTextField = dataTextField;
			//InitializeComponents();
		}
        
		public override object DataSource
		{
			get
			{
				return InnerDataSource;
			}
		}

//		public override string Value
//		{
//			get
//			{
//				return base.Value;
//			}
//			set
//			{
//				base.Value = value;
//			}
//		}

		public override string Description
		{
			get
			{
				return "Custom List";
			}
		}
	}
}