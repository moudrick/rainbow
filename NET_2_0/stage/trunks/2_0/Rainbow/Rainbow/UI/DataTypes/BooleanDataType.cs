using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// BooleanDataType
	/// </summary>
	public class BooleanDataType : BaseDataType
	{
		/// <summary>
		/// innerValue - Default to False
		/// </summary>
		protected new string innerValue = "False";

		/// <summary>
		/// 
		/// </summary>
		public BooleanDataType()
		{
			InnerDataType = PropertiesDataType.Boolean;
			InitializeComponents();
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void InitializeComponents()
		{
			//Checkbox
			innerControl = new CheckBox();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Value
		{
			get
			{
				return(innerValue);
			}
			set
			{
				//Type check
				innerValue = bool.Parse(value).ToString();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				((CheckBox) innerControl).Checked =  bool.Parse(Value);
				//Return control
				return innerControl;
			}
			set
			{
				if(value.GetType().Name == "CheckBox")
				{
					innerControl = value;
					//Update value from control
					Value = ((CheckBox) innerControl).Checked.ToString();
				}
				else
					throw new ArgumentException("A CheckBox values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}
		/// <summary>
		/// Boolean
		/// </summary>
		public override string Description
		{
			get
			{
				return "Boolean";
			}
		}
	}
}