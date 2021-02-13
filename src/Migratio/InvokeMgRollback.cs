using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Migratio.Contracts;
using Migratio.Database;
using Migratio.Utils;

namespace Migratio
{
    [Cmdlet(VerbsLifecycle.Invoke, "MgRollback")]
    [OutputType(typeof(void))]
    public class InvokeMgRollback : BaseCmdlet
    {
        private readonly IDatabaseProvider _db;
        private readonly IFileManager _fileManager;
        private readonly MigrationHelper _migrationHelper;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; } = "migrations";

        public InvokeMgRollback()
        {
            _db = new PostgreDb(GetConnectionInfo());
            _fileManager = new FileManager();
            _migrationHelper = new MigrationHelper(_fileManager, new EnvironmentManager());
        }

        public InvokeMgRollback(IDatabaseProvider db, IFileManager fileManager, IEnvironmentManager environmentManager)
        {
            _db = db;
            _fileManager = fileManager;
            _migrationHelper = new MigrationHelper(_fileManager, environmentManager);
        }

        protected override void ProcessRecord()
        {
            if (!_db.MigrationTableExists())
            {
                throw new Exception("Migration table does not exist");
            }

            var scripts = _fileManager.GetAllFilesInFolder(_fileManager.RollbackDirectory(MigrationRootDir))
                .OrderByDescending(f => f).ToArray();
            if (scripts.Length == 0)
            {
                WriteWarning("No rollback scripts found");
                WriteObject(false);
                return;
            }

            var iteration = _db.GetLatestIteration();
            var scriptsForLatestIteration = _db.GetAppliedScriptsForLatestIteration();

            if (iteration == 0 || scriptsForLatestIteration?.Length == 0)
            {
                WriteWarning("No applied migrations found");
                WriteObject(false);
                return;
            }

            WriteObject($"Found {scriptsForLatestIteration.Length} migrations applied in iteration {iteration}");
            foreach (var script in scripts)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(script);
                if (scriptsForLatestIteration.Any(x => x.MigrationId.Contains(fileNameWithoutExtension)))
                {
                    var stringBuilder = new StringBuilder();
                    var scriptContent = _migrationHelper.GetScriptContent(script, false);

                    stringBuilder.Append(scriptContent);
                    stringBuilder.Append(GetMigrationQuery(fileNameWithoutExtension, iteration));

                    WriteObject($"Running rollback of migration: {fileNameWithoutExtension}");

                    _db.RunTransaction(stringBuilder.ToString());
                }
                else
                {
                    WriteObject($"Migration {fileNameWithoutExtension} was not applied in latest iteration, skipping");
                }
            }
        }

        private string GetMigrationQuery(string migrationScriptName, int iteration) => Queries.DeleteMigrationQuery
            .Replace("@tableSchema", Schema)
            .Replace("@migrationName", migrationScriptName)
            .Replace("@currentIteration", iteration.ToString());
    }
}