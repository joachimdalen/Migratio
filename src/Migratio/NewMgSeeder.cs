using System.IO;
using System.Management.Automation;
using Migratio.Contracts;

namespace Migratio
{
    [Cmdlet(VerbsCommon.New, "MgSeeder")]
    [OutputType(typeof(string))]
    public class NewMgSeeder : Cmdlet
    {
        private readonly IFileManager _fileManager;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; } = "migrations";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public NewMgSeeder()
        {
            _fileManager = new FileManager();
        }

        public NewMgSeeder(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        protected override void ProcessRecord()
        {
            var seederDir = _fileManager.SeedersDirectory(MigrationRootDir);
            if (!_fileManager.DirectoryExists(seederDir))
            {
                _fileManager.CreateDirectory(seederDir);
                WriteObject($"Created directory {seederDir}");
            }

            var fileName = Path.Combine(seederDir,
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