using System.Collections;
using System.IO;
using System.Web;
using System.Web.Caching;
using Rainbow.Framework.Context;

namespace Rainbow.Framework.Design
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
        static string innerWebPath;
        readonly string portalPath;

        ///<summary>
        ///</summary>
        ///<param name="portalPath"></param>
        public LayoutManager(string portalPath)
        {
            this.portalPath = portalPath;
        }

        /// <summary>
        /// The path of the Layout dir (Web side)
        /// used to reference images
        /// </summary>
        public static string WebPath
        {
            get
            {
                if (innerWebPath == null)
                {
                    innerWebPath =
                        Framework.Path.WebPathCombine(Framework.Path.ApplicationRoot,
                                                     "/Design/DesktopLayouts");
                }
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
                return
                    Framework.Path.WebPathCombine(Framework.Path.ApplicationRoot,
                                                 portalPath,
                                                 "/DesktopLayouts");
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
        /// Read the Path dir and returns an ArrayList with all the Layouts found.
        /// Static because the list is Always the same.
        /// </summary>
        /// <returns></returns>
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

                for (int i = 0; i < layouts.Length; i++)
                {
                    LayoutItem t = new LayoutItem();
                    t.Name = layouts[i].Substring(Path.Length + 1);
                    if (t.Name != "CVS" && t.Name != "_svn") //Ignore CVS
                    {
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
        }

        /// <summary>
        ///     
        /// </summary>
        public void ClearCacheList()
        {
            //Clear cache
            lock (typeof(LayoutManager))
            {
                // Jes1111
                //LayoutManager.cachedLayoutsList = null;
                CurrentCache.Remove(Key.LayoutList(Path));
                CurrentCache.Remove(Key.LayoutList(PortalLayoutPath));
            }
        }

        /// <summary>
        /// Read the Path dir and returns
        /// an ArrayList with all the Layouts found, public and privates
        /// </summary>
        /// <returns></returns>
        public ArrayList GetLayouts()
        {
            // Jes1111 - 27-02-2005 - new version - correct caching
            ArrayList layoutList;
            ArrayList layoutListPrivate;

            layoutList = (ArrayList) GetPublicLayouts().Clone();
            layoutListPrivate = GetPrivateLayouts();

            layoutList.AddRange(layoutListPrivate);

            return layoutList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayList GetPrivateLayouts()
        {
            ArrayList privateLayoutList;

            if (!CurrentCache.Exists(Key.LayoutList(PortalLayoutPath)))
            {
                privateLayoutList = new ArrayList();
                string[] layouts;

                // Try to read directories from private theme path
                if (Directory.Exists(PortalLayoutPath))
                {
                    layouts = Directory.GetDirectories(PortalLayoutPath);
                }
                else
                {
                    layouts = new string[0];
                }

                for (int i = 0; i <= layouts.GetUpperBound(0); i++)
                {
                    LayoutItem t = new LayoutItem();
                    t.Name = layouts[i].Substring(PortalLayoutPath.Length + 1);

                    if (t.Name != "CVS" && t.Name != "_svn" && t.Name != ".svn")
                    {
                        privateLayoutList.Add(t);
                    }
                }
                CurrentCache.Insert(Key.LayoutList(PortalLayoutPath),
                                    privateLayoutList,
                                    new CacheDependency(PortalLayoutPath));
            }
            else
            {
                privateLayoutList = (ArrayList) CurrentCache.Get(Key.LayoutList(PortalLayoutPath));
            }
            return privateLayoutList;
        }
    }
}
