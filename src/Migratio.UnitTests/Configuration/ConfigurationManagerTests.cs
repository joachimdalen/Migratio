using System;
using System.Linq;
using Migratio.Configuration;
using Migratio.Models;
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

        private string configMultiEnvContent = @"environments:
  development:
    directories:
      base: /dev/migrations
      rollout: /dev/migrations/rollout
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
  production:
    directories:
      base: /prod/migrations
      rollout: /prod/migrations/rollout
      rollback: /prod/migrations/rollback
      seeders: /prod/migrations/seeders
    envMapping:
      MG_DB_PASSWORD: DB_PROD_USERNAME
    envFile: './backend.prod.env'
    auth:
      postgres:
        host: 'localhost'
        port: 1234
        database: 'ProdDB'
        username: 'produser'
        password: '${{MG_DB_PASSWORD}}'
    replaceVariables: true
";

        [Fact(DisplayName = "Load returns false if no file")]
        public void Load_Returns_False_If_No_File()
        {
            var sut = new ConfigurationManager(_fileManagerMock.Object);

            var result = sut.Load(null, "default");

            Assert.False(result);
        }

        [Fact(DisplayName = "Load fills object correctly")]
        public void Load_Fills_Object_Correctly()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);

            var result = sut.Load("migratio.yml", "default");

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

        [Fact(DisplayName = "Load fills object with environment correctly")]
        public void Load_Fills_Nested_Object_Correctly()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configMultiEnvContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);

            var result = sut.Load("migratio.yml", "production");

            Assert.True(result);

            var config = sut.Config;
            Assert.Equal("/prod/migrations", config.Directories.Base);
            Assert.Equal("/prod/migrations/rollout", config.Directories.Rollout);
            Assert.Equal("/prod/migrations/rollback", config.Directories.Rollback);
            Assert.Equal("/prod/migrations/seeders", config.Directories.Seeders);
            Assert.Equal("MG_DB_PASSWORD", config.EnvMapping.FirstOrDefault().Key);
            Assert.Equal("DB_PROD_USERNAME", config.EnvMapping.FirstOrDefault().Value);
            Assert.Equal("./backend.prod.env", config.EnvFile);
            Assert.Equal("ProdDB", config.Auth.Postgres.Database);
            Assert.Equal("localhost", config.Auth.Postgres.Host);
            Assert.Equal(1234, config.Auth.Postgres.Port);
            Assert.Equal("produser", config.Auth.Postgres.Username);
            Assert.Equal("${{MG_DB_PASSWORD}}", config.Auth.Postgres.Password);
            Assert.True(config.ReplaceVariables);
        }

        [Fact(DisplayName = "Throws when environment is not found")]
        public void Load_Throws_When_Environment_Not_Found()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configMultiEnvContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);


            Assert.Throws<Exception>(() => sut.Load("migratio.yml", "staging"));
        }

        [Fact(DisplayName = "GetKeyFromMapping returns itemKey when no mapping is set")]
        public void GetKeyFromMapping_Returns_ItemKey_When_No_Mapping()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml", "default");
            sut.Config.EnvMapping = null;

            var result = sut.GetKeyFromMapping("MG_DB_PASSWORD");

            Assert.Equal("MG_DB_PASSWORD", result);
        }

        [Fact(DisplayName = "GetKeyFromMapping returns itemKey when no matches are found")]
        public void GetKeyFromMapping_Returns_ItemKey_When_No_Matches_Are_Found()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml", "default");

            var result = sut.GetKeyFromMapping("MG_DB_USERNAME");

            Assert.Equal("MG_DB_USERNAME", result);
        }

        [Fact(DisplayName = "GetKeyFromMapping returns mapped key")]
        public void GetKeyFromMapping_Returns_Mapped_Key()
        {
            _fileManagerMock.ReadAllText("migratio.yml", configContent);
            var sut = new ConfigurationManager(_fileManagerMock.Object);
            sut.Load("migratio.yml", "default");

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
            sut.Load("migratio.yml", "default");

            Assert.Throws<Exception>(() => sut.GetKeyFromMapping("MG_DB_PASSWORD"));
        }

        [Theory(DisplayName = "RolloutDirectory returns correct path")]
        [InlineData("/mnt/dir1", null, "/mnt/dir1/migratio.yml", "/mnt/dir1/rollout")]
        [InlineData(null, "/mnt/dir2/rollout", "/mnt/dir1/migratio.yml", "/mnt/dir2/rollout")]
        [InlineData(null, "../rollout", "/mnt/dir3/migratio.yml", "/mnt/rollout")]
        [InlineData(null, "./rollout", "/mnt/dir4/migratio.yml", "/mnt/dir4/rollout")]
        public void RolloutDirectory_Returns_Correct_Path(string migrationBaseDir, string confPath,
            string configFilePath, string expected)
        {
            var sut = new ConfigurationManager(_fileManagerMock.Object)
            {
                Config = new MgConfig {Directories = new MgDirs {Rollout = confPath}}
            };

            var result = sut.GetMigratioDir(migrationBaseDir, configFilePath, MigratioDirectory.Rollout);

            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "RollbackDirectory returns correct path")]
        [InlineData("/mnt/dir1", null, "/mnt/dir1/migratio.yml", "/mnt/dir1/rollback")]
        [InlineData(null, "/mnt/dir2/rollback", "/mnt/dir1/migratio.yml", "/mnt/dir2/rollback")]
        [InlineData(null, "../rollback", "/mnt/dir3/migratio.yml", "/mnt/rollback")]
        [InlineData(null, "./rollback", "/mnt/dir4/migratio.yml", "/mnt/dir4/rollback")]
        public void RollbackDirectory_Returns_Correct_Path(string migrationBaseDir, string confPath,
            string configFilePath, string expected)
        {
            var sut = new ConfigurationManager(_fileManagerMock.Object)
            {
                Config = new MgConfig {Directories = new MgDirs {Rollback = confPath}}
            };

            var result = sut.GetMigratioDir(migrationBaseDir, configFilePath, MigratioDirectory.Rollback);

            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "SeedersDirectory returns correct path")]
        [InlineData("/mnt/dir1", null, "/mnt/dir1/migratio.yml", "/mnt/dir1/seeders")]
        [InlineData(null, "/mnt/dir2/seeders", "/mnt/dir1/migratio.yml", "/mnt/dir2/seeders")]
        [InlineData(null, "../seeders", "/mnt/dir3/migratio.yml", "/mnt/seeders")]
        [InlineData(null, "./seeders", "/mnt/dir4/migratio.yml", "/mnt/dir4/seeders")]
        public void SeedersDirectory_Returns_Correct_Path(string migrationBaseDir, string confPath,
            string configFilePath, string expected)
        {
            var sut = new ConfigurationManager(_fileManagerMock.Object)
            {
                Config = new MgConfig {Directories = new MgDirs {Seeders = confPath}}
            };

            var result = sut.GetMigratioDir(migrationBaseDir, configFilePath, MigratioDirectory.Seeders);

            Assert.Equal(expected, result);
        }
    }
}