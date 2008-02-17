using System;
using System.Collections.Generic;
using System.Web.Security;
using Rainbow.Framework.BusinessObjects;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Providers.Exceptions;

namespace Rainbow.Framework
{
    /// <summary>
    /// The AccountSystem class encapsulates all data logic necessary to add/login/query
    /// users within the Portal Users database.
    /// It is a facade (see http://www.dofactory.com/Patterns/PatternFacade.aspx)
    /// for RainbowMembershipProvider, RainbowRoleProvider and PortalProvider.
    /// <remarks>
    /// Important Note: The AccountSystem class is only used when forms-based cookie
    /// authentication is enabled within the portal.  When windows based
    /// authentication is used instead, then either the Windows SAM or Active Directory
    /// is used to store and validate all username/password credentials.
    /// </remarks>
    /// </summary>
    [History( "jminond", "2005/03/10", "Tab to page conversion" )]
    [History( "gman3001", "2004/09/29", "Added the UpdateLastVisit method to update the user's last visit date indicator." )]
    //TODO: [moudrick] move it to Rainbow.Framework
    public class AccountSystem 
    {
        static readonly AccountSystem instance = new AccountSystem(RainbowMembershipProvider.Instance,
                                                                   RainbowRoleProvider.Instance,
                                                                   PortalProvider.Instance);

        readonly RainbowMembershipProvider membershipProvider;
        readonly RainbowRoleProvider roleProvider;
        readonly PortalProvider portalProvider;

        /// <summary>
        /// Singletone pattern standard member (http://www.dofactory.com/Patterns/PatternSingleton.aspx)
        /// </summary>
        public static AccountSystem Instance
        {
            get
            {
                return instance;
            }
        }

        Portal CurrentPortal 
        {
            get 
            {
                return portalProvider.CurrentPortal;
            }
        }

        AccountSystem(RainbowMembershipProvider membershipProvider, 
                      RainbowRoleProvider roleProvider,
                      PortalProvider portalProvider)
        {
            this.membershipProvider = membershipProvider;
            this.portalProvider = portalProvider;
            this.roleProvider = roleProvider;
        }

        #region Add Roles and Users

        /// <summary>
        /// AddRole() Method <a name="AddRole"></a>
        /// The AddRole method creates a new security role for the specified portal,
        /// and returns the new RoleID value.
        /// Other relevant sources:
        /// + <a href="AddRole.htm" style="color:green">AddRole Stored Procedure</a>
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        public Guid AddRole(string roleName)
        {
            return roleProvider.CreateRole(PortalProvider.Instance.CurrentPortal.PortalAlias, roleName);
        }

        /// <summary>
        /// AddUser
        /// </summary>
        /// <param name="portalAlias">Portal alias where user to be added</param>
        /// <param name="name">The name.</param>
        /// <param name="company">The company.</param>
        /// <param name="address">The address.</param>
        /// <param name="city">The city.</param>
        /// <param name="zip">The zip.</param>
        /// <param name="countryID">The country ID.</param>
        /// <param name="stateID">The state ID.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="fax">The fax.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="sendNewsletter">if set to <c>true</c> [send newsletter].</param>
        /// <returns>The newly created ID</returns>
        public Guid AddUser(string portalAlias,
                            string name,
                            string company,
                            string address,
                            string city,
                            string zip,
                            string countryID,
                            int stateID,
                            string phone,
                            string fax,
                            string password,
                            string email,
                            bool sendNewsletter)
        {
            MembershipCreateStatus status;
            MembershipUser user = membershipProvider.CreateUser(portalAlias,
                                                                name,
                                                                password,
                                                                email,
                                                                "vacia",
                                                                "llena",
                                                                true,
                                                                out status);

            if (user == null)
            {
                throw new ApplicationException(membershipProvider.GetErrorMessage(status));
            }
            return (Guid) user.ProviderUserKey;
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="portalAlias">Portal alias where user to be added</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public Guid AddUser(string portalAlias, string email, string password, string fullName)
        {
            Guid newUserId = AddUser(portalAlias,
                                     email,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty,
                                     0,
                                     string.Empty,
                                     string.Empty,
                                     password,
                                     email,
                                     false);
            RainbowUser user = membershipProvider.GetUser(newUserId, false) as RainbowUser;
            if (user == null)
            {
                throw new RainbowMembershipProviderException("Could not load user");
            }
            user.Name = fullName;
            membershipProvider.UpdateUser(user);
            return newUserId;
        }

        /// <summary>
        /// AddUserRole() Method <a name="AddUserRole"></a>
        /// The AddUserRole method adds the user to the specified security role.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="userID">The user ID.</param>
        public void AddUserRole(Guid roleID, Guid userID)
        {
            roleProvider.AddUsersToRoles(CurrentPortal.PortalAlias,
                                         new Guid[] {userID},
                                         new Guid[] {roleID});
        }

        #endregion

        #region Delete Roles and Users

        /// <summary>
        /// DeleteRole() Method <a name="DeleteRole"></a>
        /// The DeleteRole deletes the specified role from the portal database.
        /// Other relevant sources:
        /// + <a href="DeleteRole.htm" style="color:green">DeleteRole Stored Procedure</a>
        /// </summary>
        /// <param name="roleID">The role id.</param>
        public void DeleteRole(Guid roleID)
        {
            roleProvider.DeleteRole(CurrentPortal.PortalAlias, roleID, false);
        }

        /// <summary>
        /// The DeleteUser method deleted a  user record from the "Users" database table.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void DeleteUser(Guid userID)
        {
            MembershipUser user = Membership.GetUser(userID);
            Membership.DeleteUser(user.UserName);
        }

        /// <summary>
        /// DeleteUserRole() Method <a name="DeleteUserRole"></a>
        /// The DeleteUserRole method deletes the user from the specified role.
        /// Other relevant sources:
        /// + <a href="DeleteUserRole.htm" style="color:green">DeleteUserRole Stored Procedure</a>
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="userID">The user ID.</param>
        public void DeleteUserRole(Guid roleID, Guid userID)
        {
            roleProvider.RemoveUsersFromRoles(CurrentPortal.PortalAlias,
                                              new Guid[] {userID},
                                              new Guid[] {roleID});
        }

        #endregion

        /// <summary>
        /// The GetPortalRoles method returns a list of all roles for the specified portal.
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns>A <see cref="IList{RainbowRole}"/> containing all role objects.
        /// </returns>
        public IList<RainbowRole> GetPortalRoles(string portalAlias)
        {
            return roleProvider.GetAllRoles(portalAlias);
        }

        /// <summary>
        /// The GetUsersNoRole method retuns a list of all members that doesn´t have any roles.
        /// </summary>
        /// <param name="portalID">The portal id</param>
        /// <returns></returns>
        public string[] GetUsersNoRole(int portalID)
        {
            IList<string> res = new List<string>();
            MembershipUserCollection userCollection = Membership.GetAllUsers();
            foreach (MembershipUser user in userCollection)
            {
                if (Roles.GetRolesForUser(user.UserName).Length == 0)
                {
                    res.Add(user.UserName);
                }
            }
            return ((List<string>) res).ToArray();
        }

        /// <summary>
        /// The GetRoleNonMembers method returns a list of roles that doesn´t have any members.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="portalAlias">The portal alias</param>
        public IList<RainbowRole> GetRoleNonMembers(Guid roleId, string portalAlias)
        {
            IList<RainbowRole> result = new List<RainbowRole>();
            IList<RainbowRole> allRoles = roleProvider.GetAllRoles(portalAlias);

            foreach (RainbowRole s in allRoles)
            {
                if (roleProvider.GetUsersInRole(portalAlias, s.Id).Length == 0)
                {
                    result.Add(s);
                }
            }
            return result;
        }

        /// <summary>
        /// The GetRoles method returns a list of roles for the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns>A <code>IList&lt;RainbowRole&gt;</code> containing the user's roles</returns>
        public IList<RainbowRole> GetRoles(string email, string portalAlias)
        {
            string userName = membershipProvider.GetUserNameByEmail(portalAlias, email);
            RainbowUser user = (RainbowUser) membershipProvider.GetUser(portalAlias, userName, true);
            return roleProvider.GetRolesForUser(portalAlias, user.ProviderUserKey);
        }

        /// <summary>
        /// Return the role list the user's in
        /// </summary>
        /// <param name="userId">The User Id</param>
        /// <param name="portalAlias">The portal alias</param>
        /// <returns></returns>
        public IList<RainbowRole> GetRolesByUser(Guid userId, string portalAlias)
        {
            RainbowUser user = (RainbowUser) membershipProvider.GetUser(userId, true);
            return roleProvider.GetRolesForUser(portalAlias, user.ProviderUserKey);
        }

        /// <summary>
        /// Renames a role
        /// </summary>
        /// <param name="roleId">The role id</param>
        /// <param name="newRoleName">The new role name</param>
        /// <param name="portalAlias">The portal alias</param>
        public void UpdateRole(Guid roleId, string newRoleName, string portalAlias)
        {
            roleProvider.RenameRole(portalAlias, roleId, newRoleName);
        }

        /// <summary>
        /// Retrieves a <code>MembershipUser</code>.
        /// </summary>
        /// <param name="userName">the user's email</param>
        /// <returns></returns>
        public RainbowUser GetSingleUser(string userName) 
        {
            return membershipProvider.GetSingleUser(CurrentPortal.PortalAlias, userName);
        }

        /// <summary>
        /// UpdateUser
        /// This overload allow to change identity of the user
        /// </summary>
        /// <param name="oldUserID">The old user ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="name">The name.</param>
        /// <param name="company">The company.</param>
        /// <param name="address">The address.</param>
        /// <param name="city">The city.</param>
        /// <param name="zip">The zip.</param>
        /// <param name="countryID">The country ID.</param>
        /// <param name="stateID">The state ID.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="fax">The fax.</param>
        /// <param name="email">The email.</param>
        /// <param name="sendNewsletter">if set to <c>true</c> [send newsletter].</param>
        public void UpdateUser(Guid oldUserID,
                               Guid userID,
                               string name,
                               string company,
                               string address,
                               string city,
                               string zip,
                               string countryID,
                               int stateID,
                               string phone,
                               string fax,
                               string email,
                               bool sendNewsletter)
        {
            if (oldUserID != userID)
            {
                throw new ApplicationException("UpdateUser: oldUserID != userID");
            }

            RainbowUser user = membershipProvider.GetUser(userID, true) as RainbowUser;
            if (user == null)
            {
                throw new RainbowMembershipProviderException("Could not load user");
            }
            user.Email = email;
            user.Name = name;
            user.Company = company;
            user.Address = address;
            user.Zip = zip;
            user.City = city;
            user.CountryID = countryID;
            user.StateID = stateID;
            user.Fax = fax;
            user.Phone = phone;
            user.SendNewsletter = sendNewsletter;

            membershipProvider.UpdateUser(user);
        }

        /// <summary>
        /// UpdateUser
        /// Autogenerated by CodeWizard 04/04/2003 17.55.40
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="name">The name.</param>
        /// <param name="company">The company.</param>
        /// <param name="address">The address.</param>
        /// <param name="city">The city.</param>
        /// <param name="zip">The zip.</param>
        /// <param name="countryID">The country ID.</param>
        /// <param name="stateID">The state ID.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="fax">The fax.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="sendNewsletter">if set to <c>true</c> [send newsletter].</param>
        public void UpdateUser(Guid userID,
                               string name,
                               string company,
                               string address,
                               string city,
                               string zip,
                               string countryID,
                               int stateID,
                               string phone,
                               string fax,
                               string password,
                               string email,
                               bool sendNewsletter)
        {
            RainbowUser user = membershipProvider.GetUser(userID, true) as RainbowUser;
            if (user == null)
            {
                throw new RainbowMembershipProviderException("Could not load user");
            }
            user.Email = email;
            user.Name = name;
            user.Company = company;
            user.Address = address;
            user.Zip = zip;
            user.City = city;
            user.CountryID = countryID;
            user.StateID = stateID;
            user.Fax = fax;
            user.Phone = phone;
            user.SendNewsletter = sendNewsletter;

            membershipProvider.ChangePassword(CurrentPortal.PortalAlias,
                                              user.UserName,
                                              user.GetPassword(),
                                              password);
            membershipProvider.UpdateUser(user);
        }

        /// <summary>
        /// UpdateUser
        /// Autogenerated by CodeWizard 04/04/2003 17.55.40
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="name">The name.</param>
        /// <param name="company">The company.</param>
        /// <param name="address">The address.</param>
        /// <param name="city">The city.</param>
        /// <param name="zip">The zip.</param>
        /// <param name="countryID">The country ID.</param>
        /// <param name="stateID">The state ID.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="fax">The fax.</param>
        /// <param name="email">The email.</param>
        /// <param name="sendNewsletter">if set to <c>true</c> [send newsletter].</param>
        public void UpdateUser(Guid userID,
                               string name,
                               string company,
                               string address,
                               string city,
                               string zip,
                               string countryID,
                               int stateID,
                               string phone,
                               string fax,
                               string email,
                               bool sendNewsletter)
        {
            RainbowUser user = membershipProvider.GetUser(userID, true) as RainbowUser;
            if (user == null)
            {
                throw new RainbowMembershipProviderException("Could not load user");
            }
            user.Email = email;
            user.Name = name;
            user.Company = company;
            user.Address = address;
            user.Zip = zip;
            user.City = city;
            user.CountryID = countryID;
            user.StateID = stateID;
            user.Fax = fax;
            user.Phone = phone;
            user.SendNewsletter = sendNewsletter;

            membershipProvider.UpdateUser(user);
        }
    }
}