using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.Framework.Data.Entities;

namespace Rainbow.Framework.Data.DataSources
{
    public interface IPortalDataSource
    {
        #region Select

        /// <summary>
        /// Gets all portals.
        /// </summary>
        /// <returns>IEnumerable&lt;IPortal&gt;</returns>
        IEnumerable<IPortal> GetAll();

        /// <summary>
        /// Gets the portal by id.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <returns>IPortal</returns>
        IPortal GetById(int portalId);

        #endregion

        #region Insert

        /// <summary>
        /// Adds the specified new portal.
        /// </summary>
        /// <param name="newPortal">The new portal.</param>
        /// <returns></returns>
        void Add(ref IPortal newPortal);

        /// <summary>
        /// Adds a new portal from solution template.
        /// </summary>
        /// <param name="solutionId">The solution id.</param>
        /// <returns>IPortal</returns>
        void AddFromSolution(int solutionId, ref IPortal newPortal);

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns>IPortal</returns>
        IPortal CreateNew();

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified portal id.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        void Update(IPortal portal);

        #endregion

        #region Delete

        /// <summary>
        /// Removes the specified portal id.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        void Remove(IPortal portal);

        #endregion

        /// <summary>
        /// Commits the changes.
        /// </summary>
        void CommitChanges();
    }
}
