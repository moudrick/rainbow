namespace Rainbow.Framework.Exceptions
{
    using System;

    /// <summary>
    /// Custom exception raised when code version is behind database version. Causes redirect to error page.
    /// </summary>
    [Serializable]
    public sealed class CodeVersionException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeVersionException"/> class. 
        /// Constructor with message.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        public CodeVersionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeVersionException"/> class. 
        /// Constructor with message and innerException.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        /// <param name="inner">
        /// Inner exception.
        /// </param>
        public CodeVersionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}