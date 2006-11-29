using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Esperantus;
using Esperantus.WebControls;
using Rainbow.Configuration;
using Rainbow.Design;
using Rainbow.Security;
using Rainbow.Settings;
using Path = Rainbow.Settings.Path;

namespace Rainbow.UI
{
	// TODO: this class needs a better write-up ;-)
	/// <summary>
	/// A template page useful for constructing custom edit pages for module settings.<br/>
	/// Encapsulates some common code including: moduleid,
	/// portalSettings and settings, referrer redirection, edit permission,
	/// cancel event, etc.
	/// This page is a base page.
	/// It is named USECURE becuse no check about security is made.
	/// Usencure page reminds you that you have to do your own security on it.
	/// </summary>
	[History("ozan@rainbow.web.tr", "2005/06/01", "Added new overload for RegisterCSSFile")]
	[History("jminond", "2005/03/10", "Tab to page conversion")]
	[History("Jes1111", "2004/08/18", "Extensive changes - new way to build head element, support for multiple CSS stylesheets, etc.")]
	[History("jviladiu@portalServices.net","2004/07/22","Added Security Access.")]
    [History("John.Mandia@whitelightsolutions.com","2003/10/11","Added ability for each portal to have it's own custom icon instead of sharing one icon among many.")]
	[History("mario@hartmann.net", "2003/09/08", "Solpart Menu stylesheet support added.")]
	[History("Jes1111", "2003/03/04", "Smoothed out page event inheritance hierarchy - placed security checks and cache flushing")]
	[History("mgregory@gt.com.au", "2006/01/04", "Bug fixes to make code HTML 4.01 compliant")]
	public class Page : System.Web.UI.Page
	{
		#region Standard Page Controls
		/// <summary>
		/// Standard update button
		/// </summary>
		protected System.Web.UI.WebControls.LinkButton updateButton;

		/// <summary>
		/// Standard delete button
		/// </summary>
		protected System.Web.UI.WebControls.LinkButton deleteButton;

		/// <summary>
		/// Standard cancel button
		/// </summary>
		protected System.Web.UI.WebControls.LinkButton cancelButton;

		/// <summary>
		/// Standard html head for pages
		/// </summary>
		protected HtmlGenericControl htmlHead;

		#endregion

		#region Events
		/// <summary>
		/// Cancel Button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void CancelBtn_Click(Object sender, EventArgs e)
		{
			this.OnCancel(e);
		}

		/// <summary>
		/// Update Button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void UpdateBtn_Click(Object sender, EventArgs e)
		{
			this.OnUpdate(e);
		}

		/// <summary>
		/// Delete Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void DeleteBtn_Click(Object sender, EventArgs e)
		{
			this.OnDelete(e);
		}

		/// <summary>
		/// Handles the OnInit event at Page level<br/>
		/// Performs OnInit events that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			LoadSettings();

			if(this.cancelButton != null)
			{
				this.cancelButton.Click += new EventHandler(this.CancelBtn_Click);

				//If is an esperantus control and no custom text is provided
				if (cancelButton is LinkButton)
				{
					LinkButton esperantusCancelButton = ((LinkButton)cancelButton);
					if (esperantusCancelButton.TextKey.Length == 0)
					{
						esperantusCancelButton.TextKey = "CANCEL";
						esperantusCancelButton.Text = "Cancel";
					}
				}
				else
				{
					//Provide some text
					cancelButton.Text = Localize.GetString("CANCEL", "Cancel", cancelButton);
				}
				cancelButton.CausesValidation = false;
				cancelButton.EnableViewState = false;
			}
			if(this.updateButton != null)
			{
				this.updateButton.Click += new EventHandler(this.UpdateBtn_Click);
				updateButton.Text = Localize.GetString("APPLY", "Apply", updateButton);
				updateButton.EnableViewState = false;
			}
			if(this.deleteButton != null)
			{
				this.deleteButton.Click += new EventHandler(this.DeleteBtn_Click);
				deleteButton.Text = Localize.GetString("DELETE", "Delete", deleteButton);
				deleteButton.EnableViewState = false;

				// Assign current permissions to Delete button
				if (PortalSecurity.HasDeletePermissions(ModuleID) == false)
				{
					deleteButton.Visible = false;
				}
				else
				{
					if (!(IsClientScriptBlockRegistered("confirmDelete")))
					{
						string[] s = {"CONFIRM_DELETE"};
						RegisterClientScriptBlock("confirmDelete", PortalSettings.GetStringResource("Rainbow.aspnet_client.Rainbow_scripts.confirmDelete.js", s));
					}
					deleteButton.Attributes.Add("OnClick","return confirmDelete()");
				}
			}
			ModuleGuidInCookie();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles OnLoad event at Page level<br/>
		/// Performs OnLoad actions that are common to all Pages.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			// add CurrentTheme CSS
			this.RegisterCssFile(CurrentTheme.Name,CurrentTheme.CssFile);

			if(this.Request.Cookies["Rainbow_" + portalSettings.PortalAlias] != null)
			{
				if( !Config.ForceExpire )
				{
					//jminond - option to kill cookie after certain time always
					int minuteAdd = Config.CookieExpire;
					PortalSecurity.ExtendCookie(this.portalSettings, minuteAdd);
				}
			}

			// Stores referring URL in viewstate
			if (!Page.IsPostBack)
			{
				if(Request.UrlReferrer != null)
					UrlReferrer = Request.UrlReferrer.ToString();
			}
			base.OnLoad(e);
		}

		/// <summary>
		/// The Add event is defined using the event keyword.
		/// The type of Add is EventHandler.
		/// </summary>
		public event EventHandler Add;

		/// <summary>
		/// Handles OnAdd event at Page level<br/>
		/// Performs OnAdd actions that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnAdd(EventArgs e)
		{
			if (Add != null)
				Add(this, e); //Invokes the delegates

			//Flush cache
			OnFlushCache();

			// Verify that the current user has access to edit this module
			if (PortalSecurity.HasAddPermissions(ModuleID) == false)
				PortalSecurity.AccessDeniedEdit();

			// any other code goes here
		}

		/// <summary>
		/// The Update event is defined using the event keyword.
		/// The type of Update is EventHandler.
		/// </summary>
		public event EventHandler Update;

		/// <summary>
		/// Handles OnUpdate event at Page level<br/>
		/// Performs OnUpdate actions that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnUpdate(EventArgs e)
		{
			if (Update != null)
				Update(this, e); //Invokes the delegates

			//Flush cache
			OnFlushCache();

			// Verify that the current user has access to edit this module
			// June 23, 2003: Mark McFarlane made change to check for both Add AND Edit permissions
			// Since UI.Page.EditPage and UI.Page.AddPage both inherit from this UI.Page class
			if (PortalSecurity.HasEditPermissions(ModuleID) == false && PortalSecurity.HasAddPermissions(ModuleID) == false)
				PortalSecurity.AccessDeniedEdit();

			// any other code goes here
		}

		/// <summary>
		/// The FlushCache event is defined using the event keyword.
		/// The type of FlushCache is EventHandler.
		/// </summary>
		public event EventHandler FlushCache;

		/// <summary>
		/// Handles FlushCache event at Page level<br/>
		/// Performs FlushCache actions that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		protected virtual void OnFlushCache()
		{
			if (FlushCache != null)
				FlushCache(this, new EventArgs()); //Invokes the delegates

			// remove module output from cache, if it's there
			StringBuilder sb = new StringBuilder();
			sb.Append("rb_");
			sb.Append(portalSettings.PortalAlias.ToLower());
			sb.Append("_mid");
			sb.Append(ModuleID.ToString());
			sb.Append("[");
			sb.Append(portalSettings.PortalContentLanguage);
			sb.Append("+");
			sb.Append(portalSettings.PortalUILanguage);
			sb.Append("+");
			sb.Append(portalSettings.PortalDataFormattingCulture);
			sb.Append("]");

			if (Context.Cache[sb.ToString()] != null) 
			{
				Context.Cache.Remove(sb.ToString());
				Debug.WriteLine("************* Remove " + sb.ToString());
			}

			// any other code goes here
		}

		/// <summary>
		/// The Delete event is defined using the event keyword.
		/// The type of Delete is EventHandler.
		/// </summary>
		public event EventHandler Delete;

		/// <summary>
		/// Handles OnDelete event at Page level<br/>
		/// Performs OnDelete actions that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDelete(EventArgs e)
		{
			if (Delete != null)
				Delete(this, e); //Invokes the delegates

			//Flush cache
			OnFlushCache();

			// Verify that the current user has access to delete in this module
			if (PortalSecurity.HasDeletePermissions(ModuleID) == false)
				PortalSecurity.AccessDeniedEdit();

			// any other code goes here
		}

		/// <summary>
		/// The Cancel event is defined using the event keyword.
		/// The type of Cancel is EventHandler.
		/// </summary>
		public event EventHandler Cancel;

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnCancel()
		{
			OnCancel(new EventArgs());
		}

		/// <summary>
		/// Handles OnCancel Event at Page level<br/>
		/// Performs OnCancel actions that are common to all Pages<br/>
		/// Can be overridden
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnCancel(EventArgs e)
		{
			if (Cancel != null)
				Cancel(this, e); //Invokes the delegates

			// any other code goes here

			RedirectBackToReferringPage();
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
			}
		}
		#endregion

		#region Properties (Page)
		private string tabTitle = null;
		/// <summary>
		/// Page Title
		/// </summary>
		public string PageTitle
		{
			get
			{
				if(HttpContext.Current != null)
				{
					// Try saved viewstate value
					if ( tabTitle == null )
					{
						if (portalSettings.ActivePage.CustomSettings["TabTitle"].ToString().Length != 0)
							tabTitle = portalSettings.ActivePage.CustomSettings["TabTitle"].ToString();
						else if (portalSettings.CustomSettings["SITESETTINGS_PAGE_TITLE"].ToString().Length != 0)
							tabTitle = portalSettings.CustomSettings["SITESETTINGS_PAGE_TITLE"].ToString();
						else
							tabTitle = portalSettings.PortalTitle;
					}
				}
				return tabTitle;
			}
			set
			{
				tabTitle = value;
			}
		}

		private string tabMetaKeyWords = null;
		/// <summary>
		/// "keywords" meta element
		/// </summary>
		public string PageMetaKeyWords
		{
			get
			{
				// Try saved viewstate value
				// tabMetaKeyWords = (string) ViewState["PageMetaKeyWords"];
				if ( tabMetaKeyWords == null )
				{
					if ( HttpContext.Current != null && portalSettings.ActivePage.CustomSettings["TabMetaKeyWords"].ToString().Length != 0 )
						tabMetaKeyWords = portalSettings.ActivePage.CustomSettings["TabMetaKeyWords"].ToString();
					else if ( HttpContext.Current != null && portalSettings.CustomSettings["SITESETTINGS_PAGE_META_KEYWORDS"].ToString().Length != 0 )
						tabMetaKeyWords = portalSettings.CustomSettings["SITESETTINGS_PAGE_META_KEYWORDS"].ToString();
					else
						tabMetaKeyWords = string.Empty;

					// ViewState["PageMetaKeyWords"] = tabMetaKeyWords;
				}
				return tabMetaKeyWords;
			}
		}

		private string tabMetaDescription = null;
		/// <summary>
		/// "description" meta element
		/// </summary>
		public string PageMetaDescription
		{
			get
			{
				// Try saved viewstate value
				// tabMetaDescription = (string) ViewState["PageMetaDescription"];
				if ( tabMetaDescription == null )
				{
					if ( HttpContext.Current != null && portalSettings.ActivePage.CustomSettings["TabMetaDescription"].ToString().Length != 0 )
						tabMetaDescription = portalSettings.ActivePage.CustomSettings["TabMetaDescription"].ToString();
					else if ( HttpContext.Current != null && portalSettings.CustomSettings["SITESETTINGS_PAGE_META_DESCRIPTION"].ToString().Length != 0 )
						tabMetaDescription = portalSettings.CustomSettings["SITESETTINGS_PAGE_META_DESCRIPTION"].ToString();
					else
						tabMetaDescription = string.Empty;

					// ViewState["PageMetaDescription"] = tabMetaDescription;
				}
				return tabMetaDescription;
			}
		}

		private string tabMetaEncoding = null;
		/// <summary>
		/// "encoding" meta element
		/// </summary>
		public string PageMetaEncoding
		{
			get
			{
				// Try saved viewstate value
				// tabMetaEncoding = (string) ViewState["PageMetaEncoding"];
				if ( tabMetaEncoding == null )
				{
					if ( HttpContext.Current != null && portalSettings.ActivePage.CustomSettings["TabMetaEncoding"].ToString().Length != 0 )
						tabMetaEncoding = portalSettings.ActivePage.CustomSettings["TabMetaEncoding"].ToString();
					else if ( HttpContext.Current != null && portalSettings.CustomSettings["SITESETTINGS_PAGE_META_ENCODING"].ToString().Length != 0 )
						tabMetaEncoding = portalSettings.CustomSettings["SITESETTINGS_PAGE_META_ENCODING"].ToString();
					else
						tabMetaEncoding = string.Empty;

					// ViewState["PageMetaEncoding"] = tabMetaEncoding;
				}
				return tabMetaEncoding;
			}
		}
		
		private string tabMetaOther = null;
		/// <summary>
		/// 
		/// </summary>
		public string PageMetaOther
		{
			get
			{
				// Try saved viewstate value
				// tabMetaOther = (string) ViewState["PageMetaOther"];
				if ( tabMetaOther == null )
				{
					if ( HttpContext.Current != null && portalSettings.ActivePage.CustomSettings["TabMetaOther"].ToString().Length != 0 )
						tabMetaOther = portalSettings.ActivePage.CustomSettings["TabMetaOther"].ToString();
					else if ( HttpContext.Current != null && portalSettings.CustomSettings["SITESETTINGS_PAGE_META_OTHERS"].ToString().Length != 0 )
						tabMetaOther = portalSettings.CustomSettings["SITESETTINGS_PAGE_META_OTHERS"].ToString();
					else
						tabMetaOther = string.Empty;

					// ViewState["PageMetaOther"] = tabMetaOther;
				}
				return tabMetaOther;
			}
		}


		
		// Jes1111
		/// <summary>
		/// List of CSS files to be applied to this page
		/// </summary>
		private Hashtable cssFileList = new Hashtable(3);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsCssFileRegistered(string key)
		{
			if ( this.cssFileList.ContainsKey(key.ToLower()) )
				return true;
			else
				return false;
		}
		/// <summary>
		/// Registers CSS file given path.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="file"></param>
		public void RegisterCssFile(string key, string file)
		{
			this.cssFileList.Add(key.ToLower(), file);
		}

		/// <summary>
		/// Registers CSS file in which current theme folder or Default theme folder
		/// </summary>
		/// <param name="key">CSS file name</param>
		public void RegisterCssFile(string key)
		{
			string path=CurrentTheme.WebPath+"/"+key+".css";
			string filePath=CurrentTheme.Path+"/"+key+".css";
			if(!File.Exists(filePath))
			{
				//jes 11111 - path=ThemeManager.WebPath+"/Default/"+key+".css";
				//filePath=ThemeManager.Path+"/Default/"+key+".css";
				path=ThemeManager.WebPath+"/" + "Default" + "/" + key + ".css";
				filePath=ThemeManager.Path+"/Default/"+key+".css";
				if(!File.Exists(filePath))
				{
					return;
				}
			}
			this.cssFileList.Add(key.ToLower(),path);
		}
		
		/// <summary>
		/// Clears registered css files
		/// </summary>
		public void ClearCssFileList()
		{
			cssFileList.Clear();
		}

		/// <summary>
		/// Register the correct css module file searching in this order in current theme/mod, 
		/// default theme/mod and in module folder.
		/// </summary>
		/// <param name="folderModuleName">The name of module directory</param>
		/// <param name="file">The Css file</param>

		public void RegisterCssModule (string folderModuleName, string file) 
		{
			if(! this.IsCssFileRegistered(file)) 
			{
				string cssFile = this.currentTheme.Module_CssFile(file);
				if (cssFile.Equals(string.Empty)) 
				{
					cssFile = Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, "DesktopModules", folderModuleName, file);
					if (!File.Exists(HttpContext.Current.Server.MapPath(cssFile))) cssFile = string.Empty;
				}
				if (!cssFile.Equals(string.Empty))
					this.RegisterCssFile(file, cssFile);
			}
		}

		// Jes1111
		/// <summary>
		/// List of CSS blocks to be applied to this page.
		/// Strings added to this list will injected into a &lt;style&gt;
		/// block in the page head.
		/// </summary>
		private Hashtable cssImportList = new Hashtable(3);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsCssImportRegistered(string key)
		{
			if ( this.cssImportList.ContainsKey(key.ToLower()) )
				return true;
			else
				return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="import"></param>
		public void RegisterCssImport(string key, string import)
		{
			this.cssImportList.Add(key.ToLower(), import);
		}

		private string docType;
		/// <summary>
		/// page DOCTYPE 
		/// </summary>
		public string DocType
		{
			get
			{
				if ( docType == null )
				{
					if ( portalSettings.CustomSettings.ContainsKey("SITESETTINGS_DOCTYPE") && portalSettings.CustomSettings["SITESETTINGS_DOCTYPE"].ToString().Trim().Length > 0 )
						docType = portalSettings.CustomSettings["SITESETTINGS_DOCTYPE"].ToString();
					else
						docType = string.Empty;
				}
				return docType;
			}
		}

		/// <summary>
		/// Holds a list of javascript function calls which will be output to the body tag as a semicolon-delimited list in the 'onload' attribute
		/// </summary>
		private Hashtable bodyOnLoadList = new Hashtable(3);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsBodyOnLoadRegistered(string key)
		{
			if ( this.bodyOnLoadList.ContainsKey(key.ToLower()) )
				return true;
			else
				return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="functionCall"></param>
		public void RegisterBodyOnLoad(string key, string functionCall)
		{
			this.bodyOnLoadList.Add(key.ToLower(), functionCall);
		}

		private Hashtable clientScripts = new Hashtable(3);
// Jes1111 - 27/Nov/2004 - listeners no longer used
//		private Hashtable clientPopUpEventListeners = new Hashtable(3);

// Jes1111 - 27/Nov/2004 - listeners no longer used
//		public bool IsClientPopUpEventListenerRegistered(string key)
//		{
//			if ( clientPopUpEventListeners.ContainsKey(key.ToLower()) )
//				return true;
//			else
//				return false;
//		}

// Jes1111 - 27/Nov/2004 - listeners no longer used
//		public void RegisterClientPopUpEventListener(string key, string script)
//		{
//			clientPopUpEventListeners.Add(key.ToLower(), script);
//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsClientScriptRegistered(string key)
		{
			if ( clientScripts.ContainsKey(key.ToLower()) )
				return true;
			else
				return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="filePath"></param>
		public void RegisterClientScript(string key, string filePath)
		{
			clientScripts.Add(key.ToLower(), filePath);
		}

		// Jes1111
		/// <summary>
		/// Stores any additional Meta entries requested by modules or other code.
		/// </summary>
		private Hashtable additionalMetaElements = new Hashtable(3);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsAdditionalMetaElementRegistered(string key)
		{
			if ( this.additionalMetaElements.ContainsKey(key.ToLower()) )
				return true;
			else
				return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="element"></param>
		public void RegisterAdditionalMetaElement(string key, string element)
		{
			this.additionalMetaElements.Add(key.ToLower(), element);
		}

		private string bodyOtherAttributes = string.Empty;
		/// <summary>
		/// Allows &lt;body&gt; attributes (other than onload) to be injected into the &lt;body&gt; tag. 
		/// Has an automatic append function (i.e. just assign a string value and it will be appended 
		/// to any current contents). Module developers are therefore responsible for checking for duplicated
		/// attributes and for the syntax of any strings added. The first programmatic call to the property will
		/// automatically insert any value set in SiteSettings.
		/// </summary>
		public string BodyOtherAttributes
		{
			get
			{
				if(HttpContext.Current != null)
				{
					if ( bodyOtherAttributes == null )
					{
						if ( portalSettings.CustomSettings.ContainsKey("SITESETTINGS_BODYATTS") && portalSettings.CustomSettings["SITESETTINGS_BODYATTS"].ToString().Trim().Length > 0 )
							bodyOtherAttributes = portalSettings.CustomSettings["SITESETTINGS_BODYATTS"].ToString();
						else
							bodyOtherAttributes = string.Empty;
					}
				}
				return bodyOtherAttributes;
			}
			set
			{
				if ( bodyOtherAttributes == null )
					bodyOtherAttributes = value;
				else
					bodyOtherAttributes += value;
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

		#region Properties (Pages)
		private int _tabID = 0;
		private Hashtable _Page;

		[Obsolete("Please use PageID")]
		public int TabID
		{
			get
			{
				return PageID;
			}	
		}
		
		[Obsolete("Please use pageSettings")]
		public Hashtable tabSettings
		{
			get
			{
				return pageSettings;
			}	
		}

		/// <summary>
		/// Stores current linked module ID if applicable
		/// </summary>
		public int PageID
		{
			get
			{
				if (_tabID == 0)
				{
					// Determine PageID if specified
					if (HttpContext.Current != null && Request.Params["PageID"] != null)
						_tabID = Int32.Parse(Request.Params["PageID"]);
					else if (HttpContext.Current != null && Request.Params["TabID"] != null)
						_tabID = Int32.Parse(Request.Params["TabID"]);
				}
				return _tabID;
			}
		}

		/// <summary>
		/// Stores current tab settings
		/// </summary>
		public Hashtable pageSettings
		{
			get
			{
				if(_Page == null)
				{
					if (PageID > 0)
					{
                        // thierry@tiptopweb.com.au : custom page layout, cannot be static
						//_Page = Page.GetPageCustomSettings(PageID);
						_Page = portalSettings.ActivePage.GetPageCustomSettings(PageID);
					}
					else
					{
						// Or provides an empty hashtable
						_Page = new Hashtable();
					}
				}
				return _Page;
			}
		}

		private Theme currentTheme;
		/// <summary>
		/// Current page theme
		/// </summary>
		public Theme CurrentTheme
		{
			get
			{
				if ( currentTheme == null )
					currentTheme = this.portalSettings.GetCurrentTheme();
				return currentTheme;
			}
		}
		#endregion

		#region Properties (Modules)
		private int _moduleID = 0;
		/// <summary>
		/// Stores current linked module ID if applicable
		/// </summary>
		public int ModuleID
		{
			get
			{
				if (_moduleID == 0)
				{
					// Determine ModuleID if specified
					if (HttpContext.Current != null && Request.Params["Mid"] != null)
						_moduleID = Int32.Parse(Request.Params["Mid"]);
				}
				return _moduleID;
			}
		}

		private ModuleSettings _module;
		/// <summary>
		/// Stores current module if applicable
		/// </summary>
		public ModuleSettings Module
		{
			get
			{
				if(_module == null)
				{
					if (ModuleID > 0)
					{
						// Obtain selected module data
						foreach (ModuleSettings _mod in portalSettings.ActivePage.Modules)
						{
							if (_mod.ModuleID == ModuleID)
							{
								_module = _mod;
								return _module;
							}
						}
					}
					else
					{
						// Return null
						return null;
					}
				}
				return _module;
			}
		}

		private Hashtable _moduleSettings;
		/// <summary>
		/// Stores current module settings
		/// </summary>
		public Hashtable moduleSettings
		{
			get
			{
				if(_moduleSettings == null)
				{
					if (ModuleID > 0)
						// Get settings from the database
						_moduleSettings = ModuleSettings.GetModuleSettings(ModuleID, this);
					else
						// Or provides an empty hashtable
						_moduleSettings = new Hashtable();
				}
				return _moduleSettings;
			}
		}
		#endregion

		#region Properties (Items)
		private int _itemID = 0;

		/// <summary>
		/// Stores current item id
		/// </summary>
		public int ItemID
		{
			get
			{
				if (_itemID == 0)
				{
					// Determine ItemID if specified
					if (HttpContext.Current != null && Request.Params["ItemID"] != null)
						_itemID = Int32.Parse(Request.Params["ItemID"]);
				}
				return _itemID;
			}
			set
			{
				_itemID = value;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Load settings
		/// </summary>
		protected virtual void LoadSettings()
		{
		}

		/// <summary>
		/// Redirect back to the referring page
		/// </summary>
		public void RedirectBackToReferringPage()
		{
			// Response.Redirect throws a ThreadAbortException to make it work,
			// which is handled by the ASP.NET runtime.
			// By catching an Exception (not a specialized exception, just the
			// base exception class), you end up catching the ThreadAbortException which is
			// always thrown by the Response.Redirect method. Normally, the ASP.NET runtime
			// catches this exception and handles it itself, hence your page never really
			// realized an exception occurred. So by catching this exception, you stop the
			// normal order of events that happen when redirecting.
			try {Response.Redirect(UrlReferrer);}
			catch(ThreadAbortException) {} //Do nothing it is normal
		}

		/// <summary>
		/// Overrides Render() and writes out &lt;html&gt;, &lt;head&gt; and &lt;body&gt; elements along with page contents.
		/// </summary>
		/// <param name="writer">the HtmlTextWriter connected to the output stream</param>
		protected override void Render(HtmlTextWriter writer)
		{
			// BUILD THE DOCTYPE STATEMENT
			this.BuildDocType(writer);
			
			// OUTPUT PAGE CONTENT
			foreach ( Control c in this.Controls )
			{
				if (c is HtmlGenericControl)
				{
					HtmlGenericControl myControl = (HtmlGenericControl)c;

					if ( myControl.TagName.ToLower() == "head" )
						this.BuildHtmlHead(writer);
					else if ( myControl.TagName.ToLower() == "body" )
						this.BuildHtmlBody(writer, myControl);
					else
						c.RenderControl(writer);
				}
				else
					c.RenderControl(writer);
			}
		}
		
		/// <summary>
		/// Builds the DOCTYPE statement when requested by the Render() override.
		/// </summary>
		/// <param name="writer">the HtmlTextWriter created by Render()</param>
		protected virtual void BuildDocType(HtmlTextWriter writer)
		{
			writer.WriteLine(Server.HtmlDecode(Config.DefaultDOCTYPE)); //mark gregory testing
			/*
			// ADD PAGE DOCTYPE
			if ( this.DocType.Length != 0 )
				writer.WriteLine(DocType);				
			else if ( this.CurrentTheme.Type == "zen" || this.Request.Url.PathAndQuery.IndexOf("Viewer") > 0)
			{
				//writer.WriteLine(Server.HtmlDecode(ConfigurationSettings.AppSettings["DefaultDOCTYPE"].ToString()));
				writer.WriteLine(Server.HtmlDecode(Config.DefaultDOCTYPE));
			}
			*/
		}

		/// <summary>
		/// Builds the HTML &lt;body&gt; element 
		/// </summary>
		/// <param name="writer">HtmlTextWriter</param>
		/// <param name="myControl"></param>
		protected virtual void BuildHtmlBody(HtmlTextWriter writer, HtmlGenericControl myControl)
		{
			// open the body element start tag
			writer.WriteBeginTag("body");

			// output any existing attributes found
			// NOTE: there shouldn't be any!
			IEnumerator keys = myControl.Attributes.Keys.GetEnumerator();
			while (keys.MoveNext()) 
			{
				string key = (string)keys.Current;
				writer.WriteAttribute(key, myControl.Attributes[key]);
 			}

			// output onload attribute
			if ( this.bodyOnLoadList.Count > 0 )
			{
				writer.Write(" onload=\"");
				foreach ( string _functionCall in bodyOnLoadList.Values )
					writer.Write(_functionCall);
				writer.Write("\"");
			}

			// output other attributes set in program
			if ( this.bodyOtherAttributes.Length != 0 )
			{
				writer.Write(" ");
				writer.Write(this.BodyOtherAttributes);
			}

			// close the body element start tag
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			//Jes1111: identify the browser type to CSS by wrapping page in a <div> with id set to browser name
			writer.WriteBeginTag("div");
			string _browserName = this.Request.Browser.Browser;
			string _majorVersion = this.Request.Browser.MajorVersion.ToString();
			string _minorVersion = this.Request.Browser.MinorVersion.ToString();
			writer.WriteAttribute("id",_browserName);
			writer.WriteAttribute("class", string.Concat(_browserName,_majorVersion," ",_browserName,_majorVersion,".",_minorVersion));
			writer.Write(HtmlTextWriter.TagRightChar);

			// output the body content, which will include the <form>
			foreach ( Control c in myControl.Controls )
				c.RenderControl(writer);

			// close the browser-id <div> 
			writer.WriteEndTag("div");

			// output the body element end tag
			writer.WriteEndTag("body");
		}

		/// <summary>
		/// Builds the HTML &lt;head&gt; element 
		/// </summary>
		/// <param name="writer">the HtmlTextWriter created by Render()</param>
		protected virtual void BuildHtmlHead(HtmlTextWriter writer)
		{
			writer.WriteFullBeginTag("head");

			// ADD THE PAGE TITLE
			writer.WriteLine("<title>{0}</title>", this.PageTitle);

			// ADD THE META TAGS
			// rainbow identifier
			writer.WriteLine("<meta name=\"generator\" content=\"Rainbow Portal - see www.rainbowportal.net\" >");
			// keywords
			if ( PageMetaKeyWords.Length != 0 )
				writer.WriteLine("<meta name=\"keywords\" content=\"{0}\" >", this.PageMetaKeyWords);
			// description
			if ( PageMetaDescription.Length != 0 )
				writer.WriteLine("<meta name=\"description\" content=\"{0}\" >", this.PageMetaDescription);
			// encoding
			if ( PageMetaEncoding.Length != 0 )
				writer.WriteLine(this.PageMetaEncoding);
			// meta (other) - from Site Settings
			if ( PageMetaOther.Length != 0 ) 
				writer.WriteLine(this.PageMetaOther);
			// additional metas (added by code)
			foreach ( string _metaElement in this.additionalMetaElements.Values )
				writer.WriteLine(_metaElement);

			// ADD THE CSS <LINK> ELEMENT(S)
			foreach ( string _cssFile in cssFileList.Values )
				writer.WriteLine("<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" >",_cssFile);
					
			// ADD SHORTCUT ICON <LINK>
	// Mike Stone - 29-11-2004
	// Changed Rainbow.Settings.Path.ApplicationFullPath to
	//         Rainbow.Settings.Path.ApplicationRoot
	// This will prevent the http:// being added while on a https:// site.
			writer.WriteLine("<link rel=\"SHORTCUT ICON\" href=\"{0}/portalicon.ico\" >", Settings.Path.WebPathCombine(Path.ApplicationRoot,portalSettings.PortalPath));
	
			// ADD (OPTIONAL) <STYLE> BLOCK
			if ( cssImportList.Count > 0 )
			{
				writer.WriteLine("<style type=\"text/css\">");
				writer.WriteLine("<!--");
				foreach ( string _cssBlock in cssImportList.Values )
				{
					writer.WriteLineNoTabs(_cssBlock);
				}
				writer.WriteLine("-->");
				writer.WriteLine("</style>");
			}
	
			// ADD SUPPORT FOR SOLPART MENU STYLESHEET
			// TODO: should be removed after Mario updates Solpart code to use CssImportList
			// Mark Gregory removed this control as it is not 4.01 dtd strict compliant
			// JH has indicated it is not needed if SeparateCSS is set to true
			// SeparateCSS is the default for Rainbow and should remain this way
			//writer.WriteLine("<style id=\"spMenuStyle\" type=\"text/css\"></style>");

			// ADD CLIENTSCRIPTS 
			foreach ( string _script in this.clientScripts.Values )
				writer.WriteLine("<script type=\"text/javascript\" src=\"{0}\"></script>",_script);

			// CLOSE HEAD ELEMENT
			writer.WriteEndTag("head");
			writer.WriteLine();
		}
		#endregion

		#region Security access
		/// <summary>
		///  This array is override for edit and view pages
		///  with the guids allowed to access.
		///  jviladiu@portalServices.net (2004/07/22)
		/// </summary>
		protected virtual ArrayList AllowedModules
		{
			get
			{
				return null;
			}
		}



		/// <summary>
		///  Every guid module in tab is set in cookie.
		///  This method is override in edit &amp; view controls for read the cookie
		///  and pass or denied access to edit or view module.
		///  jviladiu@portalServices.net (2004/07/22)
		/// </summary>
		protected virtual void ModuleGuidInCookie ()
		{
			HttpCookie cookie;
			DateTime time;
			TimeSpan span;
			string guidsInUse = string.Empty;
			Guid guid;

			ModulesDB mdb = new ModulesDB();

			if (portalSettings.ActivePage.Modules.Count > 0)
			{
				foreach (ModuleSettings ms in portalSettings.ActivePage.Modules)
				{
					guid = mdb.GetModuleGuid(ms.ModuleID);
					if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
				}
			}
			cookie = new HttpCookie ("RainbowSecurity", guidsInUse);
			time = DateTime.Now;
			span = new TimeSpan (0, 2, 0, 0, 0); // 120 minutes to expire
			cookie.Expires = time.Add (span);
			base.Response.AppendCookie (cookie);
		}
		#endregion
	}
}