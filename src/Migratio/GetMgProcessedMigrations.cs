using System;
using System.Management.Automation;
using Migratio.Contracts;
using Migratio.Database;
using Migratio.Models;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MgProcessedMigrations")]
    [OutputType(typeof(Migration[]))]
    public class GetMgProcessedMigrations : BaseCmdlet
    {
        private readonly IDatabaseProvider _db;

        public GetMgProcessedMigrations()
        {
            _db = new PostgreDb();
        }

        public GetMgProcessedMigrations(IDatabaseProvider db)
        {
            _db = db;
        }

        protected override void ProcessRecord()
        {
            _db.SetConnectionInfo(GetConnectionInfo());
            if (!_db.MigrationTableExists())
            {
                throw new Exception("Migration table does not exist");
            }

            var processed = _db.GetAppliedMigrations();

            WriteObject(processed);
        }
    }
}