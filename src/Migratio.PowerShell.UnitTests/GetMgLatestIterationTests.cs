using System.Linq;
using Migratio.PowerShell.Results;
using Xunit;

namespace Migratio.PowerShell.UnitTests
{
    public class GetMgLatestIterationTests : BaseCmdletTest
    {
        [Fact(DisplayName = "Get-MgLatestIteration returns null if migration table does not exist")]
        public void GetMgLatestIteration_Returns_Null_If_Migration_Table_Does_Not_Exist()
        {
            DbMock.MigrationTableExists(false);
            ConfigManagerMock.ConfigReturns(null);
            var command = new GetMgLatestIteration(GetMockedDependencies())
            {
                Database = "database",
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
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(1);
            ConfigManagerMock.ConfigReturns(null);
            var command = new GetMgLatestIteration(GetMockedDependencies())
            {
                Database = "database",
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
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.NotNull(result);
        }
    }
}