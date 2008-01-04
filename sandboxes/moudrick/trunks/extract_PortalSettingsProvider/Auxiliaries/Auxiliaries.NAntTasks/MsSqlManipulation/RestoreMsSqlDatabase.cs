using Auxiliaries.Database.MsSql;
using NAnt.Core.Attributes;

namespace Auxiliaries.NAntTasks
{
    [TaskName("RestoreMsSqlDatabase")]
    public class RestoreMsSqlDatabase : MsSqlManipulationTask
    {
        string storedDatabaseName = null;

        [TaskAttribute("storedDatabaseName")]
        public string StoredDatabaseName
        {
            get { return storedDatabaseName ?? DatabaseName; }
            set { storedDatabaseName = value; }
        }

        protected override string ExecuteTaskInternal(DatabaseManipulator databaseManipulator)
        {
            return databaseManipulator.RestoreDatabase(DatabaseName, BackupFileName,
                StoredDatabaseName);
        }
    }
}
