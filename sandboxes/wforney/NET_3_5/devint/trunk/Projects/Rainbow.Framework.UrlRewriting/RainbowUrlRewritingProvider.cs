namespace Rainbow.Framework.UrlRewriting
{
    using UrlRewritingNet.Configuration.Provider;
    using UrlRewritingNet.Web;

    /// <summary>
    /// The rainbow url rewriting provider.
    /// </summary>
    public class RainbowUrlRewritingProvider : UrlRewritingProvider
    {
        #region Public Methods

        /// <summary>
        /// The create rewrite rule.
        /// </summary>
        /// <returns>
        /// A rewrite rule.
        /// </returns>
        public override RewriteRule CreateRewriteRule()
        {
            return new RainbowUrlRewritingRule();
        }

        #endregion
    }
}