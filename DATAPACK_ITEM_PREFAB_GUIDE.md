# DataPack Item Prefab Tasarım Rehberi

## Prefab Yapısı

Aşağıdaki yapıda bir prefab oluşturun:

### 1. Ana GameObject (DataPackItem)

**Bileşenler:**
- `RectTransform` (Width: 800-1000, Height: 100-120)
- `Image` (Background - koyu renk, örn: #1A1A26)
- `Button` (komponent - tüm alanı kapsar)
- `DataPackItemUI` (Script - yeni oluşturduğumuz script)

### 2. Alt GameObject'ler

#### A) PackLogo (Opsiyonel)
- `RectTransform` (Left anchor, Width: 80, Height: 80, X: 10)
- `Image` (Sprite için)
- Referans: `DataPackItemUI.packLogoImage`

#### B) PackInfo (Bilgiler için container)
- `RectTransform` (Left anchor, Left: 100, Right: 10)
- `VerticalLayoutGroup` (Spacing: 5, Padding: 10)

**PackInfo altında:**
- **PackNameText** (TextMeshProUGUI)
  - Font Size: 20-24
  - Font Style: Bold
  - Color: White
  - Referans: `DataPackItemUI.packNameText`

- **PackMeta** (Horizontal container)
  - `RectTransform`
  - `HorizontalLayoutGroup` (Spacing: 15)
  
  **PackMeta altında:**
  - **PackVersionText** (TextMeshProUGUI)
    - Font Size: 14
    - Color: Gray (#CCCCCC)
    - Text: "v1.0.0" (örnek)
    - Referans: `DataPackItemUI.packVersionText`
  
  - **PackAuthorText** (TextMeshProUGUI)
    - Font Size: 14
    - Color: Gray (#CCCCCC)
    - Text: "Author: Unknown"
    - Referans: `DataPackItemUI.packAuthorText`

### 3. Button Ayarları

- **Button Component:**
  - Transition: Color Tint (veya Sprite Swap)
  - Normal Color: (Default)
  - Highlighted Color: (Biraz daha açık)
  - Pressed Color: (Biraz daha koyu)
  - Selected Color: (Vurgulanmış)

### 4. Layout Ayarları

**Content (ScrollView'ın içindeki Content) GameObject'ine:**
- `VerticalLayoutGroup` ekleyin
  - Spacing: 10
  - Padding: 10 (her taraftan)
  - Child Controls Size: Width enabled
  - Child Force Expand: Width enabled

**Content > DataPackItem prefab'larına:**
- Layout Element ekleyin (opsiyonel, eğer farklı boyutlar istiyorsanız)

## Prefab Oluşturma Adımları

### Unity Editor'da:

1. **Hierarchy'de yeni bir GameObject oluşturun:**
   - Sağ tık > UI > Panel (veya boş GameObject)
   - İsmini "DataPackItem" yapın

2. **RectTransform ayarları:**
   - Width: 900
   - Height: 110

3. **Image component ekleyin:**
   - Color: #1A1A26 (koyu gri/mavi)

4. **Button component ekleyin:**
   - Transition: Color Tint
   - Colors: Normal (default), Highlighted (biraz açık), Pressed (koyu)

5. **DataPackItemUI script'ini ekleyin:**
   - Assets/Scripts/UI/DataPackItemUI.cs

6. **PackLogo GameObject'i oluşturun (opsiyonel):**
   - Sağ tık > UI > Image
   - İsmini "PackLogo" yapın
   - RectTransform: Left anchor, X: 15, Width: 80, Height: 80
   - DataPackItemUI > Pack Logo Image'e sürükleyin

7. **PackInfo container'ı oluşturun:**
   - Sağ tık > Create Empty
   - İsmini "PackInfo" yapın
   - RectTransform: Left: 105, Right: 15 (packLogo'dan sonra)
   - VerticalLayoutGroup ekleyin (Spacing: 5, Padding: Left/Right: 10)

8. **PackNameText oluşturun:**
   - PackInfo > Sağ tık > UI > Text - TextMeshPro
   - İsmini "PackNameText" yapın
   - Font Size: 22, Bold, White
   - DataPackItemUI > Pack Name Text'e sürükleyin

9. **PackMeta container'ı oluşturun:**
   - PackInfo > Sağ tık > Create Empty
   - İsmini "PackMeta" yapın
   - HorizontalLayoutGroup ekleyin (Spacing: 15)

10. **PackVersionText ve PackAuthorText oluşturun:**
    - PackMeta > Sağ tık > UI > Text - TextMeshPro (her biri için)
    - Font Size: 14, Color: Gray (#CCCCCC)
    - DataPackItemUI'deki ilgili alanlara sürükleyin

11. **Prefab'ı kaydedin:**
    - Assets/Prefabs/UI klasörüne sürükleyin
    - İsmini "DataPackItem" yapın

12. **DataPackMenuUI'da referansı bağlayın:**
    - DataPackMenu sahnesini açın
    - Canvas > DataPackMenuUI script'ini seçin
    - "Pack Item Prefab" alanına DataPackItem prefab'ını sürükleyin

## Örnek Tasarım

```
DataPackItem (Button + Image + DataPackItemUI)
├── PackLogo (Image) - 80x80, Left: 15
└── PackInfo (VerticalLayoutGroup)
    ├── PackNameText (TextMeshPro) - Bold, 22pt, White
    └── PackMeta (HorizontalLayoutGroup)
        ├── PackVersionText (TextMeshPro) - 14pt, Gray
        └── PackAuthorText (TextMeshPro) - 14pt, Gray
```

## Test

1. Prefab'ı oluşturduktan sonra
2. DataPackMenu sahnesini açın
3. DataPackMenuUI script'inde "Pack Item Prefab" alanına prefab'ı sürükleyin
4. Play moduna geçin
5. DataPack'lar listelenmeli

## Notlar

- Eğer prefab kullanmazsanız, runtime'da basit bir versiyon otomatik oluşturulur
- Prefab kullanmak daha profesyonel ve tutarlı bir görünüm sağlar
- Prefab'ı düzenledikten sonra tüm item'lar otomatik güncellenir


