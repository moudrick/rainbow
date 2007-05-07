using System;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.Settings;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;

namespace Rainbow.DesktopModules 
{
	/// <summary>
	/// ImageModule
	/// Display an image on screen
	/// </summary>
	public class ImageModule : PortalModuleControl 
	{
		/// <summary>
		/// The Image
		/// </summary>
		protected Image Image1;

		/// <summary>
		/// The Page_Load event handler on this User Control uses
		/// the Portal configuration system to obtain image details.
		/// It then sets these properties on an &lt;asp:Image&gt; server control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e) 
		{
			string imageSrc = Path.WebPathCombine(Path.ApplicationRoot, portalSettings.PortalPath, Settings["src"].ToString());
			string imageHeight = Settings["height"].ToString();
			string imageWidth = Settings["width"].ToString();

			// Set Image Source, Width and Height Properties
			if ((imageSrc != null) && (imageSrc.Length != 0)) 
			{
				Image1.ImageUrl = imageSrc;
			}

			if ((imageWidth != null) && (imageWidth.Length > 0) && (int.Parse(imageWidth) > 0)) 
			{
				Image1.Width = int.Parse(imageWidth);
			}

			if ((imageHeight != null) && (imageHeight.Length > 0) && (int.Parse(imageHeight) > 0)) 
			{
				Image1.Height = int.Parse(imageHeight);
			}
		}
 
		/// <summary>
		/// General Module GUID
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{BCF1F338-4564-461C-9606-CB024D10294E}");
			}
		}

		/// <summary>
		/// Constructor
		/// Module Settings
		/// <list type="">
		/// <item>src</item>
		/// <item>width</item>
		/// <item>height</item>
		/// </list>
		/// </summary>
		public ImageModule() 
		{                
			// modified by Hongwei Shen
			SettingItemGroup group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			int groupBase = (int)group;

			SettingItem src = new SettingItem(new UploadedFileDataType()); //PortalUrlDataType
			src.Required = true;
			src.Group = group;
			src.Order = groupBase + 25; //1;
			this._baseSettings.Add("src", src);

			SettingItem width = new SettingItem(new IntegerDataType());
			width.Required = true;
			width.MinValue = 0;
			width.MaxValue = 2048;
			width.Value = "150";
			width.Group = group;
			width.Order = groupBase + 30; //2;
			this._baseSettings.Add("width", width);

			SettingItem height = new SettingItem(new IntegerDataType());
			height.Required = true;
			height.MinValue = 0;
			height.MaxValue = 2048;
			height.Value = "250";
			height.Group = group;
			height.Order = groupBase + 35; //1;
			this._baseSettings.Add("height", height);
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Raises Init event
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			// Create a new Title the control
//			ModuleTitle = new DesktopModuleTitle();
			// Add title ad the very beginning of 
			// the control's controls collection
//			Controls.AddAt(0, ModuleTitle);
		
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
	}
}
