using System;
using System.Threading;
using System.Xml.Serialization;
using Rainbow.Settings.Cache;

namespace Rainbow.Configuration
{

	/// <summary>
	/// TabStripDetails Class encapsulates the tabstrip details
	/// -- TabName, TabID and TabOrder -- for a specific Tab in the Portal
	/// </summary>
	[XmlType(TypeName="MenuItem")]
	[Obsolete("Please use PageStripDetails")]
	public class TabStripDetails
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("AuthRoles")]    public string AuthorizedRoles;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("ParentTabID")]  public int    ParentTabID;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("TabImage")]     public string TabImage;	
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("TabIndex")]     public int    TabIndex;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("TabLayout")]    public string TabLayout;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("Label")]        public string TabName;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("TabOrder")]     public int    TabOrder;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		int tabID;

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
		[XmlAttribute("ID")]
		[Obsolete("Please use PageStripDetails")]
		public int TabID
		{
			get
			{
				return tabID;
			}
			set
			{
				tabID = value;
			}       
		}

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
		[XmlArray(ElementName="MenuGroup", IsNullable = false)]
		[Obsolete("Please use PageStripDetails")]
		public PagesBox Tabs 
		{
			get
			{
				string cacheKey = Key.TabNavigationSettings (tabID, Thread.CurrentThread.CurrentUICulture.ToString());
				PagesBox tabs;

				if (!CurrentCache.Exists (cacheKey))
				{
					//					tabs = new TabsBox();
					//
					//					SqlDataReader result = TabSettings.GetTabSettings(tabID);
					//            
					//					try
					//					{
					//						while (result.Read()) 
					//						{
					//							TabStripDetails tabDetails = new TabStripDetails();
					//							tabDetails.TabID = (int) result["TabID"];
					//							Hashtable cts = new TabSettings().GetTabCustomSettings(tabDetails.TabID);
					//							tabDetails.TabImage = cts["CustomMenuImage"].ToString();
					//							tabDetails.ParentTabID = Int32.Parse("0" + result["ParentTabID"]);
					//							tabDetails.TabName = (string) result["TabName"];
					//							tabDetails.TabOrder = (int) result["TabOrder"];
					//							tabDetails.AuthorizedRoles = (string) result["AuthorizedRoles"];
					//							tabs.Add(tabDetails);
					//						}
					//					}
					//					finally
					//					{
					//						result.Close(); //by Manu, fixed bug 807858
					//					}
					tabs = TabSettings.GetTabSettingsTabsBox(tabID);
					CurrentCache.Insert(cacheKey, tabs);
				}

				else
				{
					tabs = (PagesBox) CurrentCache.Get (cacheKey);
				}
				return tabs;
			}
		}
	}
}
