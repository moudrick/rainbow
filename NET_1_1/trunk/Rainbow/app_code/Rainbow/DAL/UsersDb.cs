using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using Rainbow.BLL.User;
using Rainbow.BLL.UserConfig;
using Rainbow.BLL.Utils;
using Rainbow.Configuration;
using Rainbow.Helpers;
using Rainbow.Settings;

namespace Rainbow.Security
{

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
	[History("jminond", "2005/03/10", "Tab to page conversion")]
	[History("gman3001", "2004/09/29", "Added the UpdateLastVisit method to update the user's last visit date indicator.")]
	public class UsersDB 
	{
		CryptoHelper cryphelp = null;
		/// <summary>
		/// AddRole() Method <a name="AddRole"></a>
		///
		/// The AddRole method creates a new security role for the specified portal,
		/// and returns the new RoleID value.
		///
		/// Other relevant sources:
		///     + <a href="AddRole.htm" style="color:green">AddRole Stored Procedure</a>
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="roleName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		public int AddRole(int portalID, string roleName) 
		{
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_AddRole", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterRoleName = new SqlParameter("@RoleName", SqlDbType.NVarChar, 50);
					parameterRoleName.Value = roleName;
					myCommand.Parameters.Add(parameterRoleName);
					SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
					parameterRoleID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterRoleID);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					finally
					{
					}
					// return the role id 
					return (int) parameterRoleID.Value;
				}
			}
		}

		/// <summary>
		/// AddUser
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="name"></param>
		/// <param name="company"></param>
		/// <param name="address"></param>
		/// <param name="city"></param>
		/// <param name="zip"></param>
		/// <param name="countryID"></param>
		/// <param name="stateID"></param>
		/// <param name="pIva"></param>
		/// <param name="cFiscale"></param>
		/// <param name="phone"></param>
		/// <param name="fax"></param>
		/// <param name="password"></param>
		/// <param name="email"></param>
		/// <param name="sendNewsletter"></param>
		/// <returns>The newly created ID</returns>
		public int AddUser(int portalID, string name, string company, string address, string city, string zip, string countryID, int stateID, string pIva, string cFiscale, string phone, string fax, string password, string email, bool sendNewsletter)
		{
			cryphelp = new CryptoHelper();
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_AddUserFull", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterCompany = new SqlParameter("@Company", SqlDbType.NVarChar, 50);
					parameterCompany.Value = company;
					myCommand.Parameters.Add(parameterCompany);
					SqlParameter parameterAddress = new SqlParameter("@Address", SqlDbType.NVarChar, 50);
					parameterAddress.Value = address;
					myCommand.Parameters.Add(parameterAddress);
					SqlParameter parameterCity = new SqlParameter("@City", SqlDbType.NVarChar, 50);
					parameterCity.Value = city;
					myCommand.Parameters.Add(parameterCity);
					SqlParameter parameterZip = new SqlParameter("@Zip", SqlDbType.NVarChar, 6);
					parameterZip.Value = zip;
					myCommand.Parameters.Add(parameterZip);
					SqlParameter parameterCountryID = new SqlParameter("@CountryID", SqlDbType.NChar, 2);
					parameterCountryID.Value = countryID;
					myCommand.Parameters.Add(parameterCountryID);
					SqlParameter parameterStateID = new SqlParameter("@StateID", SqlDbType.Int);
					parameterStateID.Value = stateID;
					myCommand.Parameters.Add(parameterStateID);
					SqlParameter parameterPIva = new SqlParameter("@PIva", SqlDbType.NVarChar, 11);
					parameterPIva.Value = pIva;
					myCommand.Parameters.Add(parameterPIva);
					SqlParameter parameterCFiscale = new SqlParameter("@CFiscale", SqlDbType.NVarChar, 16);
					parameterCFiscale.Value = cFiscale;
					myCommand.Parameters.Add(parameterCFiscale);
					SqlParameter parameterPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
					parameterPhone.Value = phone;
					myCommand.Parameters.Add(parameterPhone);
					SqlParameter parameterFax = new SqlParameter("@Fax", SqlDbType.NVarChar, 50);
					parameterFax.Value = fax;
					myCommand.Parameters.Add(parameterFax);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if(Config.EncryptPassword) 
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					else 
					{
						parameterSalt.Value = "";
						parameterPassword.Value = password;
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterSendNewsletter = new SqlParameter("@SendNewsletter", SqlDbType.Bit);
					parameterSendNewsletter.Value = sendNewsletter;
					myCommand.Parameters.Add(parameterSendNewsletter);

					myConnection.Open(); 
					try 
					{ 
						myCommand.ExecuteNonQuery(); 
					} 
					catch 
					{ 
						// failed to create a new user 
						throw;
					} 
					// Return the newly created ID
					return (int)parameterUserID.Value; 
				}
			}
		}

		/// <summary>
		/// UsersDB.AddUser() Method <a name="AddUser"></a>
		///
		/// The AddUser method inserts a new user record into the "Users" database table.
		///
		/// Other relevant sources:
		///     + <a href="AddUser.htm" style="color:green">AddUser Stored Procedure</a>
		/// </summary>
		/// <param name="fullName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="email" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="password" type="string">
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
		///     A int value...
		/// </returns>
		public int AddUser(string fullName, string email, string password, int portalID) 
		{
			cryphelp = new CryptoHelper();
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_AddUser", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterFullName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterFullName.Value = fullName;
					myCommand.Parameters.Add(parameterFullName);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if(Config.EncryptPassword) 
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					else 
					{
						parameterSalt.Value = "";
						parameterPassword.Value = password;
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterUserID);

					myConnection.Open();
					try 
					{
						myCommand.ExecuteNonQuery();
					}
					finally 
					{
					}
					return (int) parameterUserID.Value;
				}
			}
		}

		/// <summary>
		/// AddUserRole() Method <a name="AddUserRole"></a>
		/// The AddUserRole method adds the user to the specified security role.
		/// Other relevant sources:
		///     + <a href="AddUserRole.htm" style="color:green">AddUserRole Stored Procedure</a>
		/// </summary>
		/// <param name="roleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="userID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void AddUserRole(int roleID, int userID) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_AddUserRole", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
					parameterRoleID.Value = roleID;
					myCommand.Parameters.Add(parameterRoleID);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					finally
					{
					}
				}
			}
		}

		/// <summary>
		/// DeleteRole() Method <a name="DeleteRole"></a>
		/// The DeleteRole deletes the specified role from the portal database.
		/// Other relevant sources:
		///     + <a href="DeleteRole.htm" style="color:green">DeleteRole Stored Procedure</a>
		/// </summary>
		/// <param name="roleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void DeleteRole(int roleID) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_DeleteRole", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
					parameterRoleID.Value = roleID;
					myCommand.Parameters.Add(parameterRoleID);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					catch 
					{
						throw new Exception("Role with Users assigned");
					}
				}
			}
		}

		/// <summary>
		/// The DeleteUser method deleted a  user record from the "Users" database table.
		/// </summary>
		/// <param name="userID"></param>
		public void DeleteUser(int userID) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_DeleteUser", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					finally
					{
					}
				}
			}
		}

		/// <summary>
		/// DeleteUserRole() Method <a name="DeleteUserRole"></a>
		/// The DeleteUserRole method deletes the user from the specified role.
		/// Other relevant sources:
		///     + <a href="DeleteUserRole.htm" style="color:green">DeleteUserRole Stored Procedure</a>
		/// </summary>
		/// <param name="roleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="userID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void DeleteUserRole(int roleID, int userID) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_DeleteUserRole", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
					parameterRoleID.Value = roleID;
					myCommand.Parameters.Add(parameterRoleID);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					// Open the database connection and execute the command
					myConnection.Open();

					try
					{
						myCommand.ExecuteNonQuery();
					}
					finally
					{
					}
				}
			}
		}

		/// <summary>
		/// Get a full user for store on authentication cookie
		/// </summary>
		/// <param name="email"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public User GetCurrentUser(string email, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			string name = "unknown";
			string userID = "0";
			using (SqlDataReader dr = GetSingleUser(email, portalID))
			{
				if (dr.Read())
				{
					userID = dr["userID"].ToString();
					name = dr["Name"].ToString();
					email = dr["Email"].ToString();
				}
				else
				{
					throw new Exception("User not found");
				}
			}
			return new User(name, email, userID);
		}

		/// <summary>
		/// Get Current UserID
		/// </summary>
		/// <param name="UserEmail"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public int GetCurrentUserID(string UserEmail, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			int userID = 0;

			using (SqlDataReader drUsers = GetSingleUser(UserEmail, portalID))
			{
				if (drUsers.Read())
					userID = Int32.Parse(drUsers["UserID"].ToString());
			}
			return userID;
		}

		/// <summary>
		/// GetPortalRoles() Method <a name="GetPortalRoles"></a>
		///
		/// The GetPortalRoles method returns a list of all role names for the 
		/// specified portal.
		///
		/// Other relevant sources:
		///     + <a href="GetRolesByUser.htm" style="color:green">GetPortalRoles Stored Procedure</a>
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetPortalRoles(int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetPortalRoles", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// GetRoleMembers() Method <a name="GetRoleMembers"></a>
		///
		/// The GetRoleMembers method returns a list of all members in the specified
		/// security role.
		///
		/// Other relevant sources:
		///     + <a href="GetRoleMembers.htm" style="color:green">GetRoleMembers Stored Procedure</a>
		/// </summary>
		/// <param name="roleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetRoleMembers(int roleID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetRoleMembership", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
			parameterRoleID.Value = roleID;
			myCommand.Parameters.Add(parameterRoleID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// This Method Is Used To Retrieve All Users Who Do Not Belong To A Particular Role
		/// </summary>
		/// <param name="roleID"></param>
		/// <param name="portalID"></param>
		/// <returns>A SqlDataReader</returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetRoleNonMembers(int roleID, int portalID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetRoleNonMembership", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
			parameterRoleID.Value = roleID;
			myCommand.Parameters.Add(parameterRoleID);
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);

			if (Config.UseSingleUserBase)
			{
				parameterPortalID.Value = 0;
			}

			else
			{
				parameterPortalID.Value = portalID;
			}
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// This Method Is Used To Retrieve All Users Who Do Not Belong To A Particular Role But 
		/// Belong To A Second Role.
		/// </summary>
		/// <param name="roleID"></param>
		/// <param name="portalID"></param>
		/// <param name="filterRoleID"></param>
		/// <returns>A SqlDataReader</returns>
		// Added by John Mandia (www.whitelightsolutions.com) 01/09/04 (Note did not add a stored procedure to sql for this method as it's uses are limited and we already have a lot of stored procedures in the rainbow database)
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetRoleNonMembersFiltered(int roleID, int portalID,int filterRoleID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			// Set up a command with the given query and associate
			// this with the current connection.
			string commandText = "SELECT rb_UserRoles.UserID, Name, Email FROM rb_UserRoles INNER JOIN rb_Users On rb_Users.UserID = rb_UserRoles.UserID WHERE rb_UserRoles.RoleID = @RoleIDFilter AND PortalID = @PortalID AND rb_Users.UserID  NOT IN (SELECT UserID FROM rb_UserRoles WHERE rb_UserRoles.RoleID = @RoleID)";
			SqlCommand myCommand = new SqlCommand(commandText,myConnection);
			SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
			parameterRoleID.Value = roleID;
			myCommand.Parameters.Add(parameterRoleID);
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);

			if (Config.UseSingleUserBase)
			{
				parameterPortalID.Value = 0;
			}
			else
			{
				parameterPortalID.Value = portalID;
			}
			myCommand.Parameters.Add(parameterPortalID);
			SqlParameter parameterFilterRoleID = new SqlParameter("@RoleIDFilter", SqlDbType.Int, 4);
			parameterFilterRoleID.Value = filterRoleID;
			myCommand.Parameters.Add(parameterFilterRoleID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// GetRoles() Method <a name="GetRoles"></a>
		///
		/// The GetRoles method returns a list of role names for the user.
		///
		/// Other relevant sources:
		///     + <a href="GetRolesByUser.htm" style="color:green">GetRolesByUser Stored Procedure</a>
		/// </summary>
		/// <param name="email" type="string">
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
		public string[] GetRoles(string email, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetRolesByUser", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					// Open the database connection and execute the command
					myConnection.Open();
					ArrayList userRoles = new ArrayList();
					using (SqlDataReader dr = myCommand.ExecuteReader())
					{
						while (dr.Read()) 
							userRoles.Add(dr["RoleName"]);
					}
					// Return the string array of roles
					return (string[]) userRoles.ToArray(typeof(string));
				}
			}
		}

		/// <summary>
		/// UsersDB.GetRolesByUser() Method <a name="GetRolesByUser"></a>
		///
		/// The DeleteUser method deleted a  user record from the "Users" database table.
		///
		/// Other relevant sources:
		///     + <a href="GetRolesByUser.htm" style="color:green">GetRolesByUser Stored Procedure</a>
		/// </summary>
		/// <param name="email" type="string">
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
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetRolesByUser(string email, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetRolesByUser", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
			parameterEmail.Value = email;
			myCommand.Parameters.Add(parameterEmail);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// GetSingleUser Method
		///
		/// The GetSingleUser method returns a SqlDataReader containing details
		/// about a specific user from the Users database table.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetSingleUser(string email, int portalID, string IDLang) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSingleUser", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
			parameterEmail.Value = email;
			myCommand.Parameters.Add(parameterEmail);
			SqlParameter parameterIDLang = new SqlParameter("@IDLang", SqlDbType.NChar, 2);
			parameterIDLang.Value = IDLang;
			myCommand.Parameters.Add(parameterIDLang);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// GetSingleUser Method
		///
		/// The GetSingleUser method returns a SqlDataReader containing details
		/// about a specific user from the Users database table.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetSingleUser(string email, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSingleUser", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
			parameterEmail.Value = email;
			myCommand.Parameters.Add(parameterEmail);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// UsersDB.GetUncheckedUsers() Method <a name="GetUsers"></a>
		///
		/// Get only the records with CheckMail disabled from the "Users" database table.
		/// To avoid troubles MailCheck cannot check more than 10 users at time
		///
		/// Other relevant sources:
		///     + <a href="GetUncheckedUsers.htm" style="color:green">GetUncheckedUsers Stored Procedure</a>
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetUncheckedUsers(int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetUncheckedUsers", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		///  UsersDB.GetUsers() Method <a name="GetUsers"></a>
		///  Get record from the "Users" database table.
		/// </summary>
		/// <param name="portalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete(""FIX ME:Should return a collection of business layer types UserItem, not a dataset.")]
		public DataSet GetUsers(int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			DataSet myDataSet = new DataSet();

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlDataAdapter myCommand = new SqlDataAdapter("rb_GetUsers", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.SelectCommand.Parameters.Add(parameterPortalID);

					myCommand.Fill(myDataSet);
				}
			}
			return myDataSet;
		}

		/// <summary>
		/// The GetUsersCount method returns the users count.
		/// Uses GetUsersCount Stored Procedure.
		/// </summary>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public int GetUsersCount(int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			int retValue = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetUsersCount", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterUsersCount = new SqlParameter("@UsersCount", SqlDbType.Int, 4);
					parameterUsersCount.Direction = ParameterDirection.Output;
					myCommand.Parameters.Add(parameterUsersCount);
					// Open the database connection and execute the command
					myConnection.Open();

					myCommand.ExecuteNonQuery();
					retValue = Convert.ToInt32(parameterUsersCount.Value);
				}
			}
			return retValue;
		}

		/// <summary>
		/// UsersDB.GetUsersNoPassword() Method <a name="GetUsers"></a>
		///
		/// Get only the records with SendNewsletter enabled from the "Users" database table.
		///
		/// Other relevant sources:
		///     + <a href="GetUsersNoPassword.htm" style="color:green">GetUsersNoPassword Stored Procedure</a>
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetUsersNoPassword(int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetUsersNoPassword", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		/// This Method Is Used To Retrieve All Users Who Do Not Belong To A Role
		/// </summary>
		/// <returns>A SqlDataReader</returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetUsersNoRole(int portalID) 
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetUsersNoRole", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);

			if (Config.UseSingleUserBase)
			{
				parameterPortalID.Value = 0;
			}
			else
			{
				parameterPortalID.Value = portalID;
			}
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="portalID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Data.SqlClient.SqlDataReader value...
		/// </returns>
		// TODO --> [Obsolete("Replace me")]
		public SqlDataReader GetUsersReader(int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetUsers", myConnection);
			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;
			// Add Parameters to SPROC
			SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
			parameterPortalID.Value = portalID;
			myCommand.Parameters.Add(parameterPortalID);
			// Open the database connection and execute the command
			myConnection.Open();
			SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
			// Return the datareader
			return dr;
		}
		/// <summary>
		/// UsersDB.Login() Method.
		/// The Login method validates a id/password hash pair against credentials
		/// stored in the users database.  If the id/password hash pair is valid,
		/// the method returns user's name.
		/// </summary>
		/// <remarks>UserLogin Stored Procedure</remarks>
		/// <param name="uid"></param>
		/// <param name="password"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public User Login(int uid, string password, int portalID) 
		{
			cryphelp = new CryptoHelper();
			bool passwordMatch = false;
			string dbPassword;
			if (Config.UseSingleUserBase) portalID = 0;
			User myUser = null; 
			bool login = Config.EncryptPassword;
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UserLoginByID", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterEmail = new SqlParameter("@UserID", SqlDbType.NVarChar, 100);
					parameterEmail.Value = uid;
					myCommand.Parameters.Add(parameterEmail);
					// Open the database connection and execute the command
					SqlDataReader dr;
					myConnection.Open();
					dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
					dr.Read();
					
					try 
					{
						dbPassword = dr["Password"].ToString();
					}
					catch(Exception) 
					{
						return myUser;
					}
					
					if(dbPassword.Length != 40 && Config.EncryptPassword) 
					{
						cryphelp.HashPasswords();
						string salt = dr["Salt"].ToString();
						string passwordAndSalt = String.Concat(password, salt);
						string passwordAndSaltDB = String.Concat(dbPassword, salt);
						passwordAndSalt = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSalt, "SHA1");
						passwordAndSaltDB = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSaltDB, "SHA1");
						passwordMatch = passwordAndSalt.Equals(passwordAndSaltDB);
						if(passwordMatch)
							myUser = new User(dr["Name"].ToString(), dr["Email"].ToString(), dr["UserID"].ToString());
					}
					else if(dbPassword.Length == 40)
					{
						string salt = dr["Salt"].ToString();
						string passwordAndSalt = String.Concat(password, salt);
						passwordAndSalt = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSalt, "SHA1");
						passwordMatch = passwordAndSalt.Equals(dbPassword);
						if(passwordMatch)
							myUser = new User(dr["Name"].ToString(), dr["Email"].ToString(), dr["UserID"].ToString());
					}
					else if(dbPassword.Length != 40 && Config.EncryptPassword == false) 
					{
						myUser = LoginPlainByID(uid, password, portalID);
					}
				}
			}
			return myUser;
		}
		/// <summary>
		/// UsersDB.Login() Method.
		/// The Login method validates a email/password hash pair against credentials
		/// stored in the users database.  If the email/password hash pair is valid,
		/// the method returns user's name.
		/// </summary>
		/// <remarks>UserLogin Stored Procedure</remarks>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public User Login(string email, string password, int portalID) 
		{
			cryphelp = new CryptoHelper();
			bool passwordMatch = false;
			string dbPassword;
			if (Config.UseSingleUserBase) portalID = 0;
			User myUser = null; 
			
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UserLogin", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					// Open the database connection and execute the command
					SqlDataReader dr;
					//SqlDataReader drCount;
					myConnection.Open();
					dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
					dr.Read();
					try 
					{
						dbPassword = dr["Password"].ToString();
					}
					catch(Exception) 
					{
						return myUser;
					}
					if(dbPassword.Length != 40 && Config.EncryptPassword) 
					{
						cryphelp.HashPasswords();
						string salt = dr["Salt"].ToString();
						string passwordAndSalt = String.Concat(password, salt);
						string passwordAndSaltDB = String.Concat(dbPassword, salt);
						passwordAndSalt = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSalt, "SHA1");
						passwordAndSaltDB = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSaltDB, "SHA1");
						passwordMatch = passwordAndSalt.Equals(passwordAndSaltDB);
						if(passwordMatch)
						{
							myUser = new User(dr["Name"].ToString(), dr["Email"].ToString(), dr["UserID"].ToString());
						}
					}
					else if(dbPassword.Length == 40)
					{
						string salt = dr["Salt"].ToString();
						string passwordAndSalt = String.Concat(password, salt);
						passwordAndSalt = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSalt, "SHA1");
						passwordMatch = passwordAndSalt.Equals(dbPassword);
						if(passwordMatch)
							myUser = new User(dr["Name"].ToString(), dr["Email"].ToString(), dr["UserID"].ToString());
					}
					else if(dbPassword.Length != 40 && Config.EncryptPassword == false) 
					{
						myUser = LoginPlain(email, password, portalID);
					}
				}
			}
			return myUser;
		}
		/// <summary>
		/// UsersDB.Login() Method.
		/// The Login method validates a id/password pair against credentials
		/// stored in the users database.  If the id/password pair is valid,
		/// the method returns user's name.
		/// </summary>
		/// <remarks>UserLogin Stored Procedure</remarks>
		/// <param name="uid"></param>
		/// <param name="password"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public User LoginPlainByID(int uid, string password, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			User myUser = null; 

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UserLoginPlainByID", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = uid;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 20);
					parameterPassword.Value = password;
					myCommand.Parameters.Add(parameterPassword);
					// Open the database connection and execute the command
					SqlDataReader dr;
					myConnection.Open();
					dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

					if (dr.Read()) 
						myUser = new User(dr["Name"].ToString(), dr["Email"].ToString(), dr["UserID"].ToString());
				}
			}
			return myUser;
		}
		/// <summary>
		/// UsersDB.Login() Method.
		/// The Login method validates a email/password pair against credentials
		/// stored in the users database.  If the email/password pair is valid,
		/// the method returns user's name.
		/// </summary>
		/// <remarks>UserLogin Stored Procedure</remarks>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public User LoginPlain(string email, string password, int portalID) 
		{
			if (Config.UseSingleUserBase) portalID = 0;
			User myUser = null; 

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UserLoginPlain", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 20);
					parameterPassword.Value = password;
					myCommand.Parameters.Add(parameterPassword);
					// Open the database connection and execute the command
					SqlDataReader dr;
					myConnection.Open();
					dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

					if (dr.Read()) 
						myUser = new User(dr["Name"].ToString(), dr["Email"].ToString(), dr["UserID"].ToString());
				}
			}
			return myUser;
		}

		public bool VerifyPassword(SqlDataReader reader) 
		{
			bool passwordMatch = false;
			reader.Read();
			string dbPassword = reader.GetString(13);
			string salt = reader.GetString(14);
			reader.Close();
			string passwordAndSalt = String.Concat(dbPassword, salt);
			passwordAndSalt = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordAndSalt, "SHA1");
			passwordMatch = passwordAndSalt.Equals(dbPassword);
			return passwordMatch;
		}

		// Added by gman3001 9/29/2004
		/// <summary>
		/// UpdateLastVisit Method
		///
		/// The UpdateLastVisit method updates the specified user's last visit date in the Users database table.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public void UpdateLastVisit(string email, int portalID)
		{
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateLastVisit", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					// Open the database connection and execute the command
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// UpdateRole() Method <a name="UpdateRole"></a>
		///
		/// The UpdateRole method updates the friendly name of the specified role.
		///
		/// Other relevant sources:
		///     + <a href="UpdateRole.htm" style="color:green">UpdateRole Stored Procedure</a>
		/// </summary>
		/// <param name="roleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="roleName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public void UpdateRole(int roleID, string roleName) 
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateRole", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterRoleID = new SqlParameter("@RoleID", SqlDbType.Int, 4);
					parameterRoleID.Value = roleID;
					myCommand.Parameters.Add(parameterRoleID);
					SqlParameter parameterRoleName = new SqlParameter("@RoleName", SqlDbType.NVarChar, 50);
					parameterRoleName.Value = roleName;
					myCommand.Parameters.Add(parameterRoleName);
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// UpdateUser
		/// This overload allow to change identity of the user
		/// </summary>
		/// <param name="oldUserID"></param>
		/// <param name="userID"></param>
		/// <param name="portalID"></param>
		/// <param name="name"></param>
		/// <param name="company"></param>
		/// <param name="address"></param>
		/// <param name="city"></param>
		/// <param name="zip"></param>
		/// <param name="countryID"></param>
		/// <param name="stateID"></param>
		/// <param name="pIva"></param>
		/// <param name="cFiscale"></param>
		/// <param name="phone"></param>
		/// <param name="fax"></param>
		/// <param name="password"></param>
		/// <param name="email"></param>
		/// <param name="sendNewsletter"></param>
		public void UpdateUser(int oldUserID, int userID, int portalID, string name, string company, string address, string city, string zip, string countryID, int stateID, string pIva, string cFiscale, string phone, string fax, string password, string email, bool sendNewsletter)
		{
			cryphelp = new CryptoHelper();
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserFull", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameteroldUserID = new SqlParameter("@OldUserID", SqlDbType.Int);
					parameteroldUserID.Value = oldUserID;
					myCommand.Parameters.Add(parameteroldUserID);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterCompany = new SqlParameter("@Company", SqlDbType.NVarChar, 50);
					parameterCompany.Value = company;
					myCommand.Parameters.Add(parameterCompany);
					SqlParameter parameterAddress = new SqlParameter("@Address", SqlDbType.NVarChar, 50);
					parameterAddress.Value = address;
					myCommand.Parameters.Add(parameterAddress);
					SqlParameter parameterCity = new SqlParameter("@City", SqlDbType.NVarChar, 50);
					parameterCity.Value = city;
					myCommand.Parameters.Add(parameterCity);
					SqlParameter parameterZip = new SqlParameter("@Zip", SqlDbType.NVarChar, 6);
					parameterZip.Value = zip;
					myCommand.Parameters.Add(parameterZip);
					SqlParameter parameterCountryID = new SqlParameter("@CountryID", SqlDbType.NChar, 2);
					parameterCountryID.Value = countryID;
					myCommand.Parameters.Add(parameterCountryID);
					SqlParameter parameterStateID = new SqlParameter("@StateID", SqlDbType.Int);
					parameterStateID.Value = stateID;
					myCommand.Parameters.Add(parameterStateID);
					SqlParameter parameterPIva = new SqlParameter("@PIva", SqlDbType.NVarChar, 11);
					parameterPIva.Value = pIva;
					myCommand.Parameters.Add(parameterPIva);
					SqlParameter parameterCFiscale = new SqlParameter("@CFiscale", SqlDbType.NVarChar, 16);
					parameterCFiscale.Value = cFiscale;
					myCommand.Parameters.Add(parameterCFiscale);
					SqlParameter parameterPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
					parameterPhone.Value = phone;
					myCommand.Parameters.Add(parameterPhone);
					SqlParameter parameterFax = new SqlParameter("@Fax", SqlDbType.NVarChar, 50);
					parameterFax.Value = fax;
					myCommand.Parameters.Add(parameterFax);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if (Config.EncryptPassword && password != string.Empty)
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					else if (Config.EncryptPassword && password == string.Empty)
					{
						cryphelp = new CryptoHelper();
						password = cryphelp.GetPassword(userID);
						parameterPassword.Value = password.Split(',')[0].ToString();
						parameterSalt.Value = password.Split(',')[1].ToString();
					}
					else
					{
						parameterSalt.Value = "";
						parameterPassword.Value = password;
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterSendNewsletter = new SqlParameter("@SendNewsletter", SqlDbType.Bit);
					parameterSendNewsletter.Value = sendNewsletter;
					myCommand.Parameters.Add(parameterSendNewsletter);

					myConnection.Open();
					// Execute the command in a try/catch to catch duplicate username errors
					try 
					{
						myCommand.ExecuteNonQuery();
					}
					catch 
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// UpdateUser
		/// This overload allow to change identity of the user
		/// </summary>
		/// <param name="oldUserID"></param>
		/// <param name="userID"></param>
		/// <param name="portalID"></param>
		/// <param name="name"></param>
		/// <param name="password"></param>
		/// <param name="email"></param>
		public void UpdateUser(int oldUserID, int userID, int portalID, string name, string password, string email)
		{
			cryphelp = new CryptoHelper();
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserFull", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameteroldUserID = new SqlParameter("@OldUserID", SqlDbType.Int);
					parameteroldUserID.Value = oldUserID;
					myCommand.Parameters.Add(parameteroldUserID);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if(Config.EncryptPassword && password != string.Empty) 
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					else if(Config.EncryptPassword && password == string.Empty)
					{
						cryphelp = new CryptoHelper();
						password = cryphelp.GetPassword(userID);
						parameterPassword.Value = password.Split(',')[0].ToString();
						parameterSalt.Value = password.Split(',')[1].ToString();
					}
					else
					{
						parameterSalt.Value = "";
						parameterPassword.Value = password;
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);

					myConnection.Open();
					// Execute the command in a try/catch to catch duplicate username errors
					try 
					{
						myCommand.ExecuteNonQuery();
					}
					catch 
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// method added by Thierry (tiptopweb) on 5 May 2003
		/// The UpdateUser method inserts a new user record into the "Users" database table.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="portalID"></param>
		/// <param name="name"></param>
		/// <param name="company"></param>
		/// <param name="address"></param>
		/// <param name="city"></param>
		/// <param name="zip"></param>
		/// <param name="countryID"></param>
		/// <param name="stateID"></param>
		/// <param name="pIva"></param>
		/// <param name="cFiscale"></param>
		/// <param name="phone"></param>
		/// <param name="fax"></param>
		/// <param name="email"></param>
		/// <param name="sendNewsletter"></param>
		public void UpdateUser(int userID, int portalID, string name, string company, string address, string city, string zip, string countryID, int stateID, string pIva, string cFiscale, string phone, string fax, string email, bool sendNewsletter)
		{
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserFullNoPassword", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterCompany = new SqlParameter("@Company", SqlDbType.NVarChar, 50);
					parameterCompany.Value = company;
					myCommand.Parameters.Add(parameterCompany);
					SqlParameter parameterAddress = new SqlParameter("@Address", SqlDbType.NVarChar, 50);
					parameterAddress.Value = address;
					myCommand.Parameters.Add(parameterAddress);
					SqlParameter parameterCity = new SqlParameter("@City", SqlDbType.NVarChar, 50);
					parameterCity.Value = city;
					myCommand.Parameters.Add(parameterCity);
					SqlParameter parameterZip = new SqlParameter("@Zip", SqlDbType.NVarChar, 6);
					parameterZip.Value = zip;
					myCommand.Parameters.Add(parameterZip);
					SqlParameter parameterCountryID = new SqlParameter("@CountryID", SqlDbType.NChar, 2);
					parameterCountryID.Value = countryID;
					myCommand.Parameters.Add(parameterCountryID);
					SqlParameter parameterStateID = new SqlParameter("@StateID", SqlDbType.Int);
					parameterStateID.Value = stateID;
					myCommand.Parameters.Add(parameterStateID);
					SqlParameter parameterPIva = new SqlParameter("@PIva", SqlDbType.NVarChar, 11);
					parameterPIva.Value = pIva;
					myCommand.Parameters.Add(parameterPIva);
					SqlParameter parameterCFiscale = new SqlParameter("@CFiscale", SqlDbType.NVarChar, 16);
					parameterCFiscale.Value = cFiscale;
					myCommand.Parameters.Add(parameterCFiscale);
					SqlParameter parameterPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
					parameterPhone.Value = phone;
					myCommand.Parameters.Add(parameterPhone);
					SqlParameter parameterFax = new SqlParameter("@Fax", SqlDbType.NVarChar, 50);
					parameterFax.Value = fax;
					myCommand.Parameters.Add(parameterFax);
					//			SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 20);
					//			parameterPassword.Value = password;
					//			myCommand.Parameters.Add(parameterPassword);
					//
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterSendNewsletter = new SqlParameter("@SendNewsletter", SqlDbType.Bit);
					parameterSendNewsletter.Value = sendNewsletter;
					myCommand.Parameters.Add(parameterSendNewsletter);
					// Execute the command
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// UpdateUser
		/// Autogenerated by CodeWizard 04/04/2003 17.55.40
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="portalID"></param>
		/// <param name="name"></param>
		/// <param name="company"></param>
		/// <param name="address"></param>
		/// <param name="city"></param>
		/// <param name="zip"></param>
		/// <param name="countryID"></param>
		/// <param name="stateID"></param>
		/// <param name="pIva"></param>
		/// <param name="cFiscale"></param>
		/// <param name="phone"></param>
		/// <param name="fax"></param>
		/// <param name="password"></param>
		/// <param name="email"></param>
		/// <param name="sendNewsletter"></param>
		public void UpdateUser(int userID, int portalID, string name, string company, string address, string city, string zip, string countryID, int stateID, string pIva, string cFiscale, string phone, string fax, string password, string email, bool sendNewsletter)
		{
			cryphelp = new CryptoHelper();
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserFull", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterCompany = new SqlParameter("@Company", SqlDbType.NVarChar, 50);
					parameterCompany.Value = company;
					myCommand.Parameters.Add(parameterCompany);
					SqlParameter parameterAddress = new SqlParameter("@Address", SqlDbType.NVarChar, 50);
					parameterAddress.Value = address;
					myCommand.Parameters.Add(parameterAddress);
					SqlParameter parameterCity = new SqlParameter("@City", SqlDbType.NVarChar, 50);
					parameterCity.Value = city;
					myCommand.Parameters.Add(parameterCity);
					SqlParameter parameterZip = new SqlParameter("@Zip", SqlDbType.NVarChar, 6);
					parameterZip.Value = zip;
					myCommand.Parameters.Add(parameterZip);
					SqlParameter parameterCountryID = new SqlParameter("@CountryID", SqlDbType.NChar, 2);
					parameterCountryID.Value = countryID;
					myCommand.Parameters.Add(parameterCountryID);
					SqlParameter parameterStateID = new SqlParameter("@StateID", SqlDbType.Int);
					parameterStateID.Value = stateID;
					myCommand.Parameters.Add(parameterStateID);
					SqlParameter parameterPIva = new SqlParameter("@PIva", SqlDbType.NVarChar, 11);
					parameterPIva.Value = pIva;
					myCommand.Parameters.Add(parameterPIva);
					SqlParameter parameterCFiscale = new SqlParameter("@CFiscale", SqlDbType.NVarChar, 16);
					parameterCFiscale.Value = cFiscale;
					myCommand.Parameters.Add(parameterCFiscale);
					SqlParameter parameterPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
					parameterPhone.Value = phone;
					myCommand.Parameters.Add(parameterPhone);
					SqlParameter parameterFax = new SqlParameter("@Fax", SqlDbType.NVarChar, 50);
					parameterFax.Value = fax;
					myCommand.Parameters.Add(parameterFax);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if (!Config.EncryptPassword || password == string.Empty)
						if (Config.EncryptPassword && password == string.Empty)
						{
							password = cryphelp.GetPassword(userID);
							parameterPassword.Value = password.Split(',')[0].ToString();
							parameterSalt.Value = password.Split(',')[1].ToString();
						}
						else
						{
							parameterSalt.Value = "";
							parameterPassword.Value = password;
						}
					else
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterSendNewsletter = new SqlParameter("@SendNewsletter", SqlDbType.Bit);
					parameterSendNewsletter.Value = sendNewsletter;
					myCommand.Parameters.Add(parameterSendNewsletter);
					// Execute the command
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// The UpdateUser method updates a user record from the "Users" database table.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="name"></param>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <param name="portalID"></param>
		/// <param name="SendNewsletter"></param>
		public void UpdateUser(int userID, string name, string email, string password, int portalID, bool SendNewsletter)
		{
			cryphelp = new CryptoHelper();
			if (Config.UseSingleUserBase) portalID = 0;

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUser", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
					parameterEmail.Value = email;
					myCommand.Parameters.Add(parameterEmail);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if(Config.EncryptPassword && password != string.Empty) 
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					else if(Config.EncryptPassword && password == string.Empty)
					{
						cryphelp = new CryptoHelper();
						password = cryphelp.GetPassword(userID);
						parameterPassword.Value = password.Split(',')[0].ToString();
						parameterSalt.Value = password.Split(',')[1].ToString();
					}
					else
					{
						parameterSalt.Value = "";
						parameterPassword.Value = password;
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					SqlParameter parameterSendNewsletter = new SqlParameter("@SendNewsletter", SqlDbType.Bit);
					parameterSendNewsletter.Value = SendNewsletter;
					myCommand.Parameters.Add(parameterSendNewsletter);

					myConnection.Open();
					// Execute the command in a try/catch to catch duplicate username errors
					try 
					{
						myCommand.ExecuteNonQuery();
					}
					catch 
					{
						throw;
					}
				}
			}
		}

		// method added by Thierry (tiptopweb) on 12 April 2003
		/// <summary>
		/// The UpdateUser method inserts a new user record into the "Users" database table.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="Name"></param>
		/// <param name="Company"></param>
		/// <param name="Address"></param>
		/// <param name="City"></param>
		/// <param name="Zip"></param>
		/// <param name="Phone"></param>
		/// <param name="Fax"></param>
		/// <param name="PIva"></param>
		/// <param name="CFiscale"></param>
		/// <param name="SendNewsletter"></param>
		/// <param name="IDCountry"></param>
		/// <param name="IDState"></param>
		/// <returns></returns>
		public void UpdateUser(int userID, string Name, string Company, string Address, string City, string Zip, string Phone, string Fax, string PIva, string CFiscale, bool SendNewsletter, string IDCountry, int IDState)
		{
			cryphelp = new CryptoHelper();
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("UpdateUserFullNoPassword", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterIDState = new SqlParameter("@StateID", SqlDbType.Int);
					parameterIDState.Value = IDState;
					myCommand.Parameters.Add(parameterIDState);
					SqlParameter parameterIDCountry = new SqlParameter("@CountryID", SqlDbType.NChar, 2);
					parameterIDCountry.Value = IDCountry;
					myCommand.Parameters.Add(parameterIDCountry);
					SqlParameter parameterName = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
					parameterName.Value = Name;
					myCommand.Parameters.Add(parameterName);
					SqlParameter parameterAddress = new SqlParameter("@Address", SqlDbType.NVarChar, 50);
					parameterAddress.Value = Address;
					myCommand.Parameters.Add(parameterAddress);
					SqlParameter parameterCompany = new SqlParameter("@Company", SqlDbType.NVarChar, 50);
					parameterCompany.Value = Company;
					myCommand.Parameters.Add(parameterCompany);
					SqlParameter parameterCity = new SqlParameter("@City", SqlDbType.NVarChar, 50);
					parameterCity.Value = City;
					myCommand.Parameters.Add(parameterCity);
					SqlParameter parameterZip = new SqlParameter("@Zip", SqlDbType.NVarChar, 6);
					parameterZip.Value = Zip;
					myCommand.Parameters.Add(parameterZip);
					SqlParameter parameterPIva = new SqlParameter("@PIva", SqlDbType.NVarChar, 11);
					parameterPIva.Value = PIva;
					myCommand.Parameters.Add(parameterPIva);
					SqlParameter parameterCFiscale = new SqlParameter("@CFiscale", SqlDbType.NVarChar, 16);
					parameterCFiscale.Value = CFiscale;
					myCommand.Parameters.Add(parameterCFiscale);
					SqlParameter parameterPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
					parameterPhone.Value = Phone;
					myCommand.Parameters.Add(parameterPhone);
					SqlParameter parameterFax = new SqlParameter("@Fax", SqlDbType.NVarChar, 50);
					parameterFax.Value = Fax;
					myCommand.Parameters.Add(parameterFax);
					SqlParameter parameterSendNewsletter = new SqlParameter("@SendNewsletter", SqlDbType.Bit);
					parameterSendNewsletter.Value = SendNewsletter;
					myCommand.Parameters.Add(parameterSendNewsletter);

					myConnection.Open();
					// Execute the command in a try/catch to catch duplicate username errors
					try 
					{
						myCommand.ExecuteNonQuery();
					}
					catch 
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// The UpdateUserCheckEmail sets the user email as trusted and verified
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="CheckedEmail"></param>
		public void UpdateUserCheckEmail(int userID, int CheckedEmail)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserCheckEmail", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterCheckedEmail = new SqlParameter("@CheckedEmail", SqlDbType.TinyInt);
					parameterCheckedEmail.Value = CheckedEmail;
					myCommand.Parameters.Add(parameterCheckedEmail);
					// Open the database connection and execute the command
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Change the user password with a new one
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="password"></param>
		public void UpdateUserSetPassword(int userID, string password)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserSetPassword", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Add Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterSalt = new SqlParameter("@Salt", SqlDbType.NVarChar, 10);
					SqlParameter parameterPassword = new SqlParameter("@Password", SqlDbType.NVarChar, 40);
					if(Config.EncryptPassword && password != string.Empty) 
					{
						parameterSalt.Value = cryphelp.CreateSalt(5);
						parameterPassword.Value = cryphelp.CreatePasswordHash(password, parameterSalt.Value.ToString());
					}
					else if(Config.EncryptPassword && password == string.Empty)
					{
						cryphelp = new CryptoHelper();
						password = cryphelp.GetPassword(userID);
						parameterPassword.Value = password.Split(',')[0].ToString();
						parameterSalt.Value = password.Split(',')[1].ToString();
					}
					else
					{
						parameterSalt.Value = "";
						parameterPassword.Value = password;
					}
					myCommand.Parameters.Add(parameterSalt);
					myCommand.Parameters.Add(parameterPassword);
					// Open the database connection and execute the command
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
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
	sealed public class UserWinMgmtDB 
	{
		/// <summary>
		/// DeleteUserDeskTop Method
		///
		/// The DeleteUserDeskTop method deletes a specific user desktop from the serDesktop database table.
		/// </summary>
		/// <param name="userID" type="int">
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
		///     A bool value...
		/// </returns>
		public bool DeleteUserDeskTop(int userID, int portalID)
		{
			bool results = false;

			// john.mandia@whitelightsolutions.com: 29th May 2004: Since this has to do with collapsable modules on a specific tab on a specific portal I don't think it should pass the static portal ID
			//if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_DeleteUserDesktop", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);

					myConnection.Open();
					try 
					{
						myCommand.ExecuteNonQuery();
						results=true;
					} 
					catch
					{
					}
				}
				return results;
			}
		}// end of DeleteUserDeskTop

		/// <summary>
		/// GetUserDesktop() Method <a name="GetUserDesktop"></a>
		///
		/// The GetUserDesktop method returns a list of the users' desktop envrionment
		///
		/// Other relevant sources:
		///     + <a href="GetUserDesktop.htm" style="color:green">GetUserDesktop Stored Procedure</a>
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="portalID"></param>
		/// <returns></returns>
		public UserLayoutMgr GetUserDesktop(int userID, int portalID) 
		{
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
		}// end of GetUserDesktop

		/// <summary>
		/// Save the User's Window Desktop Settings
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="userID"></param>
		/// <param name="moduleID"></param>
		/// <param name="pageID"></param>
		/// <param name="wstate"></param>
		public void SaveUserDesktop(int portalID, int userID, int moduleID, int pageID, WindowStateEnum wstate) 
		{
			// john.mandia@whitelightsolutions.com: 29th May 2004: Since this has to do with collapsable modules on a specific tab on a specific portal I don't think it should pass the static portal ID
			//if (Config.UseSingleUserBase) portalID = 0;
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateUserDesktop", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;
					// Update Parameters to SPROC
					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);
					SqlParameter parameterPageID = new SqlParameter("@TabID", SqlDbType.Int);
					parameterPageID.Value = pageID;
					myCommand.Parameters.Add(parameterPageID);
					SqlParameter parameterState = new SqlParameter("@WindowState", SqlDbType.SmallInt);
					parameterState.Value = (short)wstate;
					myCommand.Parameters.Add(parameterState);
					// Execute the command
					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}// end of SaveUserDesktop

		#region Private Methods

		/// <summary>
		/// Create the Module Settings 
		/// </summary>
		/// <param name="dr"></param>
		/// <returns></returns>
		private UserModuleSettings addModuleSettings(SqlDataReader dr)
		{
			UserModuleSettings ums = new UserModuleSettings();

			try 
			{
				string name=string.Empty;

				// iterate over the fields/columns
				for ( int inx=0; inx<dr.FieldCount;++inx)
				{

					// get the values
					if ( !dr.IsDBNull(inx) )
					{
						name=dr.GetName(inx).ToLower();

						if ( name[0] == 'm') // module id
							ums.ModuleID = dr.GetInt32(inx);

						else if ( name[0] == 't' ) // tab id
							ums.PageID = dr.GetInt32(inx);

						else if ( name[0] == 's' ) // Window State
							ums.State = (WindowStateEnum) dr.GetInt16(inx);
					}
				}
			} 
#if !DEBUG

			catch {}
#else

			catch (Exception ex)
			{
				throw ex;
			}
#endif
			return ums;
		} // end of addModuleSettings

		/// <summary>
		/// Populate a User Layout from the Data Reader
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="ulm"></param>
		private UserLayoutMgr populateUserLayoutMgr(SqlDataReader dr, UserLayoutMgr ulm)
		{
			while (dr.Read())
				ulm.Add(addModuleSettings(dr));
			return ulm;
		} // end of populateUserLayoutMgr

		#endregion

	} // end of UserWinMgmtDB
	//*********************************************************************
	// [END -- Added for User 's WIndow Mgmt -- bja@reedtek.com]
	//
	//*********************************************************************
}
