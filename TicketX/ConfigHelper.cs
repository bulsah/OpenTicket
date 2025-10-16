using System;
using System.Configuration;

namespace OpenTicket
{
    /// <summary>
    /// Helper class for secure configuration management
    /// Sensitive information is retrieved from Web.config appSettings or environment variables
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Returns SQL Server connection string
        /// </summary>
        public static string GetConnectionString()
        {
            // First check environment variable (for production)
            var connString = Environment.GetEnvironmentVariable("OPENTICKET_CONNECTION_STRING");
            
            if (string.IsNullOrEmpty(connString))
            {
                // If no environment variable, get from Web.config
                connString = ConfigurationManager.ConnectionStrings["OpenTicketDb"]?.ConnectionString;
            }

            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException(
                    "Database connection information not found. " +
                    "Please define 'OpenTicketDb' connection string in Web.config " +
                    "or 'OPENTICKET_CONNECTION_STRING' environment variable.");
            }

            return connString;
        }

        /// <summary>
        /// Returns SMTP settings
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
                SenderName = GetAppSetting("SmtpSenderName", "OpenTicket"),
                EnableSsl = bool.Parse(GetAppSetting("SmtpEnableSsl", "true"))
            };
        }

        /// <summary>
        /// Reads AppSettings value, checks environment variables first
        /// </summary>
        private static string GetAppSetting(string key, string defaultValue = null)
        {
            // First check environment variable
            var value = Environment.GetEnvironmentVariable($"OPENTICKET_{key.ToUpper()}");
            
            if (string.IsNullOrEmpty(value))
            {
                // If no environment variable, get from Web.config
                value = ConfigurationManager.AppSettings[key];
            }

            if (string.IsNullOrEmpty(value) && defaultValue == null)
            {
                throw new InvalidOperationException(
                    $"Configuration setting not found: {key}. " +
                    $"Please define in Web.config appSettings or as environment variable.");
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

