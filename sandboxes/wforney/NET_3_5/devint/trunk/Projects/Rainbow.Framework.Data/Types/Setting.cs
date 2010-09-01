namespace Rainbow.Framework.Data.Types
{
    using System;
    using System.Web.UI;

    /// <summary>
    /// This class holds a single setting in the hashtable,
    ///     providing information about datatype, costraints.
    /// </summary>
    public class Setting : IComparable
    {
        #region Constants and Fields

        /// <summary>
        /// The datatype.
        /// </summary>
        private readonly BaseDataType datatype;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Setting"/> class.
        /// </summary>
        /// <param name="dt">The data type.</param>
        /// <param name="value">The value.</param>
        public Setting(BaseDataType dt, string value)
        {
            this.Description = string.Empty;
            this.Group = 7000;
            this.EnglishName = string.Empty;
            this.datatype = dt;
            this.datatype.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Setting"/> class. 
        /// Setting
        /// </summary>
        /// <param name="dt">
        /// The data type.
        /// </param>
        public Setting(BaseDataType dt)
        {
            this.Description = string.Empty;
            this.Group = 7000;
            this.EnglishName = string.Empty;
            this.datatype = dt;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     DataSource
        /// </summary>
        /// <value>The data source.</value>
        public object DataSource
        {
            get
            {
                return this.datatype.DataSource;
            }
        }

        /// <summary>
        ///     DataType
        /// </summary>
        /// <value>The type of the data.</value>
        public PropertiesDataType DataType
        {
            get
            {
                return this.datatype.Type;
            }
        }

        /// <summary>
        ///     Provide help for parameter.
        ///     Should be a brief, descriptive text that explains what
        ///     this setting should do.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        ///     EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public Control EditControl
        {
            get
            {
                return this.datatype.EditControl;
            }

            set
            {
                this.datatype.EditControl = value;
            }
        }

        /// <summary>
        ///     It is the name of the parameter in plain english.
        /// </summary>
        /// <value>The name of the english.</value>
        public string EnglishName { get; set; }

        /// <summary>
        ///     FullPath
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get
            {
                return this.datatype.FullPath;
            }
        }

        /// <summary>
        ///     Allows grouping of settings in SettingsTable - use
        ///     Rainbow.Framework.Configuration.SettingGroup enum (convert to string)
        /// </summary>
        /// <value>The group.</value>
        public int Group { get; set; }

        /// <summary>
        ///     It provides a description in plain English for
        ///     Group Key (readonly)
        /// </summary>
        /// <value>The group description.</value>
        public string GroupDescription
        {
            get
            {
                // switch (m_Group)
                // {
                // case SettingGroup.NONE:
                // return "Generic settings";

                // case SettingGroup.THEME_LAYOUT_SETTINGS:
                // return "Theme and layout settings";

                // case SettingGroup.SECURITY_USER_SETTINGS:
                // return "Users and Security settings";

                // case SettingGroup.CULTURE_SETTINGS:
                // return "Culture settings";

                // case SettingGroup.BUTTON_DISPLAY_SETTINGS:
                // return "Buttons and Display settings";

                // case SettingGroup.MODULE_SPECIAL_SETTINGS:
                // return "Specific Module settings";

                // case SettingGroup.META_SETTINGS:
                // return "Meta settings";

                // case SettingGroup.MISC_SETTINGS:
                // return "Miscellaneous settings";

                // case SettingGroup.NAVIGATION_SETTINGS:
                // return "Navigation settings";

                // case SettingGroup.CUSTOM_USER_SETTINGS:
                // return "Custom User Settings";
                // }
                return "Settings";
            }
        }

        /// <summary>
        ///     MaxValue
        /// </summary>
        /// <value>The max value.</value>
        public int MaxValue { get; set; }

        /// <summary>
        ///     MinValue
        /// </summary>
        /// <value>The min value.</value>
        public int MinValue { get; set; }

        /// <summary>
        ///     Display Order - use Rainbow.Framework.Configuration.SettingGroup enum
        ///     (add integer in range 1-999)
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        ///     Required
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required { get; set; }

        /// <summary>
        ///     Gets or sets the name of the setting.
        /// </summary>
        /// <value>The name of the setting.</value>
        /// <remarks>
        ///     Added by Bill
        /// </remarks>
        public string SettingName { get; set; }

        /// <summary>
        ///     Gets or sets the setting value.
        /// </summary>
        /// <value>The setting value.</value>
        public string SettingValue
        {
            get
            {
                return this.Value;
            }

            set
            {
                this.Value = value;
            }
        }

        /// <summary>
        ///     Value
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return this.datatype.Value;
            }

            set
            {
                this.datatype.Value = value;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///     ToString converter operator
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        public static implicit operator string(Setting value)
        {
            return value.ToString();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.datatype.Value ?? string.Empty;
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable

        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The compare to.
        /// </returns>
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            // Modified by Hongwei Shen(hongwei.shen@gmail.com) 10/9/2005
            // the "value" should be casted to Setting instead of ModuleItem 
            // 			int compareOrder = ((ModuleItem) value).Order;
            var compareOrder = ((Setting)value).Order;

            // end of modification            
            if (this.Order == compareOrder)
            {
                return 0;
            }

            if (this.Order < compareOrder)
            {
                return -1;
            }

            if (this.Order > compareOrder)
            {
                return 1;
            }

            return 0;
        }

        #endregion

        #endregion
    }
}