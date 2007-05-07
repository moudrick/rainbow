using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Esperantus;
using Esperantus.WebControls;
using Rainbow.Core;
using Rainbow.Design;
using Rainbow.Helpers;
using Rainbow.Scheduler;
using Rainbow.Security;
using Rainbow.Settings;
using Rainbow.Settings.Cache;
using Rainbow.UI.DataTypes;
using Path = Rainbow.Settings.Path;

namespace Rainbow.Configuration
{

	/// <summary>
	/// PortalSettings Class encapsulates all of the settings 
	/// for the Portal, as well as the configuration settings required 
	/// to execute the current tab view within the portal.
	/// </summary>
	[History("gman3001", "2004/09/29", "Added the GetCurrentUserProfile method to obtain a hashtable of the current user's profile details.")]
	[History("jviladiu@portalServices.net", "2004/08/19", "Add support for move & delete module roles")]
	[History("jviladiu@portalServices.net", "2004/07/30", "Added new ActiveModule property")]
	[History("Jes1111", "2003/03/09", "Added new ShowTabs property")]
	[History("Jes1111", "2003/04/02", "Added new DesktopTabsXml property (an XPathDocument)")]
	[History("Thierry", "2003/04/12", "Added PortalSecurePath property")]
	[History("Jes1111", "2003/04/17", "Added new language-related properties and methods")]
	[History("Jes1111", "2003/04/23", "Corrected string comparison case problem in language settings")]
	[History("cisakson@yahoo.com", "2003/04/28", "Added a custom setting for Windows users to assign a portal Admin")]
	public class PortalSettings
	{

		private const string strATPortalID = "@PortalID";
		private const string strATTabID = "@TabID";
		private const string strCustomLayout = "CustomLayout";
		private const string strCustomTheme = "CustomTheme";
		private const string strName = "Name";

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public TabSettings ActiveTab = new TabSettings();

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public Hashtable CustomSettings;

		//        public bool         AlwaysShowEditButton;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public ArrayList DesktopTabs = new ArrayList();

		/// <summary>
		/// Jes
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public XPathDocument DesktopTabsXml;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public ArrayList MobileTabs = new ArrayList();

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string PortalAlias;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public int PortalID;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string PortalName;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public string PortalTitle;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public static IScheduler Scheduler; // Federico (ifof@libero.it) 18 jun 2003

		/// <summary>
		/// Jes
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public bool ShowTabs = true;

		private string _currentLayout;
		private string _portalPath = string.Empty;
		private string _portalPathPrefix =
			HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty :
			HttpContext.Current.Request.ApplicationPath;

		private string _portalSecurePath; // Thierry (tiptopweb) 12 Apr 2003

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private XmlDocument _portalTabsXml;
		private Theme CurrentThemeAlt;

		// -- Thierry (Tiptopweb), 21 June [END] 
		private Theme CurrentThemeDefault;
		private static readonly bool enableMonitoring =
			(ConfigurationSettings.AppSettings["EnableMonitoring"] == null
			? true //default 
			: bool.Parse(ConfigurationSettings.AppSettings["EnableMonitoring"]));

		/// <summary>
		/// The PortalSettings Constructor encapsulates all of the logic
		/// necessary to obtain configuration settings necessary to render
		/// a Portal Tab view for a given request.<br/>
		/// These Portal Settings are stored within a SQL database, and are
		/// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
		/// This stored procedure returns values as SPROC output parameters,
		/// and using three result sets.
		/// </summary>
		/// <param name="tabID"></param>
		/// <param name="portalAlias"></param>
		public PortalSettings(int tabID, string portalAlias)
		{

			// Changes culture/language according to settings
			try
			{
				//Moved here for support db call
				LanguageSwitcher.ProcessCultures(GetLanguageList(portalAlias), portalAlias);
			}

			catch (Exception ex)
			{
				ErrorHandler.HandleException("Failed to load languages, loading defaults", ex);
				LanguageSwitcher.ProcessCultures(DesktopModules.LanguageSwitcher.LANGUAGE_DEFAULT, portalAlias);
			}

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			{

				using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettings", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
					parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
					myCommand.Parameters.Add(parameterPortalAlias);
					SqlParameter parameterTabID = new SqlParameter(strATTabID, SqlDbType.Int, 4);
					parameterTabID.Value = tabID;
					myCommand.Parameters.Add(parameterTabID);
					SqlParameter parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12);
					parameterPortalLanguage.Value = this.PortalContentLanguage.Name;
					myCommand.Parameters.Add(parameterPortalLanguage);
					// Add out parameters to Sproc
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterPortalID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterPortalName = new SqlParameter("@PortalName", SqlDbType.NVarChar, 128);
					parameterPortalName.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterPortalName);
					SqlParameter parameterPortalPath = new SqlParameter("@PortalPath", SqlDbType.NVarChar, 128);
					parameterPortalPath.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterPortalPath);
					SqlParameter parameterEditButton = new SqlParameter("@AlwaysShowEditButton", SqlDbType.Bit, 1);
					parameterEditButton.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterEditButton);
					SqlParameter parameterTabName = new SqlParameter("@TabName", SqlDbType.NVarChar, 50);
					parameterTabName.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterTabName);
					SqlParameter parameterTabOrder = new SqlParameter("@TabOrder", SqlDbType.Int, 4);
					parameterTabOrder.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterTabOrder);
					SqlParameter parameterParentTabID = new SqlParameter("@ParentTabID", SqlDbType.Int, 4);
					parameterParentTabID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterParentTabID);
					SqlParameter parameterMobileTabName = new SqlParameter("@MobileTabName", SqlDbType.NVarChar, 50);
					parameterMobileTabName.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterMobileTabName);
					SqlParameter parameterAuthRoles = new SqlParameter("@AuthRoles", SqlDbType.NVarChar, 256);
					parameterAuthRoles.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterAuthRoles);
					SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
					parameterShowMobile.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterShowMobile);
					SqlDataReader result;

					try
					{
						// Open the database connection and execute the command
						myConnection.Open();
						result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
						this.CurrentLayout = "Default";

						// Read the first resultset -- Desktop Tab Information
						while (result.Read())
						{
							TabStripDetails tabDetails = new TabStripDetails();
							tabDetails.TabID = (int) result["TabID"];
							tabDetails.ParentTabID = Int32.Parse("0" + result["ParentTabID"]);
							tabDetails.TabName = (string) result["TabName"];
							tabDetails.TabOrder = (int) result["TabOrder"];
							tabDetails.TabLayout = this.CurrentLayout;
							tabDetails.AuthorizedRoles = (string) result["AuthorizedRoles"];
							this.PortalAlias = portalAlias;
							// Update the AuthorizedRoles Variable
							this.DesktopTabs.Add(tabDetails);
						}

						if (DesktopTabs.Count == 0)
						{
							return; //Abort load
							//throw new Exception("The portal you requested has no Tabs. PortalAlias: '" + portalAlias + "'", new HttpException(404, "Portal not found"));
						}
						// Read the second result --  Mobile Tab Information
						result.NextResult();

						while (result.Read())
						{
							TabStripDetails tabDetails = new TabStripDetails();
							tabDetails.TabID = (int) result["TabID"];
							tabDetails.TabName = (string) result["MobileTabName"];
							tabDetails.TabLayout = this.CurrentLayout;
							tabDetails.AuthorizedRoles = (string) result["AuthorizedRoles"];
							this.MobileTabs.Add(tabDetails);
						}
						// Read the third result --  Module Tab Information
						result.NextResult();
						object myValue;

						while (result.Read())
						{
							ModuleSettings m = new ModuleSettings();
							m.ModuleID = (int) result["ModuleID"];
							m.ModuleDefID = (int) result["ModuleDefID"];
							m.TabID = (int) result["TabID"];
							m.PaneName = (string) result["PaneName"];
							m.ModuleTitle = (string) result["ModuleTitle"];
							myValue = result["AuthorizedEditRoles"];
							m.AuthorizedEditRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["AuthorizedViewRoles"];
							m.AuthorizedViewRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["AuthorizedAddRoles"];
							m.AuthorizedAddRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["AuthorizedDeleteRoles"];
							m.AuthorizedDeleteRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["AuthorizedPropertiesRoles"];
							m.AuthorizedPropertiesRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							// jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
							myValue = result["AuthorizedMoveModuleRoles"];
							m.AuthorizedMoveModuleRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["AuthorizedDeleteModuleRoles"];
							m.AuthorizedDeleteModuleRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							// Change by Geert.Audenaert@Syntegra.Com
							// Date: 6/2/2003
							myValue = result["AuthorizedPublishingRoles"];
							m.AuthorizedPublishingRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["SupportWorkflow"];
							m.SupportWorkflow = ! Convert.IsDBNull(myValue) ? (bool) myValue : false;
							// Date: 27/2/2003
							myValue = result["AuthorizedApproveRoles"];
							m.AuthorizedApproveRoles = ! Convert.IsDBNull(myValue) ? (string) myValue : string.Empty;
							myValue = result["WorkflowState"];
							m.WorkflowStatus = ! Convert.IsDBNull(myValue) ? (WorkflowState) (0 + (byte) myValue) : WorkflowState.Original;

							// End Change Geert.Audenaert@Syntegra.Com
							// Start Change bja@reedtek.com
							try
							{
								myValue = result["SupportCollapsable"];
							}

							catch
							{
								myValue = DBNull.Value;
							}
							m.SupportCollapsable = DBNull.Value != myValue ? (bool) myValue : false;

							// End Change  bja@reedtek.com
							// Start Change john.mandia@whitelightsolutions.com
							try
							{
								myValue = result["ShowEveryWhere"];
							}

							catch
							{
								myValue = DBNull.Value;
							}
							m.ShowEveryWhere = DBNull.Value != myValue ? (bool) myValue : false;
							// End Change  john.mandia@whitelightsolutions.com
							m.CacheTime = int.Parse(result["CacheTime"].ToString());
							m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());
							myValue = result["ShowMobile"];
							m.ShowMobile = ! Convert.IsDBNull(myValue) ? (bool) myValue : false;
							m.DesktopSrc = result["DesktopSrc"].ToString();
							m.MobileSrc = result["MobileSrc"].ToString();
							m.Admin = bool.Parse(result["Admin"].ToString());
							this.ActiveTab.Modules.Add(m);
						}
						// Now read Portal out params 
						result.NextResult();
						this.PortalID = (int) parameterPortalID.Value;
						this.PortalName = (string) parameterPortalName.Value;
						this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
						this.PortalPath = Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
						this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003
						this.ActiveTab.TabID = tabID;
						this.ActiveTab.TabLayout = this.CurrentLayout;
						this.ActiveTab.ParentTabID = Int32.Parse("0" + parameterParentTabID.Value);
						this.ActiveTab.TabOrder = (int) parameterTabOrder.Value;
						this.ActiveTab.MobileTabName = (string) parameterMobileTabName.Value;
						this.ActiveTab.AuthorizedRoles = (string) parameterAuthRoles.Value;
						this.ActiveTab.TabName = (string) parameterTabName.Value;
						this.ActiveTab.ShowMobile = (bool) parameterShowMobile.Value;
						this.ActiveTab.PortalPath = PortalPath; // thierry@tiptopweb.com.au for page custom layout
						result.Close(); //by Manu, fixed bug 807858
					}

					catch (SqlException sqex)
					{
						LogHelper.Logger.Log(LogLevel.Warn, "This may be a new db", sqex);

						//This may be a new db
						if (!HttpContext.Current.Request.RawUrl.EndsWith("/Setup/Update.aspx"))
							HttpContext.Current.Response.Redirect(Path.ApplicationRoot + "/Setup/Update.aspx");
						return;
					}

					finally
					{

						//by Manu fix close bug #2
						if (myConnection.State == ConnectionState.Open)
							myConnection.Close();
					}
				}
			}

			//Provide a valid tab id if it is missing
			if (this.ActiveTab.TabID == 0)
				this.ActiveTab.TabID = ((TabStripDetails) this.DesktopTabs[0]).TabID;
			//Go to get custom settings
			CustomSettings = GetPortalCustomSettings(PortalID, GetPortalBaseSettings(PortalPath));
			//Initialize Theme
			ThemeManager themeManager = new ThemeManager(PortalPath);
			//Default
			themeManager.Load(this.CustomSettings["SITESETTINGS_THEME"].ToString());
			CurrentThemeDefault = themeManager.CurrentTheme;

			//Alternate
			if (this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString() == CurrentThemeDefault.Name)
				CurrentThemeAlt = CurrentThemeDefault;

			else
			{
				themeManager.Load(this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
				CurrentThemeAlt = themeManager.CurrentTheme;
			}
			//themeManager.Save(this.CustomSettings["SITESETTINGS_THEME"].ToString());
			//Set layout
			this.CurrentLayout = CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();

			// Jes1111
			// Generate DesktopTabsXml
			if (bool.Parse(ConfigurationSettings.AppSettings["PortalSettingDesktopTabsXml"]))
				this.DesktopTabsXml = GetDesktopTabsXml();
		}

		/// <summary>
		/// The PortalSettings Constructor encapsulates all of the logic
		/// necessary to obtain configuration settings necessary to get
		/// custom setting for a different portal than current (EditPortal.aspx.cs)<br/>
		/// These Portal Settings are stored within a SQL database, and are
		/// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
		/// This overload it is used 
		/// </summary>
		/// <param name="PortalID"></param>
		public PortalSettings(int PortalID)
		{

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			{

				using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsPortalID", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int);
					parameterPortalID.Value = PortalID;
					myCommand.Parameters.Add(parameterPortalID);
					// Open the database connection and execute the command
					myConnection.Open();
					SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection); //by Manu CloseConnection

					try
					{

						if (result.Read())
						{
							this.PortalID = Int32.Parse(result["PortalID"].ToString());
							this.PortalName = result["PortalName"].ToString();
							this.PortalAlias = result["PortalAlias"].ToString();
							this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + result["PortalName"].ToString();
							this.PortalPath = result["PortalPath"].ToString();
							this.ActiveTab.TabID = 0;
							// added Thierry (tiptopweb) used for dropdown for layout and theme
							this.ActiveTab.PortalPath = PortalPath;
							this.ActiveModule = 0;
						}

						else
							throw new Exception("The portal you requested cannot be found. PortalID: " + PortalID.ToString(), new HttpException(404, "Portal not found"));
					}

					finally
					{
						result.Close(); //by Manu, fixed bug 807858
						myConnection.Close();
					}
				}
			}
			//Go to get custom settings
			CustomSettings = GetPortalCustomSettings(PortalID, GetPortalBaseSettings(PortalPath));
			this.CurrentLayout = CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();
			//Initialize Theme
			ThemeManager themeManager = new ThemeManager(PortalPath);
			//Default
			themeManager.Load(this.CustomSettings["SITESETTINGS_THEME"].ToString());
			CurrentThemeDefault = themeManager.CurrentTheme;
			//Alternate
			themeManager.Load(this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
			CurrentThemeAlt = themeManager.CurrentTheme;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="PortalPath" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void FlushBaseSettingsCache(string PortalPath)
		{
			CurrentCache.Remove(Key.PortalBaseSettings());
		}

		// -- Thierry (Tiptopweb), 21 Jun 2003 [START] 
		// -- Thierry (Tiptopweb),  3 Feb 2004, fixed mismatch Alt and Default theme (Alt always returned)
		// Switch the Theme if a custom theme is defined in the tab settings
		// (using custom themes from TabSettings.cs)
		// if not use the theme defined from the portalsettings
		/// <summary>
		/// Theme definition and images
		/// </summary>
		public Theme GetCurrentTheme()
		{

			// look for an custom theme
			if (this.ActiveTab.CustomSettings[strCustomTheme] != null && this.ActiveTab.CustomSettings[strCustomTheme].ToString().Length > 0)
			{
				string customTheme = this.ActiveTab.CustomSettings[strCustomTheme].ToString().Trim();
				ThemeManager themeManager = new ThemeManager(PortalPath);
				themeManager.Load(customTheme);
				return themeManager.CurrentTheme;
			}
			// no custom theme
			return CurrentThemeDefault;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="requiredTheme" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Rainbow.Design.Theme value...
		/// </returns>
		public Theme GetCurrentTheme(string requiredTheme)
		{

			switch (requiredTheme)
			{

				case "Alt":

					// look for an alternate custom theme
					if (this.ActiveTab.CustomSettings["CustomThemeAlt"] != null && this.ActiveTab.CustomSettings["CustomThemeAlt"].ToString().Length > 0)
					{
						string customTheme = this.ActiveTab.CustomSettings["CustomThemeAlt"].ToString().Trim();
						ThemeManager themeManager = new ThemeManager(PortalPath);
						themeManager.Load(customTheme);
						return themeManager.CurrentTheme;
					}
					// no custom theme
					return CurrentThemeAlt;
				default:

					// look for an custom theme
					if (this.ActiveTab.CustomSettings[strCustomTheme] != null && this.ActiveTab.CustomSettings[strCustomTheme].ToString().Length > 0)
					{
						string customTheme = this.ActiveTab.CustomSettings[strCustomTheme].ToString().Trim();
						ThemeManager themeManager = new ThemeManager(PortalPath);
						themeManager.Load(customTheme);
						return themeManager.CurrentTheme;
					}
					// no custom theme
					return CurrentThemeDefault;
			}
		}

		/// <summary>
		/// The PortalSettings.GetCurrentUserProfile Method returns a hashtable of
		/// all the fields and their values for currently logged user in the users table.
		/// Used to retrieve a specific profile detail about the current user, without knowing whether the field exists in the user table or not.
		/// </summary>
		/// <param name="PortalID"></param>
		/// <returns>A Hashtable with containing all field values for the current user's user record</returns>
		/// <remarks>
		/// Added by gman3001 9/29/2004
		/// </remarks>
		public static Hashtable GetCurrentUserProfile(int PortalID)
		{
			Hashtable userSettings = new Hashtable();
			// Obtain all current User's Information in the rb_users table into a hash table, with field name as the key
			UsersDB accountSystem = new UsersDB();

			using (SqlDataReader dr = accountSystem.GetSingleUser(CurrentUser.Identity.Email, PortalID))
			{

				// Read first row from database
				if (dr.Read())
				{

					for (int i = 0; i < dr.FieldCount; i++)
					{
						userSettings.Add(dr.GetName(i), dr.GetValue(i).ToString());
					}
				}
				dr.Close();
			}
			return userSettings;
		}

		/// <summary>
		/// Massages existing DesktopTabs ArrayList a bit, 
		/// so that it can be serialized into an XPathDocument
		/// which can be used to retrieve tabs hierarchy data
		/// without hitting the database
		/// </summary>
		/// <returns>xpd</returns>
		/// <remarks>
		///  Jes1111
		/// </remarks>
		public XPathDocument GetDesktopTabsXml()
		{
			// make a new TabsBox, because we want the top level TabStripDetails
			// to be inside a TabsBox, for the serialization
			TabsBox menuGroup = new TabsBox();
			// create a MenuData object to hold the TabsBox
			MenuData md = new MenuData();
			// transfer DesktopTabs to menuGroup
			//			foreach (TabStripDetails t in this.DesktopTabs)
			//				menuGroup.Add(t);
			int tabCount = DesktopTabs.Count;

			for (int i = 0; i < tabCount; i++)
			{
				menuGroup.Add((TabStripDetails) DesktopTabs[i]);
			}
			// Put the TabsBox into its holder
			md.TabsBox = menuGroup;
			// Create a Type array for the MenuData object
			Type[] extraTypes = new Type[2];
			extraTypes[0] = typeof (TabsBox);
			extraTypes[1] = typeof (TabStripDetails);
			// serialize the MenuData object
			XmlSerializer serializer = new XmlSerializer(typeof (MenuData), extraTypes);

			using (TextWriter tw = new StringWriter())
			{
				XmlWriter xw = new XmlTextWriter(tw);
				serializer.Serialize(xw, md);
				// create the XPathDocument
				XPathDocument xpd = new XPathDocument(new XmlTextReader(new StringReader(tw.ToString())));
				return xpd;
			}
		}

		/// <summary>
		/// Get the ParentTabID of a certain Tab 06/11/2004 Rob Siera
		/// </summary>
		/// <returns></returns>
		public static int GetParentTabID(int tabID, ArrayList tabList)
		{
			TabStripDetails tmpTab;

			for (int i = 0; i < tabList.Count; i++)
			{
				tmpTab = (TabStripDetails) tabList[i];

				if (tmpTab.TabID == tabID)
					return tmpTab.ParentTabID;
			}
			throw new ArgumentOutOfRangeException("Tab", "Root not found");
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="PortalPath" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public static Hashtable GetPortalBaseSettings(string PortalPath)
		{
			Hashtable _baseSettings;

			if (!CurrentCache.Exists(Key.PortalBaseSettings()))
			{
				//Define base settings
				_baseSettings = new Hashtable();
				int _groupOrderBase;
				SettingItemGroup _Group;

				#region Theme Management

				_Group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
				_groupOrderBase = (int) SettingItemGroup.THEME_LAYOUT_SETTINGS;
				SettingItem Image = new SettingItem(new UploadedFileDataType(Path.WebPathCombine(Path.ApplicationRoot, PortalPath))); //StringDataType
				Image.Order = _groupOrderBase + 5;
				Image.Group = _Group;
				Image.EnglishName = "Logo";
				Image.Description = "Enter the name of logo file here. The logo will be searched in your portal dir. For the default portal is (~/_Rainbow).";
				_baseSettings.Add("SITESETTINGS_LOGO", Image);
				ArrayList layoutList = new LayoutManager(PortalPath).GetLayouts();
				SettingItem TabLayoutSetting = new SettingItem(new CustomListDataType(layoutList, strName, strName));
				TabLayoutSetting.Value = "Default";
				TabLayoutSetting.Order = _groupOrderBase + 10;
				TabLayoutSetting.Group = _Group;
				TabLayoutSetting.EnglishName = "Page layout";
				TabLayoutSetting.Description = "Specify the site level page layout here.";
				_baseSettings.Add("SITESETTINGS_PAGE_LAYOUT", TabLayoutSetting);
				ArrayList themeList = new ThemeManager(PortalPath).GetThemes();
				SettingItem Theme = new SettingItem(new CustomListDataType(themeList, strName, strName));
				Theme.Required = true;
				Theme.Order = _groupOrderBase + 15;
				Theme.Group = _Group;
				Theme.EnglishName = "Theme";
				Theme.Description = "Specify the site level theme here.";
				_baseSettings.Add("SITESETTINGS_THEME", Theme);
				SettingItem ThemeAlt = new SettingItem(new CustomListDataType(new ThemeManager(PortalPath).GetThemes(), strName, strName));
				ThemeAlt.Required = true;
				ThemeAlt.Order = _groupOrderBase + 20;
				ThemeAlt.Group = _Group;
				ThemeAlt.EnglishName = "Alternate theme";
				ThemeAlt.Description = "Specify the site level alternate theme here.";
				_baseSettings.Add("SITESETTINGS_ALT_THEME", ThemeAlt);
				// Jes1111 - 2004-08-06 - Zen support
				SettingItem AllowModuleCustomThemes = new SettingItem(new BooleanDataType());
				AllowModuleCustomThemes.Order = _groupOrderBase + 25;
				AllowModuleCustomThemes.Group = _Group;
				AllowModuleCustomThemes.Value = "False";
				AllowModuleCustomThemes.EnglishName = "Allow Module Custom Themes?";
				AllowModuleCustomThemes.Description = "Select to allow Custom Theme to be set on Modules.";
				_baseSettings.Add("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES", AllowModuleCustomThemes);

				#endregion

				#region Security/User Management

				_groupOrderBase = (int) SettingItemGroup.SECURITY_USER_SETTINGS;
				_Group = SettingItemGroup.SECURITY_USER_SETTINGS;
				// Show input for Portal Admins when using Windows Authenication and Multiportal
				// cisakson@yahoo.com 28.April.2003
				// This setting is removed in Global.asa for non-Windows authenticaton sites.
				SettingItem PortalAdmins = new SettingItem(new StringDataType());
				PortalAdmins.Order = _groupOrderBase + 5;
				PortalAdmins.Group = _Group;
				PortalAdmins.Value = ConfigurationSettings.AppSettings["ADAdministratorGroup"];
				PortalAdmins.Required = false;
				PortalAdmins.Description = "Show input for Portal Admins when using Windows Authenication and Multiportal";
				_baseSettings.Add("WindowsAdmins", PortalAdmins);
				// Allow new registrations?
				SettingItem AllowNewRegistrations = new SettingItem(new BooleanDataType());
				AllowNewRegistrations.Order = _groupOrderBase + 10;
				AllowNewRegistrations.Group = _Group;
				AllowNewRegistrations.Value = "True";
				AllowNewRegistrations.EnglishName = "Allow New Registrations?";
				AllowNewRegistrations.Description = "Check this to allow users register themselves. Leave blank for register through User Manager only.";
				_baseSettings.Add("SITESETTINGS_ALLOW_NEW_REGISTRATION", AllowNewRegistrations);
				// If we can allow identity edit (moved here fom user settings)
				SettingItem AllowEditUserID = new SettingItem(new BooleanDataType());
				AllowEditUserID.Group = _Group;
				AllowEditUserID.Order = _groupOrderBase + 11;
				AllowEditUserID.Value = "False";
				AllowEditUserID.EnglishName = "Allow user change the ID";
				AllowEditUserID.Description = "This option allow an admin set one user id to match a specific number.";
				_baseSettings.Add("SITESETTINGS_ALLOW_EDIT_USER_ID", AllowEditUserID);
				//MH: added dynamic load of registertypes depending on the  content in the DesktopModules/Register/ folder
				// Register
				Hashtable regPages = new Hashtable();

				foreach (string registerPage in Directory.GetFiles(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/DesktopModules/Register/"), "register*.ascx"))
				{
					string registerPageDisplayName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1, registerPage.LastIndexOf(".") - registerPage.LastIndexOf("\\") - 1);
					string registerPageName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1);
					regPages.Add(registerPageDisplayName, registerPageName.ToLower());
				}
				// Register Layout Setting
				SettingItem RegType = new SettingItem(new CustomListDataType(regPages, "Key", "Value"));
				RegType.Required = true;
				RegType.Value = "Register.ascx";
				RegType.EnglishName = "Register Type";
				RegType.Description = "Choose here how Register Page should look like.";
				RegType.Order = _groupOrderBase + 15;
				RegType.Group = _Group;
				_baseSettings.Add("SITESETTINGS_REGISTER_TYPE", RegType);
				//MH:end
				// Register Layout Setting module id reference by manu
				SettingItem RegModuleID = new SettingItem(new IntegerDataType());
				RegModuleID.Value = "0";
				RegModuleID.Required = true;
				RegModuleID.Order = _groupOrderBase + 16;
				RegModuleID.Group = _Group;
				RegModuleID.EnglishName = "Register Module ID";
				RegModuleID.Description = "Some custom registration may require additional settings, type here the ID of the module from where we should load settings (0= not used). Usually this module is added in an hidden area.";
				_baseSettings.Add("SITESETTINGS_REGISTER_MODULEID", RegModuleID);
				// Send mail on new registration to
				SettingItem OnRegisterSendTo = new SettingItem(new StringDataType());
				OnRegisterSendTo.Value = string.Empty;
				OnRegisterSendTo.Required = false;
				OnRegisterSendTo.Order = _groupOrderBase + 17;
				OnRegisterSendTo.Group = _Group;
				OnRegisterSendTo.EnglishName = "Send Mail To";
				OnRegisterSendTo.Description = "On new registration a mail will be send to the email address you provide here.";
				_baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_TO", OnRegisterSendTo);
				//Terms of service
				SettingItem TermsOfService = new SettingItem(new PortalUrlDataType());
				TermsOfService.Order = _groupOrderBase + 20;
				TermsOfService.Group = _Group;
				TermsOfService.EnglishName = "Terms file name";
				TermsOfService.Description = "Type here a file name used for showing terms and condition in each register page. Provide localized version adding _<culturename>. E.g. Terms.txt, will search for Terms.txt and for Terms_en-US.txt";
				_baseSettings.Add("SITESETTINGS_TERMS_OF_SERVICE", TermsOfService);

				try
				{
					//Country filter limits country list, leave blank for all
					ArrayList countryList = new ArrayList(CountryInfo.GetCountries(CountryTypes.AllCountries, CountryFields.DisplayName));
					countryList.Insert(0, new CountryInfo());
					SettingItem CountriesFilter = new SettingItem(new MultiSelectListDataType(countryList, "DisplayName", strName));
					CountriesFilter.Order = _groupOrderBase + 25;
					CountriesFilter.Group = _Group;
					CountriesFilter.EnglishName = "Allowed countries";
					CountriesFilter.Description = "Allowed countries limits country list in RegisterFull page, select 'World' for no filter.";
					_baseSettings.Add("SITESETTINGS_COUNTRY_FILTER", CountriesFilter);
				}

				catch (NullReferenceException ex)
				{
					ErrorHandler.HandleException(ex);
				}

				#endregion

				#region HTML Header Management

				_groupOrderBase = (int) SettingItemGroup.META_SETTINGS;
				_Group = SettingItemGroup.META_SETTINGS;
				// added: Jes1111 - page DOCTYPE setting
				SettingItem DocType = new SettingItem(new StringDataType());

				DocType.Order = _groupOrderBase + 5;

				DocType.Group = _Group;

				DocType.EnglishName = "DOCTYPE string";

				DocType.Description = "Allows you to enter a DOCTYPE string which will be inserted as the first line of the HTML output page (i.e. above the <html> element). Use this to force Quirks or Standards mode, particularly in IE. See <a href=\"http://gutfeldt.ch/matthias/articles/doctypeswitch/table.html\" target=\"_blank\">here</a> for details. NOTE: Rainbow.Zen requires a setting that guarantees Standards mode on all browsers.";

				DocType.Value = string.Empty;
				_baseSettings.Add("SITESETTINGS_DOCTYPE", DocType);
				//by John Mandia <john.mandia@whitelightsolutions.com>
				SettingItem TabTitle = new SettingItem(new StringDataType());
				TabTitle.Order = _groupOrderBase + 10;
				TabTitle.Group = _Group;
				TabTitle.EnglishName = "Page title";
				TabTitle.Description = "Allows you to enter a default tab / page title (Shows at the top of your browser).";
				_baseSettings.Add("SITESETTINGS_PAGE_TITLE", TabTitle);
				/*
				 * John Mandia: Removed This Setting. Now You can define specific Url Keywords via Tab Settings only. This is to speed up url building.
				 * 
				SettingItem TabUrlKeyword = new SettingItem(new StringDataType());
				TabUrlKeyword.Order = _groupOrderBase + 15;
				TabUrlKeyword.Group = _Group;
				TabUrlKeyword.Value = "Portal";
				TabUrlKeyword.EnglishName = "Keyword to Identify all pages";
				TabUrlKeyword.Description = "This setting is not fully implemented yet. It was to help with search engine optimisation by allowing you to specify a default keyword that would appear in your url."; 
				_baseSettings.Add("SITESETTINGS_PAGE_URL_KEYWORD", TabUrlKeyword);
				*/
				SettingItem TabMetaKeyWords = new SettingItem(new StringDataType());
				TabMetaKeyWords.Order = _groupOrderBase + 15;
				TabMetaKeyWords.Group = _Group;
				// john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want Meta Keywords; http://sourceforge.net/tracker/index.php?func=detail&aid=915614&group_id=66837&atid=515929
				TabMetaKeyWords.EnglishName = "Page keywords";
				TabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what your site is about.";
				_baseSettings.Add("SITESETTINGS_PAGE_META_KEYWORDS", TabMetaKeyWords);
				SettingItem TabMetaDescription = new SettingItem(new StringDataType());
				TabMetaDescription.Order = _groupOrderBase + 20;
				TabMetaDescription.Group = _Group;
				TabMetaDescription.EnglishName = "Page description";
				TabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a default description (Not too long though. 1 paragraph is enough) that describes your portal.";
				// john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want a defautl descripton
				_baseSettings.Add("SITESETTINGS_PAGE_META_DESCRIPTION", TabMetaDescription);
				SettingItem TabMetaEncoding = new SettingItem(new StringDataType());
				TabMetaEncoding.Order = _groupOrderBase + 25;
				TabMetaEncoding.Group = _Group;
				TabMetaEncoding.EnglishName = "Page encoding";
				TabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the default content type.";
				TabMetaEncoding.Value = "<META http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\" />";
				_baseSettings.Add("SITESETTINGS_PAGE_META_ENCODING", TabMetaEncoding);
				SettingItem TabMetaOther = new SettingItem(new StringDataType());
				TabMetaOther.Order = _groupOrderBase + 30;
				TabMetaOther.Group = _Group;
				TabMetaOther.EnglishName = "Default Additional Meta Tag Entries";
				TabMetaOther.Description = "This setting allows you to enter new tags into the Tab / Page's HEAD Tag. As an example we have added a portal tag to identify the version, but you could have a meta refresh tag or something else like a css reference instead.";
				TabMetaOther.Value = string.Empty;
				_baseSettings.Add("SITESETTINGS_PAGE_META_OTHERS", TabMetaOther);
				SettingItem TabKeyPhrase = new SettingItem(new StringDataType());
				TabKeyPhrase.Order = _groupOrderBase + 35;
				TabKeyPhrase.Group = _Group;
				TabKeyPhrase.EnglishName = "Default Page Keyphrase";
				TabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a common message for the entire portal e.g. Welcome to x portal! This can be used for search engine optimisation. It allows you to define a keyword rich phrase to be used throughout your portal.";
				TabKeyPhrase.Value = "Enter your default keyword rich Tab / Page phrase here. ";
				_baseSettings.Add("SITESETTINGS_PAGE_KEY_PHRASE", TabKeyPhrase);
				// added: Jes1111 - <body> element attributes setting
				SettingItem BodyAttributes = new SettingItem(new StringDataType());
				BodyAttributes.Order = _groupOrderBase + 45;
				BodyAttributes.Group = _Group;
				BodyAttributes.EnglishName = "&lt;body&gt; attributes";
				BodyAttributes.Description = "Allows you to enter a string which will be inserted within the <body> element, e.g. leftmargin=\"0\" bottommargin=\"0\", etc. NOTE: not advisable to use this to inject onload() function calls as there is a programmatic function for that. NOTE also that is your CSS is well sorted you should not need anything here.";
				BodyAttributes.Required = false;
				_baseSettings.Add("SITESETTINGS_BODYATTS", BodyAttributes);

				//end by John Mandia <john.mandia@whitelightsolutions.com>
				#endregion

				# region Language/Culture Management
				_groupOrderBase = (int) SettingItemGroup.CULTURE_SETTINGS;
				_Group = SettingItemGroup.CULTURE_SETTINGS;
				// Jes1111
				//			// Language List - defines list of languages specifically offered by this portal
				//			SettingItem LangList = new SettingItem(new StringDataType());
				//			LangList.Group = _Group;
				//			LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"];
				//			LangList.Required = true;
				//			_baseSettings.Add("LangList", LangList);
				//
				//			// Culture List - defines list of Specific cultures which correspond to LangList entries
				//			SettingItem CultureList = new SettingItem(new StringDataType());
				//			CultureList.Group = _Group;
				//			CultureList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"];
				//			CultureList.Required = true;
				//			_baseSettings.Add("CultureList", CultureList);
				//Uncomment it for testing
				//SettingItem LangList = new SettingItem(new LangListDataType());
				SettingItem LangList = new SettingItem(new StringDataType());
				LangList.Group = _Group;
				LangList.EnglishName = "Language list";
				LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; //"it=it-IT;en=en-US;"; 
				LangList.Required = false; // must be false... grid is not validable
				LangList.Description = "This is a list of the languages that the site will support,it's in format <UI-Culture>=<Formatting-Culture>.";
				_baseSettings.Add("SITESETTINGS_LANGLIST", LangList);
				//			// Attempt to match browser language list?
				//			SettingItem LangUser = new SettingItem(new BooleanDataType());
				//			LangUser.Group = _Group;
				//			LangUser.Value = "False";
				//			_baseSettings.Add("LangUser", LangUser);
				//
				//			// Lock UI to Content?
				//			SettingItem LangLock = new SettingItem(new BooleanDataType());
				//			LangLock.Group = _Group;
				//			LangLock.Value = "True";
				//			_baseSettings.Add("LangLock", LangLock);
				# endregion 

				#region Miscellaneous Settings

				_groupOrderBase = (int) SettingItemGroup.MISC_SETTINGS;
				_Group = SettingItemGroup.MISC_SETTINGS;
				// Show modified by summary on/off
				SettingItem ShowModifiedBy = new SettingItem(new BooleanDataType());
				ShowModifiedBy.Order = _groupOrderBase + 10;
				ShowModifiedBy.Group = _Group;
				ShowModifiedBy.Value = "False";
				ShowModifiedBy.EnglishName = "Show modified by";
				ShowModifiedBy.Description = "Check to show by whom the module is last modified.";
				_baseSettings.Add("SITESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy);
				// Default Editor Configuration used for new modules and workflow modules. jviladiu@portalServices.net 13/07/2004
				SettingItem DefaultEditor = new SettingItem(new HtmlEditorDataType());
				DefaultEditor.Order = _groupOrderBase + 20;
				DefaultEditor.Group = _Group;
				DefaultEditor.Value = "FreeTextBox";
				DefaultEditor.EnglishName = "Default Editor";
				DefaultEditor.Description = "This Editor is used by workflow and is the default for new modules.";
				_baseSettings.Add("SITESETTINGS_DEFAULT_EDITOR", DefaultEditor);
				// Default Editor Width. jviladiu@portalServices.net 13/07/2004
				SettingItem DefaultWidth = new SettingItem(new IntegerDataType());
				DefaultWidth.Order = _groupOrderBase + 25;
				DefaultWidth.Group = _Group;
				DefaultWidth.Value = "700";
				DefaultWidth.EnglishName = "Editor Width";
				DefaultWidth.Description = "Default Editor Width";
				_baseSettings.Add("SITESETTINGS_EDITOR_WIDTH", DefaultWidth);
				// Default Editor Height. jviladiu@portalServices.net 13/07/2004
				SettingItem DefaultHeight = new SettingItem(new IntegerDataType());
				DefaultHeight.Order = _groupOrderBase + 30;
				DefaultHeight.Group = _Group;
				DefaultHeight.Value = "400";
				DefaultHeight.EnglishName = "Editor Height";
				DefaultHeight.Description = "Default Editor Height";
				_baseSettings.Add("SITESETTINGS_EDITOR_HEIGHT", DefaultHeight);
				//Show Upload (Active up editor only). jviladiu@portalServices.net 13/07/2004
				SettingItem ShowUpload = new SettingItem(new BooleanDataType());
				ShowUpload.Value = "true";
				ShowUpload.Order = _groupOrderBase + 35;
				ShowUpload.Group = _Group;
				ShowUpload.EnglishName = "Upload?";
				ShowUpload.Description = "Only used if Editor is ActiveUp HtmlTextBox";
				_baseSettings.Add("SITESETTINGS_SHOWUPLOAD", ShowUpload);
				// Default Image Folder. jviladiu@portalServices.net 29/07/2004
				SettingItem DefaultImageFolder = new SettingItem(new FolderDataType(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + PortalPath + "/images"), "default"));
				DefaultImageFolder.Order = _groupOrderBase + 40;
				DefaultImageFolder.Group = _Group;
				DefaultImageFolder.Value = "default";
				DefaultImageFolder.EnglishName = "Default Image Folder";
				DefaultImageFolder.Description = "Set the default image folder used by Current Editor";
				_baseSettings.Add("SITESETTINGS_DEFAULT_IMAGE_FOLDER", DefaultImageFolder);
				_groupOrderBase = (int) SettingItemGroup.MISC_SETTINGS;
				_Group = SettingItemGroup.MISC_SETTINGS;
				// Show module arrows to an administrator
				SettingItem ShowModuleArrows = new SettingItem(new BooleanDataType());
				ShowModuleArrows.Order = _groupOrderBase + 50;
				ShowModuleArrows.Group = _Group;
				ShowModuleArrows.Value = "True";
				ShowModuleArrows.EnglishName = "Show module arrows";
				ShowModuleArrows.Description = "Check to show the arrows in the module title to move modules.";
				_baseSettings.Add("SITESETTINGS_SHOW_MODULE_ARROWS", ShowModuleArrows);

				#endregion

				using (CacheDependency settingDependancies = new CacheDependency(null, new string[] {Key.ThemeList(ThemeManager.Path)}))
				{
					CurrentCache.Insert(Key.PortalBaseSettings(), _baseSettings, settingDependancies);
				}
			}

			else
				_baseSettings = (Hashtable) CurrentCache.Get(Key.PortalBaseSettings());
			return _baseSettings;
		}

		/// <summary>
		/// The PortalSettings.GetPortalSettings Method returns a hashtable of
		/// custom Portal specific settings from the database. This method is
		/// used by Portals to access misc settings.
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="_baseSettings"></param>
		/// <returns></returns>
		public static Hashtable GetPortalCustomSettings(int portalID, Hashtable _baseSettings)
		{

			if (!CurrentCache.Exists(Key.PortalSettings()))
			{
				// Get Settings for this Portal from the database
				Hashtable _settings = new Hashtable();

				// Create Instance of Connection and Command Object
				using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
				{

					using (SqlCommand myCommand = new SqlCommand("rb_GetPortalCustomSettings", myConnection))
					{
						// Mark the Command as a SPROC
						myCommand.CommandType = CommandType.StoredProcedure;
						// Add Parameters to SPROC
						SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
						parameterportalID.Value = portalID;
						myCommand.Parameters.Add(parameterportalID);
						// Execute the command
						myConnection.Open();
						SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

						try
						{

							while (dr.Read())
								_settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
						}

						finally
						{
							dr.Close(); //by Manu, fixed bug 807858
							myConnection.Close();
						}
					}
				}

				foreach (string key in _baseSettings.Keys)
				{

					if (_settings[key] != null)
					{
						SettingItem s = ((SettingItem) _baseSettings[key]);

						if (_settings[key].ToString() != string.Empty)
							s.Value = _settings[key].ToString();
					}
				}
				CurrentCache.Insert(Key.PortalSettings(), _baseSettings);
			}

			else
				_baseSettings = (Hashtable) CurrentCache.Get(Key.PortalSettings());
			return _baseSettings;
		}

		/// <summary>
		/// Get the proxy parameters as configured in web.config by Phillo 22/01/2003
		/// </summary>
		/// <returns></returns>
		public static WebProxy GetProxy()
		{

			if (ConfigurationSettings.AppSettings["ProxyServer"].Length > 0)
			{
				WebProxy myProxy = new WebProxy();
				NetworkCredential myCredential = new NetworkCredential();
				myCredential.Domain = ConfigurationSettings.AppSettings["ProxyDomain"];
				myCredential.UserName = ConfigurationSettings.AppSettings["ProxyUserID"];
				myCredential.Password = ConfigurationSettings.AppSettings["ProxyPassword"];
				myProxy.Credentials = myCredential;
				myProxy.Address = new Uri(ConfigurationSettings.AppSettings["ProxyServer"]);
				return (myProxy);
			}

			else
				return (null);
		}

		/// <summary>
		/// The get tab root should get the first level tab:
		/// <pre>
		///	+ Root
		///		+ Tab1
		///			+ SubTab1		-> returns Tab1
		///		+ Tab2
		///			+ SubTab2		-> returns Tab2
		///				+ SubTab2.1 -> returns Tab2
		///	</pre>
		/// </summary>
		/// <param name="parentTabID"></param>
		/// <param name="tabList"></param>
		/// <returns></returns>
		public static TabStripDetails GetRootTab(int parentTabID, ArrayList tabList)
		{
			//Changes Indah Fuldner 25.04.2003 (With assumtion that the rootlevel tab has ParentTabID = 0)
			//Search for the root tab in current array
			TabStripDetails rootTab;

			for (int i = 0; i < tabList.Count; i++)
			{
				rootTab = (TabStripDetails) tabList[i];

				// return rootTab;
				if (rootTab.TabID == parentTabID)
				{
					parentTabID = rootTab.ParentTabID;
					string parentName = rootTab.TabName;

					if (parentTabID != 0)
						i = -1;

					else
						return rootTab;
				}
			}
			//End Indah Fuldner
			throw new ArgumentOutOfRangeException("Tab", "Root not found");
		}

		/// <summary>
		/// The GetRootTab should get the first level tab:
		/// <pre>
		///	+ Root
		///		+ Tab1
		///			+ SubTab1		-> returns Tab1
		///		+ Tab2
		///			+ SubTab2		-> returns Tab2
		///				+ SubTab2.1 -> returns Tab2
		///	</pre>
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="tabList"></param>
		/// <returns></returns>
		public static TabStripDetails GetRootTab(TabSettings tab, ArrayList tabList)
		{
			return GetRootTab(tab.TabID, tabList);
		}

		/// <summary>
		/// Get resource
		/// </summary>
		/// <param name="resourceID"></param>
		/// <returns></returns>
		public static string GetStringResource(string resourceID)
		{
			string res = null;
			Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceID);
			StreamReader sr = null;

			try
			{
				sr = new StreamReader(st);
				res = sr.ReadToEnd();
			}

			catch (Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Debug, "Resource not found: " + resourceID, ex);
				throw new ArgumentNullException("Resource not found: " + resourceID);
			}

			finally
			{

				if (sr != null)
					sr.Close();

				if (st != null)
					st.Close();
			}
			return res;
		}

		/// <summary>
		/// Get resource
		/// </summary>
		/// <param name="resourceID"></param>
		/// <param name="_localize"></param>
		/// <returns></returns>
		public static string GetStringResource(string resourceID, string[] _localize)
		{
			string res = GetStringResource(resourceID);

			for (int i = 0; i <= _localize.GetUpperBound(0); i++)
			{
				string thisparam = "%" + i.ToString() + "%";
				res = res.Replace(thisparam, Localize.GetString(_localize[i]));
			}
			return res;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		public string GetTermsOfService()
		{
			string termsOfService = string.Empty;

			//Verify if we have to show conditions
			if (CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"] != null && CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString() != string.Empty)
			{
				//				// Attempt to load the required text
				//				Rainbow.UI.DataTypes.PortalUrlDataType pt = new Rainbow.UI.DataTypes.PortalUrlDataType();
				//				pt.Value = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
				//				string terms = HttpContext.Current.Server.MapPath(pt.FullPath);
				//				
				//				//Try to get localized version
				//				string localized_terms;
				//				localized_terms = terms.Replace(".", "_" + Esperantus.Localize.GetCurrentUINeutralCultureName() + ".");
				//				if (System.IO.File.Exists(localized_terms))
				//					terms = localized_terms;
				//Fix by Joerg Szepan - jszepan 
				// http://sourceforge.net/tracker/index.php?func=detail&aid=852071&group_id=66837&atid=515929
				// Wrong Terms-File if Dot in Mappath
				// Attempt to load the required text
				string terms;
				terms = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
				//Try to get localized version
				string localized_terms;
				localized_terms = terms.Replace(".", "_" + Localize.GetCurrentUINeutralCultureName() + ".");
				PortalUrlDataType pt = new PortalUrlDataType();
				pt.Value = localized_terms;

				if (File.Exists(HttpContext.Current.Server.MapPath(pt.FullPath)))
					terms = localized_terms;
				pt.Value = terms;
				terms = HttpContext.Current.Server.MapPath(pt.FullPath);

				//Load conditions
				if (File.Exists(terms))
				{

					//Try to open file
					using (StreamReader s = new StreamReader(terms, Encoding.Default))
					{
						//Get the text of the conditions
						termsOfService = s.ReadToEnd();
						// Close Streamreader
						s.Close();
					}
				}

				else
				{
					//If load fails use default
					termsOfService = "'" + terms + "' not found!";
				}
			}
			//end Fix by Joerg Szepan - jszepan 
			return termsOfService;
		}

		/// <summary>
		/// The UpdatePortalSetting Method updates a single module setting
		/// in the PortalSettings database table.
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void UpdatePortalSetting(int portalID, String key, String value)
		{

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
			{

				using (SqlCommand myCommand = new SqlCommand("rb_UpdatePortalSetting", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
					parameterportalID.Value = portalID;
					myCommand.Parameters.Add(parameterportalID);
					SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
					parameterKey.Value = key;
					myCommand.Parameters.Add(parameterKey);
					SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
					parameterValue.Value = value;
					myCommand.Parameters.Add(parameterValue);
					// Execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}

					finally
					{
						myConnection.Close();
					}
				}
			}
			CurrentCache.Remove(Key.PortalSettings());
		}

		/// <summary>
		/// Get languages list from Portaldb
		/// </summary>
		/// <param name="portalAlias"></param>
		/// <returns></returns>
		private string GetLanguageList(string portalAlias)
		{
			string langlist = string.Empty;

			if (!CurrentCache.Exists(Key.LanguageList()))
			{

				// Create Instance of Connection and Command Object
				using (SqlConnection myConnection = PortalSettings.SqlConnectionString)
				{

					using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsLangList", myConnection))
					{
						// Mark the Command as a SPROC
						myCommand.CommandType = CommandType.StoredProcedure;
						// Add Parameters to SPROC
						SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
						parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
						myCommand.Parameters.Add(parameterPortalAlias);
						// Open the database connection and execute the command
						myConnection.Open();

						try
						{
							//Better null check here by Manu
							object tmp = myCommand.ExecuteScalar();

							if (tmp != null) langlist = tmp.ToString();
						}

						catch (Exception ex)
						{
							LogHelper.Logger.Log(LogLevel.Warn, "Get languages from db", ex);
						}

						finally
						{
							myConnection.Close();
						}
					}
				}

				if (langlist.Length == 0)
					langlist = ConfigurationSettings.AppSettings["DefaultLanguage"]; //default
				CurrentCache.Insert(Key.LanguageList(), langlist);
			}

			else
				langlist = (string) CurrentCache.Get(Key.LanguageList());
			return langlist;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="myTab" type="Rainbow.Configuration.TabStripDetails">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="writer" type="System.Xml.XmlTextWriter">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		private void RecursePortalTabsXml(TabStripDetails myTab, XmlTextWriter writer)
		{
			TabsBox children = myTab.Tabs;
			bool _groupElementWritten = false;

			for (int child = 0; child < children.Count; child++)
			{
				TabStripDetails mySubTab = (TabStripDetails) children[child];

				//if ( mySubTab.ParentTabID == myTab.TabID && PortalSecurity.IsInRoles(myTab.AuthorizedRoles) )
				if (mySubTab.ParentTabID == myTab.TabID)
				{

					if (!_groupElementWritten)
					{
						writer.WriteStartElement("MenuGroup"); // start MenuGroup element
						_groupElementWritten = true;
					}
					writer.WriteStartElement("MenuItem"); // start MenuItem element
					writer.WriteAttributeString("ParentTabId", mySubTab.ParentTabID.ToString());
					writer.WriteAttributeString("Label", mySubTab.TabName);
					writer.WriteAttributeString("TabOrder", mySubTab.TabOrder.ToString());
					writer.WriteAttributeString("TabIndex", mySubTab.TabIndex.ToString());
					writer.WriteAttributeString("TabLayout", mySubTab.TabLayout);
					writer.WriteAttributeString("AuthRoles", mySubTab.AuthorizedRoles);
					writer.WriteAttributeString("ID", mySubTab.TabID.ToString());
					writer.WriteAttributeString("URL", HttpUrlBuilder.BuildUrl(string.Concat("~/", mySubTab.TabName, ".aspx"), mySubTab.TabID, 0, null, string.Empty, this.PortalAlias, "hello/goodbye"));
					RecursePortalTabsXml(mySubTab, writer);
					writer.WriteEndElement(); // end MenuItem element
				}
			}

			if (_groupElementWritten)
				writer.WriteEndElement(); // end MenuGroup element
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="mID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		private void setActiveModuleCookie(int mID)
		{
			HttpCookie cookie;
			DateTime time;
			TimeSpan span;
			cookie = new HttpCookie("ActiveModule", mID.ToString());
			time = DateTime.Now;
			span = new TimeSpan(0, 2, 0, 0, 0); // 120 minutes to expire
			cookie.Expires = time.Add(span);
			HttpContext.Current.Response.AppendCookie(cookie);
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
		public int ActiveModule
		{
			get
			{

				if (HttpContext.Current.Request.Params["mID"] != null)
				{
					setActiveModuleCookie(int.Parse(HttpContext.Current.Request.Params["mID"]));
					return int.Parse(HttpContext.Current.Request.Params["mID"]);
				}

				if (HttpContext.Current.Request.Cookies["ActiveModule"] != null)
					return int.Parse(HttpContext.Current.Request.Cookies["ActiveModule"].Value);
				return 0;
			}
			set { setActiveModuleCookie(value); }
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
		public static string ADUserName
		{
			get
			{
				string aDUserName = ConfigurationSettings.AppSettings["ADUserName"];

				if (aDUserName == null)
					return string.Empty;

				else
					return aDUserName;
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
		public static string ADUserPassword
		{
			get
			{
				string aDUserPassword = ConfigurationSettings.AppSettings["ADUserPassword"];

				if (aDUserPassword == null)
					return string.Empty;

				else
					return aDUserPassword;
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
		public bool AllowEditUserID
		{
			get { return (CustomSettings["SITESETTINGS_ALLOW_EDIT_USER_ID"] == null) ? false : bool.Parse(CustomSettings["SITESETTINGS_ALLOW_EDIT_USER_ID"].ToString()); }
		}

		/// <summary>
		/// ApplicationPath, Application dependent.
		/// Used by newsletter. Needed if you want to reference a page
		/// from an external resource (an email for example)
		/// Since it is common for all portals is declared as static.
		/// </summary>
		[Obsolete("Please use Rainbow.Settings.Path.ApplicationFullPath")]
		public static string ApplicationFullPath
		{
			get { return Path.ApplicationFullPath; }
		}

		/// <summary>
		/// ApplicationPath, Application dependent relative Application Path.
		/// Base dir for all portal code
		/// Since it is common for all portals is declared as static
		/// </summary>
		[Obsolete("Please use Rainbow.Settings.Path.ApplicationRoot")]
		public static string ApplicationPath
		{
			get { return Path.ApplicationRoot; }
		}

		/// <summary>
		/// ApplicationPhisicalPath.
		/// File system property
		/// </summary>
		[Obsolete("Please use Rainbow.Settings.Path.ApplicationPhysicalPath")]
		public static string ApplicationPhisicalPath
		{
			get { return Path.ApplicationPhysicalPath; }
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
		public static int CodeVersion
		{
			get
			{

				if (HttpContext.Current.Application["CodeVersion"] == null)
				{
					FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
					HttpContext.Current.Application.Lock();
					HttpContext.Current.Application["CodeVersion"] = f.FilePrivatePart;
					HttpContext.Current.Application.UnLock();
				}
				return (int) HttpContext.Current.Application["CodeVersion"];
			}
		}

		/// <summary>
		/// Current Layout
		/// </summary>
		public string CurrentLayout
		{
			get
			{

				//Patch for possible .NET framework bug
				//if returned an empy string caused an endless loop
				//Manu version 
				if (_currentLayout != string.Empty && _currentLayout != null)
					return _currentLayout;

				else
					return "Default";
			}
			set { _currentLayout = value; }
		}

		/// <summary>
		/// CurrentUser
		/// </summary>
		public static RainbowPrincipal CurrentUser
		{
			get
			{
				RainbowPrincipal r;

				if (HttpContext.Current.User is RainbowPrincipal)
					r = (RainbowPrincipal) HttpContext.Current.User;

				else
					r = new RainbowPrincipal(HttpContext.Current.User.Identity, null);
				return r;
			}
			set { HttpContext.Current.User = value; }
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
		public static int DatabaseVersion
		{
			//by Manu 16/10/2003
			//Added 2 mods:
			//1) Rbversion is created if it is missed.
			//   This is expecially good for empty databases.
			//   Be aware that this can break compatibility with 1613 version
			//2) Connection problems are thown immediately as errors.
			get
			{
				//Caches dbversion
				int curVersion = 0;

				if (HttpContext.Current.Application["DatabaseVersion"] == null)
				{

					try
					{
						//Create rbversion if it is missing
						string createRbVersions =
							"IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rb_Versions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
							"CREATE TABLE [rb_Versions] (" +
							"[Release] [int] NOT NULL , " +
							"[Version] [nvarchar] (50) NULL , " +
							"[ReleaseDate] [datetime] NULL " +
							") ON [PRIMARY]"
							;
						DBHelper.ExeSQL(createRbVersions);
					}

					catch (SqlException ex)
					{
						ErrorHandler.HandleException("If this fails most likely cannot connect to db or no permission", ex);
						//If this fails most likely cannot connect to db or no permission
						throw;
					}
					object version = DBHelper.ExecuteSQLScalar("SELECT TOP 1 Release FROM rb_Versions ORDER BY Release DESC");

					if (version != null)
						curVersion = Int32.Parse(version.ToString());

					else
					{
						curVersion = 1110;
						// TODO: This should be the best place
						// where run the codefor empty db
					}
					HttpContext.Current.Application.Lock();
					HttpContext.Current.Application["DatabaseVersion"] = curVersion;
					HttpContext.Current.Application.UnLock();
				}
				return (int) HttpContext.Current.Application["DatabaseVersion"];
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
		public static bool EnableADUser
		{
			get
			{
				string enableADUser = ConfigurationSettings.AppSettings["EnableADUser"];

				if (enableADUser == null)
					return false;

				else
					return bool.Parse(enableADUser);
			}
		}

		/// <summary>
		/// This static string fetches the portal's alias either via querystring, cookie or domain and returns it
		/// </summary>
		[Obsolete("Use Rainbow.Settings")]
		public static string GetPortalUniqueID
		{
			get
			{

				//The Alias parameter can be used to switch portals
				//Check for Alias first
				//Try to get portal alias from querystring
				if (HttpContext.Current.Request.Params["Alias"] != null)
				{
					//Alias is in the QueryString
					return HttpContext.Current.Request.Params["Alias"];
				}

				else if (HttpContext.Current.Request.Cookies["PortalAlias"] != null)
				{
					//If we have a Cookie, but no Param, then use cookie value
					return HttpContext.Current.Request.Cookies["PortalAlias"].Value;
				}

				else //Is the Domain a valid alias?
				{
					//parse the domain name from the URL (ie. www.domainname.com ) 
					string domainName = HttpContext.Current.Request.Url.Host.ToLower();

					//domainName = "www.mydomain.com";   
					//Remove www. from beginning of domain name (by DarkLight 10/01/2003)
					//					if (bool.Parse(ConfigurationSettings.AppSettings["RemoveWWW"]) && domainName.StartsWith("www.")) 
					//						domainName = domainName.Substring(domainName.IndexOf(@"."),(domainName.Length - domainName.IndexOf(@"."))); 
					if (bool.Parse(ConfigurationSettings.AppSettings["RemoveWWW"]) && domainName.StartsWith("www."))

						domainName = domainName.Substring(4, (domainName.Length - 4));

					//domainName = "mydomain.com";  
					//Remove trailing domain name .xx
					if (bool.Parse(ConfigurationSettings.AppSettings["IgnoreFirstDomain"]) && (domainName.LastIndexOf(@".") > 0))

						domainName = domainName.Substring(0, domainName.LastIndexOf(@"."));
					//domainName = "www.mydomain" or domainName = "mydomain"
					return domainName;
				}
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
		public static bool IsMonitoringEnabled
		{
			get { return enableMonitoring; }
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
		public CultureInfo PortalContentLanguage
		{
			get { return Thread.CurrentThread.CurrentUICulture; }
			set { Thread.CurrentThread.CurrentUICulture = value; }
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
		public CultureInfo PortalDataFormattingCulture
		{
			get { return Thread.CurrentThread.CurrentCulture; }
			set { Thread.CurrentThread.CurrentCulture = value; }
		}

		/// <summary>
		/// PortalPath.
		/// Base dir for all portal data, relative to root web dir.
		/// </summary>
		public string PortalFullPath
		{
			get
			{
				string x = Path.WebPathCombine(_portalPathPrefix, _portalPath);

				//(_portalPathPrefix + _portalPath).Replace("//", "/");
				if (x == "/") return string.Empty;
				return x;
			}
			set
			{

				if (value.StartsWith(_portalPathPrefix))
					_portalPath = value.Substring(_portalPathPrefix.Length);

				else
					_portalPath = value;
			}
		}

		/// <summary>
		/// PortalLayoutPath is the full path in which all Layout files are
		/// </summary>
		public string PortalLayoutPath
		{
			get
			{
				string ThisLayoutPath = this.CurrentLayout;
				string customLayout = string.Empty;

				// Thierry (Tiptopweb), 4 July 2003, switch to custom Layout
				if (this.ActiveTab.CustomSettings[strCustomLayout] != null && this.ActiveTab.CustomSettings[strCustomLayout].ToString().Length > 0)
					customLayout = ActiveTab.CustomSettings[strCustomLayout].ToString();

				if (customLayout != string.Empty)
				{
					// we have a custom Layout
					ThisLayoutPath = customLayout;
				}

				// Try to get layout from querystring
				if (HttpContext.Current != null && HttpContext.Current.Request.Params["Layout"] != null)
					ThisLayoutPath = HttpContext.Current.Request.Params["Layout"];
				// yiming, 18 Aug 2003, get layout from portalWebPath, if no, then WebPath
				LayoutManager layoutManager = new LayoutManager(PortalPath);

				if (Directory.Exists(layoutManager.PortalLayoutPath + "/" + ThisLayoutPath + "/"))
					return layoutManager.PortalWebPath + "/" + ThisLayoutPath + "/";

				else
					return LayoutManager.WebPath + "/" + ThisLayoutPath + "/";
				// end yiming, 18 Aug 2003
				//				//by manu
				//				string layoutManagerPath = System.IO.Path.Combine(layoutManager.PortalLayoutPath, ThisLayoutPath);
				//
				//				if (Directory.Exists(layoutManagerPath))                   
				//					return Rainbow.Settings.Path.WebPathCombine(layoutManager.PortalWebPath, ThisLayoutPath);
				//				else
				//					return Rainbow.Settings.Path.WebPathCombine(LayoutManager.WebPath, ThisLayoutPath);
			}
		}

		/// <summary>
		/// PortalPath.
		/// Base dir for all portal data, relative to application
		/// </summary>
		public string PortalPath
		{
			get { return _portalPath; }
			set
			{
				_portalPath = value;
				//				//by manu
				//				//be sure it starts with "/"
				//				if (_portalPath.Length > 0 && !_portalPath.StartsWith("/"))
				//					_portalPath = Rainbow.Settings.Path.WebPathCombine("/", _portalPath);
			}
		}

		// added Thierry (tiptopweb) 12 Apr 2003
		/// <summary>
		/// PortalSecurePath.
		/// Base dir for SSL
		/// </summary>
		public string PortalSecurePath
		{
			get
			{

				if (_portalSecurePath == null)
				{

					if (ConfigurationSettings.AppSettings["PortalSecureDirectory"] != null)
						this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003

					else
						this.PortalSecurePath = Path.WebPathCombine(Path.ApplicationRoot, "ECommerce/Secure"); //by Manu, a default
					LogHelper.Logger.Log(LogLevel.Info, this.PortalSecurePath);
				}
				return _portalSecurePath;
			}
			set { _portalSecurePath = value; }
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
		public XmlDocument PortalTabsXml
		{
			get
			{

				using (StringWriter sw = new StringWriter())
				{
					XmlTextWriter writer = new XmlTextWriter(sw);
					writer.Formatting = Formatting.None;
					writer.WriteStartDocument(true);
					writer.WriteStartElement("MenuData"); // start MenuData element
					writer.WriteStartElement("MenuGroup"); // start top MenuGroup element

					for (int i = 0; i < this.DesktopTabs.Count; i++)
					{
						TabStripDetails myTab = (TabStripDetails) this.DesktopTabs[i];

						//if ( myTab.ParentTabID == 0 && PortalSecurity.IsInRoles(myTab.AuthorizedRoles) )
						if (myTab.ParentTabID == 0)
						{
							writer.WriteStartElement("MenuItem"); // start MenuItem element
							writer.WriteAttributeString("ParentTabId", myTab.ParentTabID.ToString());
							writer.WriteAttributeString("Label", myTab.TabName);
							writer.WriteAttributeString("TabOrder", myTab.TabOrder.ToString());
							writer.WriteAttributeString("TabIndex", myTab.TabIndex.ToString());
							writer.WriteAttributeString("TabLayout", myTab.TabLayout);
							writer.WriteAttributeString("AuthRoles", myTab.AuthorizedRoles);
							writer.WriteAttributeString("ID", myTab.TabID.ToString());
							//writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",myTab.TabName,".aspx"),myTab.TabID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
							RecursePortalTabsXml(myTab, writer);
							writer.WriteEndElement(); // end MenuItem element
						}
					}
					writer.WriteEndElement(); // end top MenuGroup element
					writer.WriteEndElement(); // end MenuData element
					writer.Flush();
					_portalTabsXml = new XmlDocument();
					_portalTabsXml.LoadXml(sw.ToString());
					writer.Close();
				}
				return _portalTabsXml;
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
		public CultureInfo PortalUILanguage
		{
			get { return Thread.CurrentThread.CurrentUICulture; }
			set { Thread.CurrentThread.CurrentUICulture = value; }
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
		public static string ProductVersion
		{
			get
			{

				if (HttpContext.Current.Application["ProductVersion"] == null)
				{
					FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
					HttpContext.Current.Application.Lock();
					HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
					HttpContext.Current.Application.UnLock();
				}
				return (string) HttpContext.Current.Application["ProductVersion"];
			}
		}

		/// <summary>
		/// SmtpServer
		/// </summary>
		[Obsolete("Please use Rainbow.Settings.Portal.SmtpServer")]
		public static string SmtpServer
		{
			get { return Portal.SmtpServer; }
		}

		/// <summary>
		/// Database connection
		/// </summary>
		public static SqlConnection SqlConnectionString
		{
			get { return new SqlConnection(Portal.ConnectionString); }
		}

		/// <summary>
		/// If true all users will be loaded from portal 0 instance
		/// </summary>
		public static bool UseSingleUserBase
		{
			get { return bool.Parse(ConfigurationSettings.AppSettings["UseSingleUserBase"].ToString()); }
		}
		# region Culture/Language Settings
		# endregion
	}

	/// <summary>
	/// This is a temporary class to support DesktopTabsXml.
	/// Since PortalSettings.DesktopTab is not directly serializable,
	/// GetDesktopTabsXml uses this class to get the job done.
	/// 
	/// </summary>
	/// <remarks>
	/// Jes1111
	/// </remarks>
	public class MenuData
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlElement(ElementName="MenuGroup")] public object TabsBox;
	}
	//	public enum RegisterType : int
	//	{
	//		Simple = 0,
	//		Full = 1
	//	}
}