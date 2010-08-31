namespace Rainbow.Framework.UrlRewriting
{
    using System.Configuration;
    using System.Globalization;
    using System.Web;

    using UrlRewritingNet.Configuration;
    using UrlRewritingNet.Web;

    /// <summary>
    /// The rainbow url rewriting rule.
    /// </summary>
    public class RainbowUrlRewritingRule : RewriteRule
    {
        #region Constants and Fields

        /// <summary>
        /// The default splitter.
        /// </summary>
        private string defaultSplitter = "__";

        /// <summary>
        /// The friendly page name.
        /// </summary>
        private string friendlyPageName = "Default.aspx";

        /// <summary>
        /// The handler flag.
        /// </summary>
        private string handlerFlag = "site";

        /// <summary>
        /// The page id no splitter.
        /// </summary>
        private bool pageIdNoSplitter;

        #endregion

        #region Public Methods

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="rewriteSettings">
        /// The rewrite settings.
        /// </param>
        public override void Initialize(RewriteSettings rewriteSettings)
        {
            base.Initialize(rewriteSettings);

            if (!string.IsNullOrEmpty(rewriteSettings.Attributes["handlerflag"]))
            {
                this.handlerFlag = rewriteSettings.Attributes["handlerflag"].ToLower(CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(rewriteSettings.Attributes["handlersplitter"]))
            {
                this.defaultSplitter = rewriteSettings.Attributes["handlersplitter"];
            }
            else
            {
                if (ConfigurationManager.AppSettings["HandlerDefaultSplitter"] != null)
                {
                    this.defaultSplitter = ConfigurationManager.AppSettings["HandlerDefaultSplitter"];
                }
            }

            if (!string.IsNullOrEmpty(rewriteSettings.Attributes["pageidnosplitter"]))
            {
                this.pageIdNoSplitter = bool.Parse(rewriteSettings.Attributes["pageidnosplitter"]);
            }

            if (!string.IsNullOrEmpty(rewriteSettings.Attributes["friendlyPageName"]))
            {
                this.friendlyPageName = rewriteSettings.Attributes["friendlyPageName"];
            }
        }

        /// <summary>
        /// The is rewrite.
        /// </summary>
        /// <param name="requestUrl">
        /// The request url.
        /// </param>
        /// <returns>
        /// The is rewrite.
        /// </returns>
        public override bool IsRewrite(string requestUrl)
        {
            return string.Format("/{0}/", this.handlerFlag).Contains(requestUrl);
        }

        /// <summary>
        /// The rewrite url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The rewrite url.
        /// </returns>
        public override string RewriteUrl(string url)
        {
            var rewrittenUrl = url.Substring(0, url.IndexOf("/" + this.handlerFlag + "/"));

            var parts =
                url.Substring(url.IndexOf("/" + this.handlerFlag + "/") + ("/" + this.handlerFlag + "/").Length).Split(
                    '/');

            rewrittenUrl += string.Format("/{0}", this.friendlyPageName);
            var queryString = string.Format("?pageId={0}", parts[parts.Length - 2]);

            string queryStringParam;
            if (parts.Length > 2)
            {
                for (var i = 0; i < (parts.Length - 2); i++)
                {
                    queryStringParam = parts[i];

                    queryString += string.Format(
                        "&{0}", queryStringParam.Substring(0, queryStringParam.IndexOf(this.defaultSplitter)));
                    queryString += string.Format(
                        "={0}",
                        queryStringParam.Substring(
                            queryStringParam.IndexOf(this.defaultSplitter) + this.defaultSplitter.Length));
                }
            }

            HttpContext.Current.RewritePath(rewrittenUrl, string.Empty, queryString);

            return rewrittenUrl + queryString;
        }

        #endregion
    }
}