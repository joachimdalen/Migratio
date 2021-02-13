using System.Management.Automation;
using Migratio.Database;

namespace Migratio
{
    public abstract class BaseCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Password { get; set; }

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

        protected DbConnectionInfo GetConnectionInfo()
        {
            return new DbConnectionInfo
            {
                Database = Database,
                Username = Username,
                Host = Host,
                Password = Password,
                Port = Port,
                Schema = Schema
            };
        }
    }
}