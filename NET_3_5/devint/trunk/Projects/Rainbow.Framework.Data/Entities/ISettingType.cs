using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    public interface ISettingType : IEntity, IComparable<ISettingType>
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
    }
}
