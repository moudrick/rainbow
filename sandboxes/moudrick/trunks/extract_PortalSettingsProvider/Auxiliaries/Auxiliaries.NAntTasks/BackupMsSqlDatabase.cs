using System.IO;
using Auxiliaries.Database.MsSql;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Auxiliaries.NAntTasks
{
    [TaskName("BackupMsSqlDatabase")]
    public class BackupMsSqlDatabase : Task
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


        protected override void ExecuteTask()
        {
            DatabaseManipulator manipulator = new DatabaseManipulator(instance, 
                isTrustedConnection, userName, userPassword);
            string outputContent = manipulator.BackupDatabase(databaseName, backupFileName);
            if (!string.IsNullOrEmpty(outputFileName))
            {
                File.WriteAllText(outputFileName, outputContent);
            }
        }
    }
}
