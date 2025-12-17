# Burst Compiler Hatası Çözümü

Bu hata Unity Burst Compiler'ın Assembly-CSharp-Editor assembly'sini bulamamasından kaynaklanıyor.

## Hızlı Çözüm (Sırayla Dene)

### 1. Library Klasörünü Temizle

1. Unity Editor'ı **KAPAT**
2. Windows Explorer'da proje klasörüne git
3. **Library** klasörünü **SİL** (tamamen)
4. Unity Editor'ı **YENİDEN AÇ**
5. Unity otomatik olarak Library'yi yeniden oluşturacak

### 2. ScriptAssemblies Klasörünü Temizle

Eğer 1. çözüm işe yaramazsa:

1. Unity Editor'ı **KAPAT**
2. `Library/ScriptAssemblies` klasörünü **SİL**
3. Unity Editor'ı **YENİDEN AÇ**

### 3. Unity Cache'i Temizle

1. Unity Editor'ı **KAPAT**
2. `Library` klasörünü **SİL**
3. `Temp` klasörünü **SİL** (varsa)
4. Unity Editor'ı **YENİDEN AÇ**

### 4. Unity Editor'ı Yeniden Başlat

Bazen basit bir restart yeterli:
1. Unity Editor'ı **KAPAT**
2. Birkaç saniye bekle
3. Unity Editor'ı **YENİDEN AÇ**

### 5. Reimport All Assets

Unity Editor'da:
1. **Assets > Reimport All**
2. Bekle (biraz uzun sürebilir)

### 6. Script Compilation Hatası Kontrolü

1. Unity Editor'da **Console** penceresini aç
2. Kırmızı hataları kontrol et
3. Varsa düzelt

## PowerShell Script ile Otomatik Temizleme

Aşağıdaki komutu PowerShell'de çalıştır (Unity kapalıyken):

```powershell
cd "C:\Users\Eymen Kaplan\Desktop\TiProje\TitanSoccer"
Remove-Item -Path "Library\ScriptAssemblies" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "Temp" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "Temizlendi! Unity'yi yeniden açın."
```

## Notlar

- Bu hata genellikle **zararsızdır** ve oyunu etkilemez
- Sadece Burst Compiler'ın çalışmasını engeller
- Eğer Burst kullanmıyorsanız, bu hatayı görmezden gelebilirsiniz
- Ancak Console'da sürekli görünüyorsa, yukarıdaki çözümleri deneyin

## En Etkili Çözüm

**Library klasörünü silmek** genellikle en etkili çözümdür. Unity otomatik olarak yeniden oluşturur.


