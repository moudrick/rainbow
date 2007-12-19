using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using Esperantus;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Settings.Cache;
// using System.Diagnostics;
namespace Rainbow.Design
{

	/// <summary>
	/// The ThemeManager class encapsulates all data logic necessary to
	/// use differents themes across the entire portal.
	/// Manages the Load and Save of the Themes.
	/// Encapsulates a Theme object that contains all the settings
	/// of the current Theme.
	/// </summary>
	public class ThemeManager
	{

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public Theme CurrentTheme = new Theme();
		/// <summary>
		/// 
		/// </summary>
		string _portalPath;
		/// <summary>
		///     
		/// </summary>
		/// <param name="portalPath" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public ThemeManager(string portalPath)
		{
			_portalPath = portalPath;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public void ClearCacheList()
		{
			//Clear cache
			CurrentCache.Remove(Key.ThemeList(Path));
			CurrentCache.Remove(Key.ThemeList(PortalThemePath));
		}

		/// <summary>
		/// Read the Path dir and returns an ArrayList with all the Themes found.
		/// Static because the list is Always the same.
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetPublicThemes()
		{
			ArrayList baseThemeList;

			if (!CurrentCache.Exists (Key.ThemeList(Path)))
			{
				//Initialize array
				baseThemeList = new ArrayList();
				string[] themes;

				// Try to read directories from public theme path
				if(Directory.Exists(Path))
				{
					themes = Directory.GetDirectories(Path);
				}

				else
				{
					themes = new string[0];
				}

				for (int i = 0; i < themes.Length; i++)
				{
					ThemeItem t = new ThemeItem();
					t.Name = themes[i].Substring(Path.Length + 1);

					if(t.Name != "CVS" && t.Name != "_svn") //Ignore CVS and _svn folders
						baseThemeList.Add(t);
				}
				CurrentCache.Insert(Key.ThemeList(Path), baseThemeList, new CacheDependency(Path));
			}

			else
			{
				baseThemeList = (ArrayList) CurrentCache.Get (Key.ThemeList(Path));
			}
			return baseThemeList;
		}

		/// <summary>
		/// Read the Path dir and returns
		/// an ArrayList with all the Themes found, public and privates
		/// </summary>
		/// <returns></returns>
		public ArrayList GetPrivateThemes()
		{
			ArrayList privateThemeList;

			if ( !CurrentCache.Exists(Key.ThemeList(PortalThemePath)) )
			{
 
				privateThemeList = new ArrayList();
				string[] themes;

				// Try to read directories from private theme path
				if (Directory.Exists(PortalThemePath))
				{
					themes = Directory.GetDirectories(PortalThemePath);
				}

				else
				{
					themes = new string[0];
				}

				for (int i = 0; i <= themes.GetUpperBound(0); i++)
				{
					ThemeItem t = new ThemeItem();
					t.Name = themes[i].Substring(PortalThemePath.Length + 1);

					if(t.Name != "CVS" && t.Name != "_svn") //Ignore CVS
						privateThemeList.Add(t);
				}
				
				CurrentCache.Insert(Key.ThemeList(PortalThemePath), privateThemeList, new CacheDependency(PortalThemePath));
				//Debug.WriteLine("Storing privateThemeList in Cache: item count is " + privateThemeList.Count.ToString());

			}
			else
			{
				privateThemeList = (ArrayList) CurrentCache.Get(Key.ThemeList(PortalThemePath));
				//Debug.WriteLine("Retrieving privateThemeList from Cache: item count is " + privateThemeList.Count.ToString());
			}
			return privateThemeList;
		}

		/// <summary>
		/// Read the Path dir and returns
		/// an ArrayList with all the Themes found, public and privates
		/// </summary>
		/// <returns></returns>
		public ArrayList GetThemes()
		{
			ArrayList themeList;
			ArrayList themeListPrivate;

			themeList = (ArrayList) GetPublicThemes().Clone();
			themeListPrivate = GetPrivateThemes();
			
			themeList.AddRange(themeListPrivate);
            
			return themeList;
		}


		/// <summary>
		///     
		/// </summary>
		/// <param name="ThemeName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void Load(string ThemeName)
		{
			CurrentTheme = new Theme();
			CurrentTheme.Name = ThemeName;

			//Try loading private theme first
			if(LoadTheme(Settings.Path.WebPathCombine(PortalWebPath, ThemeName)))
				return;

			//Try loading public theme
			if(LoadTheme(Settings.Path.WebPathCombine(WebPath, ThemeName))) 
				return;
			//Try default
			CurrentTheme.Name = "default";

			if(LoadTheme(Settings.Path.WebPathCombine(WebPath, "default")))
				return;
			string errormsg = Localize.GetString("LOAD_THEME_ERROR");
			throw new FileNotFoundException(errormsg.Replace("%1%", "'" + ThemeName + "'"), WebPath + "/" + ThemeName);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="key" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="cacheItem" type="object">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="reason" type="System.Web.Caching.CacheItemRemovedReason">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public static void OnRemove(string key, object cacheItem, CacheItemRemovedReason reason)
		{
			LogHelper.Logger.Log(LogLevel.Info, "The cached value with key '" + key + "' was removed from the cache.  Reason: " + reason.ToString());
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="ThemeName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void Save(string ThemeName)
		{
			CurrentTheme.Name = ThemeName;
			CurrentTheme.WebPath = Settings.Path.WebPathCombine(WebPath, ThemeName);		
			XmlSerializer serializer = new XmlSerializer(typeof(Theme));

			// Create an XmlTextWriter using a FileStream.
			using (Stream fs = new FileStream(CurrentTheme.ThemeFileName, FileMode.Create))
			{
				XmlWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
				// Serialize using the XmlTextWriter.
				serializer.Serialize(writer, CurrentTheme);
				writer.Close();
				//Release the file
				writer = null;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="CurrentWebPath" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		private bool LoadTheme(string CurrentWebPath)
		{
			CurrentTheme.WebPath = CurrentWebPath;

			//if (!Rainbow.Settings.Cache.CurrentCache.Exists (Rainbow.Settings.Cache.Key.CurrentTheme(CurrentWebPath)))
			if (!CurrentCache.Exists (Key.CurrentTheme(CurrentTheme.Path)))
			{

				if(File.Exists(CurrentTheme.ThemeFileName))
				{

					if ( LoadXml(CurrentTheme.ThemeFileName) )
					{
						//Rainbow.Settings.Cache.CurrentCache.Insert(Rainbow.Settings.Cache.Key.CurrentTheme(CurrentWebPath), CurrentTheme, new CacheDependency(CurrentTheme.ThemeFileName));
						CurrentCache.Insert(Key.CurrentTheme(CurrentTheme.Path), CurrentTheme, new CacheDependency(CurrentTheme.Path));
					}

					else
					{
						// failed
						return false;
					}
				}

				else
				{
					//Return fail
					return false;
				}
			}

			else
			{
				//CurrentTheme = (Theme) Rainbow.Settings.Cache.CurrentCache.Get (Rainbow.Settings.Cache.Key.CurrentTheme(CurrentWebPath));
				CurrentTheme = (Theme) CurrentCache.Get (Key.CurrentTheme(CurrentTheme.Path));
			}
			CurrentTheme.WebPath = CurrentWebPath;
			return true;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="filename" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		private bool LoadXml(string filename)
		{
			XmlTextReader _xtr = null;
			NameTable _nt = new NameTable();
			XmlNamespaceManager _nsm = new XmlNamespaceManager(_nt);
			_nsm.AddNamespace(string.Empty,"http://www.w3.org/1999/xhtml");
			XmlParserContext _context = new XmlParserContext(_nt, _nsm, string.Empty, XmlSpace.None);
			bool returnValue = false;

			try
			{

				// Create an XmlTextReader using a FileStream.
				using (Stream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				{

					try
					{
						_xtr = new XmlTextReader(fs, XmlNodeType.Document, _context);
						_xtr.WhitespaceHandling = WhitespaceHandling.None;
						ThemeImage _myImage;
						ThemePart _myPart = new ThemePart();

						while ( !_xtr.EOF )
						{

							if ( _xtr.MoveToContent() == XmlNodeType.Element )
							{

								switch ( _xtr.LocalName )
								{

									case "Name" :
										this.CurrentTheme.Name = _xtr.ReadString();
										break;

									case "Type" :
										this.CurrentTheme.Type = _xtr.ReadString();
										break;

									case "Css" :
										this.CurrentTheme.Css = _xtr.ReadString();
										break;

									case "MinimizeColor" :
										this.CurrentTheme.MinimizeColor = _xtr.ReadString();
										break;

									case "ThemeImage" :
										_myImage = new ThemeImage();

										while ( _xtr.MoveToNextAttribute() )
										{

											switch ( _xtr.LocalName )
											{

												case "Name" :
													_myImage.Name = _xtr.Value;
													break;

												case "ImageUrl" :
													_myImage.ImageUrl = _xtr.Value;
													break;

												case "Width" :
													_myImage.Width = double.Parse(_xtr.Value);
													break;

												case "Height" :
													_myImage.Height = double.Parse(_xtr.Value);
													break;
												default :
													break;
											}
										}
										this.CurrentTheme.ThemeImages.Add(_myImage.Name,_myImage);
										_xtr.MoveToElement();
										break;

									case "ThemePart" :
										_myPart = new ThemePart();

										while ( _xtr.MoveToNextAttribute() )
										{

											switch ( _xtr.LocalName )
											{

												case "Name" :
													_myPart.Name = _xtr.Value;
													break;
												default :
													break;
											}
										}
										_xtr.MoveToElement();
										break;

									case "HTML" :

										if ( _myPart.Name.Length != 0 )
											_myPart.Html = _xtr.ReadString();
										//Moved here on load instead on retrival.
										//by Manu
										string w = string.Concat(CurrentTheme.WebPath, "/");
										_myPart.Html = _myPart.Html.Replace("src='", string.Concat("src='", w));
										_myPart.Html = _myPart.Html.Replace("src=\"", string.Concat("src=\"", w));
										_myPart.Html = _myPart.Html.Replace("background='", string.Concat("background='", w));
										_myPart.Html = _myPart.Html.Replace("background=\"", string.Concat("background=\"", w));
										this.CurrentTheme.ThemeParts.Add(_myPart.Name, _myPart);
										break;
									default :
										//Debug.WriteLine(" - unwanted");
										break;
								}
							}
							_xtr.Read();
						}
						returnValue = true;
					}

					catch(Exception ex)
					{
						LogHelper.Logger.Log(LogLevel.Error, "Failed to Load XML Theme : " + filename + " Message was: " + ex.Message);
					}

					finally
					{
						fs.Close();
					}
				}
			}

			catch(Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Error, "Failed to open XML Theme : " + filename + " Message was: " + ex.Message);
			}
			return returnValue;
		}

		/// <summary>
		/// The path of the Theme dir (Phisical path)
		/// used ot load Themes
		/// </summary>
		public static string Path
		{
			get
			{
				return(HttpContext.Current.Server.MapPath(WebPath));
			}
		}

		/// <summary>
		/// The path of the current portal Theme dir (Phisical path)
		/// used to load Themes
		/// </summary>
		public string PortalThemePath
		{
			get
			{
				return (HttpContext.Current.Server.MapPath(PortalWebPath));
			}
		}

		/// <summary>
		/// The path of the current portal Theme dir (Web side)
		/// used to reference images
		/// </summary>
		public string PortalWebPath
		{
			get
			{
				string myPortalWebPath = Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, _portalPath, "/Themes");
				return myPortalWebPath;
			}
		}

		/// <summary>
		/// The path of the Theme dir (Web side)
		/// used to reference images
		/// </summary>
		public static string WebPath
		{
			get
			{
				return Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, "/Design/Themes");
			}
		}
	}
}
