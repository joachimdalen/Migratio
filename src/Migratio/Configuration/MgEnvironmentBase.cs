using System.Collections.Generic;

namespace Migratio.Configuration
{
    public class MgEnvironmentBase
    {
        public Dictionary<string, MgConfig> Environments { get; set; }
    }
}