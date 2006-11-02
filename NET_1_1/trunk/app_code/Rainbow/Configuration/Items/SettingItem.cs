using System;
using System.Web.UI;
using Rainbow.UI.DataTypes;

namespace Rainbow.Configuration
{
	/// <summary>
	/// This class holds a single setting in the hashtable,
	/// providing information about datatype, costrains.
	/// </summary>
	public class SettingItem : IComparable
	{
		BaseDataType _datatype;
		int _minValue;
		int _maxValue;
		int _order = 0;
		bool _required = false;

		//by Manu
		string m_description = string.Empty;
		string m_englishName = string.Empty;
		SettingItemGroup m_Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;

		// Jes1111
		/// <summary>
		/// Allows grouping of settings in SettingsTable - use 
		/// Rainbow.Configuration.SettingItemGroup enum (convert to string)
		/// </summary>
		public SettingItemGroup Group
		{
			get
			{
				return m_Group;
			}
			set
			{
				m_Group = value;
			}
		}

		/// <summary>
		/// It provides a description in plain English for
		/// Group Key (readonly)
		/// </summary>
		public string GroupDescription
		{
			get
			{
				switch (m_Group)
				{
					case SettingItemGroup.NONE:
						return "Generic settings";

					case SettingItemGroup.THEME_LAYOUT_SETTINGS:
						return "Theme and layout settings";

					case SettingItemGroup.SECURITY_USER_SETTINGS:
						return "Users and Security settings";

					case SettingItemGroup.CULTURE_SETTINGS:
						return "Culture settings";

					case SettingItemGroup.BUTTON_DISPLAY_SETTINGS:
						return "Buttons and Display settings";

					case SettingItemGroup.MODULE_SPECIAL_SETTINGS:
						return "Specific Module settings";

					case SettingItemGroup.META_SETTINGS:
						return "Meta settings";

					case SettingItemGroup.MISC_SETTINGS:
						return "Miscellaneous settings";

					case SettingItemGroup.NAVIGATION_SETTINGS:
						return "Navigation settings";
					
					case SettingItemGroup.CUSTOM_USER_SETTINGS:
						return "Custom User Settings";
				}
				return "Settings";
			}
		}

		/// <summary>
		/// Provide help for parameter. 
		/// Should be a brief, descriptive text that explains what
		/// this setting should do.
		/// </summary>
		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		/// <summary>
		/// It is the name of the parameter in plain english.
		/// </summary>
		public string EnglishName
		{
			get
			{
				return m_englishName;
			}
			set
			{
				m_englishName = value;
			}
		}

		/// <summary>
		/// SettingItem
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="value"></param>
		public SettingItem(BaseDataType dt, string value)
		{
			_datatype = dt;
			_datatype.Value = value;
		}

		/// <summary>
		/// SettingItem
		/// </summary>
		/// <param name="dt"></param>
		public SettingItem(BaseDataType dt)
		{
			_datatype = dt;
		}

		/// <summary>
		/// ToString converter operator
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public implicit operator string(SettingItem value)
		{
			return (value.ToString());
		}

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_datatype.Value != null)
				return _datatype.Value;
			else
				return string.Empty;
		}

		/// <summary>
		/// Value
		/// </summary>
		public string Value
		{
			get
			{
				return _datatype.Value;
			}
			set
			{
				_datatype.Value = value;
			}
		}

		/// <summary>
		/// FullPath
		/// </summary>
		public string FullPath
		{
			get
			{
				return _datatype.FullPath;
			}
		}

		/// <summary>
		/// Required
		/// </summary>
		public bool Required
		{
			get{return _required;}
			set{_required = value;}
		}

		/// <summary>
		/// DataType
		/// </summary>
		public PropertiesDataType DataType
		{
			get{return _datatype.Type;}
		}

		/// <summary>
		/// EditControl
		/// </summary>
		public Control EditControl
		{
			get
			{
				return _datatype.EditControl;
			}
			set
			{
				_datatype.EditControl = value;
			}
		}
      
		/// <summary>
		/// MinValue
		/// </summary>
		public int MinValue
		{
			get{return _minValue;}
			set{_minValue = value;}
		}
        
		/// <summary>
		/// MaxValue
		/// </summary>
		public int MaxValue
		{
			get{return _maxValue;}
			set{_maxValue = value;}
		}
        
		/// <summary>
		/// DataSource
		/// </summary>
		public object DataSource
		{
			get{return _datatype.DataSource;}
		}
        
		/// <summary>
		/// Display Order - use Rainbow.Configuration.SettingItemGroup enum 
		/// (add integer in range 1-999)
		/// </summary>
		public int Order
		{
			get{return _order;}
			set{_order = value;}
		}
        
		/// <summary>
		/// Public comparer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int CompareTo(object value) 
		{
			if (value == null) return 1;
//Modified by Hongwei Shen(hongwei.shen@gmail.com) 10/9/2005
// the "value" should be casted to SettingItem instead of ModuleItem 
//			int compareOrder = ((ModuleItem) value).Order;
			int compareOrder = ((SettingItem)value).Order;
// end of modification            
			if (this.Order == compareOrder) return 0;
			if (this.Order < compareOrder) return -1;
			if (this.Order > compareOrder) return 1;
			return 0;
		}
	}
}