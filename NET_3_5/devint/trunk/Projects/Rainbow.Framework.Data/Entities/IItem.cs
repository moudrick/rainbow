using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    public interface IItem : IEntity, ISecuredEntity, IComparable<IItem>
    {
        string Name { get; set; }
    }
}
