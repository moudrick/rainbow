namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// CustomListDataType
	/// </summary>
	public class CustomListDataType : ListDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// <param name="dataSource" type="object">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataTextField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="dataValueField" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public CustomListDataType(object dataSource, string dataTextField, string dataValueField)
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = dataSource;
			DataValueField = dataValueField;
			DataTextField = dataTextField;
			//InitializeComponents();
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
		public override object DataSource
		{
			get { return InnerDataSource; }
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
			get { return "Custom List"; }
		}
	}
}