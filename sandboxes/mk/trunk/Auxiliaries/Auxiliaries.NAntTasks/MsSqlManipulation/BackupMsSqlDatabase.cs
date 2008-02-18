using Auxiliaries.Database.MsSql;
using NAnt.Core.Attributes;

namespace Auxiliaries.NAntTasks
{
    [TaskName("BackupMsSqlDatabase")]
    public class BackupMsSqlDatabase : MsSqlManipulationTask
    {
        protected override string ExecuteTaskInternal(DatabaseManipulator databaseManipulator)
        {
            return databaseManipulator.BackupDatabase(DatabaseName, BackupFileName);
        }
    }
}
