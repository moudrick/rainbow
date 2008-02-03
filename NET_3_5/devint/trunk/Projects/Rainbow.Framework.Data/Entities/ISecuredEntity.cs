using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    public interface ISecuredEntity
    {
        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        /// <remarks>
        /// This is a list of permissions from the master list that have meaning for this entity (i.e. You can't (insert action here)
        /// if the action isn't in the list)...
        /// </remarks>
        /// <value>The permissions.</value>
        IEnumerable<IPermission> Permissions { get; }

        /// <summary>
        /// Gets the permission memberships.
        /// </summary>
        /// <remarks>
        /// This holds the group of permission membership objects that say who has what permissions for this entity.
        /// </remarks>
        /// <value>The permission memberships.</value>
        IEnumerable<IPermissionMembership> PermissionMemberships { get; }

        /// <summary>
        /// Gets or sets the last editor.
        /// </summary>
        /// <value>The last editor.</value>
        Guid LastEditor { get; set; }
    }
}
