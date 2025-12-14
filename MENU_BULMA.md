# Unity Editor'da Menüyü Bulma

## Adım Adım:

1. **Unity Editor'ı aç**
2. **Üst menü çubuğuna bak** (File, Edit, Assets, GameObject, Component, Window, Help'in yanında)
3. **"TitanSoccer" menüsünü ara**
4. **Tıklayınca açılan menüde:**
   - ✅ Create Super Lig Data
   - ✅ Data Pack Editor
   - ✅ Fix Position Config
   - ✅ Create Default Position Config
   - ✅ Create 2D Sprites

## Eğer Menü Görünmüyorsa:

### Çözüm 1: Unity'yi Yeniden Başlat
- Unity Editor'ı kapat
- Tekrar aç
- Editor scriptleri bazen yeniden başlatma gerektirir

### Çözüm 2: Console'u Kontrol Et
- `Window > General > Console` (veya `Ctrl+Shift+C`)
- Kırmızı hatalar var mı kontrol et
- Hataları düzelt ve Unity'nin derlemesini bekle

### Çözüm 3: Script Dosyasını Kontrol Et
- Project penceresinde: `Assets/Scripts/Editor/SuperLigDataCreator.cs`
- Dosya var mı kontrol et
- Yoksa veya hatalıysa menü görünmez

### Çözüm 4: Assets Klasörünü Yenile
- Project penceresinde sağ tık
- "Reimport All" veya "Refresh" seç
- Unity'nin derlemesini bekle

## Alternatif Yol:

Eğer menü görünmüyorsa, **Data Pack Editor**'ı kullan:
1. `TitanSoccer > Data Pack Editor` menüsünü aç
2. Bu pencerede de takımlar ve oyuncular ekleyebilirsin

## Hızlı Test:

1. Unity Editor'da `Ctrl+Shift+C` ile Console'u aç
2. Hata var mı kontrol et
3. Hata yoksa, Unity'nin derlemesini bekle (sağ altta progress bar)
4. Derleme bitince menü görünmeli

