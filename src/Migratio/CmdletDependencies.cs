using Migratio.Configuration;
using Migratio.Contracts;
using Migratio.Database;
using Migratio.Secrets;
using Migratio.Utils;

namespace Migratio
{
    public class CmdletDependencies
    {
        private IDatabaseProvider _databaseProvider;
        private IEnvironmentManager _environmentManager;
        private IFileManager _fileManager;
        private ISecretManager _secretManager;
        private IConfiguration _configuration;

        public IConfiguration Configuration
        {
            get
            {
                if (_configuration != null) return _configuration;
                _configuration = new ConfigurationManager(FileManager);
                return _configuration;
            }
            set => _configuration = value;
        }
        
        public ISecretManager SecretManager
        {
            get
            {
                if (_secretManager != null) return _secretManager;
                _secretManager = new SecretManager(EnvironmentManager, FileManager,Configuration);
                return _secretManager;
            }
            set => _secretManager = value;
        }

        public IDatabaseProvider DatabaseProvider
        {
            get
            {
                if (_databaseProvider != null) return _databaseProvider;
                _databaseProvider = new PostgreDb();
                return _databaseProvider;
            }
            set => _databaseProvider = value;
        }

        public IEnvironmentManager EnvironmentManager
        {
            get
            {
                if (_environmentManager != null) return _environmentManager;
                _environmentManager = new EnvironmentManager();
                return _environmentManager;
            }
            set => _environmentManager = value;
        }

        public IFileManager FileManager
        {
            get
            {
                if (_fileManager != null) return _fileManager;
                _fileManager = new FileManager();
                return _fileManager;
            }
            set => _fileManager = value;
        }
    }
}