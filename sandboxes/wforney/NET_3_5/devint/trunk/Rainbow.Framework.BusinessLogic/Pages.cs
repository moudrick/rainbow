namespace Rainbow.Framework.Data
{
    using System;
    using System.Collections.Generic;

    using Rainbow.Framework.Data.DataSources;
    using Rainbow.Framework.Interfaces;

    /// <summary>
    /// Generic Pages Data Access Class
    /// </summary>
    public static class Pages
    {
        #region Constants and Fields

        /// <summary>
        ///     source provider instance
        /// </summary>
        private static readonly PageProvider Source = PageProvider.Instance();

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a Page to the data source.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public static void Add(Guid id, string name)
        {
            // add params as appropriate
            var record = Source.CreateNew(); // create new entity in memory

            // add values
            record.Id = id;
            record.Name = name;

            Source.Add(record); // add record to data source in memory
            Source.CommitChanges(); // write changes back to data source
        }

        /// <summary>
        /// The load all.
        /// </summary>
        /// <returns>
        /// </returns>
        public static List<IPage> LoadAll()
        {
            return new List<IPage>(Source.GetAll());
        }

        /// <summary>
        /// The load by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// </returns>
        public static IPage LoadById(Guid id)
        {
            return Source.GetById(id);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public static void Remove(Guid id)
        {
            var record = Source.GetById(id); // grab page from data source as IPage interface object

            Source.Remove(record); // delete record from data source in memory
            Source.CommitChanges(); // write changes back to data source
        }

        #endregion
    }
}