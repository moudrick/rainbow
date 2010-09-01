namespace Rainbow.Framework.Data.Types
{
    /// <summary>
    /// Structure used for list settings
    /// </summary>
    public struct SettingOption
    {
        #region Constants and Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingOption"/> struct.
        /// </summary>
        /// <param name="value">The value of the setting.</param>
        /// <param name="name">The name of the setting.</param>
        public SettingOption(int value, string name)
            : this()
        {
            this.Value = value;
            this.Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name of the setting.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value of the setting.</value>
        public int Value { get; set; }

        #endregion
    }
}