using System.Collections;

namespace Rainbow.Setup
{
	/// <summary>
	/// IInstaller inteface is used by installable modules
	/// </summary>
	public interface IInstaller
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateSaver"></param>
		void Install(IDictionary stateSaver);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateSaver"></param>
		void Uninstall(IDictionary stateSaver);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateSaver"></param>
		void Commit(IDictionary stateSaver);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateSaver"></param>
		void Rollback(IDictionary stateSaver);
	}
}