using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Page Setting
    /// </summary>
    public interface IPageSetting : ISetting
    {
        /// <summary>
        /// Gets or sets the page id.
        /// </summary>
        /// <value>The page id.</value>
        Guid PageId { get; set; }
    }
}
