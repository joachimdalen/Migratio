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
    [Cmdlet(VerbsLifecycle.Invoke, "MigratioRollout")]
    [OutputType(typeof(bool))]
    public class InvokeMigratioRollout : BaseCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; } = "migrations";

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter AsSingleMigrations { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter ReplaceVariables { get; set; }

        private readonly IDatabaseProvider _db;
        private readonly IFileManager _fileManager;
        private readonly SecretManager _secretManager;

        public InvokeMigratioRollout()
        {
            _db = new PostgreDb(GetConnectionInfo());
            _fileManager = new FileManager();
            _secretManager = new SecretManager(new EnvironmentManager());
        }

        public InvokeMigratioRollout(IDatabaseProvider db, IFileManager fileManager,
            IEnvironmentManager environmentManager)
        {
            _db = db;
            _fileManager = fileManager;
            _secretManager = new SecretManager(environmentManager);
        }

        protected override void ProcessRecord()
        {
            if (!_db.MigrationTableExists())
            {
                throw new Exception("Migration table does not exist");
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
            if (AsSingleMigrations.ToBool())
            {
                var currentIteration = iteration + 1;
                foreach (var script in scripts)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(script);
                    if (applied.Any(x => x.MigrationId.Contains(fileNameWithoutExtension)))
                    {
                        WriteObject($"Migration {fileNameWithoutExtension} is applied, skipping");
                        continue;
                    }

                    var stringBuilder = new StringBuilder();
                    var scriptContent = _fileManager.ReadAllText(script);
                    if (!scriptContent.EndsWith(";"))
                        scriptContent += ";";

                    if (ReplaceVariables.ToBool())
                    {
                        var replacedContent = _secretManager.ReplaceSecretsInContent(scriptContent);
                        stringBuilder.Append(replacedContent);
                    }
                    else
                    {
                        stringBuilder.Append(scriptContent);
                    }

                    stringBuilder.Append(GetMigrationQuery(fileNameWithoutExtension, currentIteration));

                    WriteObject($"Running migration {fileNameWithoutExtension}");

                    _db.RunTransaction(stringBuilder.ToString());
                    currentIteration++;
                }
            }
            else
            {
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

                    WriteObject(
                        $"Migration {Path.GetFileNameWithoutExtension(script)} is not applied adding to transaction");
                    var scriptContent = _fileManager.ReadAllText(script);
                    if (!scriptContent.EndsWith(";"))
                        scriptContent += ";";

                    if (ReplaceVariables.ToBool())
                    {
                        var replacedContent = _secretManager.ReplaceSecretsInContent(scriptContent);
                        stringBuilder.Append(replacedContent);
                    }
                    else
                    {
                        stringBuilder.Append(scriptContent);
                    }

                    stringBuilder.Append(GetMigrationQuery(fileNameWithoutExtension, currentIteration));
                }

                _db.RunTransaction(stringBuilder.ToString());
            }
        }

        private string GetMigrationQuery(string migrationScriptName, int iteration) => Queries.NewMigrationQuery
            .Replace("@tableSchema", Schema)
            .Replace("@migrationName", migrationScriptName)
            .Replace("@currentIteration", iteration.ToString());
    }
}