namespace Rainbow.Framework.Exceptions
{
    using System;

    /// <summary>
    /// Custom exception raised when database appears to be unreachable.
    ///     Configured to redirect to static (HTML) page.
    /// </summary>
    [Serializable]
    public sealed class DatabaseUnreachableException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUnreachableException"/> class. 
        /// Constructor with message.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        public DatabaseUnreachableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUnreachableException"/> class. 
        /// Constructor with message and innerException.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        /// <param name="inner">
        /// Inner exception.
        /// </param>
        public DatabaseUnreachableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}