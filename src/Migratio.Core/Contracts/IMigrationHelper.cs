namespace Migratio.Core.Contracts
{
    public interface IMigrationHelper
    {
        /// <summary>
        /// Get file content with or without replaced variables
        /// </summary>
        /// <param name="scriptPath">Path to script to load</param>
        /// <param name="replace">Whether variables should be replaced</param>
        /// <returns>File content</returns>
        string GetScriptContent(string scriptPath, bool replace);
    }
}