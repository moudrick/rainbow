using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using DUEMETRI.UI.WebControls.HWMenu;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.Settings.Cache;

namespace Rainbow.UI.WebControls 
{ 

	/// <summary>
	/// Menu navigation inherits from Menu Webcontrol
	/// and adds the 'glue' to link to tabs tree.
	/// Bugfix #656794 'Menu rendering adds all tabs' by abain
	/// </summary>
	[History("jperry","2003/05/01","Code changed to more closely resemble DesktopNavigation")]
	[History("jperry","2003/05/02","Support for new binding options.")]
	[History("MH","2003/05/23","Added bind option 'BindOptionDefinedParent' and 'ParentPageID'.")]
	[History("jviladiu@portalServices.net","2004/08/26","Add AutoShopDetect support and set url's for categories of products")]
	public class MenuNavigation : Menu, INavigation 
	{ 
		/// <summary>
		/// 
		/// </summary>
		public MenuNavigation() 
		{ 
			this.EnableViewState = false; 
			this.Load += new EventHandler(this.LoadControl);
		} 

		private void LoadControl(object sender, EventArgs e) 
		{ 
			if(AutoBind) 
				DataBind(); 
		} 

		#region INavigation implementation 
		private BindOption _bind = BindOption.BindOptionTop; 
		//MH: added 29/04/2003 by mario@hartmann.net
		private int _definedParentTab = -1;
		//MH: end

		private bool _useTabNameInUrl = false;

		/// <summary> 
		/// Indicates if control should show the tabname in the url 
		/// </summary> 
		[ Category("Data"), PersistenceMode(PersistenceMode.Attribute) ] 
		public bool UseTabNameInUrl 
		{ 
			get{return _useTabNameInUrl;} 
			set{_useTabNameInUrl = value;} 
		} 

		private bool _autoShopDetect = false;

		/// <summary> 
		/// Indicates if control should detect products module when loads 
		/// </summary> 
		[ Category("Data"), PersistenceMode(PersistenceMode.Attribute) ] 
		public bool AutoShopDetect 
		{ 
			get{return _autoShopDetect;} 
			set{_autoShopDetect = value;} 
		} 

		private bool _autoBind = false; 

		/// <summary> 
		/// Indicates if control should bind when loads 
		/// </summary> 
		[ 
			Category("Data"), 
				PersistenceMode(PersistenceMode.Attribute) 
		] 
		public bool AutoBind 
		{ 
			get{return _autoBind;} 
			set{_autoBind = value;} 
		} 

		/// <summary> 
		/// Describes how this control should bind to db data 
		/// </summary> 
		[ 
			Category("Data"), 
				PersistenceMode(PersistenceMode.Attribute) 
		] 
		public BindOption Bind 
		{ 
			get {return _bind;} 
			set 
			{ 
				if(_bind != value) 
				{ 
					_bind = value; 
				} 
			} 
		} 
		//MH: added 23/05/2003 by mario@hartmann.net
		/// <summary>
		/// defines the parentPageID when using BindOptionDefinedParent
		/// </summary>
		[
			Category("Data"),
				PersistenceMode(PersistenceMode.Attribute)
		]
		public int ParentPageID
		{ 
			get {return _definedParentTab;}
			set
			{
				if(_definedParentTab != value)
				{
					_definedParentTab = value;
				}
			}
		}
		//MH: end
		#endregion 

		/// <summary>
		/// Do databind.
		/// Thanks to abain for cleaning up the code and fixing the bugs
		/// </summary>
		public override void DataBind() 
		{ 
			base.DataBind(); 


			// Obtain PortalSettings from Current Context 
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"]; 

			bool activeIsProducts = (Bind == BindOption.BindOptionCurrentChilds) && products (portalSettings.ActivePage.PageID); 
			// Build list of tabs to be shown to user 
			ArrayList authorizedTabs = new ArrayList(); 

			authorizedTabs = (ArrayList)GetInnerDataSource();

			for (int i = 0; i < authorizedTabs.Count; i++) 
			{ 
				PageStripDetails myTab = (PageStripDetails) authorizedTabs[i];
				if (products (myTab.PageID)) 
				{
					ShopMenu (0, myTab, portalSettings.ActivePage.PageID == myTab.PageID);
				} 
				else if (activeIsProducts && myTab.ParentPageID == portalSettings.ActivePage.PageID) 
				{
					ShopDesktopNavigation (myTab);
				}
				else
				{
					AddMenuTreeNode(0, myTab);
				}
			} 
		} 

		private string giveMeUrl (string tab, int id) 
		{
			if (!UseTabNameInUrl) return HttpUrlBuilder.BuildUrl(id);
			string auxtab = string.Empty;
			foreach (char c in tab)
				if (char.IsLetterOrDigit(c)) auxtab+= c; else auxtab += "_";
			return HttpUrlBuilder.BuildUrl("~/" + auxtab + ".aspx", id);
		}

		private bool products (int tab)
		{
			if (!AutoShopDetect) return false;
			if (!CurrentCache.Exists (Key.TabNavigationSettings (tab, "Shop"))) 
			{
				PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"]; 
				bool exists = new ModulesDB().ExistModuleProductsInPage(tab, portalSettings.PortalID);
				CurrentCache.Insert(Key.TabNavigationSettings(tab, "Shop"), exists);
			}
			return (bool) CurrentCache.Get(Key.TabNavigationSettings(tab, "Shop"));
		}

		private void ShopDesktopNavigation (PageStripDetails myTab)
		{ 
			if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles)) 
			{ 
				MenuTreeNode mn = new MenuTreeNode(myTab.PageName);
				
				mn.Link = HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, myTab.ParentPageID, "ItemID=" + myTab.PageID.ToString());
				mn.Height = Height; 
				mn.Width = Width;
				mn = RecourseMenuShop(0, myTab.Pages, mn, myTab.ParentPageID); 
				this.Childs.Add(mn); 
			} 
		} 
		/// <summary>
		/// 
		/// </summary>
		protected virtual void ShopMenu(int tabIndex, PageStripDetails myTab, bool active)
		{ 
			if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles)) 
			{ 
				MenuTreeNode mn = new MenuTreeNode(myTab.PageName);
				
				mn.Link = giveMeUrl (myTab.PageName, myTab.PageID);
				mn.Height = Height; 
				mn.Width = Width;
				if (active)
					mn = RecourseMenuShop(tabIndex, myTab.Pages, mn, myTab.PageID); 
				this.Childs.Add(mn); 
				
			} 
		} 
		/// <summary>
		/// 
		/// </summary>
		protected virtual MenuTreeNode RecourseMenuShop(int tabIndex, PagesBox t, MenuTreeNode mn, int idShop)
		{ 
			if (t.Count > 0) 
			{ 
				for (int c=0; c < t.Count; c++) 
				{ 
					PageStripDetails mySubTab = t[c];
					
						if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles)) 
						{ 
							MenuTreeNode mnc = new MenuTreeNode(mySubTab.PageName);
							
							mnc.Link = HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, idShop, "ItemID=" + mySubTab.PageID.ToString());
							mnc.Width = mn.Width; 
							mnc = RecourseMenuShop(tabIndex, mySubTab.Pages, mnc, idShop); 
							mn.Childs.Add(mnc); 
						} 
				} 
			} 
			return mn; 
		}

		/// <summary> 
		/// Add a Menu Tree Node if user in in the list of Authorized roles. 
		/// Thanks to abain for fixing authorization bug.
		/// </summary> 
		/// <param name="tabIndex">Index of the tab</param> 
		/// <param name="myTab">Tab to add to the MenuTreeNodes collection</param> 
		protected virtual void AddMenuTreeNode(int tabIndex, PageStripDetails myTab) //MH:
		{ 
			if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles)) 
			{ 
				MenuTreeNode mn = new MenuTreeNode(myTab.PageName);
				
				mn.Link = giveMeUrl (myTab.PageName, myTab.PageID);
				mn.Height = Height; 
				mn.Width = Width; 
				mn = RecourseMenu(tabIndex, myTab.Pages, mn); 
				this.Childs.Add(mn); 
				
			} 
		} 
		/// <summary>
		/// 
		/// </summary>
		protected virtual MenuTreeNode RecourseMenu(int tabIndex, PagesBox t, MenuTreeNode mn) //mh:
		{ 
			if (t.Count > 0) 
			{ 
				for (int c=0; c < t.Count; c++) 
				{ 
					PageStripDetails mySubTab = t[c]; 
					if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles)) 
					{ 
						MenuTreeNode mnc = new MenuTreeNode(mySubTab.PageName);
						
							mnc.Link = giveMeUrl(mySubTab.PageName, mySubTab.PageID); 
							mnc.Width = mn.Width; 
							if (products (mySubTab.PageID)) 
							{
								PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"]; 
								if (portalSettings.ActivePage.PageID == mySubTab.PageID)
									mnc = RecourseMenuShop(tabIndex, mySubTab.Pages, mnc, mySubTab.PageID); 
							} 
							else 
							{
								mnc = RecourseMenu(tabIndex, mySubTab.Pages, mnc); 
							}
							mn.Childs.Add(mnc); 
						
					} 
				} 
			} 
			return mn; 
		}

		/// <summary>
		/// Populates ArrayList of tabs based on binding option selected.
		/// </summary>
		/// <returns></returns>
		protected object GetInnerDataSource()
		{
			ArrayList authorizedTabs = new ArrayList();

			if(HttpContext.Current != null)
			{
				// Obtain PortalSettings from Current Context
				PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

				switch (Bind)
				{
					case BindOption.BindOptionTop :
					{
						authorizedTabs = GetTabs(0, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
						break;
					}

					case BindOption.BindOptionCurrentChilds :
					{
						int currentTabRoot = PortalSettings.GetRootPage(portalSettings.ActivePage, portalSettings.DesktopPages).PageID;
						authorizedTabs = GetTabs(currentTabRoot, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
						break;
					}

					case BindOption.BindOptionSubtabSibling :
					{
						int currentTabRoot;
						if (portalSettings.ActivePage.ParentPageID == 0)
							currentTabRoot = portalSettings.ActivePage.PageID;
						else
							currentTabRoot = portalSettings.ActivePage.ParentPageID;

						authorizedTabs = GetTabs(currentTabRoot, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
						break;
					}

					case BindOption.BindOptionChildren :
					{
						authorizedTabs = GetTabs(portalSettings.ActivePage.PageID,  portalSettings.ActivePage.PageID,portalSettings.DesktopPages);
						break;
					}

					case BindOption.BindOptionSiblings :
					{
						authorizedTabs = GetTabs(portalSettings.ActivePage.ParentPageID, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
						break;
					}

						//MH: added 19/09/2003 by mario@hartmann.net
					case BindOption.BindOptionTabSibling :
					{
						authorizedTabs = GetTabs(portalSettings.ActivePage.PageID, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
						
						if (authorizedTabs.Count == 0)
							authorizedTabs = GetTabs(portalSettings.ActivePage.ParentPageID, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);

						break;
					}

						//MH: added 29/04/2003 by mario@hartmann.net
					case BindOption.BindOptionDefinedParent:
						if (ParentPageID != -1)
							authorizedTabs = GetTabs(ParentPageID, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
						break;
						//MH: end
					default:
					{
						break;
					}
				}
			}
			return authorizedTabs;
		}

		/// <summary>
		/// Seems to be unused - Jes1111
		/// </summary>
		/// <param name="parentPageID"></param>
		/// <param name="activePageID"></param>
		/// <param name="allTabs"></param>
		/// <returns></returns>
		private int GetSelectedTab(int parentPageID, int activePageID , IList allTabs)
		{
			for( int i = 0 ; i < allTabs.Count; i++)
			{
				PageStripDetails tmpTab = (PageStripDetails) allTabs[i];
				if (tmpTab.PageID == activePageID)
				{
					int selectedPageID = activePageID;
					if (tmpTab.ParentPageID != parentPageID) 
					{
						selectedPageID = GetSelectedTab(parentPageID,tmpTab.ParentPageID,allTabs);
						return selectedPageID;
					}	
					else
					{
						return selectedPageID;
					}

				}
			}
			return 0;
		}					   																				   

		private ArrayList GetTabs(int parentID, int tabID, IList Tabs)
		{
			ArrayList authorizedTabs = new ArrayList();
			//int index = -1;

			//MH:get the selected tab for this 
			//int selectedPageID = GetSelectedTab (parentID, tabID,Tabs);

			// Populate Tab List with authorized tabs
			for (int i=0; i < Tabs.Count; i++)
			{
				PageStripDetails tab = (PageStripDetails) Tabs[i];

				if(tab.ParentPageID == parentID) // Get selected row only
				{
					if (PortalSecurity.IsInRoles(tab.AuthorizedRoles))
					{
						//index = authorizedTabs.Add(tab);
						authorizedTabs.Add(tab);
					}
				}
			}
			return authorizedTabs;
		}
		/// <summary>
		/// 
		/// </summary>
		protected override string GetClientScriptPath()
		{
			return string.Concat(Path.ApplicationRoot, "/aspnet_client/DUEMETRI_UI_WebControls_HWMenu/1_0_0_0/");
		}
	} 
}