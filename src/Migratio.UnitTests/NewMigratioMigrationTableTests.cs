using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Migratio.Contracts;
using Migratio.Database;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMigratioMigrationTableTests
    {
        [Fact(DisplayName = "New-MigratioMigrationTable returns false if table exists")]
        public void NewMigratioMigrationTable_Returns_False_If_Table_Exists()
        {
            var dbMock = new Mock<IDatabaseProvider>(MockBehavior.Strict);
            dbMock.Setup(x => x.MigrationTableExists()).Returns(true);

            var command = new NewMigratioMigrationTable(dbMock.Object)
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
            dbMock.Verify(x => x.MigrationTableExists(), Times.Once);
            dbMock.Verify(x => x.CreateMigrationTable(), Times.Never);
        }

        [Fact(DisplayName = "New-MigratioMigrationTable returns true if table was created")]
        public void NewMigratioMigrationTable_Returns_True_If_Table_Was_Created()
        {
            var dbMock = new Mock<IDatabaseProvider>(MockBehavior.Strict);
            dbMock.Setup(x => x.MigrationTableExists()).Returns(false);
            dbMock.Setup(x => x.CreateMigrationTable()).Returns(1);

            var command = new NewMigratioMigrationTable(dbMock.Object)
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
            dbMock.Verify(x => x.MigrationTableExists(), Times.Once);
            dbMock.Verify(x => x.CreateMigrationTable(), Times.Once);
        }

        [Fact(DisplayName = "New-MigratioMigrationTable default constructor constructs")]
        public void NewMigratioMigrationTable_Default_Constructor_Constructs()
        {
            var result = new NewMigratioMigrationTable
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
//     typeof(NewMigratioMigrationTable), null));
// using (var ps = PowerShell.Create(iss))
// {
//     ps.AddCommand("New-MigrationMigrationTable").AddParameter("ClassName", "Win32_BIOS");
//     var res = ps.Invoke().FirstOrDefault();
//     Assert.NotNull(res);
//     Assert.False(ps.HadErrors);
// }