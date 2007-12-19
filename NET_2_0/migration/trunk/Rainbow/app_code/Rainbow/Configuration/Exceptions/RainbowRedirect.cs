using System;
using System.Configuration;
using System.Net;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Custom Exception class for Rainbow.
	/// </summary>
	// Serializable
	public class RainbowRedirect : Exception 
	{
//		/// <summary>
//		/// Default constructor.
//		/// </summary>
//		public RainbowRedirect()
//		{
//		}
//
//		/// <summary>
//		/// Constructor with message.
//		/// </summary>
//		/// <param name="message">Text message to be included in log.</param>
//		public RainbowRedirect(string message) : base(message)
//		{
//		}
//
//		/// <summary>
//		/// Constructor with message and innerException.
//		/// </summary>
//		/// <param name="message">Text message to be included in log.</param>
//		/// <param name="inner">Inner exception</param>
//		public RainbowRedirect(string message, Exception inner) : base(message, inner)
//		{
//		}
//
//
//		public RainbowRedirect(Rainbow.Configuration.LogLevel level, string message, Exception inner) : base(message, inner)
//		{
//			Level = level;
//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="redirectUrl"></param>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public RainbowRedirect(string redirectUrl, LogLevel level, string message) : base(message)
		{
			Level = level;
			RedirectUrl = redirectUrl;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public RainbowRedirect(LogLevel level, HttpStatusCode statusCode, string message, Exception inner) : base(message, inner)
		{
			Level = level;
			StatusCode = statusCode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="redirectUrl"></param>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public RainbowRedirect(string redirectUrl, LogLevel level, string message, Exception inner) : base(message, inner)
		{
			Level = level;
			RedirectUrl = redirectUrl;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="redirectUrl"></param>
		/// <param name="level"></param>
		/// <param name="statusCode"></param>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public RainbowRedirect(string redirectUrl, LogLevel level, HttpStatusCode statusCode, string message, Exception inner) : base(message, inner)
		{
			Level = level;
			StatusCode = statusCode;
			RedirectUrl = redirectUrl;
		}


		private HttpStatusCode _statusCode = HttpStatusCode.NotFound;
		/// <summary>
		/// HttpStatusCode enum
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get{return _statusCode;}
			set{_statusCode = value;}
		}

		private LogLevel _level = LogLevel.Info;
		/// <summary>
		/// ExceptionLevel enum
		/// </summary>
		public LogLevel Level
		{
			get{return _level;}
			set{_level = value;}
		}

		//private string _redirectUrl = ConfigurationSettings.AppSettings["SmartErrorRedirect"];
		private string _redirectUrl = Rainbow.Settings.Config.SmartErrorRedirect;
		public string RedirectUrl
		{
			get{return _redirectUrl;}
			set{_redirectUrl = value;}
		}
//		/// <summary>
//		/// Helper for de-serialization.
//		/// </summary>
//		/// <param name="info"></param>
//		/// <param name="context"></param>
//		protected RainbowRedirect(SerializationInfo info, StreamingContext context) : base(info,context)
//		{
//			Level = (LogLevel)info.GetValue("_level",typeof(Rainbow.Configuration.LogLevel));
//			StatusCode = (HttpStatusCode)info.GetValue("_statusCode",typeof(System.Net.HttpStatusCode));
//		}
//
//		/// <summary>
//		/// Helper for serialization.
//		/// </summary>
//		/// <param name="info"></param>
//		/// <param name="context"></param>
//		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
//		public override void GetObjectData(SerializationInfo info, StreamingContext context)
//		{
//			info.AddValue("_level", (int)Level, typeof(Rainbow.Configuration.LogLevel));
//			info.AddValue("_statusCode", (int)StatusCode, typeof(System.Net.HttpStatusCode));
//			base.GetObjectData(info,context);
//		}
	}
}