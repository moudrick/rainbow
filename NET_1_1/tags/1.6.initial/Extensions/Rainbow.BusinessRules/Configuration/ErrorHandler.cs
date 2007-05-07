using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mail;
using Rainbow.Helpers;

namespace Rainbow.Configuration
{
	/// <summary>
	/// This class in combination with the Web.Config file handles all the Errors that are not caught programatically
	/// 99% of the time Errors will be caught by Rainbow's HttpUrlModule, this class will be called, errors will be 
	/// logged depending on what was specified by the Web.Config file, after that the error cascades up and is caught
	/// by the customErrors settings in Web.Config. Here you can specify errors and which pages to redirect to.
	/// Visitors will be directed to dynamic aspx pages for General Errors and 404 Errors (Specified aspx page does not exist)
	/// These pages are dynamic and will keep the theme you selected for your portal. It also makes use of Rainbow's
	/// multi-language support. If these dynamic pages themselves have an error (e.g the Database has crashed 
	/// so it can't retrieve the theme or translations, then there is code in these pages to catch errors at the
	/// Page Level and redirect to a static html page (one for general errors and one for 404 errors). 
	/// These pages will have no theme at all, just text (So that they will work across multiple themes) and the 
	/// text will be in English (No Translation - Although multiple versions of the html pages could be created to
	/// handle this. Please specify if it is urgent.
	/// 
	/// Thanks go to  Joan M for the Original Code.
	/// Modified and extended by John Mandia.
	/// </summary>
	[History("JohnMandia", "john.mandia@whitelightsolutions.com", "1.2", "2003/04/09", "Updated LogToFile code to allow users to specify logfile location and specify frequency of the log files daily monthly yearly or all. Also created the LogHelper file with useful functions.")]
	[History("Manu", "manudea71@hotmail.com", "1.3", "2004/05/16", "Commented out obsolete code or marked as obsolete. Will be removed in future versions.")]
	public class ErrorHandler
	{
		private const string strTOE = "Time of Error: ";
		private const string strSrvName = "SERVER_NAME";
		private const string strSrc = "Source: ";
		private const string strErrMsg = "Error Message: ";
		private const string strTgtSite = "Target Site: ";
		private const string strStkTrace = "Stack Trace: ";

		/// <summary>
		/// 
		/// </summary>
		public static void HandleException()
		{
			Exception e = HttpContext.Current.Server.GetLastError();

			if (e == null)
				return;

			e = e.GetBaseException();

			if (e != null)
				HandleException(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		private static void InnerHandleException(string message, Exception e)
		{
			if (e is ThreadAbortException)
				LogHelper.Logger.Log(LogLevel.Warn, message, e);
			else if (e is HttpException && ((HttpException) e).ErrorCode == 403)
				LogHelper.Logger.Log(LogLevel.Warn, message, e);
			else
				LogHelper.Logger.Log(LogLevel.Error, message, e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public static void HandleException(Exception e)
		{
			InnerHandleException(FormatExceptionDescription(e), e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public static void HandleException(string message, Exception e)
		{
			InnerHandleException(message + Environment.NewLine
				+ FormatExceptionDescription(e), e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		protected static string FormatExceptionDescription(Exception e)
		{
			string formatExceptionDescription;

			try
			{
				StringBuilder sb = new StringBuilder();

				HttpContext context = HttpContext.Current;

				sb.Append(strTOE + DateTime.Now.ToString("g") + Environment.NewLine);
				if (context.Request.Url != null)
					sb.Append("URL: " + context.Request.Url + Environment.NewLine);
				if (context.Request.Form != null)
					sb.Append("Form: " + context.Request.Form.ToString() + Environment.NewLine);
				if (context.Request.QueryString != null)
					sb.Append("QueryString: " + context.Request.QueryString.ToString() + Environment.NewLine);
				if (context.Request.ServerVariables[strSrvName] != null)
					sb.Append("Server Name: " + context.Request.ServerVariables[strSrvName] + Environment.NewLine);
				if (context.Request.UrlReferrer != null)
					sb.Append("URL Referrer: " + context.Request.UrlReferrer + Environment.NewLine);

				// Check to see if administrator wants full error messages
				if (ConfigurationSettings.AppSettings["LogMessageMode"] == "Full")
				{
					if (context.Request.Browser.Platform != null)
						sb.Append("Platform: " + context.Request.Browser.Platform + Environment.NewLine);
					if (context.Request.UserAgent != null)
						sb.Append("User Agent: " + context.Request.UserAgent + Environment.NewLine);
					if (context.Request.UserHostAddress != null)
						sb.Append("User IP: " + context.Request.UserHostAddress + Environment.NewLine);
					if (context.Request.UserHostName != null)
						sb.Append("User Host Name: " + context.Request.UserHostName + Environment.NewLine);
					sb.Append("User is Authenticated: " + context.User.Identity.IsAuthenticated.ToString() + Environment.NewLine);
					if (PortalSettings.CurrentUser != null && PortalSettings.CurrentUser.Identity != null)
					{
						sb.Append("User Name: " + PortalSettings.CurrentUser.Identity.Name + Environment.NewLine);
						sb.Append("User Email: " + PortalSettings.CurrentUser.Identity.Email + Environment.NewLine);
					}
					sb.Append("Is Crawler : " + context.Request.Browser.Crawler.ToString() + Environment.NewLine);
					sb.Append("Supports JavaScript: " + context.Request.Browser.JavaScript.ToString() + Environment.NewLine);
					sb.Append("Supports Cookies: " + context.Request.Browser.Cookies.ToString() + Environment.NewLine);
				}

				while (e != null)
				{
					sb.Append("Type: " + e.GetType().FullName + Environment.NewLine);
					sb.Append("Message: " + e.Message + Environment.NewLine);
					sb.Append(strSrc + e.Source + Environment.NewLine);
					sb.Append("TargetSite: " + e.TargetSite + Environment.NewLine);
					sb.Append("StackTrace: " + e.StackTrace + Environment.NewLine);
					sb.Append(Environment.NewLine);

					e = e.InnerException;
				}

				sb.Append(Environment.NewLine);
				formatExceptionDescription = sb.ToString();
			}
			catch (Exception ex)
			{
				StringBuilder sb2 = new StringBuilder();
				sb2.Append(strTOE + DateTime.Now.ToString("g") + Environment.NewLine);
				sb2.Append("The ErrorHandler.FormatExceptionDescription method has thrown an error (May happen if full logging is enabled and it cannot retrieve the User's Information)" + Environment.NewLine);
				sb2.Append("This is a reduced log entry to reduce the chance of another error being thrown" + Environment.NewLine);
				sb2.Append("The FormatExceptionDescription Method failed to write the error. It received the following error: " + Environment.NewLine);
				sb2.Append(Environment.NewLine);
				sb2.Append(strErrMsg + ex.Message.ToString() + Environment.NewLine);
				sb2.Append(strSrc + ex.Source + Environment.NewLine);
				sb2.Append(strTgtSite + ex.TargetSite + Environment.NewLine);
				sb2.Append(strStkTrace + ex.StackTrace + Environment.NewLine);
				sb2.Append(Environment.NewLine);
				sb2.Append("The original error was:" + Environment.NewLine);
				sb2.Append(Environment.NewLine);
				sb2.Append(strErrMsg + e.Message.ToString() + Environment.NewLine);
				sb2.Append(strSrc + e.Source + Environment.NewLine);
				sb2.Append(strTgtSite + e.TargetSite + Environment.NewLine);
				sb2.Append(strStkTrace + e.StackTrace + Environment.NewLine);
				sb2.Append(Environment.NewLine + "End of Entry." + Environment.NewLine + Environment.NewLine);
				sb2.Append("\n\n");
				formatExceptionDescription = sb2.ToString();
			}

			return formatExceptionDescription;
		}

		/// <summary>
		/// Obsolete: We use log4net now.
		/// </summary>
		/// <param name="sText"></param>
		[Obsolete("Now we use log4net")]
		private static void LogToFile(string sText)
		{
			string sLogFileName;
			string sPath;

			try
			{
				sLogFileName = LogHelper.GetFileName("rb_error", ".log");
				sPath = LogHelper.GetLogFolderLocation() + sLogFileName;

				using (FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write))
				{
					using (StreamWriter writer = new StreamWriter(fs))
					{
						writer.Write(sText);
						writer.Close();
					}
					fs.Close();
					//fs = null; // Release the file
				}
			}
			catch (Exception)
			{
				// nothing, otherwise there is a chance you could end up with an infinite loop.
			}
		}

		/// <summary>
		/// Obsolete: We use log4net now.
		/// </summary>
		/// <param name="sBody"></param>
		/// <param name="Subject"></param>
		[Obsolete("Now we use log4net")]
		private static void LogToEmail(string sBody, string subject)
		{
			try
			{
				MailMessage msg = new MailMessage();

				msg.BodyFormat = MailFormat.Text;
				msg.To = ConfigurationSettings.AppSettings["SupportToEmailAddress"];
				msg.From = ConfigurationSettings.AppSettings["SupportFromEmailAddress"];
				msg.Subject = subject;
				msg.Body = sBody;

				if (PortalSettings.SmtpServer != null)
					SmtpMail.SmtpServer = PortalSettings.SmtpServer;

				SmtpMail.Send(msg);
			}
			catch (Exception)
			{
				// nothing, otherwise there is a chance you could end up with an infinite loop.
			}
		}

		/// <summary>
		/// Obsolete: We use log4net now.
		/// </summary>
		/// <param name="sText"></param>
		[Obsolete("Now we use log4net")]
		private static void LogToEventLog(string sText)
		{
			string _supportEventLogSource = ConfigurationSettings.AppSettings["SupportEventLogSource"];

			try
			{
				if (!EventLog.SourceExists(_supportEventLogSource))
					EventLog.CreateEventSource(_supportEventLogSource, "Application");

				using (EventLog log = new EventLog())
				{
					log.Source = _supportEventLogSource;
					log.WriteEntry(sText, EventLogEntryType.Error);
				}
			}
			catch (Exception)
			{
				// nothing, otherwise there is a chance you could end up with an infinite loop.
			}
		}

		/// <summary>
		/// Obsolete: We use log4net now.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="errorList"></param>
		/// <returns></returns>
		[Obsolete("Now we use log4net")]
		private static bool LogErrorType(Exception e, string errorList)
		{
			bool logErrorType;

			if (e is HttpException)
			{
				try
				{
					string errorCode = ((HttpException) e).GetHttpCode().ToString();
					int i;

					i = errorList.IndexOf(errorCode);

					if (errorList == "All")
						logErrorType = true;
					else if (i > -1)
						logErrorType = true;
					else
						logErrorType = false;
				}
				catch (Exception)
				{
					// If there was an error in trying to determine the error code log the original error anyway.
					logErrorType = true;
				}
			}
			else
				logErrorType = true;

			return logErrorType;
		}

	}
}