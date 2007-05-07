
		
//========================================================================
// This file was generated using the MyGeneration tool in combination
// with the Gentle.NET Business Entity template, $Rev: 44 $
//========================================================================
using System;
using System.Collections;
using Gentle.Framework;

namespace Rainbow.Data.GentleNET
{
	#region rb_Monitoring
	/// <summary>
	/// This object represents the properties and methods of a Employee.
	/// </summary>
	[Serializable]
	[TableName("rb_Monitoring")]
	public class rb_Monitoring : Persistent
	{
		#region Members
		private bool _changed = false;
		private static bool invalidatedListAll = true;
		private static ArrayList listAllCache = null;
		[TableColumn("ID", NotNull=true)]
		protected int iD;
		[TableColumn("UserID")]
		protected int userID;
		[TableColumn("PortalID")]
		protected int portalID;
		[TableColumn("PageID")]
		protected int pageID;
		[TableColumn("ActivityTime")]
		protected DateTime activityTime;
		[TableColumn("ActivityType")]
		protected string activityType;
		[TableColumn("Referrer")]
		protected string referrer;
		[TableColumn("UserAgent")]
		protected string userAgent;
		[TableColumn("UserHostAddress")]
		protected string userHostAddress;
		[TableColumn("BrowserType")]
		protected string browserType;
		[TableColumn("BrowserName")]
		protected string browserName;
		[TableColumn("BrowserVersion")]
		protected string browserVersion;
		[TableColumn("BrowserPlatform")]
		protected string browserPlatform;
		[TableColumn("BrowserIsAOL")]
		protected bool browserIsAOL;
		[TableColumn("UserField")]
		protected string userField;
		#endregion
			

		#region Constructors
	

		/// <summary> 
		/// Create a new object using the minimum required information (all not-null fields except 
		/// auto-generated primary keys). 
		/// </summary> 
		public rb_Monitoring( 
)
		{
			_changed = true;
			invalidatedListAll = true;
			iD = 0;
		}

			
		/// <summary> 
		/// Create an object from an existing row of data. This will be used by Gentle to 
		/// construct objects from retrieved rows. 
		/// </summary> 
		public rb_Monitoring( 
				int ID, 
				int UserID, 
				int PortalID, 
				int PageID, 
				DateTime ActivityTime, 
				string ActivityType, 
				string Referrer, 
				string UserAgent, 
				string UserHostAddress, 
				string BrowserType, 
				string BrowserName, 
				string BrowserVersion, 
				string BrowserPlatform, 
				bool BrowserIsAOL, 
				string UserField)
		{
			iD = ID;
			userID = UserID;
			portalID = PortalID;
			pageID = PageID;
			activityTime = ActivityTime;
			activityType = ActivityType;
			referrer = Referrer;
			userAgent = UserAgent;
			userHostAddress = UserHostAddress;
			browserType = BrowserType;
			browserName = BrowserName;
			browserVersion = BrowserVersion;
			browserPlatform = BrowserPlatform;
			browserIsAOL = BrowserIsAOL;
			userField = UserField;
		}

		#endregion

		#region Public Properties
		
		public bool Changed
		{ get { return _changed; } }
		
		public int ID
		{
			get{ return iD; }
		}
		
		public int UserID
		{
			get{ return userID; }
			set{ _changed |= userID != value; userID = value; invalidatedListAll =  _changed;}
		}
		
		public int PortalID
		{
			get{ return portalID; }
			set{ _changed |= portalID != value; portalID = value; invalidatedListAll =  _changed;}
		}
		
		public int PageID
		{
			get{ return pageID; }
			set{ _changed |= pageID != value; pageID = value; invalidatedListAll =  _changed;}
		}
		
		public DateTime ActivityTime
		{
			get{ return activityTime; }
			set{ _changed |= activityTime != value; activityTime = value; invalidatedListAll =  _changed;}
		}
		
		public string ActivityType
		{
			get{ return activityType != null ?activityType.TrimEnd() : null; }
			set{ _changed |= activityType != value; activityType = value; invalidatedListAll =  _changed;}
		}
		
		public string Referrer
		{
			get{ return referrer != null ?referrer.TrimEnd() : null; }
			set{ _changed |= referrer != value; referrer = value; invalidatedListAll =  _changed;}
		}
		
		public string UserAgent
		{
			get{ return userAgent != null ?userAgent.TrimEnd() : null; }
			set{ _changed |= userAgent != value; userAgent = value; invalidatedListAll =  _changed;}
		}
		
		public string UserHostAddress
		{
			get{ return userHostAddress != null ?userHostAddress.TrimEnd() : null; }
			set{ _changed |= userHostAddress != value; userHostAddress = value; invalidatedListAll =  _changed;}
		}
		
		public string BrowserType
		{
			get{ return browserType != null ?browserType.TrimEnd() : null; }
			set{ _changed |= browserType != value; browserType = value; invalidatedListAll =  _changed;}
		}
		
		public string BrowserName
		{
			get{ return browserName != null ?browserName.TrimEnd() : null; }
			set{ _changed |= browserName != value; browserName = value; invalidatedListAll =  _changed;}
		}
		
		public string BrowserVersion
		{
			get{ return browserVersion != null ?browserVersion.TrimEnd() : null; }
			set{ _changed |= browserVersion != value; browserVersion = value; invalidatedListAll =  _changed;}
		}
		
		public string BrowserPlatform
		{
			get{ return browserPlatform != null ?browserPlatform.TrimEnd() : null; }
			set{ _changed |= browserPlatform != value; browserPlatform = value; invalidatedListAll =  _changed;}
		}
		
		public bool BrowserIsAOL
		{
			get{ return browserIsAOL; }
			set{ _changed |= browserIsAOL != value; browserIsAOL = value; invalidatedListAll =  _changed;}
		}
		
		public string UserField
		{
			get{ return userField != null ?userField.TrimEnd() : null; }
			set{ _changed |= userField != value; userField = value; invalidatedListAll =  _changed;}
		}
		
	
		// generate a static property to retrieve all instances of a class that are stored in the database
		static public IList ListAll
		{
			get 
			{ 
				if( listAllCache == null || invalidatedListAll )
				{
					listAllCache = Broker.RetrieveList( typeof(rb_Monitoring) ) as ArrayList;
					invalidatedListAll = false;
				}
				return listAllCache;
			}
		}
		
		#endregion

		#region Debug Properties
		/// <summary>
		/// Generated only when genDebug flag enabled in MyGeneration template UI
		/// Returns number of items in internal cache
		/// </summary>
		public static int CacheCount
		{
			get{ return listAllCache == null ? 0 : listAllCache.Count;}
		}
		
		#endregion

		#region Storage and retrieval
		//Gentle.NET Business Entity script: Table associated with this class has no primary key, so no simple retrieve function is generated
		//Gentle.NET Business Entity script: Generation of complex retrieve function (multiple primary keys) is not implemented yet.

		public override void Persist()
		{
			if( Changed || !IsPersisted )
			{
				base.Persist();
				_changed=false;
			}
		}

		public override void Remove()
		{
			base.Remove();
			invalidatedListAll = true;
		}
		#endregion

		#region Relations
		// List of primary keys for this class table
		// List of foreign keys for this class table
		// List of selected relation tables for this database
		// Table: rb_Announcements
		// Table: rb_Announcements_st
		// Table: rb_Articles
		// Table: rb_Blacklist
		// Table: rb_BlogComments
		// Table: rb_Blogs
		// Table: rb_BlogStats
		// Table: rb_BookList
		// Table: rb_ComponentModule
		// Table: rb_Contacts
		// Table: rb_Contacts_st
		// Table: rb_ContentManager
		// Table: rb_Countries
		// Table: rb_Cultures
		// Table: rb_Discussion
		// Table: rb_Documents
		// Table: rb_Documents_st
		// Table: rb_EnhancedHtml
		// Table: rb_EnhancedHtml_st
		// Table: rb_EnhancedLinks
		// Table: rb_EnhancedLinks_st
		// Table: rb_Events
		// Table: rb_Events_st
		// Table: rb_FAQs
		// Table: rb_GeneralModuleDefinitions
		// Table: rb_HtmlText
		// Table: rb_HtmlText_st
		// Table: rb_Links
		// Table: rb_Links_st
		// Table: rb_Localize
		// Table: rb_Milestones
		// Table: rb_Milestones_st
		// Table: rb_ModuleDefinitions
		// Table: rb_Modules
		// Table: rb_ModuleSettings
		// Table: rb_Monitoring
		// Table: rb_Pictures
		// Table: rb_Pictures_st
		// Table: rb_Portals
		// Table: rb_PortalSettings
		// Table: rb_Roles
		// Table: rb_SolutionModuleDefinitions
		// Table: rb_Solutions
		// Table: rb_States
		// Table: rb_SurveyAnswers
		// Table: rb_SurveyOptions
		// Table: rb_SurveyQuestions
		// Table: rb_Surveys
		// Table: rb_Tabs
		// Table: rb_TabSettings
		// Table: rb_Tasks
		// Table: rb_UserDefinedData
		// Table: rb_UserDefinedFields
		// Table: rb_UserDefinedRows
		// Table: rb_UserDesktop
		// Table: rb_UserRoles
		// Table: rb_Users
		// Table: rb_Versions
		#endregion

		#region ManualCode
/***PRESERVE_BEGIN MANUAL_CODE***//***PRESERVE_END MANUAL_CODE***/
		#endregion
	}

}
#endregion


