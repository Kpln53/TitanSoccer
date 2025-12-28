# TeamOffer Sahnesi - Tasarım Rehberi

## 🎨 Tasarım Nasıl Eklenir?

### Yöntem 1: Inspector'dan Ayarlama (Kolay)

1. **Unity Editor'da TeamOffer sahnesini aç**
2. **Hierarchy'de `TeamOfferManager` GameObject'ini bul**
3. **Inspector'da `TeamOfferUI` script'ini aç**
4. **"Tasarım Ayarları" bölümünü bul:**
   - `Main Panel Color`: Ana panel arka plan rengi
   - `Offer Card Color`: Teklif kartlarının arka plan rengi
   - `Contract Panel Color`: Sözleşme detay panelinin arka plan rengi
   - `Contract Content Color`: Sözleşme içeriğinin arka plan rengi
   - `Offer Card Spacing`: Kartlar arası boşluk (pixel)
   - `Offer Card Height`: Her kartın yüksekliği (pixel)

5. **Renkleri ve boyutları istediğin gibi ayarla**
6. **Play Mode'da test et** - Değişiklikler otomatik uygulanır

### Yöntem 2: Kodla Özelleştirme (Gelişmiş)

`TeamOfferUI.cs` dosyasında `CreateOfferCardPrefab()` metodunu bul ve özelleştir:

```csharp
// Örnek: Kart tasarımını değiştir
Image bgImage = card.AddComponent<Image>();
bgImage.color = new Color(0.3f, 0.5f, 0.8f, 1f); // Mavi ton
bgImage.sprite = yourCustomSprite; // Özel sprite ekle
```

### Yöntem 3: Prefab Oluşturma (En Esnek)

1. **Unity Editor'da Play Mode'da sahneyi çalıştır**
2. **Hierarchy'de oluşturulan `OfferCardPrefab` GameObject'ini seç**
3. **Project penceresinde sağ tık > Prefab > Save As**
4. **Prefab'ı kaydet: `Assets/Prefabs/OfferCardPrefab.prefab`**
5. **Prefab'ı düzenle:**
   - Sprite'lar ekle (takım logoları, arka planlar)
   - Animasyonlar ekle
   - Özel component'ler ekle
6. **Inspector'da `TeamOfferUI` script'ine prefab'ı ata**
7. **Artık kod otomatik oluşturma yerine prefab'ı kullanacak**

## 🗑️ Eski Kartları Temizleme

Kod otomatik olarak eski kartları temizler, ama manuel temizlemek istersen:

1. **Hierarchy'de `Canvas > MainPanel > OffersContainer` altındaki eski kartları sil**
2. **Veya `OffersScrollView > Viewport > OffersContainer` altındakileri sil**
3. **"OfferCardPrefab" (Clone olmayan) varsa onu da sil**

**Not:** Kod zaten `CleanupOldElements()` metoduyla otomatik temizliyor, ama manuel kontrol istersen yukarıdaki adımları takip et.

## 📝 Tasarım İpuçları

### Renk Paleti Önerileri

**Koyu Tema:**
- Main Panel: `(0.1, 0.1, 0.1, 1)` - Çok koyu gri
- Offer Card: `(0.2, 0.2, 0.2, 0.9)` - Koyu gri
- Contract Panel: `(0, 0, 0, 0.85)` - Yarı saydam siyah

**Açık Tema:**
- Main Panel: `(0.9, 0.9, 0.9, 1)` - Açık gri
- Offer Card: `(1, 1, 1, 1)` - Beyaz
- Contract Panel: `(0.5, 0.5, 0.5, 0.9)` - Yarı saydam gri

**Renkli Tema:**
- Main Panel: `(0.15, 0.2, 0.3, 1)` - Koyu mavi
- Offer Card: `(0.3, 0.4, 0.6, 0.9)` - Mavi ton
- Contract Panel: `(0.1, 0.1, 0.2, 0.9)` - Çok koyu mavi

### Sprite Ekleme

1. **Kart arka planı için:**
   - `CreateOfferCardPrefab()` metodunda `bgImage.sprite = yourSprite;` ekle

2. **Takım logoları için:**
   - `SetupOfferCard()` metodunda takım adına göre logo sprite'ı yükle

3. **Buton tasarımları için:**
   - `CreateButton()` metodunda `image.sprite = buttonSprite;` ekle

## 🔧 Gelişmiş Özelleştirme

### Animasyon Ekleme

```csharp
// Kart'a hover animasyonu ekle
using UnityEngine.EventSystems;

public class OfferCardAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover animasyonu
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Normal animasyon
    }
}
```

### Özel Font Kullanma

```csharp
// TextMeshProUGUI'ye özel font ata
textComponent.font = yourCustomFontAsset;
```

## ✅ Kontrol Listesi

- [ ] Inspector'da tasarım ayarlarını kontrol et
- [ ] Eski kartları temizle (kod otomatik yapıyor)
- [ ] Renkleri test et
- [ ] Kart boyutlarını test et
- [ ] ScrollView'ın düzgün çalıştığını kontrol et
- [ ] Prefab oluştur (isteğe bağlı)

## 🎯 Hızlı Başlangıç

1. Unity Editor'da TeamOffer sahnesini aç
2. Play Mode'a bas
3. Inspector'da `TeamOfferUI` script'ini bul
4. "Tasarım Ayarları" bölümünden renkleri değiştir
5. Play Mode'u durdur ve tekrar başlat
6. Değişiklikleri gör!










