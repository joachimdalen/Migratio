using System;
using System.Linq;
using Migratio.Models;
using Migratio.UnitTests.Mocks;
using Xunit;

namespace Migratio.UnitTests
{
    public class InvokeMigrationRollbackTests
    {
        private readonly DatabaseProviderMock _dbMock;
        private readonly FileManagerMock _fileManagerMock;

        public InvokeMigrationRollbackTests()
        {
            _dbMock = new DatabaseProviderMock();
            _fileManagerMock = new FileManagerMock();
        }

        [Fact(DisplayName = "Invoke-MigrationRollback throws if migration table does not exist")]
        public void InvokeMigrationRollback_Throws_If_Migration_Table_Does_Not_Exist()
        {
            _dbMock.MigrationTableExists(false);

            var command = new InvokeMigratioRollback(_dbMock.Object, _fileManagerMock.Object)
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

        [Fact(DisplayName = "Invoke-MigrationRollback returns if no scripts found")]
        public void InvokeMigrationRollback_Returns_If_No_Scripts_Found()
        {
            _dbMock.MigrationTableExists(true);
            _fileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            _fileManagerMock.RollbackDirectory("migration/rollback");

            var command = new InvokeMigratioRollback(_dbMock.Object, _fileManagerMock.Object)
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

        [Fact(DisplayName = "Invoke-MigrationRollback returns if latest iteration is zero")]
        public void InvokeMigrationRollback_Returns_If_Latest_Iteration_Is_Zero()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(0);
            _dbMock.GetAppliedScriptsForLatestIteration(new[] {new Migration {Iteration = 1, MigrationId = "one.sql"}});
            _fileManagerMock.GetAllFilesInFolder(new[] {"migration/rollback/one.sql"});
            _fileManagerMock.RollbackDirectory("migration/rollback");


            var command = new InvokeMigratioRollback(_dbMock.Object, _fileManagerMock.Object)
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

        [Fact(DisplayName = "Invoke-MigrationRollback returns if applied migrations is zero")]
        public void InvokeMigrationRollback_Returns_If_Applied_Migrations_Is_Zero()
        {
            _dbMock.MigrationTableExists(true);
            _dbMock.GetLatestIteration(1);
            _dbMock.GetAppliedScriptsForLatestIteration(Array.Empty<Migration>());
            _fileManagerMock.GetAllFilesInFolder(new[] {"migration/rollback/one.sql"});
            _fileManagerMock.RollbackDirectory("migration/rollback");


            var command = new InvokeMigratioRollback(_dbMock.Object, _fileManagerMock.Object)
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

        [Fact(DisplayName = "Invoke-MigrationRollback default constructor constructs")]
        public void InvokeMigrationRollback_Default_Constructor_Constructs()
        {
            var result = new InvokeMigratioRollback
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