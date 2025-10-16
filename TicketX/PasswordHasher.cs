using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenTicket
{
    /// <summary>
    /// Secure password hashing class using PBKDF2
    /// Should be used instead of SHA1
    /// </summary>
    public static class PasswordHasher
    {
        // PBKDF2 iteration count (OWASP recommendation: at least 100,000)
        private const int Iterations = 100000;
        
        // Salt size (in bytes)
        private const int SaltSize = 32;
        
        // Hash size (in bytes)
        private const int HashSize = 32;

        /// <summary>
        /// Hashes password and returns it with salt
        /// Format: {iterations}.{salt}.{hash}
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <returns>Hashed password (in iterations.salt.hash format)</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Generate random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Generate hash using PBKDF2
            byte[] hash = GenerateHash(password, salt, Iterations);

            // Format: {iterations}.{salt}.{hash}
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies password
        /// </summary>
        /// <param name="password">Password to verify</param>
        /// <param name="hashedPassword">Hashed password (format returned by HashPassword method)</param>
        /// <returns>True if password is valid</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                // Split hashed password
                var parts = hashedPassword.Split('.');
                if (parts.Length != 3)
                    return false;

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hash = Convert.FromBase64String(parts[2]);

                // Hash entered password with same salt
                byte[] testHash = GenerateHash(password, salt, iterations);

                // Check if hashes match (constant-time comparison to prevent timing attacks)
                return SlowEquals(hash, testHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates hash using PBKDF2
        /// </summary>
        private static byte[] GenerateHash(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        /// <summary>
        /// Constant-time comparison to prevent timing attacks
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return false;

            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        /// <summary>
        /// For migrating old SHA1 hashes to new PBKDF2 format
        /// Verifies SHA1 hash (should only be used for migration)
        /// </summary>
        [Obsolete("Only use this to migrate old passwords. Use PasswordHasher.HashPassword in new system.")]
        public static bool VerifyLegacySHA1(string password, string sha1Hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(sha1Hash))
                return false;

            try
            {
                var bytes = new UnicodeEncoding().GetBytes(password);
                using (var sha1 = new SHA1Managed())
                {
                    var hash = sha1.ComputeHash(bytes);
                    var sb = new StringBuilder();
                    foreach (var b in hash)
                    {
                        sb.AppendFormat("{0:x2}", b);
                    }
                    return sb.ToString().Equals(sha1Hash, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

