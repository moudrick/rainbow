using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Entities;

namespace Rainbow.Framework.Data.DataSources
{
    interface IPageDataSource
    {
        #region Select

        /// <summary>
        /// Gets all pages.
        /// </summary>
        /// <returns>IEnumerable&lt;IPage&gt;</returns>
        IEnumerable<IPage> GetAll();

        /// <summary>
        /// Gets the page by id.
        /// </summary>
        /// <param name="Id">The page id.</param>
        /// <returns>IPage</returns>
        IPage GetById(int Id);

        #endregion

        #region Insert

        /// <summary>
        /// Adds the specified new page.
        /// </summary>
        /// <param name="newpage">The new page.</param>
        /// <returns></returns>
        void Add(ref IPage newpage);

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns>IPage</returns>
        IPage CreateNew();

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified page id.
        /// </summary>
        /// <param name="Id">The page id.</param>
        void Update(IPage page);

        #endregion

        #region Delete

        /// <summary>
        /// Removes the specified page id.
        /// </summary>
        /// <param name="Id">The page id.</param>
        void Remove(IPage page);

        #endregion

        /// <summary>
        /// Commits the changes.
        /// </summary>
        void CommitChanges();
    }
}
