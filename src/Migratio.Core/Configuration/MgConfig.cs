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
}