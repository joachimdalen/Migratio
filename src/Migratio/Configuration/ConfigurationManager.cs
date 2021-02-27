using System;
using System.IO;
using Migratio.Contracts;
using Migratio.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Migratio.Configuration
{
    public class ConfigurationManager : IConfiguration
    {
        private readonly IFileManager _fileManager;

        public ConfigurationManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public MgConfig Config { get; set; }

        public string GetKeyFromMapping(string itemKey)
        {
            if (Config?.EnvMapping == null) return itemKey;
            if (!Config.EnvMapping.ContainsKey(itemKey)) return itemKey;

            var res = Config.EnvMapping.TryGetValue(itemKey, out var val);

            if (res == false) return itemKey;

            if (val.StartsWith("${{")) throw new Exception("Variables can not be used in the value of envMappings");

            return val;
        }

        public bool Load(string configFile)
        {
            Console.WriteLine("Loading... " + configFile);
            if (string.IsNullOrEmpty(configFile)) return false;

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

        public string GetMigratioDir(string migrationBaseDir, string configPath, MigratioDirectory directoryType)
        {
            var configBase = Path.GetDirectoryName(configPath);
            var fromConfigFile = GetSystemDirFromConfig(directoryType);

            if (migrationBaseDir != null)
                return Path.GetFullPath(Path.Combine(migrationBaseDir, GetDefaultSystemDir(directoryType)));

            if (Path.IsPathRooted(fromConfigFile)) return fromConfigFile;

            if (configBase == null) throw new Exception("Unable to determine directory");

            return Path.GetFullPath(Path.Combine(configBase, fromConfigFile));
        }

        private string GetDefaultSystemDir(MigratioDirectory directoryType)
        {
            switch (directoryType)
            {
                case MigratioDirectory.Base:
                    return "migrations";
                case MigratioDirectory.Rollout:
                    return "rollout";
                case MigratioDirectory.Rollback:
                    return "rollback";
                case MigratioDirectory.Seeders:
                    return "seeders";
                default:
                    throw new ArgumentOutOfRangeException(nameof(directoryType), directoryType,
                        "Unknown directory type");
            }
        }

        private string GetSystemDirFromConfig(MigratioDirectory directoryType)
        {
            switch (directoryType)
            {
                case MigratioDirectory.Base:
                    return Config?.Directories?.Base;
                case MigratioDirectory.Rollout:
                    return Config?.Directories?.Rollout;
                case MigratioDirectory.Rollback:
                    return Config?.Directories?.Rollback;
                case MigratioDirectory.Seeders:
                    return Config?.Directories?.Seeders;
                default:
                    throw new ArgumentOutOfRangeException(nameof(directoryType), directoryType,
                        "Unknown directory type");
            }
        }
    }
}