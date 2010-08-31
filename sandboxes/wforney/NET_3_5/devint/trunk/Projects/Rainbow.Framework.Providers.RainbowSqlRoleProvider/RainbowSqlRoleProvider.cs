namespace Rainbow.Framework.Providers.RainbowRoleProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;
    using System.Web.Security;

    using Rainbow.Framework.Providers.RainbowMembershipProvider;

    /// <summary>
    /// Rainbow MS Sql Role Provider
    /// </summary>
    public class RainbowSqlRoleProvider : RainbowRoleProvider
    {
        #region Constants and Fields

        /// <summary>
        ///     The event log.
        /// </summary>
        private const string EventLog = "Application";

        /// <summary>
        ///     The event source.
        /// </summary>
        private const string EventSource = "RainbowSqlRoleProvider";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets ApplicationName.
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        /// <remarks>
        ///     should not be used after LINQ conversion
        /// </remarks>
        /// <value>The connection string.</value>
        public string ConnectionString { get; protected set; }

        /// <summary>
        ///     Gets or sets a value indicating whether WriteExceptionsToEventLog.
        ///     If false, exceptions are thrown to the caller. If true,
        ///     exceptions are written to the event log.
        /// </summary>
        public bool WriteExceptionsToEventLog { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">
        /// A string array of user names to be added to the specified roles.
        /// </param>
        /// <param name="roleNames">
        /// A string array of the role names to add the specified user names to.
        /// </param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var userIds = new Guid[usernames.Length];
            var roleIds = new Guid[roleNames.Length];

            RainbowUser user;
            for (var i = 0; i < usernames.Length; i++)
            {
                user = (RainbowUser)Membership.GetUser(usernames[i]);

                if (user == null)
                {
                    throw new RainbowMembershipProviderException(string.Format("User {0} doesn't exist", usernames[i]));
                }

                userIds[i] = user.ProviderUserKey;
            }

            RainbowRole role;
            for (var i = 0; i < roleNames.Length; i++)
            {
                role = this.GetRoleByName(this.ApplicationName, roleNames[i]);
                roleIds[i] = role.Id;
            }

            this.AddUsersToRoles(this.ApplicationName, userIds, roleIds);
        }

        /// <summary>
        /// Takes, as input, a list of user names and a list of role ids and adds the specified users to the specified roles.
        /// </summary>
        /// <param name="portalAlias">
        /// Rainbow's portal alias
        /// </param>
        /// <param name="userIds">
        /// A list of user ids
        /// </param>
        /// <param name="roleIds">
        /// A list of role ids
        /// </param>
        /// <exception cref="ProviderException">
        /// AddUsersToRoles throws a ProviderException if any of the user names or role names do not exist.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If any user name or role name is null (Nothing in Visual Basic), AddUsersToRoles throws an ArgumentNullException.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If any user name or role name is an empty string, AddUsersToRoles throws an ArgumentException.
        /// </exception>
        public override void AddUsersToRoles(string portalAlias, Guid[] userIds, Guid[] roleIds)
        {
            var userIdsStr = string.Empty;
            var roleIdsStr = string.Empty;

            userIdsStr = userIds.Aggregate(userIdsStr, (current, userId) => current + (userId + ","));

            userIdsStr = userIdsStr.Substring(0, userIdsStr.Length - 1);

            roleIdsStr = roleIds.Aggregate(roleIdsStr, (current, roleId) => current + (roleId + ","));

            roleIdsStr = roleIdsStr.Substring(0, roleIdsStr.Length - 1);

            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_UsersInRoles_AddUsersToRoles", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@UserIds", SqlDbType.VarChar, 4000).Value = userIdsStr;
            cmd.Parameters.Add("@RoleIds", SqlDbType.VarChar, 4000).Value = roleIdsStr;

            var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                var returnCode = (int)returnCodeParam.Value;

                switch (returnCode)
                {
                    case 0:
                        return;
                    case 2:
                        throw new RainbowRoleProviderException(
                            string.Format("Application {0} doesn't exist", portalAlias));
                    case 3:
                        throw new RainbowRoleProviderException("One of the roles doesn't exist");
                    case 4:
                        throw new RainbowMembershipProviderException("One of the users doesn't exist");
                    default:
                        throw new RainbowRoleProviderException(
                            string.Format("aspnet_UsersInRoles_AddUsersToRoles returned error code {0}", returnCode));
                }
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateRole");
                }

                throw new RainbowRoleProviderException(
                    "Error executing aspnet_UsersInRoles_AddUsersToRoles stored proc", e);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role to create.
        /// </param>
        public override void CreateRole(string roleName)
        {
            this.CreateRole(this.ApplicationName, roleName);
        }

        /// <summary>
        /// Takes, as input, a role name and creates the specified role.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleName">A role name</param>
        /// <returns>The new role ID</returns>
        /// <exception cref="ProviderException">
        /// CreateRole throws a ProviderException if the role already exists, the role
        /// name contains a comma, or the role name exceeds the maximum length allowed by the data source.
        /// </exception>
        public override Guid CreateRole(string portalAlias, string roleName)
        {
            if (roleName.IndexOf(',') != -1)
            {
                throw new RainbowRoleProviderException("Role name can't contain commas");
            }

            using (var connection = new SqlConnection(this.ConnectionString))
            using (
                var cmd = new SqlCommand("aspnet_Roles_CreateRole", connection)
                    {
                       CommandType = CommandType.StoredProcedure, 
                    })
            {
                cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
                cmd.Parameters.Add("@RoleName", SqlDbType.NVarChar, 256).Value = roleName;

                var newRoleIdParam = cmd.Parameters.Add("@NewRoleId", SqlDbType.UniqueIdentifier);
                newRoleIdParam.Direction = ParameterDirection.Output;

                var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
                returnCodeParam.Direction = ParameterDirection.ReturnValue;

                try
                {
                    cmd.Connection.Open();

                    cmd.ExecuteNonQuery();

                    var returnCode = (int)returnCodeParam.Value;

                    if (returnCode != 0)
                    {
                        throw new RainbowRoleProviderException(string.Format("Error creating role {0}", roleName));
                    }

                    return (Guid)newRoleIdParam.Value;
                }
                catch (SqlException e)
                {
                    if (this.WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "CreateRole");
                    }

                    throw new RainbowRoleProviderException("Error executing aspnet_Roles_CreateRole stored proc", e);
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role to delete.
        /// </param>
        /// <param name="throwOnPopulatedRole">
        /// If true, throw an exception if <paramref name="roleName"/> has one or more members and do not delete <paramref name="roleName"/>.
        /// </param>
        /// <returns>
        /// true if the role was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var role = this.GetRoleByName(this.ApplicationName, roleName);

            return this.DeleteRole(this.ApplicationName, role.Id, throwOnPopulatedRole);
        }

        /// <summary>
        /// Takes, as input, a role id and a Boolean value that indicates whether to throw an exception if there are
        /// users currently associated with the role, and then deletes the specified role.
        /// If throwOnPopulatedRole is false, DeleteRole deletes the role whether it is empty or not.
        /// When DeleteRole deletes a role and there are users assigned to that role, it also removes users from the role.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleId">The role id</param>
        /// <param name="throwOnPopulatedRole">indicates whether to throw an exception if there are users currently associated with
        /// the role, and then deletes the specified role.</param>
        /// <returns>The delete role.</returns>
        /// <exception cref="ProviderException">
        /// If the throwOnPopulatedRole input parameter is true and the specified role has one
        /// or more members, DeleteRole throws a ProviderException and does not delete the role.
        /// </exception>
        public override bool DeleteRole(string portalAlias, Guid roleId, bool throwOnPopulatedRole)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            using (
                var cmd = new SqlCommand("aspnet_Roles_DeleteRole", connection)
                    {
                       CommandType = CommandType.StoredProcedure 
                    })
            {
                cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
                cmd.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = roleId;
                cmd.Parameters.Add("@DeleteOnlyIfRoleIsEmpty", SqlDbType.Bit).Value = throwOnPopulatedRole ? 1 : 0;

                var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
                returnCodeParam.Direction = ParameterDirection.ReturnValue;

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();

                    var returnCode = (int)returnCodeParam.Value;

                    switch (returnCode)
                    {
                        case 0:
                            return true;
                        case 1:
                            throw new RainbowRoleProviderException(
                                string.Format("Application {0} doesn't exist", portalAlias));
                        case 2:
                            throw new RainbowRoleProviderException("Role has members and throwOnPopulatedRole is true");
                        default:
                            throw new RainbowRoleProviderException("Error deleting role");
                    }
                }
                catch (SqlException e)
                {
                    if (this.WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "DeleteRole");
                    }

                    throw new RainbowRoleProviderException("Error executing aspnet_Roles_DeleteRole stored proc", e);
                }
                catch (Exception e)
                {
                    if (this.WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "DeleteRole");
                    }

                    throw new RainbowRoleProviderException(string.Format("Error deleting role {0}", roleId), e);
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">
        /// The role to search in.
        /// </param>
        /// <param name="usernameToMatch">
        /// The user name to search for.
        /// </param>
        /// <returns>
        /// A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch"/> and the user is a member of the specified role.
        /// </returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return this.FindUsersInRole(this.ApplicationName, roleName, usernameToMatch);
        }

        /// <summary>
        /// Takes, as input, a search pattern and a role name and returns a list of users belonging to the specified role
        /// whose user names match the pattern. Wildcard syntax is data-source-dependent and may vary from provider to provider.
        /// User names are returned in alphabetical order.
        /// If the search finds no matches, FindUsersInRole returns an empty string array (a string array with no elements).
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <returns>A list of user names</returns>
        /// <exception cref="ProviderException">
        /// If the role does not exist, FindUsersInRole throws a ProviderException.
        /// </exception>
        public override string[] FindUsersInRole(string portalAlias, string roleName, string usernameToMatch)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            var roles = this.GetAllRoles(this.ApplicationName);

            var result = new string[roles.Count];

            for (var i = 0; i < roles.Count; i++)
            {
                result[i] = roles[i].Name;
            }

            return result;
        }

        /// <summary>
        /// The get all roles.
        /// </summary>
        /// <param name="portalAlias">
        /// The portal alias.
        /// </param>
        /// <returns>
        /// A list of RainbowRole.
        /// </returns>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        public override IList<RainbowRole> GetAllRoles(string portalAlias)
        {
            IList<RainbowRole> result = new List<RainbowRole>();
            result.Insert(0, new RainbowRole(AllUsersGuid, AllUsersRoleName, AllUsersRoleName));
            result.Insert(
                1, new RainbowRole(AuthenticatedUsersGuid, AuthenticatedUsersRoleName, AuthenticatedUsersRoleName));
            result.Insert(
                2, new RainbowRole(UnauthenticatedUsersGuid, UnauthenticatedUsersRoleName, UnauthenticatedUsersRoleName));

            using (var connection = new SqlConnection(this.ConnectionString))
            using (
                var cmd = new SqlCommand("aspnet_Roles_GetAllRoles", connection)
                    {
                       CommandType = CommandType.StoredProcedure 
                    })
            {
                cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;

                SqlDataReader reader = null;
                try
                {
                    cmd.Connection.Open();

                    using (reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var role = GetRoleFromReader(reader);

                            result.Add(role);
                        }

                        reader.Close();
                    }

                    return result;
                }
                catch (SqlException e)
                {
                    if (this.WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "GetAllRoles");
                    }

                    throw new RainbowRoleProviderException("Error executing aspnet_Roles_GetAllRoles stored proc", e);
                }
                catch (Exception e)
                {
                    if (this.WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "GetAllRoles");
                    }

                    throw new RainbowRoleProviderException("Error getting all roles", e);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    cmd.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Retrieves a <code>RainbowRole</code> given a role id
        /// </summary>
        /// <param name="roleId">A role id.</param>
        /// <returns>A <code>RainbowRole</code></returns>
        /// <exception cref="ProviderException">
        /// GetRole throws a ProviderException if the role doesn't exist
        /// </exception>
        public override RainbowRole GetRoleById(Guid roleId)
        {
            var cmd = new SqlCommand
                {
                   CommandText = "SELECT RoleId, RoleName, Description FROM aspnet_Roles WHERE RoleId=@RoleId" 
                };
            cmd.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = roleId;

            cmd.Connection = new SqlConnection(this.ConnectionString);

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return GetRoleFromReader(reader);
                    }
                    else
                    {
                        throw new RainbowRoleProviderException("Role doesn't exist");
                    }
                }
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetRoleById");
                }

                throw new RainbowRoleProviderException("Error executing method GetRoleById", e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Retrieves a
        /// <code>
        /// RainbowRole
        /// </code>
        /// given a role name
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleName">The role name</param>
        /// <returns>A <code>RainbowRole</code></returns>
        /// <exception cref="ProviderException">
        /// GetRole throws a ProviderException if the role doesn't exist
        /// </exception>
        public override RainbowRole GetRoleByName(string portalAlias, string roleName)
        {
            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_Roles_GetRoleByName", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@RoleName", SqlDbType.NVarChar, 256).Value = roleName;

            var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return GetRoleFromReader(reader);
                    }
                    else
                    {
                        throw new RainbowRoleProviderException("Role doesn't exist");
                    }
                }
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateRole");
                }

                throw new RainbowRoleProviderException(
                    "Error executing aspnet_UsersInRoles_IsUserInRole stored proc", e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">
        /// The user to return a list of roles for.
        /// </param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        public override string[] GetRolesForUser(string username)
        {
            var user = (RainbowUser)Membership.GetUser(username);
            if (user != null)
            {
                var roles = this.GetRolesForUser(this.ApplicationName, user.ProviderUserKey);

                var result = new string[roles.Count];

                for (var i = 0; i < roles.Count; i++)
                {
                    result[i] = roles[i].Name;
                }

                return result;
            }

            return new string[0];
        }

        /// <summary>
        /// Takes, as input, a user name and returns the names of the roles to which the user belongs.
        /// If the user is not assigned to any roles, GetRolesForUser returns an empty string array
        /// (a string array with no elements).
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="userId">A user id.</param>
        /// <returns>A list of role names.</returns>
        /// <exception cref="ProviderException">
        /// If the user name does not exist, GetRolesForUser throws a ProviderException.
        /// </exception>
        public override IList<RainbowRole> GetRolesForUser(string portalAlias, Guid userId)
        {
            IList<RainbowRole> result = new List<RainbowRole>();

            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_UsersInRoles_GetRolesForUser", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

            var returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCode.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var role = GetRoleFromReader(reader);

                        result.Add(role);
                    }

                    reader.Close();
                    if (((int)returnCode.Value) == 1)
                    {
                        throw new RainbowRoleProviderException("User doesn't exist");
                    }
                }

                return result;
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllRoles");
                }

                throw new RainbowRoleProviderException("Error executing aspnet_Roles_GetAllRoles stored proc", e);
            }
            catch (RainbowRoleProviderException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllRoles");
                }

                throw new RainbowRoleProviderException("Error getting roles for user " + userId, e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role to get the list of users for.
        /// </param>
        /// <returns>
        /// A string array containing the names of all the users who are members of the specified role for the configured applicationName.
        /// </returns>
        public override string[] GetUsersInRole(string roleName)
        {
            var role = this.GetRoleByName(this.ApplicationName, roleName);
            return this.GetUsersInRole(this.ApplicationName, role.Id);
        }

        /// <summary>
        /// Takes, as input, a role id and returns the ids of all users assigned to that role.
        /// If no users are associated with the specified role, GetUserInRole returns an empty string
        /// array (a string array with no elements).
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleId">A role id.</param>
        /// <returns>A list of user names.</returns>
        /// <exception cref="ProviderException">
        /// If the role does not exist, GetUsersInRole throws a ProviderException.
        /// </exception>
        public override string[] GetUsersInRole(string portalAlias, Guid roleId)
        {
            var result = new ArrayList();

            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_UsersInRoles_GetUsersInRoles", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = roleId;

            var returnCode = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCode.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try
            {
                cmd.Connection.Open();

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }

                    reader.Close();
                    if (((int)returnCode.Value) == 1)
                    {
                        throw new RainbowRoleProviderException("Role doesn't exist");
                    }
                }

                return (string[])result.ToArray(typeof(string));
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllRoles");
                }

                throw new RainbowRoleProviderException(
                    "Error executing aspnet_UsersInRoles_GetUsersInRoles stored proc", e);
            }
            catch (Exception e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllRoles");
                }

                throw new RainbowRoleProviderException("Error getting users for role " + roleId, e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">
        /// The friendly name of the provider.
        /// </param>
        /// <param name="config">
        /// A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The name of the provider is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The name of the provider has a length of zero.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.
        /// </exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "RainbowSqlRoleProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Rainbow Sql Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            this.ApplicationName = GetConfigValue(
                config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            this.WriteExceptionsToEventLog =
                Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            //// Initialize SqlConnection.
            // ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            // if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim().Equals(string.Empty))
            // {
            // throw new RainbowRoleProviderException("Connection string cannot be blank.");
            // }

            // connectionString = ConnectionStringSettings.ConnectionString;
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="username">
        /// The user name to search for.
        /// </param>
        /// <param name="roleName">
        /// The role to search in.
        /// </param>
        /// <returns>
        /// true if the specified user is in the specified role for the configured applicationName; otherwise, false.
        /// </returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            var user = (RainbowUser)Membership.GetUser(username);

            if (user == null)
            {
                throw new RainbowRoleProviderException("User doesn't exist");
            }

            var role = this.GetRoleByName(this.ApplicationName, roleName);

            return this.IsUserInRole(this.ApplicationName, user.ProviderUserKey, role.Id);
        }

        /// <summary>
        /// Takes, as input, a user id and a role id and determines whether the specified user is associated with the specified role.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="userId">The User id.</param>
        /// <param name="roleId">The Role id.</param>
        /// <returns>
        /// <code>
        /// true
        /// </code>
        /// if the specified user is associated with the specified role,
        /// <code>
        /// false
        /// </code>
        /// otherwise
        /// </returns>
        /// <exception cref="ProviderException">
        /// If the user or role does not exist, IsUserInRole throws a ProviderException.
        /// </exception>
        public override bool IsUserInRole(string portalAlias, Guid userId, Guid roleId)
        {
            if (roleId.Equals(AllUsersGuid) || roleId.Equals(AuthenticatedUsersGuid))
            {
                return true;
            }

            if (roleId.Equals(UnauthenticatedUsersGuid))
            {
                return false;
            }

            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_UsersInRoles_IsUserInRole", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = roleId;
            cmd.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;

            var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                var returnCode = (int)returnCodeParam.Value;

                switch (returnCode)
                {
                    case 0:
                        return false;
                    case 1:
                        return true;
                    case 2:
                        throw new RainbowRoleProviderException(
                            string.Format("User with Id = {0} does not exist", userId));
                    case 3:
                        throw new RainbowRoleProviderException(
                            string.Format("Role with Id = {0} does not exist", roleId));
                    default:
                        throw new RainbowRoleProviderException("Error executing IsUserInRole");
                }
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "IsUserInRole");
                }

                throw new RainbowRoleProviderException(
                    "Error executing aspnet_UsersInRoles_IsUserInRole stored proc", e);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">
        /// A string array of user names to be removed from the specified roles.
        /// </param>
        /// <param name="roleNames">
        /// A string array of role names to remove the specified user names from.
        /// </param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var userIds = new Guid[usernames.Length];
            var roleIds = new Guid[roleNames.Length];

            RainbowUser user;
            for (var i = 0; i < usernames.Length; i++)
            {
                user = (RainbowUser)Membership.GetUser(usernames[i]);

                if (user == null)
                {
                    throw new RainbowMembershipProviderException(string.Format("User {0} doesn't exist", usernames[i]));
                }

                userIds[i] = user.ProviderUserKey;
            }

            RainbowRole role;
            for (var i = 0; i < roleNames.Length; i++)
            {
                role = this.GetRoleByName(this.ApplicationName, roleNames[i]);
                roleIds[i] = role.Id;
            }

            this.RemoveUsersFromRoles(this.ApplicationName, userIds, roleIds);
        }

        /// <summary>
        /// The remove users from roles.
        /// </summary>
        /// <param name="portalAlias">
        /// The portal alias.
        /// </param>
        /// <param name="userIds">
        /// The user ids.
        /// </param>
        /// <param name="roleIds">
        /// The role ids.
        /// </param>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        /// <exception cref="RainbowMembershipProviderException">
        /// </exception>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        /// <exception cref="RainbowRoleProviderException">
        /// </exception>
        public override void RemoveUsersFromRoles(string portalAlias, Guid[] userIds, Guid[] roleIds)
        {
            var userIdsStr = string.Empty;
            var roleIdsStr = string.Empty;

            userIdsStr = userIds.Aggregate(userIdsStr, (current, userId) => current + (userId + ","));

            userIdsStr = userIdsStr.Substring(0, userIdsStr.Length - 1);

            roleIdsStr = roleIds.Aggregate(roleIdsStr, (current, roleId) => current + (roleId + ","));

            roleIdsStr = roleIdsStr.Substring(0, roleIdsStr.Length - 1);

            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_UsersInRoles_RemoveUsersFromRoles", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@UserIds", SqlDbType.VarChar, 4000).Value = userIdsStr;
            cmd.Parameters.Add("@RoleIds", SqlDbType.VarChar, 4000).Value = roleIdsStr;

            var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                var returnCode = (int)returnCodeParam.Value;

                switch (returnCode)
                {
                    case 0:
                        return;
                    case 1:
                        throw new RainbowRoleProviderException("One of the users is not in one of the specified roles");
                    case 2:
                        throw new RainbowRoleProviderException(
                            string.Format("Application {0} doesn't exist", portalAlias));
                    case 3:
                        throw new RainbowRoleProviderException("One of the roles doesn't exist");
                    case 4:
                        throw new RainbowMembershipProviderException("One of the users doesn't exist");
                    default:
                        throw new RainbowRoleProviderException(
                            string.Format("aspnet_UsersInRoles_RemoveUsersToRoles returned error code {0}", returnCode));
                }
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateRole");
                }

                throw new RainbowRoleProviderException(
                    "Error executing aspnet_UsersInRoles_RemoveUsersToRoles stored proc", e);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Takes as an input a role Id and a role name and updates the role's name identified by roleId.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleId">The role id</param>
        /// <param name="newRoleName">The new role name</param>
        /// <returns>
        /// <code>
        /// true
        /// </code>
        /// if the update was succeful, or
        /// <code>
        /// false
        /// </code>
        /// if the role was not found
        /// </returns>
        /// <exception cref="RainbowRoleProviderException">
        /// RenameRole throws an exception if there was an unexpected error
        /// renaming the role.
        /// </exception>
        public override bool RenameRole(string portalAlias, Guid roleId, string newRoleName)
        {
            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_rbRoles_RenameRole", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = roleId;
            cmd.Parameters.Add("@NewRoleName", SqlDbType.VarChar, 256).Value = newRoleName;

            var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                var returnCode = (int)returnCodeParam.Value;

                switch (returnCode)
                {
                    case 0:
                        return true;
                    case 1:
                        throw new RainbowRoleProviderException("Role doesn't exist");
                    default:
                        throw new RainbowRoleProviderException("Error renaming role");
                }
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteRole");
                }

                throw new RainbowRoleProviderException("Error executing aspnet_rbRoles_RenameRole stored proc", e);
            }
            catch (Exception e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RenameRole");
                }

                throw new RainbowRoleProviderException(string.Format("Error renaming role {0}", roleId), e);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role to search for in the data source.
        /// </param>
        /// <returns>
        /// true if the role name already exists in the data source for the configured applicationName; otherwise, false.
        /// </returns>
        public override bool RoleExists(string roleName)
        {
            try
            {
                var allRoles = this.GetAllRoles(this.ApplicationName);
                return allRoles.Any(role => role.Name.Equals(roleName));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Takes, as input, a role name and determines whether the role exists.
        /// </summary>
        /// <param name="portalAlias">Rainbow's portal alias</param>
        /// <param name="roleId">A role id.</param>
        /// <returns>Whether the specified role exists or not</returns>
        public override bool RoleExists(string portalAlias, Guid roleId)
        {
            var cmd = new SqlCommand
                {
                    CommandText = "aspnet_Roles_RoleExists", 
                    CommandType = CommandType.StoredProcedure, 
                    Connection = new SqlConnection(this.ConnectionString)
                };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = portalAlias;
            cmd.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = roleId;

            var returnCodeParam = cmd.Parameters.Add("@ReturnCode", SqlDbType.Int);
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                var returnCode = (int)returnCodeParam.Value;
                return returnCode == 1;
            }
            catch (SqlException e)
            {
                if (this.WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RoleExists");
                }

                throw new RainbowRoleProviderException("Error executing aspnet_Roles_RoleExists stored proc", e);
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A helper function that writes exception detail to the event log. Exceptions are written to the event log as a security
        ///     measure to avoid private database details from being returned to the browser. If a method does not return a status
        ///     or boolean indicating the action succeeded or failed, a generic exception is also thrown by the caller.
        /// </summary>
        /// <param name="e">
        /// The exception.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private static void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog { Source = EventSource, Log = EventLog };

            var message = new StringBuilder();
            message.Append("An exception occurred communicating with the data source.\n\n");
            message.AppendFormat("Action: {0}\n\n", action);
            message.AppendFormat("Exception: {0}", e);

            log.WriteEntry(message.ToString());
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file.
        /// </summary>
        /// <param name="configValue">The config value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The get config value.</returns>
        private static string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        /// <summary>
        /// The get role from reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A RainbowRole.
        /// </returns>
        private static RainbowRole GetRoleFromReader(SqlDataReader reader)
        {
            var roleId = reader.GetGuid(0);
            var roleName = reader.GetString(1);

            var roleDescription = string.Empty;
            if (!reader.IsDBNull(2))
            {
                roleDescription = reader.GetString(2);
            }

            return new RainbowRole(roleId, roleName, roleDescription);
        }

        #endregion
    }
}