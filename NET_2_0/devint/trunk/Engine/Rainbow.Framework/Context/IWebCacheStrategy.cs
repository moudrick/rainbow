using System.Web.Caching;

namespace Rainbow.Framework.Context
{
    public interface IWebCacheStrategy
    {
        Cache Cache { get; }
    }
}
