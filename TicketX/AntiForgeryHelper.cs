using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace OpenTicket
{
    /// <summary>
    /// Provides protection against CSRF (Cross-Site Request Forgery) attacks
    /// Adds and validates anti-forgery tokens for forms
    /// </summary>
    public static class AntiForgeryHelper
    {
        private const string TokenSessionKey = "__RequestVerificationToken";
        private const string TokenFormKey = "__RequestVerificationToken";

        /// <summary>
        /// Generates anti-forgery token and saves it to session
        /// Should be used as hidden field in forms
        /// </summary>
        /// <returns>Hidden input HTML string</returns>
        public static string GenerateToken()
        {
            var context = HttpContext.Current;
            if (context?.Session == null)
                throw new InvalidOperationException("Session is not available");

            // Generate new token
            string token = GenerateRandomToken();
            
            // Save to session
            context.Session[TokenSessionKey] = token;

            // Return as hidden field
            return $"<input type=\"hidden\" name=\"{TokenFormKey}\" value=\"{HttpUtility.HtmlEncode(token)}\" />";
        }

        /// <summary>
        /// Returns token as HtmlString (for ASP.NET)
        /// </summary>
        public static IHtmlString GetTokenHtml()
        {
            return new HtmlString(GenerateToken());
        }

        /// <summary>
        /// Validates token from form
        /// Must be called in POST operations
        /// </summary>
        /// <exception cref="InvalidOperationException">If token is invalid</exception>
        public static void ValidateToken()
        {
            var context = HttpContext.Current;
            if (context?.Session == null)
                throw new InvalidOperationException("Session is not available");

            // Token from session
            string sessionToken = context.Session[TokenSessionKey] as string;
            
            // Token from form
            string formToken = context.Request.Form[TokenFormKey];

            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new InvalidOperationException("Session token not found. Please refresh the page.");

            if (string.IsNullOrWhiteSpace(formToken))
                throw new InvalidOperationException("Form token not found. Potential CSRF attack!");

            // Compare tokens (secure against timing attacks)
            if (!SlowEquals(sessionToken, formToken))
                throw new InvalidOperationException("Token validation failed! Potential CSRF attack!");

            // Refresh token after use (replay attack protection)
            context.Session.Remove(TokenSessionKey);
        }

        /// <summary>
        /// Validates token, returns bool instead of throwing error
        /// </summary>
        public static bool TryValidateToken()
        {
            try
            {
                ValidateToken();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates random cryptographic token
        /// </summary>
        private static string GenerateRandomToken()
        {
            byte[] tokenBytes = new byte[32]; // 256 bit
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

        /// <summary>
        /// Secure string comparison to prevent timing attacks
        /// </summary>
        private static bool SlowEquals(string a, string b)
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
    }

    /// <summary>
    /// HTML string wrapper
    /// </summary>
    public class HtmlString : IHtmlString
    {
        private readonly string _value;

        public HtmlString(string value)
        {
            _value = value;
        }

        public string ToHtmlString()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
