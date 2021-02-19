using System.IO;
using Migratio.Contracts;
using Moq;

namespace Migratio.UnitTests.Mocks
{
    public class FileManagerMock
    {
        public Mock<IFileManager> MockInstance { get; set; }
        public IFileManager Object => MockInstance.Object;

        public FileManagerMock(MockBehavior behavior = MockBehavior.Strict)
        {
            MockInstance = new Mock<IFileManager>(behavior);
        }

        #region Setups

        public void RolloutDirectory(string returns)
            => MockInstance.Setup(x => x.RolloutDirectory(It.IsAny<string>())).Returns(returns);

        public void RollbackDirectory(string returns)
            => MockInstance.Setup(x => x.RollbackDirectory(It.IsAny<string>())).Returns(returns);

        public void SeedersDirectory(string returns)
            => MockInstance.Setup(x => x.SeedersDirectory(It.IsAny<string>())).Returns(returns);

        public void GetAllFilesInFolder(string[] returns)
            => MockInstance.Setup(x => x.GetAllFilesInFolder(It.IsAny<string>())).Returns(returns);

        public void ReadAllText(string path, string returns)
            => MockInstance.Setup(x => x.ReadAllText(path)).Returns(returns);
        
        public void ReadLines(string path, string[] returns)
            => MockInstance.Setup(x => x.ReadLines(path)).Returns(returns);

        public void FileExists(string path, bool returns)
            => MockInstance.Setup(x => x.FileExists(path)).Returns(returns);

        public void DirectoryExists(string path, bool returns)
            => MockInstance.Setup(x => x.DirectoryExists(path)).Returns(returns);

        public void CreateDirectory(string path)
            => MockInstance.Setup(x => x.CreateDirectory(path)).Returns(new DirectoryInfo(path));

        public void GetFilePrefix(string returns)
            => MockInstance.Setup(x => x.GetFilePrefix()).Returns(returns);

        public void GetFormattedName(string returns)
            => MockInstance.Setup(x => x.GetFormattedName(It.IsAny<string>())).Returns(returns);

        public void CreateFile(string path)
            => MockInstance.Setup(x => x.CreateFile(path)).Returns((FileStream) null);

        #endregion

        #region Verification

        public void VerifyReadAllText(string path, Times times)
            => MockInstance.Verify(x => x.ReadAllText(path), times);

        public void VerifyCreateDirectory(string path, Times times)
            => MockInstance.Verify(x => x.CreateDirectory(path), times);

        public void VerifyCreateFile(string path, Times times)
            => MockInstance.Verify(x => x.CreateFile(path), times);

        #endregion
    }
}