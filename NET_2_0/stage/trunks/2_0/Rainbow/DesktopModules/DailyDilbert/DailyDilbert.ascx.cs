using System;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.UI.DataTypes;
using Rainbow.UI.WebControls;

namespace Rainbow.DesktopModules 
{

	/// <summary>
	/// DailyDilbert Module
	/// Based on VB Module Written by SnowCovered.com
	/// Modifications and conversion for C# IBS Portal (c)2002 by Christopher S Judd, CDP
	/// email- dotNet@HorizonsLLC.com    web- www.HorizonsLLC.com/IBS
	/// Modifications and conversion for Rainbow Jakob hansen
	/// </summary>
	public class DailyDilbert : PortalModuleControl 
	{
		/// <summary>
		/// 
		/// </summary>
		protected HyperLink imgDilbert;

		/// <summary>
		/// The Page_Load event handler on this User Control is used to
		/// call the web service function that get the Dilbert Image
		/// then sets the image for the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			// Set the URl for the image
			imgDilbert.ImageUrl = this.TemplateSourceDirectory + "/DailyDilbertImage.aspx?mID=" + ModuleID.ToString();
			imgDilbert.NavigateUrl = this.TemplateSourceDirectory + "/DailyDilbertImage.aspx?mID=" + ModuleID.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531031}");
			}
		}

		/// <summary>
		/// Public constructor. Sets base settings for module.
		/// </summary>
		public DailyDilbert() 
		{
			SettingItem setImagePercent = new SettingItem(new IntegerDataType());
			setImagePercent.Required = true;
			setImagePercent.Value = "80";
			setImagePercent.Order = 1;
			setImagePercent.MinValue = 1;
			setImagePercent.MaxValue = 100;
			this._baseSettings.Add("ImagePercent", setImagePercent);
		}

		#region Web Form Designer generated code
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
//			ModuleTitle = new DesktopModuleTitle();
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
