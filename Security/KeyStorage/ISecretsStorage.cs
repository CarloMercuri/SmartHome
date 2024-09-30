namespace SmartHome.Security.KeyStorage
{
    public interface ISecretsStorage
    {
        string GetMainSecurityKey();
        string GetIssuer();
        string GetAudience();
    }
}
