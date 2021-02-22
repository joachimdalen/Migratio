using Migratio.Configuration;
using Migratio.Contracts;
using Moq;

namespace Migratio.UnitTests.Mocks
{
    public class ConfigManagerMock
    {
        public Mock<IConfiguration> MockInstance { get; set; }
        public IConfiguration Object => MockInstance.Object;

        public ConfigManagerMock(MockBehavior behavior = MockBehavior.Strict)
        {
            MockInstance = new Mock<IConfiguration>(behavior);
            MockInstance.Setup(x => x.GetKeyFromMapping("MG_DB_PASSWORD")).Returns("MG_DB_PASSWORD");
            MockInstance.Setup(x => x.Load(It.IsAny<string>())).Returns(true);
        }

        public void GetKeyFromMapping(string key, string returns)
        {
            MockInstance.Setup(x => x.GetKeyFromMapping(key)).Returns(returns);
        }

        public void ConfigReturns(MgConfig config)
        {
            MockInstance.SetupGet(x => x.Config).Returns(config);
        }

        public void Resolve<T>(T first, T second, T defaultValue, T returns)
        {
            MockInstance.Setup(x => x.Resolve(first, second, defaultValue)).Returns(returns);
        }

        public void RolloutDirectory(string returns)
            => MockInstance.Setup(x => x.RolloutDirectory(It.IsAny<string>(), It.IsAny<string>())).Returns(returns);

        public void RollbackDirectory(string returns)
            => MockInstance.Setup(x => x.RollbackDirectory(It.IsAny<string>(), It.IsAny<string>())).Returns(returns);

        public void SeedersDirectory(string returns)
            => MockInstance.Setup(x => x.SeedersDirectory(It.IsAny<string>(), It.IsAny<string>())).Returns(returns);
    }
}