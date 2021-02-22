using System;
using System.Management.Automation;
using Migratio.Core;
using Migratio.Models;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MgScriptsForLatestIteration")]
    [OutputType(typeof(Migration[]))]
    public class GetMgScriptsForLatestIteration : DbCmdlet
    {
        public GetMgScriptsForLatestIteration()
        {
        }

        public GetMgScriptsForLatestIteration(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessRecord()
        {
            DatabaseProvider.SetConnectionInfo(GetConnectionInfo());

            if (!DatabaseProvider.MigrationTableExists())
            {
                throw new Exception("Migration table does not exist");
            }

            var processed = DatabaseProvider.GetAppliedScriptsForLatestIteration();

            WriteObject(processed);
        }
    }
}