namespace Rainbow.Framework.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// UploadDialogTextBox
    /// </summary>
    public class UploadDialogTextBox : TextBox
    {
        #region Constants and Fields

        /// <summary>
        /// The link.
        /// </summary>
        private string link;

        /// <summary>
        /// The m_ return function.
        /// </summary>
        private string m_ReturnFunction;

        /// <summary>
        /// The m_ total height.
        /// </summary>
        private int m_TotalHeight;

        /// <summary>
        /// The m_ total width.
        /// </summary>
        private int m_TotalWidth;

        /// <summary>
        /// The m_ upload directory.
        /// </summary>
        private string m_UploadDirectory;

        /// <summary>
        /// The m_button width.
        /// </summary>
        private Unit m_buttonWidth = new Unit(25);

        /// <summary>
        /// The on click.
        /// </summary>
        private string onClick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadDialogTextBox"/> class. 
        ///     Constructor
        ///     Set default values
        /// </summary>
        public UploadDialogTextBox()
        {
            this.Value = " ... ";
            this.m_UploadDirectory = "images/";
            this.onClick = string.Empty;
            this.link = string.Empty;
            this.FileNameOnly = false;
            this.MaxWidth = -1;
            this.MaxHeight = -1;
            this.m_TotalWidth = -1;
            this.m_TotalHeight = -1;
            this.MinWidth = -1;
            this.MinHeight = -1;
            this.MaxBytes = -1;
            this.FileTypes = "jpg,gif,png";
            this.AllowUpload = true;
            this.ShowUploadFirst = false;
            this.AllowDelete = false;
            this.AllowEdit = false;
            this.AllowCreateDirectory = false;
            this.AllowEditTextBox = false;
            this.AllowRename = false;
            this.m_ReturnFunction = string.Empty;
            this.PreselectedFile = string.Empty;
            this.FontName = "Verdana, Helvetica, Sans";
            this.FontSize = "11px";
            this.DemoMode = false;
            this.DataStore = "Session";
            this.ShowExceptions = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether [allow create directory].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [allow create directory]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue("false"), Description("Gets/sets the AllowCreateDirectory."), Bindable(true), Category("Data")]
        public bool AllowCreateDirectory { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow delete].
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        [Description("Gets/sets the AllowDelete."), DefaultValue("false"), Bindable(true), Category("Data")]
        public bool AllowDelete { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        [Category("Data"), Description("Gets/sets the AllowEdit."), Bindable(true), DefaultValue("false")]
        public bool AllowEdit { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow edit text box].
        /// </summary>
        /// <value><c>true</c> if [allow edit text box]; otherwise, <c>false</c>.</value>
        [Description("Gets/sets the AllowEditTextBox.")]
        public bool AllowEditTextBox { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow rename].
        /// </summary>
        /// <value><c>true</c> if [allow rename]; otherwise, <c>false</c>.</value>
        [Description("Gets/sets the AllowRename.")]
        public bool AllowRename { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [allow upload].
        /// </summary>
        /// <value><c>true</c> if [allow upload]; otherwise, <c>false</c>.</value>
        [Description("Gets/sets the AllowUpload."), Bindable(true), Category("Data"), DefaultValue("true")]
        public bool AllowUpload { get; set; }

        /// <summary>
        ///     Gets or sets the data store.
        /// </summary>
        /// <value>The data store.</value>
        [Description("Gets/sets the DataStore.[Session|Application|Config]")]
        public string DataStore { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [demo mode].
        /// </summary>
        /// <value><c>true</c> if [demo mode]; otherwise, <c>false</c>.</value>
        [DefaultValue("false"), Bindable(true), Description("Gets/sets the DemoMode."), Category("Appearance")]
        public bool DemoMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [file name only].
        /// </summary>
        /// <value><c>true</c> if [file name only]; otherwise, <c>false</c>.</value>
        [Bindable(true), DefaultValue("false"), Description("Gets/sets the FileNameOnly."), Category("Data")]
        public bool FileNameOnly { get; set; }

        /// <summary>
        ///     Gets or sets the file types.
        /// </summary>
        /// <value>The file types.</value>
        [Description("Sets the FileTypes."), Bindable(true), Category("Data"), DefaultValue("jpg,gif,png")]
        public string FileTypes { get; set; }

        /// <summary>
        ///     Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        [Description("Gets/sets the FontName.")]
        public string FontName { get; set; }

        /// <summary>
        ///     Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        [Description("Gets/sets the FontSize.")]
        public string FontSize { get; set; }

        /// <summary>
        ///     Gets the javascript link.
        /// </summary>
        /// <value>The javascript link.</value>
        [Description("Gets the JavascriptLink.")]
        public string JavascriptLink
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    this.RegisterSession();
                    return this.onClick;
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets the link.
        /// </summary>
        /// <value>The link .</value>
        [Description("Gets/sets the Link.")]
        public string Link
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    this.RegisterSession();
                    return this.link;
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the max bytes.
        /// </summary>
        /// <value>The max bytes.</value>
        [Description("Gets/sets the MaxBytes."), Category("Data"), DefaultValue("-1"), Bindable(true)]
        public int MaxBytes { get; set; }

        /// <summary>
        ///     Gets or sets the height of the max.
        /// </summary>
        /// <value>The height of the max.</value>
        [Description("Gets/sets the MaxHeight."), Bindable(true), Category("Data"), DefaultValue("-1")]
        public int MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the max.
        /// </summary>
        /// <value>The width of the max.</value>
        [Bindable(true), Description("Gets/sets the MaxWidth."), Category("Data"), DefaultValue("-1")]
        public int MaxWidth { get; set; }

        /// <summary>
        ///     Gets or sets the height of the min.
        /// </summary>
        /// <value>The height of the min.</value>
        [Category("Data"), Description("Gets/sets the MinHeight."), DefaultValue("-1"), Bindable(true)]
        public int MinHeight { get; set; }

        /// <summary>
        ///     Gets or sets the width of the min.
        /// </summary>
        /// <value>The width of the min.</value>
        [Bindable(true), Category("Data"), Description("Gets/sets the MinWidth."), DefaultValue("-1")]
        public int MinWidth { get; set; }

        /// <summary>
        ///     Gets or sets the preselected file.
        /// </summary>
        /// <value>The preselected file.</value>
        [Description("Gets/sets the PreselectedFile.")]
        public string PreselectedFile { get; set; }

        /// <summary>
        ///     Gets or sets the return function.
        /// </summary>
        /// <value>The return function.</value>
        [Description("Gets/sets the ReturnFunction.")]
        public string ReturnFunction
        {
            get
            {
                return this.m_ReturnFunction;
            }

            set
            {
                this.m_ReturnFunction = value.Replace("()", string.Empty);
                this.RegisterSession();
                var textArray1 = new[]
                    {
                        "var host = window.open('" + UploadPath + "?Id=", this.m_ReturnFunction, "&selectedfile=", 
                        this.PreselectedFile, 
                        "', 'browse', 'width=700,height=400,status=yes,resizable=yes'); host.focus();"
                    };
                this.onClick = string.Concat(textArray1);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [show exceptions].
        /// </summary>
        /// <value><c>true</c> if [show exceptions]; otherwise, <c>false</c>.</value>
        [Description("Gets/sets the ShowExceptions.")]
        public bool ShowExceptions { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [show upload first].
        /// </summary>
        /// <value><c>true</c> if [show upload first]; otherwise, <c>false</c>.</value>
        [DefaultValue("false"), Category("Appearance"), Description("Gets/sets the ShowUploadFirst."), Bindable(true)]
        public bool ShowUploadFirst { get; set; }

        /// <summary>
        ///     Gets or sets the total height.
        /// </summary>
        /// <value>The total height.</value>
        [Bindable(true)]
        [DefaultValue("-1")]
        [Description("Gets/sets the TotalHeight.")]
        [Category("Data")]
        public int TotalHeight
        {
            get
            {
                return this.m_TotalHeight;
            }

            set
            {
                this.m_TotalHeight = value;
                this.MinHeight = value;
                this.MaxHeight = value;
            }
        }

        /// <summary>
        ///     Gets or sets the total width.
        /// </summary>
        /// <value>The total width.</value>
        [DefaultValue("-1")]
        [Description("Gets/sets the TotalWidth.")]
        [Category("Data")]
        [Bindable(true)]
        public int TotalWidth
        {
            get
            {
                return this.m_TotalWidth;
            }

            set
            {
                this.m_TotalWidth = value;
                this.MinWidth = value;
                this.MaxWidth = value;
            }
        }

        /// <summary>
        ///     Gets or sets the upload directory.
        /// </summary>
        /// <value>The upload directory.</value>
        [Category("Data")]
        [DefaultValue("images/")]
        [Description("Gets/sets the UploadDirectory.")]
        [Bindable(true)]
        public string UploadDirectory
        {
            get
            {
                return this.m_UploadDirectory;
            }

            set
            {
                if (value.Length > 0)
                {
                    if ((value.Substring(0, 1) != "/") && (value.Substring(0, 1) != "."))
                    {
                        if (HttpContext.Current != null)
                        {
                            this.m_UploadDirectory = HttpContext.Current.Request.ApplicationPath +
                                                     ((HttpContext.Current.Request.ApplicationPath != "/")
                                                          ? "/"
                                                          : string.Empty) + value;
                        }
                        else
                        {
                            this.m_UploadDirectory = value;
                        }
                    }
                    else
                    {
                        this.m_UploadDirectory = value;
                    }
                }
                else
                {
                    this.m_UploadDirectory = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Description("Gets/sets the Value."), Bindable(true), Category("Appearance"), DefaultValue(" ... ")]
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the width of the Web server control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref = "T:System.Web.UI.WebControls.Unit"></see> that represents the width of the control. The default is <see cref = "F:System.Web.UI.WebControls.Unit.Empty"></see>.</returns>
        /// <exception cref = "T:System.ArgumentException">The width of the Web server control was set to a negative value. </exception>
        [Description("Gets/sets the Width.")]
        [Bindable(true)]
        [Category("Appearance")]
        public override Unit Width
        {
            get
            {
                return new Unit(base.Width.Value + this.m_buttonWidth.Value);
            }

            set
            {
                base.Width = value.Value >= this.m_buttonWidth.Value ? new Unit(value.Value - this.m_buttonWidth.Value) : 0;
            }
        }

        /// <summary>
        ///     Gets the upload path.
        /// </summary>
        /// <value>The upload path.</value>
        private static string UploadPath
        {
            get
            {
                string appPath;

                // Build the relative Application Path
                if (HttpContext.Current != null)
                {
                    var req = HttpContext.Current.Request;
                    appPath = (req.ApplicationPath == "/") ? string.Empty : req.ApplicationPath;
                }
                else
                {
                    appPath = "/";
                }

                return string.Concat(appPath, "/UploadDialog.aspx");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers client script for generating postback events prior to rendering on the client, if <see cref="P:System.Web.UI.WebControls.TextBox.AutoPostBack"></see> is true.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> that contains the event data.
        /// </param>
        protected override void OnPreRender(EventArgs e)
        {
            string[] textArray1;

            if (string.Empty.Equals(this.m_ReturnFunction))
            {
                this.link = string.Format("{0}?Id={1}", UploadPath, this.ID);
                textArray1 = new[]
                    {
                        string.Format("var host = window.open('{0}?Id=", UploadPath),
                        this.ID,
                        "' + ((document.getElementById('", 
                        this.ClientID,
                        "').value!='') ? '&selectedfile=' + document.getElementById('",
                        this.ClientID, 
                        "').value : ''), 'browse', 'width=760,height=400,status=yes,resizable=yes'); host.focus();"
                    };
                this.onClick = string.Concat(textArray1);
                if (!this.AllowEditTextBox)
                {
                    this.Attributes.Add("onkeydown", "return false;");
                    this.Attributes.Add("onClick", "this.blur();" + this.onClick);
                }
            }
            else
            {
                this.link = string.Format("{0}?Id={1}", UploadPath, this.m_ReturnFunction);
                textArray1 = new[]
                    {
                        string.Format("var host = window.open('{0}?Id=", UploadPath),
                        this.m_ReturnFunction,
                        "&selectedfile=", 
                        this.PreselectedFile, 
                        "', 'browse', 'width=760,height=400,status=yes,resizable=yes'); host.focus();"
                    };
                this.onClick = string.Concat(textArray1);
            }
        }

        /// <summary>
        /// Renders the <see cref="T:System.Web.UI.WebControls.TextBox"></see> control to the specified <see cref="T:System.Web.UI.HtmlTextWriter"></see> object.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"></see> that receives the rendered output.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            this.RegisterSession();
            base.Render(writer);
            var button = new Button { Text = this.Value, Width = this.m_buttonWidth };
            button.Attributes.Add("OnClick", this.onClick);
            button.RenderControl(writer);
        }

        /// <summary>
        /// Registers the session.
        /// </summary>
        private void RegisterSession()
        {
            if (HttpContext.Current == null)
            {
                return; // design time
            }

            if (HttpContext.Current.Session == null)
            {
                throw new ApplicationException(
                    "The session is disabled, please enable before continue", new NullReferenceException());

                    // session is off
            }

            var hashtable1 = new Hashtable();
            if (string.Empty.Equals(this.m_ReturnFunction))
            {
                hashtable1.Add("ControlToFill", this.ClientID);
                hashtable1.Add("ReturnFunction", this.m_ReturnFunction);
            }
            else
            {
                hashtable1.Add("ControlToFill", this.m_ReturnFunction);
                hashtable1.Add("ReturnFunction", this.m_ReturnFunction);
            }

            hashtable1.Add("ShowExceptions", this.ShowExceptions);
            hashtable1.Add("UploadDirectory", this.m_UploadDirectory);
            hashtable1.Add("AllowUpload", this.AllowUpload.ToString());
            hashtable1.Add("ShowUploadFirst", this.ShowUploadFirst.ToString());
            hashtable1.Add("MaxWidth", this.MaxWidth.ToString());
            hashtable1.Add("MinWidth", this.MinWidth.ToString());
            hashtable1.Add("MaxHeight", this.MaxHeight.ToString());
            hashtable1.Add("MinHeight", this.MinHeight.ToString());
            hashtable1.Add("MaxBytes", this.MaxBytes.ToString());
            hashtable1.Add("AllowEdit", this.AllowEdit.ToString());
            hashtable1.Add("AllowDelete", this.AllowDelete.ToString());
            hashtable1.Add("AllowCreateDirectory", this.AllowCreateDirectory.ToString());
            hashtable1.Add("AllowRename", this.AllowRename.ToString());
            hashtable1.Add("FileTypes", this.FileTypes);
            hashtable1.Add("FileNameOnly", this.FileNameOnly.ToString());
            hashtable1.Add("DemoMode", this.DemoMode.ToString());
            hashtable1.Add("FontName", this.FontName);
            hashtable1.Add("FontSize", this.FontSize);
            if (this.DataStore == "Session")
            {
                if (string.Empty.Equals(this.m_ReturnFunction))
                {
                    HttpContext.Current.Session["UpldDlg" + this.ID] = hashtable1;
                }
                else
                {
                    HttpContext.Current.Session["UpldDlg" + this.m_ReturnFunction] = hashtable1;
                }
            }
            else if (this.DataStore == "Application")
            {
                if (string.Empty.Equals(this.m_ReturnFunction))
                {
                    HttpContext.Current.Application["UpldDlg" + this.ID] = hashtable1;
                }
                else
                {
                    HttpContext.Current.Application["UpldDlg" + this.m_ReturnFunction] = hashtable1;
                }
            }
        }

        #endregion
    }
}