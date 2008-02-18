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
    public sealed class Pages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pages"/> class.
        /// </summary>
        private Pages() { }

        /// <summary>
        /// source provider instance
        /// </summary>
        static PageProvider source = PageProvider.Instance();

        /// <summary>
        /// Add a Page to the data source.
        /// </summary>
        /// <param name="Id">The page id.</param>
        /// <param name="name">The name.</param>
        public static void Add(Guid id, string name) //add params as appropriate
        {
            IPage record = source.CreateNew() as IPage;  //create new entity in memory

            //add values
            record.Id = id;
            record.Name = name;


            source.Add(record);     //add record to data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        public static void Remove(Guid id)
        {
            IPage record = source.GetById(id) as IPage;  //grab page from data source as IPage interface object

            source.Remove(record);      //delete record from data source in memory
            source.CommitChanges();     //write changes back to data source
        }

        public static List<IPage> LoadAll()
        {
            return new List<IPage>(source.GetAll());
        }

        public static IPage LoadById(Guid id)
        {
            return source.GetById(id);
        }
    }
}
