# DataPack Detay Paneli Setup Rehberi

## Detay Paneli Yapısı

DataPackMenu sahnesinde detay paneli için aşağıdaki UI elementleri gerekli:

### 1. Detail Panel GameObject

**Bileşenler:**
- `RectTransform` (Panel boyutu)
- `Image` (Background - koyu renk, örn: #1A1A26)
- Varsayılan olarak kapalı (SetActive(false))

### 2. Pack Logo

**GameObject:** PackLogoImage
- `RectTransform` (Width: 100, Height: 100, üst ortada)
- `Image` (Sprite için)
- Referans: `DataPackMenuUI.packLogoImage`

### 3. Pack Bilgileri

#### A) Pack Name Text
- `TextMeshProUGUI`
- Font Size: 28-32, Bold
- Color: White
- Referans: `DataPackMenuUI.packNameText`

#### B) Pack Version Text
- `TextMeshProUGUI`
- Font Size: 16-18
- Color: Gray (#CCCCCC)
- Format: "Versiyon: 1.0.0"
- Referans: `DataPackMenuUI.packVersionText`

#### C) Pack Author Text
- `TextMeshProUGUI`
- Font Size: 16-18
- Color: Gray (#CCCCCC)
- Format: "Yazar: Author Name"
- Referans: `DataPackMenuUI.packAuthorText`

#### D) Pack Description Text
- `TextMeshProUGUI`
- Font Size: 14-16
- Color: White/Light Gray
- Enable Word Wrapping: true
- Referans: `DataPackMenuUI.packDescriptionText`

### 4. İstatistikler Bölümü

#### A) League Count Text
- `TextMeshProUGUI`
- Font Size: 16
- Color: White
- Format: "Lig Sayısı: X"
- Referans: `DataPackMenuUI.leagueCountText`

#### B) Team Count Text
- `TextMeshProUGUI`
- Font Size: 16
- Color: White
- Format: "Takım Sayısı: X"
- Referans: `DataPackMenuUI.teamCountText`

#### C) Player Count Text
- `TextMeshProUGUI`
- Font Size: 16
- Color: White
- Format: "Oyuncu Sayısı: X"
- Referans: `DataPackMenuUI.playerCountText`

### 5. Butonlar

#### A) Apply Button (Uygula)
- `Button`
- Text: "Uygula" veya "Aktif Et"
- Referans: `DataPackMenuUI.applyButton`
- Görünürlük: Pack aktif değilse gösterilir

#### B) Remove Button (Kaldır)
- `Button`
- Text: "Kaldır" veya "Pasif Et"
- Referans: `DataPackMenuUI.removeButton`
- Görünürlük: Pack aktifse gösterilir

#### C) Download Button (İndir) - Opsiyonel
- `Button`
- Text: "İndir"
- Referans: `DataPackMenuUI.downloadButton`
- Şimdilik kapalı (indirme sistemi yok)

#### D) Close Detail Button (Kapat)
- `Button`
- Text: "Kapat" veya "X"
- Referans: `DataPackMenuUI.closeDetailButton`
- Detay panelini kapatır

## Örnek Layout Yapısı

```
DetailPanel (Image + RectTransform)
├── PackLogoImage (Image) - Üst ortada, 100x100
├── PackNameText (TextMeshProUGUI) - Logo altında, Bold, büyük
├── PackVersionText (TextMeshProUGUI) - Küçük, gri
├── PackAuthorText (TextMeshProUGUI) - Küçük, gri
├── PackDescriptionText (TextMeshProUGUI) - Orta, uzun metin, wrap enabled
├── StatisticsContainer (VerticalLayoutGroup)
│   ├── LeagueCountText (TextMeshProUGUI)
│   ├── TeamCountText (TextMeshProUGUI)
│   └── PlayerCountText (TextMeshProUGUI)
└── ButtonsContainer (HorizontalLayoutGroup)
    ├── ApplyButton (Button)
    ├── RemoveButton (Button)
    └── CloseDetailButton (Button)
```

## Unity Editor'da Oluşturma

1. **DetailPanel GameObject oluşturun:**
   - Hierarchy > Canvas > Sağ tık > UI > Panel
   - İsmini "DetailPanel" yapın
   - RectTransform: Full screen veya belirli bir alan
   - Image Color: Koyu renk (#1A1A26)
   - Varsayılan olarak kapalı (Inspector'da Active checkbox'ı işaretli değil)

2. **Pack Logo ekleyin:**
   - DetailPanel > Sağ tık > UI > Image
   - İsmini "PackLogoImage" yapın
   - RectTransform: Üst ortada, 100x100

3. **Text elementlerini ekleyin:**
   - DetailPanel > Sağ tık > UI > Text - TextMeshPro
   - Sırayla: PackNameText, PackVersionText, PackAuthorText, PackDescriptionText
   - Font boyutları ve renkler yukarıdaki gibi ayarlayın

4. **İstatistikleri ekleyin:**
   - DetailPanel > Sağ tık > Create Empty > "StatisticsContainer"
   - VerticalLayoutGroup ekleyin
   - İçine 3 TextMeshPro ekleyin: LeagueCountText, TeamCountText, PlayerCountText

5. **Butonları ekleyin:**
   - DetailPanel > Sağ tık > Create Empty > "ButtonsContainer"
   - HorizontalLayoutGroup ekleyin (Spacing: 10)
   - İçine 3 Button ekleyin: ApplyButton, RemoveButton, CloseDetailButton

6. **DataPackMenuUI script'inde referansları bağlayın:**
   - Canvas > DataPackMenuUI script'ini seçin
   - Inspector'da tüm referansları ilgili GameObject'lere sürükleyin

## Notlar

- Detay paneli varsayılan olarak kapalı olmalı
- Pack item'ına tıklandığında açılır
- CloseDetailButton ile kapanır
- ApplyButton aktif olmayan pack'ler için gösterilir
- RemoveButton sadece aktif pack için gösterilir


