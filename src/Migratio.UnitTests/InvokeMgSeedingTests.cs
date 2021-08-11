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
    public class InvokeMgSeedingTests : BaseCmdletTest
    {
        public InvokeMgSeedingTests()
        {
            ConfigManagerMock.Resolve(null, null, "migrations", "migrations");
            ConfigManagerMock.Resolve(null, null, "migrations/seeders", "migrations/seeders");
            ConfigManagerMock.Resolve<bool?>(null, false, false, false);
        }

        [Fact(DisplayName =
            "Invoke-MgSeeding throws if seeders table does not exist and CreateTableIfNotExist is false")]
        public void InvokeMgSeeding_Throws_If_Seeders_Table_Does_Not_Exist_And_CreateTableIfNotExist_Is_False()
        {
            ConfigManagerMock.SeedersDirectory("migrations");
            ConfigManagerMock.ConfigReturns(null);
            DbMock.SeedingTableExists(false);

            var command = new InvokeMgSeeding(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.Throws<Exception>(() => command.Invoke()?.OfType<bool>()?.First());
        }

        [Fact(DisplayName = "Invoke-MgSeeding creates seeders table if CreateTableIfNotExist is true")]
        public void InvokeMgSeeding_Creates_Seeders_Table_If_CreateTableIfNotExist_Is_True()
        {
            ConfigManagerMock.ConfigReturns(null);
            ConfigManagerMock.SeedersDirectory("migrations/seeders");
            DbMock.SeedingTableExists(false);
            DbMock.CreateSeedersTable(1);
            FileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            ConfigManagerMock.SeedersDirectory("migrations/seeders");

            var command = new InvokeMgSeeding(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
                CreateTableIfNotExist = true
            };
            command.Invoke()?.OfType<string>()?.ToArray();
            DbMock.VerifyCreateSeedersTable(Times.Once());
        }

        [Fact(DisplayName = "Invoke-MgSeeding returns if no scripts found")]
        public void InvokeMgSeeding_Returns_If_No_Scripts_Found()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.SeedingTableExists(true);
            FileManagerMock.GetAllFilesInFolder(Array.Empty<string>());
            ConfigManagerMock.SeedersDirectory("migrations/seeders");

            var command = new InvokeMgSeeding(GetMockedDependencies())
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

        [Fact(DisplayName = "Invoke-MgSeeding returns if all scripts are applied")]
        public void InvokeMgSeeding_Returns_If_All_Scripts_Are_Applied()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.SeedingTableExists(true);
            ConfigManagerMock.SeedersDirectory("migrations/seeders");
            FileManagerMock.GetAllFilesInFolder(new[] {"migrations/one.sql", "migrations/two.sql"});
            DbMock.GetAppliedSeeders(new Seed[]
            {
                new() {SeedId = "one.sql"},
                new() {SeedId = "two.sql"}
            });


            var command = new InvokeMgSeeding(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();
            Assert.Equal("Number of applied seeders are the same as the total, skipping", result[3]);
        }

        [Fact(DisplayName = "Invoke-MgSeeding skips seeders if applied")]
        public void InvokeMgSeeding_Skips_Seeders_If_Applied()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.SeedingTableExists(true);
            
            ConfigManagerMock.SeedersDirectory("migrations/seeders");
            FileManagerMock.GetAllFilesInFolder(new[] {"migrations/seeders/one.sql", "migrations/seeders/two.sql"});
            FileManagerMock.ReadAllText("migrations/seeders/two.sql", "SELECT 1 from 2");

            DbMock.RunTransactionAny(1);
            DbMock.GetAppliedSeeders(new Seed[]
            {
                new() {SeedId = "one"}
            });


            var command = new InvokeMgSeeding(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();
            Assert.Equal("Seeder one is applied, skipping", result[3]);

            FileManagerMock.VerifyReadAllText("migrations/seeders/one.sql", Times.Never());
        }

        [Fact(DisplayName = "Invoke-MgSeeding should replace variables")]
        public void InvokeMgSeeding_Should_Replace_Variables()
        {
            ConfigManagerMock.ConfigReturns(null);
            ConfigManagerMock.GetKeyFromMapping("TEST_ITEM_VARIABLE", "TEST_ITEM_VARIABLE");
            DbMock.SeedingTableExists(true);
            
            ConfigManagerMock.SeedersDirectory("migrations/seeders");
            FileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/seeders/one.sql",
            });
            EnvironmentManagerMock.GetEnvironmentVariable("TEST_ITEM_VARIABLE", "ReplacedValue");
            FileManagerMock.ReadAllText("migrations/seeders/one.sql", "SELECT 1 from '${{TEST_ITEM_VARIABLE}}'");

            DbMock.RunTransactionAny(1);
            DbMock.GetAppliedSeeders(Array.Empty<Seed>());


            var command = new InvokeMgSeeding(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username",
                ReplaceVariables = true
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();

            Assert.Contains("Seeder one is not applied adding to transaction", result);
            DbMock.VerifyRunTransaction(
                "SELECT 1 from 'ReplacedValue';" + Environment.NewLine +
                "INSERT INTO \"public\".\"SEEDERS\" (\"SEED_ID\") VALUES ('one');" +
                Environment.NewLine);
        }

        [Fact(DisplayName = "Invoke-MgSeeding runs as one transaction if false")]
        public void InvokeMgSeeding_Runs_As_One_Transaction_If_False()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.SeedingTableExists(true);
            
            ConfigManagerMock.SeedersDirectory("migrations/seeders");
            FileManagerMock.GetAllFilesInFolder(new[]
            {
                "migrations/seeders/one.sql",
                "migrations/seeders/two.sql",
                "migrations/seeders/three.sql"
            });
            FileManagerMock.ReadAllText("migrations/seeders/two.sql", "SELECT 1 from 2");
            FileManagerMock.ReadAllText("migrations/seeders/three.sql", "SELECT 3 from 4");

            DbMock.RunTransactionAny(1);
            DbMock.GetAppliedSeeders(new Seed[]
            {
                new() {SeedId = "one"}
            });


            var command = new InvokeMgSeeding(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<string>()?.ToArray();

            Assert.Contains("Seeder two is not applied adding to transaction", result);
            Assert.Contains("Seeder three is not applied adding to transaction", result);
            FileManagerMock.VerifyReadAllText("migrations/seeders/one.sql", Times.Never());
            var transaction =
                "SELECT 3 from 4;" + Environment.NewLine +
                "INSERT INTO \"public\".\"SEEDERS\" (\"SEED_ID\") VALUES ('three');" +
                Environment.NewLine +
                "SELECT 1 from 2;" + Environment.NewLine +
                "INSERT INTO \"public\".\"SEEDERS\" (\"SEED_ID\") VALUES ('two');" +
                Environment.NewLine;
            DbMock.VerifyRunTransaction(transaction);
        }

        [Fact(DisplayName = "Invoke-MgSeeding default constructor constructs")]
        public void InvokeMgSeeding_Default_Constructor_Constructs()
        {
            var result = new InvokeMgSeeding
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