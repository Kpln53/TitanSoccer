# Hızlı Başlangıç - UI Setup

## Adım 1: Yeni Scene'ler Oluştur

Unity Editor'da şu scene'leri oluştur:

1. **File > New Scene > Basic (Built-in)**
   - Scene adı: `MatchPre`
   - Kaydet: `Assets/Scenes/MatchPre.unity`

2. **File > New Scene > Basic (Built-in)**
   - Scene adı: `PostMatch`
   - Kaydet: `Assets/Scenes/PostMatch.unity`

3. **File > New Scene > Basic (Built-in)**
   - Scene adı: `CharacterCreation`
   - Kaydet: `Assets/Scenes/CharacterCreation.unity`

4. **File > New Scene > Basic (Built-in)**
   - Scene adı: `TeamOffer`
   - Kaydet: `Assets/Scenes/TeamOffer.unity`

5. **File > New Scene > Basic (Built-in)**
   - Scene adı: `PlayerStats`
   - Kaydet: `Assets/Scenes/PlayerStats.unity`

## Adım 2: Her Scene'e Canvas Ekle

Her yeni scene'de:
1. Hierarchy'de sağ tık > **UI > Canvas**
2. Canvas ayarları:
   - Render Mode: **Screen Space - Overlay**
   - Canvas Scaler: **Scale With Screen Size**
   - Reference Resolution: **1080 x 1920** (Portrait için)

## Adım 3: MatchPre Scene Setup

### UI Elementleri Oluştur:

1. **Panel (Ana Container)**
   - Hierarchy: Canvas > sağ tık > UI > Panel
   - Ad: `MainPanel`
   - RectTransform: Stretch (tüm ekranı kaplasın)

2. **HomeTeamNameText**
   - Hierarchy: MainPanel > sağ tık > UI > Text - TextMeshPro
   - Ad: `HomeTeamNameText`
   - Text: "Ev Sahibi Takım"

3. **AwayTeamNameText**
   - Hierarchy: MainPanel > sağ tık > UI > Text - TextMeshPro
   - Ad: `AwayTeamNameText`
   - Text: "Deplasman Takımı"

4. **SquadStatusText**
   - Hierarchy: MainPanel > sağ tık > UI > Text - TextMeshPro
   - Ad: `SquadStatusText`
   - Text: "İlk 11"
   - Font Size: 24
   - Color: Yeşil

5. **SquadReasonText**
   - Hierarchy: MainPanel > sağ tık > UI > Text - TextMeshPro
   - Ad: `SquadReasonText`
   - Text: "Neden kadroda/ilk11/yedek açıklaması"

6. **EnergySlider**
   - Hierarchy: MainPanel > sağ tık > UI > Slider
   - Ad: `EnergySlider`
   - Min Value: 0
   - Max Value: 1

7. **EnergyText**
   - Hierarchy: EnergySlider > sağ tık > UI > Text - TextMeshPro
   - Ad: `EnergyText`
   - Text: "Enerji: 100%"

8. **MoraleSlider** (EnergySlider gibi)
9. **MoraleText** (EnergyText gibi)

10. **GoToMatchButton**
    - Hierarchy: MainPanel > sağ tık > UI > Button - TextMeshPro
    - Ad: `GoToMatchButton`
    - Text: "Maça Git"

11. **BackButton**
    - Hierarchy: MainPanel > sağ tık > UI > Button - TextMeshPro
    - Ad: `BackButton`
    - Text: "Geri"

### Script Bağlantısı:

1. Hierarchy'de boş bir GameObject oluştur: `MatchPreManager`
2. **Add Component > MatchPreScreenUI**
3. Inspector'da tüm referansları sürükle-bırak ile bağla

## Adım 4: PostMatch Scene Setup

### UI Elementleri:

1. **MainPanel** (aynı şekilde)
2. **MatchResultText** - "2 - 1"
3. **PlayerRatingText** - "Rating: 7.5"
4. **RatingColorIndicator** - Image (yeşil/sarı/kırmızı)
5. **BonusEarnedText** - "Kazanılan Bonus: 5000 €"
6. **ContinueButton** - "Devam Et"
7. **InterviewButton** - "Röportaj" (Rating >= 7.0 ise aktif)

### Interview Panel:

1. **InterviewPanel** - GameObject (başlangıçta kapalı)
   - Active: false
2. **InterviewQuestionText** - TextMeshPro
3. **GoodAnswerButton** - "İyi Cevap"
4. **BadAnswerButton** - "Kötü Cevap"
5. **NeutralAnswerButton** - "Nötr Cevap"
6. **CloseInterviewButton** - "Kapat"

### Script Bağlantısı:

1. `PostMatchManager` GameObject oluştur
2. **Add Component > PostMatchScreenUI**
3. Referansları bağla

## Adım 5: CharacterCreation Scene Setup

### UI Elementleri:

1. **MainPanel**
2. **PlayerNameInput** - TMP_InputField
3. **PlayerSurnameInput** - TMP_InputField
4. **NationalityDropdown** - TMP_Dropdown
5. **PositionDropdown** - TMP_Dropdown
6. **HairStyleDropdown** - TMP_Dropdown
7. **SkinColorDropdown** - TMP_Dropdown
8. **JerseyLengthDropdown** - TMP_Dropdown
9. **GlovesToggle** - Toggle
10. **MaskToggle** - Toggle
11. **ContinueButton**
12. **BackButton**

### Script Bağlantısı:

1. `CharacterCreationManager` GameObject
2. **Add Component > CharacterCreationUI**
3. Referansları bağla

## Adım 6: TeamOffer Scene Setup

### UI Elementleri:

1. **MainPanel**
2. **OffersContainer** - VerticalLayoutGroup
   - Spacing: 20
   - Padding: 10
3. **OfferCardPrefab** (Prefab oluştur):
   - Panel
   - TeamNameText
   - SalaryText
   - PlayingTimeText
   - DurationText
   - DetailButton
4. **ContractDetailPanel** (başlangıçta kapalı)
   - ContractTeamNameText
   - ContractSalaryText
   - ContractDurationText
   - ContractPlayingTimeText
   - ContractBonusText
   - SignButton
   - BackButton
5. **SignatureAnimation** (başlangıçta kapalı)
   - SignatureLine (Image - Fill Amount)

### Script Bağlantısı:

1. `TeamOfferManager` GameObject
2. **Add Component > TeamOfferUI**
3. Referansları bağla

## Adım 7: PlayerStats Scene Setup

### UI Elementleri:

1. **MainPanel**
2. **PlayerNameText**
3. **ClubNameText**
4. **PositionText**
5. **AgeText**
6. **OVRText** (büyük font)
7. **MarketValueText**

### Sekmeler:

1. **TabContainer** - HorizontalLayoutGroup
   - SeasonTabButton
   - CareerTabButton
   - AttributesTabButton

2. **SeasonTabContent** (GameObject)
   - SeasonStatsContainer (VerticalLayoutGroup)
   - StatRowPrefab (Prefab)

3. **CareerTabContent** (GameObject)
   - CareerStatsContainer (VerticalLayoutGroup)

4. **AttributesTabContent** (GameObject)
   - AttributesContainer (VerticalLayoutGroup)
   - AttributeBarPrefab (Prefab)

### Mini Kartlar:

1. **FormTrendCard**
2. **InjuryHistoryCard**
3. **DisciplineCard**

### Butonlar:

1. **BackButton**
2. **TrainingButton**
3. **EquipmentButton**

### Script Bağlantısı:

1. `PlayerStatsManager` GameObject
2. **Add Component > PlayerStatsScreenUI**
3. Referansları bağla

## Adım 8: Build Settings'e Ekle

1. **File > Build Settings**
2. Her yeni scene'i sürükle-bırak ile ekle
3. Sıralama:
   - Boot
   - MainMenu
   - SaveSlots
   - NewGameFlow
   - CharacterCreation
   - TeamSelection
   - TeamOffer
   - CareerHub
   - MatchPre
   - MatchScene
   - PostMatch
   - PlayerStats

## Adım 9: Test

1. MainMenu'dan başla
2. Her scene'i test et
3. Console'da hata var mı kontrol et
4. UI referansları eksikse Inspector'da bağla

## Hızlı İpucu

Tüm scene'lerde ortak yapı:
- Canvas (Screen Space - Overlay)
- EventSystem (otomatik eklenir)
- MainPanel (arka plan)
- Script Manager GameObject

Her scene için aynı pattern'i kullan!

