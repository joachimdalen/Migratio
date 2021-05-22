namespace Migratio.Core.Contracts
{
    public interface ISecretManager
    {
        /// <summary>
        /// Get keys of variables used in string content
        /// </summary>
        /// <param name="content">Content to check</param>
        /// <returns></returns>
        string[] GetVariablesInContent(string content);

        /// <summary>
        /// Replace variables in content
        /// </summary>
        /// <param name="content">Content to replace variables in</param>
        /// <returns>Replaced string content</returns>
        string ReplaceVariablesInContent(string content);

        /// <summary>
        /// Get environment variable, checks all possible Migratio sources.
        /// OS process, configuration file and environment file.
        /// </summary>
        /// <param name="key">Name of variable</param>
        /// <returns>Value of environment variable</returns>
        string GetEnvironmentVariable(string key);

        /// <summary>
        /// Check if content contains variables
        /// </summary>
        /// <param name="content">Content to check</param>
        /// <returns></returns>
        bool HasVariable(string content);
    }
}