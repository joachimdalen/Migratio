using System.IO;

namespace Migratio.Contracts
{
    public interface IFileManager
    {
        string[] GetAllFilesInFolder(string baseDir);
        string RolloutDirectory(string baseDir);
        string RollbackDirectory(string baseDir);
        string SeedersDirectory(string baseDir);
        string GetFilePrefix();
        string GetFormattedName(string baseName);
        bool DirectoryExists(string path);
        DirectoryInfo CreateDirectory(string path);
        bool FileExists(string path);
        FileStream CreateFile(string path);
        string ReadAllText(string path);
    }
}