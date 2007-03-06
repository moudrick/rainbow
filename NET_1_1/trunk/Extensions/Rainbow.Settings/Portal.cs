using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using Rainbow.Configuration;
using Rainbow.Context;

namespace Rainbow.Settings
{
	/// <summary>
	/// This class contains useful information for Extension, Module and Core Developers.
	/// </summary>
	public sealed class Portal
	{
		private static Context.Reader context = new Context.Reader(new WebContextReader());

		/// <summary>
		/// Sets reader for context in this class
		/// </summary>
		/// <param name="reader">an instance of a Concrete Strategy Reader</param>
		public static void SetReader(Context.Reader reader)
		{
			context = reader;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		private Portal()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public static int CodeVersion
		{
			get
			{
				if (context.Current != null && context.Current.Application["CodeVersion"] != null)
					return (int) context.Current.Application["CodeVersion"];
				else
					return 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static int PageID
		{
			get
			{
				string strPageID = null;

				if (FindPageIdFromQueryString(context.Current.Request.QueryString, ref strPageID))
					return Config.GetIntegerFromString(false, strPageID, 0);
				else
					return 0;
			}

		}

		/// <summary>
		/// This static string fetches the site's alias either via querystring, cookie or domain and returns it
		/// </summary>
		public static string UniqueID
		{
			// new version - Jes1111 - 07/07/2005
			get
			{
				if (context.Current.Items["PortalAlias"] == null) // not already in context
				{
					string uniquePortalID = Config.DefaultPortal; // set default value

					FindAlias(context.Current.Request, ref uniquePortalID); // will change uniquePortalID if it can

					context.Current.Items.Add("PortalAlias", uniquePortalID); // add to context

					return uniquePortalID; // return current value
				}
				else // already in context
				{
					return (string) context.Current.Items["PortalAlias"]; // return from context
				}
			}
		}

		private static void FindAlias(HttpRequest request, ref string alias)
		{
			if (FindAliasFromQueryString(request.QueryString, ref alias))
			{
				return;
			}
			else if (FindAliasFromCookies(request.Cookies, ref alias))
			{
				return;
			}
			else
			{
				FindAliasFromUri(request.Url, ref alias, Config.DefaultPortal, Config.RemoveWWW, Config.RemoveTLD, Config.SecondLevelDomains);
				return;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cookies"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static bool FindAliasFromCookies(HttpCookieCollection cookies, ref string alias)
		{
			if (cookies["PortalAlias"] != null)
			{
				string cookieValue = cookies["PortalAlias"].Value.Trim().ToLower(CultureInfo.InvariantCulture);
				if (cookieValue.Length != 0)
				{
					alias = cookieValue;
					return true;
				}
				else
				{
					//ErrorHandler.Publish(LogLevel.Warn, "FindAliasFromCookies failed - PortalAlias found but value was empty.");
					return false;
				}
			}
			else
				return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static bool FindAliasFromQueryString(NameValueCollection queryString, ref string alias)
		{
			if (queryString != null)
			{
				if (queryString["Alias"] != null)
				{
					string[] queryStringValues = queryString.GetValues("Alias");
					string queryStringValue = string.Empty;

					if (queryStringValues.Length > 0)
						queryStringValue = queryStringValues[0].Trim().ToLower(CultureInfo.InvariantCulture);

					if (queryStringValue.Length != 0)
					{
						alias = queryStringValue;
						return true;
					}
					else
					{
						//ErrorHandler.Publish(LogLevel.Warn, "FindAliasFromQueryString failed - Alias param found but value was empty.");
						return false;
					}
				}
				else
					return false;
			}
			else
				return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="pageID"></param>
		/// <returns></returns>
		public static bool FindPageIdFromQueryString(NameValueCollection queryString, ref string pageID)
		{
			string[] queryStringValues;

			if (queryString != null)
			{
				if (queryString[GlobalInternalStrings.str_PageID] != null)
				{
					queryStringValues = queryString.GetValues(GlobalInternalStrings.str_PageID);
				}
				else if (queryString[GlobalInternalStrings.str_TabID] != null)
				{
					queryStringValues = queryString.GetValues(GlobalInternalStrings.str_TabID);
				}
				else
				{
					return false;
				}

				string queryStringValue = string.Empty;

				if (queryStringValues != null && queryStringValues.Length > 0)
					queryStringValue = queryStringValues[0].Trim().ToLower(CultureInfo.InvariantCulture);

				if (queryStringValue.Length != 0)
				{
					pageID = queryStringValue;
					return true;
				}
				else
				{
					//ErrorHandler.Publish(LogLevel.Warn, "FindPageIDFromQueryString failed - Alias param found but value was empty.");
					return false;
				}
			}
			else
				return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestUri"></param>
		/// <param name="alias"></param>
		/// <param name="defaultPortal"></param>
		/// <param name="removeWWW"></param>
		/// <param name="removeTLD"></param>
		/// <param name="secondLevelDomains"></param>
		/// <returns></returns>
		public static bool FindAliasFromUri(Uri requestUri, ref string alias, string defaultPortal, bool removeWWW, bool removeTLD, string secondLevelDomains)
		{
			// if request is to localhost, return default portal 
			if (requestUri.IsLoopback)
			{
				alias = defaultPortal;
				return true;
			}
			else if (requestUri.HostNameType == UriHostNameType.Dns) // get it from hostname
			{
				char[] hostDelim = new char[] {'.'};

				// step 1: split hostname into parts
				ArrayList hostPartsList = new ArrayList(requestUri.Host.Split(hostDelim));

				// step 2: do we need to remove "www"?
				if (removeWWW && hostPartsList[0].ToString() == "www")
					hostPartsList.RemoveAt(0);

				// step 3: do we need to remove TLD?
				if (removeTLD)
				{
					hostPartsList.Reverse();
					if (hostPartsList.Count > 2 && hostPartsList[0].ToString().Length == 2)
					{
						// this is a ccTLD, so need to check if next segment is a pseudo-gTLD
						ArrayList gTLDs = new ArrayList(secondLevelDomains.Split(new char[] {';'}));
						if (gTLDs.Contains(hostPartsList[1].ToString()))
							hostPartsList.RemoveRange(0, 2);
						else
							hostPartsList.RemoveAt(0);
					}
					else
					{
						hostPartsList.RemoveAt(0);
					}
					hostPartsList.Reverse();
				}

				// step 4: re-assemble the remaining parts
				alias = String.Join(".", (string[]) hostPartsList.ToArray(typeof (String)));
				return true;
			}
			else
			{
				alias = defaultPortal;
				return true;
			}
		}

		/// <summary>
		/// Database connection
		/// </summary>
		[Obsolete("Please use Rainbow.Settings.Config.ConnectionString")]
		public static string ConnectionString
		{
			get { return Config.ConnectionString; }
		}

		/// <summary>
		/// SmtpServer
		/// </summary>
		[Obsolete("Please use Rainbow.Settings.Config.SmtpServer")]
		public static string SmtpServer
		{
			get { return Config.SmtpServer; }
		}

	}
}