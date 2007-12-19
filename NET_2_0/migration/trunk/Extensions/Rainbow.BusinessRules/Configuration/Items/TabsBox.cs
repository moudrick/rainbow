using System.Collections;
using Rainbow.Settings;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Box tab
	/// </summary>
	public class TabsBox : CollectionBase
	{
		/// <summary>
		/// Add
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public int Add(TabStripDetails t)
		{
			return InnerList.Add(t);
		}

		/// <summary>
		/// TabStripDetails indexer
		/// </summary>
		public TabStripDetails this[int index]
		{
			get { return ((TabStripDetails) InnerList[index]); }
		}
	}
}