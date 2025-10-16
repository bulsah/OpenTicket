using System;
using System.Text.RegularExpressions;
using System.Web;

namespace TicketX
{
    /// <summary>
    /// Input validation ve sanitization için helper sınıfı
    /// XSS, SQL Injection ve diğer injection saldırılarına karşı koruma
    /// </summary>
    public static class InputValidationHelper
    {
        /// <summary>
        /// Email adresi formatını doğrular
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // RFC 5322 compliant email regex (basitleştirilmiş)
                var regex = new Regex(
                    @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled,
                    TimeSpan.FromMilliseconds(250)
                );

                return regex.IsMatch(email) && email.Length <= 254;
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Telefon numarası formatını doğrular (Türkiye)
        /// </summary>
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Sadece rakamları al
            string digitsOnly = Regex.Replace(phone, @"[^\d]", "");

            // Türkiye telefon: 10 veya 11 hane (0 ile başlayabilir)
            if (digitsOnly.Length < 10 || digitsOnly.Length > 11)
                return false;

            // 5 ile başlamalı (cep telefonu)
            if (digitsOnly.StartsWith("0"))
                return digitsOnly.Length == 11 && digitsOnly[1] == '5';
            else
                return digitsOnly.Length == 10 && digitsOnly[0] == '5';
        }

        /// <summary>
        /// String'in sadece harf ve rakam içerdiğini kontrol eder
        /// </summary>
        public static bool IsAlphanumeric(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
        }

        /// <summary>
        /// String'in sadece harf içerdiğini kontrol eder
        /// </summary>
        public static bool IsAlpha(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[a-zA-ZğüşıöçĞÜŞİÖÇ]+$");
        }

        /// <summary>
        /// Pozitif integer doğrular
        /// </summary>
        public static bool IsPositiveInteger(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return int.TryParse(input, out int result) && result > 0;
        }

        /// <summary>
        /// XSS saldırılarına karşı input'u temizler ve encode eder
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // HTML encode
            string sanitized = HttpUtility.HtmlEncode(input);

            // Tehlikeli karakterleri temizle
            sanitized = Regex.Replace(sanitized, @"[<>""']", string.Empty);

            return sanitized;
        }

        /// <summary>
        /// SQL Injection karakterlerini temizler
        /// NOT: Bu, parametreli sorguların yerine geçmez! Sadek ek bir katman.
        /// </summary>
        public static string SanitizeSqlInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Tehlikeli SQL karakterlerini temizle
            // Ama asıl korumanız parametreli sorgular olmalı!
            string sanitized = input
                .Replace("'", "''")  // Single quote escape
                .Replace("--", "")   // SQL comment
                .Replace(";", "")    // Statement separator
                .Replace("/*", "")   // Multi-line comment
                .Replace("*/", "")   // Multi-line comment
                .Replace("xp_", "")  // Extended stored procedures
                .Replace("sp_", "");  // System stored procedures

            return sanitized;
        }

        /// <summary>
        /// Dosya adını güvenli hale getirir
        /// </summary>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "unnamed";

            // Geçersiz karakterleri temizle
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            string sanitized = Regex.Replace(fileName, invalidRegStr, "_");

            // Maksimum uzunluk
            if (sanitized.Length > 255)
                sanitized = sanitized.Substring(0, 255);

            return sanitized;
        }

        /// <summary>
        /// URL'nin güvenli olduğunu kontrol eder
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // URI oluşturulmaya çalışılır
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
                return false;

            // Sadece HTTP ve HTTPS kabul et
            return uriResult.Scheme == Uri.UriSchemeHttp || 
                   uriResult.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// String uzunluğunu kontrol eder
        /// </summary>
        public static bool IsValidLength(string input, int minLength, int maxLength)
        {
            if (input == null)
                return false;

            return input.Length >= minLength && input.Length <= maxLength;
        }

        /// <summary>
        /// Parola güçlülüğünü kontrol eder
        /// </summary>
        public static bool IsStrongPassword(string password, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                message = "Parola boş olamaz";
                return false;
            }

            if (password.Length < 8)
            {
                message = "Parola en az 8 karakter olmalıdır";
                return false;
            }

            if (password.Length > 128)
            {
                message = "Parola çok uzun (maksimum 128 karakter)";
                return false;
            }

            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            bool hasDigit = Regex.IsMatch(password, @"\d");
            bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]");

            int score = 0;
            if (hasUpperCase) score++;
            if (hasLowerCase) score++;
            if (hasDigit) score++;
            if (hasSpecialChar) score++;

            if (score < 3)
            {
                message = "Parola en az 3 farklı karakter tipinden oluşmalıdır (büyük harf, küçük harf, rakam, özel karakter)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// TCNO (T.C. Kimlik No) doğrular
        /// </summary>
        public static bool IsValidTCKN(string tckn)
        {
            if (string.IsNullOrWhiteSpace(tckn) || tckn.Length != 11)
                return false;

            // Sadece rakam olmalı
            if (!Regex.IsMatch(tckn, @"^\d{11}$"))
                return false;

            // İlk hane 0 olamaz
            if (tckn[0] == '0')
                return false;

            // Algoritma kontrolü
            int[] digits = new int[11];
            for (int i = 0; i < 11; i++)
            {
                digits[i] = int.Parse(tckn[i].ToString());
            }

            // 10. hane kontrolü
            int sum1 = (digits[0] + digits[2] + digits[4] + digits[6] + digits[8]) * 7;
            int sum2 = digits[1] + digits[3] + digits[5] + digits[7];
            int digit10 = (sum1 - sum2) % 10;

            if (digit10 != digits[9])
                return false;

            // 11. hane kontrolü
            int sum3 = 0;
            for (int i = 0; i < 10; i++)
            {
                sum3 += digits[i];
            }
            int digit11 = sum3 % 10;

            return digit11 == digits[10];
        }

        /// <summary>
        /// Kart numarası formatını kontrol eder (Luhn algoritması)
        /// </summary>
        public static bool IsValidCreditCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            // Sadece rakamları al
            string digitsOnly = Regex.Replace(cardNumber, @"[^\d]", "");

            // 13-19 hane arası olmalı
            if (digitsOnly.Length < 13 || digitsOnly.Length > 19)
                return false;

            // Luhn algoritması
            int sum = 0;
            bool alternate = false;

            for (int i = digitsOnly.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(digitsOnly[i].ToString());

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }
    }
}

