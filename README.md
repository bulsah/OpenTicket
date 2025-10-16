# 🎫 OpenTicket - Secure Ticket Management System

[![Security](https://img.shields.io/badge/Security-Enhanced-green)](SECURITY_AUDIT_SUMMARY.md)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Framework_4.7.2-purple.svg)](https://dotnet.microsoft.com/)

OpenTicket is a secure, enterprise-grade ticket management system built with ASP.NET Web Forms, featuring modern security practices and OWASP compliance.

---

## ✨ Features

### 🔒 Security First
- **PBKDF2 Password Hashing** (100,000 iterations with salt)
- **SQL Injection Protection** (parameterized queries)
- **XSS Protection** (output encoding)
- **CSRF Protection** (anti-forgery tokens)
- **PII Masking** in logs (GDPR/privacy compliant)
- **Security Headers** (CSP, HSTS, X-Frame-Options, etc.)
- **Secure Configuration** (environment variables support)

### 🎟️ Ticket Management
- Event creation and management
- QR code generation for tickets
- User authentication and authorization
- Admin panel for management
- Email notifications

### 📊 Logging & Monitoring
- Login attempt logging
- Operation logging with PII masking
- IP address tracking
- Secure audit trails

---

## 🚀 Quick Start

### Prerequisites
- .NET Framework 4.7.2 or higher
- SQL Server (Express/Standard/Enterprise)
- IIS 7.0 or higher
- Visual Studio 2019+ (for development)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/YOUR_USERNAME/OpenTicket.git
cd OpenTicket
```

2. **Configure database connection**

Create `TicketX/Web.config` from the example:
```bash
cp TicketX/Web.config.example TicketX/Web.config
```

Update the connection string in `Web.config`:
```xml
<connectionStrings>
  <add name="OpenTicketDb" 
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=ticket;User ID=YOUR_USER;Password=YOUR_PASSWORD;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

3. **Configure SMTP settings**

Update SMTP settings in `Web.config`:
```xml
<appSettings>
  <add key="SmtpHost" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUsername" value="your-email@gmail.com" />
  <add key="SmtpPassword" value="YOUR_APP_PASSWORD" />
  <add key="SmtpSenderEmail" value="your-email@gmail.com" />
  <add key="SmtpSenderName" value="OpenTicket" />
</appSettings>
```

**Note:** For Gmail, you need to create an [App Password](https://support.google.com/accounts/answer/185833).

4. **Build and run**
```bash
# Open in Visual Studio
start OpenTicket.sln

# Or build from command line
msbuild OpenTicket.sln /p:Configuration=Release
```

---

## 🔐 Security Best Practices

### Production Deployment

**IMPORTANT:** Never commit real credentials to git!

#### Use Environment Variables

Windows:
```powershell
setx OPENTICKET_CONNECTION_STRING "Data Source=PROD_SERVER;Initial Catalog=ticket;User ID=prod_user;Password=SECURE_PASSWORD" /M
setx OPENTICKET_SMTPHOST "smtp.gmail.com" /M
setx OPENTICKET_SMTPUSERNAME "production@yourcompany.com" /M
setx OPENTICKET_SMTPPASSWORD "SECURE_APP_PASSWORD" /M
```

Linux/Azure:
```bash
export OPENTICKET_CONNECTION_STRING="Data Source=PROD_SERVER;..."
export OPENTICKET_SMTPHOST="smtp.gmail.com"
export OPENTICKET_SMTPUSERNAME="production@yourcompany.com"
export OPENTICKET_SMTPPASSWORD="SECURE_APP_PASSWORD"
```

#### Pre-Deployment Checklist

- [ ] Set `debug="false"` in Web.config
- [ ] Set `customErrors mode="On"` in Web.config
- [ ] Enable HTTPS and configure SSL certificate
- [ ] Configure environment variables for all secrets
- [ ] Review and configure security headers
- [ ] Set up database backups
- [ ] Configure monitoring and alerting
- [ ] Review firewall rules

---

## 📚 Documentation

- [Security Documentation](TicketX/README_SECURITY.md) - Comprehensive security guide
- [Security Audit Summary](SECURITY_AUDIT_SUMMARY.md) - Latest security audit results
- [API Documentation](#) - Coming soon

---

## 🛠️ Architecture

```
OpenTicket/
├── TicketX/
│   ├── ConfigHelper.cs          # Secure configuration management
│   ├── PasswordHasher.cs        # PBKDF2 password hashing
│   ├── AntiForgeryHelper.cs     # CSRF protection
│   ├── InputValidationHelper.cs # Input validation
│   ├── vt.cs                    # Database operations
│   ├── Web.config               # Configuration (DO NOT COMMIT!)
│   ├── Web.config.example       # Configuration template
│   ├── default.aspx             # Main page
│   ├── biletal.aspx             # Ticket purchase
│   └── yonetici/                # Admin panel
│       ├── login.aspx
│       ├── etkinlik.aspx        # Event management
│       └── kullanici.aspx       # User management
└── README.md
```

---

## 🔧 Development

### Code Style
- Use meaningful variable names
- Add XML documentation comments for public methods
- Follow SOLID principles
- Write secure code (no hardcoded credentials, use parameterized queries)

### Testing
- Test all security features
- Validate input/output encoding
- Test CSRF protection
- Verify SQL injection protection

---

## 🐛 Security Vulnerabilities

**Found a security issue?** Please DO NOT open a public issue.

Instead, email us at: **security@yourcompany.com**

We take security seriously and will respond promptly.

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- [OWASP](https://owasp.org/) for security guidelines
- [QRCoder](https://github.com/codebude/QRCoder) for QR code generation
- All contributors and security researchers

---

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/YOUR_USERNAME/OpenTicket/issues)
- **Discussions:** [GitHub Discussions](https://github.com/YOUR_USERNAME/OpenTicket/discussions)
- **Email:** support@yourcompany.com

---

## 🗺️ Roadmap

- [ ] Migrate to ASP.NET Core
- [ ] Add two-factor authentication
- [ ] Implement rate limiting
- [ ] Add API endpoints (REST/GraphQL)
- [ ] Mobile app support
- [ ] Multi-language support
- [ ] Advanced reporting and analytics
- [ ] Payment gateway integration

---

Made with ❤️ for secure ticket management

