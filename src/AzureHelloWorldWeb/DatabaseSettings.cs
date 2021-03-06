namespace AzureHelloWorldWeb
{
    public class DatabaseSettings
    {
        public PersistenceMode PersistenceMode { get; set; }
        public string ConnectionSecretArn { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
    }

    public enum PersistenceMode
    {
        DynamoDb = 1,
        Database = 2
    }
}