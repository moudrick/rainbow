namespace Rainbow.UI.WebControls
{
    /// <summary>
    /// Default interface for searchable modules
    /// </summary>
	public interface ISearchable
	{
		/// <summary>
		/// Searchable module implementation
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="userID"></param>
		/// <param name="searchStr"></param>
		/// <param name="searchField"></param>
		string SearchSqlSelect(int portalID, int userID, string searchStr, string searchField);
	}
}