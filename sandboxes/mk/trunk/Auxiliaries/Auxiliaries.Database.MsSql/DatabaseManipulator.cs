using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

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
            return DoProcess(delegate(SqlConnection connection)
                 {
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
                     catch {;}

                     string backupSql = string.Format(@"
    USE [master]; 
    DECLARE @device VARCHAR(100);
    SET @device = '{0}';
    EXEC sp_addumpdevice 'disk', @device, '{1}';
    BACKUP DATABASE [{2}] TO [{0}] WITH INIT, SKIP;
    EXEC sp_dropdevice @device;", deviceName, backupFileName, databaseName);
                     ExecuteNonQuery(backupSql, connection);
                 }
            );
        }

        public string RestoreDatabase(string databaseName, string backupFileName)
        {
            return RestoreDatabase(databaseName, backupFileName, databaseName);     
        }

        public string RestoreDatabase(string databaseName, string backupFileName, string oldDatabaseName)
        {
            return DoProcess(delegate(SqlConnection connection)
                {
                    string killProcessSql =
                        string.Format(
                            @"
    USE [master]; 
    DECLARE @sql VARCHAR(8000); 
    SET @sql = ''; 
    SELECT @sql = @sql + 'KILL ' + CAST(spid AS VARCHAR(10)) + ' ' FROM master.dbo.sysprocesses AS sp LEFT JOIN master.dbo.sysdatabases AS sdb ON sp.dbid = sdb.dbid WHERE [Name] = '{0}'; 
    EXEC(@sql)",
                            databaseName);
                    ExecuteNonQuery(killProcessSql, connection);

                    //<!-- 2k: USE [master]; DECLARE @physical_name VARCHAR(8000); SELECT @physical_name=[filename] FROM dbo.sysfiles WHERE [name] = 'master'; PRINT @physical_name -->
                    //<!-- 2k5: USE [master]; DECLARE @physical_name VARCHAR(MAX); SELECT @physical_name=physical_name FROM sys.database_files WHERE [name] = 'master'; PRINT @physical_name -->
                    string physicalNameSql =
                        string.Format(
                            @"
    USE [master]; 
    DECLARE @physical_name VARCHAR(8000); 
    SELECT @physical_name=[filename] FROM dbo.sysfiles WHERE [name] = 'master'; PRINT @physical_name
    SELECT @physical_name;");
                    SqlCommand command = new SqlCommand(physicalNameSql, connection);
                    command.CommandType = CommandType.Text;
                    string instanceMasterFileName = command.ExecuteScalar().ToString().Trim();
                    string instanceDataDir = Path.GetDirectoryName(instanceMasterFileName) +
                                             Path.DirectorySeparatorChar;
                    string restoreSql =
                        string.Format(
                            @"
    USE [master]; 
    RESTORE DATABASE [{0}] 
        FROM DISK = N'{1}' 
        WITH  FILE = 1,  
        MOVE N'{2}' TO N'{3}{0}.mdf',  
        MOVE N'{2}_log' TO N'{3}{0}_log.ldf', 
        NOUNLOAD, REPLACE, STATS = 10",
                            databaseName, backupFileName, oldDatabaseName, instanceDataDir);
                    ExecuteNonQuery(restoreSql, connection);
                }
            );
        }

        delegate void ConnectedActionSet(SqlConnection conneciton);
        string DoProcess(ConnectedActionSet connectedActionSet)
        {
            StringBuilder output = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.InfoMessage += delegate(object sender, SqlInfoMessageEventArgs e)
                    {
                        output.AppendLine(e.Message);
                    };
                connection.Open();
                connectedActionSet.Invoke(connection);
            }
            return output.ToString();
        }

        static void ExecuteNonQuery(string sqlText, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand(sqlText, connection);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }
    }
}
