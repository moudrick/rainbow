using System.Web;
using System.Web.Caching;

namespace Rainbow.Framework.Context
{
    class WebCacheStrategy : IWebCacheStrategy
    {
        Cache cache = null;
        public Cache Cache
        {
            get
            {
                //TODO: [moudrick] split it to separate strategies
                if (cache == null)
                {
                    if (HttpContext.Current != null)
                    {
                        cache = HttpContext.Current.Cache;
                    }
                    else
                    {
                        // I'm in a test environment
                        cache = HttpRuntime.Cache;
                    }
                }
                return cache;
            }
        }
    }
}
