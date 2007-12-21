using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.Framework.Data.Entities
{
    interface IPage
    {
        int PageId { get; set; }
        string Name { get; set; }

    }
}
