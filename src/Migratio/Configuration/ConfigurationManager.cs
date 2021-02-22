using System;
using System.IO;
using System.Runtime.CompilerServices;
using Migratio.Contracts;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Migratio.Configuration
{
    public class ConfigurationManager : IConfiguration
    {
        public MgConfig Config { get; set; }

        private readonly IFileManager _fileManager;

        public ConfigurationManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public string GetKeyFromMapping(string itemKey)
        {
            if (Config?.EnvMapping == null) return itemKey;
            if (!Config.EnvMapping.ContainsKey(itemKey)) return itemKey;

            var res = Config.EnvMapping.TryGetValue(itemKey, out var val);

            if (res == false) return itemKey;

            if (val.StartsWith("${{"))
            {
                throw new Exception("Variables can not be used in the value of envMappings");
            }

            return val;
        }

        public bool Load(string configFile)
        {
            Console.WriteLine("Loading... " + configFile);
            if (string.IsNullOrEmpty(configFile))
            {
                return false;
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var config =
                deserializer.Deserialize<MgConfig>(_fileManager.ReadAllText(configFile));
            Config = config;
            return true;
        }

        public T Resolve<T>(T first, T second, T defaultValue)
        {
            if (first != null) return first;
            if (second != null) return second;
            return defaultValue;
        }

        public string RolloutDirectory(string migrationBaseDir, string configPath)
        {
            var configBase = Path.GetDirectoryName(configPath);
            var rollout = Config.Directories.Rollout;

            if (migrationBaseDir != null)
            {
                return Path.Combine(migrationBaseDir, "rollout");
            }

            if (Path.IsPathRooted(rollout))
            {
                return rollout;
            }

            if (configBase == null)
            {
                throw new Exception("Unable to determine rollout directory");
            }

            return Path.Combine(configBase, rollout);
        }

        public string RollbackDirectory(string migrationBaseDir, string configPath)
        {
            var configBase = Path.GetDirectoryName(configPath);
            var rollback = Config.Directories.Rollback;

            if (migrationBaseDir != null)
            {
                return Path.Combine(migrationBaseDir, "rollback");
            }

            if (Path.IsPathRooted(rollback))
            {
                return rollback;
            }

            if (configBase == null)
            {
                throw new Exception("Unable to determine rollback directory");
            }

            return Path.Combine(configBase, rollback);
        }

        public string SeedersDirectory(string migrationBaseDir, string configPath)
        {
            var configBase = Path.GetDirectoryName(configPath);
            var seeders = Config.Directories.Rollback;

            if (migrationBaseDir != null)
            {
                return Path.Combine(migrationBaseDir, "seeders");
            }

            if (Path.IsPathRooted(seeders))
            {
                return seeders;
            }

            if (configBase == null)
            {
                throw new Exception("Unable to determine seeders directory");
            }

            return Path.Combine(configBase, seeders);
        }
    }
}