namespace Migratio.Contracts
{
    public interface ISecretManager
    {
        string[] GetSecretsInContent(string content);
        string ReplaceSecretsInContent(string content, string envFilePath);
        string GetEnvironmentVariable(string key, string envFilePath = null);
        bool HasSecret(string value);
    }
}