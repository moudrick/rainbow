using System;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// UrlDataType
	/// </summary>
	public class UrlDataType : BaseDataType
	{
		protected new string innerValue = "http://localhost";

		public UrlDataType()
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
				//Check type
				if ((value != null) && (value.Length != 0)) //Check by Bill (blarm)
					base.Value = new Uri(value).ToString(); 
				else 
					base.Value = value; 
			}
		}

		public override string Description
		{
			get
			{
				return "Full valid URI";
			}
		}
	}

}