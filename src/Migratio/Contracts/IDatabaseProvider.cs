using Migratio.Database;
using Migratio.Models;

namespace Migratio.Contracts
{
    /// <summary>
    /// Handles database interactions
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        /// Set database connection info
        /// </summary>
        /// <param name="info">Connection info</param>
        void SetConnectionInfo(DbConnectionInfo info);

        /// <summary>
        /// Check if migration table exists in the database
        /// </summary>
        /// <returns>true if exists, false else</returns>
        bool MigrationTableExists();

        /// <summary>
        /// Get the latest applied migration
        /// </summary>
        /// <returns>Latest applied migration</returns>
        int GetLatestIteration();

        /// <summary>
        /// Get all applied migrations
        /// </summary>
        /// <returns>applied migrations</returns>
        Migration[] GetAppliedMigrations();

        /// <summary>
        /// Get all applied migrations for the latest iteration
        /// </summary>
        /// <returns>applied migrations for the latest iteration</returns>
        Migration[] GetAppliedScriptsForLatestIteration();

        /// <summary>
        /// Create a new migration table in the database
        /// </summary>
        /// <returns></returns>
        int CreateMigrationTable();

        /// <summary>
        /// Run a database transaction
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns>Number of affected rows</returns>
        int RunTransaction(string query);
    }
}