using System.Data;
using System.Data.SqlClient;
using System.Web;
using Rainbow.Framework.Configuration;
using Rainbow.Framework.Data;
using Rainbow.Framework.Data.MsSql;
using System;
using Rainbow.Framework.Data.MsSql.Debugger;

namespace Rainbow.Framework.Security
{
    /// <summary>
    /// Monitoring class is called by the rainbow components to write an entry
    /// into the monitoring database table.  It is used to maintain and show
    /// site statistics such as who has logged on and at what time.
    /// Written by Paul Yarrow, paul@paulyarrow.com
    /// </summary>
    public partial class Monitoring
    {
        /// <summary>
        /// Logs the entry.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="userField">The user field.</param>
        public static void LogEntry(Guid userID, int portalID, long pageID, string actionType, string userField)
        {
            // note by manu: This exception is already managed at higher level
            // a nested try catch slows down with no real use
            //return;

            // if a tab id of 0 is received, this is the home page
            // so change the number to 1
            // A page ID of -1 is sent when logging in and out
            if (pageID == 0) pageID = 1;

            Data.MsSql.Monitoring m = new Rainbow.Framework.Data.MsSql.Monitoring();
            m.UserID = userID;
            m.PortalID = portalID;
            m.PageID = (int?)pageID;
            m.ActivityType = actionType;
            m.UserField = userField;

            // Add the browser info if we have access
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
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

            using (DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString))
            {
                db.Log = new DebuggerWriter();

                db.Monitorings.InsertOnSubmit(m);

                db.SubmitChanges();
            }
        }
    }
}