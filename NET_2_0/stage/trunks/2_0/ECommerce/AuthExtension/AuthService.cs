using System;
using System.Web.Services.Protocols;


namespace Rainbow.Ecommerce.Services 
{
	public class AuthHeader : SoapHeader 
	{
		public string UserName;
		public string Password;
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class AuthExtensionAttribute : SoapExtensionAttribute 
	{
		int _priority = 1;
		
		public override int Priority 
		{
			get {return _priority;}
			set {_priority = value;}
		}
		public override Type ExtensionType 
		{
			get {return typeof(AuthExtension);}
		}
	}
	public class AuthExtension : SoapExtension 
	{
		public override void ProcessMessage(SoapMessage message) 
		{
			if(message.Stage == SoapMessageStage.AfterDeserialize) 
			{
				foreach(SoapHeader header in message.Headers) 
				{
					if(header is AuthHeader) 
					{
						AuthHeader credentials = (AuthHeader)header;
						// TODO: get mail and pass from admin or backup rb_users
						if(credentials.UserName.ToLower() == "test" && credentials.Password.ToLower() == "test")
							return;
						break;
					}
				}
				throw new SoapException("Unauthorized", SoapException.ClientFaultCode);
			}
		}
		public override Object GetInitializer(Type type) 
		{
			return GetType();
		}
		public override Object GetInitializer(LogicalMethodInfo info, SoapExtensionAttribute attribure) 
		{
			return null;
		}
		public override void Initialize(Object initializer) 
		{
		}
	}
}