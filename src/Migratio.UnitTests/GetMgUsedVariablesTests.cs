using System;
using System.Linq;
using Migratio.UnitTests.Mocks;
using Migratio.Utils;
using Xunit;

namespace Migratio.UnitTests
{
    public class GetMgUsedVariablesTests : BaseCmdletTest
    {
        [Fact(DisplayName = "Get-MgUsedVariables throws if file does not exist")]
        public void GetMgUsedVariables_Throws_If_File_Does_Not_Exist()
        {
            FileManagerMock.FileExists("migrations/rollout/one.sql", false);

            var command = new GetMgUsedVariables(GetMockedDependencies())
            {
                MigrationFile = "migrations/rollout/one.sql"
            };

            Assert.Throws<Exception>(() => command.Invoke()?.OfType<string[]>()?.First());
        }

        [Fact(DisplayName = "Get-MgUsedVariables returns empty when no variables is used")]
        public void GetMgUsedVariables_Returns_Empty_When_No_Variables_Is_Used()
        {
            FileManagerMock.FileExists("migrations/rollout/one.sql", true);
            FileManagerMock.ReadAllText("migrations/rollout/one.sql", "SELECT ME FROM YOU");

            var command = new GetMgUsedVariables(GetMockedDependencies())
            {
                MigrationFile = "migrations/rollout/one.sql"
            };

            var result = command.Invoke()?.OfType<string[]>()?.First();
            Assert.Empty(result);
        }
        
        [Fact(DisplayName = "Get-MgUsedVariables returns correct variables")]
        public void GetMgUsedVariables_Returns_Correct_Variables()
        {
            FileManagerMock.FileExists("migrations/rollout/one.sql", true);
            FileManagerMock.ReadAllText("migrations/rollout/one.sql", "SELECT ${{VAR_ME}} FROM ${{VAR_YOU}}");

            var command = new GetMgUsedVariables(GetMockedDependencies())
            {
                MigrationFile = "migrations/rollout/one.sql"
            };

            var result = command.Invoke()?.OfType<string[]>()?.First();
            Assert.Equal(new[] {"VAR_ME", "VAR_YOU"}, result);
        }


        [Fact(DisplayName = "Get-MgUsedVariables default constructor constructs")]
        public void GetMgUsedVariables_Default_Constructor_Constructs()
        {
            var result = new GetMgUsedVariables
            {
                MigrationFile = "migrations/rollout/one.sql"
            };

            Assert.NotNull(result);
        }
    }
}