namespace SmartHome.Security.KeyStorage
{
    public class LocalSecretsStorage : ISecretsStorage
    {
        public string GetAudience()
        {
            return "https://www.selution.com/auth";
        }

        public string GetIssuer()
        {
            return "https://www.selution.com";
        }

        public string GetMainSecurityKey()
        {
            return "aarZCqlczkH0WsF7w6XkgzhcXf9puThC8UqVeApF7hXQIcpyl/35EOZ/Oaa9eZ9o";
        }
    }
}
