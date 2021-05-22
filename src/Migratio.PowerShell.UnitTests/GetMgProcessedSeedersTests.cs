using System;
using System.Collections.Generic;
using System.Linq;
using Migratio.Core.Models;
using Xunit;

namespace Migratio.PowerShell.UnitTests
{
    public class GetMgProcessedSeedersTests : BaseCmdletTest
    {
        [Fact(DisplayName = "Get-MgProcessedSeeders returns records")]
        public void GetMgProcessedSeeders_Returns_Records()
        {
            ConfigManagerMock.ConfigReturns(null);
            DbMock.SeedingTableExists(true);
            DbMock.GetAppliedSeeders(
                new List<Seed>
                {
                    new() {SeedId = "0001_seed_1"},
                    new() {SeedId = "0002_seed_2"},
                }.ToArray());
            var command = new GetMgProcessedSeeders(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            var result = command.Invoke()?.OfType<Seed[]>()?.First();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Length);
        }

        [Fact(DisplayName = "Get-MgProcessedSeeders throws if seeders table does not exist")]
        public void GetMgProcessedSeeders_Throws_If_Seeders_Table_Does_Not_Exist()
        {
            DbMock.SeedingTableExists(false);
            ConfigManagerMock.ConfigReturns(null);
            
            var command = new GetMgProcessedSeeders(GetMockedDependencies())
            {
                Database = "database",
                Host = "host",
                Port = 1111,
                Schema = "public",
                Username = "username"
            };

            Assert.Throws<Exception>(() => command.Invoke()?.OfType<Seed[]>()?.First());
        }

        [Fact(DisplayName = "Get-MgProcessedSeeders default constructor constructs")]
        public void GetMgProcessedSeeders_Default_Constructor_Constructs()
        {
            var result = new GetMgProcessedSeeders
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