namespace Rainbow.Configuration
{

	/// <summary>
	/// SettingItemGroups, used to sort and group site and module
	/// settings in SettingsTable.
	/// </summary>
	public enum SettingItemGroup : int
	{
		NONE					= 0,

		THEME_LAYOUT_SETTINGS	= 1000,

		SECURITY_USER_SETTINGS	= 2000,
	
		CULTURE_SETTINGS		= 3000,
		
		BUTTON_DISPLAY_SETTINGS	= 6000,
		
		MODULE_SPECIAL_SETTINGS	= 7000,
		
		META_SETTINGS			= 8000,
		
		MISC_SETTINGS			= 9000,
		
		NAVIGATION_SETTINGS		= 10000,
		
		CUSTOM_USER_SETTINGS	= 15000,
		
		/// <summary>Module Data Filter (aka. MDF).</summary>
		MDF_SETTINGS			= 20000
	}
}
