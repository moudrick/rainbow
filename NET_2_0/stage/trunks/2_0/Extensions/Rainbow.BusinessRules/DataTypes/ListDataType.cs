using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// ListDataType
	/// </summary>
	public class ListDataType : BaseDataType
	{
		private string _dataTextField;
		private string _dataValueField;

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public ListDataType()
		{
			InnerDataType = PropertiesDataType.List;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="CsvList" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public ListDataType(string csvList)
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = csvList;
			//InitializeComponents();
		}

		/// <summary>
		/// Returning dropdown list databound to FileInfo[] array
		/// </summary>
		/// <param name="fileList"></param>
		public ListDataType(FileInfo[] fileList)
		{
			InnerDataType = PropertiesDataType.List;
			InnerDataSource = fileList;
			//this._dataTextField = "Name";
			//this._dataValueField = "Name";
			//InitializeComponents();
			//innerControl.DataBind();
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected override void InitializeComponents()
		{
			//Dropdown list
			using (DropDownList dd = new DropDownList())
			{
				dd.CssClass = "NormalTextBox";
				dd.Width = new Unit(controlWidth);
				dd.DataSource = DataSource;
				dd.DataValueField = DataValueField;
				dd.DataTextField = DataTextField;
				dd.DataBind();
				innerControl = dd;
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
		public override object DataSource
		{
			get
			{
				if (InnerDataSource != null)

				{
					if (InnerDataSource is FileInfo[])
						return InnerDataSource;

					else
						return InnerDataSource.ToString().Split(';');
				}

				else
					return null;
			}
			set { InnerDataSource = value; }
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
		public override string DataTextField
		{
			get { return _dataTextField; }
			set { _dataTextField = value; }
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
		public override string DataValueField
		{
			get { return _dataValueField; }
			set { _dataValueField = value; }
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
		public override Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();
				//Update value in control
				DropDownList dd = (DropDownList) innerControl;
				dd.ClearSelection();

				if (dd.Items.FindByValue(Value) != null)
					dd.Items.FindByValue(Value).Selected = true;
				//Return control
				return innerControl;
			}
			set
			{
				if (value.GetType().Name == "DropDownList")
				{
					innerControl = value;
					//Update value from control
					DropDownList dd = (DropDownList) innerControl;

					if (dd.SelectedItem != null)
						Value = dd.SelectedItem.Value;

					else
						Value = string.Empty;
				}

				else
					throw new ArgumentException("A DropDownList values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
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
		public override string Value
		{
			get { return (innerValue); }
			set
			{
				innerValue = value;
				//				DropDownList dd = (DropDownList) innerControl;
				//				if (dd.Items.FindByValue(value) != null)
				//				{
				//					dd.ClearSelection();
				//					dd.Items.FindByValue(value).Selected = true;
				//					innerValue = value;
				//				}
				//				else
				//				{
				//					//Invalid value
				//				}            
			}
		}
	}
}