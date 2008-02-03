﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Data.Entities
{
    public interface IPageSetting : ISetting
    {
        Guid PageId { get; set; }
    }
}
