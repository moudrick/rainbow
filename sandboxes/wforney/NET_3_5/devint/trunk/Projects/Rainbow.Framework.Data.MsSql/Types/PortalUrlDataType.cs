namespace Rainbow.Framework.Data.Types
{
    using System.Web;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Data.MsSql;

    /// <summary>
    /// PortalUrlDataType
    /// </summary>
    public class PortalUrlDataType : StringDataType
    {
        #region Constants and Fields

        /// <summary>
        /// The portal path prefix.
        /// </summary>
        /// <remarks>
        /// Change visibility to private because now we cache internal values.
        ///     Could be moved to protected again if we transform in a property and invalidate cache.
        /// </remarks>
        private readonly string portalPathPrefix = string.Empty;

        /// <summary>
        /// The inner full path.
        /// </summary>
        private string innerFullPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "PortalUrlDataType" /> class.
        /// </summary>
        public PortalUrlDataType()
        {
            this.InnerDataType = PropertiesDataType.String;

            // InitializeComponents();
            if (HttpContext.Current.Items["PortalSettings"] != null)
            {
                // Obtain PortalSettings from Current Context
                var portalSettings = (Portal)HttpContext.Current.Items["PortalSettings"];
                this.portalPathPrefix = portalSettings.PortalPathFull;
                if (!this.portalPathPrefix.EndsWith("/"))
                {
                    this.portalPathPrefix += "/";
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalUrlDataType"/> class. 
        /// Use this on portalsetting or when you want turn off automatic discovery
        /// </summary>
        /// <param name="portalFullPath">
        /// The portal full path.
        /// </param>
        public PortalUrlDataType(string portalFullPath)
        {
            this.InnerDataType = PropertiesDataType.String;

            // 			InitializeComponents();			
            this.portalPathPrefix = portalFullPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get
            {
                return "Url relative to Portal";
            }
        }

        /// <summary>
        ///     Not Implemented
        /// </summary>
        /// <value>The full path.</value>
        public override string FullPath
        {
            get
            {
                if (this.innerFullPath == null)
                {
                    this.innerFullPath = Path.WebPathCombine(this.portalPathPrefix, this.Value);
                    this.innerFullPath = this.innerFullPath.TrimEnd('/'); // Removes trailings
                }

                return this.innerFullPath;
            }
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get
            {
                return this.InnerValue;
            }

            set
            {
                // Remove portal path if present
                if (value.StartsWith(this.portalPathPrefix))
                {
                    this.InnerValue = value.Substring(this.portalPathPrefix.Length);
                }
                else
                {
                    this.InnerValue = value;
                }

                // Reset _innerFullPath
                this.innerFullPath = null;
            }
        }

        /// <summary>
        ///     Gets the portal path prefix.
        /// </summary>
        /// <value>The portal path prefix.</value>
        protected string PortalPathPrefix
        {
            get
            {
                return this.portalPathPrefix;
            }
        }

        #endregion
    }
}