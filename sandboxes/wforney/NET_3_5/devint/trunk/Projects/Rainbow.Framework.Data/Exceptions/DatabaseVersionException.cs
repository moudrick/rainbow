namespace Rainbow.Framework.Exceptions
{
    using System;

    /// <summary>
    /// Custom exception raised when database version is behind code version. Causes redirect to Database Update page.
    /// </summary>
    [Serializable]
    public sealed class DatabaseVersionException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVersionException"/> class. 
        ///     Default constructor
        /// </summary>
        public DatabaseVersionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVersionException"/> class. 
        /// Constructor with message.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        public DatabaseVersionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseVersionException"/> class. 
        /// Constructor with message and innerException.
        /// </summary>
        /// <param name="message">
        /// Text message to be included in log.
        /// </param>
        /// <param name="inner">
        /// Inner exception.
        /// </param>
        public DatabaseVersionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}