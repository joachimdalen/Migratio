using Migratio.Contracts;
using Moq;

namespace Migratio.UnitTests.Mocks
{
    public class EnvironmentManagerMock
    {
        public Mock<IEnvironmentManager> MockInstance { get; set; }
        public IEnvironmentManager Object => MockInstance.Object;

        public EnvironmentManagerMock(MockBehavior behavior = MockBehavior.Strict)
        {
            MockInstance = new Mock<IEnvironmentManager>(behavior);
        }

        #region Setups

        public void GetEnvironmentVariable(string key, string returns)
            => MockInstance.Setup(x => x.GetEnvironmentVariable(key)).Returns(returns);

        #endregion

        #region Verification

        #endregion
    }
}