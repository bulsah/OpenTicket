# TicketX - Güvenlik Dokümantasyonu

## 🔒 Güvenlik İyileştirmeleri

Bu proje kapsamlı güvenlik güncellemeleri yapılmıştır. Lütfen aşağıdaki önemli değişiklikleri ve yapılandırma adımlarını dikkatlice okuyun.

---

## ✅ Yapılan Güvenlik İyileştirmeleri

### 1. **Hassas Bilgilerin Kaldırılması**
- ✅ Hardcoded SQL Server kullanıcı adı ve parolalar kaldırıldı
- ✅ Hardcoded SMTP kullanıcı adı ve parolalar kaldırıldı
- ✅ Sunucu adları ve hassas bilgiler konfigürasyon dosyalarına taşındı

### 2. **Parola Güvenliği**
- ✅ **SHA1 kullanımı durduruldu** (artık güvenli değil)
- ✅ **PBKDF2 (RFC 2898)** ile modern parola hashleme eklendi
- ✅ Salt kullanımı (her parola için benzersiz salt)
- ✅ 100,000 iterasyon (OWASP önerisi)
- ✅ Timing attack koruması

**Yeni kullanım:**
```csharp
// Parola hashleme
string hashedPassword = PasswordHasher.HashPassword("kullanici123");

// Parola doğrulama
bool isValid = PasswordHasher.VerifyPassword("kullanici123", hashedPassword);
```

### 3. **SQL Injection Koruması**
- ✅ Tüm SQL sorguları parametreli yapıya çevrildi
- ✅ String concatenation ile sorgu oluşturma kaldırıldı
- ✅ Stored procedure çağrıları güvenli hale getirildi

**Eski (Güvensiz) Kullanım:**
```csharp
// ❌ KULLANMAYIN!
vt.InsertUpdateDelete("INSERT INTO users (name) VALUES ('" + userName + "')");
```

**Yeni (Güvenli) Kullanım:**
```csharp
// ✅ KULLANIN!
var cmd = new SqlCommand("INSERT INTO users (name) VALUES (@name)");
cmd.Parameters.AddWithValue("@name", userName);
vt.InsertUpdateDelete(cmd);
```

### 4. **XSS (Cross-Site Scripting) Koruması**
- ✅ Output encoding için `HttpUtility.HtmlEncode` kullanımı
- ✅ DropDownList güvenli veri yükleme
- ✅ Content Security Policy (CSP) header'ları eklendi

**Kullanım:**
```csharp
// Output'larda her zaman encode edin
Response.Write(HttpUtility.HtmlEncode(userInput));
```

### 5. **CSRF (Cross-Site Request Forgery) Koruması**
- ✅ ViewState MAC kontrolü aktif
- ✅ SameSite cookie attribute'ları eklendi
- ✅ Anti-forgery token altyapısı hazır

### 6. **Logging Güvenliği**
- ✅ **PII (Personally Identifiable Information) Masking** eklendi
- ✅ Email maskeleme: `user@example.com` → `u***r@example.com`
- ✅ Telefon maskeleme: `5551234567` → `555***4567`
- ✅ **Parolalar asla loglanmıyor**
- ✅ Başarısız giriş denemeleri güvenli şekilde loglanıyor

### 7. **HTTP Security Headers**
Web.config'de aşağıdaki güvenlik header'ları eklendi:
- ✅ `X-XSS-Protection`
- ✅ `X-Frame-Options` (Clickjacking koruması)
- ✅ `X-Content-Type-Options` (MIME sniffing koruması)
- ✅ `Strict-Transport-Security` (HTTPS zorunlu)
- ✅ `Content-Security-Policy`
- ✅ `Referrer-Policy`

---

## 🔧 Konfigürasyon Adımları

### 1. Web.config Ayarları

#### Development (Geliştirme) Ortamı

1. `Web.config.example` dosyasını `Web.config` olarak kopyalayın
2. Connection string'i güncelleyin:
```xml
<connectionStrings>
  <add name="TicketXDb" 
       connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=ticket;Integrated Security=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

3. SMTP ayarlarını güncelleyin:
```xml
<appSettings>
  <add key="SmtpHost" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUsername" value="your-email@gmail.com" />
  <add key="SmtpPassword" value="YOUR_APP_PASSWORD" />
  <add key="SmtpSenderEmail" value="your-email@gmail.com" />
  <add key="SmtpSenderName" value="TicketX" />
  <add key="SmtpEnableSsl" value="true" />
</appSettings>
```

#### Production (Canlı) Ortamı

**Ortam değişkenlerini kullanın:**

Windows Server:
```powershell
# SQL Connection String
setx TICKETX_CONNECTION_STRING "Data Source=PROD_SERVER;Initial Catalog=ticket;User ID=prod_user;Password=SECURE_PASSWORD" /M

# SMTP Ayarları
setx TICKETX_SMTPHOST "smtp.gmail.com" /M
setx TICKETX_SMTPPORT "587" /M
setx TICKETX_SMTPUSERNAME "production@yourcompany.com" /M
setx TICKETX_SMTPPASSWORD "SECURE_APP_PASSWORD" /M
setx TICKETX_SMTPSENDEREMAIL "production@yourcompany.com" /M
setx TICKETX_SMTPSENDERNAME "TicketX Production" /M
setx TICKETX_SMTPENABLESSL "true" /M
```

Linux/Azure:
```bash
export TICKETX_CONNECTION_STRING="Data Source=PROD_SERVER;Initial Catalog=ticket;User ID=prod_user;Password=SECURE_PASSWORD"
export TICKETX_SMTPHOST="smtp.gmail.com"
# ... diğer ayarlar
```

### 2. Gmail SMTP için App Password Oluşturma

Gmail kullanıyorsanız:

1. Google Hesabınıza gidin → Güvenlik
2. 2-Factor Authentication'ı aktif edin
3. "App Passwords" sekmesine gidin
4. "Mail" ve "Windows Computer" seçin
5. Oluşan 16 karakterlik parolayı `SmtpPassword` olarak kullanın

### 3. Veritabanı Migration (Eski Parolaları Güncelleme)

Eski SHA1 hashli parolaları PBKDF2'ye migrate etmek için:

```csharp
// Kullanıcı ilk kez giriş yaptığında
if (PasswordHasher.VerifyLegacySHA1(enteredPassword, user.PasswordHash))
{
    // Eski SHA1 hash geçerli, yeni PBKDF2 hash'e güncelle
    string newHash = PasswordHasher.HashPassword(enteredPassword);
    
    var cmd = new SqlCommand("UPDATE users SET password_hash = @hash WHERE id = @id");
    cmd.Parameters.AddWithValue("@hash", newHash);
    cmd.Parameters.AddWithValue("@id", user.Id);
    vt.InsertUpdateDelete(cmd);
}
```

---

## 📋 Güvenlik Kontrol Listesi

### Deployment Öncesi Kontroller

- [ ] Web.config'deki tüm hassas bilgiler ortam değişkenlerine taşındı mı?
- [ ] Production'da `<compilation debug="false">` yapıldı mı?
- [ ] Production'da `<customErrors mode="On">` yapıldı mı?
- [ ] HTTPS zorunlu mu? (Strict-Transport-Security)
- [ ] Tüm parolalar PBKDF2 ile hashleniyor mu?
- [ ] SQL sorguları parametreli mi?
- [ ] Output'lar HtmlEncode ediliyor mu?
- [ ] Log'larda PII maskeleme aktif mi?
- [ ] Session timeout uygun mu? (varsayılan: 20 dk)

### Code Review Kontrolleri

- [ ] `vt.Crypt()` (SHA1) kullanımı var mı? → `PasswordHasher` kullanın
- [ ] String concatenation ile SQL sorgusu var mı? → Parametreli sorgu kullanın
- [ ] `Response.Write(userInput)` gibi encode edilmemiş output var mı?
- [ ] Hardcoded parola/connection string var mı?

---

## 🚨 Kritik Uyarılar

### ⚠️ Git Repository Güvenliği

**ASLA GIT'E COMMIT ETMEYİN:**
- ❌ Gerçek connection string'ler
- ❌ Gerçek SMTP parolaları
- ❌ API anahtarları
- ❌ Secret key'ler

**Git history'den hassas bilgileri temizleme:**
```bash
# Eğer yanlışlıkla commit ettiyseniz
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch TicketX/Web.config" \
  --prune-empty --tag-name-filter cat -- --all

# Veya BFG Repo-Cleaner kullanın (daha hızlı)
bfg --delete-files Web.config
git reflog expire --expire=now --all
git gc --prune=now --aggressive
```

### ⚠️ Production Deployment

1. **HTTPS zorunlu yapın** - HTTP trafiğini kabul etmeyin
2. **Firewall kuralları** - Sadece gerekli portları açın
3. **SQL Server** - Windows Authentication tercih edin (SQL Authentication yerine)
4. **Regular backups** - Veritabanı ve konfigürasyon yedekleri
5. **Monitoring** - Başarısız login denemelerini izleyin
6. **Rate limiting** - Brute force saldırılarına karşı koruma

---

## 📚 Kaynaklar

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [OWASP SQL Injection Prevention](https://cheatsheetseries.owasp.org/cheatsheets/SQL_Injection_Prevention_Cheat_Sheet.html)
- [ASP.NET Security Best Practices](https://docs.microsoft.com/en-us/aspnet/web-forms/overview/security/)

---

## 📧 Destek

Güvenlik ile ilgili soru veya endişeleriniz için:
- Security issues: GitHub Security Advisory kullanın
- Genel sorular: Issue açın

**Güvenlik açığı bildirimi:** Lütfen güvenlik açıklarını public olarak paylaşmayın. Önce proje sahiplerine özel olarak bildirin.

---

## 📝 Changelog

### v2.0 - Güvenlik Güncellemesi (2024)
- SHA1 → PBKDF2 migration
- SQL Injection koruması
- XSS koruması
- CSRF koruması
- PII masking
- Security headers
- Configuration management (ortam değişkenleri)
- Hardcoded credentials kaldırıldı

### v1.0 - İlk versiyon
- Temel TicketX fonksiyonalitesi

