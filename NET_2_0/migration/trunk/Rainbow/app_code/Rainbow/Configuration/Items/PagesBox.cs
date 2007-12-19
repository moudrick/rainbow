using System.Collections;

namespace Rainbow.Configuration
{
    /// <summary>
    /// Box tab
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    public class PagesBox : CollectionBase
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Add(PageStripDetails t)
        {
            return InnerList.Add(t);
        }

        /// <summary>
        /// PageStripDetails indexer
        /// </summary>
        public PageStripDetails this[int index]
        {
            get
            {
                return((PageStripDetails) InnerList[index]);
            }
        }
    }
}