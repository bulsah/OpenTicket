#!/bin/bash

# Git history'den hassas bilgileri temizleme scripti
# Bu script, eski commit'lerdeki vt.cs dosyasındaki hassas bilgileri temizler

echo "⚠️  UYARI: Bu işlem git history'yi yeniden yazacaktır!"
echo "⚠️  Eğer bu repository'yi başka bir yere push ettiyseniz, orada da force push yapmanız gerekecek!"
echo ""
read -p "Devam etmek istiyor musunuz? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "İşlem iptal edildi."
    exit 1
fi

echo "🔄 Git history temizleniyor..."

# İlk commit'i amend et (daha güvenli yöntem)
# Alternatif: git rebase -i --root kullanabilirsiniz

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

# Güvenlik commit'ini cherry-pick et (eğer gerekirse)
# git cherry-pick 2fbab0b

# Eski master'ı sil
git branch -D master

# Yeni branch'i master yap
git branch -m master

echo "✅ Git history temizlendi!"
echo ""
echo "📝 Sonraki adımlar:"
echo "1. Değişiklikleri kontrol edin: git log --oneline"
echo "2. Eğer remote'a push ettiyseniz: git push -f origin master"
echo "   ⚠️  UYARI: Force push başkaları da kullanıyorsa sorun yaratabilir!"
echo ""
echo "3. Ekip üyelerinize bildirin ki onlar da repository'yi yeniden klonlasınlar"

