namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// StringDataType
	/// </summary>
	public class StringDataType : BaseDataType
	{
		/// <summary>
		/// StringDataType
		/// </summary>
		public StringDataType()
		{
			InnerDataType = PropertiesDataType.String;
			//InitializeComponents();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public StringDataType(string value)
		{
			InnerDataType = PropertiesDataType.String;
			this.Value = value;
			InitializeComponents();
		}

		/// <summary>
		/// String
		/// </summary>
		public override string Description
		{
			get
			{
				return "String";
			}
		}
	}
}