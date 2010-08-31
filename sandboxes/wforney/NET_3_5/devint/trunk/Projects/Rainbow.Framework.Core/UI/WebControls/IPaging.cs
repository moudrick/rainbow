namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;

    /// <summary>
    /// Common interface for paging controls
    /// </summary>
    public interface IPaging
    {
        #region Events

        /// <summary>
        /// The on move.
        /// </summary>
        event EventHandler OnMove;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the page number.
        /// </summary>
        /// <value>The page number.</value>
        int PageNumber { get; set; }

        /// <summary>
        ///     Gets or sets the record count.
        /// </summary>
        /// <value>The record count.</value>
        int RecordCount { get; set; }

        /// <summary>
        ///     Gets or sets the records per page.
        /// </summary>
        /// <value>The records per page.</value>
        int RecordsPerPage { get; set; }

        #endregion
    }
}