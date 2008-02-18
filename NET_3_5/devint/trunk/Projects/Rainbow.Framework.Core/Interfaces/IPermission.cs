using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Permission
    /// </summary>
    public interface IPermission : IEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }
    }
}
