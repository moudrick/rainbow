namespace Rainbow.Framework.Design
{
    /// <summary>
    /// LayoutItem encapsulates the items of Layout list.
    /// Uses IComparable interface to allow sorting by name.
    /// </summary>
    /// <remarks>by Cory Isakson</remarks>
    public class LayoutItem //: IComparable
    {
        string name;

//        /// <summary>
//        /// Compares to.
//        /// </summary>
//        /// <param name="value">The value.</param>
//        /// <returns>A int value...</returns>
//        public int CompareTo(object value)
//        {
//            return CompareTo(Name);
//        }

        /// <summary>
        /// The name of the layout
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
