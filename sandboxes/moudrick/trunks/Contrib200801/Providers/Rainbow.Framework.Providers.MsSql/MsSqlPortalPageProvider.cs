using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Data;
using Rainbow.Framework.DataTypes; //BooleanDataType, StringDataType, CustomListDataType <- ListDataType <-- BaseDataType
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Providers.MsSql
{
    ///<summary>
    /// This is interface class for get module settings values 
    /// from appropriate persistence localtion
    ///</summary>
    public class MsSqlPortalPageProvider : PortalPageProvider
    {
        const string strPortalID = "@PortalID";
        const string strPageID = "@PageID";

        protected static Portal PortalSettings
        {
            get
            {
                return PortalProvider.Instance.CurrentPortal;
            }
        }

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <param name="authorizedRoles">The authorized roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="mobilePageName">Name of the mobile page.</param>
        /// <returns></returns>
        public override int AddPage(int portalID,
                                    int parentPageID,
                                    string pageName,
                                    int pageOrder,
                                    string authorizedRoles,
                                    bool showMobile,
                                    string mobilePageName)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_AddTab", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;


                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);

                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Value = parentPageID;
                    myCommand.Parameters.Add(parameterParentPageID);

                    //Fixes a missing tab name
                    if (pageName == null || pageName.Length == 0) pageName = "New Page";
                    SqlParameter parameterTabName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50);

                    //Fixes tab name to long
                    if (pageName.Length > 50) parameterTabName.Value = pageName.Substring(0, 49);
                    else parameterTabName.Value = pageName;
                    myCommand.Parameters.Add(parameterTabName);

                    SqlParameter parameterTabOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterTabOrder.Value = pageOrder;
                    myCommand.Parameters.Add(parameterTabOrder);

                    SqlParameter parameterAuthRoles = new SqlParameter("@AuthorizedRoles", SqlDbType.NVarChar, 256);
                    parameterAuthRoles.Value = authorizedRoles;
                    myCommand.Parameters.Add(parameterAuthRoles);

                    SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);

                    SqlParameter parameterMobileTabName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobileTabName.Value = mobilePageName;
                    myCommand.Parameters.Add(parameterMobileTabName);

                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageID);

                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }

                    finally
                    {
                        myConnection.Close();
                    }
                    return (int)parameterPageID.Value;
                }
            }
        }

        /// <summary>
        /// The DeletePage method deletes the selected tab from the portal.<br/>
        /// DeletePage Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public override void DeletePage(int pageID)
        {
            //TODO: [moudrick] catch SqlException to throw appropriate Rainbow exception
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_DeleteTab", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    // Open the database connection and execute the command
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
        }

        // New const for new method AddPage defaults
        // Mike Stone 30/12/2004

        /// <summary>
        /// UpdatePage Method<br/>
        /// UpdatePage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <param name="authorizedRoles">The authorized roles.</param>
        /// <param name="mobilePageName">Name of the mobile page.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        public override void UpdatePage(int portalID,
                                        int pageID,
                                        int parentPageID,
                                        string pageName,
                                        int pageOrder,
                                        string authorizedRoles,
                                        string mobilePageName,
                                        bool showMobile)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection connection = DBHelper.SqlConnection)
            {
                using (SqlCommand command = new SqlCommand("rb_UpdateTab", connection))
                {
                    // Mark the Command as a SPROC
                    command.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    command.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    command.Parameters.Add(parameterPageID);
                    SqlParameter parameterParentPageID =
                        new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Value = parentPageID;
                    command.Parameters.Add(parameterParentPageID);

                    //Fixes a missing tab name
                    if (pageName == null || pageName.Length == 0)
                    {
                        pageName = "&nbsp;";
                    }
                    SqlParameter parameterTabName =
                        new SqlParameter("@PageName", SqlDbType.NVarChar, 50);

                    if (pageName.Length > 50)
                    {
                        parameterTabName.Value = pageName.Substring(0, 49);
                    }

                    else
                    {
                        parameterTabName.Value = pageName;
                    }
                    command.Parameters.Add(parameterTabName);
                    SqlParameter parameterTabOrder =
                        new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterTabOrder.Value = pageOrder;
                    command.Parameters.Add(parameterTabOrder);
                    SqlParameter parameterAuthRoles =
                        new SqlParameter("@AuthorizedRoles", SqlDbType.NVarChar, 256);
                    parameterAuthRoles.Value = authorizedRoles;
                    command.Parameters.Add(parameterAuthRoles);
                    SqlParameter parameterMobileTabName =
                        new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobileTabName.Value = mobilePageName;
                    command.Parameters.Add(parameterMobileTabName);
                    SqlParameter parameterShowMobile =
                        new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    command.Parameters.Add(parameterShowMobile);
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }

                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// The PageSettings.GetPageCustomSettings Method returns a hashtable of
        /// custom Page specific settings from the database.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        protected override Hashtable GetPageCustomSettings(int pageID)
        {
            // Get Settings for this Page from the database
            Hashtable settings = new Hashtable();

            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabCustomSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterPageID = new SqlParameter("@TabID", SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);

                    myConnection.Open();
                    SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    try
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                    finally
                    {
                        dr.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// Read Current Page subtabs
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>PagesBox</returns>
        protected override PagesBox GetPageSettingsPagesBox(int pageID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabSettings", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    //PageID passed type FIXED by Bill Anderson (reedtek)
                    //see: http://sourceforge.net/tracker/index.php?func=detail&aid=813789&group_id=66837&atid=515929
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter("@PageID", SqlDbType.Int);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    // The new paramater "PortalLanguage" has been added to sp rb_GetPageSettings  
                    // Onur Esnaf
                    SqlParameter parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12);
                    parameterPortalLanguage.Value = Thread.CurrentThread.CurrentUICulture.Name;
                    myCommand.Parameters.Add(parameterPortalLanguage);
                    // Open the database connection and execute the command
                    myConnection.Open();

                    using (SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        PagesBox tabs = new PagesBox();
                        try
                        {
                            while (result.Read())
                            {
                                PageStripDetails tabDetails = new PageStripDetails();
                                tabDetails.PageID = (int)result["PageID"];

                                PortalPage portalPage = InstantiateNewPortalPage(tabDetails.PageID);
                                tabDetails.PageImage = portalPage.CustomMenuImage;

                                tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                                tabDetails.PageName = (string)result["PageName"];
                                tabDetails.PageOrder = (int)result["PageOrder"];
                                tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                                tabs.Add(tabDetails);
                            }
                        }
                        finally
                        {
                            result.Close(); //by Manu, fixed bug 807858
                        }
                        return tabs;
                    }
                }
            }
        }
        
        /// <summary>
        /// Update Page Custom Settings
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void DoUpdatePageSettings(int pageID, string key, string value)
        {
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateTabCustomSettings", myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter parameterPageID = new SqlParameter("@TabID", SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                    parameterKey.Value = key;
                    myCommand.Parameters.Add(parameterKey);
                    SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
                    parameterValue.Value = value;
                    myCommand.Parameters.Add(parameterValue);
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
        }

        /// <summary>
        /// Changed by Thierry@tiptopweb.com.au
        /// Page are different for custom page layout an theme, this cannot be static
        /// Added by john.mandia@whitelightsolutions.com
        /// Cache by Manu
        /// non static function, Thierry : this is necessary for page custom layout and themes
        /// </summary>
        /// <returns>A System.Collections.Hashtable value...</returns>
        //TODO: make it private
        protected override Hashtable GetPageBaseSettings(PortalPage portalPage)
        {
            //Define base settings
            Hashtable baseSettings = new Hashtable();
            int groupOrderBase;
            SettingItemGroup group;

            #region Navigation Settings

            // 2_aug_2004 Cory Isakson
            groupOrderBase = (int)SettingItemGroup.NAVIGATION_SETTINGS;
            group = SettingItemGroup.NAVIGATION_SETTINGS;

            SettingItem tabPlaceholder = new SettingItem(new BooleanDataType());
            tabPlaceholder.Group = group;
            tabPlaceholder.Order = groupOrderBase;
            tabPlaceholder.Value = "False";
            tabPlaceholder.EnglishName = "Act as a Placeholder?";
            tabPlaceholder.Description = "Allows this tab to act as a navigation placeholder only.";
            baseSettings.Add("TabPlaceholder", tabPlaceholder);

            SettingItem tabLink = new SettingItem(new StringDataType());
            tabLink.Group = group;
            tabLink.Value = string.Empty;
            tabLink.Order = groupOrderBase + 1;
            tabLink.EnglishName = "Static Link URL";
            tabLink.Description = "Allows this tab to act as a navigation link to any URL.";
            baseSettings.Add("TabLink", tabLink);

            SettingItem tabUrlKeyword = new SettingItem(new StringDataType());
            tabUrlKeyword.Group = group;
            tabUrlKeyword.Order = groupOrderBase + 2;
            tabUrlKeyword.EnglishName = "Url Keyword";
            tabUrlKeyword.Description = "Allows you to specify a keyword that would appear in your url.";
            baseSettings.Add("TabUrlKeyword", tabUrlKeyword);

            SettingItem urlPageName = new SettingItem(new StringDataType());
            urlPageName.Group = group;
            urlPageName.Order = groupOrderBase + 3;
            urlPageName.EnglishName = "Url Page Name";
            urlPageName.Description = "This setting allows you to specify a name for this tab that will show up in the url instead of default.aspx";
            baseSettings.Add("UrlPageName", urlPageName);

            #endregion

            #region Metadata Management

            //_groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
            group = SettingItemGroup.META_SETTINGS;
            SettingItem tabTitle = new SettingItem(new StringDataType());
            tabTitle.Group = group;
            tabTitle.EnglishName = "Tab / Page Title";
            tabTitle.Description = "Allows you to enter a title (Shows at the top of your browser) for this specific Tab / Page. Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabTitle", tabTitle);

            SettingItem tabMetaKeyWords = new SettingItem(new StringDataType());
            tabMetaKeyWords.Group = group;
            tabMetaKeyWords.EnglishName = "Tab / Page Keywords";
            tabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what this Tab / Page is about.Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabMetaKeyWords", tabMetaKeyWords);

            SettingItem tabMetaDescription = new SettingItem(new StringDataType());
            tabMetaDescription.Group = group;
            tabMetaDescription.EnglishName = "Tab / Page Description";
            tabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a description (Not too long though. 1 paragraph is enough) that describes this particular Tab / Page. Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabMetaDescription", tabMetaDescription);

            SettingItem tabMetaEncoding = new SettingItem(new StringDataType());
            tabMetaEncoding.Group = group;
            tabMetaEncoding.EnglishName = "Tab / Page Encoding";
            tabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the content type for this particular Tab / Page. Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabMetaEncoding", tabMetaEncoding);

            SettingItem tabMetaOther = new SettingItem(new StringDataType());
            tabMetaOther.Group = group;
            tabMetaOther.EnglishName = "Additional Meta Tag Entries";
            tabMetaOther.Description = "This setting allows you to enter new tags into this Tab / Page's HEAD Tag. Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabMetaOther", tabMetaOther);

            SettingItem tabKeyPhrase = new SettingItem(new StringDataType());
            tabKeyPhrase.Group = group;
            tabKeyPhrase.EnglishName = "Tab / Page Keyphrase";
            tabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a message/phrase for this particular Tab / Page This can be used for search engine optimisation. Enter something here to override the default portal wide setting.";
            baseSettings.Add("TabKeyPhrase", tabKeyPhrase);

            #endregion

            #region Layout and Theme

            // changed Thierry (Tiptopweb) : have a dropdown menu to select layout and themes
            groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;
            group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
            // get the list of available layouts
            // changed: Jes1111 - 2004-08-06
            ArrayList layoutsList = new ArrayList(new LayoutManager(PortalSettings.PortalPath).GetLayouts());
            LayoutItem noCustomLayout = new LayoutItem();
            noCustomLayout.Name = string.Empty;
            layoutsList.Insert(0, noCustomLayout);
            // get the list of available themes
            // changed: Jes1111 - 2004-08-06
            ArrayList themesList = new ArrayList(new ThemeManager(PortalSettings.PortalPath).GetThemes());
            ThemeItem noCustomTheme = new ThemeItem();
            noCustomTheme.Name = string.Empty;
            themesList.Insert(0, noCustomTheme);
            // changed: Jes1111 - 2004-08-06
            SettingItem customLayout = new SettingItem(new CustomListDataType(layoutsList, "Name", "Name"));
            customLayout.Group = group;
            customLayout.Order = groupOrderBase + 11;
            customLayout.EnglishName = "Custom Layout";
            customLayout.Description = "Set a custom layout for this tab only";
            baseSettings.Add("CustomLayout", customLayout);
            //SettingItem CustomTheme = new SettingItem(new StringDataType());
            // changed: Jes1111 - 2004-08-06
            SettingItem customTheme = new SettingItem(new CustomListDataType(themesList, "Name", "Name"));
            customTheme.Group = group;
            customTheme.Order = groupOrderBase + 12;
            customTheme.EnglishName = "Custom Theme";
            customTheme.Description = "Set a custom theme for the modules in this tab only";
            baseSettings.Add("CustomTheme", customTheme);
            //SettingItem CustomThemeAlt = new SettingItem(new StringDataType());
            // changed: Jes1111 - 2004-08-06
            SettingItem customThemeAlt = new SettingItem(new CustomListDataType(themesList, "Name", "Name"));
            customThemeAlt.Group = group;
            customThemeAlt.Order = groupOrderBase + 13;
            customThemeAlt.EnglishName = "Custom Alt Theme";
            customThemeAlt.Description = "Set a custom alternate theme for the modules in this tab only";
            baseSettings.Add("CustomThemeAlt", customThemeAlt);

            SettingItem customMenuImage = new SettingItem(new CustomListDataType(GetImageMenu(portalPage), "Key", "Value"));
            customMenuImage.Group = group;
            customMenuImage.Order = groupOrderBase + 14;
            customMenuImage.EnglishName = "Custom Image Menu";
            customMenuImage.Description = "Set a custom menu image for this tab";
            baseSettings.Add("CustomMenuImage", customMenuImage);

            #endregion

            #region Language/Culture Management

            groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
            group = SettingItemGroup.CULTURE_SETTINGS;
            CultureInfo[] cultureList = Rainbow.Framework.Localization.LanguageSwitcher.GetLanguageList(true);
            //Localized tab title
            int counter = groupOrderBase + 11;

            foreach (CultureInfo cultureInfo in cultureList)
            {
                //Ignore invariant
                if (cultureInfo != CultureInfo.InvariantCulture && !baseSettings.ContainsKey(cultureInfo.Name))
                {
                    SettingItem localizedTabKeyPhrase = new SettingItem(new StringDataType());
                    localizedTabKeyPhrase.Order = counter;
                    localizedTabKeyPhrase.Group = group;
                    localizedTabKeyPhrase.EnglishName = "Tab Key Phrase (" + cultureInfo.Name + ")";
                    localizedTabKeyPhrase.Description = "Key Phrase this Tab/Page for " + cultureInfo.EnglishName + " culture.";
                    baseSettings.Add("TabKeyPhrase_" + cultureInfo.Name, localizedTabKeyPhrase);
                    SettingItem LocalizedTitle = new SettingItem(new StringDataType());
                    LocalizedTitle.Order = counter;
                    LocalizedTitle.Group = group;
                    LocalizedTitle.EnglishName = "Title (" + cultureInfo.Name + ")";
                    LocalizedTitle.Description = "Set title for " + cultureInfo.EnglishName + " culture.";
                    baseSettings.Add(cultureInfo.Name, LocalizedTitle);
                    counter++;
                }
            }

            #endregion

            return baseSettings;
        }

        /// <summary>
        /// Update Page Custom Settings
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override void UpdatePageSettings(int pageID, string key, string value)
        {
            DoUpdatePageSettings(pageID, key, value);

            //Invalidate cache
            if (CurrentCache.Exists(Key.TabSettings(pageID)))
            {
                CurrentCache.Remove(Key.TabSettings(pageID));
            }

            // Clear url builder elements
            HttpUrlBuilder.Clear(pageID);
        }

        /// <summary>
        /// Return the portal home page in case you are on pageid = 0
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public override int PortalHomePageID(int portalID)
        {
            // TODO: COnvert to stored procedure?
            // TODO: Rainbow.Framwork.Application.Site.Pages Api 

            string sql = "Select PageID  From rb_Pages  Where " +
                         "(PortalID = " + portalID + ") and " +
                         "(ParentPageID is null) and  " +
                         "(PageID > 0) and ( " +
                         "PageOrder < 2)";
            return Convert.ToInt32(DBHelper.ExecuteSQLScalar(sql));
        }

        /// <summary>
        /// Gets the pages flat table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
#warning //TODO: use PageTreeItem[] instead of DataTable  
        //public DataTable GetPagesFlatTable(int portalID)
        public override object GetPagesFlatTable(int portalID)
        {
            //        -- output with hierarchy formatted
//select rb_Tabs.TabID, 
//	rb_Tabs.ParentTabID, 
//	rb_Tabs.TabOrder, 
//	rb_Tabs.TabName, 
//	#tree.levelNo , 
//	Replicate('-', (#tree.levelNo) * 2) + rb_Tabs.TabName as PageOrder

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                string strSQL = "rb_GetPageTree " + portalID;
                SqlDataAdapter da = new SqlDataAdapter(strSQL, myConnection);

                DataTable dt_tbl = new DataTable("Pages");
                // Read the resultset
                try
                {
                    da.Fill(dt_tbl);
                }

                finally
                {
                    da.Dispose();
                }

                DataTable dt_Pages = dt_tbl;
                DataColumn[] keys = new DataColumn[2];
                keys[0] = dt_Pages.Columns["PageID"];
                dt_Pages.PrimaryKey = keys;
                return dt_Pages;
            }
        }

        /// <summary>
        /// Gets the pages parent.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        public override IList<PageItem> GetPagesParent(int portalID, int pageID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = DBHelper.SqlConnection;
            SqlCommand myCommand = new SqlCommand("rb_GetTabsParent", myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);
            SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
            parameterPageID.Value = pageID;
            myCommand.Parameters.Add(parameterPageID);
            // Execute the command
            myConnection.Open();
            SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            IList<PageItem> result = new List<PageItem>();

            while (dr.Read())
            {
                PageItem item = new PageItem();
                item.ID = Convert.ToInt32(dr["PageID"]);
                item.Name = (string)dr["PageName"];
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// The UpdatePageOrder method changes the position of the tab with respect
        /// to other tabs in the portal.<br/>
        /// UpdatePageOrder Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="pageOrder">The page order.</param>
        public override void UpdatePageOrder(int pageID, int pageOrder)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateTabOrder", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterTabOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterTabOrder.Value = pageOrder;
                    myCommand.Parameters.Add(parameterTabOrder);
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
        }

        /// <summary>
        /// Gets the pages flat.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public override ArrayList GetPagesFlat(int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = DBHelper.SqlConnection)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabsFlat", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    // Execute the command
                    myConnection.Open();
                    SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    ArrayList DesktopPages = new ArrayList();

                    // Read the resultset
                    try
                    {
                        while (result.Read())
                        {
                            PageItem tabItem = new PageItem();
                            tabItem.ID = (int)result["PageID"];
                            tabItem.Name = (string)result["PageName"];
                            tabItem.Order = (int)result["PageOrder"];
                            tabItem.NestLevel = (int)result["NestLevel"];
                            DesktopPages.Add(tabItem);
                        }
                    }

                    finally
                    {
                        result.Close(); //by Manu, fixed bug 807858
                    }
                    return DesktopPages;
                }
            }
        }

        public Hashtable GetImageMenu(PortalPage portalPage)
        {
            Hashtable imageMenuFiles;

            if (!CurrentCache.Exists(Key.ImageMenuList(PortalSettings.CurrentLayout)))
            {
                imageMenuFiles = new Hashtable();
                imageMenuFiles.Add("-Default-", string.Empty);
                LayoutManager layoutManager = new LayoutManager(portalPage.PortalPath);

                string menuDirectory = Path.WebPathCombine(layoutManager.PortalLayoutPath, PortalSettings.CurrentLayout);
                if (Directory.Exists(menuDirectory))
                {
                    menuDirectory = Path.WebPathCombine(menuDirectory, "menuimages");
                }
                else
                {
                    menuDirectory = Path.WebPathCombine(LayoutManager.Path, PortalSettings.CurrentLayout, "menuimages");
                }

                if (Directory.Exists(menuDirectory))
                {
                    FileInfo[] menuImages = (new DirectoryInfo(menuDirectory)).GetFiles("*.gif");

                    foreach (FileInfo fileInfo in menuImages)
                    {
                        if (fileInfo.Name != "spacer.gif" && fileInfo.Name != "icon_arrow.gif")
                        {
                            imageMenuFiles.Add(fileInfo.Name, fileInfo.Name);
                        }
                    }
                }
                CurrentCache.Insert(Key.ImageMenuList(PortalSettings.CurrentLayout), imageMenuFiles, null);
            }
            else
            {
                imageMenuFiles = (Hashtable)CurrentCache.Get(Key.ImageMenuList(PortalSettings.CurrentLayout));
            }
            return imageMenuFiles;
        }
    }
}