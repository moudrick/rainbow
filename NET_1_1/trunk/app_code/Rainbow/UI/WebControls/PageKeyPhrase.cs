using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Rainbow.Configuration;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// Created By john.mandia@whitelightsolutions.com - This control is used by the PageKeyPhrase Module.
	/// </summary>
	public class PageKeyPhrase : Label
	{
		// Obtain PortalSettings from Current Context
		PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
		string _tabKeyPhrase;
		string currentLanguage;

		/// <summary>
		/// Stores current Tab Key Phrase
		/// </summary>
		public string TabKeyPhrase
		{
			get
			{
				currentLanguage = "TabKeyPhrase_" + portalSettings.PortalContentLanguage.Name.ToString();
				if (portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture
					&& portalSettings.ActivePage.CustomSettings[currentLanguage] != null)
				{

					_tabKeyPhrase = (string) ViewState["TabKeyPhrase_" + portalSettings.PortalContentLanguage.Name.ToString()];
					if (_tabKeyPhrase != null)
						return _tabKeyPhrase;
					else
					{
						// Try to get this tab's key phrase
						_tabKeyPhrase = portalSettings.ActivePage.CustomSettings[currentLanguage].ToString();

						if (_tabKeyPhrase == string.Empty && portalSettings.CustomSettings != null)
						{
							_tabKeyPhrase = portalSettings.ActivePage.CustomSettings["TabKeyPhrase"].ToString();
                        
							if(_tabKeyPhrase == string.Empty)
								_tabKeyPhrase = portalSettings.CustomSettings["SITESETTINGS_PAGE_KEY_PHRASE"].ToString();						
						}

						return _tabKeyPhrase;
					}
				}
				else
				{
					_tabKeyPhrase = (string) ViewState["TabKeyPhrase"];
					if (_tabKeyPhrase != null)
						return _tabKeyPhrase;
					else 
					{
						if (portalSettings.ActivePage.CustomSettings["TabKeyPhrase"] != null)
						{
							// Try to get this tab's key phrase
							_tabKeyPhrase = portalSettings.ActivePage.CustomSettings["TabKeyPhrase"].ToString();
                                
							if (_tabKeyPhrase == string.Empty && portalSettings.CustomSettings != null)
							{
								_tabKeyPhrase = portalSettings.CustomSettings["SITESETTINGS_PAGE_KEY_PHRASE"].ToString();						
							}
							
							return _tabKeyPhrase;
						}
						return string.Empty;						
					}
				}
			}
			set
			{
				if (portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture)
				{
					ViewState["TabKeyPhrase_" + portalSettings.PortalContentLanguage.Name.ToString()] = value;
				}
				else
				{
					ViewState["TabKeyPhrase"] = value;
				}
			}		
		}
		
		/// <summary>
		/// Load event handler
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			this.Text = TabKeyPhrase;
                
			base.DataBind();
		}
	}
}
