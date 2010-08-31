namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;

    using Rainbow.KickStarter.CommonClasses;

    /// <summary>
    /// Default interface for edit modules
    /// </summary>
    public interface IEditModule
    {
        #region Events

        /// <summary>
        ///     Purpose: Notify when Editing has been canceled.
        /// </summary>
        /// <delegate>EventHandler</delegate>
        event EventHandler CancelEdit;

        /// <summary>
        ///     Purpose: Notify when Record has been Selected, Inserted, Updated or Deleted.
        /// </summary>
        /// <delegate>DataChangeEndedEventHandler</delegate>
        event DataChangeEventHandler DataActionEnd;

        /// <summary>
        ///     Purpose: Notify when Record has been Selected, Inserted, Updated or Deleted.
        /// </summary>
        /// <delegate>DataChangeStartedEventHandler</delegate>
        event DataChangeEventHandler DataActionStart;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets whether to Allow Control to add records.
        /// </summary>
        /// <value><c>true</c> if [allow add]; otherwise, <c>false</c>.</value>
        bool AllowAdd { get; set; }

        /// <summary>
        ///     Gets or sets whether to Allow Control to delete records.
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        bool AllowDelete { get; set; }

        /// <summary>
        ///     Gets or sets whether to Allow Control to update records.
        /// </summary>
        /// <value><c>true</c> if [allow update]; otherwise, <c>false</c>.</value>
        bool AllowUpdate { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Purpose: Displays any error messages.
        /// </summary>
        /// <param name="e">
        /// The Exception to display.
        /// </param>
        void HandleError(Exception e);

        /// <summary>
        /// Purpose: Method the List control after data has been updated by the Edit control.
        /// </summary>
        void Reset();

        /// <summary>
        /// The module select the requested item and starts editing the module.
        /// </summary>
        /// <param name="itemId">
        /// The param is string to be more general. Usually it contains a number.
        /// </param>
        void StartEdit(string itemId);

        #endregion
    }
}