using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Portal Setting
    /// </summary>
    public interface IPortalSetting : ISetting
    {
        /// <summary>
        /// Gets or sets the portal id.
        /// </summary>
        /// <value>The portal id.</value>
        Guid PortalId { get; set; }
    }
}
