using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using Migratio.Contracts;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMigratioSeederTests
    {
        [Fact(DisplayName = "New-MigratioSeeder creates seeds directory if not exists")]
        public void NewMigratioSeeder_Creates_Seeds_Directory_If_Not_Exists()
        {
            var seedPath = Path.Join("migrations", "seeders");
            var fileManagerMock = new Mock<IFileManager>();
            fileManagerMock.Setup(x => x.SeedersDirectory("migrations")).Returns(seedPath);
            fileManagerMock.Setup(x => x.DirectoryExists(seedPath)).Returns(false);
            fileManagerMock.Setup(x => x.CreateDirectory(seedPath)).Returns(new DirectoryInfo(seedPath));

            var command = new NewMigratioSeeder(fileManagerMock.Object)
            {
                Name = "This is my migration"
            };

            var result = command.Invoke()?.OfType<string>().First();
            Assert.NotNull(result);
            Assert.Contains(seedPath, result);
            fileManagerMock.Verify(x => x.CreateDirectory(seedPath), Times.Once);
        }

        [Fact(DisplayName = "New-MigratioSeeder default constructor constructs")]
        public void NewMigratioSeeder_Default_Constructor_Constructs()
        {
            var result = new NewMigratioSeeder
            {
                Name = "This is my name",
                MigrationRootDir = "migrations"
            };

            Assert.NotNull(result);
        }
    }
}