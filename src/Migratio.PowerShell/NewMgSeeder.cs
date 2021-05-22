using System.IO;
using System.Management.Automation;
using Migratio.Core.Models;
using Migratio.PowerShell.Core;
using Migratio.PowerShell.Results;

namespace Migratio.PowerShell
{
    [Cmdlet(VerbsCommon.New, "MgSeeder")]
    [OutputType(typeof(string))]
    public class NewMgSeeder : BaseCmdlet
    {
        public NewMgSeeder()
        {
        }

        public NewMgSeeder(CmdletDependencies dependencies) : base(dependencies)
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
            HelpMessage = "Name of seeder")
        ]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var seederDir = Configuration.GetMigratioDir(MigrationRootDir, ConfigFile, MigratioDirectory.Seeders);
            if (!FileManager.DirectoryExists(seederDir))
            {
                FileManager.CreateDirectory(seederDir);
                WriteObject($"Created directory {seederDir}");
            }

            var fileName = Path.Combine(seederDir,
                $"{FileManager.GetFilePrefix()}_{FileManager.GetFormattedName(Name)}.sql");
            if (FileManager.FileExists(fileName))
            {
                WriteObject(new MgResult {Successful = false, Details = $"File {fileName} already exists"});
            }
            else
            {
                FileManager.CreateFile(fileName);
                WriteObject(new MgResult {Successful = true, Details = $"Created file {fileName}"});
            }
        }
    }
}