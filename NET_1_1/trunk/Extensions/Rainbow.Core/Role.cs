using System;
using System.ComponentModel;

namespace Rainbow.Core
{
	/// <summary>
	/// Summary description for Role.
	/// </summary>
	[Serializable]
	public class Role : Component
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Role(IContainer container)
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

		public Role()
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

		/// <summary>
		/// Role ID
		/// </summary>
		public int RoleID
		{
			get { return roleID; }
			set { roleID = value; }
		}

		private int roleID;

		/// <summary>
		/// Role Name
		/// </summary>
		public string RoleName
		{
			get { return roleName; }
			set { roleName = value; }
		}

		private string roleName;

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator <(Role x, Role y)
		{
			if (x == null || y == null) return true;
			return x.RoleID < y.RoleID ? true : false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator >(Role x, Role y)
		{
			if (x == null || y == null) return true;
			return x.RoleID > y.RoleID ? true : false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator <=(Role x, Role y)
		{
			if (x == null || y == null) return true;
			return x.RoleID <= y.RoleID ? true : false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="y" type="Rainbow.Core.Role">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <returns>
		///     A  value...
		/// </returns>
		public static bool operator >=(Role x, Role y)
		{
			if (x == null || y == null) return true;
			return x.RoleID >= y.RoleID ? true : false;
		}
	}
}