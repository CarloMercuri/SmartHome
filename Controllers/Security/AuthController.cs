using Microsoft.AspNetCore.Mvc;
using SmartHome.Security.Oauth2;
using System.Security.Claims;

namespace SmartHome.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenProcessor _tokenProcessor;

        public AuthController(TokenProcessor tokenProcessor)
        {
            _tokenProcessor = tokenProcessor;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Verify user credentials here
            // If valid, generate tokens
            if (IsValidUser(request.Username, request.Password))
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.Username),
                // Add other claims as needed
            };

                var accessToken = _tokenProcessor.GenerateAccessToken(claims);
                var refreshToken = _tokenProcessor.GenerateRefreshToken();

                // Store the refresh token with the user (e.g., in the database)

                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }

            return Unauthorized();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            // Validate refresh token and issue a new access token
            var claimsPrincipal = ValidateRefreshToken(request.RefreshToken);
            if (claimsPrincipal != null)
            {
                var newAccessToken = _tokenProcessor.GenerateAccessToken(claimsPrincipal.Claims);
                var newRefreshToken = _tokenProcessor.GenerateRefreshToken();

                // Store the new refresh token with the user and invalidate the old one

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }

            return Unauthorized();
        }

        private bool IsValidUser(string username, string password)
        {
            // Add your user validation logic here
            return true; // Replace with actual validation
        }

        private ClaimsPrincipal ValidateRefreshToken(string refreshToken)
        {
            // Add your refresh token validation logic here
            return new ClaimsPrincipal(); // Replace with actual claims if valid
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }
}
