# TeamSelection Sahnesi Kurulum Rehberi

## Adım 1: TitleText'i Güncelle
1. Hierarchy'de `TitleText` seç
2. Inspector'da Text alanını "Takım Seçimi" yap
3. Font Size: 48
4. Alignment: Center
5. RectTransform: Anchor (0.5, 1), Position (0, -50), Size (800, 80)

## Adım 2: LeagueNameText Oluştur
1. Hierarchy'de Canvas'a sağ tık > UI > Text - TextMeshPro
2. Adını "LeagueNameText" yap
3. Text: "Lig: [Seçilen Lig]"
4. Font Size: 24
5. Alignment: Center
6. RectTransform: Anchor (0.5, 1), Position (0, -150), Size (600, 40)

## Adım 3: TeamScrollView Oluştur
1. Hierarchy'de Canvas'a sağ tık > UI > Scroll View
2. Adını "TeamScrollView" yap
3. RectTransform: Anchor (0.5, 0.5), Position (0, -50), Size (700, 400)
4. ScrollView > Viewport > Content GameObject'ini seç
5. Content'e Vertical Layout Group ekle:
   - Spacing: 15
   - Padding: 10 (tüm taraflar)
   - Child Control Width: ✓
   - Child Control Height: ✗
   - Child Force Expand Width: ✓
6. Content'e Content Size Fitter ekle:
   - Vertical Fit: Preferred Size

## Adım 4: InfoText Oluştur
1. Hierarchy'de Canvas'a sağ tık > UI > Text - TextMeshPro
2. Adını "InfoText" yap
3. Text: "Lütfen bir takım seçin."
4. Font Size: 18
5. Alignment: Center
6. RectTransform: Anchor (0.5, 0), Position (0, 100), Size (600, 40)

## Adım 5: ConfirmButton Oluştur
1. Hierarchy'de Canvas'a sağ tık > UI > Button - TextMeshPro
2. Adını "ConfirmButton" yap
3. Button Text: "Onayla"
4. Font Size: 24
5. RectTransform: Anchor (1, 0), Position (-150, 50), Size (200, 50)
6. Inspector'da Button component'inde Interactable: ✗ (başlangıçta pasif)

## Adım 6: BackButton Oluştur
1. Hierarchy'de Canvas'a sağ tık > UI > Button - TextMeshPro
2. Adını "BackButton" yap
3. Button Text: "Geri"
4. Font Size: 24
5. RectTransform: Anchor (0, 0), Position (150, 50), Size (200, 50)

## Adım 7: TeamSelectionUI GameObject'i Oluştur
1. Hierarchy'de sağ tık > Create Empty
2. Adını "TeamSelectionUI" yap
3. Inspector'da Add Component > TeamSelectionUI script'ini ekle
4. Inspector'da TeamSelectionUI component'inde şu referansları ata:
   - **League Name Text**: LeagueNameText GameObject'i
   - **Team List Parent**: TeamScrollView > Viewport > Content GameObject'i
   - **Info Text**: InfoText GameObject'i
   - **Confirm Button**: ConfirmButton GameObject'i
   - **Back Button**: BackButton GameObject'i
   - **Team Item Prefab**: (boş bırakılabilir, script otomatik oluşturur)

## Adım 8: Canvas RectTransform'u Düzelt
1. Canvas seç
2. RectTransform: Scale (1, 1, 1) olmalı (şu anda 0, 0, 0)

## Adım 9: TitleText Pozisyonunu Düzelt
1. TitleText seç
2. RectTransform: Position (0, -50) olmalı (şu anda 10, 902)

## Notlar:
- Tüm UI elementleri Canvas altında olmalı
- Font Asset: LiberationSans SDF kullanılmalı (varsayılan)
- Canvas Scaler: Reference Resolution 800x600



