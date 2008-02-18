using System;
using System.Runtime.Serialization;

namespace Rainbow.Framework.Providers.Exceptions
{
    [Serializable]
    public class StateNotFoundException : ApplicationException
    {
        public StateNotFoundException()
        {}

        public StateNotFoundException(string message) : base(message)
        {}

        public StateNotFoundException(string message, Exception inner) : base(message, inner)
        {}

        protected StateNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {}
    }
}
