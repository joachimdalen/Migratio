using System;
using System.Linq;
using Migratio.Models;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class InvokeMgRollbackTests
    {
        private readonly DatabaseProviderMock _dbMock;
        private readonly FileManagerMock _fileManagerMock;
        private readonly EnvironmentManagerMock _envMock;

        public InvokeMgRollbackTests()
        {
            _dbMock = new DatabaseProviderMock();
            _fileManagerMock = new FileManagerMock();
            _envMock = new EnvironmentManagerMock();
        }

        [Fact(DisplayName = "Invoke-MgRollback throws if migration table does not exist")]
        public void InvokeMgRollback_Throws_If_Migration_Table_Does_Not_Exist()
        {
            _dbMock.MigrationTableExists(false);

            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
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

        [Fact(DisplayName = "Invoke-MgRollback returns if no scripts found")]
        public void InvokeMgRollback_Returns_If_No_Scripts_Found()
        {
            _dbMock.MigrationTableExists(true);
            _fileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            _fileManagerMock.RollbackDirectory("migration/rollback");

            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
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

        [Fact(DisplayName = "Invoke-MgRollback returns if latest iteration is zero")]
        public void InvokeMgRollback_Returns_If_Latest_Iteration_Is_Zero()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(0);
            _dbMock.GetAppliedScriptsForLatestIteration(new[] {new Migration {Iteration = 1, MigrationId = "one.sql"}});
            _fileManagerMock.GetAllFilesInFolder(new[] {"migration/rollback/one.sql"});
            _fileManagerMock.RollbackDirectory("migration/rollback");


            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
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

        [Fact(DisplayName = "Invoke-MgRollback returns if applied migrations is zero")]
        public void InvokeMgRollback_Returns_If_Applied_Migrations_Is_Zero()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(1);
            _dbMock.GetAppliedScriptsForLatestIteration(Array.Empty<Migration>());
            _fileManagerMock.GetAllFilesInFolder(new[] {"migration/rollback/one.sql"});
            _fileManagerMock.RollbackDirectory("migration/rollback");


            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
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

        [Fact(DisplayName = "Invoke-MgRollback rollbacks correct migrations")]
        public void InvokeMgRollback_Rollbacks_Correct_Migrations()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(1);
            _dbMock.GetAppliedScriptsForLatestIteration(new[]
            {
                new Migration {Iteration = 1, MigrationId = "two"},
                new Migration {Iteration = 1, MigrationId = "three"},
            });
            _fileManagerMock.GetAllFilesInFolder(new[]
            {
                "migration/rollback/one.sql",
                "migration/rollback/two.sql",
                "migration/rollback/three.sql",
            });
            _fileManagerMock.RollbackDirectory("migration/rollback");
            _fileManagerMock.ReadAllText("migration/rollback/two.sql", "rollback 2");
            _fileManagerMock.ReadAllText("migration/rollback/three.sql", "rollback 3");
            _dbMock.RunTransactionAny(1);

            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object, _envMock.Object)
            {
                Database = "database",
                Password = "password",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>().ToArray();
            Assert.Contains("Found 2 migrations applied in iteration 1", result);
            Assert.Contains("Migration one was not applied in latest iteration, skipping", result);
            Assert.Contains("Adding rollback of migration: two to transaction", result);
            _fileManagerMock.VerifyReadAllText("migrations/rollback/one.sql", Times.Never());

            var transactions =
                "rollback 2;" + Environment.NewLine +
                "DELETE FROM \"public\".\"MIGRATIONS\" WHERE \"MIGRATION_ID\" = 'two' AND \"ITERATION\" = '1';" +
                Environment.NewLine + "rollback 3;" + Environment.NewLine +
                "DELETE FROM \"public\".\"MIGRATIONS\" WHERE \"MIGRATION_ID\" = 'three' AND \"ITERATION\" = '1';" +
                Environment.NewLine;


            _dbMock.VerifyRunTransaction(transactions);
        }

        [Fact(DisplayName = "Invoke-MgRollback default constructor constructs")]
        public void InvokeMgRollback_Default_Constructor_Constructs()
        {
            var result = new InvokeMgRollback
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