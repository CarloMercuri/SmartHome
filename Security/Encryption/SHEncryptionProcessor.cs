using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace SmartHome.Security.Encryption
{
    public class SHEncryptionProcessor
    {
        private static int _iterations = 50000;
        private static int _numBytes = 256;

        public static (string Hash, string Salt) HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16]; // 128-bit salt
            RandomNumberGenerator.Fill(salt);

            // Hash the password with the salt
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _iterations,
                numBytesRequested: _numBytes
            );

            string saltString = Convert.ToBase64String(salt);
            string hashString = Convert.ToBase64String(hash);

            return (hashString, saltString);
        }

        public static bool VerifyPassword(string password, string hash, string salt)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hash the provided password using the same salt
            byte[] newHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _iterations,
                numBytesRequested: _numBytes
            );

            // Compare the hashes
            return CryptographicOperations.FixedTimeEquals(hashBytes, newHash);
        }
    }
}
