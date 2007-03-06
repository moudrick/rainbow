using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rainbow.Core
{
    // A delegate type for hooking up change notifications.
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Page (formerly TabItem) Class
    /// This class encapsulates the basic attributes of a Tab, and is used
    /// by the administration pages when manipulating tabs.<br/>
    /// TabItem implements 
    /// the IComparable interface so that an ArrayList of TabItems may be sorted
    /// by TabOrder, using the ArrayList's Sort() method.
    /// </summary>
    [Serializable]
    public class Page : IComparable
        , ISerializable, ICloneable, IDisposable
    {
        #region Events
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event ChangedEventHandler Changed;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
        #endregion

        #region Protected variables

        protected RoleArrayList authorizedRoles;
        protected PageArrayList childPages;
        protected Hashtable customSettings; //Replace this with a setting arraylist
        protected int id;
        protected int index;
        protected string layout;
        protected string mobileName = "New Mobile Page";
        protected ModuleArrayList modules;
        protected string name = "New Page";
        protected int nestLevel;
        protected int order;
        protected int parentId;
        protected bool showMobile = true;

        #endregion

        #region Public properties

        /// <summary>
        /// Authorized Roles
        /// </summary>
        /// <remarks>
        /// This needs to be looked at again.  The security model is too
        /// basic.  We really need a system resembling ntfs file permissions.
        /// It also should really be a collection class of roles as opposed to
        /// a simple text delimited list.
        /// </remarks>
        [Category("Base Settings"),
            Description("Authorized Roles")]
        public RoleArrayList AuthorizedRoles
        {
            get { return authorizedRoles; }
            set { authorizedRoles = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        ///     Child Pages Collection
        /// </summary>
        [Category("Pages"),
            Description("Child Pages Collection")]
        public PageArrayList ChildPages
        {
            get { return childPages; }
            set { childPages = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        ///     Custom Settings
        /// </summary>
        /// <remarks>
        ///     Should move to collection of setting objects?
        ///     Why are these separate from the rest of the settings?
        /// </remarks>
        [Category("Base Settings"),
            Description("Custom Settings")]
        public Hashtable CustomSettings
        {
            get { return customSettings; }
            set { customSettings = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Page Id
        /// </summary>
        [Category("Base Settings"),
            Description("Page Id")]
        public int Id
        {
            get { return id; }
            set { id = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        ///     Index
        /// </summary>
        /// <remarks>
        /// What's this for?
        /// </remarks>
        [Category("Base Settings"),
            Description("Index")]
        public int Index
        {
            get { return index; }
            set { index = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Current Layout
        /// </summary>
        [Category("Base Settings"),
            Description("Current Layout")]
        public string Layout
        {
            get { return layout; }
            set { layout = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Mobile Tab Name
        /// </summary>
        [Category("Base Settings"),
            Description("Mobile Page Name")]
        public string MobileName
        {
            get { return mobileName; }
            set { mobileName = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Modules on this page
        /// </summary>
        [Category("Base Settings"),
            Description("Modules on this page instance")]
        public ModuleArrayList Modules
        {
            get { return modules; }
            set { modules = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        ///     Page Name
        /// </summary>
        [Category("Base Settings"),
            Description("Page Name")]
        public string Name
        {
            get { return name; }
            set { name = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Nest Level
        /// </summary>
        [Category("Base Settings"),
            Description("Nest Level")]
        public int NestLevel
        {
            get { return nestLevel; }
            set { nestLevel = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        ///     Page Order?
        /// </summary>
        [Category("Base Settings"),
            Description("Page Order?")]
        public int Order
        {
            get { return order; }
            set { order = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Parent Page Id
        /// </summary>
        /// <remarks>
        /// For this instance only.
        /// Holds this page's parent id within the current portal
        /// </remarks>
        [Category("Base Settings"),
            Description("Parent Page Id")]
        public int ParentId
        {
            get { return parentId; }
            set { parentId = value; OnChanged(EventArgs.Empty); }
        }

        /// <summary>
        ///     Show to mobile users?
        /// </summary>
        [Category("Base Settings"),
            Description("Show to mobile users?")]
        public bool ShowMobile
        {
            get { return showMobile; }
            set { showMobile = value; OnChanged(EventArgs.Empty); }
        }

        #endregion

        #region Compare functions
        #region IComparable implementation

        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            if (value == null) return 1;

            int compareOrder = ((Page)value).Order;

            if (this.Order == compareOrder) return 0;
            if (this.Order < compareOrder) return -1;
            if (this.Order > compareOrder) return 1;
            return 0;
        }

        #endregion

        /// <summary>
        ///     
        /// </summary>
        public static bool operator <(Page x, Page y)
        {
            if (x == null || y == null) return true;
            return x.Order < y.Order ? true : false;
        }

        /// <summary>
        ///     
        /// </summary>
        public static bool operator <=(Page x, Page y)
        {
            if (x == null || y == null) return true;
            return x.Order <= y.Order ? true : false;
        }

        /// <summary>
        ///     
        /// </summary>
        public static bool operator >(Page x, Page y)
        {
            if (x == null || y == null) return true;
            return x.Order > y.Order ? true : false;
        }

        /// <summary>
        ///     
        /// </summary>
        public static bool operator >=(Page x, Page y)
        {
            if (x == null || y == null) return true;
            return x.Order >= y.Order ? true : false;
        }

        #endregion

        #region Conversion functions

        /// <summary>
        /// ToInt32
        /// </summary>
        public int ToInt32()
        {
            return this.Id;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region "Object serialization/deserialization"

        /// <summary>
        ///     Serialization method. Inherited from the ISerializable interface.
        /// </summary>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("authorizedRoles", authorizedRoles);
            info.AddValue("childPages", childPages);
            info.AddValue("customSettings", customSettings);
            info.AddValue("id", id);
            info.AddValue("index", index);
            info.AddValue("layout", layout);
            info.AddValue("mobileName", mobileName);
            info.AddValue("modules", modules);
            info.AddValue("name", name);
            info.AddValue("nestLevel", nestLevel);
            info.AddValue("order", order);
            info.AddValue("parentId", parentId);
            info.AddValue("showMobile", showMobile);
        }

        /// <summary>
        ///     Deserialization constructor
        /// </summary>
        protected Page(SerializationInfo info, StreamingContext context)
        {
            this.order = info.GetInt32("order");
            this.parentId = info.GetInt32("parentId");
            this.id = info.GetInt32("id");
            this.name = info.GetString("name");
            this.authorizedRoles = info.GetValue("authorizedRoles", typeof(RoleArrayList)) as RoleArrayList;
            this.mobileName = info.GetString("mobileName");
            this.modules = info.GetValue("modules", typeof(ModuleArrayList)) as ModuleArrayList;
            this.nestLevel = info.GetInt32("nestLevel");
            this.index = info.GetInt32("index");
            this.showMobile = info.GetBoolean("showMobile");
            this.customSettings = info.GetValue("customSettings", typeof(Hashtable)) as Hashtable;
            this.layout = info.GetString("layout");
            this.childPages = info.GetValue("childPages", typeof(PageArrayList)) as PageArrayList;
        }

        #endregion //(Object serialization/deserialization)

        #region "Common object operations"

        /// <summary>
        ///     Overrides System.Object's virtual 'Equals' method
        /// </summary>
        public override bool Equals(object objother)
        {
            if (!(objother is Page)) return false;

            // Call our strongly-typed version of 'Equals'
            return this.Equals((Page)objother);
        }

        /// <summary>
        ///     Strongly-typed version of Equals which should compare every field of the class
        /// </summary>
        public bool Equals(Page pagother)
        {
            // TODO: Here you should compare each field according to the field type, like this:
            //       - for reference type fields: if ( !Object.Equals(this.[FieldNameHere], pagother.[FieldNameHere])) return false;
            //       - for value type fields:     if ( !this.[FieldNameHere].Equals(pagother.[FieldNameHere])) return false;

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
        public Page()
            : base()
        {
            // TODO: If appropriate, add custom parameters to this constructor and
            // then add here the corresponding extra fields initialization code
        }

        /// <summary>
        ///     This method will be automatically called by the .NET garbage
        ///     collector *sometime*
        /// </summary>
        ~Page()
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