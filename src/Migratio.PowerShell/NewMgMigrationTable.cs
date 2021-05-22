using System.Management.Automation;
using Migratio.PowerShell.Core;
using Migratio.PowerShell.Results;

namespace Migratio.PowerShell
{
    [Cmdlet(VerbsCommon.New, "MgMigrationTable")]
    [OutputType(typeof(bool))]
    public class NewMgMigrationTable : DbCmdlet
    {
        public NewMgMigrationTable()
        {
        }

        public NewMgMigrationTable(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessRecord()
        {
            DatabaseProvider.SetConnectionInfo(GetConnectionInfo());

            if (DatabaseProvider.MigrationTableExists())
            {
                WriteWarning("Migration table already exists");
                WriteObject(new MgResult {Successful = false, Details = "Migration table already exists"});
                return;
            }

            var result = DatabaseProvider.CreateMigrationTable();
            WriteObject(new MgResult {Successful = result == 1});
        }
    }
}