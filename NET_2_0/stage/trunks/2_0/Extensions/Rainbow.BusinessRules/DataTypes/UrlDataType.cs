using System;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// UrlDataType
	/// </summary>
	public class UrlDataType : BaseDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		new protected string innerValue = "http://localhost";

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public UrlDataType()
		{
			InnerDataType = PropertiesDataType.String;
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
				if ((value != null) && (value != string.Empty)) //Check by Bill (blarm)
					base.Value = new Uri(value).ToString();
				else
					base.Value = value;
			}
		}

		public override string Description
		{
			get { return "Full valid URI"; }
		}
	}

}