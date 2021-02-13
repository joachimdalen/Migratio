using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Migratio.Contracts;

namespace Migratio
{
    public class FileManager : IFileManager
    {
        public string[] GetAllFilesInFolder(string baseDir) =>
            Directory.GetFiles(baseDir, "*.sql", SearchOption.AllDirectories);

        public string RolloutDirectory(string baseDir) => Path.Combine(baseDir, "rollout");
        public string RollbackDirectory(string baseDir) => Path.Combine(baseDir, "rollback");
        public string SeedersDirectory(string baseDir) => Path.Combine(baseDir, "seeders");
        public string GetFilePrefix() => DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        public string GetFormattedName(string baseName) => Regex.Replace(baseName, "[^A-Za-z0-9]", "_").ToLower();
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
        public bool FileExists(string path) => File.Exists(path);
        public FileStream CreateFile(string path) => File.Create(path);
        public string ReadAllText(string path) => File.ReadAllText(path);
    }
}