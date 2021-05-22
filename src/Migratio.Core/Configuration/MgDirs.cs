namespace Migratio.Core.Configuration
{
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
}