using System.Security.Cryptography;
using System.Text;

namespace FluffyBarkFriendsWebApp.Helpers
{
    public class SecurityHelper
    {
        private static int saltSize = 16; 
        private const int hashSize = 32; 
        private const int iteration = 10000;

       
        private static byte[] encryptionKey = Encoding.UTF8.GetBytes("FluffyBarkFriendsKey2026Secure12");

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: iteration,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: hashSize);

            byte[] hashBytes = new byte[saltSize + hashSize];

            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes;

            try
            {
                hashBytes = Convert.FromBase64String(storedHash);
            }
            catch
            {
                return false;
            }

            if (hashBytes.Length != saltSize + hashSize)
            {
                return false;
            }

            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                iterations: iteration,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: hashSize);

            byte[] savedHash = new byte[hashSize];
            Array.Copy(hashBytes, saltSize, savedHash, 0, hashSize);

            return CryptographicOperations.FixedTimeEquals(enteredHash, savedHash);
        }

        public static string EncryptEmail(string email)
        {
            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.GenerateIV();

            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (StreamWriter writer = new StreamWriter(cryptoStream))
            {
                writer.Write(email);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string DecryptEmail(string encryptedEmail)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedEmail);

            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;

            byte[] iv = new byte[16];
            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using MemoryStream memoryStream = new MemoryStream(
                cipherBytes,
                iv.Length,
                cipherBytes.Length - iv.Length);

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
    }
}