using System;
using System.Threading;
using System.Web.UI.WebControls;
using Rainbow.Configuration;
using Rainbow.UI.WebControls;

namespace Rainbow.DesktopModules
{
	/// <summary>
	/// Rainbow Version
	/// </summary>
	public class RainbowVersion : PortalModuleControl
	{
		/// <summary>
		/// 
		/// </summary>
		protected Label currentLanguage;
		/// <summary>
		/// 
		/// </summary>
		protected Label currentUILanguage;
		/// <summary>
		/// 
		/// </summary>
		protected Label VersionLabel;

		private void RainbowVersion_Load(object sender, EventArgs e)
		{
			VersionLabel.Text = PortalSettings.ProductVersion;
			currentLanguage.Text = Thread.CurrentThread.CurrentCulture.Name;
			currentUILanguage.Text = Thread.CurrentThread.CurrentUICulture.Name;
		}
		/// <summary>
		/// 
		/// </summary>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{72C6F60A-50C4-4f20-8F89-3E8A27820557}");
			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Raises Init event
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
			this.Load += new EventHandler(this.RainbowVersion_Load);

		}
		#endregion
	}
}