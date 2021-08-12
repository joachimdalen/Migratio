using System.IO;
using System.Linq;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMgSeederTests : BaseCmdletTest
    {
        [Fact(DisplayName = "New-MgSeeder creates seeds directory if not exists")]
        public void NewMgSeeder_Creates_Seeds_Directory_If_Not_Exists()
        {
            var seedPath = Path.Join("migrations", "seeders");

            ConfigManagerMock.SeedersDirectory(seedPath);
            FileManagerMock.DirectoryExists(seedPath, false);
            FileManagerMock.CreateDirectory(seedPath);
            FileManagerMock.GetFilePrefix("test_prefix");
            FileManagerMock.GetFormattedName("file_name");
            FileManagerMock.FileExists("migrations/seeders/test_prefix_file_name.sql", false);
            FileManagerMock.CreateFile("migrations/seeders/test_prefix_file_name.sql");

            var command = new NewMgSeeder(GetMockedDependencies())
            {
                Name = "This is my migration"
            };

            var result = command.Invoke()?.OfType<string>().ToArray()[1];
            Assert.NotNull(result);
            Assert.Contains(seedPath, result);
            FileManagerMock.VerifyCreateDirectory(seedPath, Times.Once());
            FileManagerMock.VerifyCreateFile("migrations/seeders/test_prefix_file_name.sql", Times.Once());
        }

        [Fact(DisplayName = "New-MgSeeder default constructor constructs")]
        public void NewMgSeeder_Default_Constructor_Constructs()
        {
            var result = new NewMgSeeder
            {
                Name = "This is my name",
                MigrationRootDir = "migrations"
            };

            Assert.NotNull(result);
        }
    }
}