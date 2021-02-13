using System;
using System.Linq;
using Migratio.Contracts;
using Migratio.Models;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class InvokeMgRolloutTests
    {
        private readonly DatabaseProviderMock _dbMock;
        private readonly FileManagerMock _fileManagerMock;
        private readonly EnvironmentManagerMock _envMock;

        public InvokeMgRolloutTests()
        {
            _dbMock = new DatabaseProviderMock();
            _fileManagerMock = new FileManagerMock();
            _envMock = new EnvironmentManagerMock();
        }

        [Fact(DisplayName = "Invoke-MgRollout throws if migration table does not exist")]
        public void InvokeMgRollout_Throws_If_Migration_Table_Does_Not_Exist()
        {
            _dbMock.MigrationTableExists(false);

            var command = new InvokeMgRollout(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.Throws<Exception>(() => command.Invoke()?.OfType<bool>()?.First());
        }

        [Fact(DisplayName = "Invoke-MgRollout returns if no scripts found")]
        public void InvokeMgRollout_Returns_If_No_Scripts_Found()
        {
            _dbMock.MigrationTableExists(true);
            _fileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            _fileManagerMock.RolloutDirectory("mig/rol");

            var command = new InvokeMgRollout(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<bool>()?.First();
            Assert.False(result);
        }

        [Fact(DisplayName = "Invoke-MgRollout returns if all scripts are applied")]
        public void InvokeMgRollout_Returns_If_All_Scripts_Are_Applied()
        {
            _dbMock.MigrationTableExists(true);
            _fileManagerMock.RolloutDirectory("mig/rol");
            _fileManagerMock.GetAllFilesInFolder(new[] {"migrations/one.sql", "migrations/two.sql"});
            _dbMock.GetAppliedMigrations(new Migration[]
            {
                new() {Iteration = 1, MigrationId = "one.sql"},
                new() {Iteration = 1, MigrationId = "two.sql"}
            });


            var command = new InvokeMgRollout(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();
            Assert.Equal("Number of applied migrations are the same as the total, skipping", result[2]);
        }

        [Fact(DisplayName = "Invoke-MgRollout skips migration if applied")]
        public void InvokeMgRollout_Skips_Migration_If_Applied()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(1);

            _fileManagerMock.RolloutDirectory("migrations/rollout");
            _fileManagerMock.GetAllFilesInFolder(new[] {"migrations/rollout/one.sql", "migrations/rollout/two.sql"});
            _fileManagerMock.ReadAllText("migrations/rollout/two.sql", "SELECT 1 from 2");

            _dbMock.RunTransactionAny(1);
            _dbMock.GetAppliedMigrations(new Migration[]
            {
                new() {Iteration = 1, MigrationId = "one"}
            });


            var command = new InvokeMgRollout(_dbMock.Object, _fileManagerMock.MockInstance.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();
            Assert.Equal("Migration one is applied, skipping", result[2]);

            _fileManagerMock.VerifyReadAllText("migrations/rollout/one.sql", Times.Never());
        }

        [Fact(DisplayName = "Invoke-MgRollout should replace variables")]
        public void InvokeMgRollout_Should_Replace_Variables()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(1);

            _fileManagerMock.RolloutDirectory("migrations/rollout");
            _fileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/rollout/one.sql",
            });
            _envMock.GetEnvironmentVariable("TEST_ITEM_VARIABLE", "ReplacedValue");
            _fileManagerMock.ReadAllText("migrations/rollout/one.sql", "SELECT 1 from '${{TEST_ITEM_VARIABLE}}'");

            _dbMock.RunTransactionAny(1);
            _dbMock.GetAppliedMigrations(Array.Empty<Migration>());


            var command = new InvokeMgRollout(_dbMock.Object, _fileManagerMock.MockInstance.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
                ReplaceVariables = true
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();

            Assert.Contains("Migration one is not applied adding to transaction", result);
            _dbMock.VerifyRunTransaction(
                "SELECT 1 from 'ReplacedValue';" + Environment.NewLine +
                "INSERT INTO \"public\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('one', 2);" +
                Environment.NewLine);
        }

        [Fact(DisplayName = "Invoke-MgRollout runs as one transaction if false")]
        public void InvokeMgRollout_Runs_As_One_Transaction_If_False()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(1);

            _fileManagerMock.RolloutDirectory("migrations/rollout");
            _fileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/rollout/one.sql",
                "migrations/rollout/two.sql",
                "migrations/rollout/three.sql"
            });
            _fileManagerMock.ReadAllText("migrations/rollout/two.sql", "SELECT 1 from 2");
            _fileManagerMock.ReadAllText("migrations/rollout/three.sql", "SELECT 3 from 4");

            _dbMock.RunTransactionAny(1);
            _dbMock.GetAppliedMigrations(new Migration[]
            {
                new() {Iteration = 1, MigrationId = "one"}
            });


            var command = new InvokeMgRollout(_dbMock.Object, _fileManagerMock.MockInstance.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();

            Assert.Contains("Migration two is not applied adding to transaction", result);
            Assert.Contains("Migration three is not applied adding to transaction", result);
            _fileManagerMock.VerifyReadAllText("migrations/rollout/one.sql", Times.Never());
            var transaction =
                "SELECT 3 from 4;" + Environment.NewLine +
                "INSERT INTO \"public\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('three', 2);" +
                Environment.NewLine +
                "SELECT 1 from 2;" + Environment.NewLine +
                "INSERT INTO \"public\".\"MIGRATIONS\" (\"MIGRATION_ID\", \"ITERATION\") VALUES ('two', 2);" +
                Environment.NewLine;
            _dbMock.VerifyRunTransaction(transaction);
        }

        [Fact(DisplayName = "Invoke-MgRollout default constructor constructs")]
        public void InvokeMgRollout_Default_Constructor_Constructs()
        {
            var result = new InvokeMgRollout
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