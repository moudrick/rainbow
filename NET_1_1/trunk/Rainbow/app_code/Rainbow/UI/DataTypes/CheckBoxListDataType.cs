//Mike Stone 23/01/2005 based on John Bowens Multiselectlist
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// CheckBoxListDataType
	/// Implements a checkboxlist that allows multiple selections 
	/// (returns a colon-delimited string)
	/// by Mike Stone
	/// </summary>
	public class CheckBoxListDataType : BaseDataType
	{
		private string _dataValueField;
		private string _dataTextField;

		public CheckBoxListDataType(object _dataSource, string _textField, string _dataField)  
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = _dataSource;
			DataTextField = _textField;
			DataValueField = _dataField;
		}

		protected override void InitializeComponents()
		{
			//ListBox
			using (CheckBoxList cbl = new CheckBoxList())
			{
				// cbl.CssClass = "NormalTextBox";
				cbl.Width = new Unit("100%");
				cbl.RepeatColumns = 2;
				cbl.DataSource = DataSource;
				cbl.DataValueField = DataValueField;
				cbl.DataTextField = DataTextField;
				cbl.DataBind();

				innerControl = cbl;
			}
		}
		        
		public override string DataValueField
		{
			get
			{
				return _dataValueField;
			}
			set
			{
				_dataValueField = value;
			}
		}

		public override string DataTextField
		{
			get
			{
				return _dataTextField;
			}
			set
			{
				_dataTextField = value;
			}
		}
        
		public override object DataSource
		{
			get
			{
				return InnerDataSource;
			}
			set
			{
				InnerDataSource = value;
			}
		}


		public override string Value
		{
			get
			{
				return(innerValue);
			}
			set
			{
				innerValue = value.TrimEnd(new char[] {';'}); //Remove trailing ';'
			}
		}
		

		public override Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				CheckBoxList cbl = (CheckBoxList) innerControl;
				cbl.ClearSelection();
				// Store in string array
				string[] values = innerValue.Split(new char[] {';'});
				foreach (string _value in values)
				{
					if (cbl.Items.FindByValue(_value) != null)
						cbl.Items.FindByValue(_value).Selected = true;
				}
				//Return control
				return innerControl;
			}
			set
			{
				if(value.GetType().Name == "CheckBoxList")
				{
					innerControl = value;

					//Update value from control
				CheckBoxList cbl = (CheckBoxList) innerControl;
					StringBuilder sb = new StringBuilder();
					for (int i = 0 ; i < cbl.Items.Count ; i++)
					{
						if (cbl.Items[i].Selected)
						{
							sb.Append(cbl.Items[i].Value);
							sb.Append(";");
						}
					}
					Value = sb.ToString();
				}
				else
					throw new ArgumentException("A CheckBoxList value is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}
	}
}