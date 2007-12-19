using System;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace MasterCSharp.WebServices
{

	/// <summary>
	/// Summary description for CompressionExtensionAttribute.
	/// </summary>
	
	// Make the Attribute only Applicable to Methods
	[AttributeUsage(AttributeTargets.Method)]
	public class CompressionExtensionAttribute : System.Web.Services.Protocols.SoapExtensionAttribute
	{
    
		private int priority = 0;

		// Override the base class properties
		public override Type ExtensionType 
		{
			get { return typeof(CompressionExtension); }
		}

		public override int Priority 
		{
			get 
			{ 
				return priority; 
			}
			set 
			{ 
				priority = value; 
			}
		}

	}
}
