/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Coder: Mario Hartmann [mario[at]hartmann[dot]net // http://mario.hartmann.net/]
 * Original version: C#
 * Original product name: Rainbow
 * Official site: http://www.rainbowportal.net
 * Last updated Date: 2004/11/29
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/

using System;
using System.Web;

namespace Rainbow.Security
{
	/// <summary>
	/// Summary description for Security.
	/// </summary>
	public class MobilePortalSecurity //:Rainbow.Security.PortalSecurity
	{
		/// <summary>
		/// Single point access deny.
		/// Called when there is an unauthorized access attempt.
		/// </summary>
		public  static void AccessDenied()
		{
			//if (HttpContext.Current.User.Identity.IsAuthenticated)
				throw new HttpException(403, "Access Denied", 2);

			//	else
			//		HttpContext.Current.Response.Redirect(HttpUrlBuilder.BuildUrl("~/Admin/Logon.aspx"));
		}
	}
}
