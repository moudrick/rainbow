using System;
using System.Collections;
using Rainbow.Framework;

namespace Rainbow.Framework.BusinessObjects
{
	/// <summary>
	/// ModuleSettings Class encapsulates the detailed settings 
	/// for a specific Module in the Portal.
	/// </summary>
	[History("gman3001", "2004/10/06", "Added GetModuleDesktopSrc method to return the source path of a Module specified by its ID")]
	[History("Jes1111", "2003/04/24", "Added Cacheable property")]
	public class RainbowModule
	{
		const string strAdmin = "Admin;";

        /// <summary>
		/// ModuleID
		/// </summary>
		public int ModuleID;
		/// <summary>
		/// ModuleDefID
		/// </summary>
		public int ModuleDefID;
		/// <summary>
		/// PageID
		/// </summary>
		public int PageID;
		/// <summary>
		/// CacheTime
		/// </summary>
		public int CacheTime;
		/// <summary>
		/// ModuleOrder
		/// </summary>
		public int ModuleOrder;
		/// <summary>
		/// PaneName
		/// </summary>
		public string PaneName;
		/// <summary>
		/// ModuleTitle
		/// </summary>
		public string ModuleTitle;
		/// <summary>
		/// AuthorizedEditRoles
		/// </summary>
		public string AuthorizedEditRoles;
		/// <summary>
		/// AuthorizedViewRoles
		/// </summary>
		public string AuthorizedViewRoles;
		/// <summary>
		/// AuthorizedAddRoles
		/// </summary>
		public string AuthorizedAddRoles;
		/// <summary>
		/// AuthorizedDeleteRoles
		/// </summary>
		public string AuthorizedDeleteRoles;
		/// <summary>
		/// AuthorizedPropertiesRoles
		/// </summary>
		public string AuthorizedPropertiesRoles;
		/// <summary>
		/// AuthorizedDeleteModuleRoles 
		/// </summary>
		public string AuthorizedDeleteModuleRoles; // Added by jviladiu@portalServices.net 19/8/2004
		/// <summary>
		/// AuthorizedMoveModuleRoles
		/// </summary>
		public string AuthorizedMoveModuleRoles; // Added by jviladiu@portalServices.net 19/8/2004
		/// <summary>
		/// AuthorizedPublishingRoles
		/// </summary>
		public string AuthorizedPublishingRoles; // Change by Geert.Audenaert@Syntegra.Com - Date: 6/2/2003
		/// <summary>
		/// AuthorizedApproveRoles
		/// </summary>
		public string AuthorizedApproveRoles; // Change by Geert.Audenaert@Syntegra.Com - Date: 27/2/2003
		/// <summary>
		/// WorkflowStatus
		/// </summary>
		public WorkflowState WorkflowStatus; // Change by Geert.Audenaert@Syntegra.Com - Date: 27/2/2003
		/// <summary>
		/// SupportWorkflow
		/// </summary>
		public bool SupportWorkflow; // Change by Geert.Audenaert@Syntegra.Com - Date: 6/2/2003
		/// <summary>
		/// ShowMobile
		/// </summary>
		public bool ShowMobile;
		/// <summary>
		/// ShowEveryWhere
		/// </summary>
		public bool ShowEveryWhere; // Change by john.mandia@whitelightsolutions.com - Date: 5/24/2003
		/// <summary>
		/// SupportCollapsable
		/// </summary>
		public bool SupportCollapsable; // Change by bja@reedtek.com - Date: 5/12/2003
		/// <summary>
		/// DesktopSrc
		/// </summary>
		public string DesktopSrc;
		/// <summary>
		/// MobileSrc
		/// </summary>
		public string MobileSrc;
		/// <summary>
		/// GuidID
		/// </summary>
		public Guid GuidID;
		/// <summary>
		/// Is Admin?
		/// </summary>
		public bool Admin;
		// Change 28/Feb/2003 - Jeremy Esland - Cache
		// used to store list of files for cache dependency -
		// optionally filled by module code
		// read by code in CachedPortalModuleControl
		/// <summary>
		/// String array of cache dependency files
		/// </summary>
		public ArrayList CacheDependency = new ArrayList();
		// Jes1111
		/// <summary>
		/// Is Cacheable?
		/// </summary>
		public bool Cacheable;

        /// <summary>
		/// ModuleSettings
		/// </summary>
		//TODO: [moudrick] make it internal
        public RainbowModule()
		{
			ModuleID = 0;
			PaneName = "no pane";
			ModuleTitle = string.Empty;
			AuthorizedEditRoles = strAdmin;
			AuthorizedViewRoles = "All Users;";
			AuthorizedAddRoles = strAdmin;
			AuthorizedDeleteRoles = strAdmin;
			AuthorizedPropertiesRoles = strAdmin;
			AuthorizedMoveModuleRoles = strAdmin;
			AuthorizedDeleteModuleRoles = strAdmin;
			CacheTime = 0;
			ModuleOrder = 0;
			ShowMobile = false;
			DesktopSrc = string.Empty;
			MobileSrc = string.Empty;
			SupportCollapsable = false;
			SupportWorkflow = false;
		}
	}
}
