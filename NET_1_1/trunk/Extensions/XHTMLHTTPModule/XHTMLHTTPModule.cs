using System;
using System.Web;

namespace AspNetResources.Web
{
	/// <summary>
	/// 
	/// </summary>
	public class XHTMLHTTPModule : IHttpModule
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		public void Init(HttpApplication app)
		{
			app.ReleaseRequestState += new EventHandler(InstallResponseFilter);
		}


		/// <summary>
		/// Use this event to wire a page filter.
		/// </summary>
		private void InstallResponseFilter(object sender, EventArgs e)
		{
			HttpResponse response = HttpContext.Current.Response;

			if (response.ContentType == "text/html")
				response.Filter = new PageFilterStream(response.Filter);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
		}
	}
}