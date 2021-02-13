using System.IO;
using System.Linq;
using Migratio.Contracts;
using Moq;
using Xunit;

namespace Migratio.UnitTests
{
    public class NewMgSeederTests
    {
        [Fact(DisplayName = "New-MgSeeder creates seeds directory if not exists")]
        public void NewMgSeeder_Creates_Seeds_Directory_If_Not_Exists()
        {
            var seedPath = Path.Join("migrations", "seeders");
            var fileManagerMock = new Mock<IFileManager>();
            fileManagerMock.Setup(x => x.SeedersDirectory("migrations")).Returns(seedPath);
            fileManagerMock.Setup(x => x.DirectoryExists(seedPath)).Returns(false);
            fileManagerMock.Setup(x => x.CreateDirectory(seedPath)).Returns(new DirectoryInfo(seedPath));

            var command = new NewMgSeeder(fileManagerMock.Object)
            {
                Name = "This is my migration"
            };

            var result = command.Invoke()?.OfType<string>().First();
            Assert.NotNull(result);
            Assert.Contains(seedPath, result);
            fileManagerMock.Verify(x => x.CreateDirectory(seedPath), Times.Once);
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