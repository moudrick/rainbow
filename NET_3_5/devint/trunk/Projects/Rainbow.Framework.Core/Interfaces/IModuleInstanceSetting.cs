using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Module Instance Setting
    /// </summary>
    public interface IModuleInstanceSetting : IModuleSetting
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        Guid UserId { get; set; }
    }
}
