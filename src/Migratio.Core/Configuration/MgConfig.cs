using System.Collections.Generic;

namespace Migratio.Core.Configuration
{
    public class MgConfig
    {
        /// <summary>
        /// Directories to system folders
        /// </summary>
        public MgDirs Directories { get; set; }

        /// <summary>
        /// Mapping of named environment variables
        /// </summary>
        public IDictionary<string, string> EnvMapping { get; set; }

        /// <summary>
        /// Path to env file
        /// </summary>
        public string EnvFile { get; set; }

        /// <summary>
        /// Defines if variables should be applied while running rollout
        /// migrations.
        /// </summary>
        public bool ReplaceVariables { get; set; }

        /// <summary>
        /// Authentication options
        /// </summary>
        public MgAuth Auth { get; set; }
    }

    public class MgDirs
    {
        /// <summary>
        /// Root directory that contains the other directories.
        /// Only used when following the default naming convention.
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Path to directory containing the rollout migrations
        /// </summary>
        public string Rollout { get; set; }

        /// <summary>
        /// Path to directory containing the rollback migrations
        /// </summary>
        public string Rollback { get; set; }

        /// <summary>
        /// Path to directory containing seeders
        /// </summary>
        public string Seeders { get; set; }
    }

    public class MgAuth
    {
        public MgDb Postgres { get; set; }
    }

    public class MgDb
    {
        /// <summary>
        /// Specifies the hostname or ip address of the machine to connect to
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Specifies the name of the database to connect to
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Username of database user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the database user. Only settable from configuration file or env variable MG_DB_PASSWORD
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Specifies the port on the database server to connect to
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Specifies the default database schema. Only valid for Postgres
        /// </summary>
        public string Schema { get; set; }
    }
}