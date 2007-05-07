using System.ComponentModel;

namespace Rainbow.Data
{
	/// <summary>
	/// Summary description for DataHelper.
	/// </summary>
	public sealed class Helper : Component
	{
		#region Provider implementation

		private static readonly PortalsProvider portalsProvider = PortalsProvider.Instance() as PortalsProvider;
		private static readonly PagesProvider pagesProvider = PagesProvider.Instance() as PagesProvider;
		private static readonly ModulesProvider modulesProvider = ModulesProvider.Instance() as ModulesProvider;
		private static readonly RolesProvider rolesProvider = RolesProvider.Instance() as RolesProvider;
		private static readonly UsersProvider usersProvider = UsersProvider.Instance() as UsersProvider;

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static PortalsProvider Portals
		{
			get { return portalsProvider; }
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static PagesProvider Pages
		{
			get { return pagesProvider; }
		}

		public static ModulesProvider Modules
		{
			get 
			{
              	return modulesProvider;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static RolesProvider Roles
		{
			get { return rolesProvider; }
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		public static UsersProvider Users
		{
			get { return usersProvider; }
		}

		#endregion

		#region Component Model

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private Helper(IContainer container)
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

		private Helper()
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