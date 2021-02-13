using System.IO;
using System.Management.Automation;
using Migratio.Contracts;

namespace Migratio
{
    [Cmdlet(VerbsCommon.New, "MgMigration")]
    [OutputType(typeof(void))]
    public class NewMgMigration : Cmdlet
    {
        private readonly IFileManager _fileManager;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; } = "migrations";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public NewMgMigration()
        {
            _fileManager = new FileManager();
        }

        public NewMgMigration(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        protected override void ProcessRecord()
        {
            var rolloutDir = _fileManager.RolloutDirectory(MigrationRootDir);
            var rollbackDir = _fileManager.RollbackDirectory(MigrationRootDir);
            var dirs = new[] {rolloutDir, rollbackDir};

            foreach (var dir in dirs)
            {
                if (_fileManager.DirectoryExists(dir)) continue;
                _fileManager.CreateDirectory(dir);
                WriteObject($"Created directory {dir}");
            }

            foreach (var dir in dirs)
            {
                var fileName = Path.Combine(dir,
                    $"{_fileManager.GetFilePrefix()}_{_fileManager.GetFormattedName(Name)}.sql");
                if (_fileManager.FileExists(fileName))
                {
                    WriteWarning($"File {fileName} already exists");
                }
                else
                {
                    _fileManager.CreateFile(fileName);
                    WriteObject($"Created file {fileName}");
                }
            }
        }
    }
}