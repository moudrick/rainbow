/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario@hartmann.net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 02/JUN/2004
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.Settings.Cache;
using Solpart.WebControls;
using Path = Rainbow.Settings.Path;

namespace Rainbow.UI.WebControls 
{ 

	/// <summary>
	/// </summary>
	[History("gman3001","2004/10/06","Add support for the active root tab to use a custom css style for normal and highlighting purposes")]
	[History("jviladiu@portalServices.net","2004/08/26","Add AutoShopDetect support and set url's for categories of products")]
	[History("jviladiu@portalServices.net","2004/08/26","Add ShowIconMenu property")]
	[History("mgregory@gt.com.au","2006/01/04","Change to 1_6_1_0, remove spMenuStyle control from Page.cs as it is not needed if property SeparateCSS is set to true and it is not 4.01 dtd strict compliant")]
	public class SolpartNavigation : SolpartMenu, INavigation 
	{ 

		public SolpartNavigation() 
		{ 
			this.EnableViewState = false; 
			this.Load += new EventHandler(this.LoadControl);
		} 

		private void LoadControl(object sender, EventArgs e) 
		{ 
			base.SystemScriptPath =string.Concat(Path.ApplicationRoot, "/aspnet_client/SolpartWebControls_SolpartMenu/1_6_1_0/");
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"]; 
			string solpart = string.Concat(Path.ApplicationRoot, "/aspnet_client/SolpartWebControls_SolpartMenu/1_6_1_0/");

			if (ShowIconMenu) 
			{
				base.SystemImagesPath = Path.WebPathCombine(portalSettings.PortalLayoutPath, "menuimages/");
				string menuDirectory = HttpContext.Current.Server.MapPath(base.SystemImagesPath);

				// Create directory and copy standard images for solpart
				if (!Directory.Exists (menuDirectory))
				{
					Directory.CreateDirectory (menuDirectory);
					string solpartPhysicalDir = HttpContext.Current.Server.MapPath(solpart);
					if (File.Exists(solpartPhysicalDir + "/spacer.gif")) 
					{
						File.Copy (solpartPhysicalDir + "/spacer.gif", menuDirectory + "/spacer.gif");
						File.Copy (solpartPhysicalDir + "/spacer.gif", menuDirectory + "/menu.gif");
					}
					if (File.Exists(solpartPhysicalDir + "/icon_arrow.gif")) 
						File.Copy (solpartPhysicalDir + "/icon_arrow.gif", menuDirectory + "/icon_arrow.gif");					
				}
			}
			else base.SystemImagesPath = solpart;

			base.MenuCSSPlaceHolderControl="spMenuStyle";
			base.SeparateCSS = true;

			if(AutoBind) 
				DataBind(); 
		} 

		#region INavigation implementation 

		private BindOption _bind = BindOption.BindOptionTop; 
		private int _definedParentTab = -1;
		private bool _autoBind = false; 
		private bool _autoShopDetect = false;
		private bool _showIconMenu = false;
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

		/// <summary> 
		/// Indicates if control should detect products module when loads 
		/// </summary> 
		[ Category("Data"), PersistenceMode(PersistenceMode.Attribute) ] 
		public bool AutoShopDetect 
		{ 
			get{return _autoShopDetect;} 
			set{_autoShopDetect = value;} 
		} 

		/// <summary> 
		/// Indicates if control should render images in menu 
		/// </summary> 
		[ Category("Data"), PersistenceMode(PersistenceMode.Attribute) ] 
		public bool ShowIconMenu 
		{ 
			get{return _showIconMenu;} 
			set{_showIconMenu = value;} 
		} 

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
		#endregion 

		/// <summary>
		/// Do databind.
		/// Thanks to abain for cleaning up the code and fixing the bugs
		/// </summary>
		public override void DataBind() 
		{ 
			base.DataBind(); 

			//bool currentTabOnly = (Bind == BindOption.BindOptionCurrentChilds); 

			// Obtain PortalSettings from Current Context 
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"]; 

			// Build list of tabs to be shown to user 
			ArrayList authorizedTabs = setAutorizedTabsWithImage();

			for (int i = 0; i < authorizedTabs.Count; i++) 
			{ 
				PageStripDetails myTab = (PageStripDetails) authorizedTabs[i];
				if (products (myTab.PageID)) 
				{
					AddGraphMenuItem (null, myTab.PageID.ToString(), myTab.PageName, myTab.PageImage, giveMeUrl (myTab.PageName, myTab.PageID), false);
					if (portalSettings.ActivePage.PageID == myTab.PageID)
						ShopMenu (myTab, portalSettings.ActivePage.PageID);
				} 
				else 
				{
					// gman3001: Statement Added/Modified 2004/10/06
					// for now only set default css styles for the active root menu item, if it is not a products menu
					if(isActiveTabIn(portalSettings.ActivePage.PageID,myTab))
						AddGraphMenuItem (null, myTab.PageID.ToString(), myTab.PageName, myTab.PageImage, giveMeUrl (myTab.PageName, myTab.PageID), false, this.Attributes["MenuCSS-MenuDefaultItem"],this.Attributes["MenuCSS-MenuDefaultItemHighLight"]);
					else
						AddGraphMenuItem (null, myTab.PageID.ToString(), myTab.PageName, myTab.PageImage, giveMeUrl (myTab.PageName, myTab.PageID), false);
					
					RecourseMenu (myTab, portalSettings.ActivePage.PageID);	
				}
			} 
		} 

		private ArrayList setAutorizedTabsWithImage() 
		{
			ArrayList authorizedTabs = (ArrayList) GetInnerDataSource();
			if (!ShowIconMenu) return authorizedTabs;

			for (int i = 0; i < authorizedTabs.Count; i++) 
			{ 
				PageStripDetails myTab = (PageStripDetails) authorizedTabs[i];
				if (myTab.PageImage == null)
				{
					myTab.PageImage = (new PageSettings().GetPageCustomSettings(myTab.PageID))["CustomMenuImage"].ToString();
				}
			}
			return authorizedTabs;
		}

		// gman3001: Method Added 2004/10/06
		// Method determines if the current PageStripDetails contains the active tab, by checking all of its descendants
		private bool isActiveTabIn(int activePageID, PageStripDetails PageStripDetails)
		{
			if (PageStripDetails.PageID == activePageID)
				return true;
			PagesBox childTabs = PageStripDetails.Pages;
			if (childTabs.Count > 0) 
			{ 
				for (int c=0; c < childTabs.Count; c++) 
				{ 
					PageStripDetails mySubTab = childTabs[c]; 
					if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles)) 
					{ 
						if(isActiveTabIn(activePageID,mySubTab))
							return true;
					} 
				} 
				childTabs = null;
			} 
			return false;
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

		protected virtual void ShopMenu(PageStripDetails PageStripDetails, int activePageID)
		{ 
			PagesBox childTabs = PageStripDetails.Pages;
			if (childTabs.Count > 0) 
			{ 
				for (int c=0; c < childTabs.Count; c++) 
				{ 
					PageStripDetails mySubTab = childTabs[c]; 
					if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles)) 
					{ 
						AddGraphMenuItem (PageStripDetails.PageID.ToString(), mySubTab.PageID.ToString(), mySubTab.PageName, mySubTab.PageImage, HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, activePageID, "ItemID=" + mySubTab.PageID.ToString()), false);
						ShopMenu (mySubTab, activePageID); 
					} 
				} 
				childTabs = null;
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

		protected virtual void RecourseMenu(PageStripDetails PageStripDetails, int activePageID)
		{ 
			PagesBox childTabs = PageStripDetails.Pages;
			if (childTabs.Count > 0) 
			{ 
				for (int c=0; c < childTabs.Count; c++) 
				{ 
					PageStripDetails mySubTab = childTabs[c]; 
					if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles)) 
					{ 
						if (mySubTab.PageImage == null) 
						{
							mySubTab.PageImage = (new PageSettings().GetPageCustomSettings(mySubTab.PageID))["CustomMenuImage"].ToString();
						}
						if (products (mySubTab.PageID)) 
						{
							AddGraphMenuItem (PageStripDetails.PageID.ToString(), mySubTab.PageID.ToString(), mySubTab.PageName, mySubTab.PageImage, giveMeUrl (mySubTab.PageName, mySubTab.PageID), false);
							if (activePageID == mySubTab.PageID)
								ShopMenu (mySubTab, activePageID);
						} 
						else 
						{
							AddGraphMenuItem (PageStripDetails.PageID.ToString(), mySubTab.PageID.ToString(), mySubTab.PageName, mySubTab.PageImage, giveMeUrl (mySubTab.PageName, mySubTab.PageID), false);
							RecourseMenu (mySubTab, activePageID); 
						}
					} 
				} 
				childTabs = null;
			} 
		}
		
		// gman3001: 2004/10/06 Method modified to call more detailed overloaded version below.
		private void AddGraphMenuItem (string parent, string tab, string tabname, string iconfile, string url, bool translation)
		{
			AddGraphMenuItem(parent, tab, tabname, iconfile, url, translation, string.Empty, string.Empty);
		}

		// gman3001: 2004/10/06 Method overload added to support custom css styles for individuals menu items
		private void AddGraphMenuItem (string parent, string tab, string tabname, string iconfile, string url, bool translation, string customcss, string customhighlightcss)
		{
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"]; 
			string pathGraph = HttpContext.Current.Server.MapPath (portalSettings.PortalLayoutPath + "/menuimages/");
			string tabTranslation = tabname;
			if (translation) tabTranslation = Localize.GetString(tabname);

			XmlNode padre = null;

			// gman3001: Line added 2004/10/06
			XmlNode newNode = null;

			if (parent != null)
			{
				padre = base.FindMenuItem (parent).Node;
			}
			
			if (ShowIconMenu) 
			{
				if (File.Exists (pathGraph + iconfile)) 
				{
					// gman3001: Line modified 2004/10/06, added assignment to newNode
					newNode = base.AddMenuItem(padre, tab, tabTranslation, url, iconfile, false, string.Empty, string.Empty);
				}
				else 
				{
					if (padre == null) 
					{
						// gman3001: Line modified 2004/10/06, added assignment to newNode
						newNode = base.AddMenuItem(padre, tab, tabTranslation, url, "menu.gif", false, string.Empty, string.Empty);
					} 
					else 
					{
						// gman3001: Line modified 2004/10/06, added assignment to newNode
						newNode = base.AddMenuItem(padre, tab, tabTranslation, url, string.Empty, false, string.Empty, string.Empty);
					}
				}
			}
			else 
			{
				// gman3001: Line modified 2004/10/06, added assignment to newNode
				newNode = base.AddMenuItem(padre, tab, tabTranslation, url, string.Empty, false, string.Empty, string.Empty);
			}
			
			// gman3001: Added 2004/10/06
			// Added support to add a custom css class and a custom css highlight class to individual menu items		
			if (newNode != null)
			{
				AddAttributetoItem(newNode,"css",customcss);
				AddAttributetoItem(newNode,"highlightcss",customhighlightcss);
			}
		}

		// gman3001: Method Added 2004/10/06
		//Method to support adding/modifying Custom Attributes to a Menu Item, to be rendered by the client
		private void AddAttributetoItem(XmlNode curNode, string AttributeName, string Value)
		{
			if (curNode != null && AttributeName != null && AttributeName.Length > 0 && Value != null && Value.Length > 0)
			{
				XmlAttribute myItemAttribute = curNode.Attributes[AttributeName];
				// if current attribute exists assign new value to it
				if (myItemAttribute != null)
					myItemAttribute.Value = Value;
				// otherwise add a new attribute to the node and assign the value to it
				else 
				{
					myItemAttribute = curNode.OwnerDocument.CreateAttribute(AttributeName);
					myItemAttribute.Value = Value;	
					curNode.Attributes.SetNamedItem(myItemAttribute);
				}
			}
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
			//int selectedPageID = GetSelectedTab (parentID, tabID, Tabs);

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


	} 
}