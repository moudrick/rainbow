using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// UploadDialogTextBox
	/// </summary>
    public class UploadDialogTextBox : TextBox
    {
        /// <summary>
        /// Constructor
        /// Set default values
        /// </summary>
        public UploadDialogTextBox()
        {
            this.m_Value = " ... ";
            this.m_UploadDirectory = "images/";
            this.onClick = string.Empty;
            this.link = string.Empty;
            this.m_FileNameOnly = false;
            this.m_MaxWidth = -1;
            this.m_MaxHeight = -1;
            this.m_TotalWidth = -1;
            this.m_TotalHeight = -1;
            this.m_MinWidth = -1;
            this.m_MinHeight = -1;
            this.m_MaxBytes = -1;
            this.m_FileTypes = "jpg,gif,png";
            this.m_AllowUpload = true;
            this.m_ShowUploadFirst = false;
            this.m_AllowDelete = false;
            this.m_AllowEdit = false;
            this.m_AllowCreateDirectory = false;
            this.m_AllowEditTextBox = false;
            this.m_AllowRename = false;
            this.m_ReturnFunction = string.Empty;
            this.m_PreselectedFile = string.Empty;
            this.m_FontName = "Verdana, Helvetica, Sans";
            this.m_FontSize = "11px";
            this.m_DemoMode = false;
            this.m_DataStore = "Session";
            this.m_ShowExceptions = false;
        }


		private string uploadPath 
		{
			get 
			{
				string appPath;
				//Build the relative Application Path
				if (HttpContext.Current != null)
				{
					HttpRequest req = HttpContext.Current.Request;
					appPath = 
						(req.ApplicationPath == "/") 
						? string.Empty 
						: req.ApplicationPath;
				}
				else
				{
					appPath = "/";
				}
				return string.Concat(appPath, "/UploadDialog.aspx");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            string[] textArray1;

            if (string.Empty.Equals(this.m_ReturnFunction))
            {
                this.link = uploadPath + "?Id=" + this.ID;
                textArray1 = new string[7] { "var host = window.open('" + uploadPath + "?Id=", this.ID, "' + ((document.getElementById('", this.ClientID, "').value!='') ? '&selectedfile=' + document.getElementById('", this.ClientID, "').value : ''), 'browse', 'width=760,height=400,status=yes,resizable=yes'); host.focus();" } ;
                this.onClick = string.Concat(textArray1);
                if (!this.m_AllowEditTextBox)
                {
                    this.Attributes.Add("onkeydown", "return false;");
                    this.Attributes.Add("onClick", "this.blur();" + this.onClick);
                }
            }
            else
            {
                this.link = uploadPath + "?Id=" + this.m_ReturnFunction;
                textArray1 = new string[5] { "var host = window.open('" + uploadPath + "?Id=", this.m_ReturnFunction, "&selectedfile=", this.m_PreselectedFile, "', 'browse', 'width=760,height=400,status=yes,resizable=yes'); host.focus();" } ;
                this.onClick = string.Concat(textArray1);
            }
        }

        private void RegisterSession()
        {
			if (HttpContext.Current == null)
				return; //design time

			if (HttpContext.Current.Session == null)
				throw new ApplicationException("The session is disabled, please enable before continue", new NullReferenceException()); //session is off

        	Hashtable hashtable1 = new Hashtable();
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
            hashtable1.Add("ShowExceptions", this.m_ShowExceptions);
            hashtable1.Add("UploadDirectory", this.m_UploadDirectory);
            hashtable1.Add("AllowUpload", this.m_AllowUpload.ToString());
            hashtable1.Add("ShowUploadFirst", this.m_ShowUploadFirst.ToString());
            hashtable1.Add("MaxWidth", this.MaxWidth.ToString());
            hashtable1.Add("MinWidth", this.m_MinWidth.ToString());
            hashtable1.Add("MaxHeight", this.m_MaxHeight.ToString());
            hashtable1.Add("MinHeight", this.m_MinHeight.ToString());
            hashtable1.Add("MaxBytes", this.m_MaxBytes.ToString());
            hashtable1.Add("AllowEdit", this.m_AllowEdit.ToString());
            hashtable1.Add("AllowDelete", this.m_AllowDelete.ToString());
            hashtable1.Add("AllowCreateDirectory", this.m_AllowCreateDirectory.ToString());
            hashtable1.Add("AllowRename", this.m_AllowRename.ToString());
            hashtable1.Add("FileTypes", this.m_FileTypes);
            hashtable1.Add("FileNameOnly", this.m_FileNameOnly.ToString());
            hashtable1.Add("DemoMode", this.m_DemoMode.ToString());
            hashtable1.Add("FontName", this.m_FontName);
            hashtable1.Add("FontSize", this.m_FontSize);
            if (this.m_DataStore == "Session")
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
            else if (this.m_DataStore == "Application")
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            this.RegisterSession();
			base.Render(writer);
			Button myButton = new Button();
			myButton.Text = this.m_Value;
			myButton.Attributes.Add("OnClick", this.onClick);
			myButton.Width = m_buttonWidth;
			myButton.RenderControl(writer);
        }


		#region Properties
		/// <summary>
		/// 
		/// </summary>
        [DefaultValue("false"), Description("Gets/sets the AllowCreateDirectory."), Bindable(true), Category("Data")]
        public bool AllowCreateDirectory
        {
            get
            {
                return this.m_AllowCreateDirectory;
            }
            set
            {
                this.m_AllowCreateDirectory = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the AllowDelete."), DefaultValue("false"), Bindable(true), Category("Data")]
        public bool AllowDelete
        {
            get
            {
                return this.m_AllowDelete;
            }
            set
            {
                this.m_AllowDelete = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Category("Data"), Description("Gets/sets the AllowEdit."), Bindable(true), DefaultValue("false")]
        public bool AllowEdit
        {
            get
            {
                return this.m_AllowEdit;
            }
            set
            {
                this.m_AllowEdit = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the AllowEditTextBox.")]
        public bool AllowEditTextBox
        {
            get
            {
                return this.m_AllowEditTextBox;
            }
            set
            {
                this.m_AllowEditTextBox = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the AllowRename.")]
        public bool AllowRename
        {
            get
            {
                return this.m_AllowRename;
            }
            set
            {
                this.m_AllowRename = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the AllowUpload."), Bindable(true), Category("Data"), DefaultValue("true")]
        public bool AllowUpload
        {
            get
            {
                return this.m_AllowUpload;
            }
            set
            {
                this.m_AllowUpload = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the DataStore.[Session|Application|Config]")]
        public string DataStore
        {
            get
            {
                return this.m_DataStore;
            }
            set
            {
                this.m_DataStore = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [DefaultValue("false"), Bindable(true), Description("Gets/sets the DemoMode."), Category("Appearance")]
        public bool DemoMode
        {
            get
            {
                return this.m_DemoMode;
            }
            set
            {
                this.m_DemoMode = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Bindable(true), DefaultValue("false"), Description("Gets/sets the FileNameOnly."), Category("Data")]
        public bool FileNameOnly
        {
            get
            {
                return this.m_FileNameOnly;
            }
            set
            {
                this.m_FileNameOnly = value;
            }
        }


		/// <summary>
		/// 
		/// </summary>
        [Description("Sets the FileTypes."), Bindable(true), Category("Data"), DefaultValue("jpg,gif,png")]
        public string FileTypes
        {
            get
            {
                return this.m_FileTypes;
            }
            set
            {
                this.m_FileTypes = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the FontName.")]
        public string FontName
        {
            get
            {
                return this.m_FontName;
            }
            set
            {
                this.m_FontName = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the FontSize.")]
        public string FontSize
        {
            get
            {
                return this.m_FontSize;
            }
            set
            {
                this.m_FontSize = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
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
		/// 
		/// </summary>
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
		/// 
		/// </summary>
        [Description("Gets/sets the MaxBytes."), Category("Data"), DefaultValue("-1"), Bindable(true)]
        public int MaxBytes
        {
            get
            {
                return this.m_MaxBytes;
            }
            set
            {
                this.m_MaxBytes = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the MaxHeight."), Bindable(true), Category("Data"), DefaultValue("-1")]
        public int MaxHeight
        {
            get
            {
                return this.m_MaxHeight;
            }
            set
            {
                this.m_MaxHeight = value;
            }
        }

		/// <summary>
		/// /
		/// </summary>
        [Bindable(true), Description("Gets/sets the MaxWidth."), Category("Data"), DefaultValue("-1")]
        public int MaxWidth
        {
            get
            {
                return this.m_MaxWidth;
            }
            set
            {
                this.m_MaxWidth = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Category("Data"), Description("Gets/sets the MinHeight."), DefaultValue("-1"), Bindable(true)]
        public int MinHeight
        {
            get
            {
                return this.m_MinHeight;
            }
            set
            {
                this.m_MinHeight = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Bindable(true), Category("Data"), Description("Gets/sets the MinWidth."), DefaultValue("-1")]
        public int MinWidth
        {
            get
            {
                return this.m_MinWidth;
            }
            set
            {
                this.m_MinWidth = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the PreselectedFile.")]
        public string PreselectedFile
        {
            get
            {
                return this.m_PreselectedFile;
            }
            set
            {
                this.m_PreselectedFile = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
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
                string[] textArray1 = new string[5] { "var host = window.open('" + uploadPath + "?Id=", this.m_ReturnFunction, "&selectedfile=", this.m_PreselectedFile, "', 'browse', 'width=700,height=400,status=yes,resizable=yes'); host.focus();" } ;
                this.onClick = string.Concat(textArray1);
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the ShowExceptions.")]
        public bool ShowExceptions
        {
            get
            {
                return this.m_ShowExceptions;
            }
            set
            {
                this.m_ShowExceptions = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [DefaultValue("false"), Category("Appearance"), Description("Gets/sets the ShowUploadFirst."), Bindable(true)]
        public bool ShowUploadFirst
        {
            get
            {
                return this.m_ShowUploadFirst;
            }
            set
            {
                this.m_ShowUploadFirst = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Bindable(true), DefaultValue("-1"), Description("Gets/sets the TotalHeight."), Category("Data")]
        public int TotalHeight
        {
            get
            {
                return this.m_TotalHeight;
            }
            set
            {
                this.m_TotalHeight = value;
                this.m_MinHeight = value;
                this.m_MaxHeight = value;
            }
        }


		/// <summary>
		/// 
		/// </summary>
        [DefaultValue("-1"), Description("Gets/sets the TotalWidth."), Category("Data"), Bindable(true)]
        public int TotalWidth
        {
            get
            {
                return this.m_TotalWidth;
            }
            set
            {
                this.m_TotalWidth = value;
                this.m_MinWidth = value;
                this.m_MaxWidth = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Category("Data"), DefaultValue("images/"), Description("Gets/sets the UploadDirectory."), Bindable(true)]
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
                            this.m_UploadDirectory = HttpContext.Current.Request.ApplicationPath + ((HttpContext.Current.Request.ApplicationPath != "/") ? "/" : string.Empty) + value;
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
		/// 
		/// </summary>
        [Description("Gets/sets the Value."), Bindable(true), Category("Appearance"), DefaultValue(" ... ")]
        public string Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
        [Description("Gets/sets the Width."), Bindable(true), Category("Appearance")]
        public override Unit Width
        {
            get
            {
                return new Unit(base.Width.Value + m_buttonWidth.Value);
            }
            set
            {
				if (value.Value >= m_buttonWidth.Value)
					base.Width = new Unit(value.Value - m_buttonWidth.Value);
				else
					base.Width = 0;
            }
        }
		#endregion

		#region Declerations
		private Unit m_buttonWidth = new Unit(25);
        private string link;
        private bool m_AllowCreateDirectory;
        private bool m_AllowDelete;
        private bool m_AllowEdit;
        private bool m_AllowEditTextBox;
        private bool m_AllowRename;
        private bool m_AllowUpload;
        private string m_DataStore;
        private bool m_DemoMode;
        private bool m_FileNameOnly;
        private string m_FileTypes;
        private string m_FontName;
        private string m_FontSize;
        private int m_MaxBytes;
        private int m_MaxHeight;
        private int m_MaxWidth;
        private int m_MinHeight;
        private int m_MinWidth;
        private string m_PreselectedFile;
        private string m_ReturnFunction;
        private bool m_ShowExceptions;
        private bool m_ShowUploadFirst;
        private int m_TotalHeight;
        private int m_TotalWidth;
        private string m_UploadDirectory;
        private string m_Value;
        private string onClick;
		#endregion
    }
}

