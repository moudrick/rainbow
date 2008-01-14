using System;
using System.Runtime.Serialization;

namespace Rainbow.Framework.Providers.Exceptions
{
    [Serializable]
    public class CountryNotFoundException : ApplicationException
    {
        public CountryNotFoundException()
        {}

        public CountryNotFoundException(string message) : base(message)
        {}

        public CountryNotFoundException(string message, Exception inner) : base(message, inner)
        {}

        protected CountryNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {}
    }
}
