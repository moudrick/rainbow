using System.IO;
using Auxiliaries.Database.MsSql;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Auxiliaries.NAntTasks
{
    public abstract class MsSqlManipulationTask : Task
    {
        string databaseName;
        string backupFileName;
        string instance;
        bool isTrustedConnection = false;
        string userName = string.Empty;
        string userPassword = string.Empty;
        string outputFileName = string.Empty;

        [TaskAttribute("databaseName", Required = true)]
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        [TaskAttribute("backupFileName", Required = true)]
        public string BackupFileName
        {
            get { return backupFileName; }
            set { backupFileName = value; }
        }

        [TaskAttribute("instance", Required = true)]
        public string Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        [TaskAttribute("isTrustedConnection")]
        public bool IsTrustedConnection
        {
            get { return isTrustedConnection; }
            set { isTrustedConnection = value; }
        }

        [TaskAttribute("userName")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        [TaskAttribute("userPassword")]
        public string UserPassword
        {
            get { return userPassword; }
            set { userPassword = value; }
        }

        [TaskAttribute("outputFileName")]
        public string OutputFileName
        {
            get { return outputFileName; }
            set { outputFileName = value; }
        }

        protected abstract string ExecuteTaskInternal(DatabaseManipulator databaseManipulator);

        protected override void ExecuteTask()
        {
            string outputContent = ExecuteTaskInternal(new DatabaseManipulator(Instance, 
                                                                               IsTrustedConnection, UserName, UserPassword));
            if (!string.IsNullOrEmpty(OutputFileName))
            {
                Log(Level.Info, outputContent);
                File.WriteAllText(OutputFileName, outputContent);
            }
        }
    }
}