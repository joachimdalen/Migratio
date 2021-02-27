using Migratio.Configuration;
using Migratio.Models;

namespace Migratio.Contracts
{
    public interface IConfiguration
    {
        MgConfig Config { get; set; }
        bool Load(string configFile);
        string GetKeyFromMapping(string itemKey);
        T Resolve<T>(T first, T second, T defaultValue);
        string GetMigratioDir(string migrationBaseDir, string configPath, MigratioDirectory directoryType);
    }
}