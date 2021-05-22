using System.Collections.Generic;
using System.IO;

namespace Migratio.Core.Contracts
{
    public interface IFileManager
    {
        /// <summary>
        /// Get all files in the given directory
        /// </summary>
        /// <param name="baseDir">Path to directory</param>
        /// <returns></returns>
        string[] GetAllFilesInFolder(string baseDir);

        /// <summary>
        /// Get Migratio file prefix for migrations and seeders
        /// </summary>
        /// <returns>Formatted file prefix</returns>
        string GetFilePrefix();

        /// <summary>
        /// Get formatted name by Migratio file standard
        /// </summary>
        /// <param name="baseName">Unformatted name</param>
        /// <returns>Formatted name</returns>
        string GetFormattedName(string baseName);

        /// <summary>
        /// Check if directory exists
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>true if it exists, else false</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Create a new directory
        /// </summary>
        /// <param name="path">Directory to create</param>
        /// <returns></returns>
        DirectoryInfo CreateDirectory(string path);

        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>true if it exists, else false</returns>
        bool FileExists(string path);

        /// <summary>
        /// Create a new file
        /// </summary>
        /// <param name="path">File to create</param>
        /// <returns></returns>
        FileStream CreateFile(string path);

        /// <summary>
        /// Read all text as single string from a file at the given path
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns></returns>
        string ReadAllText(string path);

        /// <summary>
        /// Read all text as single array from a file at the given path
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns></returns>
        IEnumerable<string> ReadLines(string path);
    }
}