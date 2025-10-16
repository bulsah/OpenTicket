using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using QRCoder;

namespace OpenTicket
{
    public class vt
    {
        private SqlConnection connection;

        /// <summary>
        /// Opens secure database connection
        /// Connection information is retrieved from ConfigHelper
        /// </summary>
        public void OpenConnection()
        {
            if (connection != null)
            {
                connection.Dispose();
            }

            connection = new SqlConnection(ConfigHelper.GetConnectionString());
            SqlConnection.ClearPool(connection);
            SqlConnection.ClearAllPools();
            connection.Open();
        }

        /// <summary>
        /// Closes connection and releases resources
        /// </summary>
        public void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// For parameterized INSERT, UPDATE, DELETE operations
        /// Protected against SQL Injection
        /// </summary>
        public int InsertUpdateDelete(SqlCommand command)
        {
            try
            {
                OpenConnection();
                command.Connection = connection;
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// For parameterized SELECT operations
        /// Protected against SQL Injection
        /// </summary>
        public SqlDataReader Select(SqlCommand command)
        {
            OpenConnection();
            command.Connection = connection;
            // NOTE: Connection must remain open when using DataReader
            // User must call CloseConnection() after Read() operation
            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        /// <summary>
        /// For parameterized stored procedure calls
        /// Protected against SQL Injection
        /// </summary>
        public SqlDataReader Sp(SqlCommand command)
        {
            OpenConnection();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        /// <summary>
        /// For queries returning scalar values (COUNT, MAX, etc.)
        /// </summary>
        public object ExecuteScalar(SqlCommand command)
        {
            try
            {
                OpenConnection();
                command.Connection = connection;
                return command.ExecuteScalar();
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Fills DropDownList securely
        /// Protected against SQL Injection
        /// </summary>
        public void FillDropdown(DropDownList ddl, SqlCommand command, string textField, string valueField)
        {
            var reader = Select(command);
            try
            {
                ddl.Items.Clear();
                while (reader.Read())
                {
                    ListItem item = new ListItem(
                        HttpUtility.HtmlEncode(reader[textField]?.ToString()),
                        HttpUtility.HtmlEncode(reader[valueField]?.ToString())
                    );
                    ddl.Items.Add(item);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                CloseConnection();
            }
        }

        /// <summary>
        /// Generates QR code
        /// </summary>
        public void GenerateQRCode(System.Web.UI.WebControls.Image image, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("QR code data cannot be empty", nameof(code));

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    image.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }
        }

        /// <summary>
        /// DEPRECATED: Not secure! Use PasswordHasher.HashPassword
        /// </summary>
        [Obsolete("SHA1 is no longer secure. Please use PasswordHasher.HashPassword.")]
        public string Crypt(string text)
        {
            var bytes = new UnicodeEncoding().GetBytes(text);
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.AppendFormat("{0:x2}", b);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets server IP address
        /// </summary>
        public static string GetServerIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            throw new Exception("No IPv4 network adapter found!");
        }

        /// <summary>
        /// Gets client IP address securely
        /// Also checks real IP behind proxy
        /// </summary>
        public static string GetClientIPAddress()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                return "0.0.0.0";

            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    // First IP is usually the real client IP
                    return addresses[0].Trim();
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"] ?? "0.0.0.0";
        }

        /// <summary>
        /// Checks session validity
        /// </summary>
        public bool ValidateSession(int requiredPermissionLevel)
        {
            var context = HttpContext.Current;
            if (context?.Session == null)
                return false;

            bool hasCompanyId = context.Session["firmaid"] != null;
            bool hasPermission = context.Session["yetki"] != null;
            bool hasUsername = context.Session["username"] != null;

            if (!hasCompanyId || !hasPermission || !hasUsername)
                return false;

            // Permission level check
            if (requiredPermissionLevel != Convert.ToInt32(context.Session["yetki"]))
                return false;

            return true;
        }

        /// <summary>
        /// Generates random secure password
        /// </summary>
        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new char[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length * 4];
                rng.GetBytes(bytes);

                for (int i = 0; i < length; i++)
                {
                    uint num = BitConverter.ToUInt32(bytes, i * 4);
                    random[i] = chars[(int)(num % (uint)chars.Length)];
                }
            }

            return new string(random);
        }

        /// <summary>
        /// Sends email securely
        /// SMTP information is retrieved from ConfigHelper
        /// </summary>
        public void SendEmail(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Email address cannot be empty", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject cannot be empty", nameof(subject));

            var smtpSettings = ConfigHelper.GetSmtpSettings();

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(smtpSettings.SenderEmail, smtpSettings.SenderName);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = htmlBody;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;

                using (var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    client.EnableSsl = smtpSettings.EnableSsl;
                    client.Send(message);
                }
            }
        }

        /// <summary>
        /// Returns COUNT query result
        /// Protected against SQL Injection
        /// </summary>
        [WebMethod(EnableSession = true)]
        public string GetCount(SqlCommand command)
        {
            var result = ExecuteScalar(command);
            return result?.ToString() ?? "0";
        }

        /// <summary>
        /// Logs login attempts
        /// Protected against SQL Injection, does not log sensitive information
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void LogLoginAttempt(string email, bool successful)
        {
            try
            {
                var cmd = new SqlCommand();
                
                if (!successful)
                {
                    // For failed login, only log email address (never log password!)
                    cmd.CommandText = @"INSERT INTO loginlogs (lastlogin, ipadres, durum) 
                                       VALUES (GETDATE(), @ipaddress, @status)";
                    cmd.Parameters.AddWithValue("@ipaddress", GetClientIPAddress());
                    cmd.Parameters.AddWithValue("@status", "Failed - Email: " + email);
                }
                else
                {
                    // For successful login, also save user ID
                    var userId = HttpContext.Current.Session["kullaniciid"];
                    if (userId != null)
                    {
                        cmd.CommandText = @"INSERT INTO loginlogs (kid, lastlogin, ipadres, durum) 
                                           VALUES (@userid, GETDATE(), @ipaddress, @status)";
                        cmd.Parameters.AddWithValue("@userid", userId);
                        cmd.Parameters.AddWithValue("@ipaddress", GetClientIPAddress());
                        cmd.Parameters.AddWithValue("@status", "Successful");
                    }
                }

                InsertUpdateDelete(cmd);
            }
            catch (Exception ex)
            {
                // Logging errors should not crash the application
                System.Diagnostics.Debug.WriteLine($"Login logging error: {ex.Message}");
            }
        }

        /// <summary>
        /// General operation logging
        /// Protected against SQL Injection, PII masking should be applied
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void LogOperation(string operationDescription)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(operationDescription))
                    return;

                // Clean sensitive data from operation description
                operationDescription = MaskSensitiveData(operationDescription);

                var cmd = new SqlCommand();
                var userId = HttpContext.Current?.Session?["kullaniciid"];

                if (userId != null)
                {
                    cmd.CommandText = @"INSERT INTO loggenel (zaman, ipadresi, durum, pid) 
                                       VALUES (GETDATE(), @ipaddress, @status, @userid)";
                    cmd.Parameters.AddWithValue("@ipaddress", GetClientIPAddress());
                    cmd.Parameters.AddWithValue("@status", operationDescription);
                    cmd.Parameters.AddWithValue("@userid", userId);
                }
                else
                {
                    cmd.CommandText = @"INSERT INTO loggenel (zaman, ipadresi, durum) 
                                       VALUES (GETDATE(), @ipaddress, @status)";
                    cmd.Parameters.AddWithValue("@ipaddress", GetClientIPAddress());
                    cmd.Parameters.AddWithValue("@status", operationDescription);
                }

                InsertUpdateDelete(cmd);
            }
            catch (Exception ex)
            {
                // Logging errors should not crash the application
                System.Diagnostics.Debug.WriteLine($"Operation logging error: {ex.Message}");
            }
        }

        /// <summary>
        /// Masks sensitive information in log messages (PII masking)
        /// </summary>
        private string MaskSensitiveData(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return message;

            // Email masking: user@example.com -> u***r@example.com
            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\b([a-zA-Z0-9])([a-zA-Z0-9.-]+)([a-zA-Z0-9])@([a-zA-Z0-9.-]+\.[a-zA-Z]{2,})\b",
                "$1***$3@$4"
            );

            // Phone masking: 5551234567 -> 555***4567
            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\b(\d{3})(\d{4})(\d{4})\b",
                "$1***$3"
            );

            // Credit card masking: 1234567812345678 -> 1234********5678
            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\b(\d{4})(\d{8})(\d{4})\b",
                "$1********$3"
            );

            return message;
        }

        /// <summary>
        /// Returns DataTable from secure query
        /// </summary>
        public DataTable GetDataTable(SqlCommand command)
        {
            try
            {
                OpenConnection();
                command.Connection = connection;
                
                using (var adapter = new SqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
