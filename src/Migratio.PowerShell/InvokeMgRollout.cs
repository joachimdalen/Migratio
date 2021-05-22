using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Migratio.Core.Database;
using Migratio.Core.Models;
using Migratio.Core.Utils;
using Migratio.PowerShell.Core;
using Migratio.PowerShell.Results;

namespace Migratio.PowerShell
{
    [Cmdlet(VerbsLifecycle.Invoke, "MgRollout")]
    [OutputType(typeof(bool))]
    public class InvokeMgRollout : DbCmdlet
    {
        private readonly MigrationHelper _migrationHelper;

        public InvokeMgRollout()
        {
            _migrationHelper = new MigrationHelper(FileManager, EnvironmentManager, Configuration);
        }

        public InvokeMgRollout(CmdletDependencies dependencies) : base(dependencies)
        {
            _migrationHelper = new MigrationHelper(FileManager, EnvironmentManager, Configuration);
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
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Replace variables in migrations during rollout")
        ]
        public SwitchParameter ReplaceVariables { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Create the migration table if it does not exist in the database.")
        ]
        public SwitchParameter CreateTableIfNotExist { get; set; }

        protected override void ProcessRecord()
        {
            var rolloutDir = Configuration.GetMigratioDir(MigrationRootDir, ConfigFile, MigratioDirectory.Rollout);
            var replaceVariables = ReplaceVariables.IsPresent
                ? ReplaceVariables.ToBool()
                : Configuration.Resolve(Configuration?.Config?.ReplaceVariables, false, false);


            var cfg = GetConnectionInfo();
            DatabaseProvider.SetConnectionInfo(cfg);

            if (!DatabaseProvider.MigrationTableExists())
            {
                if (CreateTableIfNotExist.ToBool())
                {
                    DatabaseProvider.CreateMigrationTable();
                    WriteVerbose("Created migration table");
                }
                else
                {
                    throw new Exception("Migration table does not exist");
                }
            }

            var scripts = FileManager.GetAllFilesInFolder(rolloutDir)
                .OrderBy(f => f)
                .ToArray();

            if (scripts.Length == 0)
            {
                WriteWarning("No scripts found");
                WriteObject(new MgResult {Successful = false, Details = "No scripts found"});
                return;
            }

            WriteVerbose($"Found a total of {scripts.Length} migration scripts in the rollout folder");

            var applied = DatabaseProvider.GetAppliedMigrations();

            WriteObject($"Found {applied.Length} applied migrations");
            WriteObject($"Found {scripts.Length} total migrations");

            if (applied.Length == scripts.Length)
            {
                WriteObject("Number of applied migrations are the same as the total, skipping");
                return;
            }

            var iteration = DatabaseProvider.GetLatestIteration();
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

                var scriptContent = _migrationHelper.GetScriptContent(script, replaceVariables ?? false);
                stringBuilder.Append(scriptContent);
                stringBuilder.Append(GetMigrationQuery(fileNameWithoutExtension, currentIteration));
            }

            DatabaseProvider.RunTransaction(stringBuilder.ToString());
            WriteObject(new MgResult {Successful = true});
        }

        private string GetMigrationQuery(string migrationScriptName, int iteration)
        {
            return Queries.NewMigrationQuery
                .Replace("@tableSchema", Schema ?? Configuration?.Config?.Auth?.Postgres?.Schema ?? "public")
                .Replace("@migrationName", migrationScriptName)
                .Replace("@currentIteration", iteration.ToString()) + Environment.NewLine;
        }
    }
}