using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rainbow.Framework.Web.UI.WebControls;

namespace Rainbow.Framework.Data.Types
{
    /// <summary>
    /// Allows upload a file on current portal folder
    /// </summary>
    public class UploadedFileDataType : PortalUrlDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFileDataType"/> class.
        /// </summary>
        public UploadedFileDataType()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadedFileDataType"/> class.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        public UploadedFileDataType(string portalPath)
            : base(portalPath)
        {
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get { return "A file that can be uploaded to server"; }
        }

        /// <summary>
        /// InitializeComponents
        /// </summary>
        protected override void InitializeComponents()
        {
            //UploadDialogTextBox
            using (UploadDialogTextBox upload = new UploadDialogTextBox())
            {
                upload.AllowEditTextBox = true;
                upload.Width = new Unit(this.ControlWidth);
                upload.CssClass = "NormalTextBox";

                this.InnerControl = upload;
            }
        }

        /// <summary>
        /// EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public override Control EditControl
        {
            get
            {
                if (this.InnerControl == null)
                    InitializeComponents();

                //Update value in control
                UploadDialogTextBox upload = (UploadDialogTextBox) this.InnerControl;
                upload.UploadDirectory = PortalPathPrefix;
                upload.Text = Value;

                //Return control
                return this.InnerControl;
            }
            set
            {
                if (value.GetType().Name == "UploadDialogTextBox")
                {
                    this.InnerControl = value;
                    //Update value from control
                    UploadDialogTextBox upload = (UploadDialogTextBox) this.InnerControl;
                    Value = upload.Text;
                }
                else
                    throw new ArgumentException(
                        "A UploadDialogTextBox values is required, a '" + value.GetType().Name + "' is given.",
                        "EditControl");
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get { return (this.InnerValue); }
            set
            {
                //Remove portal path if present
                if (value.StartsWith(PortalPathPrefix))
                    this.InnerValue = value.Substring(PortalPathPrefix.Length);
                else
                    this.InnerValue = value;

                this.InnerValue = this.InnerValue.TrimStart('/');
            }
        }
    }
}