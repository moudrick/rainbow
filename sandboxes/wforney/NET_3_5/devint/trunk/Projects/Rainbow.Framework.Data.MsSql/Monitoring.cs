namespace Rainbow.Framework.Security
{
    using System;
    using System.Web;

    using Rainbow.Framework.Configuration;
    using Rainbow.Framework.Data.MsSql;
    using Rainbow.Framework.Data.MsSql.Debugger;

    /// <summary>
    /// Monitoring class is called by the rainbow components to write an entry
    ///     into the monitoring database table.  It is used to maintain and show
    ///     site statistics such as who has logged on and at what time.
    ///     Written by Paul Yarrow, paul@paulyarrow.com
    /// </summary>
    public class Monitoring
    {
        #region Public Methods

        /// <summary>
        /// Logs the entry.
        /// </summary>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="actionType">
        /// Type of the action.
        /// </param>
        /// <param name="userField">
        /// The user field.
        /// </param>
        public static void LogEntry(Guid userId, int portalId, long pageId, string actionType, string userField)
        {
            // note by manu: This exception is already managed at higher level
            // a nested try catch slows down with no real use
            // return;

            // if a tab id of 0 is received, this is the home page
            // so change the number to 1
            // A page ID of -1 is sent when logging in and out
            if (pageId == 0)
            {
                pageId = 1;
            }

            var m = new Data.MsSql.Monitoring
                {
                    UserID = userId,
                    PortalID = portalId,
                    PageID = (int?)pageId,
                    ActivityType = actionType,
                    UserField = userField
                };

            // Add the browser info if we have access
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    m.Referrer = HttpContext.Current.Request.UrlReferrer.ToString();
                }

                // 09_09_2003 Cory Isakson
                // Some browsers are not sending a UserAgent header
                if (HttpContext.Current.Request.UserAgent != null)
                {
                    m.UserAgent = HttpContext.Current.Request.UserAgent;
                }

                m.UserHostAddress = HttpContext.Current.Request.UserHostAddress;
                m.BrowserType = HttpContext.Current.Request.Browser.Type;
                m.BrowserName = HttpContext.Current.Request.Browser.Browser;
                m.BrowserVersion = HttpContext.Current.Request.Browser.Version;
                m.BrowserPlatform = HttpContext.Current.Request.Browser.Platform;
                m.BrowserIsAOL = HttpContext.Current.Request.Browser.AOL;
            }

            using (var db = new DataClassesDataContext(Config.ConnectionString))
            {
                db.Log = new DebuggerWriter();

                db.Monitorings.InsertOnSubmit(m);

                db.SubmitChanges();
            }
        }

        #endregion
    }
}