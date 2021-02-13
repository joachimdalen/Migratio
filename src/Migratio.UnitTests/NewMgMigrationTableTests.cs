using System.Linq;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMgMigrationTableTests
    {
        private readonly DatabaseProviderMock _dbMock;

        public NewMgMigrationTableTests()
        {
            _dbMock = new DatabaseProviderMock();
        }

        [Fact(DisplayName = "New-MgMigrationTable returns false if table exists")]
        public void NewMgMigrationTable_Returns_False_If_Table_Exists()
        {
            _dbMock.MigrationTableExists(true);

            var command = new NewMgMigrationTable(_dbMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<bool>().First();
            Assert.False(result);
            _dbMock.VerifyMigrationTableExists(Times.Once());
            _dbMock.VerifyCreateMigrationTable(Times.Never());
        }

        [Fact(DisplayName = "New-MgMigrationTable returns true if table was created")]
        public void NewMgMigrationTable_Returns_True_If_Table_Was_Created()
        {
            _dbMock.MigrationTableExists(false);
            _dbMock.CreateMigrationTable(1);

            var command = new NewMgMigrationTable(_dbMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<bool>().First();
            Assert.True(result);
            _dbMock.VerifyMigrationTableExists(Times.Once());
            _dbMock.VerifyCreateMigrationTable(Times.Once());
        }

        [Fact(DisplayName = "New-MgMigrationTable default constructor constructs")]
        public void NewMgMigrationTable_Default_Constructor_Constructs()
        {
            var result = new NewMgMigrationTable
            {
                Database = "database",
                Password = "password",
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