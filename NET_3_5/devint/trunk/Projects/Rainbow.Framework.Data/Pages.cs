using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Entities;
using Rainbow.Framework.Data.DataSources;

namespace Rainbow.Framework.Data
{
    /// <summary>
    /// Generic Pages Data Access Class
    /// </summary>
    public partial class Pages
    {
        /// <summary>
        /// Add a Page to the data source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageId">The page id.</param>
        /// <param name="name">The name.</param>
        void Add(IPageDataSource source, int pageId, string name) //add params as appropriate
        {
            IPage record = source.CreateNew();  //create new entity in memory

            //add values
            record.PageId = pageId;
            record.Name = name;

            source.Insert(record);      //add record to data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        void Remove(IPageDataSource source, int pageId)
        {
            IPage record = source.GetById(pageId);  //grab page from data source as IPage interface object

            source.Delete(record);      //delete record from data source in memory
            source.CommitChanges();     //write changes back to data source
        }
    }
}
