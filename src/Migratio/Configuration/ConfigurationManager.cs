using System;
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
    }
}