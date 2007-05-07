using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rainbow.UI.WebControls;

namespace Rainbow.UI.DataTypes
{

	/// <summary>
	/// Allows upload a file on current portal folder
	/// </summary>
	public class UploadedFileDataType : PortalUrlDataType
	{
		public UploadedFileDataType() : base()
		{
		}

		public UploadedFileDataType(string portalPath) : base(portalPath)
		{
		}

		public override string Description
		{
			get
			{
				return "A file that can be uploaded to server";
			}
		}
		
		protected override void InitializeComponents()
		{
			//UploadDialogTextBox
			using (UploadDialogTextBox upload = new UploadDialogTextBox())
			{
				upload.AllowEditTextBox = true;
				upload.Width = new Unit(controlWidth);
				upload.CssClass = "NormalTextBox";

				innerControl = upload;
			}
		}

		public override Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				UploadDialogTextBox upload = (UploadDialogTextBox) innerControl;
				upload.UploadDirectory = PortalPathPrefix;
				upload.Text = Value;

				//Return control
				return innerControl;
			}
			set
			{
				if(value.GetType().Name == "UploadDialogTextBox")
				{
					innerControl = value;
					//Update value from control
					UploadDialogTextBox upload = (UploadDialogTextBox) innerControl;
					Value = upload.Text;
				}
				else
					throw new ArgumentException("A UploadDialogTextBox values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}

		public override string Value
		{
			get
			{
				return(innerValue);
			}
			set
			{
				//Remove portal path if present
				if(value.StartsWith(PortalPathPrefix))
					innerValue = value.Substring(PortalPathPrefix.Length);
				else
					innerValue = value;

				innerValue = innerValue.TrimStart('/');
			}
		}
	}
}