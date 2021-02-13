using Migratio.Database;
using Migratio.Models;

namespace Migratio.Contracts
{
    public interface IDatabaseProvider
    {
        void SetConnectionInfo(DbConnectionInfo info);
        bool MigrationTableExists();
        int GetLatestIteration();
        Migration[] GetAppliedMigrations();
        Migration[] GetAppliedScriptsForLatestIteration();
        int CreateMigrationTable();
        int RunTransaction(string query);
    }
}