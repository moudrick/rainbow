using System;
using System.Configuration.Provider;
using System.Runtime.Serialization;

namespace Rainbow.Framework.Providers.Exceptions
{
    /// <summary>
    /// Rainbow-specific provider exception
    /// </summary>
    [Serializable]
    public class RainbowMembershipProviderException : ProviderException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowMembershipProviderException"/> class.
        /// </summary>
        public RainbowMembershipProviderException()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowMembershipProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RainbowMembershipProviderException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowMembershipProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RainbowMembershipProviderException(string message, Exception inner)
            : base(message, inner)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowMembershipProviderException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the information to deserialize.</param>
        /// <param name="context">Contextual information about the source or destination.</param>
        protected RainbowMembershipProviderException(SerializationInfo info,
                                                     StreamingContext context)
            : base(info, context)
        {}
    }
}