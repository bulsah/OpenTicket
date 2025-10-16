using System;
using System.Security.Cryptography;
using System.Text;

namespace TicketX
{
    /// <summary>
    /// Güvenli parola hashleme için PBKDF2 kullanan sınıf
    /// SHA1 yerine kullanılmalıdır
    /// </summary>
    public static class PasswordHasher
    {
        // PBKDF2 iterasyon sayısı (OWASP önerisi: en az 100,000)
        private const int Iterations = 100000;
        
        // Salt boyutu (byte cinsinden)
        private const int SaltSize = 32;
        
        // Hash boyutu (byte cinsinden)
        private const int HashSize = 32;

        /// <summary>
        /// Parolayı hashler ve salt ile birlikte döndürür
        /// Format: {iterations}.{salt}.{hash}
        /// </summary>
        /// <param name="password">Hashlenecek parola</param>
        /// <returns>Hashlenmiş parola (iterations.salt.hash formatında)</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Parola boş olamaz", nameof(password));

            // Rastgele salt oluştur
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // PBKDF2 ile hash oluştur
            byte[] hash = GenerateHash(password, salt, Iterations);

            // Format: {iterations}.{salt}.{hash}
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Parolayı doğrular
        /// </summary>
        /// <param name="password">Kontrol edilecek parola</param>
        /// <param name="hashedPassword">Hashlenmiş parola (HashPassword metodundan dönen format)</param>
        /// <returns>Parola doğruysa true</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                // Hashlenmiş parolayı parçala
                var parts = hashedPassword.Split('.');
                if (parts.Length != 3)
                    return false;

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hash = Convert.FromBase64String(parts[2]);

                // Girilen parolayı aynı salt ile hashle
                byte[] testHash = GenerateHash(password, salt, iterations);

                // Hashler eşleşiyor mu kontrol et (timing attack'a karşı sabit zamanlı karşılaştırma)
                return SlowEquals(hash, testHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// PBKDF2 kullanarak hash oluşturur
        /// </summary>
        private static byte[] GenerateHash(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        /// <summary>
        /// Timing attack'a karşı sabit zamanlı karşılaştırma
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
        /// Eski SHA1 hash'lerini yeni PBKDF2 formatına migrate etmek için
        /// SHA1 hash'i doğrular (sadece migration için kullanılmalı)
        /// </summary>
        [Obsolete("Sadece eski parolaları migrate etmek için kullanın. Yeni sistemde PasswordHasher.HashPassword kullanın.")]
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

