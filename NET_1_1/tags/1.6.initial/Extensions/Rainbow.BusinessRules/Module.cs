using System;
using System.ComponentModel;

namespace Rainbow.BusinessRules
{
	/// <summary>
	/// This class encapsulates the basic attributes of a Module, and is used
	/// by the administration pages when manipulating modules.<br/>
	/// Module implements the IComparable interface so that an ArrayList
	/// of Modules may be sorted by ModuleOrder, using the 
	/// ArrayList's Sort() method.
	/// </summary>
	[Serializable]
	[Category("BusinessRules"),
	Description("Module Business Rules Object"),
	DefaultProperty("Id"),
		DefaultValue(0)]
	public class Module : Core.Module
	{
		#region Constructor

		/// <summary>
		/// Dafault contructor, initializes default settings
		/// </summary>
		public Module()
		{
			int _groupOrderBase;
			SettingItemGroup _Group;

			// THEME MANAGEMENT
			_Group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
			_groupOrderBase = (int) SettingItemGroup.THEME_LAYOUT_SETTINGS;

			SettingItem ApplyTheme = new SettingItem(new BooleanDataType());
			ApplyTheme.Order = _groupOrderBase + 10;
			ApplyTheme.Group = _Group;
			ApplyTheme.Value = "True";
			ApplyTheme.EnglishName = "Apply Theme";
			ApplyTheme.Description = "Check this box to apply theme to this module";
			this._baseSettings.Add("MODULESETTINGS_APPLY_THEME", ApplyTheme);

			ArrayList themeOptions = new ArrayList();
			themeOptions.Add(new SettingOption((int) ThemeList.Default, Localize.GetString("MODULESETTINGS_THEME_DEFAULT", "Default", null)));
			themeOptions.Add(new SettingOption((int) ThemeList.Alt, Localize.GetString("MODULESETTINGS_THEME_ALT", "Alt", null)));
			SettingItem Theme = new SettingItem(new CustomListDataType(themeOptions, "Name", "Val"));
			Theme.Order = _groupOrderBase + 20;
			Theme.Group = _Group;
			Theme.Value = ((int) ThemeList.Default).ToString();
			Theme.EnglishName = "Theme";
			Theme.Description = "Choose theme for this module";
			this._baseSettings.Add("MODULESETTINGS_THEME", Theme);

			if (HttpContext.Current != null) // null in DesignMode
			{
				// Added: Jes1111 - 2004-08-03
				PortalSettings _portalSettings;
				_portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				// end addition: Jes1111

				if (_portalSettings != null)
				{
					//fix by The Bitland Prince
					_portalID = _portalSettings.PortalID;

					// added: Jes1111 2004-08-02 - custom module theme
					if (_portalSettings.CustomSettings.ContainsKey("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES") && _portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString() != string.Empty && bool.Parse(_portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString()))
					{
						ArrayList _tempList = new ArrayList(new ThemeManager(_portalSettings.PortalPath).GetThemes());
						ArrayList _themeList = new ArrayList();
						foreach (ThemeItem _item in _tempList)
						{
							if (_item.Name.ToLower().StartsWith("module"))
								_themeList.Add(_item);
						}
						ThemeItem _noCustomTheme = new ThemeItem();
						_noCustomTheme.Name = string.Empty;
						_themeList.Insert(0, _noCustomTheme);
						SettingItem ModuleTheme = new SettingItem(new CustomListDataType(_themeList, "Name", "Name"));
						ModuleTheme.Order = _groupOrderBase + 25;
						ModuleTheme.Group = _Group;
						ModuleTheme.EnglishName = "Custom Theme";
						ModuleTheme.Description = "Set a custom theme for this module only";
						_baseSettings.Add("MODULESETTINGS_MODULE_THEME", ModuleTheme);
					}
				}
			}

			// switches title display on/off
			SettingItem ShowTitle = new SettingItem(new BooleanDataType());
			ShowTitle.Order = _groupOrderBase + 30;
			ShowTitle.Group = _Group;
			ShowTitle.Value = "True";
			ShowTitle.EnglishName = "Show Title";
			ShowTitle.Description = "Switches title display on/off";
			this._baseSettings.Add("MODULESETTINGS_SHOW_TITLE", ShowTitle);

			// switches last modified summary on/off
			SettingItem ShowModifiedBy = new SettingItem(new BooleanDataType());
			ShowModifiedBy.Order = _groupOrderBase + 40;
			ShowModifiedBy.Group = _Group;
			ShowModifiedBy.Value = "False";
			ShowModifiedBy.EnglishName = "Show Modified by";
			ShowModifiedBy.Description = "Switches 'Show Modified by' display on/off";
			this._baseSettings.Add("MODULESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy);

			// gman3001: added 10/26/2004
			//  - implement width, height, and content scrolling options for all modules 
			//  - implement auto-stretch option
			//Windows height
			SettingItem ControlHeight = new SettingItem(new IntegerDataType());
			ControlHeight.Value = "0";
			ControlHeight.MinValue = 0;
			ControlHeight.MaxValue = 3000;
			ControlHeight.Required = true;
			ControlHeight.Order = _groupOrderBase + 50;
			ControlHeight.Group = _Group;
			ControlHeight.EnglishName = "Content Height";
			ControlHeight.Description = "Minimum height(in pixels) of the content area of this module. (0 for none)";
			this._baseSettings.Add("MODULESETTINGS_CONTENT_HEIGHT", ControlHeight);

			//Windows width
			SettingItem ControlWidth = new SettingItem(new IntegerDataType());
			ControlWidth.Value = "0";
			ControlWidth.MinValue = 0;
			ControlWidth.MaxValue = 3000;
			ControlWidth.Required = true;
			ControlWidth.Order = _groupOrderBase + 60;
			ControlWidth.Group = _Group;
			ControlWidth.EnglishName = "Content Width";
			ControlWidth.Description = "Minimum width(in pixels) of the content area of this module. (0 for none)";
			this._baseSettings.Add("MODULESETTINGS_CONTENT_WIDTH", ControlWidth);

			//Content scrolling option
			SettingItem ScrollingSetting = new SettingItem(new BooleanDataType());
			ScrollingSetting.Value = "false";
			ScrollingSetting.Order = _groupOrderBase + 70;
			ScrollingSetting.Group = _Group;
			ScrollingSetting.EnglishName = "Content Scrolling";
			ScrollingSetting.Description = "Set to enable/disable scrolling of Content based on height and width settings.";
			this._baseSettings.Add("MODULESETTINGS_CONTENT_SCROLLING", ScrollingSetting);

			//Module Stretching option
			SettingItem StretchSetting = new SettingItem(new BooleanDataType());
			StretchSetting.Value = "true";
			StretchSetting.Order = _groupOrderBase + 80;
			StretchSetting.Group = _Group;
			StretchSetting.EnglishName = "Module Auto Stretch";
			StretchSetting.Description = "Set to enable/disable automatic stretching of the module's width to fill the empty area to the right of the module.";
			this._baseSettings.Add("MODULESETTINGS_WIDTH_STRETCHING", StretchSetting);
			// gman3001: END

			// BUTTONS
			_Group = SettingItemGroup.BUTTON_DISPLAY_SETTINGS;
			_groupOrderBase = (int) SettingItemGroup.BUTTON_DISPLAY_SETTINGS;

			// Show print button in view mode?
			SettingItem PrintButton = new SettingItem(new BooleanDataType());
			PrintButton.Value = "False";
			PrintButton.Order = _groupOrderBase + 20;
			PrintButton.Group = _Group;
			PrintButton.EnglishName = "Show Print Button";
			PrintButton.Description = "Show print button in view mode?";
			this._baseSettings.Add("MODULESETTINGS_SHOW_PRINT_BUTTION", PrintButton);

			// added: Jes1111 2004-08-29 - choice! Default is 'true' for backward compatibility
			// Show Title for print?
			SettingItem ShowTitlePrint = new SettingItem(new BooleanDataType());
			ShowTitlePrint.Value = "True";
			ShowTitlePrint.Order = _groupOrderBase + 25;
			ShowTitlePrint.Group = _Group;
			ShowTitlePrint.EnglishName = "Show Title for Print";
			ShowTitlePrint.Description = "Show Title for this module in print popup?";
			this._baseSettings.Add("MODULESETTINGS_SHOW_TITLE_PRINT", ShowTitlePrint);

			// added: Jes1111 2004-08-02 - choices for Button display on module
			ArrayList buttonDisplayOptions = new ArrayList();
			buttonDisplayOptions.Add(new SettingOption((int) ModuleButton.RenderOptions.ImageOnly, Localize.GetString("MODULESETTINGS_BUTTON_DISPLAY_IMAGE", "Image only", null)));
			buttonDisplayOptions.Add(new SettingOption((int) ModuleButton.RenderOptions.TextOnly, Localize.GetString("MODULESETTINGS_BUTTON_DISPLAY_TEXT", "Text only", null)));
			buttonDisplayOptions.Add(new SettingOption((int) ModuleButton.RenderOptions.ImageAndTextCSS, Localize.GetString("MODULESETTINGS_BUTTON_DISPLAY_BOTH", "Image and Text (CSS)", null)));
			buttonDisplayOptions.Add(new SettingOption((int) ModuleButton.RenderOptions.ImageOnlyCSS, Localize.GetString("MODULESETTINGS_BUTTON_DISPLAY_IMAGECSS", "Image only (CSS)", null)));
			SettingItem ButtonDisplay = new SettingItem(new CustomListDataType(buttonDisplayOptions, "Name", "Val"));
			ButtonDisplay.Order = _groupOrderBase + 30;
			ButtonDisplay.Group = _Group;
			ButtonDisplay.Value = ((int) ModuleButton.RenderOptions.ImageOnly).ToString();
			ButtonDisplay.EnglishName = "Display Buttons as:";
			ButtonDisplay.Description = "Choose how you want module buttons to be displayed. Note that settings other than 'Image only' may require Zen or special treatment in the Theme.";
			this._baseSettings.Add("MODULESETTINGS_BUTTON_DISPLAY", ButtonDisplay);

			// Jes1111 - not implemented yet			
			//			// Show email button in view mode?
			//			SettingItem EmailButton = new SettingItem(new BooleanDataType());
			//			EmailButton.Value = "False";
			//			EmailButton.Order = _groupOrderBase + 30;
			//			EmailButton.Group = _Group;
			//			this._baseSettings.Add("ShowEmailButton",EmailButton);

			// Show arrows buttons to move modules (admin only, property authorise)
			SettingItem ArrowButtons = new SettingItem(new BooleanDataType());
			ArrowButtons.Value = "True";
			ArrowButtons.Order = _groupOrderBase + 40;
			ArrowButtons.Group = _Group;
			ArrowButtons.EnglishName = "Show Arrow Admin Buttons";
			ArrowButtons.Description = "Show Arrow Admin buttons?";
			this._baseSettings.Add("MODULESETTINGS_SHOW_ARROW_BUTTONS", ArrowButtons);

			// Show help button if exists
			SettingItem HelpButton = new SettingItem(new BooleanDataType());
			HelpButton.Value = "True";
			HelpButton.Order = _groupOrderBase + 50;
			HelpButton.Group = _Group;
			HelpButton.EnglishName = "Show Help Button";
			HelpButton.Description = "Show help button in title if exists documentation for this module";
			this._baseSettings.Add("MODULESETTINGS_SHOW_HELP_BUTTON", HelpButton);

			// LANGUAGE/CULTURE MANAGEMENT
			_groupOrderBase = (int) SettingItemGroup.CULTURE_SETTINGS;
			_Group = SettingItemGroup.CULTURE_SETTINGS;

			CultureInfo[] cultureList = LanguageSwitcher.GetLanguageList(true);

			SettingItem Culture = new SettingItem(new MultiSelectListDataType(cultureList, "DisplayName", "Name"));
			Culture.Value = string.Empty;
			Culture.Order = _groupOrderBase + 10;
			Culture.Group = _Group;
			Culture.EnglishName = "Culture";
			Culture.Description = "Please choose the culture. Invariant cultures shows always the module, if you choose one or more cultures only when culture is selected this module will shown.";
			this._baseSettings.Add("MODULESETTINGS_CULTURE", Culture);

			//Localized module title
			int counter = _groupOrderBase + 11;
			foreach (CultureInfo c in cultureList)
			{
				//Ignore invariant
				if (c != CultureInfo.InvariantCulture && !this._baseSettings.ContainsKey(c.Name))
				{
					SettingItem LocalizedTitle = new SettingItem(new StringDataType());
					LocalizedTitle.Order = counter;
					LocalizedTitle.Group = _Group;
					LocalizedTitle.EnglishName = "Title (" + c.Name + ")";
					LocalizedTitle.Description = "Set title for " + c.EnglishName + " culture.";
					this._baseSettings.Add("MODULESETTINGS_TITLE_" + c.Name, LocalizedTitle);
					counter++;
				}
			}

			// SEARCH
			if (this.Searchable)
			{
				SettingItem topicName = new SettingItem(new StringDataType());
				topicName.Required = false;
				topicName.Value = string.Empty;
				topicName.EnglishName = "Topic";
				topicName.Description = "Select a topic for this module. You may filter itmes by topic in Portal Search.";
				this._baseSettings.Add("TopicName", topicName);
			}

			//Default configuration
			_tabID = 0;

			_moduleConfiguration = new ModuleSettings();

		}

		#endregion

		#region Module Configuration

		/// <summary>
		/// Module custom settings
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Hashtable Settings
		{
			get
			{
				if (_settings == null)
					_settings = ModuleSettings.MergeModuleSettings(ModuleID, _baseSettings);
				return _settings;
			}
		}

		/// <summary>
		/// Module base settings defined by control creator
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Hashtable BaseSettings
		{
			get { return _baseSettings; }
		}

		/// <summary>
		/// Override on derivates classes.
		/// Return the path of the add control if available.
		/// </summary>
		public virtual string AddModuleControl
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// Override on derivates classes.
		/// Return the path of the edit control if available.
		/// </summary>
		public virtual string EditModuleControl
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// unique key for module caching
		/// </summary>
		public string ModuleCacheKey
		{
			get
			{
				if (HttpContext.Current != null)
				{ // Change 8/April/2003 Jes1111
					// changes to Language behaviour require addition of culture names to cache key
					// Jes1111 2003/04/24 - Added PortalAlias to cachekey
					PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
					StringBuilder sb = new StringBuilder();
					sb.Append("rb_");
					sb.Append(portalSettings.PortalAlias);
					sb.Append("_mid");
					sb.Append(ModuleID.ToString());
					sb.Append("[");
					sb.Append(portalSettings.PortalContentLanguage);
					sb.Append("+");
					sb.Append(portalSettings.PortalUILanguage);
					sb.Append("+");
					sb.Append(portalSettings.PortalDataFormattingCulture);
					sb.Append("]");

					return sb.ToString();
				}
				else
					return null;
			}
		}

		/// <summary>
		/// The current ID of the module. Is unique for all portals.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ModuleID
		{
			get
			{
				try
				{
					return _moduleConfiguration.ModuleID;
				}
				catch
				{
					return -1;
				}
			}
			set //made changeable by Manu, please be careful on changing it
			{
				_moduleConfiguration.ModuleID = value;
				_settings = null; //force cached settings to be reloaded
			}
		}

		// Jes1111
		private int _originalModuleID = -1;

		/// <summary>
		/// The ID of the orginal module (will be different to ModuleID when using shortcut module)
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int OriginalModuleID
		{
			get
			{
				try
				{
					if (_originalModuleID == -1)
						return ModuleID;
					else
						return _originalModuleID;
				}
				catch
				{
					return -1;
				}
			}
			set { _originalModuleID = value; }
		}

		/// <summary>
		/// Configuration
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ModuleSettings ModuleConfiguration
		{
			get
			{
				if (HttpContext.Current != null && _moduleConfiguration != null)
					return _moduleConfiguration;
				else
					return null;
			}
			set { _moduleConfiguration = value; }
		}

		/// <summary>
		/// GUID of module (mandatory)
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Guid GuidID
		{
			get
			{
				//1.1.8.1324 - 24/01/2003
				throw new NotImplementedException("You must implement a unique GUID for your module");
			}
		}

		/// <summary>
		/// ClassName (Used for Get/Save: not implemented)
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual String ClassName
		{
			get { return string.Empty; }
		}

		#endregion

		#region Module Supports...

		/// <summary>
		/// Override on derivates class.
		/// Return true if the module is an Admin Module.
		/// </summary>
		public virtual bool AdminModule
		{
			get { return false; }
		}

		/// <summary>
		/// Override on derivates classes.
		/// Return true if the module is Searchable.
		/// </summary>
		public virtual bool Searchable
		{
			get { return false; }
		}

		// Jes1111
		/// <summary>
		/// Override on derived class.
		/// Return true if the module is Cacheable.
		/// </summary>
		public virtual bool Cacheable
		{
			get { return _cacheable; }
			set { _cacheable = value; }
		}


		/// <summary>
		/// Override on derived class.
		/// Return true if the module supports print in pop-up window.
		/// </summary>
		public bool SupportsPrint
		{
			get { return _supportsPrint; }
			set { _supportsPrint = value; }
		}

		/// <summary>
		/// This property indicates if the specified module supports can be
		/// collpasable (minimized/maximized/closed)
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Boolean SupportCollapsable
		{
			get
			{
				if (_moduleConfiguration == null)
					return this._supportsCollapseable;
				else
					return GlobalResources.SupportWindowMgmt && _moduleConfiguration.SupportCollapsable;
			}
			set { this._supportsCollapseable = value; }
		} // end of SupportCollapsable

		/// <summary>
		/// This property indicates whether the module supports a Back button
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsBack
		{
			get { return _supportsBack; }
			set { _supportsBack = value; }
		}

		/// <summary>
		/// This property indicates if the module supports email
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsEmail
		{
			get { return _supportsEmail; }
			set { _supportsEmail = value; }
		}

		/// <summary>
		/// This property indicates if the specified module supports arrows to move modules
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsArrows
		{
			get
			{
				bool returnValue = _supportsArrows;

				if (portalSettings.CustomSettings["SITESETTINGS_SHOW_MODULE_ARROWS"] != null)
					returnValue = returnValue && bool.Parse(portalSettings.CustomSettings["SITESETTINGS_SHOW_MODULE_ARROWS"].ToString());

				if (Settings["MODULESETTINGS_SHOW_ARROW_BUTTONS"] != null)
					returnValue = returnValue && bool.Parse(Settings["MODULESETTINGS_SHOW_ARROW_BUTTONS"].ToString());

				return returnValue;
			}
			set { _supportsArrows = value; }
		}

		/// <summary>
		/// This property indicates if the specified module workflow is enabled.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Boolean SupportsWorkflow
		{
			get
			{
				if (_moduleConfiguration == null)
					return _supportsWorkflow;
				else
					return _supportsWorkflow && _moduleConfiguration.SupportWorkflow;
			}
			set { _supportsWorkflow = value; }
		}

		/// <summary>
		/// This property indicates if the specified module supports workflow.
		/// It returns the module property regardless of current module setting.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Boolean InnerSupportsWorkflow // changed Jes1111 (from 'internal')
		{
			get { return _supportsWorkflow; }
			set { _supportsWorkflow = value; }
		}

		/// <summary>
		/// This property indicates if the specified module supports help
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsHelp
		{
			get
			{
				if ((Settings["MODULESETTINGS_SHOW_HELP_BUTTON"] == null || bool.Parse(Settings["MODULESETTINGS_SHOW_HELP_BUTTON"].ToString())) && (ModuleConfiguration.DesktopSrc != string.Empty))
				{
					string aux = Path.ApplicationRoot + "/rb_documentation/Rainbow/" + ModuleConfiguration.DesktopSrc.Replace(".", "_").ToString();
					return Directory.Exists(HttpContext.Current.Server.MapPath(aux));
				}
				else
					return false;
			}
		}

		#endregion

		#region Title

		/// <summary>
		/// Return true if module has inner control of type title
		/// </summary>
		/// <remarks>Left here for backward compatibility until it proves redundant</remarks>
		protected bool HasTitle
		{
			get { return true; }
		}

		private DesktopModuleTitle _ModuleTitle;

		/// <summary>
		/// Inner Title control. Now only used for backward compatibility 
		/// </summary>
		public virtual DesktopModuleTitle ModuleTitle
		{
			get { return _ModuleTitle; }
			set { _ModuleTitle = value; }
		}

		/// <summary>
		/// Switch to turn on/off the display of Title text.
		/// </summary>
		/// <remarks>Note: won't turn off the display of Buttons like it used to! You can now have buttons displayed with no title text showing</remarks>
		public virtual bool ShowTitle
		{
			get
			{
				if (HttpContext.Current != null) // if it is not design time
					return (bool.Parse(this.Settings["MODULESETTINGS_SHOW_TITLE"].ToString()));
				return false;
			}
			set
			{
				if (HttpContext.Current != null) // if it is not design time
					this.Settings["MODULESETTINGS_SHOW_TITLE"] = value.ToString();
			}
		}

		/// <summary>
		/// Switch to turn on/off the display of the module title text (not the buttons) in the print pop-up.
		/// </summary>
		public virtual bool ShowTitlePrint
		{
			get
			{
				if (HttpContext.Current != null) // if it is not design time
					return (bool.Parse(this.Settings["MODULESETTINGS_SHOW_TITLE_PRINT"].ToString()));
				return false;
			}
			set
			{
				if (HttpContext.Current != null) // if it is not design time
					this.Settings["MODULESETTINGS_SHOW_TITLE_PRINT"] = value.ToString();
			}
		}

		private string titleText = string.Empty;

		/// <summary>
		/// The module title as it will be displayed on the page. Handles cultures automatically.
		/// </summary>
		public virtual string TitleText
		{
			get
			{
				if (HttpContext.Current != null && this.titleText == string.Empty) // if it is not design time (and not overriden - Jes1111)
				{
					if (portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture && Settings["MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name] != null && Settings["MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name].ToString().Length > 0)
						titleText = Settings["MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name].ToString();
					else
					{
						if (this.ModuleConfiguration != null)
							titleText = this.ModuleConfiguration.ModuleTitle;
						else
							titleText = "TitleText Placeholder";
					}
				}
				return titleText;
			}
			set { titleText = value; }
		}

		private string editText = "EDIT";
		private string editUrl;
		private string editTarget;

		/// <summary>
		/// Text for Edit Link
		/// </summary>
		public String EditText
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.EditText != string.Empty)
					editText = this.ModuleTitle.EditText;
				return editText;
			}
			set { editText = value; }
		}

		/// <summary>
		/// Url for Edit Link
		/// </summary>
		public String EditUrl
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.EditUrl != string.Empty)
					editUrl = this.ModuleTitle.EditUrl;
				return editUrl;
			}
			set { editUrl = value; }
		}

		/// <summary>
		/// Target frame/page for Edit Link
		/// </summary>
		public String EditTarget
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.EditTarget != string.Empty)
					editUrl = this.ModuleTitle.EditTarget;
				return editTarget;
			}
			set { editTarget = value; }
		}

		private string addText = "ADD";
		private string addUrl;
		private string addTarget;

		/// <summary>
		/// Text for Add Link
		/// </summary>
		public String AddText
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.AddText != string.Empty)
					addText = this.ModuleTitle.AddText;
				return addText;
			}
			set { addText = value; }
		}

		/// <summary>
		/// Url for Add Link
		/// </summary>
		public String AddUrl
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.AddUrl != string.Empty)
					addUrl = this.ModuleTitle.AddUrl;
				return addUrl;
			}
			set { addUrl = value; }
		}

		/// <summary>
		/// Target frame/page for Add Link
		/// </summary>
		public String AddTarget
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.AddTarget != string.Empty)
					addTarget = this.ModuleTitle.AddTarget;
				return addTarget;

			}
			set { addTarget = value; }
		}

		private string propertiesText = "PROPERTIES";
		private string propertiesUrl = "~/DesktopModules/Admin/PropertyPage.aspx";
		private string propertiesTarget;

		/// <summary>
		/// Text for Properties Link
		/// </summary>
		public String PropertiesText
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.PropertiesText != string.Empty)
					propertiesText = this.ModuleTitle.PropertiesText;
				return propertiesText;
			}
			set { propertiesText = value; }
		}

		/// <summary>
		/// Url for Properties Link
		/// </summary>
		public String PropertiesUrl
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.PropertiesUrl != string.Empty)
					propertiesUrl = this.ModuleTitle.PropertiesUrl;
				return propertiesUrl;
			}
			set { propertiesUrl = value; }
		}

		/// <summary>
		/// Target frame/page for Properties Link
		/// </summary>
		public String PropertiesTarget
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.PropertiesTarget != string.Empty)
					propertiesTarget = this.ModuleTitle.PropertiesTarget;
				return propertiesTarget;
			}
			set { propertiesTarget = value; }
		}

		private string securityText = "SECURITY";
		private string securityUrl = "~/DesktopModules/Admin/ModuleSettings.aspx";
		private string securityTarget;

		/// <summary>
		/// Text for Security Link
		/// </summary>
		public String SecurityText
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.SecurityText != string.Empty)
					securityText = this.ModuleTitle.SecurityText;
				return securityText;
			}
			set { securityText = value; }
		}

		/// <summary>
		/// Url for Security Link
		/// </summary>
		public String SecurityUrl
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.SecurityUrl != string.Empty)
					securityUrl = this.ModuleTitle.SecurityUrl;
				return securityUrl;
			}
			set { securityUrl = value; }
		}

		/// <summary>
		/// Target frame/page for Security Link
		/// </summary>
		public String SecurityTarget
		{
			get
			{
				if (this.ModuleTitle != null && this.ModuleTitle.SecurityTarget != string.Empty)
					securityTarget = this.ModuleTitle.SecurityTarget;
				return securityTarget;
			}
			set { securityTarget = value; }
		}

		#endregion

		#region Permissions

		/// <summary>
		/// View permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsViewable
		{
			get
			{
				if (_moduleConfiguration == null || _moduleConfiguration.AuthorizedViewRoles == null)
					return false;

				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (_canView == 0)
				{
					if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedViewRoles))
						_canView = 1;
					else
						_canView = 2;
				}
				return (_canView == 1);
			}
		}

		/// <summary>
		/// Add permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAddable
		{
			get
			{
				if (_moduleConfiguration == null || _moduleConfiguration.AuthorizedAddRoles == null)
					return false;

				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (_canAdd == 0)
				{
					// Change by Geert.Audenaert@Syntegra.Com
					// Date: 7/2/2003
					if (SupportsWorkflow && Version == WorkFlowVersion.Production)
						_canAdd = 2;
					else
					{
						// End Change Geert.Audenaert@Syntegra.Com
						if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedAddRoles))
							_canAdd = 1;
						else
							_canAdd = 2;
						// Change by Geert.Audenaert@Syntegra.Com
					}
					// Date: 7/2/2003
					// End Change Geert.Audenaert@Syntegra.Com
				}
				return (_canAdd == 1);
			}
		}

		/// <summary>
		/// Edit permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEditable
		{
			get
			{
				if (_moduleConfiguration == null || _moduleConfiguration.AuthorizedEditRoles == null)
					return false;

				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (_canEdit == 0)
				{
					// Change by Geert.Audenaert@Syntegra.Com
					// Date: 7/2/2003
					if (SupportsWorkflow && Version == WorkFlowVersion.Production)
						_canEdit = 2;
					else
					{
						// End Change Geert.Audenaert@Syntegra.Com
						//						if (portalSettings.AlwaysShowEditButton == true || PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedEditRoles))
						if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedEditRoles))
							_canEdit = 1;
						else
							_canEdit = 2;
						// Change by Geert.Audenaert@Syntegra.Com
						// Date: 7/2/2003
					}
					// End Change Geert.Audenaert@Syntegra.Com
				}
				return (_canEdit == 1);
			}
		}

		/// <summary>
		/// Delete permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDeleteable
		{
			get
			{
				if (_moduleConfiguration == null || _moduleConfiguration.AuthorizedDeleteRoles == null)
					return false;

				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (_canDelete == 0)
				{
					// Change by Geert.Audenaert@Syntegra.Com
					// Date: 7/2/2003
					if (SupportsWorkflow && Version == WorkFlowVersion.Production)
						_canDelete = 2;
					else
					{
						// End Change Geert.Audenaert@Syntegra.Com
						if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedDeleteRoles))
							_canDelete = 1;
						else
							_canDelete = 2;
						// Change by Geert.Audenaert@Syntegra.Com
						// Date: 7/2/2003
					}
					// End Change Geert.Audenaert@Syntegra.Com
				}
				return (_canDelete == 1);
			}
		}

		/// <summary>
		/// Edit properties permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ArePropertiesEditable
		{
			get
			{
				if (_moduleConfiguration == null || _moduleConfiguration.AuthorizedPropertiesRoles == null)
					return false;

				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (_canProperties == 0)
				{
					if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedPropertiesRoles))
						_canProperties = 1;
					else
						_canProperties = 2;
				}
				return (_canProperties == 1);
			}
		}

		/// <summary>
		/// Minimize permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanMinimized
		{
			get
			{
				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (_canMin == 0)
				{
					if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedViewRoles))
						_canMin = 1;
					else
						_canMin = 2;
				}
				return (_canMin == 1);
			}
		} // end of CanMinimized

		/// <summary>
		/// Close permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanClose
		{
			get
			{
				// Perform tri-state switch check to avoid having to perform a security
				// role lookup on every property access (instead caching the result)
				if (this._canClose == 0)
				{
					if (PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedDeleteRoles))
						_canClose = 1;
					else
						_canClose = 2;
				}
				return (_canClose == 1);
			}
		} // end of CanClose

		/// <summary>
		/// Print permission
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanPrint
		{
			get
			{
				if (this.SupportsPrint && Settings["MODULESETTINGS_SHOW_PRINT_BUTTION"] != null && bool.Parse(Settings["MODULESETTINGS_SHOW_PRINT_BUTTION"].ToString()))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for DeleteModuleButton
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanDeleteModule
		{
			get
			{
				if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteModuleRoles))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for HelpButton
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanHelp
		{
			get
			{
				if (this.SupportsHelp && ((PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles)) || (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles)) || (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles)) || (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPropertiesRoles)) || (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles))))
					return true;
				else
					return false;
			}
		}


		/// <summary>
		/// Permission for BackButton
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanBack
		{
			get
			{
				if (this.SupportsBack && this.ShowBack && this.Request.UrlReferrer != null)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for EmailButton
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanEmail
		{
			get
			{
				if (this.SupportsEmail && Settings["ShowEmailButton"] != null && bool.Parse(Settings["ShowEmailButton"].ToString()))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for EditButton
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanEdit
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if ((this.SupportsWorkflow && this.Version == WorkFlowVersion.Staging) || !this.SupportsWorkflow)
				{
					if ((PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles)) && (EditUrl != null) && (EditUrl != string.Empty) && (this.WorkflowStatus == WorkflowState.Original || this.WorkflowStatus == WorkflowState.Working))
						return true;
					else
						return false;
				}
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for AddButton
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanAdd
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if ((this.SupportsWorkflow && this.Version == WorkFlowVersion.Staging) || !this.SupportsWorkflow)
				{
					if ((PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles)) && (AddUrl != null) && (AddUrl != string.Empty) && (this.WorkflowStatus == WorkflowState.Original || this.WorkflowStatus == WorkflowState.Working))
						return true;
					else
						return false;
				}
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for Version Button
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanVersion
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if (this.SupportsWorkflow && (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles) || PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles) || PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles) || PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedApproveRoles) || PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles)) && (ProductionVersionText != null) && (StagingVersionText != null))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for Publish Button
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanPublish
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if (this.SupportsWorkflow && PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles) && (PublishText != null) && this.Version == WorkFlowVersion.Staging && this.WorkflowStatus == WorkflowState.Approved)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for Approve/Reject Buttons
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanApproveReject
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if (this.SupportsWorkflow && PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedApproveRoles) && (ApproveText != null) && (RejectText != null) && this.Version == WorkFlowVersion.Staging && this.WorkflowStatus == WorkflowState.ApprovalRequested)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for ReadyToApprove and Revert Buttons
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanRequestApproval
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if (this.SupportsWorkflow && (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles) || PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles) || PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles)) && (ReadyToApproveText != null) && this.Version == WorkFlowVersion.Staging && this.WorkflowStatus == WorkflowState.Working)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for Arrow Buttons (Up/Down/Left/Right)
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanArrows
		{
			get
			{
				if (this.ModuleConfiguration == null || this.ModuleID == 0)
					return false;

				if (this.SupportsArrows && PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedMoveModuleRoles))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for Security Button
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanSecurity
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPropertiesRoles) && this.SecurityUrl != null && this.SecurityUrl != string.Empty)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Permission for Properties Button
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanProperties
		{
			get
			{
				if (this.ModuleConfiguration == null)
					return false;

				if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPropertiesRoles) && this.PropertiesUrl != null && this.PropertiesUrl != string.Empty)
					return true;
				else
					return false;
			}
		}

		private bool _showBack = false;

		/// <summary>
		/// Can be set from module code to indicate whether module should display Back button
		/// </summary>
		public bool ShowBack
		{
			get { return _showBack; }
			set { _showBack = value; }
		}

		#endregion

		#region Workflow

		private string productionVersionText = "SWI_SWAPTOPRODUCTION";

		/// <summary>
		/// Text for version Link for swapping to production version
		/// </summary>
		public string ProductionVersionText
		{
			get { return productionVersionText; }
			set { productionVersionText = value; }
		}

		private string stagingVersionText = "SWI_SWAPTOSTAGING";

		/// <summary>
		/// Text for version Link for swapping to staging version
		/// </summary>
		public string StagingVersionText
		{
			get { return stagingVersionText; }
			set { stagingVersionText = value; }
		}

		private string publishText = "SWI_PUBLISH";

		/// <summary>
		/// Text for publish link
		/// </summary>
		public string PublishText
		{
			get { return publishText; }
			set { publishText = value; }
		}

		private string revertText = "SWI_REVERT";

		public string RevertText
		{
			get { return revertText; }
			set { revertText = value; }
		}

		private string readyToApproveText = "SWI_READYTOAPPROVE";

		/// <summary>
		/// Text for request approval link
		/// </summary>
		public string ReadyToApproveText
		{
			get { return readyToApproveText; }
			set { readyToApproveText = value; }
		}

		private string approveText = "SWI_APPROVE";

		/// <summary>
		/// Text for approve link
		/// </summary>
		public string ApproveText
		{
			get { return approveText; }
			set { approveText = value; }
		}

		private string rejectText = "SWI_REJECT";

		/// <summary>
		/// Text for reject link
		/// </summary>
		public string RejectText
		{
			get { return rejectText; }
			set { rejectText = value; }
		}

		/// <summary>
		/// Publish staging to production
		/// </summary>
		protected virtual void Publish()
		{
			// Publish module
			WorkFlowDB.Publish(ModuleConfiguration.ModuleID);

			// Show the prod version
			Version = WorkFlowVersion.Production;
		}


		// Change by Geert.Audenaert@Syntegra.Com
		// Date: 27/2/2003
		/// <summary>
		/// This property indicates the staging content state
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkflowState WorkflowStatus
		{
			get
			{
				if (SupportsWorkflow)
					return _moduleConfiguration.WorkflowStatus;
				else
					return WorkflowState.Original;
			}
		}

		// End Change Geert.Audenaert@Syntegra.Com


		// Change by Geert.Audenaert@Syntegra.Com
		// Date: 6/2/2003
		/// <summary>
		/// This property indicates which content will be shown to the user
		/// production content or staging content
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkFlowVersion Version
		{
			get
			{
				if (!SupportsWorkflow)
					return WorkFlowVersion.Staging;
				else
					return _version;
			}
			set
			{
				if (value == WorkFlowVersion.Staging)
				{
					if (! (PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedAddRoles) || PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedDeleteRoles) || PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedEditRoles) || PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedPublishingRoles) || PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedApproveRoles)))
						PortalSecurity.AccessDeniedEdit();
				}
				_version = value;
			}
		}

		#endregion

		#region Search

		/// <summary>
		/// Searchable module implementation
		/// </summary>
		/// <param name="portalID">The portal ID</param>
		/// <param name="userID">ID of the user is searching</param>
		/// <param name="searchString">The text to search</param>
		/// <param name="searchField">The fields where perfoming the search</param>
		/// <returns>The SELECT sql to perform a search on the current module</returns>
		public virtual string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
		{
			return string.Empty;
		}

		#endregion

		#region LastModified

		/// <summary>
		/// Returns the "Last Modified" string, or an empty string if option is not active.
		/// </summary>
		/// <returns></returns>
		public string GetLastModified()
		{
			// CHANGE by david.verberckmoes@syntegra.com on june, 2 2003
			if (bool.Parse(((SettingItem) portalSettings.CustomSettings["SITESETTINGS_SHOW_MODIFIED_BY"]).Value) && bool.Parse(((SettingItem) Settings["MODULESETTINGS_SHOW_MODIFIED_BY"]).Value))
			{
				// Get stuff from database
				string Email = string.Empty;
				DateTime TimeStamp = DateTime.MinValue;
				WorkFlowDB.GetLastModified(ModuleID, Version, ref Email, ref TimeStamp);

				// Do some checking
				if (Email == string.Empty)
					return string.Empty;

				// Check if email address is valid
				EmailAddressList eal = new EmailAddressList();
				try
				{
					eal.Add(Email);
					Email = "<a href=\"mailto:" + Email + "\">" + Email + "</a>";
				}
				catch
				{
				}

				// Construct the rest of the html
				return "<span class=\"LastModified\">" + Localize.GetString("LMB_LAST_MODIFIED_BY", "Last modified by", null) + "&#160;" + Email + "&#160;" + Localize.GetString("LMB_ON", "on", null) + "&#160;" + TimeStamp.ToLongDateString() + " " + TimeStamp.ToShortTimeString() + "</span>";
			}
			else
				return string.Empty;
			// END CHANGE by david.verberckmoes@syntegra.com on june, 2 2003

		}

		#endregion

		#region Arrow button functions

		// Nicholas Smeaton (24/07/2004) - Arrow button functions START
		/// <summary>
		/// function for module moving
		/// </summary>
		/// <returns></returns>
		private ModuleSettings GetModule()
		{
			// Obtain selected module data
			// get the portal setting at the Tab level and not from this class as it is not refreshed
			foreach (ModuleSettings _module in this.Page.portalSettings.ActiveTab.Modules)
			{
				if (_module.ModuleID == this.OriginalModuleID) // tiptopweb : OriginalModuleID to have it work with shortcut module
					return _module;
			}
			return null;
		}

		/// <summary>
		/// function for module moving
		/// </summary>
		/// <param name="list"></param>
		private void OrderModules(ArrayList list)
		{
			int i = 1;

			// sort the arraylist
			list.Sort();

			// renumber the order
			foreach (ModuleItem m in list)
			{
				// number the items 1, 3, 5, etc. to provide an empty order
				// number when moving items up and down in the list.
				m.Order = i;
				i += 2;
			}
		}

		/// <summary>
		/// The GetModules helper method is used to get the modules
		/// for a single pane within the tab
		/// </summary>
		/// <param name="pane"></param>
		/// <returns></returns>
		private ArrayList GetModules(String pane)
		{
			ArrayList paneModules = new ArrayList();

			// get the portal setting at the Tab level and not from this class as it is not refreshed
			foreach (ModuleSettings _module in this.Page.portalSettings.ActiveTab.Modules)
			{
				if ((_module.PaneName).ToLower() == pane.ToLower())
				{
					ModuleItem m = new ModuleItem();
					m.Title = _module.ModuleTitle;
					m.ID = _module.ModuleID;
					m.ModuleDefID = _module.ModuleDefID;
					m.Order = _module.ModuleOrder;
					m.PaneName = _module.PaneName; // tiptopweb
					paneModules.Add(m);
				}
			}

			return paneModules;
		}

		/// <summary>
		/// function for module moving
		/// </summary>
		/// <param name="url"></param>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		private string AppendModuleID(string url, int moduleID)
		{
			// tiptopweb, sometimes the home page does not have parameters 
			// so we test for both & and ?

			int selectedModIDPos = url.IndexOf("&selectedmodid");
			int selectedModIDPos2 = url.IndexOf("?selectedmodid");
			if (selectedModIDPos >= 0)
			{
				int selectedModIDEndPos = url.IndexOf("&", selectedModIDPos + 1);
				if (selectedModIDEndPos >= 0)
					return url.Substring(0, selectedModIDPos) + "&selectedmodid=" + moduleID + url.Substring(selectedModIDEndPos);
				else
					return url.Substring(0, selectedModIDPos) + "&selectedmodid=" + moduleID;
			}
			else if (selectedModIDPos2 >= 0)
			{
				int selectedModIDEndPos2 = url.IndexOf("?", selectedModIDPos2 + 1);
				if (selectedModIDEndPos2 >= 0)
					return url.Substring(0, selectedModIDPos2) + "?selectedmodid=" + moduleID + url.Substring(selectedModIDEndPos2);
				else
					return url.Substring(0, selectedModIDPos2) + "?selectedmodid=" + moduleID;
			}
			else
			{
				if (url.IndexOf("?") >= 0)
					return url + "&selectedmodid=" + moduleID;
				else
					return url + "?selectedmodid=" + moduleID;
			}
		}

		#endregion

		#region Module Content Sizing

		// Added by gman3001: 2004/10/26 to support specific module content sizing and scrolling capabilities
		/// <summary>
		/// Returns a module content sizing container tag with properties
		/// </summary>
		/// <paramref name="isBeginTag">Specifies whether to output the container's begin(true) or end(false) tag.</paramref>
		/// <returns>The literal control containing this tag</returns>
		private LiteralControl BuildModuleContentContainer(bool isBeginTag)
		{
			LiteralControl modContainer = new LiteralControl();
			int width = (Settings["MODULESETTINGS_CONTENT_WIDTH"] != null) ? Int32.Parse(Settings["MODULESETTINGS_CONTENT_WIDTH"].ToString()) : 0;
			int height = (Settings["MODULESETTINGS_CONTENT_HEIGHT"] != null) ? Int32.Parse(Settings["MODULESETTINGS_CONTENT_HEIGHT"].ToString()) : 0;
			bool scrolling = (Settings["MODULESETTINGS_CONTENT_SCROLLING"] != null) ? bool.Parse(Settings["MODULESETTINGS_CONTENT_SCROLLING"].ToString()) : false;
			if (isBeginTag)
			{
				string StartContentSizing = "<div class='modulePadding moduleScrollBars' id='modcont_" + this.ClientID + "' ";
				StartContentSizing += " style='POSITION: static; ";
				if (!_isPrint && width > 0)
					StartContentSizing += "width: " + width.ToString() + "px; ";
				if (!_isPrint && height > 0)
					StartContentSizing += "height: " + height.ToString() + "px; ";
				if (!_isPrint && scrolling)
					StartContentSizing += "overflow:auto;";
				StartContentSizing += "'>";
				modContainer.Text = StartContentSizing;
			}
			else
			{
				if (this.Page.Request.Browser.JavaScript && !_isPrint && (width > 0 || height > 0 || (width > 0 && scrolling) || (height > 0 && scrolling)))
				{
					// Register a client side script that will properly resize the content area of the module
					// to compensate for different height and width settings, as well as, the browser's tendency
					// to stretch the middle module width even when a specific width setting is specified.
					if (!this.Page.IsClientScriptBlockRegistered("autoSizeModules"))
					{
						string src = Path.ApplicationRootPath("aspnet_client/Rainbow_scripts/autoResizeModule.js");
						this.Page.RegisterClientScriptBlock("autoSizeModules", "<script language=javascript src='" + src + "'></script>");
						this.Page.RegisterStartupScript("initAutoSizeModules", "<script defer language=javascript>if (document._portalmodules) document._portalmodules.GetModules(_portalModules); document._portalmodules.ProcessAll();</script>");
					}
					this.Page.RegisterArrayDeclaration("_portalModules", "'modcont_" + this.ClientID + "'");
				}
				modContainer.Text = "</div>\r";
			}

			return modContainer;
		}

		// Added by gman3001: 2004/10/26 to support module width stretching/shrinking capability
		/// <summary>
		/// Updates the moduleControl literal control with proper width settings to render the 'module width stretching' option
		/// </summary>
		/// <paramref name="moduleControl">The literal control element to parse and modify.</paramref>
		/// <paramref name="isBeginTag">Specifies whether the moduleElement parameter is for the element's begin(true) or end(false) tag.</paramref>
		/// <returns></returns>
		private void ProcessModuleStrecthing(Control moduleControl, bool isBeginTag)
		{
			if (moduleControl is LiteralControl && moduleControl != null)
			{
				LiteralControl moduleElement = (LiteralControl) moduleControl;
				bool isStretched = (Settings["MODULESETTINGS_WIDTH_STRETCHING"] != null && bool.Parse(Settings["MODULESETTINGS_WIDTH_STRETCHING"].ToString()) == true);
				string tmp = (moduleElement.Text != null) ? moduleElement.Text.Trim() : string.Empty;
				//Need to remove the current width setting of the main table in the module Start(Title/NoTitle)Content section of the theme,
				//if this is to be a stretched module then insert a width attribute into it,
				//if not, then surround this table with another table that has an empty cell after the cell that contains the module's HTML,
				//in order to use up any space in the window that the module has not been defined for.
				//if, no width specified for module then the module will be at least 50% width of area remaining, or expand to hold its contents.
				if (isBeginTag)
				{
					MatchCollection mc;
					Regex r = new Regex("<table[^>]*>");
					mc = r.Matches(tmp.ToLower());
					//Only concerned with first match
					if (mc.Count > 0)
					{
						string TMatch = mc[0].Value;
						int TIndx = mc[0].Index;
						int TLength = mc[0].Value.Length;
						//find a width attribute in this match(if exists remove it)
						Regex r1 = new Regex("width=((['\"][^'\"]*['\"])|([0-9]+))");
						mc = r1.Matches(TMatch);
						if (mc.Count > 0)
						{
							int WIndx = mc[0].Index;
							int WLength = mc[0].Value.Length;
							tmp = tmp.Substring(0, WIndx + TIndx) + tmp.Substring(WIndx + TIndx + WLength);
							TMatch = TMatch.Substring(0, WIndx) + TMatch.Substring(WIndx + WLength);
						}
						//find a style attribute in this match(if exists)
						Regex r2 = new Regex("style=['\"][^'\"]*['\"]");
						mc = r2.Matches(TMatch);
						if (mc.Count > 0)
						{
							int SIndx = mc[0].Index;
							int SLength = mc[0].Value.Length;
							//Next find a width style property(if exists) and modify it
							Regex r3 = new Regex("width:[^;'\"]+[;'\"]");
							mc = r3.Matches(mc[0].Value);
							if (mc.Count > 0)
							{
								int SwIndx = mc[0].Index;
								int SwLength = mc[0].Value.Length - 1;
								if (isStretched)
									tmp = tmp.Substring(0, SIndx + SwIndx + TIndx) + "width:100%" + tmp.Substring(SIndx + SwIndx + TIndx + SwLength);
								else
									tmp = tmp.Substring(0, SIndx + SwIndx + TIndx) + tmp.Substring(SIndx + SwIndx + TIndx + SwLength);
							}
								//Else, Add width style property to the existing style attribute
							else if (isStretched)
								tmp = tmp.Substring(0, SIndx + TIndx + 7) + "width:100%;" + tmp.Substring(SIndx + TIndx + 7);
						}
							//Else, Add width style property to a new style attribute
						else if (isStretched)
							tmp = tmp.Substring(0, TIndx + 7) + "style='width:100%' " + tmp.Substring(TIndx + 7);
					}

					if (!isStretched)
						tmp = "<table cellpadding=0 cellspacing=0 border=0><tr>\n<td>" + tmp;
				}
				else if (!isStretched)
					tmp += "</td><td></td>\n</tr></table>";
				moduleElement.Text = tmp;
			}
		}

		#endregion

		#region Theme/Rendering

		/// <summary>
		/// Before apply theme DesktopModule designer checks this 
		/// property to be true.<br/>
		/// On inherited modules you can override this 
		/// property to not apply theme on specific controls.
		/// </summary>
		public virtual bool ApplyTheme
		{
			get
			{
				if (HttpContext.Current != null) // if it is not design time
					return (bool.Parse(this.Settings["MODULESETTINGS_APPLY_THEME"].ToString()));
				return true;
			}
			set
			{
				if (HttpContext.Current != null) // if it is not design time
					this.Settings["MODULESETTINGS_APPLY_THEME"] = value.ToString();
			}
		}

		//Added by james for localization purpose
		/// <summary>
		/// Localises Theme types: 'Default' and 'Alt'
		/// </summary>
		public enum ThemeList : int
		{
			Default = 0,
			Alt = 1
		}

		/// <summary>
		/// used to hold the consolidated list of buttons for the module
		/// </summary>
		private ArrayList ButtonList = new ArrayList(3);

		private ArrayList buttonListUser = new ArrayList(3);
		/// <summary>
		/// User Buttons
		/// </summary>
		public ArrayList ButtonListUser
		{
			get
			{
				return buttonListUser;
			}
			set
			{
				buttonListUser = value;
                
			}
		}


		private ArrayList buttonListAdmin = new ArrayList(3);
		/// <summary>
		/// Admin Buttons
		/// </summary>
		public ArrayList ButtonListAdmin
		{
			get
			{
				return buttonListAdmin;
			}
			set
			{
				buttonListAdmin = value;
                
			}
		}


		private ArrayList buttonListCustom = new ArrayList(3);
		/// <summary>
		/// Custom Buttons
		/// </summary>
		public ArrayList ButtonListCustom
		{
			get
			{
				return buttonListCustom;
			}
			set
			{
				buttonListCustom = value;
                
			}
		}


		// switches used for building module hierarchy
		private bool _buildTitle = true;
		private bool _buildButtons = true;
		private bool _buildBody = true;
		private bool _beforeContent = true;
		private bool _isPrint = false;

		/// <summary>
		/// Makes the decisions about what needs to be built and calls the appropriate method
		/// </summary>
		/// 
		protected virtual void BuildControlHierarchy()
		{
			if (this.NamingContainer.ToString().Equals("ASP.print_aspx"))
			{
				_isPrint = true;
				_buildButtons = false;
				if (!this.ShowTitlePrint)
					_buildTitle = false;
			}
			else if (this.SupportCollapsable && UserDesktop.isMinimized(this.ModuleID))
				_buildBody = false;
			else if (!this.ShowTitle)
				_buildTitle = false;

			// added Jes1111: https://sourceforge.net/tracker/index.php?func=detail&aid=1034935&group_id=66837&atid=515929
			if (this.ButtonList.Count == 0)
				_buildButtons = false;

			// changed Jes1111 - 2004-09-29 - to correct shortcut behaviour
			if (this.ModuleID != this.OriginalModuleID)
				BuildShortcut();
			else if (!ApplyTheme || _isPrint)
				BuildNoTheme();
			else if (this.CurrentTheme.Type.Equals("zen"))
				ZenBuild();
			else
				Build();

			// wrap the module in a &lt;div&gt; with the ModuleID and Module type name - needed for Zen support and useful for CSS styling and debugging ;-)
			this._header.Controls.AddAt(0, new LiteralControl(string.Format("<div id=\"mID{0}\" class=\"{1}\">", this.ModuleID.ToString(), this.GetType().Name)));
			this._footer.Controls.Add(new LiteralControl("</div>"));
		}

		protected virtual void BuildShortcut()
		{
			// do nothing - just passes the target contents through. The theme will be applied
			// to the containing shortcut module.
		}

		/// <summary>
		/// Method builds "no theme" version of module. Now obeys ShowTitle and GetLastModified.
		/// </summary>
		protected virtual void BuildNoTheme()
		{
			Table t = new Table();
			t.Attributes.Add("width", "100%");
			t.CssClass = "TitleNoTheme";
			TableRow tr = new TableRow();
			t.Controls.Add(tr);
			TableCell tc;

			if (_buildTitle)
			{
				tc = new TableCell();
				tc.Attributes.Add("width", "100%");
				tc.CssClass = "TitleNoTheme";
				tc.Controls.Add(new LiteralControl(this.TitleText));
				tr.Controls.Add(tc);
			}

			if (_buildButtons)
			{
				foreach (Control _button in this.ButtonList)
				{
					tc = new TableCell();
					tc.Controls.Add(_button); //Add Button
					tr.Controls.Add(tc);
				}
			}

			if (_buildTitle || _buildButtons)
				this._header.Controls.Add(t);

			if (!_buildBody)
			{
				for (int i = 1; i < this.Controls.Count - 1; i++)
				{
					this.Controls[i].Visible = false;
				}
			}
			else
				this._footer.Controls.Add(new LiteralControl(GetLastModified()));
		}

		/// <summary>
		/// Builds the "with theme" versions of the module, with optional Title, Buttons and Body.
		/// </summary>
		protected virtual void Build()
		{
			if (!_buildTitle && !_buildButtons)
				this._header.Controls.Add(CurrentTheme.GetLiteralControl("ControlNoTitleStart"));
			else
			{
				this._header.Controls.Add(CurrentTheme.GetLiteralControl("ControlTitleStart"));

				this._header.Controls.Add(CurrentTheme.GetLiteralControl("TitleStart"));

				if (_buildTitle)
					this._header.Controls.Add(new LiteralControl(this.TitleText));

				this._header.Controls.Add(CurrentTheme.GetLiteralControl("TitleMiddle"));

				if (_buildButtons)
				{
					foreach (Control _button in this.ButtonList)
					{
						this._header.Controls.Add(CurrentTheme.GetLiteralControl("TitleBeforeButton"));
						this._header.Controls.Add(_button);
						this._header.Controls.Add(CurrentTheme.GetLiteralControl("TitleAfterButton"));
					}
				}

				this._header.Controls.Add(CurrentTheme.GetLiteralControl("TitleEnd"));
			}

			if (!_buildBody)
			{
				for (int i = 1; i < this.Controls.Count - 1; i++)
				{
					this.Controls[i].Visible = false;
				}
			}
			else
				this._footer.Controls.Add(new LiteralControl(GetLastModified()));

			// Added by gman3001: 2004/10/26 to support auto width sizing and scrollable module content
			// this must be the first footer control (besides custom ones such as GetLastModified)
			if (_buildBody)
				this._footer.Controls.Add(BuildModuleContentContainer(false));

			// changed Jes1111: https://sourceforge.net/tracker/index.php?func=detail&aid=1034935&group_id=66837&atid=515929
			if (!_buildTitle && !_buildButtons)
				this._footer.Controls.Add(CurrentTheme.GetLiteralControl("ControlNoTitleEnd"));
			else
			{
				this._header.Controls.Add(CurrentTheme.GetLiteralControl("ControlTitleBeforeControl"));

				this._footer.Controls.AddAt(0, CurrentTheme.GetLiteralControl("ControlTitleAfterControl"));
				this._footer.Controls.Add(CurrentTheme.GetLiteralControl("ControlTitleEnd"));
			}

			// Added by gman3001: 2004/10/26 to support auto width sizing and scrollable module content
			// this must be the last header control
			if (_buildBody)
				this._header.Controls.Add(BuildModuleContentContainer(true));

			if (!_isPrint && this._header.Controls.Count > 0 && this._footer.Controls.Count > 0)
			{
				//Process the first header control as the module's outer most begin tag element
				ProcessModuleStrecthing(this._header.Controls[0], true);
				//Process the last footer control as the module's outer most end tag element
				ProcessModuleStrecthing(this._footer.Controls[this._footer.Controls.Count - 1], false);
			}
			// End add by gman3001

		}

		/// <summary>
		/// The Zen version of Build(). Parses XML Zen Module Layout.
		/// </summary>
		protected virtual void ZenBuild()
		{
			XmlTextReader _xtr = null;
			XmlTextReader _xtr2 = null;
			NameTable _nt = new NameTable();
			XmlNamespaceManager _nsm = new XmlNamespaceManager(_nt);
			_nsm.AddNamespace(string.Empty, "http://www.w3.org/1999/xhtml");
			_nsm.AddNamespace("if", "urn:MarinaTeq.Rainbow.Zen.Condition");
			_nsm.AddNamespace("loop", "urn:Marinateq.Rainbow.Zen.Looping");
			_nsm.AddNamespace("content", "urn:www.rainbowportal.net");
			XmlParserContext _context = new XmlParserContext(_nt, _nsm, string.Empty, XmlSpace.None);
			StringBuilder _fragText;
			LiteralControl _frag;
			string _loopFrag = string.Empty;

			try
			{
				_xtr = new XmlTextReader(this.CurrentTheme.GetThemePart("ModuleLayout"), XmlNodeType.Document, _context);

				while (_xtr.Read())
				{
					_frag = new LiteralControl();
					switch (_xtr.Prefix)
					{
						case "if":
						{
							if (_xtr.NodeType == XmlNodeType.Element && !ZenEvaluate(_xtr.LocalName))
								_xtr.Skip();
							break;
						}

						case "loop":
						{
							if (_xtr.NodeType == XmlNodeType.Element)
							{
								switch (_xtr.LocalName)
								{
									case "Buttons":
									{
										_loopFrag = _xtr.ReadInnerXml();
										foreach (Control c in this.ButtonList)
										{
											_xtr2 = new XmlTextReader(_loopFrag, XmlNodeType.Document, _context);
											while (_xtr2.Read())
											{
												_frag = new LiteralControl();
												switch (_xtr2.Prefix)
												{
													case "content":
													{
														switch (_xtr2.LocalName)
														{
															case "Button":
																//																if ( this.CurrentTheme.Name.ToLower().Equals("zen-zero") && c is ModuleButton )
																//																	((ModuleButton)c).RenderAs = ModuleButton.RenderOptions.TextOnly;
																//																if ( _beforeContent )
																this._header.Controls.Add(c);
																//																else
																//																	this._footer.Controls.Add(c);
																break;
															default:
																break;
														}
														break;
													}
													case "":
													default:
													{
														if (_xtr2.NodeType == XmlNodeType.Element)
														{
															_fragText = new StringBuilder("<");
															_fragText.Append(_xtr2.LocalName);
															while (_xtr2.MoveToNextAttribute())
															{
																if (_xtr2.LocalName != "xmlns")
																{
																	_fragText.Append(" ");
																	_fragText.Append(_xtr.LocalName);
																	_fragText.Append("=\"");
																	_fragText.Append(_xtr.Value);
																	_fragText.Append("\"");
																}
															}
															_fragText.Append(">");
															_frag.Text = _fragText.ToString();
															if (_beforeContent)
																this._header.Controls.Add(_frag);
															else
																this._footer.Controls.Add(_frag);
														}
														else if (_xtr2.NodeType == XmlNodeType.EndElement)
														{
															_frag.Text = string.Format("</{0}>", _xtr2.LocalName);
															if (_beforeContent)
																this._header.Controls.Add(_frag);
															else
																this._footer.Controls.Add(_frag);
														}
														break;
													}
												} // end switch
											} // end while
										} // end foreach
										break;
									}
									default:
										break;
								} // end inner switch
							}
							break;
						}

						case "content":
						{
							switch (_xtr.LocalName)
							{
								case "ModuleContent":
									_beforeContent = false;
									break;
								case "TitleText":
									_frag.Text = this.TitleText;
									if (_beforeContent)
										this._header.Controls.Add(_frag);
									else
										this._footer.Controls.Add(_frag);
									break;
								case "ModifiedBy":
									_frag.Text = this.GetLastModified();
									if (_beforeContent)
										this._header.Controls.Add(_frag);
									else
										this._footer.Controls.Add(_frag);
									break;
								default:
									break;
							}
							break;
						}

						case "":
						default:
						{
							if (_xtr.NodeType == XmlNodeType.Element)
							{
								_fragText = new StringBuilder("<");
								_fragText.Append(_xtr.LocalName);
								while (_xtr.MoveToNextAttribute())
								{
									_fragText.Append(" ");
									_fragText.Append(_xtr.LocalName);
									_fragText.Append("=\"");
									_fragText.Append(_xtr.Value);
									_fragText.Append("\"");
								}
								_fragText.Append(">");
								_frag.Text = _fragText.ToString();
								if (_beforeContent)
									this._header.Controls.Add(_frag);
								else
									this._footer.Controls.Add(_frag);
							}
							else if (_xtr.NodeType == XmlNodeType.EndElement)
							{
								_frag.Text = string.Format("</{0}>", _xtr.LocalName);
								if (_beforeContent)
									this._header.Controls.Add(_frag);
								else
									this._footer.Controls.Add(_frag);
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Fatal, "Fatal error in ZenBuildControlHierarchy(): " + ex.Message);
				throw new ApplicationException("Fatal error in ZenBuildControlHierarchy(): " + ex.Message);
			}
			finally
			{
				if (_xtr != null)
					_xtr.Close();
			}
		}

		/// <summary>
		/// Supports ZenBuild(), evaluates 'if' commands
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		private bool ZenEvaluate(string condition)
		{
			bool _returnVal = false;

			switch (condition)
			{
				case "Title":
					if (_buildTitle)
						_returnVal = true;
					break;
				case "Buttons":
					//if ( _buildButtons && this.ButtonList.Count > 0 )
					if (_buildButtons)
						_returnVal = true;
					break;
				case "Body":
				case "Footer":
					if (!_buildBody)
					{
						_returnVal = false;
						for (int i = 1; i < this.Controls.Count - 1; i++)
						{
							this.Controls[i].Visible = false;
						}
					}
					else
						_returnVal = true;
					break;
				case "ShowModifiedBy":
					if (bool.Parse(((SettingItem) portalSettings.CustomSettings["SITESETTINGS_SHOW_MODIFIED_BY"]).Value) && bool.Parse(((SettingItem) Settings["MODULESETTINGS_SHOW_MODIFIED_BY"]).Value))
						_returnVal = true;
					break;
				default:
					_returnVal = false;
					break;
			}
			return _returnVal;
		}

		#endregion

		#region Module Methods

		/// <summary>
		/// Sets the CurrentTheme - allowing custom Theme per module
		/// </summary>
		protected virtual void SetupTheme()
		{
			// changed: Jes1111 - 2004-08-05 - supports custom theme per module
			// (better to do this in OnLoad than in RenderChildren, which is too late)
			string themeName;
			if (Int32.Parse(Settings["MODULESETTINGS_THEME"].ToString()) == (int) ThemeList.Alt)
				themeName = "Alt";
			else
				themeName = "Default";
			// end: Jes1111

			// added: Jes1111 - 2004-08-05 - supports custom theme per module
			if (portalSettings.CustomSettings.ContainsKey("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES") && portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString() != string.Empty && bool.Parse(portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString()) && this.Settings.ContainsKey("MODULESETTINGS_MODULE_THEME") && this.Settings["MODULESETTINGS_MODULE_THEME"].ToString().Trim().Length > 0)
			{
				// substitute custom theme for this module
				ThemeManager _tm = new ThemeManager(portalSettings.PortalPath);
				_tm.Load(this.Settings["MODULESETTINGS_MODULE_THEME"].ToString());
				CurrentTheme = _tm.CurrentTheme;
				// get CSS file, add ModuleID to each line and add resulting string to CssImportList
				try
				{
					CssHelper cssHelper = new CssHelper();
					string selectorPrefix = string.Concat("#mID", this.ModuleID);
					string cssFileName = this.Page.Server.MapPath(CurrentTheme.CssFile);
					this.Page.RegisterCssImport(this.ModuleID.ToString(), cssHelper.ParseCss(cssFileName, selectorPrefix));
				}
				catch (Exception ex)
				{
					LogHelper.Logger.Log(LogLevel.Error, "Failed to load custom theme '" + CurrentTheme.CssFile + "' for ModuleID " + this.ModuleID + ". Continuing with default tab theme. Message was: " + ex.Message);
					CurrentTheme = portalSettings.GetCurrentTheme(themeName);
				}
			}
			else
			{
				// original behaviour unchanged
				CurrentTheme = portalSettings.GetCurrentTheme(themeName);
			}
			// end change: Jes1111
		}

		/// <summary>
		/// Merges the three public button lists into one.
		/// </summary>
		protected virtual void MergeButtonLists()
		{
			if (this.CurrentTheme.Type != "zen")
			{
				string _divider;
				try
				{
					_divider = this.CurrentTheme.GetHTMLPart("ButtonGroupsDivider");
				}
				catch
				{
					_divider = string.Concat("<img src='", this.CurrentTheme.GetImage("Spacer", "Spacer.gif").ImageUrl, "' class='rb_mod_title_sep'/>");
				}

				// merge the button lists
				if (this.ButtonListUser.Count > 0 && (this.ButtonListCustom.Count > 0 || this.ButtonListAdmin.Count > 0))
					this.ButtonListUser.Add(new LiteralControl(_divider));
				if (this.ButtonListCustom.Count > 0 && this.ButtonListAdmin.Count > 0)
					this.ButtonListCustom.Add(new LiteralControl(_divider));
			}
			foreach (Control btn in this.ButtonListUser)
			{
				this.ButtonList.Add(btn);
			}
			foreach (Control btn in this.ButtonListAdmin)
			{
				this.ButtonList.Add(btn);
			}
			foreach (Control btn in this.ButtonListCustom)
			{
				this.ButtonList.Add(btn);
			}
		}

		/// <summary>
		/// Builds the three public button lists
		/// </summary>
		protected virtual void BuildButtonLists()
		{
			// user buttons
			if (this.BackButton != null) this.ButtonListUser.Add(this.BackButton);
			if (this.PrintButton != null) this.ButtonListUser.Add(this.PrintButton);
			if (this.HelpButton != null) this.ButtonListUser.Add(this.HelpButton);

			// admin buttons
			if (this.AddButton != null)
				this.ButtonListAdmin.Add(this.AddButton);
			if (this.EditButton != null)
				this.ButtonListAdmin.Add(this.EditButton);
			if (this.DeleteModuleButton != null)
				this.ButtonListAdmin.Add(this.DeleteModuleButton);
			if (this.PropertiesButton != null)
				this.ButtonListAdmin.Add(this.PropertiesButton);
			if (this.SecurityButton != null)
				this.ButtonListAdmin.Add(this.SecurityButton);
			if (this.VersionButton != null)
				this.ButtonListAdmin.Add(this.VersionButton);
			if (this.PublishButton != null)
				this.ButtonListAdmin.Add(this.PublishButton);
			if (this.ApproveButton != null)
				this.ButtonListAdmin.Add(this.ApproveButton);
			if (this.RejectButton != null)
				this.ButtonListAdmin.Add(this.RejectButton);
			if (this.ReadyToApproveButton != null)
				this.ButtonListAdmin.Add(this.ReadyToApproveButton);
			if (this.RevertButton != null)
				this.ButtonListAdmin.Add(this.RevertButton);
			if (this.UpButton != null)
				this.ButtonListAdmin.Add(this.UpButton);
			if (this.DownButton != null)
				this.ButtonListAdmin.Add(this.DownButton);
			if (this.LeftButton != null)
				this.ButtonListAdmin.Add(this.LeftButton);
			if (this.RightButton != null)
				this.ButtonListAdmin.Add(this.RightButton);

			// custom buttons
			// recover any CustomButtons set the 'old way'
			if (this.ModuleTitle != null)
			{
				foreach (Control c in this.ModuleTitle.CustomButtons)
				{
					this.ButtonListCustom.Add(c);
				}
			}
			if (this.MinMaxButton != null)
				this.ButtonListCustom.Add(this.MinMaxButton);
			if (this.CloseButton != null)
				this.ButtonListCustom.Add(this.CloseButton);

			// set image url for standard buttons edit & delete
			if (DeleteBtn != null)
				DeleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
			if (EditBtn != null)
				EditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
		}

		/// <summary>
		/// Save module data
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DataSet SaveData(DataSet ds)
		{
			return (ds);
		}

		/// <summary>
		/// Load Data
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DataSet LoadData(DataSet ds)
		{
			return (ds);
		}

		/// <summary>
		/// Unknown
		/// </summary>
		/// <param name="stateSaver"></param>
		public virtual void Install(IDictionary stateSaver)
		{
		}

		/// <summary>
		/// Unknown
		/// </summary>
		/// <param name="stateSaver"></param>
		public virtual void Uninstall(IDictionary stateSaver)
		{
		}

		/// <summary>
		/// Unknown
		/// </summary>
		/// <param name="stateSaver"></param>
		public virtual void Commit(IDictionary stateSaver)
		{
		}

		/// <summary>
		/// Unknown
		/// </summary>
		/// <param name="stateSaver"></param>
		public virtual void Rollback(IDictionary stateSaver)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets resources from assembly
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		private string getResourceContent(string resourceID)
		{
			Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceID);
			if (st != null)
			{
				using (StreamReader sr = new StreamReader(st))
				{
					return sr.ReadToEnd();
				}
			}
			else
				return null;
		}

		#endregion


		#region Obsolete methods
		
		/// <summary>
		/// The GetModuleDesktopSrc Method returns a string of
		/// the specified Module's Desktop Src, which can be used to 
		/// Load this modules control into a page.
		/// </summary>
		/// <remarks>
		/// Added by gman3001 10/06/2004, to allow for the retrieval of a Module's Desktop src path
		/// in order to dynamically load a module control by its Module ID
		/// </remarks>
		/// <param name="moduleID"></param>
		/// <returns>The desktop source path string</returns>
		[Obsolete("Use the Module object's DesktopSrc property instead.")]
		public static string GetModuleDesktopSrc(int moduleID)
		{
			return Path.WebPathCombine(Path.ApplicationRoot, Data.Helper.Modules.Module(moduleID).DesktopSrc);

//			string ControlPath;// = string.Empty; unneeded init
//
//			using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
//			{
//				if (dr.Read())
//					ControlPath = Path.WebPathCombine(Path.ApplicationRoot, dr[strDesktopSrc].ToString());
//				dr.Close(); //by Manu, fixed bug 807858
//			}
//
//			return ControlPath;
		}


		/// <summary>
		/// The MergeModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database.  This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="_baseSettings"></param>
		/// <returns></returns>
		[Obsolete("This no longer does anything.")]
		public static Hashtable MergeModuleSettings(int moduleId, Hashtable _baseSettings)
		{
			//			if (!CurrentCache.Exists(Key.ModuleSettings(moduleID)))
			//			{
			//Hashtable _settings = new Hashtable();

			// Get Settings for this module from the database
			Hashtable _settings = Data.Helper.Modules.Settings(moduleId);
			
			#region old code
			//				// Create Instance of Connection and Command Object
			//				using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			//				{
			//					using (SqlCommand myCommand = new SqlCommand("rb_GetModuleSettings", myConnection))
			//					{
			//						// Mark the Command as a SPROC
			//						myCommand.CommandType = CommandType.StoredProcedure;
			//
			//						// Add Parameters to SPROC
			//						SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
			//						parameterModuleID.Value = moduleID;
			//						myCommand.Parameters.Add(parameterModuleID);
			//
			//						// Execute the command
			//						myConnection.Open();
			//						using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
			//						{
			//							try
			//							{
			//								while (dr.Read())
			//									_settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
			//							}
			//							finally
			//							{
			//								dr.Close(); //by Manu, fixed bug 807858
			//							}
			//						}
			//					}
			//				}
			#endregion
			
			foreach (string key in _baseSettings.Keys)
			{
				if (_settings[key] != null)
				{
					SettingItem s = _baseSettings[key] as SettingItem;
					if (_settings[key].ToString() != string.Empty)
						s.Value = _settings[key].ToString();
					//_baseSettings[key] = s;   s is a ref to _baseSettings[key] *duh*
				}
			}
			
			//			CurrentCache.Insert(Key.ModuleSettings(moduleID), _baseSettings);
			//		}
			//		else
			//		_baseSettings = (Hashtable) CurrentCache.Get(Key.ModuleSettings(moduleID));
			
			return _baseSettings;
	}


		/// <summary>
		///     Gets a module definition
		/// </summary>
		/// <param name="moduleId" type="int">
		///     <para>
		///         the module Id of the module whose definition we want to get
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Guid GetModuleDefinitionById value...
		/// </returns>
		[Obsolete("Use the Module object's properties instead.")]
		public static Guid GetModuleDefinitionById(int moduleId)
		{
			// TODO : Look up the rb_GetModuleDefinitionByID sproc to see that it does and what this should return.
			return Data.Helper.Modules.Module(moduleId).ModuleDefId as Guid;
		}


		#region antique
		//		/// <summary>
		//		/// GetModuleDefinitionByID
		//		/// </summary>
		//		/// <param name="ModuleID">ModuleID</param>
		//		/// <returns>A SqlDataReader</returns>
		//		public static SqlDataReader GetModuleDefinitionByID(int moduleID)
		//		{
		//			// Create Instance of Connection and Command Object
		//			SqlConnection myConnection = PortalSettings.SqlConnectionString;
		//			SqlCommand myCommand = new SqlCommand("rb_GetModuleDefinitionByID", myConnection);
		//
		//			// Mark the Command as a SPROC
		//			myCommand.CommandType = CommandType.StoredProcedure;
		//
		//			// Add Parameters to SPROC
		//			SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int);
		//			parameterModuleID.Value = moduleID;
		//			myCommand.Parameters.Add(parameterModuleID);
		//
		//			// Execute the command
		//			myConnection.Open();
		//			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
		//
		//			// Return the datareader
		//			return result;
		//		}
		#endregion
		
		#endregion

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public Hashtable Settings 
		{
			get 
			{
				if(this.settings == null)
					this.settings = Data.Helper.Modules.Settings(this.Id);
//				else
//					this.settings = MergeModuleSettings(this.Id,this.settings);

              	return this.settings;
			}
			set 
			{
              	this.settings = value;
				Data.Helper.Modules.Settings(this.Id) = value;
			}
		}


		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database. This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static Hashtable MergeModuleSettings(int moduleID, Page page)
		{
			// TODO : Move this function to Web project?

			string ControlPath = Path.ApplicationRoot + "/"
				+ Data.Helper.Modules.Module(moduleID).DesktopSrc;

//			using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
//			{
//				try
//				{
//					if (dr.Read())
//						ControlPath += dr[strDesktopSrc].ToString();
//				}
//				finally
//				{
//					dr.Close(); //by Manu, fixed bug 807858
//				}
//			}

			PortalModuleControl portalModule;
			Hashtable setting;
			try
			{
				portalModule = page.LoadControl(ControlPath) as PortalModuleControl;
				setting = MergeModuleSettings(moduleID, portalModule.BaseSettings);
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
				throw;
			}
			finally 
			{
				return setting;
			}
		}


		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database. This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		public static Hashtable MergeModuleSettings(int moduleID)
		{
			// TODO : Move this function to Web project?

			return (MergeModuleSettings(moduleID, new UI.Page()));
		}


		/// <summary>
		/// The UpdateModuleSetting Method updates a single module setting 
		/// in the ModuleSettings database table.
		/// </summary>
		/// <param name="moduleID"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void UpdateModuleSetting(int moduleID, String key, String value)
		{
			Data.Helper.Modules.Settings(moduleID)[key] = value;

			#region antique code
//			// Create Instance of Connection and Command Object
//			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
//			{
//				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleSetting", myConnection))
//				{
//					// Mark the Command as a SPROC
//					myCommand.CommandType = CommandType.StoredProcedure;
//
//					// Add Parameters to SPROC
//					SqlParameter parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
//					parameterModuleID.Value = moduleID;
//					myCommand.Parameters.Add(parameterModuleID);
//
//					SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
//					parameterKey.Value = key;
//					myCommand.Parameters.Add(parameterKey);
//
//					SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
//					parameterValue.Value = value;
//					myCommand.Parameters.Add(parameterValue);
//
//					// Execute the command
//					myConnection.Open();
//					try
//					{
//						myCommand.ExecuteNonQuery();
//					}
//					finally
//					{
//						myConnection.Close();
//					}
//				}
//			}
//
//			//Invalidate cache
//			CurrentCache.Remove(Key.ModuleSettings(moduleID));
			#endregion
		}

	}
}
