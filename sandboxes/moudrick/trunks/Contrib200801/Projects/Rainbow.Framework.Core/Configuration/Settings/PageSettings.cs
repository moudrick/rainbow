using System.Collections;
using System.Globalization;
using System.IO;
using System.Web;
using Rainbow.Framework.Core.Configuration.Settings;
using Rainbow.Framework.Core.Configuration.Settings.Providers;
using Rainbow.Framework.Design;
using Rainbow.Framework;
using Rainbow.Framework.Settings.Cache;
using Rainbow.Framework.DataTypes;
using Path = Rainbow.Framework.Settings.Path;

namespace Rainbow.Framework.Site.Configuration
{
	/// <summary>
	/// PageSettings Class encapsulates the detailed settings 
	/// for a specific Page in the Portal
	/// </summary>
	public class PageSettings
	{
		/// <summary>
		///     
		/// </summary>
		public string AuthorizedRoles;

		/// <summary>
		///     
		/// </summary>
		public string MobilePageName;

		/// <summary>
		///     
		/// </summary>
		public ArrayList Modules = new ArrayList();

		/// <summary>
		///     
		/// </summary>
		public int ParentPageID;

		/// <summary>
		///     
		/// </summary>
		public bool ShowMobile;

		/// <summary>
		///     
		/// </summary>
		public int PageID;

		/// <summary>
		///     
		/// </summary>
		public string PageLayout;

		/// <summary>
		///     
		/// </summary>
		public int PageOrder;

		PortalSettings portalSettings;
		Hashtable customSettings;
		string tabName;

		// Jes1111
		//		public int			TemplateId;
		/// <remarks>
		/// thierry (tiptopweb)
		/// to have dropdown list for the themes and layout, we need the data path for the portal (for private theme and layout)
		/// we need the portalPath here for this use and it has to be set from the current portalSettings before getting the
		/// CustomSettings for a tab
		/// </remarks>
		string portalPath = null;

        /// <summary>
        /// Stores current portal settings
        /// </summary>
        /// <value>The portal settings.</value>
        PortalSettings PortalSettings
        {
            get
            {
                if (portalSettings == null)
                {
                    // Obtain PortalSettings from Current Context
                    if (HttpContext.Current != null)
                    {
                        portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                    }
                }
                return portalSettings;
            }
        }

        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hashtable of
        /// custom Page specific settings from the database. This method is
        /// used by Portals to access misc Page settings.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
		public Hashtable GetPageCustomSettings(int pageID)
		{
			Hashtable baseSettings;
            if (CurrentCache.Exists(Key.TabSettings(pageID)))
            {
                baseSettings = (Hashtable) CurrentCache.Get(Key.TabSettings(pageID));
            }
            else
            {
                baseSettings = GetPageBaseSettings();
                Hashtable settings = PageSettingsProvider.GetPageCustomSettings(pageID);

                // Thierry (Tiptopweb)
                // TODO : put back the cache in GetPageBaseSettings() and reset values not found in the database
                foreach (string key in baseSettings.Keys)
                {
                    if (settings[key] != null)
                    {
                        SettingItem s = ((SettingItem) baseSettings[key]);

                        if (settings[key].ToString().Length != 0)
                            s.Value = settings[key].ToString();
                    }
                    else //by Manu
                        // Thierry (Tiptopweb), see the comment in Hashtable GetPageBaseSettings()
                        // this is not resetting key not found in the database
                    {
                        //SettingItem s = ((SettingItem)_baseSettings[key]);
                        //s.Value = string.Empty; 3_aug_2004 Cory Isakson.  This line caused an error with booleans
                    }
                }
                CurrentCache.Insert(Key.TabSettings(pageID), baseSettings);
            }
            return baseSettings;
		}

	    /// <summary>
		/// Update Page Custom Settings
		/// </summary>
		/// <param name="pageID">The page ID.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public static void UpdatePageSettings(int pageID, string key, string value)
		{
			PageSettingsProvider.UpdatePageSettings(pageID, key, value);

	        //Invalidate cache
            if (CurrentCache.Exists(Key.TabSettings(pageID)))
            {
                CurrentCache.Remove(Key.TabSettings(pageID));
            }

	        // Clear url builder elements
			HttpUrlBuilder.Clear(pageID);
		}

	    /// <summary>
		/// Gets the image menu.
		/// </summary>
		/// <returns>A System.Collections.Hashtable value...</returns>
		private Hashtable GetImageMenu()
		{
			Hashtable imageMenuFiles;

			if (!CurrentCache.Exists(Key.ImageMenuList(PortalSettings.CurrentLayout)))
			{
				imageMenuFiles = new Hashtable();
				imageMenuFiles.Add("-Default-", string.Empty);
			    LayoutManager layoutManager = new LayoutManager(PortalPath);

				string menuDirectory = Path.WebPathCombine(layoutManager.PortalLayoutPath, PortalSettings.CurrentLayout);
				if (Directory.Exists(menuDirectory))
				{
					menuDirectory = Path.WebPathCombine(menuDirectory, "menuimages");
				}
				else
				{
					menuDirectory = Path.WebPathCombine(LayoutManager.Path, PortalSettings.CurrentLayout, "menuimages");
				}

				if (Directory.Exists(menuDirectory))
				{
					FileInfo[] menuImages = (new DirectoryInfo(menuDirectory)).GetFiles("*.gif");

					foreach (FileInfo fi in menuImages)
					{
						if (fi.Name != "spacer.gif" && fi.Name != "icon_arrow.gif")
							imageMenuFiles.Add(fi.Name, fi.Name);
					}
				}
				CurrentCache.Insert(Key.ImageMenuList(PortalSettings.CurrentLayout), imageMenuFiles, null);
			}
			else
			{
				imageMenuFiles = (Hashtable)CurrentCache.Get(Key.ImageMenuList(PortalSettings.CurrentLayout));
			}
			return imageMenuFiles;
		}

		/// <summary>
		/// Changed by Thierry@tiptopweb.com.au
		/// Page are different for custom page layout an theme, this cannot be static
		/// Added by john.mandia@whitelightsolutions.com
		/// Cache by Manu
		/// non static function, Thierry : this is necessary for page custom layout and themes
		/// </summary>
		/// <returns>A System.Collections.Hashtable value...</returns>
		private Hashtable GetPageBaseSettings()
		{
			//Define base settings
			Hashtable baseSettings = new Hashtable();
			int groupOrderBase;
			SettingItemGroup group;

			#region Navigation Settings

			// 2_aug_2004 Cory Isakson
			groupOrderBase = (int)SettingItemGroup.NAVIGATION_SETTINGS;
			group = SettingItemGroup.NAVIGATION_SETTINGS;

			SettingItem tabPlaceholder = new SettingItem(new BooleanDataType());
			tabPlaceholder.Group = group;
			tabPlaceholder.Order = groupOrderBase;
			tabPlaceholder.Value = "False";
			tabPlaceholder.EnglishName = "Act as a Placeholder?";
			tabPlaceholder.Description = "Allows this tab to act as a navigation placeholder only.";
			baseSettings.Add("TabPlaceholder", tabPlaceholder);

			SettingItem tabLink = new SettingItem(new StringDataType());
			tabLink.Group = group;
			tabLink.Value = string.Empty;
			tabLink.Order = groupOrderBase + 1;
			tabLink.EnglishName = "Static Link URL";
			tabLink.Description = "Allows this tab to act as a navigation link to any URL.";
			baseSettings.Add("TabLink", tabLink);

			SettingItem tabUrlKeyword = new SettingItem(new StringDataType());
			tabUrlKeyword.Group = group;
			tabUrlKeyword.Order = groupOrderBase + 2;
			tabUrlKeyword.EnglishName = "Url Keyword";
			tabUrlKeyword.Description = "Allows you to specify a keyword that would appear in your url.";
			baseSettings.Add("TabUrlKeyword", tabUrlKeyword);

			SettingItem urlPageName = new SettingItem(new StringDataType());
			urlPageName.Group = group;
			urlPageName.Order = groupOrderBase + 3;
			urlPageName.EnglishName = "Url Page Name";
			urlPageName.Description = "This setting allows you to specify a name for this tab that will show up in the url instead of default.aspx";
			baseSettings.Add("UrlPageName", urlPageName);

			#endregion

			#region Metadata Management

			//_groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
			group = SettingItemGroup.META_SETTINGS;
			SettingItem tabTitle = new SettingItem(new StringDataType());
			tabTitle.Group = group;
			tabTitle.EnglishName = "Tab / Page Title";
			tabTitle.Description = "Allows you to enter a title (Shows at the top of your browser) for this specific Tab / Page. Enter something here to override the default portal wide setting.";
			baseSettings.Add("TabTitle", tabTitle);

			SettingItem tabMetaKeyWords = new SettingItem(new StringDataType());
			tabMetaKeyWords.Group = group;
			tabMetaKeyWords.EnglishName = "Tab / Page Keywords";
			tabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what this Tab / Page is about.Enter something here to override the default portal wide setting.";
			baseSettings.Add("TabMetaKeyWords", tabMetaKeyWords);

			SettingItem tabMetaDescription = new SettingItem(new StringDataType());
			tabMetaDescription.Group = group;
			tabMetaDescription.EnglishName = "Tab / Page Description";
			tabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a description (Not too long though. 1 paragraph is enough) that describes this particular Tab / Page. Enter something here to override the default portal wide setting.";
			baseSettings.Add("TabMetaDescription", tabMetaDescription);

			SettingItem tabMetaEncoding = new SettingItem(new StringDataType());
			tabMetaEncoding.Group = group;
			tabMetaEncoding.EnglishName = "Tab / Page Encoding";
			tabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the content type for this particular Tab / Page. Enter something here to override the default portal wide setting.";
			baseSettings.Add("TabMetaEncoding", tabMetaEncoding);

			SettingItem tabMetaOther = new SettingItem(new StringDataType());
			tabMetaOther.Group = group;
			tabMetaOther.EnglishName = "Additional Meta Tag Entries";
			tabMetaOther.Description = "This setting allows you to enter new tags into this Tab / Page's HEAD Tag. Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabMetaOther", tabMetaOther);

			SettingItem TabKeyPhrase = new SettingItem(new StringDataType());
			TabKeyPhrase.Group = group;
			TabKeyPhrase.EnglishName = "Tab / Page Keyphrase";
			TabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a message/phrase for this particular Tab / Page This can be used for search engine optimisation. Enter something here to override the default portal wide setting.";
			baseSettings.Add("TabKeyPhrase", TabKeyPhrase);

			#endregion

			#region Layout and Theme

			// changed Thierry (Tiptopweb) : have a dropdown menu to select layout and themes
			groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;
			group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
			// get the list of available layouts
			// changed: Jes1111 - 2004-08-06
			ArrayList layoutsList = new ArrayList(new LayoutManager(PortalSettings.PortalPath).GetLayouts());   
			LayoutItem noCustomLayout = new LayoutItem();
			noCustomLayout.Name = string.Empty;
			layoutsList.Insert(0, noCustomLayout);
			// get the list of available themes
			// changed: Jes1111 - 2004-08-06
			ArrayList themesList = new ArrayList(new ThemeManager(PortalSettings.PortalPath).GetThemes());
			ThemeItem noCustomTheme = new ThemeItem();
			noCustomTheme.Name = string.Empty;
			themesList.Insert(0, noCustomTheme);
			// changed: Jes1111 - 2004-08-06
			SettingItem customLayout = new SettingItem(new CustomListDataType(layoutsList, "Name", "Name"));
			customLayout.Group = group;
			customLayout.Order = groupOrderBase + 11;
			customLayout.EnglishName = "Custom Layout";
			customLayout.Description = "Set a custom layout for this tab only";
			baseSettings.Add("CustomLayout", customLayout);
			//SettingItem CustomTheme = new SettingItem(new StringDataType());
			// changed: Jes1111 - 2004-08-06
			SettingItem customTheme = new SettingItem(new CustomListDataType(themesList, "Name", "Name"));
			customTheme.Group = group;
			customTheme.Order = groupOrderBase + 12;
			customTheme.EnglishName = "Custom Theme";
			customTheme.Description = "Set a custom theme for the modules in this tab only";
			baseSettings.Add("CustomTheme", customTheme);
			//SettingItem CustomThemeAlt = new SettingItem(new StringDataType());
			// changed: Jes1111 - 2004-08-06
			SettingItem customThemeAlt = new SettingItem(new CustomListDataType(themesList, "Name", "Name"));
			customThemeAlt.Group = group;
			customThemeAlt.Order = groupOrderBase + 13;
			customThemeAlt.EnglishName = "Custom Alt Theme";
			customThemeAlt.Description = "Set a custom alternate theme for the modules in this tab only";
			baseSettings.Add("CustomThemeAlt", customThemeAlt);

			SettingItem customMenuImage = new SettingItem(new CustomListDataType(GetImageMenu(), "Key", "Value"));
			customMenuImage.Group = group;
			customMenuImage.Order = groupOrderBase + 14;
			customMenuImage.EnglishName = "Custom Image Menu";
			customMenuImage.Description = "Set a custom menu image for this tab";
			baseSettings.Add("CustomMenuImage", customMenuImage);

			#endregion

			#region Language/Culture Management

			groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
			group = SettingItemGroup.CULTURE_SETTINGS;
            CultureInfo[] cultureList = Rainbow.Framework.Localization.LanguageSwitcher.GetLanguageList(true);
			//Localized tab title
			int counter = groupOrderBase + 11;

			foreach (CultureInfo c in cultureList)
			{
				//Ignore invariant
				if (c != CultureInfo.InvariantCulture && !baseSettings.ContainsKey(c.Name))
				{
					SettingItem localizedTabKeyPhrase = new SettingItem(new StringDataType());
					localizedTabKeyPhrase.Order = counter;
					localizedTabKeyPhrase.Group = group;
					localizedTabKeyPhrase.EnglishName = "Tab Key Phrase (" + c.Name + ")";
					localizedTabKeyPhrase.Description = "Key Phrase this Tab/Page for " + c.EnglishName + " culture.";
					baseSettings.Add("TabKeyPhrase_" + c.Name, localizedTabKeyPhrase);
					SettingItem LocalizedTitle = new SettingItem(new StringDataType());
					LocalizedTitle.Order = counter;
					LocalizedTitle.Group = group;
					LocalizedTitle.EnglishName = "Title (" + c.Name + ")";
					LocalizedTitle.Description = "Set title for " + c.EnglishName + " culture.";
					baseSettings.Add(c.Name, LocalizedTitle);
					counter++;
				}
			}

			#endregion

			return baseSettings;
		}

		/// <summary>
		/// Page Settings For Search Engines
		/// </summary>
		/// <value>The custom settings.</value>
		public Hashtable CustomSettings
		{
			get
			{
                if (customSettings == null)
                {
                    customSettings = GetPageCustomSettings(PageID);
                }
			    return customSettings;
			}
		}

		/// <summary>
		/// Gets or sets the portal path.
		/// </summary>
		/// <value>The portal path.</value>
		/// <remarks>
		/// </remarks>
		public string PortalPath
		{
			set
			{
				portalPath = value;

				if (!portalPath.EndsWith("/")) portalPath += "/";
			}
			get { return portalPath; }
		}

		/// <summary>
		/// Gets or sets the name of the page.
		/// </summary>
		/// <value>The name of the page.</value>
		/// <remarks>
		/// </remarks>
		public string PageName
		{
			get { return tabName; }
			set { tabName = value; }
		}
	}
}
