using System.Management.Automation;

namespace Migratio
{
    [Cmdlet(VerbsCommon.Get, "MgScriptsForLatestIteration")]
    [OutputType(typeof(string[]))]
    public class GetMgScriptsForLatestIteration
    {
    }
}