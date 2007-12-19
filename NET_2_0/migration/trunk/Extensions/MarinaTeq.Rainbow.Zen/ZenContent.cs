using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Rainbow.Configuration;
using Rainbow.Security;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// ZenContent class, supports ZenLayout
	/// </summary>
	public class ZenContent : WebControl, INamingContainer
	{
		private ArrayList innerDataSource;
		private bool _autoBind = true; 
		private string _content;

		/// <summary>
		/// Constructor
		/// </summary>
		public ZenContent()
		{
			this.Load += new System.EventHandler(this.LoadControl);
		} 

		/// <summary>
		/// Loads control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void LoadControl(object sender, System.EventArgs e) 
		{ 
			if(AutoBind) 
			{
				DataBind(); 
			}
		} 

		/// <summary> 
		/// Indicates if control should bind when it loads 
		/// </summary> 
		public bool AutoBind 
		{ 
			get{return _autoBind;} 
			set{_autoBind = value;} 
		} 

		/// <summary>
		/// The DataSource
		/// </summary>
		public ArrayList DataSource 
		{
			get 
			{
				if (innerDataSource == null) 
				{
					InitializeDataSource();
				}
				return innerDataSource;
			}
		}
        
		/// <summary>
		/// The layout position for this content
		/// </summary>
		public string Content
		{
			get{return _content;}
			set{_content = value;}
		}

		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls
		/// </summary>
		public override void DataBind()
		{
			foreach ( Control c in DataSource )
				this.Controls.Add(c);
		}

		/// <summary>
		/// Initialize internal data source
		/// </summary>
		public void InitializeDataSource()
		{
			innerDataSource = new ArrayList();

			// Obtain PortalSettings from Current Context
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

			// Loop through each entry in the configuration system for this tab
			// Ensure that the visiting user has access to view the module
			foreach (ModuleSettings _moduleSettings in portalSettings.ActivePage.Modules)
			{
				if ( _moduleSettings.PaneName.ToLower() == this.Content.ToLower() 
					&& PortalSecurity.IsInRoles(_moduleSettings.AuthorizedViewRoles) )
				{
					//Cache. If == 0 then override with default cache in web.config
					if(ConfigurationSettings.AppSettings["ModuleOverrideCache"] != null 
						&& !_moduleSettings.Admin 
						&& _moduleSettings.CacheTime == 0)
					{
						int mCache = Int32.Parse(ConfigurationSettings.AppSettings["ModuleOverrideCache"]);
						if (mCache > 0)
							_moduleSettings.CacheTime = mCache;
					}

					// added security settings to condition test so that a user who has 
					// edit or properties permission will not cause the module output to be cached. 
					if ( 
						((_moduleSettings.CacheTime) <= 0) 
						|| (PortalSecurity.HasEditPermissions(_moduleSettings.ModuleID)) 
						|| (PortalSecurity.HasPropertiesPermissions(_moduleSettings.ModuleID)) 
						|| (PortalSecurity.HasAddPermissions(_moduleSettings.ModuleID)) 
						|| (PortalSecurity.HasDeletePermissions(_moduleSettings.ModuleID)) 
						) 
					{
						try
						{
							string portalModuleName = string.Concat(Rainbow.Settings.Path.ApplicationRoot, "/", _moduleSettings.DesktopSrc);
							PortalModuleControl portalModule = (PortalModuleControl) Page.LoadControl(portalModuleName);
	                
							portalModule.PortalID = portalSettings.PortalID;                                  
							portalModule.ModuleConfiguration = _moduleSettings;

							//TODO: This is not the best place: should be done early
							if ((portalModule.Cultures != null && portalModule.Cultures.Length == 0) || (portalModule.Cultures + ";").IndexOf(portalSettings.PortalContentLanguage.Name + ";") >= 0 )
								innerDataSource.Add(portalModule);
						}
						catch(Exception ex)
						{
							ErrorHandler.Publish(Rainbow.Configuration.LogLevel.Error,"ZenLayout: Unable to load control '" + _moduleSettings.DesktopSrc + "'!", ex);
							innerDataSource.Add(new LiteralControl("<br><span class=\"NormalRed\">" + "ZenLayout: Unable to load control '" + _moduleSettings.DesktopSrc + "'!"));
						}
					}
					else
					{
						try
						{
							CachedPortalModuleControl portalModule = new CachedPortalModuleControl();
	                
							portalModule.PortalID = portalSettings.PortalID;                                 
							portalModule.ModuleConfiguration = _moduleSettings;

							innerDataSource.Add(portalModule);
						}
						catch(Exception ex)
						{
							ErrorHandler.Publish(Rainbow.Configuration.LogLevel.Error, "ZenLayout: Unable to load cached control '" + _moduleSettings.DesktopSrc + "'!", ex);
							innerDataSource.Add(new LiteralControl("<br><span class=\"NormalRed\">" + "ZenLayout: Unable to load cached control '" + _moduleSettings.DesktopSrc + "'!"));
						}                    
					}
				}
			}
		}

		/// <summary>
		/// This member overrides Control.OnDataBinding
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDataBinding(EventArgs e) 
		{
			EnsureChildControls();
			base.OnDataBinding(e);
		}

		/// <summary>
		/// This member overrides Control.Render
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer) 
		{
			RenderContents(writer);
		}
	}
}