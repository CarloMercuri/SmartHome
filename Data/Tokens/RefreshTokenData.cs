namespace SmartHome.Data.Tokens
{
    public class RefreshTokenData
    {
        public string UserEmail { get; set; }
        public string Token { get; set; }
        public DateTime Expiration {  get; set; }
    }
}
