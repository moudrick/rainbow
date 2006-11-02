using System.Security.Principal;

namespace Rainbow.Security
{
	/// <summary>
	/// RainbowPrincipal
	/// </summary>
	public class RainbowPrincipal : GenericPrincipal
	{
		/// <summary>
		///     
		/// </summary>
		/// <param name="identity" type="System.Security.Principal.IIdentity">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="roles" type="string[]">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public RainbowPrincipal(IIdentity identity, string[] roles) : base(identity, roles)
		{
		}

		/// <summary>
		/// Get the Rainbow User
		/// </summary>
		public new User Identity 
		{
			get
			{
				if (base.Identity is User)
				{
					return base.Identity as User;
				}

				User r = new User(base.Identity.Name, "Rainbow");
				return r;
			}
		}
	}
}