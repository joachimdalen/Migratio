using System.Management.Automation;
using Migratio.Core;

namespace Migratio
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
                WriteObject(false);
                return;
            }

            var result = DatabaseProvider.CreateMigrationTable();
            WriteObject(result == 1);
        }
    }
}