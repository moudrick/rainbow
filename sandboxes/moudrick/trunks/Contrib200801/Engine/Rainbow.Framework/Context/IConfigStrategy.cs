namespace Rainbow.Framework.Context
{
    /// <summary>
    /// Interface for Config Reader IConfigStrategy
    /// </summary>
    public interface IConfigStrategy
    {
        /// <summary>
        /// Fetch value for key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>string value</returns>
        string GetAppSetting(string key);
    }
}
