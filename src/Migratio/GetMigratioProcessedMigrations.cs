using System;
using System.Management.Automation;
using Migratio.Contracts;
using Migratio.Database;
using Migratio.Models;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MigratioProcessedMigrations")]
    [OutputType(typeof(Migration[]))]
    public class GetMigratioProcessedMigrations : BaseCmdlet
    {
        private readonly IDatabaseProvider _db;

        public GetMigratioProcessedMigrations()
        {
            _db = new PostgreDb(GetConnectionInfo());
        }

        public GetMigratioProcessedMigrations(IDatabaseProvider db)
        {
            _db = db;
        }

        protected override void ProcessRecord()
        {
            if (!_db.MigrationTableExists())
            {
                throw new Exception("Migration table does not exist");
            }

            var processed = _db.GetAppliedMigrations();

            WriteObject(processed);
        }
    }
}