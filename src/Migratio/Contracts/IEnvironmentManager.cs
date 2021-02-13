namespace Migratio.Contracts
{
    public interface IEnvironmentManager
    {
        string GetEnvironmentVariable(string key);
    }
}