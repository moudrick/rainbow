using System;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// Common interface for paging controls
	/// </summary>
	public interface IPaging
	{
		/// <summary>
		/// 
		/// </summary>
		event EventHandler OnMove;

		/// <summary>
		/// 
		/// </summary>
		int PageNumber 
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		int RecordCount 
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		int RecordsPerPage 
		{
			get;
			set;
		}
	}
}
