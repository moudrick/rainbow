namespace Rainbow.Framework.Providers.RainbowRoleProvider
{
    using System;
    using System.Configuration.Provider;
    using System.Runtime.Serialization;

    /// <summary>
    /// The rainbow role provider exception.
    /// </summary>
    [Serializable]
    public class RainbowRoleProviderException : ProviderException
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "RainbowRoleProviderException" /> class.
        /// </summary>
        public RainbowRoleProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public RainbowRoleProviderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public RainbowRoleProviderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        /// <param name="info">
        /// The object that holds the information to deserialize.
        /// </param>
        /// <param name="context">
        /// Contextual information about the source or destination.
        /// </param>
        protected RainbowRoleProviderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}