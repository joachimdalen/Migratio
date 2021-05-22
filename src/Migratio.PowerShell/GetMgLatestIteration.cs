using System.Management.Automation;
using Migratio.PowerShell.Core;
using Migratio.PowerShell.Results;

namespace Migratio.PowerShell
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