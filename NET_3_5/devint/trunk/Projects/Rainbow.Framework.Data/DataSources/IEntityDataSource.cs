using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Entities;

namespace Rainbow.Framework.Data.DataSources
{
    public interface IEntityDataSource
    {
        #region Select

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntity> GetAll();

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        IEntity GetById(Guid Id);

        #endregion

        #region Insert

        /// <summary>
        /// Adds the specified new entity.
        /// </summary>
        /// <param name="newEntity">The new entity.</param>
        void Add(ref IEntity newEntity);

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns></returns>
        IEntity CreateNew();

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(IEntity entity);

        #endregion

        #region Delete

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Remove(IEntity entity);

        #endregion

        /// <summary>
        /// Commits the changes.
        /// </summary>
        void CommitChanges();
    }
}
