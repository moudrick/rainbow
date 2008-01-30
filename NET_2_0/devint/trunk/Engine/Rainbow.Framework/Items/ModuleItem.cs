using System;

namespace Rainbow.Framework.Items
{
    /// <summary>
    /// This class encapsulates the basic attributes of a Module, and is used
    /// by the administration pages when manipulating modules.<br/>
    /// ModuleItem implements the IComparable interface so that an ArrayList
    /// of ModuleItems may be sorted by ModuleOrder, using the 
    /// ArrayList's Sort() method.
    /// </summary>
    public class ModuleItem : IComparable
    {
        int moduleOrder;
        string title;
        string pane;
        int id;
        int defID;

        /// <summary>
        /// Order
        /// </summary>
        /// <value>The order.</value>
        public int Order
        {
            get { return moduleOrder; }
            set { moduleOrder = value; }
        }

        /// <summary>
        /// Title
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Pane name
        /// </summary>
        /// <value>The name of the pane.</value>
        public string PaneName
        {
            get { return pane; }
            set { pane = value; }
        }

        /// <summary>
        /// ID
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Definition id
        /// </summary>
        /// <value>The module def ID.</value>
        public int ModuleDefID
        {
            get { return defID; }
            set { defID = value; }
        }

        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            if (value == null) return 1;

            int compareOrder = ((ModuleItem) value).Order;

            if (Order == compareOrder) return 0;
            if (Order < compareOrder) return -1;
            if (Order > compareOrder) return 1;
            return 0;
        }
    }
}
