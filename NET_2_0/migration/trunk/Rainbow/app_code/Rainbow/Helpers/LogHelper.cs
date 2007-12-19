using System;
using Rainbow.Configuration;

namespace Rainbow.Helpers 
{ 
	/// <summary> 
	/// Log Helper for use with Provider implementation
	/// </summary> 
	public sealed class LogHelper 
	{
		#region Old style call
		/// <summary>
		/// Log Class - Old style call
		/// </summary>
		public sealed class Log 
		{
			/// <summary>
			/// Debug Level
			/// </summary>
			/// <param name="message"></param>
			public static void Debug(object message) 
			{
				LogHelper.Logger.Log(LogLevel.Debug, message);
			}

			/// <summary>
			/// Debug Level
			/// </summary>
			/// <param name="message"></param>
			/// <param name="ex"></param>
			public static void Debug(object message, Exception ex) 
			{
				LogHelper.Logger.Log(LogLevel.Debug, message, ex);
			}

			/// <summary>
			/// Info  Level
			/// </summary>
			/// <param name="message"></param>
			public static void Info(object message) 
			{
				LogHelper.Logger.Log(LogLevel.Info, message);
			}
			/// <summary>
			/// Info Level
			/// </summary>
			/// <param name="message"></param>
			/// <param name="ex"></param>
			public static void Info(object message, Exception ex) 
			{
				LogHelper.Logger.Log(LogLevel.Info, message, ex);
			}

			/// <summary>
			/// Warn Level
			/// </summary>
			/// <param name="message"></param>
			public static void Warn(object message) 
			{
				LogHelper.Logger.Log(LogLevel.Warn, message);
			}

			/// <summary>
			/// Warn Level
			/// </summary>
			/// <param name="message"></param>
			/// <param name="ex"></param>
			public static void Warn(object message, Exception ex) 
			{
				LogHelper.Logger.Log(LogLevel.Warn, message, ex);
			}

			/// <summary>
			/// Error Level
			/// </summary>
			/// <param name="message"></param>
			public static void Error(object message) 
			{
				LogHelper.Logger.Log(LogLevel.Error, message);
			}

			/// <summary>
			/// Error Level
			/// </summary>
			/// <param name="message"></param>
			/// <param name="ex"></param>
			public static void Error(object message, Exception ex) 
			{
				LogHelper.Logger.Log(LogLevel.Error, message, ex);
			}

			/// <summary>
			/// Fatal Error
			/// </summary>
			/// <param name="message"></param>
			public static void Fatal(object message) 
			{
				LogHelper.Logger.Log(LogLevel.Fatal, message);
			}

			/// <summary>
			/// FAtal Error
			/// </summary>
			/// <param name="message"></param>
			/// <param name="ex"></param>
			public static void Fatal(object message, Exception ex) 
			{
				LogHelper.Logger.Log(LogLevel.Fatal, message, ex);
			}
		}
		#endregion

		#region Provider implementation
		private static readonly LogProvider provider = LogProvider.Instance();

		/// <summary>
		/// LogProvider Logger
		/// </summary>
		public static LogProvider Logger
		{
			get
			{
				return provider;
			}
		}

		#endregion
	}
}
