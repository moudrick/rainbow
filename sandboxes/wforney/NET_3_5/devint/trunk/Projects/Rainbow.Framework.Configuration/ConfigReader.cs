namespace Rainbow.Framework.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Interface for Config Reader Strategy
    /// </summary>
    public interface IStrategy
    {
        #region Public Methods

        /// <summary>
        /// Fetch value for key
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <returns>
        /// string value
        /// </returns>
        string GetAppSetting(string key);

        #endregion
    }

    /// <summary>
    /// Concrete Strategy - reads from ConfigurationSettings.AppSettings
    /// </summary>
    public class ConfigReader : IStrategy
    {
        #region Implemented Interfaces

        #region IStrategy

        /// <summary>
        /// Fetches value for key from ConfigurationSettings.AppSettings
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <returns>
        /// string value
        /// </returns>
        public string GetAppSetting(string key)
        {
            return !string.IsNullOrEmpty(key) ? ConfigurationManager.AppSettings[key] : null;
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
        /// Constructor
        /// </summary>
        /// <param name="strategy">
        /// a Concrete Strategy
        /// </param>
        public Reader(IStrategy strategy)
        {
            this.strategy = strategy;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Fetches value for key - source depends on which ConcreteStrategy is set
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <returns>
        /// string value
        /// </returns>
        public string GetAppSetting(string key)
        {
            return this.strategy.GetAppSetting(key);
        }

        #endregion
    }
}