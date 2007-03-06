using System;
using System.Web;

namespace Rainbow.Design
{
	/// <summary>
	/// The Layout class encapsulates all the settings
	/// of the currently selected layout
	/// </summary>
	/// <remarks>by Cory Isakson</remarks>
	public class Layout
	{
		private string _name;

		/// <summary>
		/// The Layout Name (must be the directory in which is located)
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[NonSerialized()] private string webPath;

		/// <summary>
		/// Current Web Path.
		/// It is set at runtime and therefore is not serialized
		/// </summary>
		public string WebPath
		{
			get { return webPath; }
			set { webPath = value; }
		}


		/// <summary>
		/// Current Phisical Path. Readonly.
		/// </summary>
		public string Path
		{
			get { return (HttpContext.Current.Server.MapPath(WebPath)); }
		}
	}
}