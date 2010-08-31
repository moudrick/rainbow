namespace Rainbow.Framework.Providers.RainbowRoleProvider
{
    using System;

    /// <summary>
    /// The rainbow role.
    /// </summary>
    public class RainbowRole : IComparable, IEquatable<RainbowRole>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRole"/> class.
        /// </summary>
        /// <param name="roleId">
        /// The role id.
        /// </param>
        /// <param name="roleName">
        /// Name of the role.
        /// </param>
        public RainbowRole(Guid roleId, string roleName)
        {
            this.Id = roleId;
            this.Name = roleName;
            this.Description = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRole"/> class.
        /// </summary>
        /// <param name="roleId">
        /// The role id.
        /// </param>
        /// <param name="roleName">
        /// Name of the role.
        /// </param>
        /// <param name="roleDescription">
        /// The role description.
        /// </param>
        public RainbowRole(Guid roleId, string roleName, string roleDescription)
        {
            this.Id = roleId;
            this.Name = roleName;
            this.Description = roleDescription;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The role id.</value>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The role name.</value>
        public string Name { get; set; }

        #endregion

        #region Operators

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name = "left">The left role.</param>
        /// <param name = "right">The right role.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RainbowRole left, RainbowRole right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name = "left">The left role.</param>
        /// <param name = "right">The right role.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RainbowRole left, RainbowRole right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(RainbowRole))
            {
                return false;
            }

            var role = (RainbowRole)obj;
            return (this.Id == role.Id) && (this.Name == role.Name);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = this.Description != null ? this.Description.GetHashCode() : 0;
                result = (result * 397) ^ this.Id.GetHashCode();
                result = (result * 397) ^ (this.Name != null ? this.Name.GetHashCode() : 0);
                return result;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance. 
        /// </exception>
        public int CompareTo(object obj)
        {
            if (obj is RainbowRole)
            {
                var role = (RainbowRole)obj;
                return this.Name.CompareTo(role.Name);
            }

            throw new ArgumentException("object is not a RainbowRole");
        }

        #endregion

        #region IEquatable<RainbowRole>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(RainbowRole other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Description, this.Description) && other.Id.Equals(this.Id) && Equals(other.Name, this.Name);
        }

        #endregion

        #endregion
    }
}