using System;
using System.Management.Automation;
using Migratio.PowerShell.Core;

namespace Migratio.PowerShell.Utils
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

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "File path to the SQL migration to get used variables for")
        ]
        [ValidateNotNullOrEmpty]
        public string MigrationFile { get; set; }

        protected override void ProcessRecord()
        {
            if (!FileManager.FileExists(MigrationFile)) throw new Exception($"No such file at: {MigrationFile}");

            var content = FileManager.ReadAllText(MigrationFile);
            var usedKeys = SecretManager.GetVariablesInContent(content);

            WriteObject(usedKeys);
        }
    }
}