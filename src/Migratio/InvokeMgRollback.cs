using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Migratio.Core;
using Migratio.Database;
using Migratio.Models;
using Migratio.Results;
using Migratio.Utils;

namespace Migratio
{
    [Cmdlet(VerbsLifecycle.Invoke, "MgRollback")]
    [OutputType(typeof(void))]
    public class InvokeMgRollback : DbCmdlet
    {
        private readonly MigrationHelper _migrationHelper;

        public InvokeMgRollback()
        {
            _migrationHelper = new MigrationHelper(FileManager, EnvironmentManager, Configuration);
        }

        public InvokeMgRollback(CmdletDependencies dependencies) : base(dependencies)
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


        protected override void ProcessRecord()
        {
            var rollbackDir = Configuration.GetMigratioDir(MigrationRootDir, ConfigFile, MigratioDirectory.Rollback);

            DatabaseProvider.SetConnectionInfo(GetConnectionInfo());
            if (!DatabaseProvider.MigrationTableExists()) throw new Exception("Migration table does not exist");

            var scripts = FileManager.GetAllFilesInFolder(rollbackDir)
                .OrderByDescending(f => f).ToArray();
            if (scripts.Length == 0)
            {
                WriteWarning("No rollback scripts found");
                WriteObject(new MgResult {Successful = false});
                return;
            }

            var iteration = DatabaseProvider.GetLatestIteration();
            var scriptsForLatestIteration = DatabaseProvider.GetAppliedScriptsForLatestIteration();

            if (iteration == 0 || scriptsForLatestIteration?.Length == 0)
            {
                WriteWarning("No applied migrations found");
                WriteObject(new MgResult {Successful = false, Details = "No applied migrations found"});
                return;
            }

            WriteObject($"Found {scriptsForLatestIteration.Length} migrations applied in iteration {iteration}");
            var stringBuilder = new StringBuilder();
            foreach (var script in scripts)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(script);
                if (!scriptsForLatestIteration.Any(x => x.MigrationId.Contains(fileNameWithoutExtension)))
                {
                    WriteObject($"Migration {fileNameWithoutExtension} was not applied in latest iteration, skipping");
                    continue;
                }

                var scriptContent = _migrationHelper.GetScriptContent(script, false);

                stringBuilder.Append(scriptContent);
                stringBuilder.Append(GetMigrationQuery(fileNameWithoutExtension, iteration));

                WriteObject($"Adding rollback of migration: {fileNameWithoutExtension} to transaction");
            }

            DatabaseProvider.RunTransaction(stringBuilder.ToString());
            WriteObject(new MgResult {Successful = true});
        }


        private string GetMigrationQuery(string migrationScriptName, int iteration)
        {
            return Queries.DeleteMigrationQuery
                .Replace("@tableSchema", Schema ?? Configuration?.Config?.Auth?.Postgres?.Schema ?? "public")
                .Replace("@migrationName", migrationScriptName)
                .Replace("@currentIteration", iteration.ToString()) + Environment.NewLine;
        }
    }
}