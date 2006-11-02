using System.Web.UI.WebControls;

namespace Rainbow.DesktopModules.Sitemap
{
	/// <summary>
	/// This defines an interface for a Sitemap renderer.
	/// </summary>
	public interface ISitemapRenderer
	{
		
		/// <summary>
		/// The Render interface function
		/// </summary>
		WebControl Render(SitemapItems list);
	}
}
