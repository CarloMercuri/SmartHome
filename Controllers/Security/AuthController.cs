using Microsoft.AspNetCore.Mvc;
using SmartHome.Data.Interfaces;
using SmartHome.Data.Sqlite;
using SmartHome.Data.Users;
using SmartHome.Security.Oauth2;
using SmartHome.Security.UserSecrets;
using SQLitePCL;
using System.Net;
using System.Security.Claims;

namespace SmartHome.Controllers.Security
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        UserSecretsProcessor _processor;

        public AuthController(UserSecretsProcessor _usersProcessor)
        {
            _processor = _usersProcessor;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // If valid, generate tokens
                EmailPasswordAuthenticationResult _authResult = _processor.AuthenticateUser(request.UserEmail, request.Password);

                if (!_authResult.Success)
                {
                    return Unauthorized(new { Result = "Unauthorized", Message = _authResult.Message });
                }

                var accessToken = TokenProcessor.GenerateAccessToken(request.UserEmail);
                var refreshToken = TokenProcessor.GenerateRefreshToken();

                _processor.SaveRefreshToken(_authResult.UserData.UserEmail, refreshToken);
                

                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, "Internal error.");
            }
          

        }

        [Route("CreateNewUser")]
        [HttpPost] 
        public IActionResult CreateNewUser(CreateNewUserRequest request)
        {
            try
            {
                bool success = _processor.CreateNewUserUnsecure(request.UserEmail, request.Password);

                return Ok(success? "Success" : "Failed");
            }
            catch (Exception ex)
            {

                return base.StatusCode((int)HttpStatusCode.InternalServerError, "Error processing the request.");
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            // Validate refresh token and issue a new access token
            UserData _uData = _processor.GetUserDataRefreshToken(request.RefreshToken);

            if(_uData is null)
            {
                return Unauthorized(new {Success = false, Message = "Invalid Refresh Token"});
            }

            var newAccessToken = TokenProcessor.GenerateAccessToken(request.RefreshToken);
            var newRefreshToken = TokenProcessor.GenerateRefreshToken();
            _processor.SaveRefreshToken(_uData.UserEmail, newRefreshToken);
            // Store the new refresh token with the user and invalidate the old one

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
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
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }

    public class CreateNewUserRequest
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
