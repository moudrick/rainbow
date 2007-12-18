
		
//========================================================================
// This file was generated using the MyGeneration tool in combination
// with the Gentle.NET Business Entity template, $Rev: 44 $
//========================================================================
using System;
using System.Collections;
using Gentle.Framework;

namespace Rainbow.Data.GentleNET
{
	#region rb_UserRoles
	/// <summary>
	/// This object represents the properties and methods of a Employee.
	/// </summary>
	[Serializable]
	[TableName("rb_UserRoles")]
	public class rb_UserRoles : Persistent
	{
		#region Members
		private bool _changed = false;
		private static bool invalidatedListAll = true;
		private static ArrayList listAllCache = null;
		[TableColumn("UserID", NotNull=true), ForeignKey("rb_Users", "UserID")]
		protected int userID;
		[TableColumn("RoleID", NotNull=true), ForeignKey("rb_Roles", "RoleID")]
		protected int roleID;
		#endregion
			

		#region Constructors
	
			
		/// <summary> 
		/// Create an object from an existing row of data. This will be used by Gentle to 
		/// construct objects from retrieved rows. 
		/// </summary> 
		public rb_UserRoles( 
				int UserID, 
				int RoleID)
		{
			userID = UserID;
			roleID = RoleID;
		}

		#endregion

		#region Public Properties
		
		public bool Changed
		{ get { return _changed; } }
		
		public int UserID
		{
			get{ return userID; }
			set{ _changed |= userID != value; userID = value; invalidatedListAll =  _changed;}
		}
		
		public int RoleID
		{
			get{ return roleID; }
			set{ _changed |= roleID != value; roleID = value; invalidatedListAll =  _changed;}
		}
		
	
		// generate a static property to retrieve all instances of a class that are stored in the database
		static public IList ListAll
		{
			get 
			{ 
				if( listAllCache == null || invalidatedListAll )
				{
					listAllCache = Broker.RetrieveList( typeof(rb_UserRoles) ) as ArrayList;
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
		// Key: FK_rb_UserRoles_rb_Roles primary table: rb_Roles primary column: RoleID foreign column: RoleID foreign table: rb_UserRoles
		// Key: FK_rb_UserRoles_rb_Users primary table: rb_Users primary column: UserID foreign column: UserID foreign table: rb_UserRoles
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

		/// <summary>
		/// Return list of referenced objects from n:m relation with
		/// table "rb_Roles", using relation table "rb_UserRoles"
		/// </summary>
		public IList referencedrb_RolesUsingrb_UserRoles()
		{
			return new GentleList( typeof(rb_Roles), this, typeof(rb_UserRoles));
		}

		/// <summary>
		/// Return list of referenced objects from n:m relation with
		/// table "rb_Users", using relation table "rb_UserRoles"
		/// </summary>
		public IList referencedrb_UsersUsingrb_UserRoles()
		{
			return new GentleList( typeof(rb_Users), this, typeof(rb_UserRoles));
		}
		#endregion

		#region ManualCode
		/***PRESERVE_BEGIN MANUAL_CODE***/
		public static rb_UserRoles Retrieve(int userId, int roleId)
		{
	   
			// Return null if id is smaller than seed and/or increment for autokey
			if(userId < 0 || roleId < 0) 
			{
				return null;
			}
		  
			Key key = new Key( typeof(rb_UserRoles), true, "UserID", userId );
			foreach (rb_UserRoles ur in Broker.RetrieveList(typeof(rb_UserRoles),key))
			{
				if(ur.RoleID==roleId)
					return ur;
			}
			return null;
		}
		/***PRESERVE_END MANUAL_CODE***/
		#endregion
	}

}
#endregion


