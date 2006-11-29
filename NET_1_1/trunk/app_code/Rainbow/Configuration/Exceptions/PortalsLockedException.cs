using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Rainbow.Configuration
{
	/// <summary>
	/// Custom exception which causes all traffic (except from nominated
	/// IP addresses) to divert to a configured page (usually a static HTML page).
	/// This means you can perform maintenance tasks on an installation
	/// with minimal disruption from incoming traffic, or test a new portal before
	/// opening it.
	/// </summary>
	[Serializable]
	public class PortalsLockedException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PortalsLockedException()
		{
		}

		/// <summary>
		/// Constructor with message.
		/// </summary>
		/// <param name="message">Text message to be included in log.</param>
		public PortalsLockedException(string message) : base(message)
		{
		}

		/// <summary>
		/// Constructor with message and innerException.
		/// </summary>
		/// <param name="message">Text message to be included in log.</param>
		/// <param name="inner">Inner exception</param>
		public PortalsLockedException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Constructor with ExceptionLevel, message and innerException.
		/// </summary>
		/// <param name="level">ExceptionLevel enum</param>
		/// <param name="message">Text message to be included in log.</param>
		/// <param name="inner">Inner exception</param>
		public PortalsLockedException(LogLevel level, string message, Exception inner) : base(message, inner)
		{
			_level = level;
		}

		/// <summary>
		/// Constructor with ExceptionLevel, HttpStatusCode, message and innerException.
		/// </summary>
		/// <param name="level">ExceptionLevel enumerator</param>
		/// <param name="statusCode">HttpStatusCode enum</param>
		/// <param name="message">Text message to be included in log.</param>
		/// <param name="inner">Inner exception</param>
		public PortalsLockedException(LogLevel level, HttpStatusCode statusCode, string message, Exception inner) : base(message, inner)
		{
			_level = level;
			_statusCode = statusCode;
		}


		private LogLevel _level = LogLevel.Fatal;
		/// <summary>
		/// ExceptionLevel enumerator.
		/// </summary>
		public LogLevel Level
		{
			get{return _level;}
			set{_level = value;}
		}

		private HttpStatusCode _statusCode = HttpStatusCode.ServiceUnavailable;
		public HttpStatusCode StatusCode
		{
			get{return _statusCode;}
			set{_statusCode = value;}
		}

		/// <summary>
		/// Helper for de-serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected PortalsLockedException(SerializationInfo info, StreamingContext context) : base(info,context)
		{
			_level = (LogLevel)info.GetValue("_level",typeof(LogLevel));
			_statusCode = (HttpStatusCode)info.GetValue("_statusCode",typeof(HttpStatusCode));
		}

		/// <summary>
		/// Helper for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_level", (int)_level, typeof(LogLevel));
			info.AddValue("_statusCode", (int)_statusCode, typeof(HttpStatusCode));
			base.GetObjectData(info,context);
		}	
	}
}
