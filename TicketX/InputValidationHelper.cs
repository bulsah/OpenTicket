using System;
using System.Text.RegularExpressions;
using System.Web;

namespace OpenTicket
{
    /// <summary>
    /// Helper class for input validation and sanitization
    /// Protection against XSS, SQL Injection and other injection attacks
    /// </summary>
    public static class InputValidationHelper
    {
        /// <summary>
        /// Validates email address format
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // RFC 5322 compliant email regex (simplified)
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
        /// Validates phone number format (Turkey)
        /// </summary>
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Extract only digits
            string digitsOnly = Regex.Replace(phone, @"[^\d]", "");

            // Turkish phone: 10 or 11 digits (can start with 0)
            if (digitsOnly.Length < 10 || digitsOnly.Length > 11)
                return false;

            // Should start with 5 (mobile phone)
            if (digitsOnly.StartsWith("0"))
                return digitsOnly.Length == 11 && digitsOnly[1] == '5';
            else
                return digitsOnly.Length == 10 && digitsOnly[0] == '5';
        }

        /// <summary>
        /// Checks if string contains only letters and numbers
        /// </summary>
        public static bool IsAlphanumeric(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
        }

        /// <summary>
        /// Checks if string contains only letters
        /// </summary>
        public static bool IsAlpha(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[a-zA-ZğüşıöçĞÜŞİÖÇ]+$");
        }

        /// <summary>
        /// Validates positive integer
        /// </summary>
        public static bool IsPositiveInteger(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return int.TryParse(input, out int result) && result > 0;
        }

        /// <summary>
        /// Cleans and encodes input against XSS attacks
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // HTML encode
            string sanitized = HttpUtility.HtmlEncode(input);

            // Remove dangerous characters
            sanitized = Regex.Replace(sanitized, @"[<>""']", string.Empty);

            return sanitized;
        }

        /// <summary>
        /// Cleans SQL Injection characters
        /// NOTE: This does NOT replace parameterized queries! Just an additional layer.
        /// </summary>
        public static string SanitizeSqlInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove dangerous SQL characters
            // But your main protection should be parameterized queries!
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
        /// Makes filename safe
        /// </summary>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "unnamed";

            // Remove invalid characters
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            string sanitized = Regex.Replace(fileName, invalidRegStr, "_");

            // Maximum length
            if (sanitized.Length > 255)
                sanitized = sanitized.Substring(0, 255);

            return sanitized;
        }

        /// <summary>
        /// Checks if URL is safe
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // Try to create URI
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
                return false;

            // Accept only HTTP and HTTPS
            return uriResult.Scheme == Uri.UriSchemeHttp || 
                   uriResult.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Checks string length
        /// </summary>
        public static bool IsValidLength(string input, int minLength, int maxLength)
        {
            if (input == null)
                return false;

            return input.Length >= minLength && input.Length <= maxLength;
        }

        /// <summary>
        /// Checks password strength
        /// </summary>
        public static bool IsStrongPassword(string password, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                message = "Password cannot be empty";
                return false;
            }

            if (password.Length < 8)
            {
                message = "Password must be at least 8 characters";
                return false;
            }

            if (password.Length > 128)
            {
                message = "Password is too long (maximum 128 characters)";
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
                message = "Password must contain at least 3 different character types (uppercase, lowercase, number, special character)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates Turkish ID Number (T.C. Kimlik No)
        /// </summary>
        public static bool IsValidTCKN(string tckn)
        {
            if (string.IsNullOrWhiteSpace(tckn) || tckn.Length != 11)
                return false;

            // Must be only digits
            if (!Regex.IsMatch(tckn, @"^\d{11}$"))
                return false;

            // First digit cannot be 0
            if (tckn[0] == '0')
                return false;

            // Algorithm check
            int[] digits = new int[11];
            for (int i = 0; i < 11; i++)
            {
                digits[i] = int.Parse(tckn[i].ToString());
            }

            // 10th digit check
            int sum1 = (digits[0] + digits[2] + digits[4] + digits[6] + digits[8]) * 7;
            int sum2 = digits[1] + digits[3] + digits[5] + digits[7];
            int digit10 = (sum1 - sum2) % 10;

            if (digit10 != digits[9])
                return false;

            // 11th digit check
            int sum3 = 0;
            for (int i = 0; i < 10; i++)
            {
                sum3 += digits[i];
            }
            int digit11 = sum3 % 10;

            return digit11 == digits[10];
        }

        /// <summary>
        /// Validates credit card number format (Luhn algorithm)
        /// </summary>
        public static bool IsValidCreditCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            // Extract only digits
            string digitsOnly = Regex.Replace(cardNumber, @"[^\d]", "");

            // Must be between 13-19 digits
            if (digitsOnly.Length < 13 || digitsOnly.Length > 19)
                return false;

            // Luhn algorithm
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
