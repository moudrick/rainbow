using System;
using System.Collections.Generic;
using System.Text;

namespace Rainbow.Framework.Data.Providers.Geographic {

    /// <summary>
    /// 
    /// </summary>
    [global::System.Serializable]
    public class CountryNotFoundException : ApplicationException {

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryNotFoundException"/> class.
        /// </summary>
        public CountryNotFoundException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CountryNotFoundException( string message ) : base( message ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public CountryNotFoundException( string message, Exception inner ) : base( message, inner ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected CountryNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) { }
    }
}
