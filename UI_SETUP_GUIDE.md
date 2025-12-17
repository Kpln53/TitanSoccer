# UI Setup Guide - Titan Soccer

Bu doküman, Unity Editor'da yapılması gereken UI kurulumlarını içerir.

## Eksik Scene'ler

1. **MatchPre** - Maç öncesi ekran
2. **PostMatch** - Maç sonu ekranı (pop-up olarak da olabilir)
3. **CharacterCreation** - Karakter oluşturma ekranı
4. **TeamOffer** - Takım teklifleri ekranı
5. **PlayerStats** - Oyuncu istatistikleri ekranı

## Scene Oluşturma Sırası

### 1. MatchPre Scene (Maç Öncesi Ekran)

**Gerekli UI Elementleri:**
- Canvas (Screen Space - Overlay)
- Panel (arka plan)
- HomeTeamNameText (TextMeshPro)
- AwayTeamNameText (TextMeshPro)
- SquadStatusText (TextMeshPro) - "İlk 11", "Yedek", "Kadroda Değil"
- SquadReasonText (TextMeshPro) - Neden kadroda/ilk11/yedek
- EnergySlider (Slider)
- EnergyText (TextMeshPro)
- MoraleSlider (Slider)
- MoraleText (TextMeshPro)
- GoToMatchButton (Button)
- BackButton (Button)

**Script Bağlantısı:**
- MatchPreScreenUI script'ini bir GameObject'e ekle
- Tüm UI referanslarını Inspector'da bağla

### 2. PostMatch Scene (Maç Sonu Ekranı)

**Gerekli UI Elementleri:**
- Canvas
- Panel
- MatchResultText (TextMeshPro)
- PlayerRatingText (TextMeshPro)
- RatingColorIndicator (Image)
- BonusEarnedText (TextMeshPro)
- ContinueButton (Button)
- InterviewButton (Button)

**Interview Panel:**
- InterviewPanel (GameObject - başlangıçta kapalı)
- InterviewQuestionText (TextMeshPro)
- GoodAnswerButton (Button)
- BadAnswerButton (Button)
- NeutralAnswerButton (Button)
- CloseInterviewButton (Button)

**Script Bağlantısı:**
- PostMatchScreenUI script'ini ekle ve referansları bağla

### 3. CharacterCreation Scene (Karakter Oluşturma)

**Gerekli UI Elementleri:**
- Canvas
- Panel
- PlayerNameInput (TMP_InputField)
- PlayerSurnameInput (TMP_InputField)
- NationalityDropdown (TMP_Dropdown)
- PositionDropdown (TMP_Dropdown)
- HairStyleDropdown (TMP_Dropdown)
- SkinColorDropdown (TMP_Dropdown)
- JerseyLengthDropdown (TMP_Dropdown)
- GlovesToggle (Toggle)
- MaskToggle (Toggle)
- ContinueButton (Button)
- BackButton (Button)
- CharacterPreview (Image) - opsiyonel

**Script Bağlantısı:**
- CharacterCreationUI script'ini ekle ve referansları bağla

### 4. TeamOffer Scene (Takım Teklifleri)

**Gerekli UI Elementleri:**
- Canvas
- Panel
- OffersContainer (VerticalLayoutGroup veya GridLayoutGroup)
- OfferCardPrefab (Prefab) - Her teklif için kart
  - TeamNameText
  - SalaryText
  - PlayingTimeText
  - DurationText
  - DetailButton

**Contract Detail Panel:**
- ContractDetailPanel (GameObject - başlangıçta kapalı)
- ContractTeamNameText
- ContractSalaryText
- ContractDurationText
- ContractPlayingTimeText
- ContractBonusText
- SignButton
- BackButton

**Signature Animation:**
- SignatureAnimation (GameObject)
- SignatureLine (Image - Fill Amount ile animasyon)

**Script Bağlantısı:**
- TeamOfferUI script'ini ekle ve referansları bağla

### 5. PlayerStats Scene (Oyuncu İstatistikleri)

**Gerekli UI Elementleri:**
- Canvas
- Panel
- PlayerNameText
- ClubNameText
- PositionText
- AgeText
- OVRText
- MarketValueText

**Sekmeler:**
- SeasonTabButton (Button)
- CareerTabButton (Button)
- AttributesTabButton (Button)
- SeasonTabContent (GameObject)
- CareerTabContent (GameObject)
- AttributesTabContent (GameObject)

**Season Tab:**
- SeasonStatsContainer (VerticalLayoutGroup)
- StatRowPrefab (Prefab)
  - LabelText
  - ValueText

**Career Tab:**
- CareerStatsContainer (VerticalLayoutGroup)
- StatRowPrefab (aynı prefab)

**Attributes Tab:**
- AttributesContainer (VerticalLayoutGroup)
- AttributeBarPrefab (Prefab)
  - LabelText
  - ValueText
  - BarSlider (Slider)

**Mini Kartlar:**
- FormTrendCard (GameObject)
- InjuryHistoryCard (GameObject)
- DisciplineCard (GameObject)

**Butonlar:**
- BackButton
- TrainingButton
- EquipmentButton

**Script Bağlantısı:**
- PlayerStatsScreenUI script'ini ekle ve referansları bağla

## Critical Event Pop-up (Tüm Scene'lerde)

**Gerekli UI Elementleri:**
- CriticalEventPopUp (GameObject - başlangıçta kapalı, DontDestroyOnLoad)
- PopupPanel (Panel)
- TitleText (TextMeshPro)
- DescriptionText (TextMeshPro)
- AcceptButton (Button)
- RejectButton (Button)
- CloseButton (Button)

**Transfer Offer UI:**
- TransferOfferPanel (GameObject)
- TransferTeamNameText
- TransferSalaryText
- TransferDurationText
- TransferPlayingTimeText

**Injury UI:**
- InjuryPanel (GameObject)
- InjuryTypeText
- InjuryDurationText

**Script Bağlantısı:**
- CriticalEventPopUpUI script'ini ekle ve referansları bağla
- Bu GameObject'i DontDestroyOnLoad yap

## Build Settings'e Ekleme

Tüm yeni scene'leri Build Settings'e ekle:
1. File > Build Settings
2. Add Open Scenes butonuna tıkla
3. Veya scene'leri sürükle-bırak

## GameStateManager Scene İsimlerini Güncelle

GameStateManager.cs'de scene isimlerini güncelle:
- matchPreScene = "MatchPre"
- postMatchScene = "PostMatch" (veya pop-up olarak kalabilir)

## Önemli Notlar

1. Tüm UI elementleri Canvas altında olmalı
2. TextMeshPro kullan (Text değil)
3. Butonlar için EventSystem gerekli (otomatik eklenir)
4. Prefab'ları Assets/Prefabs klasörüne kaydet
5. Scene'leri Assets/Scenes klasörüne kaydet

## Test Sırası

1. MainMenu'dan başla
2. SaveSlots'a git
3. Yeni oyun oluştur
4. CharacterCreation'a git
5. TeamOffer'a git
6. CareerHub'a git
7. MatchPre'ye git
8. MatchScene'e git
9. PostMatch'e git

Her adımda UI'lerin doğru çalıştığını kontrol et.


