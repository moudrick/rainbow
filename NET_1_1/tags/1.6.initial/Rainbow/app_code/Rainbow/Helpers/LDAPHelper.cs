using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using Novell.Directory.Ldap;
using Rainbow.BLL.UserConfig;
using Rainbow.Configuration;
using Rainbow.Security;
using Rainbow.Settings;

namespace Rainbow.Helpers
{
	/// <summary>
	/// Summary description for LDAPHelper.
	/// </summary>
	public class LDAPHelper
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		public enum AccountType
		{
			/// <summary>
			///     None
			/// </summary>
			none = 0,
			/// <summary>
			///     User
			/// </summary>
			user = 1,
			/// <summary>
			///     Group
			/// </summary>
			group = 2
		}

		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		public LDAPHelper()
		{
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="Refresh" type="bool">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="Groups" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="AppCache" type="System.Web.Caching.Cache">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.DataTable value...
		/// </returns>
		public static DataTable GetMemberList(bool Refresh, string Groups, Cache AppCache)
		{
			// see if we want to refresh, if not, get it from the cache if available
			if ( ! Refresh )
			{
				object tmp = AppCache.Get("LDAPUsersAndGroups" + Groups);
				if ( tmp != null )
					return ((DataSet)tmp).Tables[0];
			}



			// create dataset
			using (DataSet ds = new DataSet())
				   {
				using (DataTable dt = new DataTable())
				{
					ds.Tables.Add(dt);

					DataColumn dc = new DataColumn("DisplayName", typeof(string));
					dt.Columns.Add(dc);
					dc = new DataColumn("AccountName", typeof(string));
					dt.Columns.Add(dc);
					dc = new DataColumn("AccountType", typeof(string));
					dt.Columns.Add(dc);

					// add built in users first
					dt.Rows.Add(new Object[]{"Admins", "Admins", "group"});
					dt.Rows.Add(new Object[]{"All Users", "All Users", "group"});
					dt.Rows.Add(new Object[]{"Authenticated Users", "Authenticated Users", "group"});
					dt.Rows.Add(new Object[]{"Unauthenticated Users", "Unauthenticated Users", "group"});


					//string[] login = ConfigurationSettings.AppSettings["LDAPLogin"].Split(";".ToCharArray());
					//string[] server = ConfigurationSettings.AppSettings["LDAPServer"].Split(":".ToCharArray());
					string[] login = Config.LDAPLogin.Split(";".ToCharArray());
					string[] server = Config.LDAPServer.Split(":".ToCharArray());
					string[] group = Groups.Split("/".ToCharArray());
					Hashtable users = new Hashtable();
					LdapConnection ldapConn= new LdapConnection();
					ldapConn.Connect(server[0].Trim(), Convert.ToInt32(server[1]));
					try
					{
						ldapConn.Bind(login[0].Trim(),login[1].Trim());
						LdapSearchResults lsc=ldapConn.Search(group[0].Trim(),
							LdapConnection.SCOPE_ONE,group[1].Trim(), null, false);

						while (lsc.hasMore())
						{
							LdapEntry nextEntry = null;
							try 
							{
								nextEntry = lsc.next();
							}
							catch(LdapException e) 
							{
								LogHelper.Logger.Log(LogLevel.Error, "Error Occured in LDAPHelper Logged Line 85", e);
								continue;
							}
							string fullname = nextEntry.DN;
							//string accountname = nextEntry.DN;
							dt.Rows.Add(new Object[]{fullname , fullname, AccountType.group.ToString()});

							LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
							foreach(LdapAttribute attribute in attributeSet)
							{
								if (attribute.Name.ToLower().Equals("member"))
								{
									foreach(string attributeVal in attribute.StringValueArray)
									{
										if(users[attributeVal] == null)
										{
											users.Add(attributeVal, attributeVal);
											dt.Rows.Add(new Object[]{attributeVal , attributeVal, AccountType.user.ToString()});
										}
									}
								}
							}

						}
					} 
					finally
					{
						ldapConn.Disconnect();
					}			
		

					// add dataset to the cache
					AppCache.Insert("LDAPUsersAndGroups" + Groups, ds);

					// return datatable
					return dt;
				}
			}
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="path" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public static string GetContext(string path)
		{
			StringBuilder sb = new StringBuilder();
			string[] nodes = path.Split(",".ToCharArray());
		
			if (nodes != null && nodes.Length != 0 )
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					string[] value = nodes[i].Split("=".ToCharArray());
					if (i > 0)
					{
						sb.Append(".".ToCharArray());
					}
					sb.Append(value[1]);
				}
			}

			return sb.ToString();
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="dn" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="password" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="persistent" type="bool">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="redirectPage" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public static string SignOn(string dn, string password, bool persistent, string redirectPage) 
		{
			// Obtain PortalSettings from Current Context
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

			int uid = 0;
			User usr = LDAPHelper.Login(dn, password);
			if (usr != null)
			{

				// [START] bja 5/17/2003: Get user's window configuration
				// first clear any state the user may have in the bag (cookie,session,application)
				UserDesktop.ResetDesktop(uid);
				// load in user window configuration
				UserDesktop.ConfigureDesktop(uid,portalSettings.PortalID);
				// [END] bja 5/17/2003: Get user's window configuration

				// Thierry (tiptopweb), 12 Apr 2003: Save old ShoppingCartID
				//ShoppingCartDB shoppingCart = new ShoppingCartDB();
				//string tempCartID = ShoppingCartDB.GetCurrentShoppingCartID();
				// Thierry (tiptopweb), 12 Apr 2003: migrate shopping cart
				// Thierry (tiptopweb), 5 May 2003: use Email only as CartID
				//shoppingCart.MigrateCart(tempCartID, usr.Email.ToString());

				// Use security system to set the UserID within a client-side Cookie
				FormsAuthentication.SetAuthCookie(usr.ToString(), persistent);

				// Rainbow Security cookie Required if we are sharing a single domain 
				// with portal Alias in the URL
				
				// Set a cookie to persist authentication for each portal 
				// so user can be reauthenticated 
				// automatically if they choose to Remember Login					
				HttpCookie hck = HttpContext.Current.Response.Cookies["Rainbow_" + portalSettings.PortalAlias.ToLower()];
				hck.Value = usr.ToString(); //Fill all data: name + email + id
				hck.Path = "/";

				if (persistent) // Keep the cookie?
				{
					hck.Expires = DateTime.Now.AddYears(50);
				}
				

				if (redirectPage == null || redirectPage == string.Empty)
				{
					// Redirect browser back to originating page
					if(HttpContext.Current.Request.UrlReferrer != null)
					{
						HttpContext.Current.Response.Redirect(HttpContext.Current.Request.UrlReferrer.ToString());
					}
					else
					{
						HttpContext.Current.Response.Redirect(Path.ApplicationRoot);
					}
					return usr.Email;
				}
				else
				{
					HttpContext.Current.Response.Redirect(redirectPage);
				}
			}
			return null;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="dn" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="password" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A Rainbow.Security.User value...
		/// </returns>
		public static User Login(string dn, string password) 
		{
			if (dn == null || dn == string.Empty || password == null || password == string.Empty)
			{
				return null;
			}

			User user = null; 
			// jes1111 - string[] server = ConfigurationSettings.AppSettings["LDAPServer"].Split(":".ToCharArray());
			string[] server = Config.LDAPServer.Split(":".ToCharArray());

			LdapConnection ldapConn= new LdapConnection();
			ldapConn.Connect(server[0].Trim(), Convert.ToInt32(server[1]));
			try
			{
				ldapConn.Bind(dn, password);
				LdapSearchResults lsc=ldapConn.Search(dn,
					LdapConnection.SCOPE_SUB, "ObjectClass=*", null, false);

				while (lsc.hasMore())
				{
					LdapEntry nextEntry = null;
					try 
					{
						nextEntry = lsc.next();
					}
					catch(LdapException e) 
					{
						LogHelper.Logger.Log(LogLevel.Error, "Error Occured in LDAPHelper Logged Line 236", e);
						continue;
					}

					string userID = nextEntry.DN;
					string userName = nextEntry.DN;
					string email = nextEntry.DN;;

					LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
					foreach(LdapAttribute attribute in attributeSet)
					{
						if (attribute.Name.ToLower().Equals("fullname"))
						{
							userName = attribute.StringValue;
						}
						else if(attribute.Name.ToLower().Equals("mail"))
						{
							email = attribute.StringValue;
						}
					}

					user = new User(userName, email, userID);
				}
			}
			catch{}
			finally
			{
				ldapConn.Disconnect();
			}			

			return user;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="dn" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.Hashtable value...
		/// </returns>
		public static Hashtable GetUserProfile(string dn)
		{
			Hashtable userProfile = new Hashtable();
//jes1111
//			string[] login = ConfigurationSettings.AppSettings["LDAPLogin"].Split(";".ToCharArray());
//			string[] server = ConfigurationSettings.AppSettings["LDAPServer"].Split(":".ToCharArray());
//			ArrayList admins = new ArrayList(
//				ConfigurationSettings.AppSettings["LDAPAdministratorGroup"].Split(";".ToCharArray()));
			string[] login = Config.LDAPLogin.Split(";".ToCharArray());
			string[] server = Config.LDAPServer.Split(":".ToCharArray());
			//ArrayList admins = new ArrayList(Config.LDAPAdministratorGroup.Split(";".ToCharArray()));
			
			LdapConnection ldapConn= new LdapConnection();
			ldapConn.Connect(server[0].Trim(), Convert.ToInt32(server[1]));
			try
			{
				ldapConn.Bind(login[0].Trim(),login[1].Trim());
				LdapSearchResults lsc=ldapConn.Search(dn,
					LdapConnection.SCOPE_SUB, "ObjectClass=*", null, false);

				while (lsc.hasMore())
				{
					LdapEntry nextEntry = null;
					try 
					{
						nextEntry = lsc.next();
					}
					catch(LdapException e) 
					{
						LogHelper.Logger.Log(LogLevel.Error, "Error Occured in LDAPHelper Logged Line 295", e);
						continue;
					}
					
					userProfile.Add("DN", new string[]{nextEntry.DN});
					LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
					foreach(LdapAttribute attribute in attributeSet)
					{
						userProfile.Add(attribute.Name.ToUpper(), attribute.StringValueArray);
					}

				}
			} 
			finally
			{
				ldapConn.Disconnect();
			}
			
			return userProfile;
		}
		/// <summary>
		///     
		/// </summary>
		/// <param name="dn" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string[] value...
		/// </returns>
		public static string[] GetRoles(string dn) 
		{
			ArrayList userRoles = new ArrayList();
// jes1111
//			string[] login = ConfigurationSettings.AppSettings["LDAPLogin"].Split(";".ToCharArray());
//			string[] server = ConfigurationSettings.AppSettings["LDAPServer"].Split(":".ToCharArray());
//			ArrayList admins = new ArrayList(
//				ConfigurationSettings.AppSettings["LDAPAdministratorGroup"].Split(";".ToCharArray()));
			string[] login = Config.LDAPLogin.Split(";".ToCharArray());
			string[] server = Config.LDAPServer.Split(":".ToCharArray());
			ArrayList admins = new ArrayList(Config.LDAPAdministratorGroup.Split(";".ToCharArray()));
			bool admin = false;
			
			LdapConnection ldapConn= new LdapConnection();
			ldapConn.Connect(server[0].Trim(), Convert.ToInt32(server[1]));
			try
			{
				ldapConn.Bind(login[0].Trim(),login[1].Trim());
				LdapSearchResults lsc=ldapConn.Search(dn,
					LdapConnection.SCOPE_SUB, "ObjectClass=*", null, false);

				while (lsc.hasMore())
				{
					LdapEntry nextEntry = null;
					try 
					{
						nextEntry = lsc.next();
					}
					catch(LdapException e) 
					{
						LogHelper.Logger.Log(LogLevel.Error, "Error Occured in LDAPHelper Logged Line 342", e);
						continue;
					}
					
					if (!admin)
					{
						if (admins.Contains(nextEntry.DN))
						{
							admin = true;
						}
					}

					LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
					foreach(LdapAttribute attribute in attributeSet)
					{
						if (attribute.Name.ToLower().Equals("groupmembership"))
						{
							if (!admin)
							{
								foreach(string val in attribute.StringValueArray)
								{
									if (admins.Contains(val))
									{
										admin = true;
										break;
									}
								}
							}
							userRoles.AddRange(attribute.StringValueArray);
						}
					}

				}
			} 
			finally
			{
				ldapConn.Disconnect();
			}
			
			if (admin)
			{
				userRoles.Add("Admins");
			}

			return (string[]) userRoles.ToArray(typeof(string));
		}
	}
}
