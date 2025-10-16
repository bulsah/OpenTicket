using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace TicketX
{
    /// <summary>
    /// CSRF (Cross-Site Request Forgery) saldırılarına karşı koruma sağlar
    /// Form'lara anti-forgery token ekler ve doğrular
    /// </summary>
    public static class AntiForgeryHelper
    {
        private const string TokenSessionKey = "__RequestVerificationToken";
        private const string TokenFormKey = "__RequestVerificationToken";

        /// <summary>
        /// Anti-forgery token oluşturur ve session'a kaydeder
        /// Form'larda hidden field olarak kullanılmalıdır
        /// </summary>
        /// <returns>Hidden input HTML string</returns>
        public static string GenerateToken()
        {
            var context = HttpContext.Current;
            if (context?.Session == null)
                throw new InvalidOperationException("Session kullanılabilir değil");

            // Yeni token oluştur
            string token = GenerateRandomToken();
            
            // Session'a kaydet
            context.Session[TokenSessionKey] = token;

            // Hidden field olarak döndür
            return $"<input type=\"hidden\" name=\"{TokenFormKey}\" value=\"{HttpUtility.HtmlEncode(token)}\" />";
        }

        /// <summary>
        /// Token'ı HtmlString olarak döndürür (ASP.NET için)
        /// </summary>
        public static IHtmlString GetTokenHtml()
        {
            return new HtmlString(GenerateToken());
        }

        /// <summary>
        /// Form'dan gelen token'ı doğrular
        /// POST işlemlerinde mutlaka çağrılmalıdır
        /// </summary>
        /// <exception cref="InvalidOperationException">Token geçersizse</exception>
        public static void ValidateToken()
        {
            var context = HttpContext.Current;
            if (context?.Session == null)
                throw new InvalidOperationException("Session kullanılabilir değil");

            // Session'daki token
            string sessionToken = context.Session[TokenSessionKey] as string;
            
            // Form'dan gelen token
            string formToken = context.Request.Form[TokenFormKey];

            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new InvalidOperationException("Session token bulunamadı. Lütfen sayfayı yenileyin.");

            if (string.IsNullOrWhiteSpace(formToken))
                throw new InvalidOperationException("Form token bulunamadı. Potansiyel CSRF saldırısı!");

            // Token'ları karşılaştır (timing attack'a karşı güvenli)
            if (!SlowEquals(sessionToken, formToken))
                throw new InvalidOperationException("Token doğrulaması başarısız! Potansiyel CSRF saldırısı!");

            // Token'ı kullanıldıktan sonra yenile (replay attack koruması)
            context.Session.Remove(TokenSessionKey);
        }

        /// <summary>
        /// Token doğrulamasını yapar, hata fırlatmak yerine bool döner
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
        /// Rastgele kriptografik token oluşturur
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
        /// Timing attack'a karşı güvenli string karşılaştırması
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

