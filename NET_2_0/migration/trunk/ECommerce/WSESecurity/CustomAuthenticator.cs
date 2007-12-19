using System;
using System.Security.Permissions;
using Microsoft.Web.Services2.Security.Tokens;

namespace WSESecurity
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
	public class CustomAuthenticator : UsernameTokenManager
	{
		protected override string AuthenticateToken(UsernameToken token) 
		{
			// TODO: DB Authentication
			if(token == null)
				throw new ArgumentNullException();
			if(token.Username == "danijel")
				return "danijel";
			else return "not ok";
		}
	}
}
