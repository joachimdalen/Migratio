using System.IO;
using System.Management.Automation;
using Migratio.Core;

namespace Migratio
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

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var rolloutDir = Configuration.RolloutDirectory(MigrationRootDir, ConfigFile);
            var rollbackDir = Configuration.RollbackDirectory(MigrationRootDir, ConfigFile);
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
                }
                else
                {
                    FileManager.CreateFile(fileName);
                    WriteObject($"Created file {fileName}");
                }
            }
        }
    }
}