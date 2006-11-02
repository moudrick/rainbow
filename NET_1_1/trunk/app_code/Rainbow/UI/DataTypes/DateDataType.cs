using System;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// DateDataType
	/// </summary>
	public class DateDataType : StringDataType
	{
		public DateDataType()
		{
			InnerDataType = PropertiesDataType.Date;
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
				DateTime.Parse(value);
				base.Value = value;
			}
		}

		public override string Description
		{
			get
			{
				return "DateTime";
			}
		}
	}

}