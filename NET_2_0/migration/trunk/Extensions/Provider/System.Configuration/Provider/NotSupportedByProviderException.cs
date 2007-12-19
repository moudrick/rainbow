using System.Runtime.Serialization;

namespace System.Configuration.Provider
{
	/// <summary>
	/// Summary description for NotSupportedByProviderException.
	/// </summary>
	[Serializable]
	public class NotSupportedByProviderException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public NotSupportedByProviderException()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public NotSupportedByProviderException(string message) : base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected NotSupportedByProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public NotSupportedByProviderException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}