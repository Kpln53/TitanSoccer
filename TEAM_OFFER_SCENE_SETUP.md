# TeamOffer Scene Setup - Adım 6 Tam Rehber

Bu rehber, QUICK_START_UI_SETUP.md dosyasındaki **Adım 6: TeamOffer Scene Setup**'ı tamamen uygular.

## Mevcut Durum
- ✅ Canvas var (doğru ayarlarda)
- ✅ MainPanel var
- ✅ TeamOfferUI script var
- ❌ OffersContainer eksik
- ❌ OfferCardPrefab düzgün yapılandırılmamış
- ❌ ContractDetailPanel eksik
- ❌ SignatureAnimation eksik
- ❌ TeamOfferManager GameObject eksik

## Adım 1: OffersContainer Oluştur

1. Unity Editor'da TeamOffer scene'ini aç
2. Hierarchy'de **MainPanel** seç
3. MainPanel'e sağ tık > **UI > Panel**
4. Yeni Panel'in adını **OffersContainer** yap
5. Inspector'da RectTransform ayarları:
   - Anchor: Center (0.5, 0.5)
   - Position: (0, 0)
   - Size: (1000, 1200)
6. **Add Component > Layout > Vertical Layout Group** ekle:
   - Spacing: **20**
   - Padding: **10** (tüm taraflar)
   - Child Control Width: ✓
   - Child Control Height: ✗
   - Child Force Expand Width: ✓
   - Child Force Expand Height: ✗
7. **Add Component > Layout > Content Size Fitter** ekle:
   - Vertical Fit: **Preferred Size**

## Adım 2: OfferCardPrefab Oluştur ve Yapılandır

1. Hierarchy'de **OfferCardPrefab** GameObject'ini bul (zaten var)
2. OfferCardPrefab'ı seç
3. Inspector'da:
   - **RectTransform** ayarları:
     - Anchor: Center (0.5, 0.5)
     - Size: (900, 200)
   - **Image** component'i varsa, rengini ayarla (opsiyonel)
4. OfferCardPrefab'ın içindeki Canvas'ı sil (gerekli değil)
5. OfferCardPrefab'a **Panel** ekle (arka plan için):
   - Hierarchy'de OfferCardPrefab'a sağ tık > **UI > Panel**
   - Ad: **CardPanel**
   - RectTransform: Stretch (Anchor: 0,0 - 1,1)
6. CardPanel içine şu elementleri ekle:

### TeamNameText
- Hierarchy: CardPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **TeamNameText**
- Text: "Takım Adı"
- Font Size: 32
- Alignment: Left, Middle
- RectTransform: Anchor (0, 0.5), Position (20, 0), Size (400, 40)

### SalaryText
- Hierarchy: CardPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **SalaryText**
- Text: "Maaş: 5000 €/ay"
- Font Size: 24
- Alignment: Left, Middle
- RectTransform: Anchor (0, 0.5), Position (20, -30), Size (400, 30)

### PlayingTimeText
- Hierarchy: CardPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **PlayingTimeText**
- Text: "Oynama Zamanı: İlk 11"
- Font Size: 24
- Alignment: Left, Middle
- RectTransform: Anchor (0, 0.5), Position (20, -60), Size (400, 30)

### DurationText
- Hierarchy: CardPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **DurationText**
- Text: "Süre: 12 ay"
- Font Size: 24
- Alignment: Left, Middle
- RectTransform: Anchor (0, 0.5), Position (20, -90), Size (400, 30)

### DetailButton
- Hierarchy: CardPanel > sağ tık > **UI > Button - TextMeshPro**
- Ad: **DetailButton**
- Button Text: "Detaylar"
- Font Size: 24
- RectTransform: Anchor (1, 0.5), Position (-20, 0), Size (150, 50)

7. OfferCardPrefab'ı **Prefab** olarak kaydet:
   - Project penceresinde **Assets/Prefabs** klasörü oluştur (yoksa)
   - Hierarchy'de OfferCardPrefab'ı sürükle-bırak ile Prefabs klasörüne taşı
   - Prefab adı: **OfferCardPrefab**

## Adım 3: ContractDetailPanel Oluştur

1. Hierarchy'de **MainPanel** seç
2. MainPanel'e sağ tık > **UI > Panel**
3. Ad: **ContractDetailPanel**
4. Inspector'da:
   - **RectTransform**: Stretch (Anchor: 0,0 - 1,1)
   - **Image** component: Color alpha = 0.9 (yarı saydam arka plan)
   - **GameObject Active**: ✗ (başlangıçta kapalı)
5. ContractDetailPanel içine şu elementleri ekle:

### ContractTeamNameText
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **ContractTeamNameText**
- Text: "Takım Adı"
- Font Size: 48
- Alignment: Center, Top
- RectTransform: Anchor (0.5, 1), Position (0, -50), Size (800, 60)

### ContractSalaryText
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **ContractSalaryText**
- Text: "Aylık Maaş: 5000 €"
- Font Size: 32
- Alignment: Center, Middle
- RectTransform: Anchor (0.5, 0.5), Position (0, 50), Size (600, 40)

### ContractDurationText
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **ContractDurationText**
- Text: "Sözleşme Süresi: 12 ay"
- Font Size: 32
- Alignment: Center, Middle
- RectTransform: Anchor (0.5, 0.5), Position (0, 0), Size (600, 40)

### ContractPlayingTimeText
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **ContractPlayingTimeText**
- Text: "Oynama Zamanı: İlk 11"
- Font Size: 32
- Alignment: Center, Middle
- RectTransform: Anchor (0.5, 0.5), Position (0, -50), Size (600, 40)

### ContractBonusText
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Text - TextMeshPro**
- Ad: **ContractBonusText**
- Text: "Gol Bonusu: 1000 €\nAsist Bonusu: 500 €\nGalibiyet Bonusu: 2000 €"
- Font Size: 28
- Alignment: Center, Middle
- RectTransform: Anchor (0.5, 0.5), Position (0, -150), Size (600, 120)

### SignButton
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Button - TextMeshPro**
- Ad: **SignButton**
- Button Text: "İmzala"
- Font Size: 36
- RectTransform: Anchor (0.5, 0), Position (0, 100), Size (300, 80)

### BackButton (ContractDetailPanel içinde)
- Hierarchy: ContractDetailPanel > sağ tık > **UI > Button - TextMeshPro**
- Ad: **BackButton**
- Button Text: "Geri"
- Font Size: 32
- RectTransform: Anchor (0.5, 0), Position (0, 200), Size (200, 60)

## Adım 4: SignatureAnimation Oluştur

1. Hierarchy'de **MainPanel**    seç
2. MainPanel'e sağ tık > **UI > Panel**
3. Ad: **SignatureAnimation**
4. Inspector'da:
   - **RectTransform**: Stretch (Anchor: 0,0 - 1,1)
   - **GameObject Active**: ✗ (başlangıçta kapalı)
5. SignatureAnimation içine:

### SignatureLine
- Hierarchy: SignatureAnimation > sağ tık > **UI > Image**
- Ad: **SignatureLine**
- Image Type: **Filled**
- Fill Method: **Horizontal**
- Fill Amount: **0** (başlangıçta)
- Color: Siyah veya koyu renk
- RectTransform: Anchor (0.5, 0.5), Position (0, 0), Size (400, 4)

## Adım 5: TeamOfferManager GameObject Oluştur ve Script Bağla

1. Hierarchy'de sağ tık > **Create Empty**
2. Ad: **TeamOfferManager**
3. Inspector'da **Add Component > TeamOfferUI** ekle
4. TeamOfferUI component'inde referansları bağla:
   - **Offers Container**: OffersContainer (Transform)
   - **Offer Card Prefab**: OfferCardPrefab (Prefab)
   - **Contract Detail Panel**: ContractDetailPanel (GameObject)
   - **Contract Team Name Text**: ContractTeamNameText (TextMeshProUGUI)
   - **Contract Salary Text**: ContractSalaryText (TextMeshProUGUI)
   - **Contract Duration Text**: ContractDurationText (TextMeshProUGUI)
   - **Contract Playing Time Text**: ContractPlayingTimeText (TextMeshProUGUI)
   - **Contract Bonus Text**: ContractBonusText (TextMeshProUGUI)
   - **Sign Button**: SignButton (Button)
   - **Back Button**: BackButton (Button) - ContractDetailPanel içindeki
   - **Signature Animation**: SignatureAnimation (GameObject)
   - **Signature Line**: SignatureLine (Image)

## Adım 6: Test

1. Scene'i kaydet (Ctrl+S)
2. Play moduna geç
3. Console'da hata var mı kontrol et
4. TeamOfferManager'ın tüm referansları bağlı mı kontrol et

## Notlar

- OfferCardPrefab'ın içindeki Canvas'ı silmeyi unutma
- ContractDetailPanel ve SignatureAnimation başlangıçta kapalı olmalı
- Tüm Text elementleri TextMeshPro kullanmalı
- RectTransform ayarları önemli - doğru anchor ve position kullan

## Sorun Giderme

- Eğer UI elementleri görünmüyorsa, Canvas'ın Render Mode'unu kontrol et
- Eğer butonlar çalışmıyorsa, EventSystem'in olduğundan emin ol
- Eğer layout düzgün çalışmıyorsa, VerticalLayoutGroup ayarlarını kontrol et










