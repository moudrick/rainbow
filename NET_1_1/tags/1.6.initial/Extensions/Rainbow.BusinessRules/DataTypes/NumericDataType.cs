namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// NumericDataType
	/// </summary>
	public class NumericDataType : BaseDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		new protected string innerValue = "0";

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public NumericDataType()
		{
			InnerDataType = PropertiesDataType.Double;
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
		public override string Value
		{
			get { return (innerValue); }
			set
			{
				//Type check
				innerValue = double.Parse(value).ToString();
			}
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
			get { return "Numeric"; }
		}
	}
}