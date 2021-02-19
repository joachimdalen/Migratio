using System;
using System.Management.Automation;
using Migratio.Core;

namespace Migratio.Utils
{
    [Cmdlet(VerbsCommon.Get, "MgUsedVariables")]
    [OutputType(typeof(string[]))]
    public class GetMgUsedVariables : BaseCmdlet
    {
        public GetMgUsedVariables()
        {
        }

        public GetMgUsedVariables(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationFile { get; set; }

        protected override void ProcessRecord()
        {
            if (!FileManager.FileExists(MigrationFile)) throw new Exception($"No such file at: {MigrationFile}");

            var content = FileManager.ReadAllText(MigrationFile);
            var usedKeys = SecretManager.GetSecretsInContent(content);

            WriteObject(usedKeys);
        }
    }
}