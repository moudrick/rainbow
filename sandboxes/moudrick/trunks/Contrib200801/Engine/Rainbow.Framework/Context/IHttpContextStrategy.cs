using System.Web;

namespace Rainbow.Framework.Context
{
    /// <summary>
    /// Interface for Config Reader Strategy
    /// </summary>
    public interface IHttpContextStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        HttpContext HttpContext
        {
            get;
        }
    }
}
