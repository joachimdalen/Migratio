using System.Linq;
using Migratio.Contracts;
using Migratio.Models;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class GetMgLatestIterationTests
    {
        [Fact(DisplayName = "Get-MgLatestIteration returns null if migration table does not exist")]
        public void GetMgLatestIteration_Returns_Null_If_Migration_Table_Does_Not_Exist()
        {
            var dbMock = new Mock<IDatabaseProvider>(MockBehavior.Strict);
            dbMock.Setup(x => x.MigrationTableExists()).Returns(false);
            var command = new GetMgLatestIteration(dbMock.Object)
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

        [Fact(DisplayName = "Get-MgLatestIteration returns records")]
        public void GetMgLatestIteration_Returns_Records()
        {
            var dbMock = new Mock<IDatabaseProvider>(MockBehavior.Strict);
            dbMock.Setup(x => x.MigrationTableExists()).Returns(true);
            dbMock.Setup(x => x.GetLatestIteration()).Returns(1);
            var command = new GetMgLatestIteration(dbMock.Object)
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

        [Fact(DisplayName = "Get-MgLatestIteration default constructor constructs")]
        public void GetMgLatestIteration_Default_Constructor_Constructs()
        {
            var result = new GetMgLatestIteration
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