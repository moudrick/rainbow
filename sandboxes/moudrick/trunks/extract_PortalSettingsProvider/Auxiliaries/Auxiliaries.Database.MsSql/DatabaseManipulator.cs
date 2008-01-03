using System.Data;
using System.Data.SqlClient;

namespace Auxiliaries.Database.MsSql
{
    public class DatabaseManipulator
    {
        public string Instance
        {
            get { return instance; }
        }

        public bool IsTrustedConnection
        {
            get { return isTrustedConnection; }
        }

        public string UserName
        {
            get { return userName; }
        }

        public string UserPassword
        {
            get { return userPassword; }
        }

        public string ConnectionString
        {
            get
            {
                return string.Format("server={0};Trusted_Connection={1};uid={2};pwd={3};database=master",
                    instance, isTrustedConnection, userName, userPassword);
            }
        }

        readonly string instance;
        readonly bool isTrustedConnection;
        readonly string userName;
        readonly string userPassword;

        public DatabaseManipulator(string instance,
                                   bool isTrustedConnection,
                                   string userName,
                                   string userPassword)
        {
            this.instance = instance;
            this.userPassword = userPassword;
            this.userName = userName;
            this.isTrustedConnection = isTrustedConnection;
        }

        public string BackupDatabase(string databaseName, string backupFileName)
        {
            string output = string.Empty;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string deviceName = databaseName + "_Backup";
                string dropDeviceSql = string.Format(@"
    USE [master]; 
    DECLARE @device VARCHAR(100);
    SET @device = '{0}';
    EXEC sp_dropdevice @device;", deviceName);
                try
                {
                    ExecuteNonQuery(dropDeviceSql, connection);
                }
                catch{;}

                string backupSql = string.Format(@"
    USE [master]; 
    DECLARE @device VARCHAR(100);
    SET @device = '{0}';
    EXEC sp_addumpdevice 'disk', @device, '{1}';
    BACKUP DATABASE [{2}] TO [{0}] WITH INIT, SKIP;
    EXEC sp_dropdevice @device;", deviceName, backupFileName, databaseName);
                ExecuteNonQuery(backupSql, connection);
            }
            return output;
        }

        static void ExecuteNonQuery(string dropDeviceCmdText, SqlConnection connection)
        {
            
            SqlCommand command = new SqlCommand(dropDeviceCmdText, connection);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }
    }
}
