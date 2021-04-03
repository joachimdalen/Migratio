using System;
using System.Management.Automation;
using Migratio.Core;
using Migratio.Models;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MgProcessedSeeders")]
    [OutputType(typeof(Seed[]))]
    public class GetMgProcessedSeeders : DbCmdlet
    {
        public GetMgProcessedSeeders()
        {
        }

        public GetMgProcessedSeeders(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessRecord()
        {
            DatabaseProvider.SetConnectionInfo(GetConnectionInfo());

            if (!DatabaseProvider.SeedingTableExists()) throw new Exception("Seeders table does not exist");

            var processed = DatabaseProvider.GetAppliedSeeders();

            WriteObject(processed);
        }
    }
}