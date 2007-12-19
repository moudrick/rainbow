using System;
using System.Web;

namespace Rainbow.UI.DataTypes
{
	/// <summary>
	/// ModuleListDataType
	/// </summary>
	public class ModuleListDataType : ListDataType
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ModuleType">The Module name</param>
		public ModuleListDataType(string moduleType)
		{
			InnerDataSource = moduleType;
			//InitializeComponents();
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
		public override object DataSource
		{
			get
			{
				// Obtain PortalSettings from Current Context
				PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				return new ModulesDB().GetModulesByName(InnerDataSource.ToString(), portalSettings.PortalID);
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
		public override string Description
		{
			get { return "Module List"; }
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
		public override string DataValueField
		{
			get { return "ModuleID"; }
			set { throw new ArgumentException("Value cannot be set", "ModuleID"); }
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
		public override string DataTextField
		{
			get { return "ModuleTitle"; }
			set { throw new ArgumentException("Value cannot be set", "ModuleTitle"); }
		}
	}

}