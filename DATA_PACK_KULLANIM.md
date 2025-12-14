# Data Pack Sistemi Kullanım Kılavuzu

## 1. Data Pack Oluşturma

### Adım 1: Data Pack Editor'ı Aç
- Unity Editor'da üst menüden: **TitanSoccer > Data Pack Editor**
- Veya: **TitanSoccer > Create Super Lig Data**

### Adım 2: Yeni Data Pack Oluştur
- **Data Pack Editor** penceresinde:
  - "Hedef Data Pack" alanı boşsa → "Yeni Data Pack Oluştur" butonuna tıkla
  - Dosyayı kaydet (örn: `SuperLigDataPack.asset`)

### Adım 3: Süper Lig Oluştur
- **Create Super Lig Data** penceresinde:
  - Oluşturduğun Data Pack'i seç
  - "Süper Lig Oluştur (18 Takım - 2025-2026)" butonuna tıkla
  - 18 takım otomatik oluşturulur (her takımda 25-30 oyuncu)

## 2. Data Pack'i Düzenleme

### Data Pack Editor ile Düzenleme:
1. **TitanSoccer > Data Pack Editor** menüsünü aç
2. Düzenlemek istediğin Data Pack'i seç
3. Tab'lar arasında geçiş yap:
   - **Data Pack**: Genel bilgiler
   - **Ligler**: Ligleri görüntüle/düzenle
   - **Takımlar**: Takımları görüntüle/düzenle
   - **Oyuncular**: Tüm oyuncuları görüntüle

### Takım Düzenleme:
- **Takımlar** sekmesinde:
  - Takım adını değiştir
  - Renkleri ayarla
  - Oyuncu ekle/sil
  - Oyuncu overall'larını düzenle

### Oyuncu Ekleme:
- **Takımlar** sekmesinde, bir takımın altında:
  - "Oyuncu Adı" alanına isim yaz
  - Pozisyon seç
  - Overall ayarla (0-100)
  - "Oyuncu Ekle" butonuna tıkla

## 3. Data Pack'i Kaydetme ve Kullanma

### Kaydetme:
- Data Pack'i Unity'de kaydet:
  - `Assets/Resources/DataPacks/` klasörüne koy
  - Veya istediğin bir yere kaydet

### Oyun İçinde Kullanma:
- Data Pack otomatik olarak yüklenir
- `DataPackManager` Resources klasöründen yükler
- `MatchChanceSceneManager` ilk iki takımı otomatik kullanır

## 4. Dosya Konumları

### Oluşturulan Dosyalar:
- Data Pack dosyaları: `Assets/.../DataPack.asset`
- Resources'a koy: `Assets/Resources/DataPacks/DataPack.asset`

### Script Dosyaları:
- `Assets/Scripts/Data/DataPack.cs`
- `Assets/Scripts/Data/TeamData.cs`
- `Assets/Scripts/Data/PlayerData.cs`
- `Assets/Scripts/Data/LeagueData.cs`

## 5. Hızlı Başlangıç

1. Unity Editor'ı aç
2. **TitanSoccer > Create Super Lig Data** menüsünü aç
3. "Yeni Data Pack Oluştur" → `SuperLig2025.asset` olarak kaydet
4. Oluşturulan Data Pack'i seç
5. "Süper Lig Oluştur (18 Takım - 2025-2026)" butonuna tıkla
6. Data Pack'i `Assets/Resources/DataPacks/` klasörüne taşı
7. Oyunu çalıştır - takımlar otomatik yüklenecek!

## 6. Manuel Düzenleme

### Unity Inspector'da:
- Project penceresinde Data Pack dosyasını seç
- Inspector'da tüm bilgileri görüntüle/düzenle
- Değişiklikler otomatik kaydedilir

### Kod ile:
- `DataPackManager.Instance.GetTeam("Galatasaray")` ile takım al
- `TeamData.players` listesini düzenle
- `PlayerData.overall` değerlerini değiştir

## 7. İpuçları

- Her takım için en az 11 oyuncu olmalı (ilk 11)
- Yedek oyuncular eklemek takım derinliğini artırır
- Overall değerleri 0-100 arası olmalı
- Takım gücü otomatik hesaplanır (oyuncuların ortalaması)
- Data Pack'i düzenledikten sonra Unity'de kaydet (Ctrl+S)

