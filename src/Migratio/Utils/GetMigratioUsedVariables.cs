using System;
using System.IO;
using System.Management.Automation;
using Migratio.Secrets;

namespace Migratio.Utils
{
    [Cmdlet(VerbsCommon.Get, "MigratioUsedVariables")]
    [OutputType(typeof(string[]))]
    public class GetMigratioUsedVariables : Cmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationFile { get; set; }

        protected override void ProcessRecord()
        {
            if (!File.Exists(MigrationFile))
            {
                throw new Exception($"No such file at: {MigrationFile}");
            }

            var content = File.ReadAllText(MigrationFile);
            var usedKeys = new SecretManager().GetSecretsInContent(content);

            WriteObject(usedKeys);
        }
    }
}