namespace Rainbow.Configuration
{
	/// <summary>
	/// WorkflowState
	/// by Geert.Audenaert@Syntegra.Com
	/// Date: 27/2/2003
	/// </summary>
	public enum WorkflowState
	{
		/// <summary>
		/// The staging and production content are identical
		/// </summary>
		Original = 0,
		/// <summary>
		/// Were working on the staging content
		/// </summary>
		Working = 1,
		/// <summary>
		/// The staging content is ready and approval is been requested
		/// </summary>
		ApprovalRequested = 2,
		/// <summary>
		/// The staging content is approved and ready to be published
		/// </summary>
		Approved = 3
	}
}