namespace Rainbow.Framework.Design
{
    /// <summary>
    /// ThemeItem encapsulates the items of Theme list.
    /// Uses IComparable interface to allow sorting by name.
    /// </summary>
    public class ThemeItem //: IComparable
    {
        string name;

        /// <summary>
        /// The name of the theme
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

//        /// <summary>
//        /// Compares to.
//        /// </summary>
//        /// <param name="value">The value.</param>
//        /// <returns>A int value...</returns>
//        public int CompareTo(object value)
//        {
//            return CompareTo(Name);
//        }
    }
}