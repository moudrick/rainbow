namespace Rainbow.Framework.Exceptions
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Custom exception which causes all traffic (except from nominated
    ///     IP addresses) to divert to a configured page (usually a static HTML page).
    ///     This means you can perform maintenance tasks on an installation
    ///     with minimal disruption from incoming traffic, or test a new portal before
    ///     opening it.
    /// </summary>
    [Serializable]
    public class PortalsLockedException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsLockedException"/> class. 
        ///     Default constructor.
        /// </summary>
        public PortalsLockedException()
        {
            this.Level = LogLevels.Fatal;
            this.StatusCode = HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsLockedException"/> class. 
        /// Constructor with message.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        public PortalsLockedException(string message)
            : base(message)
        {
            this.Level = LogLevels.Fatal;
            this.StatusCode = HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsLockedException"/> class. 
        /// Constructor with message and innerException.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        /// <param name="inner">
        /// Inner exception
        /// </param>
        public PortalsLockedException(string message, Exception inner)
            : base(message, inner)
        {
            this.Level = LogLevels.Fatal;
            this.StatusCode = HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsLockedException"/> class. 
        /// Constructor with ExceptionLevel, message and innerException.
        /// </summary>
        /// <param name="level">
        /// ExceptionLevel enum
        /// </param>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        /// <param name="inner">
        /// Inner exception
        /// </param>
        public PortalsLockedException(LogLevels level, string message, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = HttpStatusCode.ServiceUnavailable;
            this.Level = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsLockedException"/> class. 
        /// Constructor with ExceptionLevel, HttpStatusCode, message and innerException.
        /// </summary>
        /// <param name="level">
        /// ExceptionLevel enumerator
        /// </param>
        /// <param name="statusCode">
        /// HttpStatusCode enum
        /// </param>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        /// <param name="inner">
        /// Inner exception
        /// </param>
        public PortalsLockedException(LogLevels level, HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.Level = level;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalsLockedException"/> class. 
        /// Helper for de-serialization.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// The info parameter is null. 
        /// </exception>
        protected PortalsLockedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Level = (LogLevels)info.GetValue("_level", typeof(LogLevels));
            this.StatusCode = (HttpStatusCode)info.GetValue("_statusCode", typeof(HttpStatusCode));
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets ExceptionLevel enumerator.
        /// </summary>
        /// <value>The level.</value>
        public LogLevels Level { get; set; }

        /// <summary>
        ///     Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Helper for serialization.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The info parameter is a null reference (Nothing in Visual Basic). 
        /// </exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_level", (int)this.Level, typeof(LogLevels));
            info.AddValue("_statusCode", (int)this.StatusCode, typeof(HttpStatusCode));
            base.GetObjectData(info, context);
        }

        #endregion
    }
}