using System.IO;
using System.Management.Automation;
using Migratio.Core;
using Migratio.Models;

namespace Migratio
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

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
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
                WriteWarning($"File {fileName} already exists");
            }
            else
            {
                FileManager.CreateFile(fileName);
                WriteObject($"Created file {fileName}");
            }
        }
    }
}