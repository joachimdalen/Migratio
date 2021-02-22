using Migratio.Configuration;

namespace Migratio.Contracts
{
    public interface IConfiguration
    {
        MgConfig Config { get; set; }
        bool Load(string configFile);

        string GetKeyFromMapping(string itemKey);
        T Resolve<T>(T first, T second, T defaultValue);
        string RolloutDirectory(string migrationBaseDir, string configPath);
        string RollbackDirectory(string migrationBaseDir, string configPath);
        string SeedersDirectory(string migrationBaseDir, string configPath);
    }
}