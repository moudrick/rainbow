using System;
using System.Globalization;
using System.Net;
using System.Web;

namespace Rainbow.Framework.Configuration
{
    /// <summary>
    /// Static class for 'safe' retrieval of AppSettings from web.config file. 
    /// All members return a default value. Some members validate the value found and 
    /// return the default value if found value is invalid. (jes1111)
    /// </summary>
    public sealed class Config
    {
        /// <summary>
        /// isolates Config reader for testing purposes
        /// </summary>
        private static Reader config = new Reader(new ConfigReader());

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        private Config()
        {
            //SetReader();
        }

        #region public static methods

        /// <summary>
        /// Sets reader for all Get... methods in this class
        /// </summary>
        /// <param name="reader">an instance of a Concrete Strategy Reader</param>
        public static void SetReader(Reader reader)
        {
            config = reader;
        }

        /// <summary>
        /// Gets the integer from string.
        /// </summary>
        /// <param name="allowNegative">if set to <c>true</c> [allow negative].</param>
        /// <param name="settingValue">The setting value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int GetIntegerFromString(bool allowNegative, string settingValue, int defaultValue)
        {
            const int minChar = 48;
            const int maxChar = 57;
            const int hyphenMinus = 45;
            int isNegative;

            if (settingValue != null && settingValue.Length != 0)
            {
                settingValue = settingValue.Trim();

                if (allowNegative && settingValue[0] == hyphenMinus)
                    isNegative = 1;
                else
                    isNegative = 0;

                int adjustedLength = settingValue.Length - isNegative;

                if (adjustedLength == 0 || adjustedLength > 9)
                    return defaultValue;

                for (int i = isNegative; i < settingValue.Length; i++)
                {
                    if (settingValue[i] > maxChar || settingValue[i] < minChar)
                        return defaultValue; // return defaultValue
                }
                return Int32.Parse(settingValue, CultureInfo.InvariantCulture); // return parsed int
            }
            return defaultValue; // return defaultValue
        }

        /// <summary>
        /// Gets value from configured Reader for specified key, returning parsed signed integer.
        /// Allows integer values between -99,999,999 and 999,999,999. If key is not present, returns defaultValue.
        /// </summary>
        /// <param name="key">setting key</param>
        /// <param name="defaultValue">setting value</param>
        /// <param name="allowNegative">allow or disallow negative integers</param>
        /// <returns>an integer value</returns>
        public static int GetInteger(string key, int defaultValue, bool allowNegative)
        {
            return GetIntegerFromString(allowNegative, config.GetAppSetting(key), defaultValue);
        }

        /// <summary>
        /// Gets value from configured Reader for specified key, returning parsed integer.
        /// Allows integer values between 0 and 999,999,999. If key is not present, returns defaultValue.
        /// </summary>
        /// <param name="key">setting key</param>
        /// <param name="defaultValue">setting value</param>
        /// <returns>an integer value</returns>
        public static int GetInteger(string key, int defaultValue)
        {
            return GetInteger(key, defaultValue, false);
        }

        /// <summary>
        /// Gets value from configyred Reader for specified key, returning string.
        /// If key is not present, returns defaultValue. Empty string is an allowable value.
        /// </summary>
        /// <param name="key">collection key</param>
        /// <param name="defaultValue">default value (can be empty string or null)</param>
        /// <returns>setting value or defaultValue</returns>
        public static string GetString(string key, string defaultValue)
        {
            return GetString(key, defaultValue, true);
        }

        /// <summary>
        /// Gets value from configured reader for specified key, returning string.
        /// If key is not present, returns defaultValue. Boolean parameter controls whether
        /// empty string is an allowable value.
        /// </summary>
        /// <param name="key">collection key</param>
        /// <param name="defaultValue">default value (can be empty string or null)</param>
        /// <param name="allowEmpty">allow or disallow return of empty string from setting item</param>
        /// <returns>setting value or defaultValue</returns>
        public static string GetString(string key, string defaultValue, bool allowEmpty)
        {
            return GetStringInternal(allowEmpty, config.GetAppSetting(key), defaultValue);
        }

        /// <summary>
        /// Returns string value or defaultValue.
        /// </summary>
        /// <param name="allowEmpty">allow empty string as settingValue</param>
        /// <param name="settingValue">settingValue</param>
        /// <param name="defaultValue">defaultValue</param>
        /// <returns>settingValue or defaultValue</returns>
        private static string GetStringInternal(bool allowEmpty, string settingValue, string defaultValue)
        {
            if (settingValue != null)
            {
                if (settingValue.Length != 0 || allowEmpty)
                    return settingValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets value from configured Reader for specified key, returning boolean.
        /// If key is not present, returns defaultValue.
        /// </summary>
        /// <param name="key">collection key</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>setting value or default value</returns>
        public static bool GetBoolean(string key, bool defaultValue)
        {
            return GetBooleanInternal(defaultValue, config.GetAppSetting(key));
        }

        /// <summary>
        /// Converts string value to boolean. Returns defaultValue if
        /// settingValue is not legitimate equivalent.
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <param name="settingValue">settingValue</param>
        /// <returns>setting value or default value</returns>
        private static bool GetBooleanInternal(bool defaultValue, string settingValue)
        {
            if (settingValue != null)
            {
                if (string.Compare(settingValue, Boolean.TrueString, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;

                if (string.Compare(settingValue, Boolean.FalseString, StringComparison.OrdinalIgnoreCase) == 0)
                    return false;

                settingValue = settingValue.Trim();

                if (string.Compare(settingValue, Boolean.TrueString, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;

                if (string.Compare(settingValue, Boolean.FalseString, StringComparison.OrdinalIgnoreCase) == 0)
                    return false;

            }
            return defaultValue;
        }

        /// <summary>
        /// Gets value from configured Reader for specified key, returning HttpStatusCode enum.
        /// If key is not present or value is not equivalent to valid HttpStatusCode, returns defaultValue.
        /// </summary>
        /// <param name="key">collection key</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>setting value or default value</returns>
        public static HttpStatusCode GetHttpStatusCode(string key, HttpStatusCode defaultValue)
        {
            string settingValue;

            if (key != null && key.Length != 0)
                settingValue = config.GetAppSetting(key);
            else
                settingValue = null;

            if (settingValue != null)
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), settingValue))
                    return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), settingValue, false);
            }
            return defaultValue;
        }

        /// <summary>
        /// Converts a string to a URL by trimming it, replacing '~' with Application Root and URL Encoding it
        /// </summary>
        /// <param name="item">The string to be converted</param>
        /// <returns>the converted string</returns>
        public static Uri ToUrl(string item)
        {
            item = item.Trim();

            if (item.StartsWith("~", StringComparison.OrdinalIgnoreCase))
                item = Path.ApplicationRootPath(item.TrimStart(new char[] { '~' }));

            return new Uri(HttpUtility.UrlEncode(item));
        }

        #endregion
    }
}