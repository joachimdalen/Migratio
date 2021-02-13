using Xunit;

namespace Migratio.UnitTests
{
    public class FileManagerTests
    {
        [Theory(DisplayName = "GetFormattedName formats correctly")]
        [InlineData("This is a migration", "this_is_a_migration")]
        [InlineData("This is - a migration", "this_is___a_migration")]
        [InlineData("Migration 2.0", "migration_2_0")]
        [InlineData("This is Å migration", "this_is___migration")]
        public void GetFormattedName_Formats_Correctly(string text, string result)
        {
            var fm = new FileManager();
            var res = fm.GetFormattedName(text);
            Assert.Equal(result, res);
        }
    }
}