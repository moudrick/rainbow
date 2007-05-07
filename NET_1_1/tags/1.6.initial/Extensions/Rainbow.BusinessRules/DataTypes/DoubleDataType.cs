namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// DoubleDataType
	/// </summary>
	public class DoubleDataType : NumericDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public DoubleDataType()
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
			get { return base.Value; }
			set
			{
				//Check type
				double i = double.Parse(value);
				base.Value = value;
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
			get { return "Double"; }
		}
	}

}