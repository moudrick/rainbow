using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

namespace Rainbow.Helpers
{
	/// <summary>
	/// WebPart
	/// Added by Jakob Hansen.
	/// </summary>
	[XmlRoot(Namespace="urn:schemas-microsoft-com:webpart:", IsNullable=false)]
	public class WebPart
	{
		[XmlElement()] private int allowMinimize;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int AllowMinimize
		{
			get { return allowMinimize; }
			set { allowMinimize = value; }
		}


		[XmlElement()] private int allowRemove;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int AllowRemove
		{
			get { return allowRemove; }
			set { allowRemove = value; }
		}


		[XmlElement()] private int autoUpdate;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int AutoUpdate
		{
			get { return autoUpdate; }
			set { autoUpdate = value; }
		}


		[XmlElement()] private int cacheBehavior;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int CacheBehavior
		{
			get { return cacheBehavior; }
			set { cacheBehavior = value; }
		}


		[XmlElement()] private int cacheTimeout;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int CacheTimeout
		{
			get { return cacheTimeout; }
			set { cacheTimeout = value; }
		}


		[XmlElement()] private String content;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String Content
		{
			get { return content; }
			set { content = value; }
		}


		[XmlElement()] private String contentLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String ContentLink
		{
			get { return contentLink; }
			set { contentLink = value; }
		}


		[XmlElement()] private int contentType;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}


		[XmlElement()] private String customizationLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String CustomizationLink
		{
			get { return customizationLink; }
			set { customizationLink = value; }
		}


		[XmlElement()] private String description;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String Description
		{
			get { return description; }
			set { description = value; }
		}


		[XmlElement()] private String detailLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String DetailLink
		{
			get { return detailLink; }
			set { detailLink = value; }
		}


		[XmlElement()] private int frameState;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int FrameState
		{
			get { return frameState; }
			set { frameState = value; }
		}


		[XmlElement()] private int hasFrame;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int HasFrame
		{
			get { return hasFrame; }
			set { hasFrame = value; }
		}


		[XmlElement()] private String height;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String Height
		{
			get { return height; }
			set { height = value; }
		}


		[XmlElement()] private String helpLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String HelpLink
		{
			get { return helpLink; }
			set { helpLink = value; }
		}


		[XmlElement()] private int isIncluded;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int IsIncluded
		{
			get { return isIncluded; }
			set { isIncluded = value; }
		}


		[XmlElement()] private int isVisible;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int IsVisible
		{
			get { return isVisible; }
			set { isVisible = value; }
		}


		[XmlElement()] private String lastModified;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String LastModified
		{
			get { return lastModified; }
			set { lastModified = value; }
		}


		[XmlElement()] private String masterPartLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String MasterPartLink
		{
			get { return masterPartLink; }
			set { masterPartLink = value; }
		}


		[XmlElement()] private String _namespace;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String Namespace
		{
			get { return _namespace; }
			set { _namespace = value; }
		}


		[XmlElement()] private String partImageLarge;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String PartImageLarge
		{
			get { return partImageLarge; }
			set { partImageLarge = value; }
		}


		[XmlElement()] private String partImageSmall;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String PartImageSmall
		{
			get { return partImageSmall; }
			set { partImageSmall = value; }
		}


		[XmlElement()] private int partOrder;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int PartOrder
		{
			get { return partOrder; }
			set { partOrder = value; }
		}


		[XmlElement()] private String partStorage;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String PartStorage
		{
			get { return partStorage; }
			set { partStorage = value; }
		}


		[XmlElement()] private int requiresIsolation;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int RequiresIsolation
		{
			get { return requiresIsolation; }
			set { requiresIsolation = value; }
		}


		[XmlElement()] private String title;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String Title
		{
			get { return title; }
			set { title = value; }
		}


		[XmlElement()] private String width;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String Width
		{
			get { return width; }
			set { width = value; }
		}


		[XmlElement()] private String xSL;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String XSL
		{
			get { return xSL; }
			set { xSL = value; }
		}


		[XmlElement()] private String xSLLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public String XSLLink
		{
			get { return xSLLink; }
			set { xSLLink = value; }
		}


		[XmlElement()] private int zone;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int Zone
		{
			get { return zone; }
			set { zone = value; }
		}


		/*

		[XmlElementAttribute ]
		public String   Resource;  // Its an array thingy!!
		*/
	}

	/// <summary>
	/// WebPartParser
	/// Added by Jakob Hansen.
	/// </summary>
	public class WebPartParser
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private WebPartParser()
		{
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="fileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Rainbow.Helpers.WebPart value...
		/// </returns>
		public static WebPart Load(String fileName)
		{
			HttpContext context = HttpContext.Current;
			WebPart partData = (WebPart) context.Cache[fileName];

			if (partData == null)
			{
				if (File.Exists(fileName))
				{
					StreamReader reader = File.OpenText(fileName);

					try
					{
						XmlSerializer serializer = new XmlSerializer(typeof (WebPart));
						partData = (WebPart) serializer.Deserialize(reader);
					}

					finally
					{
						reader.Close();
					}
					context.Cache.Insert(fileName, partData, new CacheDependency(fileName));
				}
			}
			return partData;
		}
	}
}