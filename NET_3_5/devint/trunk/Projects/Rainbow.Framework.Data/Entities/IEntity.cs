using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    public interface IEntity : IComparable, IConvertible
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the object type id.
        /// </summary>
        /// <value>The object type id.</value>
        Guid ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <remarks>
        /// This should be set true if you want to delete something. The record should only be removed from the database after being
        /// dumped from the recycler. We need a Destroy function on the data source for the actual deletion.
        /// </remarks>
        /// <value>
        /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        DateTime LastModified { get; set; }
    }
}
