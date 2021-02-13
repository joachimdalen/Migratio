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

        public void GetAllFilesInFolder(string[] returns)
            => MockInstance.Setup(x => x.GetAllFilesInFolder(It.IsAny<string>())).Returns(returns);

        public void ReadAllText(string path, string returns)
            => MockInstance.Setup(x => x.ReadAllText(path)).Returns(returns);

        #endregion

        #region Verification

        public void VerifyReadAllText(string path, Times times)
            => MockInstance.Verify(x => x.ReadAllText(path), times);

        #endregion
    }
}