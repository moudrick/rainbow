using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Rainbow.Framework;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Content.Data;
using Rainbow.Framework.Data;
using Rainbow.Framework.DataTypes;
using Rainbow.Framework.Design;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Items;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Web.UI.WebControls;
using Label=Rainbow.Framework.Web.UI.WebControls.Label;
using Path=Rainbow.Framework.Path;

namespace Rainbow.Content.Web.Modules
{
    /// <summary>
    /// Rainbow Portal Pictures module
    /// (c)2002 by Ender Malkoc
    /// </summary>
    [History("Mario Hartmann", "mario@hartmann.net", "2.3 beta", "2003/10/08", "moved to seperate folder")]
    public class Pictures : PortalModuleControl
    {
        /// <summary>
        /// Datalist for pictures
        /// </summary>
        protected DataList dlPictures;

        /// <summary>
        /// Error label
        /// </summary>
        protected Label lblError;

        /// <summary>
        /// Paging for the pictures
        /// </summary>
        protected Paging pgPictures;

        /// <summary>
        /// Resize Options
        /// NoResize : Do not resize the picture
        /// FixedWidthHeight : Use the width and height specified. 
        /// MaintainAspectWidth : Use the specified height and calculate height using the original aspect ratio
        /// MaintainAspectHeight : Use the specified width and calculate width using the original aspect ration
        /// </summary>
        public enum ResizeOption
        {
            /// <summary>
            /// No resizing
            /// </summary>
            NoResize,
            /// <summary>
            /// FixedWidthHeight : Use the width and height specified. 
            /// </summary>
            FixedWidthHeight,
            /// <summary>
            /// MaintainAspectWidth : Use the specified height and calculate height using the original aspect ratio
            /// </summary>
            MaintainAspectWidth,
            /// <summary>
            /// MaintainAspectHeight : Use the specified width and calculate width using the original aspect ration
            /// </summary>
            MaintainAspectHeight
        }

        /// <summary>
        /// The Page_Load event on this page calls the BindData() method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            dlPictures.RepeatDirection = (Settings["RepeatDirectionSetting"] == null
                                              ? RepeatDirection.Horizontal
                                              :
                                          (RepeatDirection)
                                          Int32.Parse(((SettingItem) baseSettings["RepeatDirectionSetting"])));
            dlPictures.RepeatColumns = Int32.Parse(((SettingItem) Settings["RepeatColumns"]));
            dlPictures.ItemDataBound += Pictures_ItemDataBound;
            pgPictures.RecordsPerPage = Int32.Parse(Settings["PicturesPerPage"].ToString());
            BindData(pgPictures.PageNumber);
        }

        private void Page_Changed(object sender, EventArgs e)
        {
            BindData(pgPictures.PageNumber);
        }

        private void Pictures_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            PictureItem pictureItem;
            try
            {
                pictureItem =
                    (PictureItem)
                    Page.LoadControl(Path.ApplicationRoot + "/Design/PictureLayouts/" + Settings["ThumbnailLayout"]);
            }
            catch
            {
                lblError.Visible = true;
                dlPictures.Visible = false;
                pgPictures.Visible = false;
                return;
            }

            DataRowView di = (DataRowView) e.Item.DataItem;

            XmlDocument metadata = new XmlDocument();
            metadata.LoadXml((string) di["MetadataXml"]);

            XmlAttribute albumPath = metadata.CreateAttribute("AlbumPath");
            albumPath.Value = ((SettingItem) Settings["AlbumPath"]).FullPath;

            XmlAttribute itemID = metadata.CreateAttribute("ItemID");
            itemID.Value = ((int) di["ItemID"]).ToString();

            XmlAttribute moduleID = metadata.CreateAttribute("ModuleID");
            moduleID.Value = ModuleID.ToString();

            XmlAttribute wVersion = metadata.CreateAttribute("WVersion");
            wVersion.Value = Version.ToString();

            XmlAttribute isEditable = metadata.CreateAttribute("IsEditable");
            isEditable.Value = IsEditable.ToString();

            metadata.DocumentElement.Attributes.Append(albumPath);
            metadata.DocumentElement.Attributes.Append(itemID);
            metadata.DocumentElement.Attributes.Append(moduleID);
            metadata.DocumentElement.Attributes.Append(isEditable);
            metadata.DocumentElement.Attributes.Append(wVersion);

            if (Version == WorkFlowVersion.Production)
            {
                XmlNode modifiedFilenameNode = metadata.DocumentElement.SelectSingleNode("@ModifiedFilename");
                XmlNode thumbnailFilenameNode = metadata.DocumentElement.SelectSingleNode("@ThumbnailFilename");

                modifiedFilenameNode.Value = modifiedFilenameNode.Value.Replace(".jpg", ".Production.jpg");
                thumbnailFilenameNode.Value = thumbnailFilenameNode.Value.Replace(".jpg", ".Production.jpg");
            }

            pictureItem.Metadata = metadata;
            pictureItem.DataBind();
            e.Item.Controls.Add(pictureItem);
        }

        /// <summary>
        /// The Binddata method on this User Control is used to
        /// obtain a DataReader of picture information from the Pictures
        /// table, and then databind the results to a templated DataList
        /// server control. It uses the Rainbow.PictureDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        private void BindData(int page)
        {
            PicturesDB pictures = new PicturesDB();
            DataSet dsPictures =
                pictures.GetPicturesPaged(ModuleID, page, Int32.Parse(Settings["PicturesPerPage"].ToString()), Version);

            if (dsPictures.Tables.Count > 0 && dsPictures.Tables[0].Rows.Count > 0)
            {
                pgPictures.RecordCount = (int) (dsPictures.Tables[0].Rows[0]["RecordCount"]);
            }

            dlPictures.DataSource = dsPictures;
            dlPictures.DataBind();
        }

        /// <summary>
        /// Overriden from PortalModuleControl, this override deletes unnecessary picture files from the system
        /// </summary>
        protected override void Publish()
        {
            string pathToDelete = Server.MapPath(((SettingItem) Settings["AlbumPath"]).FullPath) + "\\";

            DirectoryInfo albumDirectory = new DirectoryInfo(pathToDelete);

            foreach (FileInfo fi in albumDirectory.GetFiles(ModuleID + "m*.Production.jpg"))
            {
                try
                {
                    File.Delete(fi.FullName);
                }
                catch {;}
            }

            foreach (FileInfo fi in albumDirectory.GetFiles(ModuleID + "m*"))
            {
                try
                {
                    File.Copy(fi.FullName, fi.FullName.Replace(".jpg", ".Production.jpg"), true);
                }
                catch {;}
            }

            base.Publish();
        }

        /// <summary>
        /// Given a key returns the value
        /// </summary>
        /// <param name="MetadataXml">XmlDocument containing key value pairs in attributes</param>
        /// <param name="key">key of the pair</param>
        /// <returns>value</returns>
        protected string GetMetadata(object MetadataXml, string key)
        {
            XmlDocument Metadata = new XmlDocument();
            Metadata.LoadXml((string) MetadataXml);

            XmlNode targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);
            if (targetNode == null)
            {
                return null;
            }
            else
            {
                return targetNode.Value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        [History("Tim Capps", "tim@cappsnet.com", "2.4 beta", "2004/02/18",
                "fixed order on settings and added ShowBulkLoad")]
        public Pictures()
        {
            // Add support for workflow
            SupportsWorkflow = true;

            // Album Path Setting
            Portal portal = PortalProvider.Instance.CurrentPortal;
            PortalUrl portalUrl = portal != null ? portal.PortalUrl : new PortalUrl(string.Empty);
            SettingItem albumPath = new SettingItem(portalUrl);
            albumPath.Required = true;
            albumPath.Value = "Album";
            albumPath.Order = 3;
            baseSettings.Add("AlbumPath", albumPath);

            // Thumbnail Resize Options
            ArrayList thumbnailResizeOptions = new ArrayList();
            thumbnailResizeOptions.Add(
                new Option((int) ResizeOption.FixedWidthHeight,
                           General.GetString("PICTURES_FIXED_WIDTH_AND_HEIGHT", "Fixed width and height", this)));
            thumbnailResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectWidth,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_WIDTH", "Maintain aspect fixed width", this)));
            thumbnailResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectHeight,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_HEIGHT", "Maintain aspect fixed height",
                                             this)));

            // Thumbnail Resize Settings
            SettingItem thumbnailResize = new SettingItem(new CustomListDataType(thumbnailResizeOptions, "Name", "Val"));
            thumbnailResize.Required = true;
            thumbnailResize.Value = ((int) ResizeOption.FixedWidthHeight).ToString();
            thumbnailResize.Order = 4;
            baseSettings.Add("ThumbnailResize", thumbnailResize);

            // Thumbnail Width Setting
            SettingItem thumbnailWidth = new SettingItem(new IntegerDataType());
            thumbnailWidth.Required = true;
            thumbnailWidth.Value = "100";
            thumbnailWidth.Order = 5;
            thumbnailWidth.MinValue = 2;
            thumbnailWidth.MaxValue = 9999;
            baseSettings.Add("ThumbnailWidth", thumbnailWidth);

            // Thumbnail Height Setting
            SettingItem thumbnailHeight = new SettingItem(new IntegerDataType());
            thumbnailHeight.Required = true;
            thumbnailHeight.Value = "75";
            thumbnailHeight.Order = 6;
            thumbnailHeight.MinValue = 2;
            thumbnailHeight.MaxValue = 9999;
            baseSettings.Add("ThumbnailHeight", thumbnailHeight);

            // Original Resize Options
            ArrayList originalResizeOptions = new ArrayList();
            originalResizeOptions.Add(
                new Option((int) ResizeOption.NoResize, General.GetString("PICTURES_DONT_RESIZE", "Don't Resize", this)));
            originalResizeOptions.Add(
                new Option((int) ResizeOption.FixedWidthHeight,
                           General.GetString("PICTURES_FIXED_WIDTH_AND_HEIGHT", "Fixed width and height", this)));
            originalResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectWidth,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_WIDTH", "Maintain aspect fixed width", this)));
            originalResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectHeight,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_HEIGHT", "Maintain aspect fixed height",
                                             this)));

            // Original Resize Settings
            SettingItem originalResize = new SettingItem(new CustomListDataType(originalResizeOptions, "Name", "Val"));
            originalResize.Required = true;
            originalResize.Value = ((int) ResizeOption.MaintainAspectWidth).ToString();
            originalResize.Order = 7;
            baseSettings.Add("OriginalResize", originalResize);

            // Original Width Settings
            SettingItem originalWidth = new SettingItem(new IntegerDataType());
            originalWidth.Required = true;
            originalWidth.Value = "800";
            originalWidth.Order = 8;
            originalWidth.MinValue = 2;
            originalWidth.MaxValue = 9999;
            baseSettings.Add("OriginalWidth", originalWidth);

            // Original Width Settings
            SettingItem originalHeight = new SettingItem(new IntegerDataType());
            originalHeight.Required = true;
            originalHeight.Value = "600";
            originalHeight.Order = 9;
            originalHeight.MinValue = 2;
            originalHeight.MaxValue = 9999;
            baseSettings.Add("OriginalHeight", originalHeight);

            // Repeat Direction Options
            ArrayList repeatDirectionOptions = new ArrayList();
            repeatDirectionOptions.Add(
                new Option((int) RepeatDirection.Horizontal,
                           General.GetString("PICTURES_HORIZONTAL", "Horizontal", this)));
            repeatDirectionOptions.Add(
                new Option((int) RepeatDirection.Vertical, General.GetString("PICTURES_VERTICAL", "Vertical", this)));

            // Repeat Direction Setting
            SettingItem repeatDirectionSetting =
                new SettingItem(new CustomListDataType(repeatDirectionOptions, "Name", "Val"));
            repeatDirectionSetting.Required = true;
            repeatDirectionSetting.Value = ((int) RepeatDirection.Horizontal).ToString();
            repeatDirectionSetting.Order = 10;
            baseSettings.Add("RepeatDirection", repeatDirectionSetting);

            // Repeat Columns Setting
            SettingItem repeatColumns = new SettingItem(new IntegerDataType());
            repeatColumns.Required = true;
            repeatColumns.Value = "6";
            repeatColumns.Order = 11;
            repeatColumns.MinValue = 1;
            repeatColumns.MaxValue = 200;
            baseSettings.Add("RepeatColumns", repeatColumns);

            // Layouts
            Hashtable layouts = new Hashtable();
            foreach (
                string layoutControl in
                    Directory.GetFiles(
                        HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/Design/PictureLayouts"), "*.ascx"))
            {
                string layoutControlDisplayName =
                    layoutControl.Substring(layoutControl.LastIndexOf("\\") + 1,
                                            layoutControl.LastIndexOf(".") - layoutControl.LastIndexOf("\\") - 1);
                string layoutControlName = layoutControl.Substring(layoutControl.LastIndexOf("\\") + 1);
                layouts.Add(layoutControlDisplayName, layoutControlName);
            }

            // Thumbnail Layout Setting
            SettingItem thumbnailLayoutSetting = new SettingItem(new CustomListDataType(layouts, "Key", "Value"));
            thumbnailLayoutSetting.Required = true;
            thumbnailLayoutSetting.Value = "DefaultThumbnailView.ascx";
            thumbnailLayoutSetting.Order = 12;
            baseSettings.Add("ThumbnailLayout", thumbnailLayoutSetting);

            // Thumbnail Layout Setting
            SettingItem imageLayoutSetting = new SettingItem(new CustomListDataType(layouts, "Key", "Value"));
            imageLayoutSetting.Required = true;
            imageLayoutSetting.Value = "DefaultImageView.ascx";
            imageLayoutSetting.Order = 13;
            baseSettings.Add("ImageLayout", imageLayoutSetting);

            // PicturesPerPage
            SettingItem picturesPerPage = new SettingItem(new IntegerDataType());
            picturesPerPage.Required = true;
            picturesPerPage.Value = "9999";
            picturesPerPage.Order = 14;
            picturesPerPage.MinValue = 1;
            picturesPerPage.MaxValue = 9999;
            baseSettings.Add("PicturesPerPage", picturesPerPage);

            //If false the input box for bulk loads will be hidden
            SettingItem allowBulkLoad = new SettingItem(new BooleanDataType());
            allowBulkLoad.Value = "false";
            allowBulkLoad.Order = 15;
            baseSettings.Add("AllowBulkLoad", allowBulkLoad);
        }

        #region Global Implementation

        /// <summary>
        /// GuidID
        /// </summary>
        public override Guid GuidID
        {
            get { return new Guid("{B29CB86B-AEA1-4E94-8B77-B4E4239258B0}"); }
        }

        #region Search Implementation

        /// <summary>
        /// Searchable module
        /// </summary>
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
        /// <returns>The SELECT sql to perform a search on the current module</returns>
        public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
        {
            // Parameters:
            // Table Name: the table that holds the data
            // Title field: the field that contains the title for result, must be a field in the table
            // Abstract field: the field that contains the text for result, must be a field in the table
            // Search field: pass the searchField parameter you recieve.

            SearchDefinition s =
                new SearchDefinition("rb_Pictures", "ShortDescription", "Keywords", "CreatedByUser", "CreatedDate",
                                     "Keywords");

            //Add here extra search fields, this way
            s.ArrSearchFields.Add("itm.ShortDescription");

            // Builds and returns the SELECT query
            return s.SearchSqlSelect(portalID, userID, searchString);
        }

        #endregion

        #region Install / Uninstall Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred: " + errors[0]);
            }
        }

        /// <summary>
        /// 
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

        #endregion

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

            this.dlPictures.EnableViewState = false;
            pgPictures.OnMove += new EventHandler(Page_Changed);
            this.AddText = "ADD"; //"Add New Picture"
            this.AddUrl = "~/DesktopModules/Pictures/PicturesEdit.aspx";
            base.OnInit(e);
        }

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion

        /// <summary>
        /// Structure used for list settings
        /// </summary>
        struct Option
        {
            int val;
            string name;

            /// <summary>
            /// </summary>
            public int Val
            {
                get { return val; }
                set { val = value; }
            }

            /// <summary>
            /// </summary>
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <summary>
            /// </summary>
            /// <param name="aVal"></param>
            /// <param name="aName"></param>
            public Option(int aVal, string aName)
            {
                val = aVal;
                name = aName;
            }
        }
    }
}
