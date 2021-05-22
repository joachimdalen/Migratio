using Migratio.Core.Configuration;
using Migratio.Core.Models;

namespace Migratio.Core.Contracts
{
    public interface IConfiguration
    {
        /// <summary>
        /// Configuration loaded from the Migratio configuration file
        /// </summary>
        MgConfig Config { get; set; }

        /// <summary>
        /// Load configuration from given file path
        /// </summary>
        /// <param name="configFile">Filesystem path to configuration file</param>
        /// <returns></returns>
        bool Load(string configFile);

        /// <summary>
        /// Get environment variable key from configuration defined mapping.
        /// </summary>
        /// <param name="itemKey">The item key to do lookup on</param>
        /// <returns>itemKey if no mappings is defined, or the mapping is not found</returns>
        string GetKeyFromMapping(string itemKey);

        T Resolve<T>(T first, T second, T defaultValue);

        /// <summary>
        /// Get Migratio directory path
        /// </summary>
        /// <param name="migrationBaseDir">Base directory to migration folders</param>
        /// <param name="configPath">Filesystem path to configuration file</param>
        /// <param name="directoryType">The type of directory to get</param>
        /// <returns>Path to requested folder</returns>
        string GetMigratioDir(string migrationBaseDir, string configPath, MigratioDirectory directoryType);
    }
}