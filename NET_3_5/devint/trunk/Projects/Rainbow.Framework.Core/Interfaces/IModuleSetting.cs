using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Module Setting
    /// </summary>
    public interface IModuleSetting : ISetting
    {
        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        /// <value>The module id.</value>
        Guid ModuleId { get; set; }
    }
}
