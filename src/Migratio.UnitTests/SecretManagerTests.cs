using System;
using Migratio.Secrets;
using Migratio.UnitTests.Mocks;
using Xunit;

namespace Migratio.UnitTests
{
    public class SecretManagerTests
    {
        private readonly EnvironmentManagerMock _envMock;
        private readonly FileManagerMock _fileManagerMock;

        public SecretManagerTests()
        {
            _envMock = new EnvironmentManagerMock();
            _fileManagerMock = new FileManagerMock();
        }

        [Fact(DisplayName = "GetEnvironmentVariable returns from OS variables if set")]
        public void GetEnvironmentVariable_Returns_From_Os_If_Set()
        {
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", "Hello value");
            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            var result = sut.GetEnvironmentVariable("MG_SOME_VAR");

            Assert.Equal("Hello value", result);
        }

        [Fact(DisplayName = "GetEnvironmentVariable returns empty from OS if not set and no file path given")]
        public void GetEnvironmentVariable_Returns_Empty_From_Os()
        {
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", "");
            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            var result = sut.GetEnvironmentVariable("MG_SOME_VAR");

            Assert.Equal(string.Empty, result);
        }

        [Fact(DisplayName = "GetEnvironmentVariable returns empty if not set in file")]
        public void GetEnvironmentVariable_Returns_Empty_If_Not_Set_In_File()
        {
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", string.Empty);
            _fileManagerMock.ReadLines("someenv.file", new[]
            {
                "USERNAME=user",
                "BASE_ITEM=item12"
            });

            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            var result = sut.GetEnvironmentVariable("MG_SOME_VAR", "someenv.file");

            Assert.Equal(string.Empty, result);
        }

        [Fact(DisplayName = "GetEnvironmentVariable returns empty if env value is empty")]
        public void GetEnvironmentVariable_Returns_Empty_If_Env_Value_Is_Empty()
        {
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", string.Empty);
            _fileManagerMock.ReadLines("someenv.file", new[]
            {
                "MG_SOME_VAR=",
                "USERNAME=user",
                "BASE_ITEM=item12"
            });

            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            var result = sut.GetEnvironmentVariable("MG_SOME_VAR", "someenv.file");

            Assert.Equal(string.Empty, result);
        }

        [Fact(DisplayName = "GetEnvironmentVariable returns value from file")]
        public void GetEnvironmentVariable_Returns_Value_From_File()
        {
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", string.Empty);
            _fileManagerMock.ReadLines("someenv.file", new[]
            {
                "MG_SOME_VAR=this-is-the-value",
                "USERNAME=user",
                "BASE_ITEM=item12"
            });

            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            var result = sut.GetEnvironmentVariable("MG_SOME_VAR", "someenv.file");

            Assert.Equal("this-is-the-value", result);
        }

        [Fact(DisplayName = "ReplaceSecretsInContent throws if variable is empty")]
        public void ReplaceSecretsInContent_Throws_If_Variable_Is_Empty()
        {
            var content = "CREATE ${{MG_SOME_VAR}} HERE OR THERE";
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", string.Empty);

            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            Assert.Throws<Exception>(() => sut.ReplaceSecretsInContent(content, null));
        }

        [Fact(DisplayName = "ReplaceSecretsInContent replaces correctly")]
        public void ReplaceSecretsInContent_Replaces_Correctly()
        {
            var content = "CREATE ${{MG_SOME_VAR}} HERE OR THERE";
            _envMock.GetEnvironmentVariable("MG_SOME_VAR", "THIS");

            var sut = new SecretManager(_envMock.Object, _fileManagerMock.Object);

            var result = sut.ReplaceSecretsInContent(content, null);

            Assert.Equal("CREATE THIS HERE OR THERE", result);
        }
    }
}