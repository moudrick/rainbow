using System;
using System.ComponentModel;
using System.Web.UI;
using Esperantus.WebControls;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// LangListDataType
	/// </summary>
	public class LangListDataType : ListDataType
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public LangListDataType()
		{
			InnerDataType = PropertiesDataType.List;
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
			//Language grid
			using (WebDataGrid languagesGrid = new WebDataGrid())
			{
				//languagesGrid.ItemCommand += new DataGridCommandEventHandler(languagesGrid_ItemCommand);
				innerControl = languagesGrid;
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
				//be sure it is a valid value
				LanguageCultureCollection myLanguageCultureCollection = (LanguageCultureCollection) TypeDescriptor.GetConverter(typeof (LanguageCultureCollection)).ConvertTo(value, typeof (LanguageCultureCollection));
				innerValue = (string) TypeDescriptor.GetConverter(typeof (LanguageCultureCollection)).ConvertToString(myLanguageCultureCollection);
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
		public override Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				WebDataGrid languagesGrid = (WebDataGrid) innerControl;
				LanguageCultureCollection myLanguageCultureCollection = (LanguageCultureCollection) TypeDescriptor.GetConverter(typeof (LanguageCultureCollection)).ConvertTo(innerValue, typeof (LanguageCultureCollection));
				languagesGrid.DataSource = myLanguageCultureCollection;
				languagesGrid.DataBind();
				//Return control
				return innerControl;
			}
			set
			{
				if (value.GetType().Name == "WebDataGrid")
				{
					//Update value from control
					WebDataGrid languagesGrid = (WebDataGrid) value;
					languagesGrid.UpdateRows();
					Value = (string) TypeDescriptor.GetConverter(typeof (LanguageCultureCollection)).ConvertToString(languagesGrid.DataSource);
					innerControl = languagesGrid;
				}
				else
					throw new ArgumentException("An Esperantus.LangSwitcher.WebDataGrid control is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}

		//
		//		private void languagesGrid_ItemCommand(object sender, DataGridCommandEventArgs e)
		//		{
		//			WebDataGrid languagesGrid = (WebDataGrid) sender;
		//			innerValue = (string) TypeDescriptor.GetConverter(typeof(LanguageCultureCollection)).ConvertToString(languagesGrid.DataSource);
		//		}

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
			get { return "Language List"; }
		}
	}
}