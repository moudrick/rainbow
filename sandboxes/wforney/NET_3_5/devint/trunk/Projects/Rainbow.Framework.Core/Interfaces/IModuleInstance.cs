using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Module Instance
    /// </summary>
    public interface IModuleInstance : IModule
    {
        /// <summary>
        /// Gets or sets the name of the pane.
        /// </summary>
        /// <value>The name of the pane.</value>
        string PaneName { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; set; }

        /// <summary>
        /// Gets or sets the cache time.
        /// </summary>
        /// <value>The cache time.</value>
        int CacheTime { get; set; }

        #region Collections

        // Settings is implied from IModule interface so isn't listed here.

        /// <summary>
        /// Gets or sets the user settings.
        /// </summary>
        /// <value>The user settings.</value>
        IEnumerable<IModuleInstanceSetting> UserSettings { get; }

        #endregion
    }
}
