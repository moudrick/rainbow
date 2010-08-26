using Rainbow.Framework.Configuration;
using System.Globalization;
namespace Rainbow.Framework.Configuration.Cache
{
	/// <summary>
	/// This class return the cache keys used in Rainbow.
	/// </summary>
	public sealed class Key
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
		private Key() {}

        /// <summary>
        /// This method allows you to create a custom cache key for a modules' specific settings
        /// </summary>
        /// <param name="moduleId">The Id of the module you wish to get the cache key for</param>
        /// <returns>
        /// The generated cache key for the module you specified
        /// </returns>
		public static string ModuleSettings(int moduleId)
		{
			return string.Concat(Portal.UniqueId, "_ModuleSettings_", moduleId.ToString(CultureInfo.InvariantCulture));
		}

        /// <summary>
        /// This method allows you to retrieve the cache key used to store this portal's current settings in portal settings table
        /// </summary>
        /// <returns>Cache Key For Portal Settings</returns>
		public static string PortalSettings()
		{
			return string.Concat(Portal.UniqueId, "_PortalSettings");
		}

        /// <summary>
        /// This method allows you to retrieve the cache key used to store this portal's base settings i.e. Layouts Available, Themes Available etc
        /// </summary>
        /// <returns>Cache Key For Portal Base Settings</returns>
		public static string PortalBaseSettings()
		{
			return string.Concat(Portal.UniqueId, "_PortalBaseSettings");
		}

        /// <summary>
        /// This method allows you to create a custom cache key for a Tab's specific settings
        /// </summary>
        /// <param name="tabId">The Id of the tab you wish to get the cache key for</param>
        /// <returns>
        /// The generated cache key for the tab you specified
        /// </returns>
		public static string TabSettings(int tabId)
		{
			return string.Concat(Portal.UniqueId, "_TabSettings_", tabId.ToString(CultureInfo.InvariantCulture));
		}

        /// <summary>
        /// This method allows you to create a custom cache key for Language Specific Tab Navigation
        /// </summary>
        /// <param name="tabID">The ID of the tab you wish to get the cache key for</param>
        /// <param name="language">The language you need it in</param>
        /// <returns>
        /// The generated cache key for the tab navigation you specified
        /// </returns>
		public static string TabNavigationSettings(int tabId, string language)
		{
			return string.Concat(Portal.UniqueId, "_TabNavigationSettings_", tabId.ToString(CultureInfo.InvariantCulture), language);
		}

        /// <summary>
        /// This method allows you to create a custom cache key for a list of languages used in portal
        /// </summary>
        /// <returns>The generated cache key</returns>
		public static string LanguageList()
		{
			return string.Concat(Portal.UniqueId, "_LanguageList");
		}

        /// <summary>
        /// This method allows you to create a custom cache key for a list of availables image menus in current layout
        /// </summary>
        /// <param name="currentLayout">The name of current layout</param>
        /// <returns>The generated cache key</returns>
		public static string ImageMenuList(string currentLayout)
		{
			return string.Concat(Portal.UniqueId, "_ImageMenuList_", currentLayout);
		}

        /// <summary>
        /// This method allows you to create a custom cache key for a list of availables layouts in pathname
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        /// <returns>The generated cache key</returns>
		public static string LayoutList(string pathName)
		{
			return string.Concat(Portal.UniqueId, "_LayoutList_", pathName);
		}

        /// <summary>
        /// This method allows you to create a custom cache key for a list of availables themes in pathname
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        /// <returns>The generated cache key</returns>
		public static string ThemeList(string pathName)
		{
			return string.Concat(Portal.UniqueId, "_ThemeList_", pathName);
		}

        /// <summary>
        /// This method allows you to create a custom cache key for current theme
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        /// <returns>The generated cache key</returns>
		public static string CurrentTheme(string pathName)
		{
			return string.Concat(Portal.UniqueId, "_CurrentTheme_", pathName);
		}
	}
}