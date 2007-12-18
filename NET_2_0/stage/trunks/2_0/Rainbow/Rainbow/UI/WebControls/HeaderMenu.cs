using System;
using System.Collections;
using System.ComponentModel;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Esperantus;
using Rainbow.BLL.UserConfig;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.Settings;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// HeaderMenu
	/// </summary>
	[History("jviladiu@portalServices.net","2004/09/29","Added link showHelp for show help window")]
	[History("ozan@rainbow.web.tr","2004/07/02","Added  showTabMan and showTabAdd properties for managing tab and adding tab only one click... ")]
	[History("John.Mandia@whitelightsolutions.com","2003/11/04","Added extra property DataBindOnInit. So you can decide if you wish it to bind automatically or when you call DataBind()")]
	[History("John.Mandia@whitelightsolutions.com","2003/10/25","Added ability to have more control over the menu by adding more settings.")]
	public class HeaderMenu : DataList
	{
		private object innerDataSource = null;

		private bool _showLogon = false;
		private bool _showSecureLogon = false;   // Thierry (Tiptopweb), 5 May 2003: add link to Secure directory
		private bool _showHome = true;
		private bool _showTabMan = true;	// Ozan, 2 June 2004: add link for tab management 
	
		// 26 October 2003 john.mandia@whitelightsolutions.com - Start
		private bool _showEditProfile = true; 
		private bool _showWelcome = true; 
		private bool _showLogOff = true; 
		private bool _showSaveDesktop = true; 
		private bool _dataBindOnInit = true;
		// 26 October 2003 John Mandia - Finish

		private bool _showHelp = false;	// José Viladiu, 29 Sep 2004: Add link for show help window

		/// <summary>
		/// If true shows a link to a Help Window
		/// </summary>
		[Category("Data"),
			PersistenceMode(PersistenceMode.Attribute),
			DefaultValue(false)
		]
		public bool ShowHelp
		{
			get
			{
				return _showHelp;
			}
			set
			{
				_showHelp = value;
			}
		}

		/// <summary>
		/// If true and user is not authenticated shows 
		/// a logon link in place of logoff
		/// </summary>
		[Category("Data"),
			PersistenceMode(PersistenceMode.Attribute),
			DefaultValue(false)
		]
		public bool ShowLogon
		{
			get
			{
				return _showLogon;
			}
			set
			{
				_showLogon = value;
			}
		}

		/// <summary>
		/// If true and user is not authenticated shows 
		/// a SECURE logon link in place of logoff
		/// </summary>
		[Category("Data"),
			PersistenceMode(PersistenceMode.Attribute),
			DefaultValue(false)
		]
		public bool ShowSecureLogon
		{
			get
			{
				return _showSecureLogon;
			}
			set
			{
				_showSecureLogon = value;
			}
		}

		/// <summary>
		/// Whether show home link
		/// </summary>
		[DefaultValue(true)]
		public bool ShowHome
		{
			get
			{
				return _showHome;
			}
			set
			{
				_showHome = value;
			}
		}

		// 2 June 2004 Ozan
		/// <summary>
		/// Whether show Manage Tab link
		/// </summary>
		[Category("Data"),
			PersistenceMode(PersistenceMode.Attribute),
			DefaultValue(false)
		]
		public bool ShowTabMan
		{
			get
			{
				return _showTabMan;
			}
			set
			{
				_showTabMan = value;
			}
		}
		
		// 26 October 2003 john.mandia@whitelightsolutions.com - Start
		/// <summary>
		/// Whether Edit Profile link
		/// </summary>
		[DefaultValue(true)]
		public bool ShowEditProfile
		{
			get
			{
				return _showEditProfile;
			}
			set
			{
				_showEditProfile = value;
			}
		}

		/// <summary>
		/// Whether Welcome Shows
		/// </summary>
		[DefaultValue(true)]
		public bool ShowWelcome
		{
			get
			{
				return _showWelcome;
			}
			set
			{
				_showWelcome = value;
			}
		}

		/// <summary>
		/// Whether Logoff Link Shows
		/// </summary>
		[DefaultValue(true)]
		public bool ShowLogOff
		{
			get
			{
				return _showLogOff;
			}
			set
			{
				_showLogOff = value;
			}
		}

		/// <summary>
		/// Whether Logoff ShowSaveDesktop Shows
		/// </summary>
		[DefaultValue(true)]
		public bool ShowSaveDesktop
		{
			get
			{
				return _showSaveDesktop;
			}
			set
			{
				_showSaveDesktop = value;
			}
		}

		/// <summary>
		/// Whether Logoff Link Shows
		/// </summary>
		[DefaultValue(true)]
		public bool DataBindOnInit
		{
			get
			{
				return _dataBindOnInit;
			}
			set
			{
				_dataBindOnInit = value;
			}
		}
		// 26 October 2003 John Mandia - Finish
		
		/// <summary>
		/// HeaderMenu
		/// </summary>
		public HeaderMenu()
		{
			this.EnableViewState = false;
			this.RepeatDirection = RepeatDirection.Horizontal;
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			if(DataBindOnInit)
			{
				DataBind();
			}
		}

		// Jes1111
		/// <summary>
		/// Builds a help link for the header menu and registers it with the page
		/// </summary>
		/// <returns></returns>
		private string GetHelpLink()
		{
//			string helpTarget = "RainbowHelp";
//			string helpText = Esperantus.Localize.GetString("HEADER_HELP", "Help");
//
//			StringBuilder sb = new StringBuilder();
//			sb.Append("<a href=\"");
//			sb.Append(Rainbow.Settings.Path.ApplicationRoot);
//			sb.Append("/rb_documentation/Viewer.aspx\"	target=\"");
//			sb.Append(helpTarget);
//			sb.Append("\" class=\"");
//			sb.Append(helpTarget);
//			sb.Append("\">");
//			sb.Append(helpText);
//			sb.Append("</a>");
//
//			//popup removed until fixed
//			//			string helpPopUpOptions = "toolbar=1,location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=600,height=400,screenX=15,screenY=15,top=15,left=15";
//			//			if(this.Page is Rainbow.UI.Page)
//			//			{
//			//				Rainbow.UI.Page myPage = (Rainbow.UI.Page) this.Page;
//			//				if ( !myPage.IsClientScriptRegistered("rb-popup") )
//			//					myPage.RegisterClientScript("rb-popup",Rainbow.Settings.Path.ApplicationRoot + "/aspnet_client/popupHelper/popup.js");
//			//
//			//				if ( !myPage.IsClientPopUpEventListenerRegistered(helpTarget) )
//			//				{
//			//					StringBuilder sbj = new StringBuilder();
//			//					sbj.Append("mlisten('click', getElementsByClass('");
//			//					sbj.Append(helpTarget);
//			//					sbj.Append("','a')");
//			//					if ( helpPopUpOptions.Length != 0 )
//			//					{
//			//						sbj.Append(", event_popup_features('");
//			//						sbj.Append(helpPopUpOptions);
//			//						sbj.Append("')");
//			//					}
//			//					sbj.Append(");");
//			//					myPage.RegisterClientPopUpEventListener(helpTarget, sbj.ToString());
//			//				}
//			//			}
//			//			else
//			//			{
//			//				//Avoid errors when page is not a rainbow page
//			//				if (this.Page.IsClientScriptBlockRegistered("rb-popup"))
//			//					this.Page.RegisterClientScriptBlock("rb-popup", Rainbow.Settings.Path.ApplicationRoot + "/aspnet_client/popupHelper/popup.js");
//			//			}
//			//end popup removed until fixed
//
//			return sb.ToString();

			// Jes1111 - 27/Nov/2004 - simplified help popup scheme (echoes changes in ModuleButton.cs)
			string helpTarget = "RainbowHelp";
			string popupOptions = "toolbar=1,location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=600,height=400,screenX=15,screenY=15,top=15,left=15";
            string helpText = Esperantus.Localize.GetString("HEADER_HELP", "Help");

			StringBuilder sb = new StringBuilder();
			sb.Append("<a href=\"");
			sb.Append(Path.ApplicationRoot);
			sb.Append("/rb_documentation/Viewer.aspx\"	target=\"");
			sb.Append(helpTarget);
			sb.Append("\" ");
			if ( this.Page is Page )
			{
				sb.Append("onclick=\"link_popup(this,'");
				sb.Append(popupOptions);
				sb.Append("');return false;\"");
			}
			sb.Append(" class=\"");
			sb.Append("link-is-popup");
			if (this.CssClass.Length != 0)
			{
				sb.Append(" ");
				sb.Append(this.CssClass);
			}
			sb.Append("\">");
			sb.Append(helpText);
			sb.Append("</a>");

			if ( this.Page is Page )
			{
				if ( !((Page)this.Page).IsClientScriptRegistered("rb-popup") )
					((Page)this.Page).RegisterClientScript("rb-popup",Path.ApplicationRoot + "/aspnet_client/popupHelper/popup.js");
			}

			return sb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void DataBind()
		{
			if(HttpContext.Current != null)
			{
				//Init data
				ArrayList list = new ArrayList();

				// Obtain PortalSettings from Current Context
				PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

				string homeLink = "<a";
				string menuLink;

				// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
				if (this.CssClass.Length != 0)
					homeLink = homeLink + " class=\"" + this.CssClass + "\"";

				homeLink = homeLink + " href='" + HttpUrlBuilder.BuildUrl() + "'>" +
                    Esperantus.Localize.GetString("HOME", "Home", this) + "</a>";

				// If user logged in, customize welcome message
				if (HttpContext.Current.Request.IsAuthenticated == true) 
				{
					if (ShowWelcome)
					{
                        list.Add(Esperantus.Localize.GetString("HEADER_WELCOME", "Welcome", this) + "&#160;" + PortalSettings.CurrentUser.Identity.Name + "!");
					}

					if (ShowHome)
					{
						list.Add(homeLink);
					}

					if (ShowHelp)
					{
						list.Add(GetHelpLink());
					}
					
					// Added by Mario Endara <mario@softworks.com.uy> (2004/11/06)
					// Find Tab module to see if the user has add/edit rights
					ModulesDB modules = new ModulesDB();
					Guid TabGuid = new Guid("{1C575D94-70FC-4A83-80C3-2087F726CBB3}");
					// Added by Xu Yiming <ymhsu@ms2.hinet.net> (2004/12/6)
					// Modify for support Multi or zero Pages Modules in a single portal.
					bool HasEditPermissionsOnTabs=false;
					int TabModuleID=0;
					
//					SqlDataReader result = modules.FindModulesByGuid(portalSettings.PortalID, TabGuid);
//					while(result.Read()) 
//					{
//						TabModuleID=(int)result["ModuleId"];

					foreach (ModuleItem m in modules.FindModuleItemsByGuid(portalSettings.PortalID, TabGuid))
					{
						HasEditPermissionsOnTabs = PortalSecurity.HasEditPermissions(m.ID);
						if(HasEditPermissionsOnTabs) 
						{	
							TabModuleID=m.ID;
							break;
						}
					}

					// If user logged in and has Edit permission in the Tab module, reach tab management just one click
					if ((ShowTabMan) && (HasEditPermissionsOnTabs))
					{
						// added by Mario Endara 2004/08/06 so PageLayout can return to this page
						// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
						menuLink = "<a";
						if (this.CssClass.Length != 0)
							menuLink = menuLink + " class=\"" + this.CssClass + "\"";

						// added mID by Mario Endara <mario@softworks.com.uy> to support security check (2004/11/09)
						menuLink = menuLink + " href='" + HttpUrlBuilder.BuildUrl("~/DesktopModules/Pages/PageLayout.aspx?PageID=") + 
									portalSettings.ActivePage.PageID + "&mID=" + TabModuleID.ToString() + 
									"&Alias=" + portalSettings.PortalAlias + "&lang=" + portalSettings.PortalUILanguage + 
									"&returntabid=" + portalSettings.ActivePage.PageID + "'>"  +
                            Esperantus.Localize.GetString("HEADER_MANAGE_TAB", "Edit This Page", null) + "</a>";
						list.Add(menuLink); 
					}

					if (ShowEditProfile)
					{	
						// 19/08/2004 Jonathan Fong
						// www.gt.com.au
						if (Context.User.Identity.AuthenticationType == "LDAP")
						{
							// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
							menuLink = "<a";
							if (this.CssClass.Length != 0)
								menuLink = menuLink + " class=\"" + this.CssClass + "\"";

							menuLink = menuLink + " href='" + HttpUrlBuilder.BuildUrl("~/DesktopModules/Register/Register.aspx", "userName=" + 
										PortalSettings.CurrentUser.Identity.Email) + "'>" + "Profile" + "</a>";
							list.Add(menuLink); 
						}
						// If user is form add edit user link
						else if (!(HttpContext.Current.User is WindowsPrincipal))
						{	
							// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
							menuLink = "<a";
							if (this.CssClass.Length != 0)
								menuLink = menuLink + " class=\"" + this.CssClass + "\"";

							menuLink = menuLink + " href='" + HttpUrlBuilder.BuildUrl("~/DesktopModules/Register/Register.aspx", "userName=" + 
										PortalSettings.CurrentUser.Identity.Email) + "'>" +
                                Esperantus.Localize.GetString("HEADER_EDIT_PROFILE", "Edit profile", this) + "</a>";
							list.Add(menuLink); 
						}
					}

					// if authentication mode is Cookie, provide a logoff link
					if (Context.User.Identity.AuthenticationType == "Forms" || Context.User.Identity.AuthenticationType == "LDAP") 
					{
						if (ShowLogOff)
						{
							// Corrections when ShowSecureLogon is true. jviladiu@portalServices.net (05/07/2004)
							string href = HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/Logoff.aspx");
							if (ShowSecureLogon && Context.Request.IsSecureConnection) 
							{
								string auxref = Context.Request.Url.AbsoluteUri;
								auxref = auxref.Substring(0, auxref.IndexOf(Context.Request.Url.PathAndQuery));
								href = auxref + href;
								href = href.Replace("https", "http");
							}
							// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
							menuLink = "<a";
							if (this.CssClass.Length != 0)
								menuLink = menuLink + " class=\"" + this.CssClass + "\"";

                            menuLink = menuLink + " href='" + href + "'>" + Esperantus.Localize.GetString("HEADER_LOGOFF", "Logoff", null) + "</a>";
							list.Add(menuLink); 
						}
					}
					// [START] bja@reedtek.com - Save Desktop
					//-------------------------------------------------
					// This may be a little strange but the menu header
					// does not except buttons (link/push/image/...) but
					// references (strings). So in order for me to get a
					// callback, not calling a .aspx page, I simply get 
					// a link button that is in the skin and insert a
					// callback (provided by .net) w/ the proper name
					// in the reference as done above. This way I get
					// the same affect but w/ a callback event
					//-------------------------------------------------
					// get the link button -- only need the link button even though
					// it is not visible. It's needed to provide a callback from
					// for this reference. So it is not in this datalist but just outside of it
					LinkButton lb = (LinkButton) this.Parent.FindControl("saveConfig");
					// if there a button in the layouts
// Jes1111
//					if ( lb != null && 
//						HttpContext.Current.Request.IsAuthenticated && 
//						GlobalResources.SupportWindowMgmt ) 
					if ( lb != null && HttpContext.Current.Request.IsAuthenticated && Config.WindowMgmtControls ) 
						{

						// If Statement and public/private property added by john.mandia@whitelightsolutions.com 26th October
						if(ShowSaveDesktop)
						{
							// set the callback to handle the save
							lb.Click += new EventHandler(this.HandleSave);
							// convert the name for the postback to understand -- the
							// control it will be calling
							string lb_id = lb.UniqueID.Replace(":","$");
							// insert the  callback/postback
							// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
							menuLink = "<a";
							if (this.CssClass.Length != 0)
								menuLink = menuLink + " class=\"" + this.CssClass + "\"";

							menuLink = menuLink + " href=\"javascript:__doPostBack('" + lb_id + "','')\">" +
                                Esperantus.Localize.GetString("HEADER_SAVE_DESKTOP", "Save Desktop", null) + "</a>";
							list.Add(menuLink); 
						}
					}
					// [END] bja@reedtek.com
				}
				else 
				{
					if (ShowHome)
					{
						list.Add(homeLink); 
					}

					if (ShowHelp)
					{
						list.Add(GetHelpLink());
					}

					// if not authenticated and ShowLogon is true, provide a logon link 
					if (ShowLogon) 
					{
						// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
						menuLink = "<a";
						if (this.CssClass.Length != 0)
							menuLink = menuLink + " class=\"" + this.CssClass + "\"";

                        menuLink = menuLink + " href='" + HttpUrlBuilder.BuildUrl("~/DesktopModules/Admin/Logon.aspx") + "'>" + Esperantus.Localize.GetString("LOGON", "Logon", null) + "</a>"; 
						list.Add(menuLink); 
					}
					// Thierry (Tiptopweb) 5 May 2003 : Secure Logon to Secure Directory
					if (ShowSecureLogon) 
					{
						// Added localized support. jviladiu@portalServices.net (05/07/2004)
						// added Class support by Mario Endara <mario@softworks.com.uy> 2004/10/04
						menuLink = "<a";
						if (this.CssClass.Length != 0)
							menuLink = menuLink + " class=\"" + this.CssClass + "\"";

						menuLink = menuLink + " href='" + portalSettings.PortalSecurePath + "/Logon.aspx'>" +
                            Esperantus.Localize.GetString("LOGON", "Logon", null) + "</a>";
						list.Add(menuLink); 
					}
				}
				innerDataSource = list;
			}			
			base.DataBind ();
		}


		/// <summary>
		/// DataSource
		/// </summary>
		public override object DataSource
		{
			get
			{
				return innerDataSource;
			}
			set
			{
				innerDataSource = value;                
			}
		}

		// [START] bja@reedtek.com

		/// <summary>
		/// Handles the button event to save desktop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="evt"></param>
		private void HandleSave(object sender, EventArgs evt)
		{
			// go save user desktop
			UserDesktop.SaveUserDesktop();

		} // end of HandleSave

		// [END] bja@reedtek.com

	}
}