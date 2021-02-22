using System.Collections.Generic;

namespace Migratio.Configuration
{
    public class MgConfig
    {
        public MgDirs Directories { get; set; }
        public IDictionary<string, string> EnvMapping { get; set; }
        public string EnvFile { get; set; }
        public bool ReplaceVariables { get; set; }
        public MgAuth Auth { get; set; }
    }

    public class MgDirs
    {
        public string Base { get; set; }
        public string Rollout { get; set; }
        public string Rollback { get; set; }
        public string Seeders { get; set; }
    }

    public class MgAuth
    {
        public MgDb Postgres { get; set; }
    }

    public class MgDb
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Schema { get; set; }
    }
}