using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace Rainbow.Framework.Providers.RainbowRoleProvider {

    /// <summary>
    /// 
    /// </summary>
    [global::System.Serializable]
    public class RainbowRoleProviderException : ProviderException {

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        public RainbowRoleProviderException() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RainbowRoleProviderException( string message )
            : base( message ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RainbowRoleProviderException( string message, Exception inner )
            : base( message, inner ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowRoleProviderException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the information to deserialize.</param>
        /// <param name="context">Contextual information about the source or destination.</param>
        protected RainbowRoleProviderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) {
        }
    }
}
