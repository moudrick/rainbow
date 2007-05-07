using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rainbow.Core
{
	/// <summary>
	/// This class encapsulates the basic attributes of a Module, and is used
	/// by the administration pages when manipulating modules.<br/>
	/// ModuleItem implements the IComparable interface so that an ArrayList
	/// of ModuleItems may be sorted by ModuleOrder, using the 
	/// ArrayList's Sort() method.
	/// 
	/// The PortalModuleControl class defines a custom 
	/// base class inherited by all
	/// desktop portal modules within the Portal.<br/>
	/// The PortalModuleControl class defines portal 
	/// specific properties that are used by the portal framework
	/// to correctly display portal modules.
	/// </summary>
	/// <remarks>This is the "all new" RC4 PortalModuleControl, which no longer has a separate DesktopModuleTitle.</remarks>
	[History("john.mandia@whitelightsolutions.com", "2004/09/17", "Fixed path for help system")]
	[History("Jes1111", "2003/03/05", "Added ShowTitle setting - switches Title visibility on/off")]
	[History("Jes1111", "2003/04/24", "Added PortalAlias to cachekey")]
	[History("Jes1111", "2003/04/24", "Added Cacheable property")]
	[History("bja@reedtek.com", "2003/04/26", "Added support for win. mgmt min/max/close")]
	[History("david.verberckmoes@syntegra.com", "2003/06/02", "Showing LastModified date & user in a better way with themes")]
	[History("Jes1111", "2004/08/30", "All new version! No more DesktopModuleTitle.")]
	[History("Mark, John and Jose", "2004/09/08", "Corrections in constructor for detect DesignMode")]
	// history from old DesktopModuleTitle class
	[History("Nicholas Smeaton", "2004/07/24", "Added support for arrow buttons to move modules")]
	[History("jviladiu@portalServices.net", "2004/07/13", "Corrections in workflow buttons")]
	[History("gman3001", "2004/04/08", "Added support for custom buttons in the title bar, and set all undefined title bar buttons to 'rb_mod_title_btn' css-class.")]
	[History("Pekka Ylenius", "2004/11/28", "When '?' in ulr then '&' is needed not '?'")]
	[Serializable]
	public class Module : IComparable
		, ISerializable, ICloneable, IDisposable, ISearchable, IInstaller
	{
		/// <summary>
		/// _baseSettings holds datatype information
		/// </summary>
		protected Hashtable _baseSettings = new Hashtable();

        [Obsolete("Use the collection Module.Settings instead")]
		protected ModuleSettings _moduleConfiguration;
		protected int _canEdit = 0;
		protected int _canAdd = 0;
		protected int _canView = 0;
		protected int _canDelete = 0;
		protected int _canProperties = 0;
		protected int _portalID = 0;
		protected Hashtable _settings;
		protected WorkFlowVersion _version = WorkFlowVersion.Production;
		protected bool _supportsWorkflow = false;
		protected bool _cacheable = true;
		protected bool _supportsPrint = true;
		protected bool _supportsBack = false;
		protected bool _supportsEmail = false;
		//protected bool			_supportsHelp = false;
		protected bool _supportsArrows = true;
		protected ViewControlManager _vcm = null;
		protected PlaceHolder _header = new PlaceHolder();
		protected PlaceHolder _footer = new PlaceHolder();
		protected PlaceHolder _headerPlaceHolder = new PlaceHolder();

		//private PlaceHolder		_output = new PlaceHolder();

		// --  BJA Added Min/Max/Close Attributes [START]
		// Change wjanderson@reedtek.com
		// Date 25/4/2003 ( min/max./close buttons )
		// - Note : At some point you may wish to allow 
		// -        the selection of which buttons can
		// -        be displayed; right now it is all or nothing.
		// -        Also, you may wish to allow authorized close and min. 
		protected int _canMin = 0;
		protected int _canClose = 0;
		protected bool _supportsCollapseable = false;
		protected Theme currentTheme;
		public Theme CurrentTheme
		{
			get
			{
				return currentTheme;
			}
			set
			{
				currentTheme = value;
                
			}
		}

		//TODO: Change the behavior of the modules & pages
		//...need to diagram this out to ensure that the objects make sense
		//...when compared to the database

		protected int moduleOrder;
		protected String title;
		// TODO: Move pane name and pageId to an intermediate table & rearrange this.
		protected String pane;
		protected int id;
		protected int defId;
		protected int cacheTime;

		protected WorkflowState workflowStatus;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 27/2/2003
		/// <summary>
		/// WorkflowStatus
		/// </summary>
		public WorkflowState WorkflowStatus
		{
			get { return workflowStatus; }
			set { workflowStatus = value; }
		}

		protected bool supportWorkflow;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 27/2/2003
		/// <summary>
		/// SupportWorkflow
		/// </summary>
		public bool SupportWorkflow
		{
			get { return supportWorkflow; }
			set { supportWorkflow = value; }
		}

		//TODO: Add security

		/// <summary>
		/// CacheTime
		/// </summary>
		public int CacheTime
		{
			get { return cacheTime; }
			set { cacheTime = value; }
		}
		protected bool showMobile;
		// Change by Geert.Audenaert@Syntegra.Com - Date: 6/2/2003
		/// <summary>
		/// ShowMobile
		/// </summary>
		public bool ShowMobile
		{
			get { return showMobile; }
			set { showMobile = value; }
		}


		protected bool showEveryWhere;

		/// <summary>
		/// ShowEveryWhere
		/// </summary>
		public bool ShowEveryWhere
		{
			get { return showEveryWhere; }
			set { showEveryWhere = value; }
		}

		protected bool supportCollapsable;
		// Change by john.mandia@whitelightsolutions.com - Date: 5/24/2003
		/// <summary>
		/// SupportCollapsable
		/// </summary>
		public bool SupportCollapsable
		{
			get { return supportCollapsable; }
			set { supportCollapsable = value; }
		}

		/// <summary>
		/// Order
		/// </summary>
		public int Order
		{
			get { return moduleOrder; }
			set { moduleOrder = value; }
		}
		protected string desktopSrc;
		// Change by bja@reedtek.com - Date: 5/12/2003
		/// <summary>
		/// DesktopSrc
		/// </summary>
		public string DesktopSrc
		{
			get { return desktopSrc; }
			set { desktopSrc = value; }
		}

		protected bool cacheable;
		// Jes1111
		/// <summary>
		/// Is Cacheable?
		/// </summary>
		public bool Cacheable
		{
			get { return cacheable; }
			set { cacheable = value; }
		}
		protected string mobileSrc;

		/// <summary>
		/// MobileSrc
		/// </summary>
		public string MobileSrc
		{
			get { return mobileSrc; }
			set { mobileSrc = value; }
		}

		protected bool admin;

		/// <summary>
		/// Is Admin?
		/// </summary>
		public bool Admin
		{
			get { return admin; }
			set { admin = value; }
		}
		protected Guid guidID;

		/// <summary>
		/// GuidID
		/// </summary>
		public Guid GuidID
		{
			get { return guidID; }
			set { guidID = value; }
		}
		/// <summary>
		/// Title
		/// </summary>
		public String Title
		{
			get { return title; }
			set { title = value; }
		}

		/// <summary>
		/// Pane name
		/// </summary>
		public String PaneName
		{
			get { return pane; }
			set { pane = value; }
		}

		/// <summary>
		/// ID
		/// </summary>
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// Definition id
		/// </summary>
		public int ModuleDefId
		{
			get { return defId; }
			set { defId = value; }
		}

		#region "IComparable Interface Implementation"

		/// <summary>
		/// Public comparer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int CompareTo(object value)
		{
			if (value == null) return 1;

			int compareOrder = ((Module) value).Order;

			if (this.Order == compareOrder) return 0;
			if (this.Order < compareOrder) return -1;
			if (this.Order > compareOrder) return 1;
			return 0;
		}

		#endregion

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator >(Module x, Module y)
		{
			if (x == null || y == null) return true;
			return x.Order > y.Order ? true : false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator <(Module x, Module y)
		{
			if (x == null || y == null) return true;
			return x.Order < y.Order ? true : false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator >=(Module x, Module y)
		{
			if (x == null || y == null) return true;
			return x.Order >= y.Order ? true : false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Module">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator <=(Module x, Module y)
		{
			if (x == null || y == null) return true;
			return x.Order <= y.Order ? true : false;
		}

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Title;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A int value...
		/// </returns>
		public int ToInt32()
		{
			return this.Id;
		}

		#region "Object serialization/deserialization"

		/// <summary>
		///     Serialization method. Inherited from the ISerializable interface.
		/// </summary>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("moduleOrder", moduleOrder);
			info.AddValue("title", title);
			info.AddValue("pane", pane);
			info.AddValue("id", id);
			info.AddValue("defId", defId);
		}

		/// <summary>
		///     Deserialization constructor
		/// </summary>
		protected Module(SerializationInfo info, StreamingContext context)
		{
			pane = info.GetValue("pane", typeof (String)) as String;
			title = info.GetValue("title", typeof (String)) as String;
			moduleOrder = info.GetInt32("moduleOrder");
			defId = info.GetInt32("defId");
			id = info.GetInt32("id");
		}

		#endregion //(Object serialization/deserialization)

		#region "Common object operations"

		/// <summary>
		///     Overrides System.Object's virtual 'Equals' method
		/// </summary>
		public override bool Equals(object objother)
		{
			if (!(objother is Module)) return false;

			// Call our strongly-typed version of 'Equals'
			return this.Equals((Module) objother);
		}

		/// <summary>
		///     Strongly-typed version of Equals which should compare every field of the class
		/// </summary>
		public bool Equals(Module modother)
		{
			// TODO: Here you should compare each field according to the field type, like this:
			//       - for reference type fields: if ( !Object.Equals(this.[FieldNameHere], modother.[FieldNameHere])) return false;
			//       - for value type fields:     if ( !this.[FieldNameHere].Equals(modother.[FieldNameHere])) return false;

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
		public Module() : base()
		{
			Id = 0;
			PaneName = "no pane";
			Title = string.Empty;
			//			AuthorizedEditRoles = strAdmin;
			//			AuthorizedViewRoles = "All Users;";
			//			AuthorizedAddRoles = strAdmin;
			//			AuthorizedDeleteRoles = strAdmin;
			//			AuthorizedPropertiesRoles = strAdmin;
			//			AuthorizedMoveModuleRoles = strAdmin;
			//			AuthorizedDeleteModuleRoles = strAdmin;
			CacheTime = 0;
			Order = 0;
			ShowMobile = false;
			DesktopSrc = string.Empty;
			MobileSrc = string.Empty;
			SupportCollapsable = false;
			SupportWorkflow = false;
		}

		/// <summary>
		///     This method will be automatically called by the .NET garbage
		///     collector *sometime*
		/// </summary>
		~Module()
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
}