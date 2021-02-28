namespace Migratio.Contracts
{
    public interface IEnvironmentManager
    {
        /// <summary>
        /// Get environment variable
        /// </summary>
        /// <param name="key">Name of variable</param>
        /// <returns>named variable</returns>
        string GetEnvironmentVariable(string key);
    }
}