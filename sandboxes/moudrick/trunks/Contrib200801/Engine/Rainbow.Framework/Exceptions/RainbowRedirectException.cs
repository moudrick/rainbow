using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Rainbow.Framework.Settings;

namespace Rainbow.Framework.Exceptions
{
    /// <summary>
    /// Custom Exception class for Rainbow.
    /// </summary>
    // Serializable
    public class RainbowRedirectException : Exception
    {
        HttpStatusCode statusCode = HttpStatusCode.NotFound;
        LogLevel logLevel = LogLevel.Info;
        string redirectUrl = Config.SmartErrorRedirect;
        //string redirectUrl = ConfigurationSettings.AppSettings["SmartErrorRedirect"];

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl
        {
            get { return redirectUrl; }
            set { redirectUrl = value; }
        }

        /// <summary>
        /// HttpStatusCode enum
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        /// <summary>
        /// ExceptionLevel enum
        /// </summary>
        /// <value>The level.</value>
        public LogLevel Level
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        //		/// <summary>
        //		/// Default constructor.
        //		/// </summary>
        //		public RainbowRedirectException()
        //		{
        //		}
        //
        //		/// <summary>
        //		/// Constructor with message.
        //		/// </summary>
        //		/// <param name="message">Text message to be included in log.</param>
        //		public RainbowRedirectException(string message) : base(message)
        //		{
        //		}
        //
        //		/// <summary>
        //		/// Constructor with message and innerException.
        //		/// </summary>
        //		/// <param name="message">Text message to be included in log.</param>
        //		/// <param name="inner">Inner exception</param>
        //		public RainbowRedirectException(string message, Exception inner) : base(message, inner)
        //		{
        //		}
        //
        //
        //		public RainbowRedirectException(Rainbow.Framework.Configuration.LogLevel level, string message, Exception inner) : base(message, inner)
        //		{
        //			Level = level;
        //		}

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirectException"/> class.
        /// </summary>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public RainbowRedirectException(string redirectUrl, LogLevel level, string message)
            : base(message)
        {
            Level = level;
            RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirectException"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RainbowRedirectException(LogLevel level, HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            Level = level;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirectException"/> class.
        /// </summary>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RainbowRedirectException(string redirectUrl, LogLevel level, string message, Exception inner)
            : base(message, inner)
        {
            Level = level;
            RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirectException"/> class.
        /// </summary>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="level">The level.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RainbowRedirectException(string redirectUrl, LogLevel level, HttpStatusCode statusCode, string message,
                               Exception inner)
            : base(message, inner)
        {
            Level = level;
            StatusCode = statusCode;
            RedirectUrl = redirectUrl;
        }
        
        /// <summary>
        /// Helper for de-serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected RainbowRedirectException(SerializationInfo info, StreamingContext context) 
            : base(info,context)
        {
	        Level = (LogLevel)info.GetValue("logLevel",typeof(LogLevel));
	        StatusCode = (HttpStatusCode)info.GetValue("statusCode",typeof(HttpStatusCode));
        }

        /// <summary>
        /// Helper for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
	        info.AddValue("logLevel", (int)Level, typeof(LogLevel));
	        info.AddValue("statusCode", (int)StatusCode, typeof(HttpStatusCode));
	        base.GetObjectData(info,context);
        }
    }
}
