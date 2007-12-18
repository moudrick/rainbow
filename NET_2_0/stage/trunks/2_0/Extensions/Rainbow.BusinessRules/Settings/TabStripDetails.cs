using System.Threading;
using System.Xml.Serialization;
using Rainbow.Core;

namespace Rainbow.Configuration
{
	/// <summary>
	/// TabStripDetails Class encapsulates the tabstrip details
	/// -- TabName, TabID and TabOrder -- for a specific Tab in the Portal
	/// </summary>
	[XmlType(TypeName="MenuItem")]
	public class TabStripDetails : Page
	{
		[XmlAttribute("AuthRoles")] private string _authorizedRoles;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string AuthorizedRoles
		{
			get { return _authorizedRoles; }
			set { _authorizedRoles = value; }
		}


		[XmlAttribute("ParentTabID")] private int parentTabID;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int ParentTabID
		{
			get { return parentTabID; }
			set { parentTabID = value; }
		}


		[XmlAttribute("TabImage")] private string tabImage;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string TabImage
		{
			get { return tabImage; }
			set { tabImage = value; }
		}


		[XmlAttribute("TabIndex")] private int tabIndex;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int TabIndex
		{
			get { return tabIndex; }
			set { tabIndex = value; }
		}


		[XmlAttribute("TabLayout")] private string tabLayout;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string TabLayout
		{
			get { return tabLayout; }
			set { tabLayout = value; }
		}


		[XmlAttribute("Label")] private string tabName;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string TabName
		{
			get { return tabName; }
			set { tabName = value; }
		}


		[XmlAttribute("TabOrder")] private int tabOrder;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int TabOrder
		{
			get { return tabOrder; }
			set { tabOrder = value; }
		}


		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private int tabID;

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
		public int TabID
		{
			get { return tabID; }
			set { tabID = value; }
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
		public PagesBox Pages
		{
			get
			{
				string cacheKey = Key.TabNavigationSettings(tabID, Thread.CurrentThread.CurrentUICulture.ToString());
				PagesBox pages;

				if (!CurrentCache.Exists(cacheKey))
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
					pages = TabSettings.GetTabSettingsTabsBox(tabID);
					CurrentCache.Insert(cacheKey, pages);
				}

				else
					pages = (PagesBox) CurrentCache.Get(cacheKey);
				return pages;
			}
		}
	}
}