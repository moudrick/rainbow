using System.Xml.Serialization;

namespace Rainbow.Framework.BusinessObjects
{
    /// <summary>
    /// PageStripDetails Class encapsulates the tabstrip details
    /// -- PageName, PageID and PageOrder -- for a specific Page in the Portal
    /// </summary>
    [XmlType(TypeName = "MenuItem")]
    public class PageStripDetails
    {
        [XmlAttribute("AuthRoles")]
        public string AuthorizedRoles;

        [XmlAttribute("ParentPageID")]
        public int ParentPageID;
		
        [XmlAttribute("PageImage")]
        public string PageImage;

        [XmlAttribute("PageIndex")]
        public int PageIndex;

        [XmlAttribute("PageLayout")]
        public string PageLayout;

        [XmlAttribute("Label")]
        public string PageName;

        [XmlAttribute("PageOrder")]
        public int PageOrder;

        int pageID;

        /// <summary>
        /// Gets or sets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("ID")]
        public int PageID
        {
            get
            {
                return pageID;
            }
            set
            {
                pageID = value;
            }
        }
    }
}