using System.Management.Automation;
using Migratio.Core;
using Migratio.Core.Configuration;
using Migratio.Core.Contracts;
using Migratio.Core.Database;
using Migratio.Core.Secrets;
using Migratio.Core.Utils;

namespace Migratio.PowerShell.Core
{
    public abstract class BaseCmdlet : Cmdlet
    {
        private IMigratioConfiguration _migratioConfiguration;
        private IDatabaseProvider _databaseProvider;
        private IEnvironmentManager _environmentManager;
        private IFileManager _fileManager;
        private ISecretManager _secretManager;

        protected BaseCmdlet() : this(new CmdletDependencies())
        {
        }

        public BaseCmdlet(CmdletDependencies dependencies)
        {
            SecretManager = dependencies.SecretManager;
            DatabaseProvider = dependencies.DatabaseProvider;
            EnvironmentManager = dependencies.EnvironmentManager;
            FileManager = dependencies.FileManager;
            MigratioConfiguration = dependencies.MigratioConfiguration;
        }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Path to Migratio configuration file")
        ]
        [ValidateNotNullOrEmpty]
        public string ConfigFile { get; set; }

        public IMigratioConfiguration MigratioConfiguration
        {
            get
            {
                if (_migratioConfiguration != null) return _migratioConfiguration;
                _migratioConfiguration = new MigratioConfigurationManager(FileManager);
                return _migratioConfiguration;
            }
            set => _migratioConfiguration = value;
        }

        public ISecretManager SecretManager
        {
            get
            {
                if (_secretManager != null) return _secretManager;
                _secretManager = new SecretManager(EnvironmentManager, FileManager, MigratioConfiguration);
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

        protected override void BeginProcessing()
        {
            MigratioConfiguration.Load(ConfigFile);
            base.BeginProcessing();
        }
    }
}