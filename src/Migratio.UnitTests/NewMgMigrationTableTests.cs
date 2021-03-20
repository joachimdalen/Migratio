using System.Linq;
using Migratio.Results;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMgMigrationTableTests : BaseCmdletTest
    {
        [Fact(DisplayName = "New-MgMigrationTable returns false if table exists")]
        public void NewMgMigrationTable_Returns_False_If_Table_Exists()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);

            var command = new NewMgMigrationTable(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<MgResult>().First();
            Assert.False(result.Successful);
            DbMock.VerifyMigrationTableExists(Times.Once());
            DbMock.VerifyCreateMigrationTable(Times.Never());
        }

        [Fact(DisplayName = "New-MgMigrationTable returns true if table was created")]
        public void NewMgMigrationTable_Returns_True_If_Table_Was_Created()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(false);
            DbMock.CreateMigrationTable(1);

            var command = new NewMgMigrationTable(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<MgResult>().First();
            Assert.True(result.Successful);
            DbMock.VerifyMigrationTableExists(Times.Once());
            DbMock.VerifyCreateMigrationTable(Times.Once());
        }

        [Fact(DisplayName = "New-MgMigrationTable default constructor constructs")]
        public void NewMgMigrationTable_Default_Constructor_Constructs()
        {
            var result = new NewMgMigrationTable
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.NotNull(result);
        }
    }
}

// var iss = InitialSessionState.CreateDefault2();
// iss.Commands.Add(new SessionStateCmdletEntry("New-MigrationMigrationTable",
//     typeof(NewMgMigrationTable), null));
// using (var ps = PowerShell.Create(iss))
// {
//     ps.AddCommand("New-MigrationMigrationTable").AddParameter("ClassName", "Win32_BIOS");
//     var res = ps.Invoke().FirstOrDefault();
//     Assert.NotNull(res);
//     Assert.False(ps.HadErrors);
// }