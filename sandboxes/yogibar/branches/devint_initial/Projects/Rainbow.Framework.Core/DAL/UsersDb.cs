using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using Rainbow.Framework.BLL.User;
using Rainbow.Framework.BLL.UserConfig;
using Rainbow.Framework.BLL.Utils;
using Rainbow.Framework.Helpers;
using Rainbow.Framework.Security;
using Rainbow.Framework.Settings;
using Rainbow.Framework.Site.Configuration;
using Rainbow.Framework.Providers.RainbowMembershipProvider;
using Rainbow.Framework.Providers.RainbowRoleProvider;

namespace Rainbow.Framework.Users.Data {
    /// <summary>
    /// The UsersDB class encapsulates all data logic necessary to add/login/query
    /// users within the Portal Users database.
    ///
    /// <remarks>
    /// Important Note: The UsersDB class is only used when forms-based cookie
    /// authentication is enabled within the portal.  When windows based
    /// authentication is used instead, then either the Windows SAM or Active Directory
    /// is used to store and validate all username/password credentials.
    /// </remarks>
    /// </summary>
    [History( "jminond", "2005/03/10", "Tab to page conversion" )]
    [History( "gman3001", "2004/09/29", "Added the UpdateLastVisit method to update the user's last visit date indicator." )]
    public class UsersDB {

        #region Properties

        private RainbowMembershipProvider MembershipProvider {
            get {
                if ( !( Membership.Provider is RainbowMembershipProvider ) ) {
                    throw new RainbowMembershipProviderException( "The membership provider must be a RainbowMembershipProvider implementation" );
                }
                return Membership.Provider as RainbowMembershipProvider;
            }
        }

        private RainbowRoleProvider RoleProvider {
            get {
                if ( !( Roles.Provider is RainbowRoleProvider ) ) {
                    throw new RainbowRoleProviderException( "The role provider must be a RainbowRoleProvider implementation" );
                }
                return Roles.Provider as RainbowRoleProvider;
            }
        }

        private PortalSettings CurrentPortalSettings {
            get {
                return ( PortalSettings )HttpContext.Current.Items["PortalSettings"];
            }
        }

        #endregion

        //private CryptoHelper cryphelp = null;

        #region Add Roles and Users

        /// <summary>
        /// AddRole() Method <a name="AddRole"></a>
        /// The AddRole method creates a new security role for the specified portal,
        /// and returns the new RoleID value.
        /// Other relevant sources:
        /// + <a href="AddRole.htm" style="color:green">AddRole Stored Procedure</a>
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        public Guid AddRole( string roleName ) {
            return RoleProvider.CreateRole( CurrentPortalSettings.PortalAlias, roleName );
        }

        /// <summary>
        /// AddUser
        /// </summary>
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
        public Guid AddUser( string name, string company, string address, string city, string zip,
                           string countryID, int stateID, string phone, string fax,
                           string password, string email, bool sendNewsletter ) {

            MembershipCreateStatus status;
            MembershipUser user = MembershipProvider.CreateUser( CurrentPortalSettings.PortalAlias, name,
                password, email, "vacia", "llena", true, out status );

            if ( user == null ) {
                throw new ApplicationException( MembershipProvider.GetErrorMessage( status ) );
            }

            return ( Guid )user.ProviderUserKey;
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="fullName">The full name.</param>(6)
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public Guid AddUser( string fullName, string email, string password ) {

            Guid newUserId = AddUser( email, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, 0, string.Empty, string.Empty, password, email, false );
            RainbowUser user = MembershipProvider.GetUser( newUserId, false ) as RainbowUser; 

            user.Name = fullName;
            MembershipProvider.UpdateUser( user );
            return newUserId;
        }

        /// <summary>
        /// AddUserRole() Method <a name="AddUserRole"></a>
        /// The AddUserRole method adds the user to the specified security role.
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="userID">The user ID.</param>
        public void AddUserRole( Guid roleID, Guid userID ) {
            RoleProvider.AddUsersToRoles( CurrentPortalSettings.PortalAlias, new Guid[] { userID }, new Guid[] { roleID } );
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
        public void DeleteRole( Guid roleID ) {
            RoleProvider.DeleteRole( CurrentPortalSettings.PortalAlias, roleID, false );
        }

        /// <summary>
        /// The DeleteUser method deleted a  user record from the "Users" database table.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void DeleteUser( Guid userID ) {
            MembershipUser user = Membership.GetUser( userID );
            Membership.DeleteUser( user.UserName );
        }

        /// <summary>
        /// DeleteUserRole() Method <a name="DeleteUserRole"></a>
        /// The DeleteUserRole method deletes the user from the specified role.
        /// Other relevant sources:
        /// + <a href="DeleteUserRole.htm" style="color:green">DeleteUserRole Stored Procedure</a>
        /// </summary>
        /// <param name="roleID">The role ID.</param>
        /// <param name="userID">The user ID.</param>
        public void DeleteUserRole( Guid roleID, Guid userID ) {
            RoleProvider.RemoveUsersFromRoles(CurrentPortalSettings.PortalAlias, new Guid[] { userID }, new Guid[] { roleID } );
        }

        #endregion

        /// <summary>
        /// Get Current UserID
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        //public Guid GetCurrentUserID( int portalID ) {
        //    MembershipUser user = Membership.GetUser();
        //    return (Guid)user.ProviderUserKey;
        //}

        /// <summary>
        /// The GetPortalRoles method returns a list of all roles for the specified portal.
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns>a <code>IList<RainbowRole></code> containing all role objects.
        /// </returns>
        public IList<RainbowRole> GetPortalRoles( string portalAlias ) {
            return RoleProvider.GetAllRoles( portalAlias );
        }

        /// <summary>
        /// The GetRoleMembers method returns a list of all members in the specified security role.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns>a <code>string[]</code> of user names
        /// </returns>
        public string[] GetRoleMembers( Guid roleId ) {
            return RoleProvider.GetUsersInRole( CurrentPortalSettings.PortalAlias, roleId );
        }

        /// <summary>
        /// The GetUsersNoRole method retuns a list of all members that doesn´t have any roles.
        /// </summary>
        /// <param name="PortalID">The portal id</param>
        /// <returns></returns>
        public string[] GetUsersNoRole( int PortalID ) {
            IList<string> res = new List<string>();
            MembershipUserCollection userCollection = Membership.GetAllUsers();
            foreach ( MembershipUser user in userCollection ) {
                if ( Roles.GetRolesForUser( user.UserName ).Length == 0 ) {
                    res.Add( user.UserName );
                }
            }
            return ( ( List<string> )res ).ToArray();
        }

        /// <summary>
        /// The GetRoleNonMembers method returns a list of roles that doesn´t have any members.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="portalAlias">The portal alias</param>
        public IList<RainbowRole> GetRoleNonMembers( Guid roleId, string portalAlias ) {
            IList<RainbowRole> res = new List<RainbowRole>();

            IList<RainbowRole> allRoles = RoleProvider.GetAllRoles( portalAlias );

            foreach ( RainbowRole s in allRoles ) {
                if ( RoleProvider.GetUsersInRole( portalAlias, s.Id ).Length == 0 ) {
                    res.Add( s );
                }
            }
            return res;
        }

        /// <summary>
        /// The GetRoles method returns a list of roles for the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns>A <code>IList&lt;RainbowRole&gt;</code> containing the user's roles</returns>
        public IList<RainbowRole> GetRoles( string email, string portalAlias ) {
            string userName = MembershipProvider.GetUserNameByEmail( portalAlias, email );
            RainbowUser user = (RainbowUser)MembershipProvider.GetUser( portalAlias, userName, true );
            return RoleProvider.GetRolesForUser( portalAlias, user.ProviderUserKey );
        }

        /// <summary>
        /// Return the role list the user's in
        /// </summary>
        /// <param name="userId">The User Id</param>
        /// <param name="portalAlias">The portal alias</param>
        /// <returns></returns>
        public IList<RainbowRole> GetRolesByUser( Guid userId, string portalAlias ) {
            RainbowUser user = ( RainbowUser )MembershipProvider.GetUser( userId, true );
            return RoleProvider.GetRolesForUser( portalAlias, user.ProviderUserKey );
        }

        /// <summary>
        /// Renames a role
        /// </summary>
        /// <param name="roleId">The role id</param>
        /// <param name="newRoleName">The new role name</param>
        /// <param name="portalAlias">The portal alias</param>
        public void UpdateRole( Guid roleId, string newRoleName, string portalAlias ) {
            RoleProvider.RenameRole( portalAlias, roleId, newRoleName );
        }

        /// <summary>
        /// Retrieves a <code>MembershipUser</code>.
        /// </summary>
        /// <param name="userName">the user's email</param>
        /// <returns></returns>
        public RainbowUser GetSingleUser( string userName ) {

            RainbowUser user = MembershipProvider.GetUser( CurrentPortalSettings.PortalAlias, userName, true ) as RainbowUser;
            return user;
        }

        /// <summary>
        /// The GetUser method returns the collection of users.
        /// </summary>
        /// <returns></returns>
        public MembershipUserCollection GetUsers() {
            int totalRecords;
            return MembershipProvider.GetAllUsers( CurrentPortalSettings.PortalAlias, 0, int.MaxValue, out totalRecords );
        }

        /// <summary>
        /// The GetUsersCount method returns the users count.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public int GetUsersCount( int portalID ) {
            int totalRecords;
            MembershipProvider.GetAllUsers( CurrentPortalSettings.PortalAlias, 0, 1, out totalRecords );
            return totalRecords;
        }

        /// <summary>
        /// UsersDB.Login() Method.
        /// The Login method validates a id/password hash pair against credentials
        /// stored in the users database.  If the id/password hash pair is valid,
        /// the method returns user's name.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <remarks>UserLogin Stored Procedure</remarks>
        public RainbowUser Login( Guid uid, string password ) {

            RainbowUser user = MembershipProvider.GetUser( uid, true ) as RainbowUser;
            bool isValid = MembershipProvider.ValidateUser( user.UserName, password );

            if ( isValid ) {
                return user;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// UsersDB.Login() Method.
        /// The Login method validates a email/password hash pair against credentials
        /// stored in the users database.  If the email/password hash pair is valid,
        /// the method returns user's name.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <remarks>UserLogin Stored Procedure</remarks>
        public MembershipUser Login( string email, string password ) {

            string userName = MembershipProvider.GetUserNameByEmail( CurrentPortalSettings.PortalAlias, email );

            if ( string.IsNullOrEmpty( userName ) ) {
                return null;
            }
            RainbowUser user = ( RainbowUser )MembershipProvider.GetUser( userName, true );
            bool isValid = MembershipProvider.ValidateUser( user.UserName, password );

            if ( isValid ) {
                return user;
            }
            else {
                return null;
            }
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
        public void UpdateUser( Guid oldUserID, Guid userID, string name, string company, string address,
                               string city, string zip, string countryID, int stateID,
                               string phone, string fax, string email, bool sendNewsletter ) {
            if ( oldUserID != userID ) {
                throw new ApplicationException( "UpdateUser: oldUserID != userID" );
            }

            RainbowUser user = MembershipProvider.GetUser( userID, true ) as RainbowUser;
            user.Name = name;
            user.Company = company;
            user.Address = address;
            user.City = city;
            user.Zip = zip;            
            user.CountryID = countryID;
            user.StateID = stateID;
            user.Phone = phone;
            user.Fax = fax;
            user.Email = email;            
            user.SendNewsletter = sendNewsletter;

            MembershipProvider.UpdateUser( user );
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
        public void UpdateUser( Guid userID, string name, string company, string address, string city,
                               string zip, string countryID, int stateID, string phone,
                               string fax, string password, string email, bool sendNewsletter ) {

            RainbowUser user = MembershipProvider.GetUser( userID, true ) as RainbowUser;
            user.Name = name;
            user.Company = company;
            user.Address = address;
            user.City = city;
            user.Zip = zip;
            user.CountryID = countryID;
            user.StateID = stateID;
            user.Phone = phone;
            user.Fax = fax;            
            user.Email = email;
            user.SendNewsletter = sendNewsletter;
            
            MembershipProvider.ChangePassword( CurrentPortalSettings.PortalAlias, user.UserName, user.GetPassword(), password );            
            MembershipProvider.UpdateUser( user );
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
        public void UpdateUser( Guid userID, string name, string company, string address, string city,
                               string zip, string countryID, int stateID, string phone,
                               string fax, string email, bool sendNewsletter ) {

            RainbowUser user = MembershipProvider.GetUser( userID, true ) as RainbowUser;
            user.Name = name;
            user.Company = company;
            user.Address = address;            
            user.City = city;
            user.Zip = zip;
            user.CountryID = countryID;
            user.StateID = stateID;
            user.Phone = phone;
            user.Fax = fax;
            user.Email = email;            
            user.SendNewsletter = sendNewsletter;

            MembershipProvider.UpdateUser( user );
        }

        /// <summary>
        /// The UpdateUserCheckEmail sets the user email as trusted and verified
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="CheckedEmail">The checked email.</param>
      public void UpdateUserCheckEmail( int userID, bool CheckedEmail ) {
            MembershipUser user = Membership.GetUser( userID );
            user.IsApproved = CheckedEmail;

            Membership.UpdateUser( user );
        }

        /// <summary>
        /// Change the user password with a new one
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="password">The password.</param>
      public void UpdateUserSetPassword( int userID, string password ) {
            MembershipUser user = Membership.GetUser( userID );
            user.ChangePassword( user.GetPassword(), password );

            Membership.UpdateUser( user );
        }
    }

    /// <summary>
    /// [START -- Added for User 's WIndow Mgmt -- bja@reedtek.com]
    ///
    /// UserWinMgmtDB Class
    ///
    /// The UserWinMgmtDB class encapsulates all data logic necessary to add/delete/query
    /// users window mgmt level ( min/max/close )
    /// </summary>
    public sealed class UserWinMgmtDB {
        /// <summary>
        /// DeleteUserDeskTop Method
        /// The DeleteUserDeskTop method deletes a specific user desktop from the serDesktop database table.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>A bool value...</returns>
        public bool DeleteUserDeskTop( Guid userID, int portalID ) {
            bool results = false;

            // john.mandia@whitelightsolutions.com: 29th May 2004: Since this has to do with collapsable modules on a specific tab on a specific portal I don't think it should pass the static portal ID
            //if (Config.UseSingleUserBase) portalID = 0;
            // Create Instance of Connection and Command Object
            using ( SqlConnection myConnection = Config.SqlConnectionString ) {
                using ( SqlCommand myCommand = new SqlCommand( "rb_DeleteUserDesktop", myConnection ) ) {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Update Parameters to SPROC
                    SqlParameter parameterUserID = new SqlParameter( "@UserID", SqlDbType.Int );
                    parameterUserID.Value = userID;
                    myCommand.Parameters.Add( parameterUserID );
                    SqlParameter parameterPortalID = new SqlParameter( "@PortalID", SqlDbType.Int );
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add( parameterPortalID );

                    myConnection.Open();
                    try {
                        myCommand.ExecuteNonQuery();
                        results = true;
                    }
                    catch {
                    }
                }
                return results;
            }
        } // end of DeleteUserDeskTop

        /// <summary>
        /// GetUserDesktop() Method <a name="GetUserDesktop"></a>
        /// The GetUserDesktop method returns a list of the users' desktop envrionment
        /// Other relevant sources:
        /// + <a href="GetUserDesktop.htm" style="color:green">GetUserDesktop Stored Procedure</a>
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public UserLayoutMgr GetUserDesktop( Guid userID, int portalID ) {
            UserLayoutMgr ulm = new UserLayoutMgr();
            /* Commented because rb_GetUserDesktop procedure is missing
						// Create Instance of Connection and Command Object
						using (SqlConnection myConnection = Config.SqlConnectionString)
						{
							using (SqlCommand myCommand = new SqlCommand("rb_GetUserDesktop", myConnection))
							{
								// Mark the Command as a SPROC
								myCommand.CommandType = CommandType.StoredProcedure;
								SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
								parameterUserID.Value = userID;
								myCommand.Parameters.Add(parameterUserID);
								SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
								parameterPortalID.Value = portalID;
								myCommand.Parameters.Add(parameterPortalID);
								myConnection.Open();
								using (SqlDataReader dr = myCommand.ExecuteReader()) {
									if ( dr != null )
										ulm = populateUserLayoutMgr(dr, ulm);
								} 
							}
						}
			*/
            return ulm;
        } // end of GetUserDesktop

        /// <summary>
        /// Save the User's Window Desktop Settings
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="wstate">The wstate.</param>
        public void SaveUserDesktop( int portalID, Guid userID, int moduleID, int pageID, WindowStateEnum wstate ) {
            // john.mandia@whitelightsolutions.com: 29th May 2004: Since this has to do with collapsable modules on a specific tab on a specific portal I don't think it should pass the static portal ID
            //if (Config.UseSingleUserBase) portalID = 0;
            // Create Instance of Connection and Command Object
            using ( SqlConnection myConnection = Config.SqlConnectionString ) {
                using ( SqlCommand myCommand = new SqlCommand( "rb_UpdateUserDesktop", myConnection ) ) {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Update Parameters to SPROC
                    SqlParameter parameterUserID = new SqlParameter( "@UserID", SqlDbType.Int );
                    parameterUserID.Value = userID;
                    myCommand.Parameters.Add( parameterUserID );
                    SqlParameter parameterPortalID = new SqlParameter( "@PortalID", SqlDbType.Int );
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add( parameterPortalID );
                    SqlParameter parameterModuleID = new SqlParameter( "@ModuleID", SqlDbType.Int );
                    parameterModuleID.Value = moduleID;
                    myCommand.Parameters.Add( parameterModuleID );
                    SqlParameter parameterPageID = new SqlParameter( "@TabID", SqlDbType.Int );
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add( parameterPageID );
                    SqlParameter parameterState = new SqlParameter( "@WindowState", SqlDbType.SmallInt );
                    parameterState.Value = ( short )wstate;
                    myCommand.Parameters.Add( parameterState );
                    // Execute the command
                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                }
            }
        } // end of SaveUserDesktop

        #region Private Methods

        /// <summary>
        /// Create the Module Settings
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns></returns>
        private UserModuleSettings addModuleSettings( SqlDataReader dr ) {
            UserModuleSettings ums = new UserModuleSettings();

            try {
                string name = string.Empty;

                // iterate over the fields/columns
                for ( int inx = 0; inx < dr.FieldCount; ++inx ) {
                    // get the values
                    if ( !dr.IsDBNull( inx ) ) {
                        name = dr.GetName( inx ).ToLower();

                        if ( name[0] == 'm' ) // module id
                            ums.ModuleID = dr.GetInt32( inx );

                        else if ( name[0] == 't' ) // tab id
                            ums.PageID = dr.GetInt32( inx );

                        else if ( name[0] == 's' ) // Window State
                            ums.State = ( WindowStateEnum )dr.GetInt16( inx );
                    }
                }
            }
#if !DEBUG

			catch {}
#else

            catch ( Exception ex ) {
                throw ex;
            }
#endif
            return ums;
        } // end of addModuleSettings

        /// <summary>
        /// Populate a User Layout from the Data Reader
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <param name="ulm">The ulm.</param>
        /// <returns></returns>
        private UserLayoutMgr populateUserLayoutMgr( SqlDataReader dr, UserLayoutMgr ulm ) {
            while ( dr.Read() )
                ulm.Add( addModuleSettings( dr ) );
            return ulm;
        } // end of populateUserLayoutMgr

        #endregion
    } // end of UserWinMgmtDB
    //*********************************************************************
    // [END -- Added for User 's WIndow Mgmt -- bja@reedtek.com]
    //
    //*********************************************************************
}
