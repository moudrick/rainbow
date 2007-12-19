using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.UI.WebControls;

namespace Rainbow.DesktopModules.SimpleMenu
{
	/// <summary>
	/// SimpleMenuType
	/// </summary>
	public class SimpleMenuType :UserControl
	{
		/// <summary>
		/// 
		/// </summary>
		public SimpleMenuType()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		private Hashtable _settings;
		/// <summary>
		/// 
		/// </summary>
		public Hashtable ModuleSettings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value ;
			}
		}
		

		private PortalSettings _portalSettings;
		/// <summary>
		/// 
		/// </summary>
		public PortalSettings GlobalPortalSettings
		{
			get
			{
				return _portalSettings;
			}
			set
			{
				_portalSettings = value ;
			}
		}
		

		private BindOption menuBindOption = BindOption.BindOptionNone;
		/// <summary>
		/// 
		/// </summary>
		public BindOption MenuBindOption
		{
		get
		{
			if (ModuleSettings["sm_MenuBindingType"] != null )
			menuBindOption = (BindOption) int.Parse("0" +  ModuleSettings["sm_MenuBindingType"].ToString());
			return menuBindOption;
		}
}



		private RepeatDirection menuRepeatDirection;
		/// <summary>
		/// 
		/// </summary>
		public RepeatDirection MenuRepeatDirection
		{
			get
			{
				if (ModuleSettings["sm_Menu_RepeatDirection"] != null && ModuleSettings["sm_Menu_RepeatDirection"].ToString() == "0" ) 
					menuRepeatDirection = RepeatDirection.Horizontal; 
				else
					menuRepeatDirection = RepeatDirection.Vertical;

				return menuRepeatDirection;

			}
		}
	


		private int parentTabID = 0;
		/// <summary>
		/// 
		/// </summary>
		public int ParentPageID
		{
			get
			{
				if (ModuleSettings["sm_ParentPageID"] != null)
					parentTabID = int.Parse(ModuleSettings["sm_ParentPageID"].ToString());
			
				return parentTabID;

			}
		}

	}
}
