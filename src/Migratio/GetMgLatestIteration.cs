using System.Management.Automation;
using Migratio.Core;
using Migratio.Models;
using Migratio.Results;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MgLatestIteration")]
    [OutputType(typeof(IterationResult))]
    public class GetMgLatestIteration : DbCmdlet
    {
        public GetMgLatestIteration()
        {
        }

        public GetMgLatestIteration(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessRecord()
        {
            DatabaseProvider.SetConnectionInfo(GetConnectionInfo());
            if (DatabaseProvider.MigrationTableExists())
            {
                var result = DatabaseProvider.GetLatestIteration();
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