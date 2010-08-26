using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Interfaces
{
    /// <summary>
    /// Interface for Setting Type
    /// </summary>
    public interface ISettingType : IEntity, IComparable<ISettingType>
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
    }
}
