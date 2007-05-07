using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rainbow.Core;

namespace Rainbow.BusinessRules
{
	/// <summary>
	/// Holds currently active portal, page and module
	/// </summary>
	[Serializable]
	public class Active : IDisposable, ICloneable
		, ISerializable
	{
		#region Protected variables

		protected static Portal portal;
		protected static Page page;
		protected static Module module;

		protected static int currentPortalId = 0;
		protected static int currentPageId = 0;

		#endregion

		#region Public properties

		/// <summary>
		///     The currently active portal
		/// </summary>
		[Category("Active"),
			Description("Active Portal")]
		public static Portal Portal
		{
			get {
				if (portal == null)
					portal = Data.Helper.Portals.Portal(currentPortalId);

				return portal;
			}
			set { portal = value; }
		}

		/// <summary>
		///     The currently active page
		/// </summary>
		[Category("Active"),
			Description("Active Page")]
		public static Page Page
		{
			get {
				if(page == null)
					page = Data.Helper.Pages.Page(currentPageId);

				return page;
			}
			set { page = value; }
		}

		/// <summary>
		///     The currently active module
		/// </summary>
		[Category("Active"),
			Description("Active Module")]
		public static Module Module
		{
			get { return module; }
			set { module = value; }
		}

		#endregion

		#region "Common object operations"

		/// <summary>
		///     Overrides System.Object's virtual 'Equals' method
		/// </summary>
		public override bool Equals(object objother)
		{
			if (!(objother is Active)) return false;

			// Call our strongly-typed version of 'Equals'
			return this.Equals((Active) objother);
		}

		/// <summary>
		///     Strongly-typed version of Equals which should compare every field of the class
		/// </summary>
		public bool Equals(Active actother)
		{
			// TODO: Here you should compare each field according to the field type, like this:
			//       - for reference type fields: if ( !Object.Equals(this.[FieldNameHere], actother.[FieldNameHere])) return false;
			//       - for value type fields:     if ( !this.[FieldNameHere].Equals(actother.[FieldNameHere])) return false;
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

		#region "Object serialization/deserialization"

		/// <summary>
		///     Serialization method. Inherited from the ISerializable interface.
		/// </summary>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("portal", portal);
			info.AddValue("page", page);
			info.AddValue("module", module);
		}

		/// <summary>
		///     Deserialization constructor
		/// </summary>
		protected Active(SerializationInfo info, StreamingContext context)
		{
			module = info.GetValue("module", typeof (Module)) as Module;
			page = info.GetValue("page", typeof (Page)) as Page;
			portal = info.GetValue("portal", typeof (Portal)) as Portal;
		}

		#endregion //(Object serialization/deserialization)

		#region "Object construction/destruction"

		/// <summary>
		///     ctor w/all new objects
		/// </summary>
		public Active() : base()
		{
			portal = new Portal();
			page = new Page();
			module = new Module();
		}

		/// <summary>
		///     ctor w/portal and new page/module
		/// </summary>
		public Active(Portal p) : base()
		{
			portal = p;
			page = new Page();
			module = new Module();
		}

		/// <summary>
		///     ctor w/page and new portal/module
		/// </summary>
		public Active(Page pg) : base()
		{
			portal = new Portal();
			page = pg;
			module = new Module();
		}

		/// <summary>
		///     ctor w/module and new portal/page
		/// </summary>
		public Active(Module m) : base()
		{
			portal = new Portal();
			page = new Page();
			module = m;
		}

		/// <summary>
		///     ctor w/portal, page and new module
		/// </summary>
		public Active(Portal p, Page pg) : base()
		{
			portal = p;
			page = pg;
			module = new Module();
		}

		/// <summary>
		///     ctor w/portal, module and new page
		/// </summary>
		public Active(Portal p, Module m) : base()
		{
			portal = p;
			page = new Page();
			module = m;
		}

		/// <summary>
		///     ctor w/page, module and new portal
		/// </summary>
		public Active(Page pg, Module m) : base()
		{
			portal = new Portal();
			page = pg;
			module = m;
		}

		/// <summary>
		///     ctor w/portal, page, and module
		/// </summary>
		public Active(Portal p, Page pg, Module m) : base()
		{
			portal = p;
			page = pg;
			module = m;
		}

		/// <summary>
		///     This method will be automatically called by the .NET garbage
		///     collector *sometime*
		/// </summary>
		~Active()
		{
			portal = null;
			page = null;
			module = null;
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

			portal = null;
			page = null;
			module = null;
		}

		#endregion //(Object construction/destruction)
	}
}