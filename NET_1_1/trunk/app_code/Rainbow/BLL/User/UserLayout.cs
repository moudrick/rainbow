using System;
using System.Collections;
using System.Runtime.Serialization;
using Rainbow.BLL.UserConfig;
//===============================================================================
	//
	//	Business Logic Layer
	//
	//	Rainbow.BLL.User
	//
	//
	// Classes to Manage the User Windows' information
	//
	//===============================================================================
	//
	// Created By : bja@reedtek.com Date: 26/04/2003
	//===============================================================================
namespace Rainbow.BLL.User
{
	///----------------------------------------------------------------------------
	/// <summary>
	/// Manages the user layout information
	/// </summary>
	///----------------------------------------------------------------------------
	[Serializable()]  
	public class UserLayoutMgr :  ISerializable 
	{

		#region Private Data

		Hashtable modules_ = null;

		#endregion

		#region ctors

		/// <summary>
		/// User layout manager
		/// </summary>
		public UserLayoutMgr()
		{
			modules_ = new Hashtable();
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public UserLayoutMgr (SerializationInfo info, StreamingContext context) 
		{
			SerializationInfoEnumerator  
				infoItems = info.GetEnumerator();
			modules_ = new Hashtable();

			// iterate over data
			while(infoItems.MoveNext()) 
			{
				// set the module data
				modules_.Add(Int32.Parse(infoItems.Name),infoItems.Value);
			}
		} // end of ctor

		#endregion

		#region Public Methods

		/// <summary>
		/// Serialization function.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public  void GetObjectData(SerializationInfo info, StreamingContext context)
		{

			// any data to save
			if ( modules_ != null )
			{

				// iterate over data to save
				foreach(int id in modules_.Keys) 
				{

					try 
					{
						info.AddValue(id.ToString(),modules_[id]);
					} 

					catch(Exception){}
				}
			}
		} // end of GetObjectData

		/// <summary>
		/// Get the modules in the user layout
		/// </summary>
		public Hashtable  Modules
		{
			get { return modules_;}
		}

		/// <summary>
		/// Find a module from the module id
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		public UserModuleSettings UserLayout(int moduleID)
		{			
			return (UserModuleSettings)modules_[moduleID];
		}

		/// <summary>
		/// Finf a module from the module id
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		/// 
		public UserModuleSettings this[int moduleID]
		{
			get {return (UserModuleSettings)modules_[moduleID];}
		}

		/// <summary>
		/// Conatins a module from the module id
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		/// 
		public bool Contains(int moduleID)
		{
			return modules_.Contains(moduleID);
		}

		/// <summary>
		/// Add/Change a layout, copy over the old one if it exists
		/// </summary>
		/// <param name="layout"></param>
		public void Add( UserModuleSettings layout)
		{

			// replacing
			if ( modules_[layout.ModuleID] != null)
				modules_[layout.ModuleID] = layout;

			else
				modules_.Add(layout.ModuleID,layout);
		}

		/// <summary>
		/// Remove a layout
		/// </summary>
		/// <param name="moduleID"></param>
		public void Remove(int moduleID)
		{
			// remove entry
			modules_.Remove(moduleID);
		}

		#endregion

	} // end of UserLayoutMgr
}
