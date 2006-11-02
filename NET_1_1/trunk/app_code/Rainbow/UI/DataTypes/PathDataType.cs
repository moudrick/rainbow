namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// PathDataType
	/// </summary>
	public class PathDataType : StringDataType
	{
		public PathDataType()
		{
			InnerDataType = PropertiesDataType.String;
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
				value = value.Replace("/", "\\");
				base.Value = value;
			}
		}

		public override string Description
		{
			get
			{
				return "File System path";
			}
		}
	}
}