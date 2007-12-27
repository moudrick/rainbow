using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    public interface IPage : IComparable, IComparable<IPortal>, IConvertible
    {
        int PageId { get; set; }
        string Name { get; set; }

    }
}
