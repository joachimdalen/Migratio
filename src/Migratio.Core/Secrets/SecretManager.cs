using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Migratio.Core.Configuration;
using Migratio.Core.Contracts;
using Migratio.Core.Models;
using Migratio.Core.Utils;

namespace Migratio.Core.Secrets
{
    public class SecretManager : ISecretManager
    {
        private const string SecretPattern = @"\${{(?<variable>[a-z_A-Z]+)}}";
        private readonly IConfiguration _configuration;
        private readonly IEnvironmentManager _environmentManager;
        private readonly IFileManager _fileManager;

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

        /// <inheritdoc />
        public string[] GetVariablesInContent(string content)
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

        /// <inheritdoc />
        /// <exception cref="Exception">
        /// Failed to get environment variable for one or more variable
        /// </exception>
        public string ReplaceVariablesInContent(string content)
        {
            var usedVariables = GetVariablesInContent(content);
            var replacedContent = content;
            foreach (var usedVariable in usedVariables)
            {
                var envForPattern = GetEnvironmentVariable(usedVariable);
                if (string.IsNullOrWhiteSpace(envForPattern))
                    throw new Exception($"Failed to get environment variable for {usedVariable}");

                var givenPattern = "${{" + usedVariable + "}}";
                replacedContent = replacedContent.Replace(givenPattern, envForPattern);
            }

            return replacedContent;
        }

        /// <inheritdoc />
        public string GetEnvironmentVariable(string key)
        {
            var envKey = _configuration.GetKeyFromMapping(key);
            var envForPattern = _environmentManager.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrWhiteSpace(envForPattern)) return envForPattern;

            if (_configuration?.Config?.EnvFile == null) return string.Empty;

            var items = GetFromFile(_configuration?.Config?.EnvFile);

            var envVar = items.FirstOrDefault(x => x.Key.Equals(key));

            if (envVar == null || string.IsNullOrEmpty(envVar?.Value)) return string.Empty;

            return envVar?.Value;
        }

        /// <inheritdoc />
        public bool HasVariable(string content)
        {
            return Regex.IsMatch(content, SecretPattern);
        }

        /// <summary>
        /// Load environment file
        /// </summary>
        /// <param name="path">Path to environment file</param>
        /// <returns>Mapped variables</returns>
        private IEnumerable<EnvEntry> GetFromFile(string path)
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
                    Value = m.Groups["value"].Value
                });
            }

            return parsed;
        }
    }
}