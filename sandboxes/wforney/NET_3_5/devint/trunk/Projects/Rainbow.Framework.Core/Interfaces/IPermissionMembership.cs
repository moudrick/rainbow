﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Permission Membership
    /// </summary>
    public interface IPermissionMembership : IEntity
    {
        /// <summary>
        /// Gets or sets the permission id.
        /// </summary>
        /// <value>The permission id.</value>
        Guid PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the user or role id.
        /// </summary>
        /// <value>The user or role id.</value>
        Guid UserOrRoleId { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        /// <value>The object id.</value>
        Guid ObjectId { get; set; }
    }
}
