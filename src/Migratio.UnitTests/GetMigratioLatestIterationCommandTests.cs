using System.Linq;
using Migratio.Contracts;
using Migratio.Models;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class GetMigratioLatestIterationCommandTests
    {
        [Fact(DisplayName = "Get-MigratioLatestIterationCommand returns null if migration table does not exist")]
        public void GetMigratioLatestIterationCommand_Returns_Null_If_Migration_Table_Does_Not_Exist()
        {
            var dbMock = new Mock<IDatabaseProvider>(MockBehavior.Strict);
            dbMock.Setup(x => x.MigrationTableExists()).Returns(false);
            var command = new GetMigratioLatestIterationCommand(dbMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<IterationResult>();
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Get-MigratioLatestIterationCommand returns records")]
        public void GetMigratioLatestIterationCommand_Returns_Records()
        {
            var dbMock = new Mock<IDatabaseProvider>(MockBehavior.Strict);
            dbMock.Setup(x => x.MigrationTableExists()).Returns(true);
            dbMock.Setup(x => x.GetLatestIteration()).Returns(1);
            var command = new GetMigratioLatestIterationCommand(dbMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<IterationResult>()?.First();
            Assert.NotNull(result);
            Assert.Equal(1, result.Iteration);
        }

        [Fact(DisplayName = "Get-MigratioLatestIterationCommand default constructor constructs")]
        public void GetMigratioLatestIterationCommand_Default_Constructor_Constructs()
        {
            var result = new GetMigratioLatestIterationCommand
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