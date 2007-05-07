using System.Collections;
using System.Globalization;
using Esperantus.WebControls;
using Rainbow.Core;
using Rainbow.Data;
using Rainbow.Design;
using Rainbow.UI.DataTypes;

namespace Rainbow.BusinessRules
{
	/// <summary>
	/// Summary description for Page.
	/// </summary>
	public class Page : Core.Page
	{
		public Hashtable CustomSettings
		{
			get
			{
//				if (!CurrentCache.Exists(Key.TabSettings(this.Id)))
//				{
				customSettings = GetPageBaseSettings();
				
				#region Is this redundant?  The values don't goto anywhere...

				// Get Settings for this Tab from the database
				Hashtable _settings = Helper.Pages.Settings(this.Id);

				// Thierry (Tiptopweb)
				// TODO : put back the cache in GetTabBaseSettings() and reset values not found in the database
				// nix that...  just reset values n/found in db
				foreach (string key in customSettings.Keys)
				{
					if (_settings[key] != null)
					{
						Setting s = ((Setting) customSettings[key]);

						if (_settings[key].ToString().Length != 0)
							s.Value = _settings[key].ToString();
					}
					else
					{
						//by Manu
						// Thierry (Tiptopweb), see the comment in Hashtable GetTabBaseSettings()
						// this is not resetting key not found in the database
						Setting s = ((Setting) customSettings[key]);
						//s.Value = string.Empty; 3_aug_2004 Cory Isakson.  This line caused an error with booleans
					}
				}
				#endregion

//				}
//				else
//				{
//					customSettings = (Hashtable) CurrentCache.Get(Key.TabSettings(this.Id));
//				}
//				CurrentCache.Insert(Key.TabSettings(this.Id), customSettings);

				return customSettings;
			}
			set { customSettings = value; }
		}


		/// <summary>
		/// Changed by Thierry@tiptopweb.com.au
		/// tabSettings are different for custom page layout an theme, this cannot be static
		/// Added by john.mandia@whitelightsolutions.com
		/// Cache by Manu
		/// non static function, Thierry : this is necessary for page custom layout and themes
		/// </summary>
		/// 
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		private Hashtable GetPageBaseSettings()
		{
			//Define base settings
			Hashtable _baseSettings = new Hashtable();
			int _groupOrderBase;
			Setting.SettingGroupIds _Group;

			#region Navigation Settings

			// 2_aug_2004 Cory Isakson
			_groupOrderBase = (int) Setting.SettingGroupIds.NAVIGATION_SETTINGS;
			_Group = Setting.SettingGroupIds.NAVIGATION_SETTINGS;
			Setting TabPlaceholder = new Setting(typeof (bool));
			TabPlaceholder.Group = new Setting.SettingGroup(_Group);
			TabPlaceholder.Order = _groupOrderBase;
			TabPlaceholder.Value = "False";
			TabPlaceholder.EnglishName = "Act as a Placeholder?";
			TabPlaceholder.Description = "Allows this tab to act as a navigation placeholder only.";
			_baseSettings.Add("TabPlaceholder", TabPlaceholder);
			Setting TabLink = new Setting(typeof (string));
			TabLink.Group = new Setting.SettingGroup(_Group);
			TabLink.Value = string.Empty;
			TabLink.Order = _groupOrderBase + 1;
			TabLink.EnglishName = "Static Link URL";
			TabLink.Description = "Allows this tab to act as a navigation link to any URL.";
			_baseSettings.Add("TabLink", TabLink);

			#endregion

			#region Metadata Management

			_groupOrderBase = (int) Setting.SettingGroupIds.META_SETTINGS;
			_Group = Setting.SettingGroupIds.META_SETTINGS;
			Setting TabTitle = new Setting(typeof (string));
			TabTitle.Group = new Setting.SettingGroup(_Group);
			TabTitle.EnglishName = "Tab / Page Title";
			TabTitle.Description = "Allows you to enter a title (Shows at the top of your browser) for this specific Tab / Page. Enter something here to override the default portal wide setting.";
			_baseSettings.Add("TabTitle", TabTitle);
			Setting TabUrlKeyword = new Setting(typeof (string));
			TabUrlKeyword.Group = new Setting.SettingGroup(_Group);
			TabUrlKeyword.EnglishName = "A Keyword To Identify This Tab / Page";
			TabUrlKeyword.Description = "Allows you to specify a keyword that would appear in your url.";
			_baseSettings.Add("TabUrlKeyword", TabUrlKeyword);
			Setting UrlPageName = new Setting(typeof (string));
			UrlPageName.Group = new Setting.SettingGroup(_Group);
			UrlPageName.EnglishName = "Url Page Name";
			UrlPageName.Description = "This setting allows you to specify a name for this tab that will show up in the url instead of default.aspx";
			_baseSettings.Add("UrlPageName", UrlPageName);
			Setting TabMetaKeyWords = new Setting(typeof (string));
			TabMetaKeyWords.Group = new Setting.SettingGroup(_Group);
			TabMetaKeyWords.EnglishName = "Tab / Page Keywords";
			TabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what this Tab / Page is about.Enter something here to override the default portal wide setting.";
			_baseSettings.Add("TabMetaKeyWords", TabMetaKeyWords);
			Setting TabMetaDescription = new Setting(typeof (string));
			TabMetaDescription.Group = new Setting.SettingGroup(_Group);
			TabMetaDescription.EnglishName = "Tab / Page Description";
			TabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a description (Not too long though. 1 paragraph is enough) that describes this particular Tab / Page. Enter something here to override the default portal wide setting.";
			_baseSettings.Add("TabMetaDescription", TabMetaDescription);
			Setting TabMetaEncoding = new Setting(typeof (string));
			TabMetaEncoding.Group = new Setting.SettingGroup(_Group);
			TabMetaEncoding.EnglishName = "Tab / Page Encoding";
			TabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the content type for this particular Tab / Page. Enter something here to override the default portal wide setting.";
			_baseSettings.Add("TabMetaEncoding", TabMetaEncoding);
			Setting TabMetaOther = new Setting(typeof (string));
			TabMetaOther.Group = new Setting.SettingGroup(_Group);
			TabMetaOther.EnglishName = "Additional Meta Tag Entries";
			TabMetaOther.Description = "This setting allows you to enter new tags into this Tab / Page's HEAD Tag. Enter something here to override the default portal wide setting.";
			_baseSettings.Add("TabMetaOther", TabMetaOther);
			Setting TabKeyPhrase = new Setting(typeof (string));
			TabKeyPhrase.Group = new Setting.SettingGroup(_Group);
			TabKeyPhrase.EnglishName = "Tab / Page Keyphrase";
			TabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a message/phrase for this particular Tab / Page This can be used for search engine optimisation. Enter something here to override the default portal wide setting.";
			_baseSettings.Add("TabKeyPhrase", TabKeyPhrase);

			#endregion

			#region Layout and Theme

			// changed Thierry (Tiptopweb) : have a dropdown menu to select layout and themes
			_groupOrderBase = (int) Setting.SettingGroupIds.THEME_LAYOUT_SETTINGS;
			_Group = Setting.SettingGroupIds.THEME_LAYOUT_SETTINGS;
			// get the list of available layouts
			// changed: Jes1111 - 2004-08-06
			ArrayList layoutsList = new ArrayList(new LayoutManager(Active.Portal.Path).GetLayouts());
			LayoutItem _noCustomLayout = new LayoutItem();
			_noCustomLayout.Name = string.Empty;
			layoutsList.Insert(0, _noCustomLayout);
			// get the list of available themes
			// changed: Jes1111 - 2004-08-06
			ArrayList themesList = new ArrayList(new ThemeManager(Active.Portal.Path).GetThemes());
			ThemeItem _noCustomTheme = new ThemeItem();
			_noCustomTheme.Name = string.Empty;
			themesList.Insert(0, _noCustomTheme);
			// changed: Jes1111 - 2004-08-06
			Setting CustomLayout = new Setting(new CustomListDataType(layoutsList, "Name", "Name"));
			CustomLayout.Group = new Setting.SettingGroup(_Group);
			CustomLayout.Order = _groupOrderBase + 11;
			CustomLayout.EnglishName = "Custom Layout";
			CustomLayout.Description = "Set a custom layout for this tab only";
			_baseSettings.Add("CustomLayout", CustomLayout);
			//Setting CustomTheme = new Setting(new StringDataType());
			// changed: Jes1111 - 2004-08-06
			Setting CustomTheme = new Setting(new CustomListDataType(themesList, "Name", "Name"));
			CustomTheme.Group = new Setting.SettingGroup(_Group);
			CustomTheme.Order = _groupOrderBase + 12;
			CustomTheme.EnglishName = "Custom Theme";
			CustomTheme.Description = "Set a custom theme for the modules in this tab only";
			_baseSettings.Add("CustomTheme", CustomTheme);
			//Setting CustomThemeAlt = new Setting(new StringDataType());
			// changed: Jes1111 - 2004-08-06
			Setting CustomThemeAlt = new Setting(new CustomListDataType(themesList, "Name", "Name"));
			CustomThemeAlt.Group = new Setting.SettingGroup(_Group);
			CustomThemeAlt.Order = _groupOrderBase + 13;
			CustomThemeAlt.EnglishName = "Custom Alt Theme";
			CustomThemeAlt.Description = "Set a custom alternate theme for the modules in this tab only";
			_baseSettings.Add("CustomThemeAlt", CustomThemeAlt);
			Setting CustomMenuImage = new Setting(new CustomListDataType(GetImageMenu(), "Key", "Value"));
			CustomMenuImage.Group = new Setting.SettingGroup(_Group);
			CustomMenuImage.Order = _groupOrderBase + 14;
			CustomMenuImage.EnglishName = "Custom Image Menu";
			CustomMenuImage.Description = "Set a custom menu image for this tab";
			_baseSettings.Add("CustomMenuImage", CustomMenuImage);

			#endregion

			#region Language/Culture Management

			_groupOrderBase = (int) Setting.SettingGroupIds.CULTURE_SETTINGS;
			_Group = Setting.SettingGroupIds.CULTURE_SETTINGS;
			CultureInfo[] cultureList = LanguageSwitcher.GetLanguageList(true);
			//Localized tab title
			int counter = _groupOrderBase + 11;

			foreach (CultureInfo c in cultureList)
			{
				//Ignore invariant
				if (c != CultureInfo.InvariantCulture && !_baseSettings.ContainsKey(c.Name))
				{
					Setting LocalizedTabKeyPhrase = new Setting(typeof (string));
					LocalizedTabKeyPhrase.Order = counter;
					LocalizedTabKeyPhrase.Group = new Setting.SettingGroup(_Group);
					LocalizedTabKeyPhrase.EnglishName = "Tab Key Phrase (" + c.Name + ")";
					LocalizedTabKeyPhrase.Description = "Key Phrase this Tab/Page for " + c.EnglishName + " culture.";
					_baseSettings.Add("TabKeyPhrase_" + c.Name, LocalizedTabKeyPhrase);
					Setting LocalizedTitle = new Setting(typeof (string));
					LocalizedTitle.Order = counter;
					LocalizedTitle.Group = new Setting.SettingGroup(_Group);
					LocalizedTitle.EnglishName = "Title (" + c.Name + ")";
					LocalizedTitle.Description = "Set title for " + c.EnglishName + " culture.";
					_baseSettings.Add(c.Name, LocalizedTitle);
					counter++;
				}
			}

			#endregion

			return _baseSettings;
		}
	}
}