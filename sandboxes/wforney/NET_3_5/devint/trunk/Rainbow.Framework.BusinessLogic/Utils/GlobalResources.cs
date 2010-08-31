//===============================================================================
//	Base Logic Layer
//	Rainbow.Framework.BLL.Utils
//===============================================================================
// Encapsulate resources -- it can come from anywhere
//===============================================================================

namespace Rainbow.Framework.BLL.Utils
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Summary description for GlobalResources.
    /// </summary>
    [Obsolete("use Rainbow.Framework.Settings.Config")]
    public class GlobalResources
    {
        // jes1111 - moved to GlobalInternalStrings
        // 		/// <summary>
        // 		/// non breakable html space character
        // 		/// </summary>
        // 		public  const string HTML_SPACE = "&nbsp;";
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether the Portal support WIndow Mgmt Functions/Controls
        /// </summary>
        /// <value><c>true</c> if [support window MGMT]; otherwise, <c>false</c>.</value>
        public static bool SupportWindowMgmt
        {
            get
            {
                return SafeBoolean("WindowMgmtControls", false);
            }
        }

        // end of SupportWindowMgmt

        /// <summary>
        ///     Gets a value indicating whether we support the close button
        /// </summary>
        /// <value>
        ///     <c>true</c> if [support window MGMT close]; otherwise, <c>false</c>.
        /// </value>
        public static bool SupportWindowMgmtClose
        {
            get
            {
                return SafeBoolean("WindowMgmtWantClose", false);
            }
        }

        #endregion

        // end of SupportWindowMgmtClose
        #region Public Methods

        /// <summary>
        /// Get Boolean Resource
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="defaultRet">
        /// if set to <c>true</c> [default_ret].
        /// </param>
        /// <returns>
        /// The safe boolean.
        /// </returns>
        public static bool SafeBoolean(string name, bool defaultRet)
        {
            var obj = ConfigurationManager.AppSettings[name];

            bool returnvalue;
            return bool.TryParse(obj, out returnvalue) ? returnvalue : defaultRet;
        }

        // end of SafeBoolean

        /// <summary>
        /// Get Integer Resource
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="defaultRet">
        /// The default_ret.
        /// </param>
        /// <returns>
        /// The safe int.
        /// </returns>
        public static int SafeInt(string name, int defaultRet)
        {
            var obj = ConfigurationSettings.AppSettings[name];

            int returnValue;
            return int.TryParse(obj, out returnValue) ? returnValue : defaultRet;
        }

        // end of SafeInt

        /// <summary>
        /// Get string Resource
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="defaultRet">
        /// The default_ret.
        /// </param>
        /// <returns>
        /// The safe string.
        /// </returns>
        public static string SafeString(string name, string defaultRet)
        {
            var obj = ConfigurationSettings.AppSettings[name];

            return string.IsNullOrEmpty(obj) ? defaultRet : obj;
        }

        #endregion

        // end of SafeString
    }
}