namespace Rainbow.Framework.Data.Types
{
    /// <summary>
    /// StringDataType
    /// </summary>
    public class StringDataType : BaseDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDataType"/> class.
        /// </summary>
        public StringDataType()
        {
            InnerDataType = PropertiesDataType.String;
            //InitializeComponents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringDataType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public StringDataType(string value)
        {
            InnerDataType = PropertiesDataType.String;
            Value = value;
            InitializeComponents();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "String"; }
        }
    }
}