using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Rainbow.Framework.BLL.Base;
using Rainbow.Framework.BLL.UserConfig;
using Rainbow.Framework.BLL.Utils;
//===============================================================================
//	Business Logic Layer
//
//	Rainbow.Framework.BLL.User
//
// Class to manage the User's Session Information
//===============================================================================
//
// Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
//
//	 Business Logic Layer
//
// Encapsulates the detailed settings for a specific User Session
// 
// Caveat :
//      Serializing/Deserializing adds time. 
//===============================================================================
//
// Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
namespace Rainbow.Framework.BLL.User
{
	/// <summary>
	/// Summary description for UserWindowMgmt.
	/// Server-side state management of user layout
	/// </summary>
	public class UserWindowMgmt : BLLBase
	{

		/// <summary>
		/// Ctor of the user session
		/// </summary>
		/// <param name="uid">The uid.</param>
		public UserWindowMgmt(string uid)
		{
			string key = makeKey(uid);
			// our bag holder
			IWebBagHolder user_bag = BagFactory.instance.create();
			// remember the user id;
			uid_ = uid;
			// get the user layout information, if any
			ul_mgr_ = this.deserialize(key, ref user_bag);

			// is it an annonymous user
			if (ul_mgr_ == null)
			{
				// go create one for the user
				this.addSessionUser(uid, ref user_bag);
			}
		} // end of ctor

		#region Attributes

		/// <summary>
		/// Get list of modules
		/// </summary>
		/// <value>The modules.</value>
		public Hashtable Modules
		{
			get { return this.ul_mgr_.Modules; }
		} // end of Modules

		/// <summary>
		/// Get the user layout manager
		/// </summary>
		/// <value>The layout manager.</value>
		public UserLayoutMgr LayoutManager
		{
			get { return this.ul_mgr_; }
		} // end of LayoutManager

		#endregion

		#region Public Methods

		/// <summary>
		/// Does this module/key exists
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(int key)
		{
			return this.ul_mgr_.Contains(key);
		} // end of Contains

		/// <summary>
		/// Add a module setting
		/// </summary>
		/// <param name="ums">The ums.</param>
		public void Add(UserModuleSettings ums)
		{
			this.ul_mgr_.Add(ums);
			// serialize the data
			this.Serialize();
		} // end of Add

		/// <summary>
		/// Default property for accessing resources
		/// <param name="index">
		/// Index for the desired resource</param>
		/// 	<returns>
		/// Resource string value</returns>
		/// </summary>
		/// <value></value>
		public UserModuleSettings this[int index]
		{
			get
			{
				return this.ul_mgr_[index];
			}
			set
			{
				this.ul_mgr_.Add(value);
			}
		}

		/// <summary>
		/// Remove module
		/// </summary>
		/// <param name="id">The id.</param>
		public void Remove(int id)
		{
			this.ul_mgr_.Remove(id);
			// serialize the data
			this.Serialize();
		} // end of Remove

		#endregion

		#region Protected Modules

		/// <summary>
		/// Serialize data
		/// </summary>
		protected void Serialize()
		{
			// our bag holder
			IWebBagHolder user_bag = BagFactory.instance.create();
			string key = makeKey(uid_);
			// serialize the data
			this.serialize(key, ref user_bag);
		} // end of Serialize

		#endregion

		#region Private Modules

		/// <summary>
		/// Add user information layout to the session
		/// </summary>
		/// <param name="uid">user id</param>
		/// <param name="user_bag">Session Bag</param>
		private void addSessionUser(string uid, ref IWebBagHolder user_bag)
		{
			string key = makeKey(uid);
			// create the user layout
			this.ul_mgr_ = new UserLayoutMgr();
			// serialize the data
			this.serialize(uid, ref user_bag);
		} // end of addSessionUser

		/// <summary>
		/// Create a user id key  for the session
		/// </summary>
		/// <param name="uid">user id</param>
		/// <returns>a user id string</returns>
		private string makeKey(string uid)
		{
			return UKEY_ + uid + "_";
		} // end of makeKey

		/// <summary>
		/// Serialize the user data
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="user_bag">The user_bag.</param>
		private void serialize(string key, ref IWebBagHolder user_bag)
		{
			// goin into critical area
			lock (this.ul_mgr_)
			{

				using (MemoryStream ms = new MemoryStream())
				{
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(ms, this.ul_mgr_);
					ms.Position = 0;
					// add it to the bag
					user_bag[key] = Convert.ToBase64String(ms.ToArray());//ms.ToArray();//this.ul_mgr_;
				}
			}
		} // end of serialize

		/// <summary>
		/// Deserialize the data to get the layout mgr.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="user_bag">The user_bag.</param>
		/// <returns></returns>
		private UserLayoutMgr deserialize(string key, ref IWebBagHolder user_bag)
		{
			// get the user layout information, if any
			string sdata = (string)user_bag[key];

			// any data
			if (sdata != null && sdata.Length > 0)
			{
				// convert back to byte
				byte[] data = Convert.FromBase64String(sdata);

				// create the memory stream to serialize
				using (MemoryStream ms = new MemoryStream())
				{
					// write data to the stream
					ms.Write(data, 0, data.Length);
					// formatter 
					BinaryFormatter bf = new BinaryFormatter();
					// set stream position to start
					ms.Position = 0;
					// deserialize data
					return (UserLayoutMgr)bf.Deserialize(ms);
				}
			}
			return null;
		} // end of deserialize

		#endregion

		/// <summary>
		///  Private Data Section
		/// </summary>
		#region Private Data

		private const string UKEY_ = "User_";
		private const string UDATAKEY_ = "_UserData_";
		private string uid_ = "-1";
		private UserLayoutMgr ul_mgr_ = null;

		#endregion

	}
}
