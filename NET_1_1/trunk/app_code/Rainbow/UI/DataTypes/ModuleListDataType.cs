using System;
using System.Web;
using Rainbow.Configuration;

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
		public ModuleListDataType(string ModuleType)
		{
			InnerDataSource = ModuleType;
			//InitializeComponents();
		}

		public override object DataSource
		{
			get
			{
				// Obtain PortalSettings from Current Context
				PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				return new ModulesDB().GetModulesByName(InnerDataSource.ToString(), portalSettings.PortalID);
			}
		}
        
		public override string Description
		{
			get
			{
				return "Module List";
			}
		}

		public override string DataValueField
		{
			get
			{
				return "ModuleID";
			}
			set
			{
				throw new ArgumentException("Value cannot be set", "ModuleID");
			}
		}

		public override string DataTextField
		{
			get
			{
				return "ModuleTitle";
			}
			set
			{
				throw new ArgumentException("Value cannot be set", "ModuleTitle");
			}
		}
	}

}