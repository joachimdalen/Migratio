using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Migratio.Contracts;
using Migratio.Utils;

namespace Migratio.Secrets
{
    public class SecretManager
    {
        private readonly IEnvironmentManager _environmentManager;

        public SecretManager(IEnvironmentManager environmentManager)
        {
            _environmentManager = environmentManager;
        }

        public SecretManager()
        {
            _environmentManager = new EnvironmentManager();
        }

        private const string SecretPattern = @"\${{(?<variable>[a-z_A-Z]+)}}";

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

        public string ReplaceSecretsInContent(string content)
        {
            var usedVariables = GetSecretsInContent(content);
            var replacedContent = content;
            foreach (var usedVariable in usedVariables)
            {
                var envForPattern = _environmentManager.GetEnvironmentVariable(usedVariable);
                if (string.IsNullOrWhiteSpace(envForPattern))
                {
                    throw new Exception($"Failed to get environment variable for {usedVariable}");
                }

                var givenPattern = "${{" + usedVariable + "}}";
                replacedContent = replacedContent.Replace(givenPattern, envForPattern);
            }

            return replacedContent;
        }
    }
}