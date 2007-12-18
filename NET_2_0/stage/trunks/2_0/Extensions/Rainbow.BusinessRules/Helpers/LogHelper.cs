using System;
using System.Configuration;
using System.IO;
using Rainbow.Configuration;
// Import log4net classes.
// using log4net;
// using log4net.Config;

namespace Rainbow.Helpers
{
	/// <summary> 
	/// Log Helper by john.mandia@whitelightsolutions.com 9/4/2003
	/// 
	/// Revised by Manu on 27/10/2003 for use with log4net
	/// </summary> 
	public sealed class LogHelper
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private LogHelper()
		{
		}

		#region log4net OBSOLETE

		/*

		// Define a static log4net logger variable so that it references the
		// Logger instance named "LogHelper".
		private static readonly ILog log = LogManager.GetLogger("Rainbow");
		static LogHelper()
		{
			// Initialise the logging when the application loads
			log4net.Config.DOMConfigurator.Configure();
		}

		/// <summary>
		/// Returns an instance of the current logger using log4net
		/// </summary>
		[Obsolete("The log4net implementation was replaced by provider implementation")]
		public static Ilog Log
		{
			get
			{
				return log;
			}
		}
		*/

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
		public sealed class Log
		{
			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Debug(object message)
			{
				LogHelper.Logger.Log(LogLevel.Debug, message);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <param name="ex" type="System.Exception">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Debug(object message, Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Debug, message, ex);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Info(object message)
			{
				LogHelper.Logger.Log(LogLevel.Info, message);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <param name="ex" type="System.Exception">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Info(object message, Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Info, message, ex);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Warn(object message)
			{
				LogHelper.Logger.Log(LogLevel.Warn, message);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <param name="ex" type="System.Exception">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Warn(object message, Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Warn, message, ex);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Error(object message)
			{
				LogHelper.Logger.Log(LogLevel.Error, message);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <param name="ex" type="System.Exception">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Error(object message, Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Error, message, ex);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Fatal(object message)
			{
				LogHelper.Logger.Log(LogLevel.Fatal, message);
			}

			/// <summary>
			///     
			/// </summary>
			/// <param name="message" type="object">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <param name="ex" type="System.Exception">
			///     <para>
			///         
			///     </para>
			/// </param>
			/// <returns>
			///     A void value...
			/// </returns>
			[Obsolete("Please, use provider implementation: LogHelper.Logger.Log")]
			public static void Fatal(object message, Exception ex)
			{
				LogHelper.Logger.Log(LogLevel.Fatal, message, ex);
			}
		}

		#endregion

		#region Provider implementation

		private static readonly LogProvider provider = LogProvider.Instance();

		//private static readonly LogProvider provider = new Rainbow.Configuration.Log4NetLogProvider();
		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static LogProvider Logger
		{
			get { return provider; }
		}

		#endregion

		#region Legacy implementation

		/// <summary>
		///     
		/// </summary>
		/// <param name="firstPart" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="fileExtension" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		[Obsolete("Now we use log4net")]
		public static string GetFileName(string firstPart, string fileExtension)
		{
			string sLogFileName;

			if (ConfigurationSettings.AppSettings["LogFileFormat"] == "Daily")
				sLogFileName = firstPart + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0') + DateTime.Today.Day.ToString().PadLeft(2, '0') + fileExtension;

			else if (ConfigurationSettings.AppSettings["LogFileFormat"] == "Monthly")
				sLogFileName = firstPart + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0') + fileExtension;

			else if (ConfigurationSettings.AppSettings["LogFileFormat"] == "Yearly")
				sLogFileName = firstPart + DateTime.Today.Year.ToString() + fileExtension;

			else
				sLogFileName = firstPart + fileExtension;
			return sLogFileName;
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A string value...
		/// </returns>
		[Obsolete("Now we use log4net")]
		public static string GetLogFolderLocation()
		{
			string sPath;

			if (ConfigurationSettings.AppSettings["AlternateLogLocation"] == string.Empty)
				sPath = PortalSettings.ApplicationPhisicalPath + "\\rb_logs\\";

			else
				sPath = ConfigurationSettings.AppSettings["AlternateLogLocation"];
			return sPath;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="sLogFileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		[Obsolete("Now we use log4net")]
		public static void DeleteLogFile(string sLogFileName)
		{
			string sPath;
			sPath = GetLogFolderLocation() + sLogFileName;
			File.Delete(sPath);
		}

		#endregion
	}
}