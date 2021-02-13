using System.Management.Automation;
using Migratio.Contracts;
using Migratio.Database;

namespace Migratio
{
    [Cmdlet(VerbsCommon.New, "MigratioMigrationTable")]
    [OutputType(typeof(bool))]
    public class NewMigratioMigrationTable : BaseCmdlet
    {
        private readonly IDatabaseProvider _db;

        public NewMigratioMigrationTable()
        {
            _db = new PostgreDb(GetConnectionInfo());
        }

        public NewMigratioMigrationTable(IDatabaseProvider databaseProvider)
        {
            _db = databaseProvider;
        }

        protected override void ProcessRecord()
        {
            if (_db.MigrationTableExists())
            {
                WriteWarning("Migration table already exists");
                WriteObject(false);
                return;
            }

            var result = _db.CreateMigrationTable();
            WriteObject(result == 1);
        }
    }
}