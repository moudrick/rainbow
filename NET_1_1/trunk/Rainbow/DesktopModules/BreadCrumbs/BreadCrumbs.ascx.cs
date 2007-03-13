using System;
using Rainbow.UI.WebControls;

namespace Rainbow.DesktopModules 
{

	/// <summary>
	/// BreadCrumbs Module
	/// </summary>
	public class BreadCrumbs : PortalModuleControl 
	{
		protected UI.WebControls.BreadCrumbs BreadCrumbs1;
		/// <summary>
		/// Public constructor. Sets base settings for module.
		/// </summary>
		public BreadCrumbs() 
		{
		}

		/// <summary>
		/// The Page_Load event handler on this User Control is used to
		/// databind the used BreadCrumbsControl.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			//BreadCrumbs1.DataBind();
		}


		#region general Module Implementation
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{D3182CD6-DAFF-4E72-AD9E-0B28CB44F007}");
			}
		}

	
		#endregion


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
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
