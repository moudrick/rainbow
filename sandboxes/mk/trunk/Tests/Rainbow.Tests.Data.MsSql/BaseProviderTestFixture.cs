using System.Configuration;
using Auxiliaries.Database.MsSql;
using NUnit.Framework;

namespace Rainbow.Tests.Data.MsSql
{
    public class BaseProviderTestFixture
    {
        static readonly string instance = ConfigurationManager.AppSettings["RainbowTestDBInstance"];
        static readonly string databaseName = ConfigurationManager.AppSettings["RainbowTestDBName"];
        static readonly string backupFileName = ConfigurationManager.AppSettings["TestDbBackupFileName"];

        static readonly DatabaseManipulator manipulator = new DatabaseManipulator(instance, true, 
            null, null);

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            manipulator.RestoreDatabase(databaseName, backupFileName, "rainbow");
        }
    }
}
