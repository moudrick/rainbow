using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

using Rainbow.Framework.Providers.RainbowMembershipProvider;
using System.Web.Security;
using System.Configuration;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Data.Linq;

namespace Rainbow.Framework.Providers.RainbowMembershipProvider
{

    /// <summary>
    /// SQL-specific implementation of <code>RainbowMembershipProvider</code> API
    /// </summary>
    public class RainbowSqlMembershipProvider : RainbowMembershipProvider
    {

        private const int _errorCode_UserNotFound = 1;
        private const int _errorCode_IncorrectPasswordAnswer = 3;
        private const int _errorCode_UserLockedOut = 99;

        private const int _newPasswordLength = 8;
        private const string _encryptionKey = "BE09F72BFF7A4566";
        private string eventSource = "RainbowSqlMembershipProvider";
        private string eventLog = "Application";

        /// <summary>
        /// 
        /// </summary>
        protected string connectionString;
        /// <summary>
        /// 
        /// </summary>
        protected string pApplicationName;
        /// <summary>
        /// 
        /// </summary>
        protected bool pEnablePasswordReset;
        /// <summary>
        /// 
        /// </summary>
        protected bool pEnablePasswordRetrieval;
        /// <summary>
        /// 
        /// </summary>
        protected bool pRequiresQuestionAndAnswer;
        /// <summary>
        /// 
        /// </summary>
        protected bool pRequiresUniqueEmail;
        /// <summary>
        /// 
        /// </summary>
        protected int pMaxInvalidPasswordAttempts;
        /// <summary>
        /// 
        /// </summary>
        protected int pPasswordAttemptWindow;
        /// <summary>
        /// 
        /// </summary>
        protected MembershipPasswordFormat pPasswordFormat;

        #region System.Web.Security.MembershipProvider overriden properties

        /// <summary>
        /// The name of the application using the membership provider.
        /// ApplicationName is used to scope membership data so that applications can choose whether to share membership data with other applications.
        /// This property can be read and written.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the application using the custom membership provider.</returns>
        public override string ApplicationName
        {
            get
            {
                return pApplicationName;
            }
            set
            {
                pApplicationName = value;
            }
        }

        /// <summary>
        /// Indicates whether passwords can be reset using the provider's ResetPassword method. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.</returns>
        public override bool EnablePasswordReset
        {
            get
            {
                return pEnablePasswordReset;
            }
        }

        /// <summary>
        /// Indicates whether passwords can be retrieved using the provider's GetPassword method. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.</returns>
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return pEnablePasswordRetrieval;
            }
        }

        /// <summary>
        /// Indicates whether a password answer must be supplied when calling the provider's GetPassword and ResetPassword methods. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.</returns>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return pRequiresQuestionAndAnswer;
            }
        }

        /// <summary>
        /// Indicates whether each registered user must have a unique e-mail address. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.</returns>
        public override bool RequiresUniqueEmail
        {
            get
            {
                return pRequiresUniqueEmail;
            }
        }

        /// <summary>
        /// Works in conjunction with PasswordAttemptWindow to provide a safeguard against password guessing.
        /// If the number of consecutive invalid passwords or password questions ("invalid attempts") submitted
        /// to the provider for a given user reaches MaxInvalidPasswordAttempts within the number of minutes specified
        /// by PasswordAttemptWindow, the user is locked out of the system. The user remains locked out until the
        /// provider's UnlockUser method is called to remove the lock.
        /// The count of consecutive invalid attempts is incremented when an invalid password or password answer is
        /// submitted to the provider's ValidateUser, ChangePassword, ChangePasswordQuestionAndAnswer, GetPassword, and ResetPassword methods.
        /// If a valid password or password answer is supplied before the MaxInvalidPasswordAttempts is reached,
        /// the count of consecutive invalid attempts is reset to zero.
        /// If the RequiresQuestionAndAnswer property is false, invalid password answer attempts are not tracked.
        /// This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>The number of invalid password or password-answer attempts allowed before the membership user is locked out.</returns>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return pMaxInvalidPasswordAttempts;
            }
        }

        /// <summary>
        /// For a description, see MaxInvalidPasswordAttempts. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <see cref="MaxInvalidPasswordAttempts"/>
        public override int PasswordAttemptWindow
        {
            get
            {
                return pPasswordAttemptWindow;
            }
        }

        /// <summary>
        /// Indicates what format that passwords are stored in: clear (plaintext), encrypted, or hashed.
        /// Clear and encrypted passwords can be retrieved; hashed passwords cannot. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.</returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return pPasswordFormat;
            }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        /// <summary>
        /// The minimum number of non-alphanumeric characters required in a password. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>The minimum number of special characters that must be present in a valid password.</returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return pMinRequiredNonAlphanumericCharacters;
            }
        }

        private int pMinRequiredPasswordLength;

        /// <summary>
        /// The minimum number of characters required in a password. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>The minimum length required for a password. </returns>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return pMinRequiredPasswordLength;
            }
        }

        private string pPasswordStrengthRegularExpression;

        /// <summary>
        /// A regular expression specifying a pattern to which passwords must conform. This property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>A regular expression used to evaluate a password.</returns>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return pPasswordStrengthRegularExpression;
            }
        }

        #endregion

        #region Properties

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog;

        /// <summary>
        /// Gets or sets a value indicating whether [write exceptions to event log].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [write exceptions to event log]; otherwise, <c>false</c>.
        /// </value>
        public bool WriteExceptionsToEventLog
        {
            get
            {
                return pWriteExceptionsToEventLog;
            }
            set
            {
                pWriteExceptionsToEventLog = value;
            }
        }

        #endregion

        #region System.Web.Security.MembershipProvider overriden methods

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {

            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "RainbowSqlMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Rainbow Sql Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["applicationName"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new RainbowMembershipProviderException("Password format not supported.");
            }

            // Initialize SqlConnection.
            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim().Equals(string.Empty))
            {
                throw new RainbowMembershipProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;

            if (EnablePasswordRetrieval && (PasswordFormat == MembershipPasswordFormat.Hashed))
            {
                throw new RainbowMembershipProviderException("Can't enable password retrieval when using hashed passwords");
            }
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return ChangePassword(ApplicationName, username, oldPassword, newPassword);
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return ChangePasswordQuestionAndAnswer(ApplicationName, username, password, newPasswordQuestion, newPasswordAnswer);
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return CreateUser(ApplicationName, username, password, email, passwordQuestion, passwordAnswer, isApproved, out status);
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        public override string GetPassword(string username, string answer)
        {
            return GetPassword(ApplicationName, username, answer);
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        public override string ResetPassword(string username, string answer)
        {
            return ResetPassword(ApplicationName, username, answer);
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            UpdateUser(ApplicationName, user);
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        public override bool ValidateUser(string username, string password)
        {
            return ValidateUser(ApplicationName, username, password);
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        public override bool UnlockUser(string userName)
        {
            return UnlockUser(ApplicationName, userName);
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetUserByUserId";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection( connectionString );

            //cmd.Parameters.Add( "@UserId", SqlDbType.UniqueIdentifier ).Value = providerUserKey;
            //cmd.Parameters.Add( "@CurrentTimeUtc", SqlDbType.DateTime ).Value = DateTime.Now;
            //if ( userIsOnline ) {
            //    cmd.Parameters.Add( "@UpdateLastActivity", SqlDbType.Bit ).Value = 1;
            //}
            //else {
            //    cmd.Parameters.Add( "@UpdateLastActivity", SqlDbType.Bit ).Value = 0;
            //}

            RainbowUser u = null;
            //SqlDataReader reader = null;

            //try {
            //    cmd.Connection.Open();

            //    using ( reader = cmd.ExecuteReader() ) {
            //        if ( reader.HasRows ) {
            //            reader.Read();

            //            string email = reader.IsDBNull( 0 ) ? string.Empty : reader.GetString( 0 );
            //            string passwordQuestion = reader.IsDBNull( 1 ) ? string.Empty : reader.GetString( 1 );
            //            string comment = reader.IsDBNull( 2 ) ? string.Empty : reader.GetString( 2 );
            //            bool isApproved = reader.IsDBNull( 3 ) ? false : reader.GetBoolean( 3 );
            //            DateTime creationDate = reader.IsDBNull( 4 ) ? DateTime.Now : reader.GetDateTime( 4 );
            //            DateTime lastLoginDate = reader.IsDBNull( 5 ) ? DateTime.Now : reader.GetDateTime( 5);
            //            DateTime lastActivityDate = reader.IsDBNull( 6 ) ? DateTime.Now : reader.GetDateTime( 6 );
            //            DateTime lastPasswordChangedDate = reader.IsDBNull( 7 ) ? DateTime.Now : reader.GetDateTime( 7 );
            //            string userName = reader.IsDBNull( 8 ) ? string.Empty : reader.GetString( 8 );
            //            bool isLockedOut = reader.IsDBNull( 9 ) ? false : reader.GetBoolean( 9 );
            //            DateTime lastLockedOutDate = reader.IsDBNull( 10 ) ? DateTime.Now : reader.GetDateTime( 10 );

            DataClassesDataContext db = new DataClassesDataContext();
            ISingleResult<User> results = db.aspnet_Membership_GetUserByUserId(
                                                                             (Guid?)providerUserKey, (DateTime?)DateTime.Now,
                                                                             (bool?)userIsOnline);
            foreach (User row in results)
            {
                u = InstanciateNewUser(this.Name, row.UserName, row.UserId, row.Email, row.PasswordQuestion, row.Comment,
                    row.IsApproved, row.IsLockedOut, row.CreationDate, row.LastLoginDate, row.LastActivityDate,
                    row.LastPasswordChangedDate, row.LastLockedOutDate);
            }

            LoadUserProfile(u);

            //        }
            //    }
            //}
            //catch ( SqlException e ) {
            //    if ( WriteExceptionsToEventLog ) {
            //        WriteToEventLog( e, "GetUser(object, Boolean)" );
            //    }
            //    throw new RainbowMembershipProviderException( "Error executing aspnet_Membership_GetUserByUserId stored proc", e );
            //}
            //finally {
            //    if ( reader != null ) {
            //        reader.Close();
            //    }

            //    cmd.Connection.Close();
            //}

            return u;
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return GetUser(ApplicationName, username, userIsOnline);
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            return GetUserNameByEmail(ApplicationName, email);
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return DeleteUser(ApplicationName, username, deleteAllRelatedData);
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return FindUsersByEmail(ApplicationName, emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return FindUsersByName(ApplicationName, usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return GetAllUsers(ApplicationName, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            return GetNumberOfUsersOnline(ApplicationName);
        }

        #endregion

        #region Rainbow-specific Provider methods

        /// <summary>
        /// Takes, as input, a user name, a password (the user's current password), and a new password and updates
        /// the password in the membership data source.
        /// Before changing a password, ChangePassword calls the provider's virtual OnValidatingPassword method to
        /// validate the new password. It then changes the password or cancels the action based on the outcome of the call.
        /// If the user name, password, new password, or password answer is not valid, ChangePassword
        /// does not throw an exception; it simply returns false.
        /// Following a successful password change, ChangePassword updates the user's LastPasswordChangedDate.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="oldPassword">The user's old password</param>
        /// <param name="newPassword">The user's new password</param>
        /// <returns>
        /// ChangePassword returns true if the password was updated successfully.
        /// Otherwise, it returns false.
        /// </returns>
        public override bool ChangePassword(string portalAlias, string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
                }
            }

            string passwordSalt = string.Empty;
            string encodedPassword;
            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                encodedPassword = EncodePassword(passwordSalt + newPassword);
            }
            else
            {
                encodedPassword = EncodePassword(newPassword);
            }

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_SetPassword";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            //cmd.Parameters.Add("@NewPassword", SqlDbType.NVarChar, 255).Value = encodedPassword;
            //cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 255).Value = passwordSalt;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;
            //cmd.Parameters.Add("@PasswordFormat", SqlDbType.Int).Value = PasswordFormat;

            //SqlParameter returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCode.Direction = ParameterDirection.ReturnValue;

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();

            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_SetPassword(portalAlias, username, encodedPassword, passwordSalt, (DateTime?)DateTime.Now,
                (int?)PasswordFormat);

            return returnCode == 0;

            //    return ((int)returnCode.Value) == 0;
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "ChangePassword");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_SetPassword stored proc", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, a user name, password, password question, and password answer and updates the password question and answer
        /// in the data source if the user name and password are valid.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="password">The user's password</param>
        /// <param name="newPasswordQuestion">The user's new password question</param>
        /// <param name="newPasswordAnswer">The user's new password answer</param>
        /// <returns>
        /// This method returns true if the password question and answer
        /// are successfully updated. It returns false if either the user name or password is invalid.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(string portalAlias, string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_ChangePasswordQuestionAndAnswer";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            //cmd.Parameters.Add("@NewPasswordQuestion", SqlDbType.NVarChar, 255).Value = newPasswordQuestion;
            //cmd.Parameters.Add("@NewPasswordAnswer", SqlDbType.NVarChar, 255).Value = newPasswordAnswer;

            //SqlParameter returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCode.Direction = ParameterDirection.ReturnValue;

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();

            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_ChangePasswordQuestionAndAnswer(portalAlias, username, newPasswordQuestion, newPasswordAnswer);

            return returnCode == 0;

            //    return ((int)returnCode.Value) == 0;
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
            //    }
            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_ChangePasswordQuestionAndAnswer stored proc", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, a user name, password, e-mail address, and other information and adds a new user to the membership data source.
        /// CreateUser returns a MembershipUser object representing the newly created user.
        /// Before creating a new user, CreateUser calls the provider's virtual OnValidatingPassword method to validate the supplied password.
        /// It then creates the user or cancels the action based on the outcome of the call.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">New user's name</param>
        /// <param name="password">New user's password</param>
        /// <param name="email">New user's email</param>
        /// <param name="passwordQuestion">The password question</param>
        /// <param name="passwordAnswer">The password answer</param>
        /// <param name="isApproved">Whether the user is approved or not</param>
        /// <param name="status">An out parameter (in Visual Basic, ByRef) that returns a MembershipCreateStatus value indicating whether the user was
        /// successfully created or, if the user was not created, the reason why.</param>
        /// <returns>
        /// A new <code>MembershipUser</code>. If the user was not created, CreateUser returns null.
        /// </returns>
        public override MembershipUser CreateUser(string portalAlias, string username, string password, string email,
            string passwordQuestion, string passwordAnswer, bool isApproved, out MembershipCreateStatus status)
        {

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string passwordSalt = string.Empty;
            string encodedPassword;
            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                encodedPassword = EncodePassword(passwordSalt + password);
            }
            else
            {
                encodedPassword = EncodePassword(password);
            }

            Guid? userId = null;
            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_CreateUser(portalAlias, username, encodedPassword, passwordSalt, email, passwordQuestion,
                passwordAnswer == null ? null : passwordAnswer, (bool?)isApproved, (DateTime?)DateTime.Now, (DateTime?)DateTime.Now,
                (int?)Convert.ToInt32(RequiresUniqueEmail), (int?)PasswordFormat, ref userId);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_CreateUser";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            //cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = encodedPassword;
            //cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 255).Value = passwordSalt;
            //cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = email;
            //cmd.Parameters.Add("@PasswordQuestion", SqlDbType.NVarChar, 255).Value = passwordQuestion;
            //cmd.Parameters.Add("@PasswordAnswer", SqlDbType.NVarChar, 255).Value = passwordAnswer == null ? null : passwordAnswer;
            //cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
            //cmd.Parameters.Add("@UniqueEmail", SqlDbType.Int).Value = RequiresUniqueEmail;
            //cmd.Parameters.Add("@PasswordFormat", SqlDbType.Int).Value = PasswordFormat;
            //cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
            //cmd.Parameters.Add("@CurrentTimeUTC", SqlDbType.DateTime).Value = DateTime.Now;

            //SqlParameter newUserIdParam = cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier);
            //newUserIdParam.Direction = ParameterDirection.Output;

            //SqlParameter returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCode.Direction = ParameterDirection.ReturnValue;

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();

            //    status = (MembershipCreateStatus)Enum.Parse(typeof(MembershipCreateStatus), returnCode.Value.ToString());

            status = (MembershipCreateStatus)Enum.Parse(typeof(MembershipCreateStatus), returnCode.ToString());

            //    if (((int)returnCode.Value) == 0)

            if (returnCode == 0)
            {
                // everything went OK

                //        RainbowUser user = (RainbowUser)this.GetUser(newUserIdParam.Value, false);

                RainbowUser user = (RainbowUser)this.GetUser(userId, false);

                this.SaveUserProfile(user);
                return user;
            }
            else
            {
                return null;
            }
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "CreateUser");
            //    }

            //    status = MembershipCreateStatus.ProviderError;
            //    return null;
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, a user name and deletes that user from the membership data source.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="deleteAllRelatedData">Specifies whether
        /// related data for that user should be deleted also. If deleteAllRelatedData is true, DeleteUser
        /// should delete role data, profile data, and all other data associated with that user.</param>
        /// <returns>
        /// DeleteUser returns true if the user was successfully deleted. Otherwise, it returns false.
        /// </returns>
        public override bool DeleteUser(string portalAlias, string username, bool deleteAllRelatedData)
        {
            bool profileDeleted = this.DeleteUserProfile(username);

            int? tablesDeletedFrom = null;
            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Users_DeleteUser(portalAlias, username, (deleteAllRelatedData ? 0xF : 1), ref tablesDeletedFrom);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Users_DeleteUser";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            //if (deleteAllRelatedData)
            //{
            //    cmd.Parameters.Add("@TablesToDeleteFrom", SqlDbType.Int).Value = 0xF;
            //}
            //else
            //{
            //    cmd.Parameters.Add("@TablesToDeleteFrom", SqlDbType.Int).Value = 1;
            //}

            //SqlParameter tablesDeletedFrom = cmd.Parameters.Add("@NumTablesDeletedFrom", SqlDbType.Int);
            //tablesDeletedFrom.Direction = ParameterDirection.Output;

            //SqlParameter returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCode.Direction = ParameterDirection.ReturnValue;

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();

            return (tablesDeletedFrom.GetValueOrDefault() > 0) && (returnCode == 0);

            //    return (((int)tablesDeletedFrom.Value) > 0) && (((int)returnCode.Value) == 0);
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "DeleteUser");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Users_DeleteUser stored proc", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// The results returned by GetAllUsers are constrained by the pageIndex and pageSize input parameters.
        /// pageSize specifies the maximum number of MembershipUser objects to return. pageIndex
        /// identifies which page of results to return. Page indexes are 0-based.
        /// GetAllUsers also takes an out parameter (in Visual Basic, ByRef) named totalRecords that, on return, holds a count of all registered users.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <returns>
        /// Returns a MembershipUserCollection containing MembershipUser objects representing all registered users.
        /// If there are no registered users, GetAllUsers returns an empty MembershipUserCollection.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(string portalAlias)
        {
            int records;
            return GetAllUsers(portalAlias, 0, int.MaxValue, out records);
        }

        /// <summary>
        /// The results returned by GetAllUsers are constrained by the pageIndex and pageSize input parameters.
        /// pageSize specifies the maximum number of MembershipUser objects to return. pageIndex
        /// identifies which page of results to return. Page indexes are 0-based.
        /// GetAllUsers also takes an out parameter (in Visual Basic, ByRef) named totalRecords that, on return, holds a count of all registered users.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="pageIndex">Page index to retrieve</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalRecords">Holds a count of all records.</param>
        /// <returns>
        /// Returns a MembershipUserCollection containing MembershipUser objects representing all registered users.
        /// If there are no registered users, GetAllUsers returns an empty MembershipUserCollection.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(string portalAlias, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            List<User> dbusers = null;

            DataClassesDataContext db = new DataClassesDataContext();
            totalRecords = db.aspnet_Membership_GetAllUsers(portalAlias, pageIndex, pageSize, out dbusers);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetAllUsers";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            //cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

            //SqlParameter totalRecordsParam = cmd.Parameters.Add("@TotalRecords", SqlDbType.Int);
            //totalRecordsParam.Direction = ParameterDirection.ReturnValue;

            //SqlDataReader reader = null;
            //try
            //{
            //    cmd.Connection.Open();

            //    using (reader = cmd.ExecuteReader())
            //    {

            //        while (reader.Read())
            //        {
            foreach (User u in dbusers)
            {
                //            RainbowUser u = GetUserFromReader(reader);
                RainbowUser ru = InstanciateNewUser(this.Name, u.UserName, u.UserId, u.Email, u.PasswordQuestion, u.Comment, u.IsApproved,
                    u.IsLockedOut, u.CreationDate, u.LastLoginDate, u.LastActivityDate, u.LastPasswordChangedDate, u.LastLockedOutDate);
                LoadUserProfile(ru);
                users.Add(ru);
            }
            //    }

            //    reader.Close();
            //    totalRecords = (int)totalRecordsParam.Value;
            //}
            return users;
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "GetAllUsers");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_GetAllUsers stored proc", e);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Returns a count of users that are currently online; that is, whose LastActivityDate is
        /// greater than the current date and time minus the value of the membership service's
        /// UserIsOnlineTimeWindow property, which can be read from Membership.UserIsOnlineTimeWindow.
        /// UserIsOnlineTimeWindow specifies a time in minutes and is set using the <code>&lt;membership&gt;</code>
        /// element's userIsOnlineTimeWindow attribute.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <returns>
        /// Returns a count of users that are currently online
        /// </returns>
        public override int GetNumberOfUsersOnline(string portalAlias)
        {

            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            DataClassesDataContext db = new DataClassesDataContext();
            int numOnline = db.aspnet_Membership_GetNumberOfUsersOnline(portalAlias, Membership.UserIsOnlineTimeWindow, DateTime.Now);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetNumberOfUsersOnline";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@MinutesSinceLastInActive", SqlDbType.Int).Value = Membership.UserIsOnlineTimeWindow;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;

            //int numOnline = 0;

            //try
            //{
            //    cmd.Connection.Open();

            //    numOnline = Convert.ToInt32(cmd.ExecuteScalar());
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "GetNumberOfUsersOnline");
            //    }
            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_GetNumberOfUsersOnline stored proc", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}

            return numOnline;
        }

        /// <summary>
        /// Takes, as input, a user name and a password answer and returns that user's password.
        /// Before retrieving a password, GetPassword verifies that EnablePasswordRetrieval is true.
        /// GetPassword also checks the value of the RequiresQuestionAndAnswer property before retrieving a password.
        /// If RequiresQuestionAndAnswer is true, GetPassword compares the supplied password answer to the stored password answer
        /// and throws a MembershipPasswordException if the two don't match.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="answer">The password answer</param>
        /// <returns>Returns the user's password</returns>
        /// <exception cref="System.Configuration.Provider.ProviderException">If the user name is not valid, GetPassword throws a ProviderException.</exception>
        /// <exception cref="NotSupportedException">If EnablePasswordRetrieval is false, GetPassword throws a NotSupportedException.</exception>
        /// <exception cref="System.Configuration.Provider.ProviderException">If EnablePasswordRetrieval  is true but the password format is hashed, GetPassword throws a
        /// ProviderException since hashed passwords cannot, by definition, be retrieved.</exception>
        /// <exception cref="MembershipPasswordException">GetPassword also throws a MembershipPasswordException
        /// if the user whose password is being retrieved is currently locked out.</exception>
        /// <exception cref="MembershipPasswordException">If RequiresQuestionAndAnswer is true, GetPassword compares the supplied password answer to the stored password answer
        /// and throws a MembershipPasswordException if the two don't match. </exception>
        public override string GetPassword(string portalAlias, string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new RainbowMembershipProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new RainbowMembershipProviderException("Cannot retrieve Hashed passwords.");
            }

            DataClassesDataContext db = new DataClassesDataContext();
            ISingleResult<aspnet_Membership_GetPasswordResult> result = db.aspnet_Membership_GetPassword(
                portalAlias, username, MaxInvalidPasswordAttempts, PasswordAttemptWindow, DateTime.Now, answer);

            int returnCode = (int)result.ReturnValue;

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetPassword";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = username;
            //cmd.Parameters.Add("@MaxInvalidPasswordAttempts", SqlDbType.Int).Value = MaxInvalidPasswordAttempts;
            //cmd.Parameters.Add("@PasswordAttemptWindow", SqlDbType.Int).Value = PasswordAttemptWindow;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;
            //cmd.Parameters.Add("@PasswordAnswer", SqlDbType.NVarChar, 128).Value = answer;

            //SqlParameter returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCodeParam.Direction = ParameterDirection.ReturnValue;

            string password = string.Empty;
            string passwordAnswer = string.Empty;
            MembershipPasswordFormat passwordFormat;
            //SqlDataReader reader = null;

            //try
            //{
            //    cmd.Connection.Open();

            //    using (reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            //    {
            //if (reader.HasRows)
            //{
            //    reader.Read();

            foreach (aspnet_Membership_GetPasswordResult r in result)
            {
                password = r.Column1;
                passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), r.Column2.ToString());

                //password = reader.GetString(0);
                //passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), reader.GetInt32(1).ToString());
            }

            //}
            //            reader.Close();

            //            int returnCode = (int)returnCodeParam.Value;
            switch (returnCode)
            {
                case _errorCode_UserNotFound:
                    throw new RainbowMembershipProviderException("The supplied user name was not found.");
                case _errorCode_IncorrectPasswordAnswer:
                    throw new MembershipPasswordException("Incorrect password answer.");
                case _errorCode_UserLockedOut:
                    throw new MembershipPasswordException("User is currently locked out");
                case -1:
                    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_GetPassword stored proc");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }
            return password;
            //    }
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "GetPassword");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_GetPassword stored proc", e);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, a user name or user ID (the method is overloaded) and a Boolean value indicating whether to
        /// update the user's LastActivityDate to show that the user is currently online.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="userIsOnline"></param>
        /// <returns>
        /// GetUser returns a
        /// MembershipUser object representing the specified user. If the user name or user ID is invalid (that is,
        /// if it doesn't represent a registered user) GetUser returns null (Nothing in Visual Basic).
        /// </returns>
        public override MembershipUser GetUser(string portalAlias, string username, bool userIsOnline)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            ISingleResult<User> result = db.aspnet_Membership_GetUserByName(
                portalAlias, username, DateTime.Now, userIsOnline);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetUserByName";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = username;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;
            //if (userIsOnline)
            //{
            //    cmd.Parameters.Add("@UpdateLastActivity", SqlDbType.Bit).Value = 1;
            //}
            //else
            //{
            //    cmd.Parameters.Add("@UpdateLastActivity", SqlDbType.Bit).Value = 0;
            //}

            RainbowUser u = null;
            //SqlDataReader reader = null;

            //try
            //{
            //    cmd.Connection.Open();

            //    using (reader = cmd.ExecuteReader())
            //    {
            //        if (reader.HasRows)
            //        {
            //            reader.Read();
            //string email = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            //string passwordQuestion = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            //string comment = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            //bool isApproved = reader.IsDBNull(3) ? false : reader.GetBoolean(3);
            //DateTime creationDate = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4);
            //DateTime lastLoginDate = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5);
            //DateTime lastActivityDate = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6);
            //DateTime lastPasswordChangedDate = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7);

            //Guid providerUserKey = new Guid(reader.GetValue(8).ToString());
            //bool isLockedOut = reader.IsDBNull(9) ? false : reader.GetBoolean(9);
            //DateTime lastLockedOutDate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10);

            //u = InstanciateNewUser(this.Name, username, providerUserKey, email, passwordQuestion, comment, isApproved,
            //     isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockedOutDate);

            foreach (User row in result)
            {
                u = InstanciateNewUser(this.Name, username, (Guid)row.UserId, row.Email, row.PasswordQuestion, row.Comment, row.IsApproved,
                    row.IsLockedOut, row.CreationDate, row.LastLoginDate, row.LastActivityDate, row.LastPasswordChangedDate, row.LastLockedOutDate);
            }

            LoadUserProfile(u);
            //        }
            //    }
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "GetUser(String, Boolean)");
            //    }
            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_GetUserByName stored proc", e);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }

            //    cmd.Connection.Close();
            //}

            return u;
        }

        /// <summary>
        /// Unlocks (that is, restores login privileges for) the specified user.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <returns>
        /// UnlockUser returns true if the user is successfully
        /// unlocked. Otherwise, it returns false. If the user is already unlocked, UnlockUser simply returns true.
        /// </returns>
        public override bool UnlockUser(string portalAlias, string username)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_UnlockUser(portalAlias, username);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_UnlockUser";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = username;

            //SqlParameter returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCodeParam.Direction = ParameterDirection.ReturnValue;

            //int rowsAffected = 0;

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();

            //    int returnCode = (int)returnCodeParam.Value;
            return (returnCode == 0);
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "UnlockUser");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_UnlockUser stored proc", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, an e-mail address and returns the first registered user name whose e-mail address matches the one supplied.
        /// If it doesn't find a user with a matching e-mail address, GetUserNameByEmail returns an empty string.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="email"></param>
        /// <returns>
        /// The first registered user name whose e-mail address matches the one supplied.
        /// If it doesn't find a user with a matching e-mail address, GetUserNameByEmail returns an empty string.
        /// </returns>
        public override string GetUserNameByEmail(string portalAlias, string email)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            ISingleResult<aspnet_Membership_GetUserByEmailResult> result = db.aspnet_Membership_GetUserByEmail(portalAlias, email);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetUserByEmail";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;

            string username = string.Empty;

            foreach (aspnet_Membership_GetUserByEmailResult row in result)
            {
                username = row.UserName;
            }

            //try
            //{
            //    cmd.Connection.Open();

            //    username = (string)cmd.ExecuteScalar();
            //if (username == null)
            //{
            //    username = string.Empty;
            //}
            return username;
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "GetUserNameByEmail");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_GetUserByEmail stored proc", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, a MembershipUser object representing a registered user and updates the information stored
        /// for that user in the membership data source.
        /// Note that UpdateUser is not obligated to allow all the data that can be encapsulated in a
        /// MembershipUser object to be updated in the data source.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="user">A MembershipUser object representing a registered user</param>
        /// <exception cref="System.Configuration.Provider.ProviderException">If any of the input submitted in the MembershipUser object
        /// is not valid, UpdateUser throws a ProviderException.</exception>
        public override void UpdateUser(string portalAlias, MembershipUser user)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            int totalRecords = db.aspnet_Membership_UpdateUser(portalAlias, user.UserName, user.Email, user.Comment, user.IsApproved,
                user.LastLoginDate, user.LastActivityDate, (int?)Convert.ToInt32(RequiresUniqueEmail), DateTime.Now);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_UpdateUser";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = user.UserName;
            //cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = user.Email;
            //cmd.Parameters.Add("@Comment", SqlDbType.NText).Value = user.Comment;
            //cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = user.IsApproved;
            //cmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = user.LastLoginDate;
            //cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = user.LastActivityDate;
            //cmd.Parameters.Add("@UniqueEmail", SqlDbType.Bit).Value = RequiresUniqueEmail;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;

            //SqlParameter totalRecordsParam = cmd.Parameters.Add("@TotalRecords", SqlDbType.Int);
            //totalRecordsParam.Direction = ParameterDirection.ReturnValue;

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();

            SaveUserProfile((RainbowUser)user);
            //if (((int)totalRecordsParam.Value) != 0)
            if (totalRecords != 0)
            {
                throw new RainbowMembershipProviderException("Error updating user");
            }
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "UpdateUser");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_UpdateUser stored proc", e);
            //}
            //catch (Exception e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "UpdateUser");
            //    }

            //    throw new RainbowMembershipProviderException("Error updating user", e);
            //}
            //finally
            //{
            //    cmd.Connection.Close();
            //}
        }

        /// <summary>
        /// Takes, as input, a user name and a password and verifies that they are valid-that is, that the membership
        /// data source contains a matching user name and password. ValidateUser returns true if the user name and
        /// password are valid, if the user is approved (that is, if MembershipUser.IsApproved is true), and if the
        /// user isn't currently locked out. Otherwise, it returns false.
        /// Following a successful validation, ValidateUser updates the user's LastLoginDate and fires an
        /// AuditMembershipAuthenticationSuccess Web event. Following a failed validation, it fires an
        /// AuditMembershipAuthenticationFailure Web event.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="password">The user's password</param>
        /// <returns></returns>
        public override bool ValidateUser(string portalAlias, string username, string password)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            ISingleResult<aspnet_Membership_GetPasswordWithFormatResult> result = db.aspnet_Membership_GetPasswordWithFormat(
                portalAlias, username, true, DateTime.Now);

            //SqlConnection conn = new SqlConnection(connectionString);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_GetPasswordWithFormat";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = conn;

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = username;
            //cmd.Parameters.Add("@UpdateLastLoginActivityDate", SqlDbType.Int).Value = 1;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;

            //SqlParameter returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCode.Direction = ParameterDirection.ReturnValue;

            //SqlDataReader reader = null;
            string dbPassword = string.Empty;
            string dbPasswordSalt = string.Empty;
            MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Clear;

            //try
            //{
            //    cmd.Connection.Open();

            //    using (reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            //    {
            //        if (reader.HasRows)
            //        {
            //            reader.Read();

            foreach (aspnet_Membership_GetPasswordWithFormatResult row in result)
            {
                dbPassword = row.Column1;
                passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), row.Column2.Value.ToString());
                dbPasswordSalt = row.Column3;

                //    dbPassword = reader.GetString(0);
                //    passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), reader.GetInt32(1).ToString());
                //    dbPasswordSalt = reader.GetString(2);
                //}
            }
            //reader.Close();
            //if (((int)returnCode.Value) > 0)
            if ((int)result.ReturnValue > 0)
            {
                return false;
            }
            //    }

            return CheckPassword(password, dbPassword, dbPasswordSalt, passwordFormat);
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "ValidateUser");
            //    }
            //    throw new RainbowMembershipProviderException("Error validating user", e);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }
            //    conn.Close();
            //}
        }

        /// <summary>
        /// Returns a MembershipUserCollection containing MembershipUser objects representing users whose user names match
        /// the usernameToMatch input parameter. Wildcard syntax is data source-dependent. MembershipUser objects in the
        /// MembershipUserCollection are sorted by user name.
        /// For an explanation of the pageIndex, pageSize, and totalRecords parameters, see the GetAllUsers method.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex">Page index to retrieve</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalRecords">Holds a count of all records.</param>
        /// <returns>
        /// A <code>MembershipUserCollection</code>. If FindUsersByName finds no matching users, it returns an
        /// empty MembershipUserCollection.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string portalAlias, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            List<User> userList = null;

            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_FindUsersByName(portalAlias, usernameToMatch, pageIndex, pageSize, userList);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_FindUsersByName";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserNameToMatch", SqlDbType.NVarChar, 256).Value = usernameToMatch;
            //cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            //cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

            //SqlParameter returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCode.Direction = ParameterDirection.ReturnValue;

            MembershipUserCollection users = new MembershipUserCollection();

            //SqlDataReader reader = null;

            //try
            //{
            //    cmd.Connection.Open();

            //    using (reader = cmd.ExecuteReader())
            //    {

            //        while (reader.Read())
            //        {

            foreach (User user in userList)
            {
                RainbowUser u = InstanciateNewUser(this.Name, user.UserName, user.UserId, user.Email, user.PasswordQuestion, user.Comment,
                    user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate,
                    user.LastPasswordChangedDate, user.LastLockedOutDate);
                //RainbowUser u = GetUserFromReader(reader);
                LoadUserProfile(u);
                users.Add(u);
            }

            //}

            //reader.Close();
            //totalRecords = (int)returnCode.Value;
            totalRecords = returnCode;
            //    }
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "FindUsersByName");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_FindUsersByName stored proc", e);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }

            //    cmd.Connection.Close();
            //}

            return users;
        }

        /// <summary>
        /// Returns a MembershipUserCollection containing MembershipUser objects representing users whose e-mail
        /// addresses match the emailToMatch input parameter. Wildcard syntax is data source-dependent. MembershipUser
        /// objects in the MembershipUserCollection are sorted by e-mail address.
        /// For an explanation of the pageIndex, pageSize, and totalRecords parameters, see the GetAllUsers method.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex">Page index to retrieve</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalRecords">Holds a count of all records.</param>
        /// <returns>
        /// A <code>MembershipUserCollection</code>. If FindUsersByEmail finds no
        /// matching users, it returns an empty MembershipUserCollection.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(string portalAlias, string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            List<User> userList = null;

            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_FindUsersByEmail(portalAlias, emailToMatch, pageIndex, pageSize, userList);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_FindUsersByEmail";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = new SqlConnection(connectionString);

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@EmailToMatch", SqlDbType.NVarChar, 256).Value = emailToMatch;
            //cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            //cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

            //SqlParameter returnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
            //returnValue.Direction = ParameterDirection.ReturnValue;

            MembershipUserCollection users = new MembershipUserCollection();

            //SqlDataReader reader = null;

            //try
            //{
            //    cmd.Connection.Open();

            //    using (reader = cmd.ExecuteReader())
            //    {

            //        while (reader.Read())
            //        {

            foreach (User u in userList)
            {
                RainbowUser ru = InstanciateNewUser(this.Name, u.UserName, u.UserId, u.Email, u.PasswordQuestion, u.Comment,
                    u.IsApproved, u.IsLockedOut, u.CreationDate, u.LastLoginDate, u.LastActivityDate, u.LastPasswordChangedDate,
                    u.LastLockedOutDate);

                //RainbowUser ru = GetUserFromReader(reader);

                LoadUserProfile(ru);
                users.Add(ru);
            }

            //}

            //            reader.Close();
            //            totalRecords = (int)returnValue.Value;

            totalRecords = returnCode;

            //        }
            //    }
            //    catch (SqlException e)
            //    {
            //        if (WriteExceptionsToEventLog)
            //        {
            //            WriteToEventLog(e, "FindUsersByEmail");

            //            throw new RainbowMembershipProviderException("Error executing aspnet_Membership_FindUsersByEmail stored proc", e);
            //        }
            //        else
            //        {
            //            throw e;
            //        }
            //    }
            //    finally
            //    {
            //        if (reader != null)
            //        {
            //            reader.Close();
            //        }

            //        cmd.Connection.Close();
            //    }

            return users;
        }

        /// <summary>
        /// Takes, as input, a user name and a password answer and replaces the user's current password with a new,
        /// random password.  A convenient mechanism for generating a random password is the Membership.GeneratePassword method.
        /// ResetPassword also checks the value of the RequiresQuestionAndAnswer property before resetting a password.
        /// Before resetting a password, ResetPassword verifies that EnablePasswordReset is true.
        /// Before resetting a password, ResetPassword calls the provider's virtual OnValidatingPassword method to
        /// validate the new password. It then resets the password or cancels the action based on the outcome of
        /// the call.
        /// Following a successful password reset, ResetPassword updates the user's LastPasswordChangedDate.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="username">The user's name</param>
        /// <param name="answer">The password answer</param>
        /// <returns>
        /// ResetPassword then returns the new password.
        /// </returns>
        /// <exception cref="NotSupportedException">If EnablePasswordReset is false, ResetPassword throws a NotSupportedException. </exception>
        /// <exception cref="System.Configuration.Provider.ProviderException">If the user name is not valid, ResetPassword throws a ProviderException.</exception>
        /// <exception cref="System.Configuration.Provider.ProviderException">If the new password is invalid, ResetPassword throws a ProviderException.</exception>
        /// <exception cref="MembershipPasswordException">If the user whose password is being changed is currently locked out, ResetPassword throws a MembershipPasswordException.</exception>
        /// <exception cref="MembershipPasswordException">If RequiresQuestionAndAnswer is true, ResetPassword compares the supplied password
        /// answer to the stored password answer and throws a MembershipPasswordException if the two don't match.</exception>
        public override string ResetPassword(string portalAlias, string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null)
            {
                answer = string.Empty;
            }

            string newPassword = Membership.GeneratePassword(_newPasswordLength, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, false);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new RainbowMembershipProviderException("Reset password canceled due to password validation failure.");
                }
            }

            string passwordSalt = string.Empty;
            string encodedPassword;
            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                encodedPassword = EncodePassword(passwordSalt + newPassword);
            }
            else
            {
                encodedPassword = EncodePassword(newPassword);
            }

            DataClassesDataContext db = new DataClassesDataContext();
            int returnCode = db.aspnet_Membership_ResetPassword(portalAlias, username, encodedPassword, MaxInvalidPasswordAttempts, PasswordAttemptWindow,
                passwordSalt, DateTime.Now, (int?)Convert.ToInt32(PasswordFormat), answer);

            //SqlConnection conn = new SqlConnection(connectionString);

            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "aspnet_Membership_ResetPassword";
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Connection = conn;

            //cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            //cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = username;
            //cmd.Parameters.Add("@NewPassword", SqlDbType.NVarChar, 128).Value = encodedPassword;
            //cmd.Parameters.Add("@MaxInvalidPasswordAttempts", SqlDbType.Int).Value = MaxInvalidPasswordAttempts;
            //cmd.Parameters.Add("@PasswordAttemptWindow", SqlDbType.Int).Value = PasswordAttemptWindow;
            //cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 128).Value = passwordSalt;
            //cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.Now;
            //cmd.Parameters.Add("@PasswordFormat", SqlDbType.Int).Value = PasswordFormat;
            //cmd.Parameters.Add("@PasswordAnswer", SqlDbType.NVarChar, 128).Value = answer;

            //SqlParameter returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            //returnCodeParam.Direction = ParameterDirection.ReturnValue;

            //try
            //{
            //    conn.Open();

            //    cmd.ExecuteNonQuery();

            //int returnCode = (int)returnCodeParam.Value;

            switch (returnCode)
            {
                case _errorCode_UserNotFound:
                    throw new RainbowMembershipProviderException("The supplied user name is not found.");
                case _errorCode_IncorrectPasswordAnswer:
                    throw new MembershipPasswordException("The supplied password answer is incorrect.");
                case _errorCode_UserLockedOut:
                    throw new RainbowMembershipProviderException("The supplied user is locked out.");
                case -1:
                    throw new RainbowMembershipProviderException("Error resetting password");
            }

            return newPassword;
            //}
            //catch (SqlException e)
            //{
            //    if (WriteExceptionsToEventLog)
            //    {
            //        WriteToEventLog(e, "ResetPassword");
            //    }

            //    throw new RainbowMembershipProviderException("Error executing aspnet_Membership_ResetPassword stored proc", e);
            //}
            //finally
            //{
            //    conn.Close();
            //}
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// A helper function to retrieve config values from the configuration file.
        /// </summary>
        /// <param name="configValue">The config value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        /// <summary>
        /// Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        /// </summary>
        /// <param name="password">the password</param>
        /// <returns></returns>
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    System.Security.Cryptography.HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte(_encryptionKey);
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new RainbowMembershipProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        /// <summary>
        /// Decrypts or leaves the password clear based on the PasswordFormat.
        /// </summary>
        /// <param name="encodedPassword">The encoded password.</param>
        /// <returns></returns>
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new RainbowMembershipProviderException("Cannot unencode a hashed password.");
                default:
                    throw new RainbowMembershipProviderException("Unsupported password format.");
            }

            return password;
        }

        /// <summary>
        /// A helper function that writes exception detail to the event log. Exceptions are written to the event log as a security
        /// measure to avoid private database details from being returned to the browser. If a method does not return a status
        /// or boolean indicating the action succeeded or failed, a generic exception is also thrown by the caller.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="action">The action.</param>
        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            log.WriteEntry(message);
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.
        /// </summary>
        /// <param name="hexString">The hex string.</param>
        /// <returns></returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// A helper function that takes the current row from the SqlDataReader and hydrates a MembershiUser from the values.
        /// Called by the MembershipUser.GetUser implementation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected virtual RainbowUser GetUserFromReader(SqlDataReader reader)
        {
            string username = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            string email = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            string passwordQuestion = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            string comment = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            bool isApproved = reader.IsDBNull(4) ? false : reader.GetBoolean(4);
            DateTime creationDate = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5);
            DateTime lastLoginDate = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6);
            DateTime lastActivityDate = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7);
            DateTime lastPasswordChangedDate = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8);

            Guid providerUserKey = reader.GetGuid(9);
            bool isLockedOut = reader.IsDBNull(10) ? false : reader.GetBoolean(10);
            DateTime lastLockedOutDate = reader.IsDBNull(11) ? DateTime.Now : reader.GetDateTime(11);

            RainbowUser u = InstanciateNewUser(this.Name, username, providerUserKey, email, passwordQuestion, comment, isApproved,
                 isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockedOutDate);

            return u;
        }

        /// <summary>
        /// Compares password values based on the MembershipPasswordFormat.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="dbpassword">The dbpassword.</param>
        /// <param name="passwordSalt">The password salt.</param>
        /// <param name="passwordFormat">The password format.</param>
        /// <returns></returns>
        private bool CheckPassword(string password, string dbpassword, string passwordSalt, MembershipPasswordFormat passwordFormat)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass1 = EncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = this.EncodePassword(passwordSalt + password);
                    break;
                default:
                    break;
            }

            return (pass1.Equals(pass2));
        }

        #endregion
    }
}