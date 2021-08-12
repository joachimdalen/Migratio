using System;
using System.Linq;
using Migratio.Contracts;
using Migratio.Models;
using Migratio.Results;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class InvokeMgRolloutTests : BaseCmdletTest
    {
        public InvokeMgRolloutTests()
        {
            ConfigManagerMock.Resolve(null, null, "migrations", "migrations");
            ConfigManagerMock.Resolve(null, null, "migrations/rollout", "migrations/rollout");
            ConfigManagerMock.Resolve<bool?>(null, false, false, false);
        }

        [Fact(DisplayName =
            "Invoke-MgRollout throws if migration table does not exist and CreateTableIfNotExist is false")]
        public void InvokeMgRollout_Throws_If_Migration_Table_Does_Not_Exist_And_CreateTableIfNotExist_Is_False()
        {
            ConfigManagerMock.RolloutDirectory("migrations");
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(false);

            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.Throws<Exception>(() => command.Invoke()?.OfType<bool>()?.First());
        }

        [Fact(DisplayName = "Invoke-MgRollout creates migration table if CreateTableIfNotExist is true")]
        public void InvokeMgRollout_Creates_Migration_Table_If_CreateTableIfNotExist_Is_True()
        {
            ConfigManagerMock.ConfigReturns(null);
            ConfigManagerMock.RollbackDirectory("migrations/rollback");
            DbMock.MigrationTableExists(false);
            DbMock.CreateMigrationTable(1);
            FileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            ConfigManagerMock.RolloutDirectory("migrations/rollout");

            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
                CreateTableIfNotExist = true
            };
            command.Invoke()?.OfType<string>()?.ToArray();
            DbMock.VerifyCreateMigrationTable(Times.Once());
        }

        [Fact(DisplayName = "Invoke-MgRollout returns if no scripts found")]
        public void InvokeMgRollout_Returns_If_No_Scripts_Found()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            FileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            ConfigManagerMock.RolloutDirectory("migrations/rollout");

            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<MgResult>()?.First();
            Assert.False(result.Successful);
        }

        [Fact(DisplayName = "Invoke-MgRollout returns if all scripts are applied")]
        public void InvokeMgRollout_Returns_If_All_Scripts_Are_Applied()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            ConfigManagerMock.RolloutDirectory("migrations/rollout");
            FileManagerMock.GetAllFilesInFolder(new[] {"migrations/one.sql", "migrations/two.sql"});
            DbMock.GetAppliedMigrations(new Migration[]
            {
                new() {Iteration = 1, MigrationId = "one.sql"},
                new() {Iteration = 1, MigrationId = "two.sql"}
            });


            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();
            Assert.Equal("Number of applied migrations are the same as the total, skipping", result[3]);
        }

        [Fact(DisplayName = "Invoke-MgRollout skips migration if applied")]
        public void InvokeMgRollout_Skips_Migration_If_Applied()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(1);

            ConfigManagerMock.RolloutDirectory("migrations/rollout");
            FileManagerMock.GetAllFilesInFolder(new[] {"migrations/rollout/one.sql", "migrations/rollout/two.sql"});
            FileManagerMock.ReadAllText("migrations/rollout/two.sql", "SELECT 1 from 2");

            DbMock.RunTransactionAny(1);
            DbMock.GetAppliedMigrations(new Migration[]
            {
                new() {Iteration = 1, MigrationId = "one"}
            });


            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();
            Assert.Equal("Migration one is applied, skipping", result[3]);

            FileManagerMock.VerifyReadAllText("migrations/rollout/one.sql", Times.Never());
        }

        [Fact(DisplayName = "Invoke-MgRollout should replace variables")]
        public void InvokeMgRollout_Should_Replace_Variables()
        {
            ConfigManagerMock.ConfigReturns(null);
            ConfigManagerMock.GetKeyFromMapping("TEST_ITEM_VARIABLE", "TEST_ITEM_VARIABLE");
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(1);

            ConfigManagerMock.RolloutDirectory("migrations/rollout");
            FileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/rollout/one.sql",
            });
            EnvironmentManagerMock.GetEnvironmentVariable("TEST_ITEM_VARIABLE", "ReplacedValue");
            FileManagerMock.ReadAllText("migrations/rollout/one.sql", "SELECT 1 from '${{TEST_ITEM_VARIABLE}}'");

            DbMock.RunTransactionAny(1);
            DbMock.GetAppliedMigrations(Array.Empty<Migration>());


            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
                ReplaceVariables = true
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();

            Assert.Contains("Migration one is not applied adding to transaction", result);
            DbMock.VerifyRunTransaction(
                "SELECT 1 from 'ReplacedValue';" + Environment.NewLine +
                "INSERT INTO \"public\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('one', 2);" +
                Environment.NewLine);
        }

        [Fact(DisplayName = "Invoke-MgRollout runs as one transaction if false")]
        public void InvokeMgRollout_Runs_As_One_Transaction_If_False()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(1);

            ConfigManagerMock.RolloutDirectory("migrations/rollout");
            FileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/rollout/one.sql",
                "migrations/rollout/two.sql",
                "migrations/rollout/three.sql"
            });
            FileManagerMock.ReadAllText("migrations/rollout/two.sql", "SELECT 1 from 2");
            FileManagerMock.ReadAllText("migrations/rollout/three.sql", "SELECT 3 from 4");

            DbMock.RunTransactionAny(1);
            DbMock.GetAppliedMigrations(new Migration[]
            {
                new() {Iteration = 1, MigrationId = "one"}
            });


            var command = new InvokeMgRollout(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();

            Assert.Contains("Migration two is not applied adding to transaction", result);
            Assert.Contains("Migration three is not applied adding to transaction", result);
            FileManagerMock.VerifyReadAllText("migrations/rollout/one.sql", Times.Never());
            var transaction =
                "SELECT 3 from 4;" + Environment.NewLine +
                "INSERT INTO \"public\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('three', 2);" +
                Environment.NewLine +
                "SELECT 1 from 2;" + Environment.NewLine +
                "INSERT INTO \"public\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('two', 2);" +
                Environment.NewLine;
            DbMock.VerifyRunTransaction(transaction);
        }

        [Fact(DisplayName = "Invoke-MgRollout default constructor constructs")]
        public void InvokeMgRollout_Default_Constructor_Constructs()
        {
            var result = new InvokeMgRollout
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