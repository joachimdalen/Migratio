using Migratio.Models;

namespace Migratio.Contracts
{
    public interface IDatabaseProvider
    {
        bool MigrationTableExists();
        int GetLatestIteration();
        Migration[] GetAppliedMigrations();
        Migration[] GetAppliedScriptsForLatestIteration();
        int CreateMigrationTable();
        int RunTransaction(string query);
    }
}