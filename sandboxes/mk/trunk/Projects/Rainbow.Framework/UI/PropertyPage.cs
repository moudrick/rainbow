using System;
using System.Collections;
using Rainbow.Framework.Providers;
using Rainbow.Framework.Security;

namespace Rainbow.Framework.Web.UI
{
    /// <summary>
    /// PropertyPage inherits from Rainbow.Framework.UI.SecurePage <br/>
    /// Used for properties pages<br/>
    /// Can be inherited
    /// </summary>
    [History("jviladiu@portalServices.net", "2004/07/22",
            "Added Security Access. Now inherits from Rainbow.Framework.UI.SecurePage")]
    [History("jviladiu@portalServices.net", "2004/07/22", "Clean Methods that only call to base")]
    [History("Jes1111", "2003/03/04",
            "Smoothed out page event inheritance hierarchy - placed security checks and cache flushing")]
    public class PropertyPage : SecurePage
    {
        /// <summary>
        /// Handles OnUpdate event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            // Verify that the current user has access to add in this module
            if (PortalSecurity.HasPropertiesPermissions(ModuleID) == false)
                // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
                //				&& PortalSecurity.IsInRoles("Admins") == false)
                PortalSecurity.AccessDeniedEdit();
            base.OnUpdate(e);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        protected override void LoadSettings()
        {
            // Verify that the current user has access to edit this module
            if (PortalSecurity.HasPropertiesPermissions(ModuleID) == false)
            {
                // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
                //				&& PortalSecurity.IsInRoles("Admins") == false)
                PortalSecurity.AccessDeniedEdit();
            }
            base.LoadSettings();
        }

        /// <summary>
        /// Only can use this from the original module
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override ArrayList AllowedModules
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(RainbowModuleProvider.Instance.GetModuleGuid(ModuleID).ToString());
                return list;
            }
        }
    }
}
