namespace Migratio.Contracts
{
    public interface ISecretManager
    {
        string[] GetSecretsInContent(string content);
        string ReplaceSecretsInContent(string content);
        string GetEnvironmentVariable(string key);
        bool HasSecret(string value);
    }
}