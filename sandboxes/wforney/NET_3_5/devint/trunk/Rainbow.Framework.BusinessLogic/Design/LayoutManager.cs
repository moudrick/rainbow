namespace Rainbow.Framework.Design
{
    using System.Collections;
    using System.IO;
    using System.Web;
    using System.Web.Caching;

    using Rainbow.Framework.Configuration.Cache;

    /// <summary>
    /// The LayoutManager class encapsulates all data logic necessary to
    ///     use differents Layouts across the entire portal.
    ///     Manages the Load and Save of the Layouts.
    ///     Encapsulates a Layout object that contains all the settings
    ///     of the current Layout.
    /// </summary>
    /// <remarks>
    /// by Cory Isakson
    /// </remarks>
    public class LayoutManager
    {
        #region Constants and Fields

        /// <summary>
        /// The portal path.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private readonly string portalPath;

        // Jes1111 - not needed for new version ... see below
        // 		/// <summary>
        // 		///     
        // 		/// </summary>
        // 		/// <remarks>
        // 		///     
        // 		/// </remarks>
        // 		private static ArrayList cachedLayoutsList;

        /// <summary>
        /// The inner web path.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private static string innerWebPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutManager"/> class. 
        /// The layout manager.
        /// </summary>
        /// <param name="portalPath" type="string">
        /// <para>
        /// </para>
        /// </param>
        /// <returns>
        /// A void value...
        /// </returns>
        public LayoutManager(string portalPath)
        {
            this.portalPath = portalPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The path of the Layout dir (Phisical path)
        ///     used to load Layouts
        /// </summary>
        public static string Path
        {
            get
            {
                return HttpContext.Current.Server.MapPath(WebPath);
            }
        }

        /// <summary>
        ///     The path of the Layout dir (Web side)
        ///     used to reference images
        /// </summary>
        public static string WebPath
        {
            get
            {
                return innerWebPath ??
                       (innerWebPath =
                        Configuration.Path.WebPathCombine(Configuration.Path.ApplicationRoot, "/Design/DesktopLayouts"));
            }
        }

        /// <summary>
        ///     The path of the current portal Layout dir (Phisical path)
        ///     used ot load Layouts
        /// </summary>
        public string PortalLayoutPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(this.PortalWebPath);
            }
        }

        /// <summary>
        ///     The path of the current portal Layout dir (Web side)
        ///     used to reference images
        /// </summary>
        public string PortalWebPath
        {
            get
            {
                // FIX by George James (ghjames)
                // http://sourceforge.net/tracker/index.php?func=detail&aid=735716&group_id=66837&atid=515929
                return Configuration.Path.WebPathCombine(
                    Configuration.Path.ApplicationRoot, this.portalPath, "/DesktopLayouts");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read the Path dir and returns an ArrayList with all the Layouts found.
        ///     Static because the list is Always the same.
        /// </summary>
        /// <returns>
        /// </returns>
        public static ArrayList GetPublicLayouts()
        {
            // Jes1111 - 27-02-2005 - new version - correct caching
            ArrayList baseLayoutList;

            if (!CurrentCache.Exists(Key.LayoutList(Path)))
            {
                // initialize array
                baseLayoutList = new ArrayList();
                string[] layouts;

                // Try to read directories from public Layout path
                if (Directory.Exists(Path))
                {
                    layouts = Directory.GetDirectories(Path);
                }
                else
                {
                    layouts = new string[0];
                }

                foreach (var t1 in layouts)
                {
                    var t = new LayoutItem();
                    t.Name = t1.Substring(Path.Length + 1);
                    if (t.Name != "CVS" && t.Name != "_svn")
                    {
                        // Ignore CVS
                        baseLayoutList.Add(t);
                    }
                }

                CurrentCache.Insert(Key.LayoutList(Path), baseLayoutList, new CacheDependency(Path));
            }
            else
            {
                baseLayoutList = (ArrayList)CurrentCache.Get(Key.LayoutList(Path));
            }

            return baseLayoutList;

            // Jes1111 - old version
            // 			if (LayoutManager.cachedLayoutsList == null)
            // 			{
            // 				//Initialize array
            // 				ArrayList layoutsList = new ArrayList();
            // 				string[] layouts;
            // 				// Try to read directories from public Layout path
            // 				if (Directory.Exists(Path))
            // 				{
            // 					layouts = Directory.GetDirectories(Path);
            // 				}
            // 				else
            // 				{
            // 					layouts = new string[0];
            // 				}
            // 				for (int i = 0; i < layouts.Length; i++)
            // 				{
            // 					LayoutItem t = new LayoutItem();
            // 					t.Name = layouts[i].Substring(Path.Length + 1);
            // 					if(t.Name != "CVS") //Ignore CVS
            // 						layoutsList.Add(t);
            // 				}
            // 				//store list in cache
            // 				lock (typeof(LayoutManager))
            // 				{
            // 					if (LayoutManager.cachedLayoutsList == null) 
            // 					{
            // 						LayoutManager.cachedLayoutsList = layoutsList;
            // 					}
            // 				}
            // 			}
            // 			return LayoutManager.cachedLayoutsList;
        }

        /// <summary>
        /// The clear cache list.
        /// </summary>
        public void ClearCacheList()
        {
            // Clear cache
            lock (typeof(LayoutManager))
            {
                // Jes1111
                // LayoutManager.cachedLayoutsList = null;
                CurrentCache.Remove(Key.LayoutList(Path));
                CurrentCache.Remove(Key.LayoutList(this.PortalLayoutPath));
            }
        }

        /// <summary>
        /// Read the Path dir and returns
        ///     an ArrayList with all the Layouts found, public and privates
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetLayouts()
        {
            // Jes1111 - 27-02-2005 - new version - correct caching
            ArrayList layoutList;
            ArrayList layoutListPrivate;

            layoutList = (ArrayList)GetPublicLayouts().Clone();
            layoutListPrivate = this.GetPrivateLayouts();

            layoutList.AddRange(layoutListPrivate);

            return layoutList;

            // Jes1111 - old version
            // 			//Initialize array
            // 			ArrayList layoutsList;
            // 			if (!Rainbow.Framework.Settings.Cache.CurrentCache.Exists (Rainbow.Framework.Settings.Cache.Key.LayoutList(PortalLayoutPath)))
            // 			{
            // 				//Initialize array
            // 				//It is very important to use the clone here 
            // 				//or we get duplicated Custom list each time
            // 				layoutsList = (ArrayList) GetPublicLayouts().Clone();
            // 				string[] layouts;
            // 				// Try to read directories from private Layout path
            // 				if (Directory.Exists(PortalLayoutPath))
            // 				{
            // 					layouts = Directory.GetDirectories(PortalLayoutPath);
            // 				}
            // 				else
            // 				{
            // 					layouts = new string[0];
            // 				}
            // 				for (int i = 0; i <= layouts.GetUpperBound(0); i++)
            // 				{
            // 					LayoutItem t = new LayoutItem();
            // 					t.Name = layouts[i].Substring(PortalLayoutPath.Length + 1);
            // 					if(t.Name != "CVS") //Ignore CVS
            // 						layoutsList.Add(t);
            // 				}
            // 				Rainbow.Framework.Settings.Cache.CurrentCache.Insert (Rainbow.Framework.Settings.Cache.Key.LayoutList(PortalLayoutPath), layoutsList);
            // 			}
            // 			else
            // 			{
            // 				layoutsList = (ArrayList) Rainbow.Framework.Settings.Cache.CurrentCache.Get (Rainbow.Framework.Settings.Cache.Key.LayoutList(PortalLayoutPath));
            // 			}
            // 			return layoutsList;
        }

        /// <summary>
        /// The get private layouts.
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetPrivateLayouts()
        {
            ArrayList privateLayoutList;

            if (!CurrentCache.Exists(Key.LayoutList(this.PortalLayoutPath)))
            {
                privateLayoutList = new ArrayList();
                string[] layouts;

                // Try to read directories from private theme path
                if (Directory.Exists(this.PortalLayoutPath))
                {
                    layouts = Directory.GetDirectories(this.PortalLayoutPath);
                }
                else
                {
                    layouts = new string[0];
                }

                for (var i = 0; i <= layouts.GetUpperBound(0); i++)
                {
                    var t = new LayoutItem();
                    t.Name = layouts[i].Substring(this.PortalLayoutPath.Length + 1);

                    if (t.Name != "CVS" && t.Name != "_svn")
                    {
                        // Ignore CVS
                        privateLayoutList.Add(t);
                    }
                }

                CurrentCache.Insert(
                    Key.LayoutList(this.PortalLayoutPath), privateLayoutList, new CacheDependency(this.PortalLayoutPath));
            }
            else
            {
                privateLayoutList = (ArrayList)CurrentCache.Get(Key.LayoutList(this.PortalLayoutPath));
            }

            return privateLayoutList;
        }

        #endregion
    }
}