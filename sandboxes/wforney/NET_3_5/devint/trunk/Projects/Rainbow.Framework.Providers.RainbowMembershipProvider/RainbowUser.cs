namespace Rainbow.Framework.Providers.RainbowMembershipProvider
{
    using System;
    using System.Security.Principal;
    using System.Web.Profile;
    using System.Web.Security;

    /// <summary>
    /// Rainbow specific membership user class.
    /// </summary>
    public class RainbowUser : MembershipUser, IIdentity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowUser"/> class.
        /// </summary>
        /// <param name="providerName">
        /// Name of the provider.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="passwordQuestion">
        /// The password question.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="isApproved">
        /// if set to <c>true</c> [is approved].
        /// </param>
        /// <param name="isLockedOut">
        /// if set to <c>true</c> [is locked out].
        /// </param>
        /// <param name="creationDate">
        /// The creation date.
        /// </param>
        /// <param name="lastLoginDate">
        /// The last login date.
        /// </param>
        /// <param name="lastActivityDate">
        /// The last activity date.
        /// </param>
        /// <param name="lastPasswordChangeDate">
        /// The last password change date.
        /// </param>
        /// <param name="lastLockoutDate">
        /// The last lockout date.
        /// </param>
        public RainbowUser(
            string providerName,
            string name,
            Guid providerUserKey,
            string email,
            string passwordQuestion,
            string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
            DateTime lastActivityDate,
            DateTime lastPasswordChangeDate,
            DateTime lastLockoutDate)
            : base(
                providerName,
                name,
                providerUserKey,
                email,
                passwordQuestion,
                comment,
                isApproved,
                isLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangeDate,
                lastLockoutDate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowUser"/> class.
        /// </summary>
        /// <param name="name">
        /// The user name.
        /// </param>
        public RainbowUser(string name)
            : base(
                Membership.Provider.Name,
                name,
                Guid.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                true,
                false,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue)
        {
            this.Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address { get; set; }

        /// <summary>
        ///     Gets the type of authentication used.
        /// </summary>
        /// <value></value>
        /// <returns>The type of authentication used to identify the user.</returns>
        public string AuthenticationType
        {
            get
            {
                throw new Exception("Rainbow.");
            }
        }

        /// <summary>
        ///     Gets or sets the city.
        /// </summary>
        /// <value>The city name.</value>
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public string Company { get; set; }

        /// <summary>
        ///     Gets or sets the country ID.
        /// </summary>
        /// <value>The country ID.</value>
        public string CountryID { get; set; }

        /// <summary>
        ///     Gets or sets the fax.
        /// </summary>
        /// <value>The fax number.</value>
        public string Fax { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the user has been authenticated.
        /// </summary>
        /// <value></value>
        /// <returns>true if the user was authenticated; otherwise, false.</returns>
        public bool IsAuthenticated
        {
            get
            {
                return this.IsOnline;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the current user.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the user on whose behalf the code is running.</returns>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        public string Phone { get; set; }

        /// <summary>
        ///     Gets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public ProfileBase Profile
        {
            get
            {
                return ProfileBase.Create(this.UserName);
            }
        }

        /// <summary>
        ///     Gets the user identifier from the membership data source for the user.
        /// </summary>
        /// <value></value>
        /// <returns>The user identifier from the membership data source for the user.</returns>
        public new Guid ProviderUserKey
        {
            get
            {
                return (Guid)(base.ProviderUserKey ?? Guid.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user receives newletters.
        /// </summary>
        /// <value><c>true</c> if the user receives newletters; otherwise, <c>false</c>.</value>
        public bool SendNewsletter { get; set; }

        /// <summary>
        ///     Gets or sets the state ID.
        /// </summary>
        /// <value>The state ID.</value>
        public int StateID { get; set; }

        /// <summary>
        ///     Gets or sets the zip.
        /// </summary>
        /// <value>The zip code.</value>
        public string Zip { get; set; }

        /// <summary>
        ///     Gets or sets application-specific information for the membership user. We hide this property, as it isn't used in Rainbow
        /// </summary>
        /// <value></value>
        /// <returns>Application-specific information for the membership user.</returns>
        protected new string Comment
        {
            get
            {
                return base.Comment;
            }

            set
            {
                base.Comment = value;
            }
        }

        #endregion
    }
}