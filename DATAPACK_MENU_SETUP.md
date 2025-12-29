# DataPack Menu Setup Rehberi

## Yapılan Değişiklikler

1. ✅ `DataPackManager.cs` - Klasör adı "Datapacks" olarak güncellendi

## Yapılması Gerekenler

### 1. DataPackManager GameObject'ini Boot Sahnesine Ekleyin

**Boot sahnesinde:**
1. Hierarchy'de boş bir GameObject oluşturun (Right Click > Create Empty)
2. İsmini "DataPackManager" yapın
3. Inspector'da "Add Component" butonuna tıklayın
4. "DataPackManager" script'ini ekleyin

Bu sayede DataPackManager, oyun başladığında otomatik olarak yüklenecek ve `DontDestroyOnLoad` ile tüm sahnelerde erişilebilir olacak.

### 2. DataPackMenu Sahnesinde Kontroller

**DataPackMenu sahnesinde:**
1. Canvas > DataPackMenuUI script'ini kontrol edin
2. Inspector'da şu referansların atanmış olduğundan emin olun:
   - **Pack List Parent**: Content (RectTransform) - Canvas > DataPackList > Viewport > Content
   - **Back Button**: BackButton (Button)
   - **Pack Item Prefab**: (İsteğe bağlı - boş bırakılabilir, runtime'da oluşturulur)
   - **Detail Panel**: (İsteğe bağlı - detay paneli varsa)
   - **Pack Name Text, Version Text, Author Text, Description Text**: (İsteğe bağlı)

### 3. Datapack Dosyalarının Kontrolü

**Resources/Datapacks klasöründe olmalı:**
- TurkeySuperLig2025_2026.asset
- DemoDataPack.asset
- (ve diğer datapack'lar)

## Test Etme

1. Unity Editor'da Boot sahnesini açın
2. DataPackManager GameObject'ini ekleyin
3. Play butonuna basın
4. Console'da şu mesajları görmelisiniz:
   - `[DataPackManager] DataPackManager initialized.`
   - `[DataPackManager] Found X DataPack(s) in Resources/Datapacks.`
   - `[DataPackManager] Loaded: TurkeySuperLig2025_2026`
   - `[DataPackManager] Loaded: DemoDataPack`

5. DataPackMenu sahnesine geçin
6. Canvas > Content altında datapack item'larını görmelisiniz

## Sorun Giderme

### DataPack'lar görünmüyor
- Console'da hata var mı kontrol edin
- Resources/Datapacks klasöründe .asset dosyaları var mı kontrol edin
- DataPackManager Boot sahnesinde var mı kontrol edin
- DataPackMenuUI script'inde "Pack List Parent" referansı atanmış mı kontrol edin

### DataPackManager.Instance null
- Boot sahnesinde DataPackManager GameObject'i var mı kontrol edin
- DataPackManager script'i GameObject'e eklenmiş mi kontrol edin
- Boot sahnesi oyunda ilk yüklenen sahne mi kontrol edin


