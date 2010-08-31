namespace Rainbow.Framework.Security
{
    using System.Security.Principal;
    using System.Web.Security;

    using Rainbow.Framework.Providers.RainbowMembershipProvider;

    /// <summary>
    /// RainbowPrincipal
    /// </summary>
    public class RainbowPrincipal : GenericPrincipal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowPrincipal"/> class. 
        /// </summary>
        /// <param name="identity">
        /// A basic implementation of <see cref="T:System.Security.Principal.IIdentity"></see> that represents any user.
        /// </param>
        /// <param name="roles">
        /// An array of role names to which the user represented by the identity parameter belongs.
        /// </param>
        /// <returns>
        /// A void value...
        /// </returns>
        public RainbowPrincipal(IIdentity identity, string[] roles)
            : base(identity, roles)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the Rainbow User
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref = "T:System.Security.Principal.GenericIdentity"></see> of the user represented by the <see cref = "T:System.Security.Principal.GenericPrincipal"></see>.</returns>
        public new RainbowUser Identity
        {
            get
            {
                if (base.Identity is MembershipUser)
                {
                    ErrorHandler.Publish(
                        LogLevels.Info, "RainbowPrincipal::Identity  -> base.Identity is MembershipUser");
                    return base.Identity as RainbowUser;
                }

                var users = new UsersDB();
                return users.GetSingleUser(base.Identity.Name);
            }
        }

        #endregion
    }
}