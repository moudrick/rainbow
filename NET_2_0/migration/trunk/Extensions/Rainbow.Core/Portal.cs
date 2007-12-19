using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml.Serialization;
using Rainbow.Scheduler;

namespace Rainbow.Core
{
	/// <summary>
	/// Each portal/site has a portal object.
	/// 
	/// PortalSettings Class encapsulates all of the settings 
	/// for the Portal, as well as the configuration settings required 
	/// to execute the current tab view within the portal.
	/// 
	/// This class encapsulates the basic attributes of a Portal, and is used
	/// by the administration pages when manipulating Portals.  PortalItem implements 
	/// the IComparable interface so that an ArrayList of PortalItems may be sorted
	/// by PortalOrder, using the ArrayList's Sort() method.
	/// </summary>
	[History("bill@improvdesign.com", "2005/01/08", "Moved PortalSettings into this object class and into business layers")]
	[History("gman3001", "2004/09/29", "Added the GetCurrentUserProfile method to obtain a hashtable of the current user's profile details.")]
	[History("jviladiu@portalServices.net", "2004/08/19", "Add support for move & delete module roles")]
	[History("jviladiu@portalServices.net", "2004/07/30", "Added new ActiveModule property")]
	[History("Jes1111", "2003/03/09", "Added new ShowTabs property")]
	[History("Jes1111", "2003/04/02", "Added new PagesXml property (an XPathDocument)")]
	[History("Thierry", "2003/04/12", "Added SecurePath property")]
	[History("Jes1111", "2003/04/17", "Added new language-related properties and methods")]
	[History("Jes1111", "2003/04/23", "Corrected string comparison case problem in language settings")]
	[History("cisakson@yahoo.com", "2003/04/28", "Added a custom setting for Windows users to assign a portal Admin")]
	[Serializable]
	[Category("Data"),
		Description("Portal Id"),
		DefaultProperty("Id")]
	public class Portal : IComparable
		, ISerializable, ICloneable, IDisposable
	{
		#region Public properties

		/// <summary>
		/// Active Directory User Name
		/// </summary>
		[Category("Active Directory"),
			Description("Active Directory User Name")]
		public string ADUserName
		{
			get { return adUserName; }
			set { adUserName = value; }
		}

		/// <summary>
		/// Active Directory Password
		/// </summary>
		[Category("Active Directory"),
			Description("Active Directory Password")]
		public string ADUserPassword
		{
			get { return adUserPassword; }
			set { adUserPassword = value; }
		}

		/// <summary>
		/// Portal Alias
		/// </summary>
		[Category("Base Settings"),
			Description("Portal Alias")]
		public string Alias
		{
			get { return alias; }
			set { alias = value; }
		}

		/// <summary>
		///     Custom Settings
		/// </summary>
		[Category("Custom Settings"),
			Description("Custom Settings")]
		public Hashtable CustomSettings
		{
			// TODO: Fix me to use a Setting collection
			get { return customSettings; }
			set { customSettings = value; }
		}

		/// <summary>
		/// Enable statistical monitoring of logon/off
		/// </summary>
		[Category("Statistics"),
			Description("Enable Monitoring"),
			DefaultValue(true)]
		public bool EnableMonitoring
		{
			get { return enableMonitoring; }
			set { enableMonitoring = value; }
		}

		/// <summary>
		/// Portal Id
		/// </summary>
		[Category("Base Settings"),
			Description("Portal Id"),
			DefaultValue(0)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// Ignore first domain for portal alias?
		/// </summary>
		[Category("Alias"),
			Description("Ignore first domain for portal alias?"),
			DefaultValue(true)]
		public bool IgnoreFirstDomain
		{
			get { return ignoreFirstDomain; }
			set { ignoreFirstDomain = value; }
		}

		/// <summary>
		/// Current Layout
		/// </summary>
		/// <remarks>
		/// Patch for possible .NET framework bug
		/// if returned an empy string caused an endless loop
		/// Manu version 
		/// </remarks>
		[Category("Base Settings"),
			Description("Current Layout"),
			DefaultValue("Default")]
		public string Layout
		{
			get
			{
				if (currentLayout != string.Empty && currentLayout != null)
					return currentLayout;

				else
					return "Default";
			}
			set { currentLayout = value; }
		}

		/// <summary>
		/// Gets a PageArrayList of all the pages in Pages property with ShowMobile marked true
		/// </summary>
		[Category("Pages"),
			Description("Mobile Pages")]
		public PageArrayList MobilePages
		{
			get
			{
				PageArrayList mobilePages = new PageArrayList();

				foreach (Page p in Pages)
				{
					if (p.ShowMobile)
						mobilePages.Add(p);
				}
				return mobilePages;
			}
		}

		/// <summary>
		/// Portal Name
		/// </summary>
		[Localizable(true)]
		[Category("Base Settings"),
			Description("Portal Name"),
			DefaultValue("Rainbow Portal")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Pages in this portal instance
		/// </summary>
		[Category("Pages"),
			Description("Pages in this portal instance")]
		public PageArrayList Pages
		{
			// TODO: Check this check for showpages to see exactly what it is
			get { return (showPages ? pages : new PageArrayList()); }
			set { pages = value; }
		}

		/// <summary>
		/// PortalPath.
		/// Base dir for all portal data, relative to application
		/// </summary>
		/// <remarks>
		/// by manu
		/// be sure it starts with "/"
		///		if (_portalPath.Length > 0 && !_portalPath.StartsWith("/"))
		///			_portalPath = Rainbow.Settings.Path.WebPathCombine("/", _portalPath);
		/// </remarks>
		[Category("Base Settings"),
			Description("Base dir for all portal data, relative to application")]
		public string Path
		{
			get { return path; }
			set { path = value; }
		}


		/// <summary>
		/// Remove WWW from beginning of url to find portal alias?
		/// </summary>
		[Category("Alias"),
			Description("Remove WWW from beginning of url to find portal alias?"),
			DefaultValue(true)]
		public bool RemoveWWW
		{
			get { return removeWWW; }
			set { removeWWW = value; }
		}

		/// <summary>
		/// Show Pages?
		/// </summary>
		/// <remarks>
		/// Jes
		/// </remarks>
		[Category("Pages"),
			Description("Show Pages?"),
			DefaultValue(true)]
		public bool ShowPages
		{
			get { return showPages; }
			set { showPages = value; }
		}

		/// <summary>
		/// SMTP Server
		/// </summary>
		[Category("Base Settings"),
			Description("SMTP Server used for sending email"),
			DefaultValue("localhost")]
		public string SmtpServer
		{
			get { return smtpServer; }
			set { smtpServer = value; }
		}

		/// <summary>
		/// Title of the portal
		/// </summary>
		[Category("Base Settings"),
			Description("Title of the portal"),
			DefaultValue("Rainbow Portal")]
		[Localizable(true)]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		/// <summary>
		///     
		/// </summary>
		[Category("Localization"),
			Description("UI Language/Culture")]
		public CultureInfo UILanguage
		{
			get { return Thread.CurrentThread.CurrentUICulture; }
			set { Thread.CurrentThread.CurrentUICulture = value; }
		}

		/// <summary>
		/// If true all users will be loaded from portal 0 instance
		/// </summary>
		[Category("Base Settings"),
			Description("If true all users will be loaded from portal 0 instance")]
		public bool UseSingleUserBase
		{
			// TODO: Look into putting this somewhere else more appropriate.
			// TODO: Maybe move it to the array or to active?
			get { return useSingleUserBase; }
			set { useSingleUserBase = value; }
		}

		public string CurrentThemeDefault
		{
			get { return currentThemeDefault; }
			set { currentThemeDefault = value; }
		}

		public string CurrentThemeAlt
		{
			get { return currentThemeAlt; }
			set { currentThemeAlt = value; }
		}

		#endregion

		#region Protected variables

		protected string adUserName;
		protected string adUserPassword;
		protected string alias = "rainbow";
		protected string currentLayout = "Default";
		protected string currentThemeAlt = "DefaultAlt";
		protected string currentThemeDefault = "Default";
		protected Hashtable customSettings = new Hashtable();
		protected bool enableMonitoring = true;
		protected int id;
		protected bool ignoreFirstDomain = true;
		protected string name = "Rainbow Portal";
		protected PageArrayList pages = new PageArrayList();
		protected string path;
		protected bool removeWWW = true;
		protected bool showPages = true;
		protected string smtpServer = "localhost";
		protected string title = "Rainbow Portal";
		protected bool useSingleUserBase = false;

		#endregion

		#region Public fields

		private static IScheduler scheduler;

		public static IScheduler Scheduler
		{
			get { return scheduler; }
			set { scheduler = value; }
		}

		#endregion

		#region Conversion functions

		public int ToInt32()
		{
			return this.Id;
		}

		public override string ToString()
		{
			return this.Name;
		}

		#endregion

		#region IComparable implementation

		/// <summary>
		/// Public comparer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IComparable.CompareTo(object value)
		{
			return this.CompareTo((Portal) value);
		}

		public int CompareTo(int value)
		{
			return this.CompareTo(value);
		}

		public int CompareTo(Portal value)
		{
			return this.CompareTo(value);
		}

		public int CompareTo(string value)
		{
			return this.CompareTo(value);
		}


		public static bool operator <(Portal x, Portal y)
		{
			if (x == null || y == null) return true;
			return x.Id < y.Id ? true : false;
		}

		public static bool operator >(Portal x, Portal y)
		{
			if (x == null || y == null) return true;
			return x.Id > y.Id ? true : false;
		}

		#endregion

		#region "Object serialization/deserialization"

		/// <summary>
		///     Serialization method. Inherited from the ISerializable interface.
		/// </summary>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("adUserName", adUserName);
			info.AddValue("adUserPassword", adUserPassword);
			info.AddValue("alias", alias);
			info.AddValue("currentLayout", currentLayout);
			info.AddValue("CurrentThemeAlt", CurrentThemeAlt);
			info.AddValue("CurrentThemeDefault", CurrentThemeDefault);
			info.AddValue("customSettings", customSettings);
			info.AddValue("enableMonitoring", enableMonitoring);
			info.AddValue("id", id);
			info.AddValue("ignoreFirstDomain", ignoreFirstDomain);
			info.AddValue("name", name);
			info.AddValue("pages", pages);
			info.AddValue("path", path);
			info.AddValue("removeWWW", removeWWW);
			info.AddValue("showPages", showPages);
			info.AddValue("smtpServer", smtpServer);
			info.AddValue("title", title);
			info.AddValue("useSingleUserBase", useSingleUserBase);
			info.AddValue("Scheduler", Scheduler);
		}

		/// <summary>
		///     Deserialization constructor
		/// </summary>
		protected Portal(SerializationInfo info, StreamingContext context)
		{
			this.removeWWW = info.GetBoolean("removeWWW");
			this.currentLayout = info.GetString("currentLayout");
			this.path = info.GetString("path");
			this.showPages = info.GetBoolean("showPages");
			this.CurrentThemeAlt = info.GetValue("CurrentThemeAlt", typeof (string)) as string;
			this.CurrentThemeDefault = info.GetValue("CurrentThemeDefault", typeof (string)) as string;
			this.customSettings = info.GetValue("customSettings", typeof (Hashtable)) as Hashtable;
			this.title = info.GetString("title");
			this.pages = info.GetValue("pages", typeof (PageArrayList)) as PageArrayList;
			this.smtpServer = info.GetString("smtpServer");
			this.enableMonitoring = info.GetBoolean("enableMonitoring");
			this.id = info.GetInt32("id");
			this.useSingleUserBase = info.GetBoolean("useSingleUserBase");
			scheduler = info.GetValue("Scheduler", typeof (IScheduler)) as IScheduler;
			this.ignoreFirstDomain = info.GetBoolean("ignoreFirstDomain");
			this.adUserName = info.GetString("adUserName");
			this.name = info.GetString("name");
			this.adUserPassword = info.GetString("adUserPassword");
			this.alias = info.GetString("alias");
		}

		#endregion //(Object serialization/deserialization)

		#region "Common object operations"

		/// <summary>
		///     Overrides System.Object's virtual 'Equals' method
		/// </summary>
		public override bool Equals(object objother)
		{
			if (!(objother is Portal)) return false;

			// Call our strongly-typed version of 'Equals'
			return this.Equals((Portal) objother);
		}

		/// <summary>
		///     Strongly-typed version of Equals which should compare every field of the class
		/// </summary>
		public bool Equals(Portal porother)
		{
			// TODO: Here you should compare each field according to the field type, like this:
			//       - for reference type fields: if ( !Object.Equals(this.[FieldNameHere], porother.[FieldNameHere])) return false;
			//       - for value type fields:     if ( !this.[FieldNameHere].Equals(porother.[FieldNameHere])) return false;

			return true;
		}

		/// <summary>
		///     Overrides System.Object's GetHashCode method
		/// </summary>
		public override int GetHashCode()
		{
			// TODO: Provide here an algorithm to compute the hash value for the object
			// Please note that equal objects should have the same hash value

			// TODO: Update the following line in order to return the computed hash value
			return 0;
		}

		/// <summary>
		///     Clone Portal object
		/// </summary>
		/// <remarks>
		///     Supports deep cloning
		/// </remarks>
		public Portal Clone()
		{
			MemoryStream membuffer = new MemoryStream();
			BinaryFormatter binserializer =
				new BinaryFormatter(
					null,
					new StreamingContext(
						StreamingContextStates.Clone));
			Portal objitemClone;

			// Serialize the object into the memory stream
			binserializer.Serialize(membuffer, this);

			// Move the stream pointer to the beginning of the memory stream
			membuffer.Seek(0, SeekOrigin.Begin);

			// Get the serialized object from the memory stream
			objitemClone = (Portal) binserializer.Deserialize(membuffer);

			// Release the memory stream
			membuffer.Close();

			// Return the deeply cloned object
			return objitemClone;
		}

		/// <summary>
		///     Inherited from the ICloneable interface
		/// </summary>
		/// <remarks>
		///     Supports deep cloning
		/// </remarks>
		object ICloneable.Clone()
		{
			MemoryStream membuffer = new MemoryStream();
			BinaryFormatter binserializer =
				new BinaryFormatter(
					null,
					new StreamingContext(
						StreamingContextStates.Clone));
			object objitemClone;

			// Serialize the object into the memory stream
			binserializer.Serialize(membuffer, this);

			// Move the stream pointer to the beginning of the memory stream
			membuffer.Seek(0, SeekOrigin.Begin);

			// Get the serialized object from the memory stream
			objitemClone = binserializer.Deserialize(membuffer);

			// Release the memory stream
			membuffer.Close();

			// Return the deeply cloned object
			return objitemClone;
		}

		#endregion //(Common object operations)

		#region "Object construction/destruction"

		/// <summary>
		///     TODO: Describe this constructor here
		/// </summary>
		public Portal() : base()
		{
			// TODO: If appropriate, add custom parameters to this constructor and
			// then add here the corresponding extra fields initialization code
		}

		/// <summary>
		///     This method will be automatically called by the .NET garbage
		///     collector *sometime*
		/// </summary>
		~Portal()
		{
			// TODO: Add object (undeterministic) destruction code here

		}

		/// <summary>
		///     Inherited from IDisposable
		/// </summary>
		/// <remarks>
		///     This method will *not* be called automatically. You will have
		///     to place an explicit call to it (either directly or through
		///     IDisposable) in your code when you want to execute the cleanup
		///     it performs.
		/// </remarks>
		public void Dispose()
		{
			GC.SuppressFinalize(this);

			// TODO: Add object disposal code here
		}

		#endregion //(Object construction/destruction)
	}

	/// <summary>
	/// This is a temporary class to support PagesXml.
	/// Since PortalSettings.DesktopTab is not directly serializable,
	/// GetPagesXml uses this class to get the job done.
	/// </summary>
	/// <remarks>
	/// Jes1111
	/// </remarks>
	[Serializable]
	[Obsolete("This shouldn't be needed after reorganization.")]
	public class MenuData
	{
		[XmlElement("MenuGroup")] private Object pagesBox;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public Object PagesBox
		{
			get { return pagesBox; }
			set { pagesBox = value; }
		}

	}

	//	public enum RegisterType : int
	//	{
	//		Simple = 0,
	//		Full = 1
	//	}
}