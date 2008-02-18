using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Layout
    /// </summary>
    public interface ILayout : IEntity, IComparable<ILayout>
    {
        /// <summary>
        /// The Layout Name (must be the directory in which is located)
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }

        /// <summary>
        /// Relative/Web Path from root of application
        /// </summary>
        /// <value>The web path.</value>
        Uri WebPath { get; set; }

        /// <summary>
        /// Physical Path on the hard drive
        /// </summary>
        /// <value>The path.</value>
        string PhysicalPath { get; }
    }
}
