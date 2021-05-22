using Migratio.Core.Contracts;
using Moq;

namespace Migratio.TestCore.Mocks
{
    public class SecretManagerMock
    {
        public Mock<ISecretManager> MockInstance { get; set; }
        public ISecretManager Object => MockInstance.Object;

        public SecretManagerMock(MockBehavior behavior = MockBehavior.Strict)
        {
            MockInstance = new Mock<ISecretManager>(behavior);
            MockInstance.Setup(x => x.GetEnvironmentVariable("MG_DB_PASSWORD")).Returns("password");
        }
    }
}