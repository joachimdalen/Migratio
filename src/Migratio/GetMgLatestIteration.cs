using System.Management.Automation;
using Migratio.Contracts;
using Migratio.Database;
using Migratio.Models;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MgLatestIteration")]
    [OutputType(typeof(IterationResult))]
    public class GetMgLatestIteration : BaseCmdlet
    {
        private readonly IDatabaseProvider _db;

        public GetMgLatestIteration()
        {
            _db = new PostgreDb(GetConnectionInfo());
        }

        public GetMgLatestIteration(IDatabaseProvider db)
        {
            _db = db;
        }

        protected override void ProcessRecord()
        {
            if (_db.MigrationTableExists())
            {
                var result = _db.GetLatestIteration();
                WriteObject(new IterationResult {Iteration = result});
            }
            else
            {
                WriteWarning("Migration table does not exist");
                WriteObject(null);
            }
        }
    }
}