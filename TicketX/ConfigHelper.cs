using System;
using System.Configuration;

namespace TicketX
{
    /// <summary>
    /// Güvenli konfigürasyon yönetimi için helper sınıfı
    /// Hassas bilgiler Web.config appSettings veya ortam değişkenlerinden alınır
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// SQL Server bağlantı string'ini döndürür
        /// </summary>
        public static string GetConnectionString()
        {
            // Önce ortam değişkeninden kontrol et (production için)
            var connString = Environment.GetEnvironmentVariable("TICKETX_CONNECTION_STRING");
            
            if (string.IsNullOrEmpty(connString))
            {
                // Ortam değişkeni yoksa Web.config'den al
                connString = ConfigurationManager.ConnectionStrings["TicketXDb"]?.ConnectionString;
            }

            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException(
                    "Veritabanı bağlantı bilgisi bulunamadı. " +
                    "Lütfen Web.config dosyasında 'TicketXDb' connection string'ini " +
                    "veya 'TICKETX_CONNECTION_STRING' ortam değişkenini tanımlayın.");
            }

            return connString;
        }

        /// <summary>
        /// SMTP ayarlarını döndürür
        /// </summary>
        public static SmtpSettings GetSmtpSettings()
        {
            return new SmtpSettings
            {
                Host = GetAppSetting("SmtpHost", "smtp.gmail.com"),
                Port = int.Parse(GetAppSetting("SmtpPort", "587")),
                Username = GetAppSetting("SmtpUsername"),
                Password = GetAppSetting("SmtpPassword"),
                SenderEmail = GetAppSetting("SmtpSenderEmail"),
                SenderName = GetAppSetting("SmtpSenderName", "TicketX"),
                EnableSsl = bool.Parse(GetAppSetting("SmtpEnableSsl", "true"))
            };
        }

        /// <summary>
        /// AppSettings değerini okur, önce ortam değişkenlerini kontrol eder
        /// </summary>
        private static string GetAppSetting(string key, string defaultValue = null)
        {
            // Önce ortam değişkeninden kontrol et
            var value = Environment.GetEnvironmentVariable($"TICKETX_{key.ToUpper()}");
            
            if (string.IsNullOrEmpty(value))
            {
                // Ortam değişkeni yoksa Web.config'den al
                value = ConfigurationManager.AppSettings[key];
            }

            if (string.IsNullOrEmpty(value) && defaultValue == null)
            {
                throw new InvalidOperationException(
                    $"Konfigürasyon ayarı bulunamadı: {key}. " +
                    $"Lütfen Web.config appSettings veya ortam değişkeni olarak tanımlayın.");
            }

            return value ?? defaultValue;
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public bool EnableSsl { get; set; }
    }
}

