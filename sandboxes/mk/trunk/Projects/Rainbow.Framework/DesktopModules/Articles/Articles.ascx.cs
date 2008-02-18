using System;
using System.Collections;
using System.Web.UI.WebControls;
using Rainbow.Framework;
using Rainbow.Framework.Content.Data;
using Rainbow.Framework.Data;
using Rainbow.Framework.DataTypes;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Items;
using Rainbow.Framework.Security;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Content.Web.Modules
{
    /// <summary>
    /// Articles
    /// </summary>
    public class Articles : PortalModuleControl
    {
        /// <summary>
        /// </summary>
        protected DataList myDataList;

        /// <summary>
        /// Gets a value indicating whether [show date].
        /// </summary>
        /// <value><c>true</c> if [show date]; otherwise, <c>false</c>.</value>
        protected bool ShowDate
        {
            get
            {
                // Hide/show date
                return bool.Parse(Settings["ShowDate"].ToString());
            }
        }

        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// obtain a DataReader of Article information from the Articles
        /// table, and then databind the results to a templated DataList
        /// server control.  It uses the Rainbow.ArticleDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void Page_Load(object sender, EventArgs e)
        {
            // Obtain Articles information from the Articles table
            // and bind to the datalist control
            ArticlesDB articlesDB = new ArticlesDB();
            if (PortalSecurity.IsInRoles(Settings["EXPIRED_PERMISSION_ROLE"].ToString()))
            {
                myDataList.DataSource = articlesDB.GetArticlesAll(ModuleID, Version);
            }
            else
            {
                myDataList.DataSource = articlesDB.GetArticles(ModuleID, Version);
            }
            myDataList.DataBind();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Articles"/> class.
        /// </summary>
        public Articles()
        {
            SupportsWorkflow = true;

            if (PortalSettings != null) //check for avoid design time errors
            {
                // modified by Hongwei Shen(hongwei.shen@gmail.com) 12/9/2005
                SettingItemGroup group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                int groupBase = (int) group;
                // end of modification

                // Set Editor Settings jviladiu@portalservices.net 2004/07/30
                // modified by Hongwei Shen
                //HtmlEditorDataType.HtmlEditorSettings (this.baseSettings, SettingItemGroup.MODULE_SPECIAL_SETTINGS);
                HtmlEditorDataType.HtmlEditorSettings(baseSettings, group);
                // end of modification

                //Switches date display on/off
                SettingItem showDate = new SettingItem(new BooleanDataType());
                showDate.Value = "True";
                showDate.EnglishName = "Show Date";
                // modified by Hongwei Shen
                // ShowDate.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                // ShowDate.Order = 10;
                showDate.Group = group;
                showDate.Order = groupBase + 20;
                // end of midification
                baseSettings.Add("ShowDate", showDate);

                //Added by Rob Siera
                SettingItem defaultVisibleDays = new SettingItem(new IntegerDataType());
                defaultVisibleDays.Value = "90";
                defaultVisibleDays.EnglishName = "Default Days Visible";
                // modified by Hongwei Shen
                // DefaultVisibleDays.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                // DefaultVisibleDays.Order = 20;
                defaultVisibleDays.Group = group;
                defaultVisibleDays.Order = groupBase + 25;
                // end of midification
                baseSettings.Add("DefaultVisibleDays", defaultVisibleDays);

                SettingItem richAbstract = new SettingItem(new BooleanDataType());
                richAbstract.Value = "True";
                richAbstract.EnglishName = "Rich Abstract";
                richAbstract.Description = "User rich editor for abstract";
                // modified by Hongwei Shen
                // RichAbstract.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                // RichAbstract.Order = 30;
                richAbstract.Group = group;
                richAbstract.Order = groupBase + 30;
                // end of midification
                baseSettings.Add("ARTICLES_RICHABSTRACT", richAbstract);

                SettingItem rolesViewExpiredItems = new SettingItem(new CheckBoxListDataType(
                        AccountSystem.Instance.GetPortalRoles(PortalSettings.PortalAlias), 
                        "RoleName", 
                        "RoleName"));
                rolesViewExpiredItems.Value = "Admins";
                rolesViewExpiredItems.EnglishName = "Expired items visible to";
                rolesViewExpiredItems.Description = "Role that can see expire items";
                // modified by Hongwei Shen
                // RolesViewExpiredItems.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                // RolesViewExpiredItems.Order = 40;
                rolesViewExpiredItems.Group = group;
                rolesViewExpiredItems.Order = groupBase + 40;
                // end of midification
                baseSettings.Add("EXPIRED_PERMISSION_ROLE", rolesViewExpiredItems);
            }
        }

        #region General Implementation

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{87303CF7-76D0-49B1-A7E7-A5C8E26415BA}"); }
        }

        #region Search Implementation

        /// <summary>
        /// Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get { return true; }
        }

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">The portal ID</param>
        /// <param name="userID">ID of the user is searching</param>
        /// <param name="searchString">The text to search</param>
        /// <param name="searchField">The fields where perfoming the search</param>
        /// <returns>
        /// The SELECT sql to perform a search on the current module
        /// </returns>
        public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
        {
            SearchDefinition s =
                new SearchDefinition("rb_Articles", "Title", "Abstract", "CreatedByUser", "CreatedDate", searchField);

            //Add extra search fields here, this way
            s.ArrSearchFields.Add("itm.Description");

            return s.SearchSqlSelect(portalID, userID, searchString);
        }

        #endregion

        # region Install / Uninstall Implementation

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0]);
            }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0]);
            }
        }

        # endregion

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(Page_Load);

            // View state is not needed here, so we are disabling. - jminond
            this.myDataList.EnableViewState = false;

            // Add support for the edit page
            this.AddText = "ADD_ARTICLE";
            this.AddUrl = "~/DesktopModules/Articles/ArticlesEdit.aspx";

            base.OnInit(e);
        }

        #endregion
    }
}
