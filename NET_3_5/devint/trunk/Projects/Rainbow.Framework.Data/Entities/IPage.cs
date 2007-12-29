using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Data.Entities
{
    public interface IPage : IComparable, IComparable<IPage>, IConvertible
    {
        int Id { get; set; }
        int PortalId { get; set; }
        string Name { get; set; }
        string NameMobile { get; set; }
        bool IsShowMobile { get; set; }
        string Description { get; set; }
        int Order { get; set; }
        int? ParentId { get; set; }
        string AuthorizedRoles { get; set; }
        
        Theme CurrentTheme();
        Theme CurrentTheme(string requiredTheme);
        string MenuImage { get; set; }

        int? PageLayout { get; set; }
        string Layout { get; set; }
        string LayoutPath { get; }

        IEnumerable<IPage> MenuGroup { get; }
        int NestLevel { get; set; }
        
        IEnumerable<IModule> Modules { get; set; }
        int ActiveModule { get; set; }
        void SetActiveModuleCookie(int mID);

        IEnumerable<IPage> Pages { get; set; }
        IEnumerable<IPageSetting> PageSettings { get; set; }
        IPage ParentPage { get; set; }

        string GetSettingValue(string settingName);
        void SetSettingValue(string settingName, object value);
    }
}
