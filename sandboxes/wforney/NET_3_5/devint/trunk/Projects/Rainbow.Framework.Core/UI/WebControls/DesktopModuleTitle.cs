namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// New 'stub' class for backward compatibility. Modules that add the ModuleTitle in 
    ///     their init event will be adding this. New PortalModuleControl class pulls needed 
    ///     values from this class. New modules should not use this class at all, setting 
    ///     title properties directly on PortalModuleContol instead. Jes1111.
    /// </summary>
    [DefaultProperty("Title")]
    [ToolboxData("<{0}:DesktopModuleTitle runat=server></{0}:DesktopModuleTitle>")]
    [Designer("Rainbow.Framework.UI.Design.DesktopModuleTitleDesigner")]
    public class DesktopModuleTitle : Control
    {
        // WebControl - only needs generic Control now
        // GG: added 08/04/2004 by groskrg@versifit.com to support custom buttons in the title bar
        #region Constants and Fields

        /// <summary>
        /// The edit text.
        /// </summary>
        private string editText = "EDIT";

        /// <summary>
        /// The edit url.
        /// </summary>
        private string editUrl = string.Empty;

        /// <summary>
        /// The properties text.
        /// </summary>
        private string propertiesText = "PROPERTIES";

        /// <summary>
        /// The properties url.
        /// </summary>
        private string propertiesUrl = "~/DesktopModules/CoreModules/Admin/PropertyPage.aspx";

        /// <summary>
        /// The tab id.
        /// </summary>
        private int tabId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "DesktopModuleTitle" /> class. 
        ///     Constructor
        /// </summary>
        [Obsolete("Use the corresponding properties in PortalModuleControl")]
        public DesktopModuleTitle()
        {
            this.Title = string.Empty;
            this.SecurityUrl = "~/DesktopModules/CoreModules/Admin/ModuleSettings.aspx";
            this.SecurityText = "SECURITY";
            this.SecurityTarget = string.Empty;
            this.PropertiesTarget = string.Empty;
            this.EditTarget = string.Empty;
            this.CustomButtons = new ArrayList(3);
            this.AddUrl = string.Empty;
            this.AddText = "ADD";
            this.AddTarget = string.Empty;
            this.EnableViewState = false; // No need for viewstate
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the add target.
        /// </summary>
        /// <value>The add target.</value>
        public string AddTarget { get; set; }

        /// <summary>
        ///     Gets or sets the add text.
        /// </summary>
        /// <value>The add text.</value>
        public string AddText { get; set; }

        /// <summary>
        ///     Gets or sets the add URL.
        /// </summary>
        /// <value>The add URL.</value>
        public string AddUrl { get; set; }

        /// <summary>
        ///     Gets or sets CustomButtons class allows modules to add their own buttons from Code.
        /// </summary>
        public ArrayList CustomButtons { get; set; }

        /// <summary>
        ///     Gets or sets the edit target.
        /// </summary>
        /// <value>The edit target.</value>
        public string EditTarget { get; set; }

        /// <summary>
        ///     Gets or sets the edit text.
        /// </summary>
        /// <value>The edit text.</value>
        public string EditText
        {
            get
            {
                return this.editText;
            }

            set
            {
                this.editText = value;
            }
        }

        /// <summary>
        ///     Gets or sets the edit URL.
        /// </summary>
        /// <value>The edit URL.</value>
        public string EditUrl
        {
            get
            {
                return this.editUrl;
            }

            set
            {
                this.editUrl = value;
            }
        }

        /// <summary>
        ///     Gets current linked module ID if applicable
        /// </summary>
        /// <value>The page ID.</value>
        public int PageID
        {
            get
            {
                if (this.tabId == 0)
                {
                    // Determine PageID if specified
                    if (HttpContext.Current != null && HttpContext.Current.Request.Params["PageID"] != null)
                    {
                        this.tabId = Int32.Parse(HttpContext.Current.Request.Params["PageID"]);
                    }
                    else if (HttpContext.Current != null && HttpContext.Current.Request.Params["TabID"] != null)
                    {
                        this.tabId = Int32.Parse(HttpContext.Current.Request.Params["TabID"]);
                    }
                }

                return this.tabId;
            }
        }

        /// <summary>
        ///     Gets or sets the properties target.
        /// </summary>
        /// <value>The properties target.</value>
        public string PropertiesTarget { get; set; }

        /// <summary>
        ///     Gets or sets the properties text.
        /// </summary>
        /// <value>The properties text.</value>
        public string PropertiesText
        {
            get
            {
                return this.propertiesText;
            }

            set
            {
                this.propertiesText = value;
            }
        }

        /// <summary>
        ///     Gets or sets the properties URL.
        /// </summary>
        /// <value>The properties URL.</value>
        public string PropertiesUrl
        {
            get
            {
                return this.propertiesUrl;
            }

            set
            {
                this.propertiesUrl = value;
            }
        }

        /// <summary>
        ///     Gets or sets the security target.
        /// </summary>
        /// <value>The security target.</value>
        public string SecurityTarget { get; set; }

        /// <summary>
        ///     Gets or sets the security text.
        /// </summary>
        /// <value>The security text.</value>
        public string SecurityText { get; set; }

        /// <summary>
        ///     Gets or sets the security URL.
        /// </summary>
        /// <value>The security URL.</value>
        public string SecurityUrl { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.Visible = false;
        }

        #endregion
    }
}