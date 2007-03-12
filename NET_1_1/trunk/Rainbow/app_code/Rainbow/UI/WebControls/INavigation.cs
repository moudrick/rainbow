namespace Rainbow.UI.WebControls
{
    /// <summary>
    /// Default indterface for navigation controls
    /// </summary>
    public interface INavigation
    {
        /// <summary>
        /// Indicates if control should bind when loads
        /// </summary>
        bool AutoBind
        {
            get;
            set;
        }

        /// <summary>
        /// Describes how this control should bind to db data
        /// </summary>
        BindOption Bind
        {
            get;
            set;
        }

		/// <summary>
		/// Describes how this control should bind to db data
		/// </summary>
		int ParentPageID
		{get;set;}

    }
}