using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Migratio.Contracts;
using Migratio.Database;
using Migratio.Secrets;
using Migratio.Utils;

namespace Migratio
{
    [Cmdlet(VerbsLifecycle.Invoke, "MgRollout")]
    [OutputType(typeof(bool))]
    public class InvokeMgRollout : BaseCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; } = "migrations";

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter ReplaceVariables { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter CreateTableIfNotExist { get; set; }

        private readonly IDatabaseProvider _db;
        private readonly IFileManager _fileManager;
        private readonly MigrationHelper _migrationHelper;

        public InvokeMgRollout()
        {
            _db = new PostgreDb();
            _fileManager = new FileManager();
            _migrationHelper = new MigrationHelper(_fileManager, new EnvironmentManager());
        }

        public InvokeMgRollout(IDatabaseProvider db, IFileManager fileManager,
            IEnvironmentManager environmentManager)
        {
            _db = db;
            _fileManager = fileManager;
            _migrationHelper = new MigrationHelper(_fileManager, environmentManager);
        }

        protected override void ProcessRecord()
        {
            _db.SetConnectionInfo(GetConnectionInfo());
            if (!_db.MigrationTableExists())
            {
                if (CreateTableIfNotExist.ToBool())
                {
                    _db.CreateMigrationTable();
                }
                else
                {
                    throw new Exception("Migration table does not exist");
                }
            }

            var scripts = _fileManager.GetAllFilesInFolder(_fileManager.RolloutDirectory(MigrationRootDir))
                .OrderBy(f => f)
                .ToArray();

            if (scripts.Length == 0)
            {
                WriteWarning("No scripts found");
                WriteObject(false);
                return;
            }

            WriteVerbose($"Found a total of {scripts.Length} migration scripts in the rollout folder");

            var applied = _db.GetAppliedMigrations();

            WriteObject($"Found {applied.Length} applied migrations");
            WriteObject($"Found {scripts.Length} total migrations");

            if (applied.Length == scripts.Length)
            {
                WriteObject("Number of applied migrations are the same as the total, skipping");
                return;
            }

            var iteration = _db.GetLatestIteration();
            var stringBuilder = new StringBuilder();
            var currentIteration = iteration + 1;

            foreach (var script in scripts)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(script);
                if (applied.Any(x => x.MigrationId.Contains(fileNameWithoutExtension)))
                {
                    WriteObject($"Migration {Path.GetFileNameWithoutExtension(script)} is applied, skipping");
                    continue;
                }

                WriteObject($"Migration {fileNameWithoutExtension} is not applied adding to transaction");
                var scriptContent = _migrationHelper.GetScriptContent(script, ReplaceVariables.ToBool());
                stringBuilder.Append(scriptContent);
                stringBuilder.Append(GetMigrationQuery(fileNameWithoutExtension, currentIteration));
            }

            _db.RunTransaction(stringBuilder.ToString());
        }

        private string GetMigrationQuery(string migrationScriptName, int iteration) => Queries.NewMigrationQuery
            .Replace("@tableSchema", Schema)
            .Replace("@migrationName", migrationScriptName)
            .Replace("@currentIteration", iteration.ToString()) + Environment.NewLine;
    }
}