using System;

namespace Rainbow.Configuration
{
    /// <summary>
    /// This class encapsulates the basic attributes of a Portal, and is used
    /// by the administration pages when manipulating Portals.  PortalItem implements 
    /// the IComparable interface so that an ArrayList of PortalItems may be sorted
    /// by PortalOrder, using the ArrayList's Sort() method.
    /// </summary>
    public class PortalItem : IComparable 
    {
        private string   _name;
        private string   _path;
        private int      _ID;

        /// <summary>
        /// Name
        /// </summary>
        public string Name 
        {
            get 
            {
                return _name;
            }
            set 
            {
                _name = value;
            }
        }

        /// <summary>
        /// Path
        /// </summary>
        public string Path
        {
            get 
            {
                return _path;
            }
            set 
            {
                _path = value;
            }
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID 
        {
            get 
            {
                return _ID;
            }
            set 
            {
                _ID = value;
            }
        }  
  
        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareTo(object value) 
        {
            return this.CompareTo((object) Name);
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
        {
            return this.Name;
        }
    }
}