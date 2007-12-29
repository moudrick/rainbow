using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Data.Entities
{
    public interface IPageSetting : IComparable, IComparable<IPage>, IConvertible
    {
        //Rainbow.Framework.Data.MsSql.BaseSetting BaseSetting { get; set; }
        string DataType { get; set; }
        string Description { get; set; }
        string EnglishName { get; set; }
        bool? IsRequired { get; set; }
        int? MaxValue { get; set; }
        int MinValue { get; set; }
        //Rainbow.Framework.Data.MsSql.Page Page { get; set; }
        int PageId { get; set; }
        //Rainbow.Framework.Data.MsSql.SettingGroup SettingGroup { get; set; }
        int? SettingGroupId { get; set; }
        string SettingName { get; set; }
        int? SettingOrder { get; set; }
        string SettingValue { get; set; }
    }
}
