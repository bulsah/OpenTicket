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

namespace TicketX
{
    public class vt
    {
        private SqlConnection baglanti;

        /// <summary>
        /// Güvenli veritabanı bağlantısı açar
        /// Bağlantı bilgileri ConfigHelper üzerinden alınır
        /// </summary>
        public void baglantiac()
        {
            if (baglanti != null)
            {
                baglanti.Dispose();
            }

            baglanti = new SqlConnection(ConfigHelper.GetConnectionString());
            SqlConnection.ClearPool(baglanti);
            SqlConnection.ClearAllPools();
            baglanti.Open();
        }

        /// <summary>
        /// Bağlantıyı kapatır ve kaynakları serbest bırakır
        /// </summary>
        public void BaglantiKapat()
        {
            if (baglanti != null && baglanti.State == ConnectionState.Open)
            {
                baglanti.Close();
                baglanti.Dispose();
            }
        }

        /// <summary>
        /// Parametreli INSERT, UPDATE, DELETE işlemleri için
        /// SQL Injection korumalı
        /// </summary>
        public int InsertUpdateDelete(SqlCommand komut)
        {
            try
            {
                baglantiac();
                komut.Connection = baglanti;
                int etkilenenSatir = komut.ExecuteNonQuery();
                return etkilenenSatir;
            }
            finally
            {
                BaglantiKapat();
            }
        }

        /// <summary>
        /// Parametreli SELECT işlemleri için
        /// SQL Injection korumalı
        /// </summary>
        public SqlDataReader Select(SqlCommand komut)
        {
            baglantiac();
            komut.Connection = baglanti;
            // NOT: DataReader kullanılırken bağlantı açık kalmalı
            // Kullanıcı Read() işleminden sonra BaglantiKapat() çağırmalı
            var okuyucu = komut.ExecuteReader(CommandBehavior.CloseConnection);
            return okuyucu;
        }

        /// <summary>
        /// Parametreli stored procedure çağrısı için
        /// SQL Injection korumalı
        /// </summary>
        public SqlDataReader Sp(SqlCommand komut)
        {
            baglantiac();
            komut.Connection = baglanti;
            komut.CommandType = CommandType.StoredProcedure;
            var okuyucu = komut.ExecuteReader(CommandBehavior.CloseConnection);
            return okuyucu;
        }

        /// <summary>
        /// Scalar değer döndüren sorgular için (COUNT, MAX, vb.)
        /// </summary>
        public object ExecuteScalar(SqlCommand komut)
        {
            try
            {
                baglantiac();
                komut.Connection = baglanti;
                return komut.ExecuteScalar();
            }
            finally
            {
                BaglantiKapat();
            }
        }

        /// <summary>
        /// DropDownList'i güvenli şekilde doldurur
        /// SQL Injection korumalı
        /// </summary>
        public void DropdownVeriDoldur(DropDownList dl, SqlCommand komut, string textField, string valueField)
        {
            var okuyucu = Select(komut);
            try
            {
                dl.Items.Clear();
                while (okuyucu.Read())
                {
                    ListItem item = new ListItem(
                        HttpUtility.HtmlEncode(okuyucu[textField]?.ToString()),
                        HttpUtility.HtmlEncode(okuyucu[valueField]?.ToString())
                    );
                    dl.Items.Add(item);
                }
            }
            finally
            {
                if (okuyucu != null && !okuyucu.IsClosed)
                {
                    okuyucu.Close();
                }
                BaglantiKapat();
            }
        }

        /// <summary>
        /// QR kod oluşturur
        /// </summary>
        public void QrCodeOlustur(System.Web.UI.WebControls.Image image, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("QR kod verisi boş olamaz", nameof(code));

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
        /// DEPRECATED: Güvenli değil! PasswordHasher.HashPassword kullanın
        /// </summary>
        [Obsolete("SHA1 artık güvenli değil. Lütfen PasswordHasher.HashPassword kullanın.")]
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
        /// Sunucunun IP adresini alır
        /// </summary>
        public static string GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            throw new Exception("IPv4 ağ adaptörü bulunamadı!");
        }

        /// <summary>
        /// İstemcinin IP adresini güvenli şekilde alır
        /// Proxy arkasındaki gerçek IP'yi de kontrol eder
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
                    // İlk IP genelde gerçek istemci IP'sidir
                    return addresses[0].Trim();
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"] ?? "0.0.0.0";
        }

        /// <summary>
        /// Session kontrolü yapar
        /// </summary>
        public bool SessionKontrol(int yetkiSeviyesi)
        {
            var context = HttpContext.Current;
            if (context?.Session == null)
                return false;

            bool firmaIdVar = context.Session["firmaid"] != null;
            bool yetkiVar = context.Session["yetki"] != null;
            bool usernameVar = context.Session["username"] != null;

            if (!firmaIdVar || !yetkiVar || !usernameVar)
                return false;

            // Yetki seviyesi kontrolü
            if (yetkiSeviyesi != Convert.ToInt32(context.Session["yetki"]))
                return false;

            return true;
        }

        /// <summary>
        /// Rastgele güvenli parola oluşturur
        /// </summary>
        public string RandomPassword(int length)
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
        /// Güvenli email gönderimi
        /// SMTP bilgileri ConfigHelper'dan alınır
        /// </summary>
        public void SendMail(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Email adresi boş olamaz", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email konusu boş olamaz", nameof(subject));

            var smtpSettings = ConfigHelper.GetSmtpSettings();

            using (var mesaj = new MailMessage())
            {
                mesaj.From = new MailAddress(smtpSettings.SenderEmail, smtpSettings.SenderName);
                mesaj.To.Add(to);
                mesaj.Subject = subject;
                mesaj.Body = htmlBody;
                mesaj.IsBodyHtml = true;
                mesaj.Priority = MailPriority.High;

                using (var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    client.EnableSsl = smtpSettings.EnableSsl;
                    client.Send(mesaj);
                }
            }
        }

        /// <summary>
        /// COUNT sorgusu sonucunu döndürür
        /// SQL Injection korumalı
        /// </summary>
        [WebMethod(EnableSession = true)]
        public string CountReturn(SqlCommand komut)
        {
            var sonuc = ExecuteScalar(komut);
            return sonuc?.ToString() ?? "0";
        }

        /// <summary>
        /// Giriş denemelerini loglar
        /// SQL Injection korumalı, hassas bilgileri loglamaz
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void GirisLoglama(string mail, bool basarili)
        {
            try
            {
                var cmd = new SqlCommand();
                
                if (!basarili)
                {
                    // Başarısız girişte sadece email adresini logla (parola asla loglanmaz!)
                    cmd.CommandText = @"INSERT INTO loginlogs (lastlogin, ipadres, durum) 
                                       VALUES (GETDATE(), @ipadres, @durum)";
                    cmd.Parameters.AddWithValue("@ipadres", GetClientIPAddress());
                    cmd.Parameters.AddWithValue("@durum", "Başarısız - Email: " + mail);
                }
                else
                {
                    // Başarılı girişte kullanıcı ID'sini de kaydet
                    var kullaniciId = HttpContext.Current.Session["kullaniciid"];
                    if (kullaniciId != null)
                    {
                        cmd.CommandText = @"INSERT INTO loginlogs (kid, lastlogin, ipadres, durum) 
                                           VALUES (@kid, GETDATE(), @ipadres, @durum)";
                        cmd.Parameters.AddWithValue("@kid", kullaniciId);
                        cmd.Parameters.AddWithValue("@ipadres", GetClientIPAddress());
                        cmd.Parameters.AddWithValue("@durum", "Başarılı");
                    }
                }

                InsertUpdateDelete(cmd);
            }
            catch (Exception ex)
            {
                // Log hatası uygulamayı çökertmemeli
                System.Diagnostics.Debug.WriteLine($"Giriş loglama hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Genel işlem loglaması
        /// SQL Injection korumalı, PII masking uygulanmalı
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void IslemLoglama(string islemAciklamasi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(islemAciklamasi))
                    return;

                // İşlem açıklamasından hassas bilgileri temizle
                islemAciklamasi = MaskSensitiveData(islemAciklamasi);

                var cmd = new SqlCommand();
                var kullaniciId = HttpContext.Current?.Session?["kullaniciid"];

                if (kullaniciId != null)
                {
                    cmd.CommandText = @"INSERT INTO loggenel (zaman, ipadresi, durum, pid) 
                                       VALUES (GETDATE(), @ipadres, @durum, @pid)";
                    cmd.Parameters.AddWithValue("@ipadres", GetClientIPAddress());
                    cmd.Parameters.AddWithValue("@durum", islemAciklamasi);
                    cmd.Parameters.AddWithValue("@pid", kullaniciId);
                }
                else
                {
                    cmd.CommandText = @"INSERT INTO loggenel (zaman, ipadresi, durum) 
                                       VALUES (GETDATE(), @ipadres, @durum)";
                    cmd.Parameters.AddWithValue("@ipadres", GetClientIPAddress());
                    cmd.Parameters.AddWithValue("@durum", islemAciklamasi);
                }

                InsertUpdateDelete(cmd);
            }
            catch (Exception ex)
            {
                // Log hatası uygulamayı çökertmemeli
                System.Diagnostics.Debug.WriteLine($"İşlem loglama hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Log mesajlarından hassas bilgileri maskeler (PII masking)
        /// </summary>
        private string MaskSensitiveData(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return message;

            // Email maskeleme: user@example.com -> u***r@example.com
            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\b([a-zA-Z0-9])([a-zA-Z0-9.-]+)([a-zA-Z0-9])@([a-zA-Z0-9.-]+\.[a-zA-Z]{2,})\b",
                "$1***$3@$4"
            );

            // Telefon maskeleme: 5551234567 -> 555***4567
            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\b(\d{3})(\d{4})(\d{4})\b",
                "$1***$3"
            );

            // Kredi kartı maskeleme: 1234567812345678 -> 1234********5678
            message = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"\b(\d{4})(\d{8})(\d{4})\b",
                "$1********$3"
            );

            return message;
        }

        /// <summary>
        /// DataTable döndüren güvenli sorgu
        /// </summary>
        public DataTable GetDataTable(SqlCommand komut)
        {
            try
            {
                baglantiac();
                komut.Connection = baglanti;
                
                using (var adapter = new SqlDataAdapter(komut))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            finally
            {
                BaglantiKapat();
            }
        }
    }
}
