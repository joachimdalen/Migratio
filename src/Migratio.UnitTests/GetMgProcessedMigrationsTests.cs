using System;
using System.Collections.Generic;
using System.Linq;
using Migratio.Contracts;
using Migratio.Models;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class GetMgProcessedMigrationsTests : BaseCmdletTest
    {
        [Fact(DisplayName = "Get-MgProcessedMigrations returns records")]
        public void GetMgProcessedMigrations_Returns_Records()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            DbMock.GetAppliedMigrations(
                new List<Migration>
                {
                    new() {Iteration = 1, MigrationId = "0001_migration_1"},
                    new() {Iteration = 1, MigrationId = "0002_migration_2"},
                }.ToArray());
            var command = new GetMgProcessedMigrations(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<Migration[]>()?.First();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Length);
        }

        [Fact(DisplayName = "Get-MgProcessedMigrations throws if migration table does not exist")]
        public void GetMgProcessedMigrations_Throws_If_Migration_Table_Does_Not_Exist()
        {
            DbMock.MigrationTableExists(false);
            ConfigManagerMock.ConfigReturns(null);
            
            var command = new GetMgProcessedMigrations(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.Throws<Exception>(() => command.Invoke()?.OfType<Migration[]>()?.First());
        }

        [Fact(DisplayName = "Get-MgProcessedMigrations default constructor constructs")]
        public void GetMgProcessedMigrations_Default_Constructor_Constructs()
        {
            var result = new GetMgProcessedMigrations
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