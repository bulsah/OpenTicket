# Git history'den hassas bilgileri temizleme scripti (PowerShell)
# Bu script, eski commit'lerdeki vt.cs dosyasındaki hassas bilgileri temizler

Write-Host "⚠️  UYARI: Bu işlem git history'yi yeniden yazacaktır!" -ForegroundColor Yellow
Write-Host "⚠️  Eğer bu repository'yi başka bir yere push ettiyseniz, orada da force push yapmanız gerekecek!" -ForegroundColor Yellow
Write-Host ""

$confirmation = Read-Host "Devam etmek istiyor musunuz? (y/n)"
if ($confirmation -ne 'y') {
    Write-Host "İşlem iptal edildi." -ForegroundColor Red
    exit
}

Write-Host "🔄 Git history temizleniyor..." -ForegroundColor Cyan

try {
    # Geçici branch oluştur
    git checkout --orphan temp_branch
    
    # Tüm dosyaları ekle (güncel güvenli versiyonlar)
    git add -A
    
    # İlk commit'i oluştur
    git commit -m "Initial commit - TicketX projesi (güvenli versiyon)

- Tüm hassas bilgiler kaldırıldı
- Güvenli konfigürasyon yapısı eklendi
- PBKDF2 parola hashleme
- SQL Injection koruması
- XSS/CSRF koruması
- PII masking"
    
    # Eski master'ı sil
    git branch -D master
    
    # Yeni branch'i master yap
    git branch -m master
    
    Write-Host "✅ Git history temizlendi!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Sonraki adımlar:" -ForegroundColor Cyan
    Write-Host "1. Değişiklikleri kontrol edin: git log --oneline"
    Write-Host "2. Eğer remote'a push ettiyseniz: git push -f origin master"
    Write-Host "   ⚠️  UYARI: Force push başkaları da kullanıyorsa sorun yaratabilir!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "3. Ekip üyelerinize bildirin ki onlar da repository'yi yeniden klonlasınlar"
}
catch {
    Write-Host "❌ Hata oluştu: $_" -ForegroundColor Red
    Write-Host "Master branch'e geri dönebilirsiniz: git checkout master" -ForegroundColor Yellow
}

