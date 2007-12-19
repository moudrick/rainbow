using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// BaseDataType
	/// </summary>
	public abstract class BaseDataType
	{
		/// <summary>
		/// Holds the value
		/// </summary>
		protected PropertiesDataType InnerDataType;

		/// <summary>
		///     
		/// </summary>
		protected object InnerDataSource;

		/// <summary>
		///     
		/// </summary>
		protected int controlWidth = 350;

		/// <summary>
		///     
		/// </summary>
		protected Control innerControl;

		/// <summary>
		///     
		/// </summary>
		protected string innerValue = string.Empty;

		/// <summary>
		///     
		/// </summary>
		protected virtual void InitializeComponents()
		{
			//Text box
			using (TextBox tx = new TextBox())
			{
				tx.CssClass = "NormalTextBox";
				tx.Columns = 30;
				tx.Width = new Unit(controlWidth);
				tx.MaxLength = 1500; //changed max value to 1500 since most of settings are string

				innerControl = tx;
			}
		}

		/// <summary>
		/// Gets DataSource
		/// Should be overrided from inherited classes
		/// </summary>
		public virtual object DataSource
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		///     
		/// </summary>
		public virtual string DataValueField
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		///     
		/// </summary>
		public virtual string DataTextField
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		///     
		/// </summary>
		public virtual string FullPath
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		///     
		/// </summary>
		public virtual Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				((TextBox) innerControl).Text = Value;
				//Return control
				return innerControl;
			}
			set
			{
				if (value.GetType().Name == "TextBox")
				{
					innerControl = value;
					//Update value from control
					Value = ((TextBox) innerControl).Text;
				}
				else
					throw new ArgumentException("A TextBox values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}

		/// <summary>
		///     
		/// </summary>
		public virtual PropertiesDataType Type
		{
			get { return InnerDataType; }
		}

		/// <summary>
		///     
		/// </summary>
		public virtual string Value
		{
			get { return (innerValue); }
			set { innerValue = value; }
		}

		/// <summary>
		///     
		/// </summary>
		public virtual string Description
		{
			get { return string.Empty; }
		}
	}
}