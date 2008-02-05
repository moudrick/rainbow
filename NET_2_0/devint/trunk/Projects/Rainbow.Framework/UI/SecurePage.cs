using Rainbow.Framework.Providers;
using Rainbow.Framework.Security;

namespace Rainbow.Framework.Web.UI
{
    /// <summary>
    /// SecurePage inherits from Rainbow.Framework.Web.UI.Page <br/>
    /// Used for Security Access pages<br/>
    /// Can be inherited
    /// </summary>
    [History("jviladiu@portalServices.net", "2004/07/22", "Created this to support pages that need Security Access.")]
    public class SecurePage : Page
    {
        /// <summary>
        /// Get the AllowedModules array from page if exists and set the restrictions for use
        /// For this method work, the user page need override AllowedModules with GUIDS
        /// </summary>
        protected override void ModuleGuidInCookie()
        {
            if (AllowedModules != null)
            {
                string guidsInUse = string.Empty;
                if (Request.Cookies["RainbowSecurity"] != null)
                {
                    guidsInUse = Request.Cookies["RainbowSecurity"].Value;
                }
                foreach (string moduleGuidString in AllowedModules)
                {
                    if (guidsInUse.IndexOf(moduleGuidString.ToUpper()) > -1)
                    {
                        return;
                    }
                }
                if (ModuleID != 0)
                {
                    guidsInUse = RainbowModuleProvider.Instance.GetModuleGuid(ModuleID).ToString().ToUpper();
                    foreach (string moduleGuidString in AllowedModules)
                    {
                        if (guidsInUse.IndexOf(moduleGuidString.ToUpper()) > -1)
                        {
                            return;
                        }
                    }
                }
                PortalSecurity.AccessDenied();
            }
        }

        /// <summary>
        /// Load settings
        /// </summary>
        protected override void LoadSettings()
        {
            base.LoadSettings();
            portalSettings.ActiveModule = ModuleID;
        }
    }
}
