using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rainbow.Design
{
	/// <summary>
	/// The Theme class encapsulates all the settings
	/// of the currently selected theme
	/// </summary>
	[History("bja", "2003/04/26", "C1: [Future] Added minimize color for title bar")]
	public class Theme
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public const string DefaultButtonPath = "~/Design/Themes/Default/icon";

		private Hashtable images = new Hashtable();

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public Hashtable Images
		{
			get { return images; }
			set { images = value; }
		}


		private Hashtable parts = new Hashtable();

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public Hashtable Parts
		{
			get { return parts; }
			set { parts = value; }
		}


		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string _Css = "Portal.css";

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string _minimize_color = string.Empty; //(FUTURE) [bja:C1]

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string _name;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string _webPath;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string type = "classic";

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string GetHTMLPart(string name)
		{
			//			string html = GetThemePart(name);
			//			string w = String.Concat(WebPath, "/");
			//			html = html.Replace("src='", String.Concat("src='", w));
			//			html = html.Replace("src=\"", String.Concat("src=\"", w));
			//			html = html.Replace("background='", String.Concat("background='", w));
			//			html = html.Replace("background=\"", String.Concat("background=\"", w));
			//			return html;
			return GetThemePart(name);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="defaultImagePath" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Web.UI.WebControls.Image value...
		/// </returns>
		public Image GetImage(string name, string defaultImagePath)
		{
			Image img;

			if (this.Images.ContainsKey(name))
			{
				img = ((ThemeImage) this.Images[name]).GetImage();
				img.ImageUrl = Settings.Path.WebPathCombine(WebPath, img.ImageUrl);
			}

			else
			{
				img = new Image();
				img.ImageUrl = Settings.Path.WebPathCombine(DefaultButtonPath.Replace("~", Settings.Path.ApplicationRoot), defaultImagePath);
			}
			return img;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Web.UI.WebControls.Image value...
		/// </returns>
		[Obsolete("You are strongly invited to use the new overload the takes default as parameter")]
		public Image GetImage(string name)
		{
			return GetImage(name, "NoImage.gif");
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Web.UI.LiteralControl value...
		/// </returns>
		public LiteralControl GetLiteralControl(string name)
		{
			return new LiteralControl(GetHTMLPart(name));
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="name" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="defaultImagePath" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>		
		public string GetLiteralImage(string name, string defaultImagePath)
		{
			Image img = GetImage(name, defaultImagePath);
			return "<img src='" + img.ImageUrl + "' width='" + img.Width.ToString() + "' height='" + img.Height.ToString() + "'>";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>
		/// added: Jes1111 - 2004/08/27
		/// Part of Zen support
		/// </remarks>
		public string GetThemePart(string name)
		{
			if (this.Parts.ContainsKey(name))
			{
				ThemePart part = (ThemePart) this.Parts[name];
				return part.HTML;
			}

			else
				return string.Empty;
		}

		/// <summary>
		/// Get the Css file name without any path.
		/// </summary>
		public string Css
		{
			get { return _Css; }
			set { _Css = value; }
		}

		/// <summary>
		/// Get the Css phisical file name.
		/// Set at runtime using Web Path.
		/// </summary>
		public string CssFile
		{
			get { return Settings.Path.WebPathCombine(WebPath, _Css); }
		}

		/// <summary>
		/// [START FUTURE bja:C1]
		/// The Theme minimize color 
		/// </summary>
		public string MinimizeColor
		{
			get { return _minimize_color; }
			set { _minimize_color = value; }
		} //end of MinimizeColor

		/// <summary>
		/// The Theme Name (must be the directory in which is located)
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Current Phisical Path. Readonly.
		/// </summary>
		public string Path
		{
			get { return (HttpContext.Current.Server.MapPath(WebPath)); }
		}

		/// <summary>
		/// Get the Theme physical file name.
		/// Set at runtime using Physical Path. NonSerialized.
		/// </summary>
		public string ThemeFileName
		{
			get
			{
				if (WebPath == string.Empty)
					throw new ArgumentNullException("Path", "Value cannot be null!");
				//Try to get current theme from public folder
				return System.IO.Path.Combine(Path, "Theme.xml");
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public string Type
		{
			get { return type.ToLower(); }
			set { type = value.ToLower(); }
		}

		/// <summary>
		/// Current Web Path.
		/// It is set at runtime and therefore is not serialized
		/// </summary>
		public string WebPath
		{
			get { return _webPath; }
			set { _webPath = value; }
		}
	}
}