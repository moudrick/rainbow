namespace Rainbow.Framework.Exceptions
{
    using System;
    using System.Net;

    using Rainbow.Framework.Configuration;

    /// <summary>
    /// Custom Exception class for Rainbow.
    /// </summary>
    public class RainbowRedirect : Exception
    {
        //		/// <summary>
        // 		/// Default constructor.
        // 		/// </summary>
        // 		public RainbowRedirect()
        // 		{
        // 		}
        // 		/// <summary>
        // 		/// Constructor with message.
        // 		/// </summary>
        // 		/// <param name="message">Text message to be included in log.</param>
        // 		public RainbowRedirect(string message) : base(message)
        // 		{
        // 		}
        // 		/// <summary>
        // 		/// Constructor with message and innerException.
        // 		/// </summary>
        // 		/// <param name="message">Text message to be included in log.</param>
        // 		/// <param name="inner">Inner exception</param>
        // 		public RainbowRedirect(string message, Exception inner) : base(message, inner)
        // 		{
        // 		}
        // 		public RainbowRedirect(Rainbow.Framework.Configuration.LogLevel level, string message, Exception inner) : base(message, inner)
        // 		{
        // 			Level = level;
        // 		}
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirect"/> class.
        /// </summary>
        /// <param name="redirectUrl">
        /// The redirect URL.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public RainbowRedirect(string redirectUrl, LogLevels level, string message)
            : base(message)
        {
            this.StatusCode = HttpStatusCode.NotFound;
            this.Level = level;
            this.RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirect"/> class.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public RainbowRedirect(LogLevels level, HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.RedirectUrl = Settings.SmartErrorRedirect;
            this.Level = level;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirect"/> class.
        /// </summary>
        /// <param name="redirectUrl">
        /// The redirect URL.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public RainbowRedirect(string redirectUrl, LogLevels level, string message, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = HttpStatusCode.NotFound;
            this.Level = level;
            this.RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRedirect"/> class.
        /// </summary>
        /// <param name="redirectUrl">
        /// The redirect URL.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public RainbowRedirect(
            string redirectUrl, LogLevels level, HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.Level = level;
            this.StatusCode = statusCode;
            this.RedirectUrl = redirectUrl;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     ExceptionLevel enum
        /// </summary>
        /// <value>The level.</value>
        public LogLevels Level { get; set; }

        // private string _redirectUrl = ConfigurationSettings.AppSettings["SmartErrorRedirect"];

        /// <summary>
        ///     Gets or sets the redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }

        /// <summary>
        ///     HttpStatusCode enum
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; set; }

        #endregion

        // 		/// <summary>
        // 		/// Helper for de-serialization.
        // 		/// </summary>
        // 		/// <param name="info"></param>
        // 		/// <param name="context"></param>
        // 		protected RainbowRedirect(SerializationInfo info, StreamingContext context) : base(info,context)
        // 		{
        // 			Level = (Rainbow.Framework.LogLevel)info.GetValue("_level",typeof(Rainbow.Framework.Configuration.LogLevel));
        // 			StatusCode = (HttpStatusCode)info.GetValue("_statusCode",typeof(System.Net.HttpStatusCode));
        // 		}
        // 		/// <summary>
        // 		/// Helper for serialization.
        // 		/// </summary>
        // 		/// <param name="info"></param>
        // 		/// <param name="context"></param>
        // 		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        // 		public override void GetObjectData(SerializationInfo info, StreamingContext context)
        // 		{
        // 			info.AddValue("_level", (int)Level, typeof(Rainbow.Framework.Configuration.LogLevel));
        // 			info.AddValue("_statusCode", (int)StatusCode, typeof(System.Net.HttpStatusCode));
        // 			base.GetObjectData(info,context);
        // 		}
    }
}