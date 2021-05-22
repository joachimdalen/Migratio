using System.IO;
using System.Management.Automation;
using Migratio.Core.Models;
using Migratio.PowerShell.Core;
using Migratio.PowerShell.Results;

namespace Migratio.PowerShell
{
    [Cmdlet(VerbsCommon.New, "MgMigration")]
    [OutputType(typeof(void))]
    public class NewMgMigration : BaseCmdlet
    {
        public NewMgMigration()
        {
        }

        public NewMgMigration(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage =
                "Specifies the root directory of migrations if using default directory naming." +
                "Equivalent to setting the base option in Migratio config")
        ]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Name of migration")
        ]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var rolloutDir = MigratioConfiguration.GetMigratioDir(MigrationRootDir, ConfigFile, MigratioDirectory.Rollout);
            var rollbackDir = MigratioConfiguration.GetMigratioDir(MigrationRootDir, ConfigFile, MigratioDirectory.Rollback);
            var dirs = new[] {rolloutDir, rollbackDir};

            foreach (var dir in dirs)
            {
                if (FileManager.DirectoryExists(dir)) continue;
                FileManager.CreateDirectory(dir);
                WriteObject($"Created directory {dir}");
            }

            foreach (var dir in dirs)
            {
                var fileName = Path.Combine(dir,
                    $"{FileManager.GetFilePrefix()}_{FileManager.GetFormattedName(Name)}.sql");
                if (FileManager.FileExists(fileName))
                {
                    WriteWarning($"File {fileName} already exists");
                    WriteObject(new MgResult {Successful = true, Details = $"File {fileName} already exists"});
                }
                else
                {
                    FileManager.CreateFile(fileName);
                    WriteObject(new MgResult {Successful = true, Details = $"Created file {fileName}"});
                }
            }
        }
    }
}