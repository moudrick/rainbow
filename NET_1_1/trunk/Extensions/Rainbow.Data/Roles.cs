using System;

namespace Rainbow.Data
{
	/// <summary>
	/// Summary description for Roles.
	/// </summary>
	public abstract class Roles
	{
		/// <summary>
		///*********************************************************************
		///
		/// AddRole() Method <a name="AddRole"></a>
		///
		/// The AddRole method creates a new security role for the specified portal,
		/// and returns the new RoleID value.
		///
		/// Other relevant sources:
		///     + <a href="AddRole.htm" style="color:green">AddRole Stored Procedure</a>
		///
		///*********************************************************************
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
		public abstract int Add(int portalID, String roleName);

		/// <summary>
		///*********************************************************************
		///
		/// AddUserRole() Method <a name="AddUserRole"></a>
		///
		/// The AddUserRole method adds the user to the specified security role.
		///
		/// Other relevant sources:
		///     + <a href="AddUserRole.htm" style="color:green">AddUserRole Stored Procedure</a>
		///
		///*********************************************************************
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
		public abstract void AddUserRole(int roleID, int userID);

		/// <summary>
		///*********************************************************************
		///
		/// Remove() Method <a name="Remove"></a>
		///
		/// The DeleteRole deletes the specified role from the portal database.
		///
		/// Other relevant sources:
		///     + <a href="DeleteRole.htm" style="color:green">DeleteRole Stored Procedure</a>
		///
		///*********************************************************************
		/// </summary>
		/// <param name="roleID" type="int">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public abstract void Remove(int roleID);
	}
}
