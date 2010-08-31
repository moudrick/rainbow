namespace Rainbow.Framework
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Single item in list. Language culture pair.
    /// </summary>
    public class LanguageCultureItem
    {
        #region Constants and Fields

        /// <summary>
        /// The culture.
        /// </summary>
        private CultureInfo culture;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCultureItem"/> class.
        /// </summary>
        /// <param name="uiCulture">The ui culture.</param>
        /// <param name="culture">The culture.</param>
        public LanguageCultureItem(CultureInfo uiCulture, CultureInfo culture)
        {
            this.UICulture = uiCulture ?? CultureInfo.InvariantCulture;
            this.Culture = culture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCultureItem"/> class. 
        ///     Initializes a new instance of the <see cref="T:LanguageCultureItem"/> class.
        /// </summary>
        public LanguageCultureItem()
        {
            this.UICulture = CultureInfo.InvariantCulture;
            this.Culture = CultureInfo.CreateSpecificCulture(CultureInfo.InvariantCulture.Name);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }

            set
            {
                if (value.IsNeutralCulture)
                {
                    throw new ArgumentException("Culture value cannot be neutral", "value");
                }

                this.culture = value;
            }
        }

        /// <summary>
        ///     Gets or sets the UI culture.
        /// </summary>
        /// <value>The UI culture.</value>
        public CultureInfo UICulture { get; set; }

        #endregion

        #region Operators

        /// <summary>
        ///     Implicit operators the specified item.
        /// </summary>
        /// <param name = "item">The item.</param>
        /// <returns></returns>
        public static implicit operator string(LanguageCultureItem item)
        {
            return item.ToString();
        }

        #endregion

        // 		public static bool operator==(LanguageCultureItem a, LanguageCultureItem b) 
        // 		{
        // 			return LanguageCultureItem.Equals(a, b);
        // 		}

        // 		public static bool operator!=(LanguageCultureItem a, LanguageCultureItem b) 
        // 		{
        // 			return !LanguageCultureItem.Equals(a, b);
        // 		}
        #region Public Methods

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="a">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <param name="b">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public static bool Equals(LanguageCultureItem a, LanguageCultureItem b)
        {
            return (a != null) && (b != null) && (a.ToString() == b.ToString() || a.Equals(b));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(LanguageCultureItem) && Equals(this, (LanguageCultureItem)obj);
        }

        /// <summary>
        /// We must override GetHashCode when we override Equals
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.UICulture.LCID * 5000) + this.Culture.LCID;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", this.UICulture.Name, this.Culture.Name);
        }

        #endregion
    }
}