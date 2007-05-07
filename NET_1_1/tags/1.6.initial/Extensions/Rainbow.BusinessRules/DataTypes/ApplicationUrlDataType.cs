namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// ApplicationUrlDataType
	/// </summary>
	public class ApplicationUrlDataType : UrlDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public ApplicationUrlDataType()
		{
			InnerDataType = PropertiesDataType.String;
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
			set { base.Value = value; }
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
			get { return "Url relative to Application"; }
		}
	}

}