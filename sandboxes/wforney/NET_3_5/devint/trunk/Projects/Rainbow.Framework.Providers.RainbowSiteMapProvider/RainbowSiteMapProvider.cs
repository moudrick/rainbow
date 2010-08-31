namespace Rainbow.Framework.Providers.RainbowSiteMapProvider
{
    using System.Linq;
    using System.Web;

    /// <summary>
    /// The rainbow site map provider.
    /// </summary>
    public abstract class RainbowSiteMapProvider : StaticSiteMapProvider
    {
        #region Public Methods

        /// <summary>
        /// The clear all rainbow site map caches.
        /// </summary>
        public static void ClearAllRainbowSiteMapCaches()
        {
            // Removing Sitemap Cache
            foreach (var siteMap in SiteMap.Providers.OfType<RainbowSiteMapProvider>())
            {
                siteMap.ClearCache();
            }
        }

        /// <summary>
        /// The clear cache.
        /// </summary>
        public abstract void ClearCache();

        #endregion
    }
}