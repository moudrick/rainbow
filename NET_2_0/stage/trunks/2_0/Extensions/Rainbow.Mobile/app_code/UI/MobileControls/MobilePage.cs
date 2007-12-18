/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * This code is partially based on the IbuySpy Mobile Portal Code. 
 * Last updated Date: 2004/11/29
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/
using System;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;

using Rainbow.UI;
using Rainbow.UI.MobileControls;
using Rainbow.UI.MobileControls.Globalized;

using Rainbow.Configuration;

namespace Rainbow.UI
{
	/// <summary>
	/// A template page useful for constructing custom edit 
	/// pages for module settings.<br/>
	/// Encapsulates some common code including: moduleid, 
	/// portalSettings and settings, referrer redirection, edit permission,
	/// cancel event, etc.
	/// </summary>
	public class MobilePage : System.Web.UI.MobileControls.MobilePage
	{

		#region Viewstate Overides
		protected override void SavePageStateToPersistenceMedium(object viewState)
		{
			Session["MobileUserViewState"] = viewState;
			//base.SavePageStateToPersistenceMedium (viewState);
		}
		protected override object LoadPageStateFromPersistenceMedium()
		{
			return Session["MobileUserViewState"];
			//return base.LoadPageStateFromPersistenceMedium ();
		}

		#endregion

		#region Standard Page Controls
		/// <summary>
		/// 
		/// </summary>
		//protected System.Web.UI.MobileControls.StyleSheet MobileStyleSheet;

		private StyleSheet _themeStyleSheet;

		/// <summary>
		/// 
		/// </summary>
		public  StyleSheet ThemeStyleSheet
		{
			get
			{
				if (_themeStyleSheet == null)
				{
					_themeStyleSheet = (StyleSheet) Page.FindControl("MobileStyleSheet");
				}
				if (_themeStyleSheet == null)
				{
					System.Diagnostics.Debug.WriteLine ("no mobilestylesheet control found");
					return  new StyleSheet();
				}
				return _themeStyleSheet ;
			}
		}
//
//		public string MobileStyleSheetPath
//		{
//			get
//			{
//				return portalSettings.GetCurrentTheme().WebPath + "/MobileStyle.ascx" ;
//			}
//		}

		public System.Web.UI.MobileControls.Style  MobileStyle (string styleName)
		{
			System.Web.UI.MobileControls.Style tempStyle = ThemeStyleSheet[styleName];
			if (tempStyle == null)
				tempStyle = new System.Web.UI.MobileControls.Style();
			
			return tempStyle;
		}


		#endregion

		#region Events
		
		/// <summary>
		/// Handles the OnInit event at Page level<br/>
		/// Performs OnInit events that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}


		/// <summary>
		/// Handles OnLoad event at Page level<br/>
		/// Performs OnLoad actions that are common to all Pages.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e) 
		{
			// Stores referring URL in viewstate
			if (!Page.IsPostBack)
			{
				if(Request.UrlReferrer != null)
					UrlReferrer = Request.UrlReferrer.ToString();
			}
			
			// Ensure that the visiting user has access to the current page
			if (Rainbow.Security.PortalSecurity.IsInRoles(portalSettings.ActiveTab.AuthorizedRoles) == false)
			{
				Security.MobilePortalSecurity.AccessDenied();
			}
			
			base.OnLoad(e);

		}

		#endregion

		#region Properties (Portal)
		private PortalSettings _portalSettings;
        
		/// <summary>
		/// Stores current portal settings 
		/// </summary>
		public PortalSettings portalSettings
		{
			get
			{
				if(_portalSettings == null)
				{
					// Obtain PortalSettings from Current Context
					if (HttpContext.Current != null)
						_portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				}

				return _portalSettings;
			}
			set
			{
				_portalSettings = value;
				HttpContext.Current.Items["PortalSettings"] = _portalSettings ;
			}
		}
		#endregion
		
		#region Properties (Page)
		/// <summary>
		/// Stores current page title
		/// </summary>
		public string PageTitle
		{
			get
			{
				string pageTitle;
				// Try saved viewstate value
				pageTitle = (string) ViewState["PageTitle"];
				if (pageTitle != null)
					return pageTitle;
				else
				{
					if (HttpContext.Current != null)
					{
						// Try to get this tab title
						pageTitle = portalSettings.ActiveTab.MobileTabName ;
						if (pageTitle != null && pageTitle != string.Empty)
							return pageTitle;
						else
							// Finally return portal title
							return portalSettings.PortalTitle;
					}
					else
						return string.Empty;
				}
			}
			set
			{
				ViewState["PageTitle"] = value;
			}		
		}

		/// <summary>
		/// Referring URL
		/// </summary>
		protected string UrlReferrer
		{
			get
			{
				if(ViewState["UrlReferrer"] != null)
					return (string) ViewState["UrlReferrer"];
				else
					return HttpUrlBuilder.BuildUrl();
			}
			set
			{
				ViewState["UrlReferrer"] = value;
			}
		}
		#endregion

		#region Properties (Tabs)
		private int _tabId = 0;
		private int _tabIndex = 0;
//		private Hashtable _tabSettings;

		/// <summary>
		/// Stores current tabindex
		/// </summary>
		public int TabIndex
		{
			get
			{
				if (_tabIndex == 0)
				{

					// Obtain current tab index and tab id settings
					String tabSetting = (String)HiddenVariables["ti"];

					if (tabSetting == null)
						tabSetting = (String)Request.Params["ti"];

					if (tabSetting != null) 
					{
						int comma = tabSetting.IndexOf(',');
						_tabIndex = Int32.Parse(tabSetting.Substring(0, comma));
					}
				}
				return _tabIndex;
			}		
		}

		/// <summary>
		/// Stores current linked module Id if applicable
		/// </summary>
		public int TabId
		{
			get
			{
				if (_tabId == 0)
				{
					//_tabId = portalSettings.ActiveTab.TabId ;

					// Obtain current tab index and tab id settings
					String tabSetting = (String)HiddenVariables["ti"];

					if (tabSetting == null)
							tabSetting = (String)Request.Params["ti"];

					if (tabSetting != null) 
					{
						int comma = tabSetting.IndexOf(',');
						_tabId = Int32.Parse(tabSetting.Substring(comma+1));
					}
				}
				return _tabId;
			}		
		}


		#endregion

		#region Properties (Modules)
//		private int _moduleId = 0;

//		private Hashtable _moduleSettings;

//		/// <summary>
//		/// Stores current linked module Id if applicable
//		/// </summary>
//		public int ModuleId
//		{
//			get
//			{
//				if (_moduleId == 0)
//				{
//					// Determine ModuleId if specified
//					if (HttpContext.Current != null && Request.Params["Mid"] != null)
//						_moduleId = Int32.Parse(Request.Params["Mid"]);
//				}
//				return _moduleId;
//			}
//		}

//		/// <summary>
//		/// Stores current module settings 
//		/// </summary>
//		public Hashtable moduleSettings
//		{
//			get
//			{
//				if(_moduleSettings == null)
//				{
//					if (ModuleId > 0)
//						// Get settings from the database
//						_moduleSettings = ModuleSettings.GetModuleSettings(ModuleId);
//					else
//						// Or provides an empty hashtable
//						_moduleSettings = new Hashtable();
//				}
//				return _moduleSettings;
//			}
//		}
		#endregion

		#region Properties (Items)
//		private int _itemId = 0;
//
//		/// <summary>
//		/// Stores current item id
//		/// </summary>
//		public int ItemId
//		{
//			get
//			{
//				if (_itemId == 0)
//				{
//					// Determine ItemId if specified
//					if (HttpContext.Current != null && Request.Params["ItemId"] != null)
//						_itemId = Int32.Parse(Request.Params["ItemId"]);
//				}
//				return _itemId;
//			}	
//			set
//			{
//				_itemId = value;
//			}
//		}
		#endregion

		#region Methods

		/// <summary>
		/// LoadPortalSettings is a helper methods that loads portal settings for
		/// the selected tab.  It first verifies that the settings haven't already
		/// been set within the Global.asax file -- if they are different (in the
		/// case that a tab change is made) then the method reloads the appropriate
		/// tab data.
		/// </summary>
		/// <param name="tabIndex"></param>
		/// <param name="tabId"></param>
		protected  void LoadPortalSettings(int tabIndex,int tabId) 
		{
			if (tabId == 0)
			{
				tabId = this.portalSettings.ActiveTab.TabID ;
				_tabId = tabId;
			}

			if (this.portalSettings.ActiveTab.TabID != tabId) 
			{
				this.portalSettings = new PortalSettings(tabId, portalSettings.PortalAlias);
				
				// Store tabindex and tabId in local variable
				_tabIndex = tabIndex;
				_tabId = tabId;
				// Store tabindex in a hidden variable to preserve accross round trips
				HiddenVariables["ti"] = String.Concat(tabIndex.ToString(), ",", tabId.ToString());
			}
		}

	
		/// <summary>
		/// Redirect back to the referring page
		/// </summary>
		public void RedirectBackToReferringPage()
		{
			base.RedirectToMobilePage(UrlReferrer);
		}

		#endregion
	}
}