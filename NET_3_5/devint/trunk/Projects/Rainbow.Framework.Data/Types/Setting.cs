using System;
using System.Web.UI;
using Rainbow.Framework.Data.Types;

namespace Rainbow.Framework.Data.Types
{
    /// <summary>
    /// This class holds a single setting in the hashtable,
    /// providing information about datatype, costraints.
    /// </summary>
    public partial class Setting : IComparable
    {
        private BaseDataType _datatype;
        private int _minValue;
        private int _maxValue;
        private int _order = 0;
        private bool _required = false;

        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        /// <value>The name of the setting.</value>
        /// <remarks>
        /// Added by Bill
        /// </remarks>
        public string SettingName { get; set; }

        //by Manu
        private string m_description = string.Empty;
        private string m_englishName = string.Empty;
        private int m_Group = 7000; //SettingGroup.MODULE_SPECIAL_SETTINGS;

        // Jes1111
        /// <summary>
        /// Allows grouping of settings in SettingsTable - use
        /// Rainbow.Framework.Configuration.SettingGroup enum (convert to string)
        /// </summary>
        /// <value>The group.</value>
        public int Group
        {
            get { return m_Group; }
            set { m_Group = value; }
        }

        /// <summary>
        /// It provides a description in plain English for
        /// Group Key (readonly)
        /// </summary>
        /// <value>The group description.</value>
        public string GroupDescription
        {
            get
            {
                //switch (m_Group)
                //{
                    //case SettingGroup.NONE:
                    //    return "Generic settings";

                    //case SettingGroup.THEME_LAYOUT_SETTINGS:
                    //    return "Theme and layout settings";

                    //case SettingGroup.SECURITY_USER_SETTINGS:
                    //    return "Users and Security settings";

                    //case SettingGroup.CULTURE_SETTINGS:
                    //    return "Culture settings";

                    //case SettingGroup.BUTTON_DISPLAY_SETTINGS:
                    //    return "Buttons and Display settings";

                    //case SettingGroup.MODULE_SPECIAL_SETTINGS:
                    //    return "Specific Module settings";

                    //case SettingGroup.META_SETTINGS:
                    //    return "Meta settings";

                    //case SettingGroup.MISC_SETTINGS:
                    //    return "Miscellaneous settings";

                    //case SettingGroup.NAVIGATION_SETTINGS:
                    //    return "Navigation settings";

                    //case SettingGroup.CUSTOM_USER_SETTINGS:
                    //    return "Custom User Settings";
                //}
                return "Settings";
            }
        }

        /// <summary>
        /// Provide help for parameter.
        /// Should be a brief, descriptive text that explains what
        /// this setting should do.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// It is the name of the parameter in plain english.
        /// </summary>
        /// <value>The name of the english.</value>
        public string EnglishName
        {
            get { return m_englishName; }
            set { m_englishName = value; }
        }

        /// <summary>
        /// Setting
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="value">The value.</param>
        public Setting(BaseDataType dt, string value)
        {
            _datatype = dt;
            _datatype.Value = value;
        }

        /// <summary>
        /// Setting
        /// </summary>
        /// <param name="dt">The dt.</param>
        public Setting(BaseDataType dt)
        {
            _datatype = dt;
        }

        /// <summary>
        /// ToString converter operator
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator string(Setting value)
        {
            return (value.ToString());
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            if (_datatype.Value != null)
                return _datatype.Value;
            else
                return string.Empty;
        }

        /// <summary>
        /// Value
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return _datatype.Value; }
            set { _datatype.Value = value; }
        }

        /// <summary>
        /// Gets or sets the setting value.
        /// </summary>
        /// <value>The setting value.</value>
        public string SettingValue
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// FullPath
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return _datatype.FullPath; }
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        /// <summary>
        /// DataType
        /// </summary>
        /// <value>The type of the data.</value>
        public PropertiesDataType DataType
        {
            get { return _datatype.Type; }
        }

        /// <summary>
        /// EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public Control EditControl
        {
            get { return _datatype.EditControl; }
            set { _datatype.EditControl = value; }
        }

        /// <summary>
        /// MinValue
        /// </summary>
        /// <value>The min value.</value>
        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        /// <summary>
        /// MaxValue
        /// </summary>
        /// <value>The max value.</value>
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        /// <summary>
        /// DataSource
        /// </summary>
        /// <value>The data source.</value>
        public object DataSource
        {
            get { return _datatype.DataSource; }
        }

        /// <summary>
        /// Display Order - use Rainbow.Framework.Configuration.SettingGroup enum
        /// (add integer in range 1-999)
        /// </summary>
        /// <value>The order.</value>
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            if (value == null) return 1;
            //Modified by Hongwei Shen(hongwei.shen@gmail.com) 10/9/2005
            // the "value" should be casted to Setting instead of ModuleItem 
            //			int compareOrder = ((ModuleItem) value).Order;
            int compareOrder = ((Setting)value).Order;
            // end of modification            
            if (Order == compareOrder) return 0;
            if (Order < compareOrder) return -1;
            if (Order > compareOrder) return 1;
            return 0;
        }
    }
}