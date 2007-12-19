using System.Collections;
using System.IO;
using System.Web;

namespace Rainbow.Design
{
	/// <summary>
	/// The LayoutManager class encapsulates all data logic necessary to
	/// use differents Layouts across the entire portal.
	/// Manages the Load and Save of the Layouts.
	/// Encapsulates a Layout object that contains all the settings
	/// of the current Layout.
	/// </summary>
	/// <remarks>by Cory Isakson</remarks>
	public class LayoutManager
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private string _portalPath;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private static ArrayList cachedLayoutsList;

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
		public LayoutManager(string portalPath)
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
			lock (typeof (LayoutManager))
			{
				LayoutManager.cachedLayoutsList = null;
				CurrentCache.Remove(Key.LayoutList(PortalLayoutPath));
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private static string innerWebPath;

		/// <summary>
		/// The path of the Layout dir (Web side)
		/// used to reference images
		/// </summary>
		public static string WebPath
		{
			get
			{
				if (innerWebPath == null)
					innerWebPath = Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, "/Design/DesktopLayouts");
				return innerWebPath;
			}
		}

		/// <summary>
		/// The path of the current portal Layout dir (Web side)
		/// used to reference images
		/// </summary>
		public string PortalWebPath
		{
			get
			{
				// FIX by George James (ghjames)
				// http://sourceforge.net/tracker/index.php?func=detail&aid=735716&group_id=66837&atid=515929
				return Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, _portalPath, "/DesktopLayouts");
			}
		}

		/// <summary>
		/// The path of the Layout dir (Phisical path)
		/// used to load Layouts
		/// </summary>
		public static string Path
		{
			get { return (HttpContext.Current.Server.MapPath(WebPath)); }
		}

		/// <summary>
		/// The path of the current portal Layout dir (Phisical path)
		/// used ot load Layouts
		/// </summary>
		public string PortalLayoutPath
		{
			get { return (HttpContext.Current.Server.MapPath(PortalWebPath)); }
		}

		/// <summary>
		/// Read the Path dir and returns
		/// an ArrayList with all the Layouts found, public and privates
		/// </summary>
		/// <returns></returns>
		public ArrayList GetLayouts()
		{
			//Initialize array
			ArrayList layoutsList;

			if (!CurrentCache.Exists(Key.LayoutList(PortalLayoutPath)))
			{
				//Initialize array
				//It is very important to use the clone here 
				//or we get duplicated Custom list each time
				layoutsList = (ArrayList) GetPublicLayouts().Clone();

				string[] layouts;

				// Try to read directories from private Layout path
				if (Directory.Exists(PortalLayoutPath))
					layouts = Directory.GetDirectories(PortalLayoutPath);
				else
					layouts = new string[0];

				for (int i = 0; i <= layouts.GetUpperBound(0); i++)
				{
					LayoutItem t = new LayoutItem();
					t.Name = layouts[i].Substring(PortalLayoutPath.Length + 1);
					if (t.Name != "CVS") //Ignore CVS
						layoutsList.Add(t);
				}

				CurrentCache.Insert(Key.LayoutList(PortalLayoutPath), layoutsList);
			}
			else
				layoutsList = (ArrayList) CurrentCache.Get(Key.LayoutList(PortalLayoutPath));
			return layoutsList;
		}

		/// <summary>
		/// Read the Path dir and returns an ArrayList with all the Layouts found.
		/// Static because the list is Always the same.
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetPublicLayouts()
		{
			if (LayoutManager.cachedLayoutsList == null)
			{
				//Initialize array
				ArrayList layoutsList = new ArrayList();

				string[] layouts;

				// Try to read directories from public Layout path
				if (Directory.Exists(Path))
					layouts = Directory.GetDirectories(Path);
				else
					layouts = new string[0];

				for (int i = 0; i < layouts.Length; i++)
				{
					LayoutItem t = new LayoutItem();
					t.Name = layouts[i].Substring(Path.Length + 1);
					if (t.Name != "CVS") //Ignore CVS
						layoutsList.Add(t);
				}

				//store list in cache
				lock (typeof (LayoutManager))
				{
					if (LayoutManager.cachedLayoutsList == null)
						LayoutManager.cachedLayoutsList = layoutsList;
				}
			}

			return LayoutManager.cachedLayoutsList;
		}
	}
}