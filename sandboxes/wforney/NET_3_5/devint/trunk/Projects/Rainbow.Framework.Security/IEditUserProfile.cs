namespace Rainbow.Framework.Security
{
    using System;

    /// <summary>
    /// Implement this interface in custom edit / register profile classes
    /// </summary>
    public interface IEditUserProfile
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether control is in edit mode
        /// </summary>
        bool EditMode { get; }

        /// <summary>
        ///     Gets or sets the page where to redirect user on save
        /// </summary>
        string RedirectPage { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This procedure should persist the user data on db
        /// </summary>
        /// <returns>
        /// The user id
        /// </returns>
        Guid SaveUserData();

        #endregion
    }
}