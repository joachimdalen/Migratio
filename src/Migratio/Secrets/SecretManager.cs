using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Migratio.Configuration;
using Migratio.Contracts;
using Migratio.Models;
using Migratio.Utils;

namespace Migratio.Secrets
{
    public class SecretManager : ISecretManager
    {
        private const string SecretPattern = @"\${{(?<variable>[a-z_A-Z]+)}}";
        private readonly IEnvironmentManager _environmentManager;
        private readonly IFileManager _fileManager;
        private readonly IConfiguration _configuration;

        public SecretManager(IEnvironmentManager environmentManager, IFileManager fileManager,
            IConfiguration configuration)
        {
            _environmentManager = environmentManager;
            _fileManager = fileManager;
            _configuration = configuration;
        }

        public SecretManager()
        {
            _environmentManager = new EnvironmentManager();
            _fileManager = new FileManager();
            _configuration = new ConfigurationManager(_fileManager);
        }

        public string[] GetSecretsInContent(string content)
        {
            var usedVariables = new List<string>();

            foreach (Match m in Regex.Matches(content, SecretPattern, RegexOptions.Multiline))
            {
                var grp = m.Groups["variable"];
                var variableValue = grp.Value;

                if (!usedVariables.Contains(variableValue))
                    usedVariables.Add(variableValue);
            }

            return usedVariables.ToArray();
        }

        public string ReplaceSecretsInContent(string content, string envFilePath)
        {
            var usedVariables = GetSecretsInContent(content);
            var replacedContent = content;
            foreach (var usedVariable in usedVariables)
            {
                var envForPattern = GetEnvironmentVariable(usedVariable, envFilePath);
                if (string.IsNullOrWhiteSpace(envForPattern))
                    throw new Exception($"Failed to get environment variable for {usedVariable}");

                var givenPattern = "${{" + usedVariable + "}}";
                replacedContent = replacedContent.Replace(givenPattern, envForPattern);
            }

            return replacedContent;
        }

        public string GetEnvironmentVariable(string key, string envFilePath = null)
        {
            var envKey = _configuration.GetKeyFromMapping(key);
            var envForPattern = _environmentManager.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrWhiteSpace(envForPattern)) return envForPattern;

            if (envFilePath == null) return string.Empty;

            var items = GetFromFile(envFilePath);

            var envVar = items.FirstOrDefault(x => x.Key.Equals(key));

            if (envVar == null || string.IsNullOrEmpty(envVar?.Value)) return string.Empty;

            return envVar?.Value;
        }

        private IList<EnvEntry> GetFromFile(string path)
        {
            const string pattern = @"^\s*(?<key>[\w.-]+)\s*=\s*(?<value>.*)?\s*$";
            var parsed = new List<EnvEntry>();
            var content = _fileManager.ReadLines(path);

            foreach (var envVar in content)
            {
                var m = Regex.Match(envVar, pattern);
                parsed.Add(new EnvEntry
                {
                    Key = m.Groups["key"].Value,
                    Value = m.Groups["value"].Value,
                });
            }

            return parsed;
        }

        public bool HasSecret(string value) => Regex.IsMatch(value, SecretPattern);
    }
}