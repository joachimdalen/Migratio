using System;
using Migratio.Contracts;
using Migratio.Secrets;

namespace Migratio.Utils
{
    public class MigrationHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IFileManager _fileManager;
        private readonly ISecretManager _secretManager;

        public MigrationHelper(IFileManager fileManager, IEnvironmentManager environmentManager,
            IConfiguration configuration)
        {
            _fileManager = fileManager;
            _configuration = configuration;
            _secretManager = new SecretManager(environmentManager, _fileManager, _configuration);
        }

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