namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// ApplicationUrlDataType
	/// </summary>
	public class ApplicationUrlDataType : UrlDataType
	{
		/// <summary>
		/// ApplicationUrlDataType
		/// </summary>
		public ApplicationUrlDataType()
		{
			InnerDataType = PropertiesDataType.String;
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
				base.Value = value;
			}
		}

		/// <summary>
		/// Url relative to Application
		/// </summary>
		public override string Description
		{
			get
			{
				return "Url relative to Application";
			}
		}
	}

}