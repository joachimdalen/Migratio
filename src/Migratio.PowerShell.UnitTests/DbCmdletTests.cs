using System.Collections.Generic;
using Migratio.Core.Configuration;
using Migratio.Core.Database;
using Migratio.PowerShell.Core;
using Xunit;

namespace Migratio.PowerShell.UnitTests
{
    public class DbCmdletMockItem : DbCmdlet
    {
        public DbCmdletMockItem() : base()
        {
        }

        public DbCmdletMockItem(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        public DbConnectionInfo GetConnectionDetails()
        {
            return GetConnectionInfo();
        }
    }

    public class DbCmdletTests : BaseCmdletTest
    {
        [Fact(DisplayName = "DbCmdlet returns cli value first")]
        public void DbCmdlet_Returns_Cli_Value_First()
        {
            ConfigManagerMock.ConfigReturns(null);
            var command = new DbCmdletMockItem(GetMockedDependencies())
            {
                Username = "cli_uname",
                Database = "cli_db",
                Host = "cli_host",
                Port = 1234,
                Schema = "cli_schema"
            };
            
            var coninfo = command.GetConnectionDetails();

            Assert.Equal("cli_uname", coninfo.Username);
            Assert.Equal("cli_db", coninfo.Database);
            Assert.Equal("password", coninfo.Password);
            Assert.Equal("cli_host", coninfo.Host);
            Assert.Equal(1234, coninfo.Port);
            Assert.Equal("cli_schema", coninfo.Schema);
        }

        [Fact(DisplayName = "DbCmdlet returns clean config values")]
        public void DbCmdlet_Returns_Clean_Config_Values()
        {
            ConfigManagerMock.ConfigReturns(new MgConfig
            {
                Directories = new MgDirs
                {
                    Base = "/dev/migrations",
                    Rollback = "/dev/migrations/rollback",
                    Rollout = "/dev/migrations/rollout",
                    Seeders = "/dev/migrations/seeders",
                },
                Auth = new MgAuth
                {
                    Postgres = new MgDb
                    {
                        Host = "localhost",
                        Port = 3333,
                        Database = "TestDB",
                        Password = "${{MG_DB_PASSWORD}}",
                        Schema = "public",
                        Username = "postgres"
                    }
                },
                EnvFile = ".backend.env",
                EnvMapping = new Dictionary<string, string>
                {
                    {"MG_DB_USERNAME", "DB_USERNAME"}
                },
                ReplaceVariables = true
            });
            var command = new DbCmdletMockItem(GetMockedDependencies()) {ConfigFile = "migratio.yml"};

            var coninfo = command.GetConnectionDetails();

            Assert.Equal("postgres", coninfo.Username);
            Assert.Equal("TestDB", coninfo.Database);
            Assert.Equal("password", coninfo.Password);
            Assert.Equal("localhost", coninfo.Host);
            Assert.Equal(3333, coninfo.Port);
            Assert.Equal("public", coninfo.Schema);
        }
    }
}