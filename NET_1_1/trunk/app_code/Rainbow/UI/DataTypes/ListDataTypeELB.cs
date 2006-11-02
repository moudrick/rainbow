using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ELB;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// ListDataTypeELB
	/// </summary>
	[History("Jonathan Minond", "5/15/2005", "Defined heights based on row counts")]
	[History("Jose Viladiu", "5/14/2005", "Converted to EasyListBox")]
	public class ListDataTypeELB : BaseDataType
	{
		private string _dataValueField;
		private string _dataTextField;

		public ListDataTypeELB()
		{
			InnerDataType = PropertiesDataType.List;
		}

		public ListDataTypeELB(string CsvList)
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = CsvList;
			//InitializeComponents();
		}

		/// <summary>
		/// Returning dropdown list databound to FileInfo[] array
		/// </summary>
		/// <param name="fileList"></param>
		public ListDataTypeELB(FileInfo[] fileList)
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = fileList;
		}

		protected override void InitializeComponents()
		{
			using (EasyListBox dd = new EasyListBox())
			{
				dd.CssClass = "NormalTextBox";
				dd.Width = new Unit(controlWidth);
				dd.DataSource = DataSource;
				dd.DataValueField = DataValueField;
				dd.DataTextField = DataTextField;
				dd.DataBind();

				int itemCount = dd.ItemCount;

				dd.Height = Unit.Parse("100");
				if(itemCount > 5)
					dd.Height = Unit.Parse("175");
				if(itemCount > 10)
					dd.Height = Unit.Parse("250");

				innerControl = dd;
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
				if(InnerDataSource != null)
					if(InnerDataSource is FileInfo[])
					{
						return InnerDataSource;
					}
					else
					{
						return InnerDataSource.ToString().Split(';');
					}
				else
					return null;
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
				innerValue = value;
			}
		}
		

		public override Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				EasyListBox dd = (EasyListBox) innerControl;
				dd.SelectedValue = Value;
				return innerControl;
			}
			set
			{
				if (value is EasyListBox)
				{
					innerControl = value;
					EasyListBox dd = (EasyListBox) innerControl;
					if (dd.SelectedText != null)
						Value = dd.SelectedValue;
					else
						Value = string.Empty;
				}
				else
					throw new ArgumentException("A EasyListBox value is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}
	}
}