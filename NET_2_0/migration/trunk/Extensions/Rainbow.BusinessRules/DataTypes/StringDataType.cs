namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// StringDataType
	/// </summary>
	public class StringDataType : BaseDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public StringDataType()
		{
			InnerDataType = PropertiesDataType.String;
			//InitializeComponents();
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="value" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public StringDataType(string value)
		{
			InnerDataType = PropertiesDataType.String;
			this.Value = value;
			InitializeComponents();
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
			get { return "String"; }
		}
	}
}