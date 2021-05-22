using Migratio.Core.Contracts;
using Migratio.Core.Secrets;
using Migratio.TestCore.Mocks;
using Moq;

namespace Migratio.PowerShell.UnitTests
{
    public abstract class BaseCmdletTest
    {
        protected DatabaseProviderMock DbMock { get; set; }
        protected FileManagerMock FileManagerMock { get; set; }
        protected EnvironmentManagerMock EnvironmentManagerMock { get; set; }
        protected ISecretManager SecretManagerMock { get; set; }

        protected ConfigManagerMock ConfigManagerMock { get; set; }

        protected BaseCmdletTest(MockBehavior behavior = MockBehavior.Strict)
        {
            DbMock = new DatabaseProviderMock(behavior);
            FileManagerMock = new FileManagerMock(behavior);
            EnvironmentManagerMock = new EnvironmentManagerMock(behavior);
            ConfigManagerMock = new ConfigManagerMock(behavior);
            SecretManagerMock = new SecretManager(EnvironmentManagerMock.Object, FileManagerMock.Object,
                ConfigManagerMock.Object);
        }

        internal CmdletDependencies GetMockedDependencies()
        {
            return new CmdletDependencies
            {
                FileManager = FileManagerMock.Object,
                DatabaseProvider = DbMock.Object,
                EnvironmentManager = EnvironmentManagerMock.Object,
                SecretManager = SecretManagerMock,
                Configuration = ConfigManagerMock.Object
            };
        }
    }
}