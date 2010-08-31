namespace Rainbow.Framework.Helpers
{
    using System.IO;
    using System.Web;
    using System.Web.Caching;
    using System.Xml.Serialization;

    /// <summary>
    /// WebPart
    ///     Added by Jakob Hansen.
    /// </summary>
    [XmlRoot(Namespace = "urn:schemas-microsoft-com:webpart:", IsNullable = false)]
    public class WebPart
    {
        #region Constants and Fields

        /// <summary>
        /// The allow minimize.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int AllowMinimize;

        /// <summary>
        /// The allow remove.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int AllowRemove;

        /// <summary>
        /// The auto update.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int AutoUpdate;

        /// <summary>
        /// The cache behavior.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int CacheBehavior;

        /// <summary>
        /// The cache timeout.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int CacheTimeout;

        /// <summary>
        /// The content.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string Content;

        /// <summary>
        /// The content link.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string ContentLink;

        /// <summary>
        /// The content type.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int ContentType;

        /// <summary>
        /// The customization link.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string CustomizationLink;

        /// <summary>
        /// The description.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string Description;

        /// <summary>
        /// The detail link.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string DetailLink;

        /// <summary>
        /// The frame state.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int FrameState;

        /// <summary>
        /// The has frame.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int HasFrame;

        /// <summary>
        /// The height.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string Height;

        /// <summary>
        /// The help link.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string HelpLink;

        /// <summary>
        /// The is included.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int IsIncluded;

        /// <summary>
        /// The is visible.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int IsVisible;

        /// <summary>
        /// The last modified.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string LastModified;

        /// <summary>
        /// The master part link.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string MasterPartLink;

        /// <summary>
        /// The namespace.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string Namespace;

        /// <summary>
        /// The part image large.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string PartImageLarge;

        /// <summary>
        /// The part image small.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string PartImageSmall;

        /// <summary>
        /// The part order.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int PartOrder;

        /// <summary>
        /// The part storage.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string PartStorage;

        /// <summary>
        /// The requires isolation.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int RequiresIsolation;

        /// <summary>
        /// The title.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string Title;

        /// <summary>
        /// The width.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string Width;

        /// <summary>
        /// The xsl.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string XSL;

        /// <summary>
        /// The xsl link.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public string XSLLink;

        /// <summary>
        /// The zone.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlElement]
        public int Zone;

        #endregion

        /*

		[XmlElementAttribute ]
		public string   Resource;  // Its an array thingy!!
		*/
    }

    /// <summary>
    /// WebPartParser
    ///     Added by Jakob Hansen.
    /// </summary>
    public class WebPartParser
    {
        #region Public Methods

        /// <summary>
        /// Loads the web part specified.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// A Rainbow.Framework.Helpers.WebPart value...
        /// </returns>
        public static WebPart Load(string fileName)
        {
            var context = HttpContext.Current;
            var partData = (WebPart)context.Cache[fileName];
            if (partData == null && File.Exists(fileName))
            {
                var reader = File.OpenText(fileName);

                try
                {
                    var serializer = new XmlSerializer(typeof(WebPart));
                    partData = (WebPart)serializer.Deserialize(reader);
                }
                finally
                {
                    reader.Close();
                }

                context.Cache.Insert(fileName, partData, new CacheDependency(fileName));
            }

            return partData;
        }

        #endregion
    }
}