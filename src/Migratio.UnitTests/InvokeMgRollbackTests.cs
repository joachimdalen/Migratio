using System;
using System.Linq;
using Migratio.Models;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class InvokeMgRollbackTests : BaseCmdletTest
    {
        public InvokeMgRollbackTests()
        {
            ConfigManagerMock.Resolve(null, null, "migrations", "migrations");
            ConfigManagerMock.Resolve(null, null, "migrations/rollback", "migrations/rollback");
        }

        [Fact(DisplayName = "Invoke-MgRollback throws if migration table does not exist")]
        public void InvokeMgRollback_Throws_If_Migration_Table_Does_Not_Exist()
        {
            ConfigManagerMock.ConfigReturns(null);
            ConfigManagerMock.RollbackDirectory("migrations/rollback");
            DbMock.MigrationTableExists(false);

            var command = new InvokeMgRollback(GetMockedDependencies())
            {
                Database = "database",
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
            ConfigManagerMock.ConfigReturns(null);

            DbMock.MigrationTableExists(true);
            FileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            ConfigManagerMock.RollbackDirectory("migrations/rollback");

            var command = new InvokeMgRollback(GetMockedDependencies())
            {
                Database = "database",
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
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(0);
            DbMock.GetAppliedScriptsForLatestIteration(new[] {new Migration {Iteration = 1, MigrationId = "one.sql"}});
            FileManagerMock.GetAllFilesInFolder(new[] {"migrations/rollback/one.sql"});
            ConfigManagerMock.RollbackDirectory("migrations/rollback");


            var command = new InvokeMgRollback(GetMockedDependencies())
            {
                Database = "database",
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
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(1);
            DbMock.GetAppliedScriptsForLatestIteration(Array.Empty<Migration>());
            FileManagerMock.GetAllFilesInFolder(new[] {"migrations/rollback/one.sql"});
            ConfigManagerMock.RollbackDirectory("migrations/rollback");


            var command = new InvokeMgRollback(GetMockedDependencies())
            {
                Database = "database",
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
            ConfigManagerMock.ConfigReturns(null);
            DbMock.MigrationTableExists(true);
            DbMock.GetLatestIteration(1);
            DbMock.GetAppliedScriptsForLatestIteration(new[]
            {
                new Migration {Iteration = 1, MigrationId = "two"},
                new Migration {Iteration = 1, MigrationId = "three"},
            });
            FileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/rollback/one.sql",
                "migrations/rollback/two.sql",
                "migrations/rollback/three.sql",
            });
            ConfigManagerMock.RollbackDirectory("migrations/rollback");
            FileManagerMock.ReadAllText("migrations/rollback/two.sql", "rollback 2");
            FileManagerMock.ReadAllText("migrations/rollback/three.sql", "rollback 3");
            DbMock.RunTransactionAny(1);

            var command = new InvokeMgRollback(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>().ToArray();
            Assert.Contains("Found 2 migrations applied in iteration 1", result);
            Assert.Contains("Migration one was not applied in latest iteration, skipping", result);
            Assert.Contains("Adding rollback of migration: two to transaction", result);
            FileManagerMock.VerifyReadAllText("migrations/rollback/one.sql", Times.Never());

            var transactions =
                "rollback 2;" + Environment.NewLine +
                "DELETE FROM \"public\".\"MIGRATIONS\" WHERE \"MIGRATION_ID\" = 'two' AND \"ITERATION\" = '1';" +
                Environment.NewLine + "rollback 3;" + Environment.NewLine +
                "DELETE FROM \"public\".\"MIGRATIONS\" WHERE \"MIGRATION_ID\" = 'three' AND \"ITERATION\" = '1';" +
                Environment.NewLine;


            DbMock.VerifyRunTransaction(transactions);
        }

        [Fact(DisplayName = "Invoke-MgRollback default constructor constructs")]
        public void InvokeMgRollback_Default_Constructor_Constructs()
        {
            var result = new InvokeMgRollback
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