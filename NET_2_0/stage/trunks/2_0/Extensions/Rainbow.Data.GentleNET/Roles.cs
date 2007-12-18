using System;
using System.ComponentModel;

namespace Rainbow.Data.GentleNET
{
	/// <summary>
	/// Summary description for Roles.
	/// </summary>
	public class Roles : RolesProvider
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
		public int Add(int portalID, String roleName)
		{
			//TODO: Move following line into business layer
			//if (PortalSettings.UseSingleUserBase) portalID = 0;

			rb_Roles roles = new rb_Roles(portalID, roleName);
			roles.Persist();
			return roles.RoleID;
		}

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
		public void AddUserRole(int roleID, int userID)
		{
			rb_UserRoles ur = rb_UserRoles.Retrieve(userID, roleID);
			if (ur == null)
			{
				ur = new rb_UserRoles(userID, roleID);
				ur.Persist();
			}
		}

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
		public void Remove(int roleID)
		{
			rb_Roles r = rb_Roles.Retrieve(roleID);
			if (r != null)
				r.Remove();
		}

		#region Component Model

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Roles(IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public Roles()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion
	}
}