using System;
using System.Collections.Generic;
using System.Text;

namespace Rainbow.Framework.Providers.RainbowRoleProvider {

    /// <summary>
    /// 
    /// </summary>
    public class RainbowRole : IComparable {

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRole"/> class.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="roleName">Name of the role.</param>
        public RainbowRole( Guid roleId, string roleName ) {
            this.id = roleId;
            this.name = roleName;
            this.description = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRole"/> class.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="roleDescription">The role description.</param>
        public RainbowRole( Guid roleId, string roleName, string roleDescription ) {
            this.id = roleId;
            this.name = roleName;
            this.description = roleDescription;
        }

        private Guid id;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description {
            get {
                return description;
            }
            set {
                description = value;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals( object obj ) {
            //Check for null and compare run-time types.
            if ( obj == null || GetType() != obj.GetType() ) {
                return false;
            }

            RainbowRole role = ( RainbowRole )obj;
            return ( id == role.id ) && ( name == role.name );
        }

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo( object obj ) {
            if ( obj is RainbowRole ) {
                RainbowRole role = ( RainbowRole )obj;
                return name.CompareTo( role.name );
            }
            throw new ArgumentException( "object is not a RainbowRole" );    
        }

        #endregion
    }
}
