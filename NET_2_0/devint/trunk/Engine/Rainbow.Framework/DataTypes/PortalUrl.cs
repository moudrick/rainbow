namespace Rainbow.Framework.DataTypes
{
    /// <summary>
    /// PortalUrl
    /// </summary>
    public class PortalUrl : StringDataType
    {
        /// <remarks>
        /// Change visibility to private because now we cache internal values.
        /// Could be moved to protected again if we transform in a property and invalidate cache.
        /// </remarks>
        readonly string portalPathPrefix = string.Empty;

        string innerFullPath;

        /// <summary>
        /// Use this on portalsetting or when you want turn off automatic discovery
        /// </summary>
        /// <param name="portalFullPath">The portal full path.</param>
        //TODO: [moudrick] make it  protected internal after Portal class moved into this project
        public PortalUrl(string portalFullPath)
        {
            portalPathPrefix = portalFullPath;
        }

        /// <summary>
        /// Gets the portal path prefix.
        /// </summary>
        /// <value>The portal path prefix.</value>
        protected string PortalPathPrefix
        {
            get { return portalPathPrefix; }
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <value>The full path.</value>
        public override string FullPath
        {
            get
            {
                if (innerFullPath == null)
                {
                    innerFullPath = Path.WebPathCombine(portalPathPrefix, Value);
                    innerFullPath = innerFullPath.TrimEnd('/'); //Removes trailings
                }
                return innerFullPath;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get { return (innerValue); }
            set
            {
                //Remove portal path if present
                if (value.StartsWith(portalPathPrefix))
                {
                    innerValue = value.Substring(portalPathPrefix.Length);
                }
                else
                {
                    innerValue = value;
                }
                //Reset innerFullPath
                innerFullPath = null;
            }
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get { return "Url relative to Portal"; }
        }
    }
}
