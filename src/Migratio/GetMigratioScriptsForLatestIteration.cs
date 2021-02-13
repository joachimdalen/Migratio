using System.Management.Automation;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MigratioScriptsForLatestIteration")]
    [OutputType(typeof(string[]))]
    public class GetMigratioScriptsForLatestIteration
    {
    }
}