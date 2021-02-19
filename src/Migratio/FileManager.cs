using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Migratio.Contracts;

namespace Migratio
{
    public class FileManager : IFileManager
    {
        public string[] GetAllFilesInFolder(string baseDir)
        {
            return Directory.GetFiles(baseDir, "*.sql", SearchOption.AllDirectories);
        }

        public string RolloutDirectory(string baseDir)
        {
            return Path.Combine(baseDir, "rollout");
        }

        public string RollbackDirectory(string baseDir)
        {
            return Path.Combine(baseDir, "rollback");
        }

        public string SeedersDirectory(string baseDir)
        {
            return Path.Combine(baseDir, "seeders");
        }

        public string GetFilePrefix()
        {
            return DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        }

        public string GetFormattedName(string baseName)
        {
            return Regex.Replace(baseName, "[^A-Za-z0-9]", "_").ToLower();
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public FileStream CreateFile(string path)
        {
            return File.Create(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}