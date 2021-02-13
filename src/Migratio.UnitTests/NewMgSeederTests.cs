using System.IO;
using System.Linq;
using Migratio.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMgSeederTests
    {
        private readonly FileManagerMock _fileManagerMock;

        public NewMgSeederTests()
        {
            _fileManagerMock = new FileManagerMock();
        }

        [Fact(DisplayName = "New-MgSeeder creates seeds directory if not exists")]
        public void NewMgSeeder_Creates_Seeds_Directory_If_Not_Exists()
        {
            var seedPath = Path.Join("migrations", "seeders");

            _fileManagerMock.SeedersDirectory(seedPath);
            _fileManagerMock.DirectoryExists(seedPath, false);
            _fileManagerMock.CreateDirectory(seedPath);
            _fileManagerMock.GetFilePrefix("test_prefix");
            _fileManagerMock.GetFormattedName("file_name");
            _fileManagerMock.FileExists("migrations/seeders/test_prefix_file_name.sql", false);
            _fileManagerMock.CreateFile("migrations/seeders/test_prefix_file_name.sql");

            var command = new NewMgSeeder(_fileManagerMock.Object)
            {
                Name = "This is my migration"
            };

            var result = command.Invoke()?.OfType<string>().First();
            Assert.NotNull(result);
            Assert.Contains(seedPath, result);
            _fileManagerMock.VerifyCreateDirectory(seedPath, Times.Once());
            _fileManagerMock.VerifyCreateFile("migrations/seeders/test_prefix_file_name.sql", Times.Once());
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