using Konscious.Security.Cryptography;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Business
{
    public class clsSecurity
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[size];
            rng.GetBytes(salt);
            return salt;
        }

        public static string HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 64 // 64 MB
            };

            byte[] hash = argon2.GetBytes(32);

            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            string newHash = HashPassword(enteredPassword, saltBytes);

            return newHash == storedHash;
        }
    }
}
