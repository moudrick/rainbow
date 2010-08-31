namespace Rainbow.Framework.Providers.RainbowSiteMapProvider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Configuration;

    using Rainbow.Framework.Configuration.Web;
    using Rainbow.Framework.Data.Providers;

    /// <summary>
    /// Summary description for SqlSiteMapProvider
    /// </summary>
    [SqlClientPermission(SecurityAction.Demand, Unrestricted = true)]
    public class RainbowSqlSiteMapProvider : RainbowSiteMapProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The _cache dependency name.
        /// </summary>
        public const string _cacheDependencyName = "__SiteMapCacheDependency";

        /// <summary>
        /// The _errmsg 1.
        /// </summary>
        private const string _errmsg1 = "Missing node ID";

        /// <summary>
        /// The _errmsg 2.
        /// </summary>
        private const string _errmsg2 = "Duplicate node ID";

        /// <summary>
        /// The _errmsg 4.
        /// </summary>
        private const string _errmsg4 = "Invalid parent ID";

        /// <summary>
        /// The _errmsg 5.
        /// </summary>
        private const string _errmsg5 = "Empty or missing connectionStringName";

        /// <summary>
        /// The _errmsg 6.
        /// </summary>
        private const string _errmsg6 = "Missing connection string";

        /// <summary>
        /// The _errmsg 7.
        /// </summary>
        private const string _errmsg7 = "Empty connection string";

        /// <summary>
        /// The _errmsg 8.
        /// </summary>
        private const string _errmsg8 = "Invalid sqlCacheDependency";

        /// <summary>
        /// The _root node id.
        /// </summary>
        private const int _rootNodeID = -1;

        /// <summary>
        /// The _lock.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The _nodes.
        /// </summary>
        private readonly Dictionary<int, SiteMapNode> _nodes = new Dictionary<int, SiteMapNode>(16);

        /// <summary>
        /// The _2005 dependency.
        /// </summary>
        private bool _2005dependency; // Database info for SQL Server 2005 cache dependency

        /// <summary>
        /// The _connect.
        /// </summary>
        private string _connect; // Database connection string

        /// <summary>
        /// The _database.
        /// </summary>
        private string _database; // Database info for SQL Server 7/2000 cache dependency

        /// <summary>
        /// The _index authorized roles.
        /// </summary>
        private int _indexAuthorizedRoles;

        /// <summary>
        /// The _index page description.
        /// </summary>
        private int _indexPageDescription;

        /// <summary>
        /// The _index page id.
        /// </summary>
        private int _indexPageID;

        /// <summary>
        /// The _index page layout.
        /// </summary>
        private int _indexPageLayout;

        /// <summary>
        /// The _index page name.
        /// </summary>
        private int indexPageName;

        /// <summary>
        /// The _index page order.
        /// </summary>
        private int _indexPageOrder;

        /// <summary>
        /// The _index parent page id.
        /// </summary>
        private int indexParentPageId;

        /// <summary>
        /// The _index portal id.
        /// </summary>
        private int _indexPortalID;

        /// <summary>
        /// The _root.
        /// </summary>
        private SiteMapNode root;

        /// <summary>
        /// The _table.
        /// </summary>
        private string table; // Database info for SQL Server 7/2000 cache dependency

        #endregion

        #region Properties

        /// <summary>
        /// Gets PortalID.
        /// </summary>
        private static string PortalId
        {
            get
            {
                var contextReader = new Reader(new WebContextReader());
                var context = contextReader.Current;
                return context.Items["PortalID"].ToString();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the site map information from rb_Pages table and builds the site map information 
        ///     in memory.
        /// </summary>
        /// <returns>
        /// The root System.Web.SiteMapNode of the site map navigation structure.
        /// </returns>
        public override SiteMapNode BuildSiteMap()
        {
            lock (this._lock)
            {
                // Return immediately if this method has been called before
                if (this.root != null)
                {
                    return this.root;
                }

                // Query the database for site map nodes
                var connection = new SqlConnection(this._connect);

                try
                {
                    var command = new SqlCommand(BuildSiteMap_Query(), connection)
                        {
                            CommandType = CommandType.Text 
                        };

                    // Create a SQL cache dependency if requested
                    SqlCacheDependency dependency = null;

                    if (this._2005dependency)
                    {
                        dependency = new SqlCacheDependency(command);
                    }
                    else if (!String.IsNullOrEmpty(this._database) && !string.IsNullOrEmpty(this.table))
                    {
                        dependency = new SqlCacheDependency(this._database, this.table);
                    }

                    connection.Open();

                    var reader = command.ExecuteReader();
                    this._indexPageID = reader.GetOrdinal("PageID");
                    this.indexParentPageId = reader.GetOrdinal("ParentPageID");
                    this._indexPageOrder = reader.GetOrdinal("PageOrder");
                    this._indexPortalID = reader.GetOrdinal("PortalID");
                    this.indexPageName = reader.GetOrdinal("PageName");
                    this._indexAuthorizedRoles = reader.GetOrdinal("AuthorizedRoles");
                    this._indexPageLayout = reader.GetOrdinal("PageLayout");
                    this._indexPageDescription = reader.GetOrdinal("PageDescription");

                    if (reader.Read())
                    {
                        // Create an empty root node and add it to the site map
                        this.root = new SiteMapNode(
                            this,
                            _rootNodeID.ToString(),
                            HttpUrlBuilder.BuildUrl(),
                            string.Empty,
                            string.Empty,
                            new[] { "All Users" },
                            null,
                            null,
                            null);
                        this._nodes.Add(_rootNodeID, this.root);
                        this.AddNode(this.root, null);

                        // Build a tree of SiteMapNodes underneath the root node
                        do
                        {
                            // Create another site map node and add it to the site map
                            var node = this.CreateSiteMapNodeFromDataReader(reader);
                            this.AddNode(node, this.GetParentNodeFromDataReader(reader));
                        }
                        while (reader.Read());

                        // Use the SQL cache dependency
                        if (dependency != null)
                        {
                            HttpRuntime.Cache.Insert(
                                _cacheDependencyName + PortalId,
                                new object(),
                                dependency,
                                Cache.NoAbsoluteExpiration,
                                Cache.NoSlidingExpiration,
                                CacheItemPriority.NotRemovable,
                                this.OnSiteMapChanged);
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }

                // Return the root SiteMapNode
                return this.root;
            }
        }

        /// <summary>
        /// Removes all elements in the collections of child and parent site map nodes
        ///     that the System.Web.StaticSiteMapProvider tracks as part of its state.
        /// </summary>
        public override void ClearCache()
        {
            this.Clear();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ProviderException">
        /// </exception>
        /// <exception cref="ProviderException">
        /// </exception>
        /// <exception cref="ProviderException">
        /// </exception>
        /// <exception cref="ProviderException">
        /// </exception>
        /// <exception cref="ProviderException">
        /// </exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
            {
                name = "RainbowSqlSiteMapProvider";
            }

            // Add a default "description" attribute to config if the
            // attribute doesn’t exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Rainbow SQL site map provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _connect
            var connect = config["connectionStringName"];

            if (String.IsNullOrEmpty(connect))
            {
                throw new ProviderException(_errmsg5);
            }

            config.Remove("connectionStringName");

            if (WebConfigurationManager.ConnectionStrings[connect] == null)
            {
                throw new ProviderException(_errmsg6);
            }

            this._connect = WebConfigurationManager.ConnectionStrings[connect].ConnectionString;

            if (String.IsNullOrEmpty(this._connect))
            {
                throw new ProviderException(_errmsg7);
            }

            // Initialize SQL cache dependency info
            var dependency = config["sqlCacheDependency"];

            if (!String.IsNullOrEmpty(dependency))
            {
                if (String.Equals(dependency, "CommandNotification", StringComparison.InvariantCultureIgnoreCase))
                {
                    SqlDependency.Start(this._connect);
                    this._2005dependency = true;
                }
                else
                {
                    // If not "CommandNotification", then extract database and table names
                    var info = dependency.Split(new[] { ':' });
                    if (info.Length != 2)
                    {
                        throw new ProviderException(_errmsg8);
                    }

                    this._database = info[0];
                    this.table = info[1];
                }

                config.Remove("sqlCacheDependency");
            }

            // SiteMapProvider processes the securityTrimmingEnabled
            // attribute but fails to remove it. Remove it now so we can
            // check for unrecognized configuration attributes.
            if (config["securityTrimmingEnabled"] != null)
            {
                config.Remove("securityTrimmingEnabled");
            }

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                var attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                {
                    throw new ProviderException("Unrecognized attribute: " + attr);
                }
            }
        }

        /// <summary>
        /// The is accessible to user.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The is accessible to user.
        /// </returns>
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            var isVisible = false;

            if (node.Roles != null)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    if (node.Roles.Contains("All Users"))
                    {
                        isVisible = true;
                    }
                    else
                    {
                        var enumerator = node.Roles.GetEnumerator();
                        while (!isVisible && enumerator.MoveNext())
                        {
                            isVisible = context.User.IsInRole((string)enumerator.Current);
                        }
                    }
                }
                else
                {
                    isVisible = node.Roles.Contains("All Users") || node.Roles.Contains("Unauthenticated users");
                }
            }

            return isVisible;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes all elements in the collections of child and parent site map nodes
        ///     that the System.Web.StaticSiteMapProvider tracks as part of its state.
        /// </summary>
        protected override void Clear()
        {
            base.Clear();
            this._nodes.Clear();
            this.root = null;
        }

        /// <summary>
        /// Returns the root node.
        /// </summary>
        /// <returns>
        /// The root node.
        /// </returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            lock (this._lock)
            {
                this.BuildSiteMap();
                return this.root;
            }
        }

        /// <summary>
        /// The build site map_ query.
        /// </summary>
        /// <returns>
        /// The build site map_ query.
        /// </returns>
        private static string BuildSiteMap_Query()
        {
            string s =
                string.Format(
                    @"
				SELECT	[PageID], [ParentPageID], [PageOrder], [PortalID], [PageName],
						[AuthorizedRoles], [PageLayout], [PageDescription]
				FROM  [dbo].[rb_Pages] 
				WHERE [PortalID] = {0} 
				ORDER BY [PageOrder]
			",
                    PortalId);
            return s;
        }

        // Helper methods
        /// <summary>
        /// The create site map node from data reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ProviderException">
        /// </exception>
        /// <exception cref="ProviderException">
        /// </exception>
        private SiteMapNode CreateSiteMapNodeFromDataReader(DbDataReader reader)
        {
            // Make sure the node ID is present
            if (reader.IsDBNull(this._indexPageID))
            {
                throw new ProviderException(_errmsg1);
            }

            // Get the node ID from the DataReader
            var id = reader.GetInt32(this._indexPageID);

            // Make sure the node ID is unique
            if (this._nodes.ContainsKey(id))
            {
                throw new ProviderException(_errmsg2);
            }

            var name = reader.IsDBNull(this.indexPageName) ? null : reader.GetString(this.indexPageName).Trim();
            var description = reader.IsDBNull(this._indexPageDescription)
                                  ? null
                                  : reader.GetString(this._indexPageDescription).Trim();
            var roles = reader.IsDBNull(this._indexAuthorizedRoles)
                            ? null
                            : reader.GetString(this._indexAuthorizedRoles).Trim();

            var url = HttpUrlBuilder.BuildUrl(id);

            // If roles were specified, turn the list into a string array
            string[] rolelist = null;
            if (!String.IsNullOrEmpty(roles))
            {
                rolelist = roles.Split(new[] { ',', ';' }, 512);
            }

            if (rolelist != null)
            {
                var rolelistLength = rolelist.Length;
                if (rolelistLength > 0)
                {
                    if (rolelist[rolelistLength - 1].Equals(string.Empty))
                    {
                        var auxrolelist = new string[rolelistLength - 1];
                        for (var i = 0; i < rolelistLength - 1; i++)
                        {
                            auxrolelist[i] = rolelist[i];
                        }

                        rolelist = auxrolelist;
                    }
                }
            }

            // Create a SiteMapNode
            var node = new SiteMapNode(this, id.ToString(), url, name, description, rolelist, null, null, null);

            // Record the node in the _nodes dictionary
            this._nodes.Add(id, node);

            // Return the node        
            return node;
        }

        /// <summary>
        /// The get parent node from data reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ProviderException">
        /// </exception>
        private SiteMapNode GetParentNodeFromDataReader(DbDataReader reader)
        {
            // Make sure the parent ID is present
            if (reader.IsDBNull(this.indexParentPageId))
            {
                return this._nodes[_rootNodeID];
            }

            // Get the parent ID from the DataReader
            var pid = reader.GetInt32(this.indexParentPageId);

            // Make sure the parent ID is valid
            if (!this._nodes.ContainsKey(pid))
            {
                throw new ProviderException(_errmsg4);
            }

            // Return the parent SiteMapNode
            return this._nodes[pid];
        }

        /// <summary>
        /// The on site map changed.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        private void OnSiteMapChanged(string key, object item, CacheItemRemovedReason reason)
        {
            lock (this._lock)
            {
                if (key != _cacheDependencyName || reason != CacheItemRemovedReason.DependencyChanged)
                {
                    return;
                }

                // Refresh the site map
                this.Clear();
                this._nodes.Clear();
                this.root = null;
            }
        }

        #endregion
    }
}