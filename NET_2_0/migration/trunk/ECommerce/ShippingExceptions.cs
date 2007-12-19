using System;

namespace Rainbow.ECommerce
{
	public class InvalidPackageException: InvalidOperationException
	{
		
		public InvalidPackageException() : base("Invalid Package")
		{
		}
		
		public InvalidPackageException(string message) : base(message)
		{
		}

		public InvalidPackageException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidPackageException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}

	public class InvalidDestinationException: InvalidOperationException
	{
		
		public InvalidDestinationException() : base("Invalid Destination")
		{
		}
		
		public InvalidDestinationException(string message) : base(message)
		{
		}

		public InvalidDestinationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidDestinationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
}
