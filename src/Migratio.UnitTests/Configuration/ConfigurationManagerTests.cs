using System;
using System.Linq;
using Migratio.Configuration;
using Migratio.UnitTests.Mocks;
using Xunit;

namespace Migratio.UnitTests.Configuration
{
    public class ConfigurationManagerTests
    {
        private readonly FileManagerMock _fileManagerMock;

        public ConfigurationManagerTests()
        {
            _fileManagerMock = new FileManagerMock();
        }

        [Fact(DisplayName = "Load returns false if no file")]
        public void Load_Returns_False_If_No_File()
        {
            var sut = new ConfigurationManager(_fileManagerMock.Object);

            var result = sut.Load(null);

            Assert.False(result);
        }

        [Fact(DisplayName = "Load fills object correctly")]
        public void Load_Fills_Object_Correctly()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);

            var result = sut.Load("migratio.yml");

            Assert.True(result);

            var config = sut.Config;
            Assert.Equal("/dev/migrations", config.Directories.Base);
            Assert.Equal("/dev/migrations/rollout", config.Directories.Rollout);
            Assert.Equal("/dev/migrations/rollback", config.Directories.Rollback);
            Assert.Equal("/dev/migrations/seeders", config.Directories.Seeders);
            Assert.Equal("MG_DB_PASSWORD", config.EnvMapping.FirstOrDefault().Key);
            Assert.Equal("DB_USERNAME", config.EnvMapping.FirstOrDefault().Value);
            Assert.Equal("./backend.env", config.EnvFile);
            Assert.Equal("TestDB", config.Auth.Postgres.Database);
            Assert.Equal("localhost", config.Auth.Postgres.Host);
            Assert.Equal(1234, config.Auth.Postgres.Port);
            Assert.Equal("postgres", config.Auth.Postgres.Username);
            Assert.Equal("${{MG_DB_PASSWORD}}", config.Auth.Postgres.Password);
            Assert.True(config.ReplaceVariables);
        }

        [Fact(DisplayName = "GetKeyFromMapping returns itemKey when no mapping is set")]
        public void GetKeyFromMapping_Returns_ItemKey_When_No_Mapping()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml");
            sut.Config.EnvMapping = null;

            var result = sut.GetKeyFromMapping("MG_DB_PASSWORD");

            Assert.Equal("MG_DB_PASSWORD", result);
        }

        [Fact(DisplayName = "GetKeyFromMapping returns itemKey when no matches are found")]
        public void GetKeyFromMapping_Returns_ItemKey_When_No_Matches_Are_Found()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml");

            var result = sut.GetKeyFromMapping("MG_DB_USERNAME");

            Assert.Equal("MG_DB_USERNAME", result);
        }

        [Fact(DisplayName = "GetKeyFromMapping returns mapped key")]
        public void GetKeyFromMapping_Returns_Mapped_Key()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml");

            var result = sut.GetKeyFromMapping("MG_DB_PASSWORD");

            Assert.Equal("DB_USERNAME", result);
        }

        [Fact(DisplayName = "GetKeyFromMapping throws if mapping contains variable")]
        public void GetKeyFromMapping_Throws_If_Mapping_Contains_Variable()
        {
            string wrongContent = @"directories:
  base: /dev/migrations # Path to base directory containing subfolders
  rollout: /dev/migrations/rollout # Path
  rollback: /dev/migrations/rollback
  seeders: /dev/migrations/seeders
envMapping:
  MG_DB_PASSWORD: ${{MY_VAR}}
envFile: './backend.env'
auth:
  postgres:
    host: 'localhost'
    port: 1234
    database: 'TestDB'
    username: 'postgres'
    password: '${{MG_DB_PASSWORD}}'
replaceVariables: true
";
            _fileManagerMock.ReadAllText("migratio.yml", wrongContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml");

            Assert.Throws<Exception>(() => sut.GetKeyFromMapping("MG_DB_PASSWORD"));
        }

        private string configContent = @"directories:
  base: /dev/migrations # Path to base directory containing subfolders
  rollout: /dev/migrations/rollout # Path
  rollback: /dev/migrations/rollback
  seeders: /dev/migrations/seeders
envMapping:
  MG_DB_PASSWORD: DB_USERNAME
envFile: './backend.env'
auth:
  postgres:
    host: 'localhost'
    port: 1234
    database: 'TestDB'
    username: 'postgres'
    password: '${{MG_DB_PASSWORD}}'
replaceVariables: true
";
    }
}