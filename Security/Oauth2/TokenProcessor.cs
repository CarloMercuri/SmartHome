using Microsoft.IdentityModel.Tokens;
using SmartHome.Security.KeyStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHome.Security.Oauth2
{
    public class TokenProcessor
    {
        ISecretsStorage _secrets = new LocalSecretsStorage();

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secrets.GetMainSecurityKey()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _secrets.GetIssuer(),
                audience: _secrets.GetAudience(),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Access token expiration
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString(); // Use a more robust generation method if needed
        }

        public ClaimsPrincipal ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secrets.GetMainSecurityKey());

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _secrets.GetIssuer(),
                    ValidateAudience = true,
                    ValidAudience = _secrets.GetAudience(),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
