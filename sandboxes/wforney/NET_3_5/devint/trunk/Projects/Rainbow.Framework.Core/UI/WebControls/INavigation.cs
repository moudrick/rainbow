namespace Rainbow.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Default indterface for navigation controls
    /// </summary>
    public interface INavigation
    {
        #region Properties

        /// <summary>
        ///     Gets or sets if control should bind when loads
        /// </summary>
        /// <value><c>true</c> if [auto bind]; otherwise, <c>false</c>.</value>
        bool AutoBind { get; set; }

        /// <summary>
        ///     Gets or sets how this control should bind to db data
        /// </summary>
        /// <value>The bind.</value>
        BindOption Bind { get; set; }

        /// <summary>
        ///     Gets or sets how this control should bind to db data
        /// </summary>
        /// <value>The parent page ID.</value>
        int ParentPageId { get; set; }

        #endregion
    }
}