using System.Security.Principal;
using System.Web.Security;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Context;
using Rainbow.Framework.Providers;

namespace Rainbow.Framework.Security
{
    /// <summary>
    /// RainbowPrincipal
    /// </summary>
    public class RainbowPrincipal : GenericPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RainbowPrincipal"/> class.
        /// </summary>
        /// <param name="identity">A basic implementation of <see cref="T:System.Security.Principal.IIdentity"></see> that represents any user.</param>
        /// <param name="roles">An array of role names to which the user represented by the identity parameter belongs.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public RainbowPrincipal(IIdentity identity, string[] roles)
            : base(identity, roles)
        {}

        /// <summary>
        /// Get the Rainbow User
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Security.Principal.GenericIdentity"></see> of the user represented by the <see cref="T:System.Security.Principal.GenericPrincipal"></see>.</returns>
        public new RainbowUser Identity
        {
            get
            {
                if (base.Identity is MembershipUser)
                {
                    ErrorHandler.Publish(LogLevel.Info,
                                         "RainbowPrincipal::Identity  -> base.Identity is MembershipUser");
                    return base.Identity as RainbowUser;
                }

                return RainbowMembershipProvider.Instance.GetSingleUser(
                    PortalProvider.Instance.CurrentPortal.PortalAlias,
                    base.Identity.Name);
            }
        }

        /// <summary>
        /// CurrentUser
        /// </summary>
        /// <value>The current user.</value>
        public static RainbowPrincipal CurrentUser
        {
            get
            {
                if (RainbowContext.Current.HttpContext.User is RainbowPrincipal)
                {
                    return (RainbowPrincipal) RainbowContext.Current.HttpContext.User;
                }
                else
                {
                    return new RainbowPrincipal(RainbowContext.Current.HttpContext.User.Identity, null);
                }
            }
        }
    }
}
