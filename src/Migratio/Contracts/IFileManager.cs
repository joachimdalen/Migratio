using System.Collections.Generic;
using System.IO;

namespace Migratio.Contracts
{
    public interface IFileManager
    {
        string[] GetAllFilesInFolder(string baseDir);
        string GetFilePrefix();
        string GetFormattedName(string baseName);
        bool DirectoryExists(string path);
        DirectoryInfo CreateDirectory(string path);
        bool FileExists(string path);
        FileStream CreateFile(string path);
        string ReadAllText(string path);
        IEnumerable<string> ReadLines(string path);
    }
}