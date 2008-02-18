using System.Web;

namespace Rainbow.Framework.Context
{
    /// <summary>
    /// Concrete Strategy - gets web current context
    /// </summary>
    public class WebHttpContextStrategy : IHttpContextStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpContext HttpContext
        {
            get
            {
                return HttpContext.Current;
            }
        }
    }
}
