using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Rainbow.Framework.Design;

namespace Rainbow.Framework.Data.Entities
{
    public interface IModule : IComparable, IComparable<IPage>, IConvertible
    {
        string AuthorizedRolesAdd { get; set; }
        string AuthorizedRolesApprove { get; set; }
        string AuthorizedRolesDelete { get; set; }
        string AuthorizedRolesEdit { get; set; }
        string AuthorizedRolesModuleDelete { get; set; }
        string AuthorizedRolesModuleMove { get; set; }
        string AuthorizedRolesProperties { get; set; }
        string AuthorizedRolesPublishing { get; set; }
        string AuthorizedRolesView { get; set; }
        int CacheTime { get; set; }
        bool? IsCollapsable { get; set; }
        bool? IsNewVersion { get; set; }
        bool? IsShowEveryWhere { get; set; }
        bool? IsShowMobile { get; set; }
        bool? IsSupportWorkflow { get; set; }
        string LastEditor { get; set; }
        DateTime? LastModified { get; set; }
        int DefinitionId { get; set; }
        //Rainbow.Framework.Data.MsSql.ModuleDefinition ModuleDefinition { get; set; }
        int Id { get; set; }
        //System.Data.Linq.EntitySet<Rainbow.Framework.Data.MsSql.ModuleSetting> ModuleSettings { get; set; }
        //System.Data.Linq.EntitySet<Rainbow.Framework.Data.MsSql.ModuleUserSetting> ModuleUserSettings { get; set; }
        int Order { get; set; }
        //Rainbow.Framework.Data.MsSql.Page Page { get; set; }
        int PageId { get; set; }
        string PaneName { get; set; }
        string StagingLastEditor { get; set; }
        DateTime? StagingLastModified { get; set; }
        string Title { get; set; }
        byte? WorkflowState { get; set; }
    }
}
