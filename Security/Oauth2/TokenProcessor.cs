using Microsoft.IdentityModel.Tokens;
using SmartHome.Security.KeyStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartHome.Security.Oauth2
{
    public static class TokenProcessor
    {
        static ISecretsStorage _secrets = new LocalSecretsStorage();

        public static string GenerateAccessToken(string email)
        {
            // Define the secret key (this should be stored securely)
            var key = Encoding.ASCII.GetBytes(_secrets.GetMainSecurityKey());

            // Define the claims (user info that you want to embed in the token)
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),  // Email claim
                new Claim(JwtRegisteredClaimNames.Sub, email),  // Subject (sub) using email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID (jti)
            };

            // Create the security token descriptor with claims and other settings
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiry time (1 hour)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Generate the token using JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the generated token as a string
            return tokenHandler.WriteToken(token);
        }

       
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];  // 32 bytes = 256-bit token
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);  // Fill the array with cryptographically secure random bytes
            }

            // Convert the random bytes to a Base64 string (can use hexadecimal or other encoding if you prefer)
            return Convert.ToBase64String(randomNumber);
        }

        public static string ValidateAndExtractEmail(string token)
        {
            var key = Encoding.ASCII.GetBytes(_secrets.GetMainSecurityKey()); // Same secret key used to sign the token

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // Set validation parameters (key, issuer, audience, etc.)
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,  // Adjust as per your needs (e.g., if you want to validate issuer)
                    ValidateAudience = false, // Adjust as per your needs (e.g., if you want to validate audience)
                    ValidateLifetime = true,  // Check token expiry
                    ClockSkew = TimeSpan.Zero  // Optional: default is 5 minutes, set to zero to reduce time skew
                };

                // Validate the token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Check if the token is a valid JWT
                if (!(validatedToken is JwtSecurityToken jwtToken) || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                // Extract the email claim
                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                return email;  // Return email if found and token is valid
            }
            catch (Exception ex)
            {
                // Token validation failed (e.g., expired or invalid)
                return null;
            }
        }
    }
}
