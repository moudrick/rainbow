using System.Collections;
using Rainbow.Framework.Site.Configuration;

namespace Rainbow.Framework.BusinessObjects
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
        /// <param name="pageStripDetails">The t.</param>
        /// <returns></returns>
        public int Add(PageStripDetails pageStripDetails)
        {
            return InnerList.Add(pageStripDetails);
        }

        /// <summary>
        /// PageStripDetails indexer
        /// </summary>
        /// <value></value>
        public PageStripDetails this[int index]
        {
            get { return ((PageStripDetails) InnerList[index]); }
        }
    }
}
