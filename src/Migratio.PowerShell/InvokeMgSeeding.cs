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
    [Cmdlet(VerbsLifecycle.Invoke, "MgSeeding")]
    [OutputType(typeof(bool))]
    public class InvokeMgSeeding : DbCmdlet
    {
        private readonly MigrationHelper _migrationHelper;

        public InvokeMgSeeding()
        {
            _migrationHelper = new MigrationHelper(FileManager, EnvironmentManager, MigratioConfiguration);
        }

        public InvokeMgSeeding(CmdletDependencies dependencies) : base(dependencies)
        {
            _migrationHelper = new MigrationHelper(FileManager, EnvironmentManager, MigratioConfiguration);
        }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage =
                "Specifies the root directory of seeders if using default directory naming." +
                "Equivalent to setting the base option in Migratio config")
        ]
        [ValidateNotNullOrEmpty]
        public string MigrationRootDir { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Replace variables in seeders during seeding")
        ]
        public SwitchParameter ReplaceVariables { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Create the seeders table if it does not exist in the database.")
        ]
        public SwitchParameter CreateTableIfNotExist { get; set; }

        protected override void ProcessRecord()
        {
            var seedersDir = MigratioConfiguration.GetMigratioDir(MigrationRootDir, ConfigFile, MigratioDirectory.Seeders);
            var replaceVariables = ReplaceVariables.IsPresent
                ? ReplaceVariables.ToBool()
                : MigratioConfiguration.Resolve(MigratioConfiguration?.Config?.ReplaceVariables, false, false);


            var cfg = GetConnectionInfo();
            DatabaseProvider.SetConnectionInfo(cfg);

            if (!DatabaseProvider.SeedingTableExists())
            {
                if (CreateTableIfNotExist.ToBool())
                {
                    DatabaseProvider.CreateSeedersTable();
                    WriteVerbose("Created seeders table");
                }
                else
                {
                    throw new Exception("Seeders table does not exist");
                }
            }

            var scripts = FileManager.GetAllFilesInFolder(seedersDir)
                .OrderBy(f => f)
                .ToArray();

            if (scripts.Length == 0)
            {
                WriteWarning("No scripts found");
                WriteObject(new MgResult {Successful = false, Details = "No scripts found"});
                return;
            }

            WriteVerbose($"Found a total of {scripts.Length} seeding scripts in the seeders folder");

            var applied = DatabaseProvider.GetAppliedSeeders();

            WriteObject($"Found {applied.Length} applied seeders");
            WriteObject($"Found {scripts.Length} total seeders");

            if (applied.Length == scripts.Length)
            {
                WriteObject("Number of applied seeders are the same as the total, skipping");
                return;
            }


            var stringBuilder = new StringBuilder();
            foreach (var script in scripts)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(script);
                if (applied.Any(x => x.SeedId.Contains(fileNameWithoutExtension)))
                {
                    WriteObject($"Seeder {Path.GetFileNameWithoutExtension(script)} is applied, skipping");
                    continue;
                }

                WriteObject($"Seeder {fileNameWithoutExtension} is not applied adding to transaction");

                var scriptContent = _migrationHelper.GetScriptContent(script, replaceVariables ?? false);
                stringBuilder.Append(scriptContent);
                stringBuilder.Append(GetSeederQuery(fileNameWithoutExtension));
            }

            DatabaseProvider.RunTransaction(stringBuilder.ToString());
            WriteObject(new MgResult {Successful = true});
        }

        private string GetSeederQuery(string seederScriptName)
        {
            return Queries.NewSeedersQuery
                .Replace("@tableSchema", Schema ?? MigratioConfiguration?.Config?.Auth?.Postgres?.Schema ?? "public")
                .Replace("@seederName", seederScriptName) + Environment.NewLine;
        }
    }
}