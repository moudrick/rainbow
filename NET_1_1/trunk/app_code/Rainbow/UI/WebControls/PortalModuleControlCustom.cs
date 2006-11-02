using System.Collections;
using System.Web;
using Rainbow.Configuration;
using Rainbow.Settings.Cache;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// A PortalModuleControl that supports CustomUserSettings for authenticated users. (Users can specify
	/// settings for the module instance that will apply only to them when they interact with the module).
	/// </summary>
	public class PortalModuleControlCustom : PortalModuleControl
	{
		// provide a custom Hashtable that will store user-specific settings for this instance
		// of the module.  
		protected Hashtable _customUserSettings;

		public bool HasCustomizeableSettings 
		{
			get
			{
				return this.CustomizedUserSettings.Count > 0;
			}
		}

		
		public Hashtable CustomizedUserSettings
		{
			get
			{
				if (this._customUserSettings != null)
				{
					return this._customUserSettings;
				}
				else
				{

					Hashtable tempSettings = new Hashtable();

					SettingItem _default;

					//refresh this module's settings on every call in case they logged off, so it will
					//retrieve the 'default' settings from the database.
					//Invalidate cache
					CurrentCache.Remove(Key.ModuleSettings(this.ModuleID));
					//this._baseSettings = ModuleSettings.GetModuleSettings(this.ModuleID, this._baseSettings);
			
					foreach (string str in this.Settings.Keys)
					{
						_default = (SettingItem) this.Settings[str];
						if (_default.Group == SettingItemGroup.CUSTOM_USER_SETTINGS)  //It's one we want to customize
						{
							tempSettings.Add(str, _default);	//insert the 'default' value
						}
					}

					//Now, replace the default settings with the custom settings for this user from the database.
					return ModuleSettingsCustom.GetModuleUserSettings(ModuleConfiguration.ModuleID, int.Parse(PortalSettings.CurrentUser.Identity.ID), tempSettings);
				}
			}
		}


		#region "Customize" Button Implementation
		private ModuleButton customizeButton;
		/// <summary>
		/// Module Properties button
		/// </summary>
		public ModuleButton CustomizeButton
		{
			get
			{
				if ( customizeButton == null && HttpContext.Current != null )
				{
					// check authority
					if ( this.HasCustomizeableSettings && PortalSettings.CurrentUser.Identity.IsAuthenticated)
					{
						// create the button
						customizeButton = new ModuleButton();
						customizeButton.Group = ModuleButton.ButtonGroup.Admin;
						customizeButton.EnglishName = "Customize";
						customizeButton.TranslationKey = "CUSTOMIZE";
						customizeButton.Image = this.CurrentTheme.GetImage("Buttons_Properties","Properties.gif");
						if(this.PropertiesUrl.IndexOf("?") >= 0) //Do not change if  the querystring is present (shortcut patch)
							//if ( this.ModuleID != this.OriginalModuleID ) // shortcut
							customizeButton.HRef = this.PropertiesUrl;
						else
							customizeButton.HRef = HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/CustomPropertyPage.aspx", PageID, "mID=" + this.ModuleID.ToString());
						customizeButton.Target = this.PropertiesTarget;
						customizeButton.RenderAs = this.ButtonsRenderAs;
					}
				}
				return customizeButton;
			}
		}

		protected override void BuildButtonLists()
		{
			if ( this.CustomizeButton != null ) 
				this.ButtonListAdmin.Add(this.CustomizeButton);

			base.BuildButtonLists ();
		}

		#endregion

	}

}
