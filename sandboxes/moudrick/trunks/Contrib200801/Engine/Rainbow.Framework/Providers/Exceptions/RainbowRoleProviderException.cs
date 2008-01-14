using System;
using System.Configuration.Provider;
using System.Runtime.Serialization;

namespace Rainbow.Framework.Providers.Exceptions
{
    [Serializable]
    public class RainbowRoleProviderException : ProviderException
    {
        public RainbowRoleProviderException()
        {}

        public RainbowRoleProviderException(string message)
            : base(message)
        {}

        public RainbowRoleProviderException(string message, Exception inner)
            : base(message, inner)
        {}

        protected RainbowRoleProviderException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {}
    }
}
