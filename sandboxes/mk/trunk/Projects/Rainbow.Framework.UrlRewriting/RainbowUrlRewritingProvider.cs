using UrlRewritingNet.Configuration.Provider;
using UrlRewritingNet.Web;

namespace Rainbow.Framework.UrlRewriting
{
    public class RainbowUrlRewritingProvider : UrlRewritingProvider
    {
        public override RewriteRule CreateRewriteRule()
        {
            return new RainbowUrlRewritingRule();
        }
    }
}
