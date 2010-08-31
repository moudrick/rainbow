namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Default interface for searchable modules
    /// </summary>
    public interface ISearchable
    {
        #region Public Methods

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="searchStr">
        /// The search STR.
        /// </param>
        /// <param name="searchField">
        /// The search field.
        /// </param>
        /// <returns>
        /// The search sql select.
        /// </returns>
        string SearchSqlSelect(int portalId, int userId, string searchStr, string searchField);

        #endregion
    }
}