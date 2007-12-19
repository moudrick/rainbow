using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using Rainbow.Configuration;
using Path = Rainbow.Settings.Path;

namespace Rainbow.Helpers
{
	/// <summary>
	/// ModuleInstall incapsulates all the logic for install, 
	/// uninstall modules on portal
	/// </summary>
	public class ModuleInstall
	{
		/// <summary>
		///     
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private ModuleInstall()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupFileName"></param>
		/// <param name="install"></param>
		public static void InstallGroup(string groupFileName, bool install)
		{
			DataTable modules = GetInstallGroup(groupFileName);

			foreach (DataRow r in modules.Rows)
			{
				string friendlyName = r["FriendlyName"].ToString();
				string desktopSource = r["DesktopSource"].ToString();
				string mobileSource = r["MobileSource"].ToString();
				Install(friendlyName, desktopSource, mobileSource, install);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupFileName"></param>
		/// <returns></returns>
		[Obsolete("Do not use ADO.NET data objects outside DAL.  Replace me.")]
		private static DataTable GetInstallGroup(string groupFileName)
		{
			//Load the XML as dataset
			using (DataSet ds = new DataSet())
			{
				string installer = groupFileName;

				try
				{
					ds.ReadXml(installer);
				}

				catch (Exception ex)
				{
					LogHelper.Logger.Log(LogLevel.Error, "Exception installing module: " + installer, ex);
					return null;
				}
				return ds.Tables[0];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupFileName"></param>
		public static void UninstallGroup(string groupFileName)
		{
			DataTable modules = GetInstallGroup(groupFileName);

			foreach (DataRow r in modules.Rows)
			{
				string friendlyName = r["FriendlyName"].ToString();
				string desktopSource = r["DesktopSource"].ToString();
				string mobileSource = r["MobileSource"].ToString();
				Uninstall(desktopSource, mobileSource);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="friendlyName"></param>
		/// <param name="desktopSource"></param>
		/// <param name="mobileSource"></param>
		public static void Install(string friendlyName, string desktopSource, string mobileSource)
		{
			Install(friendlyName, desktopSource, mobileSource, true);
		}

		/// <summary>
		/// Installs module
		/// </summary>
		/// <param name="friendlyName"></param>
		/// <param name="desktopSource"></param>
		/// <param name="mobileSource"></param>
		/// <param name="install"></param>
		public static void Install(string friendlyName, string desktopSource, string mobileSource, bool install)
		{
			LogHelper.Logger.Log(LogLevel.Info, "Installing DesktopModule '" + friendlyName + "' from '" + desktopSource + "'");

			if (mobileSource != null && mobileSource.Length > 0)
				LogHelper.Logger.Log(LogLevel.Info, "Installing MobileModule '" + friendlyName + "' from '" + mobileSource + "'");
			string controlFullPath = Path.ApplicationRoot + "/" + desktopSource;
			// Istantiate the module
			Page page = new Page();
			//http://sourceforge.net/tracker/index.php?func=detail&aid=738670&group_id=66837&atid=515929
			//Rainbow.UI.Page page = new Rainbow.UI.Page();
			Control myControl = page.LoadControl(controlFullPath);

			if (!(myControl is PortalModuleControl))
				throw new ApplicationException("Module '" + myControl.GetType().FullName + "' is not a PortalModule Control");
			PortalModuleControl portalModule = (PortalModuleControl) myControl;

			// Check mobile module
			if (mobileSource != null && (mobileSource != null && mobileSource.Length != 0) && mobileSource.ToLower().EndsWith(".ascx"))
			{
				//TODO: Check mobile module
				//TODO: MobilePortalModuleControl mobileModule = (MobilePortalModuleControl) page.LoadControl(Rainbow.Settings.Path.ApplicationRoot + "/" + mobileSource);
				if (!File.Exists(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + mobileSource)))
					throw new FileNotFoundException("Mobile Control not found");
			}
			// Get Module ID
			Guid defID = portalModule.GuidID;
			//Get Assembly name
			string assemblyName = portalModule.GetType().BaseType.Assembly.CodeBase;
			assemblyName = assemblyName.Substring(assemblyName.LastIndexOf('/') + 1); //Get name only
			// Get Module Class name
			string className = portalModule.GetType().BaseType.FullName;
			// Now we add the definition to module list 
			ModulesDB modules = new ModulesDB();

			if (install)
			{
				//Install as new module
				//Call Install
				try
				{
					LogHelper.Logger.Log(LogLevel.Debug, "Installing '" + friendlyName + "' as new module.");
					portalModule.Install(null);
				}

				catch (Exception ex)
				{
					//Error occurred
					portalModule.Rollback(null);
					//Rethrow exception
					throw new ApplicationException("Exception occurred installing '" + portalModule.GuidID.ToString() + "'!", ex);
				}
				// All is fine: we can call Commit
				portalModule.Commit(null);

				try
				{
					// Add a new module definition to the database
					modules.AddGeneralModuleDefinitions(defID, friendlyName, desktopSource, mobileSource, assemblyName, className, portalModule.AdminModule, portalModule.Searchable);
				}

				catch (Exception ex)
				{
					//Rethrow exception
					throw new ApplicationException("AddGeneralModuleDefinitions Exception '" + portalModule.GuidID.ToString() + "'!", ex);
				}
			}

			else
			{
				// Update the general module definition
				try
				{
					LogHelper.Logger.Log(LogLevel.Debug, "Updating '" + friendlyName + "' as new module.");
					modules.UpdateGeneralModuleDefinitions(defID, friendlyName, desktopSource, mobileSource, assemblyName, className, portalModule.AdminModule, portalModule.Searchable);
				}

				catch (Exception ex)
				{
					//Rethrow exception
					throw new ApplicationException("UpdateGeneralModuleDefinitions Exception '" + portalModule.GuidID.ToString() + "'!", ex);
				}
			}
			// Update the module definition - install for portal 0
			modules.UpdateModuleDefinitions(defID, 0, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="desktopSource"></param>
		/// <param name="mobileSource"></param>
		public static void Uninstall(string desktopSource, string mobileSource)
		{
			Page page = new Page();
			// Istantiate the module
			PortalModuleControl portalModule = (PortalModuleControl) page.LoadControl(Path.ApplicationRoot + "/" + desktopSource);

			//Call Uninstall
			try
			{
				portalModule.Uninstall(null);
			}

			catch (Exception ex)
			{
				//Rethrow exception
				throw new ApplicationException("Exception during uninstall!", ex);
			}
			// Delete definition
			new ModulesDB().DeleteModuleDefinition(portalModule.GuidID);
		}
	}
}