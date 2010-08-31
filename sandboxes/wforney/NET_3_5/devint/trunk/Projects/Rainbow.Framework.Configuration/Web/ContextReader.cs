namespace Rainbow.Framework.Configuration.Web
{
    using System.Web;

    /// <summary>
    /// Interface for Config Reader Strategy
    /// </summary>
    public interface IStrategy
    {
        #region Properties

        /// <summary>
        ///     Gets the current.
        /// </summary>
        /// <value>The current.</value>
        HttpContext Current { get; }

        #endregion
    }

    /// <summary>
    /// Concrete Strategy - gets web current context
    /// </summary>
    public class WebContextReader : IStrategy, Configuration.IStrategy
    {
        #region Properties

        /// <summary>
        ///     web current context
        /// </summary>
        /// <value></value>
        public HttpContext Current
        {
            get
            {
                return HttpContext.Current;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IStrategy

        /// <summary>
        /// Fetch value for key
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <returns>
        /// string value
        /// </returns>
        public string GetAppSetting(string key)
        {
            return HttpContext.Current.Items[key] as string;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Reader
    /// </summary>
    public class Reader
    {
        #region Constants and Fields

        /// <summary>
        /// The strategy.
        /// </summary>
        private readonly IStrategy strategy;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        /// <param name="strategy">
        /// The strategy.
        /// </param>
        public Reader(IStrategy strategy)
        {
            this.strategy = strategy;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets web current context
        /// </summary>
        /// <value>The current.</value>
        public HttpContext Current
        {
            get
            {
                return this.strategy.Current;
            }
        }

        #endregion
    }
}