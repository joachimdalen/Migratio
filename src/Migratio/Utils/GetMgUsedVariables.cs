using System;
using System.IO;
using System.Management.Automation;
using Migratio.Contracts;
using Migratio.Secrets;

namespace Migratio.Utils
{
    [Cmdlet(VerbsCommon.Get, "MgUsedVariables")]
    [OutputType(typeof(string[]))]
    public class GetMgUsedVariables : Cmdlet
    {
        private readonly IFileManager _fileManager;
        private readonly SecretManager _secretManager;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationFile { get; set; }

        public GetMgUsedVariables()
        {
            _fileManager = new FileManager();
            _secretManager = new SecretManager(new EnvironmentManager());
        }

        public GetMgUsedVariables(IFileManager fileManager, IEnvironmentManager environmentManager)
        {
            _fileManager = fileManager;
            _secretManager = new SecretManager(environmentManager);
        }

        protected override void ProcessRecord()
        {
            if (!_fileManager.FileExists(MigrationFile))
            {
                throw new Exception($"No such file at: {MigrationFile}");
            }

            var content = _fileManager.ReadAllText(MigrationFile);
            var usedKeys = _secretManager.GetSecretsInContent(content);

            WriteObject(usedKeys);
        }
    }
}