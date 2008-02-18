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
        static PageProvider source = PageProvider.Instance();

        /// <summary>
        /// Add a Page to the data source.
        /// </summary>
        /// <param name="Id">The page id.</param>
        /// <param name="name">The name.</param>
        static void Add(Guid Id, string name) //add params as appropriate
        {
            IPage record = source.CreateNew() as IPage;  //create new entity in memory

            //add values
            record.Id = Id;
            record.Name = name;


            source.Add(ref record);     //add record to data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        static void Remove(Guid Id)
        {
            IPage record = source.GetById(Id) as IPage;  //grab page from data source as IPage interface object

            source.Remove(record);      //delete record from data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        static List<IPage> LoadAll()
        {
            return source.GetAll() as List<IPage>;
        }

        static IPage LoadById(Guid id)
        {
            return source.GetById(id);
        }
    }
}
