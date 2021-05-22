using System;
using Migratio.Core.Contracts;
using Migratio.Core.Secrets;

namespace Migratio.Core.Utils
{
    public class MigrationHelper : IMigrationHelper
    {
        private readonly IFileManager _fileManager;
        private readonly ISecretManager _secretManager;

        public MigrationHelper(IFileManager fileManager, IEnvironmentManager environmentManager,
            IMigratioConfiguration migratioConfiguration)
        {
            _fileManager = fileManager;
            _secretManager = new SecretManager(environmentManager, _fileManager, migratioConfiguration);
        }

        /// <summary>
        /// Get file content with or without replaced variables
        /// </summary>
        /// <param name="scriptPath">Path to script to load</param>
        /// <param name="replace">Whether variables should be replaced</param>
        /// <returns>File content</returns>
        public string GetScriptContent(string scriptPath, bool replace)
        {
            var scriptContent = _fileManager.ReadAllText(scriptPath);
            if (!scriptContent.EndsWith(";"))
                scriptContent += ";";

            if (!replace) return scriptContent + Environment.NewLine;

            var replacedContent = _secretManager.ReplaceVariablesInContent(scriptContent);
            return replacedContent + Environment.NewLine;
        }
    }
}