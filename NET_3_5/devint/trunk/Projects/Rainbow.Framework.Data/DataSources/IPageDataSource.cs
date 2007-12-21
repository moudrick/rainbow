using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Entities;

namespace Rainbow.Framework.Data.DataSources
{
    interface IPageDataSource
    {
        IPage CreateNew();
        IPage GetById(int pageId);
        List<IPage> GetAll();
        void Insert(IPage page);
        void Update(IPage page);
        void Delete(IPage page);
        void CommitChanges();
    }
}
