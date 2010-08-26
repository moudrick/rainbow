using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rainbow.Framework.Data.MsSql
{
    partial class PortalSetting
    {
        #region old portalsettings constructor w/some linq conversions - not used in new way of doing things, commented out
        ///// <summary>
        ///// The PortalSettings Constructor encapsulates all of the logic
        ///// necessary to obtain configuration settings necessary to render
        ///// a Portal Page view for a given request.<br/>
        ///// These Portal Settings are stored within a SQL database, and are
        ///// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        ///// This stored procedure returns values as SPROC output parameters,
        ///// and using three result sets.
        ///// </summary>
        ///// <param name="pageID">The page ID.</param>
        ///// <param name="portalAlias">The portal alias.</param>
        //public IEnumerable<PortalSetting> PortalSettings(int pageID, string portalAlias)
        //{
        //    //TODO: Move this somewhere else...

            

        //    DataClassesDataContext db = new DataClassesDataContext(Config.ConnectionString);

        //    ///* First, get Out Params */
        //    if (pageID == 0)
        //    {
        //        //get the portal settings only
                
        //    }
        //    else
        //    {
        //        //get all the settings...
        //        var qPortal = from portals in db.Portals
        //                      join pages in db.Pages on portals.PortalId equals pages.PortalId
        //                      where pages.PageId == pageID && portals.PortalAlias == portalAlias
        //                      select new
        //                      {
        //                          PortalId = portals.PortalId,
        //                          PortalName = portals.PortalName,
        //                          PortalPath = portals.PortalPath,
        //                          AlwaysShowEditButton = portals.IsAlwaysShowEditButton,
        //                          Name = portals.Name,
        //                          PageOrder = pages.PageOrder,
        //                          ParentPageId = pages.ParentPageId,
        //                          MobilePageName = pages.MobilePageName,
        //                          AuthRoles = pages.AuthorizedRoles,
        //                          IsShowMobile = pages.IsShowMobile
        //                      };

        //        ///* Get Tabs list */
        //        var qPages = from p in db.Pages
        //                     where p.PortalId == portalId
        //                     orderby p.PageOrder
        //                     select new Page
        //                     {
        //                         PageName = p.Name (string.IsNullOrEmpty(from ps in db.PageSettings
        //                                                          where ps.PageId == pageID &&
        //                                                             ps.SettingName == portalLanguage &&
        //                                                             !string.IsNullOrEmpty(ps.SettingValue)
        //                                                          select ps.SettingValue) ?
        //                                                            p.PageName :
        //                                                            (from ps in db.PageSettings
        //                                                             where ps.PageId == pageID &&
        //                                                                ps.SettingName == portalLanguage &&
        //                                                                !string.IsNullOrEmpty(ps.SettingValue)
        //                                                             select ps.SettingValue)),
        //                         AuthorizedRoles,
        //                         PageId,
        //                         ParentPageId,
        //                         PageOrder,
        //                         PageLayout
        //                     };

        //        // Get Mobile Tabs list
        //        var qPagesMobile = from p in db.Pages
        //                           where p.PortalId == portalId && p.IsShowMobile
        //                           orderby p.PageOrder
        //                           select p;

        //        // Then, get the DataTable of module info
        //        var qModules = from m in db.Modules
        //                       join md in db.ModuleDefinitions on m.ModuleDefId equals md.ModuleDefId
        //                       join gmd in db.GeneralModuleDefinitions on md.GeneralModDefId equals gmd.GeneralModDefId
        //                       where (m.PageId == pageID) || (m.IsShowEveryWhere.Value) && (md.PortalId == portalId)
        //                       orderby m.Order
        //                       select m;
        //    }
        //}

        #endregion
    }
}
