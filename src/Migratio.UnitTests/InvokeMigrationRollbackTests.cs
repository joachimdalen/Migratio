using System;
using System.Linq;
using Migratio.Models;
using Migratio.UnitTests.Mocks;
using Xunit;

namespace Migratio.UnitTests
{
    public class InvokeMgRollbackTests
    {
        private readonly DatabaseProviderMock _dbMock;
        private readonly FileManagerMock _fileManagerMock;

        public InvokeMgRollbackTests()
        {
            _dbMock = new DatabaseProviderMock();
            _fileManagerMock = new FileManagerMock();
        }

        [Fact(DisplayName = "Invoke-MgRollback throws if migration table does not exist")]
        public void InvokeMgRollback_Throws_If_Migration_Table_Does_Not_Exist()
        {
            _dbMock.MigrationTableExists(false);

            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object)
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

            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object)
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


            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object)
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


            var command = new InvokeMgRollback(_dbMock.Object, _fileManagerMock.Object)
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