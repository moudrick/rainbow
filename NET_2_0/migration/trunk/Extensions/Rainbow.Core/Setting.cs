using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rainbow.Core
{
	/// <summary>
	/// This class holds a single setting in the hashtable,
	/// providing information about datatype, costrains.
	/// </summary>
	[Serializable]
	public class Setting : IComparable
		, IDisposable, ICloneable, ISerializable
	{
		private Type _datatype;
		private int _minValue;
		private int _maxValue;
		private int _order = 0;
		private bool _required = false;

		//by Manu
		private string description;
		private string englishName;

		private SettingGroup group
			= new SettingGroup(SettingGroupIds.MODULE_SPECIAL_SETTINGS);

		#region ctors

		/// <summary>
		/// SettingItem
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="value"></param>
		public Setting(Type dt, object value)
		{
			_datatype = dt;
			this.Value = value;
		}

		/// <summary>
		/// SettingItem
		/// </summary>
		/// <param name="dt"></param>
		public Setting(Type dt)
		{
			_datatype = dt;
		}

		#endregion

		/// <summary>
		/// It is the name of the parameter in plain english.
		/// </summary>
		public string EnglishName
		{
			get { return englishName; }
			set { englishName = value; }
		}


		/// <summary>
		/// Provide help for parameter. 
		/// Should be a brief, descriptive text that explains what
		/// this setting should do.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}


		// Jes1111
		/// <summary>
		/// Allows grouping of settings in SettingsTable - use 
		/// Rainbow.Configuration.SettingItemGroup enum (convert to string)
		/// </summary>
		public SettingGroup Group
		{
			get { return group; }
			set { group = value; }
		}


		/// <summary>
		/// ToString converter operator
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator string(Setting value)
		{
			return (value.ToString());
		}

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (this.Value.ToString() != null)
				return this.Value as string;
			else
				return string.Empty;
		}

		/// <summary>
		/// Value
		/// </summary>
		public object Value
		{
			get { return this.Value; }
			set { this.Value = value; }
		}

//		/// <summary>
//		/// FullPath
//		/// </summary>
//		public string FullPath
//		{
//			get { return _datatype.FullPath; }
//		}

		/// <summary>
		/// Required
		/// </summary>
		public bool Required
		{
			get { return _required; }
			set { _required = value; }
		}

//		/// <summary>
//		/// DataType
//		/// </summary>
//		public PropertiesDataType DataType
//		{
//			get { return _datatype.Type; }
//		}

//		/// <summary>
//		/// EditControl
//		/// </summary>
//		public Control EditControl
//		{
//			get { return _datatype.EditControl; }
//			set { _datatype.EditControl = value; }
//		}

		/// <summary>
		/// MinValue
		/// </summary>
		public int MinValue
		{
			get { return _minValue; }
			set { _minValue = value; }
		}

		/// <summary>
		/// MaxValue
		/// </summary>
		public int MaxValue
		{
			get { return _maxValue; }
			set { _maxValue = value; }
		}

//		/// <summary>
//		/// DataSource
//		/// </summary>
//		public object DataSource
//		{
//			get { return _datatype.DataSource; }
//		}

		/// <summary>
		/// Display Order - use Rainbow.Configuration.SettingItemGroup enum 
		/// (add integer in range 1-999)
		/// </summary>
		public int Order
		{
			get { return _order; }
			set { _order = value; }
		}

		/// <summary>
		/// Public comparer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int CompareTo(object value)
		{
			if (value == null) return 1;

			int compareOrder = ((Setting) value).Order;

			if (this.Order == compareOrder) return 0;
			if (this.Order < compareOrder) return -1;
			if (this.Order > compareOrder) return 1;
			return 0;
		}

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public sealed class SettingGroup
		{
			private int id;

			#region ctors

			/// <summary>
			///     
			/// </summary>
			/// 
			/// <returns>
			///     A void value...
			/// </returns>
			public SettingGroup()
			{
				// TODO: Fill in default group info
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="gid" type="int">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			public SettingGroup(int gid)
			{
				id = gid;
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="sgid" type="Rainbow.Core.Setting.SettingGroupIds">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			public SettingGroup(SettingGroupIds sgid)
			{
				id = (int) sgid;
			}

			#endregion

			/// <summary>
			///     
			/// </summary>
			/// <value>
			///     <para>
			///         
			///     </para>
			/// </value>
			/// <remarks>
			///     
			/// </remarks>
			public int Id
			{
				get { return id; }
				set { id = value; }
			}


			/// <summary>
			///     Name within the enum for this group id
			/// </summary>
			/// <value>
			///     <para>
			///         
			///     </para>
			/// </value>
			/// <remarks>
			///     
			/// </remarks>
			public string Name
			{
				get { return SettingGroupIds.GetName(typeof (SettingGroupIds), id); }
			}


			/// <summary>
			/// It provides a description in plain English for
			/// Group Key (readonly)
			/// </summary>
			public string Description
			{
				get
				{
					switch ((SettingGroupIds) id)
					{
						case SettingGroupIds.NONE:
							return "Generic settings";

						case SettingGroupIds.THEME_LAYOUT_SETTINGS:
							return "Theme and layout settings";

						case SettingGroupIds.SECURITY_USER_SETTINGS:
							return "Users and Security settings";

						case SettingGroupIds.CULTURE_SETTINGS:
							return "Culture settings";

						case SettingGroupIds.BUTTON_DISPLAY_SETTINGS:
							return "Buttons and Display settings";

						case SettingGroupIds.MODULE_SPECIAL_SETTINGS:
							return "Specific Module settings";

						case SettingGroupIds.META_SETTINGS:
							return "Meta settings";

						case SettingGroupIds.MISC_SETTINGS:
							return "Miscellaneous settings";

						case SettingGroupIds.NAVIGATION_SETTINGS:
							return "Navigation settings";
					}
					return "Settings";
				}
			}
		}


		/// <summary>
		/// SettingItemGroups, used to sort and group site and module
		/// settings in SettingsTable. Jes1111
		/// </summary>
		public enum SettingGroupIds : int
		{
			/// <summary>
			///     
			/// </summary>
			NONE = 0,
			/// <summary>
			///     
			/// </summary>
			THEME_LAYOUT_SETTINGS = 1000,
			/// <summary>
			///     
			/// </summary>
			SECURITY_USER_SETTINGS = 2000,
			/// <summary>
			///     
			/// </summary>
			CULTURE_SETTINGS = 3000,
			/// <summary>
			///     
			/// </summary>
			BUTTON_DISPLAY_SETTINGS = 4000,
			/// <summary>
			///     
			/// </summary>
			MODULE_SPECIAL_SETTINGS = 7000,
			/// <summary>
			///     
			/// </summary>
			META_SETTINGS = 8000,
			/// <summary>
			///     
			/// </summary>
			MISC_SETTINGS = 9000,
			/// <summary>
			///     
			/// </summary>
			NAVIGATION_SETTINGS = 10000
		}

		#region "Object construction/destruction"

		/// <summary>
		///     TODO: Describe this constructor here
		/// </summary>
		public Setting() : base()
		{
			// TODO: If appropriate, add custom parameters to this constructor and
			// then add here the corresponding extra fields initialization code
		}

		/// <summary>
		///     This method will be automatically called by the .NET garbage
		///     collector *sometime*
		/// </summary>
		~Setting()
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

		#region "Common object operations"

		/// <summary>
		///     Overrides System.Object's virtual 'Equals' method
		/// </summary>
		public override bool Equals(object objother)
		{
			if (!(objother is Setting)) return false;

			// Call our strongly-typed version of 'Equals'
			return this.Equals((Setting) objother);
		}

		/// <summary>
		///     Strongly-typed version of Equals which should compare every field of the class
		/// </summary>
		public bool Equals(Setting setother)
		{
			// TODO: Here you should compare each field according to the field type, like this:
			//       - for reference type fields: if ( !Object.Equals(this.[FieldNameHere], setother.[FieldNameHere])) return false;
			//       - for value type fields:     if ( !this.[FieldNameHere].Equals(setother.[FieldNameHere])) return false;

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
			info.AddValue("_datatype", _datatype);
			info.AddValue("_minValue", _minValue);
			info.AddValue("_maxValue", _maxValue);
			info.AddValue("_order", _order);
			info.AddValue("_required", _required);
			info.AddValue("description", description);
			info.AddValue("englishName", englishName);
			info.AddValue("group", group);
		}

		/// <summary>
		///     Deserialization constructor
		/// </summary>
		protected Setting(SerializationInfo info, StreamingContext context)
		{
			this.englishName = info.GetString("englishName");
			this.group = info.GetValue("group", typeof (SettingGroup)) as SettingGroup;
			this._datatype = info.GetValue("_datatype", typeof (Type)) as Type;
			this._minValue = info.GetInt32("_minValue");
			this._maxValue = info.GetInt32("_maxValue");
			this._order = info.GetInt32("_order");
			this._required = info.GetBoolean("_required");
			this.description = info.GetString("description");
		}

		#endregion //(Object serialization/deserialization)
	}
}