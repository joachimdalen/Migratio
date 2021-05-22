using Migratio.Core.Database;
using YamlDotNet.Serialization;

namespace Migratio.Core.Configuration
{
    public class MgDb
    {
        /// <summary>
        /// Specifies the hostname or ip address of the machine to connect to
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Specifies the name of the database to connect to
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Username of database user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the database user. Only settable from configuration file or env variable MG_DB_PASSWORD
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Specifies the port on the database server to connect to
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Specifies the default database schema. Only valid for Postgres
        /// </summary>
        public string Schema { get; set; }


        public DbConnectionInfo GetConnectionInfo()
        {
            return new DbConnectionInfo
            {
                Database = Database,
                Host = Host,
                Password = Password,
                Port = Port,
                Schema = Schema,
                Username = Username
            };
        }
    }
}