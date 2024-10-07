using Microsoft.AspNetCore.Identity;
using SmartHome.Data.Interfaces;
using SmartHome.Data.Sqlite;
using SmartHome.Data.Users;
using SmartHome.Security.Encryption;
using SQLite;
using System.Diagnostics.Contracts;

namespace SmartHome.Security.UserSecrets
{
    public class UserSecretsProcessor
    {
        IDatabaseProcessor _database;

        public UserSecretsProcessor(IDatabaseProcessor _db)
        {
            _database = _db;
        }

        public bool CreateNewUserUnsecure(string userEmail, string password)
        {
            var (hash, salt) =  SHEncryptionProcessor.HashPassword(password);
            
            return _database.CreateUserData(new UserData() { UserEmail = userEmail,
                                                             UserPasswordHash = hash,
                                                             UserSalt = salt });
        }

        public EmailPasswordAuthenticationResult AuthenticateUser(string userEmail, string password) 
        {
            EmailPasswordAuthenticationResult _result = new EmailPasswordAuthenticationResult();
            _result.Success = false;

            UserData _uData =_database.GetUserData(userEmail);

            if (_uData == null) 
            {
                _result.Success = false;
                _result.Message = "User not found.";
                return _result;
            }

            bool verified = SHEncryptionProcessor.VerifyPassword(password, _uData.UserPasswordHash, _uData.UserSalt);

            if(verified)
            {
                _result.Success = true;
                _result.UserData = _uData;
                return _result;
            }
            else
            {
                _result.Message = "Incorrect email/password combination.";
                _result.Success = false;
                return _result;
            }        
        }

        public void SaveRefreshToken(string userEmail, string refreshToken)
        {
          _database.UpdateRefreshToken(userEmail, refreshToken);
        }

        public UserData GetUserDataRefreshToken(string refreshToken)
        {
            return _database.GetUserDataFromToken(refreshToken);
        }
    }

    public class EmailPasswordAuthenticationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserData UserData { get; set; }
    }
}
