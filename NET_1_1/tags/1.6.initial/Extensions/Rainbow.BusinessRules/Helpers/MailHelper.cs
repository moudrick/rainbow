using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mail;
using Rainbow.UI.DataTypes;

namespace Rainbow.Helpers
{
	/// <summary>
	/// This class contains functions for mailing to 
	/// rainbow users
	/// </summary>
	public class MailHelper
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private MailHelper()
		{
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="Roles" type="string[]">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string[] value...
		/// </returns>
		public static string[] GetEmailAddressesInRoles(string[] roles, int portalID)
		{
			if (PortalSettings.UseSingleUserBase) portalID = 0;

			if (HttpContext.Current.User is WindowsPrincipal)
			{
				ArrayList addresses = new ArrayList();

				for (int i = 0; i < roles.Length; i++)
				{
					string account = roles[i];
					EmailAddressList eal = ADHelper.GetEmailAddresses(account);

					for (int j = 0; j < eal.Count; j++)

					{
						if (!addresses.Contains(eal[j]))
							addresses.Add(eal[j]);
					}
				}
				return (string[]) addresses.ToArray(typeof (string));
			}

			else
			{
				// No roles --> no email addresses
				if (roles.Length == 0)
					return new string[0];
				// Build the sql select
				string[] adaptedRoles = new string[roles.Length];

				for (int i = 0; i < roles.Length; i++)
				{
					adaptedRoles[i] = roles[i].Replace("'", "''");
				}
				string delimitedRoleList = "N'" + string.Join("', N'", adaptedRoles) + "'";
				string sql = "SELECT DISTINCT rb_Users.Email " +
					"FROM rb_UserRoles INNER JOIN " +
					" rb_Users ON rb_UserRoles.UserID = rb_Users.UserID INNER JOIN " +
					" rb_Roles ON rb_UserRoles.RoleID = rb_Roles.RoleID " +
					"WHERE (rb_Users.PortalID = " + portalID.ToString() + ") " +
					" AND (rb_Roles.RoleName IN (" + delimitedRoleList + "))";
				// Execute the sql
				EmailAddressList eal = new EmailAddressList();
				IDataReader myReader = DBHelper.GetDataReader(sql);

				try
				{
					while (myReader.Read())
					{
						if (!myReader.IsDBNull(0))

						{
							try
							{
								string email = myReader.GetString(0);

								if (email.Trim() != string.Empty)
									eal.Add(email);
							}

							catch
							{
							}
						}
					}
				}

				finally
				{
					myReader.Close();
				}
				// Return the result
				return (string[]) eal.ToArray(typeof (string));
			}
		}

		/// <summary>
		/// This function return's the email address of the current logged on user. 
		/// If its email-address is not valid or not found, 
		/// then an empty string is returned.
		/// </summary>
		/// <returns>string</returns>
		public static string GetCurrentUserEmailAddress()
		{
			return GetCurrentUserEmailAddress(string.Empty);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="Validated" type="bool">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public static string GetCurrentUserEmailAddress(bool validated)
		{
			return GetCurrentUserEmailAddress(string.Empty, validated);
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="DefaultEmail" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public static string GetCurrentUserEmailAddress(string defaultEmail)
		{
			return GetCurrentUserEmailAddress(defaultEmail, true);
		}

		/// <summary>
		/// This function return's the email address of the current logged on user. 
		/// If its email-address is not valid or not found, 
		/// then the Default address is returned
		/// </summary>
		/// <param name="DefaultEmail"></param>
		/// <param name="Validated"></param>
		/// <returns></returns>
		public static string GetCurrentUserEmailAddress(string defaultEmail, bool validated)
		{
			if (HttpContext.Current.User is WindowsPrincipal)
			{
				// windows user
				EmailAddressList eal = ADHelper.GetEmailAddresses(HttpContext.Current.User.Identity.Name);

				if (eal.Count == 0)
					return defaultEmail;

				else
					return (string) eal[0];
			}

			else
			{
				// Get the logged on email address from the context
				//string email = System.Web.HttpContext.Current.User.Identity.Name;
				string email = PortalSettings.CurrentUser.Identity.Email;

				if (!validated)
					return email;
				// Check if its email address is valid
				EmailAddressList eal = new EmailAddressList();

				try
				{
					eal.Add(email);
					return email;
				}

				catch
				{
					return defaultEmail;
				}
			}
		}

		// by Rob Siera 4 dec 2004
		// modified again by Bill Forney 4 dec 2004
		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		public static void SendMailNoAttachment(string from, string sendTo, string subject, string body, string cC, string bCC, string sMTPServer)
		{
			SendEMail(from, sendTo, subject, body, cC, bCC, sMTPServer);
		}

		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		/// <param name="AttachmentFile">Optional attachment file name</param>
		public static void SendMailOneAttachment(string from, string sendTo, string subject, string body, string attachmentFile, string cC, string bCC, string sMTPServer)
		{
			SendEMail(from, sendTo, subject, body, attachmentFile, cC, bCC, sMTPServer);
		}

		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		/// <param name="AttachmentFiles">Optional, list of attachment file names in form of an array list</param>
		public static void SendMailMultipleAttachments(string from, string sendTo, string subject, string body, ArrayList attachmentFiles, string cC, string bCC, string sMTPServer)
		{
			SendEMail(from, sendTo, subject, body, attachmentFiles, cC, bCC, sMTPServer);
		}

		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		public static void SendEMail(string from, string sendTo, string subject, string body, string cC, string bCC, string sMTPServer)
		{
			ArrayList AttachmentFiles = new ArrayList();
			AttachmentFiles = null;
			SendEMail(from, sendTo, subject, body, AttachmentFiles, cC, bCC, sMTPServer);
		}

		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		/// <param name="AttachmentFile">Optional attachment file name</param>
		public static void SendEMail(string from, string sendTo, string subject, string body, string attachmentFile, string cC, string bCC, string sMTPServer)
		{
			ArrayList AttachmentFiles = new ArrayList();

			if (attachmentFile != null && attachmentFile != string.Empty)
				AttachmentFiles.Add(attachmentFile);

			else
				AttachmentFiles = null;
			SendEMail(from, sendTo, subject, body, AttachmentFiles, cC, bCC, sMTPServer);
		}

		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		/// <param name="AttachmentFiles">Optional, list of attachment file names in form of an array list</param>
		public static void SendEMail(string from, string sendTo, string subject, string body, ArrayList attachmentFiles, string cC, string bCC, string sMTPServer)
		{
			SendEMail(from, sendTo, subject, body, attachmentFiles, cC, bCC, sMTPServer, MailFormat.Text);
		}

		/// <summary>
		/// Sends an email to specified address.
		/// </summary>
		/// <param name="From">Email address from</param>
		/// <param name="sendTo">Email address to</param>
		/// <param name="Subject">Email subject line</param>
		/// <param name="Body">Email body content</param>
		/// <param name="CC">Email carbon copy to</param>
		/// <param name="BCC">Email blind carbon copy to</param>
		/// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
		/// <param name="AttachmentFiles">Optional, list of attachment file names in form of an array list</param>
		/// <param name="mf">Optional, mail format (text/html)</param>
		public static void SendEMail(string from, string sendTo, string subject, string body, ArrayList attachmentFiles, string cC, string bCC, string sMTPServer, MailFormat mf)
		{
			MailMessage myMessage;

			try
			{
				myMessage = new MailMessage();
				myMessage.To = sendTo;
				myMessage.From = from;
				myMessage.Subject = subject;
				myMessage.Body = body;
				myMessage.BodyFormat = mf;

				if (cC != string.Empty) myMessage.Cc = cC;

				if (bCC != string.Empty) myMessage.Bcc = bCC;

				if (attachmentFiles != null)
				{
					foreach (string x in attachmentFiles)
					{
						if (File.Exists(x)) myMessage.Attachments.Add(x);
					}
				}

				if (sMTPServer != string.Empty) SmtpMail.SmtpServer = sMTPServer;
				SmtpMail.Send(myMessage);
			}

			catch (Exception myexp)
			{
				throw myexp;
			}
		}
	}
}