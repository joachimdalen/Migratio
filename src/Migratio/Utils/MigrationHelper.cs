using System;
using Migratio.Contracts;
using Migratio.Secrets;

namespace Migratio.Utils
{
    public class MigrationHelper
    {
        private readonly IFileManager _fileManager;
        private readonly ISecretManager _secretManager;

        public MigrationHelper(IFileManager fileManager, IEnvironmentManager environmentManager)
        {
            _fileManager = fileManager;
            _secretManager = new SecretManager(environmentManager, fileManager);
        }

        public string GetScriptContent(string scriptPath, bool replace, string envFilePath)
        {
            var scriptContent = _fileManager.ReadAllText(scriptPath);
            if (!scriptContent.EndsWith(";"))
                scriptContent += ";";

            if (!replace) return scriptContent + Environment.NewLine;

            var replacedContent = _secretManager.ReplaceSecretsInContent(scriptContent, envFilePath);
            return replacedContent + Environment.NewLine;
        }
    }
}