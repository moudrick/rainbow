using System;

namespace Rainbow.Framework.BusinessObjects
{
    public class RainbowRole : IComparable
    {
        Guid id;
        string name;
        string description;

        public RainbowRole(Guid roleId, string roleName)
            : this(roleId, roleName, string.Empty)
        {}

        public RainbowRole(Guid id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            RainbowRole role = (RainbowRole) obj;
            return (id == role.id) && (name == role.name);
        }

        ///<summary>
        ///Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        ///</summary>
        ///
        ///<returns>
        ///A hash code for the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is RainbowRole)
            {
                RainbowRole role = (RainbowRole) obj;
                return name.CompareTo(role.name);
            }
            throw new ArgumentException("object is not a RainbowRole");
        }

        #endregion
    }
}