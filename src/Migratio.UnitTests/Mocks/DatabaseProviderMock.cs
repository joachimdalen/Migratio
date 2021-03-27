using Migratio.Contracts;
using Migratio.Database;
using Migratio.Models;
using Moq;

namespace Migratio.UnitTests.Mocks
{
    public class DatabaseProviderMock
    {
        public Mock<IDatabaseProvider> MockInstance { get; set; }
        public IDatabaseProvider Object => MockInstance.Object;

        public DatabaseProviderMock(MockBehavior behavior = MockBehavior.Strict)
        {
            MockInstance = new Mock<IDatabaseProvider>(behavior);

            MockInstance.Setup(x => x.SetConnectionInfo(It.IsAny<DbConnectionInfo>()));
        }

        #region Setups

        public void MigrationTableExists(bool returns)
            => MockInstance.Setup(x => x.MigrationTableExists()).Returns(returns);

        public void SeedingTableExists(bool returns)
            => MockInstance.Setup(x => x.SeedingTableExists()).Returns(returns);

        public void CreateMigrationTable(int returns)
            => MockInstance.Setup(x => x.CreateMigrationTable()).Returns(returns);

        public void CreateSeedersTable(int returns)
            => MockInstance.Setup(x => x.CreateSeedersTable()).Returns(returns);

        public void GetLatestIteration(int returns)
            => MockInstance.Setup(x => x.GetLatestIteration()).Returns(returns);

        public void RunTransaction(string query, int returns)
            => MockInstance.Setup(x => x.RunTransaction(query)).Returns(returns);

        public void RunTransactionAny(int returns)
            => MockInstance.Setup(x => x.RunTransaction(It.IsAny<string>())).Returns(returns);

        public void GetAppliedMigrations(Migration[] returns)
            => MockInstance.Setup(x => x.GetAppliedMigrations()).Returns(returns);

        public void GetAppliedSeeders(Seed[] returns)
            => MockInstance.Setup(x => x.GetAppliedSeeders()).Returns(returns);

        public void GetAppliedScriptsForLatestIteration(Migration[] returns)
            => MockInstance.Setup(x => x.GetAppliedScriptsForLatestIteration()).Returns(returns);

        #endregion

        #region Verification

        public void VerifyRunTransaction(string query)
            => MockInstance.Verify(x => x.RunTransaction(query));

        public void VerifyMigrationTableExists(Times times)
            => MockInstance.Verify(x => x.MigrationTableExists(), times);
        
        public void VerifySeedingTableExists(Times times)
            => MockInstance.Verify(x => x.SeedingTableExists(), times);

        public void VerifyCreateMigrationTable(Times times)
            => MockInstance.Verify(x => x.CreateMigrationTable(), times);
        
        public void VerifyCreateSeedersTable(Times times)
            => MockInstance.Verify(x => x.CreateSeedersTable(), times);

        #endregion
    }
}