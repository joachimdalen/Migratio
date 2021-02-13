namespace Migratio.Database
{
    public class DbConnectionInfo
    {
        public string Username { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}