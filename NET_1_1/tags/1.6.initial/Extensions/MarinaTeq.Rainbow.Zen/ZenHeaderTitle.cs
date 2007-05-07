using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;

using Rainbow;
using Rainbow.UI;
using Rainbow.UI.WebControls;
using Rainbow.Configuration;

namespace Rainbow.UI.WebControls
{
	/// <summary>
	/// Summary description for ZenHeaderTitle.
	/// </summary>
	public class ZenHeaderTitle : Rainbow.UI.WebControls.HeaderTitle
	{
		/// <summary>
		/// 
		/// </summary>
		protected string _imageUrl = string.Empty;
		/// <summary>
		/// 
		/// </summary>
		protected bool _showImage = true;

		/// <summary>
		/// 
		/// </summary>
		public virtual bool ShowImage
		{
			get{return _showImage;}
			set{_showImage = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual string ImageUrl
		{
			get{return _imageUrl;}
			set{_imageUrl = value;}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ZenHeaderTitle()
		{
			this.EnableViewState = false; 
			this.Load += new System.EventHandler(this.LoadControl);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void LoadControl(object sender, System.EventArgs e)
		{
			if(HttpContext.Current != null)
			{
				// Obtain PortalSettings from Current Context
				PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

				// PortalTitle                
				this.Text = portalSettings.PortalName;
			}
		}

		/// <summary>
		/// Overrides Render to produce structure suitable for Zen
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			if ( ShowImage )
				// show image
				//<h1 id="portaltitle" class="portaltitle">Rainbow Portal<span></span></h1>
			{
				writer.Write("<h1 id=\"portaltitle\" class=\"portaltitle\">");
				writer.Write(this.Text);
				writer.Write("<span></span></h1>");
			}
			else
				// show text 
				//<h1 class="portaltitle">Rainbow Portal</h1>
			{
				writer.Write("<h1 class=\"portaltitle\">");
				writer.Write(this.Text);
				writer.Write("</h1>");
			}
		}
	}
}
