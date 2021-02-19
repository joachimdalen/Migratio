using System.Management.Automation;
using Migratio.Database;

namespace Migratio.Core
{
    public abstract class DbCmdlet : BaseCmdlet
    {
        public DbCmdlet()
        {
        }

        public DbCmdlet(CmdletDependencies dependencies) : base(dependencies)
        {
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Username { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Database { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public int Port { get; set; } = 5432;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Host { get; set; } = "127.0.0.1";

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Schema { get; set; } = "public";

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string EnvFile { get; set; }

        protected DbConnectionInfo GetConnectionInfo()
        {
            var password = SecretManager.GetEnvironmentVariable("MG_DB_PASSWORD");

            return new DbConnectionInfo
            {
                Database = Database,
                Username = Username,
                Host = Host,
                Password = password,
                Port = Port,
                Schema = Schema
            };
        }
    }
}