using System.Collections;
using Rainbow.Configuration;

namespace Rainbow.UI
{
	/// <summary>
	/// PropertyPage_custom inherits from Rainbow.UI.PropertyPage <br/>
	/// Used for properties pages to display custom properties of a module<br/>
	/// Can be inherited
	/// </summary>
	public class PropertyPageCustom : PropertyPage
	{
		private Hashtable customUserSettings;
		/// <summary>
		/// Stores current module settings
		/// </summary>
		public Hashtable CustomUserSettings
		{
			get
			{
				if(customUserSettings == null)
				{
					if (ModuleID > 0)
						// Get settings from the database
						customUserSettings = ModuleSettingsCustom.GetModuleUserSettings(ModuleID, int.Parse(PortalSettings.CurrentUser.Identity.ID), this);
					else
						// Or provides an empty hashtable
						customUserSettings = new Hashtable();
				}
				return customUserSettings;
			}
		}
	}
}