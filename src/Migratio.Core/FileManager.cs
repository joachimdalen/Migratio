using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Migratio.Core.Contracts;

namespace Migratio.Core
{
    public class FileManager : IFileManager
    {
        /// <inheritdoc /> 
        public string[] GetAllFilesInFolder(string baseDir)
        {
            return Directory.GetFiles(baseDir, "*.sql", SearchOption.AllDirectories);
        }

        /// <inheritdoc /> 
        public string GetFilePrefix()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        }

        /// <inheritdoc /> 
        public string GetFormattedName(string baseName)
        {
            return Regex.Replace(baseName, "[^A-Za-z0-9]", "_").ToLower();
        }

        /// <inheritdoc /> 
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc /> 
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        /// <inheritdoc /> 
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc /> 
        public FileStream CreateFile(string path)
        {
            return File.Create(path);
        }

        /// <inheritdoc /> 
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <inheritdoc /> 
        public IEnumerable<string> ReadLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}