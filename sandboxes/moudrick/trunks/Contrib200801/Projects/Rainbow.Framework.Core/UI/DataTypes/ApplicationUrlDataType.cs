namespace Rainbow.Framework.DataTypes
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
        /// Url relative to Application
        /// </summary>
        /// <value>The description.</value>
		public override string Description
		{
			get
			{
				return "Url relative to Application";
			}
		}
	}
}
